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
