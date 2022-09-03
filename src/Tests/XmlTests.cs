using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Tests;

public record XmlTests(ITestOutputHelper Output)
{
    [Fact]
    public void RemoveNamespacesFromElement()
    {
        var doc = XDocument.Load("package.opf");
        var nons = doc.Root!.RemoveNamespaces();

        var xmlns = new XmlNamespaceManager(new NameTable());
        xmlns.AddNamespace("opf", "http://www.idpf.org/2007/opf");
        xmlns.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

        var yearns = doc.XPathSelectElement("/opf:package/opf:metadata/opf:meta[@property='dcterms:date']", xmlns)?.Value;

        // NOTE: since we're at the element level now, we don't need to reference the root element
        var year = nons.XPathSelectElement("/metadata/meta[@property='dcterms:date']")?.Value;

        //Output.WriteLine(doc.Root!.Elements().First().ToString());
        //Output.WriteLine(nons.Elements().First().ToString());

        Assert.NotNull(yearns);
        Assert.NotNull(year);

        Assert.Equal(yearns, year);
    }

    [Fact]
    public void RemoveElementsFromReader()
    {
        using var reader = XmlReader.Create("package.opf").SkipElements("manifest");
        var doc = XDocument.Load(reader);

        var all = XDocument.Load("package.opf");

        Assert.NotEqual(doc.Root!.Elements().Count(), all.Root!.Elements().Count());
    }
}
