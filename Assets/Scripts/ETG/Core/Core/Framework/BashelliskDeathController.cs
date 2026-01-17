// Decompiled with JetBrains decompiler
// Type: BashelliskDeathController
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
    public class BashelliskDeathController : BraveBehaviour
    {
      public VFXPool HeadVfx;

      public void Start()
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
      }

      protected override void OnDestroy() => base.OnDestroy();

      private void OnBossDeath(Vector2 dir) => this.StartCoroutine(this.OnDeathCR());

      [DebuggerHidden]
      private IEnumerator OnDeathCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BashelliskDeathController.<OnDeathCR>c__Iterator0()
        {
          _this = this
        };
      }
    }

}
