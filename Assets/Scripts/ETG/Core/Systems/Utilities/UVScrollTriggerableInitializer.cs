// Decompiled with JetBrains decompiler
// Type: UVScrollTriggerableInitializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class UVScrollTriggerableInitializer : MonoBehaviour
  {
    public int NumberFrames;
    public float TimePerFrame;

    public void OnSpawned() => this.ResetAnimation();

    public void TriggerAnimation()
    {
      float num = UnityEngine.Time.realtimeSinceStartup % ((float) this.NumberFrames * this.TimePerFrame);
      Material material = this.GetComponent<MeshRenderer>().material;
      material.SetFloat("_TimeOffset", num);
      material.SetFloat("_ForcedFrame", -1f);
    }

    public void ResetAnimation()
    {
      this.GetComponent<MeshRenderer>().material.SetFloat("_ForcedFrame", 0.0f);
    }
  }

