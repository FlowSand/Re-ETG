// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.Vector2Interpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation
{
  public class Vector2Interpolator : Interpolator<Vector2>
  {
    protected static Vector2Interpolator singleton;

    public override Vector2 Add(Vector2 lhs, Vector2 rhs) => lhs + rhs;

    public override Vector2 Interpolate(Vector2 startValue, Vector2 endValue, float time)
    {
      return startValue + (endValue - startValue) * time;
    }

    public static Interpolator<Vector2> Default
    {
      get
      {
        if (Vector2Interpolator.singleton == null)
          Vector2Interpolator.singleton = new Vector2Interpolator();
        return (Interpolator<Vector2>) Vector2Interpolator.singleton;
      }
    }
  }
}
