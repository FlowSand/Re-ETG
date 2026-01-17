// Decompiled with JetBrains decompiler
// Type: TweenSpriteExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    public static class TweenSpriteExtensions
    {
      public static DaikonForge.Tween.Tween<float> TweenAlpha(this SpriteRenderer sprite)
      {
        return DaikonForge.Tween.Tween<float>.Obtain().SetStartValue(sprite.color.a).SetEndValue(sprite.color.a).SetDuration(1f).OnExecute((TweenAssignmentCallback<float>) (currentValue =>
        {
          Color color = sprite.color with { a = currentValue };
          sprite.color = color;
        }));
      }

      public static DaikonForge.Tween.Tween<Color> TweenColor(this SpriteRenderer sprite)
      {
        return DaikonForge.Tween.Tween<Color>.Obtain().SetStartValue(sprite.color).SetEndValue(sprite.color).SetDuration(1f).OnExecute((TweenAssignmentCallback<Color>) (currentValue => sprite.color = currentValue));
      }
    }

}
