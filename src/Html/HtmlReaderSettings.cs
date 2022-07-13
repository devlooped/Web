using System;
using System.Xml;
using Sgml;

namespace Devlooped.Html;

/// <summary>
/// Specifies a set of features to support when loading 
/// HTML via <see cref="HtmlDocument"/>.
/// </summary>
public sealed class HtmlReaderSettings
{
    /// <summary>
    /// HTML is case insensitive, so you can choose between converting
    /// to lower case or upper case tags. "None" means that the case is left
    /// alone, except that end tags will be folded to match the start tags.
    /// </summary>
    public CaseFolding CaseFolding { get; set; }

    /// <summary>
    /// Specifies how leading and trailing whitespace is handled.
    /// Note that this is a <see cref="FlagsAttribute"/>-enum.
    /// </summary>
    public TextWhitespaceHandling TextWhitespace { get; set; }

    /// <summary>
    /// Specifies how whitespace nodes are handled.
    /// </summary>
    public WhitespaceHandling WhitespaceHandling { get; set; }
}
