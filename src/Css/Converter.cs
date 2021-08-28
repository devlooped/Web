// Partially ported from https://github.com/css2xpath/css2xpath/blob/master/index.js 
// See also https://ghostinspector.com/docs/css-xpath-conversion/#classes
using System;
using System.Text;

namespace Devlooped.Xml.Css;

class Converter
{
    public static string CssToXPath(Selector selector) => new Converter().FromCss(selector);

    string FromCss(Selector selector)
    {
        var expression = new StringBuilder();
        foreach (var step in selector)
        {
            var axis = step.Combinator switch
            {
                Combinator.None => ".//",
                Combinator.Descendant => "//",
                Combinator.Child => "/child::*[1]/self::",
                Combinator.NextSibling => "/following-sibling::*[1]/self::",
                Combinator.SubsequentSibling => "/following-sibling::",
                _ => throw new NotSupportedException(),
            };

            expression.Append(axis);

            foreach (var simple in step.SelectorSequence)
            {
                switch (simple)
                {
                    case UniversalSelector:
                        expression.Append("*");
                        break;
                    case TypeSelector type:
                        expression.Append(type.Name);
                        break;
                    case ClassSelector cls:
                        // See https://ghostinspector.com/docs/css-xpath-conversion/#classes
                        // '[contains(concat(" ",normalize-space(@class)," ")," val ")]'
                        expression.Append($"[contains(concat(\" \",normalize-space(@class),\" \"),\" {cls.Name} \")]");
                        break;
                    case IdSelector id:
                        expression.Append($"[@id=\"{id.Id}\"]");
                        break;
                    case AttributeSelector attr:
                        expression.Append('[');

                        if (attr.Value == null)
                        {
                            expression.Append('@').Append(attr.Name);
                        }
                        else if (attr.Matching != null)
                        {
                            switch (attr.Matching)
                            {
                                case ValueMatching.Equals:
                                    expression.Append($"@{attr.Name}=\"{attr.Value}\"");
                                    break;
                                case ValueMatching.Includes:
                                    // [contains(concat(" ",normalize-space(@attr)," "),concat(" ","val"," "))]
                                    expression.Append($"contains(concat(\" \",normalize-space(@{attr.Name}),\" \"),concat(\" \",\"{attr.Value}\",\" \"))");
                                    break;
                                case ValueMatching.Dash:
                                    // [@attr="val" or starts-with(@attr,concat("val","-"))]'
                                    expression.Append($"@{attr.Name}=\"{attr.Value}\" or starts-with(@{attr.Name},concat(\"{attr.Value}\",\"-\"))");
                                    break;
                                case ValueMatching.Prefix:
                                    // [starts-with(@attr,"value")]
                                    expression.Append($"starts-with(@{attr.Name},\"{attr.Value}\")");
                                    break;
                                case ValueMatching.Suffix:
                                    // [substring(@attr,string-length(@attr)-(string-length("val")-1))="val"]
                                    expression.Append($"substring(@{attr.Name},string-length(@{attr.Name})-(string-length(\"{attr.Value}\")-1))=\"{attr.Value}\"");
                                    break;
                                case ValueMatching.Substring:
                                    // [contains(@attr,"val")]
                                    expression.Append($"contains(@{attr.Name},\"{attr.Value}\")");
                                    break;
                                default:
                                    throw new NotSupportedException();
                            }
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                        expression.Append(']');
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        return expression.ToString();
    }
}
