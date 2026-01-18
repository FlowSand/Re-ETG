// Decompiled with JetBrains decompiler
// Type: dfMarkupTagList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[dfMarkupTagInfo("ul")]
[dfMarkupTagInfo("ol")]
public class dfMarkupTagList : dfMarkupTag
  {
    public dfMarkupTagList()
      : base("ul")
    {
    }

    public dfMarkupTagList(dfMarkupTag original)
      : base(original)
    {
    }

    internal int BulletWidth { get; private set; }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      if (this.ChildNodes.Count == 0)
        return;
      style = this.applyTextStyleAttributes(style);
      style.Align = dfMarkupTextAlign.Left;
      dfMarkupBox dfMarkupBox = new dfMarkupBox((dfMarkupElement) this, dfMarkupDisplayType.block, style);
      container.AddChild(dfMarkupBox);
      this.calculateBulletWidth(style);
      for (int index = 0; index < this.ChildNodes.Count; ++index)
      {
        if (this.ChildNodes[index] is dfMarkupTag childNode && !(childNode.TagName != "li"))
          childNode.PerformLayout(dfMarkupBox, style);
      }
      dfMarkupBox.FitToContents();
    }

    private void calculateBulletWidth(dfMarkupStyle style)
    {
      if (this.TagName == "ul")
      {
        this.BulletWidth = Mathf.CeilToInt(style.Font.MeasureText("•", style.FontSize, style.FontStyle).x);
      }
      else
      {
        int num = 0;
        for (int index = 0; index < this.ChildNodes.Count; ++index)
        {
          if (this.ChildNodes[index] is dfMarkupTag childNode && childNode.TagName == "li")
            ++num;
        }
        string text = new string('X', num.ToString().Length) + ".";
        this.BulletWidth = Mathf.CeilToInt(style.Font.MeasureText(text, style.FontSize, style.FontStyle).x);
      }
    }
  }

