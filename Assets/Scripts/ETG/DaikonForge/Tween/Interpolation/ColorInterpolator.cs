// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.ColorInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation
{
  public class ColorInterpolator : Interpolator<Color>
  {
    protected static ColorInterpolator singleton;

    public override Color Add(Color lhs, Color rhs) => lhs + rhs;

    public override Color Interpolate(Color startValue, Color endValue, float time)
    {
      return Color.Lerp(startValue, endValue, time);
    }

    public static Interpolator<Color> Default
    {
      get
      {
        if (ColorInterpolator.singleton == null)
          ColorInterpolator.singleton = new ColorInterpolator();
        return (Interpolator<Color>) ColorInterpolator.singleton;
      }
    }
  }
}
