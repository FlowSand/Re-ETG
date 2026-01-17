// Decompiled with JetBrains decompiler
// Type: PlayerOrbital
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class PlayerOrbital : BraveBehaviour, IPlayerOrbital
    {
      public PlayerOrbital.SpecialOrbitalIdentifier SpecialID;
      public PlayerOrbital.OrbitalMotionStyle motionStyle;
      public Projectile shootProjectile;
      public int numToShoot = 1;
      public float shootCooldown = 1f;
      public float orbitRadius = 3f;
      public float orbitDegreesPerSecond = 90f;
      public bool shouldRotate = true;
      public float perfectOrbitalFactor;
      public bool DamagesEnemiesOnShot;
      public float DamageToEnemiesOnShot = 10f;
      public float DamageToEnemiesOnShotCooldown = 3f;
      private float m_damageOnShotCooldown;
      public bool TriggersMachoBraceOnShot;
      public bool PreventOutline;
      public string IdleAnimation;
      [Header("Synergies")]
      public PlayerOrbitalSynergyData[] synergies;
      public bool ExplodesOnTriggerCollision;
      public ExplosionData TriggerExplosionData;
      private bool m_initialized;
      private PlayerController m_owner;
      private AIActor m_currentTarget;
      private float m_currentAngle;
      private float m_shootTimer;
      private float m_retargetTimer;
      private int m_orbitalTier;
      private int m_orbitalTierIndex;
      private Vector2 m_ownerCenterAverage;
      private bool m_hasLuteBuff;
      private GameObject m_luteOverheadVfx;
      [NonSerialized]
      public PlayerOrbitalItem SourceItem;
      private float m_lastExplosionTime;
      public float SinWavelength = 3f;
      public float SinAmplitude = 1f;

      public PlayerController Owner => this.m_owner;

      public static int GetNumberOfOrbitalsInTier(PlayerController owner, int tier)
      {
        int ofOrbitalsInTier = 0;
        for (int index = 0; index < owner.orbitals.Count; ++index)
        {
          if (owner.orbitals[index].GetOrbitalTier() == tier)
            ++ofOrbitalsInTier;
        }
        return ofOrbitalsInTier;
      }

      public static int CalculateTargetTier(PlayerController owner, IPlayerOrbital orbital)
      {
        float orbitalRadius1 = orbital.GetOrbitalRadius();
        float orbitalRotationalSpeed1 = orbital.GetOrbitalRotationalSpeed();
        int a = -1;
        for (int index = 0; index < owner.orbitals.Count; ++index)
        {
          if (owner.orbitals[index] != orbital)
          {
            a = Mathf.Max(a, owner.orbitals[index].GetOrbitalTier());
            float orbitalRadius2 = owner.orbitals[index].GetOrbitalRadius();
            float orbitalRotationalSpeed2 = owner.orbitals[index].GetOrbitalRotationalSpeed();
            if (Mathf.Approximately(orbitalRadius2, orbitalRadius1) && Mathf.Approximately(orbitalRotationalSpeed2, orbitalRotationalSpeed1))
              return owner.orbitals[index].GetOrbitalTier();
          }
        }
        return a + 1;
      }

      public void Initialize(PlayerController owner)
      {
        this.m_initialized = true;
        this.m_owner = owner;
        this.SetOrbitalTier(PlayerOrbital.CalculateTargetTier(owner, (IPlayerOrbital) this));
        this.SetOrbitalTierIndex(PlayerOrbital.GetNumberOfOrbitalsInTier(owner, this.m_orbitalTier));
        UnityEngine.Debug.LogError((object) $"new orbital tier: {(object) this.GetOrbitalTier()} and index: {(object) this.GetOrbitalTierIndex()}");
        owner.orbitals.Add((IPlayerOrbital) this);
        this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
        this.spriteAnimator = this.GetComponentInChildren<tk2dSpriteAnimator>();
        if (!this.PreventOutline)
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
        this.m_ownerCenterAverage = this.m_owner.CenterPosition;
        if ((bool) (UnityEngine.Object) this.specRigidbody && (this.DamagesEnemiesOnShot || this.TriggersMachoBraceOnShot))
          this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
        if (!(bool) (UnityEngine.Object) this.specRigidbody || !this.ExplodesOnTriggerCollision)
          return;
        this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerCollisionExplosion);
      }

      private void HandleTriggerCollisionExplosion(
        SpeculativeRigidbody otherRigidbody,
        SpeculativeRigidbody sourceSpecRigidbody,
        CollisionData collisionData)
      {
        if (!(bool) (UnityEngine.Object) otherRigidbody || !(bool) (UnityEngine.Object) otherRigidbody.aiActor || (double) UnityEngine.Time.time - (double) this.m_lastExplosionTime <= 5.0)
          return;
        this.m_lastExplosionTime = UnityEngine.Time.time;
        Exploder.Explode((Vector3) this.specRigidbody.UnitCenter, this.TriggerExplosionData, Vector2.zero);
        this.Disappear();
      }

      private void Disappear()
      {
        this.specRigidbody.enabled = false;
        SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, false);
        this.sprite.renderer.enabled = false;
      }

      private void Reappear()
      {
        this.specRigidbody.enabled = true;
        this.sprite.renderer.enabled = true;
        SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, true);
        this.specRigidbody.Reinitialize();
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
      }

      public void DecoupleBabyDragun()
      {
        if ((bool) (UnityEngine.Object) this.SourceItem)
        {
          this.SourceItem.DecoupleOrbital();
          this.m_owner.RemovePassiveItem(this.SourceItem.PickupObjectId);
        }
        this.m_owner.orbitals.Remove((IPlayerOrbital) this);
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          this.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
      }

      private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (!(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.projectile)
          return;
        if (this.DamagesEnemiesOnShot && (double) this.m_damageOnShotCooldown <= 0.0)
        {
          if ((bool) (UnityEngine.Object) this.m_owner)
          {
            this.StartCoroutine(this.FlashSprite(this.sprite));
            this.m_owner.CurrentRoom.ApplyActionToNearbyEnemies(this.m_owner.CenterPosition, 100f, (Action<AIActor, float>) ((enemy, dist) =>
            {
              if (!(bool) (UnityEngine.Object) enemy || !(bool) (UnityEngine.Object) enemy.healthHaver)
                return;
              enemy.healthHaver.ApplyDamage(this.DamageToEnemiesOnShot, Vector2.zero, string.Empty);
            }));
          }
          this.m_damageOnShotCooldown = this.DamageToEnemiesOnShotCooldown;
        }
        if (!this.TriggersMachoBraceOnShot || !(bool) (UnityEngine.Object) this.m_owner)
          return;
        for (int index = 0; index < this.m_owner.passiveItems.Count; ++index)
        {
          if (this.m_owner.passiveItems[index] is MachoBraceItem)
          {
            (this.m_owner.passiveItems[index] as MachoBraceItem).ForceTrigger(this.m_owner);
            break;
          }
        }
      }

      [DebuggerHidden]
      private IEnumerator FlashSprite(tk2dBaseSprite targetSprite, float flashTime = 1f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PlayerOrbital__FlashSpritec__Iterator0()
        {
          targetSprite = targetSprite,
          flashTime = flashTime
        };
      }

      private void Update()
      {
        if (!this.m_initialized)
          return;
        if (this.ExplodesOnTriggerCollision && !this.specRigidbody.enabled && (double) UnityEngine.Time.time - (double) this.m_lastExplosionTime > 5.0)
          this.Reappear();
        this.HandleMotion();
        this.HandleCombat();
        bool flag1 = false;
        bool flag2 = false;
        for (int index = 0; index < this.synergies.Length; ++index)
        {
          PlayerOrbitalSynergyData synergy = this.synergies[index];
          flag1 |= synergy.HasOverrideAnimations;
          if (synergy.HasOverrideAnimations && this.m_owner.HasActiveBonusSynergy(synergy.SynergyToCheck))
          {
            flag2 = true;
            if (!this.spriteAnimator.IsPlaying(synergy.OverrideIdleAnimation))
              this.spriteAnimator.Play(synergy.OverrideIdleAnimation);
          }
        }
        if (flag1 && !flag2 && !string.IsNullOrEmpty(this.IdleAnimation) && !this.spriteAnimator.IsPlaying(this.IdleAnimation))
          this.spriteAnimator.Play(this.IdleAnimation);
        if (this.motionStyle != PlayerOrbital.OrbitalMotionStyle.ORBIT_TARGET)
          this.m_retargetTimer -= BraveTime.DeltaTime;
        if ((bool) (UnityEngine.Object) this.shootProjectile && (bool) (UnityEngine.Object) this.specRigidbody)
        {
          if (this.m_hasLuteBuff && (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun || !this.m_owner.CurrentGun.LuteCompanionBuffActive))
          {
            if ((bool) (UnityEngine.Object) this.m_luteOverheadVfx)
            {
              UnityEngine.Object.Destroy((UnityEngine.Object) this.m_luteOverheadVfx);
              this.m_luteOverheadVfx = (GameObject) null;
            }
            if ((bool) (UnityEngine.Object) this.specRigidbody)
              this.specRigidbody.OnPostRigidbodyMovement -= new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.UpdateVFXOnMovement);
            this.m_hasLuteBuff = false;
          }
          else if (!this.m_hasLuteBuff && (bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.LuteCompanionBuffActive)
          {
            this.m_luteOverheadVfx = SpawnManager.SpawnVFX((GameObject) ResourceCache.Acquire("Global VFX/VFX_Buff_Status"), this.specRigidbody.UnitCenter.ToVector3ZisY().Quantize(1f / 16f) + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
            if ((bool) (UnityEngine.Object) this.specRigidbody)
              this.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.UpdateVFXOnMovement);
            this.m_hasLuteBuff = true;
          }
        }
        this.m_damageOnShotCooldown -= BraveTime.DeltaTime;
        this.m_shootTimer -= BraveTime.DeltaTime;
      }

      private void UpdateVFXOnMovement(SpeculativeRigidbody arg1, Vector2 arg2, IntVector2 arg3)
      {
        if (!this.m_hasLuteBuff || !(bool) (UnityEngine.Object) this.m_luteOverheadVfx)
          return;
        this.m_luteOverheadVfx.transform.position = this.specRigidbody.UnitCenter.ToVector3ZisY().Quantize(1f / 16f) + new Vector3(0.0f, 1f, 0.0f);
      }

      protected override void OnDestroy()
      {
        for (int index = 0; index < this.m_owner.orbitals.Count; ++index)
        {
          if (this.m_owner.orbitals[index].GetOrbitalTier() == this.GetOrbitalTier() && this.m_owner.orbitals[index].GetOrbitalTierIndex() > this.GetOrbitalTierIndex())
            this.m_owner.orbitals[index].SetOrbitalTierIndex(this.m_owner.orbitals[index].GetOrbitalTierIndex() - 1);
        }
        this.m_owner.orbitals.Remove((IPlayerOrbital) this);
      }

      public void Reinitialize()
      {
        this.specRigidbody.Reinitialize();
        this.m_ownerCenterAverage = this.m_owner.CenterPosition;
      }

      public void ReinitializeWithDelta(Vector2 delta)
      {
        this.specRigidbody.Reinitialize();
        this.m_ownerCenterAverage += delta;
      }

      private void HandleMotion()
      {
        Vector2 centerPosition = this.m_owner.CenterPosition;
        if ((double) Vector2.Distance(centerPosition, this.transform.position.XY()) > 20.0)
        {
          this.transform.position = centerPosition.ToVector3ZUp();
          this.specRigidbody.Reinitialize();
        }
        if (this.motionStyle == PlayerOrbital.OrbitalMotionStyle.ORBIT_TARGET && (UnityEngine.Object) this.m_currentTarget != (UnityEngine.Object) null)
          centerPosition = this.m_currentTarget.CenterPosition;
        Vector2 vector2_1 = centerPosition - this.m_ownerCenterAverage;
        float num1 = Mathf.Min(Mathf.Lerp(0.1f, 15f, vector2_1.magnitude / 4f) * BraveTime.DeltaTime, vector2_1.magnitude);
        float z = (float) (360.0 / (double) PlayerOrbital.GetNumberOfOrbitalsInTier(this.m_owner, this.GetOrbitalTier()) * (double) this.GetOrbitalTierIndex() + (double) BraveTime.ScaledTimeSinceStartup * (double) this.orbitDegreesPerSecond);
        Vector2 vector2_2 = Vector2.Lerp(this.m_ownerCenterAverage + (centerPosition - this.m_ownerCenterAverage).normalized * num1, centerPosition, this.perfectOrbitalFactor);
        Vector2 vector = vector2_2 + (Quaternion.Euler(0.0f, 0.0f, z) * Vector3.right * this.orbitRadius).XY();
        if (this.SpecialID == PlayerOrbital.SpecialOrbitalIdentifier.BABY_DRAGUN)
        {
          float num2 = Mathf.Sin(UnityEngine.Time.time * this.SinWavelength) * this.SinAmplitude;
          vector += (Quaternion.Euler(0.0f, 0.0f, z) * Vector3.right).XY().normalized * num2;
        }
        this.m_ownerCenterAverage = vector2_2;
        this.specRigidbody.Velocity = (vector.Quantize(1f / 16f) - this.transform.position.XY()) / BraveTime.DeltaTime;
        this.m_currentAngle = z % 360f;
        if (!this.shouldRotate)
          return;
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, this.m_currentAngle);
      }

      private void AcquireTarget()
      {
        this.m_retargetTimer = 0.25f;
        this.m_currentTarget = (AIActor) null;
        if ((UnityEngine.Object) this.m_owner == (UnityEngine.Object) null || this.m_owner.CurrentRoom == null)
          return;
        List<AIActor> activeEnemies = this.m_owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies == null || activeEnemies.Count <= 0)
          return;
        AIActor aiActor1 = (AIActor) null;
        float num1 = -1f;
        for (int index = 0; index < activeEnemies.Count; ++index)
        {
          AIActor aiActor2 = activeEnemies[index];
          if ((bool) (UnityEngine.Object) aiActor2 && aiActor2.HasBeenEngaged && aiActor2.IsWorthShootingAt)
          {
            float num2 = Vector2.Distance(this.transform.position.XY(), aiActor2.specRigidbody.UnitCenter);
            if ((UnityEngine.Object) aiActor1 == (UnityEngine.Object) null || (double) num2 < (double) num1)
            {
              aiActor1 = aiActor2;
              num1 = num2;
            }
          }
        }
        if (!(bool) (UnityEngine.Object) aiActor1)
          return;
        this.m_currentTarget = aiActor1;
      }

      private Projectile GetProjectile()
      {
        Projectile projectile = this.shootProjectile;
        for (int index = 0; index < this.synergies.Length; ++index)
        {
          PlayerOrbitalSynergyData synergy = this.synergies[index];
          if ((bool) (UnityEngine.Object) synergy.OverrideProjectile && this.m_owner.HasActiveBonusSynergy(synergy.SynergyToCheck))
            projectile = synergy.OverrideProjectile;
        }
        return projectile;
      }

      private Vector2 FindPredictedTargetPosition()
      {
        float num1 = this.GetProjectile().baseData.speed;
        if ((double) num1 < 0.0)
          num1 = float.MaxValue;
        Vector2 a = this.transform.position.XY();
        Vector2 b = this.m_currentTarget.specRigidbody.HitboxPixelCollider == null ? this.m_currentTarget.specRigidbody.UnitCenter : this.m_currentTarget.specRigidbody.HitboxPixelCollider.UnitCenter;
        float num2 = Vector2.Distance(a, b) / num1;
        return b + this.m_currentTarget.specRigidbody.Velocity * num2;
      }

      private void Shoot(Vector2 targetPosition, Vector2 startOffset)
      {
        Vector2 position = this.transform.position.XY() + startOffset;
        Vector2 vector2 = targetPosition - position;
        float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
        Projectile component = SpawnManager.SpawnProjectile(this.GetProjectile().gameObject, (Vector3) position, Quaternion.Euler(0.0f, 0.0f, z)).GetComponent<Projectile>();
        component.collidesWithEnemies = true;
        component.collidesWithPlayer = false;
        component.Owner = (GameActor) this.m_owner;
        component.Shooter = this.m_owner.specRigidbody;
        component.TreatedAsNonProjectileForChallenge = true;
        if (!(bool) (UnityEngine.Object) this.m_owner)
          return;
        if (PassiveItem.IsFlagSetForCharacter(this.m_owner, typeof (BattleStandardItem)))
          component.baseData.damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
        if ((bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.LuteCompanionBuffActive)
        {
          component.baseData.damage *= 2f;
          component.RuntimeUpdateScale(1.75f);
        }
        this.m_owner.DoPostProcessProjectile(component);
      }

      public void ToggleRenderer(bool value)
      {
        this.sprite.renderer.enabled = value;
        if (this.PreventOutline)
          return;
        SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, value);
      }

      private int GetNumberToFire()
      {
        int numToShoot = this.numToShoot;
        if (this.synergies != null && (bool) (UnityEngine.Object) this.m_owner)
        {
          for (int index = 0; index < this.synergies.Length; ++index)
          {
            if (this.m_owner.HasActiveBonusSynergy(this.synergies[index].SynergyToCheck))
              numToShoot += this.synergies[index].AdditionalShots;
          }
        }
        return numToShoot;
      }

      private float GetModifiedCooldown()
      {
        float shootCooldown = this.shootCooldown;
        if (this.synergies != null && (bool) (UnityEngine.Object) this.m_owner)
        {
          for (int index = 0; index < this.synergies.Length; ++index)
          {
            if (this.m_owner.HasActiveBonusSynergy(this.synergies[index].SynergyToCheck))
              shootCooldown *= this.synergies[index].ShootCooldownMultiplier;
          }
        }
        if ((bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.LuteCompanionBuffActive)
          shootCooldown /= 1.5f;
        return shootCooldown;
      }

      private void HandleCombat()
      {
        if (GameManager.Instance.IsPaused || !(bool) (UnityEngine.Object) this.m_owner || this.m_owner.CurrentInputState != PlayerInputState.AllInput || this.m_owner.IsInputOverridden || (UnityEngine.Object) this.shootProjectile == (UnityEngine.Object) null)
          return;
        if ((double) this.m_retargetTimer <= 0.0)
          this.m_currentTarget = (AIActor) null;
        if ((UnityEngine.Object) this.m_currentTarget == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) this.m_currentTarget || this.m_currentTarget.healthHaver.IsDead)
          this.AcquireTarget();
        if ((UnityEngine.Object) this.m_currentTarget == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) this.m_currentTarget)
          return;
        if ((double) this.m_shootTimer <= 0.0)
        {
          this.m_shootTimer = this.GetModifiedCooldown();
          Vector2 predictedTargetPosition = this.FindPredictedTargetPosition();
          if (!this.m_owner.IsStealthed)
          {
            int numberToFire = this.GetNumberToFire();
            for (int index = 0; index < numberToFire; ++index)
            {
              Vector2 startOffset = Vector2.zero;
              if (index > 0)
                startOffset = UnityEngine.Random.insideUnitCircle.normalized;
              this.Shoot(predictedTargetPosition + startOffset, startOffset);
            }
          }
        }
        if (!this.shouldRotate)
          return;
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(this.m_currentTarget.CenterPosition - this.transform.position.XY()) - 90f);
      }

      public Transform GetTransform() => this.transform;

      public int GetOrbitalTier() => this.m_orbitalTier;

      public void SetOrbitalTier(int tier) => this.m_orbitalTier = tier;

      public int GetOrbitalTierIndex() => this.m_orbitalTierIndex;

      public void SetOrbitalTierIndex(int tierIndex) => this.m_orbitalTierIndex = tierIndex;

      public float GetOrbitalRadius() => this.orbitRadius;

      public float GetOrbitalRotationalSpeed() => this.orbitDegreesPerSecond;

      public enum SpecialOrbitalIdentifier
      {
        NONE,
        BABY_DRAGUN,
      }

      public enum OrbitalMotionStyle
      {
        ORBIT_PLAYER_ALWAYS,
        ORBIT_TARGET,
      }
    }

}
