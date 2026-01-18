using System;
using System.Reflection;

#nullable disable
namespace FullSerializer.Internal
{
  internal static class fsTypeLookup
  {
    public static Type GetType(string typeName)
    {
      Type type1 = Type.GetType(typeName);
      if (type1 != null)
        return type1;
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        Type type2 = assembly.GetType(typeName);
        if (type2 != null)
          return type2;
      }
      return (Type) null;
    }
  }
}
