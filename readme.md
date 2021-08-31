![Icon](https://raw.githubusercontent.com/devlooped/css/main/assets/img/icon-32.png) Linq to Css
============

[![Version](https://img.shields.io/nuget/vpre/Devlooped.Xml.Css.svg?color=royalblue)](https://www.nuget.org/packages/Devlooped.Xml.Css)
[![Downloads](https://img.shields.io/nuget/dt/Devlooped.Xml.Css.svg?color=green)](https://www.nuget.org/packages/Devlooped.Xml.Css)
[![License](https://img.shields.io/github/license/devlooped/css.svg?color=blue)](https://github.com/devlooped/css/blob/main/license.txt)
[![Build](https://github.com/devlooped/css/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/css/actions)

Implements CSS selectors for XLinq.

# Usage

```csharp
using Devlooped.Xml.Css;

var page = XDocument.Load("page.xhtml")
IEnumerable<XElement> elements = page.CssSelectElements("div.menuitem");

XElement title = page.CssSelectElement("div[role=alert]");
```

At the moment, supports the following CSS selector features: 

- [Type selector](https://www.w3.org/TR/selectors-3/#type-selectors)
- [Universal selector](https://www.w3.org/TR/selectors-3/#universal-selector)
- [Attribute selector](https://www.w3.org/TR/selectors-3/#attribute-selectors)
- [Class selector](https://www.w3.org/TR/selectors-3/#class-html)
- [ID selector](https://www.w3.org/TR/selectors-3/#id-selectors)

And all [combinators](https://www.w3.org/TR/selectors-3/#combinators)


# Dogfooding

[![CI Version](https://img.shields.io/endpoint?url=https://shields.kzu.io/vpre/Devlooped.Xml.Css/main&label=nuget.ci&color=brightgreen)](https://pkg.kzu.io/index.json)
[![Build](https://github.com/devlooped/css/workflows/build/badge.svg?branch=main)](https://github.com/devlooped/css/actions)

We also produce CI packages from branches and pull requests so you can dogfood builds as quickly as they are produced. 

The CI feed is `https://pkg.kzu.io/index.json`. 

The versioning scheme for packages is:

- PR builds: *42.42.42-pr*`[NUMBER]`
- Branch builds: *42.42.42-*`[BRANCH]`.`[COMMITS]`



## Sponsors

<h3 style="vertical-align: text-top" id="by-clarius">
<img src="https://raw.githubusercontent.com/devlooped/oss/main/assets/images/sponsors.svg" alt="sponsors" height="36" width="36" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">&nbsp;&nbsp;by&nbsp;<a href="https://github.com/clarius">@clarius</a>&nbsp;<img src="https://raw.githubusercontent.com/clarius/branding/main/logo/logo.svg" alt="sponsors" height="36" width="36" style="vertical-align: text-top; border: 0px; padding: 0px; margin: 0px">
</h3>

*[get mentioned here too](https://github.com/sponsors/devlooped)!*
