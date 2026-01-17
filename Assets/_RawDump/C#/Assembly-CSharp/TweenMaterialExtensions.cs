// Decompiled with JetBrains decompiler
// Type: TweenMaterialExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween;
using UnityEngine;

#nullable disable
public static class TweenMaterialExtensions
{
  public static DaikonForge.Tween.Tween<Color> TweenColor(this Material material)
  {
    return DaikonForge.Tween.Tween<Color>.Obtain().SetStartValue(material.color).SetEndValue(material.color).SetDuration(1f).OnExecute((TweenAssignmentCallback<Color>) (currentValue => material.color = currentValue));
  }

  public static DaikonForge.Tween.Tween<float> TweenAlpha(this Material material)
  {
    return DaikonForge.Tween.Tween<float>.Obtain().SetStartValue(material.color.a).SetEndValue(material.color.a).SetDuration(1f).OnExecute((TweenAssignmentCallback<float>) (currentValue =>
    {
      Color color = material.color with { a = currentValue };
      material.color = color;
    }));
  }
}
