// Decompiled with JetBrains decompiler
// Type: UVScrollTimeOffsetInitializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class UVScrollTimeOffsetInitializer : MonoBehaviour
    {
      public int NumberFrames;
      public float TimePerFrame;

      public void OnSpawned()
      {
        float num = UnityEngine.Time.realtimeSinceStartup % ((float) this.NumberFrames * this.TimePerFrame);
        this.GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", num);
      }
    }

}
