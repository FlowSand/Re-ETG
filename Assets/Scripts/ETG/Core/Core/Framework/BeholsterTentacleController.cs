using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class BeholsterTentacleController : BraveBehaviour
  {
    public DirectionalAnimation IdleAnimation;
    public DirectionalAnimation ShootAnimation;
    [PickupIdentifier]
    public int GunId;
    public Projectile OverrideProjectile;
    public bool UsesOverrideProjectileData;
    public ProjectileData OverrideProjectileData;
    public float FireTime;
    public float ShotCooldown;
    public float Cooldown;
    public BeholsterTentacleController.DirectionalAnimationBool gunBehindTentacle;
    public BeholsterTentacleController.DirectionalAnimationBool tentacleBehindBody;
    public bool RampBullets;
    [ShowInInspectorIf("RampBullets", false)]
    public float RampStartHeight = 2f;
    [ShowInInspectorIf("RampBullets", false)]
    public float RampTime = 1f;
    public bool SpawnBullets;
    [ShowInInspectorIf("SpawnBullets", false)]
    public float MinSpawnRadius;
    [ShowInInspectorIf("SpawnBullets", false)]
    public float MaxSpawnRadius;
    [ShowInInspectorIf("SpawnBullets", false)]
    public float MaxConcurrentAdds;
    private BulletScriptSource m_cachedBulletScriptSource;
    private BeholsterController m_body;
    private Gun m_gun;
    private ProjectileData m_overrideProjectileData;
    private Transform m_gunAttachPoint;
    private float m_gunAngle;
    private bool m_gunFlipped;
    private DirectionalAnimation m_currentAnimation;
    private float m_fireTimer;
    private float m_shotCooldown;
    private float m_cooldown;
    private Vector2 m_targetLocation;
    private List<SpawningProjectile> m_spawningProjectiles;

    public Gun Gun => this.m_gun;

    public void Start()
    {
      this.m_body = this.transform.parent.GetComponent<BeholsterController>();
      this.m_gunAttachPoint = this.transform.Find("gun");
      this.m_gun = UnityEngine.Object.Instantiate<PickupObject>(PickupObjectDatabase.GetById(this.GunId)) as Gun;
      this.m_gun.transform.parent = this.m_gunAttachPoint;
      this.m_gun.NoOwnerOverride = true;
      this.m_gun.Initialize((GameActor) this.m_body.aiActor);
      this.m_gun.gameObject.SetActive(true);
      this.m_gun.sprite.HeightOffGround = 0.05f;
      this.sprite.AttachRenderer(this.m_gun.sprite);
      if ((bool) (UnityEngine.Object) this.OverrideProjectile)
        this.m_gun.DefaultModule.projectiles = new List<Projectile>()
        {
          this.OverrideProjectile
        };
      if (this.UsesOverrideProjectileData)
        this.m_overrideProjectileData = this.OverrideProjectileData;
      else
        this.m_overrideProjectileData = new ProjectileData(this.m_gun.singleModule.projectiles[0].baseData)
        {
          damage = 0.5f
        };
      this.m_gun.ammo = int.MaxValue;
      this.m_gun.DefaultModule.numberOfShotsInClip = 0;
      this.m_gun.DefaultModule.usesOptionalFinalProjectile = false;
      if (this.RampBullets)
      {
        this.m_gun.rampBullets = true;
        this.m_gun.rampStartHeight = this.RampStartHeight;
        this.m_gun.rampTime = this.RampTime;
      }
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.3f);
      SpriteOutlineManager.AddOutlineToSprite(this.m_gun.sprite, Color.black, 0.35f);
      this.m_body.healthHaver.RegisterBodySprite(this.sprite);
      this.m_cooldown = this.Cooldown;
      this.spriteAnimator.AnimationCompleted += (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) ((animator, clip) =>
      {
        if (!this.enabled)
          return;
        this.m_currentAnimation = (DirectionalAnimation) null;
      });
      if (!this.SpawnBullets)
        return;
      this.m_spawningProjectiles = new List<SpawningProjectile>();
    }

    public void Update()
    {
      float facingDirection = this.m_body.aiAnimator.FacingDirection;
      DirectionalAnimation.Info info;
      if (this.m_currentAnimation != null)
      {
        info = this.m_currentAnimation.GetInfo(facingDirection);
        this.spriteAnimator.Play(this.spriteAnimator.GetClipByName(info.name), this.spriteAnimator.ClipTimeSeconds, this.spriteAnimator.ClipFps);
      }
      else
      {
        info = this.IdleAnimation.GetInfo(facingDirection);
        this.spriteAnimator.Play(info.name);
      }
      this.sprite.FlipX = info.flipped;
      bool flag1;
      bool flag2;
      if ((double) facingDirection <= 155.0 && (double) facingDirection >= 25.0)
      {
        if ((double) facingDirection < 120.0 && (double) facingDirection >= 60.0)
        {
          flag1 = this.tentacleBehindBody.Back;
          flag2 = this.gunBehindTentacle.Back;
        }
        else
        {
          flag1 = (double) Mathf.Abs(facingDirection) >= 90.0 ? this.tentacleBehindBody.BackRight : this.tentacleBehindBody.BackLeft;
          flag2 = (double) Mathf.Abs(facingDirection) >= 90.0 ? this.gunBehindTentacle.BackRight : this.gunBehindTentacle.BackLeft;
        }
      }
      else if ((double) facingDirection <= -60.0 && (double) facingDirection >= -120.0)
      {
        flag1 = this.tentacleBehindBody.Forward;
        flag2 = this.gunBehindTentacle.Forward;
      }
      else
      {
        flag1 = (double) Mathf.Abs(facingDirection) < 90.0 ? this.tentacleBehindBody.ForwardRight : this.tentacleBehindBody.ForwardLeft;
        flag2 = (double) Mathf.Abs(facingDirection) < 90.0 ? this.gunBehindTentacle.ForwardRight : this.gunBehindTentacle.ForwardLeft;
      }
      this.sprite.HeightOffGround = !flag1 ? 0.1f : -0.1f;
      this.m_gun.sprite.HeightOffGround = !flag2 ? 0.05f : -0.05f;
      if (this.m_body.LaserActive)
      {
        float f = this.m_body.aiAnimator.FacingDirection * ((float) Math.PI / 180f);
        this.m_targetLocation = this.m_body.LaserFiringCenter + new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * 10f;
      }
      else if ((bool) (UnityEngine.Object) this.m_body.aiActor.TargetRigidbody)
        this.m_targetLocation = this.m_body.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
      this.m_gunAngle = this.m_gun.HandleAimRotation((Vector3) this.m_targetLocation);
      if (!this.m_gunFlipped && (double) Mathf.Abs(this.m_gunAngle) > 105.0)
      {
        this.m_gun.HandleSpriteFlip(true);
        this.m_gunFlipped = true;
      }
      else if (this.m_gunFlipped && (double) Mathf.Abs(this.m_gunAngle) < 75.0)
      {
        this.m_gun.HandleSpriteFlip(false);
        this.m_gunFlipped = false;
      }
      if ((double) this.m_fireTimer > 0.0)
      {
        this.m_fireTimer -= BraveTime.DeltaTime;
        if ((double) this.m_fireTimer <= 0.0 || this.SpawnBullets && (double) this.CurrentAdds >= (double) this.MaxConcurrentAdds)
          this.CeaseAttack();
        else if ((double) this.ShotCooldown <= 0.0)
        {
          this.m_gun.ContinueAttack(overrideProjectileData: this.m_overrideProjectileData);
        }
        else
        {
          this.m_shotCooldown -= BraveTime.DeltaTime;
          if ((double) this.m_shotCooldown > 0.0)
            return;
          this.m_gun.CeaseAttack();
          this.Fire();
          this.m_shotCooldown = this.ShotCooldown;
        }
      }
      else
        this.m_cooldown = Mathf.Max(0.0f, this.m_cooldown - BraveTime.DeltaTime);
    }

    public void StartFiring()
    {
      this.Fire();
      this.m_fireTimer = this.FireTime;
      this.m_shotCooldown = this.ShotCooldown;
      this.m_cooldown = this.Cooldown;
      if (this.m_gun.singleModule.shootStyle != ProjectileModule.ShootStyle.SemiAutomatic)
        return;
      this.Play(this.ShootAnimation);
    }

    public void SingleFire(float? angleOffset = null)
    {
      this.m_gun.ClearCooldowns();
      this.Fire(angleOffset);
      this.m_cooldown = this.Cooldown;
      if (this.m_gun.singleModule.shootStyle != ProjectileModule.ShootStyle.SemiAutomatic)
        return;
      this.Play(this.ShootAnimation);
    }

    public void CeaseAttack()
    {
      this.m_gun.CeaseAttack();
      this.m_cooldown = this.Cooldown;
    }

    public bool IsReady
    {
      get
      {
        return (!this.SpawnBullets || (double) this.CurrentAdds < (double) this.MaxConcurrentAdds) && (!(bool) (UnityEngine.Object) this.m_cachedBulletScriptSource || this.m_cachedBulletScriptSource.IsEnded) && (double) this.m_fireTimer <= 0.0 && (double) this.m_cooldown <= 0.0;
      }
    }

    public int CurrentAdds
    {
      get
      {
        if (!this.SpawnBullets)
          return 0;
        int num = 0;
        this.m_spawningProjectiles.RemoveAll((Predicate<SpawningProjectile>) (p => !(bool) (UnityEngine.Object) p));
        int currentAdds = num + this.m_spawningProjectiles.Count;
        List<AIActor> activeEnemies = this.m_body.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies != null)
          currentAdds += activeEnemies.Count - 1;
        return currentAdds;
      }
    }

    public void Play(DirectionalAnimation anim)
    {
      this.m_currentAnimation = anim;
      DirectionalAnimation.Info info = anim.GetInfo(this.m_body.aiAnimator.FacingDirection, true);
      this.sprite.FlipX = info.flipped;
      this.spriteAnimator.Play(info.name);
    }

    public BulletScriptSource BulletScriptSource
    {
      get
      {
        if ((UnityEngine.Object) this.m_cachedBulletScriptSource == (UnityEngine.Object) null)
          this.m_cachedBulletScriptSource = this.m_gun.barrelOffset.gameObject.GetOrAddComponent<BulletScriptSource>();
        return this.m_cachedBulletScriptSource;
      }
    }

    public void ShootBulletScript(BulletScriptSelector bulletScript)
    {
      this.m_body.bulletBank.rampBullets = this.RampBullets;
      this.m_body.bulletBank.rampStartHeight = this.RampStartHeight;
      this.m_body.bulletBank.rampTime = this.RampTime;
      this.m_body.bulletBank.OverrideGun = this.Gun;
      BulletScriptSource bulletScriptSource = this.BulletScriptSource;
      bulletScriptSource.BulletManager = this.m_body.bulletBank;
      bulletScriptSource.BulletScript = bulletScript;
      bulletScriptSource.Initialize();
    }

    private void Fire(float? angleOffset = null)
    {
      if (this.SpawnBullets && (bool) (UnityEngine.Object) this.m_body.aiActor.TargetRigidbody)
      {
        Vector2 unitCenter = this.m_body.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        float angle = (this.m_body.specRigidbody.GetUnitCenter(ColliderType.HitBox) - unitCenter).ToAngle();
        Vector2 ownerAimPoint = unitCenter + BraveMathCollege.DegreesToVector(angle + UnityEngine.Random.Range(-90f, 90f), UnityEngine.Random.Range(this.MinSpawnRadius, this.MaxSpawnRadius));
        double num1 = (double) this.m_gun.HandleAimRotation((Vector3) ownerAimPoint);
        int num2 = (int) this.m_gun.Attack(this.m_overrideProjectileData);
        if (!(bool) (UnityEngine.Object) this.m_gun.LastProjectile || !(this.m_gun.LastProjectile is SpawningProjectile))
          return;
        SpawningProjectile lastProjectile = this.m_gun.LastProjectile as SpawningProjectile;
        float num3 = (ownerAimPoint - lastProjectile.transform.position.XY()).magnitude / lastProjectile.baseData.speed;
        lastProjectile.gravity = (float) (-2.0 * (double) lastProjectile.startingHeight / ((double) num3 * (double) num3));
        this.m_spawningProjectiles.Add(lastProjectile);
        this.m_gun.LastProjectile.collidesWithPlayer = false;
        this.m_gun.LastProjectile.UpdateCollisionMask();
      }
      else
      {
        if (angleOffset.HasValue)
        {
          this.m_gun.DefaultModule.angleFromAim = angleOffset.Value;
          this.m_gun.DefaultModule.angleVariance = 0.0f;
          this.m_gun.DefaultModule.alternateAngle = false;
        }
        int num = (int) this.m_gun.Attack(this.m_overrideProjectileData);
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

    [Serializable]
    public class DirectionalAnimationBool
    {
      public bool Back;
      public bool BackRight;
      public bool ForwardRight;
      public bool Forward;
      public bool ForwardLeft;
      public bool BackLeft;
    }
  }

