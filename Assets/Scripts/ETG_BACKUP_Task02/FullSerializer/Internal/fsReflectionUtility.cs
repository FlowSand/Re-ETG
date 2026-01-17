// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsReflectionUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace FullSerializer.Internal;

public static class fsReflectionUtility
{
  public static Type GetInterface(Type type, Type interfaceType)
  {
    if (interfaceType.Resolve().IsGenericType && !interfaceType.Resolve().IsGenericTypeDefinition)
      throw new ArgumentException("GetInterface requires that if the interface type is generic, then it must be the generic type definition, not a specific generic type instantiation");
    for (; type != null; type = type.Resolve().BaseType)
    {
      foreach (Type type1 in type.GetInterfaces())
      {
        if (type1.Resolve().IsGenericType)
        {
          if (interfaceType == type1.GetGenericTypeDefinition())
            return type1;
        }
        else if (interfaceType == type1)
          return type1;
      }
    }
    return (Type) null;
  }
}
