using System;
using System.Linq;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Devlooped.Xml.Css;

class Parser
{
    // NOTE: we don't account for unicode escapes since those are already 
    // supported in the C# string literals. Perhaps something to support in the future.
    internal static TextParser<char> Escape { get; } =
        Character.EqualTo('\\').Then(_ => Character.Except(c => char.IsLetterOrDigit(c), "letter or digit"));

    // NOTE: we don't perform any validation on identifiers, it's 
    // up to consumers to ensure they provide valid ones. 
    internal static TextParser<string> Identifier { get; } =
        from start in Character.EqualTo('-').Or(Escape).Or(Character.Letter)
        from rest in Character.EqualTo('-').Or(Escape).Or(Character.LetterOrDigit)
                     .Many()
        select start + new string(rest);

    internal static TextParser<string?> NamespacePrefix { get; } =
        from identifier in Identifier.Or(Character.EqualTo('*').Select(c => "*")).Cast<string, string?>().OptionalOrDefault()
        from separator in Character.EqualTo('|')
        select identifier;

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
         from identifier in Identifier
         from __ in Character.WhiteSpace.IgnoreMany()
         from matching in MatchingParser.Optional()
         from ___ in Character.WhiteSpace.IgnoreMany()
         from value in Character.Matching(c => c != ']' && !char.IsWhiteSpace(c), "attribute value").Many()
         from ____ in Character.WhiteSpace.IgnoreMany()
         from end in Character.EqualTo(']')
         select (SimpleSelector)new AttributeSelector(identifier,
             value.Length == 0 ? null : new string(value), matching))
        .Named("attribute selector");

    internal static TextParser<SimpleSelector> PseudoSelector { get; } =
        (from start in Character.EqualTo(':')
         from identifier in Span.EqualTo("checked")
            .Or(Span.EqualTo("only-child"))
            .Or(Span.EqualTo("empty"))
            .Or(Span.EqualTo("first-").Or(Span.EqualTo("last-"))
                .Then(x => Span.EqualTo("of-type").Or(Span.EqualTo("child"))
                .Select(s => new TextSpan(x.ToStringValue() + s.ToStringValue()))))
         select identifier.ToStringValue() switch
         {
             "checked" => CheckedSelector.Default,
             "only-child" => OnlyChildSelector.Default,
             "empty" => EmptySelector.Default,
             "first-child" => FirstChildSelector.Default,
             "last-child" => LastChildSelector.Default,
             "first-of-type" => FirstOfTypeSelector.Default,
             "last-of-type" => LastOfTypeSelector.Default,
             _ => throw new NotSupportedException()
         })
        .Named("checked pseudo selector");

    internal static TextParser<SimpleSelector[]> SimpleSelectorSequence { get; } =
        from start in UniversalSelector.Or(TypeSelector).Cast<SimpleSelector, SimpleSelector?>().OptionalOrDefault()
        from rest in ClassSelector
                        .Or(IdSelector)
                        .Or(AttributeSelector)
                        .Or(PseudoSelector)
                    .Many()
        select (start == null && rest.Length == 0) ?
            Array.Empty<SimpleSelector>() :
            new[] { start ?? Css.UniversalSelector.Default }.Concat(rest).ToArray();

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
            (combinator == null && sequence.Length == 0) ?
            null :
            (combinator == null && sequence.Length > 0) ?
            // Omitted combinator is interpreted as a // according to spec.
            new CombinedSelector(Combinator.Descendant, sequence) :
            new CombinedSelector((Combinator)combinator, sequence);

    internal static TextParser<Selector> Selector { get; } =
        from start in SimpleSelectorSequence
        from _ in Character.WhiteSpace.IgnoreMany()
        from steps in SelectorStep.ManyDelimitedBy(Span.WhiteSpace)
        select new Selector(
            start.Length == 0 ? new[] { Css.UniversalSelector.Default } : start,
            steps.Where(x => x != null));

    public static Selector Parse(string expression) =>
        string.IsNullOrEmpty(expression) ?
        throw new ArgumentException("Empty selector", nameof(expression)) :
        Selector.Parse(expression);
}