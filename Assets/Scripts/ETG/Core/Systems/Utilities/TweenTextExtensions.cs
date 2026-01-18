// Decompiled with JetBrains decompiler
// Type: TweenTextExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween;
using UnityEngine;

#nullable disable

  public static class TweenTextExtensions
  {
    public static DaikonForge.Tween.Tween<float> TweenAlpha(this TextMesh text)
    {
      return DaikonForge.Tween.Tween<float>.Obtain().SetStartValue(text.color.a).SetEndValue(text.color.a).SetDuration(1f).OnExecute((TweenAssignmentCallback<float>) (currentValue =>
      {
        Color color = text.color;
        color.a = currentValue;
        text.color = color;
      }));
    }

    public static DaikonForge.Tween.Tween<Color> TweenColor(this TextMesh text)
    {
      return DaikonForge.Tween.Tween<Color>.Obtain().SetStartValue(text.color).SetEndValue(text.color).SetDuration(1f).OnExecute((TweenAssignmentCallback<Color>) (currentValue => text.color = currentValue));
    }
  }

