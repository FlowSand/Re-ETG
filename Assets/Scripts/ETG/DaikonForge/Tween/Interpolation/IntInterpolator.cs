using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation
{
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
}
