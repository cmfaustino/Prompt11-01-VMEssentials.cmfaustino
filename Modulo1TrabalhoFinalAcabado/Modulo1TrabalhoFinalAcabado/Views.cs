using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WebReflector
{
    public struct TagHTML
    {
        public string BeginTag { get; set; }
        public Dictionary<string, string> AttributesValues { get; set; }
        public List<TagHTML> NestedTagsHTML { get; set; }
        public string TextContent { get; set; }
        public string EndTag { get; set; }
    }

    static public class Views
    {
        public const string ENDING_BEGIN_TAG = ">";

        #region Metodos de Processamento de Tags

        static public string processTagHTML2string(TagHTML tagHtml)
        {
            var result_sb = new StringBuilder();
            if (!string.IsNullOrEmpty(tagHtml.BeginTag))
            {
                result_sb.Append(tagHtml.BeginTag);
                if (tagHtml.AttributesValues != null)
                {
                    foreach (var AttribVal in tagHtml.AttributesValues)
                    {
                        if ((!string.IsNullOrEmpty(AttribVal.Key)) && (!string.IsNullOrEmpty(AttribVal.Value)))
                        {
                            //result_sb.AppendFormat(" \"{0}\"=\"{1}\"", WebUtility.HtmlEncode(AttribVal.Key), WebUtility.HtmlEncode(AttribVal.Value));
                            result_sb.AppendFormat(" {0}=\"{1}\"", WebUtility.HtmlEncode(AttribVal.Key), WebUtility.HtmlEncode(AttribVal.Value));
                        }
                    }
                }
                result_sb.Append(ENDING_BEGIN_TAG);
            }
            if (tagHtml.NestedTagsHTML != null)
            {
                foreach (var nestedTagHtml in tagHtml.NestedTagsHTML)
                    result_sb.Append(processTagHTML2string(nestedTagHtml));
            }
            if (!string.IsNullOrEmpty(tagHtml.TextContent))
            {
                result_sb.Append(WebUtility.HtmlEncode(tagHtml.TextContent));
            }
            if ((!string.IsNullOrEmpty(tagHtml.BeginTag)) && (!string.IsNullOrEmpty(tagHtml.EndTag)))
            {
                result_sb.Append(tagHtml.EndTag);
            }
            return result_sb.ToString();
        }

        static public string processTagHTML2string(params TagHTML[] nested)
        {
            var result_sb = new StringBuilder();
            if ((nested != null) && (nested.Length > 0))
            {
                foreach (var tagHtml in nested)
                {
                    result_sb.Append(processTagHTML2string(tagHtml));
                }
            }
            return result_sb.ToString();
        }


        #endregion

        #region Metodo de Tag de texto simples

        static public TagHTML Text2TagHTML(string text)
        {
            var thml = new TagHTML();
            thml.BeginTag = "";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
            thml.TextContent = text ?? "";
            thml.EndTag = "";
            return thml;
        }

        #endregion

        #region Metodos de Tags de Topo - HTML, HEAD, TITLE de texto simples, BODY

        static public TagHTML HTML(TagHTML head, TagHTML body)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<HTML";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(head);
                    thml.NestedTagsHTML.Add(body);
            thml.TextContent = "";
            thml.EndTag = "</HTML>";
            return thml;
        }

        static public TagHTML HEAD(TagHTML title)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<HEAD";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(title);
            thml.TextContent = "";
            thml.EndTag = "</HEAD>";
            return thml;
        }

        static public TagHTML TITLE(string title)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<TITLE";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
            thml.TextContent = title ?? "";
            thml.EndTag = "</TITLE>";
            return thml;
        }

        static public TagHTML BODY(TagHTML nestedtagHtml)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<BODY";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(nestedtagHtml);
            thml.TextContent = "";
            thml.EndTag = "</BODY>";
            return thml;
        }

        static public TagHTML BODY(params TagHTML[] nested)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<BODY";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
            if ((nested!=null)&&(nested.Length > 0))
            {
                foreach (var tagHtml in nested)
                {
                    thml.NestedTagsHTML.Add(tagHtml);
                }
            }
            thml.TextContent = "";
            thml.EndTag = "</BODY>";
            return thml;
        }

        #endregion

        #region Metodos de Tags de Heading - H1 e H3 apenas, de texto simples

        static public TagHTML H1(string textHeading) //podia-se acrescentar como parametros 2 string[], um para atributos e outro para valores, por ex.
        {
            var thml = new TagHTML();
            thml.BeginTag = "<H1";
            thml.AttributesValues = new Dictionary<string, string>();
                    thml.AttributesValues.Add("align","center");
                thml.NestedTagsHTML = new List<TagHTML>();
            thml.TextContent = textHeading ?? "";
            thml.EndTag = "</H1>";
            return thml;
        }

        static public TagHTML H3(string textHeading)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<H3";
            thml.AttributesValues = new Dictionary<string, string>();
                    thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
            thml.TextContent = textHeading ?? "";
            thml.EndTag = "</H3>";
            return thml;
        }

        #endregion

        #region Metodos de Tag de Paragrafo - P apenas

        static public TagHTML P(string text)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<P";
            thml.AttributesValues = new Dictionary<string, string>();
                    thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
            thml.TextContent = text ?? "";
            thml.EndTag = "</P>";
            return thml;
        }

        static public TagHTML P(TagHTML nestedtagHtml)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<P";
            thml.AttributesValues = new Dictionary<string, string>();
                    thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(nestedtagHtml);
            thml.TextContent = "";
            thml.EndTag = "</P>";
            return thml;
        }

        static public TagHTML P(params TagHTML[] nested)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<P";
            thml.AttributesValues = new Dictionary<string, string>();
                    thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
            if ((nested != null) && (nested.Length > 0))
            {
                foreach (var tagHtml in nested)
                {
                    thml.NestedTagsHTML.Add(tagHtml);
                }
            }
            thml.TextContent = "";
            thml.EndTag = "</P>";
            return thml;
        }

        #endregion

        #region Metodo de Tag de texto PRE

        static public TagHTML PRE(string text)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<PRE";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
            thml.TextContent = text ?? "";
            thml.EndTag = "</PRE>";
            return thml;
        }

        static public TagHTML PRE(TagHTML nestedtagHtml)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<PRE";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(nestedtagHtml);
            thml.TextContent = "";
            thml.EndTag = "</PRE>";
            return thml;
        }

        #endregion

        #region Metodos de Tags de Tabelas - TABLE, TR, TD

        static public TagHTML TABLE(TagHTML nestedtagHtml)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<TABLE";
            thml.AttributesValues = new Dictionary<string, string>();
                    thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(nestedtagHtml);
            thml.TextContent = "";
            thml.EndTag = "</TABLE>";
            return thml;
        }

        static public TagHTML TABLE(params TagHTML[] nested)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<TABLE";
            thml.AttributesValues = new Dictionary<string, string>();
                    thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
            if ((nested!=null)&&(nested.Length > 0))
            {
                foreach (var tagHtml in nested)
                {
                    thml.NestedTagsHTML.Add(tagHtml);
                }
            }
            thml.TextContent = "";
            thml.EndTag = "</TABLE>";
            return thml;
        }

        static public TagHTML TR(TagHTML nestedtagHtml)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<TR";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(nestedtagHtml);
            thml.TextContent = "";
            thml.EndTag = "</TR>";
            return thml;
        }

        static public TagHTML TR(params TagHTML[] nested)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<TR";
            thml.AttributesValues = new Dictionary<string, string>();
                thml.NestedTagsHTML = new List<TagHTML>();
            if ((nested != null) && (nested.Length > 0))
            {
                foreach (var tagHtml in nested)
                {
                    thml.NestedTagsHTML.Add(tagHtml);
                }
            }
            thml.TextContent = "";
            thml.EndTag = "</TR>";
            return thml;
        }

        static public TagHTML TD(TagHTML nestedtagHtml)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<TD";
            thml.AttributesValues = new Dictionary<string, string>();
                    //thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(nestedtagHtml);
            thml.TextContent = "";
            thml.EndTag = "</TD>";
            return thml;
        }

        static public TagHTML TD(params TagHTML[] nested)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<TD";
            thml.AttributesValues = new Dictionary<string, string>();
                    //thml.AttributesValues.Add("align", "center");
                thml.NestedTagsHTML = new List<TagHTML>();
            if ((nested != null) && (nested.Length > 0))
            {
                foreach (var tagHtml in nested)
                {
                    thml.NestedTagsHTML.Add(tagHtml);
                }
            }
            thml.TextContent = "";
            thml.EndTag = "</TD>";
            return thml;
        }

        #endregion

        #region Metodos de Tag de Hiperligacoes, incluindo ancoras - A

        static public TagHTML A(bool isLinkAndNotAnchorName, string linkOrAnchorName, TagHTML nestedtagHtml)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<A";
            thml.AttributesValues = new Dictionary<string, string>();
                if (!isLinkAndNotAnchorName)
                {
                    thml.AttributesValues.Add("name",(linkOrAnchorName ?? ""));
                }
                else
                {
                    thml.AttributesValues.Add("href",(linkOrAnchorName ?? ""));
                }
                thml.NestedTagsHTML = new List<TagHTML>();
                    thml.NestedTagsHTML.Add(nestedtagHtml);
            thml.TextContent = "";
            thml.EndTag = "</A>";
            return thml;
        }

        static public TagHTML A(bool isLinkAndNotAnchorName, string linkOrAnchorName, params TagHTML[] nested)
        {
            var thml = new TagHTML();
            thml.BeginTag = "<A";
            thml.AttributesValues = new Dictionary<string, string>();
                if (!isLinkAndNotAnchorName)
                {
                    thml.AttributesValues.Add("name", (linkOrAnchorName ?? ""));
                }
                else
                {
                    thml.AttributesValues.Add("href", (linkOrAnchorName ?? ""));
                }
                thml.NestedTagsHTML = new List<TagHTML>();
            if ((nested != null) && (nested.Length > 0))
            {
                foreach (var tagHtml in nested)
                {
                    thml.NestedTagsHTML.Add(tagHtml);
                }
            }
            thml.TextContent = "";
            thml.EndTag = "</A>";
            return thml;
        }

        #endregion
    }
}
