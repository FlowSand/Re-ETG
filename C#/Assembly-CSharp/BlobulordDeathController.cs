// Decompiled with JetBrains decompiler
// Type: BlobulordDeathController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class BlobulordDeathController : BraveBehaviour
{
  public List<GameObject> explosionVfx;
  public float explosionMidDelay = 0.3f;
  public int explosionCount = 10;
  public float finalScale = 0.1f;
  public GameObject bigExplosionVfx;
  public float crawlerSpawnDelay = 0.3f;
  [EnemyIdentifier]
  public string crawlerGuid;

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
    return (IEnumerator) new BlobulordDeathController.\u003COnDeathExplosionsCR\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
