using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullSerializer.Internal.DirectConverters
{
  public class LayerMask_DirectConverter : fsDirectConverter<LayerMask>
  {
    protected override fsResult DoSerialize(LayerMask model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<int>(serialized, "value", model.value);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LayerMask model)
    {
      fsResult success = fsResult.Success;
      int num = model.value;
      fsResult fsResult = success + this.DeserializeMember<int>(data, "value", out num);
      model.value = num;
      return fsResult;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new LayerMask();
    }
  }
}
