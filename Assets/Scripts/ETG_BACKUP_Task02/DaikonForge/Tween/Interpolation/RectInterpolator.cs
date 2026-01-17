// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.RectInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation;

public class RectInterpolator : Interpolator<Rect>
{
  protected static RectInterpolator singleton;

  public override Rect Add(Rect lhs, Rect rhs)
  {
    return new Rect(lhs.xMin + rhs.xMin, lhs.yMin + rhs.yMin, lhs.width + rhs.width, lhs.height + rhs.height);
  }

  public override Rect Interpolate(Rect startValue, Rect endValue, float time)
  {
    return new Rect(startValue.xMin + (endValue.xMin - startValue.xMin) * time, startValue.yMin + (endValue.yMin - startValue.yMin) * time, startValue.width + (endValue.width - startValue.width) * time, startValue.height + (endValue.height - startValue.height) * time);
  }

  public static Interpolator<Rect> Default
  {
    get
    {
      if (RectInterpolator.singleton == null)
        RectInterpolator.singleton = new RectInterpolator();
      return (Interpolator<Rect>) RectInterpolator.singleton;
    }
  }
}
