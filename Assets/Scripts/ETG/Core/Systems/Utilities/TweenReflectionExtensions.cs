using DaikonForge.Tween;
using DaikonForge.Tween.Interpolation;

#nullable disable

  public static class TweenReflectionExtensions
  {
    public static DaikonForge.Tween.Tween<T> TweenProperty<T>(this object target, string propertyName)
    {
      return TweenNamedProperty<T>.Obtain(target, propertyName);
    }

    public static DaikonForge.Tween.Tween<T> TweenProperty<T>(
      this object target,
      string propertyName,
      Interpolator<T> interpolator)
    {
      return TweenNamedProperty<T>.Obtain(target, propertyName, interpolator);
    }
  }

