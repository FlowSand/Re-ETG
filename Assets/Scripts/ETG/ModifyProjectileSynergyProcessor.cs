// Decompiled with JetBrains decompiler
// Type: ModifyProjectileSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class ModifyProjectileSynergyProcessor : MonoBehaviour
{
  [LongNumericEnum]
  public CustomSynergyType SynergyToCheck;
  public bool TintsBullets;
  public Color BulletTint;
  [Header("Spawn Proj Modifiers")]
  public bool IncreaseSpawnedProjectileCount;
  [ShowInInspectorIf("IncreaseSpawnedProjectileCount", false)]
  public float SpawnedProjectileCountMultiplier = 2f;
  public bool IncreasesSpawnProjectileRate;
  [ShowInInspectorIf("IncreasesSpawnProjectileRate", false)]
  public float SpawnProjectileRateMultiplier = 1f;
  public bool AddsSpawnedProjectileInFlight;
  [ShowInInspectorIf("AddsSpawnedProjectileInFlight", false)]
  public Projectile AddFlightSpawnedProjectile;
  [ShowInInspectorIf("AddsSpawnedProjectileInFlight", false)]
  public float InFlightSpawnCooldown = 1f;
  [ShowInInspectorIf("AddsSpawnedProjectileInFlight", false)]
  public string InFlightAudioEvent;
  public bool AddsSpawnedProjectileOnDeath;
  [ShowInInspectorIf("AddsSpawnedProjectileOnDeath", false)]
  public Projectile AddDeathSpawnedProjectile;
  [ShowInInspectorIf("AddsSpawnedProjectileOnDeath", false)]
  public int NumDeathSpawnProjectiles;
  [ShowInInspectorIf("AddsSpawnedProjectileOnDeath", false)]
  public bool OnlySpawnDeathProjectilesInAir;
  [Header("Other Settings")]
  public int AddsBounces;
  public int AddsPierces;
  public bool AddsHoming;
  [ShowInInspectorIf("AddsHoming", false)]
  public float HomingRadius = 5f;
  [ShowInInspectorIf("AddsHoming", false)]
  public float HomingAngularVelocity = 360f;
  [ShowInInspectorIf("AddsHoming", false)]
  public bool HomingIsLockOn;
  [ShowInInspectorIf("HomingIsLockOn", false)]
  public GameObject LockOnVFX;
  public bool OverridesPreviousEffects;
  public bool AddsFire;
  public GameActorFireEffect FireEffect;
  public bool AddsPoison;
  public GameActorHealthEffect PoisonEffect;
  public bool AddsFreeze;
  public GameActorFreezeEffect FreezeEffect;
  public bool AddsSlow;
  public GameActorSpeedEffect SpeedEffect;
  public bool CopiesDevolverModifier;
  [ShowInInspectorIf("CopiesDevolverModifier", false)]
  public DevolverModifier DevolverSourceModifier;
  public bool AddsExplosion;
  public ExplosionData Explosion;
  public float BossDamageMultiplier = 1f;
  public float DamageMultiplier = 1f;
  public float RangeMultiplier = 1f;
  public float ScaleMultiplier = 1f;
  public float SpeedMultiplier = 1f;
  public bool AddsAccelCurve;
  public AnimationCurve AccelCurve;
  public float AccelCurveTime = 1f;
  public bool AddsChainLightning;
  [ShowInInspectorIf("AddsChainLightning", false)]
  public GameObject ChainLinkVFX;
  public bool AddsTransmogrifyChance;
  public float TransmogrifyChance;
  [EnemyIdentifier]
  public string[] TransmogrifyTargetGuids;
  public bool AddsStun;
  public float StunChance;
  public float StunDuration = 2f;
  public bool Dejams;
  public bool Blanks;
  private Projectile m_projectile;

  private void Awake()
  {
    this.m_projectile = this.GetComponent<Projectile>();
    if (this.Dejams)
      this.m_projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.DejamEnemy);
    if (!this.Blanks)
      return;
    this.m_projectile.OnDestruction += new Action<Projectile>(this.DoBlank);
  }

  private void DoBlank(Projectile obj)
  {
    if (!(bool) (UnityEngine.Object) this.m_projectile || !(this.m_projectile.Owner is PlayerController))
      return;
    (this.m_projectile.Owner as PlayerController).ForceBlank(overrideCenter: new Vector2?(this.m_projectile.specRigidbody.UnitCenter));
  }

  private void DejamEnemy(Projectile source, SpeculativeRigidbody target, bool kill)
  {
    if (!(bool) (UnityEngine.Object) target || !(bool) (UnityEngine.Object) target.aiActor || !target.aiActor.IsBlackPhantom)
      return;
    target.aiActor.UnbecomeBlackPhantom();
  }

  private void Start()
  {
    PlayerController owner = this.m_projectile.Owner as PlayerController;
    if (!(bool) (UnityEngine.Object) owner || !owner.HasActiveBonusSynergy(this.SynergyToCheck))
      return;
    if (this.TintsBullets)
      this.m_projectile.AdjustPlayerProjectileTint(this.BulletTint, 0);
    if (this.IncreaseSpawnedProjectileCount)
    {
      SpawnProjModifier component = this.m_projectile.GetComponent<SpawnProjModifier>();
      component.numToSpawnInFlight = (int) ((double) component.numToSpawnInFlight * (double) this.SpawnedProjectileCountMultiplier);
      component.numberToSpawnOnCollison = (int) ((double) component.numberToSpawnOnCollison * (double) this.SpawnedProjectileCountMultiplier);
    }
    if (this.IncreasesSpawnProjectileRate)
      this.m_projectile.GetComponent<SpawnProjModifier>().inFlightSpawnCooldown *= this.SpawnProjectileRateMultiplier;
    if (this.AddsSpawnedProjectileInFlight && !(bool) (UnityEngine.Object) this.m_projectile.GetComponent<SpawnProjModifier>())
    {
      SpawnProjModifier spawnProjModifier = this.m_projectile.gameObject.AddComponent<SpawnProjModifier>();
      spawnProjModifier.spawnProjectilesInFlight = true;
      spawnProjModifier.projectileToSpawnInFlight = this.AddFlightSpawnedProjectile;
      spawnProjModifier.numToSpawnInFlight = 1;
      spawnProjModifier.inFlightSpawnCooldown = this.InFlightSpawnCooldown;
      spawnProjModifier.inFlightAimAtEnemies = true;
      spawnProjModifier.spawnAudioEvent = this.InFlightAudioEvent;
    }
    if (this.AddsSpawnedProjectileOnDeath && !(bool) (UnityEngine.Object) this.m_projectile.GetComponent<SpawnProjModifier>())
    {
      SpawnProjModifier spawnProjModifier = this.m_projectile.gameObject.AddComponent<SpawnProjModifier>();
      spawnProjModifier.spawnProjectilesOnCollision = !this.OnlySpawnDeathProjectilesInAir;
      spawnProjModifier.spawnProjecitlesOnDieInAir = true;
      spawnProjModifier.projectileToSpawnOnCollision = this.AddDeathSpawnedProjectile;
      if (this.OnlySpawnDeathProjectilesInAir)
        spawnProjModifier.collisionSpawnStyle = SpawnProjModifier.CollisionSpawnStyle.FLAK_BURST;
      if (this.NumDeathSpawnProjectiles == 1)
        spawnProjModifier.alignToSurfaceNormal = true;
      spawnProjModifier.numberToSpawnOnCollison = this.NumDeathSpawnProjectiles;
    }
    if (this.AddsFire && (!this.m_projectile.AppliesFire || this.OverridesPreviousEffects))
    {
      this.m_projectile.AppliesFire = true;
      this.m_projectile.fireEffect = this.FireEffect;
    }
    if (this.AddsPoison && (!this.m_projectile.AppliesPoison || this.OverridesPreviousEffects))
    {
      this.m_projectile.AppliesPoison = true;
      this.m_projectile.healthEffect = this.PoisonEffect;
    }
    if (this.AddsFreeze && (!this.m_projectile.AppliesFreeze || this.OverridesPreviousEffects))
    {
      this.m_projectile.AppliesFreeze = true;
      this.m_projectile.freezeEffect = this.FreezeEffect;
    }
    if (this.AddsSlow && (!this.m_projectile.AppliesSpeedModifier || this.OverridesPreviousEffects))
    {
      this.m_projectile.AppliesSpeedModifier = true;
      this.m_projectile.speedEffect = this.SpeedEffect;
    }
    if (this.AddsExplosion && !(bool) (UnityEngine.Object) this.m_projectile.GetComponent<ExplosiveModifier>())
      this.m_projectile.gameObject.AddComponent<ExplosiveModifier>().explosionData = this.Explosion;
    if (this.AddsHoming)
    {
      if (this.HomingIsLockOn)
      {
        LockOnHomingModifier onHomingModifier = this.m_projectile.GetComponent<LockOnHomingModifier>();
        if (!(bool) (UnityEngine.Object) onHomingModifier)
        {
          onHomingModifier = this.m_projectile.gameObject.AddComponent<LockOnHomingModifier>();
          onHomingModifier.HomingRadius = 0.0f;
          onHomingModifier.AngularVelocity = 0.0f;
        }
        onHomingModifier.HomingRadius += this.HomingRadius;
        onHomingModifier.AngularVelocity += this.HomingAngularVelocity;
        onHomingModifier.LockOnVFX = this.LockOnVFX;
      }
      else
      {
        HomingModifier homingModifier = this.m_projectile.GetComponent<HomingModifier>();
        if (!(bool) (UnityEngine.Object) homingModifier)
        {
          homingModifier = this.m_projectile.gameObject.AddComponent<HomingModifier>();
          homingModifier.HomingRadius = 0.0f;
          homingModifier.AngularVelocity = 0.0f;
        }
        homingModifier.HomingRadius += this.HomingRadius;
        homingModifier.AngularVelocity += this.HomingAngularVelocity;
      }
    }
    if (this.AddsBounces > 0)
      this.m_projectile.gameObject.GetOrAddComponent<BounceProjModifier>().numberOfBounces += this.AddsBounces;
    if (this.AddsPierces > 0)
      this.m_projectile.gameObject.GetOrAddComponent<PierceProjModifier>().penetration += this.AddsPierces;
    if (this.CopiesDevolverModifier)
    {
      DevolverModifier devolverModifier = this.m_projectile.gameObject.AddComponent<DevolverModifier>();
      devolverModifier.chanceToDevolve = this.DevolverSourceModifier.chanceToDevolve;
      devolverModifier.DevolverHierarchy = this.DevolverSourceModifier.DevolverHierarchy;
      devolverModifier.EnemyGuidsToIgnore = this.DevolverSourceModifier.EnemyGuidsToIgnore;
    }
    if (this.AddsChainLightning && !(bool) (UnityEngine.Object) this.m_projectile.GetComponent<ChainLightningModifier>())
    {
      ChainLightningModifier lightningModifier = this.m_projectile.gameObject.AddComponent<ChainLightningModifier>();
      lightningModifier.LinkVFXPrefab = this.ChainLinkVFX;
      lightningModifier.maximumLinkDistance = 7f;
      lightningModifier.damagePerHit = 5f;
      lightningModifier.damageCooldown = 1f;
    }
    if ((double) this.BossDamageMultiplier != 1.0)
      this.m_projectile.BossDamageMultiplier *= this.BossDamageMultiplier;
    if ((double) this.DamageMultiplier != 1.0)
      this.m_projectile.baseData.damage *= this.DamageMultiplier;
    if ((double) this.RangeMultiplier != 1.0)
      this.m_projectile.baseData.range *= this.RangeMultiplier;
    if ((double) this.ScaleMultiplier != 1.0)
      this.m_projectile.RuntimeUpdateScale(this.ScaleMultiplier);
    if ((double) this.SpeedMultiplier != 1.0)
    {
      this.m_projectile.baseData.speed *= this.SpeedMultiplier;
      this.m_projectile.UpdateSpeed();
    }
    if (this.AddsAccelCurve)
    {
      this.m_projectile.baseData.AccelerationCurve = this.AccelCurve;
      this.m_projectile.baseData.UsesCustomAccelerationCurve = true;
      this.m_projectile.baseData.CustomAccelerationCurveDuration = this.AccelCurveTime;
    }
    if (this.AddsTransmogrifyChance && !this.m_projectile.CanTransmogrify)
    {
      this.m_projectile.CanTransmogrify = true;
      this.m_projectile.ChanceToTransmogrify = this.TransmogrifyChance;
      this.m_projectile.TransmogrifyTargetGuids = this.TransmogrifyTargetGuids;
    }
    if (!this.AddsStun || this.m_projectile.AppliesStun)
      return;
    this.m_projectile.AppliesStun = true;
    this.m_projectile.StunApplyChance = this.StunChance;
    this.m_projectile.AppliedStunDuration = this.StunDuration;
  }
}
