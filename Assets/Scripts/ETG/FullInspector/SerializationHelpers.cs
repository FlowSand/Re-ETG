using System.Collections.Generic;

using FullInspector.Internal;
using FullSerializer.Internal;

#nullable disable
namespace FullInspector
{
    public static class SerializationHelpers
    {
        public static T DeserializeFromContent<T, TSerializer>(string content) where TSerializer : BaseSerializer
        {
            return (T) SerializationHelpers.DeserializeFromContent<TSerializer>(typeof (T), content);
        }

        public static object DeserializeFromContent<TSerializer>(System.Type storageType, string content) where TSerializer : BaseSerializer
        {
            TSerializer serializer = fiSingletons.Get<TSerializer>();
            NotSupportedSerializationOperator serializationOperator = fiSingletons.Get<NotSupportedSerializationOperator>();
            return serializer.Deserialize(fsPortableReflection.AsMemberInfo(storageType), content, (ISerializationOperator) serializationOperator);
        }

        public static string SerializeToContent<T, TSerializer>(T value) where TSerializer : BaseSerializer
        {
            return SerializationHelpers.SerializeToContent<TSerializer>(typeof (T), (object) value);
        }

        public static string SerializeToContent<TSerializer>(System.Type storageType, object value) where TSerializer : BaseSerializer
        {
            TSerializer serializer = fiSingletons.Get<TSerializer>();
            NotSupportedSerializationOperator serializationOperator = fiSingletons.Get<NotSupportedSerializationOperator>();
            return serializer.Serialize(fsPortableReflection.AsMemberInfo(storageType), value, (ISerializationOperator) serializationOperator);
        }

        public static T Clone<T, TSerializer>(T obj) where TSerializer : BaseSerializer
        {
            return (T) SerializationHelpers.Clone<TSerializer>(typeof (T), (object) obj);
        }

        public static object Clone<TSerializer>(System.Type storageType, object obj) where TSerializer : BaseSerializer
        {
            TSerializer serializer = fiSingletons.Get<TSerializer>();
            ListSerializationOperator serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = new List<UnityEngine.Object>();
            string serializedState = serializer.Serialize(fsPortableReflection.AsMemberInfo(storageType), obj, (ISerializationOperator) serializationOperator);
            object obj1 = serializer.Deserialize(fsPortableReflection.AsMemberInfo(storageType), serializedState, (ISerializationOperator) serializationOperator);
            serializationOperator.SerializedObjects = (List<UnityEngine.Object>) null;
            return obj1;
        }
    }
}
