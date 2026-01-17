// Decompiled with JetBrains decompiler
// Type: FullInspector.Serializers.FullSerializer.SerializationCallbackReceiverObjectProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;

#nullable disable
namespace FullInspector.Serializers.FullSerializer
{
  public class SerializationCallbackReceiverObjectProcessor : fsObjectProcessor
  {
    public override bool CanProcess(System.Type type)
    {
      return !typeof (UnityEngine.Object).Resolve().IsAssignableFrom(type.Resolve()) && typeof (ISerializationCallbackReceiver).Resolve().IsAssignableFrom(type.Resolve()) && !typeof (BaseObject).Resolve().IsAssignableFrom(type.Resolve());
    }

    public override void OnBeforeSerialize(System.Type storageType, object instance)
    {
      ((ISerializationCallbackReceiver) instance)?.OnBeforeSerialize();
    }

    public override void OnAfterSerialize(System.Type storageType, object instance, ref fsData data)
    {
    }

    public override void OnBeforeDeserialize(System.Type storageType, ref fsData data)
    {
    }

    public override void OnAfterDeserialize(System.Type storageType, object instance)
    {
      ((ISerializationCallbackReceiver) instance)?.OnAfterDeserialize();
    }
  }
}
