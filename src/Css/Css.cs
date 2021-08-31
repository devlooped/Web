using System;
using System.Collections.Generic;
using System.Text;

namespace Devlooped.Xml.Css;

class Selector : List<CombinedSelector>
{
    public Selector(SimpleSelector[] start, IEnumerable<CombinedSelector> selectors)
    {
        Add(new CombinedSelector(Combinator.None, start));
        AddRange(selectors);
    }

    public string ToXPath()
    {
        var builder = new StringBuilder();

        foreach (var step in this)
        {
            builder.Append(ToAxis(step.Combinator));
            foreach (var selector in step.SelectorSequence)
                selector.Append(builder);
        }

        return builder.ToString();
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

record CombinedSelector(Combinator Combinator, SimpleSelector[] SelectorSequence);

abstract record SimpleSelector
{
    public abstract void Append(StringBuilder builder);
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

        if (Value == null)
        {
            builder.Append('@').Append(Name);
        }
        else if (Matching != null)
        {
            switch (Matching)
            {
                case ValueMatching.Equals:
                    builder.Append($"@{Name}=\"{Value}\"");
                    break;
                case ValueMatching.Includes:
                    // [contains(concat(" ",normalize-space(@attr)," "),concat(" ","val"," "))]
                    builder.Append($"contains(concat(\" \",normalize-space(@{Name}),\" \"),concat(\" \",\"{Value}\",\" \"))");
                    break;
                case ValueMatching.Dash:
                    // [@attr="val" or starts-with(@attr,concat("val","-"))]'
                    builder.Append($"@{Name}=\"{Value}\" or starts-with(@{Name},concat(\"{Value}\",\"-\"))");
                    break;
                case ValueMatching.Prefix:
                    // [starts-with(@attr,"value")]
                    builder.Append($"starts-with(@{Name},\"{Value}\")");
                    break;
                case ValueMatching.Suffix:
                    // [substring(@attr,string-length(@attr)-(string-length("val")-1))="val"]
                    builder.Append($"substring(@{Name},string-length(@{Name})-(string-length(\"{Value}\")-1))=\"{Value}\"");
                    break;
                case ValueMatching.Substring:
                    // [contains(@attr,"val")]
                    builder.Append($"contains(@{Name},\"{Value}\")");
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        else
        {
            throw new NotSupportedException();
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
    public override void Append(StringBuilder builder) => builder.Append($"[contains(concat(\" \",normalize-space(@class),\" \"),\" {Name} \")]");
}

record IdSelector(string Id) : SimpleSelector
{
    public override void Append(StringBuilder builder) => builder.Append($"[@id=\"{Id}\"]");
}

record CheckedSelector : SimpleSelector
{
    public static SimpleSelector Default { get; } = new CheckedSelector();
    CheckedSelector() { }
    public override void Append(StringBuilder builder) => builder.Append("[@selected or @checked]");
}


enum Combinator
{
    None,
    Descendant,
    Child,
    NextSibling,
    SubsequentSibling,
}
