// Decompiled with JetBrains decompiler
// Type: BulletKingDeathController
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
    public class BulletKingDeathController : BraveBehaviour
    {
      public List<GameObject> explosionVfx;
      public float explosionMidDelay = 0.3f;
      public int explosionCount = 10;
      public GameObject bigExplosionVfx;
      public float throneFallDelay = 1f;
      public GameObject thronePrefab;

      public void Start()
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir)
      {
        this.aiAnimator.PlayUntilFinished("death", true);
        this.StartCoroutine(this.OnDeathExplosionsCR());
      }

      [DebuggerHidden]
      private IEnumerator OnDeathExplosionsCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletKingDeathController.<OnDeathExplosionsCR>c__Iterator0()
        {
          $this = this
        };
      }
    }

}
