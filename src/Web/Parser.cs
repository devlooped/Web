using System;
using System.Linq;
using Superpower;
using Superpower.Parsers;

namespace Devlooped.Web;

class Parser
{
    // NOTE: we don't account for unicode escapes since those are already 
    // supported in the C# string literals. Perhaps something to support in the future.
    internal static TextParser<char> Escape { get; } =
        Character.EqualTo('\\').Then(_ => Character.Except(c => char.IsLetterOrDigit(c), "letter or digit"));

    // NOTE: we don't perform any validation on identifiers, it's 
    // up to consumers to ensure they provide valid ones. 
    // See https://www.w3.org/TR/CSS21/syndata.html#value-def-identifier
    internal static TextParser<string> Identifier { get; } =
        from start in Character.EqualTo('-').Or(Character.EqualTo('_')).Or(Escape).Or(Character.Letter)
        from rest in Character.EqualTo('-').Or(Character.EqualTo('_')).Or(Escape).Or(Character.LetterOrDigit)
                     .Many()
        select start + new string(rest);

    internal static TextParser<string?> NamespacePrefix { get; } =
        from identifier in Identifier.Or(Character.EqualTo('*').Select(c => "*")).Cast<string, string?>().OptionalOrDefault()
        from separator in Character.EqualTo('|')
        select identifier;

    internal static TextParser<string> NodeText { get; } = Span.EqualTo("text()").Select(s => s.ToStringValue());

    internal static TextParser<SimpleSelector> UniversalSelector { get; } =
        (from ns in NamespacePrefix.Try().OptionalOrDefault()
         from star in Character.EqualTo('*')
         select (SimpleSelector)new UniversalSelector(ns))
        .Named("universal selector");

    internal static TextParser<SimpleSelector> TypeSelector { get; } =
        (from ns in NamespacePrefix.Try().OptionalOrDefault()
         from identifier in Identifier
         select (SimpleSelector)new TypeSelector(identifier, ns))
        .Named("type selector");

    internal static TextParser<SimpleSelector> ClassSelector { get; } =
        (from dot in Character.EqualTo('.')
         from identifier in Identifier
         select (SimpleSelector)new ClassSelector(identifier))
        .Named("class selector");

    internal static TextParser<SimpleSelector> IdSelector { get; } =
        (from hash in Character.EqualTo('#')
         from identifier in Identifier
         select (SimpleSelector)new IdSelector(identifier))
        .Named("ID selector");

    internal static TextParser<ValueMatching> MatchingParser { get; } =
        from modifier in Character.In('~', '|', '^', '$', '*').Optional()
        from eq in Character.EqualTo('=')
        select modifier switch
        {
            '~' => ValueMatching.Includes,
            '|' => ValueMatching.Dash,
            '^' => ValueMatching.Prefix,
            '$' => ValueMatching.Suffix,
            '*' => ValueMatching.Substring,
            _ => ValueMatching.Equals
        };

    internal static TextParser<SimpleSelector> AttributeSelector { get; } =
        (from start in Character.EqualTo('[')
         from _ in Character.WhiteSpace.IgnoreMany()
         from identifier in NodeText.Try().Or(Identifier)
         from __ in Character.WhiteSpace.IgnoreMany()
         from matching in MatchingParser.Optional()
         from value in Span.WithoutAny(c => c == ']').Named("attribute value").Optional()
         from end in Character.EqualTo(']')
         let attr = value?.ToStringValue().Trim()
         select (SimpleSelector)new AttributeSelector(identifier,
             attr?.Length == 0 ? null : attr, matching))
        .Named("attribute selector");

    internal static TextParser<SimpleSelector> PositionSelector { get; } =
        from number in Character.Numeric.AtLeastOnce()
        select (SimpleSelector)new PositionSelector(new string(number));

    internal static TextParser<BaseSelector?> PseudoArgumentSelector { get; } =
        from _ in Character.EqualTo('(')
        from selector in ClassSelector.Cast<SimpleSelector, BaseSelector>().Try()
                        .Or(IdSelector.Cast<SimpleSelector, BaseSelector>().Try())
                        .Or(AttributeSelector.Cast<SimpleSelector, BaseSelector>().Try())
                        .Or(PseudoSelector!.Cast<SimpleSelector, BaseSelector>().Try())
                        .Or(PositionSelector.Cast<SimpleSelector, BaseSelector>().Try())
                        .Or(RelativeSelector!.ManyDelimitedBy(Character.EqualTo(',')).Select(x => (BaseSelector)new CompositeSelector(x.OfType<BaseSelector>().ToArray())).Try())
        from __ in Character.EqualTo(')')
        select selector;

