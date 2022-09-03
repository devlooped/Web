using System.Xml;

namespace Devlooped.Web;

/// <summary>
/// Removes all XML namespaces, since for HTML content it's typically 
/// irrelevant.
/// </summary>
class IgnoreXmlNsReader : XmlWrappingReader
{
    const string XmlNsNamespace = "http://www.w3.org/2000/xmlns/";

    public IgnoreXmlNsReader(XmlReader baseReader) : base(baseReader) { }

    public override int AttributeCount
    {
        get
        {
            var count = 0;
            for (var go = MoveToFirstAttribute(); go; go = MoveToNextAttribute())
                count++;

            return count;
        }
    }

    public override bool MoveToFirstAttribute()
    {
        var moved = base.MoveToFirstAttribute();
        while (moved && (IsXmlNs || IsLocalXmlNs))
            moved = MoveToNextAttribute();

        if (!moved)
            base.MoveToElement();

        return moved;
    }

    public override bool MoveToNextAttribute()
    {
        var moved = base.MoveToNextAttribute();
        while (moved && (IsXmlNs || IsLocalXmlNs))
            moved = MoveToNextAttribute();

        return moved;
    }

    /// <summary>
    /// We only support the <c>xml</c> prefix, used for <c>xml:lang</c> and <c>xml:space</c> 
    /// built-in text handling in XHTML.
    /// </summary>
    public override string Prefix => base.Prefix == "xml" ? "xml" : "";

    public override string NamespaceURI => Prefix == "xml" ? base.NamespaceURI : "";

    bool IsXmlNs => base.NamespaceURI == XmlNsNamespace;

    bool IsLocalXmlNs => Prefix == "xmlns";
}

