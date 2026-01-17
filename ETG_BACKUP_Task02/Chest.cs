// Decompiled with JetBrains decompiler
// Type: Chest
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
public class Chest : DungeonPlaceableBehaviour, IPlaceConfigurable, IPlayerInteractable
{
  public static bool HasDroppedResourcefulRatNoteThisSession;
  public static bool DoneResourcefulRatMimicThisSession;
  public static bool HasDroppedSerJunkanThisSession;
  protected const float MULTI_ITEM_SPREAD_FACTOR = 1f;
  protected const float ITEM_PRESENTATION_SPEED = 1.5f;
  public Chest.SpecialChestIdentifier ChestIdentifier;
  private Chest.GeneralChestType m_chestType;
  [NonSerialized]
  public List<PickupObject> contents;
  [PickupIdentifier]
  public List<int> forceContentIds;
  public LootData lootTable;
  public float breakertronNothingChance = 0.1f;
  public LootData breakertronLootTable;
  public bool prefersDungeonProcContents;
  public tk2dSprite ShadowSprite;
  public bool pickedUp;
  [CheckAnimation(null)]
  public string spawnAnimName;
  [CheckAnimation(null)]
  public string openAnimName;
  [CheckAnimation(null)]
  public string breakAnimName;
  [NonSerialized]
  private string overrideSpawnAnimName;
  [NonSerialized]
  private string overrideOpenAnimName;
  [NonSerialized]
  private string overrideBreakAnimName;
  [PickupIdentifier]
  public int overrideJunkId = -1;
  public GameObject VFX_PreSpawn;
  public GameObject VFX_GroundHit;
  public float groundHitDelay = 0.73f;
  public Transform spawnTransform;
  public AnimationCurve spawnCurve;
  public tk2dSpriteAnimator LockAnimator;
  public string LockOpenAnim = "lock_open";
  public string LockBreakAnim = "lock_break";
  public string LockNoKeyAnim = "lock_nokey";
  public tk2dSpriteAnimator[] SubAnimators;
  [NonSerialized]
  private GameObject minimapIconInstance;
  [NonSerialized]
  public bool IsLockBroken;
  public bool IsLocked;
  public bool IsSealed;
  public bool IsOpen;
  public bool IsBroken;
  public bool AlwaysBroadcastsOpenEvent;
  [NonSerialized]
  public float GeneratedMagnificence;
  protected bool m_temporarilyUnopenable;
  public bool IsRainbowChest;
  public bool IsMirrorChest;
  [NonSerialized]
  public bool ForceGlitchChest;
  protected bool m_isKeyOpening;
  private RoomHandler m_room;
  private RoomHandler m_registeredIconRoom;
  private bool m_isMimic;
  private bool m_isMimicBreathing;
  private System.Random m_runtimeRandom;
  [EnemyIdentifier]
  [Header("Mimic")]
  public string MimicGuid;
  public IntVector2 mimicOffset;
  [CheckAnimation(null)]
  public string preMimicIdleAnim;
  public float preMimicIdleAnimDelay = 3f;
  public float overrideMimicChance = -1f;
  private static bool m_IsCoopMode;
  public GameObject MinimapIconPrefab;
  private const float SPAWN_PUSH_RADIUS = 5f;
  private const float SPAWN_PUSH_FORCE = 22f;
  private bool m_hasCheckedBowler;
  private GameObject m_bowlerInstance;
  private Tribool m_bowlerFireStatus;
  private bool m_secretRainbowRevealed;
  private float m_bowlerFireTimer;
  private Color m_cachedOutlineColor;
  [NonSerialized]
  private ChestFuseController extantFuse;
  private const float RESOURCEFULRAT_CHEST_NOTE_CHANCE = 10.025f;
  private bool m_forceDropOkayForRainbowRun;
  private bool m_isGlitchChest;
  private bool m_cachedLockedState;
  private int m_cachedShadowSpriteID = -1;
  [NonSerialized]
  private int m_cachedSpriteForCoop = -1;
  private IntVector2 m_cachedCoopManualOffset;
  private IntVector2 m_cachedCoopManualDimensions;
  private bool m_configured;
  private bool m_hasBeenCheckedForFuses;
  [NonSerialized]
  public bool PreventFuse;

  public Chest.GeneralChestType ChestType
  {
    get => this.m_chestType;
    set
    {
      if (this.m_chestType == Chest.GeneralChestType.WEAPON)
        --StaticReferenceManager.WeaponChestsSpawnedOnFloor;
      else if (this.m_chestType == Chest.GeneralChestType.ITEM)
        --StaticReferenceManager.ItemChestsSpawnedOnFloor;
      this.m_chestType = value;
      if (this.m_chestType == Chest.GeneralChestType.WEAPON)
      {
        ++StaticReferenceManager.WeaponChestsSpawnedOnFloor;
      }
      else
      {
        if (this.m_chestType != Chest.GeneralChestType.ITEM)
          return;
        ++StaticReferenceManager.ItemChestsSpawnedOnFloor;
      }
    }
  }

  public bool TemporarilyUnopenable => this.m_temporarilyUnopenable;

  public bool IsTruthChest => this.name.Contains("TruthChest");

  public bool IsMimic => this.m_isMimic;

  public static Chest Spawn(Chest chestPrefab, IntVector2 basePosition)
  {
    RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(basePosition);
    return Chest.Spawn(chestPrefab, basePosition, roomFromPosition);
  }

  public static Chest Spawn(
    Chest chestPrefab,
    IntVector2 basePosition,
    RoomHandler room,
    bool ForceNoMimic = false)
  {
    return Chest.Spawn(chestPrefab, basePosition.ToVector3(), room, ForceNoMimic);
  }

  public static Chest Spawn(
    Chest chestPrefab,
    Vector3 basePosition,
    RoomHandler room,
    bool ForceNoMimic = false)
  {
    if ((UnityEngine.Object) chestPrefab == (UnityEngine.Object) null)
      return (Chest) null;
    Chest component = UnityEngine.Object.Instantiate<GameObject>(chestPrefab.gameObject, basePosition, Quaternion.identity).GetComponent<Chest>();
    if (ForceNoMimic)
      component.MimicGuid = (string) null;
    component.m_room = room;
    component.HandleSpawnBehavior();
    return component;
  }

  public static void ToggleCoopChests(bool coopDead)
  {
    Chest.m_IsCoopMode = coopDead;
    for (int index = 0; index < StaticReferenceManager.AllChests.Count; ++index)
    {
      if (coopDead)
        StaticReferenceManager.AllChests[index].BecomeCoopChest();
      else
        StaticReferenceManager.AllChests[index].UnbecomeCoopChest();
    }
  }

  public void RegisterChestOnMinimap(RoomHandler r)
  {
    this.m_registeredIconRoom = r;
    GameObject iconPrefab = this.MinimapIconPrefab ?? BraveResources.Load("Global Prefabs/Minimap_Treasure_Icon") as GameObject;
    this.minimapIconInstance = Minimap.Instance.RegisterRoomIcon(r, iconPrefab);
  }

