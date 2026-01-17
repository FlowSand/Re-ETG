// Decompiled with JetBrains decompiler
// Type: AkEmitterObstructionOcclusion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Audio.Integration
{
    [RequireComponent(typeof (AkGameObj))]
    [AddComponentMenu("Wwise/AkEmitterObstructionOcclusion")]
    public class AkEmitterObstructionOcclusion : AkObstructionOcclusion
    {
      private AkGameObj m_gameObj;

      private void Awake()
      {
        this.InitIntervalsAndFadeRates();
        this.m_gameObj = this.GetComponent<AkGameObj>();
      }

      protected override void UpdateObstructionOcclusionValuesForListeners()
      {
        if (AkRoom.IsSpatialAudioEnabled)
        {
          this.UpdateObstructionOcclusionValues(AkSpatialAudioListener.TheSpatialAudioListener);
        }
        else
        {
          if (this.m_gameObj.IsUsingDefaultListeners)
            this.UpdateObstructionOcclusionValues(AkAudioListener.DefaultListeners.ListenerList);
          this.UpdateObstructionOcclusionValues(this.m_gameObj.ListenerList);
        }
      }

      protected override void SetObstructionOcclusion(
        KeyValuePair<AkAudioListener, AkObstructionOcclusion.ObstructionOcclusionValue> ObsOccPair)
      {
        if (AkRoom.IsSpatialAudioEnabled)
        {
          int num1 = (int) AkSoundEngine.SetEmitterObstruction(this.gameObject, ObsOccPair.Value.currentValue);
        }
        else
        {
          int num2 = (int) AkSoundEngine.SetObjectObstructionAndOcclusion(this.gameObject, ObsOccPair.Key.gameObject, 0.0f, ObsOccPair.Value.currentValue);
        }
      }
    }

}
