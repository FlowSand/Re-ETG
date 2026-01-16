// Decompiled with JetBrains decompiler
// Type: FullSerializer.Internal.DirectConverters.Gradient_DirectConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FullSerializer.Internal.DirectConverters;

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
