// Decompiled with JetBrains decompiler
// Type: MachoBraceSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class MachoBraceSynergyProcessor : MonoBehaviour
{
  [LongNumericEnum]
  public CustomSynergyType RequiredSynergy;
  public float DamageMultiplier = 1.25f;
  public GameObject DustUpVFX;
  public GameObject BurstVFX;
  public GameObject OverheadVFX;
  public bool TriggersOnStandingStill;
  public float StandStillTimer = 3f;
  public bool TriggersOnTableFlip;
  public float FlipDuration = 3f;
  public bool TriggersOnAimRotation;
  private float m_lastGunAngle;
  private float m_cumulativeGunRotation;
  private float m_zeroRotationTime;
  private float m_standStillTimer;
  private PassiveItem m_item;
  private bool m_initialized;
  private PlayerController m_lastOwner;
  private bool m_hasUsedShot;
  private float m_beamTickElapsed;
  private StatModifier m_damageStat;
  private GameObject m_instanceVFX;
  private int m_destroyVFXSemaphore;

  private void Awake()
  {
    this.m_damageStat = new StatModifier();
    this.m_damageStat.statToBoost = PlayerStats.StatType.Damage;
    this.m_damageStat.modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE;
    this.m_damageStat.amount = this.DamageMultiplier;
    this.m_item = this.GetComponent<PassiveItem>();
  }

  private void Update()
  {
    if (Dungeon.IsGenerating || !PhysicsEngine.HasInstance)
      return;
    bool flag = (bool) (UnityEngine.Object) this.m_item.Owner && this.m_item.Owner.HasActiveBonusSynergy(this.RequiredSynergy);
    if (flag && !this.m_initialized)
      this.Initialize(this.m_item.Owner);
    else if (this.m_initialized && !flag)
    {
      if ((bool) (UnityEngine.Object) this.m_lastOwner)
      {
        this.m_lastOwner.PostProcessProjectile -= new Action<Projectile, float>(this.HandleProjectileFired);
        this.m_lastOwner.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.HandleBeamTick);
        this.m_lastOwner.OnTableFlipped -= new Action<FlippableCover>(this.HandleTableFlip);
      }
      this.m_initialized = false;
      this.m_lastOwner = (PlayerController) null;
    }
    else
    {
      if (!this.m_initialized || !flag)
        return;
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
      {
        this.m_hasUsedShot = true;
      }
      else
      {
        if (this.TriggersOnStandingStill && this.m_destroyVFXSemaphore <= 0 && (double) this.m_item.Owner.Velocity.magnitude < 0.05000000074505806)
        {
          this.m_standStillTimer += BraveTime.DeltaTime;
          if ((double) this.m_standStillTimer > (double) this.StandStillTimer)
          {
            this.ForceTrigger(this.m_item.Owner);
            this.m_standStillTimer = 0.0f;
          }
        }
        if (!this.TriggersOnAimRotation || !(bool) (UnityEngine.Object) this.m_item.Owner.CurrentGun)
          return;
        float f = Mathf.Clamp(Vector2.SignedAngle(BraveMathCollege.DegreesToVector((this.m_item.Owner.unadjustedAimPoint.XY() - this.m_item.Owner.CenterPosition).ToAngle()), BraveMathCollege.DegreesToVector(this.m_lastGunAngle)), -90f, 90f);
        if ((double) Mathf.Abs(f) < 120.0 * (double) BraveTime.DeltaTime)
        {
          this.m_zeroRotationTime += UnityEngine.Time.deltaTime;
          if ((double) this.m_zeroRotationTime < 0.0333000011742115)
            return;
          f = 0.0f;
          this.m_cumulativeGunRotation = 0.0f;
        }
        else
          this.m_zeroRotationTime = 0.0f;
        this.m_lastGunAngle = (this.m_item.Owner.unadjustedAimPoint.XY() - this.m_item.Owner.CenterPosition).ToAngle();
        this.m_cumulativeGunRotation += f;
        if ((double) this.m_cumulativeGunRotation > 360.0)
        {
          this.m_cumulativeGunRotation = 0.0f;
          this.ForceTrigger(this.m_item.Owner);
        }
        else
        {
          if ((double) this.m_cumulativeGunRotation >= -360.0)
            return;
          this.m_cumulativeGunRotation = 0.0f;
          this.ForceTrigger(this.m_item.Owner);
        }
      }
    }
  }

  public void Initialize(PlayerController player)
  {
    this.m_initialized = true;
    player.PostProcessProjectile += new Action<Projectile, float>(this.HandleProjectileFired);
    player.PostProcessBeamTick += new Action<BeamController, SpeculativeRigidbody, float>(this.HandleBeamTick);
    player.OnTableFlipped += new Action<FlippableCover>(this.HandleTableFlip);
    this.m_lastOwner = player;
    if (!(bool) (UnityEngine.Object) player.CurrentGun)
      return;
    this.m_lastGunAngle = player.CurrentGun.CurrentAngle;
  }

  private void HandleTableFlip(FlippableCover obj)
  {
    if (!this.TriggersOnTableFlip || !(bool) (UnityEngine.Object) this.m_item.Owner)
      return;
    this.ForceTrigger(this.m_item.Owner);
  }

  private void HandleBeamTick(BeamController arg1, SpeculativeRigidbody arg2, float arg3)
  {
    if (!(bool) (UnityEngine.Object) this.m_item.Owner || this.m_hasUsedShot)
      return;
    this.m_beamTickElapsed += BraveTime.DeltaTime;
    if ((double) this.m_beamTickElapsed <= 1.0)
      return;
    this.m_hasUsedShot = true;
  }

  private void HandleProjectileFired(Projectile firedProjectile, float arg2)
  {
    if (!(bool) (UnityEngine.Object) this.m_item.Owner || this.m_destroyVFXSemaphore <= 0)
      return;
    firedProjectile.AdjustPlayerProjectileTint(new Color(1f, 0.9f, 0.0f), 50);
    if (this.m_hasUsedShot)
      return;
    this.m_hasUsedShot = true;
    if ((bool) (UnityEngine.Object) this.m_item.Owner && (bool) (UnityEngine.Object) this.DustUpVFX)
    {
      this.m_item.Owner.PlayEffectOnActor(this.DustUpVFX, new Vector3(0.0f, -0.625f, 0.0f), false);
      int num = (int) AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Trigger_01", this.gameObject);
    }
    if (!(bool) (UnityEngine.Object) this.m_item.Owner || !(bool) (UnityEngine.Object) this.BurstVFX)
      return;
    this.m_item.Owner.PlayEffectOnActor(this.BurstVFX, new Vector3(0.0f, 0.375f, 0.0f), false);
  }

  public void EnableVFX(PlayerController target)
  {
    if (this.m_destroyVFXSemaphore != 0)
      return;
    Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(target.sprite);
    if ((UnityEngine.Object) outlineMaterial != (UnityEngine.Object) null)
      outlineMaterial.SetColor("_OverrideColor", new Color(99f, 99f, 0.0f));
    if (!(bool) (UnityEngine.Object) this.OverheadVFX || (bool) (UnityEngine.Object) this.m_instanceVFX)
      return;
    this.m_instanceVFX = target.PlayEffectOnActor(this.OverheadVFX, new Vector3(0.0f, 1.375f, 0.0f), alreadyMiddleCenter: true);
  }

  public void DisableVFX(PlayerController target)
  {
    if (this.m_destroyVFXSemaphore != 0)
      return;
    Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(target.sprite);
    if ((UnityEngine.Object) outlineMaterial != (UnityEngine.Object) null)
      outlineMaterial.SetColor("_OverrideColor", new Color(0.0f, 0.0f, 0.0f));
    if (this.m_hasUsedShot)
      ;
    if (!(bool) (UnityEngine.Object) this.m_instanceVFX)
      return;
    SpawnManager.Despawn(this.m_instanceVFX);
    this.m_instanceVFX = (GameObject) null;
  }

  public void ForceTrigger(PlayerController target)
  {
    target.StartCoroutine(this.HandleDamageBoost(target));
  }

  [DebuggerHidden]
  private IEnumerator HandleDamageBoost(PlayerController target)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new MachoBraceSynergyProcessor.\u003CHandleDamageBoost\u003Ec__Iterator0()
    {
      target = target,
      \u0024this = this
    };
  }
}
