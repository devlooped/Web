using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Devlooped.Web;

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
    public static XElement? CssSelectElement(this XNode? node, string expression)
    {
        if (node == null)
            return null;

        var selector = Parser.Parse(expression);
        var xpath = selector.ToXPath();
        return node.XPathSelectElement(xpath);
    }

    /// <summary>
    /// Selects a collection of elements using a CSS3 selector expression.
    /// </summary>
    /// <param name="node">The <see cref="XNode"/> on which to evaluate the XPath expression.</param>
    /// <param name="expression">A <see cref="string"/> that contains a CSS3 selector expression.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="XElement"/> that
    /// contains the selected elements.
    /// </returns>
    public static IEnumerable<XElement> CssSelectElements(this XNode? node, string expression)
    {
        if (node == null)
            return Array.Empty<XElement>();

        var selector = Parser.Parse(expression);
        var xpath = selector.ToXPath();
        return node.XPathSelectElements(xpath, new CssContext());
    }

    // The custom context allows resolving the fn:sum which properly implements the XPath 2.0 fn:sum
    // see https://www.w3.org/TR/xquery-operators/#func-sum.
    class CssContext : XsltContext
    {
        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
        {
            if (name == "sum" && ArgTypes.All(x => x == XPathResultType.Number))
                return new SumFunction(ArgTypes);

            throw new XPathException($"Unsupported function {name}");
        }

        public override bool Whitespace => true;

        public override int CompareDocument(string baseUri, string nextbaseUri) => 0;

        public override bool PreserveWhitespace(XPathNavigator node) => true;

        public override IXsltContextVariable ResolveVariable(string prefix, string name) => null!;

        record SumFunction(XPathResultType[] ArgTypes) : IXsltContextFunction
        {
            public int Maxargs => ArgTypes.Length;

            public int Minargs => ArgTypes.Length;

            public XPathResultType ReturnType => XPathResultType.Number;

            public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                => args.Cast<double>().Sum();
        }
    }
}