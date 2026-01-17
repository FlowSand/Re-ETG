// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.Vector4Interpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation
{
  public class Vector4Interpolator : Interpolator<Vector4>
  {
    protected static Vector4Interpolator singleton;

    public override Vector4 Add(Vector4 lhs, Vector4 rhs) => lhs + rhs;

    public override Vector4 Interpolate(Vector4 startValue, Vector4 endValue, float time)
    {
      return startValue + endValue * time;
    }

    public static Interpolator<Vector4> Default
    {
      get
      {
        if (Vector4Interpolator.singleton == null)
          Vector4Interpolator.singleton = new Vector4Interpolator();
        return (Interpolator<Vector4>) Vector4Interpolator.singleton;
      }
    }
  }
}
