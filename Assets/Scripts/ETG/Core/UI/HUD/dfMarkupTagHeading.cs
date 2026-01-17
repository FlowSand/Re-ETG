// Decompiled with JetBrains decompiler
// Type: dfMarkupTagHeading
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [dfMarkupTagInfo("h3")]
    [dfMarkupTagInfo("h2")]
    [dfMarkupTagInfo("h1")]
    [dfMarkupTagInfo("h6")]
    [dfMarkupTagInfo("h5")]
    [dfMarkupTagInfo("h4")]
    public class dfMarkupTagHeading : dfMarkupTag
    {
      public dfMarkupTagHeading()
        : base("h1")
      {
      }

      public dfMarkupTagHeading(dfMarkupTag original)
        : base(original)
      {
      }

      protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
      {
        dfMarkupBorders margins = new dfMarkupBorders();
        dfMarkupStyle style1 = this.applyTextStyleAttributes(this.applyDefaultStyles(style, ref margins));
        dfMarkupAttribute attribute = this.findAttribute("margin");
        if (attribute != null)
          margins = dfMarkupBorders.Parse(attribute.Value);
        dfMarkupBox dfMarkupBox = new dfMarkupBox((dfMarkupElement) this, dfMarkupDisplayType.block, style1);
        dfMarkupBox.Margins = margins;
        container.AddChild(dfMarkupBox);
        base._PerformLayoutImpl(dfMarkupBox, style1);
        dfMarkupBox.FitToContents();
      }

      private dfMarkupStyle applyDefaultStyles(dfMarkupStyle style, ref dfMarkupBorders margins)
      {
        float num1 = 1f;
        float num2 = 1f;
        switch (this.TagName)
        {
          case "h1":
            num2 = 2f;
            num1 = 0.65f;
            break;
          case "h2":
            num2 = 1.5f;
            num1 = 0.75f;
            break;
          case "h3":
            num2 = 1.35f;
            num1 = 0.85f;
            break;
          case "h4":
            num2 = 1.15f;
            num1 = 0.0f;
            break;
          case "h5":
            num2 = 0.85f;
            num1 = 1.5f;
            break;
          case "h6":
            num2 = 0.75f;
            num1 = 1.75f;
            break;
        }
        style.FontSize = (int) ((double) style.FontSize * (double) num2);
        style.FontStyle = FontStyle.Bold;
        style.Align = dfMarkupTextAlign.Left;
        float num3 = num1 * (float) style.FontSize;
        margins.top = margins.bottom = (int) num3;
        return style;
      }
    }

}
