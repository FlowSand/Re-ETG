// Decompiled with JetBrains decompiler
// Type: dfMarkupTagAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.UI.HUD
{
    [dfMarkupTagInfo("a")]
    public class dfMarkupTagAnchor : dfMarkupTag
    {
      public dfMarkupTagAnchor()
        : base("a")
      {
      }

      public dfMarkupTagAnchor(dfMarkupTag original)
        : base(original)
      {
      }

      public string HRef
      {
        get
        {
          dfMarkupAttribute attribute = this.findAttribute("href");
          return attribute != null ? attribute.Value : string.Empty;
        }
      }

      protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
      {
        style.TextDecoration = dfMarkupTextDecoration.Underline;
        style = this.applyTextStyleAttributes(style);
        for (int index = 0; index < this.ChildNodes.Count; ++index)
        {
          dfMarkupElement childNode = this.ChildNodes[index];
          if (childNode is dfMarkupString && (childNode as dfMarkupString).Text == "\n")
          {
            if (style.PreserveWhitespace)
              container.AddLineBreak();
          }
          else
            childNode.PerformLayout(container, style);
        }
      }
    }

}
