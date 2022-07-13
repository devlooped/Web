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
    /// Default settings when reading HTML, which are:
    /// <see cref="CaseFolding.ToLower" />, <see cref="IgnoreXmlNamespaces"/>=true  
    /// and <see cref="SkipElements"/>=["script", "style"].
    /// </summary>
    public static HtmlReaderSettings Default { get; } = new HtmlReaderSettings
    {
        CaseFolding = CaseFolding.ToLower,
        IgnoreXmlNamespaces = true,
        SkipElements = new string[] { "script", "style" },
    };

    /// <summary>
    /// HTML is case insensitive, so you can choose between converting
    /// to lower case or upper case tags. "None" means that the case is left
    /// alone, except that end tags will be folded to match the start tags.
    /// </summary>
    public CaseFolding CaseFolding { get; set; }

    /// <summary>
    /// Whether to ignore XML namespaces in the input. Default is true.
    /// </summary>
    public bool IgnoreXmlNamespaces { get; set; } = true;

    /// <summary>
    /// Elements that should be skipped when reading the HTML so they are 
    /// not loaded into the resulting XML document. Defaults to no elements.
    /// </summary>
    public string[] SkipElements { get; set; } = Array.Empty<string>();

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
