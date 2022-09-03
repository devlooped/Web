using System;
using System.Collections.Generic;
using System.Xml;

namespace Devlooped.Web;

/// <summary>
/// Ignores specific elements from the input XML.
/// </summary>
class SkipElementsReader : XmlWrappingReader
{
    readonly HashSet<string> skipElements;

    public SkipElementsReader(XmlReader baseReader, string[] skipElements) : base(baseReader)
    {
        this.skipElements = new HashSet<string>(skipElements, StringComparer.OrdinalIgnoreCase);
    }

    public override bool Read()
    {
        var read = base.Read();
        if (read && base.NodeType == XmlNodeType.Element && skipElements.Contains(LocalName))
            base.Skip();

        return read;
    }
}
