using UnityEngine;

using DaikonForge.Tween;

#nullable disable

    public static class TweenMaterialExtensions
    {
        public static DaikonForge.Tween.Tween<Color> TweenColor(this Material material)
        {
            return DaikonForge.Tween.Tween<Color>.Obtain().SetStartValue(material.color).SetEndValue(material.color).SetDuration(1f).OnExecute((TweenAssignmentCallback<Color>) (currentValue => material.color = currentValue));
        }

        public static DaikonForge.Tween.Tween<float> TweenAlpha(this Material material)
        {
            return DaikonForge.Tween.Tween<float>.Obtain().SetStartValue(material.color.a).SetEndValue(material.color.a).SetDuration(1f).OnExecute((TweenAssignmentCallback<float>) (currentValue =>
            {
                Color color = material.color;
                color.a = currentValue;
                material.color = color;
            }));
        }
    }

