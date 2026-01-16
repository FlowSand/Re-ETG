// Decompiled with JetBrains decompiler
// Type: ResourcefulRatQuickDaggers1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/ResourcefulRat/QuickDaggers1")]
public class ResourcefulRatQuickDaggers1 : Script
{
  private const int NumWaves = 1;
  private const int NumDaggersPerWave = 4;
  private const int AttackDelay = 26;
  private const float DaggerSpeed = 60f;
  private List<GameObject> m_reticles = new List<GameObject>();

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ResourcefulRatQuickDaggers1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public override void OnForceEnded() => this.CleanupReticles();

  public Vector2 GetPredictedTargetPosition(float leadAmount, float speed, float fireDelay)
  {
    return BraveMathCollege.GetPredictedPosition(this.BulletManager.PlayerPosition() + this.BulletManager.PlayerVelocity() * fireDelay, this.BulletManager.PlayerVelocity(), this.Position, speed);
  }

  private void CleanupReticles()
  {
    for (int index = 0; index < this.m_reticles.Count; ++index)
      SpawnManager.Despawn(this.m_reticles[index]);
    this.m_reticles.Clear();
  }
}
