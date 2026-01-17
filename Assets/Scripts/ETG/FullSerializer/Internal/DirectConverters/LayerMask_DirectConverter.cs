// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.DirectConverters.LayerMask_DirectConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
