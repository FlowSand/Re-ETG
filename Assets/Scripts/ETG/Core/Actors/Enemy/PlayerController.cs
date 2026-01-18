using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Pathfinding;
using UnityEngine;
using UnityEngine.Rendering;

using Dungeonator;

#nullable disable

public class PlayerController : GameActor, ILevelLoadedListener
    {
        public const float c_averageVelocityPeriod = 0.5f;
        public const float s_dodgeRollBlinkMinPressTime = 0.2f;
        [Header("Player Properties")]
        public PlayableCharacters characterIdentity;
        [NonSerialized]
        private bool m_isTemporaryEeveeForUnlock;
        [NonSerialized]
        public Texture2D portalEeveeTex;
        [NonSerialized]
        public bool IsGhost;
        [NonSerialized]
        public bool IsDarkSoulsHollow;
        [Header("UI Stuff")]
        public string uiPortraitName;
        public float BosscardSpriteFPS;
        public List<Texture2D> BosscardSprites;
        public PerCharacterCoopPositionData CoopBosscardOffset;
        [Header("Stats")]
        public PlayerStats stats;
        public DodgeRollStats rollStats;
        public PitHelpers pitHelpers;
        public int MAX_GUNS_HELD = 3;
        public int MAX_ITEMS_HELD = 2;
        [NonSerialized]
        public bool UsingAlternateStartingGuns;
        [PickupIdentifier(typeof (Gun))]
        public List<int> startingGunIds;
        [PickupIdentifier(typeof (Gun))]
        public List<int> startingAlternateGunIds;
        [PickupIdentifier(typeof (Gun))]
        public List<int> finalFightGunIds;
        public PlayerConsumables carriedConsumables;
        public RandomStartingEquipmentSettings randomStartingEquipmentSettings;
        public bool AllowZeroHealthState;
        public bool ForceZeroHealthState;
        [NonSerialized]
        public bool HealthAndArmorSwapped;
        [NonSerialized]
        public List<LootModData> lootModData = new List<LootModData>();
        private int m_blanks;
        public Transform gunAttachPoint;
        [NonSerialized]
        public Transform secondaryGunAttachPoint;
        public Vector3 downwardAttachPointPosition;
        private Vector3 m_startingAttachPointPosition;
        public float collisionKnockbackStrength = 10f;
        public PlayerHandController primaryHand;
        public PlayerHandController secondaryHand;
        public Vector3 unadjustedAimPoint;
        private Vector2 m_lastVelocity;
        public Color outlineColor;
        public GameObject minimapIconPrefab;
        public tk2dSpriteAnimation AlternateCostumeLibrary;
        public List<ActorAudioEvent> animationAudioEvents;
        public string characterAudioSpeechTag;
        public bool usingForcedInput;
        public Vector2 forcedInput;
        public Vector2? forceAimPoint;
        public bool forceFire;
        public bool forceFireDown;
        public bool forceFireUp;
        public bool DrawAutoAim;
        [NonSerialized]
        public bool PastAccessible;
        public Action<PlayerController> OnIgnited;
        [NonSerialized]
        public bool WasPausedThisFrame;
        [NonSerialized]
        private bool m_isOnFire;
        [NonSerialized]
        private RuntimeGameActorEffectData m_onFireEffectData;
        [NonSerialized]
        public float CurrentFireMeterValue;
        [NonSerialized]
        public float CurrentPoisonMeterValue;
        [NonSerialized]
        public float CurrentDrainMeterValue;
        [NonSerialized]
        public float CurrentCurseMeterValue;
        [NonSerialized]
        public bool CurseIsDecaying = true;
        [NonSerialized]
        public float CurrentFloorDamageCooldown;
        [NonSerialized]
        public float CurrentStoneGunTimer;
        [NonSerialized]
        public List<IPlayerOrbital> orbitals = new List<IPlayerOrbital>();
        [NonSerialized]
        public List<PlayerOrbitalFollower> trailOrbitals = new List<PlayerOrbitalFollower>();
        [NonSerialized]
        public List<AIActor> companions = new List<AIActor>();
        private OverridableBool m_capableOfStealing = new OverridableBool(false);
        [NonSerialized]
        public bool IsEthereal;
        [NonSerialized]
        public bool HasGottenKeyThisRun;
        [NonSerialized]
        public int DeathsThisRun;
        private const float BasePoisonMeterDecayPerSecond = 0.5f;
        private const float BaseDrainMeterDecayPerSecond = 0.1f;
        private const float BaseCurseMeterDecayPerSecond = 0.5f;
        [NonSerialized]
        public Color baseFlatColorOverride = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        [NonSerialized]
        public List<int> ActiveExtraSynergies = new List<int>();
        [NonSerialized]
        public List<CustomSynergyType> CustomEventSynergies = new List<CustomSynergyType>();
        [NonSerialized]
        public bool DeferredStatRecalculationRequired;
        public bool ForceMetalGearMenu;
        protected GungeonActions m_activeActions;
        [NonSerialized]
        public bool CharacterUsesRandomGuns;
        [NonSerialized]
        public bool UnderstandsGleepGlorp;
        private static float AAStickTime = 0.0f;
        private static float AANonStickTime = 0.0f;
        private static float AALastWarnTime = -1000f;
        private static bool AACanWarn = true;
        private const float AAStickMultiplier = 1.5f;
        private const float AAMinWarnDelay = 300f;
        private const float AATotalStickTime = 660f;
        private const float AAWarnTime = 300f;
        private const float AAActivateTime = 600f;
        public OverridableBool InfiniteAmmo = new OverridableBool(false);
        public OverridableBool OnlyFinalProjectiles = new OverridableBool(false);
        [NonSerialized]
        public bool IsStationary;
        [NonSerialized]
        public bool IsGunLocked;
        private TeleporterController m_returnTeleporter;
        private bool m_additionalReceivesTouchDamage = true;
        public bool IsTalking;
        private bool m_wasTalkingThisFrame;
        public TalkDoerLite TalkPartner;
        private bool m_isInCombat;
        public System.Action OnEnteredCombat;
        private float m_superDuperAutoAimTimer;
        private bool m_isVisible = true;
        [HideInInspector]
        public GunInventory inventory;
        [NonSerialized]
        private Gun m_cachedQuickEquipGun;
        [NonSerialized]
        private Gun m_backupCachedQuickEquipGun;
        [NonSerialized]
        public int maxActiveItemsHeld = 2;
        [NonSerialized]
        public int spiceCount;
        [PickupIdentifier(typeof (PlayerItem))]
        public List<int> startingActiveItemIds;
        [NonSerialized]
        public List<PlayerItem> activeItems = new List<PlayerItem>();
        [PickupIdentifier(typeof (PassiveItem))]
        public List<int> startingPassiveItemIds;
        [NonSerialized]
        public List<PassiveItem> passiveItems = new List<PassiveItem>();
        public List<StatModifier> ownerlessStatModifiers = new List<StatModifier>();
        [NonSerialized]
        public List<PickupObject> additionalItems = new List<PickupObject>();
        public bool ForceHandless;
        public bool HandsOnAltCostume;
        public bool SwapHandsOnAltCostume;
        public string altHandName;
        public bool hasArmorlessAnimations;
        public GameObject lostAllArmorVFX;
        public GameObject lostAllArmorAltVfx;
        public GameObject CustomDodgeRollEffect;
        public Func<Gun, Projectile, Projectile> OnPreFireProjectileModifier;
        private int m_enemiesKilled;
        private float m_gunGameDamageThreshold = 200f;
        private const float c_gunGameDamageThreshold = 200f;
        private float m_gunGameElapsed;
        private const float c_gunGameElapsedThreshold = 20f;
        private const float c_fireMeterChargeRate = 0.666666f;
        private const float c_fireMeterRollingChargeRate = 0.2f;
        private const float c_fireMeterRollDecrease = 0.5f;
        public Action<PlayerController, Chest> OnChestBroken;
        public Action<Projectile, PlayerController> OnHitByProjectile;
        public Action<PlayerController, Gun> OnReloadPressed;
        public Action<PlayerController, Gun> OnReloadedGun;
        public Action<FlippableCover> OnTableFlipped;
        public Action<FlippableCover> OnTableFlipCompleted;
        public Action<PlayerController> OnNewFloorLoaded;
        [HideInInspector]
        public Vector2 knockbackComponent;
        [HideInInspector]
        public Vector2 immutableKnockbackComponent;
        [HideInInspector]
        public OverridableBool ImmuneToPits = new OverridableBool(false);
        private MeshRenderer m_renderer;
        private CoinBloop m_blooper;
        private KeyBullet m_setupKeyBullet;
        public float RealtimeEnteredCurrentRoom;
        private RoomHandler m_currentRoom;
        private Vector3 m_spriteDimensions;
        private int m_equippedGunShift;
        private List<tk2dBaseSprite> m_attachedSprites = new List<tk2dBaseSprite>();
        private List<float> m_attachedSpriteDepths = new List<float>();
        [NonSerialized]
        public Dictionary<string, GameObject> SpawnedSubobjects = new Dictionary<string, GameObject>();
        private PlayerInputState m_inputState;
        private OverridableBool m_disableInput = new OverridableBool(false);
        protected bool m_shouldContinueFiring;
        protected bool m_handlingQueuedAnimation;
        private bool m_interruptingPitRespawn;
        private bool m_skipPitRespawn;
        private Vector2 lockedDodgeRollDirection;
        private int m_selectedItemIndex;
        private IPlayerInteractable m_lastInteractionTarget;
        private List<IPlayerInteractable> m_leapInteractables = new List<IPlayerInteractable>();
        private float m_currentGunAngle;
        private float? m_overrideGunAngle;
        [NonSerialized]
        public MineCartController currentMineCart;
        public MineCartController previousMineCart;
        protected PlayerController.DodgeRollState m_dodgeRollState = PlayerController.DodgeRollState.None;
        private float m_dodgeRollTimer;
        private bool m_isSlidingOverSurface;
        private Vector3 m_cachedAimDirection = Vector3.right;
        private bool m_cachedGrounded = true;
        private bool m_highAccuracyAimMode;
        private Vector2 m_previousAimVector;
        private int m_masteryTokensCollectedThisRun;
        [NonSerialized]
        public bool EverHadMap;
        [NonSerialized]
        public tk2dSpriteAnimation OverrideAnimationLibrary;
        [NonSerialized]
        private tk2dSpriteAnimation BaseAnimationLibrary;
        [NonSerialized]
        public bool PlayerIsRatTransformed;
        private string m_overridePlayerSwitchState;
        public int PlayerIDX = -1;
        [NonSerialized]
        public int NumRoomsCleared;
        [NonSerialized]
        public string LevelToLoadOnPitfall;
        [NonSerialized]
        private string m_cachedLevelToLoadOnPitfall;
        private const bool c_coopSynergies = true;
        [NonSerialized]
        public bool ZeroVelocityThisFrame;
        private float dx9counter;
        [NonSerialized]
        public bool IsUsingAlternateCostume;
        private bool m_usingCustomHandType;
        private int m_baseHandId;
        private tk2dSpriteCollectionData m_baseHandCollection;
        private StatModifier m_turboSpeedModifier;
        private StatModifier m_turboEnemyBulletModifier;
        private StatModifier m_turboRollSpeedModifier;
        public bool FlatColorOverridden;
        private bool m_usesRandomStartingEquipment;
        private bool m_randomStartingItemsInitialized;
        private int m_randomStartingEquipmentSeed = -1;
        public bool IsCurrentlyCoopReviving;
        private string[] confettiPaths;
        private float m_coopRoomTimer;
        private Material[] m_cachedOverrideMaterials;
        private bool m_isStartingTeleport;
        protected float m_elapsedNonalertTime;
        public Action<float, bool, HealthHaver> OnAnyEnemyReceivedDamage;
        private bool m_cloneWaitingForCoopDeath;
        public System.Action LostArmor;
        private bool m_revenging;
        private AfterImageTrailController m_hollowAfterImage;
        private Color m_ghostUnchargedColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        private Color m_ghostChargedColor = new Color(0.2f, 0.3f, 1f, 1f);
        private bool m_isCoopArrowing;
        private bool m_isThreatArrowing;
        private AIActor m_threadArrowTarget;
        public Action<PlayerController> OnRealPlayerDeath;
        private bool m_suppressItemSwitchTo;
        protected GameObject BlankVFXPrefab;
        private Color m_alienDamageColor = new Color(1f, 0.0f, 0.0f, 1f);
        private Color m_alienBlankColor = new Color(0.35f, 0.35f, 1f, 1f);
        protected Coroutine m_currentActiveItemDestructionCoroutine;
        protected float m_postDodgeRollGunTimer;
        private const float AIM_VECTOR_MAGNITUDE_CUTOFF = 0.4f;
        public OverridableBool AdditionalCanDodgeRollWhileFlying = new OverridableBool(false);
        private bool m_handleDodgeRollStartThisFrame;
        private float m_timeHeldBlinkButton;
        private Vector2 m_cachedBlinkPosition;
        private tk2dSprite m_extantBlinkShadow;
        private int m_currentDodgeRollDepth;
        public Action<tk2dSprite> OnBlinkShadowCreated;
        public List<FlippableCover> TablesDamagedThisSlide = new List<FlippableCover>();
        private bool m_hasFiredWhileSliding;
        [NonSerialized]
        public bool LastFollowerVisibilityState = true;
        private bool m_gunChangePressedWhileMetalGeared;
        private int exceptionTracker;
        private bool m_interactedThisFrame;
        private bool m_preventItemSwitching;
        protected RoomHandler m_roomBeforeExit;
        protected RoomHandler m_previousExitLinkedRoom;
        protected bool m_inExitLastFrame;
        private List<IntVector2> m_bellygeonDepressedTiles = new List<IntVector2>();
        private static Dictionary<IntVector2, float> m_bellygeonDepressedTileTimers = new Dictionary<IntVector2, float>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
        private IntVector2 m_cachedLastCenterCellBellygeon = IntVector2.NegOne;
        private float m_highStressTimer;
        private Vector2 m_cachedTeleportSpot;
        private OverridableBool m_hideRenderers = new OverridableBool(false);
        private OverridableBool m_hideGunRenderers = new OverridableBool(false);
        private OverridableBool m_hideHandRenderers = new OverridableBool(false);
        private CellVisualData.CellFloorType? m_prevFloorType;
        protected List<AIActor> m_rollDamagedEnemies = new List<AIActor>();
        protected Vector2 m_playerCommandedDirection;
        private Vector2 m_lastNonzeroCommandedDirection;
        private float m_controllerSemiAutoTimer;
        private float m_startingMovementSpeed;
        private float m_maxIceFactor;
        private float m_blankCooldownTimer;
        public float gunReloadDisplayTimer;
        private float m_dropGunTimer;
        private float m_metalGearTimer;
        private int m_metalGearFrames;
        private bool m_gunWasDropped;
        private bool m_metalWasGeared;
        private float m_dropItemTimer;
        private bool m_itemWasDropped;
        private const float GunDropTimerThreshold = 0.5f;
        private const float MetalGearTimerThreshold = 0.175f;
        private const float CoopGhostBlankCooldown = 5f;
        private float c_iceVelocityMinClamp = 0.125f;
        private bool m_newFloorNoInput;
        private bool m_allowMoveAsAim;
        private float m_petDirection;
        public CompanionController m_pettingTarget;

        public bool IsTemporaryEeveeForUnlock
        {
            get => this.m_isTemporaryEeveeForUnlock;
            set
            {
                this.m_isTemporaryEeveeForUnlock = value;
                this.ClearOverrideShader();
                if (!value)
                    return;
                Texture2D portalEeveeTex = this.portalEeveeTex;
                if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.sprite || !(bool) (UnityEngine.Object) this.sprite.renderer || !(bool) (UnityEngine.Object) this.sprite.renderer.material)
                    return;
                this.sprite.renderer.material.SetTexture("_EeveeTex", (Texture) portalEeveeTex);
            }
        }

        public void SetTemporaryEeveeSafeNoShader(bool value) => this.m_isTemporaryEeveeForUnlock = value;

        public int Blanks
        {
            get => this.m_blanks;
            set
            {
                this.m_blanks = value;
                GameStatsManager.Instance.UpdateMaximum(TrackedMaximums.MOST_BLANKS_HELD, (float) this.m_blanks);
                GameUIRoot.Instance.UpdatePlayerBlankUI(this);
                GameUIRoot.Instance.UpdatePlayerConsumables(this.carriedConsumables);
            }
        }

        public bool IsOnFire
        {
            get => this.m_isOnFire;
            set
            {
                if (value && this.HasActiveBonusSynergy(CustomSynergyType.FOSSIL_PHOENIX))
                    value = false;
                if (value && this.stats.UsesFireSourceEffect)
                {
                    if (this.m_onFireEffectData == null)
                        this.m_onFireEffectData = GameActorFireEffect.ApplyFlamesToTarget((GameActor) this, this.stats.OnFireSourceEffect);
                }
                else if (!value && this.stats.UsesFireSourceEffect && this.m_onFireEffectData != null)
                {
                    GameActorFireEffect.DestroyFlames(this.m_onFireEffectData);
                    this.m_onFireEffectData = (RuntimeGameActorEffectData) null;
                }
                if (value && !this.m_isOnFire && this.OnIgnited != null)
                    this.OnIgnited(this);
                this.m_isOnFire = value;
            }
        }

        public void IncreaseFire(float amount)
        {
            if (this.SuppressEffectUpdates)
                return;
            this.CurrentFireMeterValue += amount * this.healthHaver.GetDamageModifierForType(CoreDamageTypes.Fire);
        }

        public void IncreasePoison(float amount)
        {
            if (this.SuppressEffectUpdates || this.IsGhost || (bool) (UnityEngine.Object) this.healthHaver && !this.healthHaver.IsVulnerable)
                return;
            this.CurrentPoisonMeterValue += amount * this.healthHaver.GetDamageModifierForType(CoreDamageTypes.Poison);
        }

        public Vector2 SmoothedCameraCenter
        {
            get
            {
                if ((bool) (UnityEngine.Object) this.specRigidbody && this.specRigidbody.HitboxPixelCollider != null)
                    return this.specRigidbody.HitboxPixelCollider.UnitCenter + this.specRigidbody.Position.Remainder.Quantize(1f / 16f / Pixelator.Instance.ScaleTileScale);
                return (bool) (UnityEngine.Object) this.sprite ? this.sprite.WorldCenter : this.transform.position.XY();
            }
        }

        public bool IsCapableOfStealing => this.m_capableOfStealing.Value;

        public void SetCapableOfStealing(bool value, string reason, float? duration = null)
        {
            this.m_capableOfStealing.SetOverride(reason, value, duration);
            this.ForceRefreshInteractable = true;
        }

        public int KillsThisRun => this.m_enemiesKilled;

        public override Gun CurrentGun
        {
            get
            {
                if (this.inventory == null)
                    return (Gun) null;
                return this.IsGhost ? (Gun) null : this.inventory.CurrentGun;
            }
        }

        public Gun CurrentSecondaryGun
        {
            get
            {
                if (this.inventory == null)
                    return (Gun) null;
                if (!this.inventory.DualWielding)
                    return (Gun) null;
                return this.IsGhost ? (Gun) null : this.inventory.CurrentSecondaryGun;
            }
        }

        public PlayerItem CurrentItem
        {
            get
            {
                if (this.m_selectedItemIndex <= 0 || this.m_selectedItemIndex >= this.activeItems.Count)
                    this.m_selectedItemIndex = 0;
                return this.activeItems.Count > 0 ? this.activeItems[this.m_selectedItemIndex] : (PlayerItem) null;
            }
        }

        public override Transform GunPivot => this.gunAttachPoint;

        public override Transform SecondaryGunPivot
        {
            get
            {
                if ((UnityEngine.Object) this.secondaryGunAttachPoint == (UnityEngine.Object) null)
                {
                    this.secondaryGunAttachPoint = new GameObject("secondary attach point").transform;
                    this.secondaryGunAttachPoint.parent = this.gunAttachPoint.parent;
                    this.secondaryGunAttachPoint.localPosition = this.gunAttachPoint.localPosition;
                }
                return this.secondaryGunAttachPoint;
            }
        }

        public override Vector3 SpriteDimensions => this.m_spriteDimensions;

        public override bool IsFlying
        {
            get
            {
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                    return false;
                return this.m_isFlying.Value || this.IsGhost;
            }
        }

        public Vector3 LockedApproximateSpriteCenter => (Vector3) this.CenterPosition;

        public Vector3 SpriteBottomCenter
        {
            get
            {
                return this.sprite.transform.position.WithX(this.sprite.transform.position.x + (!this.sprite.FlipX ? this.m_spriteDimensions.x / 2f : (float) (-1.0 * (double) this.m_spriteDimensions.x / 2.0)));
            }
        }

        public override bool SpriteFlipped => this.sprite.FlipX;

        public bool BossKillingMode { get; set; }

        public bool CanReturnTeleport => (UnityEngine.Object) this.m_returnTeleporter != (UnityEngine.Object) null;

        public bool ReceivesTouchDamage
        {
            get
            {
                if (PassiveItem.ActiveFlagItems.ContainsKey(this))
                {
                    Dictionary<System.Type, int> activeFlagItem = PassiveItem.ActiveFlagItems[this];
                    if (activeFlagItem.ContainsKey(typeof (LiveAmmoItem)) || activeFlagItem.ContainsKey(typeof (SpikedArmorItem)) || activeFlagItem.ContainsKey(typeof (HelmetItem)))
                        return false;
                }
                return this.m_additionalReceivesTouchDamage;
            }
            set => this.m_additionalReceivesTouchDamage = value;
        }

        protected bool m_CanAttack
        {
            get
            {
                return (!this.IsDodgeRolling || this.IsSlidingOverSurface) && !this.IsGunLocked && (double) this.CurrentStoneGunTimer <= 0.0;
            }
        }

        public bool WasTalkingThisFrame => this.m_wasTalkingThisFrame;

        public bool IgnoredByCamera { get; private set; }

        public bool IsDodgeRolling
        {
            get
            {
                return this.m_dodgeRollState != PlayerController.DodgeRollState.None && this.m_dodgeRollState != PlayerController.DodgeRollState.AdditionalDelay;
            }
        }

        public bool IsInMinecart => (bool) (UnityEngine.Object) this.currentMineCart;

        public bool IsInCombat
        {
            get
            {
                return this.CurrentRoom != null && this.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
            }
        }

        public bool CanBeGrabbed
        {
            get => this.healthHaver.IsVulnerable && !this.IsFalling && !this.IsGhost && !this.IsEthereal;
        }

        public bool IsThief { get; set; }

        public RoomHandler CurrentRoom => this.m_currentRoom;

        public bool IsVisible
        {
            get => this.m_isVisible;
            set
            {
                if (value == this.m_isVisible)
                    return;
                this.m_isVisible = value;
                this.ToggleRenderer(this.m_isVisible, "isVisible");
                this.ToggleGunRenderers(this.m_isVisible, "isVisible");
                this.ToggleHandRenderers(this.m_isVisible, "isVisible");
            }
        }

        public bool CanDetectHiddenEnemies
        {
            get
            {
                return (bool) (UnityEngine.Object) this.CurrentGun && (bool) (UnityEngine.Object) this.CurrentGun.GetComponent<PredatorGunController>();
            }
        }

        private IAutoAimTarget SuperAutoAimTarget { get; set; }

        private IAutoAimTarget SuperDuperAimTarget { get; set; }

        private Vector2 SuperDuperAimPoint { get; set; }

        public float BulletScaleModifier
        {
            get => this.stats.GetStatValue(PlayerStats.StatType.PlayerBulletScale);
        }

        public bool SuppressThisClick { get; set; }

        public bool InExitCell { get; set; }

        public CellData CurrentExitCell { get; set; }

        public Vector2 AverageVelocity { get; set; }

        public bool ArmorlessAnimations
        {
            get => this.hasArmorlessAnimations && !GameManager.Instance.IsFoyer;
            set => this.hasArmorlessAnimations = value;
        }

        public bool UseArmorlessAnim
        {
            get
            {
                return this.ArmorlessAnimations && (double) this.healthHaver.Armor == 0.0 && (UnityEngine.Object) this.OverrideAnimationLibrary == (UnityEngine.Object) null;
            }
        }

        public event System.Action OnPitfall;

        public event Action<Projectile, float> PostProcessProjectile;

        public event Action<BeamController> PostProcessBeam;

        public event Action<BeamController, SpeculativeRigidbody, float> PostProcessBeamTick;

        public event Action<BeamController> PostProcessBeamChanceTick;

        public event Action<Projectile> PostProcessThrownGun;

        public event Action<PlayerController, float> OnDealtDamage;

        public event Action<PlayerController, float, bool, HealthHaver> OnDealtDamageContext;

        public event Action<PlayerController> OnKilledEnemy;

        public event Action<PlayerController, HealthHaver> OnKilledEnemyContext;

        public event Action<PlayerController, PlayerItem> OnUsedPlayerItem;

        public event Action<PlayerController> OnTriedToInitiateAttack;

        public event Action<PlayerController, int> OnUsedBlank;

        public event Action<Gun, Gun, bool> GunChanged;

        public event Action<PlayerController> OnDidUnstealthyAction;

        public event Action<PlayerController> OnPreDodgeRoll;

        public event Action<PlayerController, Vector2> OnRollStarted;

        public event Action<PlayerController> OnIsRolling;

        public event Action<PlayerController, AIActor> OnRolledIntoEnemy;

        public event Action<Projectile> OnDodgedProjectile;

        public event Action<BeamController, PlayerController> OnDodgedBeam;

        public event Action<PlayerController> OnReceivedDamage;

        public event Action<PlayerController, ShopItemController> OnItemPurchased;

        public event Action<PlayerController, ShopItemController> OnItemStolen;

        public event Action<PlayerController> OnRoomClearEvent;

        public float AdditionalChestSpawnChance { get; set; }

        public PlayerInputState CurrentInputState
        {
            get
            {
                if (this.m_disableInput.Value)
                    return PlayerInputState.NoInput;
                return this.m_inputState == PlayerInputState.AllInput && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || this.m_inputState == PlayerInputState.AllInput && GameManager.Instance.IsFoyer ? PlayerInputState.FoyerInputOnly : this.m_inputState;
            }
            set => this.m_inputState = value;
        }

        public bool AcceptingAnyInput => this.CurrentInputState != PlayerInputState.NoInput;

        public bool AcceptingNonMotionInput
        {
            get
            {
                return (this.CurrentInputState == PlayerInputState.AllInput || this.CurrentInputState == PlayerInputState.NoMovement) && !GameManager.Instance.PreventPausing;
            }
        }

        public bool IsInputOverridden => this.m_disableInput.Value;

        public void SetInputOverride(string reason) => this.m_disableInput.AddOverride(reason);

        public void ClearInputOverride(string reason) => this.m_disableInput.RemoveOverride(reason);

        public void ClearAllInputOverrides()
        {
            this.m_disableInput.ClearOverrides();
            this.CurrentInputState = PlayerInputState.AllInput;
        }

        public IPlayerInteractable GetLastInteractable() => this.m_lastInteractionTarget;

        public PlayerController.DodgeRollState CurrentRollState => this.m_dodgeRollState;

        public bool IsSlidingOverSurface
        {
            get => this.m_isSlidingOverSurface;
            set => this.m_isSlidingOverSurface = value;
        }

        private bool RenderBodyHand
        {
            get
            {
                if (this.ForceHandless || !((UnityEngine.Object) this.CurrentSecondaryGun == (UnityEngine.Object) null))
                    return false;
                return (UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null || this.CurrentGun.Handedness != GunHandedness.TwoHanded;
            }
        }

        public bool IsFiring { get; set; }

        public bool ForceRefreshInteractable { get; set; }

        public bool IsCachedLeapInteractable(IPlayerInteractable ixable)
        {
            return this.m_leapInteractables.Contains(ixable);
        }

        public bool HighAccuracyAimMode
        {
            get => this.m_highAccuracyAimMode;
            set
            {
                if (this.m_highAccuracyAimMode != value)
                    this.m_previousAimVector = Vector2.zero;
                this.m_highAccuracyAimMode = value;
            }
        }

        public bool HasTakenDamageThisRun { get; set; }

        public bool HasTakenDamageThisFloor { get; set; }

        public bool HasReceivedNewGunThisFloor { get; set; }

        public bool HasFiredNonStartingGun { get; set; }

        public int MasteryTokensCollectedThisRun
        {
            get => this.m_masteryTokensCollectedThisRun;
            set
            {
                this.m_masteryTokensCollectedThisRun = value;
                if (this.m_masteryTokensCollectedThisRun < 5)
                    return;
                GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COLLECT_FIVE_MASTERY_TOKENS);
            }
        }

        protected override bool QueryGroundedFrame()
        {
            return (!this.IsDodgeRolling || !this.DodgeRollIsBlink || (double) this.m_dodgeRollTimer >= 0.55555558204650879 * (double) this.rollStats.GetModifiedTime(this)) && (!this.IsDodgeRolling || (double) this.m_dodgeRollTimer >= 0.55555558204650879 * (double) this.rollStats.GetModifiedTime(this)) && this.spriteAnimator.QueryGroundedFrame();
        }

        public string OverridePlayerSwitchState
        {
            get => this.m_overridePlayerSwitchState;
            set
            {
                this.m_overridePlayerSwitchState = value;
                int num = (int) AkSoundEngine.SetSwitch("CHR_Player", this.m_overridePlayerSwitchState == null ? this.characterIdentity.ToString() : this.m_overridePlayerSwitchState, this.gameObject);
            }
        }

        protected override float DustUpMultiplier
        {
            get => this.stats.MovementSpeed / this.m_startingMovementSpeed;
        }

        public bool IsPrimaryPlayer => this.PlayerIDX == 0;

        public override void Awake()
        {
            base.Awake();
            this.m_overrideFlatColorID = Shader.PropertyToID("_FlatColor");
            this.m_specialFlagsID = Shader.PropertyToID("_SpecialFlags");
            this.m_stencilID = Shader.PropertyToID("_StencilVal");
            this.m_blooper = this.GetComponentInChildren<CoinBloop>();
            Transform transform = this.transform.Find("PlayerSprite");
            this.sprite = !((UnityEngine.Object) transform != (UnityEngine.Object) null) ? (tk2dBaseSprite) null : (tk2dBaseSprite) transform.GetComponent<tk2dSprite>();
            if ((UnityEngine.Object) this.sprite == (UnityEngine.Object) null)
                this.sprite = (tk2dBaseSprite) this.transform.Find("PlayerRotatePoint").Find("PlayerSprite").GetComponent<tk2dSprite>();
            this.m_renderer = this.sprite.GetComponent<MeshRenderer>();
            this.spriteAnimator = this.sprite.GetComponent<tk2dSpriteAnimator>();
            PlayerStats playerStats = this.gameObject.AddComponent<PlayerStats>();
            playerStats.CopyFrom(this.stats);
            this.stats = playerStats;
            this.stats.RecalculateStats(this, true);
            if (this.characterIdentity == PlayableCharacters.Eevee)
                this.m_usesRandomStartingEquipment = true;
            if ((bool) (UnityEngine.Object) GameManager.Instance && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
            {
                if (this.characterIdentity == PlayableCharacters.CoopCultist && GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Eevee)
                    this.m_usesRandomStartingEquipment = true;
            }
            else if (this.characterIdentity == PlayableCharacters.CoopCultist)
            {
                foreach (PlayerController playerController in UnityEngine.Object.FindObjectsOfType<PlayerController>())
                {
                    if (playerController.characterIdentity == PlayableCharacters.Eevee)
                    {
                        this.m_usesRandomStartingEquipment = true;
                        break;
                    }
                }
            }
            if (!this.m_usesRandomStartingEquipment)
                return;
            if ((bool) (UnityEngine.Object) GameManager.Instance && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                GameStatsManager.Instance.CurrentEeveeEquipSeed = -1;
            if (GameStatsManager.Instance.CurrentEeveeEquipSeed < 0)
                GameStatsManager.Instance.CurrentEeveeEquipSeed = UnityEngine.Random.Range(1, 10000000);
            this.m_randomStartingEquipmentSeed = GameStatsManager.Instance.CurrentEeveeEquipSeed;
            this.SetUpRandomStartingEquipment();
        }

        public PickupObject.ItemQuality GetQualityFromChances(
            System.Random r,
            float dChance,
            float cChance,
            float bChance,
            float aChance,
            float sChance)
        {
            float num = (dChance + cChance + bChance + aChance + sChance) * (float) r.NextDouble();
            if ((double) num < (double) dChance)
                return PickupObject.ItemQuality.D;
            if ((double) num < (double) dChance + (double) cChance)
                return PickupObject.ItemQuality.C;
            if ((double) num < (double) dChance + (double) cChance + (double) bChance)
                return PickupObject.ItemQuality.B;
            return (double) num < (double) dChance + (double) cChance + (double) bChance + (double) aChance ? PickupObject.ItemQuality.A : PickupObject.ItemQuality.S;
        }

        private void SetUpRandomStartingEquipment()
        {
            this.startingGunIds.Clear();
            this.startingAlternateGunIds.Clear();
            this.startingPassiveItemIds.Clear();
            this.startingActiveItemIds.Clear();
            this.finalFightGunIds.Clear();
            System.Random random = new System.Random(this.m_randomStartingEquipmentSeed);
            PickupObject.ItemQuality qualityFromChances1 = this.GetQualityFromChances(random, this.randomStartingEquipmentSettings.D_CHANCE, this.randomStartingEquipmentSettings.C_CHANCE, this.randomStartingEquipmentSettings.B_CHANCE, this.randomStartingEquipmentSettings.A_CHANCE, this.randomStartingEquipmentSettings.S_CHANCE);
            PickupObject.ItemQuality qualityFromChances2 = this.GetQualityFromChances(random, this.randomStartingEquipmentSettings.D_CHANCE, this.randomStartingEquipmentSettings.C_CHANCE, this.randomStartingEquipmentSettings.B_CHANCE, this.randomStartingEquipmentSettings.A_CHANCE, this.randomStartingEquipmentSettings.S_CHANCE);
            Gun randomStartingGun = PickupObjectDatabase.GetRandomStartingGun(random);
            Gun randomGunOfQualities = PickupObjectDatabase.GetRandomGunOfQualities(random, new List<int>((IEnumerable<int>) this.randomStartingEquipmentSettings.ExcludedPickups)
            {
                GlobalItemIds.Blasphemy
            }, qualityFromChances1);
            PassiveItem passiveOfQualities = PickupObjectDatabase.GetRandomPassiveOfQualities(random, this.randomStartingEquipmentSettings.ExcludedPickups, qualityFromChances2);
            this.startingGunIds.Add(randomStartingGun.PickupObjectId);
            if ((bool) (UnityEngine.Object) randomGunOfQualities)
                this.startingGunIds.Add(randomGunOfQualities.PickupObjectId);
            if ((bool) (UnityEngine.Object) passiveOfQualities)
                this.startingPassiveItemIds.Add(passiveOfQualities.PickupObjectId);
            if (!(bool) (UnityEngine.Object) randomGunOfQualities)
                return;
            this.finalFightGunIds.Add(randomGunOfQualities.PickupObjectId);
        }

        public override void Start()
        {
            base.Start();
            if (PassiveItem.ActiveFlagItems == null)
                PassiveItem.ActiveFlagItems = new Dictionary<PlayerController, Dictionary<System.Type, int>>();
            if (!PassiveItem.ActiveFlagItems.ContainsKey(this))
                PassiveItem.ActiveFlagItems.Add(this, new Dictionary<System.Type, int>());
            this.m_allowMoveAsAim = GameManager.Options.autofaceMovementDirection;
            UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
            int num = (int) AkSoundEngine.SetSwitch("CHR_Player", this.m_overridePlayerSwitchState == null ? this.characterIdentity.ToString() : this.m_overridePlayerSwitchState, this.gameObject);
            if (this.IsPrimaryPlayer)
            {
                AkAudioListener component = this.GetComponent<AkAudioListener>();
                if ((bool) (UnityEngine.Object) component)
                    UnityEngine.Object.Destroy((UnityEngine.Object) component);
                new GameObject("listener")
                {
                    transform = {
                        parent = this.transform,
                        localPosition = (this.specRigidbody.UnitBottomCenter - this.transform.position.XY()).ToVector3ZUp(5f)
                    }
                }.GetOrAddComponent<AkAudioListener>().listenerId = !this.IsPrimaryPlayer ? 1 : 0;
            }
            this.ActorName = "Player ID 0";
            this.spriteAnimator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
            this.m_spriteDimensions = this.sprite.GetUntrimmedBounds().size;
            this.m_startingAttachPointPosition = this.gunAttachPoint.localPosition;
            this.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(this.gunAttachPoint.localPosition, 16f);
            this.stats.RecalculateStats(this);
            this.m_startingMovementSpeed = this.stats.MovementSpeed;
            this.Blanks = GameManager.Instance.CurrentGameType != GameManager.GameType.SINGLE_PLAYER ? this.stats.NumBlanksPerFloorCoop : this.stats.NumBlanksPerFloor;
            this.InitializeInventory();
            this.InitializeCallbacks();
            if (this.HasShadow)
                this.sprite.AttachRenderer((tk2dBaseSprite) this.GenerateDefaultBlobShadow().GetComponent<tk2dSprite>());
            SpriteOutlineManager.AddOutlineToSprite(this.sprite, this.outlineColor, 0.1f, outlineType: this.characterIdentity != PlayableCharacters.Eevee ? SpriteOutlineManager.OutlineType.NORMAL : SpriteOutlineManager.OutlineType.EEVEE);
            this.OnGunChanged((Gun) null, this.CurrentGun, (Gun) null, (Gun) null, true);
            this.gameObject.AddComponent<AkGameObj>();
            if (!GameStatsManager.Instance.IsInSession)
                GameStatsManager.Instance.BeginNewSession(this);
            tk2dSpriteAttachPoint spriteAttachPoint = this.sprite.GetComponent<tk2dSpriteAttachPoint>();
            if ((UnityEngine.Object) spriteAttachPoint == (UnityEngine.Object) null)
                spriteAttachPoint = this.sprite.gameObject.AddComponent<tk2dSpriteAttachPoint>();
            if ((UnityEngine.Object) spriteAttachPoint.GetAttachPointByName("jetpack") == (UnityEngine.Object) null)
                spriteAttachPoint.ForceAddAttachPoint("jetpack");
            spriteAttachPoint.centerUnusedAttachPoints = true;
            if (this.IsPrimaryPlayer)
            {
                this.carriedConsumables.Initialize();
                for (int index = 0; index < this.passiveItems.Count; ++index)
                {
                    if ((bool) (UnityEngine.Object) this.passiveItems[index] && this.passiveItems[index] is BriefcaseFullOfCashItem)
                        this.carriedConsumables.Currency += (this.passiveItems[index] as BriefcaseFullOfCashItem).CurrencyAmount;
                }
            }
            else
            {
                this.carriedConsumables = GameManager.Instance.PrimaryPlayer.carriedConsumables;
                this.lootModData = GameManager.Instance.PrimaryPlayer.lootModData;
            }
            this.unadjustedAimPoint = this.LockedApproximateSpriteCenter + new Vector3(5f, 0.0f);
            if ((bool) (UnityEngine.Object) this.primaryHand)
                this.primaryHand.InitializeWithPlayer(this, true);
            if ((bool) (UnityEngine.Object) this.secondaryHand)
                this.secondaryHand.InitializeWithPlayer(this, false);
            this.ProcessHandAttachment();
            this.sprite.usesOverrideMaterial = true;
            this.sprite.renderer.material.SetFloat("_Perpendicular", this.sprite.renderer.material.GetFloat("_Perpendicular"));
            if (this.characterIdentity == PlayableCharacters.Pilot || this.characterIdentity == PlayableCharacters.Robot || this.characterIdentity == PlayableCharacters.Guide)
                this.sprite.renderer.material.SetFloat("_PlayerGhostAdjustFactor", 4f);
            else
                this.sprite.renderer.material.SetFloat("_PlayerGhostAdjustFactor", 3f);
            this.healthHaver.RegisterBodySprite(this.primaryHand.sprite);
            this.healthHaver.RegisterBodySprite(this.secondaryHand.sprite);
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                GameManager.Instance.FrameDelayedEnteredFoyer(this);
            this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
            if ((UnityEngine.Object) GameUIRoot.Instance != (UnityEngine.Object) null)
                GameUIRoot.Instance.UpdatePlayerConsumables(this.carriedConsumables);
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER && this.characterIdentity != PlayableCharacters.Eevee || !(bool) (UnityEngine.Object) this.sprite || !(bool) (UnityEngine.Object) this.sprite.renderer || this is PlayerSpaceshipController)
                return;
            this.sprite.renderer.material.shader = ShaderCache.Acquire(this.LocalShaderName);
        }

        private void Instance_OnNewLevelFullyLoaded()
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= new System.Action(this.Instance_OnNewLevelFullyLoaded);
            this.StartCoroutine(this.FrameDelayedInitialDeath());
        }

        public void DieOnMidgameLoad() => this.StartCoroutine(this.FrameDelayedInitialDeath(true));

        public static bool AnyoneHasActiveBonusSynergy(CustomSynergyType synergy, out int count)
        {
            count = 0;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                if (!GameManager.Instance.AllPlayers[index].IsGhost)
                    count += GameManager.Instance.AllPlayers[index].CountActiveBonusSynergies(synergy);
            }
            return count > 0;
        }

        public int CountActiveBonusSynergies(CustomSynergyType synergy)
        {
            if (!((UnityEngine.Object) this.stats != (UnityEngine.Object) null))
                return 0;
            int num = 0;
            for (int index = 0; index < this.stats.ActiveCustomSynergies.Count; ++index)
            {
                if (this.stats.ActiveCustomSynergies[index] == synergy)
                    ++num;
            }
            for (int index = 0; index < this.CustomEventSynergies.Count; ++index)
            {
                if (this.CustomEventSynergies[index] == synergy)
                    ++num;
            }
            return num;
        }

        public bool HasActiveBonusSynergy(CustomSynergyType synergy, bool recursive = false)
        {
            if (this.CustomEventSynergies.Contains(synergy))
                return true;
            return (UnityEngine.Object) this.stats != (UnityEngine.Object) null && this.stats.ActiveCustomSynergies.Contains(synergy);
        }

        [DebuggerHidden]
        private IEnumerator FrameDelayedInitialDeath(bool delayTilPostGeneration = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__FrameDelayedInitialDeathc__Iterator0()
            {
                delayTilPostGeneration = delayTilPostGeneration,
                _this = this
            };
        }

        protected void HandlePostDodgeRollTimer()
        {
            if ((double) this.m_postDodgeRollGunTimer <= 0.0)
                return;
            this.m_postDodgeRollGunTimer -= BraveTime.DeltaTime;
            if ((double) this.m_postDodgeRollGunTimer > 0.0)
                return;
            this.ToggleGunRenderers(true, "postdodgeroll");
            this.ToggleHandRenderers(true, "postdodgeroll");
        }

        [DebuggerHidden]
        private IEnumerator DestroyEnemyBulletsInCircleForDuration(
            Vector2 center,
            float radius,
            float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__DestroyEnemyBulletsInCircleForDurationc__Iterator1()
            {
                duration = duration,
                center = center,
                radius = radius
            };
        }

        protected void EndBlinkDodge()
        {
            this.IsEthereal = false;
            this.IsVisible = true;
            this.m_dodgeRollState = PlayerController.DodgeRollState.AdditionalDelay;
            if (this.IsPrimaryPlayer)
                GameManager.Instance.MainCameraController.UseOverridePlayerOnePosition = false;
            else
                GameManager.Instance.MainCameraController.UseOverridePlayerTwoPosition = false;
            this.WarpToPoint(this.m_cachedBlinkPosition + (this.transform.position.XY() - this.specRigidbody.UnitCenter));
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
            this.StartCoroutine(this.DestroyEnemyBulletsInCircleForDuration(this.specRigidbody.UnitCenter, 2f, 0.05f));
            this.previousMineCart = (MineCartController) null;
            this.ClearBlinkShadow();
        }

        private void ClearDodgeRollState()
        {
            this.m_dodgeRollState = PlayerController.DodgeRollState.None;
            this.m_currentDodgeRollDepth = 0;
            this.m_leapInteractables.Clear();
        }

        public override void Update()
        {
            base.Update();
            if (GameManager.Instance.IsPaused || GameManager.Instance.UnpausedThisFrame || GameManager.Instance.IsLoadingLevel)
                return;
            this.m_interactedThisFrame = false;
            if (this.IsPetting && (!this.spriteAnimator.IsPlaying("pet") || !(bool) (UnityEngine.Object) this.m_pettingTarget || (UnityEngine.Object) this.m_pettingTarget.m_pettingDoer != (UnityEngine.Object) this || (double) Vector2.Distance(this.specRigidbody.UnitCenter, this.m_pettingTarget.specRigidbody.UnitCenter) > 3.0 || this.IsDodgeRolling))
            {
                this.ToggleGunRenderers(true, "petting");
                this.ToggleHandRenderers(true, "petting");
                this.m_pettingTarget = (CompanionController) null;
            }
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9)
            {
                this.dx9counter += GameManager.INVARIANT_DELTA_TIME;
                if ((double) this.dx9counter > 5.0)
                {
                    this.dx9counter = 0.0f;
                    foreach (tk2dBaseSprite componentsInChild in this.GetComponentsInChildren<tk2dSprite>())
                        componentsInChild.ForceBuild();
                }
                if (Input.GetKeyDown(KeyCode.F8))
                {
                    tk2dBaseSprite[] objectsOfType = UnityEngine.Object.FindObjectsOfType<tk2dBaseSprite>();
                    for (int index = 0; index < objectsOfType.Length; ++index)
                    {
                        if ((bool) (UnityEngine.Object) objectsOfType[index])
                            objectsOfType[index].ForceBuild();
                    }
                    ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
                    for (int index = 0; index < allProjectiles.Count; ++index)
                    {
                        Projectile projectile = allProjectiles[index];
                        if ((bool) (UnityEngine.Object) projectile && (bool) (UnityEngine.Object) projectile.sprite)
                            projectile.sprite.ForceBuild();
                    }
                }
            }
            if (this.healthHaver.IsDead && !this.IsGhost)
                return;
            if (this.CharacterUsesRandomGuns && this.inventory != null)
            {
                while (this.inventory.AllGuns.Count > 1)
                    this.inventory.DestroyGun(this.inventory.AllGuns[0]);
            }
            this.HandlePostDodgeRollTimer();
            this.m_activeActions = BraveInput.GetInstanceForPlayer(this.PlayerIDX).ActiveActions;
            if ((!this.AcceptingNonMotionInput || (double) this.CurrentStoneGunTimer > 0.0) && (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null && this.CurrentGun.IsFiring && (!this.CurrentGun.IsCharging || this.CurrentInputState != PlayerInputState.OnlyMovement && !GameManager.IsBossIntro))
            {
                this.CurrentGun.CeaseAttack(false);
                if ((bool) (UnityEngine.Object) this.CurrentSecondaryGun)
                    this.CurrentSecondaryGun.CeaseAttack(false);
            }
            if (this.inventory != null)
                this.inventory.FrameUpdate();
            Projectile.UpdateEnemyBulletSpeedMultiplier();
            float num1 = Mathf.Clamp01(BraveTime.DeltaTime / 0.5f);
            if ((double) num1 > 0.0 && (double) num1 < 1.0)
                this.AverageVelocity = BraveMathCollege.ClampSafe(this.AverageVelocity * (1f - num1) + this.specRigidbody.Velocity * num1, -20f, 20f);
            if (this.m_isFalling)
                return;
            if ((this.IsDodgeRolling || this.m_dodgeRollState == PlayerController.DodgeRollState.AdditionalDelay) && (double) this.m_dodgeRollTimer >= (double) this.rollStats.GetModifiedTime(this))
            {
                if (this.DodgeRollIsBlink)
                {
                    if ((double) this.m_dodgeRollTimer > (double) this.rollStats.GetModifiedTime(this) + 0.10000000149011612)
                    {
                        this.IsEthereal = false;
                        this.IsVisible = true;
                        this.ClearDodgeRollState();
                        this.previousMineCart = (MineCartController) null;
                    }
                    else if ((double) this.m_dodgeRollTimer > (double) this.rollStats.GetModifiedTime(this))
                        this.EndBlinkDodge();
                }
                else
                {
                    this.ClearDodgeRollState();
                    this.previousMineCart = (MineCartController) null;
                }
            }
            if (this.IsDodgeRolling && this.OnIsRolling != null)
                this.OnIsRolling(this);
            CellVisualData.CellFloorType typeFromPosition = GameManager.Instance.Dungeon.GetFloorTypeFromPosition(this.specRigidbody.UnitBottomCenter);
            if (!this.m_prevFloorType.HasValue || this.m_prevFloorType.Value != typeFromPosition)
            {
                this.m_prevFloorType = new CellVisualData.CellFloorType?(typeFromPosition);
                int num2 = (int) AkSoundEngine.SetSwitch("FS_Surfaces", typeFromPosition.ToString(), this.gameObject);
            }
            this.m_playerCommandedDirection = Vector2.zero;
            this.IsFiring = false;
            if (!BraveUtility.isLoadingLevel && !GameManager.Instance.IsLoadingLevel)
            {
                this.ProcessDebugInput();
                if (GameUIRoot.Instance.MetalGearActive)
                {
                    if (this.m_activeActions.GunDownAction.WasPressed || this.m_activeActions.GunUpAction.WasPressed)
                        this.m_gunChangePressedWhileMetalGeared = true;
                }
                else
                    this.m_gunChangePressedWhileMetalGeared = false;
                if (this.AcceptingAnyInput)
                {
                    try
                    {
                        this.m_playerCommandedDirection = this.HandlePlayerInput();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Caught PlayerController.HandlePlayerInput() exception. i={this.exceptionTracker}, ex={ex.ToString()}");
                    }
                }
                if (this.m_newFloorNoInput && (double) this.m_playerCommandedDirection.magnitude > 0.0)
                    this.m_newFloorNoInput = false;
                if (this.usingForcedInput)
                    this.m_playerCommandedDirection = this.forcedInput;
                if (this.m_playerCommandedDirection != Vector2.zero)
                    GameManager.Instance.platformInterface.ProcessDlcUnlocks();
            }
            if (this.IsDodgeRolling || this.m_dodgeRollState == PlayerController.DodgeRollState.AdditionalDelay)
                this.HandleContinueDodgeRoll();
            if (PassiveItem.IsFlagSetForCharacter(this, typeof (HeavyBootsItem)))
                this.knockbackComponent = Vector2.zero;
            if (this.IsDodgeRolling)
            {
                if (this.usingForcedInput)
                    this.specRigidbody.Velocity = this.forcedInput.normalized * this.GetDodgeRollSpeed() + this.knockbackComponent + this.immutableKnockbackComponent;
                else if (this.DodgeRollIsBlink)
                    this.specRigidbody.Velocity = Vector2.zero;
                else
                    this.specRigidbody.Velocity = this.lockedDodgeRollDirection.normalized * this.GetDodgeRollSpeed() + this.knockbackComponent + this.immutableKnockbackComponent;
            }
            else
            {
                float num3 = 1f;
                if (!this.IsInCombat && GameManager.Options.IncreaseSpeedOutOfCombat)
                {
                    bool flag = true;
                    List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
                    if (allEnemies != null)
                    {
                        for (int index = 0; index < allEnemies.Count; ++index)
                        {
                            AIActor aiActor = allEnemies[index];
                            if ((bool) (UnityEngine.Object) aiActor && aiActor.IsMimicEnemy && !aiActor.IsGone && (double) Vector2.Distance(aiActor.CenterPosition, this.CenterPosition) < 40.0)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag)
                        num3 *= 1.5f;
                }
                this.specRigidbody.Velocity = this.ApplyMovementModifiers(this.m_playerCommandedDirection * this.stats.MovementSpeed * num3, this.knockbackComponent) + this.immutableKnockbackComponent;
            }
            this.specRigidbody.Velocity += this.ImpartedVelocity;
            this.ImpartedVelocity = Vector2.zero;
            this.m_maxIceFactor = typeFromPosition != CellVisualData.CellFloorType.Ice || this.IsFlying || PassiveItem.IsFlagSetForCharacter(this, typeof (HeavyBootsItem)) ? (!this.IsFlying || PassiveItem.IsFlagSetForCharacter(this, typeof (HeavyBootsItem)) ? Mathf.Clamp01(this.m_maxIceFactor - BraveTime.DeltaTime * 1.5f) : 0.0f) : Mathf.Clamp01(this.m_maxIceFactor + BraveTime.DeltaTime * 4f);
            if ((double) this.m_maxIceFactor > 0.0)
            {
                float max = Mathf.Max(this.m_lastVelocity.magnitude, this.specRigidbody.Velocity.magnitude);
                float b = Mathf.Lerp(1f / BraveTime.DeltaTime, Mathf.Lerp(0.5f, 1.5f, 1f - Mathf.Clamp01(Mathf.Abs(Vector2.Angle(this.m_lastVelocity, this.specRigidbody.Velocity)) / 180f)), this.m_maxIceFactor);
                if ((double) this.m_lastVelocity.magnitude < 0.25)
                    b = Mathf.Min(1f / BraveTime.DeltaTime, Mathf.Max(b * (float) (1.0 / (30.0 * (double) BraveTime.DeltaTime)), b));
                this.specRigidbody.Velocity = Vector2.Lerp(this.m_lastVelocity, this.specRigidbody.Velocity, b * BraveTime.DeltaTime);
                this.specRigidbody.Velocity = this.specRigidbody.Velocity.normalized * Mathf.Clamp(this.specRigidbody.Velocity.magnitude, 0.0f, max);
                if (float.IsNaN(this.specRigidbody.Velocity.x) || float.IsNaN(this.specRigidbody.Velocity.y))
                {
                    this.specRigidbody.Velocity = Vector2.zero;
                    UnityEngine.Debug.Log((object) $"{(object) this.m_lastVelocity}|{(object) this.m_lastVelocity.magnitude}| NaN correction");
                }
                if ((double) this.specRigidbody.Velocity.magnitude < (double) this.c_iceVelocityMinClamp)
                    this.specRigidbody.Velocity = Vector2.zero;
            }
            if (this.ZeroVelocityThisFrame)
            {
                this.specRigidbody.Velocity = Vector2.zero;
                this.ZeroVelocityThisFrame = false;
            }
            this.HandleFlipping(this.m_currentGunAngle);
            this.HandleAnimations(this.m_playerCommandedDirection, this.m_currentGunAngle);
            if (!this.IsPrimaryPlayer)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
                if ((bool) (UnityEngine.Object) otherPlayer)
                {
                    float num4 = -0.55f;
                    float heightOffGround = this.sprite.HeightOffGround;
                    if ((double) otherPlayer.sprite.transform.position.z == (double) this.sprite.transform.position.z)
                    {
                        if ((double) heightOffGround == (double) num4)
                            this.sprite.HeightOffGround = num4 + 0.1f;
                        else if ((double) heightOffGround == (double) num4 + 0.10000000149011612)
                            this.sprite.HeightOffGround = num4;
                        this.sprite.UpdateZDepth();
                    }
                }
            }
            if (this.IsSlidingOverSurface)
            {
                if ((double) this.sprite.HeightOffGround < 0.0)
                    this.sprite.HeightOffGround = 1.5f;
            }
            else if ((double) this.sprite.HeightOffGround > 0.0)
                this.sprite.HeightOffGround = !this.IsPrimaryPlayer ? -0.55f : -0.5f;
            this.HandleAttachedSpriteDepth(this.m_currentGunAngle);
            this.HandleShellCasingDisplacement();
            this.HandlePitChecks();
            this.HandleRoomProcessing();
            this.HandleGunAttachPoint();
            this.CheckSpawnEmergencyCrate();
            this.CheckSpawnAlertArrows();
            bool flag1 = this.QueryGroundedFrame() && !this.IsFlying;
            if (!this.m_cachedGrounded && flag1 && !this.m_isFalling && this.IsVisible)
                GameManager.Instance.Dungeon.dungeonDustups.InstantiateLandDustup((Vector3) this.specRigidbody.UnitCenter);
            this.m_cachedGrounded = flag1;
            if (this.m_playerCommandedDirection != Vector2.zero)
                this.m_lastNonzeroCommandedDirection = this.m_playerCommandedDirection;
            this.transform.position = this.transform.position.WithZ(this.transform.position.y - this.sprite.HeightOffGround);
            if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                this.CurrentGun.transform.position = this.CurrentGun.transform.position.WithZ(this.gunAttachPoint.position.z);
            if ((UnityEngine.Object) this.CurrentSecondaryGun != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.SecondaryGunPivot)
                this.CurrentSecondaryGun.transform.position = this.CurrentSecondaryGun.transform.position.WithZ(this.SecondaryGunPivot.position.z);
            if (this.m_capableOfStealing.UpdateTimers(BraveTime.DeltaTime))
                this.ForceRefreshInteractable = true;
            if ((double) this.m_superDuperAutoAimTimer <= 0.0)
                return;
            this.m_superDuperAutoAimTimer = Mathf.Max(0.0f, this.m_superDuperAutoAimTimer - BraveTime.DeltaTime);
        }

        private void UpdatePlayerShadowPosition()
        {
            GameObject defaultBlobShadow = this.GenerateDefaultBlobShadow();
            defaultBlobShadow.transform.localPosition = Vector3.zero;
            defaultBlobShadow.transform.localPosition = new Vector3(this.SpriteBottomCenter.x - this.transform.position.x, 0.0f, 0.1f);
            defaultBlobShadow.transform.position = defaultBlobShadow.transform.position.Quantize(1f / 16f);
        }

        public void SwapToAlternateCostume(tk2dSpriteAnimation overrideTargetLibrary = null)
        {
            if ((UnityEngine.Object) this.AlternateCostumeLibrary == (UnityEngine.Object) null && (UnityEngine.Object) overrideTargetLibrary == (UnityEngine.Object) null)
                return;
            if ((UnityEngine.Object) this.BaseAnimationLibrary != (UnityEngine.Object) null)
                this.ResetOverrideAnimationLibrary();
            tk2dSpriteAnimation library = this.spriteAnimator.Library;
            this.spriteAnimator.Library = this.AlternateCostumeLibrary;
            this.AlternateCostumeLibrary = library;
            this.spriteAnimator.StopAndResetFrame();
            if (this.spriteAnimator.CurrentClip != null)
                this.spriteAnimator.Play(this.spriteAnimator.CurrentClip.name);
            this.IsUsingAlternateCostume = !this.IsUsingAlternateCostume;
            if (this.HandsOnAltCostume)
                this.ForceHandless = !this.IsUsingAlternateCostume;
            if (this.SwapHandsOnAltCostume)
            {
                this.RevertHandsToBaseType();
                tk2dSpriteCollectionData newCollection = this.sprite.Collection;
                tk2dSpriteAnimationClip clipByName = this.spriteAnimator.Library.GetClipByName(this.GetBaseAnimationName(Vector2.zero, 0.0f));
                if (clipByName != null && clipByName.frames != null && clipByName.frames.Length > 0)
                    newCollection = clipByName.frames[0].spriteCollection;
                string altHandName = this.altHandName;
                if ((bool) (UnityEngine.Object) this.primaryHand)
                {
                    this.altHandName = this.primaryHand.sprite.GetCurrentSpriteDef().name;
                    this.primaryHand.sprite.SetSprite(newCollection, altHandName);
                }
                if ((bool) (UnityEngine.Object) this.secondaryHand)
                    this.secondaryHand.sprite.SetSprite(newCollection, altHandName);
            }
            if ((bool) (UnityEngine.Object) this.lostAllArmorVFX && (bool) (UnityEngine.Object) this.lostAllArmorAltVfx)
            {
                GameObject lostAllArmorVfx = this.lostAllArmorVFX;
                this.lostAllArmorVFX = this.lostAllArmorAltVfx;
                this.lostAllArmorAltVfx = lostAllArmorVfx;
            }
            this.m_spriteDimensions = this.sprite.GetUntrimmedBounds().size;
            this.UpdatePlayerShadowPosition();
        }

        public void RevertHandsToBaseType()
        {
            if (!this.m_usingCustomHandType)
                return;
            this.m_usingCustomHandType = false;
            if ((bool) (UnityEngine.Object) this.primaryHand)
                this.primaryHand.sprite.SetSprite(this.m_baseHandCollection, this.m_baseHandId);
            if ((bool) (UnityEngine.Object) this.secondaryHand)
                this.secondaryHand.sprite.SetSprite(this.m_baseHandCollection, this.m_baseHandId);
            this.m_baseHandCollection = (tk2dSpriteCollectionData) null;
        }

        public void ChangeHandsToCustomType(tk2dSpriteCollectionData handCollection, int handId)
        {
            if (!this.m_usingCustomHandType)
            {
                this.m_baseHandId = this.primaryHand.sprite.spriteId;
                this.m_baseHandCollection = this.primaryHand.sprite.Collection;
            }
            this.m_usingCustomHandType = true;
            if ((bool) (UnityEngine.Object) this.primaryHand)
                this.primaryHand.sprite.SetSprite(handCollection, handId);
            if (!(bool) (UnityEngine.Object) this.secondaryHand)
                return;
            this.secondaryHand.sprite.SetSprite(handCollection, handId);
        }

        private void ResetOverrideAnimationLibrary()
        {
            if (!((UnityEngine.Object) this.BaseAnimationLibrary != (UnityEngine.Object) null) || !((UnityEngine.Object) this.spriteAnimator.Library != (UnityEngine.Object) this.BaseAnimationLibrary))
                return;
            this.spriteAnimator.Library = this.BaseAnimationLibrary;
            this.spriteAnimator.StopAndResetFrame();
            this.spriteAnimator.Play(this.spriteAnimator.CurrentClip.name);
            this.BaseAnimationLibrary = (tk2dSpriteAnimation) null;
        }

        private void UpdateTurboModeStats()
        {
            if (GameManager.IsTurboMode)
            {
                if (this.m_turboSpeedModifier == null)
                {
                    this.m_turboSpeedModifier = StatModifier.Create(PlayerStats.StatType.MovementSpeed, StatModifier.ModifyMethod.MULTIPLICATIVE, TurboModeController.sPlayerSpeedMultiplier);
                    this.m_turboSpeedModifier.ignoredForSaveData = true;
                    this.ownerlessStatModifiers.Add(this.m_turboSpeedModifier);
                }
                if (this.m_turboRollSpeedModifier == null)
                {
                    this.m_turboRollSpeedModifier = StatModifier.Create(PlayerStats.StatType.DodgeRollSpeedMultiplier, StatModifier.ModifyMethod.MULTIPLICATIVE, TurboModeController.sPlayerRollSpeedMultiplier);
                    this.m_turboRollSpeedModifier.ignoredForSaveData = true;
                    this.ownerlessStatModifiers.Add(this.m_turboRollSpeedModifier);
                }
                if (this.IsPrimaryPlayer)
                {
                    if (this.m_turboEnemyBulletModifier == null)
                    {
                        this.m_turboEnemyBulletModifier = StatModifier.Create(PlayerStats.StatType.EnemyProjectileSpeedMultiplier, StatModifier.ModifyMethod.MULTIPLICATIVE, TurboModeController.sEnemyBulletSpeedMultiplier);
                        this.m_turboEnemyBulletModifier.ignoredForSaveData = true;
                        this.ownerlessStatModifiers.Add(this.m_turboEnemyBulletModifier);
                        this.stats.RecalculateStats(this);
                    }
                }
                else if (this.m_turboEnemyBulletModifier != null)
                {
                    this.ownerlessStatModifiers.Remove(this.m_turboEnemyBulletModifier);
                    this.m_turboEnemyBulletModifier = (StatModifier) null;
                    this.stats.RecalculateStats(this);
                }
                if ((this.m_turboEnemyBulletModifier == null || (double) this.m_turboEnemyBulletModifier.amount == (double) TurboModeController.sEnemyBulletSpeedMultiplier) && (double) this.m_turboSpeedModifier.amount == (double) TurboModeController.sPlayerSpeedMultiplier && (double) this.m_turboRollSpeedModifier.amount == (double) TurboModeController.sPlayerRollSpeedMultiplier)
                    return;
                this.m_turboRollSpeedModifier.amount = TurboModeController.sPlayerRollSpeedMultiplier;
                this.m_turboSpeedModifier.amount = TurboModeController.sPlayerSpeedMultiplier;
                this.m_turboEnemyBulletModifier.amount = TurboModeController.sEnemyBulletSpeedMultiplier;
                this.stats.RecalculateStats(this);
            }
            else
            {
                if (this.m_turboSpeedModifier == null && this.m_turboEnemyBulletModifier == null && this.m_turboRollSpeedModifier == null)
                    return;
                this.ownerlessStatModifiers.Remove(this.m_turboEnemyBulletModifier);
                this.m_turboEnemyBulletModifier = (StatModifier) null;
                this.ownerlessStatModifiers.Remove(this.m_turboSpeedModifier);
                this.m_turboSpeedModifier = (StatModifier) null;
                this.ownerlessStatModifiers.Remove(this.m_turboRollSpeedModifier);
                this.m_turboRollSpeedModifier = (StatModifier) null;
                this.stats.RecalculateStats(this);
            }
        }

        private void LateUpdate()
        {
            this.UpdateTurboModeStats();
            this.WasPausedThisFrame = false;
            if (!this.m_handleDodgeRollStartThisFrame)
                this.m_timeHeldBlinkButton = 0.0f;
            if (this.DeferredStatRecalculationRequired)
                this.stats.RecalculateStatsInternal(this);
            this.m_wasTalkingThisFrame = this.IsTalking;
            this.m_lastVelocity = this.specRigidbody.Velocity;
            if (this.IsPrimaryPlayer && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER && !this.m_newFloorNoInput && !GameManager.Instance.IsPaused && !Dungeon.IsGenerating && !GameManager.Instance.IsLoadingLevel)
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIME_PLAYED, UnityEngine.Time.unscaledDeltaTime);
            if (GameManager.Options.RealtimeReflections)
                this.sprite.renderer.sharedMaterial.SetFloat("_ReflectionYOffset", this.actorReflectionAdditionalOffset);
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsLoadingLevel)
                return;
            if (this.CurrentRoom == null)
            {
                this.m_isInCombat = false;
            }
            else
            {
                bool isInCombat = this.m_isInCombat;
                this.m_isInCombat = this.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
                if (this.OnEnteredCombat != null && this.m_isInCombat && !isInCombat)
                    this.OnEnteredCombat();
            }
            if (!this.IsPrimaryPlayer && this.CharacterUsesRandomGuns != GameManager.Instance.GetOtherPlayer(this).CharacterUsesRandomGuns)
                this.CharacterUsesRandomGuns = GameManager.Instance.GetOtherPlayer(this).CharacterUsesRandomGuns;
            this.UpdateStencilVal();
            if (this.CharacterUsesRandomGuns)
            {
                this.m_gunGameElapsed += BraveTime.DeltaTime;
                if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null && this.CurrentGun.CurrentAmmo == 0)
                    this.ChangeToRandomGun();
                else if (this.CurrentRoom != null && (double) this.m_gunGameElapsed > 20.0 && this.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && this.IsInCombat)
                    this.ChangeToRandomGun();
                else if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null && !this.IsGhost && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES && !GameManager.Instance.IsLoadingLevel)
                {
                    UnityEngine.Debug.Log((object) "Changing to random gun because we don't have any gun at all!");
                    this.ChangeToRandomGun();
                }
            }
            if ((bool) (UnityEngine.Object) this.specRigidbody)
            {
                float magnitude = (this.specRigidbody.Velocity * BraveTime.DeltaTime).magnitude;
                if (this.IsFlying)
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.DISTANCE_FLOWN, magnitude);
                else
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.DISTANCE_WALKED, magnitude);
            }
            if (this.characterIdentity != PlayableCharacters.Eevee)
            {
                if ((UnityEngine.Object) this.OverrideAnimationLibrary != (UnityEngine.Object) null)
                {
                    if ((UnityEngine.Object) this.spriteAnimator.Library != (UnityEngine.Object) this.OverrideAnimationLibrary)
                    {
                        this.BaseAnimationLibrary = this.spriteAnimator.Library;
                        this.spriteAnimator.Library = this.OverrideAnimationLibrary;
                        this.spriteAnimator.StopAndResetFrame();
                        this.spriteAnimator.Play(this.spriteAnimator.CurrentClip.name);
                    }
                }
                else if ((UnityEngine.Object) this.BaseAnimationLibrary != (UnityEngine.Object) null && (UnityEngine.Object) this.spriteAnimator.Library != (UnityEngine.Object) this.BaseAnimationLibrary)
                    this.ResetOverrideAnimationLibrary();
            }
            this.CurrentFloorDamageCooldown = Mathf.Max(0.0f, this.CurrentFloorDamageCooldown - BraveTime.DeltaTime);
            if ((double) this.m_blankCooldownTimer > 0.0)
            {
                this.m_blankCooldownTimer = Mathf.Max(0.0f, this.m_blankCooldownTimer - BraveTime.DeltaTime);
                if (this.IsGhost && (double) this.m_blankCooldownTimer <= 0.0)
                    this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
            }
            if ((double) this.m_highStressTimer > 0.0)
            {
                this.m_highStressTimer -= BraveTime.DeltaTime;
                if ((double) this.m_highStressTimer <= 0.0 && (bool) (UnityEngine.Object) this.healthHaver)
                    this.healthHaver.NextShotKills = false;
            }
            if (!this.IsGhost)
            {
                this.DeregisterOverrideColor("player status effects");
                Color a1 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                Color targetColor = this.baseFlatColorOverride;
                float a2 = 0.25f + Mathf.PingPong(UnityEngine.Time.timeSinceLevelLoad, 0.25f);
                GameUIRoot.Instance.SetAmmoCountColor(Color.white, this);
                if ((double) this.CurrentDrainMeterValue > 0.0)
                {
                    if ((UnityEngine.Object) this.m_currentGoop == (UnityEngine.Object) null || !this.m_currentGoop.DrainsAmmo || !this.QueryGroundedFrame())
                        this.CurrentDrainMeterValue = Mathf.Max(0.0f, this.CurrentDrainMeterValue - BraveTime.DeltaTime * 0.1f);
                    GameUIRoot.Instance.SetAmmoCountColor(Color.Lerp(Color.white, new Color(1f, 0.0f, 0.0f, 1f), this.CurrentDrainMeterValue), this);
                    if ((double) this.CurrentDrainMeterValue >= 1.0)
                        GameUIRoot.Instance.SetAmmoCountColor(new Color(1f, 0.0f, 0.0f, 1f), this);
                }
                else
                    this.inventory.ClearAmmoDrain();
                Color a3 = Color.Lerp(a1, new Color(0.65f, 0.0f, 0.6f, a2), this.CurrentDrainMeterValue);
                if (this.IsOnFire && (double) this.healthHaver.GetDamageModifierForType(CoreDamageTypes.Fire) > 0.0 && !this.IsEthereal && !this.IsTalking && !this.HasActiveBonusSynergy(CustomSynergyType.FIRE_IMMUNITY))
                {
                    if (!this.IsDodgeRolling)
                        this.IncreaseFire(BraveTime.DeltaTime * 0.666666f);
                    else
                        this.IncreaseFire(BraveTime.DeltaTime * 0.2f);
                    if ((double) this.CurrentFireMeterValue >= 1.0)
                    {
                        --this.CurrentFireMeterValue;
                        if (!this.m_isFalling)
                            this.healthHaver.ApplyDamage(0.5f, Vector2.zero, StringTableManager.GetEnemiesString("#FIRE"), CoreDamageTypes.Fire, DamageCategory.Environment, true);
                        GlobalSparksDoer.DoRadialParticleBurst(12, this.specRigidbody.HitboxPixelCollider.UnitBottomLeft.ToVector3ZisY(), this.specRigidbody.HitboxPixelCollider.UnitTopRight.ToVector3ZisY(), 15f, 2.25f, 1f, startColor: new Color?(Color.red));
                    }
                    targetColor = new Color(1f, 0.0f, 0.0f, 0.7f);
                }
                else
                {
                    this.CurrentFireMeterValue = 0.0f;
                    this.IsOnFire = false;
                }
                if ((double) this.CurrentPoisonMeterValue > 0.0 && (double) this.healthHaver.GetDamageModifierForType(CoreDamageTypes.Poison) > 0.0)
                {
                    if ((UnityEngine.Object) this.m_currentGoop == (UnityEngine.Object) null || !this.m_currentGoop.damagesPlayers || !this.QueryGroundedFrame())
                        this.CurrentPoisonMeterValue = Mathf.Max(0.0f, this.CurrentPoisonMeterValue - BraveTime.DeltaTime * 0.5f);
                }
                else
                    this.CurrentPoisonMeterValue = 0.0f;
                Color a4 = Color.Lerp(a3, new Color(0.0f, 1f, 0.0f, a2), this.CurrentPoisonMeterValue);
                if ((double) this.CurrentCurseMeterValue > 0.0 && this.CurseIsDecaying)
                    this.CurrentCurseMeterValue = Mathf.Max(0.0f, this.CurrentCurseMeterValue - BraveTime.DeltaTime * 0.5f);
                Color overrideColor = Color.Lerp(a4, new Color(0.0f, 0.0f, 0.0f, a2), this.CurrentCurseMeterValue);
                if ((double) this.CurrentStoneGunTimer > 0.0)
                {
                    this.CurrentStoneGunTimer -= BraveTime.DeltaTime;
                    targetColor = new Color(0.4f, 0.4f, 0.33f, Mathf.Clamp01(this.CurrentStoneGunTimer / 0.25f));
                }
                this.RegisterOverrideColor(overrideColor, "player status effects");
                if (!this.FlatColorOverridden)
                    this.ChangeFlatColorOverride(targetColor);
                GameUIRoot.Instance.UpdatePlayerHealthUI(this, this.healthHaver);
                GameUIRoot.Instance.UpdatePlayerBlankUI(this);
                if ((UnityEngine.Object) GameUIRoot.Instance != (UnityEngine.Object) null)
                {
                    GameUIRoot.Instance.UpdateGunData(this.inventory, this.m_equippedGunShift, this);
                    GameUIRoot.Instance.UpdateItemData(this, this.CurrentItem, this.activeItems);
                    GameUIRoot.Instance.GetReloadBarForPlayer(this).UpdateStatusBars(this);
                    for (int index = 0; index < this.activeItems.Count; ++index)
                    {
                        if ((UnityEngine.Object) this.activeItems[index] == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) this.activeItems[index])
                            UnityEngine.Debug.Log((object) ("We have encountered a null item at item index: " + (object) index));
                    }
                    if ((UnityEngine.Object) this.CurrentItem == (UnityEngine.Object) null)
                        this.m_selectedItemIndex = 0;
                }
            }
            else
            {
                GameUIRoot.Instance.UpdateGhostUI(this);
                this.ToggleHandRenderers(false, "ghostliness");
                this.IsOnFire = false;
                this.CurrentPoisonMeterValue = 0.0f;
                this.CurrentFireMeterValue = 0.0f;
                this.CurrentDrainMeterValue = 0.0f;
                this.CurrentCurseMeterValue = 0.0f;
                this.ChangeFlatColorOverride(Color.Lerp(this.m_ghostChargedColor, this.m_ghostUnchargedColor, Mathf.Clamp01(this.m_blankCooldownTimer / 5f)));
                if (this.CurrentInputState != PlayerInputState.NoInput && !GameManager.Instance.MainCameraController.ManualControl)
                {
                    if (!GameManager.Instance.MainCameraController.PointIsVisible(this.CenterPosition, 0.05f))
                    {
                        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
                        IntVector2? nullable = new IntVector2?();
                        if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.CurrentRoom != null)
                        {
                            CellValidator cellValidator = (CellValidator) (p => GameManager.Instance.MainCameraController.PointIsVisible(p.ToCenterVector2()));
                            Vector2 nearbyPoint = BraveMathCollege.ClosestPointOnRectangle(this.CenterPosition, GameManager.Instance.MainCameraController.MinVisiblePoint, GameManager.Instance.MainCameraController.MaxVisiblePoint - GameManager.Instance.MainCameraController.MinVisiblePoint);
                            nullable = otherPlayer.CurrentRoom.GetNearestAvailableCell(nearbyPoint, new IntVector2?(IntVector2.One * 3), new CellTypes?(CellTypes.FLOOR | CellTypes.PIT), cellValidator: cellValidator);
                        }
                        if (nullable.HasValue)
                        {
                            LootEngine.DoDefaultPurplePoof(this.CenterPosition, true);
                            this.WarpToPoint(nullable.Value.ToVector2() + Vector2.one);
                            LootEngine.DoDefaultPurplePoof(this.CenterPosition, true);
                        }
                        else
                        {
                            LootEngine.DoDefaultPurplePoof(this.CenterPosition, true);
                            this.ReuniteWithOtherPlayer(GameManager.Instance.GetOtherPlayer(this));
                            LootEngine.DoDefaultPurplePoof(this.CenterPosition, true);
                        }
                    }
                    else if (!GameManager.Instance.MainCameraController.PointIsVisible(this.CenterPosition, 0.0f))
                        this.specRigidbody.ImpartedPixelsToMove = ((BraveMathCollege.ClosestPointOnRectangle(this.CenterPosition, GameManager.Instance.MainCameraController.MinVisiblePoint, GameManager.Instance.MainCameraController.MaxVisiblePoint - GameManager.Instance.MainCameraController.MinVisiblePoint) - this.CenterPosition) * 16f).ToIntVector2();
                }
            }
            if ((UnityEngine.Object) Minimap.Instance != (UnityEngine.Object) null)
                Minimap.Instance.UpdatePlayerPositionExact(this.transform.position, this);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                this.HandleCoopSpecificTimers();
            if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
                return;
            GameUIRoot.Instance.UpdatePlayerConsumables(this.carriedConsumables);
        }

        private void SetStencilVal(int v)
        {
            if (!(bool) (UnityEngine.Object) this.sprite || !(bool) (UnityEngine.Object) this.sprite.renderer)
                return;
            this.sprite.renderer.material.SetInt(this.m_stencilID, v);
        }

        private void UpdateStencilVal()
        {
            if (!(bool) (UnityEngine.Object) this.sprite || !(bool) (UnityEngine.Object) this.sprite.renderer)
                return;
            switch (this.sprite.renderer.material.GetInt(this.m_stencilID))
            {
                case 146:
                    break;
                case 147:
                    break;
                default:
                    this.SetStencilVal(146);
                    break;
            }
        }

        public void ChangeSpecialShaderFlag(int flagIndex, float val)
        {
            Vector4 vector = this.healthHaver.bodySprites[0].renderer.material.GetVector(this.m_specialFlagsID);
            vector[flagIndex] = val;
            for (int index = 0; index < this.healthHaver.bodySprites.Count; ++index)
            {
                this.healthHaver.bodySprites[index].usesOverrideMaterial = true;
                this.healthHaver.bodySprites[index].renderer.material.SetColor(this.m_specialFlagsID, (Color) vector);
            }
            if ((bool) (UnityEngine.Object) this.primaryHand && (bool) (UnityEngine.Object) this.primaryHand.sprite)
                this.primaryHand.sprite.renderer.material.SetColor(this.m_specialFlagsID, (Color) vector);
            if (!(bool) (UnityEngine.Object) this.secondaryHand || !(bool) (UnityEngine.Object) this.secondaryHand.sprite)
                return;
            this.secondaryHand.sprite.renderer.material.SetColor(this.m_specialFlagsID, (Color) vector);
        }

        public void ChangeFlatColorOverride(Color targetColor)
        {
            for (int index = 0; index < this.healthHaver.bodySprites.Count; ++index)
            {
                this.healthHaver.bodySprites[index].usesOverrideMaterial = true;
                this.healthHaver.bodySprites[index].renderer.material.SetColor(this.m_overrideFlatColorID, targetColor);
            }
        }

        public void UpdateRandomStartingEquipmentCoop(bool shouldUseRandom)
        {
            if (shouldUseRandom && !this.m_usesRandomStartingEquipment)
            {
                this.m_usesRandomStartingEquipment = true;
                if ((bool) (UnityEngine.Object) GameManager.Instance && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                    GameStatsManager.Instance.CurrentEeveeEquipSeed = -1;
                if (GameStatsManager.Instance.CurrentEeveeEquipSeed < 0)
                    GameStatsManager.Instance.CurrentEeveeEquipSeed = UnityEngine.Random.Range(1, 10000000);
                this.m_randomStartingEquipmentSeed = GameStatsManager.Instance.CurrentEeveeEquipSeed;
                this.SetUpRandomStartingEquipment();
                this.m_turboEnemyBulletModifier = (StatModifier) null;
                this.m_turboRollSpeedModifier = (StatModifier) null;
                this.m_turboSpeedModifier = (StatModifier) null;
                this.ResetToFactorySettings(forceAllItems: true);
            }
            else
            {
                if (shouldUseRandom || !this.m_usesRandomStartingEquipment)
                    return;
                this.m_usesRandomStartingEquipment = false;
                PlayerController component = GameManager.LastUsedCoopPlayerPrefab.GetComponent<PlayerController>();
                this.startingGunIds = new List<int>((IEnumerable<int>) component.startingGunIds);
                this.startingAlternateGunIds = new List<int>((IEnumerable<int>) component.startingAlternateGunIds);
                this.startingPassiveItemIds = new List<int>((IEnumerable<int>) component.startingPassiveItemIds);
                this.startingActiveItemIds = new List<int>((IEnumerable<int>) component.startingActiveItemIds);
                this.finalFightGunIds = new List<int>((IEnumerable<int>) component.finalFightGunIds);
                this.m_turboEnemyBulletModifier = (StatModifier) null;
                this.m_turboRollSpeedModifier = (StatModifier) null;
                this.m_turboSpeedModifier = (StatModifier) null;
                this.ResetToFactorySettings(forceAllItems: true);
            }
        }

        public void ResetToFactorySettings(
            bool includeFullHeal = false,
            bool useFinalFightGuns = false,
            bool forceAllItems = false)
        {
            if (!this.IsDarkSoulsHollow || useFinalFightGuns)
                this.inventory.DestroyAllGuns();
            if (useFinalFightGuns && this.finalFightGunIds != null && this.finalFightGunIds.Count > 0)
            {
                for (int index = 0; index < this.finalFightGunIds.Count; ++index)
                {
                    if (this.finalFightGunIds[index] >= 0)
                        this.inventory.AddGunToInventory(PickupObjectDatabase.GetById(this.finalFightGunIds[index]) as Gun, true);
                }
            }
            else if (this.UsingAlternateStartingGuns)
            {
                for (int index = 0; index < this.startingAlternateGunIds.Count; ++index)
                {
                    Gun byId = PickupObjectDatabase.GetById(this.startingAlternateGunIds[index]) as Gun;
                    if (forceAllItems || includeFullHeal || useFinalFightGuns || byId.PreventStartingOwnerFromDropping)
                        this.inventory.AddGunToInventory(byId, true);
                }
            }
            else
            {
                for (int index = 0; index < this.startingGunIds.Count; ++index)
                {
                    Gun byId = PickupObjectDatabase.GetById(this.startingGunIds[index]) as Gun;
                    if (forceAllItems || includeFullHeal || useFinalFightGuns || byId.PreventStartingOwnerFromDropping)
                        this.inventory.AddGunToInventory(byId, true);
                }
            }
            for (int index = 0; index < this.passiveItems.Count; ++index)
            {
                if (!this.passiveItems[index].PersistsOnDeath)
                {
                    DebrisObject debrisObject = this.DropPassiveItem(this.passiveItems[index]);
                    if ((UnityEngine.Object) debrisObject != (UnityEngine.Object) null)
                    {
                        UnityEngine.Object.Destroy((UnityEngine.Object) debrisObject.gameObject);
                        --index;
                    }
                }
            }
            for (int index = 0; index < this.activeItems.Count; ++index)
            {
                if (!this.activeItems[index].PersistsOnDeath)
                {
                    DebrisObject debrisObject = this.DropActiveItem(this.activeItems[index], isDeathDrop: true);
                    if ((UnityEngine.Object) debrisObject != (UnityEngine.Object) null)
                    {
                        UnityEngine.Object.Destroy((UnityEngine.Object) debrisObject.gameObject);
                        --index;
                    }
                }
            }
            for (int index = 0; index < this.startingActiveItemIds.Count; ++index)
            {
                PlayerItem byId = PickupObjectDatabase.GetById(this.startingActiveItemIds[index]) as PlayerItem;
                if ((forceAllItems || !byId.consumable) && !this.HasActiveItem(byId.PickupObjectId) && (forceAllItems || includeFullHeal || useFinalFightGuns || byId.PreventStartingOwnerFromDropping))
                {
                    EncounterTrackable.SuppressNextNotification = true;
                    byId.Pickup(this);
                    EncounterTrackable.SuppressNextNotification = false;
                }
            }
            for (int index = 0; index < this.startingPassiveItemIds.Count; ++index)
            {
                PassiveItem byId = PickupObjectDatabase.GetById(this.startingPassiveItemIds[index]) as PassiveItem;
                if (!this.HasPassiveItem(byId.PickupObjectId))
                {
                    EncounterTrackable.SuppressNextNotification = true;
                    LootEngine.GivePrefabToPlayer(byId.gameObject, this);
                    EncounterTrackable.SuppressNextNotification = false;
                }
            }
            if (this.ownerlessStatModifiers != null)
            {
                if (useFinalFightGuns || includeFullHeal)
                {
                    this.ownerlessStatModifiers.Clear();
                }
                else
                {
                    for (int index = 0; index < this.ownerlessStatModifiers.Count; ++index)
                    {
                        if (!this.ownerlessStatModifiers[index].PersistsOnCoopDeath)
                        {
                            this.ownerlessStatModifiers.RemoveAt(index);
                            --index;
                        }
                    }
                }
            }
            this.stats.RecalculateStats(this);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                GameManager.Instance.GetOtherPlayer(this).stats.RecalculateStats(GameManager.Instance.GetOtherPlayer(this));
            if (useFinalFightGuns && this.characterIdentity == PlayableCharacters.Robot)
                this.healthHaver.Armor = 6f;
            if (!includeFullHeal)
                return;
            this.healthHaver.FullHeal();
        }

        [DebuggerHidden]
        private IEnumerator CoopResurrectInternal(
            Vector3 targetPosition,
            tk2dSpriteAnimationClip clipToWaitFor,
            bool isChest = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__CoopResurrectInternalc__Iterator2()
            {
                targetPosition = targetPosition,
                isChest = isChest,
                clipToWaitFor = clipToWaitFor,
                _this = this
            };
        }

        public virtual void ResurrectFromBossKill()
        {
            PlayerController playerController = !((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) this) ? GameManager.Instance.PrimaryPlayer : GameManager.Instance.SecondaryPlayer;
            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
            tk2dSpriteAnimationClip clipToWaitFor = this.spriteAnimator.GetClipByName("chest_recover") == null ? this.spriteAnimator.GetClipByName(!this.UseArmorlessAnim ? "pitfall_return" : "pitfall_return_armorless") : this.spriteAnimator.GetClipByName(!this.UseArmorlessAnim ? "chest_recover" : "chest_recover_armorless");
            Chest.ToggleCoopChests(false);
            CellData cellData = GameManager.Instance.Dungeon.data[this.transform.position.IntXY(VectorConversions.Floor)];
            Vector3 targetPosition = this.transform.position;
            if (cellData == null || cellData.type != CellType.FLOOR || cellData.IsPlayerInaccessible)
                targetPosition = playerController.CurrentRoom.GetBestRewardLocation(IntVector2.One, RoomHandler.RewardLocationStyle.PlayerCenter).ToVector3();
            this.StartCoroutine(this.CoopResurrectInternal(targetPosition, clipToWaitFor));
        }

        public void ResurrectFromChest(Vector2 chestBottomCenter)
        {
            tk2dSpriteAnimationClip clipToWaitFor = this.spriteAnimator.GetClipByName("chest_recover") == null ? this.spriteAnimator.GetClipByName(!this.UseArmorlessAnim ? "pitfall_return" : "pitfall_return_armorless") : this.spriteAnimator.GetClipByName(!this.UseArmorlessAnim ? "chest_recover" : "chest_recover_armorless");
            Chest.ToggleCoopChests(false);
            if (this.confettiPaths == null)
                this.confettiPaths = new string[3]
                {
                    "Global VFX/Confetti_Blue_001",
                    "Global VFX/Confetti_Yellow_001",
                    "Global VFX/Confetti_Green_001"
                };
            Vector2 vector = chestBottomCenter + new Vector2(-0.75f, -0.25f);
            for (int index = 0; index < 8; ++index)
            {
                WaftingDebrisObject component = UnityEngine.Object.Instantiate<GameObject>((GameObject) ResourceCache.Acquire(this.confettiPaths[UnityEngine.Random.Range(0, 3)])).GetComponent<WaftingDebrisObject>();
                component.sprite.PlaceAtPositionByAnchor(vector.ToVector3ZUp() + new Vector3(0.5f, 0.5f, 0.0f), tk2dBaseSprite.Anchor.MiddleCenter);
                Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                component.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0.0f);
            }
            this.StartCoroutine(this.CoopResurrectInternal(vector.ToVector3ZUp(), clipToWaitFor, true));
        }

        private void HandleCoopSpecificTimers()
        {
            PlayerController other = !this.IsPrimaryPlayer ? GameManager.Instance.PrimaryPlayer : GameManager.Instance.SecondaryPlayer;
            if ((UnityEngine.Object) other != (UnityEngine.Object) null && !other.healthHaver.IsDead && other.CurrentRoom != null && other.CurrentRoom.IsSealed && other.CurrentRoom != this.CurrentRoom)
            {
                this.m_coopRoomTimer += BraveTime.DeltaTime;
                if ((double) this.m_coopRoomTimer <= 1.0)
                    return;
                if (this.IsGhost)
                    LootEngine.DoDefaultPurplePoof(this.CenterPosition, true);
                this.ReuniteWithOtherPlayer(other);
                if (this.IsGhost)
                    LootEngine.DoDefaultPurplePoof(this.CenterPosition, true);
                this.healthHaver.TriggerInvulnerabilityPeriod();
                this.m_coopRoomTimer = 0.0f;
            }
            else
                this.m_coopRoomTimer = 0.0f;
        }

        public void DoPostProcessProjectile(Projectile p)
        {
            p.Owner = (GameActor) this;
            this.HandleShadowBulletStat(p);
            float num1 = 1f;
            if ((bool) (UnityEngine.Object) this.CurrentGun && this.CurrentGun.DefaultModule != null)
            {
                float num2 = 0.0f;
                if ((UnityEngine.Object) this.CurrentGun.Volley != (UnityEngine.Object) null)
                {
                    List<ProjectileModule> projectiles = this.CurrentGun.Volley.projectiles;
                    for (int index = 0; index < projectiles.Count; ++index)
                        num2 += projectiles[index].GetEstimatedShotsPerSecond(this.CurrentGun.reloadTime);
                }
                else if (this.CurrentGun.DefaultModule != null)
                    num2 += this.CurrentGun.DefaultModule.GetEstimatedShotsPerSecond(this.CurrentGun.reloadTime);
                if ((double) num2 > 0.0)
                    num1 = 3.5f / num2;
            }
            if (this.PostProcessProjectile == null)
                return;
            this.PostProcessProjectile(p, num1);
        }

        public void CustomPostProcessProjectile(Projectile p, float effectChanceScalar)
        {
            if (this.PostProcessProjectile == null)
                return;
            this.PostProcessProjectile(p, effectChanceScalar);
        }

        public void DoPostProcessThrownGun(Projectile p)
        {
            if (this.PostProcessThrownGun == null)
                return;
            this.PostProcessThrownGun(p);
        }

        public void SpawnShadowBullet(Projectile obj, bool shadowColoration)
        {
            float a = 0.0f;
            if ((bool) (UnityEngine.Object) obj.sprite && (double) obj.sprite.GetBounds().size.x > 0.5)
                a += obj.sprite.GetBounds().size.x / 10f;
            float additionalDelay = Mathf.Max(a, 0.1f);
            this.StartCoroutine(this.SpawnShadowBullet(obj, additionalDelay, shadowColoration));
        }

        [DebuggerHidden]
        protected IEnumerator SpawnShadowBullet(
            Projectile obj,
            float additionalDelay,
            bool shadowColoration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__SpawnShadowBulletc__Iterator3()
            {
                obj = obj,
                additionalDelay = additionalDelay,
                shadowColoration = shadowColoration,
                _this = this
            };
        }

        protected void HandleShadowBulletStat(Projectile obj)
        {
            if ((double) UnityEngine.Random.value < (double) (this.stats.GetStatValue(PlayerStats.StatType.ExtremeShadowBulletChance) / 100f))
            {
                this.StartCoroutine(this.SpawnShadowBullet(obj, 0.05f, false));
                if ((double) UnityEngine.Random.value >= 0.5)
                    return;
                this.StartCoroutine(this.SpawnShadowBullet(obj, 0.1f, false));
                if ((double) UnityEngine.Random.value >= 0.5)
                    return;
                this.StartCoroutine(this.SpawnShadowBullet(obj, 0.15f, false));
                if ((double) UnityEngine.Random.value >= 0.5)
                    return;
                this.StartCoroutine(this.SpawnShadowBullet(obj, 0.2f, false));
            }
            else
            {
                if ((double) UnityEngine.Random.value >= (double) (this.stats.GetStatValue(PlayerStats.StatType.ShadowBulletChance) / 100f))
                    return;
                this.SpawnShadowBullet(obj, true);
            }
        }

        public void DoPostProcessBeam(BeamController beam)
        {
            int num1 = Mathf.FloorToInt(this.stats.GetStatValue(PlayerStats.StatType.AdditionalShotBounces));
            int num2 = Mathf.FloorToInt(this.stats.GetStatValue(PlayerStats.StatType.AdditionalShotPiercing));
            if ((num1 > 0 || num2 > 0) && beam is BasicBeamController)
            {
                BasicBeamController basicBeamController = beam as BasicBeamController;
                if (!basicBeamController.playerStatsModified)
                {
                    basicBeamController.penetration += num2;
                    basicBeamController.reflections += num1;
                    basicBeamController.playerStatsModified = true;
                }
            }
            if (this.PostProcessBeam == null)
                return;
            this.PostProcessBeam(beam);
        }

        public void DoPostProcessBeamTick(
            BeamController beam,
            SpeculativeRigidbody hitRigidbody,
            float tickRate)
        {
            if ((bool) (UnityEngine.Object) beam && (bool) (UnityEngine.Object) beam.projectile && (double) beam.projectile.baseData.damage == 0.0 || this.PostProcessBeamTick == null)
                return;
            this.PostProcessBeamTick(beam, hitRigidbody, tickRate);
        }

        public void DoPostProcessBeamChanceTick(BeamController beam)
        {
            if (this.PostProcessBeamChanceTick == null)
                return;
            this.PostProcessBeamChanceTick(beam);
        }

        public Material[] SetOverrideShader(Shader overrideShader)
        {
            if (this.m_cachedOverrideMaterials == null)
                this.m_cachedOverrideMaterials = new Material[3];
            for (int index = 0; index < this.m_cachedOverrideMaterials.Length; ++index)
                this.m_cachedOverrideMaterials[index] = (Material) null;
            this.sprite.renderer.material.shader = overrideShader;
            this.m_cachedOverrideMaterials[0] = this.sprite.renderer.material;
            if ((bool) (UnityEngine.Object) this.primaryHand && (bool) (UnityEngine.Object) this.primaryHand.sprite)
                this.m_cachedOverrideMaterials[1] = this.primaryHand.SetOverrideShader(overrideShader);
            if ((bool) (UnityEngine.Object) this.secondaryHand && (bool) (UnityEngine.Object) this.secondaryHand.sprite)
                this.m_cachedOverrideMaterials[2] = this.secondaryHand.SetOverrideShader(overrideShader);
            return this.m_cachedOverrideMaterials;
        }

        public static string DefaultShaderName
        {
            get => !GameOptions.SupportsStencil ? "Brave/PlayerShaderNoStencil" : "Brave/PlayerShader";
        }

        public string LocalShaderName
        {
            get
            {
                if (!GameOptions.SupportsStencil)
                    return "Brave/PlayerShaderNoStencil";
                return this.characterIdentity == PlayableCharacters.Eevee || this.IsTemporaryEeveeForUnlock ? "Brave/PlayerShaderEevee" : "Brave/PlayerShader";
            }
        }

        public void ClearOverrideShader()
        {
            if ((bool) (UnityEngine.Object) this && (bool) (UnityEngine.Object) this.sprite && (bool) (UnityEngine.Object) this.sprite.renderer && (bool) (UnityEngine.Object) this.sprite.renderer.material)
                this.sprite.renderer.material.shader = ShaderCache.Acquire(this.LocalShaderName);
            if ((bool) (UnityEngine.Object) this.primaryHand && (bool) (UnityEngine.Object) this.primaryHand.sprite)
                this.primaryHand.ClearOverrideShader();
            if (!(bool) (UnityEngine.Object) this.secondaryHand || !(bool) (UnityEngine.Object) this.secondaryHand.sprite)
                return;
            this.secondaryHand.ClearOverrideShader();
        }

        public void Reinitialize()
        {
            this.specRigidbody.Reinitialize();
            this.WarpFollowersToPlayer();
        }

        public void ReinitializeGuns()
        {
            this.inventory.DestroyAllGuns();
            List<int> intList = this.startingGunIds;
            if (this.UsingAlternateStartingGuns)
                intList = this.startingAlternateGunIds;
            for (int index = 0; index < intList.Count; ++index)
            {
                Gun byId = PickupObjectDatabase.GetById(intList[index]) as Gun;
                if ((bool) (UnityEngine.Object) byId.encounterTrackable)
                {
                    EncounterTrackable.SuppressNextNotification = true;
                    byId.encounterTrackable.HandleEncounter();
                    EncounterTrackable.SuppressNextNotification = false;
                }
                this.inventory.AddGunToInventory(byId, true);
            }
            this.inventory.ChangeGun(1);
        }

        private void InitializeInventory()
        {
            this.inventory = new GunInventory((GameActor) this);
            this.inventory.maxGuns = this.MAX_GUNS_HELD + (int) this.stats.GetStatValue(PlayerStats.StatType.AdditionalGunCapacity);
            this.inventory.maxGuns = int.MaxValue;
            if (this.CharacterUsesRandomGuns)
                this.inventory.maxGuns = 1;
            List<int> intList = this.startingGunIds;
            if (this.UsingAlternateStartingGuns)
                intList = this.startingAlternateGunIds;
            for (int index = 0; index < intList.Count; ++index)
            {
                Gun byId = PickupObjectDatabase.GetById(intList[index]) as Gun;
                if ((bool) (UnityEngine.Object) byId.encounterTrackable)
                {
                    EncounterTrackable.SuppressNextNotification = true;
                    byId.encounterTrackable.HandleEncounter();
                    EncounterTrackable.SuppressNextNotification = false;
                }
                this.inventory.AddGunToInventory(byId, true);
            }
            this.inventory.ChangeGun(1);
            if (this.m_usesRandomStartingEquipment && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                return;
            for (int index = 0; index < this.startingPassiveItemIds.Count; ++index)
                this.AcquirePassiveItemPrefabDirectly(PickupObjectDatabase.GetById(this.startingPassiveItemIds[index]) as PassiveItem);
            for (int index = 0; index < this.startingActiveItemIds.Count; ++index)
            {
                EncounterTrackable.SuppressNextNotification = true;
                (PickupObjectDatabase.GetById(this.startingActiveItemIds[index]) as PlayerItem).Pickup(this);
                EncounterTrackable.SuppressNextNotification = false;
            }
        }

        public DebrisObject ForceDropGun(Gun g)
        {
            if (!g.CanActuallyBeDropped(this))
                return (DebrisObject) null;
            if (this.inventory.GunLocked.Value)
                return (DebrisObject) null;
            bool flag = (UnityEngine.Object) g == (UnityEngine.Object) this.CurrentGun;
            g.HasEverBeenAcquiredByPlayer = true;
            this.inventory.RemoveGunFromInventory(g);
            g.ToggleRenderers(true);
            DebrisObject debrisObject = g.DropGun();
            if (flag)
                this.ProcessHandAttachment();
            return debrisObject;
        }

        public void UpdateInventoryMaxGuns()
        {
            if (this.inventory == null || this.inventory.maxGuns > 1000)
                return;
            this.inventory.maxGuns = this.MAX_GUNS_HELD + (int) this.stats.GetStatValue(PlayerStats.StatType.AdditionalGunCapacity);
            this.inventory.maxGuns = int.MaxValue;
            while (this.inventory.maxGuns < this.inventory.GunCountModified)
            {
                Gun currentGun = this.CurrentGun;
                currentGun.HasEverBeenAcquiredByPlayer = true;
                this.inventory.RemoveGunFromInventory(currentGun);
                currentGun.DropGun();
            }
        }

        public void UpdateInventoryMaxItems()
        {
            if (this.activeItems == null)
                return;
            this.maxActiveItemsHeld = this.MAX_ITEMS_HELD + (int) this.stats.GetStatValue(PlayerStats.StatType.AdditionalItemCapacity);
            while (this.maxActiveItemsHeld < this.activeItems.Count)
                this.DropActiveItem(this.activeItems[this.activeItems.Count - 1]);
        }

        public void ResetTarnisherClipCapacity()
        {
            for (int index = this.ownerlessStatModifiers.Count - 1; index >= 0; --index)
            {
                if (this.ownerlessStatModifiers[index].statToBoost == PlayerStats.StatType.TarnisherClipCapacityMultiplier)
                    this.ownerlessStatModifiers.RemoveAt(index);
            }
            this.stats.RecalculateStats(this);
        }

        public void ChangeAttachedSpriteDepth(tk2dBaseSprite targetSprite, float targetDepth)
        {
            if (!this.m_attachedSprites.Contains(targetSprite))
                return;
            this.m_attachedSpriteDepths[this.m_attachedSprites.IndexOf(targetSprite)] = targetDepth;
        }

        public GameObject RegisterAttachedObject(GameObject prefab, string attachPoint, float depth = 0.0f)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
            if (!string.IsNullOrEmpty(attachPoint))
            {
                tk2dSpriteAttachPoint orAddComponent = this.sprite.gameObject.GetOrAddComponent<tk2dSpriteAttachPoint>();
                gameObject.transform.parent = orAddComponent.GetAttachPointByName(attachPoint);
            }
            else
                gameObject.transform.parent = this.sprite.transform;
            gameObject.transform.localPosition = Vector3.zero;
            if ((UnityEngine.Object) gameObject.transform.parent == (UnityEngine.Object) null)
                UnityEngine.Debug.LogError((object) $"FAILED TO FIND ATTACHPOINT {attachPoint} ON PLAYER");
            tk2dBaseSprite attachment = gameObject.GetComponent<tk2dBaseSprite>();
            if ((UnityEngine.Object) attachment == (UnityEngine.Object) null)
                attachment = gameObject.GetComponentInChildren<tk2dBaseSprite>();
            this.sprite.AttachRenderer(attachment);
            foreach (tk2dBaseSprite componentsInChild in gameObject.GetComponentsInChildren<tk2dBaseSprite>())
            {
                this.m_attachedSprites.Add(componentsInChild);
                this.m_attachedSpriteDepths.Add(depth);
            }
            return gameObject;
        }

        public void DeregisterAttachedObject(GameObject instance, bool completeDestruction = true)
        {
            if (!(bool) (UnityEngine.Object) instance)
                return;
            tk2dBaseSprite[] componentsInChildren = instance.GetComponentsInChildren<tk2dBaseSprite>();
            for (int index = 0; index < componentsInChildren.Length; ++index)
            {
                if ((bool) (UnityEngine.Object) componentsInChildren[index])
                {
                    this.m_attachedSpriteDepths.RemoveAt(this.m_attachedSprites.IndexOf(componentsInChildren[index]));
                    this.m_attachedSprites.Remove(componentsInChildren[index]);
                }
            }
            if (completeDestruction)
                UnityEngine.Object.Destroy((UnityEngine.Object) instance);
            else
                instance.transform.parent = (Transform) null;
        }

        public void ForceStaticFaceDirection(Vector2 dir)
        {
            this.m_lastNonzeroCommandedDirection = dir;
            this.unadjustedAimPoint = (Vector3) (this.CenterPosition + dir.normalized * 5f);
        }

        public void ForceIdleFacePoint(Vector2 dir, bool quadrantize = true)
        {
            float gunAngle = !quadrantize ? BraveMathCollege.Atan2Degrees(dir) : (float) (BraveMathCollege.VectorToQuadrant(dir) * 90);
            string baseAnimationName = this.GetBaseAnimationName(Vector2.zero, gunAngle);
            if (!this.spriteAnimator.IsPlaying(baseAnimationName))
                this.spriteAnimator.Play(baseAnimationName);
            this.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
            this.m_currentGunAngle = gunAngle;
            this.ForceStaticFaceDirection(dir);
            if (!(bool) (UnityEngine.Object) this.CurrentGun)
                return;
            double num = (double) this.CurrentGun.HandleAimRotation((Vector3) (this.CenterPosition + dir));
        }

        public void TeleportToPoint(Vector2 targetPoint, bool useDefaultTeleportVFX)
        {
            if (this.m_isStartingTeleport)
                return;
            this.m_isStartingTeleport = true;
            GameObject gameObject = (GameObject) null;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
                if ((bool) (UnityEngine.Object) otherPlayer)
                    otherPlayer.TeleportToPoint(targetPoint, useDefaultTeleportVFX);
            }
            this.m_isStartingTeleport = false;
            if (useDefaultTeleportVFX)
                gameObject = (GameObject) ResourceCache.Acquire("Global VFX/VFX_Teleport_Beam");
            this.DoVibration(Vibration.Time.Normal, Vibration.Strength.Medium);
            this.StartCoroutine(this.HandleTeleportToPoint(targetPoint, gameObject, (GameObject) null, gameObject));
        }

        [DebuggerHidden]
        private IEnumerator HandleTeleportToPoint(
            Vector2 targetPoint,
            GameObject departureVFXPrefab,
            GameObject arrivalVFX1Prefab,
            GameObject arrivalVFX2Prefab)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleTeleportToPointc__Iterator4()
            {
                departureVFXPrefab = departureVFXPrefab,
                targetPoint = targetPoint,
                arrivalVFX1Prefab = arrivalVFX1Prefab,
                arrivalVFX2Prefab = arrivalVFX2Prefab,
                _this = this
            };
        }

        public bool IsPositionObscuredByTopWall(Vector2 newPosition)
        {
            for (int x1 = 0; x1 < 2; ++x1)
            {
                for (int y1 = 0; y1 < 2; ++y1)
                {
                    int x2 = newPosition.ToIntVector2(VectorConversions.Floor).x + x1;
                    int y2 = newPosition.ToIntVector2(VectorConversions.Floor).y + y1;
                    if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(newPosition.ToIntVector2(VectorConversions.Floor) + new IntVector2(x1, y1)) && (GameManager.Instance.Dungeon.data.isTopWall(x2, y2) || GameManager.Instance.Dungeon.data.isWall(x2, y2)))
                        return true;
                }
            }
            return false;
        }

        public bool IsValidPlayerPosition(Vector2 newPosition)
        {
            for (int x = 0; x < 2; ++x)
            {
                for (int y = 0; y < 2; ++y)
                {
                    if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(newPosition.ToIntVector2(VectorConversions.Floor) + new IntVector2(x, y)))
                        return false;
                }
            }
            int mask = CollisionMask.LayerToMask(CollisionLayer.EnemyCollider, CollisionLayer.EnemyHitBox, CollisionLayer.Projectile);
            Func<SpeculativeRigidbody, bool> func1 = (Func<SpeculativeRigidbody, bool>) (rigidbody => (bool) (UnityEngine.Object) rigidbody.minorBreakable);
            PhysicsEngine instance = PhysicsEngine.Instance;
            SpeculativeRigidbody specRigidbody = this.specRigidbody;
            List<CollisionData> collisionDataList = (List<CollisionData>) null;
            bool flag1 = true;
            bool flag2 = true;
            int? nullable1 = new int?();
            int? nullable2 = new int?(mask);
            Func<SpeculativeRigidbody, bool> func2 = func1;
            SpeculativeRigidbody rigidbody1 = specRigidbody;
            List<CollisionData> overlappingCollisions = collisionDataList;
            int num1 = flag1 ? 1 : 0;
            int num2 = flag2 ? 1 : 0;
            int? overrideCollisionMask = nullable1;
            int? ignoreCollisionMask = nullable2;
            Vector2? overridePosition = new Vector2?(newPosition);
            Func<SpeculativeRigidbody, bool> rigidbodyExcluder = func2;
            SpeculativeRigidbody[] speculativeRigidbodyArray = new SpeculativeRigidbody[0];
            return !instance.OverlapCast(rigidbody1, overlappingCollisions, num1 != 0, num2 != 0, overrideCollisionMask, ignoreCollisionMask, false, overridePosition, rigidbodyExcluder, speculativeRigidbodyArray);
        }

        public void WarpFollowersToPlayer(bool excludeCompanions = false)
        {
            for (int index = 0; index < this.orbitals.Count; ++index)
            {
                this.orbitals[index].GetTransform().position = this.transform.position;
                this.orbitals[index].Reinitialize();
            }
            for (int index = 0; index < this.trailOrbitals.Count; ++index)
            {
                this.trailOrbitals[index].transform.position = this.transform.position;
                this.trailOrbitals[index].specRigidbody.Reinitialize();
            }
            if (excludeCompanions)
                return;
            this.WarpCompanionsToPlayer();
        }

        public void WarpCompanionsToPlayer(bool isRoomSealWarp = false)
        {
            Vector3 vector3 = this.transform.position;
            if (this.InExitCell && this.CurrentRoom != null)
                vector3 = this.CurrentRoom.GetBestRewardLocation(new IntVector2(2, 2), RoomHandler.RewardLocationStyle.PlayerCenter, false).ToVector3() + new Vector3(1f, 1f, 0.0f);
            for (int index = 0; index < this.companions.Count; ++index)
            {
                Vector3 targetPosition = vector3;
                if (isRoomSealWarp && this.companions[index].CompanionSettings.WarpsToRandomPoint)
                {
                    RoomHandler currentRoom = this.CurrentRoom;
                    IntVector2? footprint = new IntVector2?(this.companions[index].Clearance * 3);
                    CellTypes? passableCellTypes = new CellTypes?(CellTypes.FLOOR);
                    // ISSUE: reference to a compiler-generated field
                    if (PlayerController._f__mg_cache0 == null)
                    {
                        // ISSUE: reference to a compiler-generated field
                        PlayerController._f__mg_cache0 = new CellValidator(Pathfinder.CellValidator_NoTopWalls);
                    }
                    // ISSUE: reference to a compiler-generated field
                    CellValidator fMgCache0 = PlayerController._f__mg_cache0;
                    IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(footprint, passableCellTypes, cellValidator: fMgCache0);
                    if (randomAvailableCell.HasValue)
                        targetPosition = (randomAvailableCell.Value + IntVector2.One).ToVector3();
                }
                this.companions[index].CompanionWarp(targetPosition);
            }
        }

        public void WarpToPointAndBringCoopPartner(
            Vector2 targetPoint,
            bool useDefaultPoof = false,
            bool doFollowers = false)
        {
            this.WarpToPoint(targetPoint, useDefaultPoof, doFollowers);
            PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
            if (!(bool) (UnityEngine.Object) otherPlayer)
                return;
            Vector2 vector2_1 = this.specRigidbody.UnitBottomLeft - this.transform.position.XY();
            Vector2 vector2_2 = otherPlayer.specRigidbody.UnitBottomLeft - otherPlayer.transform.position.XY();
            otherPlayer.WarpToPoint(targetPoint + (vector2_1 - vector2_2), useDefaultPoof, doFollowers);
        }

        public void WarpToPoint(Vector2 targetPoint, bool useDefaultPoof = false, bool doFollowers = false)
        {
            if (useDefaultPoof)
                LootEngine.DoDefaultItemPoof(this.CenterPosition, true);
            this.transform.position = (Vector3) targetPoint;
            this.specRigidbody.Reinitialize();
            this.specRigidbody.RecheckTriggers = true;
            if ((bool) (UnityEngine.Object) this.CurrentItem && this.CurrentItem is GrapplingHookItem)
            {
                GrapplingHookItem currentItem = this.CurrentItem as GrapplingHookItem;
                if ((bool) (UnityEngine.Object) currentItem && currentItem.IsActive)
                {
                    float destroyTime = -1f;
                    currentItem.Use(this, out destroyTime);
                }
            }
            if (!doFollowers)
                return;
            this.WarpFollowersToPlayer();
        }

        public void AttemptTeleportToRoom(RoomHandler targetRoom, bool force = false, bool noFX = false)
        {
            if (this.IsInMinecart)
                return;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
                if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.IsInMinecart)
                    return;
            }
            bool flag = this.CurrentRoom != null && this.CurrentRoom.CanTeleportFromRoom() && targetRoom != null && targetRoom.CanTeleportToRoom();
            if (GameManager.Instance.InTutorial && !flag && this.CurrentRoom == targetRoom && targetRoom.GetRoomName().Equals("Tutorial_Room_0065_teleporter", StringComparison.OrdinalIgnoreCase))
                flag = true;
            if (force)
                flag = true;
            if (!flag)
                return;
            if (this.OnDidUnstealthyAction != null)
                this.OnDidUnstealthyAction(this);
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_teleport_depart_01", this.gameObject);
            this.m_cachedTeleportSpot = !force ? this.specRigidbody.Position.UnitPosition : this.CurrentRoom.GetCenteredVisibleClearSpot(2, 2).ToVector2();
            targetRoom.SetRoomActive(true);
            TeleporterController componentInChildren = !(bool) (UnityEngine.Object) targetRoom.hierarchyParent ? (TeleporterController) null : targetRoom.hierarchyParent.GetComponentInChildren<TeleporterController>(true);
            if (!(bool) (UnityEngine.Object) componentInChildren)
            {
                List<TeleporterController> componentsInRoom = targetRoom.GetComponentsInRoom<TeleporterController>();
                if (componentsInRoom.Count > 0)
                    componentInChildren = componentsInRoom[0];
            }
            Vector2 vector2;
            if ((bool) (UnityEngine.Object) componentInChildren)
            {
                vector2 = componentInChildren.sprite.WorldCenter;
            }
            else
            {
                IntVector2? randomAvailableCell = targetRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), new CellTypes?(CellTypes.FLOOR));
                vector2 = !randomAvailableCell.HasValue ? targetRoom.GetCenterCell().ToVector2() : randomAvailableCell.Value.ToVector2();
            }
            Vector2 targetSpot = vector2 - this.SpriteDimensions.XY().WithY(0.0f) / 2f;
            this.StartCoroutine(this.HandleTeleport(componentInChildren, targetSpot, false, noFX));
        }

        public void AttemptReturnTeleport(TeleporterController teleporter)
        {
            if (this.CurrentRoom == null || !this.CurrentRoom.CanTeleportFromRoom() || !this.CanReturnTeleport || !((UnityEngine.Object) teleporter == (UnityEngine.Object) this.m_returnTeleporter))
                return;
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_teleport_depart_01", this.gameObject);
            this.StartCoroutine(this.HandleTeleport(teleporter, this.m_cachedTeleportSpot, true));
        }

        [DebuggerHidden]
        private IEnumerator HandleTeleport(
            TeleporterController teleporter,
            Vector2 targetSpot,
            bool isReturnTeleport,
            bool noFX = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleTeleportc__Iterator5()
            {
                noFX = noFX,
                teleporter = teleporter,
                targetSpot = targetSpot,
                isReturnTeleport = isReturnTeleport,
                _this = this
            };
        }

        protected virtual void CheckSpawnAlertArrows()
        {
            if (!this.IsPrimaryPlayer)
                return;
            if (GameManager.IsReturningToBreach || GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating)
            {
                this.m_elapsedNonalertTime = 0.0f;
                this.m_isThreatArrowing = false;
                this.m_threadArrowTarget = (AIActor) null;
            }
            else if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE || this.CurrentRoom == null)
            {
                this.m_elapsedNonalertTime = 0.0f;
                this.m_isThreatArrowing = false;
                this.m_threadArrowTarget = (AIActor) null;
            }
            else
            {
                if (this.CurrentRoom == null || !this.IsInCombat)
                    return;
                List<AIActor> activeEnemies = this.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies == null || activeEnemies.Count == 0)
                {
                    this.m_elapsedNonalertTime = 0.0f;
                    this.m_isThreatArrowing = false;
                    this.m_threadArrowTarget = (AIActor) null;
                }
                else
                {
                    AIActor aiActor1 = (AIActor) null;
                    bool flag = false;
                    for (int index = 0; index < activeEnemies.Count; ++index)
                    {
                        AIActor aiActor2 = activeEnemies[index];
                        if ((bool) (UnityEngine.Object) aiActor2 && (!aiActor2.IgnoreForRoomClear || aiActor2.AlwaysShowOffscreenArrow) && (!(bool) (UnityEngine.Object) aiActor2.healthHaver || !aiActor2.healthHaver.IsBoss))
                        {
                            if (GameManager.Instance.MainCameraController.PointIsVisible(aiActor2.CenterPosition))
                                flag = true;
                            else if (!(bool) (UnityEngine.Object) aiActor1 || !aiActor1.AlwaysShowOffscreenArrow && aiActor2.AlwaysShowOffscreenArrow)
                                aiActor1 = aiActor2;
                        }
                    }
                    if ((bool) (UnityEngine.Object) aiActor1 && (!flag || aiActor1.AlwaysShowOffscreenArrow))
                    {
                        this.m_elapsedNonalertTime += BraveTime.DeltaTime;
                        this.m_threadArrowTarget = aiActor1;
                        if ((double) this.m_elapsedNonalertTime <= 3.0 && !aiActor1.AlwaysShowOffscreenArrow || this.m_isThreatArrowing)
                            return;
                        this.StartCoroutine(this.HandleThreatArrow());
                    }
                    else
                    {
                        this.m_elapsedNonalertTime = 0.0f;
                        this.m_isThreatArrowing = false;
                        this.m_threadArrowTarget = (AIActor) null;
                    }
                }
            }
        }

        protected virtual void CheckSpawnEmergencyCrate()
        {
            if (this.CurrentRoom == null || (UnityEngine.Object) this.CurrentRoom.ExtantEmergencyCrate != (UnityEngine.Object) null || GameManager.Instance.Dungeon.SuppressEmergencyCrates || !this.CurrentRoom.IsSealed && !this.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) || !this.CurrentRoom.area.IsProceduralRoom && this.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET || this.CharacterUsesRandomGuns)
                return;
            bool flag = false;
            for (int index = 0; index < this.inventory.AllGuns.Count; ++index)
            {
                if (this.inventory.AllGuns[index].CurrentAmmo > 0 || this.inventory.AllGuns[index].InfiniteAmmo)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            this.SpawnEmergencyCrate();
        }

        public IntVector2 SpawnEmergencyCrate(GenericLootTable overrideTable = null)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("EmergencyCrate"));
            EmergencyCrateController component = gameObject.GetComponent<EmergencyCrateController>();
            if ((UnityEngine.Object) overrideTable != (UnityEngine.Object) null)
                component.gunTable = overrideTable;
            IntVector2 bestRewardLocation = this.CurrentRoom.GetBestRewardLocation(new IntVector2(1, 1));
            component.Trigger(new Vector3(-5f, -5f, -5f), bestRewardLocation.ToVector3() + new Vector3(15f, 15f, 15f), this.CurrentRoom, (UnityEngine.Object) overrideTable == (UnityEngine.Object) null);
            this.CurrentRoom.ExtantEmergencyCrate = gameObject;
            return bestRewardLocation;
        }

        public void ReinitializeMovementRestrictors()
        {
            this.specRigidbody.MovementRestrictor = (SpeculativeRigidbody.MovementRestrictorDelegate) null;
            this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.RollPitMovementRestrictor);
            if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
                return;
            this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.CameraBoundsMovementRestrictor);
        }

        private void InitializeCallbacks()
        {
            this.healthHaver.persistsOnDeath = true;
            this.healthHaver.OnDeath += new Action<Vector2>(this.Die);
            this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.Damaged);
            this.healthHaver.OnHealthChanged += new HealthHaver.OnHealthChangedEvent(this.HealthChanged);
            this.spriteAnimator.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
            this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
            this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.RollPitMovementRestrictor);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.CameraBoundsMovementRestrictor);
            this.inventory.OnGunChanged += new GunInventory.OnGunChangedEvent(this.OnGunChanged);
        }

        private void CameraBoundsMovementRestrictor(
            SpeculativeRigidbody specRigidbody,
            IntVector2 prevPixelOffset,
            IntVector2 pixelOffset,
            ref bool validLocation)
        {
            if (!validLocation)
                return;
            IntVector2 pixel1 = PhysicsEngine.UnitToPixel(GameManager.Instance.MainCameraController.MinVisiblePoint);
            IntVector2 pixel2 = PhysicsEngine.UnitToPixel(GameManager.Instance.MainCameraController.MaxVisiblePoint);
            if (specRigidbody.PixelColliders[0].LowerLeft.x < pixel1.x && pixelOffset.x < prevPixelOffset.x)
                validLocation = false;
            else if (specRigidbody.PixelColliders[0].UpperRight.x > pixel2.x && pixelOffset.x > prevPixelOffset.x)
                validLocation = false;
            else if (specRigidbody.PixelColliders[0].LowerLeft.y < pixel1.y && pixelOffset.y < prevPixelOffset.y)
                validLocation = false;
            else if (specRigidbody.PixelColliders[1].UpperRight.y > pixel2.y && pixelOffset.y > prevPixelOffset.y)
                validLocation = false;
            if (validLocation || !StaticReferenceManager.ActiveMineCarts.ContainsKey(this))
                return;
            StaticReferenceManager.ActiveMineCarts[this].EvacuateSpecificPlayer(this);
        }

        public void ReuniteWithOtherPlayer(PlayerController other, bool useDefaultVFX = false)
        {
            this.WarpToPoint((Vector2) other.transform.position, useDefaultVFX);
        }

        public void HandleItemStolen(ShopItemController item)
        {
            if (this.OnItemStolen == null)
                return;
            this.OnItemStolen(this, item);
        }

        public void HandleItemPurchased(ShopItemController item)
        {
            if (this.OnItemPurchased == null)
                return;
            this.OnItemPurchased(this, item);
        }

        public void OnRoomCleared()
        {
            for (int index = 0; index < this.activeItems.Count; ++index)
                this.activeItems[index].ClearedRoom();
            ++this.NumRoomsCleared;
            if (this.CharacterUsesRandomGuns && (double) this.m_gunGameElapsed > 20.0)
                this.ChangeToRandomGun();
            if (this.OnRoomClearEvent == null)
                return;
            this.OnRoomClearEvent(this);
        }

        public void ChangeToRandomGun()
        {
            if (this.IsGhost)
                return;
            this.m_gunGameElapsed = 0.0f;
            this.m_gunGameDamageThreshold = 200f;
            if (this.inventory.GunLocked.Value || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL)
                return;
            Gun currentGun = this.CurrentGun;
            this.inventory.AddGunToInventory(PickupObjectDatabase.GetRandomGun());
            this.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_MagicFavor_Change") as GameObject, new Vector3(0.0f, -1f, 0.0f));
            if (!(bool) (UnityEngine.Object) currentGun)
                return;
            if (currentGun.IsFiring)
                currentGun.CeaseAttack();
            this.inventory.RemoveGunFromInventory(currentGun);
            UnityEngine.Object.Destroy((UnityEngine.Object) currentGun.gameObject);
        }

        public void OnAnyEnemyTookAnyDamage(float damageDone, bool fatal, HealthHaver target)
        {
            if (this.OnAnyEnemyReceivedDamage != null)
                this.OnAnyEnemyReceivedDamage(damageDone, fatal, target);
            AIActor aiActor = !(bool) (UnityEngine.Object) target ? (AIActor) null : target.aiActor;
            if ((bool) (UnityEngine.Object) aiActor && !aiActor.IsNormalEnemy || this.IsGhost || target.PreventCooldownGainFromDamage)
                return;
            for (int index = 0; index < this.activeItems.Count; ++index)
                this.activeItems[index].DidDamage(this, damageDone);
            if (this.inventory == null || this.inventory.AllGuns == null)
                return;
            for (int index = 0; index < this.inventory.AllGuns.Count; ++index)
            {
                if (this.inventory.AllGuns[index].UsesRechargeLikeActiveItem)
                    this.inventory.AllGuns[index].ApplyActiveCooldownDamage(this, damageDone);
            }
        }

        public void OnDidDamage(float damageDone, bool fatal, HealthHaver target)
        {
            if (this.OnDealtDamage != null)
                this.OnDealtDamage(this, damageDone);
            if (this.OnDealtDamageContext != null)
                this.OnDealtDamageContext(this, damageDone, fatal, target);
            if (fatal)
            {
                ++this.m_enemiesKilled;
                this.m_gunGameDamageThreshold = 200f;
                if (this.CharacterUsesRandomGuns && this.m_enemiesKilled % 5 == 0)
                    this.ChangeToRandomGun();
            }
            if (this.CharacterUsesRandomGuns)
            {
                this.m_gunGameDamageThreshold -= Mathf.Max(damageDone, 3f);
                if ((double) this.m_gunGameDamageThreshold < 0.0)
                    this.ChangeToRandomGun();
            }
            if (fatal && this.OnKilledEnemy != null)
                this.OnKilledEnemy(this);
            if (!fatal || this.OnKilledEnemyContext == null)
                return;
            this.OnKilledEnemyContext(this, target);
        }

        protected void HandleAnimationEvent(
            tk2dSpriteAnimator animator,
            tk2dSpriteAnimationClip clip,
            int frameNo)
        {
            tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
            for (int index = 0; index < this.animationAudioEvents.Count; ++index)
            {
                if (this.animationAudioEvents[index].eventTag == frame.eventInfo)
                {
                    int num = (int) AkSoundEngine.PostEvent(this.animationAudioEvents[index].eventName, this.gameObject);
                }
            }
        }

        public void HandleDodgedBeam(BeamController beam)
        {
            if (this.OnDodgedBeam == null)
                return;
            this.OnDodgedBeam(beam, this);
        }

        protected virtual void OnPreRigidbodyCollision(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myCollider,
            SpeculativeRigidbody otherRigidbody,
            PixelCollider otherCollider)
        {
            if (this.DodgeRollIsBlink && this.IsDodgeRolling && (bool) (UnityEngine.Object) otherRigidbody && ((bool) (UnityEngine.Object) otherRigidbody.projectile || !(bool) (UnityEngine.Object) otherRigidbody.GetComponent<DungeonDoorController>()))
                PhysicsEngine.SkipCollision = true;
            else if (this.IsGhost && (bool) (UnityEngine.Object) otherRigidbody && (bool) (UnityEngine.Object) otherRigidbody.aiActor)
            {
                PhysicsEngine.SkipCollision = true;
            }
            else
            {
                if (!this.IsDodgeRolling || !(bool) (UnityEngine.Object) otherRigidbody)
                    return;
                if ((bool) (UnityEngine.Object) otherRigidbody.projectile && this.OnDodgedProjectile != null)
                    this.OnDodgedProjectile(otherRigidbody.projectile);
                if (!(bool) (UnityEngine.Object) otherRigidbody.aiActor)
                    return;
                if (this.DodgeRollIsBlink)
                {
                    PhysicsEngine.SkipCollision = true;
                }
                else
                {
                    FreezeOnDeath component = otherRigidbody.GetComponent<FreezeOnDeath>();
                    if ((bool) (UnityEngine.Object) component && component.IsDeathFrozen)
                        return;
                    AIActor aiActor = otherRigidbody.aiActor;
                    if ((bool) (UnityEngine.Object) aiActor.healthHaver)
                    {
                        float num = this.stats.rollDamage * this.stats.GetStatValue(PlayerStats.StatType.DodgeRollDamage);
                        if (aiActor.healthHaver.IsDead)
                            PhysicsEngine.SkipCollision = true;
                        else if (!this.m_rollDamagedEnemies.Contains(aiActor) && (double) aiActor.healthHaver.GetCurrentHealth() < (double) num && aiActor.healthHaver.CanCurrentlyBeKilled)
                        {
                            this.ApplyRollDamage(aiActor);
                            PhysicsEngine.SkipCollision = true;
                        }
                    }
                    if (!aiActor.IsFrozen)
                        return;
                    float damage = !(aiActor.GetEffect("freeze") is GameActorFreezeEffect effect) ? 0.0f : aiActor.healthHaver.GetMaxHealth() * effect.UnfreezeDamagePercent;
                    if (effect != null && (double) damage >= (double) aiActor.healthHaver.GetCurrentHealth() && aiActor.healthHaver.CanCurrentlyBeKilled)
                    {
                        aiActor.healthHaver.ApplyDamage(damage, this.lockedDodgeRollDirection, "DODGEROLL OF AWESOME", damageCategory: DamageCategory.Collision, ignoreInvulnerabilityFrames: true);
                        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.KILL_FROZEN_ENEMY_WITH_ROLL);
                        PhysicsEngine.SkipCollision = true;
                    }
                    else
                    {
                        if (!(bool) (UnityEngine.Object) aiActor.knockbackDoer)
                            return;
                        aiActor.knockbackDoer.ApplyKnockback(this.lockedDodgeRollDirection, 5f);
                    }
                }
            }
        }

        public void ApplyRollDamage(AIActor actor)
        {
            if (this.m_rollDamagedEnemies.Contains(actor))
                return;
            bool flag = false;
            if (actor.HasOverrideDodgeRollDeath && string.IsNullOrEmpty(actor.healthHaver.overrideDeathAnimation))
            {
                flag = true;
                actor.healthHaver.overrideDeathAnimation = actor.OverrideDodgeRollDeath;
            }
            if ((bool) (UnityEngine.Object) actor.specRigidbody && PassiveItem.ActiveFlagItems.ContainsKey(this) && (PassiveItem.ActiveFlagItems[this].ContainsKey(typeof (SpikedArmorItem)) || PassiveItem.ActiveFlagItems[this].ContainsKey(typeof (HelmetItem))))
            {
                PixelCollider hitboxPixelCollider = actor.specRigidbody.HitboxPixelCollider;
                if (hitboxPixelCollider != null)
                {
                    Vector2 position = BraveMathCollege.ClosestPointOnRectangle(this.specRigidbody.GetUnitCenter(ColliderType.HitBox), hitboxPixelCollider.UnitBottomLeft, hitboxPixelCollider.UnitDimensions);
                    SpawnManager.SpawnVFX((GameObject) BraveResources.Load("Global VFX/VFX_DodgeRollHit"), (Vector3) position, Quaternion.identity, true);
                }
            }
            actor.healthHaver.ApplyDamage(this.stats.rollDamage * this.stats.GetStatValue(PlayerStats.StatType.DodgeRollDamage), this.lockedDodgeRollDirection, "DODGEROLL");
            this.m_rollDamagedEnemies.Add(actor);
            if (this.OnRolledIntoEnemy != null)
                this.OnRolledIntoEnemy(this, actor);
            if (!flag)
                return;
            actor.healthHaver.overrideDeathAnimation = string.Empty;
        }

        private void HealthChanged(float result, float max)
        {
            if ((UnityEngine.Object) GameUIRoot.Instance == (UnityEngine.Object) null)
                return;
            UnityEngine.Debug.Log((object) $"changing health to: {(object) result}|{(object) max}");
            GameUIRoot.Instance.UpdatePlayerHealthUI(this, this.healthHaver);
        }

        public void HandleCloneItem(ExtraLifeItem source)
        {
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                if (GameManager.Instance.GetOtherPlayer(this).IsGhost)
                    this.DoCloneEffect();
                else
                    this.m_cloneWaitingForCoopDeath = true;
            }
            else
                this.DoCloneEffect();
        }

        private void DoCloneEffect() => this.StartCoroutine(this.HandleCloneEffect());

        [DebuggerHidden]
        private IEnumerator HandleCloneEffect()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleCloneEffectc__Iterator6()
            {
                _this = this
            };
        }

        public void EscapeRoom(
            PlayerController.EscapeSealedRoomStyle escapeStyle,
            bool resetCurrentRoom,
            RoomHandler targetRoom = null)
        {
            this.RespawnInPreviousRoom(false, escapeStyle, resetCurrentRoom, targetRoom);
            targetRoom?.EnsureUpstreamLocksUnlocked();
            this.specRigidbody.Velocity = Vector2.zero;
            this.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
        }

        public void RespawnInPreviousRoom(
            bool doFullHeal,
            PlayerController.EscapeSealedRoomStyle escapeStyle,
            bool resetCurrentRoom,
            RoomHandler targetRoom = null)
        {
            RoomHandler currentRoom = this.CurrentRoom;
            if (targetRoom == null)
                targetRoom = this.GetPreviousRoom(this.CurrentRoom);
            this.m_lastInteractionTarget = (IPlayerInteractable) null;
            if (escapeStyle == PlayerController.EscapeSealedRoomStyle.TELEPORTER)
            {
                IntVector2? randomAvailableCell = targetRoom.GetRandomAvailableCell(new IntVector2?(new IntVector2(2, 2)), new CellTypes?(CellTypes.FLOOR));
                if (randomAvailableCell.HasValue)
                    this.TeleportToPoint(randomAvailableCell.Value.ToCenterVector2(), true);
                if (!resetCurrentRoom || this.CurrentRoom == targetRoom)
                    return;
                this.StartCoroutine(this.DelayedRoomReset(currentRoom));
            }
            else
            {
                if (!resetCurrentRoom)
                    return;
                this.StartCoroutine(this.HandleResetAndRespawn_CR(targetRoom, currentRoom, doFullHeal, escapeStyle));
            }
        }

        private RoomHandler GetPreviousRoom(RoomHandler currentRoom)
        {
            RoomHandler previousRoom = (RoomHandler) null;
            for (int index = 0; index < currentRoom.connectedRooms.Count; ++index)
            {
                if (currentRoom.connectedRooms[index].visibility != RoomHandler.VisibilityStatus.OBSCURED && currentRoom.distanceFromEntrance > currentRoom.connectedRooms[index].distanceFromEntrance)
                {
                    previousRoom = currentRoom.connectedRooms[index];
                    break;
                }
            }
            if (previousRoom == null)
            {
                for (int index = 0; index < currentRoom.connectedRooms.Count; ++index)
                {
                    if (currentRoom.connectedRooms[index].visibility != RoomHandler.VisibilityStatus.OBSCURED)
                    {
                        previousRoom = currentRoom.connectedRooms[index];
                        break;
                    }
                }
            }
            if (previousRoom != null && previousRoom.area.IsProceduralRoom && previousRoom.area.proceduralCells != null)
            {
                for (int index = 0; index < previousRoom.connectedRooms.Count; ++index)
                {
                    if (previousRoom.connectedRooms[index].visibility != RoomHandler.VisibilityStatus.OBSCURED && previousRoom.distanceFromEntrance > previousRoom.connectedRooms[index].distanceFromEntrance && previousRoom.connectedRooms[index] != currentRoom)
                    {
                        previousRoom = previousRoom.connectedRooms[index];
                        break;
                    }
                }
            }
            if (previousRoom == null)
            {
                UnityEngine.Debug.Log((object) "Could not find a previous room that has been visited!");
                previousRoom = GameManager.Instance.Dungeon.data.Entrance;
            }
            return previousRoom;
        }

        [DebuggerHidden]
        private IEnumerator DelayedRoomReset(RoomHandler targetRoom)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__DelayedRoomResetc__Iterator7()
            {
                targetRoom = targetRoom,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleResetAndRespawn_CR(
            RoomHandler roomToSpawnIn,
            RoomHandler roomToReset,
            bool doFullHeal,
            PlayerController.EscapeSealedRoomStyle escapeStyle)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleResetAndRespawn_CRc__Iterator8()
            {
                doFullHeal = doFullHeal,
                escapeStyle = escapeStyle,
                roomToSpawnIn = roomToSpawnIn,
                roomToReset = roomToReset,
                _this = this
            };
        }

        public void OnLostArmor()
        {
            this.ForceBlank();
            if ((UnityEngine.Object) this.lostAllArmorVFX != (UnityEngine.Object) null && (double) this.healthHaver.Armor == 0.0)
                SpawnManager.SpawnDebris(this.lostAllArmorVFX, (Vector3) this.specRigidbody.UnitTopCenter, Quaternion.identity).GetComponent<DebrisObject>().Trigger(Vector3.zero, 0.5f);
            if (this.LostArmor == null)
                return;
            this.LostArmor();
        }

        private void Damaged(
            float resultValue,
            float maxValue,
            CoreDamageTypes damageTypes,
            DamageCategory damageCategory,
            Vector2 damageDirection)
        {
            PlatformInterface.SetAlienFXColor((Color32) this.m_alienDamageColor, 1f);
            this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
            this.HasTakenDamageThisRun = true;
            this.HasTakenDamageThisFloor = true;
            if (this.CurrentRoom != null)
                this.CurrentRoom.PlayerHasTakenDamageInThisRoom = true;
            if ((bool) (UnityEngine.Object) this.CurrentGun && this.CurrentGun.IsCharging)
                this.CurrentGun.CeaseAttack(false);
            Pixelator.Instance.HandleDamagedVignette(damageDirection);
            Exploder.DoRadialKnockback((Vector3) this.CenterPosition, 50f, 3f);
            bool flag1 = (double) this.healthHaver.Armor > 0.0;
            if ((double) resultValue <= 0.0 && !flag1 && !this.m_revenging && PassiveItem.IsFlagSetForCharacter(this, typeof (PoweredByRevengeItem)))
            {
                this.StartCoroutine(this.HandleFueledByRevenge());
                this.healthHaver.ApplyHealing(0.5f - resultValue);
                resultValue = 0.5f;
            }
            if ((double) resultValue <= 0.0 && !flag1 && this.CurrentItem is RationItem)
            {
                RationItem currentItem = this.CurrentItem as RationItem;
                this.UseItem();
                resultValue += currentItem.healingAmount;
            }
            bool flag2 = (double) resultValue <= 0.0 && !flag1;
            if (damageCategory != DamageCategory.DamageOverTime && flag2)
                GameManager.Instance.MainCameraController.DoScreenShake(new ScreenShakeSettings(0.25f, 7f, 0.1f, 0.3f), new Vector2?(this.specRigidbody.UnitCenter));
            bool flag3 = false;
            if ((GameManager.Instance.InTutorial || flag3) && (double) resultValue <= 0.0 && !flag1)
            {
                this.RespawnInPreviousRoom(true, PlayerController.EscapeSealedRoomStyle.DEATH_SEQUENCE, true);
                this.specRigidbody.Velocity = Vector2.zero;
                this.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
                foreach (Gun allGun in this.inventory.AllGuns)
                    allGun.ammo = allGun.AdjustedMaxAmmo;
            }
            else if ((double) resultValue <= 0.0 && !flag1)
            {
                if ((bool) (UnityEngine.Object) this.CurrentGun)
                    this.CurrentGun.CeaseAttack(false);
                this.CurrentInputState = PlayerInputState.NoInput;
                this.m_handlingQueuedAnimation = true;
                this.HandleDarkSoulsHollowTransition(false);
            }
            else
            {
                if (this.OnReceivedDamage != null)
                    this.OnReceivedDamage(this);
                if (this.ownerlessStatModifiers == null)
                    return;
                bool flag4 = false;
                for (int index = 0; index < this.ownerlessStatModifiers.Count; ++index)
                {
                    if (this.ownerlessStatModifiers[index].isMeatBunBuff)
                    {
                        flag4 = true;
                        this.ownerlessStatModifiers.RemoveAt(index);
                        --index;
                    }
                }
                if (!flag4 || !((UnityEngine.Object) this.stats != (UnityEngine.Object) null))
                    return;
                UnityEngine.Debug.LogError((object) "Did remove meatbun buff!");
                this.stats.RecalculateStats(this);
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleFueledByRevenge()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleFueledByRevengec__Iterator9()
            {
                _this = this
            };
        }

        private void RevengeRevive(PlayerController obj) => this.healthHaver.FullHeal();

        public void HandleDarkSoulsHollowTransition(bool isHollow = true)
        {
            if (isHollow)
            {
                this.IsDarkSoulsHollow = true;
                if ((UnityEngine.Object) this.m_hollowAfterImage == (UnityEngine.Object) null)
                {
                    this.m_hollowAfterImage = this.sprite.gameObject.AddComponent<AfterImageTrailController>();
                    this.m_hollowAfterImage.spawnShadows = true;
                    this.m_hollowAfterImage.shadowTimeDelay = 0.05f;
                    this.m_hollowAfterImage.shadowLifetime = 0.3f;
                    this.m_hollowAfterImage.minTranslation = 0.05f;
                    this.m_hollowAfterImage.maxEmission = 0.0f;
                    this.m_hollowAfterImage.minEmission = 0.0f;
                    this.m_hollowAfterImage.dashColor = new Color(0.0f, 113f / 256f, 143f / 256f);
                    this.m_hollowAfterImage.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");
                }
                else
                    this.m_hollowAfterImage.spawnShadows = true;
                this.ChangeSpecialShaderFlag(2, 1f);
            }
            else
            {
                this.IsDarkSoulsHollow = false;
                if ((UnityEngine.Object) this.m_hollowAfterImage != (UnityEngine.Object) null)
                    this.m_hollowAfterImage.spawnShadows = false;
                this.ChangeSpecialShaderFlag(2, 0.0f);
            }
        }

        public void TriggerDarkSoulsReset(bool dropItems = true, int cursedHealthMaximum = 1)
        {
            this.IsOnFire = false;
            this.CurrentFireMeterValue = 0.0f;
            this.CurrentPoisonMeterValue = 0.0f;
            this.CurrentCurseMeterValue = 0.0f;
            this.CurrentDrainMeterValue = 0.0f;
            int num = (int) AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !GameManager.Instance.GetOtherPlayer(this).IsGhost)
            {
                this.DropPileOfSouls();
                this.HandleDarkSoulsHollowTransition();
                this.StartCoroutine(this.HandleCoopDeath(this.m_isFalling));
            }
            else
            {
                this.m_interruptingPitRespawn = true;
                this.healthHaver.FullHeal();
                if (this.characterIdentity == PlayableCharacters.Robot)
                    this.healthHaver.Armor = 2f;
                this.specRigidbody.Velocity = Vector2.zero;
                this.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
                if ((UnityEngine.Object) this.m_returnTeleporter != (UnityEngine.Object) null)
                {
                    this.m_returnTeleporter.ClearReturnActive();
                    this.m_returnTeleporter = (TeleporterController) null;
                }
                GameManager.Instance.Dungeon.DarkSoulsReset(this, dropItems, cursedHealthMaximum);
            }
        }

        private void ContinueDarkSoulResetCoop()
        {
            this.StartCoroutine(this.CoopResurrectInternal(this.transform.position, (tk2dSpriteAnimationClip) null, true));
            this.healthHaver.FullHeal();
            this.specRigidbody.Velocity = Vector2.zero;
            this.knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
            if ((UnityEngine.Object) this.m_returnTeleporter != (UnityEngine.Object) null)
            {
                this.m_returnTeleporter.ClearReturnActive();
                this.m_returnTeleporter = (TeleporterController) null;
            }
            GameManager.Instance.Dungeon.DarkSoulsReset(this, false, 1);
        }

        protected virtual void Die(Vector2 finalDamageDirection)
        {
            ++this.DeathsThisRun;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
                GameManager.Instance.platformInterface.AchievementUnlock(Achievement.DIE_IN_PAST);
            GameUIRoot.Instance.GetReloadBarForPlayer(this).UpdateStatusBars(this);
            bool flag = true;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
                if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.healthHaver.IsAlive)
                    flag = false;
            }
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER || flag)
            {
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
                    if (otherPlayer.m_cloneWaitingForCoopDeath)
                    {
                        otherPlayer.DoCloneEffect();
                        return;
                    }
                    if (otherPlayer.IsDarkSoulsHollow && otherPlayer.IsGhost)
                    {
                        otherPlayer.ContinueDarkSoulResetCoop();
                        this.StartCoroutine(this.HandleCoopDeath(this.m_isFalling));
                        return;
                    }
                }
                GameManager.Instance.PauseRaw(true);
                BraveTime.RegisterTimeScaleMultiplier(0.0f, GameManager.Instance.gameObject);
                int num1 = (int) AkSoundEngine.PostEvent("Stop_SND_All", this.gameObject);
                this.StartCoroutine(this.HandleDeath_CR());
                int num2 = (int) AkSoundEngine.PostEvent("Play_UI_gameover_start_01", this.gameObject);
            }
            else
                this.StartCoroutine(this.HandleCoopDeath(this.m_isFalling));
        }

        private void HandleCoopDeathItemDropping()
        {
            List<Gun> gunList = new List<Gun>();
            List<PickupObject> pickupObjectList = new List<PickupObject>();
            if ((bool) (UnityEngine.Object) this.CurrentGun)
            {
                MimicGunController component = this.CurrentGun.GetComponent<MimicGunController>();
                if ((bool) (UnityEngine.Object) component)
                    component.ForceClearMimic(true);
            }
            for (int index1 = 0; index1 < this.inventory.AllGuns.Count; ++index1)
            {
                if (this.inventory.AllGuns[index1].CanActuallyBeDropped(this) && !this.inventory.AllGuns[index1].PersistsOnDeath)
                {
                    bool flag = false;
                    for (int index2 = 0; index2 < this.startingGunIds.Count; ++index2)
                    {
                        if (this.inventory.AllGuns[index1].PickupObjectId == this.startingGunIds[index2])
                            flag = true;
                    }
                    for (int index3 = 0; index3 < this.startingAlternateGunIds.Count; ++index3)
                    {
                        if (this.inventory.AllGuns[index1].PickupObjectId == this.startingAlternateGunIds[index3])
                            flag = true;
                    }
                    if (!flag)
                    {
                        gunList.Add(this.inventory.AllGuns[index1]);
                        pickupObjectList.Add((PickupObject) this.inventory.AllGuns[index1]);
                    }
                }
            }
            for (int index = 0; index < this.passiveItems.Count; ++index)
            {
                if (this.passiveItems[index].CanActuallyBeDropped(this) && !this.passiveItems[index].PersistsOnDeath && !(this.passiveItems[index] is ExtraLifeItem))
                    pickupObjectList.Add((PickupObject) this.passiveItems[index]);
            }
            for (int index = 0; index < this.activeItems.Count; ++index)
            {
                if (this.activeItems[index].CanActuallyBeDropped(this) && !this.activeItems[index].PersistsOnDeath)
                    pickupObjectList.Add((PickupObject) this.activeItems[index]);
            }
            int count = pickupObjectList.Count;
            for (int index4 = 0; index4 < count; ++index4)
            {
                if (index4 == 0 && gunList.Count > 0)
                {
                    int index5 = UnityEngine.Random.Range(0, gunList.Count);
                    pickupObjectList.Remove((PickupObject) gunList[index5]);
                    this.ForceDropGun(gunList[index5]);
                    gunList.RemoveAt(index5);
                }
                else if (pickupObjectList.Count > 0)
                {
                    int index6 = UnityEngine.Random.Range(0, pickupObjectList.Count);
                    if (pickupObjectList[index6] is Gun)
                    {
                        DebrisObject debrisObject = this.ForceDropGun(pickupObjectList[index6] as Gun);
                        PickupObject componentInChildren = !(bool) (UnityEngine.Object) debrisObject ? (PickupObject) null : debrisObject.GetComponentInChildren<PickupObject>();
                        if ((bool) (UnityEngine.Object) componentInChildren)
                        {
                            componentInChildren.IgnoredByRat = true;
                            componentInChildren.ClearIgnoredByRatFlagOnPickup = true;
                        }
                    }
                    else if (pickupObjectList[index6] is PassiveItem)
                    {
                        DebrisObject debrisObject = this.DropPassiveItem(pickupObjectList[index6] as PassiveItem);
                        PickupObject componentInChildren = !(bool) (UnityEngine.Object) debrisObject ? (PickupObject) null : debrisObject.GetComponentInChildren<PickupObject>();
                        if ((bool) (UnityEngine.Object) componentInChildren)
                        {
                            componentInChildren.IgnoredByRat = true;
                            componentInChildren.ClearIgnoredByRatFlagOnPickup = true;
                        }
                    }
                    else
                    {
                        DebrisObject debrisObject = this.DropActiveItem(pickupObjectList[index6] as PlayerItem, isDeathDrop: true);
                        PickupObject componentInChildren = !(bool) (UnityEngine.Object) debrisObject ? (PickupObject) null : debrisObject.GetComponentInChildren<PickupObject>();
                        if ((bool) (UnityEngine.Object) componentInChildren)
                        {
                            componentInChildren.IgnoredByRat = true;
                            componentInChildren.ClearIgnoredByRatFlagOnPickup = true;
                        }
                    }
                    pickupObjectList.RemoveAt(index6);
                }
            }
        }

        [DebuggerHidden]
        public IEnumerator HandleCoopDeath(bool ignoreCorpse = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleCoopDeathc__IteratorA()
            {
                ignoreCorpse = ignoreCorpse,
                _this = this
            };
        }

        private void BecomeGhost()
        {
            this.IsGhost = true;
            GameManager.Instance.MainCameraController.IsLerping = true;
            this.ChangeSpecialShaderFlag(0, 1f);
            GameUIRoot.Instance.TransitionToGhostUI(this);
            this.ChangeFlatColorOverride(new Color(0.2f, 0.3f, 1f, 1f));
            this.specRigidbody.enabled = true;
            this.specRigidbody.CollideWithTileMap = true;
            this.specRigidbody.CollideWithOthers = true;
            this.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            this.specRigidbody.Reinitialize();
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
            this.ToggleShadowVisiblity(false);
            this.ToggleHandRenderers(false, "ghostliness");
            SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
            this.m_handlingQueuedAnimation = false;
            this.CurrentInputState = PlayerInputState.AllInput;
        }

        public void DoCoopArrow()
        {
            if (this.healthHaver.IsDead || !this.gameObject.activeSelf || this.m_isCoopArrowing)
                return;
            this.m_isCoopArrowing = true;
            this.StartCoroutine(this.HandleCoopArrow());
        }

        [DebuggerHidden]
        private IEnumerator HandleThreatArrow()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleThreatArrowc__IteratorB()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandleCoopArrow()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleCoopArrowc__IteratorC()
            {
                _this = this
            };
        }

        public void QueueSpecificAnimation(string animationName)
        {
            this.m_handlingQueuedAnimation = true;
            this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.QueuedAnimationComplete);
            this.spriteAnimator.Play(animationName);
        }

        protected void QueuedAnimationComplete(tk2dSpriteAnimator anima, tk2dSpriteAnimationClip clippy)
        {
            this.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.QueuedAnimationComplete);
            this.m_handlingQueuedAnimation = false;
        }

        [DebuggerHidden]
        private IEnumerator InvariantWait(float delay)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__InvariantWaitc__IteratorD()
            {
                delay = delay,
                _this = this
            };
        }

        protected void HandleDeathPhotography()
        {
            GameUIRoot.Instance.ForceClearReload();
            GameUIRoot.Instance.notificationController.ForceHide();
            Pixelator.Instance.CacheCurrentFrameToBuffer = true;
            Pixelator.Instance.CacheScreenSpacePositionsForDeathFrame(this.CenterPosition, this.CenterPosition);
        }

        [DebuggerHidden]
        private IEnumerator HandleDeath_CR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleDeath_CRc__IteratorE()
            {
                _this = this
            };
        }

        public void ClearDeadFlags()
        {
            this.CurrentInputState = PlayerInputState.AllInput;
            this.m_handlingQueuedAnimation = false;
        }

        private void RollPitMovementRestrictor(
            SpeculativeRigidbody specRigidbody,
            IntVector2 prevPixelOffset,
            IntVector2 pixelOffset,
            ref bool validLocation)
        {
            if (!validLocation || this.m_dodgeRollState != PlayerController.DodgeRollState.OnGround || this.IsFlying)
                return;
            Func<IntVector2, bool> func = (Func<IntVector2, bool>) (pixel =>
            {
                Vector2 unitMidpoint = PhysicsEngine.PixelToUnitMidpoint(pixel);
                if (!GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) unitMidpoint))
                    return false;
                List<SpeculativeRigidbody> platformsAt = GameManager.Instance.Dungeon.GetPlatformsAt((Vector3) unitMidpoint);
                if (platformsAt != null)
                {
                    for (int index = 0; index < platformsAt.Count; ++index)
                    {
                        if (platformsAt[index].PrimaryPixelCollider.ContainsPixel(pixel))
                            return false;
                    }
                }
                return true;
            });
            PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
            if (primaryPixelCollider == null)
                return;
            IntVector2 intVector2 = pixelOffset - prevPixelOffset;
            if (intVector2 == IntVector2.Down && func(primaryPixelCollider.LowerLeft + pixelOffset) && func(primaryPixelCollider.LowerRight + pixelOffset) && (!func(primaryPixelCollider.UpperRight + prevPixelOffset) || !func(primaryPixelCollider.UpperLeft + prevPixelOffset)))
                validLocation = false;
            else if (intVector2 == IntVector2.Right && func(primaryPixelCollider.LowerRight + pixelOffset) && func(primaryPixelCollider.UpperRight + pixelOffset) && (!func(primaryPixelCollider.UpperLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerLeft + prevPixelOffset)))
                validLocation = false;
            else if (intVector2 == IntVector2.Up && func(primaryPixelCollider.UpperRight + pixelOffset) && func(primaryPixelCollider.UpperLeft + pixelOffset) && (!func(primaryPixelCollider.LowerLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerRight + prevPixelOffset)))
            {
                validLocation = false;
            }
            else
            {
                if (!(intVector2 == IntVector2.Left) || !func(primaryPixelCollider.UpperLeft + pixelOffset) || !func(primaryPixelCollider.LowerLeft + pixelOffset) || func(primaryPixelCollider.LowerRight + prevPixelOffset) && func(primaryPixelCollider.UpperRight + prevPixelOffset))
                    return;
                validLocation = false;
            }
        }

        public void AcquirePuzzleItem(PickupObject item)
        {
            item.transform.parent = this.GunPivot;
            item.transform.localPosition = Vector3.zero;
            if ((bool) (UnityEngine.Object) item && (bool) (UnityEngine.Object) item.sprite)
                SpriteOutlineManager.RemoveOutlineFromSprite(item.sprite, true);
            this.additionalItems.Add(item);
        }

        public void UsePuzzleItem(PickupObject item)
        {
            if (!this.additionalItems.Contains(item))
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object) item.gameObject);
            this.additionalItems.Remove(item);
        }

        public PickupObject DropPuzzleItem(PickupObject item)
        {
            if (!this.additionalItems.Contains(item) || !(item is NPCCellKeyItem))
                return (PickupObject) null;
            this.additionalItems.Remove(item);
            item.transform.parent = (Transform) null;
            (item as NPCCellKeyItem).DropLogic();
            GameUIRoot.Instance.UpdatePlayerConsumables(this.carriedConsumables);
            return item;
        }

        public void AcquirePassiveItemPrefabDirectly(PassiveItem item)
        {
            PassiveItem component1 = UnityEngine.Object.Instantiate<GameObject>(item.gameObject).GetComponent<PassiveItem>();
            EncounterTrackable component2 = component1.GetComponent<EncounterTrackable>();
            if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
                component2.DoNotificationOnEncounter = false;
            component1.suppressPickupVFX = true;
            component1.Pickup(this);
        }

        public void AcquirePassiveItem(PassiveItem item)
        {
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_passive_get_01", this.gameObject);
            this.passiveItems.Add(item);
            item.transform.parent = this.GunPivot;
            item.transform.localPosition = Vector3.zero;
            item.renderer.enabled = false;
            if ((UnityEngine.Object) item.GetComponent<DebrisObject>() != (UnityEngine.Object) null)
                UnityEngine.Object.Destroy((UnityEngine.Object) item.GetComponent<DebrisObject>());
            if ((UnityEngine.Object) item.GetComponent<SquishyBounceWiggler>() != (UnityEngine.Object) null)
                UnityEngine.Object.Destroy((UnityEngine.Object) item.GetComponent<SquishyBounceWiggler>());
            GameUIRoot.Instance.AddPassiveItemToDock(item, this);
            this.stats.RecalculateStats(this);
        }

        public void DropPileOfSouls()
        {
            Vector3 position = this.specRigidbody.UnitBottomLeft.ToVector3ZUp();
            if (this.CurrentRoom != null)
                position = this.CurrentRoom.GetBestRewardLocation(new IntVector2(2, 2), this.specRigidbody.UnitBottomLeft, false).ToVector3();
            PileOfDarkSoulsPickup component = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global Prefabs/PileOfSouls"), position, Quaternion.identity).GetComponent<PileOfDarkSoulsPickup>();
            component.TargetPlayerID = this.PlayerIDX;
            RoomHandler.unassignedInteractableObjects.Add((IPlayerInteractable) component);
            component.containedCurrency = this.carriedConsumables.Currency;
            this.carriedConsumables.Currency = 0;
            for (int index = 0; index < this.passiveItems.Count; ++index)
            {
                if (this.passiveItems[index].CanActuallyBeDropped(this) && !this.passiveItems[index].PersistsOnDeath && this.passiveItems[index] is ExtraLifeItem && (this.passiveItems[index] as ExtraLifeItem).extraLifeMode == ExtraLifeItem.ExtraLifeMode.DARK_SOULS)
                {
                    DebrisObject debrisObject = this.DropPassiveItem(this.passiveItems[index]);
                    if ((bool) (UnityEngine.Object) debrisObject)
                    {
                        component.passiveItems.Add(debrisObject.GetComponent<PassiveItem>());
                        debrisObject.enabled = false;
                        --index;
                    }
                }
            }
            component.ToggleItems(false);
        }

        private void DontDontDestroyOnLoad(GameObject target)
        {
            if (!(bool) (UnityEngine.Object) target || !(bool) (UnityEngine.Object) GameManager.Instance.Dungeon || !((UnityEngine.Object) target.transform.parent == (UnityEngine.Object) null))
                return;
            target.transform.parent = GameManager.Instance.Dungeon.transform;
            target.transform.parent = (Transform) null;
        }

        public DebrisObject DropPassiveItem(PassiveItem item)
        {
            if ((bool) (UnityEngine.Object) item && this.startingPassiveItemIds != null && this.characterIdentity != PlayableCharacters.Eevee)
            {
                for (int index = 0; index < this.startingPassiveItemIds.Count; ++index)
                {
                    if (this.startingPassiveItemIds[index] == item.PickupObjectId)
                        return (DebrisObject) null;
                }
            }
            if (this.passiveItems.Contains(item))
            {
                this.passiveItems.Remove(item);
                GameUIRoot.Instance.RemovePassiveItemFromDock(item);
                DebrisObject debrisObject = item.Drop(this);
                this.stats.RecalculateStats(this);
                this.DontDontDestroyOnLoad(debrisObject.gameObject);
                return debrisObject;
            }
            UnityEngine.Debug.LogError((object) ("Failed to drop item because the player doesn't have it? " + item.DisplayName));
            return (DebrisObject) null;
        }

        public DebrisObject DropActiveItem(PlayerItem item, float overrideForce = 4f, bool isDeathDrop = false)
        {
            if (isDeathDrop && (bool) (UnityEngine.Object) item && this.startingActiveItemIds != null)
            {
                for (int index = 0; index < this.startingActiveItemIds.Count; ++index)
                {
                    PlayerItem byId = PickupObjectDatabase.GetById(this.startingActiveItemIds[index]) as PlayerItem;
                    if (byId.PickupObjectId == item.PickupObjectId && !byId.CanActuallyBeDropped(this))
                        return (DebrisObject) null;
                }
            }
            if (this.activeItems.Contains(item))
            {
                UnityEngine.Debug.Log((object) "DROPPING ACTIVE ITEM NOW");
                this.activeItems.Remove(item);
                DebrisObject debrisObject = item.Drop(this, overrideForce);
                UnityEngine.Object.Destroy((UnityEngine.Object) item.gameObject);
                return debrisObject;
            }
            UnityEngine.Debug.LogError((object) ("Failed to drop item because the player doesn't have it? " + item.DisplayName));
            return (DebrisObject) null;
        }

        public void GetEquippedWith(PlayerItem item, bool switchTo = false)
        {
            if (this.m_preventItemSwitching)
            {
                this.RemoveActiveItemAt(this.m_selectedItemIndex);
                this.StopCoroutine(this.m_currentActiveItemDestructionCoroutine);
                this.m_currentActiveItemDestructionCoroutine = (Coroutine) null;
                this.m_preventItemSwitching = false;
            }
            if (this.m_suppressItemSwitchTo)
                switchTo = false;
            item.transform.parent = this.GunPivot;
            item.transform.localPosition = Vector3.zero;
            int index1 = -1;
            for (int index2 = 0; index2 < this.activeItems.Count; ++index2)
            {
                if (this.activeItems[index2].PickupObjectId == item.PickupObjectId)
                {
                    index1 = index2;
                    break;
                }
            }
            int num1 = 0;
            for (int index3 = 0; index3 < item.passiveStatModifiers.Length; ++index3)
            {
                if (item.passiveStatModifiers[index3].statToBoost == PlayerStats.StatType.AdditionalItemCapacity)
                    num1 += Mathf.RoundToInt(item.passiveStatModifiers[index3].amount);
            }
            if (item is TeleporterPrototypeItem)
            {
                for (int index4 = 0; index4 < this.activeItems.Count; ++index4)
                {
                    if (this.activeItems[index4] is ChestTeleporterItem)
                    {
                        ++num1;
                        break;
                    }
                }
            }
            else if (item is ChestTeleporterItem)
            {
                for (int index5 = 0; index5 < this.activeItems.Count; ++index5)
                {
                    if (this.activeItems[index5] is TeleporterPrototypeItem)
                    {
                        ++num1;
                        break;
                    }
                }
            }
            if (index1 == -1)
            {
                int num2 = this.MAX_ITEMS_HELD + (int) this.stats.GetStatValue(PlayerStats.StatType.AdditionalItemCapacity) + num1;
                if ((UnityEngine.Object) this.stats != (UnityEngine.Object) null)
                {
                    for (int index6 = 0; this.activeItems.Count >= num2 && index6 < 100; num2 = this.MAX_ITEMS_HELD + (int) this.stats.GetStatValue(PlayerStats.StatType.AdditionalItemCapacity) + num1)
                    {
                        ++index6;
                        this.DropActiveItem(this.CurrentItem);
                        this.stats.RecalculateStats(this);
                    }
                }
                this.activeItems.Add(item);
                if (switchTo)
                    this.m_selectedItemIndex = this.activeItems.Count - 1;
            }
            else
            {
                if (item.canStack)
                {
                    this.activeItems[index1].numberOfUses += item.numberOfUses;
                    if (switchTo)
                        this.m_selectedItemIndex = index1;
                }
                UnityEngine.Object.Destroy((UnityEngine.Object) item.gameObject);
            }
            this.stats.RecalculateStats(this);
        }

        public void ForceConsumableBlank()
        {
            if (!this.AcceptingNonMotionInput || (double) UnityEngine.Time.timeScale <= 0.0)
                return;
            this.DoConsumableBlank();
        }

        protected void DoConsumableBlank()
        {
            if (this.Blanks <= 0)
                return;
            --this.Blanks;
            PlatformInterface.SetAlienFXColor((Color32) this.m_alienBlankColor, 1f);
            this.ForceBlank();
            if (!this.IsInCombat)
            {
                for (int index = 0; index < StaticReferenceManager.AllAdvancedShrineControllers.Count; ++index)
                {
                    if (StaticReferenceManager.AllAdvancedShrineControllers[index].IsBlankShrine && StaticReferenceManager.AllAdvancedShrineControllers[index].transform.position.GetAbsoluteRoom() == this.CurrentRoom)
                        StaticReferenceManager.AllAdvancedShrineControllers[index].OnBlank();
                }
            }
            for (int index = 0; index < StaticReferenceManager.AllRatTrapdoors.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) StaticReferenceManager.AllRatTrapdoors[index])
                    StaticReferenceManager.AllRatTrapdoors[index].OnBlank();
            }
            this.m_blankCooldownTimer = 0.5f;
            this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
            if (this.OnUsedBlank == null)
                return;
            this.OnUsedBlank(this, this.Blanks);
        }

        public void ForceBlank(
            float overrideRadius = 25f,
            float overrideTimeAtMaxRadius = 0.5f,
            bool silent = false,
            bool breaksWalls = true,
            Vector2? overrideCenter = null,
            bool breaksObjects = true,
            float overrideForce = -1f)
        {
            if (!silent)
            {
                if ((UnityEngine.Object) this.BlankVFXPrefab == (UnityEngine.Object) null)
                    this.BlankVFXPrefab = (GameObject) BraveResources.Load("Global VFX/BlankVFX");
                int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", this.gameObject);
                int num2 = (int) AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", this.gameObject);
            }
            SilencerInstance silencerInstance = new GameObject("silencer").AddComponent<SilencerInstance>();
            float additionalTimeAtMaxRadius = overrideTimeAtMaxRadius;
            silencerInstance.TriggerSilencer(!overrideCenter.HasValue ? this.CenterPosition : overrideCenter.Value, 50f, overrideRadius, !silent ? this.BlankVFXPrefab : (GameObject) null, !silent ? 0.15f : 0.0f, !silent ? 0.2f : 0.0f, !silent ? 50f : 0.0f, !silent ? 10f : 0.0f, !silent ? ((double) overrideForce < 0.0 ? 140f : overrideForce) : 0.0f, !breaksObjects ? 0.0f : (!silent ? 15f : 5f), additionalTimeAtMaxRadius, this, breaksWalls);
            this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
        }

        protected void DoGhostBlank()
        {
            if ((UnityEngine.Object) this.BlankVFXPrefab == (UnityEngine.Object) null)
                this.BlankVFXPrefab = (GameObject) BraveResources.Load("Global VFX/BlankVFX_Ghost");
            PlatformInterface.SetAlienFXColor((Color32) this.m_alienBlankColor, 1f);
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", this.gameObject);
            new GameObject("silencer").AddComponent<SilencerInstance>().TriggerSilencer(this.CenterPosition, 20f, 3f, this.BlankVFXPrefab, 0.0f, 3f, 50f, 4f, 30f, 3f, 0.25f, this, false);
            this.QueueSpecificAnimation("ghost_sneeze_right");
            this.m_blankCooldownTimer = 5f;
            this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
        }

        protected void UseItem()
        {
            PlayerItem currentItem = this.CurrentItem;
            if (!((UnityEngine.Object) currentItem != (UnityEngine.Object) null) || !currentItem.CanBeUsed(this))
                return;
            if (this.OnUsedPlayerItem != null && !currentItem.IsOnCooldown)
                this.OnUsedPlayerItem(this, currentItem);
            float destroyTime = -1f;
            if (currentItem.Use(this, out destroyTime))
            {
                if ((double) destroyTime >= 0.0)
                    this.m_currentActiveItemDestructionCoroutine = this.StartCoroutine(this.TimedRemoveActiveItem(this.m_selectedItemIndex, destroyTime));
                else
                    this.RemoveActiveItemAt(this.m_selectedItemIndex);
            }
            else if (!currentItem.consumable || currentItem.numberOfUses <= 0)
                ;
            this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
        }

        [DebuggerHidden]
        private IEnumerator TimedRemoveActiveItem(int indexToRemove, float delay)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__TimedRemoveActiveItemc__IteratorF()
            {
                delay = delay,
                indexToRemove = indexToRemove,
                _this = this
            };
        }

        public void RemoveAllActiveItems()
        {
            for (int index = this.activeItems.Count - 1; index >= 0; --index)
                this.RemoveActiveItemAt(index);
        }

        public void RemoveAllPassiveItems()
        {
            for (int index = this.passiveItems.Count - 1; index >= 0; --index)
                this.RemovePassiveItemAt(index);
        }

        public void RemoveActiveItem(int pickupId)
        {
            int index = this.activeItems.FindIndex((Predicate<PlayerItem>) (a => a.PickupObjectId == pickupId));
            if (index >= 0)
                this.RemoveActiveItemAt(index);
            else
                UnityEngine.Debug.LogError((object) ("Failed to remove active item because the player doesn't have it? pickupId = " + (object) pickupId));
        }

        protected void RemoveActiveItemAt(int index)
        {
            if (index < 0 || index >= this.activeItems.Count)
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object) this.activeItems[index].gameObject);
            this.activeItems.RemoveAt(index);
            if (this.m_selectedItemIndex >= 0 && this.m_selectedItemIndex < this.activeItems.Count)
                return;
            this.m_selectedItemIndex = 0;
        }

        public bool HasPickupID(int pickupId)
        {
            return this.HasGun(pickupId) || this.HasActiveItem(pickupId) || this.HasPassiveItem(pickupId);
        }

        public bool HasGun(int pickupId)
        {
            for (int index = 0; index < this.inventory.AllGuns.Count; ++index)
            {
                if (this.inventory.AllGuns[index].PickupObjectId == pickupId)
                    return true;
            }
            return false;
        }

        public bool HasActiveItem(int pickupId)
        {
            return this.activeItems.FindIndex((Predicate<PlayerItem>) (a => a.PickupObjectId == pickupId)) >= 0;
        }

        public bool HasPassiveItem(int pickupId)
        {
            return this.passiveItems.FindIndex((Predicate<PassiveItem>) (a => a.PickupObjectId == pickupId)) >= 0;
        }

        public void RemovePassiveItem(int pickupId)
        {
            int index = this.passiveItems.FindIndex((Predicate<PassiveItem>) (p => p.PickupObjectId == pickupId));
            if (index >= 0)
                this.RemovePassiveItemAt(index);
            else
                UnityEngine.Debug.LogError((object) ("Failed to remove passive item because the player doesn't have it? pickupId = " + (object) pickupId));
        }

        protected void RemovePassiveItemAt(int index)
        {
            PassiveItem passiveItem = this.passiveItems[index];
            this.passiveItems.RemoveAt(index);
            GameUIRoot.Instance.RemovePassiveItemFromDock(passiveItem);
            UnityEngine.Object.Destroy((UnityEngine.Object) passiveItem);
            this.stats.RecalculateStats(this);
        }

        public void BloopItemAboveHead(tk2dBaseSprite targetSprite, string overrideSprite = "")
        {
            this.m_blooper.DoBloop(targetSprite, overrideSprite, Color.white);
        }

        public void BloopItemAboveHead(
            tk2dBaseSprite targetSprite,
            string overrideSprite,
            Color tintColor,
            bool addOutline = false)
        {
            this.m_blooper.DoBloop(targetSprite, overrideSprite, tintColor, addOutline);
        }

        protected override void Fall()
        {
            if (this.m_isFalling || this.IsDodgeRolling && this.DodgeRollIsBlink)
                return;
            base.Fall();
            if (this.OnPitfall != null)
                this.OnPitfall();
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                GameManager.Instance.platformInterface.AchievementUnlock(Achievement.FALL_IN_END_TIMES);
            if (GameManager.Instance.InTutorial)
            {
                GameManager.BroadcastRoomTalkDoerFsmEvent("playerFellInPit");
                if (this.m_dodgeRollState == PlayerController.DodgeRollState.OnGround)
                    GameManager.BroadcastRoomTalkDoerFsmEvent("playerFellInPitEarly");
                else
                    GameManager.BroadcastRoomTalkDoerFsmEvent("playerFellInPitLate");
            }
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.PITS_FALLEN_INTO, 1f);
            this.CurrentInputState = PlayerInputState.NoInput;
            this.healthHaver.IsVulnerable = false;
            this.healthHaver.EndFlashEffects();
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON && this.CurrentRoom != null && this.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL && (this.CurrentRoom.area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.STANDARD_SHOP || this.CurrentRoom.area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.WEIRD_SHOP))
                this.LevelToLoadOnPitfall = "tt_nakatomi";
            if (!string.IsNullOrEmpty(this.LevelToLoadOnPitfall) && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                GameManager.Instance.GetOtherPlayer(this).m_inputState = PlayerInputState.NoInput;
            this.m_cachedLevelToLoadOnPitfall = this.LevelToLoadOnPitfall;
            if (!string.IsNullOrEmpty(this.m_cachedLevelToLoadOnPitfall))
            {
                Pixelator.Instance.FadeToBlack(0.5f, holdTime: 0.5f);
                GameUIRoot.Instance.HideCoreUI(string.Empty);
                GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
            }
            this.LevelToLoadOnPitfall = string.Empty;
            this.StartCoroutine(this.FallDownCR());
        }

        protected override void ModifyPitVectors(ref Rect modifiedRect)
        {
            base.ModifyPitVectors(ref modifiedRect);
            if (this.m_dodgeRollState == PlayerController.DodgeRollState.OnGround)
            {
                if ((double) Mathf.Abs(this.lockedDodgeRollDirection.x) > 0.0099999997764825821)
                {
                    if ((double) this.lockedDodgeRollDirection.x > 0.0099999997764825821)
                        modifiedRect.xMax += PhysicsEngine.PixelToUnit(this.pitHelpers.Landing.x);
                    else if ((double) this.lockedDodgeRollDirection.x < -0.0099999997764825821)
                        modifiedRect.xMin -= PhysicsEngine.PixelToUnit(this.pitHelpers.Landing.x);
                }
                if ((double) Mathf.Abs(this.lockedDodgeRollDirection.y) <= 0.0099999997764825821)
                    return;
                if ((double) this.lockedDodgeRollDirection.y > 0.0099999997764825821)
                {
                    modifiedRect.yMax += PhysicsEngine.PixelToUnit(this.pitHelpers.Landing.y);
                }
                else
                {
                    if ((double) this.lockedDodgeRollDirection.y >= -0.0099999997764825821)
                        return;
                    modifiedRect.yMin -= PhysicsEngine.PixelToUnit(this.pitHelpers.Landing.y);
                }
            }
            else
            {
                if ((double) Mathf.Abs(this.m_playerCommandedDirection.x) > 0.0099999997764825821)
                {
                    if ((double) this.m_playerCommandedDirection.x < -0.0099999997764825821)
                        modifiedRect.xMax += PhysicsEngine.PixelToUnit(this.pitHelpers.PreJump.x);
                    else if ((double) this.m_playerCommandedDirection.x > 0.0099999997764825821)
                        modifiedRect.xMin -= PhysicsEngine.PixelToUnit(this.pitHelpers.PreJump.x);
                }
                if ((double) Mathf.Abs(this.m_playerCommandedDirection.y) <= 0.0099999997764825821)
                    return;
                if ((double) this.m_playerCommandedDirection.y < -0.0099999997764825821)
                {
                    modifiedRect.yMax += PhysicsEngine.PixelToUnit(this.pitHelpers.PreJump.y);
                    modifiedRect.yMax += PhysicsEngine.PixelToUnit(this.specRigidbody.PrimaryPixelCollider.Width - this.specRigidbody.PrimaryPixelCollider.Height);
                }
                else
                {
                    if ((double) this.m_playerCommandedDirection.y <= 0.0099999997764825821)
                        return;
                    modifiedRect.yMin -= PhysicsEngine.PixelToUnit(this.pitHelpers.PreJump.y);
                }
            }
        }

        public void PrepareForSceneTransition()
        {
            this.m_inputState = PlayerInputState.NoInput;
            this.IsVisible = false;
        }

        public void DoInitialFallSpawn(float invisibleDelay)
        {
            this.StartCoroutine(this.HandleFallSpawn(invisibleDelay));
        }

        public void DoSpinfallSpawn(float invisibleDelay)
        {
            if (this.healthHaver.IsDead)
                return;
            this.StartCoroutine(this.HandleSpinfallSpawn(invisibleDelay));
        }

        [DebuggerHidden]
        protected IEnumerator HandleSpinfallSpawn(float invisibleDelay)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleSpinfallSpawnc__Iterator10()
            {
                invisibleDelay = invisibleDelay,
                _this = this
            };
        }

        [DebuggerHidden]
        protected IEnumerator HandleFallSpawn(float invisibleDelay)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleFallSpawnc__Iterator11()
            {
                invisibleDelay = invisibleDelay,
                _this = this
            };
        }

        public void DoSpitOut() => this.StartCoroutine(this.HandleSpitOut());

        [DebuggerHidden]
        protected IEnumerator HandleSpitOut()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleSpitOutc__Iterator12()
            {
                _this = this
            };
        }

        protected void TogglePitClipping(bool doClip)
        {
            if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.sprite || !(bool) (UnityEngine.Object) this.sprite.gameObject)
                return;
            TileSpriteClipper component = this.sprite.gameObject.GetComponent<TileSpriteClipper>();
            if ((bool) (UnityEngine.Object) component)
                component.enabled = doClip;
            tk2dBaseSprite[] outlineSprites = (tk2dBaseSprite[]) SpriteOutlineManager.GetOutlineSprites(this.sprite);
            if (outlineSprites == null)
                return;
            for (int index = 0; index < outlineSprites.Length; ++index)
            {
                if ((bool) (UnityEngine.Object) outlineSprites[index])
                    component = outlineSprites[index].GetComponent<TileSpriteClipper>();
                if ((bool) (UnityEngine.Object) component)
                    component.enabled = doClip;
            }
        }

        private RoomHandler GetCurrentCellPitfallTarget()
        {
            IntVector2 intVector2 = this.CenterPosition.ToIntVector2(VectorConversions.Floor);
            return GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) ? GameManager.Instance.Dungeon.data[intVector2].targetPitfallRoom : (RoomHandler) null;
        }

        [DebuggerHidden]
        protected IEnumerator PitRespawn(Vector2 splashPoint)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__PitRespawnc__Iterator13()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator FallDownCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__FallDownCRc__Iterator14()
            {
                _this = this
            };
        }

        protected void AnimationCompleteDelegate(tk2dSpriteAnimator anima, tk2dSpriteAnimationClip clippy)
        {
            if (clippy.name.ToLowerInvariant().Contains("dodge"))
            {
                this.ToggleGunRenderers(true, "dodgeroll");
                this.ToggleHandRenderers(true, "dodgeroll");
                if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null || string.IsNullOrEmpty(this.CurrentGun.dodgeAnimation))
                {
                    this.ToggleGunRenderers(false, "postdodgeroll");
                    this.ToggleHandRenderers(false, "postdodgeroll");
                    this.m_postDodgeRollGunTimer = 0.05f;
                }
            }
            if (clippy.name.ToLowerInvariant().Contains("item_get"))
            {
                this.CurrentInputState = PlayerInputState.AllInput;
                this.GetComponent<HealthHaver>().IsVulnerable = true;
                this.ToggleGunRenderers(true, "itemGet");
                this.ToggleHandRenderers(true, "itemGet");
            }
            this.m_handlingQueuedAnimation = false;
            this.m_overrideGunAngle = new float?();
        }

        public void TriggerItemAcquisition()
        {
            this.m_handlingQueuedAnimation = true;
            this.CurrentInputState = PlayerInputState.NoInput;
            this.specRigidbody.Velocity = Vector2.zero;
            this.ToggleGunRenderers(false, "itemGet");
            this.ToggleHandRenderers(false, "itemGet");
            this.GetComponent<HealthHaver>().IsVulnerable = false;
            this.spriteAnimator.Play(!this.UseArmorlessAnim ? "item_get" : "item_get_armorless");
        }

        private void HandleAttachedSpriteDepth(float gunAngle)
        {
            float num1 = 1f;
            if (this.IsDodgeRolling)
                gunAngle = BraveMathCollege.Atan2Degrees(this.lockedDodgeRollDirection);
            float num2;
            if ((double) gunAngle <= 155.0 && (double) gunAngle >= 25.0)
            {
                num1 = -1f;
                num2 = (double) gunAngle >= 120.0 || (double) gunAngle < 60.0 ? 0.15f : 0.15f;
            }
            else
                num2 = (double) gunAngle > -60.0 || (double) gunAngle < -120.0 ? -0.15f : -0.15f;
            for (int index = 0; index < this.m_attachedSprites.Count; ++index)
                this.m_attachedSprites[index].HeightOffGround = num2 + num1 * this.m_attachedSpriteDepths[index];
        }

        public void ForceWalkInDirectionWhilePaused(DungeonData.Direction direction, float thresholdValue)
        {
            this.StartCoroutine(this.HandleForceWalkInDirectionWhilePaused(direction, thresholdValue));
        }

        [DebuggerHidden]
        private IEnumerator HandleForceWalkInDirectionWhilePaused(
            DungeonData.Direction direction,
            float thresholdValue)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleForceWalkInDirectionWhilePausedc__Iterator15()
            {
                direction = direction,
                thresholdValue = thresholdValue,
                _this = this
            };
        }

        public bool IsBackfacing()
        {
            float num = !this.IsDodgeRolling || !this.m_handlingQueuedAnimation ? this.m_currentGunAngle : BraveMathCollege.Atan2Degrees(this.lockedDodgeRollDirection);
            return (double) num <= 155.0 && (double) num >= 25.0;
        }

        public string GetBaseAnimationSuffix(bool useCardinal = false)
        {
            float num = !this.IsDodgeRolling || !this.m_handlingQueuedAnimation ? this.m_currentGunAngle : BraveMathCollege.Atan2Degrees(this.lockedDodgeRollDirection);
            return (double) num <= 155.0 && (double) num >= 25.0 ? ((double) num < 120.0 && (double) num >= 60.0 ? (useCardinal ? "_north" : "_back") : (useCardinal ? "_north_east" : "_back_right")) : ((double) num <= -60.0 && (double) num >= -120.0 ? (useCardinal ? "_south" : "_front") : (useCardinal ? "_south_east" : "_front_right"));
        }

        public int GetMirrorSpriteID()
        {
            float gunAngle = BraveMathCollege.Atan2Degrees(Vector2.Scale(BraveMathCollege.DegreesToVector(this.m_currentGunAngle), new Vector2(1f, -1f)));
            tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.GetBaseAnimationName(this.m_playerCommandedDirection.WithY(this.m_playerCommandedDirection.y * -1f), gunAngle, true));
            int currentFrame = this.spriteAnimator.CurrentFrame;
            return clipByName != null && currentFrame >= 0 && currentFrame < clipByName.frames.Length ? clipByName.frames[currentFrame].spriteId : this.sprite.spriteId;
        }

        protected virtual string GetBaseAnimationName(
            Vector2 v,
            float gunAngle,
            bool invertThresholds = false,
            bool forceTwoHands = false)
        {
            string empty = string.Empty;
            bool flag1 = (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null;
            if (flag1 && this.CurrentGun.Handedness == GunHandedness.NoHanded)
                forceTwoHands = true;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                flag1 = false;
            float num1 = 155f;
            float num2 = 25f;
            if (invertThresholds)
            {
                num1 = -155f;
                num2 -= 50f;
            }
            float num3 = 120f;
            float num4 = 60f;
            float num5 = -60f;
            float num6 = -120f;
            bool flag2 = (double) gunAngle <= (double) num1 && (double) gunAngle >= (double) num2;
            if (invertThresholds)
                flag2 = (double) gunAngle <= (double) num1 || (double) gunAngle >= (double) num2;
            string baseAnimationName;
            if (this.IsGhost)
            {
                if (flag2)
                {
                    if ((double) gunAngle < (double) num3 && (double) gunAngle >= (double) num4)
                    {
                        baseAnimationName = "ghost_idle_back";
                    }
                    else
                    {
                        float num7 = 105f;
                        baseAnimationName = (double) Mathf.Abs(gunAngle) <= (double) num7 ? "ghost_idle_back_right" : "ghost_idle_back_left";
                    }
                }
                else if ((double) gunAngle <= (double) num5 && (double) gunAngle >= (double) num6)
                {
                    baseAnimationName = "ghost_idle_front";
                }
                else
                {
                    float num8 = 105f;
                    baseAnimationName = (double) Mathf.Abs(gunAngle) <= (double) num8 ? "ghost_idle_right" : "ghost_idle_left";
                }
            }
            else if (this.IsFlying)
                baseAnimationName = !flag2 ? ((double) gunAngle > (double) num5 || (double) gunAngle < (double) num6 ? (!this.RenderBodyHand ? "jetpack_right" : "jetpack_right_hand") : (!this.RenderBodyHand ? "jetpack_down" : "jetpack_down_hand")) : ((double) gunAngle >= (double) num3 || (double) gunAngle < (double) num4 ? "jetpack_right_bw" : "jetpack_up");
            else if (v == Vector2.zero || this.IsStationary)
                baseAnimationName = !this.IsPetting ? (!flag2 ? ((double) gunAngle > (double) num5 || (double) gunAngle < (double) num6 ? (!forceTwoHands && flag1 || this.ForceHandless ? (!this.RenderBodyHand ? "idle" : "idle_hand") : "idle_twohands") : (!forceTwoHands && flag1 || this.ForceHandless ? (!this.RenderBodyHand ? "idle_forward" : "idle_forward_hand") : "idle_forward_twohands")) : ((double) gunAngle >= (double) num3 || (double) gunAngle < (double) num4 ? (!forceTwoHands && flag1 || this.ForceHandless ? "idle_bw" : "idle_bw_twohands") : (!forceTwoHands && flag1 || this.ForceHandless ? (!this.RenderBodyHand ? "idle_backward" : "idle_backward_hand") : "idle_backward_twohands"))) : "pet";
            else if (flag2)
            {
                string str = !forceTwoHands && flag1 || this.ForceHandless ? "run_right_bw" : "run_right_bw_twohands";
                if ((double) gunAngle < (double) num3 && (double) gunAngle >= (double) num4)
                    str = !forceTwoHands && flag1 || this.ForceHandless ? (!this.RenderBodyHand ? "run_up" : "run_up_hand") : "run_up_twohands";
                baseAnimationName = str;
            }
            else
            {
                string str = "run_right";
                if ((double) gunAngle <= (double) num5 && (double) gunAngle >= (double) num6)
                    str = "run_down";
                if ((forceTwoHands || !flag1) && !this.ForceHandless)
                    str += "_twohands";
                else if (this.RenderBodyHand)
                    str += "_hand";
                baseAnimationName = str;
            }
            if (this.UseArmorlessAnim && !this.IsGhost)
                baseAnimationName += "_armorless";
            return baseAnimationName;
        }

        private void HandleAnimations(Vector2 v, float gunAngle)
        {
            if (this.m_handlingQueuedAnimation)
                return;
            if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null || this.IsGhost)
                gunAngle = BraveMathCollege.Atan2Degrees(!(this.m_playerCommandedDirection == Vector2.zero) ? this.m_playerCommandedDirection : this.m_lastNonzeroCommandedDirection);
            string baseAnimationName = this.GetBaseAnimationName(v, gunAngle);
            if (this.spriteAnimator.IsPlaying(baseAnimationName))
                return;
            this.spriteAnimator.Play(baseAnimationName);
        }

        protected bool IsKeyboardAndMouse()
        {
            return BraveInput.GetInstanceForPlayer(this.PlayerIDX).IsKeyboardAndMouse();
        }

        protected bool UseFakeSemiAutoCooldown => true;

        protected Vector3 DetermineAimPointInWorld()
        {
            if ((double) UnityEngine.Time.timeScale == 0.0)
                return this.unadjustedAimPoint;
            Vector3 aimPointInWorld = Vector3.zero;
            Camera component = GameManager.Instance.MainCameraController.GetComponent<Camera>();
            Vector3 position = this.gunAttachPoint.position;
            if (this.forceAimPoint.HasValue)
            {
                this.unadjustedAimPoint = (Vector3) this.forceAimPoint.Value;
                aimPointInWorld = this.unadjustedAimPoint;
            }
            else if (this.IsKeyboardAndMouse())
            {
                Ray ray = component.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
                float enter;
                if (new Plane(Vector3.forward, position).Raycast(ray, out enter))
                {
                    this.unadjustedAimPoint = ray.GetPoint(enter);
                    aimPointInWorld = this.unadjustedAimPoint;
                }
            }
            else
            {
                bool flag1 = BraveInput.AutoAimMode == BraveInput.AutoAim.SuperAutoAim;
                Vector2 unitCenter = this.specRigidbody.HitboxPixelCollider.UnitCenter;
                Vector2 vector2_1 = this.m_activeActions.Aim.Vector;
                bool flag2 = (BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButton(GungeonActions.GungeonActionType.Shoot) || BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButtonDown(GungeonActions.GungeonActionType.Shoot) || this.SuperAutoAimTarget != null) & (double) vector2_1.magnitude > 0.40000000596046448;
                bool flag3 = false;
                bool flag4 = false;
                switch (GameManager.Options.controllerAutoAim)
                {
                    case GameOptions.ControllerAutoAim.ALWAYS:
                        flag4 = true;
                        break;
                    case GameOptions.ControllerAutoAim.NEVER:
                        flag4 = false;
                        break;
                    case GameOptions.ControllerAutoAim.COOP_ONLY:
                        flag4 = this.PlayerIDX != 0;
                        break;
                }
                if (GameManager.Options.controllerAutoAim == GameOptions.ControllerAutoAim.AUTO_DETECT && !this.IsKeyboardAndMouse() && this.IsPrimaryPlayer)
                {
                    if (this.IsInCombat)
                    {
                        if ((double) vector2_1.magnitude < 0.40000000596046448)
                        {
                            float aaNonStickTime = PlayerController.AANonStickTime;
                            PlayerController.AANonStickTime = Mathf.Min(PlayerController.AANonStickTime + BraveTime.DeltaTime, 660f);
                            PlayerController.AAStickTime = Mathf.Min(PlayerController.AAStickTime, 660f - PlayerController.AANonStickTime);
                        }
                        else
                        {
                            PlayerController.AAStickTime = Mathf.Min(PlayerController.AAStickTime + BraveTime.DeltaTime * 1.5f, 660f);
                            PlayerController.AANonStickTime = Mathf.Min(PlayerController.AANonStickTime, 660f - PlayerController.AAStickTime);
                        }
                        if (!PlayerController.AACanWarn && (double) PlayerController.AANonStickTime < 300.0 && (double) UnityEngine.Time.realtimeSinceStartup > (double) PlayerController.AALastWarnTime + 300.0)
                            PlayerController.AACanWarn = true;
                    }
                    else if ((double) PlayerController.AANonStickTime > 600.0)
                    {
                        this.DoAutoAimNotification(false);
                        GameManager.Options.controllerAutoAim = GameOptions.ControllerAutoAim.ALWAYS;
                        PlayerController.AAStickTime = 0.0f;
                        PlayerController.AANonStickTime = 0.0f;
                        PlayerController.AALastWarnTime = -1000f;
                        PlayerController.AACanWarn = true;
                    }
                    else if (PlayerController.AACanWarn && (double) PlayerController.AANonStickTime > 300.0)
                    {
                        this.DoAutoAimNotification(true);
                        PlayerController.AALastWarnTime = UnityEngine.Time.realtimeSinceStartup;
                        PlayerController.AACanWarn = false;
                    }
                }
                bool flag5 = flag4 && (double) vector2_1.magnitude < 0.40000000596046448;
                if (this.HighAccuracyAimMode)
                {
                    if (!this.m_activeActions.HighAccuracyAimMode)
                        this.m_activeActions.HighAccuracyAimMode = true;
                    vector2_1 = (double) vector2_1.magnitude >= 0.20000000298023224 ? vector2_1.normalized * Mathf.Lerp(0.2f, 1f, vector2_1.magnitude) : Vector2.zero;
                    if (this.m_previousAimVector != Vector2.zero && (double) this.m_previousAimVector.magnitude > 0.8 && vector2_1 != Vector2.zero && (double) vector2_1.magnitude < 0.60000002384185791)
                    {
                        float num = BraveMathCollege.AbsAngleBetween(vector2_1.ToAngle(), this.m_previousAimVector.ToAngle());
                        if ((double) num < 15.0 || (double) num > 155.0)
                            vector2_1 = this.m_previousAimVector.normalized * 0.5f;
                    }
                    if (vector2_1 == Vector2.zero || this.m_previousAimVector == Vector2.zero || (double) BraveMathCollege.AbsAngleBetween(vector2_1.ToAngle(), this.m_previousAimVector.ToAngle()) > 10.0)
                        this.m_previousAimVector = vector2_1;
                    vector2_1 = BraveMathCollege.MovingAverage(this.m_previousAimVector, vector2_1, 3);
                    this.m_previousAimVector = vector2_1;
                }
                else
                {
                    if (this.m_activeActions.HighAccuracyAimMode)
                        this.m_activeActions.HighAccuracyAimMode = false;
                    if ((double) vector2_1.magnitude < 0.40000000596046448)
                    {
                        if (this.m_allowMoveAsAim)
                            vector2_1 = this.m_activeActions.Move.Vector;
                        else
                            flag3 = true;
                    }
                    vector2_1 = this.AdjustInputVector(vector2_1, BraveInput.MagnetAngles.aimCardinal, BraveInput.MagnetAngles.aimOrdinal);
                }
                if (flag1 && !flag2)
                    this.SuperAutoAimTarget = (IAutoAimTarget) null;
                if ((double) vector2_1.magnitude < 0.40000000596046448)
                    vector2_1 = (Vector2) this.m_cachedAimDirection;
                this.m_cachedAimDirection = (Vector3) vector2_1;
                this.unadjustedAimPoint = position + (Vector3) (vector2_1.normalized * 6f);
                aimPointInWorld = position + (Vector3) (vector2_1.normalized * 150f);
                bool flag6 = false;
                float num1 = 20f;
                bool flag7 = (bool) (UnityEngine.Object) this.CurrentGun || this is PlayerSpaceshipController;
                bool flag8 = !((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null) && this.CurrentGun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Beam;
                if (((double) GameManager.Options.controllerAimAssistMultiplier > 0.0 || flag5) && flag7 && (GameManager.Options.controllerBeamAimAssist || !flag8) && this.CurrentRoom != null && !flag3)
                {
                    float angle = (unitCenter + vector2_1.normalized * num1 - unitCenter).ToAngle();
                    List<IAutoAimTarget> autoAimTargets = this.CurrentRoom.GetAutoAimTargets();
                    if (this.CurrentRoom != null && (autoAimTargets != null || GameManager.PVP_ENABLED))
                    {
                        Projectile projectile = (Projectile) null;
                        if ((bool) (UnityEngine.Object) this.CurrentGun && this.CurrentGun.DefaultModule != null)
                            projectile = this.CurrentGun.DefaultModule.GetCurrentProjectile();
                        float num2 = !(bool) (UnityEngine.Object) projectile ? float.MaxValue : projectile.baseData.speed;
                        if ((double) num2 < 0.0)
                            num2 = float.MaxValue;
                        IAutoAimTarget autoAimTarget1 = (IAutoAimTarget) null;
                        float num3 = 0.0f;
                        float num4 = 0.0f;
                        int num5 = (autoAimTargets == null ? 0 : autoAimTargets.Count) + (!GameManager.PVP_ENABLED ? 0 : 1);
                        for (int index = 0; index < num5; ++index)
                        {
                            IAutoAimTarget autoAimTarget2 = autoAimTargets == null || index >= autoAimTargets.Count ? (IAutoAimTarget) GameManager.Instance.GetOtherPlayer(this) : autoAimTargets[index];
                            if (autoAimTarget2 != null && (!(autoAimTarget2 is Component) || (bool) (UnityEngine.Object) (autoAimTarget2 as Component)) && autoAimTarget2.IsValid)
                            {
                                Vector2 aimCenter = autoAimTarget2.AimCenter;
                                if (GameManager.Instance.MainCameraController.PointIsVisible(aimCenter, 0.05f))
                                {
                                    float num6 = Vector2.Distance(unitCenter, aimCenter) / num2;
                                    Vector2 vector2_2 = aimCenter + autoAimTarget2.Velocity * num6;
                                    float num7 = Mathf.Abs(BraveMathCollege.ClampAngle180((vector2_2 - unitCenter).ToAngle() - angle));
                                    if (flag1 && this.SuperAutoAimTarget == autoAimTarget2)
                                    {
                                        num7 *= BraveInput.ControllerAutoAimDegrees / BraveInput.ControllerSuperAutoAimDegrees;
                                        if (flag8)
                                            num7 *= 3f;
                                    }
                                    else if (flag8)
                                        num7 *= 2f;
                                    if (flag5)
                                    {
                                        Vector2 vector2_3 = vector2_2 - unitCenter;
                                        float num8 = !(vector2_3 == Vector2.zero) ? vector2_3.magnitude : 0.0f;
                                        if (!autoAimTarget2.IgnoreForSuperDuperAutoAim && (double) num8 >= (double) autoAimTarget2.MinDistForSuperDuperAutoAim && (autoAimTarget1 == null || (double) num8 < (double) num4) && ((double) this.m_superDuperAutoAimTimer <= 0.0 || autoAimTarget2 == this.SuperDuperAimTarget))
                                        {
                                            RaycastResult result;
                                            if (!PhysicsEngine.Instance.Raycast(unitCenter, vector2_3.normalized, vector2_3.magnitude - 2f, out result, collideWithRigidbodies: false))
                                            {
                                                aimPointInWorld = (Vector3) vector2_2;
                                                flag6 = true;
                                                this.SuperDuperAimPoint = (Vector2) aimPointInWorld;
                                                autoAimTarget1 = autoAimTarget2;
                                                num4 = num8;
                                            }
                                            RaycastResult.Pool.Free(ref result);
                                        }
                                    }
                                    else if ((double) num7 < (double) BraveInput.ControllerAutoAimDegrees && (autoAimTarget1 == null || (double) num7 < (double) num3))
                                    {
                                        Vector2 vector2_4 = vector2_2 - unitCenter;
                                        RaycastResult result;
                                        if (!PhysicsEngine.Instance.Raycast(unitCenter, vector2_4.normalized, vector2_4.magnitude - 2f, out result, collideWithRigidbodies: false))
                                        {
                                            aimPointInWorld = (Vector3) vector2_2;
                                            flag6 = true;
                                            autoAimTarget1 = autoAimTarget2;
                                            num3 = num7;
                                        }
                                        RaycastResult.Pool.Free(ref result);
                                    }
                                }
                            }
                        }
                        if (flag5)
                        {
                            if (!flag6 && (double) this.m_superDuperAutoAimTimer > 0.0)
                                aimPointInWorld = (Vector3) this.SuperDuperAimPoint;
                            if (autoAimTarget1 != this.SuperDuperAimTarget)
                            {
                                this.SuperDuperAimTarget = autoAimTarget1;
                                this.m_superDuperAutoAimTimer = 0.5f;
                            }
                            else if (autoAimTarget1 == null && (double) this.m_superDuperAutoAimTimer <= 0.0)
                                this.SuperDuperAimTarget = (IAutoAimTarget) null;
                        }
                        if (flag1)
                        {
                            if (this.SuperAutoAimTarget != null && this.SuperAutoAimTarget != autoAimTarget1)
                                this.SuperAutoAimTarget = (IAutoAimTarget) null;
                            else if (this.SuperAutoAimTarget == null && autoAimTarget1 != null && flag2)
                                this.SuperAutoAimTarget = autoAimTarget1;
                        }
                    }
                }
            }
            this.m_cachedAimDirection = aimPointInWorld - position;
            return aimPointInWorld;
        }

        public void ForceStopDodgeRoll()
        {
            this.m_handlingQueuedAnimation = false;
            this.m_dodgeRollTimer = this.rollStats.GetModifiedTime(this);
            this.ClearDodgeRollState();
            this.previousMineCart = (MineCartController) null;
        }

        protected virtual bool CanDodgeRollWhileFlying
        {
            get
            {
                if (this.AdditionalCanDodgeRollWhileFlying.Value)
                    return true;
                return PassiveItem.ActiveFlagItems.ContainsKey(this) && PassiveItem.ActiveFlagItems[this].ContainsKey(typeof (WingsItem));
            }
        }

        public virtual bool DodgeRollIsBlink
        {
            get
            {
                return (!(bool) (UnityEngine.Object) GameManager.Instance.Dungeon || !GameManager.Instance.Dungeon.IsEndTimes) && PassiveItem.ActiveFlagItems.ContainsKey(this) && PassiveItem.ActiveFlagItems[this].ContainsKey(typeof (BlinkPassiveItem));
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleBlinkDodgeRoll()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleBlinkDodgeRollc__Iterator16()
            {
                _this = this
            };
        }

        private bool CheckDodgeRollDepth()
        {
            if (this.IsSlidingOverSurface && !this.DodgeRollIsBlink)
                return !this.CurrentRoom.IsShop && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.TUTORIAL;
            bool flag = PassiveItem.IsFlagSetForCharacter(this, typeof (PegasusBootsItem));
            int num = !flag ? 1 : 2;
            if (flag && this.HasActiveBonusSynergy(CustomSynergyType.TRIPLE_JUMP))
                ++num;
            if (this.DodgeRollIsBlink)
                num = 1;
            return !this.IsDodgeRolling || this.m_currentDodgeRollDepth < num;
        }

        private bool StartDodgeRoll(Vector2 direction)
        {
            if (direction == Vector2.zero || !this.CheckDodgeRollDepth() || this.IsFlying && !this.CanDodgeRollWhileFlying)
                return false;
            if (this.OnPreDodgeRoll != null)
                this.OnPreDodgeRoll(this);
            if (this.IsStationary)
                return false;
            if ((bool) (UnityEngine.Object) this.knockbackDoer)
                this.knockbackDoer.ClearContinuousKnockbacks();
            this.lockedDodgeRollDirection = direction;
            this.m_rollDamagedEnemies.Clear();
            this.spriteAnimator.Stop();
            this.m_dodgeRollTimer = 0.0f;
            this.m_dodgeRollState = !this.rollStats.hasPreDodgeDelay ? PlayerController.DodgeRollState.InAir : PlayerController.DodgeRollState.PreRollDelay;
            ++this.m_currentDodgeRollDepth;
            if (this.OnRollStarted != null)
                this.OnRollStarted(this, this.lockedDodgeRollDirection);
            if (this.DodgeRollIsBlink)
            {
                this.IsEthereal = true;
                this.IsVisible = false;
                this.PlayDodgeRollAnimation(direction);
            }
            else
            {
                this.PlayDodgeRollAnimation(direction);
                if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                    this.CurrentGun.HandleDodgeroll(this.rollStats.GetModifiedTime(this));
                if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null || string.IsNullOrEmpty(this.CurrentGun.dodgeAnimation))
                    this.ToggleGunRenderers(false, "dodgeroll");
                this.ToggleHandRenderers(false, "dodgeroll");
            }
            if ((double) this.CurrentFireMeterValue > 0.0)
            {
                this.CurrentFireMeterValue = Mathf.Max(0.0f, this.CurrentFireMeterValue -= 0.5f);
                if ((double) this.CurrentFireMeterValue == 0.0)
                    this.IsOnFire = false;
            }
            return true;
        }

        public bool ForceStartDodgeRoll(Vector2 vec) => this.StartDodgeRoll(vec);

        public bool ForceStartDodgeRoll()
        {
            return this.StartDodgeRoll(this.AdjustInputVector(this.m_activeActions.Move.Vector, BraveInput.MagnetAngles.movementCardinal, BraveInput.MagnetAngles.movementOrdinal));
        }

        protected bool CanBlinkToPoint(Vector2 point, Vector2 centerOffset)
        {
            bool point1 = this.IsValidPlayerPosition(point + centerOffset);
            if (point1 && this.CurrentRoom != null)
            {
                CellData cellData = GameManager.Instance.Dungeon.data[point.ToIntVector2(VectorConversions.Floor)];
                if (cellData == null)
                    return false;
                RoomHandler nearestRoom = cellData.nearestRoom;
                if (cellData.type != CellType.FLOOR)
                    point1 = false;
                if (this.CurrentRoom.IsSealed && nearestRoom != this.CurrentRoom)
                    point1 = false;
                if (this.CurrentRoom.IsSealed && cellData.isExitCell)
                    point1 = false;
                if (nearestRoom.visibility == RoomHandler.VisibilityStatus.OBSCURED || nearestRoom.visibility == RoomHandler.VisibilityStatus.REOBSCURED)
                    point1 = false;
            }
            if (this.CurrentRoom == null)
                point1 = false;
            return point1;
        }

        protected void UpdateBlinkShadow(Vector2 delta, bool canWarpDirectly)
        {
            if ((UnityEngine.Object) this.m_extantBlinkShadow == (UnityEngine.Object) null)
            {
                this.m_extantBlinkShadow = tk2dSprite.AddComponent(new GameObject("blinkshadow"), this.sprite.Collection, this.sprite.spriteId);
                this.m_extantBlinkShadow.transform.position = (Vector3) (this.m_cachedBlinkPosition + (this.sprite.transform.position.XY() - this.specRigidbody.UnitCenter));
                this.m_extantBlinkShadow.gameObject.AddComponent<tk2dSpriteAnimator>().Library = this.spriteAnimator.Library;
                this.m_extantBlinkShadow.renderer.material.SetColor(this.m_overrideFlatColorID, !canWarpDirectly ? new Color(0.4f, 0.0f, 0.0f, 1f) : new Color(0.25f, 0.25f, 0.25f, 1f));
                this.m_extantBlinkShadow.usesOverrideMaterial = true;
                this.m_extantBlinkShadow.FlipX = this.sprite.FlipX;
                this.m_extantBlinkShadow.FlipY = this.sprite.FlipY;
                if (this.OnBlinkShadowCreated != null)
                    this.OnBlinkShadowCreated(this.m_extantBlinkShadow);
            }
            else
            {
                if (delta == Vector2.zero)
                {
                    this.m_extantBlinkShadow.spriteAnimator.Stop();
                    this.m_extantBlinkShadow.SetSprite(this.sprite.Collection, this.sprite.spriteId);
                }
                else
                {
                    string baseAnimationName = this.GetBaseAnimationName(delta, this.m_currentGunAngle, forceTwoHands: true);
                    if (!string.IsNullOrEmpty(baseAnimationName) && !this.m_extantBlinkShadow.spriteAnimator.IsPlaying(baseAnimationName))
                        this.m_extantBlinkShadow.spriteAnimator.Play(baseAnimationName);
                }
                this.m_extantBlinkShadow.renderer.material.SetColor(this.m_overrideFlatColorID, !canWarpDirectly ? new Color(0.4f, 0.0f, 0.0f, 1f) : new Color(0.25f, 0.25f, 0.25f, 1f));
                this.m_extantBlinkShadow.transform.position = (Vector3) (this.m_cachedBlinkPosition + (this.sprite.transform.position.XY() - this.specRigidbody.UnitCenter));
            }
            this.m_extantBlinkShadow.FlipX = this.sprite.FlipX;
            this.m_extantBlinkShadow.FlipY = this.sprite.FlipY;
        }

        protected void ClearBlinkShadow()
        {
            if (!(bool) (UnityEngine.Object) this.m_extantBlinkShadow)
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantBlinkShadow.gameObject);
            this.m_extantBlinkShadow = (tk2dSprite) null;
        }

        protected bool HandleStartDodgeRoll(Vector2 direction)
        {
            this.m_handleDodgeRollStartThisFrame = true;
            if (this.WasPausedThisFrame || !this.CheckDodgeRollDepth() || this.m_dodgeRollState == PlayerController.DodgeRollState.AdditionalDelay || !this.DodgeRollIsBlink && direction == Vector2.zero)
                return false;
            this.rollStats.blinkDistanceMultiplier = 1f;
            if (this.IsFlying && !this.CanDodgeRollWhileFlying)
                return false;
            bool flag1 = false;
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.PlayerIDX);
            if (this.DodgeRollIsBlink)
            {
                bool flag2 = false;
                if (instanceForPlayer.GetButtonDown(GungeonActions.GungeonActionType.DodgeRoll))
                {
                    flag2 = true;
                    this.healthHaver.TriggerInvulnerabilityPeriod(1f / 1000f);
                    instanceForPlayer.ConsumeButtonDown(GungeonActions.GungeonActionType.DodgeRoll);
                }
                if (instanceForPlayer.ActiveActions.DodgeRollAction.IsPressed)
                {
                    this.m_timeHeldBlinkButton += BraveTime.DeltaTime;
                    if ((double) this.m_timeHeldBlinkButton < 0.20000000298023224)
                    {
                        this.m_cachedBlinkPosition = this.specRigidbody.UnitCenter;
                    }
                    else
                    {
                        Vector2 cachedBlinkPosition = this.m_cachedBlinkPosition;
                        if (this.IsKeyboardAndMouse())
                            this.m_cachedBlinkPosition = this.unadjustedAimPoint.XY() - (this.CenterPosition - this.specRigidbody.UnitCenter);
                        else
                            this.m_cachedBlinkPosition += this.m_activeActions.Aim.Vector.normalized * BraveTime.DeltaTime * 15f;
                        this.m_cachedBlinkPosition = BraveMathCollege.ClampToBounds(this.m_cachedBlinkPosition, GameManager.Instance.MainCameraController.MinVisiblePoint, GameManager.Instance.MainCameraController.MaxVisiblePoint);
                        this.UpdateBlinkShadow(this.m_cachedBlinkPosition - cachedBlinkPosition, this.CanBlinkToPoint(this.m_cachedBlinkPosition, this.transform.position.XY() - this.specRigidbody.UnitCenter));
                    }
                }
                else if (instanceForPlayer.ActiveActions.DodgeRollAction.WasReleased || flag2)
                {
                    if (direction != Vector2.zero || (double) this.m_timeHeldBlinkButton >= 0.20000000298023224)
                        flag1 = true;
                }
                else
                    this.m_timeHeldBlinkButton = 0.0f;
            }
            else if (instanceForPlayer.GetButtonDown(GungeonActions.GungeonActionType.DodgeRoll))
            {
                instanceForPlayer.ConsumeButtonDown(GungeonActions.GungeonActionType.DodgeRoll);
                flag1 = true;
            }
            if (flag1)
            {
                this.DidUnstealthyAction();
                if (GameManager.Instance.InTutorial)
                    GameManager.BroadcastRoomTalkDoerFsmEvent("playerDodgeRoll");
                if (!this.DodgeRollIsBlink)
                    return this.StartDodgeRoll(direction);
                if ((double) this.m_timeHeldBlinkButton < 0.20000000298023224)
                    this.m_cachedBlinkPosition = this.specRigidbody.UnitCenter + direction.normalized * this.rollStats.GetModifiedDistance(this);
                this.BlinkToPoint(this.m_cachedBlinkPosition);
                this.m_timeHeldBlinkButton = 0.0f;
            }
            return false;
        }

        public void BlinkToPoint(Vector2 targetPoint)
        {
            this.m_cachedBlinkPosition = targetPoint;
            this.lockedDodgeRollDirection = (this.m_cachedBlinkPosition - this.specRigidbody.UnitCenter).normalized;
            Vector2 centerOffset = this.transform.position.XY() - this.specRigidbody.UnitCenter;
            if (this.CanBlinkToPoint(this.m_cachedBlinkPosition, centerOffset))
            {
                this.StartCoroutine(this.HandleBlinkDodgeRoll());
            }
            else
            {
                Vector2 vector2 = this.specRigidbody.UnitCenter - this.m_cachedBlinkPosition;
                float magnitude = vector2.magnitude;
                Vector2? nullable = new Vector2?();
                float num = 0.0f;
                vector2 = vector2.normalized;
                while ((double) magnitude > 0.0)
                {
                    ++num;
                    --magnitude;
                    Vector2 point = this.m_cachedBlinkPosition + vector2 * num;
                    if (this.CanBlinkToPoint(point, centerOffset))
                    {
                        nullable = new Vector2?(point);
                        break;
                    }
                }
                if (!nullable.HasValue)
                    return;
                if ((double) Vector2.Dot((nullable.Value - this.specRigidbody.UnitCenter).normalized, this.lockedDodgeRollDirection) > 0.0)
                {
                    this.m_cachedBlinkPosition = nullable.Value;
                    this.StartCoroutine(this.HandleBlinkDodgeRoll());
                }
                else
                    this.ClearBlinkShadow();
            }
        }

        public void DidUnstealthyAction()
        {
            if (this.OnDidUnstealthyAction != null)
                this.OnDidUnstealthyAction(this);
            if (!this.IsPetting || !(bool) (UnityEngine.Object) this.m_pettingTarget)
                return;
            this.m_pettingTarget.StopPet();
        }

        protected void ContinueDodgeRollAnimation()
        {
            Vector2 dodgeRollDirection = this.lockedDodgeRollDirection;
            dodgeRollDirection.Normalize();
            tk2dSpriteAnimationClip clip = (double) Mathf.Abs(dodgeRollDirection.x) >= 0.10000000149011612 ? this.spriteAnimator.GetClipByName(((double) dodgeRollDirection.y <= 0.10000000149011612 ? "dodge_left" : "dodge_left_bw") + (!this.UseArmorlessAnim ? string.Empty : "_armorless")) : this.spriteAnimator.GetClipByName(((double) dodgeRollDirection.y <= 0.10000000149011612 ? "dodge" : "dodge_bw") + (!this.UseArmorlessAnim ? string.Empty : "_armorless"));
            if (clip == null)
                return;
            float overrideFps = (float) clip.frames.Length / this.rollStats.GetModifiedTime(this);
            this.spriteAnimator.Play(clip, 0.0f, overrideFps);
            int currFrame = 0;
            for (int index = 0; index < clip.frames.Length; ++index)
            {
                if (clip.frames[index].groundedFrame)
                {
                    currFrame = index;
                    break;
                }
            }
            this.m_dodgeRollTimer = (float) currFrame / (float) clip.frames.Length * this.rollStats.GetModifiedTime(this);
            this.spriteAnimator.SetFrame(currFrame);
            this.m_handlingQueuedAnimation = true;
        }

        protected virtual void PlayDodgeRollAnimation(Vector2 direction)
        {
            tk2dSpriteAnimationClip clip = (tk2dSpriteAnimationClip) null;
            direction.Normalize();
            if (this.m_dodgeRollState != PlayerController.DodgeRollState.PreRollDelay)
            {
                clip = (double) Mathf.Abs(direction.x) >= 0.10000000149011612 ? this.spriteAnimator.GetClipByName(((double) direction.y <= 0.10000000149011612 ? "dodge_left" : "dodge_left_bw") + (!this.UseArmorlessAnim ? string.Empty : "_armorless")) : this.spriteAnimator.GetClipByName(((double) direction.y <= 0.10000000149011612 ? "dodge" : "dodge_bw") + (!this.UseArmorlessAnim ? string.Empty : "_armorless"));
                if (this.IsVisible)
                {
                    Vector2 velocity = new Vector2(direction.x, direction.y);
                    if ((double) Mathf.Abs(velocity.x) < 0.0099999997764825821)
                        velocity.x = 0.0f;
                    if ((double) Mathf.Abs(velocity.y) < 0.0099999997764825821)
                        velocity.y = 0.0f;
                    if ((UnityEngine.Object) this.CustomDodgeRollEffect != (UnityEngine.Object) null)
                        SpawnManager.SpawnVFX(this.CustomDodgeRollEffect, this.SpriteBottomCenter, Quaternion.identity);
                    else
                        GameManager.Instance.Dungeon.dungeonDustups.InstantiateDodgeDustup(velocity, this.SpriteBottomCenter);
                }
            }
            if (clip == null)
                return;
            float overrideFps = (float) clip.frames.Length / this.rollStats.GetModifiedTime(this);
            this.spriteAnimator.Play(clip, 0.0f, overrideFps);
            this.m_handlingQueuedAnimation = true;
        }

        public void HandleContinueDodgeRoll()
        {
            this.m_dodgeRollTimer += BraveTime.DeltaTime;
            if (GameManager.Instance.InTutorial && GameManager.Instance.Dungeon.CellIsPit((Vector3) this.specRigidbody.UnitCenter))
                GameManager.BroadcastRoomTalkDoerFsmEvent("playerDodgeRollOverPit");
            if (this.m_dodgeRollState == PlayerController.DodgeRollState.PreRollDelay)
            {
                if ((double) this.m_dodgeRollTimer <= (double) this.rollStats.preDodgeDelay)
                    return;
                this.m_dodgeRollState = PlayerController.DodgeRollState.InAir;
                this.PlayDodgeRollAnimation(this.lockedDodgeRollDirection);
                this.m_dodgeRollTimer = BraveTime.DeltaTime;
            }
            else if (this.m_dodgeRollState == PlayerController.DodgeRollState.InAir)
            {
                bool flag = false;
                if (this.IsSlidingOverSurface)
                {
                    if (this.m_hasFiredWhileSliding)
                        this.ToggleGunRenderers(true, "dodgeroll");
                    flag = true;
                    this.m_dodgeRollTimer -= BraveTime.DeltaTime;
                    string name = "slide_right";
                    if ((double) this.lockedDodgeRollDirection.y > 0.10000000149011612)
                        name = "slide_up";
                    if ((double) this.lockedDodgeRollDirection.y < -0.10000000149011612)
                        name = "slide_down";
                    if (this.UseArmorlessAnim)
                        name += "_armorless";
                    if (!this.spriteAnimator.IsPlaying(name) && this.spriteAnimator.GetClipByName(name) != null)
                        this.spriteAnimator.Play(name);
                    this.IsSlidingOverSurface = false;
                    List<SpeculativeRigidbody> overlappingRigidbodies = PhysicsEngine.Instance.GetOverlappingRigidbodies(this.specRigidbody);
                    for (int index = 0; index < overlappingRigidbodies.Count; ++index)
                    {
                        if ((double) this.specRigidbody.Velocity.magnitude < 1.0 && (bool) (UnityEngine.Object) overlappingRigidbodies[index].GetComponent<MajorBreakable>())
                            overlappingRigidbodies[index].GetComponent<MajorBreakable>().Break(Vector2.zero);
                        if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].GetComponent<SlideSurface>())
                        {
                            this.IsSlidingOverSurface = true;
                            break;
                        }
                    }
                }
                if (flag && this.IsSlidingOverSurface || (this.DodgeRollIsBlink || this.spriteAnimator.CurrentClip.name.Contains("dodge")) && !this.QueryGroundedFrame())
                    return;
                this.m_dodgeRollState = PlayerController.DodgeRollState.OnGround;
                this.DoVibration(Vibration.Time.Quick, Vibration.Strength.UltraLight);
                if (!flag)
                    return;
                this.m_hasFiredWhileSliding = false;
                this.TablesDamagedThisSlide.Clear();
                this.m_dodgeRollTimer = this.rollStats.GetModifiedTime(this);
                this.ToggleHandRenderers(true, "dodgeroll");
                this.ToggleGunRenderers(true, "dodgeroll");
                this.m_handlingQueuedAnimation = false;
            }
            else
            {
                if (this.m_dodgeRollState == PlayerController.DodgeRollState.OnGround || this.m_dodgeRollState != PlayerController.DodgeRollState.Blink)
                    return;
                float t = this.m_dodgeRollTimer / this.rollStats.GetModifiedTime(this);
                Vector2 vector2 = this.CenterPosition - this.specRigidbody.UnitCenter;
                if (this.IsPrimaryPlayer)
                    GameManager.Instance.MainCameraController.OverridePlayerOnePosition = Vector2.Lerp(this.specRigidbody.UnitCenter, this.m_cachedBlinkPosition, t) + vector2;
                else
                    GameManager.Instance.MainCameraController.OverridePlayerTwoPosition = Vector2.Lerp(this.specRigidbody.UnitCenter, this.m_cachedBlinkPosition, t) + vector2;
            }
        }

        private void ToggleOrbitals(bool value)
        {
            bool visible = value;
            for (int index = 0; index < this.orbitals.Count; ++index)
                this.orbitals[index].ToggleRenderer(visible);
            for (int index = 0; index < this.trailOrbitals.Count; ++index)
                this.trailOrbitals[index].ToggleRenderer(visible);
        }

        public void ToggleFollowerRenderers(bool value)
        {
            this.LastFollowerVisibilityState = value;
            if (this.orbitals != null)
            {
                for (int index = 0; index < this.orbitals.Count; ++index)
                    this.orbitals[index].ToggleRenderer(value);
            }
            if (this.trailOrbitals != null)
            {
                for (int index = 0; index < this.trailOrbitals.Count; ++index)
                    this.trailOrbitals[index].ToggleRenderer(value);
            }
            if (this.companions == null)
                return;
            for (int index = 0; index < this.companions.Count; ++index)
                this.companions[index].ToggleRenderers(value);
        }

        public void ToggleRenderer(bool value, string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
            {
                this.m_hideRenderers.ClearOverrides();
                if (!value)
                    this.m_hideRenderers.SetOverride("generic", true);
            }
            else
            {
                this.m_hideRenderers.RemoveOverride("generic");
                this.m_hideRenderers.SetOverride(reason, !value);
            }
            bool flag = !this.m_hideRenderers.Value;
            this.m_renderer.enabled = flag;
            this.ToggleAttachedRenderers(flag);
            if ((bool) (UnityEngine.Object) this.ShadowObject)
                this.ShadowObject.GetComponent<Renderer>().enabled = flag;
            SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, flag);
        }

        private void ToggleAttachedRenderers(bool value)
        {
            for (int index = 0; index < this.m_attachedSprites.Count; ++index)
                this.m_attachedSprites[index].renderer.enabled = value;
        }

        public void ToggleGunRenderers(bool value, string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
            {
                this.m_hideGunRenderers.ClearOverrides();
                if (!value)
                    this.m_hideGunRenderers.SetOverride("generic", true);
            }
            else
            {
                this.m_hideGunRenderers.RemoveOverride("generic");
                this.m_hideGunRenderers.SetOverride(reason, !value);
            }
            bool flag = !this.m_hideGunRenderers.Value;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES && !ArkController.IsResettingPlayers && value)
                flag = false;
            if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                this.CurrentGun.ToggleRenderers(flag);
            if (!((UnityEngine.Object) this.CurrentSecondaryGun != (UnityEngine.Object) null))
                return;
            this.CurrentSecondaryGun.ToggleRenderers(flag);
        }

        public void ToggleHandRenderers(bool value, string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
            {
                this.m_hideHandRenderers.ClearOverrides();
                if (!value)
                    this.m_hideHandRenderers.SetOverride("generic", true);
            }
            else
            {
                this.m_hideHandRenderers.RemoveOverride("generic");
                this.m_hideHandRenderers.SetOverride(reason, !value);
            }
            bool flag = !this.m_hideHandRenderers.Value;
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES && !ArkController.IsResettingPlayers && value)
                flag = false;
            this.primaryHand.ForceRenderersOff = !flag;
            this.secondaryHand.ForceRenderersOff = !flag;
            if (!(bool) (UnityEngine.Object) this.CurrentGun)
                return;
            if (this.CurrentGun.additionalHandState == AdditionalHandState.HideBoth)
            {
                this.primaryHand.ForceRenderersOff = true;
                this.secondaryHand.ForceRenderersOff = true;
            }
            else if (this.CurrentGun.additionalHandState == AdditionalHandState.HidePrimary)
            {
                this.primaryHand.ForceRenderersOff = true;
            }
            else
            {
                if (this.CurrentGun.additionalHandState != AdditionalHandState.HideSecondary)
                    return;
                this.secondaryHand.ForceRenderersOff = true;
            }
        }

        protected virtual void HandleFlipping(float gunAngle)
        {
            bool flag = false;
            if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null)
                gunAngle = BraveMathCollege.Atan2Degrees(!(this.m_playerCommandedDirection == Vector2.zero) ? this.m_playerCommandedDirection : this.m_lastNonzeroCommandedDirection);
            if (this.IsGhost)
                gunAngle = 0.0f;
            if (!this.IsSlidingOverSurface)
            {
                if (this.IsDodgeRolling)
                {
                    if ((double) this.lockedDodgeRollDirection.x < -0.10000000149011612)
                        gunAngle = 180f;
                    else if ((double) this.lockedDodgeRollDirection.x > 0.10000000149011612)
                        gunAngle = 0.0f;
                }
                else if (this.IsPetting)
                    gunAngle = this.m_petDirection;
                else if (this.m_handlingQueuedAnimation && !this.m_overrideGunAngle.HasValue)
                    return;
            }
            float num1 = 75f;
            float num2 = 105f;
            if ((double) gunAngle <= 155.0 && (double) gunAngle >= 25.0)
            {
                num1 = 75f;
                num2 = 105f;
            }
            if (!this.SpriteFlipped && (double) Mathf.Abs(gunAngle) > (double) num2)
            {
                this.sprite.FlipX = true;
                this.sprite.gameObject.transform.localPosition = new Vector3(this.m_spriteDimensions.x, 0.0f, 0.0f);
                if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                    this.CurrentGun.HandleSpriteFlip(true);
                if ((UnityEngine.Object) this.CurrentSecondaryGun != (UnityEngine.Object) null)
                    this.CurrentSecondaryGun.HandleSpriteFlip(true);
                flag = true;
            }
            else if (this.SpriteFlipped && (double) Mathf.Abs(gunAngle) < (double) num1)
            {
                this.sprite.FlipX = false;
                this.sprite.gameObject.transform.localPosition = Vector3.zero;
                if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                    this.CurrentGun.HandleSpriteFlip(false);
                if ((UnityEngine.Object) this.CurrentSecondaryGun != (UnityEngine.Object) null)
                    this.CurrentSecondaryGun.HandleSpriteFlip(false);
                flag = true;
            }
            if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                this.HandleGunDepthInternal(this.CurrentGun, gunAngle);
            if ((UnityEngine.Object) this.CurrentSecondaryGun != (UnityEngine.Object) null)
                this.HandleGunDepthInternal(this.CurrentSecondaryGun, gunAngle, true);
            this.sprite.UpdateZDepth();
            if (!flag)
                return;
            this.ProcessHandAttachment();
        }

        private void HandleGunDepthInternal(Gun targetGun, float gunAngle, bool isSecondary = false)
        {
            tk2dBaseSprite sprite = targetGun.GetSprite();
            if (targetGun.preventRotation)
                sprite.HeightOffGround = 0.4f;
            else if (targetGun.usesDirectionalIdleAnimations)
            {
                float num = -0.075f;
                if ((double) gunAngle > 0.0 && (double) gunAngle <= 155.0 && (double) gunAngle >= 25.0 || (double) gunAngle <= -60.0 && (double) gunAngle >= -120.0)
                    num = 0.075f;
                sprite.HeightOffGround = num;
            }
            else if ((double) gunAngle > 0.0 && (double) gunAngle <= 155.0 && (double) gunAngle >= 25.0)
            {
                sprite.HeightOffGround = -0.075f;
            }
            else
            {
                float num = targetGun.Handedness != GunHandedness.TwoHanded ? -0.075f : 0.075f;
                if (isSecondary)
                    num = 0.075f;
                sprite.HeightOffGround = num;
            }
            sprite.UpdateZDepth();
        }

        private float GetDodgeRollSpeed()
        {
            if (this.m_dodgeRollState == PlayerController.DodgeRollState.PreRollDelay)
                return 0.0f;
            float time = Mathf.Clamp01((this.m_dodgeRollTimer - BraveTime.DeltaTime) / this.rollStats.GetModifiedTime(this));
            return (Mathf.Clamp01(this.rollStats.speed.Evaluate(Mathf.Clamp01(this.m_dodgeRollTimer / this.rollStats.GetModifiedTime(this)))) - Mathf.Clamp01(this.rollStats.speed.Evaluate(time))) * this.rollStats.GetModifiedDistance(this) / BraveTime.DeltaTime;
        }

        public void ProcessHandAttachment()
        {
            if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null)
            {
                this.primaryHand.attachPoint = (Transform) null;
                this.secondaryHand.attachPoint = (Transform) null;
            }
            else
            {
                if (this.inventory.DualWielding && (UnityEngine.Object) this.CurrentSecondaryGun != (UnityEngine.Object) null)
                {
                    this.primaryHand.attachPoint = !this.CurrentGun.IsPreppedForThrow ? this.CurrentGun.PrimaryHandAttachPoint : this.CurrentGun.ThrowPrepTransform;
                    this.secondaryHand.attachPoint = !this.CurrentSecondaryGun.IsPreppedForThrow ? this.CurrentSecondaryGun.PrimaryHandAttachPoint : this.CurrentSecondaryGun.ThrowPrepTransform;
                }
                else if (this.CurrentGun.Handedness == GunHandedness.NoHanded)
                {
                    this.primaryHand.attachPoint = (Transform) null;
                    this.secondaryHand.attachPoint = (Transform) null;
                }
                else
                {
                    this.primaryHand.attachPoint = this.CurrentGun.Handedness == GunHandedness.HiddenOneHanded ? (Transform) null : (!this.CurrentGun.IsPreppedForThrow ? this.CurrentGun.PrimaryHandAttachPoint : this.CurrentGun.ThrowPrepTransform);
                    this.secondaryHand.attachPoint = this.CurrentGun.Handedness != GunHandedness.TwoHanded ? (Transform) null : this.CurrentGun.SecondaryHandAttachPoint;
                }
                if (this.CurrentGun.additionalHandState == AdditionalHandState.None)
                    return;
                switch (this.CurrentGun.additionalHandState)
                {
                    case AdditionalHandState.HidePrimary:
                        if (!(bool) (UnityEngine.Object) this.primaryHand)
                            break;
                        this.primaryHand.attachPoint = (Transform) null;
                        break;
                    case AdditionalHandState.HideSecondary:
                        if (!(bool) (UnityEngine.Object) this.secondaryHand)
                            break;
                        this.secondaryHand.attachPoint = (Transform) null;
                        break;
                    case AdditionalHandState.HideBoth:
                        if ((bool) (UnityEngine.Object) this.primaryHand)
                            this.primaryHand.attachPoint = (Transform) null;
                        if (!(bool) (UnityEngine.Object) this.secondaryHand)
                            break;
                        this.secondaryHand.attachPoint = (Transform) null;
                        break;
                }
            }
        }

        private void HandleGunUnequipInternal(Gun previous)
        {
            if (!((UnityEngine.Object) previous != (UnityEngine.Object) null))
                return;
            tk2dBaseSprite sprite = previous.GetSprite();
            this.sprite.DetachRenderer(sprite);
            sprite.DetachRenderer(this.primaryHand.sprite);
            sprite.DetachRenderer(this.secondaryHand.sprite);
            SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) previous.GetComponent<tk2dSprite>());
        }

        private void HandleGunEquipInternal(Gun current, PlayerHandController hand)
        {
            if (!((UnityEngine.Object) current != (UnityEngine.Object) null))
                return;
            tk2dBaseSprite sprite = current.GetSprite();
            this.sprite.AttachRenderer(sprite);
            sprite.AttachRenderer(hand.sprite);
            if (!this.inventory.DualWielding && (!this.RenderBodyHand || current.IsTrickGun))
                sprite.AttachRenderer(this.secondaryHand.sprite);
            if (!current.PreventOutlines)
                SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) current.GetComponent<tk2dSprite>(), this.outlineColor, 0.2f, 0.05f);
            current.ToggleRenderers(!this.m_hideGunRenderers.Value);
        }

        private void OnGunChanged(
            Gun previous,
            Gun current,
            Gun previousSecondary,
            Gun currentSecondary,
            bool newGun)
        {
            this.HandleGunUnequipInternal(previous);
            this.HandleGunUnequipInternal(previousSecondary);
            this.HandleGunEquipInternal(current, this.primaryHand);
            this.HandleGunEquipInternal(currentSecondary, this.secondaryHand);
            this.HandleGunAttachPoint();
            this.ProcessHandAttachment();
            this.stats.RecalculateStats(this);
            if ((bool) (UnityEngine.Object) current && current.ammo > current.AdjustedMaxAmmo && !(bool) (UnityEngine.Object) current.GetComponent<ArtfulDodgerGunController>())
                current.ammo = current.AdjustedMaxAmmo;
            if (this.GunChanged == null)
                return;
            this.GunChanged(previous, current, newGun);
        }

        protected Vector2 AdjustInputVector(
            Vector2 rawInput,
            float cardinalMagnetAngle,
            float ordinalMagnetAngle)
        {
            float num1 = BraveMathCollege.ClampAngle360(BraveMathCollege.Atan2Degrees(rawInput));
            float num2 = num1 % 90f;
            float num3 = (float) (((double) num1 + 45.0) % 90.0);
            float num4 = 0.0f;
            if ((double) cardinalMagnetAngle > 0.0)
            {
                if ((double) num2 < (double) cardinalMagnetAngle)
                    num4 = -num2;
                else if ((double) num2 > 90.0 - (double) cardinalMagnetAngle)
                    num4 = 90f - num2;
            }
            if ((double) ordinalMagnetAngle > 0.0)
            {
                if ((double) num3 < (double) ordinalMagnetAngle)
                    num4 = -num3;
                else if ((double) num3 > 90.0 - (double) ordinalMagnetAngle)
                    num4 = 90f - num3;
            }
            return (Quaternion.Euler(0.0f, 0.0f, num1 + num4) * Vector3.right).XY() * rawInput.magnitude;
        }

        protected void ProcessDebugInput()
        {
        }

        public void ForceMoveToPoint(Vector2 targetPosition, float initialDelay = 0.0f, float maximumTime = 2f)
        {
            this.StartCoroutine(this.HandleForcedMove(targetPosition, false, false, initialDelay, maximumTime));
        }

        public void ForceMoveInDirectionUntilThreshold(
            Vector2 direction,
            float axialThreshold,
            float initialDelay = 0.0f,
            float maximumTime = 1f,
            List<SpeculativeRigidbody> passThroughRigidbodies = null)
        {
            Vector2 centerPosition = this.CenterPosition;
            bool axialX = false;
            bool axialY = false;
            if (!Mathf.Approximately(direction.x, 0.0f))
            {
                centerPosition.x = axialThreshold;
                axialX = true;
            }
            if (!Mathf.Approximately(direction.y, 0.0f))
            {
                centerPosition.y = axialThreshold;
                axialY = true;
            }
            this.StartCoroutine(this.HandleForcedMove(centerPosition, axialX, axialY, initialDelay, maximumTime, passThroughRigidbodies));
        }

        [DebuggerHidden]
        private IEnumerator HandleForcedMove(
            Vector2 targetPoint,
            bool axialX,
            bool axialY,
            float initialDelay = 0.0f,
            float maximumTime = 1f,
            List<SpeculativeRigidbody> passThroughRigidbodies = null)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PlayerController__HandleForcedMovec__Iterator17()
            {
                initialDelay = initialDelay,
                passThroughRigidbodies = passThroughRigidbodies,
                targetPoint = targetPoint,
                axialX = axialX,
                axialY = axialY,
                maximumTime = maximumTime,
                _this = this
            };
        }

        public bool IsQuickEquipGun(Gun gunToCheck)
        {
            return (UnityEngine.Object) gunToCheck == (UnityEngine.Object) this.m_cachedQuickEquipGun || (UnityEngine.Object) gunToCheck == (UnityEngine.Object) this.CurrentGun;
        }

        public void DoQuickEquip()
        {
            if (GameManager.Options.QuickSelectEnabled)
            {
                if ((UnityEngine.Object) this.m_cachedQuickEquipGun != (UnityEngine.Object) null && this.inventory.AllGuns.Contains(this.m_cachedQuickEquipGun) && (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) this.m_cachedQuickEquipGun)
                {
                    Gun cachedQuickEquipGun = this.m_cachedQuickEquipGun;
                    this.CacheQuickEquipGun();
                    this.ChangeGun(this.inventory.AllGuns.IndexOf(cachedQuickEquipGun) - this.inventory.AllGuns.IndexOf(this.CurrentGun));
                    this.m_equippedGunShift = -1;
                }
                else if ((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) this.m_cachedQuickEquipGun && (UnityEngine.Object) this.m_backupCachedQuickEquipGun != (UnityEngine.Object) null && this.inventory.AllGuns.Contains(this.m_backupCachedQuickEquipGun) && (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) this.m_backupCachedQuickEquipGun)
                {
                    Gun cachedQuickEquipGun = this.m_backupCachedQuickEquipGun;
                    this.CacheQuickEquipGun();
                    this.ChangeGun(this.inventory.AllGuns.IndexOf(cachedQuickEquipGun) - this.inventory.AllGuns.IndexOf(this.CurrentGun));
                    this.m_equippedGunShift = -1;
                }
                else
                    this.ChangeGun(-1);
            }
            else
                this.ChangeGun(-1);
        }

        protected virtual Vector2 HandlePlayerInput()
        {
            this.exceptionTracker = 0;
            if (this.m_activeActions == null)
                return Vector2.zero;
            Vector2 direction = Vector2.zero;
            if (this.CurrentInputState != PlayerInputState.NoMovement)
                direction = this.AdjustInputVector(this.m_activeActions.Move.Vector, BraveInput.MagnetAngles.movementCardinal, BraveInput.MagnetAngles.movementOrdinal);
            if ((double) direction.magnitude > 1.0)
                direction.Normalize();
            this.HandleStartDodgeRoll(direction);
            CollisionData result = (CollisionData) null;
            if ((double) direction.x > 0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Right, out result, collideWithRigidbodies: false))
                direction.x = 0.0f;
            CollisionData.Pool.Free(ref result);
            if ((double) direction.x < -0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Left, out result, collideWithRigidbodies: false))
                direction.x = 0.0f;
            CollisionData.Pool.Free(ref result);
            if ((double) direction.y > 0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Up, out result, collideWithRigidbodies: false))
                direction.y = 0.0f;
            CollisionData.Pool.Free(ref result);
            if ((double) direction.y < -0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Down, out result, collideWithRigidbodies: false))
                direction.y = 0.0f;
            CollisionData.Pool.Free(ref result);
            if (this.IsGhost)
            {
                bool flag1 = (!this.IsPrimaryPlayer ? (int) GameManager.Options.additionalBlankControlTwo : (int) GameManager.Options.additionalBlankControl) == 1 && this.m_activeActions.CheckBothSticksButton();
                if ((double) UnityEngine.Time.timeScale > 0.0)
                {
                    bool flag2 = false;
                    if (this.m_activeActions.Device != null)
                        flag2 = ((flag2 ? 1 : 0) | (this.m_activeActions.Device.Action1.WasPressed || this.m_activeActions.Device.Action2.WasPressed || this.m_activeActions.Device.Action3.WasPressed || this.m_activeActions.Device.Action4.WasPressed ? 1 : (this.m_activeActions.MenuSelectAction.WasPressed ? 1 : 0))) != 0;
                    if (this.IsKeyboardAndMouse() && Input.GetMouseButtonDown(0))
                        flag2 = true;
                    if ((double) this.m_blankCooldownTimer <= 0.0 && (flag2 || this.m_activeActions.ShootAction.WasPressed || this.m_activeActions.UseItemAction.WasPressed || this.m_activeActions.BlankAction.WasPressed || flag1))
                        this.DoGhostBlank();
                }
                return direction;
            }
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.PlayerIDX);
            if (this.AcceptingNonMotionInput)
            {
                if (this.IsKeyboardAndMouse() && !GameManager.Options.DisableQuickGunKeys)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                        this.ChangeToGunSlot(0);
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                        this.ChangeToGunSlot(1);
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                        this.ChangeToGunSlot(2);
                    if (Input.GetKeyDown(KeyCode.Alpha4))
                        this.ChangeToGunSlot(3);
                    if (Input.GetKeyDown(KeyCode.Alpha5))
                        this.ChangeToGunSlot(4);
                    if (Input.GetKeyDown(KeyCode.Alpha6))
                        this.ChangeToGunSlot(5);
                    if (Input.GetKeyDown(KeyCode.Alpha7))
                        this.ChangeToGunSlot(6);
                    if (Input.GetKeyDown(KeyCode.Alpha8))
                        this.ChangeToGunSlot(7);
                    if (Input.GetKeyDown(KeyCode.Alpha9))
                        this.ChangeToGunSlot(8);
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                        this.ChangeToGunSlot(9);
                }
                this.m_equippedGunShift = 0;
                if (!this.m_gunWasDropped && !GameUIRoot.Instance.MetalGearActive && !Minimap.Instance.IsFullscreen)
                {
                    if (this.m_activeActions.GunDownAction.WasReleased)
                    {
                        if (!this.m_gunChangePressedWhileMetalGeared)
                            this.ChangeGun(1);
                        this.m_gunChangePressedWhileMetalGeared = false;
                    }
                    if (this.m_activeActions.GunUpAction.WasReleased)
                    {
                        if (!this.m_gunChangePressedWhileMetalGeared)
                            this.ChangeGun(-1);
                        this.m_gunChangePressedWhileMetalGeared = false;
                    }
                    if (this.inventory.DualWielding && this.m_activeActions.SwapDualGunsAction.WasPressed)
                        this.inventory.SwapDualGuns();
                    if (this.m_activeActions.GunQuickEquipAction.WasReleased)
                        this.DoQuickEquip();
                }
                if ((this.m_activeActions.GunQuickEquipAction.IsPressed || this.ForceMetalGearMenu) && !GameManager.IsBossIntro && !Minimap.Instance.IsFullscreen && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES)
                {
                    ++this.m_metalGearFrames;
                    this.m_metalGearTimer += GameManager.INVARIANT_DELTA_TIME;
                    if ((double) this.m_metalGearTimer > 0.17499999701976776 && !this.m_metalWasGeared)
                    {
                        this.m_metalWasGeared = true;
                        this.m_metalGearTimer = 0.0f;
                        this.m_metalGearFrames = 0;
                        GameUIRoot.Instance.TriggerMetalGearGunSelect(this);
                    }
                }
                else
                {
                    this.m_metalWasGeared = false;
                    this.m_metalGearTimer = 0.0f;
                    this.m_metalGearFrames = 0;
                }
                if (this.m_activeActions.DropGunAction.IsPressed && (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null && this.inventory.AllGuns.Count > 1 && !this.inventory.GunLocked.Value && this.CurrentGun.CanActuallyBeDropped(this) && !this.m_gunWasDropped)
                {
                    this.m_dropGunTimer += BraveTime.DeltaTime;
                    if ((double) this.m_dropGunTimer > 0.5)
                    {
                        this.m_gunWasDropped = true;
                        this.m_dropGunTimer = 0.0f;
                        this.ForceDropGun(this.CurrentGun);
                        this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
                    }
                }
                else if (!this.m_activeActions.DropGunAction.IsPressed)
                {
                    this.m_gunWasDropped = false;
                    this.m_dropGunTimer = 0.0f;
                }
                if (!this.m_itemWasDropped)
                {
                    if (this.m_activeActions.ItemUpAction.WasReleased)
                        this.ChangeItem(1);
                    else if (this.m_activeActions.ItemDownAction.WasReleased)
                        this.ChangeItem(-1);
                }
                if (this.m_activeActions.DropItemAction.IsPressed && (UnityEngine.Object) this.CurrentItem != (UnityEngine.Object) null && this.CurrentItem.CanActuallyBeDropped(this) && !this.m_itemWasDropped && !this.m_preventItemSwitching)
                {
                    this.m_dropItemTimer += BraveTime.DeltaTime;
                    if ((double) this.m_dropItemTimer > 0.5)
                    {
                        this.m_itemWasDropped = true;
                        this.m_dropItemTimer = 0.0f;
                        this.DropActiveItem(this.CurrentItem);
                        this.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
                    }
                }
                else if (!this.m_activeActions.DropItemAction.IsPressed)
                {
                    this.m_itemWasDropped = false;
                    this.m_dropItemTimer = 0.0f;
                }
                if (this.m_activeActions.ReloadAction.WasPressed && (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                {
                    this.CurrentGun.Reload();
                    if (this.CurrentGun.OnReloadPressed != null)
                        this.CurrentGun.OnReloadPressed(this, this.CurrentGun, true);
                    if ((bool) (UnityEngine.Object) this.CurrentSecondaryGun)
                    {
                        this.CurrentSecondaryGun.Reload();
                        if (this.CurrentSecondaryGun.OnReloadPressed != null)
                            this.CurrentSecondaryGun.OnReloadPressed(this, this.CurrentSecondaryGun, true);
                    }
                    if (this.OnReloadPressed != null)
                        this.OnReloadPressed(this, this.CurrentGun);
                }
                bool buttonDown = instanceForPlayer.GetButtonDown(GungeonActions.GungeonActionType.UseItem);
                bool flag3 = true;
                if (buttonDown && (!this.IsDodgeRolling || (bool) (UnityEngine.Object) this.CurrentItem && this.CurrentItem.usableDuringDodgeRoll))
                {
                    this.UseItem();
                    if (flag3)
                        instanceForPlayer.ConsumeButtonDown(GungeonActions.GungeonActionType.UseItem);
                }
                bool flag4 = (!this.IsPrimaryPlayer ? (int) GameManager.Options.additionalBlankControlTwo : (int) GameManager.Options.additionalBlankControl) == 1 && this.m_activeActions.CheckBothSticksButton();
                if ((double) UnityEngine.Time.timeScale > 0.0 && (double) this.m_blankCooldownTimer <= 0.0 && (this.m_activeActions.BlankAction.WasPressed || flag4))
                    this.DoConsumableBlank();
                if ((UnityEngine.Object) Minimap.Instance != (UnityEngine.Object) null && !GameUIRoot.Instance.MetalGearActive)
                {
                    bool wasPressed = this.m_activeActions.MapAction.WasPressed;
                    bool holdOpen = false;
                    if (wasPressed)
                        Minimap.Instance.ToggleMinimap(true, holdOpen);
                }
            }
            if (this.CurrentInputState == PlayerInputState.AllInput || this.CurrentInputState == PlayerInputState.FoyerInputOnly)
            {
                IPlayerInteractable playerInteractable = (IPlayerInteractable) null;
                if (this.m_currentRoom != null)
                    playerInteractable = this.m_currentRoom.GetNearestInteractable(this.CenterPosition, 1f, this);
                if (playerInteractable != this.m_lastInteractionTarget || this.ForceRefreshInteractable)
                {
                    this.exceptionTracker = 100;
                    if (this.m_lastInteractionTarget is MonoBehaviour && !(bool) (UnityEngine.Object) (this.m_lastInteractionTarget as MonoBehaviour))
                        this.m_lastInteractionTarget = (IPlayerInteractable) null;
                    this.exceptionTracker = 101;
                    if (this.m_lastInteractionTarget != null)
                    {
                        this.m_lastInteractionTarget.OnExitRange(this);
                        this.exceptionTracker = 102;
                        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && this.m_lastInteractionTarget != null)
                        {
                            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                            {
                                this.exceptionTracker = 103;
                                PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                                if ((bool) (UnityEngine.Object) allPlayer && !((UnityEngine.Object) allPlayer == (UnityEngine.Object) this) && !allPlayer.healthHaver.IsDead && allPlayer.CurrentRoom != null)
                                {
                                    this.exceptionTracker = 104;
                                    if (this.m_lastInteractionTarget == allPlayer.CurrentRoom.GetNearestInteractable(allPlayer.CenterPosition, 1f, allPlayer))
                                        this.m_lastInteractionTarget.OnEnteredRange(allPlayer);
                                    this.exceptionTracker = 105;
                                }
                            }
                        }
                    }
                    playerInteractable?.OnEnteredRange(this);
                    this.m_lastInteractionTarget = playerInteractable;
                }
                if (playerInteractable != null && this.m_activeActions.InteractAction.WasPressed)
                {
                    if (this.IsDodgeRolling)
                    {
                        this.ToggleGunRenderers(true, "dodgeroll");
                        this.ToggleHandRenderers(true, "dodgeroll");
                    }
                    GameUIRoot.Instance.levelNameUI.BanishLevelNameText();
                    bool shouldBeFlipped;
                    string str1 = playerInteractable.GetAnimationState(this, out shouldBeFlipped);
                    playerInteractable.Interact(this);
                    if (this.IsSlidingOverSurface)
                        str1 = string.Empty;
                    if (!(playerInteractable is ShopItemController))
                        this.DidUnstealthyAction();
                    if (str1 != string.Empty)
                    {
                        this.HandleFlipping(!shouldBeFlipped ? 0.0f : 180f);
                        this.m_handlingQueuedAnimation = true;
                        string str2 = !((UnityEngine.Object) this.CurrentGun == (UnityEngine.Object) null) || this.ForceHandless ? "_hand" : "_twohands";
                        string str3 = !this.UseArmorlessAnim ? string.Empty : "_armorless";
                        if (this.RenderBodyHand && this.spriteAnimator.GetClipByName(str1 + str2 + str3) != null)
                            this.spriteAnimator.Play(str1 + str2 + str3);
                        else if (this.spriteAnimator.GetClipByName(str1 + str3) != null)
                            this.spriteAnimator.Play(str1 + str3);
                        this.m_overrideGunAngle = new float?(!shouldBeFlipped ? 0.0f : 180f);
                    }
                }
                else if (playerInteractable == null && this.m_activeActions.InteractAction.WasPressed && !this.IsPetting && !this.IsInCombat && !this.IsDodgeRolling && !this.m_handlingQueuedAnimation)
                {
                    List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
                    for (int index = 0; index < allEnemies.Count; ++index)
                    {
                        AIActor aiActor = allEnemies[index];
                        if ((bool) (UnityEngine.Object) aiActor && !aiActor.IsNormalEnemy && (bool) (UnityEngine.Object) aiActor.CompanionOwner)
                        {
                            CompanionController component = aiActor.GetComponent<CompanionController>();
                            if (component.CanBePet && (double) Vector2.Distance(this.CenterPosition, component.specRigidbody.GetUnitCenter(ColliderType.HitBox)) <= 2.5)
                            {
                                component.DoPet(this);
                                this.spriteAnimator.Play("pet");
                                this.ToggleGunRenderers(false, "petting");
                                this.ToggleHandRenderers(false, "petting");
                                this.m_petDirection = (double) aiActor.specRigidbody.UnitCenter.x <= (double) this.specRigidbody.UnitCenter.x ? 180f : 0.0f;
                                this.m_pettingTarget = component;
                                break;
                            }
                        }
                    }
                }
            }
            this.ForceRefreshInteractable = false;
            if (this.AcceptingNonMotionInput || this.CurrentInputState == PlayerInputState.FoyerInputOnly)
            {
                Vector2 aimPointInWorld = (Vector2) this.DetermineAimPointInWorld();
                if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                {
                    this.m_currentGunAngle = this.CurrentGun.HandleAimRotation((Vector3) aimPointInWorld);
                    if ((bool) (UnityEngine.Object) this.CurrentSecondaryGun)
                    {
                        double num = (double) this.CurrentSecondaryGun.HandleAimRotation((Vector3) aimPointInWorld);
                    }
                }
                if (this.m_overrideGunAngle.HasValue)
                {
                    this.m_currentGunAngle = this.m_overrideGunAngle.Value;
                    this.gunAttachPoint.localRotation = Quaternion.Euler(this.gunAttachPoint.localRotation.x, this.gunAttachPoint.localRotation.y, this.m_currentGunAngle);
                }
                else
                    this.m_currentGunAngle = (aimPointInWorld - this.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
            }
            if (this.AcceptingNonMotionInput)
            {
                this.sprite.UpdateZDepth();
                if (this.inventory.DualWielding && (bool) (UnityEngine.Object) this.CurrentSecondaryGun)
                    this.HandleGunFiringInternal(this.CurrentSecondaryGun, instanceForPlayer, true);
                this.HandleGunFiringInternal(this.CurrentGun, instanceForPlayer, false);
            }
            else if (this.CurrentInputState == PlayerInputState.OnlyMovement && (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null && this.CurrentGun.IsCharging && instanceForPlayer.GetButton(GungeonActions.GungeonActionType.Shoot) && this.m_shouldContinueFiring)
                this.CurrentGun.ContinueAttack(this.m_CanAttack);
            return direction;
        }

        private void HandleGunFiringInternal(
            Gun targetGun,
            BraveInput currentBraveInput,
            bool isSecondary)
        {
            if (!((UnityEngine.Object) targetGun != (UnityEngine.Object) null))
                return;
            bool flag1 = currentBraveInput.GetButtonDown(GungeonActions.GungeonActionType.Shoot) || this.forceFireDown;
            if (this.OnTriedToInitiateAttack != null && flag1)
                this.OnTriedToInitiateAttack(this);
            if (this.SuppressThisClick)
            {
                this.exceptionTracker = 200;
                while (currentBraveInput.GetButtonDown(GungeonActions.GungeonActionType.Shoot))
                {
                    currentBraveInput.ConsumeButtonDown(GungeonActions.GungeonActionType.Shoot);
                    if (currentBraveInput.GetButtonUp(GungeonActions.GungeonActionType.Shoot))
                        currentBraveInput.ConsumeButtonUp(GungeonActions.GungeonActionType.Shoot);
                }
                this.exceptionTracker = 201;
                if (!currentBraveInput.GetButton(GungeonActions.GungeonActionType.Shoot))
                    this.SuppressThisClick = false;
            }
            else if (this.m_CanAttack && flag1)
            {
                this.exceptionTracker = 202;
                bool flag2 = false;
                Gun.AttackResult attackResult = targetGun.Attack();
                bool flag3 = flag2 | (attackResult == Gun.AttackResult.Fail ? 1 : (attackResult == Gun.AttackResult.OnCooldown ? 1 : 0)) == 0;
                this.m_newFloorNoInput = false;
                this.exceptionTracker = 203;
                if (!this.HasFiredNonStartingGun && attackResult == Gun.AttackResult.Success && !targetGun.StarterGunForAchievement)
                    this.HasFiredNonStartingGun = true;
                this.m_shouldContinueFiring = true;
                this.IsFiring = attackResult == Gun.AttackResult.Success && !targetGun.IsCharging;
                this.exceptionTracker = 204;
                if (attackResult == Gun.AttackResult.Success)
                    this.DidUnstealthyAction();
                if (flag3 && !isSecondary)
                    currentBraveInput.ConsumeButtonDown(GungeonActions.GungeonActionType.Shoot);
                this.m_controllerSemiAutoTimer = 0.0f;
            }
            else if ((currentBraveInput.GetButtonUp(GungeonActions.GungeonActionType.Shoot) || this.forceFireUp) && !this.KeepChargingDuringRoll)
            {
                this.exceptionTracker = 205;
                this.IsFiring = targetGun.CeaseAttack(this.m_CanAttack);
                if (!isSecondary)
                    currentBraveInput.ConsumeButtonUp(GungeonActions.GungeonActionType.Shoot);
                this.m_shouldContinueFiring = false;
            }
            else if ((currentBraveInput.GetButton(GungeonActions.GungeonActionType.Shoot) || this.forceFire || this.KeepChargingDuringRoll) && this.m_shouldContinueFiring)
            {
                this.exceptionTracker = 206;
                bool flag4 = this.IsDodgeRolling && !this.IsSlidingOverSurface;
                if (this.IsSlidingOverSurface)
                    this.m_hasFiredWhileSliding = true;
                if (this.UseFakeSemiAutoCooldown && targetGun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.SemiAutomatic && !targetGun.HasShootStyle(ProjectileModule.ShootStyle.Charged) && !flag4 && targetGun.CurrentAmmo > 0)
                {
                    this.m_controllerSemiAutoTimer += BraveTime.DeltaTime;
                    if ((double) this.m_controllerSemiAutoTimer > (double) BraveInput.ControllerFakeSemiAutoCooldown && !targetGun.IsEmpty && this.m_CanAttack)
                    {
                        this.exceptionTracker = 207;
                        targetGun.CeaseAttack(false);
                        if (targetGun.Attack() == Gun.AttackResult.Success)
                        {
                            this.m_controllerSemiAutoTimer = 0.0f;
                            this.IsFiring = !targetGun.IsCharging;
                        }
                    }
                    else
                    {
                        this.exceptionTracker = 208;
                        this.IsFiring = targetGun.ContinueAttack(this.m_CanAttack) && !targetGun.IsCharging;
                    }
                }
                else
                {
                    this.exceptionTracker = 209;
                    this.IsFiring = targetGun.ContinueAttack(this.m_CanAttack) && !targetGun.IsCharging;
                }
                this.exceptionTracker = 210;
                if (!targetGun.IsReloading)
                    this.DidUnstealthyAction();
            }
            else if (targetGun.IsFiring || targetGun.IsPreppedForThrow)
            {
                this.exceptionTracker = 211;
                this.IsFiring = targetGun.CeaseAttack(this.m_CanAttack);
                this.m_shouldContinueFiring = false;
            }
            if (!this.IsFiring)
                return;
            this.m_isThreatArrowing = false;
            this.m_elapsedNonalertTime = 0.0f;
        }

        private bool KeepChargingDuringRoll
        {
            get
            {
                return this.IsDodgeRolling && (UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null && this.CurrentGun.HasChargedProjectileReady;
            }
        }

        public void RemoveBrokenInteractable(IPlayerInteractable ixable)
        {
            if (this.m_lastInteractionTarget != ixable)
                return;
            this.m_lastInteractionTarget.OnExitRange(this);
            this.m_lastInteractionTarget = (IPlayerInteractable) null;
        }

        private void ChangeItem(int change)
        {
            if (this.m_preventItemSwitching)
                return;
            if (this.activeItems.Count > 1)
            {
                this.CurrentItem.OnItemSwitched(this);
                this.m_selectedItemIndex += change;
                this.m_selectedItemIndex = (this.m_selectedItemIndex + this.activeItems.Count) % this.activeItems.Count;
            }
            else
                this.m_selectedItemIndex = 0;
            if (EncounterTrackable.SuppressNextNotification)
                return;
            GameUIRoot.Instance.TemporarilyShowItemName(this.IsPrimaryPlayer);
        }

        public void CacheQuickEquipGun()
        {
            this.m_backupCachedQuickEquipGun = this.m_cachedQuickEquipGun;
            this.m_cachedQuickEquipGun = this.CurrentGun;
        }

        public void ChangeToGunSlot(int slotIndex, bool overrideGunLock = false)
        {
            if (this.inventory.AllGuns.Count == 0 || !(bool) (UnityEngine.Object) this.CurrentGun || slotIndex < 0 || slotIndex >= this.inventory.AllGuns.Count)
                return;
            int num = this.inventory.AllGuns.IndexOf(this.CurrentGun);
            this.ChangeGun(slotIndex - num, true, overrideGunLock);
        }

        public void ChangeGun(int change, bool forceEmptySelect = false, bool overrideGunLock = false)
        {
            if (this.inventory.AllGuns.Count == 0 || this.inventory.DualWielding && this.inventory.AllGuns.Count <= 2 && (UnityEngine.Object) this.CurrentSecondaryGun != (UnityEngine.Object) null || change % this.inventory.AllGuns.Count == 0)
                return;
            if (this.IsDodgeRolling)
                this.CurrentGun.ToggleRenderers(true);
            bool flag1 = GameManager.Options.HideEmptyGuns && this.IsInCombat && !forceEmptySelect;
            bool dualWielding = this.inventory.DualWielding;
            if (flag1 || dualWielding)
            {
                int num = 0;
                while (flag1 && this.inventory.GetTargetGunWithChange(change).CurrentAmmo == 0 || dualWielding && (UnityEngine.Object) this.inventory.GetTargetGunWithChange(change) == (UnityEngine.Object) this.CurrentSecondaryGun)
                {
                    ++num;
                    change += Math.Sign(change);
                    if (num >= this.inventory.AllGuns.Count)
                        break;
                }
                if ((UnityEngine.Object) this.inventory.GetTargetGunWithChange(change) == (UnityEngine.Object) this.CurrentSecondaryGun)
                    change += Math.Sign(change);
            }
            GameUIRoot.Instance.ForceClearReload(this.PlayerIDX);
            GunInventory inventory = this.inventory;
            int num1 = change;
            bool flag2 = overrideGunLock;
            int amt = num1;
            int num2 = flag2 ? 1 : 0;
            inventory.ChangeGun(amt, overrideGunLock: num2 != 0);
            if (this.IsDodgeRolling)
                this.CurrentGun.ToggleRenderers(false);
            this.m_equippedGunShift = change;
        }

        public void ClearPerLevelData()
        {
            this.m_currentRoom = (RoomHandler) null;
            this.m_lastInteractionTarget = (IPlayerInteractable) null;
            this.stats.ToNextLevel();
            this.m_bellygeonDepressedTiles.Clear();
            PlayerController.m_bellygeonDepressedTileTimers.Clear();
            for (int index = 0; index < this.additionalItems.Count; ++index)
                UnityEngine.Object.Destroy((UnityEngine.Object) this.additionalItems[index].gameObject);
            this.additionalItems.Clear();
        }

        public void BraveOnLevelWasLoaded()
        {
            this.m_newFloorNoInput = true;
            this.HasGottenKeyThisRun = false;
            this.LevelToLoadOnPitfall = string.Empty;
            this.m_cachedLevelToLoadOnPitfall = string.Empty;
            this.m_interruptingPitRespawn = false;
            this.m_cachedPosition = Vector2.zero;
            this.m_currentRoom = (RoomHandler) null;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                GameManager.Instance.GetOtherPlayer(this).m_currentRoom = (RoomHandler) null;
            if (GameManager.Instance.InTutorial)
                this.sprite.gameObject.SetLayerRecursively(LayerMask.NameToLayer("ShadowCaster"));
            if ((UnityEngine.Object) GameUIRoot.Instance != (UnityEngine.Object) null)
            {
                GameUIRoot.Instance.UpdatePlayerHealthUI(this, this.healthHaver);
                if (this.passiveItems != null && this.passiveItems.Count > 0)
                {
                    for (int index = 0; index < this.passiveItems.Count; ++index)
                        GameUIRoot.Instance.AddPassiveItemToDock(this.passiveItems[index], this);
                }
                this.Blanks = Mathf.Max(this.Blanks, (GameManager.Instance.CurrentGameType != GameManager.GameType.SINGLE_PLAYER ? this.stats.NumBlanksPerFloorCoop : this.stats.NumBlanksPerFloor) + Mathf.FloorToInt(this.stats.GetStatValue(PlayerStats.StatType.AdditionalBlanksPerFloor)));
                if (GameManager.Instance.InTutorial)
                    this.Blanks = 0;
                GameUIRoot.Instance.UpdatePlayerBlankUI(this);
                this.carriedConsumables.ForceUpdateUI();
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    GameUIRoot.Instance.UpdateGunData(GameManager.Instance.GetOtherPlayer(this).inventory, 0, GameManager.Instance.GetOtherPlayer(this));
                    GameUIRoot.Instance.UpdateItemData(GameManager.Instance.GetOtherPlayer(this), GameManager.Instance.GetOtherPlayer(this).CurrentItem, GameManager.Instance.GetOtherPlayer(this).activeItems);
                }
                if (this.IsGhost)
                {
                    GameUIRoot.Instance.DisableCoopPlayerUI(this);
                    GameUIRoot.Instance.TransitionToGhostUI(this);
                }
                else if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
                    this.CurrentGun.ForceImmediateReload();
                if (this.OnNewFloorLoaded != null)
                    this.OnNewFloorLoaded(this);
            }
            if ((bool) (UnityEngine.Object) this.knockbackDoer)
                this.knockbackDoer.ClearContinuousKnockbacks();
            Shader.SetGlobalFloat("_MeduziReflectionsEnabled", 0.0f);
            if (!this.m_usesRandomStartingEquipment || this.m_randomStartingItemsInitialized || !GameManager.Instance.IsLoadingFirstShortcutFloor && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
                return;
            this.m_randomStartingItemsInitialized = true;
            for (int index = 0; index < this.startingPassiveItemIds.Count; ++index)
            {
                if (!this.HasPassiveItem(this.startingPassiveItemIds[index]))
                    this.AcquirePassiveItemPrefabDirectly(PickupObjectDatabase.GetById(this.startingPassiveItemIds[index]) as PassiveItem);
            }
            for (int index = 0; index < this.startingActiveItemIds.Count; ++index)
            {
                if (!this.HasActiveItem(this.startingActiveItemIds[index]))
                {
                    EncounterTrackable.SuppressNextNotification = true;
                    (PickupObjectDatabase.GetById(this.startingActiveItemIds[index]) as PlayerItem).Pickup(this);
                    EncounterTrackable.SuppressNextNotification = false;
                }
            }
        }

        private void EnteredNewRoom(RoomHandler newRoom)
        {
            this.RealtimeEnteredCurrentRoom = UnityEngine.Time.realtimeSinceStartup;
        }

        public void ForceChangeRoom(RoomHandler newRoom)
        {
            RoomHandler currentRoom = this.m_currentRoom;
            this.m_currentRoom = newRoom;
            if (currentRoom != null)
            {
                currentRoom.PlayerExit(this);
                currentRoom.OnBecameInvisible(this);
            }
            this.m_currentRoom.PlayerEnter(this);
            this.EnteredNewRoom(this.m_currentRoom);
            this.m_inExitLastFrame = false;
            GameManager.Instance.MainCameraController.AssignBoundingPolygon(this.m_currentRoom.cameraBoundingPolygon);
        }

        private void HandleRoomProcessing()
        {
            if (BraveUtility.isLoadingLevel || GameManager.Instance.IsLoadingLevel)
                return;
            if (this.m_roomBeforeExit == null)
                this.m_roomBeforeExit = this.m_currentRoom;
            Dungeon dungeon = GameManager.Instance.Dungeon;
            DungeonData data = dungeon.data;
            CellData cellSafe1 = data.GetCellSafe(PhysicsEngine.PixelToUnit(this.specRigidbody.PrimaryPixelCollider.LowerLeft).ToIntVector2(VectorConversions.Floor));
            CellData cellSafe2 = data.GetCellSafe(PhysicsEngine.PixelToUnit(this.specRigidbody.PrimaryPixelCollider.LowerRight).ToIntVector2(VectorConversions.Floor));
            CellData cellSafe3 = data.GetCellSafe(PhysicsEngine.PixelToUnit(this.specRigidbody.PrimaryPixelCollider.UpperLeft).ToIntVector2(VectorConversions.Floor));
            CellData cellSafe4 = data.GetCellSafe(PhysicsEngine.PixelToUnit(this.specRigidbody.PrimaryPixelCollider.UpperRight).ToIntVector2(VectorConversions.Floor));
            if (cellSafe1 == null || cellSafe2 == null || cellSafe3 == null || cellSafe4 == null)
                return;
            RoomHandler roomHandler = (RoomHandler) null;
            CellData cellData = (CellData) null;
            if (cellSafe1.isExitCell || cellSafe2.isExitCell || cellSafe3.isExitCell || cellSafe4.isExitCell)
            {
                cellData = !cellSafe1.isExitCell ? (!cellSafe2.isExitCell ? (!cellSafe3.isExitCell ? (!cellSafe4.isExitCell ? (CellData) null : cellSafe4) : cellSafe3) : cellSafe2) : cellSafe1;
                if (cellData != null)
                    roomHandler = cellData.connectedRoom1 == this.m_currentRoom ? cellData.connectedRoom2 : cellData.connectedRoom1;
                this.m_previousExitLinkedRoom = roomHandler;
                this.m_inExitLastFrame = true;
            }
            this.InExitCell = cellData != null;
            this.CurrentExitCell = cellData;
            if (!this.m_inExitLastFrame)
                this.m_roomBeforeExit = this.m_currentRoom;
            if (roomHandler == null)
                roomHandler = cellSafe1.parentRoom == this.m_currentRoom ? (cellSafe2.parentRoom == this.m_currentRoom ? (cellSafe3.parentRoom == this.m_currentRoom ? (cellSafe4.parentRoom == this.m_currentRoom ? (RoomHandler) null : cellSafe4.parentRoom) : cellSafe3.parentRoom) : cellSafe2.parentRoom) : cellSafe1.parentRoom;
            if (roomHandler != null)
            {
                if (roomHandler.visibility == RoomHandler.VisibilityStatus.OBSCURED || roomHandler.visibility == RoomHandler.VisibilityStatus.REOBSCURED || roomHandler.IsSealed)
                {
                    bool flag = cellSafe1.isDoorFrameCell || cellSafe2.isDoorFrameCell || cellSafe3.isDoorFrameCell || cellSafe4.isDoorFrameCell;
                    if (cellSafe1.parentRoom != this.m_currentRoom && cellSafe2.parentRoom != this.m_currentRoom && cellSafe3.parentRoom != this.m_currentRoom && cellSafe4.parentRoom != this.m_currentRoom && !flag)
                    {
                        if (this.m_currentRoom != null)
                            this.m_currentRoom.PlayerExit(this);
                        this.m_currentRoom = roomHandler;
                        this.m_currentRoom.PlayerEnter(this);
                        this.EnteredNewRoom(this.m_currentRoom);
                        this.m_inExitLastFrame = false;
                        GameManager.Instance.MainCameraController.AssignBoundingPolygon(this.m_currentRoom.cameraBoundingPolygon);
                    }
                }
                else
                {
                    if (this.m_currentRoom != null)
                        this.m_currentRoom.OnBecameVisible(this);
                    if (cellData != null && (UnityEngine.Object) cellData.exitDoor != (UnityEngine.Object) null)
                    {
                        if (cellData.exitDoor.IsOpenForVisibilityTest && ((UnityEngine.Object) cellData.exitDoor.subsidiaryBlocker == (UnityEngine.Object) null || !cellData.exitDoor.subsidiaryBlocker.isSealed) && ((UnityEngine.Object) cellData.exitDoor.subsidiaryDoor == (UnityEngine.Object) null || cellData.exitDoor.subsidiaryDoor.IsOpenForVisibilityTest))
                            roomHandler.OnBecameVisible(this);
                        else
                            roomHandler.OnBecameInvisible(this);
                    }
                    else
                        roomHandler.OnBecameVisible(this);
                    if (!cellSafe1.isExitCell && !cellSafe2.isExitCell && !cellSafe3.isExitCell && !cellSafe4.isExitCell && cellSafe1.parentRoom == roomHandler)
                    {
                        this.m_inExitLastFrame = false;
                        if (this.m_currentRoom != null)
                            this.m_currentRoom.PlayerExit(this);
                        this.m_currentRoom = roomHandler;
                        this.m_currentRoom.PlayerEnter(this);
                        this.EnteredNewRoom(this.m_currentRoom);
                        GameManager.Instance.MainCameraController.AssignBoundingPolygon(this.m_currentRoom.cameraBoundingPolygon);
                    }
                }
            }
            else if (this.m_inExitLastFrame)
            {
                this.m_inExitLastFrame = false;
                if (this.m_previousExitLinkedRoom != null && this.m_previousExitLinkedRoom.visibility != RoomHandler.VisibilityStatus.OBSCURED)
                    Pixelator.Instance.ProcessRoomAdditionalExits(IntVector2.Zero, this.m_previousExitLinkedRoom, false);
            }
            for (int index = 0; index < data.rooms.Count; ++index)
            {
                RoomHandler room = data.rooms[index];
                if (room.visibility == RoomHandler.VisibilityStatus.CURRENT && room != this.m_currentRoom && room != roomHandler)
                {
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                    {
                        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(this);
                        if ((bool) (UnityEngine.Object) otherPlayer && (otherPlayer.CurrentRoom == room || otherPlayer.InExitCell && otherPlayer.CurrentExitCell != null && (bool) (UnityEngine.Object) otherPlayer.CurrentExitCell.exitDoor && (otherPlayer.CurrentExitCell.exitDoor.upstreamRoom == room || otherPlayer.CurrentExitCell.exitDoor.downstreamRoom == room)))
                            continue;
                    }
                    room.PlayerExit(this);
                }
            }
            if (this.m_currentRoom != null)
            {
                this.m_currentRoom.PlayerInCell(this, cellSafe1.position, this.specRigidbody.PrimaryPixelCollider.UnitBottomLeft);
                this.m_currentRoom.PlayerInCell(this, cellSafe2.position, this.specRigidbody.PrimaryPixelCollider.UnitBottomRight);
                this.m_currentRoom.PlayerInCell(this, cellSafe3.position, this.specRigidbody.PrimaryPixelCollider.UnitTopLeft);
                this.m_currentRoom.PlayerInCell(this, cellSafe4.position, this.specRigidbody.PrimaryPixelCollider.UnitTopRight);
            }
            if ((UnityEngine.Object) dungeon != (UnityEngine.Object) null && dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.BELLYGEON)
            {
                IntVector2 intVector2 = this.SpriteBottomCenter.IntXY(VectorConversions.Floor);
                if (intVector2 != this.m_cachedLastCenterCellBellygeon)
                {
                    this.m_cachedLastCenterCellBellygeon = intVector2;
                    if (this.m_bellygeonDepressedTiles.Contains(intVector2))
                    {
                        PlayerController.m_bellygeonDepressedTileTimers[intVector2] = 1f;
                    }
                    else
                    {
                        this.m_bellygeonDepressedTiles.Add(intVector2);
                        PlayerController.m_bellygeonDepressedTileTimers.Add(intVector2, 1f);
                    }
                    data.TriggerFloorAnimationsInCell(intVector2);
                }
                for (int index = 0; index < this.m_bellygeonDepressedTiles.Count; ++index)
                {
                    if (!(this.m_bellygeonDepressedTiles[index] == intVector2))
                    {
                        PlayerController.m_bellygeonDepressedTileTimers[this.m_bellygeonDepressedTiles[index]] = PlayerController.m_bellygeonDepressedTileTimers[this.m_bellygeonDepressedTiles[index]] - BraveTime.DeltaTime;
                        if ((double) PlayerController.m_bellygeonDepressedTileTimers[this.m_bellygeonDepressedTiles[index]] <= 0.0)
                        {
                            data.UntriggerFloorAnimationsInCell(this.m_bellygeonDepressedTiles[index]);
                            PlayerController.m_bellygeonDepressedTileTimers.Remove(this.m_bellygeonDepressedTiles[index]);
                            this.m_bellygeonDepressedTiles.RemoveAt(index);
                            --index;
                        }
                    }
                }
            }
            this.HandleCurrentRoomExtraData();
        }

        private void HandleCurrentRoomExtraData()
        {
            bool flag = false;
            if (this.IsPrimaryPlayer)
                flag = true;
            else if (GameManager.Instance.PrimaryPlayer.healthHaver.IsDead)
                flag = true;
            if (!flag)
                return;
            if (GameManager.Instance.Dungeon.OverrideAmbientLight)
                RenderSettings.ambientLight = GameManager.Instance.Dungeon.OverrideAmbientColor;
            else if (this.CurrentRoom != null && !this.CurrentRoom.area.IsProceduralRoom && this.CurrentRoom.area.runtimePrototypeData != null && this.CurrentRoom.area.runtimePrototypeData.usesCustomAmbient)
            {
                Color color = this.CurrentRoom.area.runtimePrototypeData.customAmbient;
                if (GameManager.Options.LightingQuality == GameOptions.GenericHighMedLowOption.LOW && this.CurrentRoom.area.runtimePrototypeData.usesDifferentCustomAmbientLowQuality)
                    color = this.CurrentRoom.area.runtimePrototypeData.customAmbientLowQuality;
                Vector3 target = new Vector3(color.r, color.g, color.b) * RenderSettings.ambientIntensity;
                Vector3 current = new Vector3(RenderSettings.ambientLight.r, RenderSettings.ambientLight.g, RenderSettings.ambientLight.b);
                if (!(target != current))
                    return;
                Vector3 vector3 = Vector3.MoveTowards(current, target, 0.35f * GameManager.INVARIANT_DELTA_TIME);
                RenderSettings.ambientLight = new Color(vector3.x, vector3.y, vector3.z, RenderSettings.ambientLight.a);
            }
            else
            {
                Color color = GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW ? GameManager.Instance.Dungeon.decoSettings.ambientLightColor : GameManager.Instance.Dungeon.decoSettings.lowQualityAmbientLightColor;
                Vector3 target = new Vector3(color.r, color.g, color.b) * RenderSettings.ambientIntensity;
                Vector3 current = new Vector3(RenderSettings.ambientLight.r, RenderSettings.ambientLight.g, RenderSettings.ambientLight.b);
                if (!(target != current))
                    return;
                Vector3 vector3 = Vector3.MoveTowards(current, target, 0.35f * GameManager.INVARIANT_DELTA_TIME);
                RenderSettings.ambientLight = new Color(vector3.x, vector3.y, vector3.z, RenderSettings.ambientLight.a);
            }
        }

        private void HandleGunAttachPointInternal(Gun targetGun, bool isSecondary = false)
        {
            if ((UnityEngine.Object) targetGun == (UnityEngine.Object) null)
                return;
            Vector3 vector3_1 = this.m_startingAttachPointPosition;
            Vector3 vector3_2 = this.downwardAttachPointPosition;
            if (targetGun.IsForwardPosition)
            {
                vector3_1 = vector3_1.WithX(this.m_spriteDimensions.x - vector3_1.x);
                vector3_2 = vector3_2.WithX(this.m_spriteDimensions.x - vector3_2.x);
            }
            if (this.SpriteFlipped)
            {
                vector3_1 = vector3_1.WithX(this.m_spriteDimensions.x - vector3_1.x);
                vector3_2 = vector3_2.WithX(this.m_spriteDimensions.x - vector3_2.x);
            }
            float x = !this.SpriteFlipped ? 1f : -1f;
            Vector3 vector3_3 = targetGun.GetCarryPixelOffset(this.characterIdentity).ToVector3();
            vector3_1 += Vector3.Scale(vector3_3, new Vector3(x, 1f, 1f)) * (1f / 16f);
            vector3_2 += Vector3.Scale(vector3_3, new Vector3(x, 1f, 1f)) * (1f / 16f);
            if (targetGun.Handedness == GunHandedness.NoHanded && this.SpriteFlipped)
            {
                vector3_1 += Vector3.Scale(targetGun.leftFacingPixelOffset.ToVector3(), new Vector3(x, 1f, 1f)) * (1f / 16f);
                vector3_2 += Vector3.Scale(targetGun.leftFacingPixelOffset.ToVector3(), new Vector3(x, 1f, 1f)) * (1f / 16f);
            }
            if (this.IsFlying)
            {
                vector3_1 += new Vector3(0.0f, 3f / 16f, 0.0f);
                vector3_2 += new Vector3(0.0f, 3f / 16f, 0.0f);
            }
            if (isSecondary)
            {
                if ((UnityEngine.Object) targetGun.transform.parent != (UnityEngine.Object) this.SecondaryGunPivot)
                {
                    targetGun.transform.parent = this.SecondaryGunPivot;
                    targetGun.transform.localRotation = Quaternion.identity;
                    targetGun.HandleSpriteFlip(this.SpriteFlipped);
                    targetGun.UpdateAttachTransform();
                }
                this.SecondaryGunPivot.position = this.gunAttachPoint.position + x * new Vector3(-0.75f, 0.0f, 0.0f);
            }
            else
            {
                if ((UnityEngine.Object) targetGun.transform.parent != (UnityEngine.Object) this.gunAttachPoint)
                {
                    targetGun.transform.parent = this.gunAttachPoint;
                    targetGun.transform.localRotation = Quaternion.identity;
                    targetGun.HandleSpriteFlip(this.SpriteFlipped);
                    targetGun.UpdateAttachTransform();
                }
                if (targetGun.IsHeroSword)
                {
                    float t = (float) (1.0 - (double) Mathf.Abs(this.m_currentGunAngle - 90f) / 90.0);
                    this.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(Vector3.Slerp(vector3_1, vector3_2, t), 16f);
                }
                else if (targetGun.Handedness == GunHandedness.TwoHanded)
                {
                    float t = Mathf.PingPong(Mathf.Abs((float) (1.0 - (double) Mathf.Abs(this.m_currentGunAngle + 90f) / 90.0)), 1f);
                    Vector2 zero = Vector2.zero;
                    Vector2 vector2 = (double) this.m_currentGunAngle <= 0.0 ? Vector2.Scale(targetGun.carryPixelDownOffset.ToVector2(), new Vector2(x, 1f)) * (1f / 16f) : Vector2.Scale(targetGun.carryPixelUpOffset.ToVector2(), new Vector2(x, 1f)) * (1f / 16f);
                    if (targetGun.LockedHorizontalOnCharge)
                        vector2 = (Vector2) Vector3.Slerp((Vector3) vector2, (Vector3) Vector2.zero, targetGun.GetChargeFraction());
                    if ((double) this.m_currentGunAngle < 0.0)
                        this.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(Vector3.Slerp(vector3_1, vector3_2 + vector2.ToVector3ZUp(), t), 16f);
                    else
                        this.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(Vector3.Slerp(vector3_1, vector3_1 + vector2.ToVector3ZUp(), t), 16f);
                }
                else
                    this.gunAttachPoint.localPosition = BraveUtility.QuantizeVector(vector3_1, 16f);
            }
        }

        private void HandleGunAttachPoint()
        {
            if ((bool) (UnityEngine.Object) this.CurrentGun)
                this.HandleGunAttachPointInternal(this.CurrentGun);
            if (this.inventory == null || !this.inventory.DualWielding || !(bool) (UnityEngine.Object) this.CurrentSecondaryGun)
                return;
            this.HandleGunAttachPointInternal(this.CurrentSecondaryGun, true);
        }

        private void HandleShellCasingDisplacement()
        {
        }

        protected override void OnDestroy()
        {
            this.ClearOverrideShader();
            if (PassiveItem.ActiveFlagItems != null)
                PassiveItem.ActiveFlagItems.Remove(this);
            base.OnDestroy();
        }

        public void TriggerHighStress(float duration)
        {
            if ((bool) (UnityEngine.Object) this.healthHaver)
                this.healthHaver.NextShotKills = true;
            this.m_highStressTimer = duration;
        }

        private void DoAutoAimNotification(bool warnOnly)
        {
            dfLabel nameLabel = GameUIRoot.Instance.notificationController.NameLabel;
            if (warnOnly)
                GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.PostprocessString(nameLabel.ForceGetLocalizedValue("#SUPERDUPERAUTOAIM_WARNING_TITLE")), StringTableManager.PostprocessString(nameLabel.ForceGetLocalizedValue("#SUPERDUPERAUTOAIM_WARNING_BODY")), (tk2dSpriteCollectionData) null, -1);
            else
                GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.PostprocessString(nameLabel.ForceGetLocalizedValue("#SUPERDUPERAUTOAIM_POPUP_TITLE")), StringTableManager.PostprocessString(nameLabel.ForceGetLocalizedValue("#SUPERDUPERAUTOAIM_WARNING_BODY_B")), (tk2dSpriteCollectionData) null, -1);
        }

        public void DoVibration(Vibration.Time time, Vibration.Strength strength)
        {
            BraveInput.GetInstanceForPlayer(this.PlayerIDX).DoVibration(time, strength);
        }

        public void DoVibration(float time, Vibration.Strength strength)
        {
            BraveInput.GetInstanceForPlayer(this.PlayerIDX).DoVibration(time, strength);
        }

        public void DoVibration(
            Vibration.Time time,
            Vibration.Strength largeMotor,
            Vibration.Strength smallMotor)
        {
            BraveInput.GetInstanceForPlayer(this.PlayerIDX).DoVibration(time, largeMotor, smallMotor);
        }

        public void DoScreenShakeVibration(float time, float magnitude)
        {
            BraveInput.GetInstanceForPlayer(this.PlayerIDX).DoScreenShakeVibration(time, magnitude);
        }

        public void DoSustainedVibration(Vibration.Strength strength)
        {
            BraveInput.GetInstanceForPlayer(this.PlayerIDX).DoSustainedVibration(strength);
        }

        public void DoSustainedVibration(Vibration.Strength largeMotor, Vibration.Strength smallMotor)
        {
            BraveInput.GetInstanceForPlayer(this.PlayerIDX).DoSustainedVibration(largeMotor, smallMotor);
        }

        public Vector2 LastCommandedDirection => this.m_playerCommandedDirection;

        public Vector2 NonZeroLastCommandedDirection
        {
            get
            {
                return this.m_playerCommandedDirection != Vector2.zero ? this.m_playerCommandedDirection : this.m_lastNonzeroCommandedDirection;
            }
        }

        public bool IsPetting => (UnityEngine.Object) this.m_pettingTarget != (UnityEngine.Object) null;

        public enum DodgeRollState
        {
            PreRollDelay,
            InAir,
            OnGround,
            None,
            AdditionalDelay,
            Blink,
        }

        public enum EscapeSealedRoomStyle
        {
            DEATH_SEQUENCE,
            ESCAPE_SPIN,
            NONE,
            TELEPORTER,
            GRIP_MASTER,
        }
    }

