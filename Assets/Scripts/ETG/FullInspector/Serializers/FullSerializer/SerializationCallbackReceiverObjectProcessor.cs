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
