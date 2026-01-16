// Decompiled with JetBrains decompiler
// Type: ProjectileTrailDamageZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ProjectileTrailDamageZone : MonoBehaviour
{
  public float delayTime = 0.5f;
  public float additionalDestroyTime = 0.5f;
  public float damageToDeal = 5f;
  public bool AppliesFire;
  public GameActorFireEffect FireEffect;

  public void OnSpawned() => this.StartCoroutine(this.HandleSpawnBehavior());

  [DebuggerHidden]
  public IEnumerator HandleSpawnBehavior()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ProjectileTrailDamageZone.\u003CHandleSpawnBehavior\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
