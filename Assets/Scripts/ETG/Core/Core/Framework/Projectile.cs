using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.Serialization;

using Brave.BulletScript;
using Dungeonator;
using PathologicalGames;

#nullable disable

public class Projectile : BraveBehaviour
    {
        public static bool s_delayPlayerDamage;
        public static float s_maxProjectileScale = 3.5f;
        private static float s_enemyBulletSpeedModfier = 1f;
        private static float s_baseEnemyBulletSpeedMultiplier = 1f;
        [NonSerialized]
        public Gun PossibleSourceGun;
        [NonSerialized]
        public bool SpawnedFromOtherPlayerProjectile;
        [NonSerialized]
        public float PlayerProjectileSourceGameTimeslice = -1f;
        [NonSerialized]
        private GameActor m_owner;
        [FormerlySerializedAs("BulletMLSettings")]
        public BulletScriptSettings BulletScriptSettings;
        [EnumFlags]
        public CoreDamageTypes damageTypes;
        public bool allowSelfShooting;
        public bool collidesWithPlayer = true;
        public bool collidesWithProjectiles;
        [ShowInInspectorIf("collidesWithProjectiles", true)]
        public bool collidesOnlyWithPlayerProjectiles;
        [ShowInInspectorIf("collidesWithProjectiles", true)]
        public int projectileHitHealth;
        public bool collidesWithEnemies = true;
        public bool shouldRotate;
        [FormerlySerializedAs("shouldFlip")]
        [ShowInInspectorIf("shouldRotate", false)]
        public bool shouldFlipVertically;
        public bool shouldFlipHorizontally;
        public bool ignoreDamageCaps;
        [NonSerialized]
        private float m_cachedInitialDamage = -1f;
        public ProjectileData baseData;
        public bool AppliesPoison;
        public float PoisonApplyChance = 1f;
        public GameActorHealthEffect healthEffect;
        public bool AppliesSpeedModifier;
        public float SpeedApplyChance = 1f;
        public GameActorSpeedEffect speedEffect;
        public bool AppliesCharm;
        public float CharmApplyChance = 1f;
        public GameActorCharmEffect charmEffect;
        public bool AppliesFreeze;
        public float FreezeApplyChance = 1f;
        public GameActorFreezeEffect freezeEffect;
        public bool AppliesFire;
        public float FireApplyChance = 1f;
        public GameActorFireEffect fireEffect;
        public bool AppliesStun;
        public float StunApplyChance = 1f;
        public float AppliedStunDuration = 1f;
        public bool AppliesBleed;
        public GameActorBleedEffect bleedEffect;
        public bool AppliesCheese;
        public float CheeseApplyChance = 1f;
        public GameActorCheeseEffect cheeseEffect;
        public float BleedApplyChance = 1f;
        public bool CanTransmogrify;
        [ShowInInspectorIf("CanTransmogrify", false)]
        public float ChanceToTransmogrify;
        [EnemyIdentifier]
        public string[] TransmogrifyTargetGuids;
        [NonSerialized]
        public float BossDamageMultiplier = 1f;
        [NonSerialized]
        public bool SpawnedFromNonChallengeItem;
        [NonSerialized]
        public bool TreatedAsNonProjectileForChallenge;
        public ProjectileImpactVFXPool hitEffects;
        public bool CenterTilemapHitEffectsByProjectileVelocity;
        public VFXPool wallDecals;
        public bool damagesWalls = true;
        public float persistTime = 0.25f;
        public float angularVelocity;
        public float angularVelocityVariance;
        [EnemyIdentifier]
        public string spawnEnemyGuidOnDeath;
        public bool HasFixedKnockbackDirection;
        public float FixedKnockbackDirection;
        public bool pierceMinorBreakables;
        [Header("Audio Flags")]
        public string objectImpactEventName = string.Empty;
        public string enemyImpactEventName = string.Empty;
        public string onDestroyEventName = string.Empty;
        public string additionalStartEventName = string.Empty;
        [Header("Unusual Options")]
        public bool IsRadialBurstLimited;
        public int MaxRadialBurstLimit = -1;
        public SynergyBurstLimit[] AdditionalBurstLimits;
        public bool AppliesKnockbackToPlayer;
        public float PlayerKnockbackForce;
        public bool HasDefaultTint;
        [ShowInInspectorIf("HasDefaultTint", false)]
        public Color DefaultTintColor;
        [NonSerialized]
        public bool IsCritical;
        [NonSerialized]
        public float BlackPhantomDamageMultiplier = 1f;
        [Header("For Brents")]
        public bool PenetratesInternalWalls;
        public bool neverMaskThis;
        public bool isFakeBullet;
        public bool CanBecomeBlackBullet = true;
        public TrailRenderer TrailRenderer;
        public CustomTrailRenderer CustomTrailRenderer;
        public ParticleSystem ParticleTrail;
        public bool DelayedDamageToExploders;
        public Action<Projectile, SpeculativeRigidbody, bool> OnHitEnemy;
        public Action<Projectile, SpeculativeRigidbody> OnWillKillEnemy;
        public Action<DebrisObject> OnBecameDebris;
        public Action<DebrisObject> OnBecameDebrisGrounded;
        [NonSerialized]
        public bool IsBlackBullet;
        private bool m_forceBlackBullet;
        [NonSerialized]
        public List<GameActorEffect> statusEffectsToApply = new List<GameActorEffect>();
        private bool m_initialized;
        private Transform m_transform;
        private bool? m_cachedHasBeamController;
        public float AdditionalScaleMultiplier = 1f;
        private int m_cachedLayer;
        private int m_currentTintPriority = -1;
        public Func<Vector2, Vector2> ModifyVelocity;
        [NonSerialized]
        public bool CurseSparks;
        private Vector2? m_lastSparksPoint;
        public Action<Projectile> PreMoveModifiers;
        [NonSerialized]
        public ProjectileMotionModule OverrideMotionModule;
        [NonSerialized]
        protected bool m_usesNormalMoveRegardless;
        public static Dungeonator.Dungeon m_cachedDungeon;
        public static int m_cacheTick;
        protected bool m_isInWall;
        private SpeculativeRigidbody m_shooter;
        protected float m_currentSpeed;
        protected Vector2 m_currentDirection;
        protected MeshRenderer m_renderer;
        protected float m_timeElapsed;
        protected float m_distanceElapsed;
        protected Vector3 m_lastPosition;
        protected bool m_hasImpactedObject;
        protected bool m_hasImpactedEnemy;
        protected bool m_hasDiedInAir;
        protected bool m_hasPierced;
        private int m_healthHaverHitCount;
        private bool m_cachedCollidesWithPlayer;
        private bool m_cachedCollidesWithProjectiles;
        private bool m_cachedCollidesWithEnemies;
        private bool m_cachedDamagesWalls;
        private ProjectileData m_cachedBaseData;
        private BulletScriptSettings m_cachedBulletScriptSettings;
        private bool m_cachedCollideWithTileMap;
        private bool m_cachedCollideWithOthers;
        private int m_cachedSpriteId = -1;
        private PrefabPool m_spawnPool;
        private bool m_isRamping;
        private float m_rampTimer;
        private float m_rampDuration;
        private float m_currentRampHeight;
        private float m_startRampHeight;
        private float m_ignoreTileCollisionsTimer;
        private float m_outOfBoundsCounter;
        private bool m_isExitClippingTiles;
        private float m_exitClippingDistance;
        public static float CurrentProjectileDepth = 0.8f;
        public const float c_DefaultProjectileDepth = 0.8f;

        public static float EnemyBulletSpeedMultiplier => Projectile.s_enemyBulletSpeedModfier;

        public static void UpdateEnemyBulletSpeedMultiplier()
        {
            float num = 1f;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                num = GameManager.Instance.COOP_ENEMY_PROJECTILE_SPEED_MULTIPLIER;
            if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null)
                Projectile.s_enemyBulletSpeedModfier = Projectile.s_baseEnemyBulletSpeedMultiplier * GameManager.Instance.Dungeon.GetNewPlayerSpeedMultiplier() * PlayerStats.GetTotalEnemyProjectileSpeedMultiplier() * num;
            else
                Projectile.s_enemyBulletSpeedModfier = Projectile.s_baseEnemyBulletSpeedMultiplier;
        }

        public static float BaseEnemyBulletSpeedMultiplier
        {
            get => Projectile.s_baseEnemyBulletSpeedMultiplier;
            set
            {
                Projectile.s_baseEnemyBulletSpeedMultiplier = value;
                Projectile.UpdateEnemyBulletSpeedMultiplier();
            }
        }

        public BulletScriptBehavior braveBulletScript { get; set; }

        [HideInInspector]
        public GameActor Owner
        {
            get => this.m_owner;
            set
            {
                this.m_owner = value;
                if (this.m_owner is AIActor)
                    this.OwnerName = (this.m_owner as AIActor).GetActorName();
                else if (this.m_owner is PlayerController)
                {
                    if ((UnityEngine.Object) this.PossibleSourceGun == (UnityEngine.Object) null)
                        this.PossibleSourceGun = (this.m_owner as PlayerController).CurrentGun;
                    this.OwnerName = !(this.m_owner as PlayerController).IsPrimaryPlayer ? "secondaryplayer" : "primaryplayer";
                }
                this.CheckBlackPhantomness();
            }
        }

        public ProjectileTrapController TrapOwner { get; set; }

        public string OwnerName { get; set; }

        public void SetOwnerSafe(GameActor owner, string ownerName)
        {
            this.m_owner = owner;
            this.OwnerName = ownerName;
            this.CheckBlackPhantomness();
        }

        public float GetCachedBaseDamage => this.m_cachedInitialDamage;

        public float ModifiedDamage => this.baseData.damage;

        public bool SuppressHitEffects { get; set; }

        public event Action<Projectile> OnPostUpdate;

        public event Action<Projectile> OnReflected;

        public event Action<Projectile> OnDestruction;

        protected float LocalTimeScale
        {
            get
            {
                if ((bool) (UnityEngine.Object) this.Owner && this.Owner is AIActor)
                    return (this.Owner as AIActor).LocalTimeScale;
                return (bool) (UnityEngine.Object) this.TrapOwner ? this.TrapOwner.LocalTimeScale : UnityEngine.Time.timeScale;
            }
        }

        public float LocalDeltaTime
        {
            get
            {
                return (bool) (UnityEngine.Object) this.Owner && this.Owner is AIActor ? (this.Owner as AIActor).LocalDeltaTime : BraveTime.DeltaTime;
            }
        }

        public SpeculativeRigidbody Shooter
        {
            get => this.m_shooter;
            set
            {
                this.m_shooter = value;
                if (this.allowSelfShooting)
                    return;
                this.specRigidbody.RegisterSpecificCollisionException(this.m_shooter);
            }
        }

        public float Speed
        {
            get => this.m_currentSpeed;
            set => this.m_currentSpeed = value;
        }

        public Vector2 Direction
        {
            get => this.m_currentDirection;
            set => this.m_currentDirection = value;
        }

        public bool CanKillBosses
        {
            get
            {
                return !((UnityEngine.Object) this.Owner == (UnityEngine.Object) null) && this.Owner is PlayerController && (this.Owner as PlayerController).BossKillingMode;
            }
        }

        public Projectile.ProjectileDestroyMode DestroyMode { get; set; }

        public bool Inverted { get; set; }

        public Vector2 LastVelocity { get; set; }

        public bool ManualControl { get; set; }

        public bool ForceBlackBullet
        {
            get => this.m_forceBlackBullet;
            set
            {
                if (this.m_forceBlackBullet != value)
                {
                    this.m_forceBlackBullet = value;
                    this.CheckBlackPhantomness();
                }
                else
                    this.m_forceBlackBullet = value;
            }
        }

        public bool IsBulletScript { get; set; }

        public bool CanBeKilledByExplosions
        {
            get
            {
                if (!this.m_cachedHasBeamController.HasValue)
                    this.m_cachedHasBeamController = new bool?((bool) (UnityEngine.Object) this.GetComponent<BeamController>());
                return !this.m_cachedHasBeamController.Value && !this.ImmuneToBlanks && !this.ImmuneToSustainedBlanks;
            }
        }

        public bool CanBeCaught
        {
            get
            {
                PierceProjModifier component = this.GetComponent<PierceProjModifier>();
                return (!((UnityEngine.Object) component != (UnityEngine.Object) null) || component.BeastModeLevel == PierceProjModifier.BeastModeStatus.NOT_BEAST_MODE) && (bool) (UnityEngine.Object) this.sprite;
            }
        }

        public float ElapsedTime => this.m_timeElapsed;

        public Vector2? OverrideTrailPoint { get; set; }

        public bool SkipDistanceElapsedCheck { get; set; }

        public bool ImmuneToBlanks { get; set; }

        public bool ImmuneToSustainedBlanks { get; set; }

        public bool ForcePlayerBlankable { get; set; }

        public bool IsReflectedBySword { get; set; }

        public int LastReflectedSlashId { get; set; }

        public ProjectileTrailRendererController TrailRendererController { get; set; }

        public static void SetGlobalProjectileDepth(float newDepth)
        {
            Projectile.CurrentProjectileDepth = newDepth;
        }

        public static void ResetGlobalProjectileDepth() => Projectile.CurrentProjectileDepth = 0.8f;

        public void Awake()
        {
            if (this.baseData == null)
                this.baseData = new ProjectileData();
            if (this.BulletScriptSettings == null)
                this.BulletScriptSettings = new BulletScriptSettings();
            this.m_transform = this.transform;
            this.m_cachedInitialDamage = this.baseData.damage;
            if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
            {
                if (this.PenetratesInternalWalls)
                    this.specRigidbody.OnPreTileCollision += new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreTileCollision);
                this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
                this.specRigidbody.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
                this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
            }
            if (!(bool) (UnityEngine.Object) this.sprite)
                this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
            if (!(bool) (UnityEngine.Object) this.spriteAnimator && (bool) (UnityEngine.Object) this.sprite)
                this.spriteAnimator = this.sprite.spriteAnimator;
            if (!((UnityEngine.Object) this.m_renderer == (UnityEngine.Object) null))
                return;
            this.m_renderer = this.GetComponentInChildren<MeshRenderer>();
        }

        public void Reawaken()
        {
            if (!(bool) (UnityEngine.Object) this.sprite)
                this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
            if (!(bool) (UnityEngine.Object) this.spriteAnimator && (bool) (UnityEngine.Object) this.sprite)
                this.spriteAnimator = this.sprite.spriteAnimator;
            if (!((UnityEngine.Object) this.m_renderer == (UnityEngine.Object) null))
                return;
            this.m_renderer = this.GetComponentInChildren<MeshRenderer>();
        }

        public void RuntimeUpdateScale(float multiplier)
        {
            if (!(bool) (UnityEngine.Object) this.sprite)
                return;
            float num = Mathf.Clamp(this.sprite.scale.x * multiplier, 0.01f, Projectile.s_maxProjectileScale);
            this.AdditionalScaleMultiplier *= multiplier;
            this.sprite.scale = new Vector3(num, num, num);
            if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
                this.specRigidbody.UpdateCollidersOnScale = true;
            if ((double) num <= 1.5)
                return;
            Vector3 size = this.sprite.GetBounds().size;
            if ((double) size.x <= 4.0 && (double) size.y <= 4.0)
                return;
            this.sprite.HeightOffGround = UnityEngine.Random.Range(0.0f, -3f);
        }

        public virtual void Start()
        {
            if (this.m_initialized)
                return;
            this.m_initialized = true;
            this.m_transform = this.transform;
            if (!string.IsNullOrEmpty(this.additionalStartEventName))
            {
                int num1 = (int) AkSoundEngine.PostEvent(this.additionalStartEventName, this.gameObject);
            }
            StaticReferenceManager.AddProjectile(this);
            if ((bool) (UnityEngine.Object) this.GetComponent<BeamController>())
            {
                this.enabled = false;
            }
            else
            {
                if ((bool) (UnityEngine.Object) this.m_renderer)
                    DepthLookupManager.ProcessRenderer((Renderer) this.m_renderer);
                if ((bool) (UnityEngine.Object) this.sprite)
                {
                    this.sprite.HeightOffGround = Projectile.CurrentProjectileDepth;
                    this.m_currentRampHeight = 0.0f;
                    float num2 = BraveMathCollege.ClampAngle360(this.m_transform.eulerAngles.z);
                    if (this.Owner is PlayerController)
                    {
                        float num3 = Mathf.Clamp((this.Owner as PlayerController).BulletScaleModifier * this.AdditionalScaleMultiplier, 0.01f, Projectile.s_maxProjectileScale);
                        this.sprite.scale = new Vector3(num3, num3, num3);
                        if ((double) num3 != 1.0)
                        {
                            if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
                            {
                                this.specRigidbody.UpdateCollidersOnScale = true;
                                this.specRigidbody.ForceRegenerate();
                            }
                            if ((UnityEngine.Object) this.sprite.transform != (UnityEngine.Object) this.m_transform)
                                this.sprite.transform.localPosition = Vector3.Scale(this.sprite.transform.localPosition, this.sprite.scale);
                            this.DoWallExitClipping();
                        }
                        if (this.HasDefaultTint)
                            this.AdjustPlayerProjectileTint(this.DefaultTintColor, 0);
                    }
                    if (this.shouldRotate && this.shouldFlipVertically)
                        this.sprite.FlipY = (double) num2 < 270.0 && (double) num2 > 90.0;
                    if (this.shouldFlipHorizontally)
                        this.sprite.FlipX = (double) num2 > 90.0 && (double) num2 < 270.0;
                }
                if ((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null && this.Owner is PlayerController)
                {
                    this.specRigidbody.UpdateCollidersOnRotation = true;
                    this.specRigidbody.UpdateCollidersOnScale = true;
                }
                if (this.isFakeBullet)
                {
                    this.enabled = false;
                    this.sprite.HeightOffGround = Projectile.CurrentProjectileDepth;
                    this.sprite.UpdateZDepth();
                }
                else
                {
                    if ((UnityEngine.Object) this.specRigidbody == (UnityEngine.Object) null)
                        UnityEngine.Debug.LogError((object) "No speculative rigidbody found on projectile!", (UnityEngine.Object) this);
                    if (GameManager.PVP_ENABLED && !this.TreatedAsNonProjectileForChallenge)
                        this.collidesWithPlayer = true;
                    if (this.collidesWithPlayer && this.Owner is AIActor && (UnityEngine.Object) (this.Owner as AIActor).CompanionOwner != (UnityEngine.Object) null)
                        this.collidesWithPlayer = false;
                    if (this.collidesWithProjectiles)
                    {
                        for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                            this.specRigidbody.PixelColliders[index].CollisionLayerCollidableOverride |= CollisionMask.LayerToMask(CollisionLayer.Projectile);
                    }
                    if (!this.collidesWithPlayer)
                    {
                        for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                            this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox);
                    }
                    if (!this.collidesWithEnemies)
                    {
                        for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                            this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
                    }
                    if (this.Owner is PlayerController)
                    {
                        for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                            this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker);
                    }
                    else if (this.Owner is AIActor && this.collidesWithEnemies && PassiveItem.IsFlagSetAtAll(typeof (BattleStandardItem)))
                        this.baseData.damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
                    if (this.Owner is PlayerController)
                        this.PostprocessPlayerBullet();
                    if (this.specRigidbody.UpdateCollidersOnRotation)
                        this.specRigidbody.ForceRegenerate();
                    this.m_timeElapsed = 0.0f;
                    this.LastPosition = this.m_transform.position;
                    this.m_currentSpeed = this.baseData.speed;
                    this.m_currentDirection = (Vector2) this.m_transform.right;
                    if (!this.shouldRotate)
                        this.m_transform.rotation = Quaternion.identity;
                    if (this.CanKillBosses)
                        this.StartCoroutine(this.CheckIfBossKillShot());
                    if (!this.shouldRotate)
                    {
                        this.specRigidbody.IgnorePixelGrid = true;
                        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
                    }
                    if ((double) this.angularVelocity != 0.0)
                        this.angularVelocity = BraveUtility.RandomSign() * this.angularVelocity + UnityEngine.Random.Range(-this.angularVelocityVariance, this.angularVelocityVariance);
                    this.CheckBlackPhantomness();
                }
            }
        }

        private void CheckBlackPhantomness()
        {
            if (this.CanBecomeBlackBullet && (this.ForceBlackBullet || this.Owner is AIActor && (this.Owner as AIActor).IsBlackPhantom))
            {
                this.BecomeBlackBullet();
            }
            else
            {
                if (!this.IsBlackBullet)
                    return;
                this.ReturnFromBlackBullet();
            }
        }

        public int GetRadialBurstLimit(PlayerController source)
        {
            int a = int.MaxValue;
            for (int index = 0; index < this.AdditionalBurstLimits.Length; ++index)
            {
                if (source.HasActiveBonusSynergy(this.AdditionalBurstLimits[index].RequiredSynergy))
                    a = Mathf.Min(a, this.AdditionalBurstLimits[index].limit);
            }
            if (this.IsRadialBurstLimited && this.MaxRadialBurstLimit > -1)
                a = Mathf.Min(a, this.MaxRadialBurstLimit);
            return a;
        }

        public void CacheLayer(int targetLayer)
        {
            if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
                return;
            this.m_cachedLayer = this.sprite.gameObject.layer;
            this.gameObject.SetLayerRecursively(targetLayer);
        }

        public void DecacheLayer()
        {
            if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
                return;
            this.gameObject.SetLayerRecursively(this.m_cachedLayer);
        }

        private void PostprocessPlayerBullet()
        {
            PlayerController owner = this.Owner as PlayerController;
            int num1 = Mathf.FloorToInt(owner.stats.GetStatValue(PlayerStats.StatType.AdditionalShotPiercing));
            if ((bool) (UnityEngine.Object) this.PossibleSourceGun && this.PossibleSourceGun.gunClass == GunClass.SHOTGUN && owner.HasActiveBonusSynergy(CustomSynergyType.SHOTGUN_SPEED))
            {
                this.baseData.speed *= 2f;
                this.baseData.force *= 3f;
                ++num1;
            }
            if (num1 > 0)
            {
                PierceProjModifier component = this.GetComponent<PierceProjModifier>();
                if ((UnityEngine.Object) component == (UnityEngine.Object) null)
                {
                    PierceProjModifier pierceProjModifier = this.gameObject.AddComponent<PierceProjModifier>();
                    pierceProjModifier.penetration = num1;
                    pierceProjModifier.penetratesBreakables = true;
                    pierceProjModifier.BeastModeLevel = PierceProjModifier.BeastModeStatus.NOT_BEAST_MODE;
                }
                else
                    component.penetration += num1;
            }
            int num2 = Mathf.FloorToInt(owner.stats.GetStatValue(PlayerStats.StatType.AdditionalShotBounces));
            if (num2 <= 0)
                return;
            BounceProjModifier component1 = this.GetComponent<BounceProjModifier>();
            if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
                this.gameObject.AddComponent<BounceProjModifier>().numberOfBounces = num2;
            else
                component1.numberOfBounces += num2;
        }

        public void AdjustPlayerProjectileTint(Color targetTintColor, int priority, float lerpTime = 0.0f)
        {
            if (priority <= this.m_currentTintPriority && (priority != this.m_currentTintPriority || (double) UnityEngine.Random.value >= 0.5))
                return;
            this.m_currentTintPriority = priority;
            if (!(this.Owner is PlayerController))
                return;
            this.ChangeTintColorShader(lerpTime, targetTintColor);
        }

        public void RemovePlayerOnlyModifiers()
        {
            HomingModifier component = this.GetComponent<HomingModifier>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object) component);
        }

        public void MakeLookLikeEnemyBullet(bool applyScaleChanges = true)
        {
            if ((bool) (UnityEngine.Object) this.specRigidbody && (bool) (UnityEngine.Object) this.sprite && applyScaleChanges)
            {
                Bounds bounds = this.sprite.GetCurrentSpriteDef().GetBounds();
                float num1 = Mathf.Max(bounds.size.x, bounds.size.y);
                if ((double) num1 < 0.5)
                {
                    float num2 = 0.5f / num1;
                    UnityEngine.Debug.Log((object) $"{(object) num1}|{(object) num2}");
                    this.sprite.scale = new Vector3(num2, num2, num2);
                    if ((double) num2 != 1.0 && (UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null)
                    {
                        this.specRigidbody.UpdateCollidersOnScale = true;
                        this.specRigidbody.ForceRegenerate();
                    }
                }
            }
            if (!(bool) (UnityEngine.Object) this.sprite || !(bool) (UnityEngine.Object) this.sprite.renderer)
                return;
            Material sharedMaterial = this.sprite.renderer.sharedMaterial;
            this.sprite.usesOverrideMaterial = true;
            Material targetMaterial = new Material(ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive"));
            targetMaterial.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
            targetMaterial.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
            this.LerpMaterialGlow(targetMaterial, 0.0f, 22f, 0.4f);
            targetMaterial.SetFloat("_EmissiveColorPower", 8f);
            targetMaterial.SetColor("_EmissiveColor", Color.red);
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.red);
            this.sprite.renderer.material = targetMaterial;
        }

        private void HandleSparks(Vector2? overridePoint = null)
        {
            if (this.damageTypes == (this.damageTypes | CoreDamageTypes.Electric) && (bool) (UnityEngine.Object) this.specRigidbody)
            {
                Vector2 maxPosition = !overridePoint.HasValue ? this.specRigidbody.UnitCenter : overridePoint.Value;
                Vector2 minPosition = !this.m_lastSparksPoint.HasValue ? this.m_lastPosition.XY() : this.m_lastSparksPoint.Value;
                this.m_lastSparksPoint = new Vector2?(maxPosition);
                GlobalSparksDoer.DoLinearParticleBurst(Mathf.Max(1, (int) ((double) (this.m_lastPosition.XY() - maxPosition).magnitude * 6.0)), (Vector3) minPosition, (Vector3) maxPosition, 360f, 5f, 0.5f, startLifetime: new float?(0.2f), startColor: new Color?(new Color(0.25f, 0.25f, 1f, 1f)));
            }
            if (!this.CurseSparks || !(bool) (UnityEngine.Object) this.specRigidbody)
                return;
            Vector2 maxPosition1 = !overridePoint.HasValue ? this.specRigidbody.UnitCenter : overridePoint.Value;
            Vector2 minPosition1 = !this.m_lastSparksPoint.HasValue ? this.m_lastPosition.XY() : this.m_lastSparksPoint.Value;
            this.m_lastSparksPoint = new Vector2?(maxPosition1);
            GlobalSparksDoer.DoLinearParticleBurst(Mathf.Max(1, (int) ((double) (this.m_lastPosition.XY() - maxPosition1).magnitude * 3.0)), (Vector3) minPosition1, (Vector3) maxPosition1, 360f, 5f, 0.5f, startLifetime: new float?(0.2f), startColor: new Color?(new Color(0.25f, 0.25f, 1f, 1f)), systemType: GlobalSparksDoer.SparksType.DARK_MAGICKS);
        }

        public virtual void Update()
        {
            tk2dBaseSprite sprite = this.sprite;
            bool flag1 = (bool) (UnityEngine.Object) sprite;
            if (UnityEngine.Time.frameCount != Projectile.m_cacheTick)
            {
                Projectile.m_cachedDungeon = !(bool) (UnityEngine.Object) GameManager.Instance ? (Dungeon) null : GameManager.Instance.Dungeon;
                Projectile.m_cacheTick = UnityEngine.Time.frameCount;
            }
            if (this.IsBlackBullet)
            {
                if (!this.ForceBlackBullet && this.Owner is AIActor && !(this.Owner as AIActor).IsBlackPhantom)
                    this.ReturnFromBlackBullet();
                if ((bool) (UnityEngine.Object) this.Owner && !(this.Owner is AIActor))
                    this.ReturnFromBlackBullet();
            }
            if (this.m_isRamping && flag1)
            {
                float currentRampHeight = this.m_currentRampHeight;
                if ((double) this.m_rampTimer <= (double) this.m_rampDuration)
                {
                    this.m_currentRampHeight = Mathf.Lerp(this.m_startRampHeight, 0.0f, this.m_rampTimer / this.m_rampDuration);
                }
                else
                {
                    this.m_currentRampHeight = 0.0f;
                    this.m_isRamping = false;
                }
                sprite.HeightOffGround -= currentRampHeight - this.m_currentRampHeight;
                sprite.UpdateZDepthLater();
                float localDeltaTime = this.LocalDeltaTime;
                if (!(this.Owner is PlayerController))
                    localDeltaTime *= Projectile.EnemyBulletSpeedMultiplier;
                this.m_rampTimer += localDeltaTime;
            }
            if ((double) this.m_ignoreTileCollisionsTimer > 0.0)
            {
                float localDeltaTime = this.LocalDeltaTime;
                if (!(this.Owner is PlayerController))
                    localDeltaTime *= Projectile.EnemyBulletSpeedMultiplier;
                this.m_rampTimer += localDeltaTime;
                this.m_ignoreTileCollisionsTimer = Mathf.Max(0.0f, this.m_ignoreTileCollisionsTimer - localDeltaTime);
                if ((double) this.m_ignoreTileCollisionsTimer <= 0.0)
                    this.specRigidbody.CollideWithTileMap = true;
            }
            this.HandleSparks();
            if (!this.IsBulletScript)
            {
                this.HandleRange();
                if (!this.ManualControl)
                {
                    if (this.PreMoveModifiers != null)
                        this.PreMoveModifiers(this);
                    if (this.OverrideMotionModule != null && !this.m_usesNormalMoveRegardless)
                    {
                        this.OverrideMotionModule.Move(this, this.m_transform, this.sprite, this.specRigidbody, ref this.m_timeElapsed, ref this.m_currentDirection, this.Inverted, this.shouldRotate);
                        this.LastVelocity = this.specRigidbody.Velocity;
                    }
                    else
                        this.Move();
                }
                this.specRigidbody.Velocity *= this.LocalTimeScale;
                if (!(this.Owner is PlayerController))
                    this.specRigidbody.Velocity *= Projectile.EnemyBulletSpeedMultiplier;
                this.DoModifyVelocity();
            }
            Vector2 position = (Vector2) this.m_transform.position;
            if (this.m_isInWall && Projectile.m_cachedDungeon.data.CheckInBounds((int) position.x, (int) position.y))
            {
                CellData cellData = Projectile.m_cachedDungeon.data[(int) position.x, (int) position.y];
                if (cellData != null && cellData.type != CellType.WALL)
                    this.m_isInWall = false;
            }
            if ((this.shouldFlipHorizontally || this.shouldFlipVertically) && flag1)
            {
                if (this.shouldFlipHorizontally && this.shouldRotate && this.shouldFlipVertically)
                {
                    bool flag2 = (double) this.Direction.x < 0.0;
                    sprite.FlipX = flag2;
                    sprite.FlipY = flag2;
                }
                else if (this.shouldFlipHorizontally)
                    sprite.FlipX = (double) this.Direction.x < 0.0;
                else if (this.shouldRotate && this.shouldFlipVertically)
                    sprite.FlipY = (double) this.Direction.x < 0.0;
            }
            if ((UnityEngine.Object) Projectile.m_cachedDungeon != (UnityEngine.Object) null && !Projectile.m_cachedDungeon.data.CheckInBounds((int) position.x, (int) position.y))
            {
                this.m_outOfBoundsCounter += BraveTime.DeltaTime;
                if ((double) this.m_outOfBoundsCounter > 5.0)
                {
                    this.gameObject.SetActive(false);
                    SpawnManager.Despawn(this.gameObject);
                }
            }
            else
                this.m_outOfBoundsCounter = 0.0f;
            if (this.damageTypes != CoreDamageTypes.None)
                this.HandleGoopChecks();
            if (this.m_isExitClippingTiles && (double) this.m_distanceElapsed > (double) this.m_exitClippingDistance)
            {
                this.specRigidbody.OnPreTileCollision -= new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.PreTileCollisionExitClipping);
                this.m_isExitClippingTiles = false;
            }
            if (this.OnPostUpdate == null)
                return;
            this.OnPostUpdate(this);
        }

        protected virtual void DoModifyVelocity()
        {
            if (this.ModifyVelocity == null)
                return;
            this.specRigidbody.Velocity = this.ModifyVelocity(this.specRigidbody.Velocity);
            if (!(this.specRigidbody.Velocity != Vector2.zero))
                return;
            this.m_currentDirection = this.specRigidbody.Velocity.normalized;
        }

        protected void HandleGoopChecks()
        {
            IntVector2 intVector2 = this.specRigidbody.UnitCenter.ToIntVector2();
            if (!Projectile.m_cachedDungeon.data.CheckInBounds(intVector2))
                return;
            List<DeadlyDeadlyGoopManager> roomGoops = Projectile.m_cachedDungeon.data.GetAbsoluteRoomFromPosition(intVector2).RoomGoops;
            if (roomGoops == null)
                return;
            for (int index = 0; index < roomGoops.Count; ++index)
                roomGoops[index].ProcessProjectile(this);
        }

        public virtual void SetNewShooter(SpeculativeRigidbody newShooter)
        {
            if ((bool) (UnityEngine.Object) this.specRigidbody)
            {
                this.specRigidbody.DeregisterSpecificCollisionException(this.m_shooter);
                if (!this.allowSelfShooting)
                    this.specRigidbody.RegisterSpecificCollisionException(newShooter);
            }
            this.m_shooter = newShooter;
        }

        public void UpdateSpeed() => this.m_currentSpeed = this.baseData.speed;

        public void UpdateCollisionMask()
        {
            if (!(bool) (UnityEngine.Object) this.specRigidbody)
                return;
            if (this.collidesWithProjectiles)
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerCollidableOverride |= CollisionMask.LayerToMask(CollisionLayer.Projectile);
            }
            else
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerCollidableOverride &= ~CollisionMask.LayerToMask(CollisionLayer.Projectile);
            }
            if (!this.collidesWithEnemies)
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
            }
            else
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride &= ~CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox);
            }
            if (!this.collidesWithPlayer)
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox);
            }
            else
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride &= ~CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox);
            }
            if (this.Owner is PlayerController)
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker);
            }
            else
            {
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].CollisionLayerIgnoreOverride &= ~CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker);
            }
        }

        public void SendInDirection(Vector2 dirVec, bool resetDistance, bool updateRotation = true)
        {
            if (this.shouldRotate && updateRotation)
                this.m_transform.eulerAngles = new Vector3(0.0f, 0.0f, dirVec.ToAngle());
            this.m_currentDirection = dirVec.normalized;
            this.specRigidbody.Velocity = this.m_currentDirection * this.m_currentSpeed * this.LocalTimeScale;
            if (this.OverrideMotionModule != null)
                this.OverrideMotionModule.SentInDirection(this.baseData, this.m_transform, this.sprite, this.specRigidbody, ref this.m_timeElapsed, ref this.m_currentDirection, this.shouldRotate, dirVec, resetDistance, updateRotation);
            if (!resetDistance)
                return;
            this.ResetDistance();
        }

        public void ResetDistance() => this.m_distanceElapsed = 0.0f;

        public float GetElapsedDistance() => this.m_distanceElapsed;

        public void Reflected()
        {
            if (this.OnReflected == null)
                return;
            this.OnReflected(this);
        }

        [DebuggerHidden]
        public IEnumerator CheckIfBossKillShot()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Projectile__CheckIfBossKillShotc__Iterator0()
            {
                _this = this
            };
        }

        public void HandlePassthroughHitEffects(Vector3 point)
        {
            if (this.hitEffects == null)
                return;
            this.hitEffects.HandleEnemyImpact(point, 0.0f, (Transform) null, Vector2.zero, this.specRigidbody.Velocity, false);
        }

        public void Ramp(float startHeightOffset, float duration)
        {
            if (!(bool) (UnityEngine.Object) this.sprite)
                return;
            this.m_isRamping = true;
            this.m_rampDuration = duration;
            this.m_rampTimer = 0.0f;
            this.m_startRampHeight = startHeightOffset;
            float currentRampHeight = this.m_currentRampHeight;
            this.m_currentRampHeight = this.m_startRampHeight;
            this.sprite.HeightOffGround -= currentRampHeight - this.m_currentRampHeight;
            this.sprite.UpdateZDepthLater();
        }

        public virtual float EstimatedTimeToTarget(Vector2 targetPoint, Vector2? overridePos = null)
        {
            return Vector2.Distance(!overridePos.HasValue ? this.specRigidbody.UnitCenter : overridePos.Value, targetPoint) / this.Speed;
        }

        public virtual Vector2 GetPredictedTargetPosition(
            Vector2 targetCenter,
            Vector2 targetVelocity,
            Vector2? overridePos = null,
            float? overrideProjectileSpeed = null)
        {
            Vector2 aimOrigin = !overridePos.HasValue ? this.specRigidbody.UnitCenter : overridePos.Value;
            float firingSpeed = !overrideProjectileSpeed.HasValue ? this.baseData.speed : overrideProjectileSpeed.Value;
            return BraveMathCollege.GetPredictedPosition(targetCenter, targetVelocity, aimOrigin, firingSpeed);
        }

        public void RemoveBulletScriptControl()
        {
            if (!(bool) (UnityEngine.Object) this.braveBulletScript)
                return;
            if (this.braveBulletScript.bullet != null)
                this.braveBulletScript.bullet.DontDestroyGameObject = true;
            this.braveBulletScript.RemoveBullet();
            this.braveBulletScript.enabled = false;
            this.BulletScriptSettings.surviveRigidbodyCollisions = false;
            this.BulletScriptSettings.surviveTileCollisions = false;
            this.IsBulletScript = false;
        }

        public void IgnoreTileCollisionsFor(float time)
        {
            this.specRigidbody.CollideWithTileMap = false;
            this.m_ignoreTileCollisionsTimer = time;
        }

        public void DoWallExitClipping(float pixelMultiplier = 1f)
        {
            this.m_isExitClippingTiles = true;
            this.specRigidbody.OnPreTileCollision += new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.PreTileCollisionExitClipping);
            PixelCollider primaryPixelCollider = this.specRigidbody.PrimaryPixelCollider;
            this.m_exitClippingDistance = pixelMultiplier * Mathf.Max(primaryPixelCollider.UnitWidth, primaryPixelCollider.UnitHeight);
        }

        protected virtual void Move()
        {
            this.m_timeElapsed += this.LocalDeltaTime;
            if ((double) this.angularVelocity != 0.0)
                this.m_transform.RotateAround((Vector3) this.m_transform.position.XY(), Vector3.forward, this.angularVelocity * this.LocalDeltaTime);
            if (this.baseData.UsesCustomAccelerationCurve)
                this.m_currentSpeed = this.baseData.AccelerationCurve.Evaluate(Mathf.Clamp01((this.m_timeElapsed - this.baseData.IgnoreAccelCurveTime) / this.baseData.CustomAccelerationCurveDuration)) * this.baseData.speed;
            this.specRigidbody.Velocity = this.m_currentDirection * this.m_currentSpeed;
            this.m_currentSpeed *= (float) (1.0 - (double) this.baseData.damping * (double) this.LocalDeltaTime);
            this.LastVelocity = this.specRigidbody.Velocity;
        }

        protected virtual void HandleRange()
        {
            this.m_distanceElapsed += Vector3.Distance(this.m_lastPosition, this.m_transform.position);
            this.LastPosition = this.m_transform.position;
            if (this.SkipDistanceElapsedCheck || (double) this.m_distanceElapsed <= (double) this.baseData.range)
                return;
            this.DieInAir();
        }

        public void ForceDestruction() => this.HandleDestruction((CollisionData) null);

        protected virtual void HandleDestruction(
            CollisionData lcr,
            bool allowActorSpawns = true,
            bool allowProjectileSpawns = true)
        {
            this.HandleSparks(lcr == null ? new Vector2?() : new Vector2?(lcr.Contact));
            if (this.hitEffects != null && this.hitEffects.HasProjectileDeathVFX)
                this.hitEffects.HandleProjectileDeathVFX((Vector3) (lcr == null || this.hitEffects.CenterDeathVFXOnProjectile ? this.specRigidbody.UnitCenter : lcr.Contact), 0.0f, (Transform) null, lcr == null ? Vector2.zero : lcr.Normal, this.specRigidbody.Velocity);
            if ((bool) (UnityEngine.Object) this.braveBulletScript)
            {
                if (lcr == null)
                    this.braveBulletScript.HandleBulletDestruction(Bullet.DestroyType.DieInAir, (SpeculativeRigidbody) null, allowProjectileSpawns);
                else if ((bool) (UnityEngine.Object) lcr.OtherRigidbody)
                    this.braveBulletScript.HandleBulletDestruction(Bullet.DestroyType.HitRigidbody, lcr.OtherRigidbody, allowProjectileSpawns);
                else
                    this.braveBulletScript.HandleBulletDestruction(Bullet.DestroyType.HitTile, (SpeculativeRigidbody) null, allowProjectileSpawns);
            }
            if (allowProjectileSpawns && this.baseData.onDestroyBulletScript != null && !this.baseData.onDestroyBulletScript.IsNull)
            {
                if (lcr != null)
                {
                    Vector2 unitCenter = this.specRigidbody.UnitCenter;
                    if (!lcr.IsInverse)
                        unitCenter += PhysicsEngine.PixelToUnit(lcr.NewPixelsToMove);
                    SpawnManager.SpawnBulletScript(this.Owner, this.baseData.onDestroyBulletScript, new Vector2?(unitCenter + lcr.Normal.normalized * PhysicsEngine.PixelToUnit(2)), new Vector2?(lcr.Normal), this.collidesWithEnemies, this.OwnerName);
                }
                else
                    SpawnManager.SpawnBulletScript(this.Owner, this.baseData.onDestroyBulletScript, new Vector2?((Vector2) this.m_transform.position), new Vector2?(Vector2.up), this.collidesWithEnemies, this.OwnerName);
            }
            if (!string.IsNullOrEmpty(this.spawnEnemyGuidOnDeath) && allowActorSpawns)
            {
                IntVector2 intVector2 = this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
                RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(intVector2);
                AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.spawnEnemyGuidOnDeath), intVector2, roomFromPosition, true);
                if ((bool) (UnityEngine.Object) aiActor.specRigidbody)
                    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiActor.specRigidbody);
                if (this.IsBlackBullet && (bool) (UnityEngine.Object) aiActor)
                    aiActor.ForceBlackPhantom = true;
            }
            if (this.OnDestruction != null)
                this.OnDestruction(this);
            switch (this.DestroyMode)
            {
                case Projectile.ProjectileDestroyMode.Destroy:
                    if (!SpawnManager.Despawn(this.gameObject, this.m_spawnPool))
                    {
                        this.gameObject.SetActive(false);
                        break;
                    }
                    break;
                case Projectile.ProjectileDestroyMode.DestroyComponent:
                    this.specRigidbody.Velocity = Vector2.zero;
                    this.specRigidbody.DeregisterSpecificCollisionException(this.Shooter);
                    for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                        this.specRigidbody.PixelColliders[index].IsTrigger = true;
                    this.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
                    this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
                    this.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
                    if (this.m_isExitClippingTiles)
                        this.specRigidbody.OnPreTileCollision -= new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.PreTileCollisionExitClipping);
                    UnityEngine.Object.Destroy((UnityEngine.Object) this);
                    break;
                case Projectile.ProjectileDestroyMode.BecomeDebris:
                    this.specRigidbody.Velocity = Vector2.zero;
                    this.specRigidbody.DeregisterSpecificCollisionException(this.Shooter);
                    for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                        this.specRigidbody.PixelColliders[index].IsTrigger = true;
                    this.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
                    this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
                    this.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
                    if (this.m_isExitClippingTiles)
                        this.specRigidbody.OnPreTileCollision -= new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.PreTileCollisionExitClipping);
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.GetComponentInChildren<SimpleSpriteRotator>());
                    DebrisObject debrisObject = this.BecomeDebris(lcr != null ? lcr.Normal.ToVector3ZUp(0.1f) : Vector3.zero, 0.5f);
                    if (this.OnBecameDebris != null)
                        this.OnBecameDebris(debrisObject);
                    if (this.OnBecameDebrisGrounded != null)
                        debrisObject.OnGrounded += this.OnBecameDebrisGrounded;
                    UnityEngine.Object.Destroy((UnityEngine.Object) this);
                    break;
            }
            if (!GameManager.AUDIO_ENABLED || string.IsNullOrEmpty(this.onDestroyEventName))
                return;
            int num = (int) AkSoundEngine.PostEvent(this.onDestroyEventName, this.gameObject);
        }

        public DebrisObject BecomeDebris(Vector3 force, float height)
        {
            DebrisObject orAddComponent = this.gameObject.GetOrAddComponent<DebrisObject>();
            orAddComponent.angularVelocity = !this.shouldRotate ? 0.0f : 45f;
            orAddComponent.angularVelocityVariance = !this.shouldRotate ? 0.0f : 20f;
            orAddComponent.decayOnBounce = 0.5f;
            orAddComponent.bounceCount = 1;
            orAddComponent.canRotate = this.shouldRotate;
            orAddComponent.shouldUseSRBMotion = true;
            orAddComponent.AssignFinalWorldDepth(-0.5f);
            orAddComponent.sprite = this.specRigidbody.sprite;
            orAddComponent.animatePitFall = true;
            orAddComponent.Trigger(force, height);
            return orAddComponent;
        }

        public void DieInAir(
            bool suppressInAirEffects = false,
            bool allowActorSpawns = true,
            bool allowProjectileSpawns = true,
            bool killedEarly = false)
        {
            if (!this.gameObject.activeSelf || this.m_hasDiedInAir)
                return;
            this.m_hasDiedInAir = true;
            BeamController component1 = this.GetComponent<BeamController>();
            if ((bool) (UnityEngine.Object) component1)
                component1.DestroyBeam();
            SpawnProjModifier component2 = this.GetComponent<SpawnProjModifier>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && allowProjectileSpawns && (component2.spawnProjectilesOnCollision && component2.spawnOnObjectCollisions || component2.spawnProjecitlesOnDieInAir))
                component2.SpawnCollisionProjectiles(this.m_transform.position.XY(), this.specRigidbody.Velocity.normalized, (SpeculativeRigidbody) null);
            ExplosiveModifier component3 = this.GetComponent<ExplosiveModifier>();
            if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
                component3.Explode(Vector2.zero, this.ignoreDamageCaps);
            if (!suppressInAirEffects)
                this.HandleHitEffectsMidair(killedEarly);
            this.HandleDestruction((CollisionData) null, allowActorSpawns, allowProjectileSpawns);
        }

        public void ChangeColor(float time, Color color)
        {
            if (!(bool) (UnityEngine.Object) this.sprite)
                return;
            if (this.Owner is PlayerController && (bool) (UnityEngine.Object) this.sprite.renderer && this.sprite.renderer.material.HasProperty("_VertexColor"))
            {
                this.sprite.usesOverrideMaterial = true;
                this.sprite.renderer.material.SetFloat("_VertexColor", 1f);
            }
            if ((double) time == 0.0)
                this.sprite.color = color;
            else
                this.StartCoroutine(this.ChangeColorCR(time, color));
        }

        [DebuggerHidden]
        private IEnumerator ChangeColorCR(float time, Color color)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Projectile__ChangeColorCRc__Iterator1()
            {
                time = time,
                color = color,
                _this = this
            };
        }

        public void ChangeTintColorShader(float time, Color color)
        {
            if (!(bool) (UnityEngine.Object) this.sprite)
                return;
            this.sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
            Material material = this.sprite.renderer.material;
            bool flag = material.HasProperty("_EmissivePower");
            float num1 = 0.0f;
            float num2 = 0.0f;
            if (flag)
            {
                num1 = material.GetFloat("_EmissivePower");
                num2 = material.GetFloat("_EmissiveColorPower");
            }
            Shader shader = flag ? ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive") : ShaderCache.Acquire("tk2d/CutoutVertexColorTintableTilted");
            if ((UnityEngine.Object) this.sprite.renderer.material.shader != (UnityEngine.Object) shader)
            {
                this.sprite.renderer.material.shader = shader;
                this.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                if (flag)
                {
                    this.sprite.renderer.material.SetFloat("_EmissivePower", num1);
                    this.sprite.renderer.material.SetFloat("_EmissiveColorPower", num2);
                }
            }
            if ((double) time == 0.0)
                this.sprite.renderer.sharedMaterial.SetColor("_OverrideColor", color);
            else
                this.StartCoroutine(this.ChangeTintColorCR(time, color));
        }

        [DebuggerHidden]
        private IEnumerator ChangeTintColorCR(float time, Color color)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Projectile__ChangeTintColorCRc__Iterator2()
            {
                time = time,
                color = color,
                _this = this
            };
        }

        protected void HandleWallDecals(CollisionData lcr, Transform parent)
        {
            if ((double) lcr.Normal.y >= 0.0)
                return;
            VFXPool vfxPool1 = (VFXPool) null;
            if (this.wallDecals != null && this.wallDecals.effects.Length > 0)
            {
                for (int index1 = 0; index1 < this.wallDecals.effects.Length; ++index1)
                {
                    for (int index2 = 0; index2 < this.wallDecals.effects[index1].effects.Length; ++index2)
                    {
                        this.wallDecals.effects[index1].effects[index2].orphaned = false;
                        this.wallDecals.effects[index1].effects[index2].destructible = true;
                    }
                }
                vfxPool1 = this.wallDecals;
            }
            else
            {
                DamageTypeEffectDefinition definitionForType = GameManager.Instance.Dungeon.damageTypeEffectMatrix.GetDefinitionForType(this.damageTypes);
                if (definitionForType != null)
                    vfxPool1 = definitionForType.wallDecals;
            }
            if (vfxPool1 == null)
                return;
            float num = (float) ((double) UnityEngine.Random.value * 0.5 - 0.25);
            Vector3 vector3Zup = lcr.Contact.ToVector3ZUp(-0.5f);
            vector3Zup.y += num;
            VFXPool vfxPool2 = vfxPool1;
            Vector3 position = vector3Zup;
            Transform parent1 = parent;
            Vector2? sourceNormal = new Vector2?(lcr.Normal);
            Vector2? sourceVelocity = new Vector2?(this.specRigidbody.Velocity);
            float? heightOffGround = new float?(0.75f + num);
            // ISSUE: reference to a compiler-generated field
            if (Projectile._f__mg_cache0 == null)
            {
                // ISSUE: reference to a compiler-generated field
                Projectile._f__mg_cache0 = new VFXComplex.SpawnMethod(SpawnManager.SpawnDecal);
            }
            // ISSUE: reference to a compiler-generated field
            VFXComplex.SpawnMethod fMgCache0 = Projectile._f__mg_cache0;
            vfxPool2.SpawnAtPosition(position, parent: parent1, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, heightOffGround: heightOffGround, spawnMethod: fMgCache0);
        }

        protected virtual void OnPreTileCollision(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myPixelCollider,
            PhysicsEngine.Tile tile,
            PixelCollider otherPixelCollider)
        {
            if (!this.PenetratesInternalWalls)
                return;
            CellData cellData = GameManager.Instance.Dungeon.data[tile.Position];
            if (cellData != null && !cellData.isRoomInternal)
                return;
            if (!this.m_isInWall)
            {
                CollisionData lcr = CollisionData.Pool.Allocate();
                lcr.Normal = BraveUtility.GetMajorAxis(this.m_transform.position.XY() - tile.Position.ToCenterVector2()).normalized;
                lcr.Contact = tile.Position.ToCenterVector2() + lcr.Normal / 2f;
                this.HandleHitEffectsTileMap(lcr, false);
                CollisionData.Pool.Free(ref lcr);
            }
            this.m_isInWall = true;
            PhysicsEngine.SkipCollision = true;
        }

        private void PreTileCollisionExitClipping(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myPixelCollider,
            PhysicsEngine.Tile tile,
            PixelCollider tilePixelCollider)
        {
            if (!GameManager.HasInstance || (UnityEngine.Object) GameManager.Instance.Dungeon == (UnityEngine.Object) null)
                return;
            DungeonData data = GameManager.Instance.Dungeon.data;
            int x = tile.Position.x;
            int y = tile.Position.y;
            Vector2 velocity = myRigidbody.Velocity;
            if ((double) velocity.y > 0.0 && data.isFaceWallHigher(x, y) || (double) velocity.y < 0.0 && data.hasTopWall(x, y) || (double) velocity.x < 0.0 && data.isLeftSideWall(x, y) || (double) velocity.x > 0.0 && data.isRightSideWall(x, y))
                return;
            PhysicsEngine.SkipCollision = true;
        }

        protected virtual void OnPreCollision(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myCollider,
            SpeculativeRigidbody otherRigidbody,
            PixelCollider otherCollider)
        {
            if ((UnityEngine.Object) otherRigidbody == (UnityEngine.Object) this.m_shooter && !this.allowSelfShooting)
                PhysicsEngine.SkipCollision = true;
            else if ((UnityEngine.Object) otherRigidbody.gameActor != (UnityEngine.Object) null && otherRigidbody.gameActor is PlayerController && (!this.collidesWithPlayer || (otherRigidbody.gameActor as PlayerController).IsGhost || (otherRigidbody.gameActor as PlayerController).IsEthereal))
            {
                PhysicsEngine.SkipCollision = true;
            }
            else
            {
                if ((bool) (UnityEngine.Object) otherRigidbody.aiActor)
                {
                    if (this.Owner is PlayerController && !otherRigidbody.aiActor.IsNormalEnemy)
                    {
                        PhysicsEngine.SkipCollision = true;
                        return;
                    }
                    if (this.Owner is AIActor && !this.collidesWithEnemies && otherRigidbody.aiActor.IsNormalEnemy && !otherRigidbody.aiActor.HitByEnemyBullets)
                    {
                        PhysicsEngine.SkipCollision = true;
                        return;
                    }
                }
                if (!GameManager.PVP_ENABLED && this.Owner is PlayerController && (UnityEngine.Object) otherRigidbody.GetComponent<PlayerController>() != (UnityEngine.Object) null && !this.allowSelfShooting)
                {
                    PhysicsEngine.SkipCollision = true;
                }
                else
                {
                    if (GameManager.Instance.InTutorial)
                    {
                        PlayerController component = otherRigidbody.GetComponent<PlayerController>();
                        if ((bool) (UnityEngine.Object) component)
                        {
                            if (component.spriteAnimator.QueryInvulnerabilityFrame())
                                GameManager.BroadcastRoomTalkDoerFsmEvent("playerDodgedBullet");
                            else if (component.IsDodgeRolling)
                                GameManager.BroadcastRoomTalkDoerFsmEvent("playerAlmostDodgedBullet");
                            else
                                GameManager.BroadcastRoomTalkDoerFsmEvent("playerDidNotDodgeBullet");
                        }
                    }
                    if ((UnityEngine.Object) otherRigidbody.healthHaver != (UnityEngine.Object) null && (UnityEngine.Object) otherRigidbody.healthHaver.spriteAnimator != (UnityEngine.Object) null && otherCollider.CollisionLayer == CollisionLayer.PlayerHitBox && otherRigidbody.spriteAnimator.QueryInvulnerabilityFrame())
                    {
                        PhysicsEngine.SkipCollision = true;
                        this.StartCoroutine(this.HandlePostInvulnerabilityFrameExceptions(otherRigidbody));
                    }
                    else
                    {
                        if (!this.collidesWithProjectiles || !this.collidesOnlyWithPlayerProjectiles || !(bool) (UnityEngine.Object) otherRigidbody.projectile || otherRigidbody.projectile.Owner is PlayerController)
                            return;
                        PhysicsEngine.SkipCollision = true;
                    }
                }
            }
        }

        public void ForceCollision(SpeculativeRigidbody otherRigidbody, LinearCastResult lcr)
        {
            CollisionData rigidbodyCollision = CollisionData.Pool.Allocate();
            rigidbodyCollision.SetAll(lcr);
            rigidbodyCollision.OtherRigidbody = otherRigidbody;
            rigidbodyCollision.OtherPixelCollider = otherRigidbody.PrimaryPixelCollider;
            rigidbodyCollision.MyRigidbody = this.specRigidbody;
            rigidbodyCollision.MyPixelCollider = this.specRigidbody.PrimaryPixelCollider;
            rigidbodyCollision.Normal = (this.specRigidbody.UnitCenter - otherRigidbody.UnitCenter).normalized;
            rigidbodyCollision.Contact = (otherRigidbody.UnitCenter + this.specRigidbody.UnitCenter) / 2f;
            this.OnRigidbodyCollision(rigidbodyCollision);
            CollisionData.Pool.Free(ref rigidbodyCollision);
        }

        protected virtual void OnRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            if (this.specRigidbody.IsGhostCollisionException(rigidbodyCollision.OtherRigidbody))
                return;
            GameObject gameObject = rigidbodyCollision.OtherRigidbody.gameObject;
            SpeculativeRigidbody otherRigidbody = rigidbodyCollision.OtherRigidbody;
            PlayerController component1 = otherRigidbody.GetComponent<PlayerController>();
            bool killedTarget;
            Projectile.HandleDamageResult handleDamageResult = this.HandleDamage(rigidbodyCollision.OtherRigidbody, rigidbodyCollision.OtherPixelCollider, out killedTarget, component1);
            bool flag1 = handleDamageResult != Projectile.HandleDamageResult.NO_HEALTH;
            if ((bool) (UnityEngine.Object) this.braveBulletScript && this.braveBulletScript.bullet != null && this.BulletScriptSettings.surviveTileCollisions && !flag1 && rigidbodyCollision.OtherPixelCollider.CollisionLayer == CollisionLayer.HighObstacle)
            {
                if ((bool) (UnityEngine.Object) otherRigidbody.minorBreakable)
                    return;
                this.braveBulletScript.bullet.ManualControl = true;
                this.braveBulletScript.bullet.Position = this.specRigidbody.UnitCenter;
                PhysicsEngine.PostSliceVelocity = new Vector2?(Vector2.zero);
            }
            else
            {
                this.HandleSparks(new Vector2?(rigidbodyCollision.Contact));
                if (flag1)
                {
                    this.m_hasImpactedEnemy = true;
                    if (this.OnHitEnemy != null)
                        this.OnHitEnemy(this, rigidbodyCollision.OtherRigidbody, killedTarget);
                }
                else if (ChallengeManager.CHALLENGE_MODE_ACTIVE && ((bool) (UnityEngine.Object) otherRigidbody.GetComponent<BeholsterBounceRocket>() || (bool) (UnityEngine.Object) otherRigidbody.healthHaver || (bool) (UnityEngine.Object) otherRigidbody.GetComponent<BashelliskBodyPickupController>() || (bool) (UnityEngine.Object) otherRigidbody.projectile))
                    this.m_hasImpactedEnemy = true;
                PierceProjModifier pierceProjModifier1 = this.GetComponent<PierceProjModifier>();
                BounceProjModifier bounceProjModifier = this.GetComponent<BounceProjModifier>();
                if (this.m_hasImpactedEnemy && (bool) (UnityEngine.Object) pierceProjModifier1 && (bool) (UnityEngine.Object) otherRigidbody.healthHaver && otherRigidbody.healthHaver.IsBoss && pierceProjModifier1.HandleBossImpact())
                {
                    bounceProjModifier = (BounceProjModifier) null;
                    pierceProjModifier1 = (PierceProjModifier) null;
                }
                if ((bool) (UnityEngine.Object) this.GetComponent<KeyProjModifier>())
                {
                    Chest component2 = otherRigidbody.GetComponent<Chest>();
                    if ((bool) (UnityEngine.Object) component2 && component2.IsLocked && component2.ChestIdentifier != Chest.SpecialChestIdentifier.RAT)
                        component2.ForceUnlock();
                }
                MinorBreakable minorBreakable = otherRigidbody.minorBreakable;
                MajorBreakable majorBreakable = otherRigidbody.majorBreakable;
                if ((UnityEngine.Object) majorBreakable != (UnityEngine.Object) null)
                {
                    float num1 = 1f;
                    if (((UnityEngine.Object) this.m_shooter != (UnityEngine.Object) null && (UnityEngine.Object) this.m_shooter.aiActor != (UnityEngine.Object) null || this.m_owner is AIActor) && majorBreakable.InvulnerableToEnemyBullets)
                        num1 = 0.0f;
                    if ((UnityEngine.Object) pierceProjModifier1 != (UnityEngine.Object) null && pierceProjModifier1.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
                    {
                        if (majorBreakable.ImmuneToBeastMode)
                            ++num1;
                        else
                            num1 = 1000f;
                    }
                    if (!majorBreakable.IsSecretDoor || !((UnityEngine.Object) this.PossibleSourceGun != (UnityEngine.Object) null) || !this.PossibleSourceGun.InfiniteAmmo)
                    {
                        float num2 = !(this.Owner is AIActor) ? this.ModifiedDamage : ProjectileData.FixedEnemyDamageToBreakables;
                        if ((double) num2 <= 0.0 && GameManager.Instance.InTutorial)
                            majorBreakable.ApplyDamage(1.5f, this.specRigidbody.Velocity, false);
                        else
                            majorBreakable.ApplyDamage(num2 * num1, this.specRigidbody.Velocity, this.Owner is AIActor);
                    }
                }
                if (rigidbodyCollision.OtherRigidbody.PreventPiercing)
                    pierceProjModifier1 = (PierceProjModifier) null;
                if (!flag1 && (bool) (UnityEngine.Object) bounceProjModifier && !(bool) (UnityEngine.Object) minorBreakable && (!bounceProjModifier.onlyBounceOffTiles || !(bool) (UnityEngine.Object) majorBreakable) && !(bool) (UnityEngine.Object) pierceProjModifier1 && (!bounceProjModifier.useLayerLimit || rigidbodyCollision.OtherPixelCollider.CollisionLayer == bounceProjModifier.layerLimit))
                {
                    this.OnTileCollision(rigidbodyCollision);
                }
                else
                {
                    bool flag2 = (bool) (UnityEngine.Object) majorBreakable && majorBreakable.IsSecretDoor;
                    if (!(bool) (UnityEngine.Object) majorBreakable && otherRigidbody.name.StartsWith("secret exit collider"))
                        flag2 = true;
                    if (flag2)
                        this.OnTileCollision(rigidbodyCollision);
                    else if (otherRigidbody.ReflectProjectiles)
                    {
                        int num3 = (int) AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", GameManager.Instance.gameObject);
                        if (this.IsBulletScript && (bool) (UnityEngine.Object) bounceProjModifier && bounceProjModifier.removeBulletScriptControl)
                            this.RemoveBulletScriptControl();
                        Vector2 vector = rigidbodyCollision.Normal;
                        if (otherRigidbody.ReflectProjectilesNormalGenerator != null)
                            vector = otherRigidbody.ReflectProjectilesNormalGenerator(rigidbodyCollision.Contact, rigidbodyCollision.Normal);
                        float angle1 = (-rigidbodyCollision.MyRigidbody.Velocity).ToAngle();
                        float angle2 = vector.ToAngle();
                        float num4 = BraveMathCollege.ClampAngle360(angle1 + (float) (2.0 * ((double) angle2 - (double) angle1)));
                        if (this.shouldRotate)
                            this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, num4);
                        this.m_currentDirection = BraveMathCollege.DegreesToVector(num4);
                        if ((bool) (UnityEngine.Object) this.braveBulletScript && this.braveBulletScript.bullet != null)
                            this.braveBulletScript.bullet.Direction = num4;
                        if (!(bool) (UnityEngine.Object) bounceProjModifier || !bounceProjModifier.suppressHitEffectsOnBounce)
                            this.HandleHitEffectsEnemy(rigidbodyCollision.OtherRigidbody, rigidbodyCollision, false);
                        PhysicsEngine.PostSliceVelocity = new Vector2?(this.m_currentDirection * this.m_currentSpeed * this.LocalTimeScale);
                        if ((bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody)
                        {
                            this.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0.0f, new float?(0.5f));
                            rigidbodyCollision.OtherRigidbody.RegisterTemporaryCollisionException(this.specRigidbody, 0.0f, new float?(0.5f));
                        }
                        if (!(bool) (UnityEngine.Object) otherRigidbody.knockbackDoer || !otherRigidbody.knockbackDoer.knockbackWhileReflecting)
                            return;
                        this.HandleKnockback(otherRigidbody, component1);
                    }
                    else
                    {
                        bool flag3 = false;
                        bool flag4 = false;
                        if (flag1)
                        {
                            if (!killedTarget || !((UnityEngine.Object) component1 != (UnityEngine.Object) null))
                                flag3 = true;
                            if (GameManager.AUDIO_ENABLED && !string.IsNullOrEmpty(this.enemyImpactEventName))
                            {
                                int num = (int) AkSoundEngine.PostEvent($"Play_WPN_{this.enemyImpactEventName}_impact_01", this.gameObject);
                            }
                        }
                        else
                        {
                            flag4 = true;
                            if (GameManager.AUDIO_ENABLED && !string.IsNullOrEmpty(this.objectImpactEventName))
                            {
                                int num = (int) AkSoundEngine.PostEvent($"Play_WPN_{this.objectImpactEventName}_impact_01", this.gameObject);
                            }
                        }
                        if (!Projectile.s_delayPlayerDamage || !(bool) (UnityEngine.Object) component1)
                        {
                            if (flag1)
                            {
                                if (!rigidbodyCollision.OtherRigidbody.healthHaver.IsDead || killedTarget)
                                    this.HandleKnockback(rigidbodyCollision.OtherRigidbody, component1, killedTarget);
                            }
                            else
                                this.HandleKnockback(rigidbodyCollision.OtherRigidbody, component1);
                        }
                        if (!(bool) (UnityEngine.Object) component1)
                        {
                            foreach (AppliedEffectBase component3 in this.GetComponents<AppliedEffectBase>())
                                component3.AddSelfToTarget(gameObject);
                        }
                        this.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, maxTime: new float?(0.5f));
                        PhysicsEngine.CollisionHaltsVelocity = new bool?(false);
                        Projectile projectile = rigidbodyCollision.OtherRigidbody.projectile;
                        if (this.CanTransmogrify && flag1 && handleDamageResult != Projectile.HandleDamageResult.HEALTH_AND_KILLED && (double) UnityEngine.Random.value < (double) this.ChanceToTransmogrify && (bool) (UnityEngine.Object) otherRigidbody.aiActor && !otherRigidbody.aiActor.IsMimicEnemy && (bool) (UnityEngine.Object) otherRigidbody.aiActor.healthHaver && !otherRigidbody.aiActor.healthHaver.IsBoss && otherRigidbody.aiActor.healthHaver.IsVulnerable)
                            otherRigidbody.aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid(this.TransmogrifyTargetGuids[UnityEngine.Random.Range(0, this.TransmogrifyTargetGuids.Length)]), (GameObject) ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
                        if ((UnityEngine.Object) pierceProjModifier1 != (UnityEngine.Object) null && pierceProjModifier1.preventPenetrationOfActors && flag1)
                            pierceProjModifier1 = (PierceProjModifier) null;
                        bool flag5 = false;
                        bool flag6 = false;
                        bool flag7 = (bool) (UnityEngine.Object) otherRigidbody && (bool) (UnityEngine.Object) otherRigidbody.GetComponent<PlayerOrbital>();
                        if (this.BulletScriptSettings.surviveRigidbodyCollisions)
                        {
                            flag5 = true;
                            flag6 = true;
                        }
                        else if ((UnityEngine.Object) pierceProjModifier1 != (UnityEngine.Object) null && pierceProjModifier1.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
                        {
                            flag5 = true;
                            flag6 = true;
                        }
                        else if ((UnityEngine.Object) pierceProjModifier1 != (UnityEngine.Object) null && pierceProjModifier1.penetration > 0 && flag1)
                        {
                            --pierceProjModifier1.penetration;
                            flag5 = true;
                            flag6 = true;
                        }
                        else if ((UnityEngine.Object) pierceProjModifier1 != (UnityEngine.Object) null && pierceProjModifier1.penetratesBreakables && pierceProjModifier1.penetration > 0)
                        {
                            --pierceProjModifier1.penetration;
                            flag5 = true;
                            flag6 = true;
                        }
                        else if ((bool) (UnityEngine.Object) projectile && this.projectileHitHealth > 0)
                        {
                            PierceProjModifier component4 = projectile.GetComponent<PierceProjModifier>();
                            if ((bool) (UnityEngine.Object) component4 && component4.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE || projectile is RobotechProjectile)
                            {
                                this.projectileHitHealth -= 2;
                                projectile.m_hasImpactedEnemy = true;
                            }
                            else
                            {
                                --this.projectileHitHealth;
                                projectile.m_hasImpactedEnemy = true;
                            }
                            flag5 = this.projectileHitHealth >= 0;
                            flag6 = flag5;
                        }
                        else if ((bool) (UnityEngine.Object) minorBreakable && this.pierceMinorBreakables)
                        {
                            flag5 = true;
                            flag6 = true;
                        }
                        else if ((UnityEngine.Object) bounceProjModifier != (UnityEngine.Object) null && !flag1 && !this.m_hasImpactedEnemy)
                        {
                            bounceProjModifier.HandleChanceToDie();
                            if (flag1 && bounceProjModifier.ExplodeOnEnemyBounce && (bool) (UnityEngine.Object) this.GetComponent<ExplosiveModifier>())
                                bounceProjModifier.numberOfBounces = 0;
                            int num5 = 1;
                            PierceProjModifier pierceProjModifier2 = (PierceProjModifier) null;
                            if ((bool) (UnityEngine.Object) otherRigidbody && (bool) (UnityEngine.Object) otherRigidbody.projectile)
                                pierceProjModifier2 = otherRigidbody.GetComponent<PierceProjModifier>();
                            if ((bool) (UnityEngine.Object) pierceProjModifier2 && pierceProjModifier2.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
                                num5 = 2;
                            if (bounceProjModifier.numberOfBounces - num5 >= 0 & (!bounceProjModifier.useLayerLimit ? 0 : (rigidbodyCollision.OtherPixelCollider.CollisionLayer != bounceProjModifier.layerLimit ? 1 : 0)) == 0 & !flag7)
                            {
                                if (this.IsBulletScript && bounceProjModifier.removeBulletScriptControl)
                                    this.RemoveBulletScriptControl();
                                Vector2 normal = rigidbodyCollision.Normal;
                                if ((bool) (UnityEngine.Object) rigidbodyCollision.MyRigidbody)
                                {
                                    Vector2 velocity = rigidbodyCollision.MyRigidbody.Velocity;
                                    float angle3 = (-velocity).ToAngle();
                                    float angle4 = normal.ToAngle();
                                    float num6 = BraveMathCollege.ClampAngle360(angle3 + (float) (2.0 * ((double) angle4 - (double) angle3)));
                                    if (this.shouldRotate)
                                        this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, num6);
                                    this.m_currentDirection = BraveMathCollege.DegreesToVector(num6);
                                    this.m_currentSpeed *= 1f - bounceProjModifier.percentVelocityToLoseOnBounce;
                                    if ((bool) (UnityEngine.Object) this.braveBulletScript && this.braveBulletScript.bullet != null)
                                    {
                                        this.braveBulletScript.bullet.Direction = num6;
                                        this.braveBulletScript.bullet.Speed *= 1f - bounceProjModifier.percentVelocityToLoseOnBounce;
                                    }
                                    Vector2 inVec = this.m_currentDirection * this.m_currentSpeed * this.LocalTimeScale;
                                    Vector2 vector2 = bounceProjModifier.AdjustBounceVector(this, inVec, otherRigidbody);
                                    if (this.shouldRotate && vector2.normalized != this.m_currentDirection)
                                        this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(vector2.normalized));
                                    this.m_currentDirection = vector2.normalized;
                                    if (this is HelixProjectile)
                                        (this as HelixProjectile).AdjustRightVector(Mathf.DeltaAngle(velocity.ToAngle(), num6));
                                    if (this.OverrideMotionModule != null)
                                        this.OverrideMotionModule.UpdateDataOnBounce(Mathf.DeltaAngle(velocity.ToAngle(), num6));
                                    bounceProjModifier.Bounce(this, rigidbodyCollision.Contact, otherRigidbody);
                                    PhysicsEngine.PostSliceVelocity = new Vector2?(vector2);
                                    if ((bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody)
                                    {
                                        this.specRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0.0f, new float?(0.5f));
                                        rigidbodyCollision.OtherRigidbody.RegisterTemporaryCollisionException(this.specRigidbody, 0.0f, new float?(0.5f));
                                    }
                                    flag5 = true;
                                }
                            }
                        }
                        if (flag3)
                            this.HandleHitEffectsEnemy(rigidbodyCollision.OtherRigidbody, rigidbodyCollision, !flag6 && !flag5);
                        if (flag4)
                            this.HandleHitEffectsObject(rigidbodyCollision.OtherRigidbody, rigidbodyCollision, !flag6 && !flag5);
                        this.m_hasPierced |= flag6;
                        if (flag6 || flag5 || this.m_hasImpactedObject)
                            return;
                        this.m_hasImpactedObject = true;
                        for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                            this.specRigidbody.PixelColliders[index].IsTrigger = true;
                        if (flag1 && this.gameObject.activeInHierarchy)
                        {
                            this.StartCoroutine(this.HandlePostCollisionPersistence(rigidbodyCollision, component1));
                        }
                        else
                        {
                            this.HandleNormalProjectileDeath(rigidbodyCollision, !flag7);
                            PhysicsEngine.HaltRemainingMovement = true;
                        }
                    }
                }
            }
        }

        protected virtual void OnTileCollision(CollisionData tileCollision)
        {
            if ((!this.damagesWalls || this.SuppressHitEffects) && (bool) (UnityEngine.Object) this.specRigidbody && ((double) this.specRigidbody.UnitWidth > 1.0 || (double) this.specRigidbody.UnitHeight > 1.0))
            {
                this.damagesWalls = true;
                this.SuppressHitEffects = false;
            }
            BounceProjModifier component1 = this.GetComponent<BounceProjModifier>();
            SpawnProjModifier component2 = this.GetComponent<SpawnProjModifier>();
            ExplosiveModifier component3 = this.GetComponent<ExplosiveModifier>();
            GoopModifier component4 = this.GetComponent<GoopModifier>();
            if (GameManager.AUDIO_ENABLED && !string.IsNullOrEmpty(this.objectImpactEventName))
            {
                int num1 = (int) AkSoundEngine.PostEvent($"Play_WPN_{this.objectImpactEventName}_impact_01", this.gameObject);
            }
            this.HandleSparks(new Vector2?(tileCollision.Contact));
            if (this.BulletScriptSettings.surviveTileCollisions)
            {
                PhysicsEngine.PostSliceVelocity = new Vector2?(Vector2.zero);
            }
            else
            {
                if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
                    component1.HandleChanceToDie();
                if (this.damagesWalls)
                    this.HandleWallDecals(tileCollision, (Transform) null);
                bool flag1 = (bool) (UnityEngine.Object) tileCollision.OtherRigidbody && (bool) (UnityEngine.Object) tileCollision.OtherRigidbody.GetComponent<PlayerOrbital>();
                int num2 = 1;
                PierceProjModifier pierceProjModifier = (PierceProjModifier) null;
                if ((bool) (UnityEngine.Object) tileCollision.OtherRigidbody && (bool) (UnityEngine.Object) tileCollision.OtherRigidbody.projectile)
                    pierceProjModifier = tileCollision.OtherRigidbody.GetComponent<PierceProjModifier>();
                if ((bool) (UnityEngine.Object) pierceProjModifier && pierceProjModifier.BeastModeLevel == PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE)
                    num2 = 2;
                bool flag2 = (UnityEngine.Object) component1 != (UnityEngine.Object) null && component1.numberOfBounces - num2 >= 0;
                if (flag2)
                    flag2 = flag2 & (!component1.useLayerLimit ? 0 : (tileCollision.OtherPixelCollider.CollisionLayer != component1.layerLimit ? 1 : 0)) == 0 & !flag1;
                if (flag2)
                {
                    if (this.IsBulletScript && component1.removeBulletScriptControl)
                        this.RemoveBulletScriptControl();
                    Vector2 vector = tileCollision.Normal;
                    if ((bool) (UnityEngine.Object) tileCollision.OtherRigidbody && tileCollision.OtherRigidbody.ReflectProjectilesNormalGenerator != null)
                        vector = tileCollision.OtherRigidbody.ReflectProjectilesNormalGenerator(tileCollision.Contact, vector);
                    if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && component2.spawnProjectilesOnCollision && component2.spawnCollisionProjectilesOnBounce)
                        component2.SpawnCollisionProjectiles(tileCollision.PostCollisionUnitCenter, tileCollision.Normal, (SpeculativeRigidbody) null);
                    if (!(bool) (UnityEngine.Object) tileCollision.MyRigidbody)
                        return;
                    Vector2 velocity = tileCollision.MyRigidbody.Velocity;
                    float angle1 = (-velocity).ToAngle();
                    float angle2 = vector.ToAngle();
                    float num3 = BraveMathCollege.ClampAngle360(angle1 + (float) (2.0 * ((double) angle2 - (double) angle1)));
                    if (this.shouldRotate)
                        this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, num3);
                    this.m_currentDirection = BraveMathCollege.DegreesToVector(num3);
                    this.m_currentSpeed *= 1f - component1.percentVelocityToLoseOnBounce;
                    if ((bool) (UnityEngine.Object) this.braveBulletScript && this.braveBulletScript.bullet != null)
                    {
                        this.braveBulletScript.bullet.Direction = num3;
                        this.braveBulletScript.bullet.Speed *= 1f - component1.percentVelocityToLoseOnBounce;
                    }
                    if (!component1.suppressHitEffectsOnBounce)
                        this.HandleHitEffectsTileMap(tileCollision, false);
                    Vector2 inVec = this.m_currentDirection * this.m_currentSpeed * this.LocalTimeScale;
                    Vector2 vector2 = component1.AdjustBounceVector(this, inVec, (SpeculativeRigidbody) null);
                    if (this.shouldRotate && vector2.normalized != this.m_currentDirection)
                        this.m_transform.rotation = Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(vector2.normalized));
                    this.m_currentDirection = vector2.normalized;
                    if (this is HelixProjectile)
                        (this as HelixProjectile).AdjustRightVector(Mathf.DeltaAngle(velocity.ToAngle(), num3));
                    if (this.OverrideMotionModule != null)
                        this.OverrideMotionModule.UpdateDataOnBounce(Mathf.DeltaAngle(velocity.ToAngle(), num3));
                    component1.Bounce(this, tileCollision.Contact, tileCollision.OtherRigidbody);
                    PhysicsEngine.PostSliceVelocity = new Vector2?(vector2);
                }
                else
                {
                    if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && component2.spawnProjectilesOnCollision)
                        component2.SpawnCollisionProjectiles(tileCollision.PostCollisionUnitCenter, tileCollision.Normal, (SpeculativeRigidbody) null);
                    if ((UnityEngine.Object) component4 != (UnityEngine.Object) null)
                        component4.SpawnCollisionGoop(tileCollision);
                    if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
                        component3.Explode(tileCollision.Normal, this.ignoreDamageCaps);
                    if (!this.SuppressHitEffects)
                        this.HandleHitEffectsTileMap(tileCollision, true);
                    if (GlobalDungeonData.GUNGEON_EXPERIMENTAL)
                    {
                        Vector2 vector2 = tileCollision.Contact + (-1f * tileCollision.Normal).normalized * 0.5f;
                        IntVector2 intVector2 = new IntVector2(Mathf.FloorToInt(vector2.x), Mathf.FloorToInt(vector2.y));
                        GameManager.Instance.Dungeon.DestroyWallAtPosition(intVector2.x, intVector2.y);
                    }
                    this.HandleDestruction(tileCollision, allowProjectileSpawns: !flag1);
                    PhysicsEngine.HaltRemainingMovement = true;
                }
            }
        }

        public void BeamCollision(Projectile currentProjectile)
        {
            if (!this.collidesWithProjectiles)
                return;
            this.DieInAir();
        }

        [DebuggerHidden]
        private IEnumerator HandlePostInvulnerabilityFrameExceptions(SpeculativeRigidbody otherRigidbody)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Projectile__HandlePostInvulnerabilityFrameExceptionsc__Iterator3()
            {
                otherRigidbody = otherRigidbody,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandlePostCollisionPersistence(CollisionData lcr, PlayerController player)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Projectile__HandlePostCollisionPersistencec__Iterator4()
            {
                lcr = lcr,
                player = player,
                _this = this
            };
        }

        private Vector2 SafeCenter
        {
            get
            {
                if ((bool) (UnityEngine.Object) this.specRigidbody)
                    return this.specRigidbody.UnitCenter;
                return (bool) (UnityEngine.Object) this.m_transform ? this.m_transform.position.XY() : this.LastPosition.XY();
            }
        }

        private void HandleNormalProjectileDeath(CollisionData lcr, bool allowProjectileSpawns = true)
        {
            SpawnProjModifier component1 = this.GetComponent<SpawnProjModifier>();
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && allowProjectileSpawns && component1.spawnProjectilesOnCollision && component1.spawnOnObjectCollisions)
            {
                Vector2 contact = this.SafeCenter;
                if (lcr != null && (bool) (UnityEngine.Object) lcr.MyRigidbody)
                    contact = lcr.PostCollisionUnitCenter;
                component1.SpawnCollisionProjectiles(contact, lcr.Normal, lcr.OtherRigidbody, true);
            }
            GoopModifier component2 = this.GetComponent<GoopModifier>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
            {
                if (lcr == null)
                    component2.SpawnCollisionGoop(this.SafeCenter);
                else
                    component2.SpawnCollisionGoop(lcr);
            }
            ExplosiveModifier component3 = this.GetComponent<ExplosiveModifier>();
            if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
                component3.Explode(Vector2.zero, this.ignoreDamageCaps, lcr);
            this.HandleDestruction(lcr, allowProjectileSpawns: allowProjectileSpawns);
        }

        protected virtual Projectile.HandleDamageResult HandleDamage(
            SpeculativeRigidbody rigidbody,
            PixelCollider hitPixelCollider,
            out bool killedTarget,
            PlayerController player,
            bool alreadyPlayerDelayed = false)
        {
            killedTarget = false;
            if (rigidbody.ReflectProjectiles || !(bool) (UnityEngine.Object) rigidbody.healthHaver)
                return Projectile.HandleDamageResult.NO_HEALTH;
            if (!alreadyPlayerDelayed && Projectile.s_delayPlayerDamage && (bool) (UnityEngine.Object) player || (UnityEngine.Object) rigidbody.spriteAnimator != (UnityEngine.Object) null && rigidbody.spriteAnimator.QueryInvulnerabilityFrame())
                return Projectile.HandleDamageResult.HEALTH;
            bool flag1 = !rigidbody.healthHaver.IsDead;
            float damage1 = this.ModifiedDamage;
            if (this.Owner is AIActor && (bool) (UnityEngine.Object) rigidbody && (bool) (UnityEngine.Object) rigidbody.aiActor && (this.Owner as AIActor).IsNormalEnemy)
            {
                damage1 = ProjectileData.FixedFallbackDamageToEnemies;
                if (rigidbody.aiActor.HitByEnemyBullets)
                    damage1 /= 4f;
            }
            if (this.Owner is PlayerController && this.m_hasPierced && this.m_healthHaverHitCount >= 1)
            {
                int index = Mathf.Clamp(this.m_healthHaverHitCount - 1, 0, GameManager.Instance.PierceDamageScaling.Length - 1);
                damage1 *= GameManager.Instance.PierceDamageScaling[index];
            }
            if (this.OnWillKillEnemy != null && (double) damage1 >= (double) rigidbody.healthHaver.GetCurrentHealth())
                this.OnWillKillEnemy(this, rigidbody);
            if (rigidbody.healthHaver.IsBoss)
                damage1 *= this.BossDamageMultiplier;
            if ((double) this.BlackPhantomDamageMultiplier != 1.0 && (bool) (UnityEngine.Object) rigidbody.aiActor && rigidbody.aiActor.IsBlackPhantom)
                damage1 *= this.BlackPhantomDamageMultiplier;
            bool flag2 = false;
            if (this.DelayedDamageToExploders)
                flag2 = (bool) (UnityEngine.Object) rigidbody.GetComponent<ExplodeOnDeath>() && (double) rigidbody.healthHaver.GetCurrentHealth() <= (double) damage1;
            if (!flag2)
            {
                HealthHaver healthHaver = rigidbody.healthHaver;
                float num1 = damage1;
                Vector2 velocity = this.specRigidbody.Velocity;
                string ownerName = this.OwnerName;
                CoreDamageTypes damageTypes1 = this.damageTypes;
                DamageCategory damageCategory = !this.IsBlackBullet ? DamageCategory.Normal : DamageCategory.BlackBullet;
                PixelCollider pixelCollider = hitPixelCollider;
                double damage2 = (double) num1;
                Vector2 direction = velocity;
                string sourceName = ownerName;
                int damageTypes2 = (int) damageTypes1;
                int num2 = (int) damageCategory;
                PixelCollider hitPixelCollider1 = pixelCollider;
                int num3 = this.ignoreDamageCaps ? 1 : 0;
                healthHaver.ApplyDamage((float) damage2, direction, sourceName, (CoreDamageTypes) damageTypes2, (DamageCategory) num2, hitPixelCollider: hitPixelCollider1, ignoreDamageCaps: num3 != 0);
                if ((bool) (UnityEngine.Object) player && player.OnHitByProjectile != null)
                    player.OnHitByProjectile(this, player);
            }
            else
                rigidbody.StartCoroutine(this.HandleDelayedDamage(rigidbody, damage1, this.specRigidbody.Velocity, hitPixelCollider));
            if ((bool) (UnityEngine.Object) this.Owner && this.Owner is AIActor && (bool) (UnityEngine.Object) player)
                (this.Owner as AIActor).HasDamagedPlayer = true;
            killedTarget = flag1 && rigidbody.healthHaver.IsDead;
            if (!killedTarget && (UnityEngine.Object) rigidbody.gameActor != (UnityEngine.Object) null)
            {
                if (this.AppliesPoison && (double) UnityEngine.Random.value < (double) this.PoisonApplyChance)
                    rigidbody.gameActor.ApplyEffect((GameActorEffect) this.healthEffect);
                if (this.AppliesSpeedModifier && (double) UnityEngine.Random.value < (double) this.SpeedApplyChance)
                    rigidbody.gameActor.ApplyEffect((GameActorEffect) this.speedEffect);
                if (this.AppliesCharm && (double) UnityEngine.Random.value < (double) this.CharmApplyChance)
                    rigidbody.gameActor.ApplyEffect((GameActorEffect) this.charmEffect);
                if (this.AppliesFreeze && (double) UnityEngine.Random.value < (double) this.FreezeApplyChance)
                    rigidbody.gameActor.ApplyEffect((GameActorEffect) this.freezeEffect);
                if (this.AppliesCheese && (double) UnityEngine.Random.value < (double) this.CheeseApplyChance)
                    rigidbody.gameActor.ApplyEffect((GameActorEffect) this.cheeseEffect);
                if (this.AppliesBleed && (double) UnityEngine.Random.value < (double) this.BleedApplyChance)
                    rigidbody.gameActor.ApplyEffect((GameActorEffect) this.bleedEffect, -1f, this);
                if (this.AppliesFire && (double) UnityEngine.Random.value < (double) this.FireApplyChance)
                    rigidbody.gameActor.ApplyEffect((GameActorEffect) this.fireEffect);
                if (this.AppliesStun && (double) UnityEngine.Random.value < (double) this.StunApplyChance && (bool) (UnityEngine.Object) rigidbody.gameActor.behaviorSpeculator)
                    rigidbody.gameActor.behaviorSpeculator.Stun(this.AppliedStunDuration);
                for (int index = 0; index < this.statusEffectsToApply.Count; ++index)
                    rigidbody.gameActor.ApplyEffect(this.statusEffectsToApply[index]);
            }
            ++this.m_healthHaverHitCount;
            return killedTarget ? Projectile.HandleDamageResult.HEALTH_AND_KILLED : Projectile.HandleDamageResult.HEALTH;
        }

        [DebuggerHidden]
        private IEnumerator HandleDelayedDamage(
            SpeculativeRigidbody targetRigidbody,
            float damage,
            Vector2 damageVec,
            PixelCollider hitPixelCollider)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Projectile__HandleDelayedDamagec__Iterator5()
            {
                targetRigidbody = targetRigidbody,
                damage = damage,
                damageVec = damageVec,
                hitPixelCollider = hitPixelCollider,
                _this = this
            };
        }

        public void HandleKnockback(
            SpeculativeRigidbody rigidbody,
            PlayerController player,
            bool killedTarget = false,
            bool alreadyPlayerDelayed = false)
        {
            if (!alreadyPlayerDelayed && Projectile.s_delayPlayerDamage && (bool) (UnityEngine.Object) player)
                return;
            KnockbackDoer knockbackDoer = rigidbody.knockbackDoer;
            Vector2 direction = this.LastVelocity;
            if (this.HasFixedKnockbackDirection)
                direction = BraveMathCollege.DegreesToVector(this.FixedKnockbackDirection);
            if (!(bool) (UnityEngine.Object) knockbackDoer)
                return;
            if (killedTarget)
                knockbackDoer.ApplySourcedKnockback(direction, this.baseData.force * knockbackDoer.deathMultiplier, this.gameObject);
            else
                knockbackDoer.ApplySourcedKnockback(direction, this.baseData.force, this.gameObject);
        }

        protected virtual void HandleHitEffectsEnemy(
            SpeculativeRigidbody rigidbody,
            CollisionData lcr,
            bool playProjectileDeathVfx)
        {
            if (this.hitEffects == null)
                return;
            if (this.hitEffects.alwaysUseMidair)
            {
                this.HandleHitEffectsMidair();
            }
            else
            {
                Vector3 vector3Zup = lcr.Contact.ToVector3ZUp(-1f);
                float num = 0.0f;
                bool flag = false;
                if ((UnityEngine.Object) rigidbody != (UnityEngine.Object) null)
                {
                    HitEffectHandler hitEffectHandler = rigidbody.hitEffectHandler;
                    if ((UnityEngine.Object) hitEffectHandler != (UnityEngine.Object) null)
                    {
                        if (hitEffectHandler.SuppressAllHitEffects)
                        {
                            flag = true;
                        }
                        else
                        {
                            if (hitEffectHandler.additionalHitEffects.Length > 0)
                                hitEffectHandler.HandleAdditionalHitEffects(this.specRigidbody.Velocity, lcr.OtherPixelCollider);
                            if (hitEffectHandler.overrideHitEffectPool != null && hitEffectHandler.overrideHitEffectPool.type != VFXPoolType.None)
                            {
                                hitEffectHandler.overrideHitEffectPool.SpawnAtPosition(vector3Zup, num, rigidbody.transform, new Vector2?(lcr.Normal), new Vector2?(this.specRigidbody.Velocity));
                                flag = true;
                            }
                            else if (hitEffectHandler.overrideHitEffect != null && hitEffectHandler.overrideHitEffect.effects.Length > 0)
                            {
                                hitEffectHandler.overrideHitEffect.SpawnAtPosition(vector3Zup, num, rigidbody.transform, new Vector2?(lcr.Normal), new Vector2?(this.specRigidbody.Velocity));
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                    return;
                this.hitEffects.HandleEnemyImpact(vector3Zup, num, rigidbody.transform, lcr.Normal, this.specRigidbody.Velocity, playProjectileDeathVfx);
            }
        }

        protected void HandleHitEffectsObject(
            SpeculativeRigidbody srb,
            CollisionData lcr,
            bool playProjectileDeathVfx)
        {
            if (this.hitEffects == null)
                return;
            if (this.hitEffects.alwaysUseMidair)
            {
                this.HandleHitEffectsMidair();
            }
            else
            {
                Vector3 vector3Zup = lcr.Contact.ToVector3ZUp(-1f);
                float rotation = Mathf.Atan2(lcr.Normal.y, lcr.Normal.x) * 57.29578f;
                bool flag = false;
                if ((UnityEngine.Object) srb != (UnityEngine.Object) null)
                {
                    HitEffectHandler hitEffectHandler = srb.hitEffectHandler;
                    if ((UnityEngine.Object) hitEffectHandler != (UnityEngine.Object) null)
                    {
                        if (hitEffectHandler.SuppressAllHitEffects)
                            flag = true;
                        else if ((UnityEngine.Object) hitEffectHandler.overrideMaterialDefinition != (UnityEngine.Object) null)
                        {
                            VFXComplex.SpawnMethod overrideSpawnMethod = !this.CenterTilemapHitEffectsByProjectileVelocity ? (VFXComplex.SpawnMethod) null : new VFXComplex.SpawnMethod(this.SpawnVFXProjectileCenter);
                            if ((double) Mathf.Abs(lcr.Normal.x) > (double) Mathf.Abs(lcr.Normal.y))
                            {
                                if (this.hitEffects.HasTileMapHorizontalEffects)
                                {
                                    this.hitEffects.HandleTileMapImpactHorizontal(vector3Zup, rotation, lcr.Normal, this.specRigidbody.Velocity, playProjectileDeathVfx, srb.transform, overrideSpawnMethod, new VFXComplex.SpawnMethod(this.SpawnVFXPostProcessStickyGrenades));
                                    flag = true;
                                }
                                else if (hitEffectHandler.overrideMaterialDefinition.fallbackHorizontalTileMapEffects.Length > 0)
                                {
                                    hitEffectHandler.overrideMaterialDefinition.SpawnRandomHorizontal(vector3Zup, rotation, srb.transform, lcr.Normal, this.specRigidbody.Velocity);
                                    flag = true;
                                }
                            }
                            else if (this.hitEffects.HasTileMapVerticalEffects)
                            {
                                if ((double) lcr.Normal.y > 0.0)
                                    this.hitEffects.HandleTileMapImpactVertical(vector3Zup, -0.25f, rotation, lcr.Normal, this.specRigidbody.Velocity, playProjectileDeathVfx, srb.transform, overrideSpawnMethod, new VFXComplex.SpawnMethod(this.SpawnVFXPostProcessStickyGrenades));
                                else
                                    this.hitEffects.HandleTileMapImpactVertical(vector3Zup, 0.25f, rotation, lcr.Normal, this.specRigidbody.Velocity, playProjectileDeathVfx, srb.transform, overrideSpawnMethod, new VFXComplex.SpawnMethod(this.SpawnVFXPostProcessStickyGrenades));
                                flag = true;
                            }
                            else if (hitEffectHandler.overrideMaterialDefinition.fallbackVerticalTileMapEffects.Length > 0)
                            {
                                hitEffectHandler.overrideMaterialDefinition.SpawnRandomVertical(vector3Zup, rotation, srb.transform, lcr.Normal, this.specRigidbody.Velocity);
                                flag = true;
                            }
                            if (this.damagesWalls)
                            {
                                Vector3 vector3 = lcr.Normal.normalized.ToVector3ZUp() * 0.1f;
                                Vector3 position = vector3Zup + vector3;
                                float damage = !(this.Owner is AIActor) ? this.ModifiedDamage : ProjectileData.FixedEnemyDamageToBreakables;
                                hitEffectHandler.overrideMaterialDefinition.SpawnRandomShard(position, lcr.Normal, damage);
                            }
                            this.HandleWallDecals(lcr, srb.transform);
                        }
                        else if (hitEffectHandler.overrideHitEffectPool != null && hitEffectHandler.overrideHitEffectPool.type != VFXPoolType.None)
                        {
                            hitEffectHandler.overrideHitEffectPool.SpawnAtPosition(vector3Zup, parent: srb.transform, sourceNormal: new Vector2?(lcr.Normal), sourceVelocity: new Vector2?(this.specRigidbody.Velocity));
                            flag = true;
                        }
                        else if (hitEffectHandler.overrideHitEffect != null && hitEffectHandler.overrideHitEffect.effects.Length > 0)
                        {
                            hitEffectHandler.overrideHitEffect.SpawnAtPosition(vector3Zup, parent: srb.transform, sourceNormal: new Vector2?(lcr.Normal), sourceVelocity: new Vector2?(this.specRigidbody.Velocity));
                            flag = true;
                        }
                    }
                }
                if (this is SharkProjectile)
                    flag = true;
                if (flag)
                    return;
                this.hitEffects.HandleEnemyImpact(vector3Zup, 0.0f, (Transform) null, lcr.Normal, this.specRigidbody.Velocity, true);
            }
        }

        public void LerpMaterialGlow(
            Material targetMaterial,
            float startGlow,
            float targetGlow,
            float duration)
        {
            this.StartCoroutine(this.LerpMaterialGlowCR(targetMaterial, startGlow, targetGlow, duration));
        }

        [DebuggerHidden]
        private IEnumerator LerpMaterialGlowCR(
            Material targetMaterial,
            float startGlow,
            float targetGlow,
            float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Projectile__LerpMaterialGlowCRc__Iterator6()
            {
                duration = duration,
                targetMaterial = targetMaterial,
                startGlow = startGlow,
                targetGlow = targetGlow,
                _this = this
            };
        }

        public GameObject SpawnVFXPostProcessStickyGrenades(
            GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            bool ignoresPools)
        {
            GameObject gameObject = SpawnManager.SpawnVFX(prefab, position, rotation);
            StickyGrenadeBuff component1 = this.GetComponent<StickyGrenadeBuff>();
            if ((bool) (UnityEngine.Object) component1)
            {
                StickyGrenadePersistentDebris component2 = gameObject.GetComponent<StickyGrenadePersistentDebris>();
                if ((bool) (UnityEngine.Object) component2)
                    component2.InitializeSelf(component1);
            }
            StrafeBleedBuff component3 = this.GetComponent<StrafeBleedBuff>();
            if ((bool) (UnityEngine.Object) component3)
            {
                StrafeBleedPersistentDebris component4 = gameObject.GetComponent<StrafeBleedPersistentDebris>();
                if ((bool) (UnityEngine.Object) component4)
                    component4.InitializeSelf(component3);
            }
            return gameObject;
        }

        public GameObject SpawnVFXProjectileCenter(
            GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            bool ignoresPools)
        {
            Vector3 position1 = position;
            if ((bool) (UnityEngine.Object) this.specRigidbody)
            {
                Vector3 vector3Zup = this.specRigidbody.UnitCenter.ToVector3ZUp(position.z);
                float num = Vector2.Distance((Vector2) vector3Zup, (Vector2) position);
                position1 = vector3Zup + this.LastVelocity.normalized.ToVector3ZUp() * num;
            }
            return SpawnManager.SpawnVFX(prefab, position1, rotation);
        }

        protected void HandleHitEffectsTileMap(CollisionData lcr, bool playProjectileDeathVfx)
        {
            if (this.hitEffects == null)
                return;
            if (this.hitEffects.alwaysUseMidair)
            {
                this.HandleHitEffectsMidair();
            }
            else
            {
                int x = Mathf.RoundToInt(lcr.Contact.x);
                int y = Mathf.RoundToInt(lcr.Contact.y);
                float num = 0.0f;
                if (GameManager.Instance.Dungeon.data.CheckInBounds(x, y))
                {
                    CellData cellData = GameManager.Instance.Dungeon.data[x, y];
                    if (cellData != null && cellData.diagonalWallType != DiagonalWallType.NONE)
                    {
                        if (cellData.diagonalWallType == DiagonalWallType.NORTHEAST || cellData.diagonalWallType == DiagonalWallType.NORTHWEST)
                            lcr.Normal = Vector2.down;
                        else
                            lcr.Normal = Vector2.up;
                    }
                }
                Vector3 vector3Zup = lcr.Contact.ToVector3ZUp(-1f);
                float rotation = Mathf.Atan2(lcr.Normal.y, lcr.Normal.x) * 57.29578f;
                VFXComplex.SpawnMethod overrideSpawnMethod = !this.CenterTilemapHitEffectsByProjectileVelocity ? (VFXComplex.SpawnMethod) null : new VFXComplex.SpawnMethod(this.SpawnVFXProjectileCenter);
                if ((double) lcr.Normal.y < -0.10000000149011612)
                    this.hitEffects.HandleTileMapImpactVertical(vector3Zup, 0.5f + num, rotation, lcr.Normal, this.specRigidbody.Velocity, playProjectileDeathVfx, overrideSpawnMethod: overrideSpawnMethod, overrideDeathSpawnMethod: new VFXComplex.SpawnMethod(this.SpawnVFXPostProcessStickyGrenades));
                else if ((double) lcr.Normal.y > 0.10000000149011612)
                    this.hitEffects.HandleTileMapImpactVertical(vector3Zup, num - 0.25f, rotation, lcr.Normal, this.specRigidbody.Velocity, playProjectileDeathVfx, overrideSpawnMethod: overrideSpawnMethod, overrideDeathSpawnMethod: new VFXComplex.SpawnMethod(this.SpawnVFXPostProcessStickyGrenades));
                else
                    this.hitEffects.HandleTileMapImpactHorizontal(vector3Zup, rotation, lcr.Normal, this.specRigidbody.Velocity, playProjectileDeathVfx, overrideSpawnMethod: overrideSpawnMethod, overrideDeathSpawnMethod: new VFXComplex.SpawnMethod(this.SpawnVFXPostProcessStickyGrenades));
                if (!this.damagesWalls)
                    return;
                Vector3 vector3_1 = lcr.Normal.normalized.ToVector3ZUp() * 0.1f;
                Vector3 vector3_2 = vector3Zup + vector3_1;
                if (!((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null))
                    return;
                int visualTypeAtPosition = GameManager.Instance.Dungeon.data.GetRoomVisualTypeAtPosition(vector3_2.XY());
                float damage = !(this.Owner is AIActor) ? this.ModifiedDamage : ProjectileData.FixedEnemyDamageToBreakables;
                GameManager.Instance.Dungeon.roomMaterialDefinitions[visualTypeAtPosition].SpawnRandomShard(vector3_2, lcr.Normal, damage);
            }
        }

        protected void HandleHitEffectsMidair(bool killedEarly = false)
        {
            if (this.hitEffects == null)
                return;
            if (killedEarly && (UnityEngine.Object) this.hitEffects.overrideEarlyDeathVfx != (UnityEngine.Object) null)
            {
                SpawnManager.SpawnVFX(this.hitEffects.overrideEarlyDeathVfx, this.m_transform.position, Quaternion.identity);
            }
            else
            {
                if (this.hitEffects.suppressMidairDeathVfx)
                    return;
                if ((UnityEngine.Object) this.hitEffects.overrideMidairDeathVFX != (UnityEngine.Object) null || this.hitEffects.alwaysUseMidair)
                {
                    GameObject gameObject = SpawnManager.SpawnVFX(this.hitEffects.overrideMidairDeathVFX, this.m_transform.position, !this.hitEffects.midairInheritsRotation ? Quaternion.identity : this.m_transform.rotation);
                    BraveBehaviour component = gameObject.GetComponent<BraveBehaviour>();
                    if (this.hitEffects.midairInheritsFlip)
                    {
                        component.sprite.FlipX = this.sprite.FlipX;
                        component.sprite.FlipY = this.sprite.FlipY;
                    }
                    if (this.hitEffects.overrideMidairZHeight != -1)
                        component.sprite.HeightOffGround = (float) this.hitEffects.overrideMidairZHeight;
                    if (this.hitEffects.midairInheritsVelocity)
                    {
                        if ((bool) (UnityEngine.Object) component.debris)
                        {
                            component.debris.Trigger(this.specRigidbody.Velocity.ToVector3ZUp(0.5f), 0.1f);
                        }
                        else
                        {
                            SimpleMover orAddComponent = gameObject.GetOrAddComponent<SimpleMover>();
                            orAddComponent.velocity = (Vector3) (this.m_currentDirection * this.m_currentSpeed * 0.4f);
                            if ((UnityEngine.Object) component.spriteAnimator != (UnityEngine.Object) null)
                            {
                                float num = (float) component.spriteAnimator.DefaultClip.frames.Length / component.spriteAnimator.DefaultClip.fps;
                                orAddComponent.acceleration = orAddComponent.velocity / num * -1f;
                            }
                        }
                    }
                    else if ((bool) (UnityEngine.Object) component.debris)
                        component.debris.Trigger(new Vector3((float) ((double) UnityEngine.Random.value * 2.0 - 1.0), (float) ((double) UnityEngine.Random.value * 2.0 - 1.0), 5f), 0.1f);
                    if (!(bool) (UnityEngine.Object) component.particleSystem)
                        return;
                    gameObject.transform.localRotation = Quaternion.Euler(0.0f, 90f, 0.0f);
                }
                else
                {
                    if (this.hitEffects == null || !(bool) (UnityEngine.Object) this.specRigidbody)
                        return;
                    this.hitEffects.HandleTileMapImpactVertical(this.m_transform.position, 0.0f, 0.0f, Vector2.zero, this.specRigidbody.Velocity, false);
                }
            }
        }

        protected override void OnDestroy()
        {
            StaticReferenceManager.RemoveProjectile(this);
            base.OnDestroy();
        }

        public void OnSpawned()
        {
            if (this.m_cachedBaseData == null)
            {
                this.m_cachedCollidesWithPlayer = this.collidesWithPlayer;
                this.m_cachedCollidesWithProjectiles = this.collidesWithProjectiles;
                this.m_cachedCollidesWithEnemies = this.collidesWithEnemies;
                this.m_cachedDamagesWalls = this.damagesWalls;
                this.m_cachedBaseData = new ProjectileData(this.baseData);
                this.m_cachedBulletScriptSettings = new BulletScriptSettings(this.BulletScriptSettings);
                if ((bool) (UnityEngine.Object) this.specRigidbody)
                {
                    this.m_cachedCollideWithTileMap = this.specRigidbody.CollideWithTileMap;
                    this.m_cachedCollideWithOthers = this.specRigidbody.CollideWithOthers;
                }
                if (!(bool) (UnityEngine.Object) this.sprite)
                    this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
                if (!(bool) (UnityEngine.Object) this.spriteAnimator && (bool) (UnityEngine.Object) this.sprite)
                    this.spriteAnimator = this.sprite.spriteAnimator;
                if ((bool) (UnityEngine.Object) this.sprite)
                    this.m_cachedSpriteId = this.sprite.spriteId;
            }
            if (this.enabled)
            {
                this.Start();
                this.specRigidbody.enabled = true;
                for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
                    this.specRigidbody.PixelColliders[index].IsTrigger = false;
                this.specRigidbody.Reinitialize();
                if ((bool) (UnityEngine.Object) this.TrailRenderer)
                    this.TrailRenderer.Clear();
                if ((bool) (UnityEngine.Object) this.CustomTrailRenderer)
                    this.CustomTrailRenderer.Clear();
                if ((bool) (UnityEngine.Object) this.ParticleTrail)
                    BraveUtility.EnableEmission(this.ParticleTrail, true);
            }
            this.m_spawnPool = SpawnManager.LastPrefabPool;
        }

        public virtual void OnDespawned() => this.Cleanup();

        public void BecomeBlackBullet()
        {
            if (this.IsBlackBullet || !(bool) (UnityEngine.Object) this.sprite)
                return;
            this.ForceBlackBullet = true;
            this.IsBlackBullet = true;
            this.sprite.usesOverrideMaterial = true;
            this.sprite.renderer.material.SetFloat("_BlackBullet", 1f);
            this.sprite.renderer.material.SetFloat("_EmissivePower", -40f);
        }

        public void ReturnFromBlackBullet()
        {
            if (!this.IsBlackBullet)
                return;
            this.IsBlackBullet = false;
            this.sprite.renderer.material.SetFloat("_BlackBullet", 0.0f);
            this.sprite.usesOverrideMaterial = false;
            this.sprite.ForceUpdateMaterial();
        }

        private void Cleanup()
        {
            StaticReferenceManager.RemoveProjectile(this);
            if ((bool) (UnityEngine.Object) this.specRigidbody)
                this.specRigidbody.enabled = false;
            this.ManualControl = false;
            this.IsBulletScript = false;
            this.SuppressHitEffects = false;
            this.ReturnFromBlackBullet();
            this.m_forceBlackBullet = false;
            this.collidesWithPlayer = this.m_cachedCollidesWithPlayer;
            this.collidesWithProjectiles = this.m_cachedCollidesWithProjectiles;
            this.collidesWithEnemies = this.m_cachedCollidesWithEnemies;
            this.damagesWalls = this.m_cachedDamagesWalls;
            this.m_timeElapsed = 0.0f;
            this.m_distanceElapsed = 0.0f;
            this.LastPosition = Vector3.zero;
            this.m_hasImpactedObject = false;
            this.m_hasDiedInAir = false;
            this.m_hasPierced = false;
            this.m_healthHaverHitCount = 0;
            this.m_ignoreTileCollisionsTimer = 0.0f;
            if (this.m_cachedBaseData != null && this.baseData != null)
                this.baseData.SetAll(this.m_cachedBaseData);
            if ((bool) (UnityEngine.Object) this.TrailRenderer)
                this.TrailRenderer.Clear();
            if ((bool) (UnityEngine.Object) this.CustomTrailRenderer)
                this.CustomTrailRenderer.Clear();
            if ((bool) (UnityEngine.Object) this.ParticleTrail)
                BraveUtility.EnableEmission(this.ParticleTrail, false);
            if (this.m_cachedBulletScriptSettings != null && this.BulletScriptSettings != null)
                this.BulletScriptSettings.SetAll(this.m_cachedBulletScriptSettings);
            if ((bool) (UnityEngine.Object) this.specRigidbody)
            {
                this.specRigidbody.CollideWithTileMap = this.m_cachedCollideWithTileMap;
                this.specRigidbody.CollideWithOthers = this.m_cachedCollideWithOthers;
                this.specRigidbody.ClearSpecificCollisionExceptions();
            }
            if ((bool) (UnityEngine.Object) this.spriteAnimator && !this.spriteAnimator.playAutomatically)
                this.spriteAnimator.Stop();
            if ((bool) (UnityEngine.Object) this.sprite && this.m_cachedSpriteId >= 0)
                this.sprite.SetSprite(this.m_cachedSpriteId);
            if ((bool) (UnityEngine.Object) this.sprite && this.m_isRamping)
            {
                this.m_isRamping = false;
                this.m_currentRampHeight = 0.0f;
                this.sprite.HeightOffGround = Projectile.CurrentProjectileDepth;
            }
            this.Owner = (GameActor) null;
            this.m_shooter = (SpeculativeRigidbody) null;
            this.TrapOwner = (ProjectileTrapController) null;
            this.OwnerName = (string) null;
            this.m_spawnPool = (PrefabPool) null;
            if ((bool) (UnityEngine.Object) this.specRigidbody)
                this.specRigidbody.Cleanup();
            this.m_initialized = false;
            if (!(bool) (UnityEngine.Object) this.specRigidbody)
                return;
            PhysicsEngine.Instance.DeregisterWhenAvailable(this.specRigidbody);
        }

        public Vector3 LastPosition
        {
            get => this.m_lastPosition;
            set => this.m_lastPosition = value;
        }

        public bool HasImpactedEnemy => this.m_hasImpactedEnemy;

        public int NumberHealthHaversHit => this.m_healthHaverHitCount;

        public bool HasDiedInAir => this.m_hasDiedInAir;

        public enum ProjectileDestroyMode
        {
            Destroy,
            DestroyComponent,
            BecomeDebris,
            None,
        }

        protected enum HandleDamageResult
        {
            NO_HEALTH,
            HEALTH,
            HEALTH_AND_KILLED,
        }
    }

