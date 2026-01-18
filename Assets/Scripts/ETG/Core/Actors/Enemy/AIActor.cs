using Dungeonator;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

[RequireComponent(typeof (SpeculativeRigidbody))]
[RequireComponent(typeof (HitEffectHandler))]
public class AIActor : GameActor, IPlaceConfigurable
  {
    private static readonly string[] s_floorTypeNames;
    private static float m_healthModifier = 1f;
    [HideInInspector]
    public int EnemyId = -1;
    [DisableInInspector]
    public string EnemyGuid;
    [DisableInInspector]
    public int ForcedPositionInAmmonomicon = -1;
    [Header("Flags")]
    public bool SetsFlagOnDeath;
    [LongEnum]
    public GungeonFlags FlagToSetOnDeath;
    public bool SetsFlagOnActivation;
    [LongEnum]
    public GungeonFlags FlagToSetOnActivation;
    public bool SetsCharacterSpecificFlagOnDeath;
    [LongEnum]
    public CharacterSpecificGungeonFlags CharacterSpecificFlagToSetOnDeath;
    [Header("Core Enemy Stats")]
    public bool IsNormalEnemy = true;
    public bool IsSignatureEnemy;
    public bool IsHarmlessEnemy;
    [HideInInspectorIf("IsNormalEnemy", false)]
    public ActorCompanionSettings CompanionSettings;
    [NonSerialized]
    public bool ForceBlackPhantom;
    [NonSerialized]
    public bool PreventBlackPhantom;
    [NonSerialized]
    public bool IsInReinforcementLayer;
    [NonSerialized]
    public PlayerController CompanionOwner;
    [FormerlySerializedAs("m_movementSpeed")]
    [SerializeField]
    public float MovementSpeed = 2f;
    [EnumFlags]
    public CellTypes PathableTiles = CellTypes.FLOOR;
    public GameObject LosPoint;
    [Header("Collision Data")]
    public bool DiesOnCollison;
    public float CollisionDamage = 1f;
    public float CollisionKnockbackStrength = 5f;
    public CoreDamageTypes CollisionDamageTypes;
    public float EnemyCollisionKnockbackStrengthOverride = -1f;
    public VFXPool CollisionVFX;
    public VFXPool NonActorCollisionVFX;
    public bool CollisionSetsPlayerOnFire;
    public bool TryDodgeBullets = true;
    public float AvoidRadius = 4f;
    public bool ReflectsProjectilesWhileInvulnerable;
    public bool HitByEnemyBullets;
    public bool HasOverrideDodgeRollDeath;
    [ShowInInspectorIf("HasOverrideDodgeRollDeath", false)]
    public string OverrideDodgeRollDeath;
    [Header("Loot Settings")]
    public bool CanDropCurrency = true;
    public float AdditionalSingleCoinDropChance;
    [NonSerialized]
    public int AssignedCurrencyToDrop;
    public bool CanDropItems = true;
    [ShowInInspectorIf("CanDropCurrency", true)]
    public GenericLootTable CustomLootTable;
    public bool CanDropDuplicateItems;
    public int CustomLootTableMinDrops = 1;
    public int CustomLootTableMaxDrops = 1;
    public GenericLootTable CustomChestTable;
    public float ChanceToDropCustomChest;
    public bool IgnoreForRoomClear;
    [HideInInspector]
    [NonSerialized]
    public List<PickupObject> AdditionalSimpleItemDrops = new List<PickupObject>();
    [HideInInspector]
    [NonSerialized]
    public List<PickupObject> AdditionalSafeItemDrops = new List<PickupObject>();
    public bool SpawnLootAtRewardChestPos;
    [Header("Extra Visual Settings")]
    public GameObject CorpseObject;
    [ShowInInspectorIf("CorpseObject", true)]
    public bool CorpseShadow = true;
    [ShowInInspectorIf("CorpseObject", true)]
    public bool TransferShadowToCorpse;
    public AIActor.ShadowDeathType shadowDeathType = AIActor.ShadowDeathType.Fade;
    public bool PreventDeathKnockback;
    public VFXPool OnCorpseVFX;
    public GameObject OnEngagedVFX;
    public tk2dBaseSprite.Anchor OnEngagedVFXAnchor;
    public float shadowHeightOffset;
    public bool invisibleUntilAwaken;
    public bool procedurallyOutlined = true;
    public bool forceUsesTrimmedBounds = true;
    public AIActor.ReinforceType reinforceType;
    public Texture2D optionalPalette;
    public bool UsesVaryingEmissiveShaderPropertyBlock;
    public Transform OverrideBuffEffectPosition;
    [Header("Audio")]
    public string EnemySwitchState;
    public string OverrideSpawnReticleAudio;
    public string OverrideSpawnAppearAudio;
    public bool UseMovementAudio;
    [ShowInInspectorIf("UseMovementAudio", true)]
    public string StartMovingEvent;
    [ShowInInspectorIf("UseMovementAudio", true)]
    public string StopMovingEvent;
    private bool m_audioMovedLastFrame;
    [SerializeField]
    public List<ActorAudioEvent> animationAudioEvents;
    [Header("Other")]
    public List<AIActor.HealthOverride> HealthOverrides;
    public AIActor.EnemyTypeIdentifier IdentifierForEffects;
    [HideInInspector]
    public bool BehaviorOverridesVelocity;
    [HideInInspector]
    public Vector2 BehaviorVelocity = Vector2.zero;
    public Vector2? OverridePathVelocity;
    public bool AlwaysShowOffscreenArrow;
    [NonSerialized]
    public float BaseMovementSpeed;
    [NonSerialized]
    public float LocalTimeScale = 1f;
    [NonSerialized]
    public bool UniquePlayerTargetFlag;
    private bool? m_cachedIsMimicEnemy;
    [NonSerialized]
    public bool HasBeenBloodthirstProcessed;
    [NonSerialized]
    public bool CanBeBloodthirsted;
    private Vector2 m_currentlyAppliedEnemyScale = Vector2.one;
    private bool m_canTargetPlayers = true;
    private bool m_canTargetEnemies;
    public BlackPhantomProperties BlackPhantomProperties;
    public bool ForceBlackPhantomParticles;
    public bool OverrideBlackPhantomParticlesCollider;
    [ShowInInspectorIf("OverrideBlackPhantomParticlesCollider", true)]
    public int BlackPhantomParticlesCollider;
    private AIActor.EnemyChampionType m_championType;
    private bool? m_isWorthShootingAt;
    public bool PreventFallingInPitsEver;
    private bool m_isPaletteSwapped;
    [NonSerialized]
    private Color? OverrideOutlineColor;
    private bool m_cachedTurboness;
    private bool m_turboWake;
    private int m_cachedBodySpriteCount;
    private Shader m_cachedBodySpriteShader;
    private Shader m_cachedGunSpriteShader;
    private bool? ShouldDoBlackPhantomParticles;
    private const float c_particlesPerSecond = 40f;
    private List<IntVector2> m_upcomingPathTiles = new List<IntVector2>();
    private bool m_cachedHasLineOfSightToTarget;
    private SpeculativeRigidbody m_cachedLosTarget;
    private int m_cachedLosFrame;
    private Vector2 m_lastPosition;
    private IntVector2? m_clearance;
    private CellVisualData.CellFloorType? m_prevFloorType;
    protected System.Action OnPostStartInitialization;
    private bool m_hasGivenRewards;
    public System.Action OnHandleRewards;
    private bool m_isSafeMoving;
    private float m_safeMoveTimer;
    private float m_safeMoveTime;
    private Vector2? m_safeMoveStartPos;
    private Vector2? m_safeMoveEndPos;
    private CustomEngageDoer m_customEngageDoer;
    private CustomReinforceDoer m_customReinforceDoer;
    private Func<SpeculativeRigidbody, bool> m_rigidbodyExcluder;
    private Vector3 m_spriteDimensions;
    private Vector3 m_spawnPosition;
    private RoomHandler parentRoom;
    private int m_currentPhase;
    private bool m_isReadyForRepath = true;
    private Path m_currentPath;
    private Vector2? m_overridePathEnd;
    private int m_strafeDirection = 1;
    private bool m_hasBeenEngaged;
    private string m_awakenAnimation;
    protected bool? m_forcedOutlines;
    private Vector2 m_knockbackVelocity;
    public const float c_minStartingDistanceFromPlayer = 8f;
    public const float c_maxCloseStartingDistanceFromPlayer = 15f;

    static AIActor()
    {
      AIActor.s_floorTypeNames = Enum.GetNames(typeof (CellVisualData.CellFloorType));
    }

    public static void ClearPerLevelData() => StaticReferenceManager.AllEnemies.Clear();

    public static float BaseLevelHealthModifier
    {
      get
      {
        float levelHealthModifier = 1f;
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          levelHealthModifier = GameManager.Instance.COOP_ENEMY_HEALTH_MULTIPLIER;
        GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
        if (loadedLevelDefinition != null)
          levelHealthModifier *= loadedLevelDefinition.enemyHealthMultiplier;
        return levelHealthModifier;
      }
    }

    public static float HealthModifier
    {
      get => AIActor.m_healthModifier;
      set
      {
        float healthModifier = AIActor.m_healthModifier;
        AIActor.m_healthModifier = value;
        for (int index = 0; index < StaticReferenceManager.AllEnemies.Count; ++index)
        {
          if ((UnityEngine.Object) StaticReferenceManager.AllEnemies[index] != (UnityEngine.Object) null && (bool) (UnityEngine.Object) StaticReferenceManager.AllEnemies[index])
          {
            HealthHaver healthHaver = StaticReferenceManager.AllEnemies[index].healthHaver;
            if (!healthHaver.healthIsNumberOfHits)
              healthHaver.SetHealthMaximum(healthHaver.GetMaxHealth() / healthModifier * AIActor.m_healthModifier);
          }
        }
      }
    }

    public float EnemyCollisionKnockbackStrength
    {
      get
      {
        return (double) this.EnemyCollisionKnockbackStrengthOverride >= 0.0 ? this.EnemyCollisionKnockbackStrengthOverride : this.CollisionKnockbackStrength;
      }
    }

    public Vector2 VoluntaryMovementVelocity
    {
      get
      {
        if ((bool) (UnityEngine.Object) this.behaviorSpeculator && this.behaviorSpeculator.IsStunned)
          return Vector2.zero;
        return this.BehaviorOverridesVelocity ? this.BehaviorVelocity : this.GetPathVelocityContribution();
      }
    }

    public bool IsMimicEnemy
    {
      get
      {
        if (!this.m_cachedIsMimicEnemy.HasValue)
        {
          this.m_cachedIsMimicEnemy = new bool?(false);
          if ((bool) (UnityEngine.Object) this.encounterTrackable && !string.IsNullOrEmpty(this.encounterTrackable.EncounterGuid))
            this.m_cachedIsMimicEnemy = new bool?(this.encounterTrackable.EncounterGuid == GlobalEncounterGuids.Mimic);
        }
        return this.m_cachedIsMimicEnemy.Value;
      }
    }

    public float LocalDeltaTime
    {
      get
      {
        return this.IsBlackPhantom ? BraveTime.DeltaTime * this.LocalTimeScale * this.BlackPhantomProperties.LocalTimeScaleMultiplier : BraveTime.DeltaTime * this.LocalTimeScale;
      }
    }

    public Vector2 EnemyScale
    {
      get => this.m_currentlyAppliedEnemyScale;
      set
      {
        this.m_currentlyAppliedEnemyScale = value;
        this.transform.localScale = value.ToVector3ZUp(1f);
        if (!(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        this.specRigidbody.UpdateCollidersOnScale = true;
        this.specRigidbody.RegenerateColliders = true;
      }
    }

    [HideInInspector]
    public bool HasDamagedPlayer { get; set; }

    public bool CanTargetPlayers
    {
      get => this.m_canTargetPlayers;
      set
      {
        this.PlayerTarget = (GameActor) null;
        this.m_canTargetPlayers = value;
      }
    }

    public bool CanTargetEnemies
    {
      get => this.m_canTargetEnemies;
      set
      {
        this.PlayerTarget = (GameActor) null;
        this.m_canTargetEnemies = value;
      }
    }

    public bool OverrideHitEnemies
    {
      get
      {
        int mask = CollisionMask.LayerToMask(CollisionLayer.EnemyCollider);
        return (this.specRigidbody.GetPixelCollider(ColliderType.Ground).CollisionLayerCollidableOverride & mask) == mask;
      }
      set
      {
        int mask = CollisionMask.LayerToMask(CollisionLayer.EnemyCollider);
        PixelCollider pixelCollider = this.specRigidbody.GetPixelCollider(ColliderType.Ground);
        if (value)
          pixelCollider.CollisionLayerCollidableOverride |= mask;
        else
          pixelCollider.CollisionLayerCollidableOverride &= ~mask;
      }
    }

    public bool IsOverPit
    {
      get
      {
        return GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) (!((UnityEngine.Object) this.specRigidbody != (UnityEngine.Object) null) ? this.CenterPosition : this.specRigidbody.GroundPixelCollider.UnitCenter));
      }
    }

    public bool IsBlackPhantom => this.m_championType == AIActor.EnemyChampionType.JAMMED;

    public bool SuppressBlackPhantomCorpseBurn { get; set; }

    public Shader OverrideBlackPhantomShader { get; set; }

    public bool IsBuffEnemy { get; set; }

    public SpeculativeRigidbody TargetRigidbody
    {
      get
      {
        if ((UnityEngine.Object) this.OverrideTarget != (UnityEngine.Object) null)
        {
          if ((bool) (UnityEngine.Object) this.OverrideTarget)
            return this.OverrideTarget;
          this.OverrideTarget = (SpeculativeRigidbody) null;
        }
        return (UnityEngine.Object) this.PlayerTarget != (UnityEngine.Object) null ? this.PlayerTarget.specRigidbody : (SpeculativeRigidbody) null;
      }
    }

    public Vector2 TargetVelocity
    {
      get
      {
        if ((bool) (UnityEngine.Object) this.OverrideTarget)
        {
          PlayerController gameActor = this.OverrideTarget.gameActor as PlayerController;
          return (bool) (UnityEngine.Object) gameActor ? gameActor.AverageVelocity : this.OverrideTarget.Velocity;
        }
        if (!(bool) (UnityEngine.Object) this.PlayerTarget)
          return Vector2.zero;
        PlayerController playerTarget = this.PlayerTarget as PlayerController;
        return (bool) (UnityEngine.Object) playerTarget ? playerTarget.AverageVelocity : this.PlayerTarget.specRigidbody.Velocity;
      }
    }

    public float SpeculatorDelayTime { get; set; }

    public bool IsWorthShootingAt
    {
      get
      {
        return this.m_isWorthShootingAt.HasValue ? this.m_isWorthShootingAt.Value : !this.IsHarmlessEnemy;
      }
      set => this.m_isWorthShootingAt = new bool?(value);
    }

    public bool HasDonePlayerEnterCheck { get; set; }

    public bool PreventAutoKillOnBossDeath { get; set; }

    public string OverridePitfallAnim { get; set; }

    public event AIActor.CustomPitHandlingDelegate CustomPitDeathHandling;

    public GameActor PlayerTarget { get; set; }

    public SpeculativeRigidbody OverrideTarget { get; set; }

    public RoomHandler ParentRoom
    {
      get => this.parentRoom;
      set => this.parentRoom = value;
    }

    public bool HasBeenGlittered { get; set; }

    public bool IsTransmogrified { get; set; }

    public AIActor.ActorState State { get; set; }

    public bool HasBeenAwoken
    {
      get => this.State != AIActor.ActorState.Inactive && this.State != AIActor.ActorState.Awakening;
    }

    public AIActor.AwakenAnimationType AwakenAnimType { get; set; }

    public virtual bool InBossAmmonomiconTab
    {
      get
      {
        return (bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsBoss && !this.healthHaver.IsSubboss;
      }
    }

    public static AIActor Spawn(
      AIActor prefabActor,
      IntVector2 position,
      RoomHandler source,
      bool correctForWalls = false,
      AIActor.AwakenAnimationType awakenAnimType = AIActor.AwakenAnimationType.Default,
      bool autoEngage = true)
    {
      if (!(bool) (UnityEngine.Object) prefabActor)
        return (AIActor) null;
      GameObject objectToInstantiate = prefabActor.gameObject;
      if (prefabActor is AIActorDummy)
        objectToInstantiate = (prefabActor as AIActorDummy).realPrefab;
      GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(objectToInstantiate, source, position - source.area.basePosition, false, awakenAnimType, autoEngage);
      if (!(bool) (UnityEngine.Object) gameObject)
        return (AIActor) null;
      AIActor component = gameObject.GetComponent<AIActor>();
      if (!(bool) (UnityEngine.Object) component)
        return (AIActor) null;
      component.specRigidbody.Initialize();
      if (correctForWalls)
        component.CorrectForWalls();
      return component;
    }

    public static AIActor Spawn(
      AIActor prefabActor,
      Vector2 position,
      RoomHandler source,
      bool correctForWalls = false,
      AIActor.AwakenAnimationType awakenAnimType = AIActor.AwakenAnimationType.Default,
      bool autoEngage = true)
    {
      GameObject objectToInstantiate = prefabActor.gameObject;
      if (prefabActor is AIActorDummy)
        objectToInstantiate = (prefabActor as AIActorDummy).realPrefab;
      IntVector2 intVector2 = position.ToIntVector2(VectorConversions.Floor);
      GameObject gameObject = DungeonPlaceableUtility.InstantiateDungeonPlaceable(objectToInstantiate, source, intVector2 - source.area.basePosition, false, awakenAnimType, autoEngage);
      if (!(bool) (UnityEngine.Object) gameObject)
        return (AIActor) null;
      AIActor component = gameObject.GetComponent<AIActor>();
      if (!(bool) (UnityEngine.Object) component)
        return (AIActor) null;
      component.specRigidbody.Initialize();
      component.transform.position -= (Vector3) (component.specRigidbody.UnitCenter - position);
      component.specRigidbody.Reinitialize();
      if (correctForWalls)
        component.CorrectForWalls();
      return component;
    }

    private void CorrectForWalls()
    {
      if (!PhysicsEngine.Instance.OverlapCast(this.specRigidbody, collideWithRigidbodies: false))
        return;
      Vector2 vector2 = this.transform.position.XY();
      IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
      int num1 = 0;
      int num2 = 1;
      do
      {
        for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
        {
          this.transform.position = (Vector3) (vector2 + PhysicsEngine.PixelToUnit(cardinalsAndOrdinals[index] * num2));
          this.specRigidbody.Reinitialize();
          if (!PhysicsEngine.Instance.OverlapCast(this.specRigidbody, collideWithRigidbodies: false))
            return;
        }
        ++num2;
        ++num1;
      }
      while (num1 <= 200);
      UnityEngine.Debug.LogError((object) "FREEZE AVERTED!  TELL RUBEL!  (you're welcome) 147");
    }

    public override void Awake()
    {
      base.Awake();
      this.BaseMovementSpeed = this.MovementSpeed;
      this.m_currentlyAppliedEnemyScale = Vector2.one;
      StaticReferenceManager.AllEnemies.Add(this);
      if ((bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.healthIsNumberOfHits)
      {
        this.healthHaver.SetHealthMaximum(this.healthHaver.GetMaxHealth());
      }
      else
      {
        this.healthHaver.SetHealthMaximum(this.healthHaver.GetMaxHealth() * AIActor.BaseLevelHealthModifier);
        this.healthHaver.SetHealthMaximum(this.healthHaver.GetMaxHealth() * AIActor.HealthModifier);
      }
      if (GameManager.Instance.InTutorial && this.name.Contains("turret", true))
        this.HasDonePlayerEnterCheck = true;
      this.m_customEngageDoer = this.GetComponent<CustomEngageDoer>();
      this.m_customReinforceDoer = this.GetComponent<CustomReinforceDoer>();
      this.m_rigidbodyExcluder = new Func<SpeculativeRigidbody, bool>(this.RigidbodyBlocksLineOfSight);
      if ((UnityEngine.Object) this.aiShooter != (UnityEngine.Object) null)
        this.aiShooter.Initialize();
      this.InitializeCallbacks();
    }

    public override void Start()
    {
      base.Start();
      if (this.UsesVaryingEmissiveShaderPropertyBlock && this.sprite is tk2dSprite)
        (this.sprite as tk2dSprite).ApplyEmissivePropertyBlock = true;
      if (GameManager.Instance.InTutorial && this.name.Contains("turret", true))
      {
        List<AIActor> activeEnemies = this.parentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        Transform transform1 = this.transform;
        Transform transform2 = this.transform;
        for (int index = 0; index < activeEnemies.Count; ++index)
        {
          AIActor aiActor = activeEnemies[index];
          if (aiActor.name.Contains("turret", true))
          {
            if ((double) aiActor.transform.position.y < (double) transform1.position.y)
              transform1 = aiActor.transform;
            if ((double) aiActor.transform.position.y > (double) transform2.position.y)
              transform2 = aiActor.transform;
          }
        }
        if ((UnityEngine.Object) transform1 != (UnityEngine.Object) this.transform && (UnityEngine.Object) transform2 != (UnityEngine.Object) this.transform)
        {
          foreach (AIBulletBank.Entry bullet in this.bulletBank.Bullets)
            bullet.PlayAudio = false;
        }
      }
      if (this.PreventFallingInPitsEver && (bool) (UnityEngine.Object) this.specRigidbody)
        this.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.NoPitsMovementRestrictor);
      if (!string.IsNullOrEmpty(this.EnemySwitchState))
      {
        int num = (int) AkSoundEngine.SetSwitch("CHR_Enemy", this.EnemySwitchState, this.gameObject);
      }
      this.m_spriteDimensions = this.sprite.GetUntrimmedBounds().size;
      if (this.forceUsesTrimmedBounds)
        this.sprite.depthUsesTrimmedBounds = true;
      this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1f);
      DepthLookupManager.ProcessRenderer(this.renderer);
      this.m_spawnPosition = this.transform.position;
      if (this.HitByEnemyBullets)
        this.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile));
      if (this.HasShadow)
      {
        if (!(bool) (UnityEngine.Object) this.ShadowObject)
          this.GenerateDefaultBlobShadow(this.shadowHeightOffset);
        tk2dBaseSprite component = (tk2dBaseSprite) this.ShadowObject.GetComponent<tk2dSprite>();
        this.sprite.AttachRenderer(component);
        component.HeightOffGround = -0.05f;
        if ((bool) (UnityEngine.Object) this.ShadowParent)
        {
          component.transform.parent = this.ShadowParent;
          component.transform.localPosition = Vector3.zero;
        }
        if (GameManager.Instance.InTutorial && this.name.Contains("turret", true))
          component.renderer.enabled = false;
      }
      this.gameObject.GetOrAddComponent<AkGameObj>();
      this.m_lastPosition = this.specRigidbody.UnitCenter;
      foreach (PixelCollider pixelCollider in this.specRigidbody.PixelColliders)
      {
        if (pixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.BagelCollider && pixelCollider.CollisionLayer == CollisionLayer.BulletBlocker)
        {
          this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.ReflectBulletPreCollision);
          break;
        }
      }
      if ((this.PathableTiles & CellTypes.PIT) == CellTypes.PIT)
        this.SetIsFlying(true, "innate flight");
      this.InitializePalette();
      this.CheckForBlackPhantomness();
      if (this.procedurallyOutlined && (!this.m_forcedOutlines.HasValue || this.m_forcedOutlines.Value))
        this.SetOutlines(true);
      if (this.invisibleUntilAwaken)
      {
        if (this.State == AIActor.ActorState.Inactive)
          this.ToggleRenderers(false);
        if (!this.HasBeenAwoken)
        {
          this.specRigidbody.CollideWithOthers = false;
          this.IsGone = true;
          if ((bool) (UnityEngine.Object) this.knockbackDoer)
            this.knockbackDoer.SetImmobile(true, "awaken");
        }
      }
      if (GameManager.Instance.InTutorial && this.name.StartsWith("BulletManTutorial"))
        this.WanderHack();
      if (this.OnPostStartInitialization == null)
        return;
      this.OnPostStartInitialization();
    }

    public void SetOverrideOutlineColor(Color c)
    {
      this.OverrideOutlineColor = new Color?(c);
      if (!SpriteOutlineManager.HasOutline(this.sprite))
        return;
      Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(this.sprite);
      if ((UnityEngine.Object) outlineMaterial != (UnityEngine.Object) null)
        outlineMaterial.SetColor("_OverrideColor", c);
      HealthHaver healthHaver = this.healthHaver;
      if (!(bool) (UnityEngine.Object) healthHaver)
        return;
      healthHaver.UpdateCachedOutlineColor(outlineMaterial, c);
    }

    public void ClearOverrideOutlineColor()
    {
      this.OverrideOutlineColor = new Color?();
      if (!SpriteOutlineManager.HasOutline(this.sprite))
        return;
      Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(this.sprite);
      if (!((UnityEngine.Object) outlineMaterial != (UnityEngine.Object) null))
        return;
      outlineMaterial.SetColor("_OverrideColor", this.OutlineColor);
    }

    private Color OutlineColor
    {
      get
      {
        if (this.OverrideOutlineColor.HasValue)
          return this.OverrideOutlineColor.Value;
        return this.CanBeBloodthirsted ? Color.red : Color.black;
      }
    }

    public void SetOutlines(bool value)
    {
      if (!this.procedurallyOutlined)
        return;
      if (value)
      {
        if (!SpriteOutlineManager.HasOutline(this.sprite))
          SpriteOutlineManager.AddOutlineToSprite(this.sprite, this.OutlineColor, 0.1f);
        this.m_forcedOutlines = new bool?(true);
      }
      else
      {
        if (value)
          return;
        if (SpriteOutlineManager.HasOutline(this.sprite))
          SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
        this.m_forcedOutlines = new bool?(false);
      }
    }

    private void NoPitsMovementRestrictor(
      SpeculativeRigidbody specRigidbody,
      IntVector2 prevPixelOffset,
      IntVector2 pixelOffset,
      ref bool validLocation)
    {
      if (!(bool) (UnityEngine.Object) specRigidbody || specRigidbody.GroundPixelCollider == null || !GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) (specRigidbody.GroundPixelCollider.UnitCenter + PhysicsEngine.PixelToUnit(pixelOffset))))
        return;
      validLocation = false;
    }

    public string GetActorName()
    {
      if (!string.IsNullOrEmpty(this.OverrideDisplayName))
        return StringTableManager.GetEnemiesString(this.OverrideDisplayName);
      return (bool) (UnityEngine.Object) this.encounterTrackable ? this.encounterTrackable.journalData.GetPrimaryDisplayName() : StringTableManager.GetEnemiesString("#KILLEDBYDEFAULT");
    }

    private void UpdateTurboMode()
    {
      if ((bool) (UnityEngine.Object) this.CompanionOwner)
        return;
      if (this.m_cachedTurboness && !GameManager.IsTurboMode)
      {
        this.m_cachedTurboness = false;
        this.MovementSpeed /= TurboModeController.sEnemyMovementSpeedMultiplier;
        if ((bool) (UnityEngine.Object) this.behaviorSpeculator)
          this.behaviorSpeculator.CooldownScale /= TurboModeController.sEnemyCooldownMultiplier;
      }
      else if (!this.m_cachedTurboness && GameManager.IsTurboMode)
      {
        this.m_cachedTurboness = true;
        this.MovementSpeed *= TurboModeController.sEnemyMovementSpeedMultiplier;
        if ((bool) (UnityEngine.Object) this.behaviorSpeculator)
          this.behaviorSpeculator.CooldownScale *= TurboModeController.sEnemyCooldownMultiplier;
      }
      if (this.m_cachedTurboness && !this.m_turboWake && this.State == AIActor.ActorState.Awakening)
      {
        this.m_turboWake = true;
        if (!(bool) (UnityEngine.Object) this.aiAnimator)
          return;
        this.aiAnimator.FpsScale *= TurboModeController.sEnemyWakeTimeMultiplier;
      }
      else
      {
        if (this.m_cachedTurboness && this.State == AIActor.ActorState.Awakening || !this.m_turboWake)
          return;
        this.m_turboWake = false;
        if (!(bool) (UnityEngine.Object) this.aiAnimator)
          return;
        this.aiAnimator.FpsScale /= TurboModeController.sEnemyWakeTimeMultiplier;
      }
    }

    public override void Update()
    {
      base.Update();
      if (this.IsMimicEnemy)
      {
        RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
        if (roomFromPosition != null && roomFromPosition != this.parentRoom)
        {
          if (this.parentRoom != null)
            this.parentRoom.DeregisterEnemy(this);
          this.parentRoom = roomFromPosition;
          this.parentRoom.RegisterEnemy(this);
        }
      }
      if (this.ReflectsProjectilesWhileInvulnerable && (bool) (UnityEngine.Object) this.specRigidbody && (bool) (UnityEngine.Object) this.spriteAnimator)
      {
        this.specRigidbody.ReflectProjectiles = this.spriteAnimator.QueryInvulnerabilityFrame();
        this.specRigidbody.ReflectBeams = this.spriteAnimator.QueryInvulnerabilityFrame();
      }
      if (this.State == AIActor.ActorState.Awakening && (string.IsNullOrEmpty(this.m_awakenAnimation) || (bool) (UnityEngine.Object) this.aiAnimator && !this.aiAnimator.IsPlaying(this.m_awakenAnimation)))
      {
        if ((bool) (UnityEngine.Object) this.aiShooter)
        {
          this.aiShooter.ToggleGunAndHandRenderers(true, "Reinforce");
          this.aiShooter.ToggleGunAndHandRenderers(true, "Awaken");
        }
        this.State = AIActor.ActorState.Normal;
      }
      if (this.invisibleUntilAwaken)
      {
        if (this.State == AIActor.ActorState.Inactive && this.renderer.enabled)
          this.ToggleRenderers(false);
        if (this.State == AIActor.ActorState.Normal)
        {
          this.specRigidbody.CollideWithOthers = true;
          this.IsGone = false;
          if ((bool) (UnityEngine.Object) this.knockbackDoer)
            this.knockbackDoer.SetImmobile(false, "awaken");
          this.invisibleUntilAwaken = false;
        }
      }
      if ((this.PathableTiles & CellTypes.PIT) != CellTypes.PIT)
        this.HandlePitChecks();
      if (this.healthHaver.IsDead)
      {
        this.specRigidbody.Velocity = !this.PreventDeathKnockback ? this.m_knockbackVelocity : Vector2.zero;
      }
      else
      {
        CellVisualData.CellFloorType typeFromPosition = GameManager.Instance.Dungeon.GetFloorTypeFromPosition(this.specRigidbody.UnitBottomCenter);
        if (!this.m_prevFloorType.HasValue || this.m_prevFloorType.Value != typeFromPosition)
        {
          this.m_prevFloorType = new CellVisualData.CellFloorType?(typeFromPosition);
          int num = (int) AkSoundEngine.SetSwitch("FS_Surfaces", AIActor.s_floorTypeNames[(int) typeFromPosition], this.gameObject);
        }
        if ((UnityEngine.Object) this.aiShooter != (UnityEngine.Object) null)
          this.aiShooter.AimAtTarget();
        if (!this.isActiveAndEnabled)
          ;
        Vector2 movementVelocity = this.VoluntaryMovementVelocity;
        if (this.UseMovementAudio)
        {
          bool flag = movementVelocity != Vector2.zero;
          if (flag && !this.m_audioMovedLastFrame)
          {
            int num1 = (int) AkSoundEngine.PostEvent(this.StartMovingEvent, this.gameObject);
          }
          else if (!flag && this.m_audioMovedLastFrame)
          {
            int num2 = (int) AkSoundEngine.PostEvent(this.StopMovingEvent, this.gameObject);
          }
          this.m_audioMovedLastFrame = flag;
        }
        this.specRigidbody.Velocity = this.ApplyMovementModifiers(movementVelocity, this.m_knockbackVelocity) * this.LocalTimeScale;
        this.specRigidbody.Velocity += this.ImpartedVelocity;
        this.ImpartedVelocity = Vector2.zero;
        if (this.m_isSafeMoving)
        {
          this.m_safeMoveTimer += BraveTime.DeltaTime;
          this.transform.position = (Vector3) Vector2.Lerp(this.m_safeMoveStartPos.Value, this.m_safeMoveEndPos.Value, Mathf.Clamp01(this.m_safeMoveTimer / this.m_safeMoveTime));
          this.specRigidbody.Reinitialize();
          if ((double) this.m_safeMoveTimer >= (double) this.m_safeMoveTime)
            this.m_isSafeMoving = false;
        }
        this.m_lastPosition = this.specRigidbody.UnitCenter;
        if (this.IsBlackPhantom)
          this.UpdateBlackPhantomShaders();
        if (this.IsBlackPhantom || this.ForceBlackPhantomParticles)
          this.UpdateBlackPhantomParticles();
        this.ProcessHealthOverrides();
        if (!(bool) (UnityEngine.Object) this.healthHaver || !this.healthHaver.IsBoss)
          return;
        if ((double) this.FreezeAmount > 0.0)
          this.SetResistance(EffectResistanceType.Freeze, Mathf.Clamp(this.GetResistanceForEffectType(EffectResistanceType.Freeze) + 0.01f * BraveTime.DeltaTime, 0.6f, 1f));
        if (this.GetEffect(EffectResistanceType.Fire) == null)
          return;
        this.SetResistance(EffectResistanceType.Fire, Mathf.Clamp(this.GetResistanceForEffectType(EffectResistanceType.Fire) + 0.025f * BraveTime.DeltaTime, 0.25f, 0.75f));
      }
    }

    private void UpdateBlackPhantomShaders()
    {
      if (this.healthHaver.bodySprites.Count == this.m_cachedBodySpriteCount)
        return;
      this.m_cachedBodySpriteCount = this.healthHaver.bodySprites.Count;
      for (int index = 0; index < this.healthHaver.bodySprites.Count; ++index)
      {
        tk2dBaseSprite bodySprite = this.healthHaver.bodySprites[index];
        bodySprite.usesOverrideMaterial = true;
        Material material = bodySprite.renderer.material;
        if ((UnityEngine.Object) this.m_cachedBodySpriteShader == (UnityEngine.Object) null)
          this.m_cachedBodySpriteShader = material.shader;
        if (this.IsBlackPhantom)
        {
          if ((UnityEngine.Object) this.OverrideBlackPhantomShader != (UnityEngine.Object) null)
          {
            material.shader = this.OverrideBlackPhantomShader;
          }
          else
          {
            material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
            material.SetFloat("_PhantomGradientScale", this.BlackPhantomProperties.GradientScale);
            material.SetFloat("_PhantomContrastPower", this.BlackPhantomProperties.ContrastPower);
            if ((UnityEngine.Object) bodySprite != (UnityEngine.Object) this.sprite)
              material.SetFloat("_ApplyFade", 0.0f);
          }
        }
        else
          material.shader = this.m_cachedBodySpriteShader;
        bodySprite.renderer.material = material;
      }
      if (!(bool) (UnityEngine.Object) this.aiShooter || !(bool) (UnityEngine.Object) this.aiShooter.CurrentGun)
        return;
      tk2dBaseSprite sprite = this.aiShooter.CurrentGun.GetSprite();
      sprite.usesOverrideMaterial = true;
      Material material1 = sprite.renderer.material;
      if ((UnityEngine.Object) this.m_cachedGunSpriteShader == (UnityEngine.Object) null)
        this.m_cachedGunSpriteShader = material1.shader;
      if (this.IsBlackPhantom)
      {
        if ((UnityEngine.Object) this.OverrideBlackPhantomShader != (UnityEngine.Object) null)
        {
          material1.shader = this.OverrideBlackPhantomShader;
        }
        else
        {
          material1.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
          material1.SetFloat("_PhantomGradientScale", this.BlackPhantomProperties.GradientScale);
          material1.SetFloat("_PhantomContrastPower", this.BlackPhantomProperties.ContrastPower);
          material1.SetFloat("_ApplyFade", 0.3f);
        }
      }
      else
        material1.shader = this.m_cachedBodySpriteShader;
      sprite.renderer.material = material1;
    }

    private void UpdateBlackPhantomParticles()
    {
      if (!this.ShouldDoBlackPhantomParticles.HasValue)
        this.ShouldDoBlackPhantomParticles = !(bool) (UnityEngine.Object) this.GetComponent<DraGunDeathController>() ? new bool?(true) : new bool?(false);
      if (this.ShouldDoBlackPhantomParticles.HasValue && !this.ShouldDoBlackPhantomParticles.Value || !this.HasBeenEngaged || (bool) (UnityEngine.Object) this.sprite && !this.sprite.renderer.enabled || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
        return;
      PixelCollider pixelCollider = !this.OverrideBlackPhantomParticlesCollider ? this.specRigidbody.HitboxPixelCollider : this.specRigidbody.PixelColliders[this.BlackPhantomParticlesCollider];
      Vector3 vector3ZisY1 = pixelCollider.UnitBottomLeft.ToVector3ZisY();
      Vector3 vector3ZisY2 = pixelCollider.UnitTopRight.ToVector3ZisY();
      int num = Mathf.CeilToInt(Mathf.Max(1f, 40f * (float) (((double) vector3ZisY2.y - (double) vector3ZisY1.y) * ((double) vector3ZisY2.x - (double) vector3ZisY1.x)) * BraveTime.DeltaTime));
      GlobalSparksDoer.SparksType systemType = !this.ForceBlackPhantomParticles ? GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE : GlobalSparksDoer.SparksType.DARK_MAGICKS;
      GlobalSparksDoer.DoRandomParticleBurst(num, vector3ZisY1, vector3ZisY2, Vector3.up / 2f, 120f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: systemType);
      if ((double) UnityEngine.Random.value < 0.5)
        GlobalSparksDoer.DoRandomParticleBurst(1, vector3ZisY1, vector3ZisY2.WithY(vector3ZisY1.y + 0.1f), Vector3.right / 2f, 25f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: systemType);
      else
        GlobalSparksDoer.DoRandomParticleBurst(1, vector3ZisY1, vector3ZisY2.WithY(vector3ZisY1.y + 0.1f), Vector3.left / 2f, 25f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: systemType);
    }

    public void LateUpdate()
    {
      this.sprite.UpdateZDepth();
      this.UpdateTurboMode();
      if (!(bool) (UnityEngine.Object) this.renderer || !(bool) (UnityEngine.Object) this.renderer.material || !this.HasOverrideColor() || this.OverrideColorOverridden || !((UnityEngine.Object) this.m_colorOverridenMaterial != (UnityEngine.Object) this.renderer.material) && !((UnityEngine.Object) this.m_colorOverridenShader != (UnityEngine.Object) this.renderer.material.shader))
        return;
      this.OnOverrideColorsChanged();
    }

    protected virtual void OnWillRenderObject()
    {
      if (!Pixelator.IsRenderingReflectionMap)
        return;
      this.sprite.renderer.sharedMaterial.SetFloat("_ReflectionYOffset", this.sprite.GetBounds().min.y * 2f + this.actorReflectionAdditionalOffset);
    }

    protected override void OnDestroy()
    {
      if ((UnityEngine.Object) this.CurrentGun != (UnityEngine.Object) null)
        this.CurrentGun.DespawnVFX();
      StaticReferenceManager.AllEnemies.Remove(this);
      bool flag = (!(bool) (UnityEngine.Object) this.healthHaver ? 0 : (this.healthHaver.IsBoss ? 1 : 0)) == 0;
      if (GameManager.IsShuttingDown || GameManager.IsReturningToBreach || !GameManager.HasInstance || GameManager.Instance.IsLoadingLevel)
        flag = false;
      if (this.ParentRoom != null && flag)
        this.ParentRoom.DeregisterEnemy(this);
      if (this.parentRoom != null)
        this.parentRoom.Entered -= new RoomHandler.OnEnteredEventHandler(this.OnPlayerEntered);
      this.DeregisterCallbacks();
    }

    public void CompanionWarp(Vector3 targetPosition)
    {
      GameObject prefab = (GameObject) ResourceCache.Acquire("Global VFX/VFX_Breakable_Column_Puff");
      SpawnManager.SpawnVFX(prefab).GetComponent<tk2dSprite>().PlaceAtPositionByAnchor((Vector3) this.specRigidbody.UnitBottomCenter, tk2dBaseSprite.Anchor.LowerCenter);
      Vector2 vector = this.specRigidbody.UnitBottomCenter - this.transform.position.XY();
      this.transform.position = targetPosition - vector.ToVector3ZUp();
      this.specRigidbody.Reinitialize();
      this.CorrectForWalls();
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
      SpawnManager.SpawnVFX(prefab).GetComponent<tk2dSprite>().PlaceAtPositionByAnchor((Vector3) this.specRigidbody.UnitBottomCenter, tk2dBaseSprite.Anchor.LowerCenter);
    }

    public void WanderHack()
    {
      if ((bool) (UnityEngine.Object) this.behaviorSpeculator)
        this.behaviorSpeculator.enabled = false;
      this.StartCoroutine(this.WanderHackCR());
    }

    [DebuggerHidden]
    private IEnumerator WanderHackCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__WanderHackCRc__Iterator0()
      {
        _this = this
      };
    }

    public bool PathfindToPosition(
      Vector2 targetPosition,
      Vector2? overridePathEnd = null,
      bool smooth = true,
      CellValidator cellValidator = null,
      ExtraWeightingFunction extraWeightingFunction = null,
      CellTypes? overridePathableTiles = null,
      bool canPassOccupied = false)
    {
      bool position = false;
      Pathfinder.Instance.RemoveActorPath(this.m_upcomingPathTiles);
      CellTypes passableCellTypes = !overridePathableTiles.HasValue ? this.PathableTiles : overridePathableTiles.Value;
      Path path = (Path) null;
      if (Pathfinder.Instance.GetPath(this.PathTile, targetPosition.ToIntVector2(VectorConversions.Floor), out path, new IntVector2?(this.Clearance), passableCellTypes, cellValidator, extraWeightingFunction, canPassOccupied))
      {
        this.m_currentPath = path;
        this.m_overridePathEnd = overridePathEnd;
        if (this.m_currentPath != null && this.m_currentPath.WillReachFinalGoal)
          position = true;
        if (this.m_currentPath.Count == 0)
          this.m_currentPath = (Path) null;
        else if (smooth)
          path.Smooth(this.specRigidbody.UnitCenter, this.specRigidbody.UnitDimensions / 2f, passableCellTypes, canPassOccupied, this.Clearance);
      }
      this.UpdateUpcomingPathTiles(2f);
      Pathfinder.Instance.UpdateActorPath(this.m_upcomingPathTiles);
      return position;
    }

    public void FakePathToPosition(Vector2 targetPosition)
    {
      Pathfinder.Instance.RemoveActorPath(this.m_upcomingPathTiles);
      this.m_currentPath = (Path) null;
      this.m_overridePathEnd = new Vector2?(targetPosition);
    }

    public void ClearPath()
    {
      Pathfinder.Instance.RemoveActorPath(this.m_upcomingPathTiles);
      this.m_upcomingPathTiles.Clear();
      this.m_upcomingPathTiles.Add(this.PathTile);
      Pathfinder.Instance.UpdateActorPath(this.m_upcomingPathTiles);
      this.m_currentPath = (Path) null;
      this.m_overridePathEnd = new Vector2?();
    }

    private bool GetNextTargetPosition(out Vector2 targetPos)
    {
      if (this.m_currentPath != null && this.m_currentPath.Count > 0)
      {
        targetPos = this.m_currentPath.GetFirstCenterVector2();
        return true;
      }
      if (this.m_overridePathEnd.HasValue)
      {
        targetPos = this.m_overridePathEnd.Value;
        return true;
      }
      targetPos = Vector2.zero;
      return false;
    }

    private Vector2 GetPathTarget()
    {
      Vector2 unitCenter = this.specRigidbody.UnitCenter;
      Vector2 pathTarget = unitCenter;
      float num1 = this.MovementSpeed * this.LocalDeltaTime;
      Vector2 vector2 = unitCenter;
      Vector2 targetPos = unitCenter;
      while ((double) num1 > 0.0 && this.GetNextTargetPosition(out targetPos))
      {
        float num2 = Vector2.Distance(targetPos, unitCenter);
        if ((double) num2 < (double) num1)
        {
          num1 -= num2;
          vector2 = targetPos;
          pathTarget = vector2;
          if (this.m_currentPath != null && this.m_currentPath.Count > 0)
            this.m_currentPath.RemoveFirst();
          else
            this.m_overridePathEnd = new Vector2?();
        }
        else
        {
          pathTarget = (targetPos - vector2).normalized * num1 + vector2;
          break;
        }
      }
      return pathTarget;
    }

    private Vector2 GetPathVelocityContribution()
    {
      if (this.OverridePathVelocity.HasValue)
        return this.OverridePathVelocity.Value;
      if ((this.m_currentPath == null || this.m_currentPath.Count == 0) && !this.m_overridePathEnd.HasValue)
        return Vector2.zero;
      Vector2 unitCenter = this.specRigidbody.UnitCenter;
      Vector2 vector2 = this.GetPathTarget() - unitCenter;
      return (double) this.MovementSpeed * (double) this.LocalDeltaTime > (double) vector2.magnitude ? vector2 / this.LocalDeltaTime : this.MovementSpeed * vector2.normalized;
    }

    private Vector2 GetPathVelocityContribution_Old()
    {
      if ((this.m_currentPath == null || this.m_currentPath.Count == 0) && !this.m_overridePathEnd.HasValue)
        return Vector2.zero;
      Vector2 unitCenter = this.specRigidbody.UnitCenter;
      Vector2 vector2_1 = this.m_currentPath == null ? this.m_overridePathEnd.Value : this.m_currentPath.GetFirstCenterVector2();
      bool flag1 = (this.m_currentPath != null ? this.m_currentPath.Count : 0) + (this.m_overridePathEnd.HasValue ? 1 : 0) == 1;
      bool flag2 = false;
      if ((double) Vector2.Distance(unitCenter, vector2_1) < (double) PhysicsEngine.PixelToUnit(1))
        flag2 = true;
      else if (!flag1)
      {
        Vector2 b = BraveMathCollege.ClosestPointOnLineSegment(vector2_1, this.m_lastPosition, unitCenter);
        if ((double) Vector2.Distance(vector2_1, b) < (double) PhysicsEngine.PixelToUnit(1))
          flag2 = true;
      }
      if (flag2)
      {
        if (this.m_currentPath != null && this.m_currentPath.Count > 0)
        {
          this.m_currentPath.RemoveFirst();
          if (this.m_currentPath.Count == 0)
          {
            this.m_currentPath = (Path) null;
            return Vector2.zero;
          }
        }
        else if (this.m_overridePathEnd.HasValue)
          this.m_overridePathEnd = new Vector2?();
      }
      Vector2 vector2_2 = vector2_1 - unitCenter;
      return flag1 && (double) this.MovementSpeed * (double) this.LocalDeltaTime > (double) vector2_2.magnitude ? vector2_2 / this.LocalDeltaTime : this.MovementSpeed * vector2_2.normalized;
    }

    public void ReflectBulletPreCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherCollider)
    {
    }

    protected bool CheckTableRaycast(SpeculativeRigidbody source, SpeculativeRigidbody target)
    {
      if ((UnityEngine.Object) target == (UnityEngine.Object) null || (UnityEngine.Object) source == (UnityEngine.Object) null)
        return true;
      Vector2 unitCenter = source.GetUnitCenter(ColliderType.Ground);
      Vector2 direction = target.GetUnitCenter(ColliderType.Ground) - unitCenter;
      RaycastResult result;
      if (!PhysicsEngine.Instance.RaycastWithIgnores(unitCenter, direction, direction.magnitude, out result, false, rayMask: CollisionMask.LayerToMask(CollisionLayer.LowObstacle, CollisionLayer.HighObstacle), ignoreList: (ICollection<SpeculativeRigidbody>) new SpeculativeRigidbody[2]
      {
        source,
        target
      }))
        return true;
      RaycastResult.Pool.Free(ref result);
      return false;
    }

    protected virtual void OnCollision(CollisionData collision)
    {
      if (this.ManualKnockbackHandling)
        return;
      if (collision.collisionType == CollisionData.CollisionType.Rigidbody)
      {
        if (this.IsFrozen)
        {
          PlayerController component = collision.OtherRigidbody.GetComponent<PlayerController>();
          if ((bool) (UnityEngine.Object) component && collision.Overlap)
            component.specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
        }
        else
        {
          if (this.CanTargetPlayers)
          {
            PlayerController component = collision.OtherRigidbody.GetComponent<PlayerController>();
            if (!this.healthHaver.IsDead && (UnityEngine.Object) component != (UnityEngine.Object) null && this.CheckTableRaycast(collision.MyRigidbody, collision.OtherRigidbody))
            {
              Vector2 normalized = (component.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter).normalized;
              if (component.IsDodgeRolling)
                component.ApplyRollDamage(this);
              if (component.ReceivesTouchDamage)
              {
                float damage = this.CollisionDamage;
                if (this.IsBlackPhantom)
                  damage = 1f;
                if (this.IsCheezen)
                  damage = 0.0f;
                component.healthHaver.ApplyDamage(damage, normalized, this.GetActorName(), damageCategory: !this.IsBlackPhantom ? DamageCategory.Collision : DamageCategory.BlackBullet);
                if (Mathf.Approximately(normalized.magnitude, 0.0f))
                  normalized = UnityEngine.Random.insideUnitCircle.normalized;
                component.knockbackDoer.ApplySourcedKnockback(normalized, this.CollisionKnockbackStrength, this.gameObject);
                if ((bool) (UnityEngine.Object) this.knockbackDoer)
                  this.knockbackDoer.ApplySourcedKnockback(-normalized, component.collisionKnockbackStrength, this.gameObject);
              }
              else
              {
                if (Mathf.Approximately(normalized.magnitude, 0.0f))
                  normalized = UnityEngine.Random.insideUnitCircle.normalized;
                component.knockbackDoer.ApplySourcedKnockback(normalized, Mathf.Max(50f, this.CollisionKnockbackStrength), this.gameObject);
                if ((bool) (UnityEngine.Object) this.knockbackDoer)
                  this.knockbackDoer.ApplySourcedKnockback(-normalized, Mathf.Max(50f, component.collisionKnockbackStrength), this.gameObject);
              }
              if (this.CollisionSetsPlayerOnFire)
                component.IsOnFire = true;
              this.CollisionVFX.SpawnAtPosition((Vector3) collision.Contact, sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero), heightOffGround: new float?(2f));
              if (collision.Overlap)
                component.specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
              if (this.DiesOnCollison)
                this.healthHaver.ApplyDamage(1000f, -normalized, "Contact", damageCategory: DamageCategory.Unstoppable);
            }
          }
          if (this.CanTargetEnemies || this.OverrideHitEnemies)
          {
            AIActor component = collision.OtherRigidbody.GetComponent<AIActor>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null && !this.healthHaver.IsDead && (this.IsNormalEnemy || component.IsNormalEnemy))
            {
              Vector2 normalized = (component.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter).normalized;
              if (this.CanTargetEnemies)
                component.healthHaver.ApplyDamage(this.CollisionDamage * 5f, normalized, this.GetActorName(), this.CollisionDamageTypes, DamageCategory.Collision);
              if (Mathf.Approximately(normalized.magnitude, 0.0f))
                normalized = UnityEngine.Random.insideUnitCircle.normalized;
              if ((bool) (UnityEngine.Object) component.knockbackDoer)
                component.knockbackDoer.ApplySourcedKnockback(normalized, this.EnemyCollisionKnockbackStrength, this.gameObject);
              if ((bool) (UnityEngine.Object) this.knockbackDoer)
                this.knockbackDoer.ApplySourcedKnockback(-normalized, component.EnemyCollisionKnockbackStrength, this.gameObject);
              this.CollisionVFX.SpawnAtPosition((Vector3) collision.Contact, sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero), heightOffGround: new float?(2f));
              if (collision.Overlap)
                component.specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
              if (this.DiesOnCollison)
                this.healthHaver.ApplyDamage(1000f, -normalized, "Contact", damageCategory: DamageCategory.Unstoppable);
            }
          }
        }
      }
      if (!(bool) (UnityEngine.Object) collision.OtherRigidbody || !(bool) (UnityEngine.Object) collision.OtherRigidbody.gameActor)
        this.NonActorCollisionVFX.SpawnAtPosition((Vector3) collision.Contact, sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero), heightOffGround: new float?(2f));
      this.m_strafeDirection *= -1;
    }

    public Vector3 SpawnPosition => this.m_spawnPosition;

    public IntVector2 SpawnGridPosition => this.m_spawnPosition.IntXY(VectorConversions.Floor);

    public Vector3 Position => (Vector3) this.specRigidbody.UnitCenter;

    public IntVector2 GridPosition
    {
      get => this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
    }

    public IntVector2 PathTile
    {
      get => this.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
    }

    public bool PathComplete
    {
      get
      {
        return (this.m_currentPath == null || this.m_currentPath.Count == 0) && !this.m_overridePathEnd.HasValue;
      }
    }

    public Path Path => this.m_currentPath;

    public float DistanceToTarget
    {
      get
      {
        SpeculativeRigidbody targetRigidbody = this.TargetRigidbody;
        return (UnityEngine.Object) this.TargetRigidbody == (UnityEngine.Object) null ? 0.0f : Vector2.Distance(this.specRigidbody.UnitCenter, targetRigidbody.GetUnitCenter(ColliderType.HitBox));
      }
    }

    public bool RigidbodyBlocksLineOfSight(SpeculativeRigidbody testRigidbody)
    {
      return testRigidbody.gameObject.CompareTag("Intangible");
    }

    public bool HasLineOfSightToRigidbody(SpeculativeRigidbody targetRigidbody)
    {
      if ((UnityEngine.Object) targetRigidbody == (UnityEngine.Object) null)
        return false;
      Vector2 unitCenter = targetRigidbody.GetUnitCenter(ColliderType.HitBox);
      Vector2 vector2 = !(bool) (UnityEngine.Object) this.LosPoint ? this.specRigidbody.UnitCenter : this.LosPoint.transform.position.XY();
      float dist = Vector2.Distance(vector2, unitCenter);
      int enemyVisibilityMask = CollisionMask.GetComplexEnemyVisibilityMask(this.CanTargetPlayers, this.CanTargetEnemies);
      RaycastResult result;
      if (!PhysicsEngine.Instance.Raycast(vector2, unitCenter - vector2, dist, out result, rayMask: enemyVisibilityMask, rigidbodyExcluder: this.m_rigidbodyExcluder, ignoreRigidbody: this.specRigidbody))
      {
        RaycastResult.Pool.Free(ref result);
        return false;
      }
      if ((UnityEngine.Object) result.SpeculativeRigidbody == (UnityEngine.Object) null || (UnityEngine.Object) result.SpeculativeRigidbody != (UnityEngine.Object) targetRigidbody)
      {
        RaycastResult.Pool.Free(ref result);
        return false;
      }
      RaycastResult.Pool.Free(ref result);
      return true;
    }

    public bool HasLineOfSightToTarget
    {
      get
      {
        if ((UnityEngine.Object) this.TargetRigidbody != (UnityEngine.Object) this.m_cachedLosTarget || UnityEngine.Time.frameCount != this.m_cachedLosFrame)
        {
          this.m_cachedHasLineOfSightToTarget = this.HasLineOfSightToRigidbody(this.TargetRigidbody);
          this.m_cachedLosTarget = this.TargetRigidbody;
          this.m_cachedLosFrame = UnityEngine.Time.frameCount;
        }
        return this.m_cachedHasLineOfSightToTarget;
      }
    }

    public bool HasLineOfSightToTargetFromPosition(Vector2 hypotheticalPosition)
    {
      if ((UnityEngine.Object) this.TargetRigidbody == (UnityEngine.Object) null)
        return false;
      Vector2 unitCenter = this.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
      float distanceToTarget = this.DistanceToTarget;
      int enemyVisibilityMask = CollisionMask.GetComplexEnemyVisibilityMask(this.CanTargetPlayers, this.CanTargetEnemies);
      RaycastResult result;
      if (!PhysicsEngine.Instance.Raycast(hypotheticalPosition, unitCenter - hypotheticalPosition, distanceToTarget, out result, rayMask: enemyVisibilityMask, rigidbodyExcluder: this.m_rigidbodyExcluder, ignoreRigidbody: this.specRigidbody))
      {
        RaycastResult.Pool.Free(ref result);
        return false;
      }
      if ((UnityEngine.Object) result.SpeculativeRigidbody == (UnityEngine.Object) null || (UnityEngine.Object) result.SpeculativeRigidbody != (UnityEngine.Object) this.TargetRigidbody)
      {
        RaycastResult.Pool.Free(ref result);
        return false;
      }
      RaycastResult.Pool.Free(ref result);
      return true;
    }

    public float DesiredCombatDistance
    {
      get
      {
        return (UnityEngine.Object) this.behaviorSpeculator == (UnityEngine.Object) null ? -1f : this.behaviorSpeculator.GetDesiredCombatDistance();
      }
    }

    public IntVector2 Clearance
    {
      get
      {
        if (!this.m_clearance.HasValue)
          this.m_clearance = new IntVector2?(this.specRigidbody.UnitDimensions.ToIntVector2(VectorConversions.Ceil));
        return this.m_clearance.Value;
      }
    }

    private void CheckForBlackPhantomness()
    {
      if ((UnityEngine.Object) this.CompanionOwner != (UnityEngine.Object) null || !this.IsNormalEnemy || this.PreventBlackPhantom)
        return;
      int totalCurse = PlayerStats.GetTotalCurse();
      float num = totalCurse > 0 ? (totalCurse > 2 ? (totalCurse > 4 ? (totalCurse > 6 ? (totalCurse > 8 ? (totalCurse != 9 ? 0.5f : 0.25f) : 0.1f) : 0.05f) : 0.02f) : 0.01f) : 0.0f;
      if (this.healthHaver.IsBoss)
        num = totalCurse >= 7 ? (totalCurse >= 9 ? (totalCurse >= 10 ? 0.5f : 0.3f) : 0.2f) : 0.0f;
      if (!this.ForceBlackPhantom && (double) UnityEngine.Random.value >= (double) num)
        return;
      this.BecomeBlackPhantom();
    }

    public void BecomeBlackPhantom()
    {
      if (this.IsBlackPhantom)
        return;
      this.m_championType = AIActor.EnemyChampionType.JAMMED;
      this.m_cachedBodySpriteCount = -1;
      this.UpdateBlackPhantomShaders();
      if ((bool) (UnityEngine.Object) this.healthHaver && !this.healthHaver.healthIsNumberOfHits)
      {
        float healthPercentIncrease = this.BlackPhantomProperties.BonusHealthPercentIncrease;
        float healthFlatIncrease = this.BlackPhantomProperties.BonusHealthFlatIncrease;
        float num1;
        float num2;
        if (this.healthHaver.IsBoss)
        {
          num1 = healthPercentIncrease + BlackPhantomProperties.GlobalBossPercentIncrease;
          num2 = healthFlatIncrease + BlackPhantomProperties.GlobalBossFlatIncrease;
        }
        else
        {
          num1 = healthPercentIncrease + BlackPhantomProperties.GlobalPercentIncrease;
          num2 = healthFlatIncrease + BlackPhantomProperties.GlobalFlatIncrease;
        }
        float num3 = this.healthHaver.GetMaxHealth() * (1f + num1) + num2;
        if ((double) this.BlackPhantomProperties.MaxTotalHealth > 0.0 && !this.healthHaver.IsBoss)
          num3 = Mathf.Min(num3, this.BlackPhantomProperties.MaxTotalHealth * AIActor.BaseLevelHealthModifier);
        this.healthHaver.SetHealthMaximum(num3, keepHealthPercentage: true);
      }
      this.MovementSpeed *= this.BlackPhantomProperties.MovementSpeedMultiplier;
      if (!(bool) (UnityEngine.Object) this.behaviorSpeculator)
        return;
      this.behaviorSpeculator.CooldownScale /= this.BlackPhantomProperties.CooldownMultiplier;
    }

    public void UnbecomeBlackPhantom()
    {
      if (!this.IsBlackPhantom)
        return;
      this.m_championType = AIActor.EnemyChampionType.NORMAL;
      this.m_cachedBodySpriteCount = -1;
      this.UpdateBlackPhantomShaders();
      if ((bool) (UnityEngine.Object) this.healthHaver)
      {
        float healthPercentIncrease = this.BlackPhantomProperties.BonusHealthPercentIncrease;
        float healthFlatIncrease = this.BlackPhantomProperties.BonusHealthFlatIncrease;
        float num1;
        float num2;
        if (this.healthHaver.IsBoss)
        {
          num1 = healthPercentIncrease + BlackPhantomProperties.GlobalBossPercentIncrease;
          num2 = healthFlatIncrease + BlackPhantomProperties.GlobalBossFlatIncrease;
        }
        else
        {
          num1 = healthPercentIncrease + BlackPhantomProperties.GlobalPercentIncrease;
          num2 = healthFlatIncrease + BlackPhantomProperties.GlobalFlatIncrease;
        }
        float num3 = (float) (((double) this.healthHaver.GetMaxHealth() - (double) num2) / (1.0 + (double) num1));
        if ((double) this.BlackPhantomProperties.MaxTotalHealth > 0.0 && !this.healthHaver.IsBoss)
          num3 = Mathf.Max(num3, 10f);
        this.healthHaver.SetHealthMaximum(num3, keepHealthPercentage: true);
      }
      this.MovementSpeed /= this.BlackPhantomProperties.MovementSpeedMultiplier;
      if (!(bool) (UnityEngine.Object) this.behaviorSpeculator)
        return;
      this.behaviorSpeculator.CooldownScale *= this.BlackPhantomProperties.CooldownMultiplier;
    }

    private void InitializePalette()
    {
      if (!((UnityEngine.Object) this.optionalPalette != (UnityEngine.Object) null))
        return;
      this.m_isPaletteSwapped = true;
      this.sprite.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX;
      this.sprite.renderer.material.SetTexture("_PaletteTex", (Texture) this.optionalPalette);
    }

    private void ProcessHealthOverrides()
    {
      for (int index = 0; index < this.HealthOverrides.Count; ++index)
      {
        AIActor.HealthOverride healthOverride = this.HealthOverrides[index];
        if (!healthOverride.HasBeenUsed && (double) this.healthHaver.GetCurrentHealthPercentage() <= (double) healthOverride.HealthPercentage)
        {
          foreach (FieldInfo field in this.GetType().GetFields())
          {
            if (field.Name == healthOverride.Stat)
            {
              field.SetValue((object) this, (object) healthOverride.Value);
              healthOverride.HasBeenUsed = true;
              break;
            }
          }
          if (!healthOverride.HasBeenUsed)
          {
            UnityEngine.Debug.LogError((object) $"Failed to find the field {healthOverride.Stat} on AIActor.");
            healthOverride.HasBeenUsed = true;
          }
        }
      }
    }

    private void UpdateUpcomingPathTiles(float time)
    {
      this.m_upcomingPathTiles.Clear();
      this.m_upcomingPathTiles.Add(this.PathTile);
      if (this.m_currentPath == null || this.m_currentPath.Count <= 0)
        return;
      float num = 0.0f;
      Vector2 position = (Vector2) this.Position;
      LinkedListNode<IntVector2> linkedListNode = this.m_currentPath.Positions.First;
      Vector2 centerVector2 = linkedListNode.Value.ToCenterVector2();
      Vector2 vector2;
      for (; (double) num < (double) time; num += vector2.magnitude / this.MovementSpeed)
      {
        vector2 = centerVector2 - position;
        if ((double) vector2.sqrMagnitude > 0.039999999105930328)
          vector2 = vector2.normalized * 0.2f;
        position += vector2;
        IntVector2 intVector2 = position.ToIntVector2(VectorConversions.Floor);
        if (this.m_upcomingPathTiles[this.m_upcomingPathTiles.Count - 1] != intVector2)
          this.m_upcomingPathTiles.Add(intVector2);
        if ((double) vector2.magnitude < 0.20000000298023224)
        {
          linkedListNode = linkedListNode.Next;
          if (linkedListNode == null)
            break;
          centerVector2 = linkedListNode.Value.ToCenterVector2();
        }
      }
    }

    public int CurrentPhase
    {
      get => this.m_currentPhase;
      set => this.m_currentPhase = value;
    }

    public bool HasBeenEngaged
    {
      get => this.m_hasBeenEngaged;
      set
      {
        if (!value || this.m_hasBeenEngaged)
          return;
        this.OnEngaged();
      }
    }

    public bool IsReadyForRepath => this.m_isReadyForRepath;

    public Vector2 KnockbackVelocity
    {
      get => this.m_knockbackVelocity;
      set => this.m_knockbackVelocity = value;
    }

    public override Vector3 SpriteDimensions => this.m_spriteDimensions;

    public override Gun CurrentGun
    {
      get => (UnityEngine.Object) this.aiShooter != (UnityEngine.Object) null ? this.aiShooter.CurrentGun : (Gun) null;
    }

    public override bool SpriteFlipped
    {
      get
      {
        return (UnityEngine.Object) this.aiAnimator != (UnityEngine.Object) null ? this.aiAnimator.SpriteFlipped : this.sprite.FlipX;
      }
    }

    public override Transform GunPivot
    {
      get
      {
        return (UnityEngine.Object) this.aiShooter != (UnityEngine.Object) null ? this.aiShooter.gunAttachPoint : (Transform) null;
      }
    }

    public bool ManualKnockbackHandling { get; set; }

    public bool SuppressTargetSwitch { get; set; }

    private void OnEnable()
    {
      if (!this.invisibleUntilAwaken)
        return;
      if (this.State == AIActor.ActorState.Inactive)
        this.ToggleRenderers(false);
      if (this.HasBeenAwoken)
        return;
      this.specRigidbody.CollideWithOthers = false;
      this.IsGone = true;
      if (!(bool) (UnityEngine.Object) this.knockbackDoer)
        return;
      this.knockbackDoer.SetImmobile(true, "awaken");
    }

    private void OnDisable()
    {
    }

    public void ToggleRenderers(bool e)
    {
      tk2dSprite[] componentsInChildren = this.GetComponentsInChildren<tk2dSprite>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
        componentsInChildren[index].enabled = e;
      foreach (Behaviour componentsInChild in this.GetComponentsInChildren<tk2dSpriteAnimator>())
        componentsInChild.enabled = e;
      foreach (Renderer componentsInChild in this.GetComponentsInChildren<Renderer>())
        componentsInChild.enabled = e;
      if (e && this.m_forcedOutlines.HasValue && !this.m_forcedOutlines.Value)
      {
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          if (componentsInChildren[index].IsOutlineSprite)
            componentsInChildren[index].renderer.enabled = false;
        }
      }
      if (!(bool) (UnityEngine.Object) this.aiShooter || !e)
        return;
      this.aiShooter.UpdateGunRenderers();
      this.aiShooter.UpdateHandRenderers();
    }

    private void InitializeCallbacks()
    {
      if ((bool) (UnityEngine.Object) this.healthHaver)
      {
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.PreDeath);
        this.healthHaver.OnDeath += new Action<Vector2>(this.Die);
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.Damaged);
      }
      if ((bool) (UnityEngine.Object) this.spriteAnimator)
        this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
      if (!(bool) (UnityEngine.Object) this.specRigidbody)
        return;
      this.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
    }

    private void DeregisterCallbacks()
    {
      if ((bool) (UnityEngine.Object) this.healthHaver)
      {
        this.healthHaver.OnPreDeath -= new Action<Vector2>(this.PreDeath);
        this.healthHaver.OnDeath -= new Action<Vector2>(this.Die);
        this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.Damaged);
      }
      if ((bool) (UnityEngine.Object) this.spriteAnimator)
        this.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
      if (!(bool) (UnityEngine.Object) this.specRigidbody)
        return;
      this.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
    }

    protected void HandleAnimationEvent(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frameNo)
    {
      tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
      if (GameManager.AUDIO_ENABLED)
      {
        for (int index = 0; index < this.animationAudioEvents.Count; ++index)
        {
          if (this.animationAudioEvents[index].eventTag == frame.eventInfo)
          {
            int num = (int) AkSoundEngine.PostEvent(this.animationAudioEvents[index].eventName, this.gameObject);
          }
        }
      }
      if (this.procedurallyOutlined && frame.eventOutline != tk2dSpriteAnimationFrame.OutlineModifier.Unspecified)
      {
        if (frame.eventOutline == tk2dSpriteAnimationFrame.OutlineModifier.TurnOn)
          this.SetOutlines(true);
        else if (frame.eventOutline == tk2dSpriteAnimationFrame.OutlineModifier.TurnOff)
          this.SetOutlines(false);
      }
      if (this.State != AIActor.ActorState.Inactive && this.State != AIActor.ActorState.Awakening || !frame.finishedSpawning)
        return;
      this.specRigidbody.CollideWithOthers = true;
      this.IsGone = false;
      if ((bool) (UnityEngine.Object) this.knockbackDoer)
        this.knockbackDoer.SetImmobile(false, "awaken");
      this.healthHaver.IsVulnerable = true;
    }

    public void SkipOnEngaged() => this.m_hasBeenEngaged = true;

    public void DelayActions(float delay) => this.SpeculatorDelayTime += delay;

    public void ConfigureOnPlacement(RoomHandler room)
    {
      this.parentRoom = room;
      this.parentRoom.RegisterEnemy(this);
      this.parentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.OnPlayerEntered);
      if (!this.healthHaver.IsBoss || GameManager.Instance.InTutorial || GameManager.Instance.BestActivePlayer.CurrentRoom == room)
        return;
      if (!this.CanDropItems && (UnityEngine.Object) this.CustomLootTable != (UnityEngine.Object) null)
        room.OverrideBossRewardTable = this.CustomLootTable;
      foreach (SpeculativeRigidbody componentsInChild in this.GetComponentsInChildren<SpeculativeRigidbody>())
        componentsInChild.CollideWithOthers = false;
      this.IsGone = true;
    }

    private void OnPlayerEntered(PlayerController enterer)
    {
      if (this.HasDonePlayerEnterCheck || !this.isPassable)
        return;
      this.specRigidbody.Initialize();
      Vector2 unitCenter = GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter;
      bool flag = !Pathfinder.Instance.IsPassable(this.PathTile, new IntVector2?(this.Clearance), new CellTypes?(this.PathableTiles));
      if (flag)
        UnityEngine.Debug.LogErrorFormat("Tried to spawn a {0} in an invalid location in room {1}.", (object) this.name, (object) this.ParentRoom.GetRoomName());
      if ((bool) (UnityEngine.Object) this.GetComponent<KeyBulletManController>())
        this.TeleportSomewhere(keepClose: true);
      else if (flag || !this.IsHarmlessEnemy && (double) Vector2.Distance(unitCenter, this.specRigidbody.UnitCenter) < 8.0)
        this.TeleportSomewhere();
      this.HasDonePlayerEnterCheck = true;
    }

    public void TeleportSomewhere(IntVector2? overrideClearance = null, bool keepClose = false)
    {
      float sqrMinDist = 64f;
      float sqrMaxDist = 225f;
      PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
      Vector2 playerPosition = primaryPlayer.specRigidbody.UnitCenter;
      Vector2? otherPlayerPosition = new Vector2?();
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(primaryPlayer);
        if ((bool) (UnityEngine.Object) otherPlayer && (bool) (UnityEngine.Object) otherPlayer.healthHaver && otherPlayer.healthHaver.IsAlive)
          otherPlayerPosition = new Vector2?(otherPlayer.specRigidbody.UnitCenter);
      }
      IntVector2 clearance = !overrideClearance.HasValue ? this.Clearance : overrideClearance.Value;
      CellValidator cellValidator = (CellValidator) (c =>
      {
        if ((double) (playerPosition - c.ToCenterVector2()).sqrMagnitude <= (double) sqrMinDist || otherPlayerPosition.HasValue && (double) (otherPlayerPosition.Value - c.ToCenterVector2()).sqrMagnitude <= (double) sqrMinDist)
          return false;
        if (keepClose)
        {
          bool flag = false;
          if ((double) (playerPosition - c.ToCenterVector2()).sqrMagnitude <= (double) sqrMaxDist)
            flag = true;
          if (otherPlayerPosition.HasValue && (double) (otherPlayerPosition.Value - c.ToCenterVector2()).sqrMagnitude <= (double) sqrMaxDist)
            flag = true;
          if (!flag)
            return false;
        }
        for (int index1 = 0; index1 < clearance.x; ++index1)
        {
          for (int index2 = 0; index2 < clearance.y; ++index2)
          {
            if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(new IntVector2(c.x + index1, c.y + index2)) || GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2) || !GameManager.Instance.Dungeon.data[c.x + index1, c.y + index2].isGridConnected)
              return false;
          }
        }
        return true;
      });
      IntVector2? randomAvailableCell = this.ParentRoom.GetRandomAvailableCell(new IntVector2?(clearance), new CellTypes?(this.PathableTiles), cellValidator: cellValidator);
      if (!randomAvailableCell.HasValue)
        return;
      this.specRigidbody.Initialize();
      Vector2 vector2 = this.specRigidbody.UnitCenter - this.transform.position.XY();
      this.transform.position = (Vector3) BraveUtility.QuantizeVector(Pathfinder.GetClearanceOffset(randomAvailableCell.Value, this.Clearance) - vector2);
      this.specRigidbody.Reinitialize();
    }

    public void HandleReinforcementFallIntoRoom(float delay = 0.0f)
    {
      this.HasDonePlayerEnterCheck = true;
      this.IsInReinforcementLayer = true;
      this.StartCoroutine(this.HandleReinforcementFall_CR(delay));
    }

    protected void DisableOutlinesPostStart()
    {
      this.OnPostStartInitialization -= new System.Action(this.DisableOutlinesPostStart);
      if (this.procedurallyOutlined)
        this.SetOutlines(false);
      if (!this.HasShadow)
        return;
      this.ToggleShadowVisiblity(false);
    }

    [DebuggerHidden]
    private IEnumerator HandleReinforcementFall_CR(float delay)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__HandleReinforcementFall_CRc__Iterator1()
      {
        delay = delay,
        _this = this
      };
    }

    private void OnEngaged(bool isReinforcement = false)
    {
      if (this.m_hasBeenEngaged)
        return;
      if (this.SetsFlagOnActivation)
        GameStatsManager.Instance.SetFlag(this.FlagToSetOnActivation, true);
      if (!isReinforcement && (bool) (UnityEngine.Object) this.m_customEngageDoer && !this.m_customEngageDoer.IsFinished)
      {
        this.StartCoroutine(this.DoCustomEngage());
      }
      else
      {
        if (this.invisibleUntilAwaken)
          this.ToggleRenderers(true);
        if ((UnityEngine.Object) this.aiAnimator != (UnityEngine.Object) null && this.m_awakenAnimation == null)
          this.m_awakenAnimation = this.AwakenAnimType != AIActor.AwakenAnimationType.Spawn ? this.aiAnimator.PlayDefaultSpawnState() : this.aiAnimator.PlayDefaultAwakenedState();
        this.State = !string.IsNullOrEmpty(this.m_awakenAnimation) ? AIActor.ActorState.Awakening : AIActor.ActorState.Normal;
        if ((bool) (UnityEngine.Object) this.aiShooter && this.invisibleUntilAwaken && this.State == AIActor.ActorState.Awakening)
          this.aiShooter.ToggleGunAndHandRenderers(false, "Awaken");
        if (this.healthHaver.IsBoss && this.healthHaver.HasHealthBar)
        {
          GameUIBossHealthController healthController = !this.healthHaver.UsesVerticalBossBar ? (this.healthHaver.UsesSecondaryBossBar ? GameUIRoot.Instance.bossController2 : GameUIRoot.Instance.bossController) : GameUIRoot.Instance.bossControllerSide;
          string bossName = this.GetActorName();
          if (!string.IsNullOrEmpty(this.healthHaver.overrideBossName))
            bossName = StringTableManager.GetEnemiesString(this.healthHaver.overrideBossName);
          healthController.RegisterBossHealthHaver(this.healthHaver, bossName);
        }
        if ((UnityEngine.Object) this.OnEngagedVFX != (UnityEngine.Object) null)
        {
          Vector2 positionFromAnchor = this.sprite.GetRelativePositionFromAnchor(this.OnEngagedVFXAnchor);
          GameObject gameObject = SpawnManager.SpawnVFX(this.OnEngagedVFX);
          Transform transform = gameObject.transform;
          tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
          transform.parent = this.transform;
          transform.localPosition = positionFromAnchor.ToVector3ZUp(-0.1f);
          component.automaticallyManagesDepth = false;
          this.sprite.AttachRenderer((tk2dBaseSprite) component);
        }
        if (this.IdentifierForEffects == AIActor.EnemyTypeIdentifier.SNIPER_TYPE && PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.SNIPER_WOLF, out int _))
          this.ApplyEffect((GameActorEffect) GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect);
        this.m_hasBeenEngaged = true;
      }
    }

    [DebuggerHidden]
    private IEnumerator DoCustomEngage()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__DoCustomEngagec__Iterator2()
      {
        _this = this
      };
    }

    private void HandleLootPinata(int additionalMetas = 0)
    {
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
        return;
      List<GameObject> gameObjectList = new List<GameObject>();
      Vector3 vector3 = !(bool) (UnityEngine.Object) this.specRigidbody ? this.transform.position : this.specRigidbody.UnitCenter.ToVector3ZUp(this.transform.position.z);
      if (this.SpawnLootAtRewardChestPos && this.parentRoom != null)
      {
        IntVector2 chestSpawnPosition = this.parentRoom.area.runtimePrototypeData.rewardChestSpawnPosition;
        if (chestSpawnPosition.x >= 0 && chestSpawnPosition.y >= 0)
          vector3 = (this.parentRoom.area.UnitBottomLeft + chestSpawnPosition.ToCenterVector2()).ToVector3ZisY();
      }
      if (this.healthHaver.IsBoss && !this.parentRoom.HasOtherBoss(this))
      {
        int num;
        if (this.healthHaver.IsSubboss)
        {
          num = 1;
        }
        else
        {
          num = UnityEngine.Random.Range(GameManager.Instance.RewardManager.CurrentRewardData.MinMetaCurrencyFromBoss, GameManager.Instance.RewardManager.CurrentRewardData.MaxMetaCurrencyFromBoss + 1);
          switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
          {
            case GlobalDungeonData.ValidTilesets.HELLGEON:
            case GlobalDungeonData.ValidTilesets.RATGEON:
              num = 0;
              break;
          }
        }
        if (GameManager.Instance.InTutorial)
          num = 0;
        int amountToDrop1 = num + additionalMetas;
        if (GameManager.Instance.BestActivePlayer.CharacterUsesRandomGuns && (!ChallengeManager.CHALLENGE_MODE_ACTIVE || ChallengeManager.Instance.ChallengeMode != ChallengeModeType.ChallengeMegaMode))
          amountToDrop1 = 0;
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT)
          amountToDrop1 = UnityEngine.Random.Range(1, 3);
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
          amountToDrop1 = 0;
        int amountToDrop2 = this.AssignedCurrencyToDrop;
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH || GameManager.Instance.InTutorial)
          amountToDrop2 = 0;
        if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)
          amountToDrop2 = 0;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (GameManager.Instance.AllPlayers[index].IsDarkSoulsHollow && !GameManager.Instance.AllPlayers[index].IsGhost)
          {
            amountToDrop2 = 0;
            amountToDrop1 = 0;
          }
        }
        if (amountToDrop1 > 0)
        {
          if (this.ParentRoom != null && !this.ParentRoom.PlayerHasTakenDamageInThisRoom)
            amountToDrop1 *= 2;
          Vector2 vector2 = Vector2.down * 1.5f;
          if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON && (bool) (UnityEngine.Object) GameManager.Instance.BestActivePlayer && (bool) (UnityEngine.Object) this.specRigidbody)
            vector2 = BraveUtility.GetMajorAxis(GameManager.Instance.BestActivePlayer.CenterPosition - vector3.XY()).normalized * 1.5f;
          float startingHeight = 0.05f;
          float startingZForce = 4f;
          if ((bool) (UnityEngine.Object) this.specRigidbody)
          {
            startingHeight = Mathf.Max(0.05f, this.specRigidbody.UnitCenter.y - this.specRigidbody.UnitBottom);
            UnityEngine.Debug.Log((object) ("assigning SZH: " + (object) startingHeight));
          }
          LootEngine.SpawnCurrency(vector3.XY(), amountToDrop1, true, new Vector2?(vector2), new float?(45f), startingZForce, startingHeight);
        }
        if (PassiveItem.IsFlagSetAtAll(typeof (BankBagItem)))
          amountToDrop2 *= 2;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          if ((bool) (UnityEngine.Object) allPlayer)
          {
            float statValue = allPlayer.stats.GetStatValue(PlayerStats.StatType.MoneyMultiplierFromEnemies);
            if ((double) statValue != 1.0 && (double) statValue > 0.0)
              amountToDrop2 = Mathf.RoundToInt((float) amountToDrop2 * statValue);
          }
        }
        if (amountToDrop2 > 0)
          gameObjectList.AddRange((IEnumerable<GameObject>) GameManager.Instance.Dungeon.sharedSettingsPrefab.GetCurrencyToDrop(amountToDrop2));
      }
      else if ((this.CanDropCurrency || (double) this.AdditionalSingleCoinDropChance > 0.0) && !this.HasDamagedPlayer && !this.healthHaver.IsBoss)
      {
        GenericCurrencyDropSettings currencyDropSettings = GameManager.Instance.Dungeon.sharedSettingsPrefab.currencyDropSettings;
        int amountToDrop = !this.CanDropCurrency ? 0 : this.AssignedCurrencyToDrop;
        if ((double) this.AdditionalSingleCoinDropChance > 0.0 && (double) UnityEngine.Random.value < (double) this.AdditionalSingleCoinDropChance)
          ++amountToDrop;
        if (this.IsBlackPhantom)
          amountToDrop += currencyDropSettings.blackPhantomCoinDropChances.SelectByWeight();
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (GameManager.Instance.AllPlayers[index].IsDarkSoulsHollow && !GameManager.Instance.AllPlayers[index].IsGhost)
            amountToDrop = 0;
        }
        if (PassiveItem.IsFlagSetAtAll(typeof (BankBagItem)))
          amountToDrop *= 2;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          if ((bool) (UnityEngine.Object) allPlayer)
          {
            float statValue = allPlayer.stats.GetStatValue(PlayerStats.StatType.MoneyMultiplierFromEnemies);
            if ((double) statValue != 1.0 && (double) statValue > 0.0)
              amountToDrop = Mathf.RoundToInt((float) amountToDrop * statValue);
          }
        }
        if (amountToDrop > 0)
          gameObjectList.AddRange((IEnumerable<GameObject>) GameManager.Instance.Dungeon.sharedSettingsPrefab.GetCurrencyToDrop(amountToDrop));
      }
      if (this.AdditionalSimpleItemDrops.Count > 0)
      {
        for (int index = 0; index < this.AdditionalSimpleItemDrops.Count; ++index)
          gameObjectList.Add(this.AdditionalSimpleItemDrops[index].gameObject);
      }
      float num1 = 360f / (float) gameObjectList.Count;
      for (int index = 0; index < gameObjectList.Count; ++index)
      {
        Vector3 vector = Quaternion.Euler(0.0f, 0.0f, num1 * (float) index) * Vector3.up * 2f;
        float x = 0.0f;
        tk2dBaseSprite component = gameObjectList[index].GetComponent<tk2dBaseSprite>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          x = -1f * component.GetBounds().center.x;
        if ((bool) (UnityEngine.Object) gameObjectList[index].GetComponent<RobotArmBalloonsItem>() || (bool) (UnityEngine.Object) gameObjectList[index].GetComponent<RobotArmItem>())
        {
          LootEngine.SpawnItem(gameObjectList[index], vector3 + new Vector3(x, 0.0f, 0.0f), Vector2.zero, 0.5f, doDefaultItemPoof: true);
        }
        else
        {
          DebrisObject orAddComponent = SpawnManager.SpawnDebris(gameObjectList[index], vector3 + new Vector3(x, 0.0f, 0.0f), Quaternion.identity).GetOrAddComponent<DebrisObject>();
          orAddComponent.shouldUseSRBMotion = true;
          orAddComponent.angularVelocity = 0.0f;
          orAddComponent.Priority = EphemeralObject.EphemeralPriority.Critical;
          orAddComponent.Trigger(vector.WithZ(4f), 0.05f);
          orAddComponent.canRotate = false;
        }
      }
      if ((UnityEngine.Object) this.CustomChestTable != (UnityEngine.Object) null && (double) UnityEngine.Random.value < (double) this.ChanceToDropCustomChest)
      {
        GameObject gameObject = this.CustomChestTable.SelectByWeight();
        if ((bool) (UnityEngine.Object) gameObject)
        {
          IntVector2? randomAvailableCell = this.parentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR));
          IntVector2? nullable = !randomAvailableCell.HasValue ? new IntVector2?() : new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
          if (nullable.HasValue)
          {
            Chest chest = Chest.Spawn(gameObject.GetComponent<Chest>(), nullable.Value);
            if ((bool) (UnityEngine.Object) chest)
              chest.RegisterChestOnMinimap(this.parentRoom);
          }
        }
      }
      GameObject gameObject1 = (GameObject) null;
      if (this.CanDropItems && (UnityEngine.Object) this.CustomLootTable != (UnityEngine.Object) null)
      {
        int num2 = UnityEngine.Random.Range(this.CustomLootTableMinDrops, this.CustomLootTableMaxDrops);
        if (num2 == 1)
        {
          GameObject gameObject2 = this.CustomLootTable.SelectByWeight();
          if ((UnityEngine.Object) gameObject2 != (UnityEngine.Object) null)
            LootEngine.SpawnItem(gameObject2, vector3, Vector2.up, 1f, doDefaultItemPoof: true);
        }
        else
        {
          List<GameObject> itemsToSpawn = new List<GameObject>();
          for (int index1 = 0; index1 < num2; ++index1)
          {
            for (int index2 = 0; index2 < 3; ++index2)
            {
              gameObject1 = this.CustomLootTable.SelectByWeight();
              if (this.CanDropDuplicateItems || (UnityEngine.Object) gameObject1 == (UnityEngine.Object) null || !itemsToSpawn.Contains(gameObject1))
                break;
            }
            if ((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null && (this.CanDropDuplicateItems || !itemsToSpawn.Contains(gameObject1)))
              itemsToSpawn.Add(gameObject1);
          }
          LootEngine.SpewLoot(itemsToSpawn, vector3);
        }
      }
      if (this.AdditionalSafeItemDrops.Count > 0 && GameStatsManager.Instance.IsRainbowRun && this.IsMimicEnemy)
      {
        Vector2 vector2 = vector3.XY();
        RoomHandler absoluteRoom = vector2.GetAbsoluteRoom();
        LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteMimic, vector2, absoluteRoom, true);
      }
      else
      {
        for (int index = 0; index < this.AdditionalSafeItemDrops.Count; ++index)
        {
          RoomHandler roomHandler = this.parentRoom;
          if (this.IsMimicEnemy)
          {
            IntVector2 intVector2 = vector3.XY().ToIntVector2(VectorConversions.Floor);
            if (GameManager.Instance.Dungeon.data.GetRoomFromPosition(intVector2) == null)
            {
              LootEngine.SpawnItem(this.AdditionalSafeItemDrops[index].gameObject, vector3, Vector2.up, 1f, doDefaultItemPoof: true);
              continue;
            }
            roomHandler = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector2);
          }
          IntVector2 bestRewardLocation = roomHandler.GetBestRewardLocation(IntVector2.One, vector3.XY());
          LootEngine.SpawnItem(this.AdditionalSafeItemDrops[index].gameObject, (Vector3) bestRewardLocation.ToCenterVector2(), Vector2.zero, 0.0f);
        }
      }
    }

    public void EraseFromExistenceWithRewards(bool suppressDeathSounds = false)
    {
      this.HandleRewards();
      this.EraseFromExistence(suppressDeathSounds);
    }

    public void EraseFromExistence(bool suppressDeathSounds = false)
    {
      this.StealthDeath = true;
      if ((bool) (UnityEngine.Object) this.behaviorSpeculator)
        this.behaviorSpeculator.InterruptAndDisable();
      if (suppressDeathSounds)
        this.healthHaver.SuppressDeathSounds = true;
      this.healthHaver.ApplyDamage(1E+07f, Vector2.zero, "Erasure", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    private void PreDeath(Vector2 finalDamageDirection)
    {
      if ((bool) (UnityEngine.Object) this.aiAnimator)
        this.aiAnimator.FpsScale = 1f;
      if (this.shadowDeathType != AIActor.ShadowDeathType.None)
      {
        float scaleTime = 0.0f;
        if ((bool) (UnityEngine.Object) this.aiAnimator && this.aiAnimator.HasDirectionalAnimation("death"))
        {
          scaleTime = this.aiAnimator.GetDirectionalAnimationLength("death");
        }
        else
        {
          tk2dSpriteAnimationClip deathClip = this.healthHaver.GetDeathClip(finalDamageDirection.ToAngle());
          if (deathClip != null)
            scaleTime = (float) deathClip.frames.Length / deathClip.fps;
        }
        if ((double) scaleTime > 0.0)
        {
          if (this.shadowDeathType == AIActor.ShadowDeathType.Fade)
            this.StartCoroutine(this.FadeShadowCR(scaleTime));
          else
            this.StartCoroutine(this.ScaleShadowCR(scaleTime));
        }
      }
      if (this.healthHaver.IsBoss)
      {
        if (this.HasBeenGlittered)
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.KILL_BOSS_WITH_GLITTER);
        if (this.IsBlackPhantom)
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_A_JAMMED_BOSS);
      }
      else if (this.IsBlackPhantom && !this.SuppressBlackPhantomCorpseBurn)
        this.StartCoroutine(this.BurnBlackPhantomCorpse());
      if (!this.IsNormalEnemy)
        return;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive && allPlayer.IsInMinecart)
        {
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.ENEMIES_KILLED_WHILE_IN_CARTS, 1f);
          break;
        }
      }
    }

    [DebuggerHidden]
    private IEnumerator FadeShadowCR(float scaleTime)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__FadeShadowCRc__Iterator3()
      {
        scaleTime = scaleTime,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ScaleShadowCR(float scaleTime)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__ScaleShadowCRc__Iterator4()
      {
        scaleTime = scaleTime,
        _this = this
      };
    }

    public void Transmogrify(AIActor EnemyPrefab, GameObject EffectVFX)
    {
      if (this.IsTransmogrified && this.ActorName == EnemyPrefab.ActorName || this.IsMimicEnemy || !(bool) (UnityEngine.Object) this.healthHaver || this.healthHaver.IsBoss || !this.healthHaver.IsVulnerable || this.parentRoom == null)
        return;
      Vector2 centerPosition = this.CenterPosition;
      if ((UnityEngine.Object) EffectVFX != (UnityEngine.Object) null)
        SpawnManager.SpawnVFX(EffectVFX, (Vector3) centerPosition, Quaternion.identity);
      AIActor aiActor = AIActor.Spawn(EnemyPrefab, centerPosition.ToIntVector2(VectorConversions.Floor), this.parentRoom, true);
      if ((bool) (UnityEngine.Object) aiActor)
        aiActor.IsTransmogrified = true;
      if (EnemyPrefab.name == "Chicken")
      {
        int num1 = (int) AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", this.gameObject);
        int num2 = (int) AkSoundEngine.PostEvent("Play_PET_chicken_cluck_01", this.gameObject);
      }
      else if (EnemyPrefab.name == "Snake")
      {
        int num3 = (int) AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", this.gameObject);
      }
      else
      {
        int num4 = (int) AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", this.gameObject);
      }
      this.HandleRewards();
      this.EraseFromExistence();
    }

    private void Die(Vector2 finalDamageDirection) => this.ForceDeath(finalDamageDirection);

    [DebuggerHidden]
    private IEnumerator BurnBlackPhantomCorpse()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__BurnBlackPhantomCorpsec__Iterator5()
      {
        _this = this
      };
    }

    private void HandleRewards()
    {
      if (this.m_hasGivenRewards || this.IsTransmogrified)
        return;
      GameStatsManager.Instance.huntProgress.ProcessKill(this);
      int additionalMetas = 0;
      if (this.SetsFlagOnDeath)
      {
        if (this.FlagToSetOnDeath == GungeonFlags.TUTORIAL_COMPLETED && !GameStatsManager.Instance.GetFlag(GungeonFlags.TUTORIAL_RECEIVED_META_CURRENCY))
        {
          GameStatsManager.Instance.SetFlag(GungeonFlags.TUTORIAL_RECEIVED_META_CURRENCY, true);
          additionalMetas = 10;
        }
        GameStatsManager.Instance.SetFlag(this.FlagToSetOnDeath, true);
        if (this.FlagToSetOnDeath == GungeonFlags.BOSSKILLED_DRAGUN || this.FlagToSetOnDeath == GungeonFlags.BOSSKILLED_HIGHDRAGUN)
        {
          if (GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Robot)
            GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_DRAGUN_WITH_ROBOT, true);
          if (GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Bullet)
            GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_DRAGUN_WITH_BULLET, true);
          if (GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Eevee)
            GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_DRAGUN_PARADOX, true);
          if (GameManager.Instance.BestActivePlayer.CharacterUsesRandomGuns)
            GameStatsManager.Instance.SetFlag(GungeonFlags.SORCERESS_BLESSED_MODE_COMPLETE, true);
          if (GameManager.IsTurboMode)
          {
            GameStatsManager.Instance.SetFlag(GungeonFlags.BOSSKILLED_DRAGUN_TURBO_MODE, true);
            GameStatsManager.Instance.SetFlag(GungeonFlags.TONIC_TURBO_MODE_COMPLETE, true);
          }
        }
      }
      if (this.SetsCharacterSpecificFlagOnDeath)
        GameStatsManager.Instance.SetCharacterSpecificFlag(this.CharacterSpecificFlagToSetOnDeath, true);
      GameStatsManager.Instance.RegisterStatChange(TrackedStats.ENEMIES_KILLED, 1f);
      this.HandleLootPinata(additionalMetas);
      if (this.OnHandleRewards != null)
        this.OnHandleRewards();
      this.m_hasGivenRewards = true;
    }

    public void ForceDeath(Vector2 finalDamageDirection, bool allowCorpse = true)
    {
      EncounterTrackable component1 = this.GetComponent<EncounterTrackable>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        GameStatsManager.Instance.HandleEncounteredObject(component1);
      SpawnEnemyOnDeath component2 = this.GetComponent<SpawnEnemyOnDeath>();
      if ((bool) (UnityEngine.Object) component2)
        component2.ManuallyTrigger(finalDamageDirection);
      this.HandleRewards();
      if (!this.StealthDeath)
        this.OnCorpseVFX.SpawnAtPosition((Vector3) this.specRigidbody.GetUnitCenter(ColliderType.HitBox), sourceNormal: new Vector2?(Vector2.zero), sourceVelocity: new Vector2?(Vector2.zero));
      if ((UnityEngine.Object) this.CorpseObject != (UnityEngine.Object) null && !this.m_isFalling && allowCorpse && !this.StealthDeath)
      {
        if (this.IsBlackPhantom)
        {
          if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW && (bool) (UnityEngine.Object) this.sprite)
          {
            Vector3 vector3ZisY1 = this.sprite.WorldBottomLeft.ToVector3ZisY();
            Vector3 vector3ZisY2 = this.sprite.WorldTopRight.ToVector3ZisY();
            Vector3 vector3 = vector3ZisY2 - vector3ZisY1;
            Vector3 minPosition = vector3ZisY1 + vector3 * 0.15f;
            Vector3 maxPosition = vector3ZisY2 - vector3 * 0.15f;
            GlobalSparksDoer.DoRandomParticleBurst(Mathf.CeilToInt(40f * (float) (((double) maxPosition.y - (double) minPosition.y) * ((double) maxPosition.x - (double) minPosition.x))), minPosition, maxPosition, Vector3.up / 2f, 120f, 0.2f, startLifetime: new float?(UnityEngine.Random.Range(1f, 1.65f)), systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
          }
          this.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_BlackPhantomDeath") as GameObject, Vector3.zero, false);
        }
        else
        {
          GameObject gameObject = SpawnManager.SpawnDebris(this.CorpseObject, this.transform.position, Quaternion.identity);
          DebrisObject component3 = gameObject.GetComponent<DebrisObject>();
          if ((bool) (UnityEngine.Object) component3)
          {
            if (PassiveItem.IsFlagSetAtAll(typeof (CorpseExplodeActiveItem)))
              component3.Priority = EphemeralObject.EphemeralPriority.Critical;
            component3.IsCorpse = true;
          }
          StaticReferenceManager.AllCorpses.Add(gameObject);
          tk2dSprite component4 = gameObject.GetComponent<tk2dSprite>();
          CorpseSpawnController component5 = gameObject.GetComponent<CorpseSpawnController>();
          if ((bool) (UnityEngine.Object) component5)
            component5.Init(this);
          bool flag = true;
          if ((UnityEngine.Object) component4 != (UnityEngine.Object) null && (UnityEngine.Object) component5 == (UnityEngine.Object) null)
          {
            Material sharedMaterial = this.sprite.renderer.sharedMaterial;
            component4.SetSprite(this.sprite.Collection, this.sprite.spriteId);
            component4.OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_COMPLEX;
            if (this.CorpseShadow && !this.m_isPaletteSwapped)
            {
              Renderer renderer = component4.renderer;
              if (sharedMaterial.HasProperty("_OverrideColor") && (double) sharedMaterial.GetColor("_OverrideColor").a > 0.0)
              {
                renderer.material = sharedMaterial;
                Color color = this.CurrentOverrideColor;
                for (int index = 0; index < this.m_activeEffects.Count; ++index)
                {
                  if (this.m_activeEffects[index].AppliesDeathTint)
                  {
                    color = this.m_activeEffects[index].DeathTintColor;
                    renderer.material.SetFloat("_ValueMaximum", 0.6f);
                    renderer.material.SetFloat("_ValueMinimum", 0.2f);
                    if (renderer.material.shader.name.Contains("PixelShadow"))
                      renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUber");
                  }
                }
                renderer.material.SetColor("_OverrideColor", color);
                renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_OFF");
                renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                renderer.material.DisableKeyword("EMISSIVE_ON");
                renderer.material.EnableKeyword("EMISSIVE_OFF");
                flag = false;
              }
              else
                renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutFastPixelShadow");
            }
            else if (this.CorpseShadow && this.m_isPaletteSwapped)
            {
              Renderer renderer = component4.renderer;
              if (sharedMaterial.HasProperty("_OverrideColor") && (double) sharedMaterial.GetColor("_OverrideColor").a > 0.0)
              {
                renderer.material = sharedMaterial;
                Color color = this.CurrentOverrideColor;
                for (int index = 0; index < this.m_activeEffects.Count; ++index)
                {
                  if (this.m_activeEffects[index].AppliesDeathTint)
                  {
                    color = this.m_activeEffects[index].DeathTintColor;
                    renderer.material.SetFloat("_ValueMaximum", 0.6f);
                    renderer.material.SetFloat("_ValueMinimum", 0.2f);
                  }
                }
                renderer.material.SetColor("_OverrideColor", color);
                renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_OFF");
                renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_ON");
                renderer.material.DisableKeyword("EMISSIVE_ON");
                renderer.material.EnableKeyword("EMISSIVE_OFF");
                flag = false;
              }
              else
              {
                renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutoutFastPixelShadowPalette");
                renderer.material.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
                renderer.material.SetTexture("_PaletteTex", sharedMaterial.GetTexture("_PaletteTex"));
              }
            }
            else if (this.m_isPaletteSwapped)
              component4.renderer.material = sharedMaterial;
            if (this.TransferShadowToCorpse && (bool) (UnityEngine.Object) this.ShadowObject)
              this.ShadowObject.transform.parent = gameObject.transform;
            component4.IsPerpendicular = false;
            component4.HeightOffGround = -1f;
            component4.UpdateZDepth();
          }
          if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
          {
            if (finalDamageDirection != Vector2.zero)
              finalDamageDirection.Normalize();
            component3.Trigger((Vector3) finalDamageDirection, 0.1f);
            if (flag)
              component3.FadeToOverrideColor(new Color(0.0f, 0.0f, 0.0f, 0.6f), 0.25f);
            component3.AssignFinalWorldDepth(-1.25f);
          }
        }
      }
      if (this.IsMimicEnemy)
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && GameManager.Instance.AllPlayers[index].OnChestBroken != null)
            GameManager.Instance.AllPlayers[index].OnChestBroken(GameManager.Instance.AllPlayers[index], (Chest) null);
        }
      }
      if (this.healthHaver.IsBoss && !this.healthHaver.IsSubboss && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE)
      {
        bool flag = false;
        List<AIActor> activeEnemies = this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        for (int index = 0; index < activeEnemies.Count; ++index)
        {
          HealthHaver healthHaver = activeEnemies[index].healthHaver;
          if ((bool) (UnityEngine.Object) healthHaver && healthHaver.IsBoss && healthHaver.IsAlive && (UnityEngine.Object) activeEnemies[index] != (UnityEngine.Object) this.aiActor)
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)
          {
            if ((bool) (UnityEngine.Object) this.GetComponent<InfinilichDeathController>())
              GameManager.Instance.Dungeon.FloorCleared();
          }
          else
            GameManager.Instance.Dungeon.FloorCleared();
        }
      }
      if (this.parentRoom != null)
        this.parentRoom.DeregisterEnemy(this);
      else
        UnityEngine.Debug.LogError((object) "An enemy who does not have a parent room is dying... this could be a problem.");
      Pathfinder.Instance.RemoveActorPath(this.m_upcomingPathTiles);
    }

    private void Damaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      if (!this.HasBeenEngaged)
        this.HasBeenEngaged = true;
      if (damageCategory == DamageCategory.DamageOverTime || !(bool) (UnityEngine.Object) this.aiAnimator)
        return;
      this.aiAnimator.PlayHitState(damageDirection);
    }

    public void StrafeTarget(float targetDistance)
    {
    }

    public void JumpToPoint(Vector2 targetPoint, float speedMultiplier, float jumpHeight)
    {
      float jumpTime = Vector2.Distance(this.transform.position.XY(), targetPoint) / (this.MovementSpeed * speedMultiplier);
      this.StartCoroutine(this.HandleJumpToPoint(targetPoint, jumpTime, jumpHeight));
    }

    [DebuggerHidden]
    private IEnumerator HandleJumpToPoint(
      Vector2 flattenedEndPosition,
      float jumpTime,
      float jumpHeight)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__HandleJumpToPointc__Iterator6()
      {
        jumpTime = jumpTime,
        _this = this
      };
    }

    public void SetAIMovementContribution(Vector2 vel) => this.m_currentPath = (Path) null;

    [DebuggerHidden]
    private IEnumerator DashInDirection(Vector3 direction, float duration, float speedMultiplier)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__DashInDirectionc__Iterator7()
      {
        duration = duration,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator DashByDistance(Vector3 direction, float distance, float speedMultiplier)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__DashByDistancec__Iterator8()
      {
        direction = direction,
        speedMultiplier = speedMultiplier,
        distance = distance,
        _this = this
      };
    }

    public void SimpleMoveToPosition(Vector3 targetPosition)
    {
      IntVector2 intVector2 = targetPosition.IntXY();
      Path path = new Path();
      path.Positions = new LinkedList<IntVector2>();
      path.Positions.AddFirst(intVector2);
      this.m_currentPath = path;
    }

    private Vector2 CalculateTargetStrafeVelocity(
      Vector3 targetPosition,
      int direction,
      float targetDistance)
    {
      Vector2 vector2 = (Vector2) targetPosition - this.specRigidbody.UnitCenter;
      float magnitude = vector2.magnitude;
      float num = 90f;
      if ((double) magnitude > (double) targetDistance)
        num = 45f;
      return (Vector2) ((Quaternion.Euler(0.0f, 0.0f, num * Mathf.Sign((float) direction)) * new Vector3(vector2.x, vector2.y, 0.0f)).normalized * this.MovementSpeed);
    }

    private Vector2 CalculateSteering()
    {
      if (!this.TryDodgeBullets)
        return Vector2.zero;
      float num1 = 5f;
      Collider[] colliderArray = Physics.OverlapSphere((Vector3) this.specRigidbody.UnitCenter, this.AvoidRadius);
      Vector2 steering = Vector2.zero;
      int num2 = 0;
      foreach (Collider collider in colliderArray)
      {
        if ((UnityEngine.Object) collider.transform.parent != (UnityEngine.Object) null && (UnityEngine.Object) collider.transform.parent.GetComponent<Projectile>() != (UnityEngine.Object) null)
        {
          Vector2 velocity = collider.transform.parent.GetComponent<SpeculativeRigidbody>().Velocity;
          float num3 = Vector3.Distance(collider.transform.position, (Vector3) this.specRigidbody.UnitCenter);
          Vector3 b = collider.transform.position + new Vector3(velocity.normalized.x * num3, velocity.normalized.y * num3, 0.0f);
          if ((double) Vector3.Distance((Vector3) this.specRigidbody.UnitCenter, b) <= (double) Vector3.Distance((Vector3) this.specRigidbody.UnitCenter, collider.transform.position))
          {
            int num4 = (double) this.specRigidbody.UnitCenter.x >= (double) collider.transform.position.x ? 1 : -1;
            Vector2 vector2_1 = (this.specRigidbody.UnitCenter - (Vector2) collider.transform.position) * (float) num4;
            Vector2 vector2_2 = (Vector2) ((b - collider.transform.position) * (float) num4);
            int z = (double) Mathf.Atan2(vector2_1.y, vector2_1.x) <= (double) Mathf.Atan2(vector2_2.y, vector2_2.x) ? -90 : 90;
            float num5 = num3 / this.AvoidRadius;
            Vector3 vector3 = Quaternion.Euler(0.0f, 0.0f, (float) z) * new Vector3(velocity.x, velocity.y, 0.0f);
            Vector2 normalized = new Vector2(vector3.x, vector3.y).normalized;
            steering += normalized * (1f - num5);
            ++num2;
          }
        }
      }
      if (num2 > 0)
        steering = steering / (float) num2 * num1;
      return steering;
    }

    protected override void Fall()
    {
      if (this.m_isFalling)
        return;
      base.Fall();
      if ((bool) (UnityEngine.Object) this.behaviorSpeculator)
        this.behaviorSpeculator.InterruptAndDisable();
      if ((bool) (UnityEngine.Object) this.aiShooter)
        this.aiShooter.ToggleGunAndHandRenderers(false, "Pitfall");
      if ((bool) (UnityEngine.Object) this.aiAnimator)
      {
        this.aiAnimator.FpsScale = 1f;
        if (!string.IsNullOrEmpty(this.OverridePitfallAnim))
          this.aiAnimator.PlayUntilCancelled(this.OverridePitfallAnim);
        else if (this.aiAnimator.HasDirectionalAnimation("pitfall"))
          this.aiAnimator.PlayUntilCancelled("pitfall");
        else if (this.spriteAnimator.GetClipByName("pitfall") != null)
          this.aiAnimator.PlayUntilCancelled("pitfall");
        else if (this.spriteAnimator.GetClipByName("pitfall_right") != null)
          this.aiAnimator.PlayUntilCancelled("pitfall_right");
      }
      this.StartCoroutine(this.FallDownCR());
    }

    public bool HasSplashed { get; set; }

    [DebuggerHidden]
    private IEnumerator FallDownCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AIActor__FallDownCRc__Iterator9()
      {
        _this = this
      };
    }

    public override void RecoverFromFall()
    {
      base.RecoverFromFall();
      if ((bool) (UnityEngine.Object) this.behaviorSpeculator)
        this.behaviorSpeculator.enabled = true;
      if ((bool) (UnityEngine.Object) this.aiShooter)
        this.aiShooter.ToggleGunAndHandRenderers(true, "Pitfall");
      this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
      if ((bool) (UnityEngine.Object) this.aiAnimator)
        this.aiAnimator.EndAnimation();
      this.specRigidbody.CollideWithTileMap = true;
      this.specRigidbody.CollideWithOthers = true;
      this.IsGone = false;
      this.renderer.enabled = true;
      SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, true);
    }

    private void PitFallMovementRestrictor(
      SpeculativeRigidbody specRigidbody,
      IntVector2 prevPixelOffset,
      IntVector2 pixelOffset,
      ref bool validLocation)
    {
      if (!validLocation)
        return;
      PixelCollider hitboxPixelCollider = specRigidbody.HitboxPixelCollider;
      Vector2 unitMidpoint1 = PhysicsEngine.PixelToUnitMidpoint(hitboxPixelCollider.UpperLeft);
      Vector2 unitMidpoint2 = PhysicsEngine.PixelToUnitMidpoint(hitboxPixelCollider.UpperRight);
      Vector2 unitMidpoint3 = PhysicsEngine.PixelToUnitMidpoint(hitboxPixelCollider.LowerLeft);
      Vector2 unitMidpoint4 = PhysicsEngine.PixelToUnitMidpoint(hitboxPixelCollider.LowerRight);
      Vector2 unit1 = PhysicsEngine.PixelToUnit(prevPixelOffset);
      Vector2 unit2 = PhysicsEngine.PixelToUnit(pixelOffset);
      if ((!GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint1 + unit1)) || GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint1 + unit2))) && (!GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint2 + unit1)) || GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint2 + unit2))) && (!GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint3 + unit1)) || GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint3 + unit2))) && (!GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint4 + unit1)) || GameManager.Instance.Dungeon.CellIsPit((Vector3) (unitMidpoint4 + unit2))))
        return;
      validLocation = false;
    }

    public void MoveToSafeSpot(float time)
    {
      this.m_isSafeMoving = false;
      if (!GameManager.HasInstance || (UnityEngine.Object) GameManager.Instance.Dungeon == (UnityEngine.Object) null)
        return;
      DungeonData data = GameManager.Instance.Dungeon.data;
      Vector2[] vector2Array = new Vector2[6]
      {
        this.specRigidbody.UnitBottomLeft,
        this.specRigidbody.UnitBottomCenter,
        this.specRigidbody.UnitBottomRight,
        this.specRigidbody.UnitTopLeft,
        this.specRigidbody.UnitTopCenter,
        this.specRigidbody.UnitTopRight
      };
      bool flag = false;
      for (int index = 0; index < vector2Array.Length; ++index)
      {
        IntVector2 intVector2 = vector2Array[index].ToIntVector2(VectorConversions.Floor);
        if (!data.CheckInBoundsAndValid(intVector2) || data.isWall(intVector2) || data.isTopWall(intVector2.x, intVector2.y) || data[intVector2].isOccupied)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
      CellValidator cellValidator = (CellValidator) (c =>
      {
        for (int index1 = 0; index1 < this.Clearance.x; ++index1)
        {
          int x = c.x + index1;
          for (int index2 = 0; index2 < this.Clearance.y; ++index2)
          {
            int y = c.y + index2;
            if (GameManager.Instance.Dungeon.data.isTopWall(x, y))
              return false;
          }
        }
        return true;
      });
      Vector2 vector2 = this.specRigidbody.UnitBottomCenter - this.transform.position.XY();
      IntVector2? nearestAvailableCell = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor)).GetNearestAvailableCell(this.specRigidbody.UnitCenter, new IntVector2?(this.Clearance), new CellTypes?(this.PathableTiles), cellValidator: cellValidator);
      if (nearestAvailableCell.HasValue)
      {
        this.m_isSafeMoving = true;
        this.m_safeMoveTimer = 0.0f;
        this.m_safeMoveTime = time;
        this.m_safeMoveStartPos = new Vector2?((Vector2) this.transform.position);
        this.m_safeMoveEndPos = new Vector2?(Pathfinder.GetClearanceOffset(nearestAvailableCell.Value, this.Clearance).WithY((float) nearestAvailableCell.Value.y) - vector2);
      }
      else
      {
        this.m_safeMoveStartPos = new Vector2?();
        this.m_safeMoveEndPos = new Vector2?();
      }
    }

    public enum ReinforceType
    {
      FullVfx,
      SkipVfx,
      Instant,
    }

    public enum ShadowDeathType
    {
      Fade = 10, // 0x0000000A
      Scale = 20, // 0x00000014
      None = 30, // 0x0000001E
    }

    public enum EnemyTypeIdentifier
    {
      UNIDENTIFIED,
      SNIPER_TYPE,
    }

    public enum EnemyChampionType
    {
      NORMAL,
      JAMMED,
      KTHULIBER_JAMMED,
    }

    public delegate void CustomPitHandlingDelegate(AIActor actor, ref bool suppressDamage);

    public enum ActorState
    {
      Inactive,
      Awakening,
      Normal,
    }

    public enum AwakenAnimationType
    {
      Default,
      Awaken,
      Spawn,
    }

    [Serializable]
    public class HealthOverride
    {
      public float HealthPercentage;
      public string Stat;
      public float Value;
      [NonSerialized]
      public bool HasBeenUsed;
    }
  }

