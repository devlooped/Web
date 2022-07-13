using System.Xml.XPath;
using Devlooped.Html;

namespace Devlooped.Tests;

public record HtmlTests(ITestOutputHelper Output)
{
    [Fact]
    public void SkipsAllScripts()
    {
        var doc = HtmlDocument.Load(new Uri("file://" + new FileInfo("wikipedia.html").FullName).AbsoluteUri);

        Assert.Empty(doc.XPathSelectElements("//script"));
    }

    [Fact]
    public void SkipsAllStylesAsync()
    {
        var doc = HtmlDocument.Load(new Uri("file://" + new FileInfo("wikipedia.html").FullName).AbsoluteUri);

        Assert.Empty(doc.XPathSelectElements("//stype"));
    }
}