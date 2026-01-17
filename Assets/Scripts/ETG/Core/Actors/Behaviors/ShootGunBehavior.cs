// Decompiled with JetBrains decompiler
// Type: ShootGunBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class ShootGunBehavior : BasicAttackBehavior
    {
      [InspectorCategory("Conditions")]
      public float GroupCooldownVariance = 0.2f;
      [InspectorCategory("Conditions")]
      public bool LineOfSight = true;
      public WeaponType WeaponType;
      [InspectorIndent]
      [InspectorShowIf("IsAiShooter")]
      public string OverrideBulletName;
      [InspectorShowIf("IsBulletScript")]
      [InspectorIndent]
      public BulletScriptSelector BulletScript;
      [InspectorShowIf("IsComplexBullet")]
      [InspectorIndent]
      public bool FixTargetDuringAttack;
      public bool StopDuringAttack;
      public float LeadAmount;
      [InspectorShowIf("ShowLeadChance")]
      [InspectorIndent]
      public float LeadChance = 1f;
      public bool RespectReload;
      [InspectorIndent]
      [InspectorShowIf("RespectReload")]
      public float MagazineCapacity = 1f;
      [InspectorShowIf("RespectReload")]
      [InspectorIndent]
      public float ReloadSpeed = 1f;
      [InspectorIndent]
      [InspectorShowIf("RespectReload")]
      public bool EmptiesClip = true;
      [InspectorIndent]
      [InspectorShowIf("RespectReload")]
      public bool SuppressReloadAnim;
      [InspectorIndent]
      [InspectorShowIf("ShowTimeBetweenShots")]
      public float TimeBetweenShots = -1f;
      public bool PreventTargetSwitching;
      [InspectorCategory("Visuals")]
      public string OverrideAnimation;
      [InspectorCategory("Visuals")]
      public string OverrideDirectionalAnimation;
      [InspectorShowIf("IsComplexBullet")]
      [InspectorCategory("Visuals")]
      public bool HideGun;
      [InspectorCategory("Visuals")]
      public bool UseLaserSight;
      [InspectorShowIf("UseLaserSight")]
      [InspectorCategory("Visuals")]
      public bool UseGreenLaser;
      [InspectorShowIf("UseLaserSight")]
      [InspectorCategory("Visuals")]
      public float PreFireLaserTime = -1f;
      [InspectorCategory("Visuals")]
      public bool AimAtFacingDirectionWhenSafe;
      private ShootGunBehavior.State m_state;
      private LaserSightController m_laserSight;
      private float m_remainingAmmo;
      private float m_reloadTimer;
      private float m_prefireLaserTimer;
      private float m_nextShotTimer;
      private float m_preFireTime;
      private float m_timeSinceLastShot;

      private bool IsAiShooter() => this.WeaponType == WeaponType.AIShooterProjectile;

      private bool IsBulletScript() => this.WeaponType == WeaponType.BulletScript;

      private bool IsComplexBullet() => this.WeaponType != WeaponType.AIShooterProjectile;

      private bool ShowLeadChance() => (double) this.LeadAmount != 0.0;

      private bool ShowTimeBetweenShots() => this.RespectReload && this.EmptiesClip;

      public override void Start()
      {
        base.Start();
        this.m_remainingAmmo = this.MagazineCapacity;
        if (this.UseLaserSight)
        {
          if (this.UseGreenLaser)
            this.m_aiActor.CurrentGun.LaserSightIsGreen = true;
          this.m_aiActor.CurrentGun.ForceLaserSight = true;
        }
        Gun byId = PickupObjectDatabase.GetById(this.m_aiShooter.equippedGunId) as Gun;
        if ((bool) (UnityEngine.Object) byId && !string.IsNullOrEmpty(byId.enemyPreFireAnimation))
          this.m_preFireTime = byId.spriteAnimator.GetClipByName(byId.enemyPreFireAnimation).BaseClipLength;
        if (!this.UseLaserSight)
          return;
        PhysicsEngine.Instance.OnPostRigidbodyMovement += new System.Action(this.OnPostRigidbodyMovement);
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_nextShotTimer);
        this.DecrementTimer(ref this.m_reloadTimer);
        this.DecrementTimer(ref this.m_prefireLaserTimer);
        this.m_timeSinceLastShot += this.m_deltaTime;
        if (this.UseLaserSight && !(bool) (UnityEngine.Object) this.m_laserSight && (bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.CurrentGun && (bool) (UnityEngine.Object) this.m_aiActor.CurrentGun.LaserSight)
        {
          this.m_laserSight = this.m_aiActor.CurrentGun.LaserSight.GetComponent<LaserSightController>();
          if ((double) this.PreFireLaserTime > 0.0 && this.m_state != ShootGunBehavior.State.PreFireLaser)
            this.m_laserSight.renderer.enabled = false;
        }
        if (!this.AimAtFacingDirectionWhenSafe || !((UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody == (UnityEngine.Object) null))
          return;
        this.m_aiShooter.AimInDirection(BraveMathCollege.DegreesToVector(this.m_aiAnimator.FacingDirection));
      }

      public override BehaviorResult Update()
      {
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady() || (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody == (UnityEngine.Object) null)
          return BehaviorResult.Continue;
        bool flag1 = this.RespectReload && (double) this.m_reloadTimer > 0.0;
        bool flag2 = this.EmptiesClip && (double) this.m_remainingAmmo < (double) this.MagazineCapacity;
        bool flag3 = (double) this.Range > 0.0 && (double) this.m_aiActor.DistanceToTarget > (double) this.Range && !flag2;
        bool flag4 = this.LineOfSight && !this.m_aiActor.HasLineOfSightToTarget && !flag2;
        if (flag1 || (UnityEngine.Object) this.m_aiActor.TargetRigidbody == (UnityEngine.Object) null || flag3 || flag4)
        {
          this.m_aiShooter.CeaseAttack();
          return BehaviorResult.Continue;
        }
        this.BeginAttack();
        if (this.PreventTargetSwitching)
          this.m_aiActor.SuppressTargetSwitch = true;
        this.m_updateEveryFrame = true;
        return this.StopDuringAttack ? BehaviorResult.RunContinuous : BehaviorResult.RunContinuousInClass;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if (this.m_state == ShootGunBehavior.State.Idle)
          return ContinuousBehaviorResult.Finished;
        bool flag = (double) this.LeadAmount > 0.0 && (double) this.LeadChance >= 1.0;
        if (this.m_state == ShootGunBehavior.State.PreFireLaser && this.UseLaserSight && (double) this.m_prefireLaserTimer > 0.0)
          flag = false;
        if ((UnityEngine.Object) this.m_aiShooter.CurrentGun != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        {
          Vector2 a = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
          if (flag)
          {
            Vector2 predictedTargetPosition = this.FindPredictedTargetPosition();
            a = Vector2.Lerp(a, predictedTargetPosition, this.LeadAmount);
          }
          this.m_aiShooter.OverrideAimPoint = new Vector2?(a);
        }
        if (this.m_state == ShootGunBehavior.State.WaitingForNextShot)
        {
          if ((double) this.m_nextShotTimer <= 0.0)
            this.BeginAttack();
        }
        else if (this.m_state == ShootGunBehavior.State.PreFireLaser)
        {
          if (this.UseLaserSight && (bool) (UnityEngine.Object) this.m_laserSight && (double) this.PreFireLaserTime > 0.0)
          {
            this.m_laserSight.renderer.enabled = true;
            this.m_laserSight.UpdateCountdown(this.m_prefireLaserTimer, this.PreFireLaserTime);
          }
          if ((double) this.m_prefireLaserTimer <= 0.0)
          {
            if (this.UseLaserSight && (bool) (UnityEngine.Object) this.m_laserSight && (double) this.PreFireLaserTime > 0.0)
              this.m_laserSight.ResetCountdown();
            this.m_state = ShootGunBehavior.State.PreFire;
            this.m_aiShooter.StartPreFireAnim();
          }
        }
        else if (this.m_state == ShootGunBehavior.State.PreFire)
        {
          if (this.m_aiShooter.IsPreFireComplete)
            this.Fire();
        }
        else if (this.m_state == ShootGunBehavior.State.Firing && this.IsBulletSourceEnded())
        {
          if (this.FixTargetDuringAttack)
            this.m_aiActor.bulletBank.FixedPlayerPosition = new Vector2?();
          if (!this.RespectReload || !this.EmptiesClip || (double) this.m_reloadTimer > 0.0)
            return ContinuousBehaviorResult.Finished;
          this.m_state = ShootGunBehavior.State.WaitingForNextShot;
          this.m_nextShotTimer = (double) this.TimeBetweenShots <= 0.0 ? this.Cooldown : this.TimeBetweenShots;
        }
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        this.m_updateEveryFrame = false;
        this.m_state = ShootGunBehavior.State.Idle;
        if (this.HideGun)
          this.m_aiShooter.ToggleGunRenderers(true, nameof (ShootGunBehavior));
        this.m_aiShooter.OverrideAimPoint = new Vector2?();
        if (this.FixTargetDuringAttack)
          this.m_aiActor.bulletBank.FixedPlayerPosition = new Vector2?();
        if (this.PreventTargetSwitching)
          this.m_aiActor.SuppressTargetSwitch = false;
        if (!string.IsNullOrEmpty(this.OverrideDirectionalAnimation))
          this.m_aiAnimator.EndAnimationIf(this.OverrideDirectionalAnimation);
        else if (!string.IsNullOrEmpty(this.OverrideAnimation))
          this.m_aiAnimator.EndAnimationIf(this.OverrideAnimation);
        if (this.UseLaserSight && (bool) (UnityEngine.Object) this.m_laserSight)
          this.m_laserSight.ResetCountdown();
        this.UpdateCooldowns();
      }

      public override bool IsReady()
      {
        return base.IsReady() && (!this.RespectReload || (double) this.m_reloadTimer <= 0.0);
      }

      protected override void UpdateCooldowns()
      {
        base.UpdateCooldowns();
        if ((double) this.GroupCooldownVariance > 0.0)
        {
          List<AIActor> activeEnemies = this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          for (int index = 0; index < activeEnemies.Count; ++index)
          {
            if (!((UnityEngine.Object) activeEnemies[index] == (UnityEngine.Object) this.m_aiActor) && (double) (activeEnemies[index].specRigidbody.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter).sqrMagnitude < 6.25)
            {
              this.m_cooldownTimer += UnityEngine.Random.value * this.GroupCooldownVariance;
              break;
            }
          }
        }
        if ((double) this.m_preFireTime >= (double) this.Cooldown)
          return;
        this.m_cooldownTimer = Mathf.Max(0.0f, this.m_cooldownTimer - this.m_preFireTime);
      }

      private Vector2 FindPredictedTargetPosition()
      {
        AIBulletBank.Entry bulletEntry = this.m_aiShooter.GetBulletEntry(this.OverrideBulletName);
        float firingSpeed = !bulletEntry.OverrideProjectile ? bulletEntry.BulletObject.GetComponent<Projectile>().baseData.speed : bulletEntry.ProjectileData.speed;
        if ((double) firingSpeed < 0.0)
          firingSpeed = float.MaxValue;
        return BraveMathCollege.GetPredictedPosition(this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox), this.m_aiActor.TargetVelocity, this.m_aiActor.specRigidbody.UnitCenter, firingSpeed);
      }

      private bool IsBulletSourceEnded()
      {
        return this.WeaponType != WeaponType.BulletScript || this.m_aiShooter.BraveBulletSource.IsEnded;
      }

      private void BeginAttack()
      {
        if (this.UseLaserSight && (double) this.PreFireLaserTime > 0.0)
        {
          this.m_state = ShootGunBehavior.State.PreFireLaser;
          this.m_prefireLaserTimer = this.PreFireLaserTime;
        }
        else if (this.ShouldPreFire)
        {
          this.m_state = ShootGunBehavior.State.PreFire;
          this.m_aiShooter.StartPreFireAnim();
        }
        else
          this.Fire();
      }

      private bool ShouldPreFire
      {
        get
        {
          return (double) this.m_preFireTime < (double) this.Cooldown || (double) this.m_timeSinceLastShot > (double) this.Cooldown * 2.0;
        }
      }

      private void Fire()
      {
        this.m_timeSinceLastShot = 0.0f;
        switch (this.WeaponType)
        {
          case WeaponType.AIShooterProjectile:
            this.HandleAIShoot();
            break;
          case WeaponType.BulletScript:
            this.m_aiShooter.ShootBulletScript(this.BulletScript);
            break;
        }
        if (this.RespectReload)
        {
          --this.m_remainingAmmo;
          if ((double) this.m_remainingAmmo == 0.0)
          {
            this.m_remainingAmmo = this.MagazineCapacity;
            this.m_reloadTimer = this.ReloadSpeed;
            if (!this.SuppressReloadAnim)
              this.m_aiShooter.Reload();
          }
        }
        if (!string.IsNullOrEmpty(this.OverrideDirectionalAnimation))
          this.m_aiAnimator.PlayUntilFinished(this.OverrideDirectionalAnimation, true);
        else if (!string.IsNullOrEmpty(this.OverrideAnimation))
          this.m_aiAnimator.PlayUntilFinished(this.OverrideAnimation);
        if (this.IsComplexBullet())
        {
          if (this.StopDuringAttack)
            this.m_aiActor.ClearPath();
          if (this.FixTargetDuringAttack && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
            this.m_aiActor.bulletBank.FixedPlayerPosition = new Vector2?(this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
          if (this.HideGun)
            this.m_aiShooter.ToggleGunRenderers(false, nameof (ShootGunBehavior));
          this.m_state = ShootGunBehavior.State.Firing;
        }
        else if (this.RespectReload && this.EmptiesClip && (double) this.m_reloadTimer <= 0.0)
        {
          this.m_state = ShootGunBehavior.State.WaitingForNextShot;
          this.m_nextShotTimer = (double) this.TimeBetweenShots <= 0.0 ? this.Cooldown : this.TimeBetweenShots;
        }
        else
          this.m_state = ShootGunBehavior.State.Idle;
      }

      private void HandleAIShoot()
      {
        if ((double) this.LeadAmount <= 0.0 || (double) this.LeadChance < 1.0 && (double) UnityEngine.Random.value >= (double) this.LeadChance)
        {
          this.m_aiShooter.ShootAtTarget(this.OverrideBulletName);
        }
        else
        {
          if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
            return;
          PixelCollider pixelCollider = this.m_aiActor.TargetRigidbody.GetPixelCollider(ColliderType.HitBox);
          Vector2 point = Vector2.Lerp(pixelCollider == null ? this.m_aiActor.TargetRigidbody.UnitCenter : pixelCollider.UnitCenter, this.FindPredictedTargetPosition(), this.LeadAmount);
          if ((UnityEngine.Object) this.m_aiShooter.CurrentGun == (UnityEngine.Object) null)
          {
            this.m_aiShooter.ShootInDirection(point - this.m_aiShooter.specRigidbody.UnitCenter);
          }
          else
          {
            this.m_aiShooter.OverrideAimPoint = new Vector2?(point);
            this.m_aiShooter.AimAtPoint(point);
            this.m_aiShooter.Shoot(this.OverrideBulletName);
            this.m_aiShooter.OverrideAimPoint = new Vector2?();
          }
        }
      }

      private void OnPostRigidbodyMovement()
      {
        if (this.m_state != ShootGunBehavior.State.PreFireLaser || !this.UseLaserSight || (double) this.m_prefireLaserTimer <= 0.0 || !((UnityEngine.Object) this.m_aiShooter.CurrentGun != (UnityEngine.Object) null) || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          return;
        this.m_aiShooter.OverrideAimPoint = new Vector2?(this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
        this.m_aiShooter.AimAtOverride();
      }

      public override void OnActorPreDeath()
      {
        if (this.UseLaserSight && PhysicsEngine.HasInstance)
          PhysicsEngine.Instance.OnPostRigidbodyMovement -= new System.Action(this.OnPostRigidbodyMovement);
        base.OnActorPreDeath();
      }

      private enum State
      {
        Idle,
        PreFireLaser,
        PreFire,
        Firing,
        WaitingForNextShot,
      }
    }

}
