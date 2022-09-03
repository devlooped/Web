using System.ComponentModel;
using Devlooped.Web;

namespace System.Xml
{
    /// <summary>
    /// Extension methods for <see cref="XmlReader"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XmlReaderExtensions
    {
        /// <summary>
        /// Creates a wrapping reader that ignores all XML namespace declarations, 
        /// so that all resulting elements and attributes have no namespaces.
        /// </summary>
        public static XmlReader IgnoreNamespaces(this XmlReader reader)
            => new IgnoreXmlNsReader(reader);

        /// <summary>
        /// Creates a wrapping reader that skips elements (and their child nodes) with 
        /// the given local names (without namespace, if any).
        /// </summary>
        public static XmlReader SkipElements(this XmlReader reader, params string[] localNames)
            => new SkipElementsReader(reader, localNames);
    }
}

namespace System.Xml.Linq
{
    /// <summary>
    /// Extension methods for <see cref="XElement"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XElementExtensions
    {
        /// <summary>
        /// Returns a clone of the node, with XML namespaces removed.
        /// </summary>
        public static XElement RemoveNamespaces(this XElement element)
            => XElement.Load(element.CreateReader().IgnoreNamespaces());

        /// <summary>
        /// Returns a clone of the root node, with XML namespaces removed.
        /// </summary>
        public static XElement? RemoveNamespaces(this XDocument document)
            => document.Root == null ? null : XElement.Load(document.Root.CreateReader().IgnoreNamespaces());
    }
}