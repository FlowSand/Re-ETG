// Decompiled with JetBrains decompiler
// Type: GameActorBleedEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class GameActorBleedEffect : GameActorEffect
{
  public float ChargeAmount = 10f;
  public float ChargeDispelFactor = 10f;
  public GameObject vfxChargingReticle;
  public GameObject vfxExplosion;
  private GameObject m_extantReticle;
  private bool m_isHammerOfDawn;

  public bool ShouldVanishOnDeath(GameActor actor)
  {
    return (!(bool) (UnityEngine.Object) actor.healthHaver || !actor.healthHaver.IsBoss) && (!(actor is AIActor) || !(actor as AIActor).IsSignatureEnemy);
  }

  public override void ApplyTint(GameActor actor)
  {
  }

  public override void OnEffectApplied(
    GameActor actor,
    RuntimeGameActorEffectData effectData,
    float partialAmount = 1f)
  {
    if ((bool) (UnityEngine.Object) actor && (bool) (UnityEngine.Object) actor.healthHaver && actor.healthHaver.IsDead)
    {
      effectData.accumulator = 0.0f;
    }
    else
    {
      this.m_isHammerOfDawn = (UnityEngine.Object) this.vfxExplosion.GetComponent<HammerOfDawnController>() != (UnityEngine.Object) null;
      effectData.accumulator += this.ChargeAmount * partialAmount;
    }
  }

  public override void OnDarkSoulsAccumulate(
    GameActor actor,
    RuntimeGameActorEffectData effectData,
    float partialAmount = 1f,
    Projectile sourceProjectile = null)
  {
    if ((bool) (UnityEngine.Object) actor && (bool) (UnityEngine.Object) actor.healthHaver && actor.healthHaver.IsDead)
      effectData.accumulator = 0.0f;
    else if (this.m_isHammerOfDawn && HammerOfDawnController.HasExtantHammer(sourceProjectile))
    {
      effectData.accumulator = 0.0f;
    }
    else
    {
      effectData.accumulator += this.ChargeAmount * partialAmount;
      if ((!this.m_isHammerOfDawn || !HammerOfDawnController.HasExtantHammer(sourceProjectile)) && !(bool) (UnityEngine.Object) this.m_extantReticle)
      {
        this.m_extantReticle = UnityEngine.Object.Instantiate<GameObject>(this.vfxChargingReticle, (Vector3) actor.specRigidbody.HitboxPixelCollider.UnitBottomCenter, Quaternion.identity);
        this.m_extantReticle.transform.parent = actor.transform;
        RailgunChargeEffectController component = this.m_extantReticle.GetComponent<RailgunChargeEffectController>();
        if ((bool) (UnityEngine.Object) component)
          component.IsManuallyControlled = true;
      }
      if ((double) effectData.accumulator <= 100.0 || !actor.healthHaver.IsAlive)
        return;
      effectData.accumulator = 0.0f;
      if (!this.m_isHammerOfDawn || !HammerOfDawnController.HasExtantHammer(sourceProjectile))
      {
        GameObject gameObject = !this.m_isHammerOfDawn ? actor.PlayEffectOnActor(this.vfxExplosion, Vector3.zero, false) : UnityEngine.Object.Instantiate<GameObject>(this.vfxExplosion, actor.transform.position, Quaternion.identity);
        tk2dBaseSprite component1 = gameObject.GetComponent<tk2dBaseSprite>();
        if ((bool) (UnityEngine.Object) actor && (bool) (UnityEngine.Object) actor.specRigidbody && (bool) (UnityEngine.Object) component1)
          component1.PlaceAtPositionByAnchor((Vector3) actor.specRigidbody.HitboxPixelCollider.UnitBottomCenter, tk2dBaseSprite.Anchor.LowerCenter);
        HammerOfDawnController component2 = gameObject.GetComponent<HammerOfDawnController>();
        if ((bool) (UnityEngine.Object) component2 && (bool) (UnityEngine.Object) sourceProjectile)
          component2.AssignOwner(sourceProjectile.Owner as PlayerController, sourceProjectile);
      }
      if (!(bool) (UnityEngine.Object) this.m_extantReticle)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantReticle.gameObject);
      this.m_extantReticle = (GameObject) null;
    }
  }

  public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
  {
    if ((double) effectData.accumulator > 0.0)
      effectData.accumulator = Mathf.Max(0.0f, effectData.accumulator - BraveTime.DeltaTime * this.ChargeDispelFactor);
    if (!(bool) (UnityEngine.Object) this.m_extantReticle)
      return;
    RailgunChargeEffectController component = this.m_extantReticle.GetComponent<RailgunChargeEffectController>();
    if (!(bool) (UnityEngine.Object) component)
      return;
    component.ManualCompletionPercentage = effectData.accumulator / 100f;
  }

  public override void OnEffectRemoved(GameActor actor, RuntimeGameActorEffectData effectData)
  {
    if (!(bool) (UnityEngine.Object) this.m_extantReticle)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantReticle.gameObject);
    this.m_extantReticle = (GameObject) null;
  }

  public override bool IsFinished(
    GameActor actor,
    RuntimeGameActorEffectData effectData,
    float elapsedTime)
  {
    return (double) effectData.accumulator <= 0.0;
  }
}
