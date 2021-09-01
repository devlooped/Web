using System.Linq;
using System.Xml.Linq;

namespace Devlooped.Tests;

public record XPathTests(ITestOutputHelper Console)
{
    [InlineData("foo", ".//foo")]
    [InlineData(".foo", ".//*[contains(concat(\" \",normalize-space(@class),\" \"),\" foo \")]")]
    [InlineData("foo > bar", ".//foo/child::*[1]/self::bar")]
    // .//*[@data-tags="flow" or starts-with(@data-tags,concat("flow","-"))]
    [InlineData("[data-tags|=flow]", ".//*[@data-tags=\"flow\" or starts-with(@data-tags,concat(\"flow\",\"-\"))]")]
    [InlineData("[role*=nu]", ".//*[contains(@role,\"nu\")]")]
    [Theory]
    public void ToXPath(string css, string xpath)
    {
        var result = Parser.Parse(css).ToXPath();
        if (result != xpath)
            Console.WriteLine(result);

        Assert.Equal(xpath, result);
    }

    // If we do the multi-element selection concatenation ourselves, in the test, 
    // values will be separated by whitespace, but if the selector selects a 
    // single element, we'd get them all concatenated using XML rules, with no spaces.
    [InlineData("span", "1 2 4 5")]
    [InlineData("div > div", "Hello")]
    [InlineData("span + p", "3")]
    [InlineData("#title", "Hello")]
    [InlineData(".container", "Hello1234")]
    [InlineData(".main", "Hello1234")]
    [InlineData("div[role=menuitem]", "File Archivo Edit")]
    [InlineData("div[role]", "Warning Standard File Archivo Edit Footer")]
    [InlineData("div[role][lang|=es]", "Archivo")]
    [InlineData("[tags~=flow]", "File Edit")]
    [InlineData("[tags~=selected]", "File")]
    [InlineData("div[role^=menu]", "Standard File Archivo Edit")]
    [InlineData("div[role$=item]", "File Archivo Edit")]
    [InlineData("div[role*=nu]", "Standard File Archivo Edit")]
    [InlineData("div.menuitem", "File Archivo Edit")]
    [InlineData("div[role=alert]", "Warning")]
    [InlineData("input:checked", " ")]
    [InlineData("option:checked", "second")]
    [InlineData("div:first-child", "Warning Hello")]
    [InlineData("div:last-child", "Footer")]
    [InlineData("h1:only-child", "Sub-header")]
    [InlineData(":empty", "   ")] // head + 3 inputs
    [Theory]
    public void EvaluatePageHtml(string expression, string value)
        => Assert.Equal(value, string.Join(' ', XDocument.Load("page.html")
            .CssSelectElements(expression)
            .Select(x => x.Value)));

}
