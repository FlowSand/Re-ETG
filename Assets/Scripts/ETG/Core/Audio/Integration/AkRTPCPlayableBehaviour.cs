// Decompiled with JetBrains decompiler
// Type: AkRTPCPlayableBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using AK.Wwise;
using System;
using UnityEngine;
using UnityEngine.Playables;

#nullable disable

[Serializable]
public class AkRTPCPlayableBehaviour : PlayableBehaviour
  {
    private bool m_OverrideTrackObject;
    private RTPC m_Parameter;
    private GameObject m_RTPCObject;
    private bool m_SetRTPCGlobally;
    public float RTPCValue;

    public bool setRTPCGlobally
    {
      set => this.m_SetRTPCGlobally = value;
    }

    public bool overrideTrackObject
    {
      set => this.m_OverrideTrackObject = value;
    }

    public GameObject rtpcObject
    {
      set => this.m_RTPCObject = value;
      get => this.m_RTPCObject;
    }

    public RTPC parameter
    {
      set => this.m_Parameter = value;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
      if (!this.m_OverrideTrackObject)
      {
        GameObject gameObject = playerData as GameObject;
        if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
          this.m_RTPCObject = gameObject;
      }
      if (this.m_Parameter == null)
        return;
      if (this.m_SetRTPCGlobally || (UnityEngine.Object) this.m_RTPCObject == (UnityEngine.Object) null)
        this.m_Parameter.SetGlobalValue(this.RTPCValue);
      else
        this.m_Parameter.SetValue(this.m_RTPCObject, this.RTPCValue);
    }
  }

