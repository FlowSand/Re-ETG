using System.Collections.Generic;
using System.Text;
using UnityEngine;

#nullable disable

public class dfMarkupTag : dfMarkupElement
  {
    private static int ELEMENTID;
    public List<dfMarkupAttribute> Attributes;
    private dfRichTextLabel owner;
    private string id;

    public dfMarkupTag(string tagName)
    {
      this.Attributes = new List<dfMarkupAttribute>();
      this.TagName = tagName;
      this.id = tagName + dfMarkupTag.ELEMENTID++.ToString("X");
    }

    public dfMarkupTag(dfMarkupTag original)
    {
      this.TagName = original.TagName;
      this.Attributes = original.Attributes;
      this.IsEndTag = original.IsEndTag;
      this.IsClosedTag = original.IsClosedTag;
      this.IsInline = original.IsInline;
      this.id = original.id;
      List<dfMarkupElement> childNodes = original.ChildNodes;
      for (int index = 0; index < childNodes.Count; ++index)
        this.AddChildNode(childNodes[index]);
    }

    public string TagName { get; set; }

    public string ID => this.id;

    public virtual bool IsEndTag { get; set; }

    public virtual bool IsClosedTag { get; set; }

    public virtual bool IsInline { get; set; }

    public dfRichTextLabel Owner
    {
      get => this.owner;
      set
      {
        this.owner = value;
        for (int index = 0; index < this.ChildNodes.Count; ++index)
        {
          if (this.ChildNodes[index] is dfMarkupTag childNode)
            childNode.Owner = value;
        }
      }
    }

    internal override void Release() => base.Release();

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      if (this.IsEndTag)
        return;
      if (this.findAttribute("margin") == null)
        ;
      for (int index = 0; index < this.ChildNodes.Count; ++index)
        this.ChildNodes[index].PerformLayout(container, style);
    }

    protected dfMarkupStyle applyTextStyleAttributes(dfMarkupStyle style)
    {
      dfMarkupAttribute attribute1 = this.findAttribute("font", "font-family");
      if (attribute1 != null)
        style.Font = dfDynamicFont.FindByName(attribute1.Value) ?? this.owner.Font;
      dfMarkupAttribute attribute2 = this.findAttribute(nameof (style), "font-style");
      if (attribute2 != null)
        style.FontStyle = dfMarkupStyle.ParseFontStyle(attribute2.Value, style.FontStyle);
      dfMarkupAttribute attribute3 = this.findAttribute("size", "font-size");
      if (attribute3 != null)
        style.FontSize = dfMarkupStyle.ParseSize(attribute3.Value, style.FontSize);
      dfMarkupAttribute attribute4 = this.findAttribute("color");
      if (attribute4 != null)
      {
        Color color = dfMarkupStyle.ParseColor(attribute4.Value, style.Color) with
        {
          a = style.Opacity
        };
        style.Color = color;
      }
      dfMarkupAttribute attribute5 = this.findAttribute("align", "text-align");
      if (attribute5 != null)
        style.Align = dfMarkupStyle.ParseTextAlignment(attribute5.Value);
      dfMarkupAttribute attribute6 = this.findAttribute("valign", "vertical-align");
      if (attribute6 != null)
        style.VerticalAlign = dfMarkupStyle.ParseVerticalAlignment(attribute6.Value);
      dfMarkupAttribute attribute7 = this.findAttribute("line-height");
      if (attribute7 != null)
        style.LineHeight = dfMarkupStyle.ParseSize(attribute7.Value, style.LineHeight);
      dfMarkupAttribute attribute8 = this.findAttribute("text-decoration");
      if (attribute8 != null)
        style.TextDecoration = dfMarkupStyle.ParseTextDecoration(attribute8.Value);
      dfMarkupAttribute attribute9 = this.findAttribute("background", "background-color");
      if (attribute9 != null)
      {
        style.BackgroundColor = dfMarkupStyle.ParseColor(attribute9.Value, Color.clear);
        style.BackgroundColor.a = style.Opacity;
      }
      return style;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      if (this.IsEndTag)
        stringBuilder.Append("/");
      stringBuilder.Append(this.TagName);
      for (int index = 0; index < this.Attributes.Count; ++index)
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(this.Attributes[index].ToString());
      }
      if (this.IsClosedTag)
        stringBuilder.Append("/");
      stringBuilder.Append("]");
      if (!this.IsClosedTag)
      {
        for (int index = 0; index < this.ChildNodes.Count; ++index)
          stringBuilder.Append(this.ChildNodes[index].ToString());
        stringBuilder.Append("[/");
        stringBuilder.Append(this.TagName);
        stringBuilder.Append("]");
      }
      return stringBuilder.ToString();
    }

    protected dfMarkupAttribute findAttribute(params string[] names)
    {
      for (int index1 = 0; index1 < this.Attributes.Count; ++index1)
      {
        for (int index2 = 0; index2 < names.Length; ++index2)
        {
          if (this.Attributes[index1].Name == names[index2])
            return this.Attributes[index1];
        }
      }
      return (dfMarkupAttribute) null;
    }
  }

