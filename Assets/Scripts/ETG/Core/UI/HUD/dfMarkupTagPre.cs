#nullable disable

[dfMarkupTagInfo("pre")]
public class dfMarkupTagPre : dfMarkupTag
  {
    public dfMarkupTagPre()
      : base("pre")
    {
    }

    public dfMarkupTagPre(dfMarkupTag original)
      : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      style = this.applyTextStyleAttributes(style);
      style.PreserveWhitespace = true;
      style.Preformatted = true;
      if (style.Align == dfMarkupTextAlign.Justify)
        style.Align = dfMarkupTextAlign.Left;
      dfMarkupBox dfMarkupBox;
      if ((double) style.BackgroundColor.a > 0.10000000149011612)
      {
        dfMarkupBoxSprite dfMarkupBoxSprite = new dfMarkupBoxSprite((dfMarkupElement) this, dfMarkupDisplayType.block, style);
        dfMarkupBoxSprite.LoadImage(this.Owner.Atlas, this.Owner.BlankTextureSprite);
        dfMarkupBoxSprite.Style.Color = style.BackgroundColor;
        dfMarkupBox = (dfMarkupBox) dfMarkupBoxSprite;
      }
      else
        dfMarkupBox = new dfMarkupBox((dfMarkupElement) this, dfMarkupDisplayType.block, style);
      dfMarkupAttribute attribute1 = this.findAttribute("margin");
      if (attribute1 != null)
        dfMarkupBox.Margins = dfMarkupBorders.Parse(attribute1.Value);
      dfMarkupAttribute attribute2 = this.findAttribute("padding");
      if (attribute2 != null)
        dfMarkupBox.Padding = dfMarkupBorders.Parse(attribute2.Value);
      container.AddChild(dfMarkupBox);
      base._PerformLayoutImpl(dfMarkupBox, style);
      dfMarkupBox.FitToContents();
    }
  }