    internal static TextParser<SimpleSelector> PseudoSelector { get; } =
        (from start in Character.EqualTo(':')
         from identifier in Span.EqualTo("checked")
            .Or(Span.EqualTo("only-child"))
            .Or(Span.EqualTo("empty"))
            .Or(Span.EqualTo("first-child").Try())
            .Or(Span.EqualTo("last-child").Try())
            .Or(Span.EqualTo("first-of-type").Try())
            .Or(Span.EqualTo("last-of-type").Try())
            .Or(Span.EqualTo("not").Try())
            .Or(Span.EqualTo("nth-of-type").Try())
            .Or(Span.EqualTo("nth-child").Try())
            .Or(Span.EqualTo("has"))
         from argument in PseudoArgumentSelector.OptionalOrDefault()
         select identifier.ToStringValue() switch
         {
             "checked" => CheckedSelector.Default,
             "only-child" => OnlyChildSelector.Default,
             "empty" => EmptySelector.Default,
             "first-child" => FirstChildSelector.Default,
             "last-child" => LastChildSelector.Default,
             "first-of-type" => FirstOfTypeSelector.Default,
             "last-of-type" => LastOfTypeSelector.Default,
             "not" => new NegationSelector(argument),
             "nth-of-type" => (SimpleSelector)argument,
             "has" => new HasSelector(argument),
             string id when id == "nth-child" && argument is PositionSelector position => new NthChildSelector(position.Position),
             _ => throw new NotSupportedException()
         })
        .Named("pseudo selector");

    internal static TextParser<CompositeSelector> SimpleSelectorSequence { get; } =
        from start in UniversalSelector.Or(TypeSelector).Cast<SimpleSelector, SimpleSelector?>().OptionalOrDefault()
        from rest in ClassSelector
                        .Or(IdSelector)
                        .Or(AttributeSelector)
                        .Or(PseudoSelector)
                    .Many()
        select (start == null && rest.Length == 0) ?
            CompositeSelector.Empty :
            new[] { start ?? Web.UniversalSelector.Default }.Concat(rest).ToArray();

    internal static TextParser<Combinator?> CombinatorParser { get; } =
        from combinator in Character.In('>', '+', '~').Or(Character.WhiteSpace).Optional().Named("combinator")
        select combinator switch
        {
            '>' => Combinator.Child,
            '+' => Combinator.NextSibling,
            '~' => Combinator.SubsequentSibling,
            ' ' => Combinator.Descendant,
            _ => default(Combinator?),
        };

    internal static TextParser<CombinedSelector?> SelectorStep { get; } =
        from combinator in CombinatorParser
        from _ in Character.WhiteSpace.IgnoreMany()
        from sequence in SimpleSelectorSequence
        select
            (combinator == null && sequence.Sequence.Length == 0) ?
            null :
            (combinator == null && sequence.Sequence.Length > 0) ?
            // Omitted combinator is interpreted as a // according to spec.
            new CombinedSelector(Combinator.Descendant, sequence) :
            new CombinedSelector((Combinator)combinator, sequence);

    internal static TextParser<CombinedSelector?> RelativeSelector { get; } =
        from combinator in CombinatorParser
        from _ in Character.WhiteSpace.IgnoreMany()
        from sequence in SimpleSelectorSequence
        select
            (combinator == null && sequence.Sequence.Length == 0) ?
            null :
            (combinator == null && sequence.Sequence.Length > 0) ?
            // Omitted combinator is interpreted as a .// in this case
            new CombinedSelector(Combinator.None, sequence) :
            new CombinedSelector(combinator == Combinator.Descendant ? Combinator.None : (Combinator)combinator, sequence);

    internal static TextParser<Selector> Selector { get; } =
        from start in SimpleSelectorSequence
        from _ in Character.WhiteSpace.IgnoreMany()
        from steps in SelectorStep.ManyDelimitedBy(Span.WhiteSpace)
        select new Selector(
            start.Sequence.Length == 0 ? new[] { Web.UniversalSelector.Default } : start.Sequence,
            steps.Where(x => x != null));

    internal static TextParser<Selector[]> SelectorGroup { get; } = Selector.ManyDelimitedBy(Character.In(','));

    public static SelectorGroup Parse(string expression) =>
        string.IsNullOrEmpty(expression) ?
        throw new ArgumentException("Empty expression", nameof(expression)) :
        new SelectorGroup(SelectorGroup.Parse(expression));
}