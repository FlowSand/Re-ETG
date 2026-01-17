// Decompiled with JetBrains decompiler
// Type: BossFinalMarineDeathController
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
    public class BossFinalMarineDeathController : BraveBehaviour
    {
      public List<GameObject> explosionVfx;
      public float explosionMidDelay = 0.3f;
      public int explosionCount = 10;
      [Space(12f)]
      public List<GameObject> bigExplosionVfx;
      public float bigExplosionMidDelay = 0.3f;
      public int bigExplosionCount = 10;

      public void Start()
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        this.healthHaver.OverrideKillCamTime = new float?(5f);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir)
      {
        this.behaviorSpeculator.enabled = false;
        this.aiActor.BehaviorOverridesVelocity = true;
        this.aiActor.BehaviorVelocity = Vector2.zero;
        this.aiAnimator.PlayUntilCancelled("die");
        GameManager.Instance.Dungeon.StartCoroutine(this.OnDeathExplosionsCR());
      }

      [DebuggerHidden]
      private IEnumerator OnDeathExplosionsCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalMarineDeathController__OnDeathExplosionsCRc__Iterator0()
        {
          _this = this
        };
      }
    }

}
