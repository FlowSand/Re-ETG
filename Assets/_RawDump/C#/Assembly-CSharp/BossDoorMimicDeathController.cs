// Decompiled with JetBrains decompiler
// Type: BossDoorMimicDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class BossDoorMimicDeathController : BraveBehaviour
{
  public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);

  protected override void OnDestroy() => base.OnDestroy();

  private void OnBossDeath(Vector2 dir)
  {
    BossDoorMimicIntroDoer component = this.GetComponent<BossDoorMimicIntroDoer>();
    if (!(bool) (UnityEngine.Object) component)
      return;
    component.PhantomDoorBlocker.Unseal();
  }
}
