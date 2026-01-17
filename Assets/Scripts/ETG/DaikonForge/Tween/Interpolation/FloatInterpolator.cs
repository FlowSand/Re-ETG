// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.FloatInterpolator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace DaikonForge.Tween.Interpolation
{
  public class FloatInterpolator : Interpolator<float>
  {
    protected static FloatInterpolator singleton;

    public override float Add(float lhs, float rhs) => lhs + rhs;

    public override float Interpolate(float startValue, float endValue, float time)
    {
      return startValue + (endValue - startValue) * time;
    }

    public static Interpolator<float> Default
    {
      get
      {
        if (FloatInterpolator.singleton == null)
          FloatInterpolator.singleton = new FloatInterpolator();
        return (Interpolator<float>) FloatInterpolator.singleton;
      }
    }
  }
}
