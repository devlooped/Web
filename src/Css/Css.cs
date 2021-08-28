using System.Collections.Generic;

namespace Devlooped.Xml.Css;

class Selector : List<CombinedSelector>
{
    public Selector(SimpleSelector[] start, IEnumerable<CombinedSelector> selectors)
    {
        Add(new CombinedSelector(Combinator.None, start));
        AddRange(selectors);
    }
}

record CombinedSelector(Combinator Combinator, SimpleSelector[] SelectorSequence);
abstract record SimpleSelector;
record TypeSelector(string Name, string? NamespacePrefix = default) : SimpleSelector;
record UniversalSelector(string? NamespacePrefix = default) : SimpleSelector
{
    public static SimpleSelector Default { get; } = new UniversalSelector();
}
record AttributeSelector(string Name, string? Value = default, ValueMatching? Matching = default) : SimpleSelector;
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
record ClassSelector(string Name) : SimpleSelector;
record IdSelector(string Id) : SimpleSelector;
enum Combinator
{
    None,
    Descendant,
    Child,
    NextSibling,
    SubsequentSibling,
}
