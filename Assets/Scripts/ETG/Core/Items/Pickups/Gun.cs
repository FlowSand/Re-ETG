using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.Serialization;

using Dungeonator;

#nullable disable

public class Gun : PickupObject, IPlayerInteractable
    {
        public static bool ActiveReloadActivated;
        public static bool ActiveReloadActivatedPlayerTwo;
        public static float s_DualWieldFactor = 0.75f;
        public string gunName = "gun";
        [FormerlySerializedAs("overrideAudioGunName")]
        public string gunSwitchGroup = string.Empty;
        public bool isAudioLoop;
        public bool lowersAudioWhileFiring;
        public GunClass gunClass;
        [SerializeField]
        public StatModifier[] currentGunStatModifiers;
        [SerializeField]
        public StatModifier[] passiveStatModifiers;
        [SerializeField]
        public DamageTypeModifier[] currentGunDamageTypeModifiers;
        [SerializeField]
        public int ArmorToGainOnPickup;
        public Transform barrelOffset;
        public Transform muzzleOffset;
        public Transform chargeOffset;
        public Transform reloadOffset;
        public IntVector2 carryPixelOffset;
        public IntVector2 carryPixelUpOffset;
        public IntVector2 carryPixelDownOffset;
        public bool UsesPerCharacterCarryPixelOffsets;
        public CharacterCarryPixelOffset[] PerCharacterPixelOffsets;
        public IntVector2 leftFacingPixelOffset;
        public GunHandedness gunHandedness;
        public GunHandedness overrideOutOfAmmoHandedness;
        public AdditionalHandState additionalHandState;
        public GunPositionOverride gunPosition;
        public bool forceFlat;
        [FormerlySerializedAs("volley")]
        [SerializeField]
        private ProjectileVolleyData rawVolley;
        public ProjectileModule singleModule;
        [SerializeField]
        public ProjectileVolleyData rawOptionalReloadVolley;
        [NonSerialized]
        public bool OverrideFinaleAudio;
        [NonSerialized]
        public bool HasFiredHolsterShot;
        [NonSerialized]
        public bool HasFiredReloadSynergy;
        [NonSerialized]
        public ProjectileVolleyData modifiedVolley;
        [NonSerialized]
        public ProjectileVolleyData modifiedFinalVolley;
        [NonSerialized]
        public ProjectileVolleyData modifiedOptionalReloadVolley;
        [NonSerialized]
        public List<int> DuctTapeMergedGunIDs;
        [NonSerialized]
        public bool PreventNormalFireAudio;
        [NonSerialized]
        public string OverrideNormalFireAudioEvent;
        public int ammo = 25;
        public bool CanGainAmmo = true;
        [FormerlySerializedAs("InfiniteAmmo")]
        public bool LocalInfiniteAmmo;
        public const float c_FallbackBossDamageModifier = 0.8f;
        public const float c_LuteCompanionDamageMultiplier = 2f;
        public const float c_LuteCompanionScaleMultiplier = 1.75f;
        public const float c_LuteCompanionFireRateMultiplier = 1.5f;
        public bool UsesBossDamageModifier;
        public float CustomBossDamageModifier = -1f;
        [SerializeField]
        private int maxAmmo = -1;
        public float reloadTime;
        [NonSerialized]
        public bool CanReloadNoMatterAmmo;
        public bool blankDuringReload;
        [ShowInInspectorIf("blankDuringReload", false)]
        public float blankReloadRadius = 1f;
        [ShowInInspectorIf("blankDuringReload", false)]
        public bool reflectDuringReload;
        [ShowInInspectorIf("blankDuringReload", false)]
        public float blankKnockbackPower = 20f;
        [ShowInInspectorIf("blankDuringReload", false)]
        public float blankDamageToEnemies;
        [ShowInInspectorIf("blankDuringReload", false)]
        public float blankDamageScalingOnEmptyClip = 1f;
        [NonSerialized]
        private float AdditionalReloadMultiplier = 1f;
        [NonSerialized]
        private int SequentialActiveReloads;
        public bool doesScreenShake = true;
        public ScreenShakeSettings gunScreenShake = new ScreenShakeSettings(1f, 1f, 0.5f, 0.5f);
        public bool directionlessScreenShake;
        public int damageModifier;
        public GameObject thrownObject;
        public ProceduralGunData procGunData;
        public ActiveReloadData activeReloadData;
        public bool ClearsCooldownsLikeAWP;
        public bool AppliesHoming;
        public float AppliedHomingAngularVelocity = 180f;
        public float AppliedHomingDetectRadius = 4f;
        [SerializeField]
        private bool m_unswitchableGun;
        [CheckAnimation(null)]
        public string shootAnimation = string.Empty;
        [ShowInInspectorIf("shootAnimation", false)]
        public bool usesContinuousFireAnimation;
        [CheckAnimation(null)]
        public string reloadAnimation = string.Empty;
        [CheckAnimation(null)]
        public string emptyReloadAnimation = string.Empty;
        [CheckAnimation(null)]
        public string idleAnimation = string.Empty;
        [CheckAnimation(null)]
        public string chargeAnimation = string.Empty;
        [CheckAnimation(null)]
        public string dischargeAnimation = string.Empty;
        [CheckAnimation(null)]
        public string emptyAnimation = string.Empty;
        [CheckAnimation(null)]
        public string introAnimation = string.Empty;
        [CheckAnimation(null)]
        public string finalShootAnimation = string.Empty;
        [CheckAnimation(null)]
        public string enemyPreFireAnimation = string.Empty;
        [CheckAnimation(null)]
        public string outOfAmmoAnimation = string.Empty;
        [CheckAnimation(null)]
        public string criticalFireAnimation = string.Empty;
        [CheckAnimation(null)]
        public string dodgeAnimation = string.Empty;
        public bool usesDirectionalIdleAnimations;
        public bool usesDirectionalAnimator;
        public bool preventRotation;
        public VFXPool muzzleFlashEffects;
        [ShowInInspectorIf("muzzleFlashEffects", false)]
        public bool usesContinuousMuzzleFlash;
        public VFXPool finalMuzzleFlashEffects;
        public VFXPool reloadEffects;
        public VFXPool emptyReloadEffects;
        public VFXPool activeReloadSuccessEffects;
        public VFXPool activeReloadFailedEffects;
        public Light light;
        public float baseLightIntensity;
        public GameObject shellCasing;
        public int shellsToLaunchOnFire = 1;
        public int shellCasingOnFireFrameDelay;
        public int shellsToLaunchOnReload;
        public int reloadShellLaunchFrame;
        public GameObject clipObject;
        public int clipsToLaunchOnReload;
        public int reloadClipLaunchFrame;
        [HideInInspector]
        public string prefabName = string.Empty;
        public bool rampBullets;
        public float rampStartHeight = 2f;
        public float rampTime = 1f;
        public bool IgnoresAngleQuantization;
        public bool IsTrickGun;
        public bool TrickGunAlternatesHandedness;
        public bool PreventOutlines;
        public ProjectileVolleyData alternateVolley;
        [CheckAnimation(null)]
        public string alternateShootAnimation;
        [CheckAnimation(null)]
        public string alternateReloadAnimation;
        [CheckAnimation(null)]
        public string alternateIdleAnimation;
        public string alternateSwitchGroup;
        public bool IsHeroSword;
        public bool HeroSwordDoesntBlank;
        public bool StarterGunForAchievement;
        private float HeroSwordCooldown;
        public bool CanSneakAttack;
        public float SneakAttackDamageMultiplier = 3f;
        public bool SuppressLaserSight;
        public bool RequiresFundsToShoot;
        public int CurrencyCostPerShot = 1;
        public GunWeaponPanelSpriteOverride weaponPanelSpriteOverride;
        public bool IsLuteCompanionBuff;
        public bool MovesPlayerForwardOnChargeFire;
        public bool LockedHorizontalOnCharge;
        public float LockedHorizontalCenterFireOffset = -1f;
        [NonSerialized]
        public bool LockedHorizontalOnReload;
        private float LockedHorizontalCachedAngle = -1f;
        public bool GoopReloadsFree;
        public bool IsUndertaleGun;
        public bool LocalActiveReload;
        public bool UsesRechargeLikeActiveItem;
        public float ActiveItemStyleRechargeAmount = 100f;
        public bool CanAttackThroughObjects;
        private float m_remainingActiveCooldownAmount;
        public bool CanCriticalFire;
        public float CriticalChance = 0.1f;
        public float CriticalDamageMultiplier = 3f;
        public VFXPool CriticalMuzzleFlashEffects;
        public Projectile CriticalReplacementProjectile;
        public bool GainsRateOfFireAsContinueAttack;
        public float RateOfFireMultiplierAdditionPerSecond;
        public bool OnlyUsesIdleInWeaponBox;
        public bool DisablesRendererOnCooldown;
        [FormerlySerializedAs("ObjectToInstantiatedOnClipDepleted")]
        [SerializeField]
        public GameObject ObjectToInstantiateOnReload;
        [NonSerialized]
        public int AdditionalClipCapacity;
        private RoomHandler m_minimapIconRoom;
        private GameObject m_instanceMinimapIcon;
        private GunHandedness? m_cachedGunHandedness;
        public Action<GameActor> OnInitializedWithOwner;
        public Action<Projectile> PostProcessProjectile;
        public Action<ProjectileVolleyData> PostProcessVolley;
        public System.Action OnDropped;
        public Action<PlayerController, Gun> OnAutoReload;
        public Action<PlayerController, Gun, bool> OnReloadPressed;
        public Action<PlayerController, Gun> OnFinishAttack;
        public Action<PlayerController, Gun> OnPostFired;
        public Action<PlayerController, Gun> OnAmmoChanged;
        public Action<PlayerController, Gun> OnBurstContinued;
        public Func<float, float> OnReflectedBulletDamageModifier;
        public Func<float, float> OnReflectedBulletScaleModifier;
        [NonSerialized]
        private tk2dTiledSprite m_extantLaserSight;
        [NonSerialized]
        public int LastShotIndex = -1;
        [NonSerialized]
        public bool DidTransformGunThisFrame;
        [NonSerialized]
        public float CustomLaserSightDistance = 30f;
        [NonSerialized]
        public float CustomLaserSightHeight = 0.25f;
        [NonSerialized]
        public AIActor LastLaserSightEnemy;
        private GameObject m_extantLockOnSprite;
        private bool m_hasReinitializedAudioSwitch;
        [NonSerialized]
        public bool HasEverBeenAcquiredByPlayer;
        private SingleSpawnableGunPlacedObject m_extantAmp;
        [NonSerialized]
        public bool ForceNextShotCritical;
        private bool m_isCritting;
        public Func<Gun, Projectile, ProjectileModule, Projectile> OnPreFireProjectileModifier;
        public Func<float, float> ModifyActiveCooldownDamage;
        private const bool c_clickingCanActiveReload = true;
        private const bool c_DUAL_WIELD_PARALLEL_RELOAD = false;
        private bool m_hasSwappedTrickGunsThisCycle;
        protected List<ModuleShootData> m_activeBeams = new List<ModuleShootData>();
        private string[] m_directionalIdleNames;
        private bool m_preventIdleLoop;
        private bool m_hasDecrementedFunds;
        private Transform m_throwPrepTransform;
        private tk2dBaseSprite m_sprite;
        private tk2dSpriteAnimator m_anim;
        private GameActor m_owner;
        private int m_defaultSpriteID;
        private Transform m_transform;
        private Transform m_attachTransform;
        private List<Transform> m_childTransformsToFlip;
        private Vector3 m_defaultLocalPosition;
        private MeshRenderer m_meshRenderer;
        private Transform m_clipLaunchAttachPoint;
        private Transform m_localAttachPoint;
        private Transform m_offhandAttachPoint;
        private Transform m_casingLaunchAttachPoint;
        private float gunAngle;
        private float prevGunAngleUnmodified;
        private float gunCooldownModifier;
        private Vector2 m_localAimPoint;
        private Vector3 m_unroundedBarrelPosition;
        private Vector3 m_originalBarrelOffsetPosition;
        private Vector3 m_originalMuzzleOffsetPosition;
        private Vector3 m_originalChargeOffsetPosition;
        private float m_fractionalAmmoUsage;
        public bool HasBeenPickedUp;
        private bool m_reloadWhenDoneFiring;
        private bool m_isReloading;
        private bool m_isThrown;
        private bool m_thrownOnGround = true;
        private bool m_canAttemptActiveReload;
        private bool m_isCurrentlyFiring;
        private bool m_isAudioLooping;
        private float m_continuousAttackTime;
        private float m_reloadElapsed;
        private bool m_hasDoneSingleReloadBlankEffect;
        private bool m_cachedIsGunBlocked;
        private bool m_playedEmptyClipSound;
        private VFXPool m_currentlyPlayingChargeVFX;
        private bool m_midBurstFire;
        private bool m_continueBurstInUpdate;
        private bool m_isContinuousMuzzleFlashOut;
        private Dictionary<ProjectileModule, ModuleShootData> m_moduleData;
        [NonSerialized]
        private List<ActiveAmmunitionData> m_customAmmunitions = new List<ActiveAmmunitionData>();
        private int m_currentStrengthTier;
        [NonSerialized]
        public Dictionary<string, string> AdditionalShootSoundsByModule = new Dictionary<string, string>();
        [NonSerialized]
        public float? OverrideAngleSnap;
        private bool m_isPreppedForThrow;
        private float m_prepThrowTime = -0.3f;
        private const float c_prepTime = 1.2f;
        private const bool c_attackingCanReload = true;
        private const bool c_throwGunOnFire = true;

        public IntVector2 GetCarryPixelOffset(PlayableCharacters id)
        {
            IntVector2 carryPixelOffset = this.carryPixelOffset;
            if (this.UsesPerCharacterCarryPixelOffsets)
            {
                for (int index = 0; index < this.PerCharacterPixelOffsets.Length; ++index)
                {
                    if (this.PerCharacterPixelOffsets[index].character == id)
                        carryPixelOffset += this.PerCharacterPixelOffsets[index].carryPixelOffset;
                }
            }
            return carryPixelOffset;
        }

        public GameActor CurrentOwner => this.m_owner;

        public bool OwnerHasSynergy(CustomSynergyType synergyToCheck)
        {
            return (bool) (UnityEngine.Object) this.m_owner && this.m_owner is PlayerController && (this.m_owner as PlayerController).HasActiveBonusSynergy(synergyToCheck);
        }

        public ProjectileVolleyData RawSourceVolley
        {
            get => this.rawVolley;
            set => this.rawVolley = value;
        }

        public ProjectileVolleyData Volley
        {
            get => this.modifiedVolley ?? this.rawVolley;
            set => this.rawVolley = value;
        }

        public ProjectileVolleyData OptionalReloadVolley
        {
            get => this.modifiedOptionalReloadVolley ?? this.rawOptionalReloadVolley;
        }

        public int CurrentAmmo
        {
            get
            {
                return this.RequiresFundsToShoot && this.m_owner is PlayerController ? this.ClipShotsRemaining : this.ammo;
            }
            set => this.ammo = value;
        }

        public bool InfiniteAmmo
        {
            get
            {
                if (!(bool) (UnityEngine.Object) this.m_owner || !(this.m_owner is PlayerController))
                    return this.LocalInfiniteAmmo;
                return this.LocalInfiniteAmmo || (this.m_owner as PlayerController).InfiniteAmmo.Value;
            }
            set => this.LocalInfiniteAmmo = value;
        }

        public int GetBaseMaxAmmo() => this.maxAmmo;

        public int AdjustedMaxAmmo
        {
            get
            {
                if (this.InfiniteAmmo)
                    return int.MaxValue;
                if ((UnityEngine.Object) this.m_owner == (UnityEngine.Object) null || !(this.m_owner is PlayerController))
                    return this.maxAmmo;
                if (this.RequiresFundsToShoot)
                    return this.ClipShotsRemaining;
                return (UnityEngine.Object) (this.m_owner as PlayerController).stats != (UnityEngine.Object) null ? Mathf.RoundToInt((this.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.AmmoCapacityMultiplier) * (float) this.maxAmmo) : this.maxAmmo;
            }
        }

        public void SetBaseMaxAmmo(int a) => this.maxAmmo = a;

        public float AdjustedReloadTime
        {
            get
            {
                float num = 1f;
                if (this.m_owner is PlayerController)
                {
                    PlayerController owner = this.m_owner as PlayerController;
                    if ((bool) (UnityEngine.Object) owner.CurrentGun && (bool) (UnityEngine.Object) owner.CurrentSecondaryGun && (UnityEngine.Object) owner.CurrentSecondaryGun == (UnityEngine.Object) this && (UnityEngine.Object) owner.CurrentGun != (UnityEngine.Object) this)
                        return owner.CurrentGun.AdjustedReloadTime;
                    num = owner.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);
                }
                return this.reloadTime * num * this.AdditionalReloadMultiplier;
            }
        }

        public bool UnswitchableGun => this.m_unswitchableGun;

        public bool LuteCompanionBuffActive => this.IsLuteCompanionBuff && this.IsFiring;

        public float RemainingActiveCooldownAmount
        {
            get => this.m_remainingActiveCooldownAmount;
            set
            {
                if ((double) this.m_remainingActiveCooldownAmount > 0.0 && (double) value <= 0.0 && (bool) (UnityEngine.Object) this.m_owner)
                {
                    int num = (int) AkSoundEngine.PostEvent("Play_UI_cooldown_ready_01", this.m_owner.gameObject);
                }
                this.m_remainingActiveCooldownAmount = value;
            }
        }

        public float CurrentActiveItemChargeAmount
        {
            get
            {
                return Mathf.Clamp01((float) (1.0 - (double) this.m_remainingActiveCooldownAmount / (double) this.ActiveItemStyleRechargeAmount));
            }
        }

        public float CurrentAngle => this.gunAngle;

        public ProjectileModule DefaultModule
        {
            get
            {
                if (!(bool) (UnityEngine.Object) this.Volley)
                    return this.singleModule;
                if (this.Volley.ModulesAreTiers)
                {
                    for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    {
                        ProjectileModule projectile = this.Volley.projectiles[index];
                        if (projectile != null && (projectile.CloneSourceIndex < 0 ? index : projectile.CloneSourceIndex) == this.CurrentStrengthTier)
                            return projectile;
                    }
                }
                return this.Volley.projectiles[0];
            }
        }

        public bool IsAutomatic => this.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Automatic;

        public bool HasChargedProjectileReady
        {
            get
            {
                if (!this.m_isCurrentlyFiring)
                    return false;
                if ((UnityEngine.Object) this.Volley == (UnityEngine.Object) null)
                {
                    if (this.singleModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                    {
                        ProjectileModule.ChargeProjectile chargeProjectile = this.singleModule.GetChargeProjectile(this.m_moduleData[this.singleModule].chargeTime);
                        if (chargeProjectile != null && (bool) (UnityEngine.Object) chargeProjectile.Projectile)
                            return true;
                    }
                    return false;
                }
                ProjectileVolleyData volley = this.Volley;
                for (int index = 0; index < volley.projectiles.Count; ++index)
                {
                    ProjectileModule projectile = volley.projectiles[index];
                    if (projectile.shootStyle == ProjectileModule.ShootStyle.Charged)
                    {
                        ModuleShootData moduleShootData = this.m_moduleData[projectile];
                        ProjectileModule.ChargeProjectile chargeProjectile = projectile.GetChargeProjectile(moduleShootData.chargeTime);
                        if (chargeProjectile != null && (bool) (UnityEngine.Object) chargeProjectile.Projectile)
                            return true;
                    }
                }
                return false;
            }
        }

        public GameUIAmmoType.AmmoType AmmoType => this.DefaultModule.ammoType;

        public string CustomAmmoType => this.DefaultModule.customAmmoType;

        public override string DisplayName
        {
            get
            {
                EncounterTrackable component = this.GetComponent<EncounterTrackable>();
                return (bool) (UnityEngine.Object) component ? component.GetModifiedDisplayName() : this.gunName;
            }
        }

        public int ClipShotsRemaining
        {
            get
            {
                if (this.RequiresFundsToShoot && this.m_owner is PlayerController)
                    return Mathf.FloorToInt((float) (this.m_owner as PlayerController).carriedConsumables.Currency / (float) this.CurrencyCostPerShot);
                int ammo = this.ammo;
                int a = this.m_moduleData == null || !this.m_moduleData.ContainsKey(this.DefaultModule) ? (this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner) > 0 ? this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner) : this.ammo) : (this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner) > 0 ? this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner) - this.m_moduleData[this.DefaultModule].numberShotsFired : this.ammo);
                if (a > this.ammo)
                    this.ClipShotsRemaining = this.ammo;
                return Mathf.Min(a, this.ammo);
            }
            set
            {
                if (!this.m_moduleData.ContainsKey(this.DefaultModule))
                    return;
                this.m_moduleData[this.DefaultModule].numberShotsFired = this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner) - value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (UnityEngine.Object) this.Volley != (UnityEngine.Object) null ? !this.CheckHasLoadedModule(this.Volley) : !this.CheckHasLoadedModule(this.singleModule);
            }
        }

        public int ClipCapacity
        {
            get
            {
                return this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner) <= 0 ? this.AdjustedMaxAmmo : this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner);
            }
        }

        private Vector3 ClipLaunchPoint
        {
            get
            {
                return (UnityEngine.Object) this.m_clipLaunchAttachPoint == (UnityEngine.Object) null ? Vector3.zero : this.m_clipLaunchAttachPoint.position;
            }
        }

        private Vector3 CasingLaunchPoint
        {
            get
            {
                return (UnityEngine.Object) this.m_casingLaunchAttachPoint == (UnityEngine.Object) null ? this.barrelOffset.position : this.m_casingLaunchAttachPoint.position;
            }
        }

        public GunHandedness Handedness
        {
            get
            {
                bool flag1 = this.m_owner is PlayerController && (this.m_owner as PlayerController).inventory != null && (this.m_owner as PlayerController).inventory.DualWielding;
                if (this.ammo == 0 && this.overrideOutOfAmmoHandedness != GunHandedness.AutoDetect)
                    return this.overrideOutOfAmmoHandedness;
                if (this.IsPreppedForThrow)
                    return GunHandedness.OneHanded;
                if (!this.m_cachedGunHandedness.HasValue)
                {
                    if (this.gunHandedness == GunHandedness.AutoDetect)
                    {
                        Transform transform = this.transform.Find("SecondaryHand");
                        bool flag2 = (UnityEngine.Object) transform != (UnityEngine.Object) null;
                        if (this.IsTrickGun && this.TrickGunAlternatesHandedness)
                            flag2 = (UnityEngine.Object) transform != (UnityEngine.Object) null && transform.gameObject.activeSelf;
                        this.m_cachedGunHandedness = new GunHandedness?(!flag2 ? GunHandedness.OneHanded : GunHandedness.TwoHanded);
                    }
                    else
                        this.m_cachedGunHandedness = new GunHandedness?(this.gunHandedness);
                }
                GunHandedness? cachedGunHandedness = this.m_cachedGunHandedness;
                return (cachedGunHandedness.GetValueOrDefault() != GunHandedness.TwoHanded ? 0 : (cachedGunHandedness.HasValue ? 1 : 0)) != 0 && flag1 ? GunHandedness.OneHanded : this.m_cachedGunHandedness.Value;
            }
        }

        public bool IsForwardPosition
        {
            get
            {
                switch (this.gunPosition)
                {
                    case GunPositionOverride.AutoDetect:
                        return this.Handedness == GunHandedness.OneHanded || this.Handedness == GunHandedness.HiddenOneHanded;
                    case GunPositionOverride.Forward:
                        return true;
                    case GunPositionOverride.Back:
                        return false;
                    default:
                        UnityEngine.Debug.LogWarning((object) ("Unhandled GunPositionOverride type: " + (object) this.gunPosition));
                        return true;
                }
            }
        }

        public Transform PrimaryHandAttachPoint => this.m_localAttachPoint;

        public Transform SecondaryHandAttachPoint
        {
            get
            {
                if (this.IsTrickGun && this.TrickGunAlternatesHandedness && (UnityEngine.Object) this.m_offhandAttachPoint == (UnityEngine.Object) null)
                    this.m_offhandAttachPoint = this.transform.Find("SecondaryHand");
                return this.m_offhandAttachPoint;
            }
        }

        public bool IsFiring => this.m_isCurrentlyFiring;

        public bool IsReloading => this.m_isReloading;

        public bool IsCharging
        {
            get
            {
                if (!this.m_isCurrentlyFiring)
                    return false;
                if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
                {
                    for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    {
                        ProjectileModule projectile = this.Volley.projectiles[index];
                        if (projectile.shootStyle == ProjectileModule.ShootStyle.Charged && !this.m_moduleData[projectile].chargeFired)
                            return true;
                    }
                }
                else if (this.singleModule.shootStyle == ProjectileModule.ShootStyle.Charged && !this.m_moduleData[this.singleModule].chargeFired)
                    return true;
                return false;
            }
        }

        public bool NoOwnerOverride { get; set; }

        public Projectile LastProjectile { get; set; }

        public int DefaultSpriteID
        {
            get => this.m_defaultSpriteID;
            set => this.m_defaultSpriteID = value;
        }

        public bool IsInWorld => this.m_isThrown;

        public bool LaserSightIsGreen { get; set; }

        public bool DoubleWideLaserSight { get; set; }

        public bool ForceLaserSight { get; set; }

        public tk2dBaseSprite LaserSight => (tk2dBaseSprite) this.m_extantLaserSight;

        public bool IsMinusOneGun { get; set; }

        public void TransformToTargetGun(Gun targetGun)
        {
            int clipShotsRemaining = this.ClipShotsRemaining;
            if (this.m_currentlyPlayingChargeVFX != null)
            {
                this.m_currentlyPlayingChargeVFX.DestroyAll();
                this.m_currentlyPlayingChargeVFX = (VFXPool) null;
            }
            ProjectileVolleyData volley = this.Volley;
            this.rawVolley = targetGun.rawVolley;
            this.singleModule = targetGun.singleModule;
            this.modifiedVolley = (ProjectileVolleyData) null;
            if ((bool) (UnityEngine.Object) targetGun.sprite)
            {
                this.m_defaultSpriteID = targetGun.sprite.spriteId;
                this.m_sprite.SetSprite(targetGun.sprite.Collection, this.m_defaultSpriteID);
                if ((bool) (UnityEngine.Object) this.spriteAnimator && (bool) (UnityEngine.Object) targetGun.spriteAnimator)
                    this.spriteAnimator.Library = targetGun.spriteAnimator.Library;
                tk2dSpriteDefinition.AttachPoint[] attachPoints = this.m_sprite.Collection.GetAttachPoints(this.m_defaultSpriteID);
                tk2dSpriteDefinition.AttachPoint attachPoint = attachPoints == null ? (tk2dSpriteDefinition.AttachPoint) null : Array.Find<tk2dSpriteDefinition.AttachPoint>(attachPoints, (Predicate<tk2dSpriteDefinition.AttachPoint>) (a => a.name == "PrimaryHand"));
                if (attachPoint != null)
                    this.m_defaultLocalPosition = -attachPoint.position;
            }
            if (targetGun.maxAmmo != this.maxAmmo && targetGun.maxAmmo > 0)
            {
                int num = !this.InfiniteAmmo ? this.AdjustedMaxAmmo : this.maxAmmo;
                this.maxAmmo = targetGun.maxAmmo;
                if (this.AdjustedMaxAmmo > 0 && num > 0 && this.ammo > 0 && !this.InfiniteAmmo)
                {
                    this.ammo = Mathf.FloorToInt((float) this.ammo / (float) num * (float) this.AdjustedMaxAmmo);
                    this.ammo = Mathf.Min(this.ammo, this.AdjustedMaxAmmo);
                }
                else
                    this.ammo = Mathf.Min(this.ammo, this.maxAmmo);
            }
            this.gunSwitchGroup = targetGun.gunSwitchGroup;
            this.isAudioLoop = targetGun.isAudioLoop;
            this.gunClass = targetGun.gunClass;
            if (!string.IsNullOrEmpty(this.gunSwitchGroup))
            {
                int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.gunSwitchGroup, this.gameObject);
            }
            this.currentGunDamageTypeModifiers = targetGun.currentGunDamageTypeModifiers;
            this.carryPixelOffset = targetGun.carryPixelOffset;
            this.carryPixelUpOffset = targetGun.carryPixelUpOffset;
            this.carryPixelDownOffset = targetGun.carryPixelDownOffset;
            this.leftFacingPixelOffset = targetGun.leftFacingPixelOffset;
            this.UsesPerCharacterCarryPixelOffsets = targetGun.UsesPerCharacterCarryPixelOffsets;
            this.PerCharacterPixelOffsets = targetGun.PerCharacterPixelOffsets;
            this.gunPosition = targetGun.gunPosition;
            this.forceFlat = targetGun.forceFlat;
            if (targetGun.GainsRateOfFireAsContinueAttack != this.GainsRateOfFireAsContinueAttack)
            {
                this.GainsRateOfFireAsContinueAttack = targetGun.GainsRateOfFireAsContinueAttack;
                this.RateOfFireMultiplierAdditionPerSecond = targetGun.RateOfFireMultiplierAdditionPerSecond;
            }
            if ((bool) (UnityEngine.Object) this.barrelOffset && (bool) (UnityEngine.Object) targetGun.barrelOffset)
            {
                this.barrelOffset.localPosition = targetGun.barrelOffset.localPosition;
                this.m_originalBarrelOffsetPosition = targetGun.barrelOffset.localPosition;
            }
            if ((bool) (UnityEngine.Object) this.muzzleOffset && (bool) (UnityEngine.Object) targetGun.muzzleOffset)
            {
                this.muzzleOffset.localPosition = targetGun.muzzleOffset.localPosition;
                this.m_originalMuzzleOffsetPosition = targetGun.muzzleOffset.localPosition;
            }
            if ((bool) (UnityEngine.Object) this.chargeOffset && (bool) (UnityEngine.Object) targetGun.chargeOffset)
            {
                this.chargeOffset.localPosition = targetGun.chargeOffset.localPosition;
                this.m_originalChargeOffsetPosition = targetGun.chargeOffset.localPosition;
            }
            this.reloadTime = targetGun.reloadTime;
            this.blankDuringReload = targetGun.blankDuringReload;
            this.blankReloadRadius = targetGun.blankReloadRadius;
            this.reflectDuringReload = targetGun.reflectDuringReload;
            this.blankKnockbackPower = targetGun.blankKnockbackPower;
            this.blankDamageToEnemies = targetGun.blankDamageToEnemies;
            this.blankDamageScalingOnEmptyClip = targetGun.blankDamageScalingOnEmptyClip;
            this.doesScreenShake = targetGun.doesScreenShake;
            this.gunScreenShake = targetGun.gunScreenShake;
            this.directionlessScreenShake = targetGun.directionlessScreenShake;
            this.AppliesHoming = targetGun.AppliesHoming;
            this.AppliedHomingAngularVelocity = targetGun.AppliedHomingAngularVelocity;
            this.AppliedHomingDetectRadius = targetGun.AppliedHomingDetectRadius;
            this.GoopReloadsFree = targetGun.GoopReloadsFree;
            this.gunHandedness = targetGun.gunHandedness;
            this.m_cachedGunHandedness = new GunHandedness?();
            this.shootAnimation = targetGun.shootAnimation;
            this.usesContinuousFireAnimation = targetGun.usesContinuousFireAnimation;
            this.reloadAnimation = targetGun.reloadAnimation;
            this.emptyReloadAnimation = targetGun.emptyReloadAnimation;
            this.idleAnimation = targetGun.idleAnimation;
            this.chargeAnimation = targetGun.chargeAnimation;
            this.dischargeAnimation = targetGun.dischargeAnimation;
            this.emptyAnimation = targetGun.emptyAnimation;
            this.introAnimation = targetGun.introAnimation;
            this.finalShootAnimation = targetGun.finalShootAnimation;
            this.enemyPreFireAnimation = targetGun.enemyPreFireAnimation;
            this.dodgeAnimation = targetGun.dodgeAnimation;
            this.muzzleFlashEffects = targetGun.muzzleFlashEffects;
            this.usesContinuousMuzzleFlash = targetGun.usesContinuousMuzzleFlash;
            this.finalMuzzleFlashEffects = targetGun.finalMuzzleFlashEffects;
            this.reloadEffects = targetGun.reloadEffects;
            this.emptyReloadEffects = targetGun.emptyReloadEffects;
            this.activeReloadSuccessEffects = targetGun.activeReloadSuccessEffects;
            this.activeReloadFailedEffects = targetGun.activeReloadFailedEffects;
            this.shellCasing = targetGun.shellCasing;
            this.shellsToLaunchOnFire = targetGun.shellsToLaunchOnFire;
            this.shellCasingOnFireFrameDelay = targetGun.shellCasingOnFireFrameDelay;
            this.shellsToLaunchOnReload = targetGun.shellsToLaunchOnReload;
            this.reloadShellLaunchFrame = targetGun.reloadShellLaunchFrame;
            this.clipObject = targetGun.clipObject;
            this.clipsToLaunchOnReload = targetGun.clipsToLaunchOnReload;
            this.reloadClipLaunchFrame = targetGun.reloadClipLaunchFrame;
            this.IsTrickGun = targetGun.IsTrickGun;
            this.TrickGunAlternatesHandedness = targetGun.TrickGunAlternatesHandedness;
            this.alternateVolley = targetGun.alternateVolley;
            this.alternateShootAnimation = targetGun.alternateShootAnimation;
            this.alternateReloadAnimation = targetGun.alternateReloadAnimation;
            this.alternateIdleAnimation = targetGun.alternateIdleAnimation;
            this.alternateSwitchGroup = targetGun.alternateSwitchGroup;
            this.rampBullets = targetGun.rampBullets;
            this.rampStartHeight = targetGun.rampStartHeight;
            this.rampTime = targetGun.rampTime;
            this.usesDirectionalAnimator = targetGun.usesDirectionalAnimator;
            this.usesDirectionalIdleAnimations = targetGun.usesDirectionalIdleAnimations;
            if ((bool) (UnityEngine.Object) this.aiAnimator)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object) this.aiAnimator);
                this.aiAnimator = (AIAnimator) null;
            }
            if ((bool) (UnityEngine.Object) targetGun.aiAnimator)
            {
                AIAnimator aiAnimator1 = this.gameObject.AddComponent<AIAnimator>();
                AIAnimator aiAnimator2 = targetGun.aiAnimator;
                aiAnimator1.facingType = aiAnimator2.facingType;
                aiAnimator1.DirectionParent = aiAnimator2.DirectionParent;
                aiAnimator1.faceSouthWhenStopped = aiAnimator2.faceSouthWhenStopped;
                aiAnimator1.faceTargetWhenStopped = aiAnimator2.faceTargetWhenStopped;
                aiAnimator1.directionalType = aiAnimator2.directionalType;
                aiAnimator1.RotationQuantizeTo = aiAnimator2.RotationQuantizeTo;
                aiAnimator1.RotationOffset = aiAnimator2.RotationOffset;
                aiAnimator1.ForceKillVfxOnPreDeath = aiAnimator2.ForceKillVfxOnPreDeath;
                aiAnimator1.SuppressAnimatorFallback = aiAnimator2.SuppressAnimatorFallback;
                aiAnimator1.IsBodySprite = aiAnimator2.IsBodySprite;
                aiAnimator1.IdleAnimation = aiAnimator2.IdleAnimation;
                aiAnimator1.MoveAnimation = aiAnimator2.MoveAnimation;
                aiAnimator1.FlightAnimation = aiAnimator2.FlightAnimation;
                aiAnimator1.HitAnimation = aiAnimator2.HitAnimation;
                aiAnimator1.OtherAnimations = aiAnimator2.OtherAnimations;
                aiAnimator1.OtherVFX = aiAnimator2.OtherVFX;
                aiAnimator1.OtherScreenShake = aiAnimator2.OtherScreenShake;
                aiAnimator1.IdleFidgetAnimations = aiAnimator2.IdleFidgetAnimations;
                this.aiAnimator = aiAnimator1;
            }
            MultiTemporaryOrbitalSynergyProcessor component1 = targetGun.GetComponent<MultiTemporaryOrbitalSynergyProcessor>();
            MultiTemporaryOrbitalSynergyProcessor component2 = this.GetComponent<MultiTemporaryOrbitalSynergyProcessor>();
            if (!(bool) (UnityEngine.Object) component1 && (bool) (UnityEngine.Object) component2)
                UnityEngine.Object.Destroy((UnityEngine.Object) component2);
            else if ((bool) (UnityEngine.Object) component1 && !(bool) (UnityEngine.Object) component2)
            {
                MultiTemporaryOrbitalSynergyProcessor synergyProcessor = this.gameObject.AddComponent<MultiTemporaryOrbitalSynergyProcessor>();
                synergyProcessor.RequiredSynergy = component1.RequiredSynergy;
                synergyProcessor.OrbitalPrefab = component1.OrbitalPrefab;
            }
            if ((UnityEngine.Object) this.rawVolley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.rawVolley.projectiles.Count; ++index)
                    this.rawVolley.projectiles[index].ResetRuntimeData();
            }
            else
                this.singleModule.ResetRuntimeData();
            if ((UnityEngine.Object) volley != (UnityEngine.Object) null)
                this.RawSourceVolley = DuctTapeItem.TransferDuctTapeModules(volley, this.RawSourceVolley, this);
            if (this.m_owner is PlayerController)
            {
                PlayerController owner = this.m_owner as PlayerController;
                if ((UnityEngine.Object) owner.stats != (UnityEngine.Object) null)
                    owner.stats.RecalculateStats(owner);
            }
            if (this.gameObject.activeSelf)
                this.StartCoroutine(this.HandleFrameDelayedTransformation());
            this.DidTransformGunThisFrame = true;
        }

        [DebuggerHidden]
        private IEnumerator HandleFrameDelayedTransformation()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Gun__HandleFrameDelayedTransformationc__Iterator0()
            {
                _this = this
            };
        }

        public void Initialize(GameActor owner)
        {
            if (!(bool) (UnityEngine.Object) this.m_sprite)
                this.Awake();
            this.m_owner = owner;
            this.m_transform = this.transform;
            this.m_attachTransform = this.transform.parent;
            this.m_anim = this.GetComponent<tk2dSpriteAnimator>();
            this.m_anim.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
            this.m_sprite.automaticallyManagesDepth = false;
            this.m_sprite.IsPerpendicular = !this.forceFlat;
            this.m_sprite.independentOrientation = true;
            if (this.forceFlat)
            {
                owner.sprite.AttachRenderer(this.m_sprite);
                this.m_sprite.HeightOffGround = 0.25f;
                this.m_sprite.UpdateZDepth();
            }
            this.m_moduleData = new Dictionary<ProjectileModule, ModuleShootData>();
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    ModuleShootData moduleShootData = new ModuleShootData();
                    if (this.ammo < this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner))
                        moduleShootData.numberShotsFired = this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                    this.m_moduleData.Add(this.Volley.projectiles[index], moduleShootData);
                }
            }
            else
            {
                ModuleShootData moduleShootData = new ModuleShootData();
                if (this.ammo < this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner))
                    moduleShootData.numberShotsFired = this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                this.m_moduleData.Add(this.singleModule, moduleShootData);
            }
            if ((bool) (UnityEngine.Object) this.modifiedFinalVolley)
            {
                for (int index = 0; index < this.modifiedFinalVolley.projectiles.Count; ++index)
                {
                    ModuleShootData moduleShootData = new ModuleShootData();
                    if (this.ammo < this.modifiedFinalVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner))
                        moduleShootData.numberShotsFired = this.modifiedFinalVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                    this.m_moduleData.Add(this.modifiedFinalVolley.projectiles[index], moduleShootData);
                }
            }
            if ((bool) (UnityEngine.Object) this.modifiedOptionalReloadVolley)
            {
                for (int index = 0; index < this.modifiedOptionalReloadVolley.projectiles.Count; ++index)
                {
                    ModuleShootData moduleShootData = new ModuleShootData();
                    if (this.ammo < this.modifiedOptionalReloadVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner))
                        moduleShootData.numberShotsFired = this.modifiedOptionalReloadVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                    this.m_moduleData.Add(this.modifiedOptionalReloadVolley.projectiles[index], moduleShootData);
                }
            }
            if ((UnityEngine.Object) this.procGunData != (UnityEngine.Object) null)
                this.ApplyProcGunData(this.procGunData);
            if (this.m_childTransformsToFlip == null)
                this.m_childTransformsToFlip = new List<Transform>();
            for (int index = 0; index < this.m_transform.childCount; ++index)
            {
                Transform child = this.m_transform.GetChild(index);
                if ((UnityEngine.Object) child.GetComponent<Light>() != (UnityEngine.Object) null)
                    this.m_childTransformsToFlip.Add(child);
            }
            tk2dSpriteDefinition.AttachPoint[] attachPoints = this.m_sprite.Collection.GetAttachPoints(this.m_defaultSpriteID);
            this.m_defaultLocalPosition = -(attachPoints == null ? (tk2dSpriteDefinition.AttachPoint) null : Array.Find<tk2dSpriteDefinition.AttachPoint>(attachPoints, (Predicate<tk2dSpriteDefinition.AttachPoint>) (a => a.name == "PrimaryHand"))).position;
            if (this.AppliesHoming)
                this.PostProcessProjectile += new Action<Projectile>(this.ApplyHomingToProjectile);
            if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
                this.m_owner.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.PostRigidbodyMovement);
            if (this.OnInitializedWithOwner == null)
                return;
            this.OnInitializedWithOwner(this.m_owner);
        }

        public void UpdateAttachTransform() => this.m_attachTransform = this.transform.parent;

        private void ApplyHomingToProjectile(Projectile obj)
        {
            if (obj is HomingProjectile)
                return;
            HomingModifier component = obj.GetComponent<HomingModifier>();
            if ((bool) (UnityEngine.Object) component)
            {
                component.AngularVelocity = Mathf.Max(component.AngularVelocity, this.AppliedHomingAngularVelocity);
                component.HomingRadius = Mathf.Max(component.HomingRadius, this.AppliedHomingDetectRadius);
            }
            else
            {
                HomingModifier homingModifier = obj.gameObject.AddComponent<HomingModifier>();
                homingModifier.AngularVelocity = this.AppliedHomingAngularVelocity;
                homingModifier.HomingRadius = this.AppliedHomingDetectRadius;
            }
        }

        private void InitializeDefaultFrame()
        {
            if (this.m_defaultSpriteID != 0)
                return;
            PickupObject byId = PickupObjectDatabase.Instance.InternalGetById(this.PickupObjectId);
            if ((UnityEngine.Object) byId != (UnityEngine.Object) null)
                this.m_defaultSpriteID = byId.sprite.spriteId;
            else
                this.m_defaultSpriteID = this.sprite.spriteId;
        }

        public void ReinitializeModuleData(ProjectileVolleyData originalSourceVolley)
        {
            Dictionary<ProjectileModule, ModuleShootData> moduleData = this.m_moduleData;
            if (this.m_moduleData == null)
                this.m_moduleData = new Dictionary<ProjectileModule, ModuleShootData>();
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    ProjectileModule projectile = this.Volley.projectiles[index];
                    if (!this.m_moduleData.ContainsKey(projectile))
                    {
                        ModuleShootData moduleShootData1 = new ModuleShootData();
                        if (this.ammo < projectile.GetModNumberOfShotsInClip(this.CurrentOwner))
                            moduleShootData1.numberShotsFired = projectile.GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                        ModuleShootData moduleShootData2;
                        if (moduleData != null && (UnityEngine.Object) originalSourceVolley != (UnityEngine.Object) null && originalSourceVolley.projectiles != null && index < originalSourceVolley.projectiles.Count && moduleData.TryGetValue(originalSourceVolley.projectiles[index], out moduleShootData2) && (bool) (UnityEngine.Object) moduleShootData2.beam)
                        {
                            moduleShootData1.alternateAngleSign = moduleShootData2.alternateAngleSign;
                            moduleShootData1.beam = moduleShootData2.beam;
                            moduleShootData1.beamKnockbackID = moduleShootData2.beamKnockbackID;
                            moduleShootData1.angleForShot = moduleShootData2.angleForShot;
                            this.m_activeBeams.Remove(moduleShootData2);
                            this.m_activeBeams.Add(moduleShootData1);
                        }
                        this.m_moduleData.Add(projectile, moduleShootData1);
                    }
                }
            }
            else
            {
                ModuleShootData moduleShootData = new ModuleShootData();
                if (this.ammo < this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner))
                    moduleShootData.numberShotsFired = this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                this.m_moduleData.Add(this.singleModule, moduleShootData);
            }
            if ((bool) (UnityEngine.Object) this.modifiedFinalVolley)
            {
                for (int index = 0; index < this.modifiedFinalVolley.projectiles.Count; ++index)
                {
                    if (!this.m_moduleData.ContainsKey(this.modifiedFinalVolley.projectiles[index]))
                    {
                        ModuleShootData moduleShootData = new ModuleShootData();
                        if (this.ammo < this.modifiedFinalVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner))
                            moduleShootData.numberShotsFired = this.modifiedFinalVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                        this.m_moduleData.Add(this.modifiedFinalVolley.projectiles[index], moduleShootData);
                    }
                }
            }
            if ((bool) (UnityEngine.Object) this.modifiedOptionalReloadVolley)
            {
                for (int index = 0; index < this.modifiedOptionalReloadVolley.projectiles.Count; ++index)
                {
                    if (!this.m_moduleData.ContainsKey(this.modifiedOptionalReloadVolley.projectiles[index]))
                    {
                        ModuleShootData moduleShootData = new ModuleShootData();
                        if (this.ammo < this.modifiedOptionalReloadVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner))
                            moduleShootData.numberShotsFired = this.modifiedOptionalReloadVolley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo;
                        this.m_moduleData.Add(this.modifiedOptionalReloadVolley.projectiles[index], moduleShootData);
                    }
                }
            }
            if (!((UnityEngine.Object) originalSourceVolley != (UnityEngine.Object) null))
                return;
            for (int index1 = 0; index1 < originalSourceVolley.projectiles.Count; ++index1)
            {
                if (!string.IsNullOrEmpty(originalSourceVolley.projectiles[index1].runtimeGuid) && moduleData.ContainsKey(originalSourceVolley.projectiles[index1]))
                {
                    for (int index2 = 0; index2 < this.Volley.projectiles.Count; ++index2)
                    {
                        if (originalSourceVolley.projectiles[index1].runtimeGuid == this.Volley.projectiles[index2].runtimeGuid)
                        {
                            this.m_activeBeams.Remove(this.m_moduleData[this.Volley.projectiles[index2]]);
                            this.m_activeBeams.Add(moduleData[originalSourceVolley.projectiles[index1]]);
                            this.m_moduleData[this.Volley.projectiles[index2]] = moduleData[originalSourceVolley.projectiles[index1]];
                        }
                    }
                }
            }
        }

        public void Awake()
        {
            this.m_sprite = (tk2dBaseSprite) this.GetComponent<tk2dSprite>();
            this.AwakeAudio();
            this.m_clipLaunchAttachPoint = this.transform.Find("Clip");
            this.m_casingLaunchAttachPoint = this.transform.Find("Casing");
            this.m_localAttachPoint = this.transform.Find("PrimaryHand");
            this.m_offhandAttachPoint = this.transform.Find("SecondaryHand");
            this.m_meshRenderer = this.transform.GetComponent<MeshRenderer>();
            if (!string.IsNullOrEmpty(this.gunSwitchGroup))
            {
                int num = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.gunSwitchGroup, this.gameObject);
            }
            this.InitializeDefaultFrame();
            if ((UnityEngine.Object) this.rawVolley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.rawVolley.projectiles.Count; ++index)
                    this.rawVolley.projectiles[index].ResetRuntimeData();
            }
            else
                this.singleModule.ResetRuntimeData();
            if ((UnityEngine.Object) this.alternateVolley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.alternateVolley.projectiles.Count; ++index)
                    this.alternateVolley.projectiles[index].ResetRuntimeData();
            }
            if ((bool) (UnityEngine.Object) this.barrelOffset)
                this.m_originalBarrelOffsetPosition = this.barrelOffset.localPosition;
            if ((bool) (UnityEngine.Object) this.muzzleOffset)
                this.m_originalMuzzleOffsetPosition = this.muzzleOffset.localPosition;
            if ((bool) (UnityEngine.Object) this.chargeOffset)
                this.m_originalChargeOffsetPosition = this.chargeOffset.localPosition;
            this.weaponPanelSpriteOverride = this.GetComponent<GunWeaponPanelSpriteOverride>();
        }

        private void AwakeAudio()
        {
            AkGameObj akGameObj = this.GetComponent<AkGameObj>();
            if (!(bool) (UnityEngine.Object) akGameObj)
                akGameObj = this.gameObject.AddComponent<AkGameObj>();
            int num = (int) akGameObj.Register();
        }

        public void OnEnable()
        {
            if (this.m_isThrown)
                return;
            if (!this.NoOwnerOverride && !this.m_isThrown && ((UnityEngine.Object) this.m_owner == (UnityEngine.Object) null || (UnityEngine.Object) this.m_owner.CurrentGun != (UnityEngine.Object) this) && (!(this.m_owner is PlayerController) || (this.m_owner as PlayerController).inventory == null || !((UnityEngine.Object) (this.m_owner as PlayerController).CurrentSecondaryGun == (UnityEngine.Object) this)))
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                if (!this.NoOwnerOverride)
                    this.HandleSpriteFlip(this.m_owner.SpriteFlipped);
                if (!(bool) (UnityEngine.Object) this.m_owner)
                    return;
                this.gameObject.GetOrAddComponent<AkGameObj>();
                this.m_transform.localPosition = BraveUtility.QuantizeVector(this.m_transform.localPosition, 16f);
                this.m_transform.localRotation = Quaternion.identity;
                if (this.ClearsCooldownsLikeAWP)
                    this.ClearCooldowns();
                this.m_isReloading = false;
                this.m_reloadWhenDoneFiring = false;
                if (!this.m_isThrown)
                {
                    if (!string.IsNullOrEmpty(this.introAnimation) && this.m_anim.GetClipByName(this.introAnimation) != null)
                        this.Play(this.introAnimation);
                    else
                        this.PlayIdleAnimation();
                }
                this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
            }
        }

        public void Update()
        {
            this.m_isCritting = false;
            if ((double) this.HeroSwordCooldown > 0.0)
                this.HeroSwordCooldown -= BraveTime.DeltaTime;
            if ((UnityEngine.Object) this.m_owner == (UnityEngine.Object) null)
            {
                this.HandlePickupCurseParticles();
                if (!this.m_isBeingEyedByRat && UnityEngine.Time.frameCount % 50 == 0 && this.ShouldBeTakenByRat(this.sprite.WorldCenter))
                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleRatTheft());
            }
            else if (this.UsesRechargeLikeActiveItem && this.m_owner is PlayerController && (this.m_owner as PlayerController).CharacterUsesRandomGuns)
                this.RemainingActiveCooldownAmount = Mathf.Max(0.0f, this.m_remainingActiveCooldownAmount - 25f * BraveTime.DeltaTime);
            if (this.m_reloadWhenDoneFiring && (string.IsNullOrEmpty(this.shootAnimation) || !this.spriteAnimator.IsPlaying(this.shootAnimation)) && (string.IsNullOrEmpty(this.finalShootAnimation) || !this.spriteAnimator.IsPlaying(this.finalShootAnimation)) && (string.IsNullOrEmpty(this.criticalFireAnimation) || !this.spriteAnimator.IsPlaying(this.criticalFireAnimation)))
            {
                this.Reload();
                if (this.OnReloadPressed != null)
                    this.OnReloadPressed(this.CurrentOwner as PlayerController, this, false);
            }
            if (this.m_continueBurstInUpdate)
            {
                this.ContinueAttack();
                if (!this.m_midBurstFire)
                    this.CeaseAttack(false);
            }
            if (this.m_owner is PlayerController && (bool) (UnityEngine.Object) this.m_sprite && this.m_sprite.FlipX)
            {
                tk2dSprite[] outlineSprites = SpriteOutlineManager.GetOutlineSprites(this.m_sprite);
                if (outlineSprites != null)
                {
                    for (int index = 0; index < outlineSprites.Length; ++index)
                    {
                        if ((bool) (UnityEngine.Object) outlineSprites[index])
                            outlineSprites[index].scale = this.m_sprite.scale;
                    }
                }
            }
            if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null && (UnityEngine.Object) this.m_instanceMinimapIcon != (UnityEngine.Object) null)
                this.GetRidOfMinimapIcon();
            if (this.IsReloading && this.blankDuringReload)
            {
                this.m_reloadElapsed += BraveTime.DeltaTime;
                if ((UnityEngine.Object) this.spriteAnimator == (UnityEngine.Object) null || this.spriteAnimator.IsPlaying(this.reloadAnimation))
                {
                    Vector2 unitCenter = this.m_owner.specRigidbody.GetUnitCenter(ColliderType.HitBox);
                    if (this.reflectDuringReload)
                    {
                        float damageModifier = 1f;
                        if (this.OnReflectedBulletDamageModifier != null)
                            damageModifier = this.OnReflectedBulletDamageModifier(damageModifier);
                        float scaleModifier = 1f;
                        if (this.OnReflectedBulletScaleModifier != null)
                            scaleModifier = this.OnReflectedBulletScaleModifier(scaleModifier);
                        PassiveReflectItem.ReflectBulletsInRange(unitCenter, this.blankReloadRadius, true, this.m_owner, 10f, scaleModifier, damageModifier);
                    }
                    else
                        SilencerInstance.DestroyBulletsInRange(unitCenter, this.blankReloadRadius, true, false);
                    float num = this.blankDamageToEnemies;
                    if ((double) this.blankDamageScalingOnEmptyClip > 1.0)
                    {
                        float t = Mathf.Clamp01(1f - (float) this.ClipShotsRemaining / (float) this.ClipCapacity);
                        num = Mathf.Lerp(num, num * this.blankDamageScalingOnEmptyClip, t);
                    }
                    if ((double) num > 0.0)
                    {
                        if ((double) this.m_reloadElapsed > 0.125 && !this.m_hasDoneSingleReloadBlankEffect)
                        {
                            this.m_hasDoneSingleReloadBlankEffect = true;
                            Vector2 arcOrigin = this.PrimaryHandAttachPoint.position.XY();
                            float arcRadius = this.blankReloadRadius * 2f;
                            float arcAngle = 45f;
                            this.DealDamageToEnemiesInArc(arcOrigin, arcAngle, arcRadius, num, this.blankKnockbackPower);
                        }
                    }
                    else
                        Exploder.DoRadialKnockback((Vector3) unitCenter, this.blankKnockbackPower, this.blankReloadRadius + 1.25f);
                }
            }
            if (this.m_isPreppedForThrow)
            {
                bool flag1 = (double) this.m_prepThrowTime < 1.2000000476837158;
                bool flag2 = (double) this.m_prepThrowTime < 0.0;
                this.m_prepThrowTime += BraveTime.DeltaTime;
                PlayerController currentOwner = this.CurrentOwner as PlayerController;
                if ((double) this.m_prepThrowTime < 1.2000000476837158)
                {
                    this.HandleSpriteFlip(this.m_sprite.FlipY);
                    if ((bool) (UnityEngine.Object) currentOwner)
                        currentOwner.DoSustainedVibration(Vibration.Strength.UltraLight);
                }
                else
                {
                    if (flag1)
                        this.DoChargeCompletePoof();
                    if ((bool) (UnityEngine.Object) currentOwner)
                        currentOwner.DoSustainedVibration(Vibration.Strength.Light);
                }
                if (flag2 && (double) this.m_prepThrowTime >= 0.0)
                    currentOwner.ProcessHandAttachment();
            }
            if (!this.m_isThrown || !this.m_sprite.FlipY)
                return;
            this.m_sprite.FlipY = false;
        }

        public void OnWillRenderObject()
        {
            if (!Pixelator.IsRenderingReflectionMap)
                return;
            Bounds bounds = this.sprite.GetBounds();
            float num1 = bounds.min.y * 2f;
            if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null && (UnityEngine.Object) this.m_owner.CurrentGun == (UnityEngine.Object) this)
            {
                bool flipY = this.sprite.FlipY;
                int num2 = !flipY ? 1 : -1;
                if (flipY)
                    num1 += 2f * bounds.size.y;
                float a = 0.0f;
                float t = (float) (1.0 - (double) Mathf.Abs((float) (90.0 - ((double) this.gunAngle + 540.0) % 180.0)) / 90.0);
                if ((UnityEngine.Object) this.CurrentOwner != (UnityEngine.Object) null)
                    a = (float) (-1 * num2) * (this.transform.position.y - this.CurrentOwner.transform.position.y);
                float num3 = Mathf.Lerp(a, (float) num2 * bounds.size.y, t) + (float) (-3.0 / 16.0 * (double) num2 * (1.0 - (double) t));
                num1 += num3;
            }
            this.sprite.renderer.material.SetFloat("_ReflectionYOffset", num1);
        }

        public void OnDisable()
        {
            if (this.m_activeBeams.Count > 0 && this.doesScreenShake && (UnityEngine.Object) GameManager.Instance.MainCameraController != (UnityEngine.Object) null)
                GameManager.Instance.MainCameraController.StopContinuousScreenShake((Component) this);
            if ((bool) (UnityEngine.Object) this.m_extantLockOnSprite)
                SpawnManager.Despawn(this.m_extantLockOnSprite);
            this.DespawnVFX();
            this.sprite.SetSprite(this.m_defaultSpriteID);
            this.m_reloadWhenDoneFiring = false;
        }

        protected override void OnDestroy()
        {
            if (Minimap.HasInstance)
                this.GetRidOfMinimapIcon();
            base.OnDestroy();
        }

        public void ToggleRenderers(bool value)
        {
            this.m_meshRenderer.enabled = value;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_sprite, value);
            if ((bool) (UnityEngine.Object) this.m_extantLaserSight)
                this.m_extantLaserSight.renderer.enabled = value;
            if (this.m_currentlyPlayingChargeVFX == null)
                return;
            this.m_currentlyPlayingChargeVFX.ToggleRenderers(value);
            if (this.DefaultModule == null || !this.m_moduleData.ContainsKey(this.DefaultModule))
                return;
            ModuleShootData moduleShootData = this.m_moduleData[this.DefaultModule];
            if (moduleShootData == null || moduleShootData.lastChargeProjectile == null)
                return;
            this.TogglePreviousChargeEffectsIfNecessary(moduleShootData.lastChargeProjectile, value);
        }

        public tk2dBaseSprite GetSprite()
        {
            if ((UnityEngine.Object) this.m_sprite == (UnityEngine.Object) null)
                this.m_sprite = (tk2dBaseSprite) this.GetComponent<tk2dSprite>();
            return this.m_sprite;
        }

        public void DespawnVFX()
        {
            if ((UnityEngine.Object) this.m_extantLaserSight != (UnityEngine.Object) null)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantLaserSight.gameObject);
                this.m_extantLaserSight = (tk2dTiledSprite) null;
            }
            this.muzzleFlashEffects.DestroyAll();
            this.m_isContinuousMuzzleFlashOut = false;
            this.finalMuzzleFlashEffects.DestroyAll();
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index1 = 0; index1 < this.Volley.projectiles.Count; ++index1)
                {
                    if (this.Volley.projectiles[index1].chargeProjectiles != null)
                    {
                        for (int index2 = 0; index2 < this.Volley.projectiles[index1].chargeProjectiles.Count; ++index2)
                        {
                            if (this.Volley.projectiles[index1].chargeProjectiles[index2].UsesOverrideMuzzleFlashVfxPool)
                                this.Volley.projectiles[index1].chargeProjectiles[index2].OverrideMuzzleFlashVfxPool.DestroyAll();
                        }
                    }
                }
            }
            else if (this.singleModule.chargeProjectiles != null)
            {
                for (int index = 0; index < this.singleModule.chargeProjectiles.Count; ++index)
                {
                    if (this.singleModule.chargeProjectiles[index].UsesOverrideMuzzleFlashVfxPool)
                        this.singleModule.chargeProjectiles[index].OverrideMuzzleFlashVfxPool.DestroyAll();
                }
            }
            this.reloadEffects.DestroyAll();
            this.emptyReloadEffects.DestroyAll();
            this.activeReloadSuccessEffects.DestroyAll();
            this.activeReloadFailedEffects.DestroyAll();
        }

        public void ApplyProcGunData(ProceduralGunData data)
        {
            int randomIntValue = data.ammoData.GetRandomIntValue();
            this.ammo = randomIntValue;
            this.maxAmmo = randomIntValue;
            this.damageModifier = data.damageData.GetRandomIntValue();
            this.gunCooldownModifier = data.cooldownData.GetRandomValue();
        }

        public void HandleSpriteFlip(bool flipped)
        {
            if (this.m_isThrown)
                flipped = false;
            if (this.usesDirectionalIdleAnimations || this.preventRotation)
                flipped = false;
            if (flipped && !this.forceFlat)
            {
                if (!this.m_sprite.FlipY)
                {
                    this.barrelOffset.localPosition = this.m_originalBarrelOffsetPosition.WithY(-this.m_originalBarrelOffsetPosition.y);
                    if ((bool) (UnityEngine.Object) this.muzzleOffset)
                        this.muzzleOffset.localPosition = this.m_originalMuzzleOffsetPosition.WithY(-this.m_originalMuzzleOffsetPosition.y);
                    if ((bool) (UnityEngine.Object) this.chargeOffset)
                        this.chargeOffset.localPosition = this.m_originalChargeOffsetPosition.WithY(-this.m_originalChargeOffsetPosition.y);
                    if ((bool) (UnityEngine.Object) this.reloadOffset)
                        this.reloadOffset.localPosition = this.reloadOffset.localPosition.WithY(-this.reloadOffset.localPosition.y);
                    for (int index = 0; index < this.m_childTransformsToFlip.Count; ++index)
                        this.m_childTransformsToFlip[index].localPosition = this.m_childTransformsToFlip[index].localPosition.WithY(-this.m_childTransformsToFlip[index].localPosition.y);
                    this.m_sprite.FlipY = true;
                }
            }
            else if (this.m_sprite.FlipY)
            {
                this.barrelOffset.localPosition = this.m_originalBarrelOffsetPosition;
                if ((bool) (UnityEngine.Object) this.muzzleOffset)
                    this.muzzleOffset.localPosition = this.m_originalMuzzleOffsetPosition;
                if ((bool) (UnityEngine.Object) this.chargeOffset)
                    this.chargeOffset.localPosition = this.m_originalChargeOffsetPosition;
                if ((bool) (UnityEngine.Object) this.reloadOffset)
                    this.reloadOffset.localPosition = this.reloadOffset.localPosition.WithY(-this.reloadOffset.localPosition.y);
                for (int index = 0; index < this.m_childTransformsToFlip.Count; ++index)
                    this.m_childTransformsToFlip[index].localPosition = this.m_childTransformsToFlip[index].localPosition.WithY(-this.m_childTransformsToFlip[index].localPosition.y);
                this.m_sprite.FlipY = false;
            }
            if (this.m_isPreppedForThrow)
            {
                Vector3 vector3 = this.m_defaultLocalPosition.WithZ(0.0f);
                if (flipped)
                    vector3 = Vector3.Scale(vector3, new Vector3(1f, -1f, 1f));
                this.transform.localPosition = Vector3.Lerp(vector3.WithZ(0.0f), this.ThrowPrepPosition, Mathf.Clamp01(this.m_prepThrowTime / 1.2f));
            }
            else
            {
                this.m_transform.localPosition = this.m_defaultLocalPosition.WithZ(0.0f);
                if (flipped)
                    this.m_transform.localPosition = Vector3.Scale(this.m_transform.localPosition, new Vector3(1f, -1f, 1f));
            }
            this.m_transform.localPosition = BraveUtility.QuantizeVector(this.m_transform.localPosition, 16f);
        }

        private bool ShouldDoLaserSight()
        {
            return !this.m_isPreppedForThrow && !this.m_isReloading && !this.SuppressLaserSight && (this.ForceLaserSight || this.PickupObjectId == GlobalItemIds.ArtfulDodgerChallengeGun || this.CurrentOwner is PlayerController && PassiveItem.ActiveFlagItems.ContainsKey(this.CurrentOwner as PlayerController) && PassiveItem.ActiveFlagItems[this.CurrentOwner as PlayerController].ContainsKey(typeof (LaserSightItem)));
        }

        public float GetChargeFraction()
        {
            bool flag = false;
            float a = 1f;
            if (this.IsFiring)
            {
                if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
                {
                    for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    {
                        ProjectileModule projectile = this.Volley.projectiles[index];
                        if (projectile.shootStyle == ProjectileModule.ShootStyle.Charged)
                        {
                            ModuleShootData moduleShootData = this.m_moduleData[projectile];
                            if ((double) projectile.LongestChargeTime > 0.0)
                            {
                                a = Mathf.Min(a, Mathf.Clamp01(moduleShootData.chargeTime / projectile.LongestChargeTime));
                                flag = true;
                            }
                        }
                    }
                }
                else
                {
                    ProjectileModule singleModule = this.singleModule;
                    if (singleModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                    {
                        ModuleShootData moduleShootData = this.m_moduleData[singleModule];
                        if ((double) singleModule.LongestChargeTime > 0.0)
                        {
                            a = Mathf.Min(a, Mathf.Clamp01(moduleShootData.chargeTime / singleModule.LongestChargeTime));
                            flag = true;
                        }
                    }
                }
            }
            if (!flag)
                a = 0.0f;
            return a;
        }

        public float HandleAimRotation(Vector3 ownerAimPoint, bool limitAimSpeed = false, float aimTimeScale = 1f)
        {
            if (this.m_isThrown)
                return this.prevGunAngleUnmodified;
            Vector2 b;
            if (this.usesDirectionalIdleAnimations)
            {
                (this.m_transform.position + Quaternion.Euler(0.0f, 0.0f, -this.m_attachTransform.localRotation.z) * this.barrelOffset.localPosition).XY();
                b = this.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter;
            }
            else
                b = !this.LockedHorizontalOnCharge ? (this.m_transform.position + Quaternion.Euler(0.0f, 0.0f, this.gunAngle) * Quaternion.Euler(0.0f, 0.0f, -this.m_attachTransform.localRotation.z) * this.barrelOffset.localPosition).XY() : this.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter;
            float t = Mathf.Clamp01((float) (((double) Vector2.Distance(ownerAimPoint.XY(), b) - 2.0) / 3.0));
            Vector2 vector2_1 = Vector2.Lerp(this.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter, b, t);
            this.m_localAimPoint = ownerAimPoint.XY();
            Vector2 vector2_2 = this.m_localAimPoint - vector2_1;
            float num1 = Mathf.Atan2(vector2_2.y, vector2_2.x) * 57.29578f;
            if (this.OverrideAngleSnap.HasValue)
                num1 = BraveMathCollege.QuantizeFloat(num1, this.OverrideAngleSnap.Value);
            if (limitAimSpeed && (double) aimTimeScale != 1.0 || this.m_activeBeams.Count > 0)
            {
                float max = float.MaxValue * BraveTime.DeltaTime * aimTimeScale;
                if (this.m_activeBeams.Count > 0 && (bool) (UnityEngine.Object) this.Volley && this.Volley.UsesBeamRotationLimiter)
                    max = this.Volley.BeamRotationDegreesPerSecond * BraveTime.DeltaTime * aimTimeScale;
                num1 = BraveMathCollege.ClampAngle180(this.prevGunAngleUnmodified + Mathf.Clamp(BraveMathCollege.ClampAngle180(num1 - this.prevGunAngleUnmodified), -max, max));
                this.m_localAimPoint = (this.transform.position + (Quaternion.Euler(0.0f, 0.0f, num1) * Vector3.right).normalized * vector2_2.magnitude).XY();
            }
            this.prevGunAngleUnmodified = num1;
            this.gunAngle = num1;
            this.m_attachTransform.localRotation = Quaternion.Euler(this.m_attachTransform.localRotation.x, this.m_attachTransform.localRotation.y, num1);
            this.m_unroundedBarrelPosition = this.barrelOffset.position;
            float num2 = !this.forceFlat ? (float) (Mathf.RoundToInt(num1 / 10f) * 10) : (float) (Mathf.RoundToInt(num1 / 3f) * 3);
            if (this.IgnoresAngleQuantization)
                num2 = num1;
            bool flag1 = this.sprite.FlipY;
            float num3 = 75f;
            float num4 = 105f;
            if ((double) num2 <= 155.0 && (double) num2 >= 25.0)
            {
                num3 = 75f;
                num4 = 105f;
            }
            if (!this.sprite.FlipY && (double) Mathf.Abs(num2) > (double) num4)
                flag1 = true;
            else if (this.sprite.FlipY && (double) Mathf.Abs(num2) < (double) num3)
                flag1 = false;
            if (this.LockedHorizontalOnCharge)
            {
                float chargeFraction = this.GetChargeFraction();
                this.LockedHorizontalCachedAngle = num1;
                num2 = Mathf.LerpAngle(num2, !flag1 ? 0.0f : 180f, chargeFraction);
            }
            if (this.LockedHorizontalOnReload && this.IsReloading)
                num2 = !flag1 ? 0.0f : 180f;
            if (this.m_isPreppedForThrow)
                num2 = (double) this.m_prepThrowTime >= 1.2000000476837158 ? (float) Mathf.FloorToInt(Mathf.PingPong(this.m_prepThrowTime * 15f, 10f) - 95f) : (float) Mathf.FloorToInt(Mathf.LerpAngle(num2, -90f, Mathf.Clamp01(this.m_prepThrowTime / 1.2f)));
            if (this.preventRotation)
                num2 = 0.0f;
            if (this.usesDirectionalIdleAnimations)
            {
                float message = (float) (BraveMathCollege.AngleToOctant(num2 + 90f) * -45);
                UnityEngine.Debug.Log((object) message);
                this.m_attachTransform.localRotation = Quaternion.Euler(this.m_attachTransform.localRotation.x, this.m_attachTransform.localRotation.y, (float) (((double) num2 + 360.0) % 360.0) - message);
            }
            else
                this.m_attachTransform.localRotation = Quaternion.Euler(this.m_attachTransform.localRotation.x, this.m_attachTransform.localRotation.y, num2);
            if (this.m_currentlyPlayingChargeVFX != null)
                this.UpdateChargeEffectZDepth(vector2_2);
            if ((UnityEngine.Object) this.m_sprite != (UnityEngine.Object) null)
                this.m_sprite.ForceRotationRebuild();
            if (this.ShouldDoLaserSight())
            {
                if ((UnityEngine.Object) this.m_extantLaserSight == (UnityEngine.Object) null)
                {
                    string path = "Global VFX/VFX_LaserSight";
                    if (!(this.m_owner is PlayerController))
                        path = !this.LaserSightIsGreen ? "Global VFX/VFX_LaserSight_Enemy" : "Global VFX/VFX_LaserSight_Enemy_Green";
                    this.m_extantLaserSight = SpawnManager.SpawnVFX((GameObject) BraveResources.Load(path)).GetComponent<tk2dTiledSprite>();
                    this.m_extantLaserSight.IsPerpendicular = false;
                    this.m_extantLaserSight.HeightOffGround = this.CustomLaserSightHeight;
                    this.m_extantLaserSight.renderer.enabled = this.m_meshRenderer.enabled;
                    this.m_extantLaserSight.transform.parent = this.barrelOffset;
                    if (this.m_owner is AIActor)
                        this.m_extantLaserSight.renderer.enabled = false;
                }
                this.m_extantLaserSight.transform.localPosition = Vector3.zero;
                this.m_extantLaserSight.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num1);
                if (this.m_extantLaserSight.renderer.enabled)
                {
                    Func<SpeculativeRigidbody, bool> rigidbodyExcluder = (Func<SpeculativeRigidbody, bool>) (otherRigidbody => (bool) (UnityEngine.Object) otherRigidbody.minorBreakable && !otherRigidbody.minorBreakable.stopsBullets);
                    bool flag2 = false;
                    float a = float.MaxValue;
                    if (this.DoubleWideLaserSight)
                    {
                        int mask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, !(this.m_owner is PlayerController) ? CollisionLayer.PlayerHitBox : CollisionLayer.EnemyHitBox, CollisionLayer.BulletBreakable);
                        Vector2 vector = BraveMathCollege.DegreesToVector(vector2_2.ToAngle() + 90f, 1f / 16f);
                        RaycastResult result;
                        if (PhysicsEngine.Instance.Raycast(this.barrelOffset.position.XY() + vector, vector2_2, this.CustomLaserSightDistance, out result, rayMask: mask, rigidbodyExcluder: rigidbodyExcluder))
                        {
                            flag2 = true;
                            a = Mathf.Min(a, result.Distance);
                        }
                        RaycastResult.Pool.Free(ref result);
                        if (PhysicsEngine.Instance.Raycast(this.barrelOffset.position.XY() - vector, vector2_2, this.CustomLaserSightDistance, out result, rayMask: mask, rigidbodyExcluder: rigidbodyExcluder))
                        {
                            flag2 = true;
                            a = Mathf.Min(a, result.Distance);
                        }
                        RaycastResult.Pool.Free(ref result);
                    }
                    else
                    {
                        int mask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, !(this.m_owner is PlayerController) ? CollisionLayer.PlayerHitBox : CollisionLayer.EnemyHitBox, CollisionLayer.BulletBreakable);
                        RaycastResult result;
                        if (PhysicsEngine.Instance.Raycast(this.barrelOffset.position.XY(), vector2_2, this.CustomLaserSightDistance, out result, rayMask: mask, rigidbodyExcluder: rigidbodyExcluder))
                        {
                            flag2 = true;
                            a = result.Distance;
                            if ((bool) (UnityEngine.Object) result.SpeculativeRigidbody && (bool) (UnityEngine.Object) result.SpeculativeRigidbody.aiActor)
                                this.HandleEnemyHitByLaserSight(result.SpeculativeRigidbody.aiActor);
                        }
                        RaycastResult.Pool.Free(ref result);
                    }
                    this.m_extantLaserSight.dimensions = new Vector2(!flag2 ? 480f : a / (1f / 16f), 1f);
                    this.m_extantLaserSight.ForceRotationRebuild();
                    this.m_extantLaserSight.UpdateZDepth();
                }
            }
            else if ((UnityEngine.Object) this.m_extantLaserSight != (UnityEngine.Object) null)
            {
                SpawnManager.Despawn(this.m_extantLaserSight.gameObject);
                this.m_extantLaserSight = (tk2dTiledSprite) null;
            }
            if (!this.OwnerHasSynergy(CustomSynergyType.PLASMA_LASER) && (bool) (UnityEngine.Object) this.m_extantLockOnSprite)
                SpawnManager.Despawn(this.m_extantLockOnSprite);
            if (this.usesDirectionalAnimator)
            {
                this.aiAnimator.LockFacingDirection = true;
                this.aiAnimator.FacingDirection = num1;
            }
            return num1;
        }

        protected void HandleEnemyHitByLaserSight(AIActor hitEnemy)
        {
            if (!(bool) (UnityEngine.Object) hitEnemy || !((UnityEngine.Object) this.LastLaserSightEnemy != (UnityEngine.Object) hitEnemy) || !this.OwnerHasSynergy(CustomSynergyType.PLASMA_LASER))
                return;
            if ((bool) (UnityEngine.Object) this.m_extantLockOnSprite)
                SpawnManager.Despawn(this.m_extantLockOnSprite);
            this.m_extantLockOnSprite = hitEnemy.PlayEffectOnActor((GameObject) BraveResources.Load("Global VFX/VFX_LockOn"), Vector3.zero, alreadyMiddleCenter: true, useHitbox: true);
            this.LastLaserSightEnemy = hitEnemy;
        }

        protected void UpdateChargeEffectZDepth(Vector2 currentAimDirection)
        {
            this.m_currentlyPlayingChargeVFX.SetHeightOffGround(Mathf.Lerp(1.6f, 0.9f, (float) (((double) currentAimDirection.normalized.y + 1.0) / 2.0)));
        }

        protected void UpdatePerpendicularity(Vector2 gunToAim)
        {
            if (this.forceFlat)
                return;
            if (BraveMathCollege.VectorToQuadrant(gunToAim) == 2)
                this.m_sprite.IsPerpendicular = false;
            else
                this.m_sprite.IsPerpendicular = true;
        }

        protected float DealSwordDamageToEnemy(
            AIActor targetEnemy,
            Vector2 arcOrigin,
            Vector2 contact,
            float angle,
            float overrideDamage = -1f,
            float overrideForce = -1f)
        {
            Projectile currentProjectile = this.DefaultModule.GetCurrentProjectile();
            float damage = (double) overrideDamage <= 0.0 ? currentProjectile.baseData.damage : overrideDamage;
            float force = (double) overrideForce <= 0.0 ? currentProjectile.baseData.force : overrideForce;
            if ((bool) (UnityEngine.Object) targetEnemy.healthHaver)
                targetEnemy.healthHaver.ApplyDamage(damage, contact - arcOrigin, this.m_owner.ActorName);
            if ((bool) (UnityEngine.Object) targetEnemy.knockbackDoer)
                targetEnemy.knockbackDoer.ApplyKnockback(contact - arcOrigin, force);
            currentProjectile.hitEffects.HandleEnemyImpact((Vector3) contact, angle, targetEnemy.transform, Vector2.zero, Vector2.zero, true);
            return damage;
        }

        protected void DealDamageToEnemiesInArc(
            Vector2 arcOrigin,
            float arcAngle,
            float arcRadius,
            float overrideDamage = -1f,
            float overrideForce = -1f,
            List<SpeculativeRigidbody> alreadyHit = null)
        {
            RoomHandler roomHandler = (RoomHandler) null;
            if (this.m_owner is PlayerController)
                roomHandler = ((PlayerController) this.m_owner).CurrentRoom;
            else if (this.m_owner is AIActor)
                roomHandler = ((AIActor) this.m_owner).ParentRoom;
            if (roomHandler == null)
                return;
            List<AIActor> activeEnemies = roomHandler.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies == null)
                return;
            for (int index1 = 0; index1 < activeEnemies.Count; ++index1)
            {
                AIActor targetEnemy = activeEnemies[index1];
                if ((bool) (UnityEngine.Object) targetEnemy && (bool) (UnityEngine.Object) targetEnemy.specRigidbody && targetEnemy.IsNormalEnemy && !targetEnemy.IsGone && (bool) (UnityEngine.Object) targetEnemy.healthHaver && (alreadyHit == null || !alreadyHit.Contains(targetEnemy.specRigidbody)))
                {
                    for (int index2 = 0; index2 < targetEnemy.healthHaver.NumBodyRigidbodies; ++index2)
                    {
                        SpeculativeRigidbody bodyRigidbody = targetEnemy.healthHaver.GetBodyRigidbody(index2);
                        PixelCollider hitboxPixelCollider = bodyRigidbody.HitboxPixelCollider;
                        if (hitboxPixelCollider != null)
                        {
                            Vector2 vector2 = BraveMathCollege.ClosestPointOnRectangle(arcOrigin, hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
                            float dist = Vector2.Distance(vector2, arcOrigin);
                            float target = BraveMathCollege.Atan2Degrees(vector2 - arcOrigin);
                            if ((double) dist < (double) arcRadius && (double) Mathf.DeltaAngle(this.CurrentAngle, target) < (double) arcAngle)
                            {
                                bool flag = true;
                                int mask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable);
                                RaycastResult result;
                                if (PhysicsEngine.Instance.Raycast(arcOrigin, vector2 - arcOrigin, dist, out result, rayMask: mask) && (UnityEngine.Object) result.SpeculativeRigidbody != (UnityEngine.Object) bodyRigidbody)
                                    flag = false;
                                RaycastResult.Pool.Free(ref result);
                                if (flag)
                                {
                                    float enemy = this.DealSwordDamageToEnemy(targetEnemy, arcOrigin, vector2, arcAngle, overrideDamage, overrideForce);
                                    if (alreadyHit != null)
                                    {
                                        if (alreadyHit.Count == 0)
                                            StickyFrictionManager.Instance.RegisterSwordDamageStickyFriction(enemy);
                                        alreadyHit.Add(targetEnemy.specRigidbody);
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void HandleHeroSwordSlash(
            List<SpeculativeRigidbody> alreadyHit,
            Vector2 arcOrigin,
            int slashId)
        {
            float arcRadius = (this.m_casingLaunchAttachPoint.position.XY() - this.PrimaryHandAttachPoint.position.XY()).magnitude * 1.85f;
            float arcAngle = 45f;
            float num1 = arcRadius * arcRadius;
            if (this.HeroSwordDoesntBlank)
            {
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                for (int index = allProjectiles.Count - 1; index >= 0; --index)
                {
                    Projectile p = allProjectiles[index];
                    if ((bool) (UnityEngine.Object) p && !(p.Owner is PlayerController) && p.IsReflectedBySword && p.LastReflectedSlashId != slashId && (!(p.Owner is AIActor) || (p.Owner as AIActor).IsNormalEnemy))
                    {
                        Vector2 worldCenter = p.sprite.WorldCenter;
                        if ((double) Vector2.SqrMagnitude(worldCenter - arcOrigin) < (double) num1 && (double) Mathf.DeltaAngle(this.CurrentAngle, BraveMathCollege.Atan2Degrees(worldCenter - arcOrigin)) < (double) arcAngle)
                        {
                            PassiveReflectItem.ReflectBullet(p, true, this.m_owner, 2f);
                            p.LastReflectedSlashId = slashId;
                        }
                    }
                }
            }
            else
            {
                ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                for (int index = allProjectiles.Count - 1; index >= 0; --index)
                {
                    Projectile projectile = allProjectiles[index];
                    if ((bool) (UnityEngine.Object) projectile && (!(projectile.Owner is PlayerController) || projectile.ForcePlayerBlankable) && (!(projectile.Owner is AIActor) || (projectile.Owner as AIActor).IsNormalEnemy))
                    {
                        Vector2 worldCenter = projectile.sprite.WorldCenter;
                        if ((double) Vector2.SqrMagnitude(worldCenter - arcOrigin) < (double) num1 && (double) Mathf.DeltaAngle(this.CurrentAngle, BraveMathCollege.Atan2Degrees(worldCenter - arcOrigin)) < (double) arcAngle)
                            projectile.DieInAir(killedEarly: true);
                    }
                }
            }
            this.DealDamageToEnemiesInArc(arcOrigin, arcAngle, arcRadius, alreadyHit: alreadyHit);
            float damage1 = this.DefaultModule.GetCurrentProjectile().baseData.damage;
            float num2 = arcRadius * arcRadius;
            List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
            for (int index = allMinorBreakables.Count - 1; index >= 0; --index)
            {
                MinorBreakable minorBreakable = allMinorBreakables[index];
                if ((bool) (UnityEngine.Object) minorBreakable && (bool) (UnityEngine.Object) minorBreakable.specRigidbody && !minorBreakable.IsBroken && (bool) (UnityEngine.Object) minorBreakable.sprite && (double) (minorBreakable.sprite.WorldCenter - arcOrigin).sqrMagnitude < (double) num2)
                    minorBreakable.Break();
            }
            List<MajorBreakable> allMajorBreakables = StaticReferenceManager.AllMajorBreakables;
            for (int index = allMajorBreakables.Count - 1; index >= 0; --index)
            {
                MajorBreakable majorBreakable = allMajorBreakables[index];
                if ((bool) (UnityEngine.Object) majorBreakable && (bool) (UnityEngine.Object) majorBreakable.specRigidbody && !alreadyHit.Contains(majorBreakable.specRigidbody) && !majorBreakable.IsSecretDoor && !majorBreakable.IsDestroyed && (double) (majorBreakable.specRigidbody.UnitCenter - arcOrigin).sqrMagnitude <= (double) num2)
                {
                    float damage2 = damage1;
                    if ((bool) (UnityEngine.Object) majorBreakable.healthHaver)
                        damage2 *= 0.2f;
                    majorBreakable.ApplyDamage(damage2, majorBreakable.specRigidbody.UnitCenter - arcOrigin, false);
                    alreadyHit.Add(majorBreakable.specRigidbody);
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleSlash()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Gun__HandleSlashc__Iterator1()
            {
                _this = this
            };
        }

        public bool IsGunBlocked()
        {
            if (this.RequiresFundsToShoot && this.m_owner is PlayerController && (this.m_owner as PlayerController).carriedConsumables.Currency < this.CurrencyCostPerShot)
                return true;
            bool flag1 = false;
            Vector2 unitCenter = this.m_owner.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            Vector2 vector2_1 = this.barrelOffset.transform.position.XY();
            Vector2 vector2_2 = vector2_1 - unitCenter;
            float magnitude = vector2_2.magnitude;
            int mask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable);
            if ((bool) (UnityEngine.Object) this.m_owner && !(this.m_owner is PlayerController))
                mask |= CollisionMask.LayerToMask(CollisionLayer.EnemyBulletBlocker);
            SpeculativeRigidbody result1;
            if (PhysicsEngine.Instance.Pointcast(vector2_1, out result1, false, true, CollisionMask.LayerToMask(CollisionLayer.BeamBlocker), new CollisionLayer?(), false) && (bool) (UnityEngine.Object) result1.ultraFortunesFavor)
            {
                result1.ultraFortunesFavor.HitFromPoint(vector2_1);
                return true;
            }
            if (this.CanAttackThroughObjects)
                return false;
            PhysicsEngine.Instance.Pointcast(unitCenter, out result1, false, true, mask, new CollisionLayer?(), false, this.m_owner.specRigidbody);
            bool flag2 = false;
            if ((UnityEngine.Object) this.Volley == (UnityEngine.Object) null && this.singleModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                flag2 = true;
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null && this.Volley.projectiles.Count == 1 && this.Volley.projectiles[0].shootStyle == ProjectileModule.ShootStyle.Charged)
                flag2 = true;
            int num = 100;
            RaycastResult result2;
            while (PhysicsEngine.Instance.Raycast(unitCenter, vector2_2, magnitude, out result2, rayMask: mask, ignoreRigidbody: result1))
            {
                --num;
                if (num <= 0)
                {
                    flag1 = true;
                    break;
                }
                SpeculativeRigidbody speculativeRigidbody = result2.SpeculativeRigidbody;
                RaycastResult.Pool.Free(ref result2);
                if ((UnityEngine.Object) speculativeRigidbody != (UnityEngine.Object) null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if ((UnityEngine.Object) component != (UnityEngine.Object) null && (!flag2 || this.m_currentlyPlayingChargeVFX != null) && !component.OnlyBrokenByCode)
                    {
                        component.Break(vector2_2.normalized * 3f);
                        continue;
                    }
                }
                if (GameManager.Instance.InTutorial && (UnityEngine.Object) speculativeRigidbody != (UnityEngine.Object) null && (bool) (UnityEngine.Object) speculativeRigidbody.GetComponent<Chest>() && (bool) (UnityEngine.Object) speculativeRigidbody.majorBreakable)
                {
                    speculativeRigidbody.majorBreakable.Break(vector2_2);
                }
                else
                {
                    flag1 = true;
                    break;
                }
            }
            return flag1;
        }

        public void ForceThrowGun() => this.ThrowGun();

        public DebrisObject DropGun(float dropHeight = 0.5f)
        {
            this.m_isThrown = true;
            this.m_thrownOnGround = true;
            if ((UnityEngine.Object) this.m_sprite == (UnityEngine.Object) null)
                this.m_sprite = this.sprite;
            this.gameObject.SetActive(true);
            this.m_owner = (GameActor) null;
            Vector3 position = this.transform.position;
            if ((bool) (UnityEngine.Object) this.PrimaryHandAttachPoint)
                position = this.PrimaryHandAttachPoint.position;
            GameObject gameObject = SpawnManager.SpawnProjectile("ThrownGunProjectile", position, Quaternion.identity);
            Projectile component = gameObject.GetComponent<Projectile>();
            this.LastProjectile = component;
            component.Shooter = !((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null) ? (SpeculativeRigidbody) null : this.m_owner.specRigidbody;
            component.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
            component.shouldRotate = false;
            if ((bool) (UnityEngine.Object) component)
            {
                TrailRenderer componentInChildren = component.GetComponentInChildren<TrailRenderer>();
                if ((bool) (UnityEngine.Object) componentInChildren)
                    UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren);
            }
            gameObject.GetComponent<SpeculativeRigidbody>().sprite = this.sprite;
            this.transform.parent = gameObject.transform;
            if (this.m_sprite.FlipY)
                this.HandleSpriteFlip(false);
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            if ((bool) (UnityEngine.Object) this.PrimaryHandAttachPoint)
                this.transform.localPosition -= this.PrimaryHandAttachPoint.localPosition;
            if (this.m_defaultSpriteID >= 0)
            {
                this.spriteAnimator.StopAndResetFrame();
                this.m_sprite.SetSprite(this.m_defaultSpriteID);
            }
            if (!RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
                RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) this);
            DebrisObject debrisObject = component.BecomeDebris(Vector3.zero, dropHeight);
            debrisObject.Priority = EphemeralObject.EphemeralPriority.Critical;
            debrisObject.FlagAsPickup();
            debrisObject.inertialMass = 10f;
            debrisObject.canRotate = false;
            UnityEngine.Object.Destroy((UnityEngine.Object) component.GetComponentInChildren<SimpleSpriteRotator>());
            UnityEngine.Object.Destroy((UnityEngine.Object) component);
            component.ForceDestruction();
            SpriteOutlineManager.AddOutlineToSprite(this.m_sprite, Color.black, 0.1f, 0.05f);
            this.m_sprite.ForceRotationRebuild();
            if ((UnityEngine.Object) this.m_anim != (UnityEngine.Object) null)
                this.PlayIdleAnimation();
            if (this.OnDropped != null)
                this.OnDropped();
            this.RegisterMinimapIcon();
            return debrisObject;
        }

        public void PrepGunForThrow()
        {
            if (!this.m_isPreppedForThrow && this.CurrentOwner is PlayerController)
            {
                this.m_isPreppedForThrow = true;
                this.m_prepThrowTime = -0.3f;
                this.HandleSpriteFlip(this.m_sprite.FlipY);
                (this.CurrentOwner as PlayerController).ProcessHandAttachment();
            }
            int num = (int) AkSoundEngine.PostEvent("Play_BOSS_doormimic_turn_01", this.gameObject);
        }

        public void UnprepGunForThrow()
        {
            if (this.m_isPreppedForThrow)
            {
                this.m_isPreppedForThrow = false;
                this.m_prepThrowTime = -0.3f;
                this.HandleSpriteFlip(this.m_sprite.FlipY);
                (this.CurrentOwner as PlayerController).ProcessHandAttachment();
            }
            int num = (int) AkSoundEngine.PostEvent("Stop_BOSS_doormimic_turn_01", this.gameObject);
        }

        private void ThrowGun()
        {
            this.m_isThrown = true;
            this.m_thrownOnGround = false;
            this.gameObject.SetActive(true);
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", this.gameObject);
            Vector3 vector3 = this.ThrowPrepTransform.parent.TransformPoint((this.ThrowPrepPosition * -1f).WithX(0.0f));
            Vector2 vector2 = this.m_localAimPoint - vector3.XY();
            float z = BraveMathCollege.Atan2Degrees(vector2);
            GameObject gameObject = SpawnManager.SpawnProjectile("ThrownGunProjectile", vector3, Quaternion.Euler(0.0f, 0.0f, z));
            Projectile component1 = gameObject.GetComponent<Projectile>();
            component1.Shooter = this.m_owner.specRigidbody;
            component1.DestroyMode = Projectile.ProjectileDestroyMode.BecomeDebris;
            component1.baseData.damage *= (this.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ThrownGunDamage);
            SpeculativeRigidbody component2 = gameObject.GetComponent<SpeculativeRigidbody>();
            component2.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTrigger);
            component2.sprite = this.sprite;
            this.m_sprite.scale = Vector3.one;
            this.transform.parent = gameObject.transform;
            this.transform.localRotation = Quaternion.identity;
            this.m_sprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
            if (this.m_sprite.FlipY)
                this.transform.localPosition = Vector3.Scale(new Vector3(-1f, 1f, 1f), this.transform.localPosition);
            Bounds bounds = this.sprite.GetBounds();
            component2.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            component2.PrimaryPixelCollider.ManualOffsetX = -Mathf.RoundToInt(bounds.extents.x / (1f / 16f));
            component2.PrimaryPixelCollider.ManualOffsetY = -Mathf.RoundToInt(bounds.extents.y / (1f / 16f));
            component2.PrimaryPixelCollider.ManualWidth = Mathf.RoundToInt(bounds.size.x / (1f / 16f));
            component2.PrimaryPixelCollider.ManualHeight = Mathf.RoundToInt(bounds.size.y / (1f / 16f));
            component2.UpdateCollidersOnRotation = true;
            component2.UpdateCollidersOnScale = true;
            component1.Reawaken();
            component1.Owner = this.CurrentOwner;
            component1.Start();
            component1.SendInDirection(vector2, true, false);
            component1.OnBecameDebris += (Action<DebrisObject>) (a =>
            {
                if ((bool) (UnityEngine.Object) this.barrelOffset)
                    this.barrelOffset.localPosition = this.m_originalBarrelOffsetPosition;
                if ((bool) (UnityEngine.Object) this.muzzleOffset)
                    this.muzzleOffset.localPosition = this.m_originalMuzzleOffsetPosition;
                if ((bool) (UnityEngine.Object) this.chargeOffset)
                    this.chargeOffset.localPosition = this.m_originalChargeOffsetPosition;
                if (!(bool) (UnityEngine.Object) a)
                    return;
                a.FlagAsPickup();
                a.Priority = EphemeralObject.EphemeralPriority.Critical;
                TrailRenderer componentInChildren = a.gameObject.GetComponentInChildren<TrailRenderer>();
                if ((bool) (UnityEngine.Object) componentInChildren)
                    UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren);
                SpeculativeRigidbody component3 = a.GetComponent<SpeculativeRigidbody>();
                if (!(bool) (UnityEngine.Object) component3)
                    return;
                component3.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile, CollisionLayer.EnemyHitBox));
            });
            component1.OnBecameDebrisGrounded += new Action<DebrisObject>(this.HandleThrownGunGrounded);
            component1.angularVelocity = (double) vector2.x <= 0.0 ? 1080f : -1080f;
            if (!RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
                RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) this);
            component2.ForceRegenerate();
            if ((bool) (UnityEngine.Object) this.m_owner)
                (this.m_owner as PlayerController).DoPostProcessThrownGun(component1);
            this.m_owner = (GameActor) null;
        }

        private void HandleThrownGunGrounded(DebrisObject obj)
        {
            obj.OnGrounded -= new Action<DebrisObject>(this.HandleThrownGunGrounded);
            obj.inertialMass = 10f;
            if ((bool) (UnityEngine.Object) this.barrelOffset)
                this.barrelOffset.localPosition = this.m_originalBarrelOffsetPosition;
            if ((bool) (UnityEngine.Object) this.muzzleOffset)
                this.muzzleOffset.localPosition = this.m_originalMuzzleOffsetPosition;
            if ((bool) (UnityEngine.Object) this.chargeOffset)
                this.chargeOffset.localPosition = this.m_originalChargeOffsetPosition;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.m_sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(this.m_sprite, Color.black, 0.1f, 0.05f);
            this.m_sprite.UpdateZDepth();
            this.m_thrownOnGround = true;
        }

        public void RegisterNewCustomAmmunition(ActiveAmmunitionData ammodata)
        {
            if (ammodata == null || ammodata.ShotsRemaining <= 0 || this.m_customAmmunitions.Contains(ammodata))
                return;
            this.m_customAmmunitions.Add(ammodata);
        }

        public void RegisterMinimapIcon()
        {
            if ((double) this.transform.position.y < -300.0)
                return;
            GameObject iconPrefab = (GameObject) BraveResources.Load("Global Prefabs/Minimap_Gun_Icon");
            if (!((UnityEngine.Object) iconPrefab != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_owner == (UnityEngine.Object) null))
                return;
            this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
            this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, iconPrefab);
        }

        public void GetRidOfMinimapIcon()
        {
            if (!((UnityEngine.Object) this.m_instanceMinimapIcon != (UnityEngine.Object) null))
                return;
            Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.m_instanceMinimapIcon);
            this.m_instanceMinimapIcon = (GameObject) null;
        }

        public void GainAmmo(int amt)
        {
            if (!this.CanGainAmmo || this.InfiniteAmmo)
                return;
            if (amt > 0)
                this.UnprepGunForThrow();
            this.ammo += amt;
            if (this.AdjustedMaxAmmo > 0)
                this.ammo = Math.Min(this.AdjustedMaxAmmo, this.ammo);
            this.ammo = Mathf.Clamp(this.ammo, 0, 100000000);
            if (this.OnAmmoChanged == null)
                return;
            this.OnAmmoChanged(this.m_owner as PlayerController, this);
        }

        public void LoseAmmo(int amt)
        {
            this.ammo -= amt;
            if (this.ammo < 0)
                this.ammo = 0;
            if (this.ClipShotsRemaining > this.ammo)
                this.ClipShotsRemaining = this.ammo;
            if (this.OnAmmoChanged == null)
                return;
            this.OnAmmoChanged(this.m_owner as PlayerController, this);
        }

        public void GainAmmo(Gun g)
        {
            if (!this.CanGainAmmo)
                return;
            this.ammo += g.ammo;
            if (this.AdjustedMaxAmmo > 0)
                this.ammo = Math.Min(this.AdjustedMaxAmmo, this.ammo);
            if (this.OnAmmoChanged == null)
                return;
            this.OnAmmoChanged(this.m_owner as PlayerController, this);
        }

        public float GetPrimaryCooldown()
        {
            return (UnityEngine.Object) this.Volley != (UnityEngine.Object) null ? this.Volley.projectiles[0].cooldownTime : this.singleModule.cooldownTime;
        }

        public void ClearOptionalReloadVolleyCooldownAndReloadData()
        {
            if (!((UnityEngine.Object) this.OptionalReloadVolley != (UnityEngine.Object) null))
                return;
            for (int index = 0; index < this.OptionalReloadVolley.projectiles.Count; ++index)
            {
                if (this.m_moduleData.ContainsKey(this.OptionalReloadVolley.projectiles[index]))
                {
                    this.m_moduleData[this.OptionalReloadVolley.projectiles[index]].onCooldown = false;
                    this.m_moduleData[this.OptionalReloadVolley.projectiles[index]].needsReload = false;
                }
            }
        }

        public void ClearCooldowns()
        {
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    this.m_moduleData[this.Volley.projectiles[index]].onCooldown = false;
            }
            else
                this.m_moduleData[this.singleModule].onCooldown = false;
            if (!this.UsesRechargeLikeActiveItem)
                return;
            this.RemainingActiveCooldownAmount = 0.0f;
        }

        public void ClearReloadData()
        {
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    this.m_moduleData[this.Volley.projectiles[index]].needsReload = false;
                this.m_isReloading = false;
            }
            else
            {
                this.m_moduleData[this.singleModule].needsReload = false;
                this.m_isReloading = false;
            }
        }

        public Gun.AttackResult Attack(
            ProjectileData overrideProjectileData = null,
            GameObject overrideBulletObject = null)
        {
            if (this.m_isCurrentlyFiring && this.m_midBurstFire)
                return Gun.AttackResult.Fail;
            if (!this.m_hasReinitializedAudioSwitch)
            {
                this.m_hasReinitializedAudioSwitch = true;
                if (!string.IsNullOrEmpty(this.gunSwitchGroup))
                {
                    int num = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.gunSwitchGroup, this.gameObject);
                }
            }
            this.m_playedEmptyClipSound = false;
            this.m_continuousAttackTime = 0.0f;
            if (this.m_isReloading)
            {
                this.Reload();
                return Gun.AttackResult.Reload;
            }
            if (this.CurrentAmmo < 0)
                this.CurrentAmmo = 0;
            if (this.CurrentAmmo == 0)
            {
                if (this.InfiniteAmmo)
                {
                    this.GainAmmo(this.maxAmmo);
                }
                else
                {
                    this.HandleOutOfAmmo();
                    return Gun.AttackResult.Empty;
                }
            }
            this.m_cachedIsGunBlocked = this.IsGunBlocked();
            this.m_isCurrentlyFiring = true;
            bool flag1 = false;
            if (this.CanCriticalFire)
            {
                float num = (float) PlayerStats.GetTotalCoolness() / 100f;
                if (this.m_owner.IsStealthed)
                    num = 10f;
                if ((double) UnityEngine.Random.value < (double) this.CriticalChance + (double) num)
                    this.m_isCritting = true;
                if (this.ForceNextShotCritical)
                {
                    this.ForceNextShotCritical = false;
                    this.m_isCritting = true;
                }
            }
            if (this.IsHeroSword)
            {
                flag1 = true;
                if (!this.m_anim.IsPlaying(this.shootAnimation) && !this.m_anim.IsPlaying(this.reloadAnimation) && (double) this.HeroSwordCooldown <= 0.0)
                {
                    this.HeroSwordCooldown = 0.5f;
                    this.StartCoroutine(this.HandleSlash());
                    this.HandleShootAnimation((ProjectileModule) null);
                }
            }
            else if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                bool flag2 = this.CheckHasLoadedModule(this.Volley);
                if (!flag2)
                    this.AttemptedFireNeedReload();
                if (flag2 || (double) this.reloadTime <= 0.0)
                {
                    ProjectileVolleyData Volley = this.Volley;
                    if ((UnityEngine.Object) this.modifiedFinalVolley != (UnityEngine.Object) null && this.DefaultModule.HasFinalVolleyOverride() && this.DefaultModule.IsFinalShot(this.m_moduleData[this.DefaultModule], this.CurrentOwner))
                        Volley = this.modifiedFinalVolley;
                    flag1 = this.HandleInitialGunShoot(Volley, overrideProjectileData, overrideBulletObject);
                    this.m_midBurstFire = false;
                    for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    {
                        ProjectileModule projectile = this.Volley.projectiles[index];
                        if (projectile.shootStyle == ProjectileModule.ShootStyle.Burst && this.m_moduleData[projectile].numberShotsFiredThisBurst < projectile.burstShotCount)
                        {
                            this.m_midBurstFire = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                bool flag3 = this.CheckHasLoadedModule(this.singleModule);
                if (!flag3)
                    this.AttemptedFireNeedReload();
                if (flag3 || (double) this.reloadTime <= 0.0)
                {
                    flag1 = this.HandleInitialGunShoot(this.singleModule, overrideProjectileData, overrideBulletObject);
                    this.m_midBurstFire = false;
                    if (this.singleModule.shootStyle == ProjectileModule.ShootStyle.Burst && this.m_moduleData[this.singleModule].numberShotsFiredThisBurst < this.singleModule.burstShotCount)
                        this.m_midBurstFire = true;
                }
            }
            this.m_isCurrentlyFiring = flag1;
            if (this.m_isCurrentlyFiring && this.lowersAudioWhileFiring)
            {
                int num1 = (int) AkSoundEngine.PostEvent("play_state_volume_lower_01", GameManager.Instance.gameObject);
            }
            if (flag1 && this.OnPostFired != null && this.m_owner is PlayerController)
                this.OnPostFired(this.m_owner as PlayerController, this);
            return flag1 ? Gun.AttackResult.Success : Gun.AttackResult.OnCooldown;
        }

        public bool ContinueAttack(bool canAttack = true, ProjectileData overrideProjectileData = null)
        {
            if (!this.m_isCurrentlyFiring)
            {
                if (!this.HasShootStyle(ProjectileModule.ShootStyle.Charged) && !this.HasShootStyle(ProjectileModule.ShootStyle.Automatic) && !this.HasShootStyle(ProjectileModule.ShootStyle.Burst) || this.IsEmpty || this.m_isReloading)
                    return false;
                if (this.CurrentAmmo < 0)
                    this.CurrentAmmo = 0;
                return this.CurrentAmmo != 0 && canAttack && this.Attack(overrideProjectileData) == Gun.AttackResult.Success;
            }
            if (this.m_isReloading)
                return false;
            if (this.CurrentAmmo < 0)
                this.CurrentAmmo = 0;
            if (this.CurrentAmmo == 0)
            {
                this.CeaseAttack(false);
                return false;
            }
            if (!this.m_playedEmptyClipSound && this.ClipShotsRemaining == 0)
            {
                if (GameManager.AUDIO_ENABLED)
                {
                    int num = (int) AkSoundEngine.PostEvent("Play_WPN_gun_empty_01", this.gameObject);
                }
                this.m_playedEmptyClipSound = true;
            }
            this.m_cachedIsGunBlocked = this.IsGunBlocked();
            this.m_isCurrentlyFiring = true;
            this.m_continuousAttackTime += BraveTime.DeltaTime;
            bool flag = false;
            if (!canAttack || this.m_cachedIsGunBlocked)
            {
                if (this.m_activeBeams.Count > 0)
                    this.ClearBeams();
                else if (this.isAudioLoop && this.m_isAudioLooping)
                {
                    if (GameManager.AUDIO_ENABLED)
                    {
                        int num = (int) AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", this.gameObject);
                    }
                    this.m_isAudioLooping = false;
                }
                this.ClearBurstState();
                if (this.usesContinuousMuzzleFlash)
                {
                    this.muzzleFlashEffects.DestroyAll();
                    this.m_isContinuousMuzzleFlashOut = false;
                }
                this.m_continuousAttackTime = 0.0f;
            }
            if (this.m_activeBeams.Count > 0 && this.m_owner is PlayerController)
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.BEAM_WEAPON_FIRE_TIME, BraveTime.DeltaTime);
            if (this.CanCriticalFire)
            {
                float num = (float) PlayerStats.GetTotalCoolness() / 100f;
                if (this.m_owner.IsStealthed)
                    num = 10f;
                if ((double) UnityEngine.Random.value < (double) this.CriticalChance + (double) num)
                    this.m_isCritting = true;
                if (this.ForceNextShotCritical)
                {
                    this.ForceNextShotCritical = false;
                    this.m_isCritting = true;
                }
            }
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                if (this.CheckHasLoadedModule(this.Volley))
                {
                    ProjectileVolleyData Volley = this.Volley;
                    if ((UnityEngine.Object) this.modifiedFinalVolley != (UnityEngine.Object) null && this.DefaultModule.HasFinalVolleyOverride() && this.DefaultModule.IsFinalShot(this.m_moduleData[this.DefaultModule], this.CurrentOwner))
                        Volley = this.modifiedFinalVolley;
                    flag = this.HandleContinueGunShoot(Volley, canAttack, overrideProjectileData);
                    this.m_midBurstFire = false;
                    for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    {
                        ProjectileModule projectile = this.Volley.projectiles[index];
                        if (projectile.shootStyle == ProjectileModule.ShootStyle.Burst && this.m_moduleData[projectile].numberShotsFiredThisBurst < projectile.burstShotCount)
                        {
                            this.m_midBurstFire = true;
                            break;
                        }
                    }
                }
                else
                    this.CeaseAttack(false);
            }
            else if (this.CheckHasLoadedModule(this.singleModule))
            {
                flag = this.HandleContinueGunShoot(this.singleModule, canAttack, overrideProjectileData);
            }
            else
            {
                this.CeaseAttack(false);
                this.m_midBurstFire = false;
                if (this.singleModule.shootStyle == ProjectileModule.ShootStyle.Burst && this.m_moduleData[this.singleModule].numberShotsFiredThisBurst < this.singleModule.burstShotCount)
                    this.m_midBurstFire = true;
            }
            if (flag && this.OnPostFired != null && this.m_owner is PlayerController)
                this.OnPostFired(this.m_owner as PlayerController, this);
            return flag;
        }

        public void OnPrePlayerChange()
        {
            if (!this.m_isPreppedForThrow)
                return;
            this.UnprepGunForThrow();
        }

        public bool CeaseAttack(bool canAttack = true, ProjectileData overrideProjectileData = null)
        {
            if (this.m_isPreppedForThrow && (double) this.m_prepThrowTime < 1.2000000476837158)
                this.UnprepGunForThrow();
            else if (this.m_isPreppedForThrow)
            {
                (this.m_owner as PlayerController).inventory.RemoveGunFromInventory(this);
                this.ThrowGun();
                return true;
            }
            if (!this.m_isCurrentlyFiring)
                return false;
            if (this.m_midBurstFire && canAttack)
            {
                this.m_continueBurstInUpdate = true;
                return true;
            }
            this.m_continueBurstInUpdate = false;
            if (this.m_isCurrentlyFiring && this.lowersAudioWhileFiring)
            {
                int num1 = (int) AkSoundEngine.PostEvent("stop_state_volume_lower_01", GameManager.Instance.gameObject);
            }
            this.m_isCurrentlyFiring = false;
            this.m_hasDecrementedFunds = false;
            this.m_continuousAttackTime = 0.0f;
            this.m_cachedIsGunBlocked = this.IsGunBlocked();
            if (this.CanCriticalFire)
            {
                float num2 = (float) PlayerStats.GetTotalCoolness() / 100f;
                if (this.m_owner.IsStealthed)
                    num2 = 10f;
                if ((double) UnityEngine.Random.value < (double) this.CriticalChance + (double) num2)
                    this.m_isCritting = true;
                if (this.ForceNextShotCritical)
                {
                    this.ForceNextShotCritical = false;
                    this.m_isCritting = true;
                }
            }
            if (this.LockedHorizontalOnCharge)
            {
                this.m_attachTransform.localRotation = Quaternion.Euler(this.m_attachTransform.localRotation.x, this.m_attachTransform.localRotation.y, this.LockedHorizontalCachedAngle);
                this.gunAngle = this.LockedHorizontalCachedAngle;
            }
            bool flag = !((UnityEngine.Object) this.Volley != (UnityEngine.Object) null) ? this.HandleEndGunShoot(this.singleModule, canAttack, overrideProjectileData) : this.HandleEndGunShoot(this.Volley, canAttack, overrideProjectileData);
            if (this.MovesPlayerForwardOnChargeFire && flag && (bool) (UnityEngine.Object) this.m_owner && this.m_owner is PlayerController)
                this.m_owner.knockbackDoer.ApplyKnockback(BraveMathCollege.DegreesToVector(this.CurrentAngle), 40f, 0.25f);
            if (GameManager.AUDIO_ENABLED)
            {
                int num3 = (int) AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", this.gameObject);
            }
            this.m_isAudioLooping = false;
            this.ClearBeams();
            if (this.usesContinuousFireAnimation)
            {
                this.m_anim.StopAndResetFrame();
                this.AnimationCompleteDelegate(this.m_anim, (tk2dSpriteAnimationClip) null);
            }
            if (this.usesContinuousMuzzleFlash)
            {
                this.muzzleFlashEffects.DestroyAll();
                this.m_isContinuousMuzzleFlashOut = false;
            }
            if (!this.m_isReloading && this.DefaultModule.GetModNumberOfShotsInClip(this.CurrentOwner) == 1)
                this.m_reloadWhenDoneFiring = true;
            if ((bool) (UnityEngine.Object) this.Volley)
            {
                ProjectileVolleyData volley = this.Volley;
                if ((bool) (UnityEngine.Object) volley)
                {
                    for (int index = 0; index < volley.projectiles.Count; ++index)
                        this.m_moduleData[volley.projectiles[index]].numberShotsFiredThisBurst = 0;
                }
            }
            else
                this.m_moduleData[this.singleModule].numberShotsFiredThisBurst = 0;
            if (this.CurrentOwner is PlayerController && this.OnFinishAttack != null)
                this.OnFinishAttack(this.CurrentOwner as PlayerController, this);
            return flag;
        }

        public void AttemptedFireNeedReload()
        {
            PlayerController owner = this.m_owner as PlayerController;
            this.Reload();
            if (this.OnReloadPressed == null || !(bool) (UnityEngine.Object) owner)
                return;
            this.OnReloadPressed(owner, this, false);
        }

        protected void OnActiveReloadSuccess()
        {
            this.FinishReload(true);
            float num = 1f;
            if (Gun.ActiveReloadActivated && this.m_owner is PlayerController && (this.m_owner as PlayerController).IsPrimaryPlayer)
                num *= CogOfBattleItem.ACTIVE_RELOAD_DAMAGE_MULTIPLIER;
            if (Gun.ActiveReloadActivatedPlayerTwo && this.m_owner is PlayerController && !(this.m_owner as PlayerController).IsPrimaryPlayer)
                num *= CogOfBattleItem.ACTIVE_RELOAD_DAMAGE_MULTIPLIER;
            if (this.LocalActiveReload)
                num *= Mathf.Pow(this.activeReloadData.damageMultiply, (float) (this.SequentialActiveReloads + 1));
            UnityEngine.Debug.LogError((object) ("total damage multiplier: " + (object) num));
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                    this.m_moduleData[this.Volley.projectiles[index]].activeReloadDamageModifier = num;
            }
            else
                this.m_moduleData[this.singleModule].activeReloadDamageModifier = num;
        }

        private void HandleOutOfAmmo()
        {
            if (this.m_owner is PlayerController)
                this.PrepGunForThrow();
            else
                this.m_owner.aiShooter.Inventory.RemoveGunFromInventory(this);
        }

        public void HandleShootAnimation(ProjectileModule module)
        {
            if (!((UnityEngine.Object) this.m_anim != (UnityEngine.Object) null))
                return;
            string name = this.shootAnimation;
            if (module != null && !string.IsNullOrEmpty(this.finalShootAnimation) && module.IsFinalShot(this.m_moduleData[module], this.CurrentOwner))
                name = this.finalShootAnimation;
            if (this.m_isCritting && !string.IsNullOrEmpty(this.criticalFireAnimation))
                name = this.criticalFireAnimation;
            if (module != null && module.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(this.m_moduleData[module].chargeTime);
                if (chargeProjectile != null && chargeProjectile.UsesOverrideShootAnimation)
                    name = chargeProjectile.OverrideShootAnimation;
            }
            this.PlayIfExists(name, true);
        }

        public void HandleShootEffects(ProjectileModule module)
        {
            Transform parent = !(bool) (UnityEngine.Object) this.muzzleOffset ? this.barrelOffset : this.muzzleOffset;
            Vector3 position = parent.position - new Vector3(0.0f, 0.0f, 0.1f);
            VFXPool vfxPool = this.muzzleFlashEffects;
            if (module != null && this.finalMuzzleFlashEffects.type != VFXPoolType.None && module.IsFinalShot(this.m_moduleData[module], this.CurrentOwner))
                vfxPool = this.finalMuzzleFlashEffects;
            if (this.m_isCritting && this.CriticalMuzzleFlashEffects.type != VFXPoolType.None)
                vfxPool = this.CriticalMuzzleFlashEffects;
            if (module != null && module.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(this.m_moduleData[module].chargeTime);
                if (chargeProjectile != null && chargeProjectile.UsesOverrideMuzzleFlashVfxPool)
                    vfxPool = chargeProjectile.OverrideMuzzleFlashVfxPool;
            }
            if (!this.usesContinuousMuzzleFlash || !this.m_isContinuousMuzzleFlashOut)
                vfxPool.SpawnAtPosition(position, !this.preventRotation ? this.gunAngle : 0.0f, parent, new Vector2?(Vector2.zero), new Vector2?(Vector2.zero), new float?(-0.05f), true);
            if (this.usesContinuousMuzzleFlash)
                this.m_isContinuousMuzzleFlashOut = true;
            if (this.shellsToLaunchOnFire <= 0)
                return;
            if (this.shellCasingOnFireFrameDelay <= 0)
            {
                for (int index = 0; index < this.shellsToLaunchOnFire; ++index)
                    this.SpawnShellCasingAtPosition(this.CasingLaunchPoint);
            }
            else
                this.StartCoroutine(this.HandleShellCasingFireDelay());
        }

        private void TogglePreviousChargeEffectsIfNecessary(
            ProjectileModule.ChargeProjectile cp,
            bool visible)
        {
            if (cp == null)
                return;
            if (cp.previousChargeProjectile != null && cp.previousChargeProjectile.DelayedVFXDestruction)
                this.TogglePreviousChargeEffectsIfNecessary(cp.previousChargeProjectile, visible);
            if (!cp.UsesVfx || cp.VfxPool == null)
                return;
            cp.VfxPool.ToggleRenderers(visible);
        }

        private void DestroyPreviousChargeEffectsIfNecessary(ProjectileModule.ChargeProjectile cp)
        {
            if (cp.previousChargeProjectile != null && cp.previousChargeProjectile.DelayedVFXDestruction)
                this.DestroyPreviousChargeEffectsIfNecessary(cp.previousChargeProjectile);
            if (!cp.UsesVfx)
                return;
            cp.VfxPool.DestroyAll();
        }

        private void HandleChargeEffects(
            ProjectileModule.ChargeProjectile oldChargeProjectile,
            ProjectileModule.ChargeProjectile newChargeProjectile)
        {
            Transform parent = !(bool) (UnityEngine.Object) this.chargeOffset ? (!(bool) (UnityEngine.Object) this.muzzleOffset ? this.barrelOffset : this.muzzleOffset) : this.chargeOffset;
            Vector3 position = parent.position - new Vector3(0.0f, 0.0f, 0.1f);
            if (oldChargeProjectile != null)
            {
                if (!oldChargeProjectile.DelayedVFXDestruction || newChargeProjectile == null)
                    this.DestroyPreviousChargeEffectsIfNecessary(oldChargeProjectile);
                if (oldChargeProjectile.UsesVfx && oldChargeProjectile.VfxPool == this.m_currentlyPlayingChargeVFX)
                    this.m_currentlyPlayingChargeVFX = (VFXPool) null;
                if (oldChargeProjectile.UsesScreenShake)
                    GameManager.Instance.MainCameraController.StopContinuousScreenShake((Component) this);
            }
            if (newChargeProjectile == null)
                return;
            newChargeProjectile.previousChargeProjectile = oldChargeProjectile;
            if (newChargeProjectile.UsesVfx)
            {
                newChargeProjectile.VfxPool.SpawnAtPosition(position, this.gunAngle, parent, new Vector2?(Vector2.zero), new Vector2?(Vector2.zero), new float?(2f), true);
                this.m_currentlyPlayingChargeVFX = newChargeProjectile.VfxPool;
                if (!this.m_meshRenderer.enabled)
                    this.m_currentlyPlayingChargeVFX.ToggleRenderers(false);
                else
                    this.m_currentlyPlayingChargeVFX.ToggleRenderers(true);
            }
            if (newChargeProjectile.ShouldDoChargePoof && this.m_owner is PlayerController)
                this.DoChargeCompletePoof();
            if (!newChargeProjectile.UsesScreenShake)
                return;
            GameManager.Instance.MainCameraController.DoContinuousScreenShake(newChargeProjectile.ScreenShake, (Component) this, this.m_owner is PlayerController);
        }

        private void DoChargeCompletePoof()
        {
            GameObject gameObject = SpawnManager.SpawnVFX(BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge"));
            gameObject.transform.parent = this.m_owner.transform;
            gameObject.transform.position = (Vector3) this.m_owner.specRigidbody.UnitCenter;
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            component.HeightOffGround = -1f;
            component.UpdateZDepth();
            (this.CurrentOwner as PlayerController).DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
        }

        private void HandleChargeIntensity(ProjectileModule module, ModuleShootData shootData)
        {
            if (!(bool) (UnityEngine.Object) this.light)
                return;
            ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(shootData.chargeTime);
            if (chargeProjectile == null)
                return;
            float a = !chargeProjectile.UsesLightIntensity ? this.baseLightIntensity : chargeProjectile.LightIntensity;
            float chargeTime = chargeProjectile.ChargeTime;
            float b1 = a;
            float b2 = chargeTime;
            int num = module.chargeProjectiles.IndexOf(chargeProjectile);
            if (num < module.chargeProjectiles.Count - 1)
            {
                b1 = !module.chargeProjectiles[num + 1].UsesLightIntensity ? this.baseLightIntensity : module.chargeProjectiles[num + 1].LightIntensity;
                b2 = module.chargeProjectiles[num + 1].ChargeTime;
            }
            this.light.intensity = Mathf.Lerp(a, b1, Mathf.InverseLerp(chargeTime, b2, shootData.chargeTime));
        }

        private void EndChargeIntensity()
        {
            if (!(bool) (UnityEngine.Object) this.light)
                return;
            this.light.intensity = this.baseLightIntensity;
        }

        [DebuggerHidden]
        private IEnumerator HandleShellCasingFireDelay()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Gun__HandleShellCasingFireDelayc__Iterator2()
            {
                _this = this
            };
        }

        private void SpawnShellCasingAtPosition(Vector3 position)
        {
            if (!((UnityEngine.Object) this.shellCasing != (UnityEngine.Object) null) || !(bool) (UnityEngine.Object) this.m_transform)
                return;
            GameObject gameObject = SpawnManager.SpawnDebris(this.shellCasing, position.WithZ(this.m_transform.position.z), Quaternion.Euler(0.0f, 0.0f, this.gunAngle));
            ShellCasing component1 = gameObject.GetComponent<ShellCasing>();
            if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
                component1.Trigger();
            DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
            if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null))
                return;
            int num1 = (double) component2.transform.right.x <= 0.0 ? -1 : 1;
            Vector3 vector3 = Vector3.up * (float) ((double) UnityEngine.Random.value * 1.5 + 1.0) + -1.5f * Vector3.right * (float) num1 * (UnityEngine.Random.value + 1.5f);
            Vector3 startingForce = new Vector3(vector3.x, 0.0f, vector3.y);
            if (this.m_owner is PlayerController)
            {
                PlayerController owner = this.m_owner as PlayerController;
                if (owner.CurrentRoom != null && owner.CurrentRoom.area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.CATACOMBS_BRIDGE_ROOM)
                    startingForce = ((float) ((double) vector3.x * (double) num1 * -1.0) * (this.barrelOffset.position.XY() - this.m_localAimPoint).normalized).ToVector3ZUp(vector3.y);
            }
            float y = this.m_owner.transform.position.y;
            float num2 = (float) ((double) position.y - (double) this.m_owner.transform.position.y + 0.20000000298023224);
            float startingHeight = (float) ((double) component2.transform.position.y - (double) y + (double) UnityEngine.Random.value * 0.5);
            component2.additionalHeightBoost = num2 - startingHeight;
            if ((double) this.gunAngle > 25.0 && (double) this.gunAngle < 155.0)
                component2.additionalHeightBoost += -0.25f;
            else
                component2.additionalHeightBoost += 0.25f;
            component2.Trigger(startingForce, startingHeight);
        }

        private void SpawnClipAtPosition(Vector3 position)
        {
            if (!((UnityEngine.Object) this.clipObject != (UnityEngine.Object) null))
                return;
            DebrisObject component = SpawnManager.SpawnDebris(this.clipObject, position.WithZ(-0.05f), Quaternion.Euler(0.0f, 0.0f, this.gunAngle)).GetComponent<DebrisObject>();
            if (!(bool) (UnityEngine.Object) component)
                return;
            float startingHeight = 0.25f;
            int num = (double) component.transform.right.x <= 0.0 ? -1 : 1;
            Vector3 startingForce = new Vector3(0.0f, -1f, 0.0f);
            if (this.m_owner is PlayerController)
            {
                PlayerController owner = this.m_owner as PlayerController;
                if (owner.CurrentRoom != null && owner.CurrentRoom.area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.CATACOMBS_BRIDGE_ROOM)
                {
                    startingForce = new Vector3((float) num * 0.5f * (this.barrelOffset.position.XY() - this.m_localAimPoint).x, 0.0f, 1f);
                    startingHeight = 0.5f;
                }
            }
            component.Trigger(startingForce, startingHeight);
        }

        private void DoScreenShake()
        {
            Vector2 dir = (Vector2) (Quaternion.Euler(0.0f, 0.0f, this.gunAngle) * Vector3.right);
            if ((UnityEngine.Object) GameManager.Instance.MainCameraController == (UnityEngine.Object) null)
                return;
            if (this.directionlessScreenShake)
                dir = Vector2.zero;
            GameManager.Instance.MainCameraController.DoGunScreenShake(this.gunScreenShake, dir, new Vector2?(), this.m_owner as PlayerController);
        }

        public override void Pickup(PlayerController player)
        {
            if (RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
                RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable) this);
            if (GameManager.Instance.InTutorial)
                GameManager.BroadcastRoomTalkDoerFsmEvent("playerAcquiredGun");
            this.m_isThrown = false;
            this.m_isBeingEyedByRat = false;
            this.OnSharedPickup();
            if (!this.HasEverBeenAcquiredByPlayer)
                player.HasReceivedNewGunThisFloor = true;
            this.HasEverBeenAcquiredByPlayer = true;
            if (!this.ShouldBeDestroyedOnExistence())
            {
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.GUNS_PICKED_UP, 1f);
                if (!PileOfDarkSoulsPickup.IsPileOfDarkSoulsPickup)
                    player.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Ammo_Sparkles_001") as GameObject, Vector3.zero);
                this.HandleEncounterable(player);
                this.GetRidOfMinimapIcon();
                if (GameManager.AUDIO_ENABLED)
                {
                    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_weapon_pickup_01", this.gameObject);
                }
                if (player.CharacterUsesRandomGuns)
                {
                    player.ChangeToRandomGun();
                }
                else
                {
                    Gun inventory = player.inventory.AddGunToInventory(this, true);
                    if (inventory.AdjustedMaxAmmo > 0)
                        inventory.ammo = Math.Min(inventory.AdjustedMaxAmmo, inventory.ammo);
                }
                PlatformInterface.SetAlienFXColor((Color32) this.m_alienPickupColor, 1f);
            }
            if ((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) null)
                UnityEngine.Object.Destroy((UnityEngine.Object) this.transform.parent.gameObject);
            else
                UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }

        public bool HasShootStyle(ProjectileModule.ShootStyle shootStyle)
        {
            ProjectileVolleyData volley = this.Volley;
            if ((UnityEngine.Object) this.Volley == (UnityEngine.Object) null)
                return this.singleModule.shootStyle == shootStyle;
            for (int index = 0; index < volley.projectiles.Count; ++index)
            {
                if (volley.projectiles[index] != null && volley.projectiles[index].shootStyle == shootStyle)
                    return true;
            }
            return false;
        }

        protected void AnimationCompleteDelegate(tk2dSpriteAnimator anima, tk2dSpriteAnimationClip clippy)
        {
            if (clippy == null || this.DisablesRendererOnCooldown && this.m_reloadWhenDoneFiring)
                return;
            this.PlayIdleAnimation();
        }

        public void OnTrigger(
            SpeculativeRigidbody otherRigidbody,
            SpeculativeRigidbody source,
            CollisionData collisionData)
        {
            PlayerController component = otherRigidbody.GetComponent<PlayerController>();
            if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || !BraveInput.WasSelectPressed())
                return;
            this.Pickup(component);
        }

        private void PostRigidbodyMovement(
            SpeculativeRigidbody specRigidbody,
            Vector2 unitDelta,
            IntVector2 pixelDelta)
        {
            if (!(bool) (UnityEngine.Object) this || !this.enabled)
                return;
            for (int index = this.m_activeBeams.Count - 1; index >= 0; --index)
            {
                if (index >= 0 && index < this.m_activeBeams.Count)
                {
                    ModuleShootData activeBeam = this.m_activeBeams[index];
                    if (!(bool) (UnityEngine.Object) activeBeam.beam)
                    {
                        if (activeBeam.beamKnockbackID >= 0)
                        {
                            if ((bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.m_owner.knockbackDoer)
                                this.m_owner.knockbackDoer.EndContinuousKnockback(activeBeam.beamKnockbackID);
                            activeBeam.beamKnockbackID = -1;
                        }
                        this.m_activeBeams.RemoveAt(index);
                    }
                    else
                        activeBeam.beam.LateUpdatePosition(this.m_unroundedBarrelPosition + (Vector3) unitDelta);
                }
            }
        }

        private bool CheckHasLoadedModule(ProjectileModule module)
        {
            if (this.RequiresFundsToShoot && this.m_owner is PlayerController)
            {
                this.m_moduleData[module].numberShotsFired = 0;
                this.m_moduleData[module].needsReload = false;
                return (this.m_owner as PlayerController).carriedConsumables.Currency > 0;
            }
            return !module.ignoredForReloadPurposes && !this.m_moduleData[module].needsReload;
        }

        private bool CheckHasLoadedModule(ProjectileVolleyData Volley)
        {
            if (this.RequiresFundsToShoot && this.m_owner is PlayerController)
            {
                for (int index = 0; index < Volley.projectiles.Count; ++index)
                {
                    this.m_moduleData[Volley.projectiles[index]].numberShotsFired = 0;
                    this.m_moduleData[Volley.projectiles[index]].needsReload = false;
                }
                return (this.m_owner as PlayerController).carriedConsumables.Currency > 0;
            }
            for (int index = 0; index < Volley.projectiles.Count; ++index)
            {
                ProjectileModule projectile = Volley.projectiles[index];
                if (!projectile.ignoredForReloadPurposes && !this.m_moduleData[projectile].needsReload)
                    return true;
            }
            return false;
        }

        private void CreateAmp()
        {
            if (!(bool) (UnityEngine.Object) this.ObjectToInstantiateOnReload || !(bool) (UnityEngine.Object) this.m_owner || !(this.m_owner is PlayerController))
                return;
            if (this.m_extantAmp != null)
            {
                if ((bool) (UnityEngine.Object) (this.m_extantAmp as ShootProjectileOnGunfireDoer))
                {
                    this.m_extantAmp.Deactivate();
                    this.m_extantAmp = (SingleSpawnableGunPlacedObject) null;
                }
                else if ((bool) (UnityEngine.Object) (this.m_extantAmp as BreakableShieldController) && !(this.m_extantAmp as BreakableShieldController).majorBreakable.IsDestroyed)
                    return;
            }
            GameObject gObj = UnityEngine.Object.Instantiate<GameObject>(this.ObjectToInstantiateOnReload, (this.m_owner as PlayerController).CurrentRoom.GetBestRewardLocation(IntVector2.One, RoomHandler.RewardLocationStyle.PlayerCenter, false).ToVector3(), Quaternion.identity);
            if (!(bool) (UnityEngine.Object) gObj)
                return;
            this.m_extantAmp = gObj.GetInterface<SingleSpawnableGunPlacedObject>();
            if (this.m_extantAmp == null)
                return;
            this.m_extantAmp.Initialize(this);
        }

        private bool HandleInitialGunShoot(
            ProjectileModule module,
            ProjectileData overrideProjectileData = null,
            GameObject overrideBulletObject = null)
        {
            if (this.m_moduleData[module].needsReload)
            {
                UnityEngine.Debug.LogError((object) "Trying to shoot a gun without being loaded, should never happen.");
                return false;
            }
            return !this.m_moduleData[module].onCooldown && (!this.UsesRechargeLikeActiveItem || (double) this.m_remainingActiveCooldownAmount <= 0.0) && this.HandleSpecificInitialGunShoot(module, overrideProjectileData, overrideBulletObject);
        }

        private void IncrementModuleFireCountAndMarkReload(
            ProjectileModule mod,
            ProjectileModule.ChargeProjectile currentChargeProjectile)
        {
            ++this.m_moduleData[mod].numberShotsFired;
            ++this.m_moduleData[mod].numberShotsFiredThisBurst;
            if (this.m_moduleData[mod].numberShotsActiveReload > 0)
                --this.m_moduleData[mod].numberShotsActiveReload;
            if (currentChargeProjectile != null && currentChargeProjectile.DepleteAmmo)
            {
                foreach (ProjectileModule key in this.m_moduleData.Keys)
                {
                    if (!key.IsDuctTapeModule)
                    {
                        this.m_moduleData[key].numberShotsFired = key.GetModNumberOfShotsInClip(this.CurrentOwner);
                        this.m_moduleData[key].needsReload = true;
                    }
                }
            }
            if (mod.GetModNumberOfShotsInClip(this.CurrentOwner) > 0 && this.m_moduleData[mod].numberShotsFired >= mod.GetModNumberOfShotsInClip(this.CurrentOwner))
                this.m_moduleData[mod].needsReload = true;
            if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
                return;
            mod.IncrementShootCount();
        }

        private bool RawFireVolley(ProjectileVolleyData Volley)
        {
            bool flag1 = false;
            bool flag2 = true;
            for (int index = 0; index < Volley.projectiles.Count; ++index)
            {
                ProjectileModule projectile = Volley.projectiles[index];
                if (!this.m_moduleData[projectile].needsReload && !this.m_moduleData[projectile].onCooldown && (!this.UsesRechargeLikeActiveItem || (double) this.m_remainingActiveCooldownAmount <= 0.0))
                {
                    if (Volley.ModulesAreTiers)
                        flag2 = (projectile.CloneSourceIndex < 0 ? index : projectile.CloneSourceIndex) == this.m_currentStrengthTier;
                    if (flag2)
                        flag1 |= this.HandleSpecificInitialGunShoot(projectile, playEffects: false);
                    else if (!this.m_cachedIsGunBlocked)
                        this.IncrementModuleFireCountAndMarkReload(projectile, (ProjectileModule.ChargeProjectile) null);
                }
            }
            return flag1;
        }

        private bool HandleInitialGunShoot(
            ProjectileVolleyData Volley,
            ProjectileData overrideProjectileData = null,
            GameObject overrideBulletObject = null)
        {
            bool playEffects = true;
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = true;
            bool flag4 = false;
            for (int index = 0; index < Volley.projectiles.Count; ++index)
            {
                ProjectileModule projectile = Volley.projectiles[index];
                if (!this.m_moduleData[projectile].needsReload)
                {
                    flag1 = true;
                    if (!this.m_moduleData[projectile].onCooldown && (!this.UsesRechargeLikeActiveItem || (double) this.m_remainingActiveCooldownAmount <= 0.0))
                    {
                        if (Volley.ModulesAreTiers)
                        {
                            if (projectile.IsDuctTapeModule)
                                flag3 = true;
                            else if ((projectile.CloneSourceIndex < 0 ? index : projectile.CloneSourceIndex) == this.m_currentStrengthTier)
                            {
                                playEffects = !flag4;
                                flag3 = true;
                                flag4 = true;
                            }
                            else
                            {
                                playEffects = false;
                                flag3 = false;
                            }
                        }
                        if (flag3)
                            flag2 |= this.HandleSpecificInitialGunShoot(projectile, overrideProjectileData, overrideBulletObject, playEffects);
                        else if (!this.m_cachedIsGunBlocked)
                            this.IncrementModuleFireCountAndMarkReload(projectile, (ProjectileModule.ChargeProjectile) null);
                        playEffects = false;
                    }
                }
            }
            if (!flag1)
                UnityEngine.Debug.LogError((object) "Trying to shoot a gun without being loaded, should never happen.");
            return flag2;
        }

        private bool HandleSpecificInitialGunShoot(
            ProjectileModule module,
            ProjectileData overrideProjectileData = null,
            GameObject overrideBulletObject = null,
            bool playEffects = true)
        {
            if (module.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic || module.shootStyle == ProjectileModule.ShootStyle.Burst || module.shootStyle == ProjectileModule.ShootStyle.Automatic)
            {
                if (this.m_cachedIsGunBlocked)
                    return false;
                if (playEffects)
                {
                    this.HandleShootAnimation(module);
                    this.HandleShootEffects(module);
                    if (this.doesScreenShake)
                        this.DoScreenShake();
                }
                if (playEffects || module.runtimeGuid != null && this.AdditionalShootSoundsByModule.ContainsKey(module.runtimeGuid))
                {
                    if (module.runtimeGuid != null && this.AdditionalShootSoundsByModule.ContainsKey(module.runtimeGuid))
                    {
                        int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.AdditionalShootSoundsByModule[module.runtimeGuid], this.gameObject);
                    }
                    if (GameManager.AUDIO_ENABLED && (!this.isAudioLoop || !this.m_isAudioLooping))
                    {
                        string in_pszEventName = !module.IsFinalShot(this.m_moduleData[module], this.m_owner) || this.OverrideFinaleAudio ? "Play_WPN_gun_shot_01" : "Play_WPN_gun_finale_01";
                        if (!this.PreventNormalFireAudio)
                        {
                            int num2 = (int) AkSoundEngine.PostEvent(in_pszEventName, this.gameObject);
                        }
                        else
                        {
                            int num3 = (int) AkSoundEngine.PostEvent(this.OverrideNormalFireAudioEvent, this.gameObject);
                        }
                        this.m_isAudioLooping = true;
                    }
                    if (!string.IsNullOrEmpty(this.gunSwitchGroup))
                    {
                        int num4 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.gunSwitchGroup, this.gameObject);
                    }
                }
                this.ShootSingleProjectile(module, overrideProjectileData, overrideBulletObject);
                this.DecrementAmmoCost(module);
                this.TriggerModuleCooldown(module);
                return true;
            }
            if (module.shootStyle == ProjectileModule.ShootStyle.Beam)
            {
                if (this.m_cachedIsGunBlocked)
                    return false;
                if (playEffects)
                {
                    if ((UnityEngine.Object) this.m_anim != (UnityEngine.Object) null)
                        this.PlayIfExists(this.shootAnimation);
                    this.HandleShootEffects(module);
                }
                if (playEffects || module.runtimeGuid != null && this.AdditionalShootSoundsByModule.ContainsKey(module.runtimeGuid))
                {
                    if (module.runtimeGuid != null && this.AdditionalShootSoundsByModule.ContainsKey(module.runtimeGuid))
                    {
                        int num5 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.AdditionalShootSoundsByModule[module.runtimeGuid], this.gameObject);
                    }
                    if (GameManager.AUDIO_ENABLED && (!this.isAudioLoop || !this.m_isAudioLooping))
                    {
                        string in_pszEventName = !module.IsFinalShot(this.m_moduleData[module], this.m_owner) || this.OverrideFinaleAudio ? "Play_WPN_gun_shot_01" : "Play_WPN_gun_finale_01";
                        if (!this.PreventNormalFireAudio)
                        {
                            int num6 = (int) AkSoundEngine.PostEvent(in_pszEventName, this.gameObject);
                        }
                        this.m_isAudioLooping = true;
                    }
                    if (!string.IsNullOrEmpty(this.gunSwitchGroup))
                    {
                        int num7 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.gunSwitchGroup, this.gameObject);
                    }
                }
                this.BeginFiringBeam(module);
                return true;
            }
            if (module.shootStyle != ProjectileModule.ShootStyle.Charged)
                return false;
            ModuleShootData shootData = this.m_moduleData[module];
            shootData.chargeTime = 0.0f;
            shootData.chargeFired = false;
            if (playEffects)
            {
                this.PlayIfExists(this.chargeAnimation);
                ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(shootData.chargeTime);
                this.HandleChargeEffects((ProjectileModule.ChargeProjectile) null, chargeProjectile);
                this.HandleChargeIntensity(module, shootData);
                shootData.lastChargeProjectile = chargeProjectile;
                if (GameManager.AUDIO_ENABLED)
                {
                    int num = (int) AkSoundEngine.PostEvent("Play_WPN_gun_charge_01", this.gameObject);
                }
            }
            return true;
        }

        private bool HandleContinueGunShoot(
            ProjectileModule module,
            bool canAttack = true,
            ProjectileData overrideProjectileData = null)
        {
            if (this.m_moduleData[module].needsReload)
            {
                UnityEngine.Debug.LogError((object) "Attempting to continue fire on an unloaded gun. This should never happen.");
                return false;
            }
            return !this.m_moduleData[module].onCooldown && (!this.UsesRechargeLikeActiveItem || (double) this.m_remainingActiveCooldownAmount <= 0.0) && this.HandleSpecificContinueGunShoot(module, canAttack, overrideProjectileData);
        }

        private bool HandleContinueGunShoot(
            ProjectileVolleyData Volley,
            bool canAttack = true,
            ProjectileData overrideProjectileData = null)
        {
            bool playEffects = true;
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = true;
            for (int index = 0; index < Volley.projectiles.Count; ++index)
            {
                ProjectileModule projectile = Volley.projectiles[index];
                if (!this.m_moduleData[projectile].needsReload)
                {
                    flag1 = true;
                    if (!this.m_moduleData[projectile].onCooldown && (!this.UsesRechargeLikeActiveItem || (double) this.m_remainingActiveCooldownAmount <= 0.0))
                    {
                        if (Volley.ModulesAreTiers)
                        {
                            if (projectile.IsDuctTapeModule)
                                flag3 = true;
                            else if ((projectile.CloneSourceIndex < 0 ? index : projectile.CloneSourceIndex) == this.m_currentStrengthTier)
                            {
                                playEffects = true;
                                flag3 = true;
                            }
                            else
                            {
                                playEffects = false;
                                flag3 = false;
                            }
                        }
                        if (projectile.isExternalAddedModule)
                            playEffects = false;
                        if (flag3)
                            flag2 |= this.HandleSpecificContinueGunShoot(projectile, canAttack, overrideProjectileData, playEffects);
                        else if ((projectile.shootStyle == ProjectileModule.ShootStyle.Automatic || projectile.shootStyle == ProjectileModule.ShootStyle.Burst) && !this.m_cachedIsGunBlocked && canAttack)
                            this.IncrementModuleFireCountAndMarkReload(projectile, (ProjectileModule.ChargeProjectile) null);
                        if (flag2)
                            playEffects = false;
                    }
                }
            }
            if (!flag1)
                UnityEngine.Debug.LogError((object) "Attempting to continue fire without being loaded. This should never happen.");
            return flag2;
        }

        private bool HandleSpecificContinueGunShoot(
            ProjectileModule module,
            bool canAttack = true,
            ProjectileData overrideProjectileData = null,
            bool playEffects = true)
        {
            if (module.shootStyle == ProjectileModule.ShootStyle.Automatic || module.shootStyle == ProjectileModule.ShootStyle.Burst)
            {
                if (this.m_cachedIsGunBlocked || !canAttack)
                    return false;
                if (module.shootStyle == ProjectileModule.ShootStyle.Burst && this.m_moduleData[module].numberShotsFiredThisBurst >= module.burstShotCount)
                {
                    this.m_moduleData[module].numberShotsFiredThisBurst = 0;
                    if (this.OnBurstContinued != null)
                        this.OnBurstContinued(this.CurrentOwner as PlayerController, this);
                }
                if (playEffects)
                {
                    if (!this.usesContinuousFireAnimation)
                        this.Play(string.IsNullOrEmpty(this.finalShootAnimation) || !module.IsFinalShot(this.m_moduleData[module], this.CurrentOwner) ? this.shootAnimation : this.finalShootAnimation);
                    this.HandleShootEffects(module);
                    if (this.doesScreenShake)
                        this.DoScreenShake();
                }
                if (playEffects || module.runtimeGuid != null && this.AdditionalShootSoundsByModule.ContainsKey(module.runtimeGuid))
                {
                    if (module.runtimeGuid != null && this.AdditionalShootSoundsByModule.ContainsKey(module.runtimeGuid))
                    {
                        int num1 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.AdditionalShootSoundsByModule[module.runtimeGuid], this.gameObject);
                    }
                    if (GameManager.AUDIO_ENABLED && (!this.isAudioLoop || !this.m_isAudioLooping))
                    {
                        string in_pszEventName = !module.IsFinalShot(this.m_moduleData[module], this.m_owner) || this.OverrideFinaleAudio ? "Play_WPN_gun_shot_01" : "Play_WPN_gun_finale_01";
                        if (!this.PreventNormalFireAudio)
                        {
                            int num2 = (int) AkSoundEngine.PostEvent(in_pszEventName, this.gameObject);
                        }
                        this.m_isAudioLooping = true;
                    }
                    if (!string.IsNullOrEmpty(this.gunSwitchGroup))
                    {
                        int num3 = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.gunSwitchGroup, this.gameObject);
                    }
                }
                this.ShootSingleProjectile(module, overrideProjectileData);
                this.DecrementAmmoCost(module);
                this.TriggerModuleCooldown(module);
                return true;
            }
            if (module.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                ModuleShootData shootData = this.m_moduleData[module];
                if (shootData.chargeFired)
                    return true;
                float num4 = 1f;
                if (this.m_owner is PlayerController)
                    num4 = (this.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ChargeAmountMultiplier);
                shootData.chargeTime += BraveTime.DeltaTime * num4;
                if ((double) module.maxChargeTime > 0.0 && (double) shootData.chargeTime >= (double) module.maxChargeTime && canAttack && !this.m_cachedIsGunBlocked)
                {
                    if (playEffects)
                    {
                        if (!this.usesContinuousFireAnimation)
                            this.Play(this.shootAnimation);
                        this.HandleShootEffects(module);
                        if (shootData.lastChargeProjectile != null)
                        {
                            if (GameManager.AUDIO_ENABLED)
                            {
                                int num5 = module.chargeProjectiles.IndexOf(shootData.lastChargeProjectile);
                                string str = !module.IsFinalShot(this.m_moduleData[module], this.m_owner) || this.OverrideFinaleAudio ? "Play_WPN_gun_shot_" : "Play_WPN_gun_finale_";
                                if (GameManager.AUDIO_ENABLED && (!this.isAudioLoop || !this.m_isAudioLooping))
                                {
                                    int num6 = (int) AkSoundEngine.PostEvent($"{str}{num5 + 1:D2}", this.gameObject);
                                    this.m_isAudioLooping = true;
                                }
                                if (shootData.lastChargeProjectile.UsesAdditionalWwiseEvent)
                                {
                                    int num7 = (int) AkSoundEngine.PostEvent(shootData.lastChargeProjectile.AdditionalWwiseEvent, this.gameObject);
                                }
                            }
                            this.HandleChargeEffects(shootData.lastChargeProjectile, (ProjectileModule.ChargeProjectile) null);
                            this.EndChargeIntensity();
                            shootData.lastChargeProjectile = (ProjectileModule.ChargeProjectile) null;
                        }
                        if (this.doesScreenShake)
                            this.DoScreenShake();
                    }
                    this.ShootSingleProjectile(module, overrideProjectileData);
                    this.DecrementAmmoCost(module);
                    this.TriggerModuleCooldown(module);
                    shootData.chargeFired = true;
                    return true;
                }
                if (playEffects)
                {
                    ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(shootData.chargeTime);
                    this.PlayIfExistsAndNotPlaying(this.chargeAnimation);
                    if (chargeProjectile != shootData.lastChargeProjectile)
                    {
                        if (GameManager.AUDIO_ENABLED)
                        {
                            int num8 = module.chargeProjectiles.IndexOf(chargeProjectile);
                            if (GameManager.AUDIO_ENABLED)
                            {
                                int num9 = (int) AkSoundEngine.PostEvent($"Play_WPN_gun_charge_{num8 + 2:D2}", this.gameObject);
                            }
                        }
                        this.HandleChargeEffects(shootData.lastChargeProjectile, chargeProjectile);
                        shootData.lastChargeProjectile = chargeProjectile;
                    }
                    this.HandleChargeIntensity(module, shootData);
                    if (this.CurrentOwner is PlayerController)
                        (this.CurrentOwner as PlayerController).DoSustainedVibration(chargeProjectile == null || !(bool) (UnityEngine.Object) chargeProjectile.Projectile ? Vibration.Strength.UltraLight : Vibration.Strength.Light);
                }
                return false;
            }
            if (module.shootStyle != ProjectileModule.ShootStyle.Beam || this.m_cachedIsGunBlocked)
                return false;
            ModuleShootData moduleShootData = this.m_moduleData[module];
            if (canAttack && !this.m_activeBeams.Contains(moduleShootData))
                this.HandleSpecificInitialGunShoot(module, overrideProjectileData, playEffects: playEffects);
            else if (moduleShootData != null && (bool) (UnityEngine.Object) moduleShootData.beam)
            {
                BeamController beam = moduleShootData.beam;
                beam.Direction = (Vector2) this.GetBeamAimDirection(moduleShootData.angleForShot);
                beam.Origin = (Vector2) this.m_unroundedBarrelPosition;
                if (beam.knocksShooterBack && moduleShootData.beamKnockbackID >= 0)
                    this.m_owner.knockbackDoer.UpdateContinuousKnockback(-beam.Direction, beam.knockbackStrength, moduleShootData.beamKnockbackID);
                if (beam.ShouldUseAmmo)
                {
                    float num = !(this.m_owner is PlayerController) ? 1f : (this.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.RateOfFire);
                    this.m_fractionalAmmoUsage += BraveTime.DeltaTime * (float) module.ammoCost * num;
                    if ((double) this.m_fractionalAmmoUsage > 1.0)
                    {
                        this.ammo = Math.Max(0, this.ammo - (int) ((double) this.m_fractionalAmmoUsage / 1.0));
                        if (module.numberOfShotsInClip > 0)
                        {
                            moduleShootData.numberShotsFired += (int) ((double) this.m_fractionalAmmoUsage / 1.0);
                            if (module.GetModNumberOfShotsInClip(this.CurrentOwner) > 0 && moduleShootData.numberShotsFired >= module.GetModNumberOfShotsInClip(this.CurrentOwner))
                                moduleShootData.needsReload = true;
                        }
                        this.DecrementCustomAmmunitions((int) ((double) this.m_fractionalAmmoUsage / 1.0));
                        this.m_fractionalAmmoUsage %= 1f;
                    }
                }
            }
            return true;
        }

        private bool HandleEndGunShoot(
            ProjectileModule module,
            bool canAttack = true,
            ProjectileData overrideProjectileData = null)
        {
            return !this.m_moduleData[module].needsReload && !this.m_moduleData[module].onCooldown && (!this.UsesRechargeLikeActiveItem || (double) this.m_remainingActiveCooldownAmount <= 0.0) && this.HandleSpecificEndGunShoot(module, canAttack, overrideProjectileData);
        }

        private bool HandleEndGunShoot(
            ProjectileVolleyData Volley,
            bool canAttack = true,
            ProjectileData overrideProjectileData = null)
        {
            bool playEffects = true;
            bool flag = false;
            foreach (ProjectileModule projectile in Volley.projectiles)
            {
                if (!this.m_moduleData[projectile].needsReload && !this.m_moduleData[projectile].onCooldown && (!this.UsesRechargeLikeActiveItem || (double) this.m_remainingActiveCooldownAmount <= 0.0))
                {
                    flag |= this.HandleSpecificEndGunShoot(projectile, canAttack, overrideProjectileData, playEffects);
                    if (flag)
                        playEffects = false;
                }
            }
            return flag;
        }

        private bool HandleSpecificEndGunShoot(
            ProjectileModule module,
            bool canAttack = true,
            ProjectileData overrideProjectileData = null,
            bool playEffects = true)
        {
            if (module.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                ModuleShootData moduleShootData = this.m_moduleData[module];
                if (!moduleShootData.chargeFired)
                {
                    float num1 = 1f;
                    if (this.m_owner is PlayerController)
                        num1 = (this.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ChargeAmountMultiplier);
                    moduleShootData.chargeTime += BraveTime.DeltaTime * num1;
                    ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(moduleShootData.chargeTime);
                    if (chargeProjectile != null && (UnityEngine.Object) chargeProjectile.Projectile != (UnityEngine.Object) null && canAttack && !this.m_cachedIsGunBlocked)
                    {
                        if (playEffects)
                        {
                            if (!this.usesContinuousFireAnimation)
                                this.HandleShootAnimation(module);
                            if (GameManager.AUDIO_ENABLED)
                            {
                                int num2 = module.chargeProjectiles.IndexOf(moduleShootData.lastChargeProjectile);
                                string str = !module.IsFinalShot(this.m_moduleData[module], this.m_owner) || this.OverrideFinaleAudio ? "Play_WPN_gun_shot_" : "Play_WPN_gun_finale_";
                                if (GameManager.AUDIO_ENABLED && (!this.isAudioLoop || !this.m_isAudioLooping) && !this.PreventNormalFireAudio)
                                {
                                    if (this.PickupObjectId == GlobalItemIds.Starpew && str == "Play_WPN_gun_shot_" && (double) moduleShootData.chargeTime >= 2.0)
                                    {
                                        int num3 = (int) AkSoundEngine.PostEvent("Play_WPN_Starpew_Blast_01", this.gameObject);
                                    }
                                    else
                                    {
                                        int num4 = (int) AkSoundEngine.PostEvent($"{str}{num2 + 1:D2}", this.gameObject);
                                    }
                                    this.m_isAudioLooping = true;
                                }
                                if (moduleShootData.lastChargeProjectile != null && moduleShootData.lastChargeProjectile.UsesAdditionalWwiseEvent)
                                {
                                    int num5 = (int) AkSoundEngine.PostEvent(moduleShootData.lastChargeProjectile.AdditionalWwiseEvent, this.gameObject);
                                }
                            }
                            this.HandleShootEffects(module);
                            if (moduleShootData.lastChargeProjectile != null)
                            {
                                this.HandleChargeEffects(moduleShootData.lastChargeProjectile, (ProjectileModule.ChargeProjectile) null);
                                this.EndChargeIntensity();
                                moduleShootData.lastChargeProjectile = (ProjectileModule.ChargeProjectile) null;
                            }
                            if (this.doesScreenShake)
                                this.DoScreenShake();
                        }
                        else if (moduleShootData.lastChargeProjectile != null)
                        {
                            this.HandleChargeEffects(moduleShootData.lastChargeProjectile, (ProjectileModule.ChargeProjectile) null);
                            this.EndChargeIntensity();
                            moduleShootData.lastChargeProjectile = (ProjectileModule.ChargeProjectile) null;
                        }
                        this.ShootSingleProjectile(module, overrideProjectileData);
                        this.DecrementAmmoCost(module);
                        this.TriggerModuleCooldown(module);
                        moduleShootData.chargeFired = true;
                        return true;
                    }
                    if (playEffects)
                    {
                        if (!string.IsNullOrEmpty(this.dischargeAnimation))
                            this.Play(this.dischargeAnimation);
                        else
                            this.PlayIdleAnimation();
                        if (moduleShootData.lastChargeProjectile != null)
                        {
                            this.HandleChargeEffects(moduleShootData.lastChargeProjectile, (ProjectileModule.ChargeProjectile) null);
                            this.EndChargeIntensity();
                            moduleShootData.lastChargeProjectile = (ProjectileModule.ChargeProjectile) null;
                        }
                    }
                    if (module.triggerCooldownForAnyChargeAmount)
                        this.TriggerModuleCooldown(module);
                    return false;
                }
            }
            else if (module.shootStyle == ProjectileModule.ShootStyle.Beam)
            {
                if (playEffects)
                    this.PlayIdleAnimation();
                ModuleShootData moduleShootData = this.m_moduleData[module];
                if ((bool) (UnityEngine.Object) moduleShootData.beam)
                {
                    if (moduleShootData.beam.knocksShooterBack && moduleShootData.beamKnockbackID >= 0)
                    {
                        this.m_owner.knockbackDoer.EndContinuousKnockback(moduleShootData.beamKnockbackID);
                        moduleShootData.beamKnockbackID = -1;
                    }
                    if (this.doesScreenShake)
                        GameManager.Instance.MainCameraController.StopContinuousScreenShake((Component) this);
                    moduleShootData.beam.CeaseAttack();
                    moduleShootData.beam = (BeamController) null;
                    this.m_activeBeams.Remove(moduleShootData);
                }
                return true;
            }
            return false;
        }

        public void ForceFireProjectile(Projectile targetProjectile)
        {
            ProjectileModule mod = (ProjectileModule) null;
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index1 = 0; index1 < this.Volley.projectiles.Count; ++index1)
                {
                    for (int index2 = 0; index2 < this.Volley.projectiles[index2].projectiles.Count; ++index2)
                    {
                        if (targetProjectile.name.Contains(this.Volley.projectiles[index2].projectiles[index2].name))
                        {
                            mod = this.Volley.projectiles[index2];
                            break;
                        }
                    }
                    if (mod != null)
                        break;
                }
            }
            else
            {
                for (int index = 0; index < this.singleModule.projectiles.Count; ++index)
                {
                    if (targetProjectile.name.Contains(this.singleModule.projectiles[index].name))
                    {
                        mod = this.singleModule;
                        break;
                    }
                }
            }
            if (mod == null)
                return;
            this.ShootSingleProjectile(mod);
        }

        private void DecrementCustomAmmunitions(int ammoCost)
        {
            for (int index = 0; index < this.m_customAmmunitions.Count; ++index)
            {
                this.m_customAmmunitions[index].ShotsRemaining -= ammoCost;
                if (this.m_customAmmunitions[index].ShotsRemaining <= 0)
                {
                    this.m_customAmmunitions.RemoveAt(index);
                    --index;
                }
            }
        }

        private void ApplyCustomAmmunitionsToProjectile(Projectile target)
        {
            for (int index = 0; index < this.m_customAmmunitions.Count; ++index)
                this.m_customAmmunitions[index].HandleAmmunition(target, this);
        }

        private void ShootSingleProjectile(
            ProjectileModule mod,
            ProjectileData overrideProjectileData = null,
            GameObject overrideBulletObject = null)
        {
            PlayerController owner1 = this.m_owner as PlayerController;
            AIActor owner2 = this.m_owner as AIActor;
            Projectile projectile = (Projectile) null;
            ProjectileModule.ChargeProjectile currentChargeProjectile = (ProjectileModule.ChargeProjectile) null;
            if ((bool) (UnityEngine.Object) overrideBulletObject)
                projectile = overrideBulletObject.GetComponent<Projectile>();
            else if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                currentChargeProjectile = mod.GetChargeProjectile(this.m_moduleData[mod].chargeTime);
                if (currentChargeProjectile != null)
                {
                    projectile = currentChargeProjectile.Projectile;
                    projectile.pierceMinorBreakables = true;
                }
            }
            else
                projectile = mod.GetCurrentProjectile(this.m_moduleData[mod], this.CurrentOwner);
            if (!(bool) (UnityEngine.Object) projectile)
            {
                ++this.m_moduleData[mod].numberShotsFired;
                ++this.m_moduleData[mod].numberShotsFiredThisBurst;
                if (this.m_moduleData[mod].numberShotsActiveReload > 0)
                    --this.m_moduleData[mod].numberShotsActiveReload;
                if (mod.GetModNumberOfShotsInClip(this.CurrentOwner) > 0 && this.m_moduleData[mod].numberShotsFired >= mod.GetModNumberOfShotsInClip(this.CurrentOwner))
                    this.m_moduleData[mod].needsReload = true;
                if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
                    return;
                mod.IncrementShootCount();
            }
            else
            {
                if ((bool) (UnityEngine.Object) owner1 && owner1.OnPreFireProjectileModifier != null)
                    projectile = owner1.OnPreFireProjectileModifier(this, projectile);
                if (this.m_isCritting && (bool) (UnityEngine.Object) this.CriticalReplacementProjectile)
                    projectile = this.CriticalReplacementProjectile;
                if (this.OnPreFireProjectileModifier != null)
                    projectile = this.OnPreFireProjectileModifier(this, projectile, mod);
                if (GameManager.Instance.InTutorial && (UnityEngine.Object) owner1 != (UnityEngine.Object) null)
                    GameManager.BroadcastRoomTalkDoerFsmEvent("playerFiredGun");
                Vector3 vector3 = this.barrelOffset.position;
                vector3 = new Vector3(vector3.x, vector3.y, -1f);
                float num1 = !((UnityEngine.Object) owner1 != (UnityEngine.Object) null) ? 1f : owner1.stats.GetStatValue(PlayerStats.StatType.Accuracy);
                float varianceMultiplier = !(this.m_owner is DumbGunShooter) || !(this.m_owner as DumbGunShooter).overridesInaccuracy ? num1 : (this.m_owner as DumbGunShooter).inaccuracyFraction;
                float angleForShot = mod.GetAngleForShot(this.m_moduleData[mod].alternateAngleSign, varianceMultiplier);
                if (this.m_moduleData[mod].numberShotsActiveReload > 0 && this.activeReloadData.usesOverrideAngleVariance)
                    angleForShot = mod.GetAngleForShot(varianceMultiplier: varianceMultiplier, overrideAngleVariance: new float?(this.activeReloadData.overrideAngleVariance));
                if (mod.alternateAngle)
                    this.m_moduleData[mod].alternateAngleSign *= -1f;
                if (this.LockedHorizontalOnCharge && (double) this.LockedHorizontalCenterFireOffset >= 0.0)
                    vector3 = (Vector3) (this.m_owner.specRigidbody.HitboxPixelCollider.UnitCenter + BraveMathCollege.DegreesToVector(this.gunAngle, this.LockedHorizontalCenterFireOffset));
                GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile.gameObject, vector3 + Quaternion.Euler(0.0f, 0.0f, this.gunAngle) * mod.positionOffset, Quaternion.Euler(0.0f, 0.0f, this.gunAngle + angleForShot));
                Projectile component1 = gameObject1.GetComponent<Projectile>();
                this.LastProjectile = component1;
                component1.Owner = this.m_owner;
                component1.Shooter = this.m_owner.specRigidbody;
                component1.baseData.damage += (float) this.damageModifier;
                component1.Inverted = mod.inverted;
                if (this.m_owner is PlayerController && (this.LocalActiveReload || owner1.IsPrimaryPlayer && Gun.ActiveReloadActivated || !owner1.IsPrimaryPlayer && Gun.ActiveReloadActivatedPlayerTwo))
                    component1.baseData.damage *= this.m_moduleData[mod].activeReloadDamageModifier;
                if ((bool) (UnityEngine.Object) this.m_owner.aiShooter)
                    component1.collidesWithEnemies = this.m_owner.aiShooter.CanShootOtherEnemies;
                if (this.rampBullets)
                {
                    component1.Ramp(this.rampStartHeight, this.rampTime);
                    TrailController componentInChildren = gameObject1.GetComponentInChildren<TrailController>();
                    if ((bool) (UnityEngine.Object) componentInChildren)
                    {
                        componentInChildren.rampHeight = true;
                        componentInChildren.rampStartHeight = this.rampStartHeight;
                        componentInChildren.rampTime = this.rampTime;
                    }
                }
                if (this.m_owner is PlayerController)
                {
                    PlayerStats stats = owner1.stats;
                    component1.baseData.damage *= stats.GetStatValue(PlayerStats.StatType.Damage);
                    component1.baseData.speed *= stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                    component1.baseData.force *= stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                    component1.baseData.range *= stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
                    if (owner1.inventory.DualWielding)
                        component1.baseData.damage *= Gun.s_DualWieldFactor;
                    if (this.CanSneakAttack && owner1.IsStealthed)
                        component1.baseData.damage *= this.SneakAttackDamageMultiplier;
                    if (this.m_isCritting)
                    {
                        component1.baseData.damage *= this.CriticalDamageMultiplier;
                        component1.IsCritical = true;
                    }
                    if (this.UsesBossDamageModifier)
                        component1.BossDamageMultiplier = (double) this.CustomBossDamageModifier < 0.0 ? 0.8f : this.CustomBossDamageModifier;
                }
                if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null && this.Volley.UsesShotgunStyleVelocityRandomizer)
                    component1.baseData.speed *= this.Volley.GetVolleySpeedMod();
                if ((UnityEngine.Object) owner2 != (UnityEngine.Object) null && owner2.IsBlackPhantom)
                    component1.baseData.speed *= owner2.BlackPhantomProperties.BulletSpeedMultiplier;
                if (this.m_moduleData[mod].numberShotsActiveReload > 0)
                {
                    if (!this.activeReloadData.ActiveReloadStacks)
                        component1.baseData.damage *= this.activeReloadData.damageMultiply;
                    component1.baseData.force *= this.activeReloadData.knockbackMultiply;
                }
                if (overrideProjectileData != null)
                    component1.baseData.SetAll(overrideProjectileData);
                this.LastShotIndex = this.m_moduleData[mod].numberShotsFired;
                component1.PlayerProjectileSourceGameTimeslice = UnityEngine.Time.time;
                if (!this.IsMinusOneGun)
                {
                    this.ApplyCustomAmmunitionsToProjectile(component1);
                    if (this.m_owner is PlayerController)
                        owner1.DoPostProcessProjectile(component1);
                    if (this.PostProcessProjectile != null)
                        this.PostProcessProjectile(component1);
                }
                if (mod.mirror)
                {
                    GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile.gameObject, vector3 + Quaternion.Euler(0.0f, 0.0f, this.gunAngle) * mod.InversePositionOffset, Quaternion.Euler(0.0f, 0.0f, this.gunAngle - angleForShot));
                    Projectile component2 = gameObject2.GetComponent<Projectile>();
                    this.LastProjectile = component2;
                    component2.Inverted = true;
                    component2.Owner = this.m_owner;
                    component2.Shooter = this.m_owner.specRigidbody;
                    if ((bool) (UnityEngine.Object) this.m_owner.aiShooter)
                        component2.collidesWithEnemies = this.m_owner.aiShooter.CanShootOtherEnemies;
                    if (this.rampBullets)
                    {
                        component2.Ramp(this.rampStartHeight, this.rampTime);
                        TrailController componentInChildren = gameObject2.GetComponentInChildren<TrailController>();
                        if ((bool) (UnityEngine.Object) componentInChildren)
                        {
                            componentInChildren.rampHeight = true;
                            componentInChildren.rampStartHeight = this.rampStartHeight;
                            componentInChildren.rampTime = this.rampTime;
                        }
                    }
                    component2.PlayerProjectileSourceGameTimeslice = UnityEngine.Time.time;
                    if (!this.IsMinusOneGun)
                    {
                        this.ApplyCustomAmmunitionsToProjectile(component2);
                        if (this.m_owner is PlayerController)
                            owner1.DoPostProcessProjectile(component2);
                        if (this.PostProcessProjectile != null)
                            this.PostProcessProjectile(component2);
                    }
                    component2.baseData.SetAll(component1.baseData);
                    component2.IsCritical = component1.IsCritical;
                }
                if ((UnityEngine.Object) this.modifiedFinalVolley != (UnityEngine.Object) null && mod == this.modifiedFinalVolley.projectiles[0])
                    mod = this.DefaultModule;
                if (currentChargeProjectile != null && currentChargeProjectile.ReflectsIncomingBullets && (bool) (UnityEngine.Object) this.barrelOffset)
                {
                    if (currentChargeProjectile.MegaReflection)
                    {
                        if (PassiveReflectItem.ReflectBulletsInRange(this.barrelOffset.position.XY(), 2.66f, true, this.m_owner, 30f, 1.25f, 1.5f, true) > 0)
                        {
                            int num2 = (int) AkSoundEngine.PostEvent("Play_WPN_duelingpistol_impact_01", this.gameObject);
                            int num3 = (int) AkSoundEngine.PostEvent("Play_PET_junk_punch_01", this.gameObject);
                        }
                    }
                    else if (PassiveReflectItem.ReflectBulletsInRange(this.barrelOffset.position.XY(), 2.66f, true, this.m_owner, 30f, applyPostprocess: true) > 0)
                    {
                        int num4 = (int) AkSoundEngine.PostEvent("Play_WPN_duelingpistol_impact_01", this.gameObject);
                        int num5 = (int) AkSoundEngine.PostEvent("Play_PET_junk_punch_01", this.gameObject);
                    }
                }
                this.IncrementModuleFireCountAndMarkReload(mod, currentChargeProjectile);
                if (!(this.m_owner is PlayerController))
                    return;
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.BULLETS_FIRED, 1f);
                if (!((UnityEngine.Object) projectile != (UnityEngine.Object) null) || !projectile.AppliesKnockbackToPlayer)
                    return;
                owner1.knockbackDoer.ApplyKnockback(-1f * BraveMathCollege.DegreesToVector(this.gunAngle), projectile.PlayerKnockbackForce);
            }
        }

        public void TriggerActiveCooldown()
        {
            if (!this.UsesRechargeLikeActiveItem)
                return;
            this.RemainingActiveCooldownAmount = this.ActiveItemStyleRechargeAmount;
        }

        public void ApplyActiveCooldownDamage(PlayerController Owner, float damageDone)
        {
            if (!this.UsesRechargeLikeActiveItem || (UnityEngine.Object) Owner.CurrentGun == (UnityEngine.Object) this && !PlayerItem.AllowDamageCooldownOnActive)
                return;
            float num = 1f;
            GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
            if (loadedLevelDefinition != null)
                num /= loadedLevelDefinition.enemyHealthMultiplier;
            damageDone *= num;
            if (this.ModifyActiveCooldownDamage != null)
                damageDone = this.ModifyActiveCooldownDamage(damageDone);
            this.RemainingActiveCooldownAmount = Mathf.Max(0.0f, this.m_remainingActiveCooldownAmount - damageDone);
        }

        private void TriggerModuleCooldown(ProjectileModule mod)
        {
            if (this.UsesRechargeLikeActiveItem)
                this.TriggerActiveCooldown();
            GameManager.Instance.StartCoroutine(this.HandleModuleCooldown(mod));
        }

        [DebuggerHidden]
        private IEnumerator HandleModuleCooldown(ProjectileModule mod)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Gun__HandleModuleCooldownc__Iterator3()
            {
                mod = mod,
                _this = this
            };
        }

        private void BeginFiringBeam(ProjectileModule mod)
        {
            GameObject gameObject = SpawnManager.SpawnProjectile(mod.GetCurrentProjectile(this.m_moduleData[mod], this.CurrentOwner).gameObject, this.m_unroundedBarrelPosition, Quaternion.identity);
            Projectile component1 = gameObject.GetComponent<Projectile>();
            component1.Owner = this.CurrentOwner;
            this.LastProjectile = component1;
            BeamController component2 = gameObject.GetComponent<BeamController>();
            component2.Owner = this.m_owner;
            component2.Gun = this;
            component2.HitsPlayers = this.m_owner is AIActor;
            component2.HitsEnemies = this.m_owner is PlayerController;
            if (this.m_owner is PlayerController)
            {
                PlayerStats stats = (this.m_owner as PlayerController).stats;
                component1.baseData.damage *= stats.GetStatValue(PlayerStats.StatType.Damage);
                component1.baseData.speed *= stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                component1.baseData.force *= stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
                component1.baseData.range *= stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
                if ((this.m_owner as PlayerController).inventory.DualWielding)
                    component1.baseData.damage *= Gun.s_DualWieldFactor;
                if (this.UsesBossDamageModifier)
                    component1.BossDamageMultiplier = (double) this.CustomBossDamageModifier < 0.0 ? 0.8f : this.CustomBossDamageModifier;
            }
            if (this.doesScreenShake && (UnityEngine.Object) GameManager.Instance.MainCameraController != (UnityEngine.Object) null)
                GameManager.Instance.MainCameraController.DoContinuousScreenShake(this.gunScreenShake, (Component) this);
            float varianceMultiplier = !(this.m_owner is PlayerController) ? 1f : (this.m_owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.Accuracy);
            float angleForShot = mod.GetAngleForShot(this.m_moduleData[mod].alternateAngleSign, varianceMultiplier);
            Vector3 beamAimDirection = this.GetBeamAimDirection(angleForShot);
            component2.Direction = (Vector2) beamAimDirection;
            component2.Origin = (Vector2) this.m_unroundedBarrelPosition;
            ModuleShootData moduleShootData = this.m_moduleData[mod];
            moduleShootData.beam = component2;
            moduleShootData.angleForShot = angleForShot;
            KnockbackDoer knockbackDoer = this.m_owner.knockbackDoer;
            moduleShootData.beamKnockbackID = -1;
            if (component2.knocksShooterBack)
                moduleShootData.beamKnockbackID = knockbackDoer.ApplyContinuousKnockback((Vector2) -beamAimDirection, component2.knockbackStrength);
            this.m_activeBeams.Add(moduleShootData);
        }

        private void ClearBeams()
        {
            if (this.m_activeBeams.Count <= 0)
                return;
            for (int index = 0; index < this.m_activeBeams.Count; ++index)
            {
                BeamController beam = this.m_activeBeams[index].beam;
                if ((bool) (UnityEngine.Object) beam && beam.knocksShooterBack)
                {
                    this.m_owner.knockbackDoer.EndContinuousKnockback(this.m_activeBeams[index].beamKnockbackID);
                    this.m_activeBeams[index].beamKnockbackID = -1;
                }
                if (this.doesScreenShake && (UnityEngine.Object) GameManager.Instance.MainCameraController != (UnityEngine.Object) null)
                    GameManager.Instance.MainCameraController.StopContinuousScreenShake((Component) this);
                if ((bool) (UnityEngine.Object) beam)
                    beam.CeaseAttack();
            }
            this.m_activeBeams.Clear();
            if (GameManager.AUDIO_ENABLED)
            {
                int num = (int) AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", this.gameObject);
            }
            this.m_isAudioLooping = false;
        }

        public void ForceImmediateReload(bool forceImmediate = false)
        {
            if (this.gameObject.activeSelf)
                this.ClearBeams();
            if (this.IsReloading)
            {
                this.FinishReload(isImmediate: forceImmediate);
            }
            else
            {
                if (!this.HaveAmmoToReloadWith())
                    return;
                this.FinishReload(silent: true, isImmediate: forceImmediate);
            }
        }

        private void OnActiveReloadPressed(PlayerController p, Gun g, bool actualPress)
        {
            if (!this.m_isReloading && (double) this.reloadTime >= 0.0)
                return;
            PlayerController owner = this.m_owner as PlayerController;
            if (!(bool) (UnityEngine.Object) owner || !actualPress && false || !this.LocalActiveReload && (!owner.IsPrimaryPlayer || !Gun.ActiveReloadActivated) && (owner.IsPrimaryPlayer || !Gun.ActiveReloadActivatedPlayerTwo) || !this.m_canAttemptActiveReload || GameUIRoot.Instance.GetReloadBarForPlayer(this.m_owner as PlayerController).IsActiveReloadGracePeriod())
                return;
            if (GameUIRoot.Instance.AttemptActiveReload(this.m_owner as PlayerController))
            {
                this.OnActiveReloadSuccess();
                GunFormeSynergyProcessor component1 = this.GetComponent<GunFormeSynergyProcessor>();
                if ((bool) (UnityEngine.Object) component1)
                    component1.JustActiveReloaded = true;
                ChamberGunProcessor component2 = this.GetComponent<ChamberGunProcessor>();
                if ((bool) (UnityEngine.Object) component2)
                    component2.JustActiveReloaded = true;
            }
            this.m_canAttemptActiveReload = false;
            this.OnReloadPressed -= new Action<PlayerController, Gun, bool>(this.OnActiveReloadPressed);
        }

        private bool ReloadIsFree()
        {
            return this.GoopReloadsFree && (UnityEngine.Object) this.m_owner.CurrentGoop != (UnityEngine.Object) null;
        }

        public bool Reload()
        {
            if (this.IsHeroSword && !this.HeroSwordDoesntBlank && !this.m_isCurrentlyFiring && !this.m_anim.IsPlaying(this.reloadAnimation))
            {
                SilencerInstance.DestroyBulletsInRange(this.m_owner.specRigidbody.GetUnitCenter(ColliderType.HitBox), this.blankReloadRadius, true, false);
                this.Play(this.reloadAnimation);
                return false;
            }
            this.m_continuousAttackTime = 0.0f;
            this.ClearBurstState();
            if (this.m_isReloading || (double) this.reloadTime < 0.0)
            {
                if (this.m_canAttemptActiveReload)
                    this.OnActiveReloadPressed(this.m_owner as PlayerController, this, true);
                return false;
            }
            bool flag1 = false;
            bool flag2 = this.ReloadIsFree();
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    if (this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired != 0)
                    {
                        flag1 = true;
                        break;
                    }
                }
                if (this.ammo == 0 && !flag2)
                    flag1 = false;
            }
            else
            {
                if (this.m_moduleData[this.singleModule].numberShotsFired != 0)
                    flag1 = true;
                if (this.ClipShotsRemaining == this.ammo && !flag2)
                    flag1 = false;
            }
            if (flag1)
                flag1 = flag2 || this.HaveAmmoToReloadWith();
            if (flag2)
            {
                this.GainAmmo(Mathf.Max(0, this.ClipCapacity - this.ClipShotsRemaining));
                DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(this.m_owner.CenterPosition, 2f);
            }
            if (!flag1)
                return false;
            if (!this.m_isReloading && this.IsCharging)
                this.CeaseAttack(false);
            this.m_isReloading = true;
            this.m_canAttemptActiveReload = true;
            this.m_reloadElapsed = 0.0f;
            this.m_hasDoneSingleReloadBlankEffect = false;
            this.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.OnActiveReloadPressed);
            if (this.ClipShotsRemaining == 0 && this.OnAutoReload != null)
                this.OnAutoReload(this.CurrentOwner as PlayerController, this);
            if (GameManager.AUDIO_ENABLED)
            {
                int num = (int) AkSoundEngine.PostEvent("Play_WPN_gun_reload_01", this.gameObject);
            }
            if ((bool) (UnityEngine.Object) this.reloadOffset)
            {
                float zRotation = !this.m_owner.SpriteFlipped ? this.gunAngle + this.reloadOffset.transform.localEulerAngles.z : this.gunAngle - 180f - this.reloadOffset.transform.localEulerAngles.z;
                this.reloadOffset.localScale = Vector3.one;
                (!this.IsEmpty || this.emptyReloadEffects.type == VFXPoolType.None ? this.reloadEffects : this.emptyReloadEffects).SpawnAtPosition(this.reloadOffset.position, zRotation, this.reloadOffset, new Vector2?(Vector2.zero), new Vector2?(Vector2.zero), new float?(0.0375f), true);
                if (this.m_owner.SpriteFlipped)
                    this.reloadOffset.localScale = new Vector3(-1f, 1f, 1f);
            }
            if (this.m_owner is PlayerController)
            {
                PlayerController owner = this.m_owner as PlayerController;
                if ((UnityEngine.Object) this.OptionalReloadVolley != (UnityEngine.Object) null)
                {
                    this.RawFireVolley(this.OptionalReloadVolley);
                    this.ClearOptionalReloadVolleyCooldownAndReloadData();
                }
                if (owner.OnReloadedGun != null)
                    owner.OnReloadedGun(owner, this);
                if ((bool) (UnityEngine.Object) this.ObjectToInstantiateOnReload)
                    this.CreateAmp();
                if ((double) this.AdjustedReloadTime > 0.10000000149011612)
                {
                    Vector3 offset = new Vector3(0.1f, (float) ((double) this.m_owner.SpriteDimensions.y / 2.0 + 0.25), 0.0f);
                    GameUIRoot.Instance.StartPlayerReloadBar(owner, offset, this.AdjustedReloadTime);
                }
            }
            if (this.m_isReloading)
            {
                if ((double) this.AdjustedReloadTime > 0.0)
                    this.StartCoroutine(this.HandleReload());
                else
                    this.FinishReload(isImmediate: true);
            }
            this.m_reloadWhenDoneFiring = false;
            return true;
        }

        public void HandleDodgeroll(float fullDodgeTime)
        {
            if (string.IsNullOrEmpty(this.dodgeAnimation))
                return;
            if (this.usesDirectionalAnimator)
            {
                AIAnimator aiAnimator = this.aiAnimator;
                string dodgeAnimation = this.dodgeAnimation;
                float num = fullDodgeTime;
                string name = dodgeAnimation;
                double warpClipDuration = (double) num;
                aiAnimator.PlayUntilFinished(name, warpClipDuration: (float) warpClipDuration);
            }
            else
            {
                if (!((UnityEngine.Object) this.m_anim != (UnityEngine.Object) null))
                    return;
                tk2dSpriteAnimationClip clipByName = this.m_anim.GetClipByName(this.dodgeAnimation);
                if (clipByName == null)
                    return;
                float overrideFps = (float) clipByName.frames.Length / fullDodgeTime;
                this.m_anim.Play(clipByName, 0.0f, overrideFps);
            }
        }

        private void ClearBurstState()
        {
            this.m_midBurstFire = false;
            this.m_continueBurstInUpdate = false;
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    if (this.Volley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Burst)
                        this.m_moduleData[this.Volley.projectiles[index]].numberShotsFiredThisBurst = 0;
                }
            }
            else
            {
                if (this.singleModule.shootStyle != ProjectileModule.ShootStyle.Burst)
                    return;
                this.m_moduleData[this.singleModule].numberShotsFiredThisBurst = 0;
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleReload()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Gun__HandleReloadc__Iterator4()
            {
                _this = this
            };
        }

        public void MoveBulletsIntoClip(int numBullets)
        {
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    int num = Mathf.Min(Mathf.Min(numBullets, this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired), this.ammo - (this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired));
                    if (num > 0)
                    {
                        this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired -= num;
                        this.m_moduleData[this.Volley.projectiles[index]].needsReload = false;
                    }
                }
            }
            else
            {
                int num = Mathf.Min(Mathf.Min(numBullets, this.m_moduleData[this.singleModule].numberShotsFired), this.ammo - (this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner) - this.m_moduleData[this.singleModule].numberShotsFired));
                if (num <= 0)
                    return;
                this.m_moduleData[this.singleModule].numberShotsFired -= num;
                this.m_moduleData[this.singleModule].needsReload = false;
            }
        }

        private void FinishReload(bool activeReload = false, bool silent = false, bool isImmediate = false)
        {
            if (isImmediate)
            {
                string name = string.IsNullOrEmpty(this.emptyReloadAnimation) || !this.IsEmpty ? this.reloadAnimation : this.emptyReloadAnimation;
                if (this.IsTrickGun && !this.m_hasSwappedTrickGunsThisCycle)
                {
                    this.m_hasSwappedTrickGunsThisCycle = true;
                    if (!string.IsNullOrEmpty(this.gunSwitchGroup) && !string.IsNullOrEmpty(this.alternateSwitchGroup))
                    {
                        BraveUtility.Swap<string>(ref this.gunSwitchGroup, ref this.alternateSwitchGroup);
                        int num = (int) AkSoundEngine.SetSwitch("WPN_Guns", this.gunSwitchGroup, this.gameObject);
                    }
                    tk2dSpriteAnimationClip clipByName = this.m_anim.GetClipByName(name);
                    this.m_defaultSpriteID = clipByName.frames[clipByName.frames.Length - 1].spriteId;
                }
            }
            if (!silent)
            {
                if (this.IsTrickGun)
                {
                    BraveUtility.Swap<string>(ref this.reloadAnimation, ref this.alternateReloadAnimation);
                    BraveUtility.Swap<string>(ref this.shootAnimation, ref this.alternateShootAnimation);
                    if (!string.IsNullOrEmpty(this.alternateIdleAnimation))
                        BraveUtility.Swap<string>(ref this.idleAnimation, ref this.alternateIdleAnimation);
                    BraveUtility.Swap<ProjectileVolleyData>(ref this.rawVolley, ref this.alternateVolley);
                    (this.CurrentOwner as PlayerController).stats.RecalculateStats(this.CurrentOwner as PlayerController);
                }
                if (this.IsTrickGun && this.TrickGunAlternatesHandedness)
                {
                    if (this.Handedness == GunHandedness.OneHanded)
                    {
                        this.m_cachedGunHandedness = new GunHandedness?(GunHandedness.TwoHanded);
                        this.carryPixelOffset = new IntVector2(10, 0);
                    }
                    else if (this.Handedness == GunHandedness.TwoHanded)
                    {
                        this.m_cachedGunHandedness = new GunHandedness?(GunHandedness.OneHanded);
                        this.carryPixelOffset = new IntVector2(0, 0);
                    }
                    (this.m_owner as PlayerController).ProcessHandAttachment();
                }
            }
            this.m_hasSwappedTrickGunsThisCycle = false;
            this.HasFiredHolsterShot = false;
            this.HasFiredReloadSynergy = false;
            this.OnReloadPressed -= new Action<PlayerController, Gun, bool>(this.OnActiveReloadPressed);
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    int num1 = this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired;
                    int num2 = Math.Max(this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo, 0);
                    this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired = num2;
                    this.m_moduleData[this.Volley.projectiles[index]].needsReload = false;
                    this.m_moduleData[this.Volley.projectiles[index]].activeReloadDamageModifier = 1f;
                    int num3 = this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - num1;
                    if (activeReload)
                        this.m_moduleData[this.Volley.projectiles[index]].numberShotsActiveReload = num3;
                }
            }
            else
            {
                int num4 = this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner) - this.m_moduleData[this.singleModule].numberShotsFired;
                this.m_moduleData[this.singleModule].numberShotsFired = Math.Max(this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner) - this.ammo, 0);
                this.m_moduleData[this.singleModule].needsReload = false;
                this.m_moduleData[this.singleModule].activeReloadDamageModifier = 1f;
                int num5 = this.singleModule.GetModNumberOfShotsInClip(this.CurrentOwner) - num4;
                if (activeReload)
                    this.m_moduleData[this.singleModule].numberShotsActiveReload = num5;
            }
            if (!silent)
            {
                this.PlayIdleAnimation();
                this.SequentialActiveReloads = !activeReload ? 0 : this.SequentialActiveReloads + 1;
                if (this.LocalActiveReload && this.activeReloadData.ActiveReloadStacks)
                {
                    if (activeReload)
                    {
                        if (this.activeReloadData.ActiveReloadIncrementsTier)
                            this.CurrentStrengthTier = Mathf.Min(this.CurrentStrengthTier + 1, this.activeReloadData.MaxTier - 1);
                        this.AdditionalReloadMultiplier /= this.activeReloadData.reloadSpeedMultiplier;
                    }
                    else
                    {
                        if (this.activeReloadData.ActiveReloadIncrementsTier)
                            this.CurrentStrengthTier = 0;
                        this.AdditionalReloadMultiplier = 1f;
                    }
                }
                this.HandleActiveReloadEffects(activeReload);
            }
            this.m_isReloading = false;
        }

        private void HandleActiveReloadEffects(bool activeReload)
        {
            if (!(bool) (UnityEngine.Object) this.CurrentOwner || !((UnityEngine.Object) this.CurrentOwner.CurrentGun == (UnityEngine.Object) this))
                return;
            VFXPool vfxPool = (VFXPool) null;
            if (activeReload)
            {
                if (this.activeReloadSuccessEffects.type != VFXPoolType.None)
                    vfxPool = this.activeReloadSuccessEffects;
            }
            else if (this.activeReloadFailedEffects.type != VFXPoolType.None)
                vfxPool = this.activeReloadFailedEffects;
            if (!(bool) (UnityEngine.Object) this.CurrentOwner || vfxPool == null)
                return;
            vfxPool.SpawnAtPosition((Vector3) (this.CurrentOwner.CenterPosition + new Vector2(0.0f, 2f)), parent: this.CurrentOwner.transform, sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero), heightOffGround: new float?(5f), keepReferences: true);
        }

        private void PotentialShuffleAmmoForLargeClipGuns()
        {
            bool flag = false;
            int num1 = 0;
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null && this.Volley.projectiles.Count > 1)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    if (!this.Volley.projectiles[index].ignoredForReloadPurposes && this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) > 100)
                    {
                        ++num1;
                        flag = true;
                    }
                }
            }
            if (num1 < 2)
                flag = false;
            if (!flag)
                return;
            for (int index = 0; index < this.Volley.projectiles.Count; ++index)
            {
                if (this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) > 100)
                {
                    int num2 = this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - 100;
                    if (this.m_moduleData.ContainsKey(this.Volley.projectiles[index]) && num2 > this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired)
                        this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired = num2;
                }
            }
        }

        private bool HaveAmmoToReloadWith()
        {
            this.PotentialShuffleAmmoForLargeClipGuns();
            if (this.CanReloadNoMatterAmmo)
                return true;
            if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null)
            {
                for (int index = 0; index < this.Volley.projectiles.Count; ++index)
                {
                    if (!this.Volley.projectiles[index].ignoredForReloadPurposes && !this.Volley.projectiles[index].IsDuctTapeModule && this.Volley.projectiles[index].GetModNumberOfShotsInClip(this.CurrentOwner) - this.m_moduleData[this.Volley.projectiles[index]].numberShotsFired >= this.ammo)
                        return false;
                }
            }
            else if (this.singleModule.GetModifiedNumberOfFinalProjectiles(this.CurrentOwner) - this.m_moduleData[this.singleModule].numberShotsFired >= this.ammo)
                return false;
            return true;
        }

        public List<ModuleShootData> ActiveBeams => this.m_activeBeams;

        private Vector3 GetBeamAimDirection(float angleForShot)
        {
            Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, this.gunAngle) * Vector3.right;
            return Quaternion.Euler(0.0f, 0.0f, angleForShot) * vector3;
        }

        public void PlayIdleAnimation()
        {
            if (this.m_preventIdleLoop)
                return;
            this.m_preventIdleLoop = true;
            if (!string.IsNullOrEmpty(this.outOfAmmoAnimation) && this.ammo == 0)
                this.Play(this.outOfAmmoAnimation);
            else if (!string.IsNullOrEmpty(this.emptyAnimation) && this.ClipShotsRemaining <= 0)
            {
                this.Play(this.emptyAnimation);
            }
            else
            {
                if ((UnityEngine.Object) this.m_anim == (UnityEngine.Object) null)
                    this.m_anim = this.GetComponent<tk2dSpriteAnimator>();
                if (this.usesDirectionalIdleAnimations)
                {
                    if (this.m_directionalIdleNames == null)
                        this.m_directionalIdleNames = new string[8]
                        {
                            this.idleAnimation + "_E",
                            this.idleAnimation + "_SE",
                            this.idleAnimation + "_S",
                            this.idleAnimation + "_SW",
                            this.idleAnimation + "_W",
                            this.idleAnimation + "_NW",
                            this.idleAnimation + "_N",
                            this.idleAnimation + "_NE"
                        };
                    float num = this.gunAngle;
                    if (this.CurrentOwner is PlayerController)
                        num = BraveMathCollege.Atan2Degrees((this.CurrentOwner as PlayerController).unadjustedAimPoint.XY() - this.m_attachTransform.position.XY());
                    int octant = BraveMathCollege.AngleToOctant(num + 90f);
                    if (!this.m_anim.IsPlaying(this.m_directionalIdleNames[octant]))
                        this.Play(this.m_directionalIdleNames[octant]);
                }
                else if (!string.IsNullOrEmpty(this.idleAnimation) && this.m_anim.GetClipByName(this.idleAnimation) != null)
                {
                    this.Play(this.idleAnimation);
                }
                else
                {
                    this.m_anim.Stop();
                    this.m_sprite.spriteId = this.m_defaultSpriteID;
                }
            }
            this.m_preventIdleLoop = false;
        }

        private void DecrementAmmoCost(ProjectileModule module)
        {
            if ((UnityEngine.Object) this.modifiedFinalVolley != (UnityEngine.Object) null && module == this.modifiedFinalVolley.projectiles[0])
                module = this.DefaultModule;
            int ammoCost = module.ammoCost;
            if (module.shootStyle == ProjectileModule.ShootStyle.Charged)
            {
                ProjectileModule.ChargeProjectile chargeProjectile = module.GetChargeProjectile(this.m_moduleData[module].chargeTime);
                if (chargeProjectile.UsesAmmo)
                    ammoCost = chargeProjectile.AmmoCost;
            }
            if (this.InfiniteAmmo)
                ammoCost = 0;
            if (this.RequiresFundsToShoot && !this.m_hasDecrementedFunds)
            {
                this.m_hasDecrementedFunds = true;
                (this.m_owner as PlayerController).carriedConsumables.Currency -= this.CurrencyCostPerShot;
            }
            this.ammo = Math.Max(0, this.ammo - ammoCost);
            this.DecrementCustomAmmunitions(ammoCost);
        }

        public bool OverrideAnimations { get; set; }

        private void Play(string animName)
        {
            if (this.OverrideAnimations)
                return;
            if (this.usesDirectionalAnimator)
                this.aiAnimator.PlayUntilFinished(animName);
            else
                this.m_anim.Play(animName);
        }

        private void PlayIfExists(string name, bool restartIfPlaying = false)
        {
            if (this.OverrideAnimations)
                return;
            if (this.usesDirectionalAnimator && this.aiAnimator.HasDirectionalAnimation(name))
            {
                this.aiAnimator.PlayUntilFinished(name);
            }
            else
            {
                tk2dSpriteAnimationClip clipByName = this.m_anim.GetClipByName(name);
                if (clipByName == null)
                    return;
                if (restartIfPlaying && this.m_anim.IsPlaying(name))
                    this.m_anim.PlayFromFrame(0);
                else
                    this.m_anim.Play(clipByName);
            }
        }

        private void PlayIfExistsAndNotPlaying(string name)
        {
            if (this.OverrideAnimations)
                return;
            if (this.usesDirectionalAnimator && this.aiAnimator.HasDirectionalAnimation(name) && !this.aiAnimator.IsPlaying(name))
            {
                this.aiAnimator.PlayUntilFinished(name);
            }
            else
            {
                tk2dSpriteAnimationClip clipByName = this.m_anim.GetClipByName(name);
                if (clipByName == null || this.m_anim.IsPlaying(clipByName))
                    return;
                this.m_anim.Play(clipByName);
            }
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!this.gameObject.activeSelf || (UnityEngine.Object) this.CurrentOwner != (UnityEngine.Object) null)
                return 10000f;
            if (this.IsBeingSold || !(bool) (UnityEngine.Object) this.m_sprite || this.m_isThrown && (!this.m_thrownOnGround || (UnityEngine.Object) this.m_transform != (UnityEngine.Object) null && (UnityEngine.Object) this.m_transform.parent != (UnityEngine.Object) null && (UnityEngine.Object) this.m_transform.parent.GetComponent<Projectile>() != (UnityEngine.Object) null))
                return 1000f;
            Bounds bounds = this.m_sprite.GetBounds();
            Vector2 origin = this.transform.position.XY() + (this.transform.rotation * bounds.min).XY();
            Vector2 vector2 = origin + (this.transform.rotation * bounds.size).XY();
            return BraveMathCollege.DistToRectangle(point, origin, vector2 - origin);
        }

        public float GetOverrideMaxDistance() => -1f;

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this || !RoomHandler.unassignedInteractableObjects.Contains((IPlayerInteractable) this))
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.m_sprite);
            SpriteOutlineManager.AddOutlineToSprite(this.m_sprite, Color.white, 0.1f);
            this.m_sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!(bool) (UnityEngine.Object) this)
                return;
            SpriteOutlineManager.RemoveOutlineFromSprite(this.m_sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(this.m_sprite, Color.black, 0.1f, 0.05f);
            this.m_sprite.UpdateZDepth();
        }

        public void Interact(PlayerController interactor)
        {
            if (!this.gameObject.activeSelf)
                return;
            if (GameStatsManager.HasInstance && GameStatsManager.Instance.IsRainbowRun)
            {
                if ((bool) (UnityEngine.Object) interactor && interactor.CurrentRoom != null && interactor.CurrentRoom == GameManager.Instance.Dungeon.data.Entrance && UnityEngine.Time.frameCount == PickupObject.s_lastRainbowPickupFrame)
                    return;
                PickupObject.s_lastRainbowPickupFrame = UnityEngine.Time.frameCount;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(this.m_sprite, true);
            this.Pickup(interactor);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public Transform ThrowPrepTransform
        {
            get
            {
                if ((UnityEngine.Object) this.m_throwPrepTransform == (UnityEngine.Object) null)
                {
                    this.m_throwPrepTransform = this.transform.Find("throw point");
                    if ((UnityEngine.Object) this.m_throwPrepTransform == (UnityEngine.Object) null)
                    {
                        this.m_throwPrepTransform = new GameObject("throw point").transform;
                        this.m_throwPrepTransform.parent = this.transform;
                    }
                }
                this.m_throwPrepTransform.localPosition = this.ThrowPrepPosition * -1f;
                return this.m_throwPrepTransform;
            }
        }

        public Vector3 ThrowPrepPosition
        {
            get
            {
                Vector3 throwPrepPosition = this.barrelOffset.localPosition.WithX(this.barrelOffset.transform.parent.InverseTransformPoint((Vector3) this.sprite.WorldTopRight).x) * -1f;
                if ((UnityEngine.Object) this.m_throwPrepTransform != (UnityEngine.Object) null)
                    this.m_throwPrepTransform.localPosition = throwPrepPosition * -1f;
                return throwPrepPosition;
            }
        }

        public void TriggerTemporaryBoost(
            float damageMultiplier,
            float scaleMultiplier,
            float duration,
            bool oneShot)
        {
            this.StartCoroutine(this.HandleTemporaryBoost(damageMultiplier, scaleMultiplier, duration, oneShot));
        }

        [DebuggerHidden]
        private IEnumerator HandleTemporaryBoost(
            float damageMultiplier,
            float scaleMultiplier,
            float duration,
            bool oneShot)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new Gun__HandleTemporaryBoostc__Iterator5()
            {
                scaleMultiplier = scaleMultiplier,
                damageMultiplier = damageMultiplier,
                duration = duration,
                oneShot = oneShot,
                _this = this
            };
        }

        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);
            int num = 0;
            if (this.UsesRechargeLikeActiveItem)
            {
                ++num;
                data.Add((object) this.m_remainingActiveCooldownAmount);
            }
            IGunInheritable[] interfaces = this.gameObject.GetInterfaces<IGunInheritable>();
            for (int index = 0; index < interfaces.Length; ++index)
                interfaces[index].MidGameSerialize(data, index + num);
        }

        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameDeserialize(data);
            int dataIndex = 0;
            if (this.UsesRechargeLikeActiveItem)
            {
                this.m_remainingActiveCooldownAmount = (float) data[dataIndex];
                ++dataIndex;
            }
            foreach (IGunInheritable gunInheritable in this.gameObject.GetInterfaces<IGunInheritable>())
                gunInheritable.MidGameDeserialize(data, ref dataIndex);
        }

        public void CopyStateFrom(Gun other)
        {
            if (!(bool) (UnityEngine.Object) other || !other.UsesRechargeLikeActiveItem)
                return;
            this.m_remainingActiveCooldownAmount = other.m_remainingActiveCooldownAmount;
        }

        public void AddAdditionalFlipTransform(Transform t)
        {
            if (this.m_childTransformsToFlip == null)
                this.m_childTransformsToFlip = new List<Transform>();
            this.m_childTransformsToFlip.Add(t);
        }

        public bool MidBurstFire => this.m_midBurstFire;

        public Dictionary<ProjectileModule, ModuleShootData> RuntimeModuleData => this.m_moduleData;

        public int CurrentStrengthTier
        {
            get => this.m_currentStrengthTier;
            set
            {
                this.m_currentStrengthTier = value;
                if (!(bool) (UnityEngine.Object) this.CurrentOwner || !(this.CurrentOwner is PlayerController))
                    return;
                PlayerController currentOwner = this.CurrentOwner as PlayerController;
                if (!((UnityEngine.Object) currentOwner.stats != (UnityEngine.Object) null))
                    return;
                currentOwner.stats.RecalculateStats(currentOwner);
            }
        }

        public bool IsPreppedForThrow => this.m_isPreppedForThrow && (double) this.m_prepThrowTime > 0.0;

        public enum AttackResult
        {
            Success,
            OnCooldown,
            Reload,
            Empty,
            Fail,
        }
    }

