using System;
using System.Collections;

#nullable disable
namespace FullSerializer.Internal
{
    public class fsReflectedConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return !type.Resolve().IsArray && !typeof (ICollection).IsAssignableFrom(type);
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            serialized = fsData.CreateDictionary();
            fsResult success = fsResult.Success;
            fsMetaType fsMetaType = fsMetaType.Get(instance.GetType());
            fsMetaType.EmitAotData();
            for (int index = 0; index < fsMetaType.Properties.Length; ++index)
            {
                fsMetaProperty property = fsMetaType.Properties[index];
                if (property.CanRead && !property.JsonDeserializeOnly)
                {
                    fsData data;
                    fsResult result = this.Serializer.TrySerialize(property.StorageType, property.Read(instance), out data);
                    success.AddMessages(result);
                    if (!result.Failed)
                        serialized.AsDictionary[property.JsonName] = data;
                }
            }
            return success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            fsResult fsResult;
            if ((fsResult = fsResult.Success + this.CheckType(data, fsDataType.Object)).Failed)
                return fsResult;
            fsMetaType fsMetaType = fsMetaType.Get(storageType);
            fsMetaType.EmitAotData();
            for (int index = 0; index < fsMetaType.Properties.Length; ++index)
            {
                fsMetaProperty property = fsMetaType.Properties[index];
                fsData data1;
                if (property.CanWrite && data.AsDictionary.TryGetValue(property.JsonName, out data1))
                {
                    object result1 = (object) null;
                    if (property.CanRead)
                        result1 = property.Read(instance);
                    fsResult result2 = this.Serializer.TryDeserialize(data1, property.StorageType, ref result1);
                    fsResult.AddMessages(result2);
                    if (!result2.Failed)
                        property.Write(instance, result1);
                }
            }
            return fsResult;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return fsMetaType.Get(storageType).CreateInstance();
        }
    }
}
