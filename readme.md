![Icon](https://raw.githubusercontent.com/devlooped/web/main/assets/icon.png) HTML => XML + CSS with XLinq 🤘
============

[![Version](https://img.shields.io/nuget/vpre/Devlooped.Web.svg?color=royalblue)](https://www.nuget.org/packages/Devlooped.Web)
[![Downloads](https://img.shields.io/nuget/dt/Devlooped.Web.svg?color=green)](https://www.nuget.org/packages/Devlooped.Web)
[![License](https://img.shields.io/github/license/devlooped/web.svg?color=blue)](https://github.com/devlooped/web/blob/main/license.txt)

<!-- #content -->
<!-- include https://github.com/devlooped/.github/raw/main/sponsorlink.md -->
*This project uses [SponsorLink](https://github.com/devlooped#sponsorlink) 
and may issue IDE-only warnings if no active sponsorship is detected.*

<!-- https://github.com/devlooped/.github/raw/main/sponsorlink.md -->

Read HTML as XML and query it with CSS over XLinq (or HtmlAgilityPack killer 😉). 
Provides `HtmlDocument.Load` and `CssSelectElement(s)` extension methods 
for `XDocument`/`XElement`.

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

In addition, the following extension methods make it easier to work 
with XML documents where you want to query with CSS or XPath without 
having to deal with XML namespaces:

```csharp
using System.Xml;
using System.Xml.Linq;
using Devlooped.Web;

var doc = XDocument.Load("doc.xml")
// Will remove all xmlns declarations, and allow querying elements 
// as if none had namespaces, returns the root element
XElement nons = doc.RemoveNamespaces();

// Alternatively, you can also ignore at the XmlReader level
using var reader = XmlReader.Create("doc.xml").IgnoreNamespaces();
doc = XDocument.Load(reader);

// Finally, you can also skip elements at the reader level
using var reader = XmlReader.Create("doc.xml").SkipElements("foo", "bar");
doc = XDocument.Load(reader);
```

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

<!-- #content -->

# Dogfooding

[![CI Version](https://img.shields.io/endpoint?url=https://shields.kzu.app/vpre/Devlooped.Web/main&label=nuget.ci&color=brightgreen)](https://pkg.kzu.app/index.json)
[![Build](https://github.com/devlooped/web/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/devlooped/web/actions)

We also produce CI packages from branches and pull requests so you can dogfood builds as quickly as they are produced. 

The CI feed is `https://pkg.kzu.app/index.json`. 

The versioning scheme for packages is:

- PR builds: *42.42.42-pr*`[NUMBER]`
- Branch builds: *42.42.42-*`[BRANCH]`.`[COMMITS]`


<!-- #sponsors -->
<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://avatars.githubusercontent.com/u/71888636?v=4&s=39 "Clarius Org")](https://github.com/clarius)
[![MFB Technologies, Inc.](https://avatars.githubusercontent.com/u/87181630?v=4&s=39 "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![SandRock](https://avatars.githubusercontent.com/u/321868?u=99e50a714276c43ae820632f1da88cb71632ec97&v=4&s=39 "SandRock")](https://github.com/sandrock)
[![DRIVE.NET, Inc.](https://avatars.githubusercontent.com/u/15047123?v=4&s=39 "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![Keith Pickford](https://avatars.githubusercontent.com/u/16598898?u=64416b80caf7092a885f60bb31612270bffc9598&v=4&s=39 "Keith Pickford")](https://github.com/Keflon)
[![Thomas Bolon](https://avatars.githubusercontent.com/u/127185?u=7f50babfc888675e37feb80851a4e9708f573386&v=4&s=39 "Thomas Bolon")](https://github.com/tbolon)
[![Kori Francis](https://avatars.githubusercontent.com/u/67574?u=3991fb983e1c399edf39aebc00a9f9cd425703bd&v=4&s=39 "Kori Francis")](https://github.com/kfrancis)
[![Uno Platform](https://avatars.githubusercontent.com/u/52228309?v=4&s=39 "Uno Platform")](https://github.com/unoplatform)
[![Reuben Swartz](https://avatars.githubusercontent.com/u/724704?u=2076fe336f9f6ad678009f1595cbea434b0c5a41&v=4&s=39 "Reuben Swartz")](https://github.com/rbnswartz)
[![Jacob Foshee](https://avatars.githubusercontent.com/u/480334?v=4&s=39 "Jacob Foshee")](https://github.com/jfoshee)
[![](https://avatars.githubusercontent.com/u/33566379?u=bf62e2b46435a267fa246a64537870fd2449410f&v=4&s=39 "")](https://github.com/Mrxx99)
[![Eric Johnson](https://avatars.githubusercontent.com/u/26369281?u=41b560c2bc493149b32d384b960e0948c78767ab&v=4&s=39 "Eric Johnson")](https://github.com/eajhnsn1)
[![Jonathan ](https://avatars.githubusercontent.com/u/5510103?u=98dcfbef3f32de629d30f1f418a095bf09e14891&v=4&s=39 "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Ken Bonny](https://avatars.githubusercontent.com/u/6417376?u=569af445b6f387917029ffb5129e9cf9f6f68421&v=4&s=39 "Ken Bonny")](https://github.com/KenBonny)
[![Simon Cropp](https://avatars.githubusercontent.com/u/122666?v=4&s=39 "Simon Cropp")](https://github.com/SimonCropp)
[![agileworks-eu](https://avatars.githubusercontent.com/u/5989304?v=4&s=39 "agileworks-eu")](https://github.com/agileworks-eu)
[![Zheyu Shen](https://avatars.githubusercontent.com/u/4067473?v=4&s=39 "Zheyu Shen")](https://github.com/arsdragonfly)
[![Vezel](https://avatars.githubusercontent.com/u/87844133?v=4&s=39 "Vezel")](https://github.com/vezel-dev)
[![ChilliCream](https://avatars.githubusercontent.com/u/16239022?v=4&s=39 "ChilliCream")](https://github.com/ChilliCream)
[![4OTC](https://avatars.githubusercontent.com/u/68428092?v=4&s=39 "4OTC")](https://github.com/4OTC)
[![domischell](https://avatars.githubusercontent.com/u/66068846?u=0a5c5e2e7d90f15ea657bc660f175605935c5bea&v=4&s=39 "domischell")](https://github.com/DominicSchell)
[![Adrian Alonso](https://avatars.githubusercontent.com/u/2027083?u=129cf516d99f5cb2fd0f4a0787a069f3446b7522&v=4&s=39 "Adrian Alonso")](https://github.com/adalon)
[![Michael Hagedorn](https://avatars.githubusercontent.com/u/61711586?u=8f653dfcb641e8c18cc5f78692ebc6bb3a0c92be&v=4&s=39 "Michael Hagedorn")](https://github.com/Eule02)
[![torutek](https://avatars.githubusercontent.com/u/33917059?v=4&s=39 "torutek")](https://github.com/torutek)
[![mccaffers](https://avatars.githubusercontent.com/u/16667079?u=739e110e62a75870c981640447efa5eb2cb3bc8f&v=4&s=39 "mccaffers")](https://github.com/mccaffers)
[![Seika Logiciel](https://avatars.githubusercontent.com/u/2564602?v=4&s=39 "Seika Logiciel")](https://github.com/SeikaLogiciel)
[![Andrew Grant](https://avatars.githubusercontent.com/devlooped-user?s=39 "Andrew Grant")](https://github.com/wizardness)


<!-- sponsors.md -->
[![Sponsor this project](https://avatars.githubusercontent.com/devlooped-sponsor?s=118 "Sponsor this project")](https://github.com/sponsors/devlooped)

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
