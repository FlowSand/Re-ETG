using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace FullSerializer.Internal.DirectConverters
{
    public class Gradient_DirectConverter : fsDirectConverter<Gradient>
    {
        protected override fsResult DoSerialize(Gradient model, Dictionary<string, fsData> serialized)
        {
            return fsResult.Success + this.SerializeMember<GradientAlphaKey[]>(serialized, "alphaKeys", model.alphaKeys) + this.SerializeMember<GradientColorKey[]>(serialized, "colorKeys", model.colorKeys);
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Gradient model)
        {
            fsResult success = fsResult.Success;
            GradientAlphaKey[] alphaKeys = model.alphaKeys;
            fsResult fsResult1 = success + this.DeserializeMember<GradientAlphaKey[]>(data, "alphaKeys", out alphaKeys);
            model.alphaKeys = alphaKeys;
            GradientColorKey[] colorKeys = model.colorKeys;
            fsResult fsResult2 = fsResult1 + this.DeserializeMember<GradientColorKey[]>(data, "colorKeys", out colorKeys);
            model.colorKeys = colorKeys;
            return fsResult2;
        }

        public override object CreateInstance(fsData data, System.Type storageType)
        {
            return (object) new Gradient();
        }
    }
}
