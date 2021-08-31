using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Devlooped.Xml.Css;

/// <summary>
/// Provides extension methods for <see cref="XNode"/> that allow selecting 
/// nodes using CSS selector expressions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class CssSelectorExtensions
{
    /// <summary>
    /// Selects an <see cref="XElement"/> using a CSS3 selector expression.
    /// </summary>
    /// <param name="node">The <see cref="XNode"/> on which to evaluate the XPath expression.</param>
    /// <param name="expression">A <see cref="string"/> that contains a CSS3 selector expression.</param>
    /// <returns>An <see cref="XElement"/>, or null.</returns>
    public static XElement CssSelectElement(this XNode node, string expression)
    {
        var selector = Parser.Parse(expression);
        return node.XPathSelectElement(selector.ToXPath());
    }

    /// <summary>
    /// Selects a collection of elements using a CSS3 selector expression.
    /// </summary>
    /// <param name="node">The <see cref="XNode"/> on which to evaluate the XPath expression.</param>
    /// <param name="expression">A <see cref="string"/> that contains a CSS3 selector expression.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="XElement"/> that
    /// contains the selected elements.
    /// </returns>
    public static IEnumerable<XElement> CssSelectElements(this XNode node, string expression)
    {
        var selector = Parser.Parse(expression);
        return node.XPathSelectElements(selector.ToXPath());
    }
}