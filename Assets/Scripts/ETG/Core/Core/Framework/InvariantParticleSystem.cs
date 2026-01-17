// Decompiled with JetBrains decompiler
// Type: InvariantParticleSystem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    [RequireComponent(typeof (ParticleSystem))]
    public class InvariantParticleSystem : BraveBehaviour
    {
      private ParticleSystem m_particleSystem;

      public void Awake() => this.m_particleSystem = this.GetComponent<ParticleSystem>();

      public void Update()
      {
        this.m_particleSystem.Simulate(GameManager.INVARIANT_DELTA_TIME, true, false);
      }
    }

}
