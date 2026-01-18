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
