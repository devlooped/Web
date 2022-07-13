using System.Xml;
using System.Xml.Linq;
using Sgml;

/// <summary>
/// Allows loading an HTML document as an <see cref="XDocument"/>.
/// </summary>
partial class HtmlDocument
{
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
    public static XDocument Load(string uri)
    {
        using var stream = (Stream)new XmlUrlResolver().GetEntity(new Uri(uri), string.Empty, typeof(Stream));
        return Load(stream);
    }

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
    public static XDocument Load(Stream stream)
        => Load(new StreamReader(stream));

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
    public static XDocument Load(TextReader textReader)
    {
        using var reader = new SgmlReader(new XmlReaderSettings
        {
            CheckCharacters = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
        })
        {
            InputStream = textReader,
            WhitespaceHandling = WhitespaceHandling.Significant,
        };

        return XDocument.Load(new XhtmlContentReader(reader));
    }

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
    /// <returns>
    /// An <see cref="XDocument"/> containing an XML tree initialized from the
    /// passed in HTML string.
    /// </returns>
    public static XDocument Parse(string html)
        => Load(new StringReader(html));

    /// <summary>
    /// Skips known non-content elements such as scripts and styles. 
    /// It also removes all XML namespaces, since for HTML content it's typically 
    /// irrelevant.
    /// </summary>
    class XhtmlContentReader : XmlWrappingReader
    {
        const string XmlNsNamespace = "http://www.w3.org/2000/xmlns/";

        /// <summary>
        /// Skips elements that typically aren't processed as content: 
        /// script, noscript, style and iframe.
        /// </summary>
        public static HashSet<string> DefaultSkipElements { get; } = new()
        {
            "script",
            "noscript",
            "style"
        };

        public XhtmlContentReader(XmlReader baseReader) : base(baseReader) { }

        public HashSet<string> SkipElements { get; } = DefaultSkipElements;

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

        public override bool Read()
        {
            var read = base.Read();
            if (read && base.NodeType == XmlNodeType.Element && SkipElements.Contains(LocalName))
                base.Skip();

            return read;
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
}

