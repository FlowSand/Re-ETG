// Decompiled with JetBrains decompiler
// Type: dfMarkupTagParagraph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

[dfMarkupTagInfo("p")]
public class dfMarkupTagParagraph : dfMarkupTag
  {
    public dfMarkupTagParagraph()
      : base("p")
    {
    }

    public dfMarkupTagParagraph(dfMarkupTag original)
      : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      if (this.ChildNodes.Count == 0)
        return;
      style = this.applyTextStyleAttributes(style);
      int lineHeight = container.Children.Count != 0 ? style.LineHeight : 0;
      dfMarkupBox dfMarkupBox;
      if ((double) style.BackgroundColor.a > 0.004999999888241291)
      {
        dfMarkupBoxSprite dfMarkupBoxSprite = new dfMarkupBoxSprite((dfMarkupElement) this, dfMarkupDisplayType.block, style);
        dfMarkupBoxSprite.Atlas = this.Owner.Atlas;
        dfMarkupBoxSprite.Source = this.Owner.BlankTextureSprite;
        dfMarkupBoxSprite.Style.Color = style.BackgroundColor;
        dfMarkupBox = (dfMarkupBox) dfMarkupBoxSprite;
      }
      else
        dfMarkupBox = new dfMarkupBox((dfMarkupElement) this, dfMarkupDisplayType.block, style);
      dfMarkupBox.Margins = new dfMarkupBorders(0, 0, lineHeight, style.LineHeight);
      dfMarkupAttribute attribute1 = this.findAttribute("margin");
      if (attribute1 != null)
        dfMarkupBox.Margins = dfMarkupBorders.Parse(attribute1.Value);
      dfMarkupAttribute attribute2 = this.findAttribute("padding");
      if (attribute2 != null)
        dfMarkupBox.Padding = dfMarkupBorders.Parse(attribute2.Value);
      container.AddChild(dfMarkupBox);
      base._PerformLayoutImpl(dfMarkupBox, style);
      if (dfMarkupBox.Children.Count > 0)
        dfMarkupBox.Children[dfMarkupBox.Children.Count - 1].IsNewline = true;
      dfMarkupBox.FitToContents(true);
    }
  }

