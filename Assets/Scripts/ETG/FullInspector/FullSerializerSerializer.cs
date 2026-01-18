using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FullInspector.Serializers.FullSerializer;
using FullSerializer;
using UnityEngine;

#nullable disable
namespace FullInspector
{
    public class FullSerializerSerializer : BaseSerializer
    {
        [ThreadStatic]
        private static fsSerializer _serializer;
        private static readonly List<fsSerializer> _serializers = new List<fsSerializer>();
        private static readonly List<System.Type> _converters = new List<System.Type>();
        private static readonly List<System.Type> _processors = new List<System.Type>();

        static FullSerializerSerializer()
        {
            FullSerializerSerializer.AddConverter<UnityObjectConverter>();
            FullSerializerSerializer.AddProcessor<SerializationCallbackReceiverObjectProcessor>();
        }

        private static fsSerializer Serializer
        {
            get
            {
                if (FullSerializerSerializer._serializer == null)
                {
                    lock ((object) typeof (FullSerializerSerializer))
                    {
                        FullSerializerSerializer._serializer = new fsSerializer();
                        FullSerializerSerializer._serializers.Add(FullSerializerSerializer._serializer);
                        foreach (System.Type converter in FullSerializerSerializer._converters)
                            FullSerializerSerializer._serializer.AddConverter((fsBaseConverter) Activator.CreateInstance(converter));
                        foreach (System.Type processor in FullSerializerSerializer._processors)
                            FullSerializerSerializer._serializer.AddProcessor((fsObjectProcessor) Activator.CreateInstance(processor));
                    }
                }
                return FullSerializerSerializer._serializer;
            }
        }

        public static void AddConverter<TConverter>() where TConverter : fsConverter, new()
        {
            lock ((object) typeof (FullSerializerSerializer))
            {
                FullSerializerSerializer._converters.Add(typeof (TConverter));
                foreach (fsSerializer serializer in FullSerializerSerializer._serializers)
                    serializer.AddConverter((fsBaseConverter) new TConverter());
            }
        }

        public static void AddProcessor<TProcessor>() where TProcessor : fsObjectProcessor, new()
        {
            lock ((object) typeof (FullSerializerSerializer))
            {
                FullSerializerSerializer._processors.Add(typeof (TProcessor));
                foreach (fsSerializer serializer in FullSerializerSerializer._serializers)
                    serializer.AddProcessor((fsObjectProcessor) new TProcessor());
            }
        }

        public override string Serialize(
            MemberInfo storageType,
            object value,
            ISerializationOperator serializationOperator)
        {
            FullSerializerSerializer.Serializer.Context.Set<ISerializationOperator>(serializationOperator);
            fsData data;
            if (FullSerializerSerializer.EmitFailWarning(FullSerializerSerializer.Serializer.TrySerialize(BaseSerializer.GetStorageType(storageType), value, out data)))
                return (string) null;
            return fiSettings.PrettyPrintSerializedJson ? fsJsonPrinter.PrettyJson(data) : fsJsonPrinter.CompressedJson(data);
        }

        public override object Deserialize(
            MemberInfo storageType,
            string serializedState,
            ISerializationOperator serializationOperator)
        {
            fsData data;
            if (FullSerializerSerializer.EmitFailWarning(fsJsonParser.Parse(serializedState, out data)))
                return (object) null;
            FullSerializerSerializer.Serializer.Context.Set<ISerializationOperator>(serializationOperator);
            object result = (object) null;
            return FullSerializerSerializer.EmitFailWarning(FullSerializerSerializer.Serializer.TryDeserialize(data, BaseSerializer.GetStorageType(storageType), ref result)) ? (object) null : result;
        }

        public override bool SupportsMultithreading => true;

        private static bool EmitFailWarning(fsResult result)
        {
            if (fiSettings.EmitWarnings && result.RawMessages.Any<string>())
                Debug.LogWarning((object) result.FormattedMessages);
            return result.Failed;
        }
    }
}
