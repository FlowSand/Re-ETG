using DaikonForge.Tween;
using UnityEngine;

#nullable disable

  public static class TweenSpriteExtensions
  {
    public static DaikonForge.Tween.Tween<float> TweenAlpha(this SpriteRenderer sprite)
    {
      return DaikonForge.Tween.Tween<float>.Obtain().SetStartValue(sprite.color.a).SetEndValue(sprite.color.a).SetDuration(1f).OnExecute((TweenAssignmentCallback<float>) (currentValue =>
      {
        Color color = sprite.color;
        color.a = currentValue;
        sprite.color = color;
      }));
    }

    public static DaikonForge.Tween.Tween<Color> TweenColor(this SpriteRenderer sprite)
    {
      return DaikonForge.Tween.Tween<Color>.Obtain().SetStartValue(sprite.color).SetEndValue(sprite.color).SetDuration(1f).OnExecute((TweenAssignmentCallback<Color>) (currentValue => sprite.color = currentValue));
    }
  }

