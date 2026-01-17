// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.DirectConverters.Keyframe_DirectConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullSerializer.Internal.DirectConverters
{
  public class Keyframe_DirectConverter : fsDirectConverter<Keyframe>
  {
    protected override fsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<float>(serialized, "time", model.time) + this.SerializeMember<float>(serialized, "value", model.value) + this.SerializeMember<int>(serialized, "tangentMode", model.tangentMode) + this.SerializeMember<float>(serialized, "inTangent", model.inTangent) + this.SerializeMember<float>(serialized, "outTangent", model.outTangent);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model)
    {
      fsResult success = fsResult.Success;
      float time = model.time;
      fsResult fsResult1 = success + this.DeserializeMember<float>(data, "time", out time);
      model.time = time;
      float num = model.value;
      fsResult fsResult2 = fsResult1 + this.DeserializeMember<float>(data, "value", out num);
      model.value = num;
      int tangentMode = model.tangentMode;
      fsResult fsResult3 = fsResult2 + this.DeserializeMember<int>(data, "tangentMode", out tangentMode);
      model.tangentMode = tangentMode;
      float inTangent = model.inTangent;
      fsResult fsResult4 = fsResult3 + this.DeserializeMember<float>(data, "inTangent", out inTangent);
      model.inTangent = inTangent;
      float outTangent = model.outTangent;
      fsResult fsResult5 = fsResult4 + this.DeserializeMember<float>(data, "outTangent", out outTangent);
      model.outTangent = outTangent;
      return fsResult5;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new Keyframe();
    }
  }
}
