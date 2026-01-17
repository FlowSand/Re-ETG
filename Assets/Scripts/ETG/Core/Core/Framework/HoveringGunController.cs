// Decompiled with JetBrains decompiler
// Type: HoveringGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class HoveringGunController : BraveBehaviour, IPlayerOrbital
    {
      public HoveringGunController.HoverPosition Position;
      public HoveringGunController.FireType Trigger;
      public HoveringGunController.AimType Aim;
      public float AimRotationAngularSpeed = 360f;
      public float ShootDuration = 2f;
      public float CooldownTime = 1f;
      public bool OnlyOnEmptyReload;
      public bool ConsumesTargetGunAmmo;
      public float ChanceToConsumeTargetGunAmmo = 1f;
      public string ShootAudioEvent;
      public string OnEveryShotAudioEvent;
      public string FinishedShootingAudioEvent;
      private bool m_initialized;
      private Transform m_parentTransform;
      private Transform m_shootPointTransform;
      private Gun m_targetGun;
      private PlayerController m_owner;
      private float m_currentAimTarget;
      private bool m_hasEnemyTarget;
      private float m_fireCooldown;
      private Vector2 m_ownerCenterAverage;
      private float m_orbitalAngle;
      private int m_orbitalTier;
      private int m_orbitalTierIndex;

      public void Initialize(Gun targetGun, PlayerController owner)
      {
        this.m_targetGun = targetGun;
        this.m_owner = owner;
        this.m_parentTransform = new GameObject("hover rotator").transform;
        this.m_parentTransform.parent = this.transform.parent;
        this.transform.parent = this.m_parentTransform;
        this.sprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
        this.sprite.SetSprite(targetGun.sprite.Collection, targetGun.sprite.spriteId);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
        this.m_shootPointTransform = new GameObject("shoot point").transform;
        this.m_shootPointTransform.parent = this.transform;
        this.m_shootPointTransform.localPosition = targetGun.barrelOffset.localPosition;
        if (this.Position == HoveringGunController.HoverPosition.CIRCULATE)
        {
          this.SetOrbitalTier(PlayerOrbital.CalculateTargetTier(this.m_owner, (IPlayerOrbital) this));
          this.SetOrbitalTierIndex(PlayerOrbital.GetNumberOfOrbitalsInTier(this.m_owner, this.GetOrbitalTier()));
          this.m_owner.orbitals.Add((IPlayerOrbital) this);
          this.m_ownerCenterAverage = this.m_owner.CenterPosition;
        }
        if (this.Trigger == HoveringGunController.FireType.ON_DODGED_BULLET)
          this.m_owner.OnDodgedProjectile += new Action<Projectile>(this.HandleDodgedProjectileFire);
        if (this.Trigger == HoveringGunController.FireType.ON_FIRED_GUN)
          this.m_owner.PostProcessProjectile += new Action<Projectile, float>(this.HandleFiredGun);
        if (this.Aim == HoveringGunController.AimType.NEAREST_ENEMY)
          this.m_fireCooldown = 0.25f;
        this.UpdatePosition();
        LootEngine.DoDefaultSynergyPoof(this.sprite.WorldCenter);
        this.m_initialized = true;
      }

      private void HandleFiredGun(Projectile arg1, float arg2)
      {
        if ((double) this.m_fireCooldown > 0.0)
          return;
        this.Fire();
      }

      private void HandleDodgedProjectileFire(Projectile sourceProjectile)
      {
        if ((double) this.m_fireCooldown > 0.0 || !sourceProjectile.collidesWithPlayer)
          return;
        this.Fire();
      }

      public void LateUpdate()
      {
        if (!this.m_initialized || Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
          return;
        this.UpdatePosition();
        this.UpdateFiring();
      }

      private void AimAt(Vector2 point, bool instant = false)
      {
        this.m_currentAimTarget = BraveMathCollege.Atan2Degrees(point - this.sprite.WorldCenter);
        if (!instant)
          return;
        this.m_parentTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, this.m_currentAimTarget);
      }

      private void UpdatePosition()
      {
        switch (this.Aim)
        {
          case HoveringGunController.AimType.NEAREST_ENEMY:
            bool flag1 = false;
            if ((bool) (UnityEngine.Object) this.m_owner && this.m_owner.CurrentRoom != null)
            {
              float nearestDistance = -1f;
              AIActor nearestEnemy = this.m_owner.CurrentRoom.GetNearestEnemy(this.m_owner.CenterPosition, out nearestDistance);
              if ((bool) (UnityEngine.Object) nearestEnemy)
              {
                this.m_hasEnemyTarget = true;
                this.AimAt(nearestEnemy.CenterPosition);
                flag1 = true;
              }
            }
            if (!flag1)
            {
              this.m_hasEnemyTarget = false;
              this.AimAt(this.m_owner.unadjustedAimPoint.XY());
              break;
            }
            break;
          case HoveringGunController.AimType.PLAYER_AIM:
            this.AimAt(this.m_owner.unadjustedAimPoint.XY());
            break;
        }
        this.m_parentTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.MoveTowardsAngle(this.m_parentTransform.localRotation.eulerAngles.z, this.m_currentAimTarget, this.AimRotationAngularSpeed * BraveTime.DeltaTime));
        bool flag2 = (double) this.m_parentTransform.localRotation.eulerAngles.z > 90.0 && (double) this.m_parentTransform.localRotation.eulerAngles.z < 270.0;
        if (flag2 && !this.sprite.FlipY)
        {
          this.transform.localPosition += new Vector3(0.0f, this.sprite.GetUntrimmedBounds().extents.y, 0.0f);
          this.m_shootPointTransform.localPosition = this.m_shootPointTransform.localPosition.WithY(-this.m_shootPointTransform.localPosition.y);
          this.sprite.FlipY = true;
        }
        else if (!flag2 && this.sprite.FlipY)
        {
          this.sprite.FlipY = false;
          this.transform.localPosition -= new Vector3(0.0f, this.sprite.GetUntrimmedBounds().extents.y, 0.0f);
          this.m_shootPointTransform.localPosition = this.m_shootPointTransform.localPosition.WithY(-this.m_shootPointTransform.localPosition.y);
        }
        switch (this.Position)
        {
          case HoveringGunController.HoverPosition.OVERHEAD:
            this.m_parentTransform.position = (this.m_owner.CenterPosition + new Vector2(0.0f, 1.5f)).ToVector3ZisY();
            this.sprite.HeightOffGround = 2f;
            this.sprite.UpdateZDepth();
            break;
          case HoveringGunController.HoverPosition.CIRCULATE:
            this.HandleOrbitalMotion();
            break;
        }
      }

      private void HandleOrbitalMotion()
      {
        Vector2 centerPosition = this.m_owner.CenterPosition;
        if ((double) Vector2.Distance(centerPosition, this.m_parentTransform.position.XY()) > 20.0)
        {
          this.m_parentTransform.position = centerPosition.ToVector3ZUp();
          this.m_ownerCenterAverage = centerPosition;
          if ((bool) (UnityEngine.Object) this.specRigidbody)
            this.specRigidbody.Reinitialize();
        }
        Vector2 vector2_1 = centerPosition - this.m_ownerCenterAverage;
        float num = Mathf.Min(Mathf.Lerp(0.1f, 15f, vector2_1.magnitude / 4f) * BraveTime.DeltaTime, vector2_1.magnitude);
        float z = (float) (360.0 / (double) PlayerOrbital.GetNumberOfOrbitalsInTier(this.m_owner, this.GetOrbitalTier()) * (double) this.GetOrbitalTierIndex() + (double) BraveTime.ScaledTimeSinceStartup * (double) this.GetOrbitalRotationalSpeed());
        Vector2 vector2_2 = this.m_ownerCenterAverage + (centerPosition - this.m_ownerCenterAverage).normalized * num;
        Vector2 vector1 = vector2_2 + (Quaternion.Euler(0.0f, 0.0f, z) * Vector3.right * this.GetOrbitalRadius()).XY();
        this.m_ownerCenterAverage = vector2_2;
        Vector2 vector2 = vector1.Quantize(1f / 16f);
        Vector2 vector2_3 = (vector2 - this.m_parentTransform.position.XY()) / BraveTime.DeltaTime;
        if ((bool) (UnityEngine.Object) this.specRigidbody)
        {
          this.specRigidbody.Velocity = vector2_3;
        }
        else
        {
          this.m_parentTransform.position = vector2.ToVector3ZisY();
          this.sprite.HeightOffGround = 0.5f;
          this.sprite.UpdateZDepth();
        }
        this.m_orbitalAngle = z % 360f;
      }

      private void UpdateFiring()
      {
        if ((double) this.m_fireCooldown <= 0.0)
        {
          switch (this.Trigger)
          {
            case HoveringGunController.FireType.ON_RELOAD:
              if (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun || !this.m_owner.CurrentGun.IsReloading || this.OnlyOnEmptyReload && this.m_owner.CurrentGun.ClipShotsRemaining > 0)
                break;
              this.Fire();
              break;
            case HoveringGunController.FireType.ON_COOLDOWN:
              if (this.Aim == HoveringGunController.AimType.NEAREST_ENEMY && !this.m_hasEnemyTarget)
                break;
              this.Fire();
              break;
          }
        }
        else
          this.m_fireCooldown = (this.m_fireCooldown -= BraveTime.DeltaTime);
      }

      private Vector2 ShootPoint => this.m_shootPointTransform.position.XY();

      private void Fire()
      {
        this.m_fireCooldown = this.CooldownTime;
        Projectile currentProjectile1 = this.m_targetGun.DefaultModule.GetCurrentProjectile();
        bool flag = (UnityEngine.Object) currentProjectile1.GetComponent<BeamController>() != (UnityEngine.Object) null;
        if (!string.IsNullOrEmpty(this.ShootAudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.ShootAudioEvent, this.gameObject);
        }
        if (flag)
        {
          this.m_owner.StartCoroutine(this.HandleFireShortBeam(currentProjectile1, this.m_owner, this.ShootDuration));
          this.m_fireCooldown = Mathf.Max(this.m_fireCooldown, this.ShootDuration);
        }
        else if ((UnityEngine.Object) this.m_targetGun.Volley != (UnityEngine.Object) null)
        {
          if ((double) this.ShootDuration > 0.0)
            this.StartCoroutine(this.FireVolleyForDuration(this.m_targetGun.Volley, this.m_owner, this.ShootDuration));
          else
            this.FireVolley(this.m_targetGun.Volley, this.m_owner, this.m_parentTransform.eulerAngles.z, new Vector2?(this.ShootPoint));
        }
        else
        {
          ProjectileModule defaultModule = this.m_targetGun.DefaultModule;
          Projectile currentProjectile2 = defaultModule.GetCurrentProjectile();
          if (!(bool) (UnityEngine.Object) currentProjectile2)
            return;
          float angleForShot = defaultModule.GetAngleForShot();
          if (flag)
            return;
          this.DoSingleProjectile(currentProjectile2, this.m_owner, this.m_parentTransform.eulerAngles.z + angleForShot, new Vector2?(this.ShootPoint), true);
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleFireShortBeam(
        Projectile projectileToSpawn,
        PlayerController source,
        float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HoveringGunController.<HandleFireShortBeam>c__Iterator0()
        {
          projectileToSpawn = projectileToSpawn,
          source = source,
          duration = duration,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator FireVolleyForDuration(
        ProjectileVolleyData volley,
        PlayerController source,
        float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HoveringGunController.<FireVolleyForDuration>c__Iterator1()
        {
          duration = duration,
          volley = volley,
          source = source,
          _this = this
        };
      }

      private void FireVolley(
        ProjectileVolleyData volley,
        PlayerController source,
        float targetAngle,
        Vector2? overrideSpawnPoint)
      {
        if (!string.IsNullOrEmpty(this.OnEveryShotAudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.OnEveryShotAudioEvent, this.gameObject);
        }
        for (int index = 0; index < volley.projectiles.Count; ++index)
        {
          ProjectileModule projectile = volley.projectiles[index];
          Projectile currentProjectile = projectile.GetCurrentProjectile();
          if ((bool) (UnityEngine.Object) currentProjectile)
          {
            float angleForShot = projectile.GetAngleForShot();
            if (!((UnityEngine.Object) currentProjectile.GetComponent<BeamController>() != (UnityEngine.Object) null))
              this.DoSingleProjectile(currentProjectile, source, targetAngle + angleForShot, overrideSpawnPoint);
          }
        }
      }

      private void DoSingleProjectile(
        Projectile projectileToSpawn,
        PlayerController source,
        float targetAngle,
        Vector2? overrideSpawnPoint,
        bool doAudio = false)
      {
        if (doAudio && !string.IsNullOrEmpty(this.OnEveryShotAudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.OnEveryShotAudioEvent, this.gameObject);
        }
        if (this.ConsumesTargetGunAmmo && (bool) (UnityEngine.Object) this.m_targetGun && this.m_owner.inventory.AllGuns.Contains(this.m_targetGun))
        {
          if (this.m_targetGun.ammo == 0)
            return;
          if ((double) UnityEngine.Random.value < (double) this.ChanceToConsumeTargetGunAmmo)
            this.m_targetGun.LoseAmmo(1);
        }
        Vector2 position = !overrideSpawnPoint.HasValue ? source.specRigidbody.UnitCenter : overrideSpawnPoint.Value;
        Projectile component1 = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, (Vector3) position, Quaternion.Euler(0.0f, 0.0f, targetAngle)).GetComponent<Projectile>();
        component1.Owner = (GameActor) source;
        component1.Shooter = source.specRigidbody;
        source.DoPostProcessProjectile(component1);
        BounceProjModifier component2 = component1.GetComponent<BounceProjModifier>();
        if (!(bool) (UnityEngine.Object) component2)
          return;
        component2.numberOfBounces = Mathf.Min(3, component2.numberOfBounces);
      }

      private BeamController BeginFiringBeam(
        Projectile projectileToSpawn,
        PlayerController source,
        float targetAngle,
        Vector2? overrideSpawnPoint)
      {
        Vector2 position = !overrideSpawnPoint.HasValue ? source.CenterPosition : overrideSpawnPoint.Value;
        GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, (Vector3) position, Quaternion.identity);
        gameObject.GetComponent<Projectile>().Owner = (GameActor) source;
        BeamController component = gameObject.GetComponent<BeamController>();
        component.Owner = (GameActor) source;
        component.HitsPlayers = false;
        component.HitsEnemies = true;
        Vector3 vector = (Vector3) BraveMathCollege.DegreesToVector(targetAngle);
        component.Direction = (Vector2) vector;
        component.Origin = position;
        return component;
      }

      private void ContinueFiringBeam(
        BeamController beam,
        PlayerController source,
        float angle,
        Vector2? overrideSpawnPoint)
      {
        Vector2 origin = !overrideSpawnPoint.HasValue ? source.CenterPosition : overrideSpawnPoint.Value;
        beam.Direction = BraveMathCollege.DegreesToVector(angle);
        beam.Origin = origin;
        beam.LateUpdatePosition((Vector3) origin);
      }

      private void CeaseBeam(BeamController beam) => beam.CeaseAttack();

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.m_owner)
          this.m_owner.OnDodgedProjectile -= new Action<Projectile>(this.HandleDodgedProjectileFire);
        if ((bool) (UnityEngine.Object) this.m_owner)
          this.m_owner.PostProcessProjectile -= new Action<Projectile, float>(this.HandleFiredGun);
        if (!string.IsNullOrEmpty(this.FinishedShootingAudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.FinishedShootingAudioEvent, this.gameObject);
        }
        if (this.Position == HoveringGunController.HoverPosition.CIRCULATE)
        {
          for (int index = 0; index < this.m_owner.orbitals.Count; ++index)
          {
            if (this.m_owner.orbitals[index].GetOrbitalTier() == this.GetOrbitalTier() && this.m_owner.orbitals[index].GetOrbitalTierIndex() > this.GetOrbitalTierIndex())
              this.m_owner.orbitals[index].SetOrbitalTierIndex(this.m_owner.orbitals[index].GetOrbitalTierIndex() - 1);
          }
          this.m_owner.orbitals.Remove((IPlayerOrbital) this);
        }
        LootEngine.DoDefaultSynergyPoof(this.sprite.WorldCenter);
      }

      public void Reinitialize()
      {
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          this.specRigidbody.Reinitialize();
        this.m_ownerCenterAverage = this.m_owner.CenterPosition;
      }

      public Transform GetTransform() => this.m_parentTransform;

      public void ToggleRenderer(bool visible) => this.sprite.renderer.enabled = visible;

      public int GetOrbitalTier() => this.m_orbitalTier;

      public void SetOrbitalTier(int tier) => this.m_orbitalTier = tier;

      public int GetOrbitalTierIndex() => this.m_orbitalTierIndex;

      public void SetOrbitalTierIndex(int tierIndex) => this.m_orbitalTierIndex = tierIndex;

      public float GetOrbitalRadius() => 2.5f;

      public float GetOrbitalRotationalSpeed() => 120f;

      public enum HoverPosition
      {
        OVERHEAD,
        CIRCULATE,
      }

      public enum FireType
      {
        ON_RELOAD,
        ON_COOLDOWN,
        ON_DODGED_BULLET,
        ON_FIRED_GUN,
      }

      public enum AimType
      {
        NEAREST_ENEMY,
        PLAYER_AIM,
      }
    }

}
