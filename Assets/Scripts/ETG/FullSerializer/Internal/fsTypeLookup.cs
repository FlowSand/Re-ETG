// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.fsTypeLookup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
