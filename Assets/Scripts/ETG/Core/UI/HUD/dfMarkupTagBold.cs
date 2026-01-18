// Decompiled with JetBrains decompiler
// Type: dfMarkupTagBold
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[dfMarkupTagInfo("b")]
[dfMarkupTagInfo("strong")]
public class dfMarkupTagBold : dfMarkupTag
  {
    public dfMarkupTagBold()
      : base("b")
    {
    }

    public dfMarkupTagBold(dfMarkupTag original)
      : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      style = this.applyTextStyleAttributes(style);
      if (style.FontStyle == FontStyle.Normal)
        style.FontStyle = FontStyle.Bold;
      else if (style.FontStyle == FontStyle.Italic)
        style.FontStyle = FontStyle.BoldAndItalic;
      base._PerformLayoutImpl(container, style);
    }
  }

