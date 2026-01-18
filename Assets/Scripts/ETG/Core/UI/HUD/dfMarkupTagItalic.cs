// Decompiled with JetBrains decompiler
// Type: dfMarkupTagItalic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[dfMarkupTagInfo("i")]
[dfMarkupTagInfo("em")]
public class dfMarkupTagItalic : dfMarkupTag
  {
    public dfMarkupTagItalic()
      : base("i")
    {
    }

    public dfMarkupTagItalic(dfMarkupTag original)
      : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
      style = this.applyTextStyleAttributes(style);
      if (style.FontStyle == FontStyle.Normal)
        style.FontStyle = FontStyle.Italic;
      else if (style.FontStyle == FontStyle.Bold)
        style.FontStyle = FontStyle.BoldAndItalic;
      base._PerformLayoutImpl(container, style);
    }
  }

