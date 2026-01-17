// Decompiled with JetBrains decompiler
// Type: FullInspector.Internal.BehaviorTypeToSerializerTypeMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer.Internal;
using System;
using System.Collections.Generic;

#nullable disable
namespace FullInspector.Internal
{
  public static class BehaviorTypeToSerializerTypeMap
  {
    private static List<BehaviorTypeToSerializerTypeMap.SerializationMapping> _mappings = new List<BehaviorTypeToSerializerTypeMap.SerializationMapping>();

    public static void Register(Type behaviorType, Type serializerType)
    {
      BehaviorTypeToSerializerTypeMap._mappings.Add(new BehaviorTypeToSerializerTypeMap.SerializationMapping()
      {
        BehaviorType = behaviorType,
        SerializerType = serializerType
      });
    }

    public static Type GetSerializerType(Type behaviorType)
    {
      for (int index = 0; index < BehaviorTypeToSerializerTypeMap._mappings.Count; ++index)
      {
        BehaviorTypeToSerializerTypeMap.SerializationMapping mapping = BehaviorTypeToSerializerTypeMap._mappings[index];
        if (mapping.BehaviorType.Resolve().IsAssignableFrom(behaviorType.Resolve()))
          return mapping.SerializerType;
      }
      return fiInstalledSerializerManager.DefaultMetadata.SerializerType;
    }

    private struct SerializationMapping
    {
      public Type BehaviorType;
      public Type SerializerType;
    }
  }
}
