// Decompiled with JetBrains decompiler
// Type: AIShooter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Core.Framework
{
    [RequireComponent(typeof (AIBulletBank))]
    public class AIShooter : BraveBehaviour
    {
      public ProjectileVolleyData volley;
      [HideInInspectorIf("volley", false)]
      [PickupIdentifier]
      public int equippedGunId = -1;
      [HideInInspectorIf("volley", false)]
      public bool shouldUseGunReload;
      [ShowInInspectorIf("volley", true)]
      public Transform volleyShootPosition;
      [ShowInInspectorIf("volley", true)]
      public GameObject volleyShellCasing;
      [ShowInInspectorIf("volley", true)]
      public Transform volleyShellTransform;
      [ShowInInspectorIf("volley", true)]
      public GameObject volleyShootVfx;
      [ShowInInspectorIf("volley", true)]
      public bool usesOctantShootVFX = true;
      [Header("Bullet Properties")]
      public string bulletName = "default";
      public float customShootCooldownPeriod;
      public bool doesScreenShake;
      public bool rampBullets;
      [ShowInInspectorIf("rampBullets", false)]
      public float rampStartHeight = 2f;
      [ShowInInspectorIf("rampBullets", false)]
      public float rampTime = 1f;
      [Header("Hands")]
      public Transform gunAttachPoint;
      [FormerlySerializedAs("bulletMLAttachPoint")]
      public Transform bulletScriptAttachPoint;
      public IntVector2 overallGunAttachOffset;
      public IntVector2 flippedGunAttachOffset;
      public PlayerHandController handObject;
      public bool AllowTwoHands;
      public bool ForceGunOnTop;
      public bool IsReallyBigBoy;
      public bool BackupAimInMoveDirection;
      public Action<Projectile> PostProcessProjectile;
      private BulletScriptSource m_cachedBraveBulletSource;
      private float m_aimTimeScale = 1f;
      private bool m_hasCachedGun;
      private Gun m_cachedGun;
      private OverridableBool m_hideGunRenderers = new OverridableBool(false);
      private OverridableBool m_hideHandRenderers = new OverridableBool(false);
      private Vector3 attachPointCachedPosition;
      private Vector3 attachPointCachedFlippedPosition;
      private bool m_onCooldown;
      private GunInventory m_inventory;
      private List<PlayerHandController> m_attachedHands = new List<PlayerHandController>();

      public bool CanShootOtherEnemies => this.aiActor.CanTargetEnemies;

      public Vector2? OverrideAimPoint { get; set; }

      public Vector2? OverrideAimDirection
      {
        get
        {
          return !this.OverrideAimPoint.HasValue ? new Vector2?() : new Vector2?((this.OverrideAimPoint.Value - this.aiActor.CenterPosition).normalized);
        }
        set
        {
          Vector2? nullable1;
          if (!value.HasValue)
          {
            nullable1 = new Vector2?();
          }
          else
          {
            Vector2? nullable2 = !value.HasValue ? new Vector2?() : new Vector2?(value.GetValueOrDefault() * 5f);
            nullable1 = !nullable2.HasValue ? new Vector2?() : new Vector2?(this.aiActor.CenterPosition + nullable2.GetValueOrDefault());
          }
          this.OverrideAimPoint = nullable1;
        }
      }

      public GunInventory Inventory => this.m_inventory;

      public Transform BulletSourceTransform
      {
        get
        {
          if ((bool) (UnityEngine.Object) this.bulletScriptAttachPoint)
            return this.bulletScriptAttachPoint;
          if ((bool) (UnityEngine.Object) this.CurrentGun)
            return this.CurrentGun.barrelOffset;
          if ((bool) (UnityEngine.Object) this.volley && (bool) (UnityEngine.Object) this.volleyShootPosition)
            return this.volleyShootPosition;
          return (bool) (UnityEngine.Object) this.gunAttachPoint ? this.gunAttachPoint : this.transform;
        }
      }

      public BulletScriptSource BraveBulletSource
      {
        get
        {
          if ((UnityEngine.Object) this.m_cachedBraveBulletSource == (UnityEngine.Object) null)
            this.m_cachedBraveBulletSource = this.BulletSourceTransform.gameObject.GetOrAddComponent<BulletScriptSource>();
          return this.m_cachedBraveBulletSource;
        }
      }

      public float AimTimeScale
      {
        get
        {
          return (bool) (UnityEngine.Object) this.aiActor ? this.m_aimTimeScale * this.aiActor.LocalTimeScale : this.m_aimTimeScale;
        }
        set => this.m_aimTimeScale = value;
      }

      public Gun EquippedGun
      {
        get
        {
          if (!this.m_hasCachedGun)
          {
            if (this.equippedGunId >= 0)
              this.m_cachedGun = PickupObjectDatabase.GetById(this.equippedGunId) as Gun;
            this.m_hasCachedGun = true;
          }
          return this.m_cachedGun;
        }
      }

      public void Start() => this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);

      private void Update()
      {
        if (this.aiActor.HasBeenEngaged)
          this.HandleGunFlipping();
        if (this.aiActor.IsFalling && (UnityEngine.Object) this.m_cachedBraveBulletSource != (UnityEngine.Object) null)
          this.m_cachedBraveBulletSource.enabled = false;
        if (!this.BackupAimInMoveDirection || (bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
          return;
        this.AimInDirection(BraveMathCollege.DegreesToVector(this.aiActor.FacingDirection));
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void ShootInDirection(Vector2 direction, string overrideBulletName = null)
      {
        if (this.healthHaver.IsDead)
          return;
        if ((UnityEngine.Object) this.EquippedGun == (UnityEngine.Object) null && (UnityEngine.Object) this.volley != (UnityEngine.Object) null)
        {
          this.ShootInDirection(direction, this.volley, overrideBulletName);
        }
        else
        {
          if (!((UnityEngine.Object) this.EquippedGun != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_inventory.CurrentGun != (UnityEngine.Object) null))
            return;
          this.AimInDirection(direction);
          this.Shoot(overrideBulletName);
          this.m_inventory.CurrentGun.ClearCooldowns();
          this.m_inventory.CurrentGun.ClearReloadData();
        }
      }

      public void ShootAtTarget(string overrideBulletName = null)
      {
        if (this.healthHaver.IsDead)
          return;
        if (!(bool) (UnityEngine.Object) this.aiActor.OverrideTarget)
          this.aiActor.OverrideTarget = (SpeculativeRigidbody) null;
        if ((UnityEngine.Object) this.aiActor.TargetRigidbody == (UnityEngine.Object) null)
          return;
        if ((UnityEngine.Object) this.EquippedGun == (UnityEngine.Object) null && (UnityEngine.Object) this.volley != (UnityEngine.Object) null)
        {
          this.ShootAtTarget(this.volley, overrideBulletName);
        }
        else
        {
          if (!((UnityEngine.Object) this.EquippedGun != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_inventory.CurrentGun != (UnityEngine.Object) null))
            return;
          this.AimAtPoint(this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
          this.Shoot(overrideBulletName);
          if (!((UnityEngine.Object) this.m_inventory.CurrentGun != (UnityEngine.Object) null))
            return;
          this.m_inventory.CurrentGun.ClearCooldowns();
          this.m_inventory.CurrentGun.ClearReloadData();
        }
      }

      public void CeaseAttack()
      {
        if (this.m_inventory == null || !(bool) (UnityEngine.Object) this.m_inventory.CurrentGun)
          return;
        this.m_inventory.CurrentGun.CeaseAttack();
      }

      public void Reload()
      {
        if (!((UnityEngine.Object) this.m_inventory.CurrentGun != (UnityEngine.Object) null))
          return;
        this.m_inventory.CurrentGun.Reload();
      }

      public void ShootBulletScript(BulletScriptSelector bulletScript)
      {
        BulletScriptSource braveBulletSource = this.BraveBulletSource;
        braveBulletSource.BulletManager = this.bulletBank;
        braveBulletSource.BulletScript = bulletScript;
        braveBulletSource.Initialize();
      }

      public AIBulletBank.Entry GetBulletEntry(string overrideBulletName = null)
      {
        string str = string.IsNullOrEmpty(overrideBulletName) ? this.bulletName : overrideBulletName;
        if (string.IsNullOrEmpty(str))
          return (AIBulletBank.Entry) null;
        AIBulletBank.Entry bulletEntry = (AIBulletBank.Entry) null;
        List<AIBulletBank.Entry> bullets = this.bulletBank.Bullets;
        for (int index = 0; index < bullets.Count; ++index)
        {
          if (bullets[index].Name == str)
          {
            bulletEntry = bullets[index];
            break;
          }
        }
        if (bulletEntry != null)
          return bulletEntry;
        UnityEngine.Debug.LogError((object) $"Unknown bullet type {this.transform.name} on {str}", (UnityEngine.Object) this.gameObject);
        return (AIBulletBank.Entry) null;
      }

      private void OnPreDeath(Vector2 obj)
      {
        if ((UnityEngine.Object) this.m_cachedBraveBulletSource != (UnityEngine.Object) null)
          this.m_cachedBraveBulletSource.enabled = false;
        this.ToggleGunAndHandRenderers(false, nameof (OnPreDeath));
      }

      public void ToggleGunRenderers(bool value, string reason)
      {
        if (string.IsNullOrEmpty(reason))
        {
          this.m_hideGunRenderers.BaseValue = !value;
          if (value)
            this.m_hideGunRenderers.ClearOverrides();
        }
        else
          this.m_hideGunRenderers.SetOverride(reason, !value);
        this.UpdateGunRenderers();
      }

      public void UpdateGunRenderers()
      {
        bool flag = !this.m_hideGunRenderers.Value;
        if (!((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null))
          return;
        this.CurrentGun.ToggleRenderers(flag);
      }

      public void ToggleHandRenderers(bool value, string reason)
      {
        if (string.IsNullOrEmpty(reason))
        {
          this.m_hideHandRenderers.BaseValue = !value;
          if (value)
            this.m_hideHandRenderers.ClearOverrides();
        }
        else
          this.m_hideHandRenderers.SetOverride(reason, !value);
        this.UpdateHandRenderers();
      }

      public void UpdateHandRenderers()
      {
        bool flag = !this.m_hideHandRenderers.Value;
        for (int index = 0; index < this.m_attachedHands.Count; ++index)
          this.m_attachedHands[index].ForceRenderersOff = !flag;
      }

      public void ToggleGunAndHandRenderers(bool value, string reason)
      {
        this.ToggleGunRenderers(value, reason);
        this.ToggleHandRenderers(value, reason);
      }

      public void StartPreFireAnim()
      {
        if (!(bool) (UnityEngine.Object) this.CurrentGun || string.IsNullOrEmpty(this.CurrentGun.enemyPreFireAnimation))
          return;
        this.CurrentGun.spriteAnimator.Play(this.CurrentGun.enemyPreFireAnimation);
      }

      public bool IsPreFireComplete
      {
        get
        {
          return !(bool) (UnityEngine.Object) this.CurrentGun || string.IsNullOrEmpty(this.CurrentGun.enemyPreFireAnimation) || !this.CurrentGun.spriteAnimator.IsPlaying(this.CurrentGun.enemyPreFireAnimation);
        }
      }

      protected void ShootAtTarget(ProjectileVolleyData volley, string overrideBulletName = null)
      {
        if ((UnityEngine.Object) this.aiActor.TargetRigidbody == (UnityEngine.Object) null)
          return;
        for (int index = 0; index < volley.projectiles.Count; ++index)
        {
          ProjectileModule projectile = volley.projectiles[index];
          float angleForShot = projectile.GetAngleForShot();
          this.ShootAtTarget(projectile, overrideBulletName, projectile.positionOffset, angleForShot);
          if (projectile.mirror)
            this.ShootAtTarget(projectile, overrideBulletName, projectile.InversePositionOffset, -angleForShot);
          projectile.IncrementShootCount();
        }
      }

      protected void ShootAtTarget(
        ProjectileModule projectileModule,
        string overrideBulletName,
        Vector3 positionOffset,
        float angleOffset)
      {
        if ((UnityEngine.Object) this.aiActor.TargetRigidbody == (UnityEngine.Object) null)
          return;
        Vector2 vector2 = this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.volleyShootPosition.position.XY();
        float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
        GameObject prefab = projectileModule.GetCurrentProjectile().gameObject;
        AIBulletBank.Entry bulletEntry = this.GetBulletEntry(overrideBulletName);
        if (bulletEntry != null && (bool) (UnityEngine.Object) bulletEntry.BulletObject)
          prefab = bulletEntry.BulletObject;
        Projectile component = SpawnManager.SpawnProjectile(prefab, this.volleyShootPosition.position + Quaternion.Euler(0.0f, 0.0f, z) * positionOffset, Quaternion.Euler(0.0f, 0.0f, z + angleOffset)).GetComponent<Projectile>();
        if (bulletEntry != null && bulletEntry.OverrideProjectile)
          component.baseData.SetAll(bulletEntry.ProjectileData);
        if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.IsBlackPhantom)
          component.baseData.speed *= this.aiActor.BlackPhantomProperties.BulletSpeedMultiplier;
        component.collidesWithEnemies = this.aiActor.CanTargetEnemies;
        component.collidesWithPlayer = this.aiActor.CanTargetPlayers;
        component.Shooter = this.specRigidbody;
        if (this.PostProcessProjectile == null)
          return;
        this.PostProcessProjectile(component);
      }

      protected void ShootInDirection(
        Vector2 direction,
        ProjectileVolleyData volley,
        string overrideBulletName = null)
      {
        for (int index = 0; index < volley.projectiles.Count; ++index)
        {
          ProjectileModule projectile = volley.projectiles[index];
          float angleForShot = projectile.GetAngleForShot();
          this.ShootInDirection(direction, projectile, overrideBulletName, projectile.positionOffset, angleForShot);
          if (projectile.mirror)
            this.ShootInDirection(direction, projectile, overrideBulletName, projectile.InversePositionOffset, -angleForShot);
          projectile.IncrementShootCount();
        }
        this.SpawnVolleyShellCasing(!((UnityEngine.Object) this.volleyShellTransform != (UnityEngine.Object) null) ? this.volleyShootPosition.position : this.volleyShellTransform.position);
        if ((bool) (UnityEngine.Object) this.gunAttachPoint && (bool) (UnityEngine.Object) this.volleyShellTransform && (bool) (UnityEngine.Object) this.volleyShootVfx)
        {
          if (this.usesOctantShootVFX)
          {
            tk2dSprite component = SpawnManager.SpawnVFX(this.volleyShootVfx, this.volleyShootPosition.position, Quaternion.Euler(0.0f, 0.0f, (float) (90 + BraveMathCollege.VectorToOctant((Vector2) (this.volleyShootPosition.position - this.volleyShellTransform.position)) * -45))).GetComponent<tk2dSprite>();
            component.HeightOffGround = 2f;
            this.sprite.AttachRenderer((tk2dBaseSprite) component);
            component.IsPerpendicular = true;
            component.usesOverrideMaterial = true;
          }
          else
          {
            tk2dSprite component = SpawnManager.SpawnVFX(this.volleyShootVfx, this.volleyShootPosition.position, Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(direction))).GetComponent<tk2dSprite>();
            component.HeightOffGround = 2f;
            this.sprite.AttachRenderer((tk2dBaseSprite) component);
            component.IsPerpendicular = true;
            component.usesOverrideMaterial = true;
          }
        }
        int num = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Shoot_01", this.gameObject);
      }

      protected void ShootInDirection(
        Vector2 direction,
        ProjectileModule projectileModule,
        string overrideBulletName,
        Vector3 positionOffset,
        float angleOffset)
      {
        float z = Mathf.Atan2(direction.y, direction.x) * 57.29578f;
        GameObject prefab = projectileModule.GetCurrentProjectile().gameObject;
        AIBulletBank.Entry bulletEntry = this.GetBulletEntry(overrideBulletName);
        if (bulletEntry != null && (bool) (UnityEngine.Object) bulletEntry.BulletObject)
          prefab = bulletEntry.BulletObject;
        Projectile component = SpawnManager.SpawnProjectile(prefab, this.volleyShootPosition.position + Quaternion.Euler(0.0f, 0.0f, z) * positionOffset, Quaternion.Euler(0.0f, 0.0f, z + angleOffset)).GetComponent<Projectile>();
        if (bulletEntry != null && bulletEntry.OverrideProjectile)
          component.baseData.SetAll(bulletEntry.ProjectileData);
        if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.IsBlackPhantom)
          component.baseData.speed *= this.aiActor.BlackPhantomProperties.BulletSpeedMultiplier;
        component.Shooter = this.specRigidbody;
        if ((bool) (UnityEngine.Object) this.aiActor)
          component.SetOwnerSafe((GameActor) this.aiActor, this.aiActor.GetActorName());
        if (this.PostProcessProjectile == null)
          return;
        this.PostProcessProjectile(component);
      }

      protected void SpawnVolleyShellCasing(Vector3 position)
      {
        if (!((UnityEngine.Object) this.volleyShellCasing != (UnityEngine.Object) null))
          return;
        GameObject gameObject = SpawnManager.SpawnDebris(this.volleyShellCasing, position, Quaternion.identity);
        ShellCasing component1 = gameObject.GetComponent<ShellCasing>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
          component1.Trigger();
        DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
        if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
          return;
        Vector3 vector3 = Vector3.up * (float) ((double) UnityEngine.Random.value * 1.5 + 1.0) + -1.5f * Vector3.right * ((double) component2.transform.right.x <= 0.0 ? -1f : 1f) * (UnityEngine.Random.value + 1.5f);
        Vector3 startingForce = new Vector3(vector3.x, 0.0f, vector3.y);
        float y = this.transform.position.y;
        float startingHeight = (float) ((double) component2.transform.position.y - (double) y + (double) UnityEngine.Random.value * 0.5);
        component2.Trigger(startingForce, startingHeight);
      }

      public bool OnCooldown => this.m_onCooldown;

      public bool ManualGunAngle { get; set; }

      public float GunAngle { get; set; }

      public Gun CurrentGun => this.m_inventory == null ? (Gun) null : this.m_inventory.CurrentGun;

      public void Initialize()
      {
        this.m_inventory = new GunInventory((GameActor) this.aiActor);
        if ((UnityEngine.Object) this.EquippedGun != (UnityEngine.Object) null)
        {
          this.m_inventory.AddGunToInventory(this.EquippedGun, true);
          if (this.CurrentGun.singleModule != null && this.CurrentGun.singleModule.shootStyle == ProjectileModule.ShootStyle.Burst)
            this.CurrentGun.singleModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
          this.CurrentGun.doesScreenShake = this.doesScreenShake;
          this.CurrentGun.ammo = int.MaxValue;
          SpriteOutlineManager.AddOutlineToSprite(this.CurrentGun.GetSprite(), Color.black, 0.1f, 0.05f);
          this.sprite.AttachRenderer(this.CurrentGun.GetSprite());
        }
        Bounds untrimmedBounds = this.sprite.GetUntrimmedBounds();
        this.attachPointCachedPosition = this.gunAttachPoint.localPosition + (Vector3) PhysicsEngine.PixelToUnit(this.overallGunAttachOffset);
        this.attachPointCachedFlippedPosition = this.gunAttachPoint.localPosition.WithX(untrimmedBounds.center.x + (untrimmedBounds.center.x - this.gunAttachPoint.localPosition.x)) + (Vector3) PhysicsEngine.PixelToUnit(this.flippedGunAttachOffset) + (Vector3) PhysicsEngine.PixelToUnit(this.overallGunAttachOffset);
        if ((UnityEngine.Object) this.handObject != (UnityEngine.Object) null)
        {
          if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null && this.CurrentGun.Handedness == GunHandedness.OneHanded)
            this.AttachNewHandToTransform(this.CurrentGun.PrimaryHandAttachPoint);
          else if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null && this.CurrentGun.Handedness == GunHandedness.TwoHanded && this.AllowTwoHands)
          {
            this.AttachNewHandToTransform(this.CurrentGun.PrimaryHandAttachPoint);
            this.AttachNewHandToTransform(this.CurrentGun.SecondaryHandAttachPoint);
          }
        }
        this.AimAtPoint((Vector2) (this.gunAttachPoint.position + BraveUtility.RandomSign() * new Vector3(5f, 0.0f, 0.0f)));
      }

      protected void AttachNewHandToTransform(Transform target)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.handObject.gameObject);
        gameObject.transform.parent = this.transform;
        PlayerHandController component = gameObject.GetComponent<PlayerHandController>();
        this.CurrentGun.GetSprite().AttachRenderer(component.sprite);
        component.attachPoint = target;
        this.m_attachedHands.Add(component);
        component.ForceRenderersOff = !this.renderer.enabled;
        if (!(bool) (UnityEngine.Object) this.healthHaver)
          return;
        this.healthHaver.RegisterBodySprite((tk2dBaseSprite) component.GetComponent<tk2dSprite>());
      }

      public void AimAtOverride()
      {
        Gun currentGun = this.m_inventory.CurrentGun;
        if ((UnityEngine.Object) currentGun == (UnityEngine.Object) null)
          return;
        this.GunAngle = currentGun.HandleAimRotation((Vector3) this.OverrideAimPoint.Value);
        this.HandleGunFlipping();
      }

      public void AimAtTarget()
      {
        if ((UnityEngine.Object) this.aiActor.TargetRigidbody == (UnityEngine.Object) null && !this.OverrideAimPoint.HasValue)
          return;
        if (this.OverrideAimPoint.HasValue)
          this.AimAtOverride();
        else
          this.AimAtPoint(this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox));
      }

      public void AimAtPoint(Vector2 point)
      {
        if (this.OverrideAimPoint.HasValue)
        {
          this.AimAtOverride();
        }
        else
        {
          if (this.m_inventory == null)
            return;
          Gun currentGun = this.m_inventory.CurrentGun;
          if ((UnityEngine.Object) currentGun == (UnityEngine.Object) null)
            return;
          float num = !this.IsReallyBigBoy ? 5f : 10f;
          if ((double) Vector2.Distance(this.specRigidbody.UnitCenter, point) < (double) num)
            point = (point - this.specRigidbody.UnitCenter).normalized * num + this.specRigidbody.UnitCenter;
          this.GunAngle = currentGun.HandleAimRotation((Vector3) point, true, this.AimTimeScale);
          this.HandleGunFlipping();
        }
      }

      public void AimInDirection(Vector2 direction)
      {
        if (this.OverrideAimPoint.HasValue)
          this.AimAtOverride();
        else
          this.AimAtPoint((Vector2) (Vector3) (this.aiActor.CenterPosition + direction * 5f));
      }

      public void Shoot(string overrideBulletName = null)
      {
        if ((UnityEngine.Object) this.EquippedGun == (UnityEngine.Object) null && (UnityEngine.Object) this.volley != (UnityEngine.Object) null)
        {
          for (int index = 0; index < this.volley.projectiles.Count; ++index)
          {
            ProjectileModule projectile = this.volley.projectiles[index];
            float angleForShot = projectile.GetAngleForShot();
            this.ShootAtTarget(projectile, overrideBulletName, projectile.positionOffset, angleForShot);
            if (projectile.mirror)
              this.ShootAtTarget(projectile, overrideBulletName, projectile.InversePositionOffset, -angleForShot);
            projectile.IncrementShootCount();
          }
        }
        else
        {
          if (!((UnityEngine.Object) this.EquippedGun != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_inventory.CurrentGun != (UnityEngine.Object) null))
            return;
          Gun currentGun = this.m_inventory.CurrentGun;
          AIBulletBank.Entry bulletEntry = this.GetBulletEntry(overrideBulletName);
          if (this.PostProcessProjectile != null)
            currentGun.PostProcessProjectile += this.PostProcessProjectile;
          if (bulletEntry != null)
          {
            int num1 = (int) currentGun.Attack(!bulletEntry.OverrideProjectile ? (ProjectileData) null : bulletEntry.ProjectileData, bulletEntry.BulletObject);
          }
          else
          {
            int num2 = (int) currentGun.Attack();
          }
          currentGun.PostProcessProjectile -= this.PostProcessProjectile;
          if (!((UnityEngine.Object) this.m_inventory.CurrentGun != (UnityEngine.Object) null))
            return;
          this.m_inventory.CurrentGun.ClearCooldowns();
          this.m_inventory.CurrentGun.ClearReloadData();
        }
      }

      public void ContinueShoot(Vector3 targetPosition)
      {
        this.AimAtPoint((Vector2) targetPosition);
        this.m_inventory.CurrentGun.ContinueAttack();
      }

      public void ShootAtTarget(Vector3 targetPosition)
      {
        if ((UnityEngine.Object) this.volley == (UnityEngine.Object) null)
        {
          this.AimAtPoint((Vector2) targetPosition);
          this.Shoot();
          this.m_inventory.CurrentGun.ClearCooldowns();
          if (this.shouldUseGunReload)
            return;
          this.m_inventory.CurrentGun.ClearReloadData();
        }
        else
          this.ShootVolleyAtTarget(targetPosition);
      }

      public void Cooldown()
      {
        float t = !((UnityEngine.Object) this.volley == (UnityEngine.Object) null) ? this.volley.projectiles[0].cooldownTime : this.m_inventory.CurrentGun.GetPrimaryCooldown();
        if ((double) this.customShootCooldownPeriod > 0.0)
          t = this.customShootCooldownPeriod;
        this.StartCoroutine(this.HandleFireRate(t));
      }

      public void Cooldown(float t) => this.StartCoroutine(this.HandleFireRate(t));

      private void HandleGunFlipping()
      {
        if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null)
          return;
        if ((double) Mathf.Abs(this.GunAngle) > 105.0)
        {
          this.gunAttachPoint.localPosition = this.attachPointCachedPosition;
          this.CurrentGun.HandleSpriteFlip(true);
        }
        else if ((double) Mathf.Abs(this.GunAngle) < 75.0)
        {
          this.gunAttachPoint.localPosition = this.attachPointCachedFlippedPosition;
          this.CurrentGun.HandleSpriteFlip(false);
        }
        if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
        {
          tk2dBaseSprite sprite = this.CurrentGun.GetSprite();
          if (!this.ForceGunOnTop && (double) this.GunAngle > 0.0 && (double) this.GunAngle <= 155.0 && (double) this.GunAngle >= 25.0)
          {
            if (!this.CurrentGun.forceFlat)
              sprite.HeightOffGround = -0.075f;
            for (int index = 0; index < this.m_attachedHands.Count; ++index)
            {
              this.m_attachedHands[index].handHeightFromGun = 0.05f;
              this.m_attachedHands[index].sprite.HeightOffGround = 0.05f;
            }
          }
          else
          {
            float num = this.CurrentGun.Handedness != GunHandedness.TwoHanded ? -0.075f : 0.875f;
            if (!this.CurrentGun.forceFlat)
              sprite.HeightOffGround = num;
            for (int index = 0; index < this.m_attachedHands.Count; ++index)
            {
              this.m_attachedHands[index].handHeightFromGun = 0.15f;
              this.m_attachedHands[index].sprite.HeightOffGround = 0.15f;
            }
          }
        }
        this.sprite.UpdateZDepth();
      }

      private void ShootVolleyAtTarget(Vector3 targetPosition)
      {
        Vector3 vector3 = (Vector3) (targetPosition.XY() - this.aiActor.CenterPosition);
        float num = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
        for (int index = 0; index < this.volley.projectiles.Count; ++index)
        {
          ProjectileModule projectile = this.volley.projectiles[index];
          float angleForShot = projectile.GetAngleForShot();
          GameObject prefab1 = projectile.GetCurrentProjectile().gameObject;
          AIBulletBank.Entry bulletEntry1 = this.GetBulletEntry();
          if (bulletEntry1 != null && (bool) (UnityEngine.Object) bulletEntry1.BulletObject)
            prefab1 = bulletEntry1.BulletObject;
          Projectile component1 = SpawnManager.SpawnProjectile(prefab1, (Vector3) this.aiActor.CenterPosition + Quaternion.Euler(0.0f, 0.0f, num + angleForShot) * projectile.positionOffset, Quaternion.Euler(0.0f, 0.0f, num + angleForShot)).GetComponent<Projectile>();
          if (bulletEntry1 != null && bulletEntry1.OverrideProjectile)
            component1.baseData.SetAll(bulletEntry1.ProjectileData);
          if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.IsBlackPhantom)
            component1.baseData.speed *= this.aiActor.BlackPhantomProperties.BulletSpeedMultiplier;
          component1.Shooter = this.specRigidbody;
          if (projectile.mirror)
          {
            GameObject prefab2 = projectile.GetCurrentProjectile().gameObject;
            AIBulletBank.Entry bulletEntry2 = this.GetBulletEntry();
            if (bulletEntry2 != null && (bool) (UnityEngine.Object) bulletEntry2.BulletObject)
              prefab2 = bulletEntry2.BulletObject;
            Projectile component2 = SpawnManager.SpawnProjectile(prefab2, (Vector3) this.aiActor.CenterPosition + Quaternion.Euler(0.0f, 0.0f, num + angleForShot) * projectile.InversePositionOffset, Quaternion.Euler(0.0f, 0.0f, num - angleForShot)).GetComponent<Projectile>();
            if (bulletEntry2 != null && bulletEntry2.OverrideProjectile)
              component2.baseData.SetAll(bulletEntry2.ProjectileData);
            if ((bool) (UnityEngine.Object) this.aiActor && this.aiActor.IsBlackPhantom)
              component2.baseData.speed *= this.aiActor.BlackPhantomProperties.BulletSpeedMultiplier;
            component2.Shooter = this.specRigidbody;
          }
          projectile.IncrementShootCount();
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleFireRate(float t)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AIShooter.\u003CHandleFireRate\u003Ec__Iterator0()
        {
          t = t,
          \u0024this = this
        };
      }
    }

}
