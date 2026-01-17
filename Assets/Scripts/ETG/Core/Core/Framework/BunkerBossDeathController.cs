// Decompiled with JetBrains decompiler
// Type: BunkerBossDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class BunkerBossDeathController : BraveBehaviour
    {
      public List<GameObject> explosionVfx;
      public float explosionMidDelay = 0.3f;
      public int explosionCount = 10;
      public List<GameObject> debrisObjects;
      public float debrisMidDelay;
      public int debrisCount;
      public int debrisMinForce = 5;
      public int debrisMaxForce = 5;
      public float debrisAngleVariance = 15f;
      public string deathAnimation;
      public float deathAnimationDelay;
      public List<GameObject> dustVfx;
      public float dustTime = 1f;
      public float dustMidDelay = 0.05f;
      public Vector2 dustOffset;
      public Vector2 dustDimensions;
      public float shakeMidDelay = 0.1f;
      public string flagAnimation;

      public void Start()
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir)
      {
        this.StartCoroutine(this.OnDeathExplosionsCR());
        this.StartCoroutine(this.OnDeathDebrisCR());
        this.StartCoroutine(this.OnDeathAnimationCR());
      }

      [DebuggerHidden]
      private IEnumerator OnDeathExplosionsCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BunkerBossDeathController.<OnDeathExplosionsCR>c__Iterator0()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator OnDeathDebrisCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BunkerBossDeathController.<OnDeathDebrisCR>c__Iterator1()
        {
          $this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator OnDeathAnimationCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BunkerBossDeathController.<OnDeathAnimationCR>c__Iterator2()
        {
          $this = this
        };
      }
    }

}
