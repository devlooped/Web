using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Sgml;

namespace Devlooped.Web;

/// <summary>
/// Allows loading an HTML document as an <see cref="XDocument"/>.
/// </summary>
public static class HtmlDocument
{
    const string DefaultPublicIdentifier = "-//W3C//DTD XHTML 1.0 Transitional//EN";
    const string DefaultSystemLiteral = "http://www.w3.org/TR/html4/loose.dtd";

    /// <summary>
    /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
    /// the passed <see cref="Stream"/> parameter.
    /// </summary>
    /// <param name="stream">
    /// A <see cref="Stream"/> containing the raw HTML to read into the newly
    /// created <see cref="XDocument"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="XDocument"/> containing the contents of the passed in
    /// <see cref="Stream"/>.
    /// </returns>
    public static XDocument Load(Stream stream) => Load(stream, HtmlReaderSettings.Default);

    /// <overloads>
    /// The Load method provides multiple strategies for creating a new
    /// <see cref="XDocument"/> and initializing it from a data source containing
    /// raw XML.  Load from a file (passing in a URI to the file), a
    /// <see cref="Stream"/> or an a <see cref="TextReader"/>. 
    /// Note:  Use <see cref="Parse(string)"/>
    /// to create an <see cref="XDocument"/> from a string containing HTML.
    /// <seealso cref="Parse(string)"/>
    /// </overloads>
    /// <summary>
    /// Create a new <see cref="XDocument"/> based on the contents of the file
    /// referenced by the URI parameter passed in. Note: Use
    /// <see cref="Parse(string)"/> to create an <see cref="XDocument"/> from
    /// a string containing HTML.
    /// <seealso cref="Parse(string)"/>
    /// </summary>
    /// <param name="uri">
    /// A URI string referencing the file to load into a new <see cref="XDocument"/>.
    /// </param>
    /// <returns>
    /// An <see cref="XDocument"/> initialized with the contents of the file referenced
    /// in the passed in uri parameter.
    /// </returns>
    public static XDocument Load(string uri) => Load(uri, HtmlReaderSettings.Default);

    /// <summary>
    /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
    /// the passed <see cref="TextReader"/> parameter.  Optionally whitespace handling
    /// can be preserved.
    /// </summary>
    /// <param name="textReader">
    /// A <see cref="TextReader"/> containing the raw HTML to read into the newly
    /// created <see cref="XDocument"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="XDocument"/> containing the contents of the passed in
    /// <see cref="TextReader"/>.
    /// </returns>
    public static XDocument Load(TextReader textReader) => Load(textReader, HtmlReaderSettings.Default);

    /// <summary>
    /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
    /// the passed <see cref="Stream"/> parameter.
    /// </summary>
    /// <param name="stream">
    /// A <see cref="Stream"/> containing the raw HTML to read into the newly
    /// created <see cref="XDocument"/>.
    /// </param>
    /// <param name="settings">The settings for the HTML load process.</param>
    /// <returns>
    /// A new <see cref="XDocument"/> containing the contents of the passed in
    /// <see cref="Stream"/>.
    /// </returns>
    public static XDocument Load(Stream stream, HtmlReaderSettings settings)
        => Load(new StreamReader(stream), settings);

    /// <overloads>
    /// The Load method provides multiple strategies for creating a new
    /// <see cref="XDocument"/> and initializing it from a data source containing
    /// raw XML.  Load from a file (passing in a URI to the file), a
    /// <see cref="Stream"/> or an a <see cref="TextReader"/>. 
    /// Note:  Use <see cref="Parse(string)"/>
    /// to create an <see cref="XDocument"/> from a string containing HTML.
    /// <seealso cref="Parse(string)"/>
    /// </overloads>
    /// <summary>
    /// Create a new <see cref="XDocument"/> based on the contents of the file
    /// referenced by the URI parameter passed in. Note: Use
    /// <see cref="Parse(string)"/> to create an <see cref="XDocument"/> from
    /// a string containing HTML.
    /// <seealso cref="Parse(string)"/>
    /// </summary>
    /// <param name="uri">
    /// A URI string referencing the file to load into a new <see cref="XDocument"/>.
    /// </param>
    /// <param name="settings">The settings for the HTML load process.</param>
    /// <returns>
    /// An <see cref="XDocument"/> initialized with the contents of the file referenced
    /// in the passed in uri parameter.
    /// </returns>
    public static XDocument Load(string uri, HtmlReaderSettings settings)
    {
        using var reader = new SgmlReader
        {
            Href = uri,
        };

        return XDocument.Load(Configure(reader, settings));
    }

    /// <summary>
    /// Create a new <see cref="XDocument"/> and initialize its underlying XML tree using
    /// the passed <see cref="TextReader"/> parameter.  Optionally whitespace handling
    /// can be preserved.
    /// </summary>
    /// <param name="textReader">
    /// A <see cref="TextReader"/> containing the raw HTML to read into the newly
    /// created <see cref="XDocument"/>.
    /// </param>
    /// <param name="settings">The settings for the HTML load process.</param>
    /// <returns>
    /// A new <see cref="XDocument"/> containing the contents of the passed in
    /// <see cref="TextReader"/>.
    /// </returns>
    public static XDocument Load(TextReader textReader, HtmlReaderSettings settings)
    {
        using var reader = new SgmlReader
        {
            InputStream = textReader,
        };

        return XDocument.Load(Configure(reader, settings));
    }

    /// <overloads>
    /// Create a new <see cref="XDocument"/> from a string containing
    /// HMTL.
    /// </overloads>
    /// <summary>
    /// Create a new <see cref="XDocument"/> from a string containing
    /// HTML.
    /// </summary>
    /// <param name="html">A string containing HTML.</param>
    /// <returns>
    /// An <see cref="XDocument"/> containing an XML tree initialized from the
    /// passed in HTML string.
    /// </returns>
    public static XDocument Parse(string html) => Parse(html, HtmlReaderSettings.Default);

    /// <overloads>
    /// Create a new <see cref="XDocument"/> from a string containing
    /// HMTL.
    /// </overloads>
    /// <summary>
    /// Create a new <see cref="XDocument"/> from a string containing
    /// HTML.
    /// </summary>
    /// <param name="html">
    /// A string containing HTML.
    /// </param>
    /// <param name="settings">The settings for the HTML load process.</param>
    /// <returns>
    /// An <see cref="XDocument"/> containing an XML tree initialized from the
    /// passed in HTML string.
    /// </returns>
    public static XDocument Parse(string html, HtmlReaderSettings settings)
        => Load(new StringReader(html), settings);

    static XmlReader Configure(SgmlReader reader, HtmlReaderSettings settings)
    {
        reader.DocType = "html";
        reader.PublicIdentifier = DefaultPublicIdentifier;
        reader.SystemLiteral = DefaultSystemLiteral;

        reader.CaseFolding = settings.CaseFolding;
        reader.WhitespaceHandling = settings.WhitespaceHandling;
        reader.TextWhitespace = settings.TextWhitespace;

        XmlReader result = reader;

        if (settings.IgnoreXmlNamespaces)
            result = new IgnoreXmlNsReader(result);

        if (settings.SkipElements.Length > 0)
            result = new SkipElementsReader(result, settings.SkipElements);

        return result;
    }
}

