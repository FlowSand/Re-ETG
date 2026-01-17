// Decompiled with JetBrains decompiler
// Type: TweenReflectionExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween;
using DaikonForge.Tween.Interpolation;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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

}
