// Decompiled with JetBrains decompiler
// Type: BossStatueDeathController
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
    public class BossStatueDeathController : BraveBehaviour
    {
      public float deathFlashInterval = 0.1f;
      public List<GameObject> explosionVfx;
      public float explosionMidDelay = 0.3f;
      public int explosionCount = 10;
      public Transform bigExplosionTransform;
      public GameObject bigExplosionVfx;
      public List<GameObject> debrisObjects;
      public int debrisCount;
      public int debrisMinForce = 5;
      public int debrisMaxForce = 5;
      public float debrisAngleVariance = 15f;
      private BossStatueController m_statueController;
      private bool m_isReallyDead;

      public void Start()
      {
        this.m_statueController = this.GetComponent<BossStatueController>();
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.transform.parent.gameObject);
        base.OnDestroy();
      }

      private void OnBossDeath(Vector2 dir) => this.StartCoroutine(this.OnDeathAnimationCR());

      [DebuggerHidden]
      private IEnumerator OnDeathAnimationCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossStatueDeathController__OnDeathAnimationCRc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator DeathFlashCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossStatueDeathController__DeathFlashCRc__Iterator1()
        {
          _this = this
        };
      }
    }

}
