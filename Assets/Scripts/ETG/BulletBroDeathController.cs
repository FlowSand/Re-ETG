// Decompiled with JetBrains decompiler
// Type: BulletBroDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class BulletBroDeathController : BraveBehaviour
{
  private void Start() => this.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);

  protected override void OnDestroy() => base.OnDestroy();

  private void OnDeath(Vector2 finalDeathDir)
  {
    BroController otherBro = BroController.GetOtherBro(this.gameObject);
    if ((bool) (UnityEngine.Object) otherBro)
      otherBro.Enrage();
    else
      GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_BULLET_BROS, true);
  }
}
