// Decompiled with JetBrains decompiler
// Type: dfMarkupParser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfMarkupParser
    {
      private static Regex TAG_PATTERN = (Regex) null;
      private static Regex ATTR_PATTERN = (Regex) null;
      private static Regex STYLE_PATTERN = (Regex) null;
      private static Dictionary<string, System.Type> tagTypes = (Dictionary<string, System.Type>) null;
      private static dfMarkupParser parserInstance = new dfMarkupParser();
      private dfRichTextLabel owner;

      static dfMarkupParser()
      {
        RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;
        dfMarkupParser.TAG_PATTERN = new Regex("(\\<\\/?)(?<tag>[a-zA-Z0-9$_]+)(\\s(?<attr>.+?))?([\\/]*\\>)", options);
        dfMarkupParser.ATTR_PATTERN = new Regex("(?<key>[a-zA-Z0-9$_]+)=(?<value>(\"((\\\\\")|\\\\\\\\|[^\"\\n])*\")|('((\\\\')|\\\\\\\\|[^'\\n])*')|\\d+|\\w+)", options);
        dfMarkupParser.STYLE_PATTERN = new Regex("(?<key>[a-zA-Z0-9\\-]+)(\\s*\\:\\s*)(?<value>[^;]+)", options);
      }

      public static dfList<dfMarkupElement> Parse(dfRichTextLabel owner, string source)
      {
        try
        {
          dfMarkupParser.parserInstance.owner = owner;
          return dfMarkupParser.parserInstance.parseMarkup(source);
        }
        finally
        {
        }
      }

      private dfList<dfMarkupElement> parseMarkup(string source)
      {
        Queue<dfMarkupElement> tokens = new Queue<dfMarkupElement>();
        MatchCollection matchCollection = dfMarkupParser.TAG_PATTERN.Matches(source);
        int startIndex = 0;
        for (int i = 0; i < matchCollection.Count; ++i)
        {
          Match tag = matchCollection[i];
          if (tag.Index > startIndex)
          {
            dfMarkupString dfMarkupString = new dfMarkupString(source.Substring(startIndex, tag.Index - startIndex));
            tokens.Enqueue((dfMarkupElement) dfMarkupString);
          }
          startIndex = tag.Index + tag.Length;
          tokens.Enqueue(this.parseTag(tag));
        }
        if (startIndex < source.Length)
        {
          dfMarkupString dfMarkupString = new dfMarkupString(source.Substring(startIndex));
          tokens.Enqueue((dfMarkupElement) dfMarkupString);
        }
        return this.processTokens(tokens);
      }

      private dfList<dfMarkupElement> processTokens(Queue<dfMarkupElement> tokens)
      {
        dfList<dfMarkupElement> dfList = dfList<dfMarkupElement>.Obtain();
        while (tokens.Count > 0)
          dfList.Add(this.parseElement(tokens));
        for (int index = 0; index < dfList.Count; ++index)
        {
          if (dfList[index] is dfMarkupTag)
            ((dfMarkupTag) dfList[index]).Owner = this.owner;
        }
        return dfList;
      }

      private dfMarkupElement parseElement(Queue<dfMarkupElement> tokens)
      {
        dfMarkupElement dfMarkupElement = tokens.Dequeue();
        if (dfMarkupElement is dfMarkupString)
          return ((dfMarkupString) dfMarkupElement).SplitWords();
        dfMarkupTag original = (dfMarkupTag) dfMarkupElement;
        if (original.IsClosedTag || original.IsEndTag)
          return (dfMarkupElement) this.refineTag(original);
        while (tokens.Count > 0)
        {
          dfMarkupElement element = this.parseElement(tokens);
          if (element is dfMarkupTag)
          {
            dfMarkupTag dfMarkupTag = (dfMarkupTag) element;
            if (dfMarkupTag.IsEndTag)
            {
              if (!(dfMarkupTag.TagName == original.TagName))
                return (dfMarkupElement) this.refineTag(original);
              break;
            }
          }
          original.AddChildNode(element);
        }
        return (dfMarkupElement) this.refineTag(original);
      }

      private dfMarkupTag refineTag(dfMarkupTag original)
      {
        if (original.IsEndTag)
          return original;
        if (dfMarkupParser.tagTypes == null)
        {
          dfMarkupParser.tagTypes = new Dictionary<string, System.Type>();
          foreach (System.Type assemblyType in this.getAssemblyTypes())
          {
            if (typeof (dfMarkupTag).IsAssignableFrom(assemblyType))
            {
              object[] customAttributes = assemblyType.GetCustomAttributes(typeof (dfMarkupTagInfoAttribute), true);
              if (customAttributes != null && customAttributes.Length != 0)
              {
                for (int index = 0; index < customAttributes.Length; ++index)
                {
                  string tagName = ((dfMarkupTagInfoAttribute) customAttributes[index]).TagName;
                  dfMarkupParser.tagTypes[tagName] = assemblyType;
                }
              }
            }
          }
        }
        if (!dfMarkupParser.tagTypes.ContainsKey(original.TagName))
          return original;
        return (dfMarkupTag) Activator.CreateInstance(dfMarkupParser.tagTypes[original.TagName], (object) original);
      }

      private System.Type[] getAssemblyTypes() => Assembly.GetExecutingAssembly().GetExportedTypes();

      private dfMarkupElement parseTag(Match tag)
      {
        string lowerInvariant = tag.Groups[nameof (tag)].Value.ToLowerInvariant();
        if (tag.Value.StartsWith("</"))
          return (dfMarkupElement) new dfMarkupTag(lowerInvariant)
          {
            IsEndTag = true
          };
        dfMarkupTag element = new dfMarkupTag(lowerInvariant);
        string input = tag.Groups["attr"].Value;
        MatchCollection matchCollection = dfMarkupParser.ATTR_PATTERN.Matches(input);
        for (int i = 0; i < matchCollection.Count; ++i)
        {
          Match match = matchCollection[i];
          string name = match.Groups["key"].Value;
          string text = dfMarkupEntity.Replace(match.Groups["value"].Value);
          if (text.StartsWith("\""))
            text = text.Trim('"');
          else if (text.StartsWith("'"))
            text = text.Trim('\'');
          if (!string.IsNullOrEmpty(text))
          {
            if (name == "style")
              this.parseStyleAttribute(element, text);
            else
              element.Attributes.Add(new dfMarkupAttribute(name, text));
          }
        }
        if (tag.Value.EndsWith("/>") || lowerInvariant == "br" || lowerInvariant == "img")
          element.IsClosedTag = true;
        return (dfMarkupElement) element;
      }

      private void parseStyleAttribute(dfMarkupTag element, string text)
      {
        MatchCollection matchCollection = dfMarkupParser.STYLE_PATTERN.Matches(text);
        for (int i = 0; i < matchCollection.Count; ++i)
        {
          Match match = matchCollection[i];
          string lowerInvariant = match.Groups["key"].Value.ToLowerInvariant();
          string str = match.Groups["value"].Value;
          element.Attributes.Add(new dfMarkupAttribute(lowerInvariant, str));
        }
      }
    }

}
