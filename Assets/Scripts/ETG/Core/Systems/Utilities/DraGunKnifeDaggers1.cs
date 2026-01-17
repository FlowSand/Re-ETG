// Decompiled with JetBrains decompiler
// Type: DraGunKnifeDaggers1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/DraGun/KnifeDaggers1")]
    public class DraGunKnifeDaggers1 : Script
    {
      private const int NumWaves = 1;
      private const int NumDaggersPerWave = 7;
      private const int AttackDelay = 42;
      private const float DaggerSpeed = 60f;
      private List<LineReticleController> m_reticles = new List<LineReticleController>();

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunKnifeDaggers1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      public override void OnForceEnded() => this.CleanupReticles();

      public Vector2 GetPredictedTargetPosition(float leadAmount, float speed, float fireDelay)
      {
        return BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition() + this.BulletManager.PlayerVelocity() * fireDelay, this.BulletManager.PlayerVelocity(), this.Position, speed);
      }

      private void CleanupReticles()
      {
        for (int index = 0; index < this.m_reticles.Count; ++index)
          this.m_reticles[index].Cleanup();
        this.m_reticles.Clear();
      }
    }

}
