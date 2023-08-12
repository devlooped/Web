![Icon](https://raw.githubusercontent.com/devlooped/web/main/assets/icon.png) HTML => XML + CSS with XLinq 🤘
============

[![Version](https://img.shields.io/nuget/vpre/Devlooped.Web.svg?color=royalblue)](https://www.nuget.org/packages/Devlooped.Web)
[![Downloads](https://img.shields.io/nuget/dt/Devlooped.Web.svg?color=green)](https://www.nuget.org/packages/Devlooped.Web)
[![License](https://img.shields.io/github/license/devlooped/web.svg?color=blue)](https://github.com/devlooped/web/blob/main/license.txt)

<!-- #content -->
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

[![CI Version](https://img.shields.io/endpoint?url=https://shields.kzu.io/vpre/Devlooped.Web/main&label=nuget.ci&color=brightgreen)](https://pkg.kzu.io/index.json)
[![Build](https://github.com/devlooped/web/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/web/actions)

We also produce CI packages from branches and pull requests so you can dogfood builds as quickly as they are produced. 

The CI feed is `https://pkg.kzu.io/index.json`. 

The versioning scheme for packages is:

- PR builds: *42.42.42-pr*`[NUMBER]`
- Branch builds: *42.42.42-*`[BRANCH]`.`[COMMITS]`


<!-- #sponsors -->
<!-- include https://github.com/devlooped/sponsors/raw/main/footer.md -->
# Sponsors 

<!-- sponsors.md -->
[![Clarius Org](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/clarius.png "Clarius Org")](https://github.com/clarius)
[![C. Augusto Proiete](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/augustoproiete.png "C. Augusto Proiete")](https://github.com/augustoproiete)
[![Kirill Osenkov](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/KirillOsenkov.png "Kirill Osenkov")](https://github.com/KirillOsenkov)
[![MFB Technologies, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MFB-Technologies-Inc.png "MFB Technologies, Inc.")](https://github.com/MFB-Technologies-Inc)
[![Stephen Shaw](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/decriptor.png "Stephen Shaw")](https://github.com/decriptor)
[![Torutek](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/torutek-gh.png "Torutek")](https://github.com/torutek-gh)
[![DRIVE.NET, Inc.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/drivenet.png "DRIVE.NET, Inc.")](https://github.com/drivenet)
[![David Kean](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/davkean.png "David Kean")](https://github.com/davkean)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/chiluap.png "")](https://github.com/chiluap)
[![Daniel Gnägi](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/dgnaegi.png "Daniel Gnägi")](https://github.com/dgnaegi)
[![Ashley Medway](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/AshleyMedway.png "Ashley Medway")](https://github.com/AshleyMedway)
[![Keith Pickford](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Keflon.png "Keith Pickford")](https://github.com/Keflon)
[![bitbonk](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/bitbonk.png "bitbonk")](https://github.com/bitbonk)
[![Thomas Bolon](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/tbolon.png "Thomas Bolon")](https://github.com/tbolon)
[![Yurii Rashkovskii](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/yrashk.png "Yurii Rashkovskii")](https://github.com/yrashk)
[![Kori Francis](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/kfrancis.png "Kori Francis")](https://github.com/kfrancis)
[![Zdenek Havlin](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/wdolek.png "Zdenek Havlin")](https://github.com/wdolek)
[![Sean Killeen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/SeanKilleen.png "Sean Killeen")](https://github.com/SeanKilleen)
[![Toni Wenzel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/twenzel.png "Toni Wenzel")](https://github.com/twenzel)
[![Giorgi Dalakishvili](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Giorgi.png "Giorgi Dalakishvili")](https://github.com/Giorgi)
[![Kelly White](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/mckhendry.png "Kelly White")](https://github.com/mckhendry)
[![Allan Ritchie](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/aritchie.png "Allan Ritchie")](https://github.com/aritchie)
[![Mike James](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MikeCodesDotNET.png "Mike James")](https://github.com/MikeCodesDotNET)
[![Uno Platform](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/unoplatform.png "Uno Platform")](https://github.com/unoplatform)
[![Dan Siegel](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/dansiegel.png "Dan Siegel")](https://github.com/dansiegel)
[![Reuben Swartz](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/rbnswartz.png "Reuben Swartz")](https://github.com/rbnswartz)
[![Jeremy Simmons](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jeremysimmons.png "Jeremy Simmons")](https://github.com/jeremysimmons)
[![Jacob Foshee](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jfoshee.png "Jacob Foshee")](https://github.com/jfoshee)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Mrxx99.png "")](https://github.com/Mrxx99)
[![Eric Johnson](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/eajhnsn1.png "Eric Johnson")](https://github.com/eajhnsn1)
[![Norman Mackay](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/mackayn.png "Norman Mackay")](https://github.com/mackayn)
[![Certify The Web](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/certifytheweb.png "Certify The Web")](https://github.com/certifytheweb)
[![Taylor Mansfield](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/lavahot.png "Taylor Mansfield")](https://github.com/lavahot)
[![Mårten Rånge](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/mrange.png "Mårten Rånge")](https://github.com/mrange)
[![David Petric](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/davidpetric.png "David Petric")](https://github.com/davidpetric)
[![Rich Lee](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/richlee.png "Rich Lee")](https://github.com/richlee)
[![Danilo Dantas](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/dannevesdantas.png "Danilo Dantas")](https://github.com/dannevesdantas)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/nietras.png "")](https://github.com/nietras)
[![Gary Woodfine](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/garywoodfine.png "Gary Woodfine")](https://github.com/garywoodfine)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/kristinnstefansson.png "")](https://github.com/kristinnstefansson)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/DarrenAtConexus.png "")](https://github.com/DarrenAtConexus)
[![Steve Bilogan](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/kazo0.png "Steve Bilogan")](https://github.com/kazo0)
[![Ix Technologies B.V.](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/IxTechnologies.png "Ix Technologies B.V.")](https://github.com/IxTechnologies)
[![New Relic](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/newrelic.png "New Relic")](https://github.com/newrelic)
[![Chris Johnston‮](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Chris-Johnston.png "Chris Johnston‮")](https://github.com/Chris-Johnston)
[![David JENNI](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/davidjenni.png "David JENNI")](https://github.com/davidjenni)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/ehonda.png "")](https://github.com/ehonda)
[![Jonathan ](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Jonathan-Hickey.png "Jonathan ")](https://github.com/Jonathan-Hickey)
[![Oleg Kyrylchuk](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/okyrylchuk.png "Oleg Kyrylchuk")](https://github.com/okyrylchuk)
[![Juan Blanco](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/juanfranblanco.png "Juan Blanco")](https://github.com/juanfranblanco)
[![LosManos](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/LosManos.png "LosManos")](https://github.com/LosManos)
[![Mariusz Kogut](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/MariuszKogut.png "Mariusz Kogut")](https://github.com/MariuszKogut)
[![Charley Wu](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/akunzai.png "Charley Wu")](https://github.com/akunzai)
[![](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/meisenring.png "")](https://github.com/meisenring)
[![Thomas Due](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/Tdue21.png "Thomas Due")](https://github.com/Tdue21)
[![Jakob Tikjøb Andersen](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/jakobt.png "Jakob Tikjøb Andersen")](https://github.com/jakobt)
[![Seann Alexander](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/seanalexander.png "Seann Alexander")](https://github.com/seanalexander)
[![Tino Hager](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/tinohager.png "Tino Hager")](https://github.com/tinohager)
[![Badre BSAILA](https://raw.githubusercontent.com/devlooped/sponsors/main/.github/avatars/pedrobsaila.png "Badre BSAILA")](https://github.com/pedrobsaila)


<!-- sponsors.md -->

[![Sponsor this project](https://raw.githubusercontent.com/devlooped/sponsors/main/sponsor.png "Sponsor this project")](https://github.com/sponsors/devlooped)
&nbsp;

[Learn more about GitHub Sponsors](https://github.com/sponsors)

<!-- https://github.com/devlooped/sponsors/raw/main/footer.md -->
