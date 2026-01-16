// Decompiled with JetBrains decompiler
// Type: FullInspector.Serializers.FullSerializer.FullSerializerMetadata
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer;
using System;
using UnityEngine;

#nullable disable
namespace FullInspector.Serializers.FullSerializer;

public class FullSerializerMetadata : fiISerializerMetadata
{
  public Guid SerializerGuid => new Guid("bc898177-6ff4-423f-91bb-589bc83d8fde");

  public System.Type SerializerType => typeof (FullSerializerSerializer);

  public System.Type[] SerializationOptInAnnotationTypes
  {
    get
    {
      return new System.Type[2]
      {
        typeof (SerializeField),
        typeof (fsPropertyAttribute)
      };
    }
  }

  public System.Type[] SerializationOptOutAnnotationTypes
  {
    get => new System.Type[1]{ typeof (fsIgnoreAttribute) };
  }
}
