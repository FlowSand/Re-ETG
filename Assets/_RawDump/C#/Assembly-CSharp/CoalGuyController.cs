// Decompiled with JetBrains decompiler
// Type: CoalGuyController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
public class CoalGuyController : BraveBehaviour
{
  [FormerlySerializedAs("fireEffect2")]
  public GameActorFireEffect fireEffect;
  public tk2dSpriteAnimator eyes;
  public float overrideMoveSpeed = -1f;
  public float overridePauseTime = -1f;
  [CheckDirectionalAnimation(null)]
  public string overrideAnimation;
  public List<DamageTypeModifier> onFireDamageTypeModifiers;

  public void Start()
  {
    this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
    this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.healthHaver)
    {
      this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
      this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
    }
    base.OnDestroy();
  }

  private void OnDamaged(
    float resultValue,
    float maxValue,
    CoreDamageTypes damageTypes,
    DamageCategory damageCategory,
    Vector2 damageDirection)
  {
    if ((damageTypes & CoreDamageTypes.Water) == CoreDamageTypes.Water || (damageTypes & CoreDamageTypes.Ice) == CoreDamageTypes.Ice)
      return;
    this.FlameOn();
    if (!(bool) (UnityEngine.Object) this.healthHaver)
      return;
    this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
  }

  private void OnPreDeath(Vector2 obj)
  {
    if (!(bool) (UnityEngine.Object) this.eyes)
      return;
    this.eyes.gameObject.SetActive(false);
  }

  private void FlameOn()
  {
    this.aiActor.ApplyEffect((GameActorEffect) this.fireEffect);
    this.healthHaver.ApplyDamageModifiers(this.onFireDamageTypeModifiers);
    if ((double) this.overrideMoveSpeed >= 0.0)
      this.aiActor.MovementSpeed = TurboModeController.MaybeModifyEnemyMovementSpeed(this.overrideMoveSpeed);
    if ((double) this.overridePauseTime >= 0.0)
    {
      for (int index = 0; index < this.behaviorSpeculator.MovementBehaviors.Count; ++index)
      {
        if (this.behaviorSpeculator.MovementBehaviors[index] is MoveErraticallyBehavior)
        {
          MoveErraticallyBehavior movementBehavior = this.behaviorSpeculator.MovementBehaviors[index] as MoveErraticallyBehavior;
          movementBehavior.PointReachedPauseTime = this.overridePauseTime;
          movementBehavior.ResetPauseTimer();
          this.aiActor.ClearPath();
        }
      }
    }
    if (!string.IsNullOrEmpty(this.overrideAnimation))
    {
      this.aiAnimator.SetBaseAnim(this.overrideAnimation);
      this.aiAnimator.EndAnimation();
    }
    if ((bool) (UnityEngine.Object) this.eyes)
    {
      this.eyes.gameObject.SetActive(true);
      this.eyes.Play(this.eyes.DefaultClip, 0.0f, this.eyes.DefaultClip.fps);
    }
    for (int index = 0; index < this.behaviorSpeculator.AttackBehaviors.Count; ++index)
    {
      if (this.behaviorSpeculator.AttackBehaviors[index] is AttackBehaviorGroup)
        this.ProcessAttackGroup(this.behaviorSpeculator.AttackBehaviors[index] as AttackBehaviorGroup);
    }
    this.aiShooter.ToggleGunAndHandRenderers(false, nameof (CoalGuyController));
    this.aiShooter.enabled = false;
    this.behaviorSpeculator.AttackCooldown = 0.66f;
  }

  private void ProcessAttackGroup(AttackBehaviorGroup attackGroup)
  {
    for (int index = 0; index < attackGroup.AttackBehaviors.Count; ++index)
    {
      AttackBehaviorGroup.AttackGroupItem attackBehavior = attackGroup.AttackBehaviors[index];
      if (attackBehavior.Behavior is AttackBehaviorGroup)
        this.ProcessAttackGroup(attackBehavior.Behavior as AttackBehaviorGroup);
      else if (attackBehavior.Behavior is ShootGunBehavior)
        attackBehavior.Probability = 0.0f;
      else if (attackBehavior.Behavior is ShootBehavior)
        attackBehavior.Probability = 1f;
    }
  }
}
