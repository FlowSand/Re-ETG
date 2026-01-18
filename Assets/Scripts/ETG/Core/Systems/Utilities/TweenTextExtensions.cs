using DaikonForge.Tween;
using UnityEngine;

#nullable disable

  public static class TweenTextExtensions
  {
    public static DaikonForge.Tween.Tween<float> TweenAlpha(this TextMesh text)
    {
      return DaikonForge.Tween.Tween<float>.Obtain().SetStartValue(text.color.a).SetEndValue(text.color.a).SetDuration(1f).OnExecute((TweenAssignmentCallback<float>) (currentValue =>
      {
        Color color = text.color;
        color.a = currentValue;
        text.color = color;
      }));
    }

    public static DaikonForge.Tween.Tween<Color> TweenColor(this TextMesh text)
    {
      return DaikonForge.Tween.Tween<Color>.Obtain().SetStartValue(text.color).SetEndValue(text.color).SetDuration(1f).OnExecute((TweenAssignmentCallback<Color>) (currentValue => text.color = currentValue));
    }
  }

