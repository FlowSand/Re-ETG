using UnityEngine;

#nullable disable

[dfMarkupTagInfo("font")]
public class dfMarkupTagFont : dfMarkupTag
  {
    public dfMarkupTagFont()
      : base("font")
    {
    }

    public dfMarkupTagFont(dfMarkupTag original)
      : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      dfMarkupAttribute attribute1 = this.findAttribute("name", "face");
      if (attribute1 != null)
        style.Font = dfDynamicFont.FindByName(attribute1.Value) ?? style.Font;
      dfMarkupAttribute attribute2 = this.findAttribute("size", "font-size");
      if (attribute2 != null)
        style.FontSize = dfMarkupStyle.ParseSize(attribute2.Value, style.FontSize);
      dfMarkupAttribute attribute3 = this.findAttribute("color");
      if (attribute3 != null)
      {
        style.Color = dfMarkupStyle.ParseColor(attribute3.Value, Color.red);
        style.Color.a = style.Opacity;
      }
      dfMarkupAttribute attribute4 = this.findAttribute(nameof (style));
      if (attribute4 != null)
        style.FontStyle = dfMarkupStyle.ParseFontStyle(attribute4.Value, style.FontStyle);
      base._PerformLayoutImpl(container, style);
    }
  }

