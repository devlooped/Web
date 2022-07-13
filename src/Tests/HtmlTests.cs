using System.Xml.XPath;
using Devlooped.Html;

namespace Devlooped.Tests;

public record HtmlTests(ITestOutputHelper Output)
{
    [Fact]
    public void IncludesScripts()
    {
        var doc = HtmlDocument.Load(new Uri("file://" + new FileInfo("wikipedia.html").FullName).AbsoluteUri);

        Assert.NotEmpty(doc.XPathSelectElements("//script"));
    }

    [Fact]
    public void IncludesStyles()
    {
        var doc = HtmlDocument.Load(new Uri("file://" + new FileInfo("wikipedia.html").FullName).AbsoluteUri);

        Assert.NotEmpty(doc.XPathSelectElements("//style"));
    }

    [Fact]
    public void RemovesXmlNamespaces()
    {
        var doc = HtmlDocument.Load(new Uri("file://" + new FileInfo("sample.xhtml").FullName).AbsoluteUri);

        Assert.NotEmpty(doc.XPathSelectElements("//h1"));
    }

    [Fact]
    public void HtmlSettings()
    {
        var doc = HtmlDocument.Load(new Uri("file://" + new FileInfo("wikipedia.html").FullName).AbsoluteUri,
            new HtmlReaderSettings
            {
                CaseFolding = Sgml.CaseFolding.ToUpper,
                TextWhitespace = Sgml.TextWhitespaceHandling.TrimBoth,
                WhitespaceHandling = System.Xml.WhitespaceHandling.None
            });

        // The source has lowercase elements
        var central = doc.XPathSelectElement("/HTML/BODY/DIV/H1/SPAN");

        Assert.NotNull(central);

        // The source contains leading and trailing whitespaces.
        Assert.Equal("Wikipedia", central!.Value);
    }

    [Fact]
    public void OptOutOfXmlNamespacesRemoval()
    {
        var doc = HtmlDocument.Load(new Uri("file://" + new FileInfo("sample.xhtml").FullName).AbsoluteUri,
            new HtmlReaderSettings { IgnoreXmlNamespaces = false });

        // Won't match because the elements will have the XHTML namespace
        Assert.Empty(doc.XPathSelectElements("//h1"));
    }
}