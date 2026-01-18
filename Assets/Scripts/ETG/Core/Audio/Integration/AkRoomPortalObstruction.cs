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

