// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.IntInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation;

public class IntInterpolator : Interpolator<int>
{
  protected static IntInterpolator singleton;

  public override int Add(int lhs, int rhs) => lhs + rhs;

  public override int Interpolate(int startValue, int endValue, float time)
  {
    return Mathf.RoundToInt((float) startValue + (float) (endValue - startValue) * time);
  }

  public static Interpolator<int> Default
  {
    get
    {
      if (IntInterpolator.singleton == null)
        IntInterpolator.singleton = new IntInterpolator();
      return (Interpolator<int>) IntInterpolator.singleton;
    }
  }
}