  public void DeregisterChestOnMinimap()
  {
    if (this.m_registeredIconRoom == null)
      return;
    Minimap.Instance.DeregisterRoomIcon(this.m_registeredIconRoom, this.minimapIconInstance);
  }

  private Color BaseOutlineColor
  {
    get
    {
      if (this.m_isMimic && !Dungeon.IsGenerating)
      {
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && GameManager.Instance.AllPlayers[index].CanDetectHiddenEnemies)
            return Color.red;
        }
      }
      return Color.black;
    }
  }

  private void Awake()
  {
    if (this.IsTruthChest)
      this.PreventFuse = true;
    StaticReferenceManager.AllChests.Add(this);
    SpriteOutlineManager.AddOutlineToSprite(this.sprite, this.BaseOutlineColor, 0.1f);
    this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleCoopChestAnimationEvent);
    this.majorBreakable.OnDamaged += new Action<float>(this.OnDamaged);
    if ((double) this.majorBreakable.DamageReduction > 1000.0)
      this.majorBreakable.ReportZeroDamage = true;
    this.majorBreakable.InvulnerableToEnemyBullets = true;
    this.m_runtimeRandom = new System.Random();
  }

  private void OnEnable()
  {
    if (!this.m_isMimic || this.m_isMimicBreathing)
      return;
    this.StartCoroutine(this.MimicIdleAnimCR());
  }

  private void HandleCoopChestAnimationEvent(
    tk2dSpriteAnimator animator,
    tk2dSpriteAnimationClip clip,
    int frameNo)
  {
    if (!(clip.GetFrame(frameNo).eventInfo == "coopchestvfx"))
      return;
    UnityEngine.Object.Instantiate(BraveResources.Load("Global VFX/VFX_ChestKnock_001"), (Vector3) (this.sprite.WorldCenter + new Vector2(0.0f, 5f / 16f)), Quaternion.identity);
  }

  protected void HandleSpawnBehavior() => this.StartCoroutine(this.SpawnBehavior_CR());

  [DebuggerHidden]
  private IEnumerator SpawnBehavior_CR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CSpawnBehavior_CR\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void PossiblyCreateBowler(bool mightBeActive)
  {
    if (this.m_hasCheckedBowler)
      return;
    this.m_hasCheckedBowler = true;
    if (this.IsRainbowChest || !GameStatsManager.HasInstance || !GameStatsManager.Instance.IsRainbowRun)
      return;
    bool flag1 = true;
    for (int index = 0; index < StaticReferenceManager.AllChests.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) StaticReferenceManager.AllChests[index] && !StaticReferenceManager.AllChests[index].IsRainbowChest && StaticReferenceManager.AllChests[index].GetAbsoluteParentRoom() == this.GetAbsoluteParentRoom() && (UnityEngine.Object) StaticReferenceManager.AllChests[index] != (UnityEngine.Object) this)
      {
        flag1 = false;
        break;
      }
    }
    if (!flag1)
      return;
    bool flag2 = this.breakAnimName.Contains("redgold") || this.breakAnimName.Contains("black");
    this.m_bowlerInstance = (GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/VFX_BowlerSit"));
    this.m_bowlerInstance.transform.parent = this.transform;
    tk2dBaseSprite component = this.m_bowlerInstance.GetComponent<tk2dBaseSprite>();
    if ((bool) (UnityEngine.Object) component)
      SpriteOutlineManager.AddOutlineToSprite(component, Color.black, 0.05f);
    if ((double) UnityEngine.Random.value < 0.0099999997764825821)
    {
      this.m_bowlerInstance.GetComponent<tk2dSpriteAnimator>().Play("salute_right");
      if (flag2)
      {
        this.m_bowlerInstance.transform.localPosition = new Vector3(0.0f, 0.625f);
        this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().HeightOffGround = 0.5f;
      }
      else
      {
        this.m_bowlerInstance.transform.localPosition = new Vector3(-7f / 16f, 0.125f);
        this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().HeightOffGround = -0.75f;
      }
      this.m_bowlerFireStatus = Tribool.Ready;
    }
    else
    {
      this.m_bowlerInstance.GetComponent<tk2dSpriteAnimator>().Play("sit_right");
      if (flag2)
      {
        this.m_bowlerInstance.transform.localPosition = new Vector3(0.0f, 0.625f);
        this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().HeightOffGround = 0.5f;
      }
      else
      {
        this.m_bowlerInstance.transform.localPosition = new Vector3(-0.25f, -5f / 16f);
        this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().HeightOffGround = -1.5f;
      }
      this.m_bowlerFireStatus = Tribool.Unready;
    }
    if (!mightBeActive)
      return;
    LootEngine.DoDefaultPurplePoof(this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().WorldCenter);
  }

  public void BecomeRainbowChest()
  {
    this.IsRainbowChest = true;
    this.lootTable.S_Chance = 0.2f;
    this.lootTable.A_Chance = 0.7f;
    this.lootTable.B_Chance = 0.4f;
    this.lootTable.C_Chance = 0.0f;
    this.lootTable.D_Chance = 0.0f;
    this.lootTable.Common_Chance = 0.0f;
    this.lootTable.canDropMultipleItems = true;
    this.lootTable.multipleItemDropChances = new WeightedIntCollection();
    this.lootTable.multipleItemDropChances.elements = new WeightedInt[1];
    this.lootTable.overrideItemLootTables = new List<GenericLootTable>();
    this.lootTable.lootTable = GameManager.Instance.RewardManager.GunsLootTable;
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.GunsLootTable);
    this.lootTable.overrideItemLootTables.Add(GameManager.Instance.RewardManager.ItemsLootTable);
    if (GameStatsManager.Instance.IsRainbowRun)
    {
      this.lootTable.C_Chance = 0.2f;
      this.lootTable.D_Chance = 0.2f;
      this.lootTable.overrideItemQualities = new List<PickupObject.ItemQuality>();
      if ((double) UnityEngine.Random.value < 0.5)
      {
        this.lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.S);
        this.lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.A);
      }
      else
      {
        this.lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.A);
        this.lootTable.overrideItemQualities.Add(PickupObject.ItemQuality.S);
      }
    }
    this.lootTable.multipleItemDropChances.elements[0] = new WeightedInt()
    {
      value = 8,
      weight = 1f,
      additionalPrerequisites = new DungeonPrerequisite[0]
    };
    this.lootTable.onlyOneGunCanDrop = false;
    if (this.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
    {
      this.spawnAnimName = "wood_chest_appear";
      this.openAnimName = "wood_chest_open";
      this.breakAnimName = "wood_chest_break";
    }
    else
    {
      this.spawnAnimName = "redgold_chest_appear";
      this.openAnimName = "redgold_chest_open";
      this.breakAnimName = "redgold_chest_break";
      this.majorBreakable.spriteNameToUseAtZeroHP = "chest_redgold_break_001";
    }
    this.sprite.usesOverrideMaterial = true;
    tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.spawnAnimName);
    this.sprite.SetSprite(clipByName.frames[clipByName.frames.Length - 1].spriteId);
    if (this.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
      return;
    if ((bool) (UnityEngine.Object) this.LockAnimator)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.LockAnimator.gameObject);
      this.LockAnimator = (tk2dSpriteAnimator) null;
    }
    this.IsLocked = false;
    this.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
  }

  public void RevealSecretRainbow()
  {
    if (this.m_secretRainbowRevealed)
      return;
    this.m_secretRainbowRevealed = true;
    this.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/RainbowChestShader");
    this.sprite.renderer.material.SetFloat("_HueTestValue", -3.5f);
    if ((bool) (UnityEngine.Object) this.LockAnimator)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.LockAnimator.gameObject);
      this.LockAnimator = (tk2dSpriteAnimator) null;
    }
    this.IsLocked = false;
  }

  protected void Initialize()
  {
    if (Chest.m_IsCoopMode && GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
      Chest.m_IsCoopMode = false;
    this.specRigidbody.Initialize();
    this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
    this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
    this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
    this.specRigidbody.PreventPiercing = true;
    MajorBreakable component = this.GetComponent<MajorBreakable>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.OnBreak += new System.Action(this.OnBroken);
    IntVector2 intVector2_1 = this.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
    IntVector2 intVector2_2 = this.specRigidbody.UnitTopRight.ToIntVector2(VectorConversions.Floor);
    for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
    {
      for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
        GameManager.Instance.Dungeon.data[new IntVector2(x, y)].isOccupied = true;
    }
    bool flag = (double) UnityEngine.Random.value < 0.00033300000359304249;
    if (this.ChestIdentifier == Chest.SpecialChestIdentifier.RAT || this.lootTable != null && this.lootTable.CompletesSynergy)
      flag = false;
    else if (!flag && this.spawnAnimName.StartsWith("wood_") && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.WOODEN_CHESTS_BROKEN) >= 5.0 && (double) UnityEngine.Random.value < 0.00033300000359304249)
    {
      flag = true;
      this.ChestIdentifier = Chest.SpecialChestIdentifier.SECRET_RAINBOW;
    }
    if (this.IsMirrorChest)
    {
      this.sprite.renderer.enabled = false;
      if ((bool) (UnityEngine.Object) this.LockAnimator)
        this.LockAnimator.Sprite.renderer.enabled = false;
      if ((bool) (UnityEngine.Object) this.ShadowSprite)
        this.ShadowSprite.renderer.enabled = false;
      this.specRigidbody.enabled = false;
    }
    else if (this.IsRainbowChest || flag)
      this.BecomeRainbowChest();
    else if (this.ForceGlitchChest || (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON || GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON) && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_ATTEMPTS) > 10.0 && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_BEHOLSTER) && !GameManager.Instance.InTutorial && (double) UnityEngine.Random.value < 1.0 / 1000.0)
    {
      this.BecomeGlitchChest();
    }
    else
    {
      if (Chest.m_IsCoopMode)
        return;
      this.MaybeBecomeMimic();
    }
  }

  private void Update()
  {
    if (this.m_isMimic && !this.m_temporarilyUnopenable && (bool) (UnityEngine.Object) this.sprite)
    {
      Color baseOutlineColor = this.BaseOutlineColor;
      if (baseOutlineColor != this.m_cachedOutlineColor)
      {
        this.m_cachedOutlineColor = baseOutlineColor;
        SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite, true);
        SpriteOutlineManager.AddOutlineToSprite(this.sprite, baseOutlineColor, 0.1f);
      }
    }
    if (!(bool) (UnityEngine.Object) this.m_bowlerInstance)
      return;
    if (this.m_bowlerFireStatus == Tribool.Ready)
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if ((double) Vector2.Distance(GameManager.Instance.AllPlayers[index].CenterPosition, (Vector2) this.m_bowlerInstance.transform.position) < 5.0)
        {
          this.m_bowlerFireStatus = Tribool.Complete;
          int num1 = (int) AkSoundEngine.PostEvent("Play_obj_bowler_ignite_01", this.gameObject);
          int num2 = (int) AkSoundEngine.PostEvent("Play_obj_bowler_burn_01", this.gameObject);
        }
      }
    }
    else
    {
      if (!(this.m_bowlerFireStatus == Tribool.Complete))
        return;
      this.m_bowlerFireTimer += BraveTime.DeltaTime * 15f;
      if ((double) this.m_bowlerFireTimer <= 1.0)
        return;
      GlobalSparksDoer.DoRadialParticleBurst(Mathf.FloorToInt(this.m_bowlerFireTimer), (Vector3) (this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().WorldBottomLeft + new Vector2(0.125f, 3f / 16f)), (Vector3) (this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().WorldTopRight - new Vector2(0.125f, 0.125f)), 0.0f, 0.0f, 0.0f, systemType: GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
      if ((bool) (UnityEngine.Object) this.sprite)
        GlobalSparksDoer.DoRadialParticleBurst(Mathf.FloorToInt(this.m_bowlerFireTimer * 3f), (Vector3) (this.sprite.WorldBottomLeft + new Vector2(0.125f, 3f / 16f)), (Vector3) (this.sprite.WorldTopRight - new Vector2(0.125f, 0.125f)), 0.0f, 0.0f, 0.0f, systemType: GlobalSparksDoer.SparksType.STRAIGHT_UP_FIRE);
      this.m_bowlerFireTimer %= 1f;
    }
  }

  protected void TriggerCountdownTimer()
  {
    if (!(bool) (UnityEngine.Object) this)
      return;
    this.extantFuse = UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Chest_Fuse"), this.transform.position + new Vector3(-1.75f, -1.5f, 0.0f), Quaternion.identity).GetComponent<ChestFuseController>();
    this.StartCoroutine(this.HandleExplosionCountdown(this.extantFuse));
  }

  public void ForceKillFuse()
  {
    if (!(bool) (UnityEngine.Object) this.extantFuse)
      return;
    int num = (int) AkSoundEngine.PostEvent("stop_obj_fuse_loop_01", this.gameObject);
    this.extantFuse = (ChestFuseController) null;
  }

  [DebuggerHidden]
  private IEnumerator HandleExplosionCountdown(ChestFuseController fuse)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CHandleExplosionCountdown\u003Ec__Iterator1()
    {
      fuse = fuse,
      \u0024this = this
    };
  }

  private void ExplodeInSadness()
  {
    MajorBreakable component = this.GetComponent<MajorBreakable>();
    this.GetRidOfBowler();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.OnBreak -= new System.Action(this.OnBroken);
    this.spriteAnimator.Play(string.IsNullOrEmpty(this.overrideBreakAnimName) ? this.breakAnimName : this.overrideBreakAnimName);
    this.specRigidbody.enabled = false;
    this.IsBroken = true;
    if ((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.LockAnimator)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.LockAnimator.gameObject);
    Transform transform = this.transform.Find("Shadow");
    if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) transform.gameObject);
    this.pickedUp = true;
    this.HandleGeneratedMagnificence();
    if (this.m_registeredIconRoom != null)
      Minimap.Instance.DeregisterRoomIcon(this.m_registeredIconRoom, this.minimapIconInstance);
    this.m_room.DeregisterInteractable((IPlayerInteractable) this);
    Exploder.DoDefaultExplosion((Vector3) this.sprite.WorldCenter, Vector2.zero);
  }

  private void OnBroken()
  {
    this.GetRidOfBowler();
    if (this.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
      this.RevealSecretRainbow();
    if (this.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW || this.IsRainbowChest || this.breakAnimName.Contains("redgold") || this.breakAnimName.Contains("black"))
      GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_GOLD_JUNK, true);
    this.spriteAnimator.Play(string.IsNullOrEmpty(this.overrideBreakAnimName) ? this.breakAnimName : this.overrideBreakAnimName);
    this.specRigidbody.enabled = false;
    this.IsBroken = true;
    IntVector2 intVector2_1 = this.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
    IntVector2 intVector2_2 = this.specRigidbody.UnitTopRight.ToIntVector2(VectorConversions.Floor);
    for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
    {
      for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
        GameManager.Instance.Dungeon.data[new IntVector2(x, y)].isOccupied = false;
    }
    if ((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null && (bool) (UnityEngine.Object) this.LockAnimator)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.LockAnimator.gameObject);
    Transform transform = this.transform.Find("Shadow");
    if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) transform.gameObject);
    if (this.pickedUp)
      return;
    this.pickedUp = true;
    this.HandleGeneratedMagnificence();
    this.m_room.DeregisterInteractable((IPlayerInteractable) this);
    if (this.m_registeredIconRoom != null)
      Minimap.Instance.DeregisterRoomIcon(this.m_registeredIconRoom, this.minimapIconInstance);
    if (this.spawnAnimName.StartsWith("wood_"))
      GameStatsManager.Instance.RegisterStatChange(TrackedStats.WOODEN_CHESTS_BROKEN, 1f);
    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.NumberOfLivingPlayers == 1)
    {
      this.StartCoroutine(this.GiveCoopPartnerBack(false));
    }
    else
    {
      bool flag1 = PassiveItem.IsFlagSetAtAll(typeof (ChestBrokenImprovementItem));
      bool flag2 = GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_GOLD_JUNK);
      float num1 = GameManager.Instance.RewardManager.ChestDowngradeChance;
      float num2 = GameManager.Instance.RewardManager.ChestHalfHeartChance;
      float num3 = GameManager.Instance.RewardManager.ChestExplosionChance;
      float num4 = GameManager.Instance.RewardManager.ChestJunkChance;
      float num5 = !flag2 ? 0.0f : 0.005f;
      bool flag3 = GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_SER_JUNKAN_UNLOCKED);
      float num6 = !flag3 || Chest.HasDroppedSerJunkanThisSession ? 0.0f : GameManager.Instance.RewardManager.ChestJunkanUnlockedChance;
      if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.carriedConsumables.KeyBullets > 0)
        num4 *= GameManager.Instance.RewardManager.HasKeyJunkMultiplier;
      if (SackKnightController.HasJunkan())
      {
        num4 *= GameManager.Instance.RewardManager.HasJunkanJunkMultiplier;
        num5 *= 3f;
      }
      if (this.IsTruthChest)
      {
        num1 = 0.0f;
        num2 = 0.0f;
        num3 = 0.0f;
        num4 = 1f;
      }
      float num7 = num4 - num5;
      float num8 = UnityEngine.Random.value * (num5 + num1 + num2 + num3 + num7 + num6);
      if (flag2 && (double) num8 < (double) num5)
      {
        this.contents = new List<PickupObject>();
        this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.GoldJunk));
        this.m_forceDropOkayForRainbowRun = true;
        this.StartCoroutine(this.PresentItem());
      }
      else if ((double) num8 < (double) num1 || flag1)
      {
        int tierShift = -4;
        bool flag4 = false;
        if (flag1)
        {
          float num9 = UnityEngine.Random.value;
          if ((double) num9 < (double) ChestBrokenImprovementItem.PickupQualChance)
          {
            flag4 = true;
            this.contents = new List<PickupObject>();
            PickupObject pickupObject = (PickupObject) null;
            while ((UnityEngine.Object) pickupObject == (UnityEngine.Object) null)
            {
              GameObject gameObject = GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable.SelectByWeight();
              if ((bool) (UnityEngine.Object) gameObject)
                pickupObject = gameObject.GetComponent<PickupObject>();
            }
            this.contents.Add(pickupObject);
            this.StartCoroutine(this.PresentItem());
          }
          else
            tierShift = (double) num9 >= (double) ChestBrokenImprovementItem.PickupQualChance + (double) ChestBrokenImprovementItem.MinusOneQualChance ? ((double) num9 >= (double) ChestBrokenImprovementItem.PickupQualChance + (double) ChestBrokenImprovementItem.EqualQualChance + (double) ChestBrokenImprovementItem.MinusOneQualChance ? 1 : 0) : -1;
        }
        if (!flag4)
        {
          this.DetermineContents(GameManager.Instance.PrimaryPlayer, tierShift);
          this.StartCoroutine(this.PresentItem());
        }
      }
      else if ((double) num8 < (double) num1 + (double) num2)
      {
        this.contents = new List<PickupObject>();
        this.contents.Add(GameManager.Instance.RewardManager.HalfHeartPrefab);
        this.m_forceDropOkayForRainbowRun = true;
        this.StartCoroutine(this.PresentItem());
      }
      else if ((double) num8 < (double) num1 + (double) num2 + (double) num7)
      {
        bool flag5 = false;
        if (!Chest.HasDroppedSerJunkanThisSession && !flag3 && (double) UnityEngine.Random.value < 0.20000000298023224)
        {
          Chest.HasDroppedSerJunkanThisSession = true;
          flag5 = true;
        }
        this.contents = new List<PickupObject>();
        int id = this.overrideJunkId < 0 ? GlobalItemIds.Junk : this.overrideJunkId;
        if (flag5)
          id = GlobalItemIds.SackKnightBoon;
        this.contents.Add(PickupObjectDatabase.GetById(id));
        this.m_forceDropOkayForRainbowRun = true;
        this.StartCoroutine(this.PresentItem());
      }
      else if ((double) num8 < (double) num1 + (double) num2 + (double) num7 + (double) num6)
      {
        Chest.HasDroppedSerJunkanThisSession = true;
        this.contents = new List<PickupObject>();
        this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.SackKnightBoon));
        this.StartCoroutine(this.PresentItem());
      }
      else
        Exploder.DoDefaultExplosion((Vector3) this.sprite.WorldCenter, Vector2.zero);
    }
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (GameManager.Instance.AllPlayers[index].OnChestBroken != null)
        GameManager.Instance.AllPlayers[index].OnChestBroken(GameManager.Instance.AllPlayers[index], this);
    }
  }

  [DebuggerHidden]
  private IEnumerator GiveCoopPartnerBack(bool doDelay = true)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CGiveCoopPartnerBack\u003Ec__Iterator2()
    {
      doDelay = doDelay,
      \u0024this = this
    };
  }

  private void OnPreRigidbodyCollision(
    SpeculativeRigidbody myRigidbody,
    PixelCollider myPixelCollider,
    SpeculativeRigidbody otherRigidbody,
    PixelCollider otherPixelCollider)
  {
    if (myPixelCollider.IsTrigger || !((UnityEngine.Object) otherRigidbody.GetComponent<KeyBullet>() != (UnityEngine.Object) null))
      return;
    PhysicsEngine.SkipCollision = true;
  }

  private void OnTriggerCollision(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody,
    CollisionData collisionData)
  {
    if (this.m_isKeyOpening)
      return;
    KeyBullet component = specRigidbody.GetComponent<KeyBullet>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    this.HandleKeyEncounter(component);
  }

  public void HandleKeyEncounter(KeyBullet key)
  {
    if (this.IsSealed || !this.IsLocked)
      return;
    this.m_isKeyOpening = true;
    Projectile component = key.GetComponent<Projectile>();
    component.specRigidbody.Velocity = Vector2.zero;
    GameObject overrideMidairDeathVfx = component.hitEffects.overrideMidairDeathVFX;
    PlayerController owner = component.Owner as PlayerController;
    UnityEngine.Object.Destroy((UnityEngine.Object) component.specRigidbody);
    UnityEngine.Object.Destroy((UnityEngine.Object) component);
    this.StartCoroutine(this.HandleKeyEncounter_CR(key, overrideMidairDeathVfx, owner));
  }

  [DebuggerHidden]
  private IEnumerator HandleKeyEncounter_CR(
    KeyBullet key,
    GameObject vfxPrefab,
    PlayerController optionalPlayer)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CHandleKeyEncounter_CR\u003Ec__Iterator3()
    {
      key = key,
      vfxPrefab = vfxPrefab,
      optionalPlayer = optionalPlayer,
      \u0024this = this
    };
  }

  public void ForceUnlock()
  {
    if (!this.IsLocked)
      return;
    this.IsLocked = false;
    if (!((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null))
      return;
    this.LockAnimator.PlayAndDestroyObject(this.LockOpenAnim);
  }

  private void OnRigidbodyCollision(CollisionData rigidbodyCollision)
  {
    if (this.ChestIdentifier != Chest.SpecialChestIdentifier.SECRET_RAINBOW || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.projectile || BraveUtility.EnumFlagsContains((uint) rigidbodyCollision.OtherRigidbody.projectile.damageTypes, 32U /*0x20*/) <= 0)
      return;
    this.RevealSecretRainbow();
  }

  public void ForceOpen(PlayerController player) => this.Open(player);

  protected void HandleGeneratedMagnificence()
  {
    if ((double) this.GeneratedMagnificence <= 0.0)
      return;
    GameManager.Instance.Dungeon.GeneratedMagnificence -= this.GeneratedMagnificence;
    this.GeneratedMagnificence = 0.0f;
  }

  private void GetRidOfBowler()
  {
    if (!(bool) (UnityEngine.Object) this.m_bowlerInstance)
      return;
    LootEngine.DoDefaultPurplePoof(this.m_bowlerInstance.GetComponent<tk2dBaseSprite>().WorldCenter);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bowlerInstance);
    this.m_bowlerInstance = (GameObject) null;
    int num = (int) AkSoundEngine.PostEvent("Stop_SND_OBJ", this.gameObject);
  }

  protected void Open(PlayerController player)
  {
    if (!((UnityEngine.Object) player != (UnityEngine.Object) null))
      return;
    this.GetRidOfBowler();
    if (GameManager.Instance.InTutorial || this.AlwaysBroadcastsOpenEvent)
      GameManager.BroadcastRoomTalkDoerFsmEvent("playerOpenedChest");
    if (this.m_registeredIconRoom != null)
      Minimap.Instance.DeregisterRoomIcon(this.m_registeredIconRoom, this.minimapIconInstance);
    if (this.m_isGlitchChest)
    {
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(player);
        if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.IsGhost)
          this.StartCoroutine(this.GiveCoopPartnerBack(false));
      }
      GameManager.Instance.InjectedFlowPath = "Core Game Flows/Secret_DoubleBeholster_Flow";
      Pixelator.Instance.FadeToBlack(0.5f);
      GameManager.Instance.DelayedLoadNextLevel(0.5f);
    }
    else if (this.m_isMimic && !Chest.m_IsCoopMode)
    {
      this.DetermineContents(player);
      this.DoMimicTransformation(this.contents);
    }
    else
    {
      if (this.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
        this.RevealSecretRainbow();
      this.pickedUp = true;
      this.IsOpen = true;
      this.HandleGeneratedMagnificence();
      this.m_room.DeregisterInteractable((IPlayerInteractable) this);
      MajorBreakable component = this.GetComponent<MajorBreakable>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.usesTemporaryZeroHitPointsState = false;
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.NumberOfLivingPlayers == 1 && this.ChestIdentifier == Chest.SpecialChestIdentifier.NORMAL)
      {
        this.spriteAnimator.Play(!string.IsNullOrEmpty(this.overrideOpenAnimName) ? this.overrideOpenAnimName : this.openAnimName);
        this.m_isMimic = false;
        this.StartCoroutine(this.GiveCoopPartnerBack());
      }
      else if (this.lootTable.CompletesSynergy)
      {
        this.StartCoroutine(this.HandleSynergyGambleChest(player));
      }
      else
      {
        this.DetermineContents(player);
        if (this.name.Contains("Chest_Red") && this.contents != null && this.contents.Count == 1 && (bool) (UnityEngine.Object) this.contents[0] && this.contents[0].ItemSpansBaseQualityTiers)
          this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.Key));
        this.spriteAnimator.Play(!string.IsNullOrEmpty(this.overrideOpenAnimName) ? this.overrideOpenAnimName : this.openAnimName);
        int num1 = (int) AkSoundEngine.PostEvent("play_obj_chest_open_01", this.gameObject);
        int num2 = (int) AkSoundEngine.PostEvent("stop_obj_fuse_loop_01", this.gameObject);
        if (this.m_isMimic)
          return;
        if (this.SubAnimators != null && this.SubAnimators.Length > 0)
        {
          for (int index = 0; index < this.SubAnimators.Length; ++index)
            this.SubAnimators[index].Play();
        }
        player.TriggerItemAcquisition();
        this.StartCoroutine(this.PresentItem());
      }
    }
  }

  [DebuggerHidden]
  private IEnumerator HandleSynergyGambleChest(PlayerController player)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CHandleSynergyGambleChest\u003Ec__Iterator4()
    {
      player = player,
      \u0024this = this
    };
  }

  protected bool HandleQuestContentsModification()
  {
    if (GameManager.Instance.InTutorial || this.IsRainbowChest)
      return false;
    if (GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_01) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_02) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_03) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_04) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_05) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_06))
      GameStatsManager.Instance.SetFlag(GungeonFlags.RESOURCEFUL_RAT_COMPLETE, true);
    if (!GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_COMPLETE) && !Chest.HasDroppedResourcefulRatNoteThisSession)
    {
      float b = 0.15f;
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_01))
        b = 0.33f;
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_LICH))
        b = 10f;
      float playerStatValue = GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_ATTEMPTS);
      if ((bool) (UnityEngine.Object) GameManager.Instance.Dungeon && GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON)
        b = 0.0f;
      if ((double) UnityEngine.Random.value < ((double) playerStatValue >= 10.0 ? (double) Mathf.Lerp(0.0f, b, Mathf.Clamp01(playerStatValue / 20f)) : 0.0))
      {
        bool flag1 = !GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_01) && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_CLEARED_GUNGEON) > 0.0;
        bool flag2 = !GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_02) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_01) && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.ITEMS_TAKEN_BY_RAT) > 0.0;
        bool flag3 = !GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_03) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_02) && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_SEWERS) > 0.0;
        bool flag4 = !GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_04) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_03) && (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.MASTERY_TOKENS_RECEIVED) > 0.0;
        bool flag5 = !GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_05) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_04) && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_MINI_FUSELIER);
        bool flag6 = this.m_isMimic && !GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_06) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_05) && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_DRAGUN);
        if (flag1)
        {
          this.contents.Clear();
          this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.RatNote01));
          Chest.HasDroppedResourcefulRatNoteThisSession = true;
          return true;
        }
        if (flag2)
        {
          this.contents.Clear();
          this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.RatNote02));
          Chest.HasDroppedResourcefulRatNoteThisSession = true;
          return true;
        }
        if (flag3)
        {
          this.contents.Clear();
          this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.RatNote03));
          Chest.HasDroppedResourcefulRatNoteThisSession = true;
          return true;
        }
        if (flag4)
        {
          this.contents.Clear();
          this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.RatNote04));
          Chest.HasDroppedResourcefulRatNoteThisSession = true;
          return true;
        }
        if (flag5)
        {
          this.contents.Clear();
          this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.RatNote05));
          Chest.HasDroppedResourcefulRatNoteThisSession = true;
          return true;
        }
        if (flag6)
        {
          this.contents.Clear();
          this.contents.Add(PickupObjectDatabase.GetById(GlobalItemIds.RatNote06));
          Chest.HasDroppedResourcefulRatNoteThisSession = true;
          return true;
        }
      }
    }
    return false;
  }

  public void GenerationDetermineContents(FloorRewardManifest manifest, System.Random safeRandom)
  {
    List<PickupObject> contents = this.GenerateContents((PlayerController) null, 0, safeRandom);
    manifest.RegisterContents(this, contents);
  }

  protected List<PickupObject> GenerateContents(
    PlayerController player,
    int tierShift,
    System.Random safeRandom = null)
  {
    List<PickupObject> contents = new List<PickupObject>();
    if ((UnityEngine.Object) this.lootTable.lootTable == (UnityEngine.Object) null)
      contents.Add(GameManager.Instance.Dungeon.baseChestContents.SelectByWeight().GetComponent<PickupObject>());
    else if (this.lootTable != null)
    {
      if (tierShift <= -1)
      {
        contents = !((UnityEngine.Object) this.breakertronLootTable.lootTable != (UnityEngine.Object) null) ? this.lootTable.GetItemsForPlayer(player, tierShift, generatorRandom: safeRandom) : this.breakertronLootTable.GetItemsForPlayer(player, -1, generatorRandom: safeRandom);
      }
      else
      {
        contents = this.lootTable.GetItemsForPlayer(player, tierShift, generatorRandom: safeRandom);
        if (this.lootTable.CompletesSynergy)
        {
          float num = Mathf.Clamp(Mathf.Clamp01((float) (0.60000002384185791 - 0.10000000149011612 * (double) this.lootTable.LastGenerationNumSynergiesCalculated)), 0.2f, 1f);
          if ((double) num > 0.0 && (safeRandom == null ? (double) UnityEngine.Random.value : safeRandom.NextDouble()) < (double) num)
          {
            this.lootTable.CompletesSynergy = false;
            contents = this.lootTable.GetItemsForPlayer(player, tierShift, generatorRandom: safeRandom);
            this.lootTable.CompletesSynergy = true;
          }
        }
      }
    }
    return contents;
  }

  public List<PickupObject> PredictContents(PlayerController player)
  {
    this.DetermineContents(player);
    return this.contents;
  }

  protected void DetermineContents(PlayerController player, int tierShift = 0)
  {
    if (this.contents == null)
    {
      this.contents = new List<PickupObject>();
      if (this.forceContentIds.Count > 0)
      {
        for (int index = 0; index < this.forceContentIds.Count; ++index)
          this.contents.Add(PickupObjectDatabase.GetById(this.forceContentIds[index]));
      }
    }
    bool flag = this.HandleQuestContentsModification();
    if (this.contents.Count != 0 || flag)
      return;
    FloorRewardManifest manifestForCurrentFloor = GameManager.Instance.RewardManager.GetSeededManifestForCurrentFloor();
    this.contents = manifestForCurrentFloor == null || !manifestForCurrentFloor.PregeneratedChestContents.ContainsKey(this) ? this.GenerateContents(player, tierShift, this.m_runtimeRandom) : manifestForCurrentFloor.PregeneratedChestContents[this];
    if (this.contents.Count != 0)
      return;
    UnityEngine.Debug.LogError((object) "Emergency Mimic swap... what's going to happen to the loot now?");
    this.m_isMimic = true;
    this.DoMimicTransformation((List<PickupObject>) null);
  }

  private void DoMimicTransformation(List<PickupObject> overrideDeathRewards)
  {
    IntVector2 intVector2_1 = this.specRigidbody.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
    IntVector2 intVector2_2 = this.specRigidbody.UnitTopRight.ToIntVector2(VectorConversions.Floor);
    for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
    {
      for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
        GameManager.Instance.Dungeon.data[new IntVector2(x, y)].isOccupied = false;
    }
    if (!this.pickedUp)
    {
      this.pickedUp = true;
      this.m_room.DeregisterInteractable((IPlayerInteractable) this);
    }
    if (this.m_registeredIconRoom != null)
      Minimap.Instance.DeregisterRoomIcon(this.m_registeredIconRoom, this.minimapIconInstance);
    AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.MimicGuid), this.transform.position.XY().ToIntVector2(VectorConversions.Floor), this.GetAbsoluteParentRoom());
    if (overrideDeathRewards != null)
      aiActor.AdditionalSafeItemDrops.AddRange((IEnumerable<PickupObject>) overrideDeathRewards);
    aiActor.specRigidbody.Initialize();
    Vector2 unitBottomLeft1 = aiActor.specRigidbody.UnitBottomLeft;
    Vector2 unitBottomLeft2 = this.specRigidbody.UnitBottomLeft;
    aiActor.transform.position -= (Vector3) (unitBottomLeft1 - unitBottomLeft2);
    aiActor.transform.position += (Vector3) PhysicsEngine.PixelToUnit(this.mimicOffset);
    aiActor.specRigidbody.Reinitialize();
    aiActor.HasDonePlayerEnterCheck = true;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  protected void SpewContentsOntoGround(List<Transform> spawnTransforms)
  {
    List<DebrisObject> items = new List<DebrisObject>();
    bool isRainbowRun = GameStatsManager.Instance.IsRainbowRun;
    if (isRainbowRun && !this.IsRainbowChest && !this.m_forceDropOkayForRainbowRun)
    {
      LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteChest, (!((UnityEngine.Object) this.spawnTransform != (UnityEngine.Object) null) ? (Vector2) (this.transform.position + this.sprite.GetBounds().extents) : (Vector2) this.spawnTransform.position) + new Vector2(-0.5f, -2.25f), this.m_room, true);
    }
    else
    {
      for (int index1 = 0; index1 < this.contents.Count; ++index1)
      {
        List<DebrisObject> collection = LootEngine.SpewLoot(new List<GameObject>()
        {
          this.contents[index1].gameObject
        }, spawnTransforms[index1].position);
        items.AddRange((IEnumerable<DebrisObject>) collection);
        for (int index2 = 0; index2 < collection.Count; ++index2)
        {
          if ((bool) (UnityEngine.Object) collection[index2])
            collection[index2].PreventFallingInPits = true;
          if (!((UnityEngine.Object) collection[index2].GetComponent<Gun>() != (UnityEngine.Object) null) && !((UnityEngine.Object) collection[index2].GetComponent<CurrencyPickup>() != (UnityEngine.Object) null) && (UnityEngine.Object) collection[index2].specRigidbody != (UnityEngine.Object) null)
          {
            collection[index2].specRigidbody.CollideWithOthers = false;
            collection[index2].OnTouchedGround += new Action<DebrisObject>(this.BecomeViableItem);
          }
        }
      }
    }
    if (!this.IsRainbowChest || !isRainbowRun || this.transform.position.GetAbsoluteRoom() != GameManager.Instance.Dungeon.data.Entrance)
      return;
    GameManager.Instance.Dungeon.StartCoroutine(this.HandleRainbowRunLootProcessing(items));
  }

  [DebuggerHidden]
  private IEnumerator HandleRainbowRunLootProcessing(List<DebrisObject> items)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CHandleRainbowRunLootProcessing\u003Ec__Iterator5()
    {
      items = items,
      \u0024this = this
    };
  }

  protected void BecomeViableItem(DebrisObject debris)
  {
    debris.OnTouchedGround -= new Action<DebrisObject>(this.BecomeViableItem);
    debris.OnGrounded -= new Action<DebrisObject>(this.BecomeViableItem);
    debris.specRigidbody.CollideWithOthers = true;
    Vector2 zero = Vector2.zero;
    Vector2 vector2 = !((UnityEngine.Object) this.spawnTransform != (UnityEngine.Object) null) ? debris.sprite.WorldCenter - this.sprite.WorldCenter : debris.sprite.WorldCenter - this.spawnTransform.position.XY();
    debris.ClearVelocity();
    debris.ApplyVelocity(vector2.normalized * 2f);
  }

  private bool CheckPresentedItemTheoreticalPosition(Vector3 targetPosition, Vector3 objectOffset)
  {
    Vector3 pos1 = targetPosition;
    Vector3 pos2 = targetPosition - new Vector3(objectOffset.x * 2f, 0.0f, 0.0f);
    Vector3 pos3 = targetPosition - new Vector3(0.0f, objectOffset.y * 2f, 0.0f);
    Vector3 pos4 = targetPosition - new Vector3(objectOffset.x * 2f, objectOffset.y * 2f, 0.0f);
    return !this.CheckCellValidForItemSpawn(pos1) || !this.CheckCellValidForItemSpawn(pos2) || !this.CheckCellValidForItemSpawn(pos3) || !this.CheckCellValidForItemSpawn(pos4);
  }

  private bool CheckCellValidForItemSpawn(Vector3 pos)
  {
    IntVector2 vec = pos.IntXY(VectorConversions.Floor);
    Dungeon dungeon = GameManager.Instance.Dungeon;
    return dungeon.data.CheckInBoundsAndValid(vec) && !dungeon.CellIsPit(pos) && !dungeon.data.isTopWall(vec.x, vec.y) && (!dungeon.data.isWall(vec.x, vec.y) || dungeon.data.isFaceWallLower(vec.x, vec.y));
  }

  [DebuggerHidden]
  private IEnumerator PresentItem()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CPresentItem\u003Ec__Iterator6()
    {
      \u0024this = this
    };
  }

  public void OnEnteredRange(PlayerController interactor)
  {
    if (!(bool) (UnityEngine.Object) this || this.IsLocked && interactor.carriedConsumables.KeyBullets <= 0 && !interactor.carriedConsumables.InfiniteKeys || this.IsSealed)
      return;
    SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
    SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.white, 0.1f);
    this.sprite.UpdateZDepth();
  }

  public void OnExitRange(PlayerController interactor)
  {
    if (!(bool) (UnityEngine.Object) this)
      return;
    SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
    SpriteOutlineManager.AddOutlineToSprite(this.sprite, this.BaseOutlineColor, 0.1f);
    this.sprite.UpdateZDepth();
  }

  public float GetDistanceToPoint(Vector2 point)
  {
    if (this.IsMirrorChest)
      return 1000f;
    Bounds bounds = this.sprite.GetBounds();
    bounds.SetMinMax(bounds.min + this.transform.position, bounds.max + this.transform.position);
    float num1 = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
    float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
    return Mathf.Sqrt((float) (((double) point.x - (double) num1) * ((double) point.x - (double) num1) + ((double) point.y - (double) num2) * ((double) point.y - (double) num2)));
  }

  public float GetOverrideMaxDistance() => -1f;

  public void BreakLock()
  {
    this.IsSealed = true;
    this.IsLocked = false;
    this.IsLockBroken = true;
    if (this.pickedUp || !((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null))
      return;
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_lock_pick_01", this.gameObject);
    this.LockAnimator.Play(this.LockBreakAnim);
  }

  private void Unlock()
  {
    this.IsLocked = false;
    if (this.pickedUp || !((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null))
      return;
    this.LockAnimator.PlayAndDestroyObject(this.LockOpenAnim);
  }

  public void Interact(PlayerController player)
  {
    if (this.IsSealed || this.IsLockBroken)
      return;
    if (this.IsLocked)
    {
      if (this.ChestIdentifier == Chest.SpecialChestIdentifier.RAT)
      {
        if (player.carriedConsumables.ResourcefulRatKeys <= 0)
          return;
        --player.carriedConsumables.ResourcefulRatKeys;
        this.Unlock();
        if (this.pickedUp)
          return;
        if (this.forceContentIds != null && this.forceContentIds.Count == 1)
        {
          for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
          {
            PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
            if ((bool) (UnityEngine.Object) allPlayer && allPlayer.HasPickupID(this.forceContentIds[0]))
            {
              this.forceContentIds.Clear();
              if ((double) UnityEngine.Random.value < 0.5)
              {
                this.ChestType = Chest.GeneralChestType.WEAPON;
                this.lootTable.lootTable = GameManager.Instance.RewardManager.GunsLootTable;
              }
              else
              {
                this.ChestType = Chest.GeneralChestType.ITEM;
                this.lootTable.lootTable = GameManager.Instance.RewardManager.ItemsLootTable;
              }
            }
          }
        }
        this.Open(player);
      }
      else if ((UnityEngine.Object) this.LockAnimator == (UnityEngine.Object) null || !this.LockAnimator.renderer.enabled)
      {
        this.Unlock();
        if (this.pickedUp)
          return;
        this.Open(player);
      }
      else if (player.carriedConsumables.KeyBullets <= 0 && !player.carriedConsumables.InfiniteKeys)
      {
        if (!((UnityEngine.Object) this.LockAnimator != (UnityEngine.Object) null))
          return;
        this.LockAnimator.Play(this.LockNoKeyAnim);
      }
      else
      {
        if (!player.carriedConsumables.InfiniteKeys)
          --player.carriedConsumables.KeyBullets;
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.CHESTS_UNLOCKED_WITH_KEY_BULLETS, 1f);
        this.Unlock();
        if (this.pickedUp)
          return;
        this.Open(player);
      }
    }
    else
    {
      if (this.pickedUp)
        return;
      this.Open(player);
    }
  }

  public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
  {
    shouldBeFlipped = false;
    return string.Empty;
  }

  public void BecomeGlitchChest()
  {
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_chestglitch_loop_01", this.gameObject);
    this.sprite.usesOverrideMaterial = true;
    Material material = this.sprite.renderer.material;
    material.shader = ShaderCache.Acquire("Brave/Internal/Glitch");
    material.SetFloat("_GlitchInterval", 0.1f);
    material.SetFloat("_DispProbability", 0.4f);
    material.SetFloat("_DispIntensity", 0.01f);
    material.SetFloat("_ColorProbability", 0.4f);
    material.SetFloat("_ColorIntensity", 0.04f);
    this.m_isGlitchChest = true;
  }

  private void BecomeCoopChest()
  {
    if (this.ChestIdentifier == Chest.SpecialChestIdentifier.RAT)
      return;
    GameManager.Instance.Dungeon.StartCoroutine(this.HandleCoopChestTransform());
  }

  public bool IsGlitched => this.m_isGlitchChest;

  [DebuggerHidden]
  private IEnumerator HandleCoopChestTransform(bool unbecome = false)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CHandleCoopChestTransform\u003Ec__Iterator7()
    {
      unbecome = unbecome,
      \u0024this = this
    };
  }

  private void UnbecomeCoopChest()
  {
    GameManager.Instance.Dungeon.StartCoroutine(this.HandleCoopChestTransform(true));
  }

  public void MaybeBecomeMimic()
  {
    if (this.IsTruthChest || this.IsRainbowChest || this.lootTable.CompletesSynergy)
      return;
    this.m_isMimic = false;
    bool flag1 = false;
    if (!GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_COMPLETE) && !Chest.HasDroppedResourcefulRatNoteThisSession && !Chest.DoneResourcefulRatMimicThisSession && !GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_06) && GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_05) && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_LICH) && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
    {
      Chest.DoneResourcefulRatMimicThisSession = true;
      flag1 = true;
    }
    if (string.IsNullOrEmpty(this.MimicGuid))
      return;
    bool flag2 = flag1 | GameManager.Instance.Dungeon.sharedSettingsPrefab.RandomShouldBecomeMimic(this.overrideMimicChance);
    GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
    bool flag3 = ((flag2 ? 1 : 0) | (loadedLevelDefinition == null || loadedLevelDefinition.lastSelectedFlowEntry == null ? 0 : (loadedLevelDefinition.lastSelectedFlowEntry.levelMode == FlowLevelEntryMode.ALL_MIMICS ? 1 : 0))) != 0;
    if (PassiveItem.IsFlagSetAtAll(typeof (MimicToothNecklaceItem)) && this.ChestIdentifier == Chest.SpecialChestIdentifier.RAT)
      flag3 = false;
    if (!flag3)
      return;
    if (PassiveItem.IsFlagSetAtAll(typeof (MimicToothNecklaceItem)))
      this.Unlock();
    if (PassiveItem.IsFlagSetAtAll(typeof (MimicRingItem)))
      return;
    this.m_isMimic = true;
    if (!this.gameObject.activeInHierarchy)
      return;
    this.StartCoroutine(this.MimicIdleAnimCR());
  }

  [DebuggerHidden]
  private IEnumerator MimicIdleAnimCR()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Chest.\u003CMimicIdleAnimCR\u003Ec__Iterator8()
    {
      \u0024this = this
    };
  }

  private void OnDamaged(float damage)
  {
    if (!this.m_isMimic || Chest.m_IsCoopMode || this.IsMirrorChest)
      return;
    GameManager.Instance.platformInterface.AchievementUnlock(Achievement.PREFIRE_ON_MIMIC);
    this.DetermineContents(GameManager.Instance.PrimaryPlayer);
    this.DoMimicTransformation(this.contents);
  }

  public void ConfigureOnPlacement(RoomHandler room)
  {
    this.m_room = room;
    if (!this.m_configured)
      this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.RoomEntered);
    this.Initialize();
    if (!this.m_configured)
      this.RegisterChestOnMinimap(room);
    this.m_configured = true;
    this.PossiblyCreateBowler(false);
  }

  private void RoomEntered(PlayerController enterer)
  {
    if (Chest.m_IsCoopMode && GameManager.HasInstance && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.healthHaver.IsAlive && (bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer && GameManager.Instance.SecondaryPlayer.healthHaver.IsAlive)
      this.UnbecomeCoopChest();
    if (Chest.m_IsCoopMode || this.IsOpen || this.IsBroken || this.m_hasBeenCheckedForFuses || this.PreventFuse || this.ChestIdentifier == Chest.SpecialChestIdentifier.RAT)
      return;
    this.m_hasBeenCheckedForFuses = true;
    float num1 = Mathf.Max(0.01f, Mathf.Clamp01(0.02f + (float) PlayerStats.GetTotalCurse() * 0.05f + (float) PlayerStats.GetTotalCoolness() * -0.025f));
    if (this.lootTable != null && this.lootTable.CompletesSynergy)
      num1 = 1f;
    if ((double) UnityEngine.Random.value >= (double) num1)
      return;
    this.TriggerCountdownTimer();
    int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_fuse_loop_01", this.gameObject);
  }

  protected override void OnDestroy()
  {
    this.majorBreakable.OnDamaged -= new Action<float>(this.OnDamaged);
    StaticReferenceManager.AllChests.Remove(this);
    base.OnDestroy();
    int num1 = (int) AkSoundEngine.PostEvent("Stop_SND_OBJ", this.gameObject);
    int num2 = (int) AkSoundEngine.PostEvent("stop_obj_fuse_loop_01", this.gameObject);
  }

  public enum GeneralChestType
  {
    UNSPECIFIED,
    WEAPON,
    ITEM,
  }

  public enum SpecialChestIdentifier
  {
    NORMAL,
    RAT,
    SECRET_RAINBOW,
  }
}
