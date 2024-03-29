﻿using System.Linq;
using System.Text;
using System.Xml.Linq;
using Superpower;

namespace Devlooped.Tests;

public record XPathTests(ITestOutputHelper Console)
{
    [InlineData("foo", ".//foo")]
    [InlineData(".foo", ".//*[contains(concat(\" \",normalize-space(@class),\" \"),\" foo \")]")]
    [InlineData("foo > bar", ".//foo/bar")]
    [InlineData("foo > bar > baz", ".//foo/bar/baz")]
    [InlineData("foo ~ bar", ".//foo/following-sibling::bar")]
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
    [InlineData("div span:first-of-type", "1")]
    [InlineData("div span:last-of-type", "4")]
    [InlineData("span:not([id])", "4 5")]
    [InlineData("div[role^=menu]:not([role=menuitem])", "Standard")]
    [InlineData("div[role=menuitem]:nth-of-type(2)", "Archivo")]
    [InlineData("p:has(h1)", "Sub-header")]
    [InlineData("div:has(div[id], span[id=one])", "Hello1234")]
    [InlineData("div[text()=Hello] + span:first-of-type", "1")]
    [InlineData("option[selected=\"selected\"]", "second")]
    [InlineData("div[tags^=\"selected flow\"]+div", "Archivo")]
    [InlineData("div[tags^=\"selected flow\"] + div", "Archivo")]
    [InlineData("div[role],span", "Warning 1 2 4 5 Standard File Archivo Edit Footer")]
    [InlineData("span:not([text()='4']),div[role]", "Warning 1 2 5 Standard File Archivo Edit Footer")]
    [InlineData(".item__hiden-content", "Archivo")]
    [InlineData("body > div > span", "1 2 4")]
    [InlineData("body > div:nth-of-type(3)", "Standard")]
    [InlineData("body > span:nth-child(5)", "5")]
    [Theory]
    public void EvaluatePageHtml(string expression, string expected)
    {
        var actual = string.Join(' ', XDocument.Load("page.html")
            .CssSelectElements(expression)
            .Select(x => x.Value));

        if (!expected.Equals(actual))
            Console.WriteLine($"{expression} > {Parser.Parse(expression).ToXPath()}");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SelectEmptyDiv()
    {
        var expr = Parser.Parse("script[text()]");

        var elements = XDocument.Load("page.html")
            .CssSelectElements("script[text()]")
            .ToArray();

        Assert.DoesNotContain(elements, t => t.IsEmpty);
    }

    [Fact]
    public void ParseSelector()
    {
        var div = Parser.SimpleSelectorSequence.Parse("div[title]");

        Assert.NotNull(div);
    }

    [InlineData("[text()=hello world]", "[text()=\"hello world\"]")]
    [InlineData("[text()='hello world']", "[text()=\"hello world\"]")]
    [InlineData("[text()=\"hello world\"]", "[text()=\"hello world\"]")]
    [InlineData("[text()*=\"hello world\"]", "[contains(text(),\"hello world\")]")]
    [InlineData("[text()*='hello world']", "[contains(text(),\"hello world\")]")]
    [InlineData("[data='hello world']", "[@data=\"hello world\"]")]
    [InlineData("[data=hello world]", "[@data=\"hello world\"]")]
    [InlineData("[data=\"hello world\"]", "[@data=\"hello world\"]")]
    [InlineData("[data*=\"hello world\"]", "[contains(@data,\"hello world\")]")]
    [InlineData("[data*='hello world']", "[contains(@data,\"hello world\")]")]
    [Theory]
    internal void AttributeSelectorNormalizesQuotes(string expression, string xpath)
    {
        var selector = (AttributeSelector)Parser.AttributeSelector.Parse(expression);

        var builder = new StringBuilder();
        selector.Append(builder);

        Assert.Equal(xpath, builder.ToString());
    }


    [Fact]
    public void RenderExpression()
    {
        var expression = "*[text()$=\"to go\"]";
        Console.WriteLine(Parser.Parse(expression).ToXPath());
    }
}
