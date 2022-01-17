// Partially ported from https://github.com/css2xpath/css2xpath/blob/master/index.js 
// See also https://ghostinspector.com/docs/css-xpath-conversion/#classes and http://plasmasturm.org/log/444/
using System;
using System.Collections.Generic;
using System.Text;

namespace Devlooped.Xml.Css;

class Selector : List<CombinedSelector>
{
    public Selector(BaseSelector[] start, IEnumerable<CombinedSelector> selectors)
    {
        Add(new CombinedSelector(Combinator.None, start));
        AddRange(selectors);
    }

    public string ToXPath()
    {
        var builder = new StringBuilder();
        foreach (var step in this)
            step.Append(builder);

        return builder.ToString();
    }
}

abstract record BaseSelector
{
    public abstract void Append(StringBuilder builder);
}

record CompositeSelector(BaseSelector[] Sequence) : BaseSelector
{
    public static CompositeSelector Empty { get; } = new CompositeSelector(Array.Empty<BaseSelector>());
    public static implicit operator CompositeSelector(BaseSelector[] sequence) => new CompositeSelector(sequence);

    public override void Append(StringBuilder builder)
    {
        foreach (var selector in Sequence)
            selector.Append(builder);
    }
}

record CombinedSelector(Combinator Combinator, CompositeSelector Selector) : BaseSelector
{
    public override void Append(StringBuilder builder)
    {
        builder.Append(ToAxis(Combinator));
        Selector.Append(builder);
    }

    string ToAxis(Combinator combinator) => combinator switch
    {
        Combinator.None => ".//",
        Combinator.Descendant => "//",
        Combinator.Child => "/child::*[1]/self::",
        Combinator.NextSibling => "/following-sibling::*[1]/self::",
        Combinator.SubsequentSibling => "/following-sibling::",
        _ => throw new NotSupportedException(),
    };
}

abstract record SimpleSelector : BaseSelector;

record TextSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new TextSelector();

    TextSelector() { }

    public override void Append(StringBuilder builder) => builder.Append("text()");
}

record TypeSelector(string Name, string? NamespacePrefix = default) : SimpleSelector
{
    public override void Append(StringBuilder builder) => builder.Append(Name);
}

record UniversalSelector(string? NamespacePrefix = default) : SimpleSelector
{
    public static SimpleSelector Default { get; } = new UniversalSelector();

    public override void Append(StringBuilder builder) => builder.Append("*");
}

