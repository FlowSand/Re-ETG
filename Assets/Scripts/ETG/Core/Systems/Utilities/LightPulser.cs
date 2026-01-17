// Decompiled with JetBrains decompiler
// Type: LightPulser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class LightPulser : MonoBehaviour
    {
      public bool flicker;
      public float pulseSpeed = 40f;
      public float waitTime = 0.05f;
      public float normalRange = 3.33f;
      public float flickerRange = 0.5f;
      private ShadowSystem m_sl;

      private void Start()
      {
        if (!this.flicker)
          return;
        this.StartCoroutine("Flicker");
      }

      public void AssignShadowSystem(ShadowSystem ss) => this.m_sl = ss;

      [DebuggerHidden]
      private IEnumerator Flicker()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new LightPulser.\u003CFlicker\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void Update()
      {
        if (this.flicker)
          return;
        if ((Object) this.m_sl != (Object) null)
          this.m_sl.uLightRange = this.flickerRange + Mathf.PingPong(UnityEngine.Time.time * this.pulseSpeed, this.normalRange - this.flickerRange);
        else
          this.GetComponent<Light>().range = this.flickerRange + Mathf.PingPong(UnityEngine.Time.time * this.pulseSpeed, this.normalRange - this.flickerRange);
      }
    }

}
