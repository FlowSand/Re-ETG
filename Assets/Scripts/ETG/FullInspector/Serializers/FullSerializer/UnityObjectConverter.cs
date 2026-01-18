using FullSerializer;
using FullSerializer.Internal;

#nullable disable
namespace FullInspector.Serializers.FullSerializer
{
  public class UnityObjectConverter : fsConverter
  {
    public override bool CanProcess(System.Type type)
    {
      return typeof (UnityEngine.Object).Resolve().IsAssignableFrom(type.Resolve());
    }

    public override bool RequestCycleSupport(System.Type storageType) => false;

    public override bool RequestInheritanceSupport(System.Type storageType) => false;

    public override fsResult TrySerialize(object instance, out fsData serialized, System.Type storageType)
    {
      UnityEngine.Object @object = (UnityEngine.Object) instance;
      return this.Serializer.TrySerialize<int>(this.Serializer.Context.Get<ISerializationOperator>().StoreObjectReference(@object), out serialized);
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, System.Type storageType)
    {
      ISerializationOperator serializationOperator = this.Serializer.Context.Get<ISerializationOperator>();
      int instance1 = 0;
      fsResult fsResult = this.Serializer.TryDeserialize<int>(data, ref instance1);
      if (fsResult.Failed)
        return fsResult;
      instance = (object) serializationOperator.RetrieveObjectReference(instance1);
      return fsResult.Success;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) storageType;
    }
  }
}
