// Decompiled with JetBrains decompiler
// Type: AgunimDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class AgunimDeathController : BraveBehaviour
{
  public void Start()
  {
    this.healthHaver.ManualDeathHandling = true;
    this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);
    this.healthHaver.OverrideKillCamTime = new float?(5f);
  }

  protected override void OnDestroy() => base.OnDestroy();

  private void OnBossDeath(Vector2 dir)
  {
    this.aiAnimator.ChildAnimator.gameObject.SetActive(false);
    this.aiAnimator.PlayUntilCancelled("death", true);
    this.StartCoroutine(this.HandlePostDeathExplosionCR());
    this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnBossDeath);
    this.StartCoroutine(this.HandlePostDeathExplosionCR());
  }

  [DebuggerHidden]
  private IEnumerator HandlePostDeathExplosionCR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AgunimDeathController.\u003CHandlePostDeathExplosionCR\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
