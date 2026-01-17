// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.Vector3Interpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation;

public class Vector3Interpolator : Interpolator<Vector3>
{
  protected static Vector3Interpolator singleton;

  public override Vector3 Add(Vector3 lhs, Vector3 rhs) => lhs + rhs;

  public override Vector3 Interpolate(Vector3 startValue, Vector3 endValue, float time)
  {
    return startValue + (endValue - startValue) * time;
  }

  public static Interpolator<Vector3> Default
  {
    get
    {
      if (Vector3Interpolator.singleton == null)
        Vector3Interpolator.singleton = new Vector3Interpolator();
      return (Interpolator<Vector3>) Vector3Interpolator.singleton;
    }
  }
}
