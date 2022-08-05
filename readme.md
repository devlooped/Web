![Icon](https://raw.githubusercontent.com/devlooped/web/main/assets/icon.png) HTML => XML + CSS with XLinq 🤘
============

[![Version](https://img.shields.io/nuget/vpre/Devlooped.Web.svg?color=royalblue)](https://www.nuget.org/packages/Devlooped.Web)
[![Downloads](https://img.shields.io/nuget/dt/Devlooped.Web.svg?color=green)](https://www.nuget.org/packages/Devlooped.Web)
[![License](https://img.shields.io/github/license/devlooped/web.svg?color=blue)](https://github.com/devlooped/web/blob/main/license.txt)

Read HTML as XML and query it with CSS over XLinq. 

No need to learn an entirely new object model for a page 🤘. 
This makes it the most productive and lean library for web 
scraping using the latest and greatest that .NET can offer.

# Usage

```csharp
using System.Xml.Linq;
using Devlooped.Web;

XDocument page = HtmlDocument.Load("page.html")
IEnumerable<XElement> elements = page.CssSelectElements("div.menuitem");

XElement title = page.CssSelectElement("html head meta[name=title]");
```

By default, `HtmlDocument.Load` will skip non-content elements `script` and 
`style`, turn all element names into lower case, and ignore all XML namespaces 
(useful when loading XHTML, for example) for easier querying. These options 
as well as granular whitespace handling can be configured using the overloads 
receiving an `HtmlReaderSettings`.

The underlying parsing is performed by the amazing [SgmlReader](https://www.nuget.org/packages/Microsoft.Xml.SgmlReader) 
library by Microsoft's [Chris Lovett](http://lovettsoftware.com/).

## CSS

At the moment, supports the following CSS selector features: 

- [Type selector](https://www.w3.org/TR/selectors-3/#type-selectors)
- [Universal selector](https://www.w3.org/TR/selectors-3/#universal-selector)
- [Attribute selector](https://www.w3.org/TR/selectors-3/#attribute-selectors)
- [Class selector](https://www.w3.org/TR/selectors-3/#class-html)
- [ID selector](https://www.w3.org/TR/selectors-3/#id-selectors)
- [Pseudo-classes](https://www.w3.org/TR/selectors-3/#pseudo-classes):
    * [:checked](https://www.w3.org/TR/selectors-3/#checked)
    * [:first-child](https://www.w3.org/TR/selectors-3/#first-child-pseudo)
    * [:last-child](https://www.w3.org/TR/selectors-3/#last-child-pseudo)
    * [:only-child](https://www.w3.org/TR/selectors-3/#only-child-pseudo)
    * [:empty](https://www.w3.org/TR/selectors-3/#empty-pseudo)
    * [:first-of-type](https://www.w3.org/TR/selectors-3/#first-of-type-pseudo)
    * [:last-of-type](https://www.w3.org/TR/selectors-3/#last-of-type-pseudo)
    * [:not(...)](https://www.w3.org/TR/selectors-3/#negation)
    * [:nth-of-type(n)](https://www.w3.org/TR/selectors-3/#nth-of-type-pseudo)
    * [:nth-child(n)](https://www.w3.org/TR/selectors-3/#nth-child-pseudo)
    * [:has(...)](https://www.w3.org/TR/selectors-4/#has-pseudo)

And all [combinators](https://www.w3.org/TR/selectors-3/#combinators)

Non-CSS features:

- `text()` pseudo-attribute selector: selects the node text contents, as specified 
  in the [XPath](https://www.w3.org/TR/1999/REC-xpath-19991116/) `text()` location 
  path. Can be used instead of an attribute name selector, such as `div[text()=foo]`. 
  All [attribute value selectors](https://www.w3.org/TR/selectors-3/#attribute-selectors) 
  are also supported:
    * `[text()=val]`: Represents an element whose text contents is exactly "val".
    * `[text()~=val]`: Represents an element whose text contents is a whitespace-separated list of words, 
       one of which is exactly "val". If "val" contains whitespace, it will never represent anything (since the words 
       are separated by spaces). Also if "val" is the empty string, it will never represent anything.
    * `[text()|=val]`: Represents an element whose text contents either being exactly "val" or 
       beginning with "val" immediately followed by "-" (U+002D). 
    * `[text()^=val]`: Represents an element whose text contents begins with the prefix "val". 
       If "val" is the empty string then the selector does not represent anything.
    * `[text()$=val]`: Represents an element whose text contents ends with the suffix "val". 
       If "val" is the empty string then the selector does not represent anything.
    * `[text()*=val]`: Represents an element whose text contents contains at least one instance of the 
       substring "val". If "val" is the empty string then the selector does not represent anything.

# Dogfooding

[![CI Version](https://img.shields.io/endpoint?url=https://shields.kzu.io/vpre/Devlooped.Web/main&label=nuget.ci&color=brightgreen)](https://pkg.kzu.io/index.json)
[![Build](https://github.com/devlooped/web/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/web/actions)

We also produce CI packages from branches and pull requests so you can dogfood builds as quickly as they are produced. 

The CI feed is `https://pkg.kzu.io/index.json`. 

The versioning scheme for packages is:

- PR builds: *42.42.42-pr*`[NUMBER]`
- Branch builds: *42.42.42-*`[BRANCH]`.`[COMMITS]`


<!-- include docs/footer.md -->
# Sponsors 

<!-- include sponsors.md -->
<!-- sponsors -->

<a href='https://github.com/KirillOsenkov'>
  <img src='https://github.com/devlooped/sponsors/raw/main/.github/avatars/KirillOsenkov.svg' alt='Kirill Osenkov' title='Kirill Osenkov'>
</a>
<a href='https://github.com/augustoproiete'>
  <img src='https://github.com/devlooped/sponsors/raw/main/.github/avatars/augustoproiete.svg' alt='C. Augusto Proiete' title='C. Augusto Proiete'>
</a>
<a href='https://github.com/sandrock'>
  <img src='https://github.com/devlooped/sponsors/raw/main/.github/avatars/sandrock.svg' alt='SandRock' title='SandRock'>
</a>
<a href='https://github.com/aws'>
  <img src='https://github.com/devlooped/sponsors/raw/main/.github/avatars/aws.svg' alt='Amazon Web Services' title='Amazon Web Services'>
</a>
<a href='https://github.com/MelbourneDeveloper'>
  <img src='https://github.com/devlooped/sponsors/raw/main/.github/avatars/MelbourneDeveloper.svg' alt='Christian Findlay' title='Christian Findlay'>
</a>
<a href='https://github.com/clarius'>
  <img src='https://github.com/devlooped/sponsors/raw/main/.github/avatars/clarius.svg' alt='Clarius Org' title='Clarius Org'>
</a>
<a href='https://github.com/MFB-Technologies-Inc'>
  <img src='https://github.com/devlooped/sponsors/raw/main/.github/avatars/MFB-Technologies-Inc.svg' alt='MFB Technologies, Inc.' title='MFB Technologies, Inc.'>
</a>

<!-- sponsors -->

<!-- sponsors.md -->

<br><br>
<a href="https://github.com/sponsors/devlooped" title="Sponsor this project">
  <img src="https://github.com/devlooped/sponsors/blob/main/sponsor.png" />
</a>
<br>

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- docs/footer.md -->
