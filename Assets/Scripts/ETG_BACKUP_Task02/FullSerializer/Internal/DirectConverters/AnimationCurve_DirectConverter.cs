// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.DirectConverters.AnimationCurve_DirectConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullSerializer.Internal.DirectConverters;

public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve>
{
  protected override fsResult DoSerialize(
    AnimationCurve model,
    Dictionary<string, fsData> serialized)
  {
    return fsResult.Success + this.SerializeMember<Keyframe[]>(serialized, "keys", model.keys) + this.SerializeMember<WrapMode>(serialized, "preWrapMode", model.preWrapMode) + this.SerializeMember<WrapMode>(serialized, "postWrapMode", model.postWrapMode);
  }

  protected override fsResult DoDeserialize(
    Dictionary<string, fsData> data,
    ref AnimationCurve model)
  {
    fsResult success = fsResult.Success;
    Keyframe[] keys = model.keys;
    fsResult fsResult1 = success + this.DeserializeMember<Keyframe[]>(data, "keys", out keys);
    model.keys = keys;
    WrapMode preWrapMode = model.preWrapMode;
    fsResult fsResult2 = fsResult1 + this.DeserializeMember<WrapMode>(data, "preWrapMode", out preWrapMode);
    model.preWrapMode = preWrapMode;
    WrapMode postWrapMode = model.postWrapMode;
    fsResult fsResult3 = fsResult2 + this.DeserializeMember<WrapMode>(data, "postWrapMode", out postWrapMode);
    model.postWrapMode = postWrapMode;
    return fsResult3;
  }

  public override object CreateInstance(fsData data, System.Type storageType)
  {
    return (object) new AnimationCurve();
  }
}
