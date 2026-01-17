// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.EulerInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation
{
  public class EulerInterpolator : Interpolator<Vector3>
  {
    protected static EulerInterpolator singleton;

    public static Interpolator<Vector3> Default
    {
      get
      {
        if (EulerInterpolator.singleton == null)
          EulerInterpolator.singleton = new EulerInterpolator();
        return (Interpolator<Vector3>) EulerInterpolator.singleton;
      }
    }

    public override Vector3 Add(Vector3 lhs, Vector3 rhs) => lhs + rhs;

    public override Vector3 Interpolate(Vector3 startValue, Vector3 endValue, float time)
    {
      return new Vector3(EulerInterpolator.clerp(startValue.x, endValue.x, time), EulerInterpolator.clerp(startValue.y, endValue.y, time), EulerInterpolator.clerp(startValue.z, endValue.z, time));
    }

    private static float clerp(float start, float end, float time)
    {
      float num1 = 0.0f;
      float num2 = 360f;
      float num3 = Mathf.Abs((float) (((double) num2 - (double) num1) / 2.0));
      float num4;
      if ((double) end - (double) start < -(double) num3)
      {
        float num5 = (num2 - start + end) * time;
        num4 = start + num5;
      }
      else if ((double) end - (double) start > (double) num3)
      {
        float num6 = (float) -((double) num2 - (double) end + (double) start) * time;
        num4 = start + num6;
      }
      else
        num4 = start + (end - start) * time;
      return num4;
    }
  }
}
