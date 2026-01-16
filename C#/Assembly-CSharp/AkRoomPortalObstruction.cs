// Decompiled with JetBrains decompiler
// Type: AkRoomPortalObstruction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (AkRoomPortal))]
[AddComponentMenu("Wwise/AkRoomPortalObstruction")]
public class AkRoomPortalObstruction : AkObstructionOcclusion
{
  private AkRoomPortal m_portal;

  private void Awake()
  {
    this.InitIntervalsAndFadeRates();
    this.m_portal = this.GetComponent<AkRoomPortal>();
  }

  protected override void UpdateObstructionOcclusionValuesForListeners()
  {
    this.UpdateObstructionOcclusionValues(AkSpatialAudioListener.TheSpatialAudioListener);
  }

  protected override void SetObstructionOcclusion(
    KeyValuePair<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> ObsOccPair)
  {
    int num = (int) AkSoundEngine.SetPortalObstruction(this.m_portal.GetID(), ObsOccPair.Value.currentValue);
  }
}
