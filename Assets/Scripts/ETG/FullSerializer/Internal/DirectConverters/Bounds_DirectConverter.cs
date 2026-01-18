using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace FullSerializer.Internal.DirectConverters
{
    public class Bounds_DirectConverter : fsDirectConverter<Bounds>
    {
        protected override fsResult DoSerialize(Bounds model, Dictionary<string, fsData> serialized)
        {
            return fsResult.Success + this.SerializeMember<Vector3>(serialized, "center", model.center) + this.SerializeMember<Vector3>(serialized, "size", model.size);
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Bounds model)
        {
            fsResult success = fsResult.Success;
            Vector3 center = model.center;
            fsResult fsResult1 = success + this.DeserializeMember<Vector3>(data, "center", out center);
            model.center = center;
            Vector3 size = model.size;
            fsResult fsResult2 = fsResult1 + this.DeserializeMember<Vector3>(data, "size", out size);
            model.size = size;
            return fsResult2;
        }

        public override object CreateInstance(fsData data, System.Type storageType)
        {
            return (object) new Bounds();
        }
    }
}
