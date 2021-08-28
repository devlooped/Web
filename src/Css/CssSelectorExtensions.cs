using System.Collections.Generic;
using System.Xml.XPath;
using Devlooped.Xml.Css;

namespace System.Xml.Linq
{
    public static class CssSelectorExtensions
    {
        public static XElement CssSelectElement(this XNode node, string expression)
        {
            var selector = Parser.Parse(expression);
            var xpath = Converter.CssToXPath(selector);
            return node.XPathSelectElement(xpath);
        }

        public static IEnumerable<XElement> CssSelectElements(this XNode node, string expression)
        {
            var selector = Parser.Parse(expression);
            var xpath = Converter.CssToXPath(selector);
            return node.XPathSelectElements(xpath);
        }
    }
}