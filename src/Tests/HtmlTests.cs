using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Devlooped.Web;

namespace Devlooped.Tests;

public record HtmlTests(ITestOutputHelper Output)
{
    [Fact]
    public void Render()
    {
        var doc = HtmlDocument.Load(File("sample.html"));

        Output.WriteLine(doc.ToString());
    }

    [Fact]
    public void ExcludesScriptsByDefault()
    {
        var doc = HtmlDocument.Load(File("wikipedia.html"));

        Assert.Empty(doc.XPathSelectElements("//script"));
    }

    [Fact]
    public void IncludeScriptsExplicitSettings()
    {
        var doc = HtmlDocument.Load(File("wikipedia.html"), new HtmlReaderSettings());

        Assert.NotEmpty(doc.XPathSelectElements("//script"));
    }

    [Fact]
    public void ExcludesStylesByDefault()
    {
        var doc = HtmlDocument.Load(File("wikipedia.html"));

        Assert.Empty(doc.XPathSelectElements("//style"));
    }

    [Fact]
    public void IncludeStylesExplicitSettings()
    {
        var doc = HtmlDocument.Load(File("wikipedia.html"), new HtmlReaderSettings());

        Assert.NotEmpty(doc.XPathSelectElements("//style"));
    }

    [Fact]
    public void ExcludesXmlNamespacesByDefault()
    {
        var doc = HtmlDocument.Load(File("sample.xhtml"));

        Assert.NotEmpty(doc.XPathSelectElements("//h1"));
    }

    [Fact]
    public void IncludeXmlNamespacesExplicitly()
    {
        var doc = HtmlDocument.Load(File("sample.xhtml"), new HtmlReaderSettings { IgnoreXmlNamespaces = false });
        var resolver = new XmlNamespaceManager(new NameTable());
        resolver.AddNamespace("xh", "http://www.w3.org/1999/xhtml");

        Assert.NotEmpty(doc.XPathSelectElements("//xh:h1", resolver));
        // Won't match because the elements will have the XHTML namespace
        Assert.Empty(doc.XPathSelectElements("//h1"));
    }

    [Fact]
    public void CanChangeToUpperCaseHtml()
    {
        var doc = HtmlDocument.Load(File("wikipedia.html"),
            new HtmlReaderSettings
            {
                CaseFolding = Sgml.CaseFolding.ToUpper,
            });

        // The source has lowercase elements
        var central = doc.XPathSelectElement("/HTML/BODY/DIV/H1/SPAN");

        Assert.NotNull(central);
    }

    [Fact]
    public void HtmlSettings()
    {
        var doc = HtmlDocument.Load(File("wikipedia.html"),
            new HtmlReaderSettings
            {
                TextWhitespace = Sgml.TextWhitespaceHandling.TrimBoth,
                WhitespaceHandling = WhitespaceHandling.None
            });

        var central = doc.XPathSelectElement("/html/body/div/h1/span");

        // The source contains leading and trailing whitespaces.
        Assert.Equal("Wikipedia", central?.Value);
    }

    string File(string path) => new Uri("file://" + new FileInfo(path).FullName).AbsoluteUri;
}