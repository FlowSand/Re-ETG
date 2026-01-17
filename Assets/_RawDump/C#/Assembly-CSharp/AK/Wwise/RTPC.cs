// Decompiled with JetBrains decompiler
// Type: AK.Wwise.RTPC
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace AK.Wwise;

[Serializable]
public class RTPC : BaseType
{
  public void SetValue(GameObject gameObject, float value)
  {
    if (!this.IsValid())
      return;
    this.Verify(AkSoundEngine.SetRTPCValue(this.GetID(), value, gameObject));
  }

  public void SetGlobalValue(float value)
  {
    if (!this.IsValid())
      return;
    this.Verify(AkSoundEngine.SetRTPCValue(this.GetID(), value));
  }
}