record AttributeSelector(string Name, string? Value = default, ValueMatching? Matching = default) : SimpleSelector
{
    public override void Append(StringBuilder builder)
    {
        builder.Append('[');

        // Account for the text pseudo-attribute selector we provide.
        var name = Name;
        if (!name.Equals("text()", StringComparison.Ordinal))
            name = "@" + name;

        if (Value == null)
        {
            builder.Append(name);
        }
        else if (Matching != null)
        {
            // Remove quotes around the value since we always add them back for XPath.
            var value = Value.Trim('"', '\'');

            switch (Matching)
            {
                case ValueMatching.Equals:
                    builder.Append($"{name}=\"{value}\"");
                    break;
                case ValueMatching.Includes:
                    // [contains(concat(" ",normalize-space(@attr)," "),concat(" ","val"," "))]
                    builder.Append($"contains(concat(\" \",normalize-space({name}),\" \"),concat(\" \",\"{value}\",\" \"))");
                    break;
                case ValueMatching.Dash:
                    // [@attr="val" or starts-with(@attr,concat("val","-"))]'
                    builder.Append($"@{Name}=\"{value}\" or starts-with({name},concat(\"{value}\",\"-\"))");
                    break;
                case ValueMatching.Prefix:
                    // [starts-with(@attr,"value")]
                    builder.Append($"starts-with({name},\"{value}\")");
                    break;
                case ValueMatching.Suffix:
                    // [substring(@attr,string-length(@attr)-(string-length("val")-1))="val"]
                    builder.Append($"substring({name},string-length({name})-(string-length(\"{value}\")-1))=\"{value}\"");
                    break;
                case ValueMatching.Substring:
                    // [contains(@attr,"val")]
                    builder.Append($"contains({name},\"{value}\")");
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        else
        {
            throw new NotSupportedException($"Specified selector '{Name}' is not supported.");
        }

        builder.Append(']');
    }
}

enum ValueMatching
{
    /// <summary>
    /// =
    /// </summary>
    Equals,
    /// <summary>
    /// ~
    /// </summary>
    Includes,
    /// <summary>
    /// |
    /// </summary>
    Dash,
    /// <summary>
    /// ^
    /// </summary>
    Prefix,
    /// <summary>
    /// $
    /// </summary>
    Suffix,
    /// <summary>
    /// *
    /// </summary>
    Substring,
}

record ClassSelector(string Name) : SimpleSelector
{
    // See https://ghostinspector.com/docs/css-xpath-conversion/#classes
    // '[contains(concat(" ",normalize-space(@class)," ")," val ")]'
    public override void Append(StringBuilder builder)
        => builder.Append($"[contains(concat(\" \",normalize-space(@class),\" \"),\" {Name} \")]");
}

record IdSelector(string Id) : SimpleSelector
{
    public override void Append(StringBuilder builder)
        => builder.Append($"[@id=\"{Id}\"]");
}

record CheckedSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new CheckedSelector();
    CheckedSelector() { }
    public override void Append(StringBuilder builder)
        => builder.Append("[@selected or @checked]");
}

record FirstChildSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new FirstChildSelector();
    FirstChildSelector() { }
    public override void Append(StringBuilder builder)
        => builder.Append("[not(preceding-sibling::*)]");
}

record LastChildSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new LastChildSelector();
    LastChildSelector() { }
    public override void Append(StringBuilder builder)
        => builder.Append("[not(following-sibling::*)]");
}

record OnlyChildSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new OnlyChildSelector();
    OnlyChildSelector() { }
    public override void Append(StringBuilder builder)
        => builder.Append("[not(preceding-sibling::*) and not(following-sibling::*)]");
}

record EmptySelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new EmptySelector();
    EmptySelector() { }
    public override void Append(StringBuilder builder)
        => builder.Append("[not(*) and not(normalize-space())]");
}

record FirstOfTypeSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new FirstOfTypeSelector();
    FirstOfTypeSelector() { }
    public override void Append(StringBuilder builder) => builder.Append("[1]");
}

record LastOfTypeSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new LastOfTypeSelector();
    LastOfTypeSelector() { }
    public override void Append(StringBuilder builder) => builder.Append("[last()]");
}

record NegationSelector(BaseSelector Selector) : SimpleSelector
{
    public override void Append(StringBuilder builder)
    {
        builder.Append("[not(self::node()");
        Selector.Append(builder);
        builder.Append(")]");
    }
}

record HasSelector(BaseSelector Selector) : SimpleSelector
{
    public override void Append(StringBuilder builder)
    {
        if (Selector is CompositeSelector composite)
        {
            if (composite.Sequence.Length == 1)
            {
                builder.Append("[count(");
                Selector.Append(builder);
                builder.Append(") > 0]");
            }
            else if (composite.Sequence.Length > 1)
            {
                var first = true;
                builder.Append("[fn:sum(");
                foreach (var step in composite.Sequence)
                {
                    if (first)
                        first = false;
                    else
                        builder.Append(", ");
                    builder.Append("count(");
                    step.Append(builder);
                    builder.Append(")");
                }
                builder.Append(") > 0]");
            }
        }
        else
        {
            builder.Append("[count(");
            Selector.Append(builder);
            builder.Append(") > 0]");
        }
    }
}

record PositionSelector(string Position) : SimpleSelector
{
    public override void Append(StringBuilder builder)
        => builder.Append("[").Append(Position).Append("]");
}

enum Combinator
{
    None,
    Descendant,
    Child,
    NextSibling,
    SubsequentSibling,
}
