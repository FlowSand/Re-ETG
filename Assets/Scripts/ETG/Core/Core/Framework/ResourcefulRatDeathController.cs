// Decompiled with JetBrains decompiler
// Type: ResourcefulRatDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ResourcefulRatDeathController : BraveBehaviour
    {
      public void Start()
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
        this.healthHaver.OverrideKillCamTime = new float?(5f);
        this.healthHaver.TrackDuringDeath = true;
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir)
      {
        this.StartCoroutine(this.BossDeathCR());
        this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnBossDeath);
      }

      [DebuggerHidden]
      private IEnumerator BossDeathCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatDeathController.<BossDeathCR>c__Iterator0()
        {
          _this = this
        };
      }
    }

}
