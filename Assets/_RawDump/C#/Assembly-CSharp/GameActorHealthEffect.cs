// Decompiled with JetBrains decompiler
// Type: GameActorHealthEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class GameActorHealthEffect : GameActorEffect
{
  public float DamagePerSecondToEnemies = 10f;
  public bool ignitesGoops;

  public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
  {
    if (this.AffectsEnemies && actor is AIActor)
      actor.healthHaver.ApplyDamage(this.DamagePerSecondToEnemies * BraveTime.DeltaTime, Vector2.zero, this.effectIdentifier, damageCategory: DamageCategory.DamageOverTime);
    if (!this.ignitesGoops)
      return;
    DeadlyDeadlyGoopManager.IgniteGoopsCircle(actor.CenterPosition, 0.5f);
  }
}
