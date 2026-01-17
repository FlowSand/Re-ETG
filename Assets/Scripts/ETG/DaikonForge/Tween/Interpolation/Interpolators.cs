// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Interpolation.Interpolators
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Interpolation
{
  public static class Interpolators
  {
    private static Dictionary<System.Type, object> registry = new Dictionary<System.Type, object>();

    static Interpolators()
    {
      Interpolators.Register<int>(IntInterpolator.Default);
      Interpolators.Register<float>(FloatInterpolator.Default);
      Interpolators.Register<Rect>(RectInterpolator.Default);
      Interpolators.Register<Color>(ColorInterpolator.Default);
      Interpolators.Register<Vector2>(Vector2Interpolator.Default);
      Interpolators.Register<Vector3>(Vector3Interpolator.Default);
      Interpolators.Register<Vector4>(Vector4Interpolator.Default);
    }

    public static Interpolator<T> Get<T>() => (Interpolator<T>) Interpolators.Get(typeof (T), true);

    public static object Get(System.Type type, bool throwOnNotFound)
    {
      if (type == null)
        throw new ArgumentNullException("You must provide a System.Type value");
      object obj = (object) null;
      if (!Interpolators.registry.TryGetValue(type, out obj) && throwOnNotFound)
        throw new KeyNotFoundException($"There is no default interpolator defined for type '{type.Name}'");
      return obj;
    }

    public static void Register<T>(Interpolator<T> interpolator)
    {
      Interpolators.registry[typeof (T)] = (object) interpolator;
    }
  }
}
