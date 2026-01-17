// Decompiled with JetBrains decompiler
// Type: GatlingGullDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class GatlingGullDeathController : BraveBehaviour
{
  public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnBossDeath);

  protected override void OnDestroy()
  {
    if (GameManager.HasInstance)
      this.Cleanup();
    base.OnDestroy();
  }

  private void OnBossDeath(Vector2 dir) => this.Cleanup();

  private void Cleanup()
  {
    foreach (SkyRocket skyRocket in UnityEngine.Object.FindObjectsOfType<SkyRocket>())
      skyRocket.DieInAir();
  }
}
