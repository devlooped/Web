using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.XPath;
using Devlooped.Xml.Css;

namespace System.Xml.Linq
{
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
            var xpath = Converter.CssToXPath(selector);
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
        public static IEnumerable<XElement> CssSelectElements(this XNode node, string expression)
        {
            var selector = Parser.Parse(expression);
            var xpath = Converter.CssToXPath(selector);
            return node.XPathSelectElements(xpath);
        }
    }
}