// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.fiSingletons
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal
{
  public static class fiSingletons
  {
    private static Dictionary<Type, object> _instances = new Dictionary<Type, object>();

    public static T Get<T>() => (T) fiSingletons.Get(typeof (T));

    public static object Get(Type type)
    {
      object instance;
      if (!fiSingletons._instances.TryGetValue(type, out instance))
      {
        instance = Activator.CreateInstance(type);
        fiSingletons._instances[type] = instance;
      }
      return instance;
    }
  }
}
