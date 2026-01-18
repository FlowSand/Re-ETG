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
