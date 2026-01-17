// Decompiled with JetBrains decompiler
// Type: ComplexLightColorAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class ComplexLightColorAnimator : MonoBehaviour
    {
      public Gradient colorGradient;
      public float period = 3f;
      public float timeOffset;
      private Light m_light;

      private void Start() => this.m_light = this.GetComponent<Light>();

      private void Update()
      {
        this.m_light.color = this.colorGradient.Evaluate((UnityEngine.Time.realtimeSinceStartup + this.timeOffset) % this.period / this.period);
      }
    }

}
