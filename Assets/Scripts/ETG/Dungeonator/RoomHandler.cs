// Decompiled with JetBrains decompiler
// Type: Dungeonator.RoomHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace Dungeonator;

public class RoomHandler
{
  public static bool DrawRandomCellLines = false;
  public int distanceFromEntrance;
  public bool IsLoopMember;
  public bool LoopIsUnidirectional;
  public Guid LoopGuid;
  public CellArea area;
  public Rect cameraBoundingRect;
  public RoomHandlerBoundingPolygon cameraBoundingPolygon;
  public RoomHandler parentRoom;
  public List<RoomHandler> childRooms;
  public FlowActionLine flowActionLine;
  public List<DungeonDoorController> connectedDoors;
  public List<DungeonDoorSubsidiaryBlocker> standaloneBlockers;
  public List<BossTriggerZone> bossTriggerZones;
  public List<RoomHandler> connectedRooms;
  public Dictionary<PrototypeRoomExit, RoomHandler> connectedRoomsByExit;
  public Dictionary<RuntimeRoomExitData, RuntimeExitDefinition> exitDefinitionsByExit;
  public RoomHandler injectionTarget;
  public List<RoomHandler> injectionFrameData;
  public Opulence opulence;
  public GameObject OptionalDoorTopDecorable;
  public RoomCreationStrategy.RoomType roomType;
  public bool CanReceiveCaps;
  [NonSerialized]
  public RoomHandler.ProceduralLockType ProceduralLockingType;
  [NonSerialized]
  public bool ShouldAttemptProceduralLock;
  [NonSerialized]
  public float AttemptProceduralLockChance;
  public bool IsOnCriticalPath;
  public int DungeonWingID = -1;
  public bool PrecludeTilemapDrawing;
  public bool DrawPrecludedCeilingTiles;
  [NonSerialized]
  public bool PlayerHasTakenDamageInThisRoom;
  [NonSerialized]
  public bool ForcePreventChannels;
  [NonSerialized]
  public tk2dTileMap OverrideTilemap;
  [NonSerialized]
  public bool PreventMinimapUpdates;
  public RoomHandler.VisibilityStatus OverrideVisibility = RoomHandler.VisibilityStatus.INVALID;
  public bool PreventRevealEver;
  private RoomHandler.VisibilityStatus m_visibility;
  public bool forceTeleportersActive;
  public bool hasEverBeenVisited;
  public bool CompletelyPreventLeaving;
  public System.Action OnRevealedOnMap;
  private bool m_forceRevealedOnMap;
  public Transform hierarchyParent;
  public IntVector2 Epicenter;
  public GameObject ExtantEmergencyCrate;
  public bool PreventStandardRoomReward;
  public static bool HasGivenRoomChestRewardThisRun = false;
  public static int NumberOfRoomsToPreventChestSpawning = 0;
  private int m_roomVisualType;
  private RoomMotionHandler m_roomMotionHandler;
  public Dictionary<IntVector2, float> OcclusionPerimeterCellMap;
  public SecretRoomManager secretRoomManager;
  private HashSet<IntVector2> rawRoomCells = new HashSet<IntVector2>();
  private List<IntVector2> roomCells = new List<IntVector2>();
  private List<IntVector2> roomCellsWithoutExits = new List<IntVector2>();
  private List<IntVector2> featureCells = new List<IntVector2>();
  private List<RoomEventTriggerArea> eventTriggerAreas;
  private Dictionary<int, RoomEventTriggerArea> eventTriggerMap;
  private float m_totalSpawnedEnemyHP;
  private float m_lastTotalSpawnedEnemyHP;
  private float m_activeDifficultyModifier = 1f;
  private List<AIActor> activeEnemies;
  private List<IPlayerInteractable> interactableObjects;
  private List<IAutoAimTarget> autoAimTargets;
  private List<PrototypeRoomObjectLayer> remainingReinforcementLayers;
  private Dictionary<PrototypeRoomObjectLayer, Dictionary<PrototypePlacedObjectData, GameObject>> preloadedReinforcementLayerData;
  public static List<IPlayerInteractable> unassignedInteractableObjects = new List<IPlayerInteractable>();
  public RoomHandler.NPCSealState npcSealState;
  private bool m_isSealed;
  private bool m_currentlyVisible;
  private bool m_hasGivenReward;
  private GameObject m_secretRoomCoverObject;
  [NonSerialized]
  public RoomHandler TargetPitfallRoom;
  [NonSerialized]
  public bool ForcePitfallForFliers;
  public System.Action OnTargetPitfallRoom;
  public Action<PlayerController> OnPlayerReturnedFromPit;
  public System.Action OnDarkSoulsReset;
  [NonSerialized]
  private bool m_hasBeenEncountered;
  public RoomHandler.CustomRoomState AdditionalRoomState;
  private bool m_allRoomsActive;
  [NonSerialized]
  public bool? ForcedActiveState;
  private int numberOfTimedWavesOnDeck;
  public Action<bool> OnChangedTerrifyingDarkState;
  public bool IsDarkAndTerrifying;
  private bool? m_cachedIsMaintenance;
  public System.Action OnEnemiesCleared;
  public Func<bool> PreEnemiesCleared;
  private List<SpeculativeRigidbody> m_roomMovingPlatforms = new List<SpeculativeRigidbody>();
  private List<DeadlyDeadlyGoopManager> m_goops;
  [NonSerialized]
  public GenericLootTable OverrideBossRewardTable;
  public bool EverHadEnemies;
  public List<RoomHandler> DarkSoulsRoomResetDependencies;
  public Action<bool> OnSealChanged;
  public Action<AIActor> OnEnemyRegistered;
  private static List<AIActor> s_tempActiveEnemies = new List<AIActor>();

  public RoomHandler(CellArea a)
  {
    this.area = a;
    this.RoomVisualSubtype = !((UnityEngine.Object) GameManager.Instance.BestGenerationDungeonPrefab != (UnityEngine.Object) null) ? GameManager.Instance.Dungeon.decoSettings.standardRoomVisualSubtypes.SelectByWeight() : GameManager.Instance.BestGenerationDungeonPrefab.decoSettings.standardRoomVisualSubtypes.SelectByWeight();
    if ((UnityEngine.Object) this.area.prototypeRoom == (UnityEngine.Object) null)
      this.RoomVisualSubtype = 0;
    if ((UnityEngine.Object) a.prototypeRoom != (UnityEngine.Object) null && a.prototypeRoom.overrideRoomVisualType >= 0)
      this.RoomVisualSubtype = a.prototypeRoom.overrideRoomVisualType;
    if ((UnityEngine.Object) a.prototypeRoom != (UnityEngine.Object) null)
    {
      Dungeon dungeon = !((UnityEngine.Object) GameManager.Instance.BestGenerationDungeonPrefab != (UnityEngine.Object) null) ? GameManager.Instance.Dungeon : GameManager.Instance.BestGenerationDungeonPrefab;
      DungeonMaterial materialDefinition = dungeon.roomMaterialDefinitions[this.m_roomVisualType];
      bool flag1 = a.prototypeRoom.ContainsPit();
      bool flag2 = false;
      for (int index = 0; index < dungeon.decoSettings.standardRoomVisualSubtypes.elements.Length; ++index)
      {
        WeightedInt element = dungeon.decoSettings.standardRoomVisualSubtypes.elements[index];
        if ((double) element.weight > 0.0 && dungeon.roomMaterialDefinitions[element.value].supportsPits)
        {
          flag2 = true;
          break;
        }
      }
      if (flag2)
      {
        for (; flag1 && !materialDefinition.supportsPits; materialDefinition = dungeon.roomMaterialDefinitions[this.m_roomVisualType])
          this.RoomVisualSubtype = dungeon.decoSettings.standardRoomVisualSubtypes.SelectByWeight();
      }
      this.PrecludeTilemapDrawing = a.prototypeRoom.precludeAllTilemapDrawing;
      this.DrawPrecludedCeilingTiles = a.prototypeRoom.drawPrecludedCeilingTiles;
    }
    if ((UnityEngine.Object) GameManager.Instance.BestGenerationDungeonPrefab != (UnityEngine.Object) null)
    {
      if (this.m_roomVisualType < 0 || this.m_roomVisualType >= GameManager.Instance.BestGenerationDungeonPrefab.roomMaterialDefinitions.Length)
        this.m_roomVisualType = 0;
    }
    else if (this.m_roomVisualType < 0 || this.m_roomVisualType >= GameManager.Instance.Dungeon.roomMaterialDefinitions.Length)
      this.m_roomVisualType = 0;
    this.roomType = RoomCreationStrategy.RoomType.PREDEFINED_ROOM;
    this.opulence = Opulence.FINE;
    this.childRooms = new List<RoomHandler>();
    this.connectedDoors = new List<DungeonDoorController>();
    this.standaloneBlockers = new List<DungeonDoorSubsidiaryBlocker>();
    this.connectedRooms = new List<RoomHandler>();
    this.connectedRoomsByExit = new Dictionary<PrototypeRoomExit, RoomHandler>();
    this.interactableObjects = new List<IPlayerInteractable>();
    this.autoAimTargets = new List<IAutoAimTarget>();
    this.OnEnemiesCleared += new System.Action(this.NotifyPlayerRoomCleared);
    this.OnEnemiesCleared += new System.Action(this.HandleRoomClearReward);
  }

  public RoomHandler.VisibilityStatus visibility
  {
    get
    {
      return this.OverrideVisibility != RoomHandler.VisibilityStatus.INVALID ? this.OverrideVisibility : this.m_visibility;
    }
    set
    {
      this.m_visibility = value;
      if (this.m_visibility == RoomHandler.VisibilityStatus.OBSCURED || this.m_visibility == RoomHandler.VisibilityStatus.REOBSCURED)
      {
        this.hasEverBeenVisited = false;
      }
      else
      {
        if (this.m_visibility != RoomHandler.VisibilityStatus.VISITED)
          return;
        this.hasEverBeenVisited = true;
      }
    }
  }

  public bool TeleportersActive => this.IsVisible || this.forceTeleportersActive;

  public bool IsVisible
  {
    get
    {
      return this.visibility != RoomHandler.VisibilityStatus.OBSCURED && this.visibility != RoomHandler.VisibilityStatus.REOBSCURED;
    }
  }

  public bool IsShop { get; set; }

  public bool IsWildWestEntrance => false;

  public bool RevealedOnMap
  {
    get => this.visibility != RoomHandler.VisibilityStatus.OBSCURED || this.m_forceRevealedOnMap;
    set
    {
      if (!this.m_forceRevealedOnMap && this.OnRevealedOnMap != null)
        this.OnRevealedOnMap();
      this.m_forceRevealedOnMap = value;
    }
  }

  public int RoomVisualSubtype
  {
    get => this.m_roomVisualType;
    set => this.m_roomVisualType = value;
  }

  public DungeonMaterial RoomMaterial
  {
    get => GameManager.Instance.Dungeon.roomMaterialDefinitions[this.RoomVisualSubtype];
  }

  public List<IntVector2> Cells => this.roomCells;

  public List<IntVector2> CellsWithoutExits => this.roomCellsWithoutExits;

  public HashSet<IntVector2> RawCells => this.rawRoomCells;

  public List<IntVector2> FeatureCells => this.featureCells;

  public bool IsSealed => this.m_isSealed;

  public IntVector2? OverrideBossPedestalLocation { get; set; }

  public event RoomHandler.OnEnteredEventHandler Entered;

  public bool IsGunslingKingChallengeRoom { get; set; }

  public bool IsWinchesterArcadeRoom { get; set; }

  protected virtual void OnEntered(PlayerController p)
  {
    this.SetRoomActive(true);
    if ((UnityEngine.Object) this.OverrideTilemap != (UnityEngine.Object) null && (UnityEngine.Object) PhysicsEngine.Instance.TileMap != (UnityEngine.Object) this.OverrideTilemap)
    {
      PhysicsEngine.Instance.ClearAllCachedTiles();
      PhysicsEngine.Instance.TileMap = this.OverrideTilemap;
    }
    else if ((UnityEngine.Object) this.OverrideTilemap == (UnityEngine.Object) null && (UnityEngine.Object) PhysicsEngine.Instance.TileMap != (UnityEngine.Object) GameManager.Instance.Dungeon.MainTilemap)
    {
      PhysicsEngine.Instance.ClearAllCachedTiles();
      PhysicsEngine.Instance.TileMap = GameManager.Instance.Dungeon.MainTilemap;
    }
    if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.TUTORIAL)
      GameManager.Instance.Dungeon.StartCoroutine(this.DeferredMarkVisibleRoomsActive(p));
    if (!this.area.IsProceduralRoom && !this.m_hasBeenEncountered)
    {
      this.m_hasBeenEncountered = true;
      GameStatsManager.Instance.HandleEncounteredRoom(this.area.runtimePrototypeData);
    }
    if (!this.m_currentlyVisible)
      this.OnBecameVisible(p);
    if (GameManager.Instance.NumberOfLivingPlayers == 1 && !p.IsGhost)
      Minimap.Instance.RevealMinimapRoom(this);
    else if (p.IsPrimaryPlayer)
      Minimap.Instance.RevealMinimapRoom(this);
    if ((UnityEngine.Object) this.m_secretRoomCoverObject != (UnityEngine.Object) null)
      this.m_secretRoomCoverObject.SetActive(false);
    this.ProcessRoomEvents(RoomEventTriggerCondition.ON_ENTER);
    List<AIActor> activeEnemies = this.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
    int num1 = 0;
    if (activeEnemies != null && activeEnemies.Exists((Predicate<AIActor>) (a => !a.healthHaver.IsDead)))
    {
      int num2 = num1 + activeEnemies.Count;
      this.ProcessRoomEvents(RoomEventTriggerCondition.ON_ENTER_WITH_ENEMIES);
      if (this.remainingReinforcementLayers != null)
      {
        for (int index = 0; index < this.remainingReinforcementLayers.Count; ++index)
        {
          num2 += this.remainingReinforcementLayers[index].placedObjects.Count;
          if (this.remainingReinforcementLayers[index].reinforcementTriggerCondition == RoomEventTriggerCondition.TIMER)
            GameManager.Instance.StartCoroutine(this.HandleTimedReinforcementLayer(this.remainingReinforcementLayers[index]));
        }
      }
    }
    if (this.Entered != null)
      this.Entered(p);
    bool flag = true;
    for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
    {
      if (GameManager.Instance.Dungeon.data.rooms[index].visibility == RoomHandler.VisibilityStatus.OBSCURED)
      {
        flag = false;
        break;
      }
    }
    if (GameManager.Instance.Dungeon.OnAnyRoomVisited != null)
      GameManager.Instance.Dungeon.OnAnyRoomVisited();
    if (!flag)
      return;
    GameManager.Instance.Dungeon.NotifyAllRoomsVisited();
  }

  [DebuggerHidden]
  public IEnumerator DeferredMarkVisibleRoomsActive(PlayerController p)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RoomHandler__DeferredMarkVisibleRoomsActivec__Iterator0()
    {
      p = p,
      _this = this
    };
  }

  public bool SetRoomActive(bool active)
  {
    if (this.ForcedActiveState.HasValue && this.ForcedActiveState.Value != active || !((UnityEngine.Object) this.m_roomMotionHandler != (UnityEngine.Object) null) || this.m_roomMotionHandler.gameObject.activeSelf == active)
      return false;
    this.m_roomMotionHandler.gameObject.SetActive(active);
    return true;
  }

  [DebuggerHidden]
  private IEnumerator HandleTimedReinforcementLayer(PrototypeRoomObjectLayer layer)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RoomHandler__HandleTimedReinforcementLayerc__Iterator1()
    {
      layer = layer,
      _this = this
    };
  }

  public event RoomHandler.OnExitedEventHandler Exited;

  protected virtual void OnExited(PlayerController p)
  {
    if (this.m_currentlyVisible)
      this.OnBecameInvisible(p);
    if ((UnityEngine.Object) this.ExtantEmergencyCrate != (UnityEngine.Object) null)
    {
      EmergencyCrateController component = this.ExtantEmergencyCrate.GetComponent<EmergencyCrateController>();
      if ((bool) (UnityEngine.Object) component)
        component.ClearLandingTarget();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.ExtantEmergencyCrate);
      this.ExtantEmergencyCrate = (GameObject) null;
    }
    this.ProcessRoomEvents(RoomEventTriggerCondition.ON_EXIT);
    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
    {
      if (!(bool) (UnityEngine.Object) GameManager.Instance.GetOtherPlayer(p) || GameManager.Instance.GetOtherPlayer(p).CurrentRoom != this)
        Minimap.Instance.DeflagPreviousRoom(this);
    }
    else
      Minimap.Instance.DeflagPreviousRoom(this);
    if (this.Exited == null)
      return;
    this.Exited();
  }

  public bool RoomFallValidForMaintenance()
  {
    GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
    if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE || this != GameManager.Instance.Dungeon.data.Entrance)
      return false;
    return tilesetId == GlobalDungeonData.ValidTilesets.GUNGEON || tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON || tilesetId == GlobalDungeonData.ValidTilesets.CATACOMBGEON || tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON;
  }

  public void BecomeTerrifyingDarkRoom(
    float duration = 1f,
    float goalIntensity = 0.1f,
    float lightIntensity = 1f,
    string wwiseEvent = "Play_ENM_darken_world_01")
  {
    if (this.IsDarkAndTerrifying || this.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS && this.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) == 0)
      return;
    if (this.OnChangedTerrifyingDarkState != null)
      this.OnChangedTerrifyingDarkState(true);
    GameManager.Instance.StartCoroutine(this.HandleBecomeTerrifyingDarkRoom(duration, goalIntensity, lightIntensity));
    int num = (int) AkSoundEngine.PostEvent(wwiseEvent, GameManager.Instance.PrimaryPlayer.gameObject);
  }

  public void EndTerrifyingDarkRoom(
    float duration = 1f,
    float goalIntensity = 0.1f,
    float lightIntensity = 1f,
    string wwiseEvent = "Play_ENM_lighten_world_01")
  {
    if (!this.IsDarkAndTerrifying)
      return;
    if (this.OnChangedTerrifyingDarkState != null)
      this.OnChangedTerrifyingDarkState(false);
    GameManager.Instance.StartCoroutine(this.HandleBecomeTerrifyingDarkRoom(duration, goalIntensity, lightIntensity, true));
    int num = (int) AkSoundEngine.PostEvent(wwiseEvent, GameManager.Instance.PrimaryPlayer.gameObject);
  }

  [DebuggerHidden]
  private IEnumerator HandleBecomeTerrifyingDarkRoom(
    float duration,
    float goalIntensity,
    float lightIntensity = 1f,
    bool reverse = false)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RoomHandler__HandleBecomeTerrifyingDarkRoomc__Iterator2()
    {
      reverse = reverse,
      duration = duration,
      goalIntensity = goalIntensity,
      lightIntensity = lightIntensity,
      _this = this
    };
  }

  public bool IsMaintenanceRoom()
  {
    if (!this.m_cachedIsMaintenance.HasValue)
      this.m_cachedIsMaintenance = new bool?(!string.IsNullOrEmpty(this.GetRoomName()) && this.GetRoomName().Contains("Maintenance"));
    return this.m_cachedIsMaintenance.Value;
  }

  public string GetRoomName() => this.area.PrototypeRoomName;

  public void PlayerEnter(PlayerController playerEntering)
  {
    if (Pathfinder.HasInstance)
      Pathfinder.Instance.TryRecalculateRoomClearances(this);
    this.OnEntered(playerEntering);
    GameManager.Instance.DungeonMusicController.NotifyEnteredNewRoom(this);
    Pixelator.Instance.ProcessRoomAdditionalExits(playerEntering.transform.position.IntXY(VectorConversions.Floor), this, false);
  }

  public void PlayerInCell(
    PlayerController p,
    IntVector2 playerCellPosition,
    Vector2 relevantCornerOfPlayer)
  {
    if ((UnityEngine.Object) this.m_roomMotionHandler != (UnityEngine.Object) null && !this.m_roomMotionHandler.gameObject.activeSelf)
    {
      this.m_roomMotionHandler.gameObject.SetActive(true);
      GameManager.Instance.Dungeon.StartCoroutine(this.DeferredMarkVisibleRoomsActive(p));
    }
    if (GameManager.Instance.Dungeon.data.Entrance == this && this.m_allRoomsActive && GameManager.Instance.Dungeon.FrameDungeonGenerationFinished > 0 && UnityEngine.Time.frameCount - GameManager.Instance.Dungeon.FrameDungeonGenerationFinished > 100)
      GameManager.Instance.Dungeon.StartCoroutine(this.DeferredMarkVisibleRoomsActive(p));
    CellData cellData = GameManager.Instance.Dungeon.data[playerCellPosition];
    if (cellData != null && !cellData.isExitCell && cellData.parentRoom != null && !cellData.parentRoom.RevealedOnMap)
      Minimap.Instance.RevealMinimapRoom(cellData.parentRoom, true);
    if (this.ForcePitfallForFliers && (bool) (UnityEngine.Object) p && !p.IsFalling && p.IsFlying && cellData != null && cellData.type == CellType.PIT && !cellData.fallingPrevented)
    {
      Rect rect = new Rect();
      rect.min = PhysicsEngine.PixelToUnitMidpoint(p.specRigidbody.PrimaryPixelCollider.LowerLeft);
      rect.max = PhysicsEngine.PixelToUnitMidpoint(p.specRigidbody.PrimaryPixelCollider.UpperRight);
      Dungeon dungeon = GameManager.Instance.Dungeon;
      bool flag1 = dungeon.ShouldReallyFall((Vector3) rect.min);
      bool flag2 = dungeon.ShouldReallyFall(new Vector3(rect.xMax, rect.yMin));
      bool flag3 = dungeon.ShouldReallyFall(new Vector3(rect.xMin, rect.yMax));
      bool flag4 = dungeon.ShouldReallyFall((Vector3) rect.max);
      bool flag5 = dungeon.ShouldReallyFall((Vector3) rect.center);
      if (flag1 && flag2 && flag5 && flag3 && flag4)
        p.ForceFall();
    }
    if (cellData.doesDamage && ((double) cellData.damageDefinition.damageToPlayersPerTick > 0.0 || cellData.damageDefinition.isPoison) && p.IsGrounded && (double) p.CurrentFloorDamageCooldown <= 0.0 && p.healthHaver.IsVulnerable)
    {
      bool flag = true;
      int tile = GameManager.Instance.Dungeon.MainTilemap.Layers[GlobalDungeonData.floorLayerIndex].GetTile(playerCellPosition.x, playerCellPosition.y);
      if (tile >= 0 && tile < GameManager.Instance.Dungeon.tileIndices.dungeonCollection.spriteDefinitions.Length)
      {
        BagelCollider[] bagelColliders = GameManager.Instance.Dungeon.tileIndices.dungeonCollection.spriteDefinitions[tile] == null ? (BagelCollider[]) null : GameManager.Instance.Dungeon.tileIndices.dungeonCollection.GetBagelColliders(tile);
        if (bagelColliders != null && bagelColliders.Length > 0)
        {
          flag = false;
          BagelCollider bagelCollider = bagelColliders[0];
          IntVector2 intVector2 = ((p.specRigidbody.PrimaryPixelCollider.UnitCenter - playerCellPosition.ToVector2()) * 16f).ToIntVector2(VectorConversions.Floor);
          if (intVector2.x >= 0 && intVector2.y >= 0 && intVector2.x < 16 /*0x10*/ && intVector2.y < 16 /*0x10*/ && bagelCollider[intVector2.x, bagelCollider.height - intVector2.y - 1])
            flag = true;
        }
      }
      if (flag)
      {
        if (cellData.damageDefinition.isPoison || GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)
        {
          p.IncreasePoison(BraveTime.DeltaTime / cellData.damageDefinition.tickFrequency);
          if ((double) p.CurrentPoisonMeterValue >= 1.0)
          {
            p.healthHaver.ApplyDamage(cellData.damageDefinition.damageToPlayersPerTick, Vector2.zero, StringTableManager.GetEnemiesString("#THEFLOOR"), cellData.damageDefinition.damageTypes, DamageCategory.Environment);
            --p.CurrentPoisonMeterValue;
          }
        }
        else if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
        {
          p.IsOnFire = true;
        }
        else
        {
          p.healthHaver.ApplyDamage(cellData.damageDefinition.damageToPlayersPerTick, Vector2.zero, StringTableManager.GetEnemiesString("#THEFLOOR"), cellData.damageDefinition.damageTypes, DamageCategory.Environment);
          p.CurrentFloorDamageCooldown = cellData.damageDefinition.tickFrequency;
        }
      }
    }
    if (this.eventTriggerAreas != null)
    {
      for (int index = 0; index < this.eventTriggerAreas.Count; ++index)
      {
        RoomEventTriggerArea eventTriggerArea = this.eventTriggerAreas[index];
        if (eventTriggerArea.triggerCells.Contains(playerCellPosition))
          eventTriggerArea.Trigger(index);
      }
    }
    if (this.activeEnemies != null)
    {
      for (int index = 0; index < this.activeEnemies.Count; ++index)
      {
        if (!(bool) (UnityEngine.Object) this.activeEnemies[index])
        {
          this.activeEnemies.RemoveAt(index);
          --index;
        }
      }
    }
    if (this.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) || this.numberOfTimedWavesOnDeck > 0 || !this.area.IsProceduralRoom && !this.area.runtimePrototypeData.DoesUnsealOnClear())
      return;
    this.UnsealRoom();
  }

  public void PlayerExit(PlayerController playerExiting) => this.OnExited(playerExiting);

  public bool ContainsPosition(IntVector2 pos) => this.rawRoomCells.Contains(pos);

  public bool ContainsCell(CellData cell) => this.roomCells.Contains(cell.position);

  public bool IsStartOfWarpWing
  {
    get
    {
      if (this.area.instanceUsedExits.Count == 0 && !this.area.IsProceduralRoom)
        return true;
      for (int index = 0; index < this.area.instanceUsedExits.Count; ++index)
      {
        if (this.area.exitToLocalDataMap.ContainsKey(this.area.instanceUsedExits[index]) && this.area.exitToLocalDataMap[this.area.instanceUsedExits[index]].isWarpWingStart)
          return true;
      }
      return false;
    }
  }

  public bool IsStandardRoom
  {
    get
    {
      return this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.NORMAL || this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.CONNECTOR || this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.HUB;
    }
  }

  public bool IsSecretRoom
  {
    get => !((UnityEngine.Object) this.secretRoomManager == (UnityEngine.Object) null) && !this.secretRoomManager.IsOpen;
  }

  public bool WasEverSecretRoom => !((UnityEngine.Object) this.secretRoomManager == (UnityEngine.Object) null);

  public event RoomHandler.OnBecameVisibleEventHandler BecameVisible;

  public virtual void OnBecameVisible(PlayerController p)
  {
    if (this.m_currentlyVisible)
      return;
    this.m_currentlyVisible = true;
    this.visibility = RoomHandler.VisibilityStatus.CURRENT;
    float delay = this.UpdateOcclusionData(p, 1f);
    if (this.BecameVisible == null)
      return;
    this.BecameVisible(delay);
  }

  public event RoomHandler.OnBecameInvisibleEventHandler BecameInvisible;

  public virtual void OnBecameInvisible(PlayerController p)
  {
    if (!this.m_currentlyVisible)
      return;
    bool flag = false;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      if (!GameManager.Instance.AllPlayers[index].healthHaver.IsDead && GameManager.Instance.AllPlayers[index].CurrentRoom == this)
        flag = true;
    }
    if (flag)
      return;
    this.m_currentlyVisible = false;
    this.visibility = RoomHandler.VisibilityStatus.VISITED;
    double num = (double) this.UpdateOcclusionData(0.3f, p.transform.position.IntXY(VectorConversions.Floor));
    if (this.BecameInvisible == null)
      return;
    this.BecameInvisible();
  }

  public bool WillSealOnEntry()
  {
    List<AIActor> activeEnemies = this.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
    bool flag = activeEnemies != null && activeEnemies.Exists((Predicate<AIActor>) (a => !a.healthHaver.IsDead));
    if (this.area.IsProceduralRoom)
      return flag;
    if (this.area.runtimePrototypeData.roomEvents != null && this.area.runtimePrototypeData.roomEvents.Count > 0)
    {
      for (int index = 0; index < this.area.runtimePrototypeData.roomEvents.Count; ++index)
      {
        RoomEventDefinition roomEvent = this.area.runtimePrototypeData.roomEvents[index];
        if (roomEvent.condition == RoomEventTriggerCondition.ON_ENTER && roomEvent.action == RoomEventTriggerAction.SEAL_ROOM || flag && roomEvent.condition == RoomEventTriggerCondition.ON_ENTER_WITH_ENEMIES && roomEvent.action == RoomEventTriggerAction.SEAL_ROOM)
          return true;
      }
    }
    return false;
  }

  protected virtual void ProcessRoomEvents(RoomEventTriggerCondition eventCondition)
  {
    if (!this.area.IsProceduralRoom && this.area.runtimePrototypeData.roomEvents != null && this.area.runtimePrototypeData.roomEvents.Count > 0)
    {
      for (int index = 0; index < this.area.runtimePrototypeData.roomEvents.Count; ++index)
      {
        RoomEventDefinition roomEvent = this.area.runtimePrototypeData.roomEvents[index];
        if (roomEvent.condition == eventCondition)
          this.HandleRoomAction(roomEvent.action);
      }
    }
    else
    {
      if (!this.area.IsProceduralRoom)
        return;
      if (eventCondition == RoomEventTriggerCondition.ON_ENTER_WITH_ENEMIES)
      {
        this.HandleRoomAction(RoomEventTriggerAction.SEAL_ROOM);
      }
      else
      {
        if (eventCondition != RoomEventTriggerCondition.ON_ENEMIES_CLEARED)
          return;
        this.HandleRoomAction(RoomEventTriggerAction.UNSEAL_ROOM);
      }
    }
  }

  public virtual void HandleRoomAction(RoomEventTriggerAction action)
  {
    switch (action)
    {
      case RoomEventTriggerAction.SEAL_ROOM:
        this.SealRoom();
        break;
      case RoomEventTriggerAction.UNSEAL_ROOM:
        this.UnsealRoom();
        break;
      case RoomEventTriggerAction.BECOME_TERRIFYING_AND_DARK:
        this.BecomeTerrifyingDarkRoom();
        break;
      case RoomEventTriggerAction.END_TERRIFYING_AND_DARK:
        this.EndTerrifyingDarkRoom();
        break;
      default:
        BraveUtility.Log("RoomHandler received event that is triggering undefined RoomEventTriggerAction.", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
        break;
    }
  }

  protected void PreLoadReinforcements()
  {
    if (this.area.runtimePrototypeData == null || this.area.runtimePrototypeData.additionalObjectLayers == null || this.area.runtimePrototypeData.additionalObjectLayers.Count == 0)
      return;
    if (this.preloadedReinforcementLayerData == null)
      this.preloadedReinforcementLayerData = new Dictionary<PrototypeRoomObjectLayer, Dictionary<PrototypePlacedObjectData, GameObject>>();
    int index1 = 0;
    int num = 0;
    for (; index1 < this.area.runtimePrototypeData.additionalObjectLayers.Count; ++index1)
    {
      PrototypeRoomObjectLayer additionalObjectLayer = this.area.runtimePrototypeData.additionalObjectLayers[index1];
      if (additionalObjectLayer.layerIsReinforcementLayer)
      {
        List<Vector2> vector2List;
        if (additionalObjectLayer.shuffle)
        {
          vector2List = new List<Vector2>((IEnumerable<Vector2>) additionalObjectLayer.placedObjectBasePositions);
          for (int index2 = vector2List.Count - 1; index2 > 0; --index2)
          {
            int index3 = UnityEngine.Random.Range(0, index2 + 1);
            if (index2 != index3)
            {
              Vector2 vector2 = vector2List[index2];
              vector2List[index2] = vector2List[index3];
              vector2List[index3] = vector2;
            }
          }
        }
        else
          vector2List = additionalObjectLayer.placedObjectBasePositions;
        for (int index4 = 0; index4 < additionalObjectLayer.placedObjects.Count && this.remainingReinforcementLayers != null && this.remainingReinforcementLayers.Contains(additionalObjectLayer); ++index4)
        {
          GameObject gameObject = this.PreloadReinforcementObject(additionalObjectLayer.placedObjects[index4], vector2List[index4].ToIntVector2(), additionalObjectLayer.suppressPlayerChecks);
          if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
            ++num;
          if (!this.preloadedReinforcementLayerData.ContainsKey(additionalObjectLayer))
            this.preloadedReinforcementLayerData.Add(additionalObjectLayer, new Dictionary<PrototypePlacedObjectData, GameObject>());
          this.preloadedReinforcementLayerData[additionalObjectLayer].Add(additionalObjectLayer.placedObjects[index4], gameObject);
        }
      }
    }
  }

  [DebuggerHidden]
  protected IEnumerator HandleReinforcementPreloading()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new RoomHandler__HandleReinforcementPreloadingc__Iterator3()
    {
      _this = this
    };
  }

  public int GetEnemiesInReinforcementLayer(int index)
  {
    return this.remainingReinforcementLayers == null || index >= this.remainingReinforcementLayers.Count ? 0 : this.remainingReinforcementLayers[index].placedObjects.Count;
  }

  public bool TriggerReinforcementLayer(
    int index,
    bool removeLayer = true,
    bool disableDrops = false,
    int specifyObjectIndex = -1,
    int specifyObjectCount = -1,
    bool instant = false)
  {
    if (this.remainingReinforcementLayers == null || index < 0 || index >= this.remainingReinforcementLayers.Count)
      return false;
    PrototypeRoomObjectLayer reinforcementLayer = this.remainingReinforcementLayers[index];
    if (removeLayer)
      this.remainingReinforcementLayers.RemoveAt(index);
    float difficultyModifier = this.m_activeDifficultyModifier;
    this.m_activeDifficultyModifier = 1f;
    int count = this.activeEnemies != null ? this.activeEnemies.Count : 0;
    List<GameObject> sourceObjects = this.PlaceObjectsFromLayer(reinforcementLayer.placedObjects, reinforcementLayer, reinforcementLayer.placedObjectBasePositions, (Dictionary<int, RoomEventTriggerArea>) null, !instant, reinforcementLayer.shuffle, reinforcementLayer.randomize, reinforcementLayer.suppressPlayerChecks, disableDrops, specifyObjectIndex, specifyObjectCount);
    bool flag = this.activeEnemies.Count > count;
    if ((double) difficultyModifier != 1.0)
      this.MakeRoomMoreDifficult(difficultyModifier, sourceObjects);
    if (GameManager.Instance.DungeonMusicController.CurrentState != DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A && GameManager.Instance.DungeonMusicController.CurrentState != DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B && !GameManager.Instance.DungeonMusicController.MusicOverridden)
      GameManager.Instance.DungeonMusicController.NotifyRoomSuddenlyHasEnemies(this);
    this.ResetEnemyHPPercentage();
    return flag;
  }

  public void TriggerNextReinforcementLayer()
  {
    if (this.remainingReinforcementLayers == null || this.remainingReinforcementLayers.Count <= 0)
      return;
    this.TriggerReinforcementLayer(0);
  }

  public void ClearReinforcementLayers()
  {
    this.remainingReinforcementLayers = (List<PrototypeRoomObjectLayer>) null;
  }

  public List<SpeculativeRigidbody> RoomMovingPlatforms => this.m_roomMovingPlatforms;

  public List<DeadlyDeadlyGoopManager> RoomGoops => this.m_goops;

  public void RegisterGoopManagerInRoom(DeadlyDeadlyGoopManager manager)
  {
    if (this.m_goops == null)
      this.m_goops = new List<DeadlyDeadlyGoopManager>();
    if (this.m_goops.Contains(manager))
      return;
    this.m_goops.Add(manager);
  }

  public RoomHandler GetRandomDownstreamRoom()
  {
    List<RoomHandler> roomHandlerList = new List<RoomHandler>();
    for (int index = 0; index < this.connectedRooms.Count; ++index)
    {
      if (this.connectedRooms[index].distanceFromEntrance > this.distanceFromEntrance)
        roomHandlerList.Add(this.connectedRooms[index]);
    }
    return roomHandlerList.Count == 0 ? (RoomHandler) null : roomHandlerList[UnityEngine.Random.Range(0, roomHandlerList.Count)];
  }

  public HashSet<IntVector2> GetCellsAndAllConnectedExitsSlow(bool includeSecret = false)
  {
    HashSet<IntVector2> connectedExitsSlow = new HashSet<IntVector2>((IEnumerable<IntVector2>) this.RawCells);
    List<IntVector2> intVector2List = new List<IntVector2>();
    if (this.area != null && this.area.instanceUsedExits != null)
    {
      for (int index = 0; index < this.area.instanceUsedExits.Count; ++index)
      {
        RuntimeRoomExitData key;
        RuntimeExitDefinition runtimeExitDefinition;
        if (this.area.exitToLocalDataMap.TryGetValue(this.area.instanceUsedExits[index], out key) && this.exitDefinitionsByExit.TryGetValue(key, out runtimeExitDefinition) && runtimeExitDefinition != null && (!runtimeExitDefinition.downstreamRoom.IsSecretRoom && !runtimeExitDefinition.upstreamRoom.IsSecretRoom || includeSecret))
        {
          HashSet<IntVector2> cellsForRoom = runtimeExitDefinition.GetCellsForRoom(this);
          HashSet<IntVector2> cellsForOtherRoom = runtimeExitDefinition.GetCellsForOtherRoom(this);
          foreach (IntVector2 intVector2 in cellsForRoom)
            connectedExitsSlow.Add(intVector2);
          foreach (IntVector2 intVector2 in cellsForOtherRoom)
            connectedExitsSlow.Add(intVector2);
        }
      }
    }
    DungeonData data = GameManager.Instance.BestGenerationDungeonPrefab.data;
    foreach (IntVector2 key in connectedExitsSlow)
    {
      if (data[key] != null && data[key].isWallMimicHideout)
        intVector2List.Add(key);
    }
    for (int index = 0; index < intVector2List.Count; ++index)
      connectedExitsSlow.Remove(intVector2List[index]);
    return connectedExitsSlow;
  }

  private List<Tuple<IntVector2, float>> GetGoodSpotsInternal(int dx, int dy, bool restrictive = false)
  {
    List<Tuple<IntVector2, float>> goodSpotsInternal = new List<Tuple<IntVector2, float>>();
    for (int index = 0; index < this.CellsWithoutExits.Count; ++index)
    {
      bool flag = true;
      CellData cellData1 = GameManager.Instance.Dungeon.data[this.CellsWithoutExits[index]];
      float num1 = 0.0f;
      for (int x = 0; x < dx; ++x)
      {
        for (int y = 0; y < dy; ++y)
        {
          CellData cellData2 = GameManager.Instance.Dungeon.data[cellData1.position + new IntVector2(x, y)];
          if (cellData2.IsTopWall())
            num1 -= 5f;
          if (cellData2.HasWallNeighbor())
            num1 -= 2f;
          else
            num1 += 2f;
          if (GameManager.Instance.Dungeon.data[cellData1.position + new IntVector2(x, y - 1)].type == CellType.PIT)
            num1 -= 50f;
          if (cellData2.type != CellType.FLOOR || cellData2.isOccupied)
          {
            flag = false;
            break;
          }
          if (restrictive && (cellData2.doesDamage || cellData2.cellVisualData.IsPhantomCarpet || cellData2.containsTrap))
          {
            flag = false;
            break;
          }
        }
        if (!flag)
          break;
      }
      int num2 = Math.Abs(this.area.basePosition.x + this.area.dimensions.x - (cellData1.position.x + dx / 2) - (cellData1.position.x + dx / 2 - this.area.basePosition.x));
      int num3 = Math.Abs(this.area.basePosition.y + this.area.dimensions.y - (cellData1.position.y + dy / 2) - (cellData1.position.y + dy / 2 - this.area.basePosition.y));
      if (num2 <= 3 && num3 <= 3)
        num1 += 10f;
      else if (num2 <= 5 && num3 <= 5)
        num1 += 5f;
      if (flag)
      {
        float second = 1f + num1;
        Tuple<IntVector2, float> tuple = new Tuple<IntVector2, float>(cellData1.position, second);
        goodSpotsInternal.Add(tuple);
      }
    }
    return goodSpotsInternal;
  }

  public IntVector2 GetRandomVisibleClearSpot(int dx, int dy)
  {
    List<Tuple<IntVector2, float>> goodSpotsInternal = this.GetGoodSpotsInternal(dx, dy);
    return goodSpotsInternal.Count == 0 ? IntVector2.Zero : goodSpotsInternal[UnityEngine.Random.Range(0, goodSpotsInternal.Count)].First;
  }

  public IntVector2 GetCenteredVisibleClearSpot(
    int dx,
    int dy,
    out bool success,
    bool restrictive = false)
  {
    List<Tuple<IntVector2, float>> goodSpotsInternal = this.GetGoodSpotsInternal(dx, dy, restrictive);
    float num = float.MinValue;
    IntVector2 visibleClearSpot = this.Epicenter;
    success = false;
    for (int index = 0; index < goodSpotsInternal.Count; ++index)
    {
      if ((double) goodSpotsInternal[index].Second > (double) num)
      {
        visibleClearSpot = goodSpotsInternal[index].First;
        num = goodSpotsInternal[index].Second;
        success = true;
      }
    }
    return visibleClearSpot;
  }

  public IntVector2 GetCenteredVisibleClearSpot(int dx, int dy)
  {
    bool success = false;
    return this.GetCenteredVisibleClearSpot(dx, dy, out success);
  }

  protected virtual void HandleBossClearReward()
  {
    if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT)
      GameStatsManager.Instance.CurrentResRatShopSeed = UnityEngine.Random.Range(1, 1000000);
    GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
    if (!this.PlayerHasTakenDamageInThisRoom && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE)
    {
      switch (tilesetId)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_GUNGEON, true);
          break;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_CASTLE, true);
          break;
        case GlobalDungeonData.ValidTilesets.MINEGEON:
          GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_MINES, true);
          break;
        case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
          GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_HOLLOW, true);
          break;
        case GlobalDungeonData.ValidTilesets.FORGEGEON:
          GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_NOBOSSDAMAGE_FORGE, true);
          break;
      }
    }
    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON || tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
      return;
    for (int index = 0; index < this.connectedRooms.Count; ++index)
    {
      if (this.connectedRooms[index].area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.EXIT)
        this.connectedRooms[index].OnBecameVisible(GameManager.Instance.BestActivePlayer);
    }
    IntVector2 zero = IntVector2.Zero;
    IntVector2 intVector2;
    if (this.OverrideBossPedestalLocation.HasValue)
      intVector2 = this.OverrideBossPedestalLocation.Value;
    else if (!this.area.IsProceduralRoom && this.area.runtimePrototypeData.rewardChestSpawnPosition != IntVector2.NegOne)
    {
      intVector2 = this.area.basePosition + this.area.runtimePrototypeData.rewardChestSpawnPosition;
    }
    else
    {
      UnityEngine.Debug.LogWarning((object) "BOSS REWARD PEDESTALS SHOULD REALLY HAVE FIXED LOCATIONS! The spawn code was written by dave, so no guarantees...");
      intVector2 = this.GetCenteredVisibleClearSpot(2, 2);
    }
    GameObject gameObject = GameManager.Instance.Dungeon.sharedSettingsPrefab.ChestsForBosses.SelectByWeight();
    Chest chestPrefab = gameObject.GetComponent<Chest>();
    if (GameStatsManager.Instance.IsRainbowRun)
      chestPrefab = (Chest) null;
    if ((UnityEngine.Object) chestPrefab != (UnityEngine.Object) null)
    {
      Chest.Spawn(chestPrefab, intVector2, this).RegisterChestOnMinimap(this);
    }
    else
    {
      DungeonData data = GameManager.Instance.Dungeon.data;
      RewardPedestal component = gameObject.GetComponent<RewardPedestal>();
      if ((bool) (UnityEngine.Object) component)
      {
        bool flag1 = tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON;
        bool flag2 = !this.PlayerHasTakenDamageInThisRoom && GameManager.Instance.Dungeon.BossMasteryTokenItemId >= 0 && !GameManager.Instance.Dungeon.HasGivenMasteryToken;
        if (flag1 && flag2)
          intVector2 += IntVector2.Left;
        if (flag1)
        {
          RewardPedestal rewardPedestal1 = RewardPedestal.Spawn(component, intVector2, this);
          rewardPedestal1.IsBossRewardPedestal = true;
          rewardPedestal1.lootTable.lootTable = this.OverrideBossRewardTable;
          rewardPedestal1.RegisterChestOnMinimap(this);
          data[intVector2].isOccupied = true;
          data[intVector2 + IntVector2.Right].isOccupied = true;
          data[intVector2 + IntVector2.Up].isOccupied = true;
          data[intVector2 + IntVector2.One].isOccupied = true;
          if (flag2)
            rewardPedestal1.OffsetTertiarySet = true;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.NumberOfLivingPlayers == 1)
            rewardPedestal1.ReturnCoopPlayerOnLand = true;
          if (this.area.PrototypeRoomName == "DoubleBeholsterRoom01")
          {
            for (int index = 0; index < 8; ++index)
            {
              IntVector2 visibleClearSpot = this.GetCenteredVisibleClearSpot(2, 2);
              RewardPedestal rewardPedestal2 = RewardPedestal.Spawn(component, visibleClearSpot, this);
              rewardPedestal2.IsBossRewardPedestal = true;
              rewardPedestal2.lootTable.lootTable = this.OverrideBossRewardTable;
              data[visibleClearSpot].isOccupied = true;
              data[visibleClearSpot + IntVector2.Right].isOccupied = true;
              data[visibleClearSpot + IntVector2.Up].isOccupied = true;
              data[visibleClearSpot + IntVector2.One].isOccupied = true;
            }
          }
        }
        else if (tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.NumberOfLivingPlayers == 1)
        {
          PlayerController playerController = !GameManager.Instance.PrimaryPlayer.healthHaver.IsDead ? GameManager.Instance.SecondaryPlayer : GameManager.Instance.PrimaryPlayer;
          playerController.specRigidbody.enabled = true;
          playerController.gameObject.SetActive(true);
          playerController.ResurrectFromBossKill();
        }
        if (flag2)
        {
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.MASTERY_TOKENS_RECEIVED, 1f);
          ++GameManager.Instance.PrimaryPlayer.MasteryTokensCollectedThisRun;
          if (flag1)
            intVector2 += new IntVector2(2, 0);
          RewardPedestal rewardPedestal = RewardPedestal.Spawn(component, intVector2, this);
          data[intVector2].isOccupied = true;
          data[intVector2 + IntVector2.Right].isOccupied = true;
          data[intVector2 + IntVector2.Up].isOccupied = true;
          data[intVector2 + IntVector2.One].isOccupied = true;
          GameManager.Instance.Dungeon.HasGivenMasteryToken = true;
          rewardPedestal.SpawnsTertiarySet = false;
          rewardPedestal.contents = PickupObjectDatabase.GetById(GameManager.Instance.Dungeon.BossMasteryTokenItemId);
          rewardPedestal.MimicGuid = (string) null;
        }
      }
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CATHEDRALGEON || GameManager.Options.CurrentGameLootProfile != GameOptions.GameLootProfile.CURRENT)
        return;
      IntVector2? randomAvailableCell = this.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR));
      IntVector2? nullable = !randomAvailableCell.HasValue ? new IntVector2?() : new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
      if (!nullable.HasValue)
        return;
      Chest chest = Chest.Spawn(GameManager.Instance.RewardManager.Synergy_Chest, nullable.Value);
      if (!(bool) (UnityEngine.Object) chest)
        return;
      chest.RegisterChestOnMinimap(this);
    }
  }

  public Chest SpawnRoomRewardChest(WeightedGameObjectCollection chestCollection, IntVector2 pos)
  {
    Chest chest = chestCollection == null ? GameManager.Instance.RewardManager.SpawnRoomClearChestAt(pos) : Chest.Spawn(chestCollection.SelectByWeight().GetComponent<Chest>(), pos, this);
    if ((UnityEngine.Object) chest != (UnityEngine.Object) null)
      chest.RegisterChestOnMinimap(this);
    return chest;
  }

  public IntVector2 GetBestRewardLocation(
    IntVector2 rewardSize,
    RoomHandler.RewardLocationStyle locationStyle = RoomHandler.RewardLocationStyle.CameraCenter,
    bool giveChestBuffer = true)
  {
    Vector2 idealPoint = locationStyle != RoomHandler.RewardLocationStyle.CameraCenter || GameManager.Instance.InTutorial ? (locationStyle != RoomHandler.RewardLocationStyle.CameraCenter || GameManager.Instance.InTutorial ? (locationStyle != RoomHandler.RewardLocationStyle.PlayerCenter ? (this.area.IsProceduralRoom || this.area.runtimePrototypeData == null || !(this.area.runtimePrototypeData.rewardChestSpawnPosition != IntVector2.NegOne) ? (this.area.IsProceduralRoom || !((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null) || !(this.area.prototypeRoom.rewardChestSpawnPosition != IntVector2.NegOne) ? this.area.basePosition.ToVector2() + this.area.dimensions.ToVector2() / 2f : (this.area.basePosition + this.area.prototypeRoom.rewardChestSpawnPosition).ToVector2() + rewardSize.ToVector2() / 2f) : (this.area.basePosition + this.area.runtimePrototypeData.rewardChestSpawnPosition).ToVector2() + rewardSize.ToVector2() / 2f) : (!GameManager.Instance.PrimaryPlayer.healthHaver.IsDead || GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER ? GameManager.Instance.PrimaryPlayer.CenterPosition : GameManager.Instance.SecondaryPlayer.CenterPosition)) : (GameManager.Instance.PrimaryPlayer.healthHaver.IsDead ? (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER || GameManager.Instance.SecondaryPlayer.healthHaver.IsDead ? (Vector2) BraveUtility.ScreenCenterWorldPoint() : GameManager.Instance.SecondaryPlayer.CenterPosition) : GameManager.Instance.PrimaryPlayer.CenterPosition)) : (Vector2) BraveUtility.ScreenCenterWorldPoint();
    return this.GetBestRewardLocation(rewardSize, idealPoint, giveChestBuffer);
  }

  public IntVector2 GetBestRewardLocation(
    IntVector2 rewardSize,
    Vector2 idealPoint,
    bool giveChestBuffer = true)
  {
    IntVector2[] playerPos = new IntVector2[GameManager.Instance.AllPlayers.Length];
    IntVector2[] playerDim = new IntVector2[playerPos.Length];
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      PixelCollider hitboxPixelCollider = GameManager.Instance.AllPlayers[index].specRigidbody.HitboxPixelCollider;
      playerPos[index] = hitboxPixelCollider.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
      playerDim[index] = hitboxPixelCollider.UnitTopRight.ToIntVector2(VectorConversions.Floor) - playerPos[index] + IntVector2.One;
    }
    IntVector2 modifiedRewardSize = !giveChestBuffer ? rewardSize : rewardSize + new IntVector2(2, 2);
    CellValidator cellValidator = (CellValidator) (c =>
    {
      IntVector2 posB = !giveChestBuffer ? c : c + new IntVector2(1, 2);
      for (int index = 0; index < playerPos.Length; ++index)
      {
        if (IntVector2.AABBOverlap(playerPos[index], playerDim[index], posB, rewardSize))
          return false;
      }
      for (int index1 = 0; index1 < modifiedRewardSize.x; ++index1)
      {
        if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y))
          return false;
        for (int index2 = 0; index2 < modifiedRewardSize.y; ++index2)
        {
          if (!GameManager.Instance.Dungeon.data.CheckInBounds(c.x + index1, c.y + index2) || GameManager.Instance.Dungeon.data.isWall(c.x + index1, c.y + index2))
            return false;
          CellData cellData = GameManager.Instance.Dungeon.data.cellData[c.x + index1][c.y + index2];
          if (cellData.containsTrap || cellData.PreventRewardSpawn)
            return false;
        }
      }
      return true;
    });
    IntVector2? nullable = this.GetNearestAvailableCell(idealPoint, new IntVector2?(modifiedRewardSize), new CellTypes?(CellTypes.FLOOR), cellValidator: cellValidator);
    if (nullable.HasValue)
    {
      if (giveChestBuffer)
        nullable = new IntVector2?(nullable.Value + new IntVector2(1, 2));
      return nullable.Value;
    }
    IntVector2 zero = IntVector2.Zero;
    return this.area.IsProceduralRoom || this.area.runtimePrototypeData == null || !(this.area.runtimePrototypeData.rewardChestSpawnPosition != IntVector2.NegOne) ? (this.area.IsProceduralRoom || !((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null) || !(this.area.prototypeRoom.rewardChestSpawnPosition != IntVector2.NegOne) ? this.GetCenteredVisibleClearSpot(3, 2) : this.area.basePosition + this.area.prototypeRoom.rewardChestSpawnPosition) : this.area.basePosition + this.area.runtimePrototypeData.rewardChestSpawnPosition;
  }

  public virtual void HandleRoomClearReward()
  {
    if (GameManager.Instance.IsFoyer || GameManager.Instance.InTutorial || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || this.m_hasGivenReward || this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD)
      return;
    this.m_hasGivenReward = true;
    if (this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && this.area.PrototypeRoomBossSubcategory == PrototypeDungeonRoom.RoomBossSubCategory.FLOOR_BOSS)
    {
      this.HandleBossClearReward();
    }
    else
    {
      if (this.PreventStandardRoomReward)
        return;
      FloorRewardData currentRewardData = GameManager.Instance.RewardManager.CurrentRewardData;
      LootEngine.AmmoDropType AmmoToDrop = LootEngine.AmmoDropType.DEFAULT_AMMO;
      bool flag1 = LootEngine.DoAmmoClipCheck(currentRewardData, out AmmoToDrop);
      string path = AmmoToDrop != LootEngine.AmmoDropType.SPREAD_AMMO ? "Ammo_Pickup" : "Ammo_Pickup_Spread";
      float num1 = UnityEngine.Random.value;
      float chanceLowerBound = currentRewardData.ChestSystem_ChestChanceLowerBound;
      float num2 = GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness) / 100f;
      float num3 = (float) -((double) GameManager.Instance.PrimaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse) / 100.0);
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        num2 += GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Coolness) / 100f;
        num3 -= GameManager.Instance.SecondaryPlayer.stats.GetStatValue(PlayerStats.StatType.Curse) / 100f;
      }
      if (PassiveItem.IsFlagSetAtAll(typeof (ChamberOfEvilItem)))
        num3 *= -2f;
      float num4 = Mathf.Clamp(chanceLowerBound + GameManager.Instance.PrimaryPlayer.AdditionalChestSpawnChance, currentRewardData.ChestSystem_ChestChanceLowerBound, currentRewardData.ChestSystem_ChestChanceUpperBound) + num2 + num3;
      bool flag2 = (UnityEngine.Object) currentRewardData.SingleItemRewardTable != (UnityEngine.Object) null;
      bool flag3 = false;
      float num5 = 0.1f;
      if (!RoomHandler.HasGivenRoomChestRewardThisRun && MetaInjectionData.ForceEarlyChest)
        flag3 = true;
      if (flag3)
      {
        if (!RoomHandler.HasGivenRoomChestRewardThisRun && (GameManager.Instance.CurrentFloor == 1 || GameManager.Instance.CurrentFloor == -1))
        {
          flag2 = false;
          num4 += num5;
          if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && GameManager.Instance.PrimaryPlayer.NumRoomsCleared > 4)
            num4 = 1f;
        }
        if (!RoomHandler.HasGivenRoomChestRewardThisRun && this.distanceFromEntrance < RoomHandler.NumberOfRoomsToPreventChestSpawning)
        {
          GameManager.Instance.Dungeon.InformRoomCleared(false, false);
          return;
        }
      }
      BraveUtility.Log("Current chest spawn chance: " + (object) num4, Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);
      if ((double) num1 > (double) num4)
      {
        if (flag1)
        {
          IntVector2 bestRewardLocation = this.GetBestRewardLocation(new IntVector2(1, 1));
          LootEngine.SpawnItem((GameObject) BraveResources.Load(path), bestRewardLocation.ToVector3(), Vector2.up, 1f, doDefaultItemPoof: true);
        }
        GameManager.Instance.Dungeon.InformRoomCleared(false, false);
      }
      else
      {
        if (flag2)
        {
          float num6 = currentRewardData.PercentOfRoomClearRewardsThatAreChests;
          if (PassiveItem.IsFlagSetAtAll(typeof (AmazingChestAheadItem)))
            num6 = Mathf.Max(0.5f, num6 * 2f);
          flag2 = (double) UnityEngine.Random.value > (double) num6;
        }
        IntVector2 bestRewardLocation1;
        if (flag2)
        {
          GameObject gameObject = (double) UnityEngine.Random.value >= 1.0 / (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER ? (double) GameManager.Instance.RewardManager.SinglePlayerPickupIncrementModifier : (double) GameManager.Instance.RewardManager.CoopPickupIncrementModifier) ? ((double) UnityEngine.Random.value >= 0.89999997615814209 ? GameManager.Instance.RewardManager.FullHeartPrefab.gameObject : GameManager.Instance.RewardManager.HalfHeartPrefab.gameObject) : currentRewardData.SingleItemRewardTable.SelectByWeight();
          UnityEngine.Debug.Log((object) (gameObject.name + "SPAWNED"));
          bestRewardLocation1 = this.GetBestRewardLocation(new IntVector2(1, 1));
          DebrisObject debrisObject = LootEngine.SpawnItem(gameObject, bestRewardLocation1.ToVector3() + new Vector3(0.25f, 0.0f, 0.0f), Vector2.up, 1f, doDefaultItemPoof: true);
          Exploder.DoRadialPush(debrisObject.sprite.WorldCenter.ToVector3ZUp(debrisObject.sprite.WorldCenter.y), 8f, 3f);
          int num7 = (int) AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", debrisObject.gameObject);
          GameManager.Instance.Dungeon.InformRoomCleared(true, false);
        }
        else
        {
          IntVector2 bestRewardLocation2 = this.GetBestRewardLocation(new IntVector2(2, 1));
          if (GameStatsManager.Instance.IsRainbowRun)
          {
            LootEngine.SpawnBowlerNote(GameManager.Instance.RewardManager.BowlerNoteChest, bestRewardLocation2.ToCenterVector2(), this, true);
            RoomHandler.HasGivenRoomChestRewardThisRun = true;
          }
          else if ((bool) (UnityEngine.Object) this.SpawnRoomRewardChest((WeightedGameObjectCollection) null, bestRewardLocation2))
            RoomHandler.HasGivenRoomChestRewardThisRun = true;
          GameManager.Instance.Dungeon.InformRoomCleared(true, true);
        }
        if (!flag1)
          return;
        bestRewardLocation1 = this.GetBestRewardLocation(new IntVector2(1, 1));
        LootEngine.DelayedSpawnItem(1f, (GameObject) BraveResources.Load(path), bestRewardLocation1.ToVector3() + new Vector3(0.25f, 0.0f, 0.0f), Vector2.up, 1f, doDefaultItemPoof: true);
      }
    }
  }

  protected virtual void NotifyPlayerRoomCleared()
  {
    if ((UnityEngine.Object) GameManager.Instance == (UnityEngine.Object) null || (UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null)
      return;
    GameManager.Instance.PrimaryPlayer.OnRoomCleared();
    if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER || !(bool) (UnityEngine.Object) GameManager.Instance.SecondaryPlayer)
      return;
    GameManager.Instance.SecondaryPlayer.OnRoomCleared();
  }

  public void AssignRoomVisualType(int type, bool respectPrototypeRooms = false)
  {
    if (respectPrototypeRooms && this.area != null && (UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null && this.area.prototypeRoom.overrideRoomVisualType > -1 && !this.area.prototypeRoom.overrideRoomVisualTypeForSecretRooms)
      return;
    this.RoomVisualSubtype = type;
  }

  public void CalculateOpulence()
  {
    if ((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null && (this.area.prototypeRoom.category == PrototypeDungeonRoom.RoomCategory.BOSS || this.area.prototypeRoom.category == PrototypeDungeonRoom.RoomCategory.REWARD || this.area.prototypeRoom.category == PrototypeDungeonRoom.RoomCategory.SPECIAL))
      ++this.opulence;
    if (this.distanceFromEntrance <= 12)
      return;
    ++this.opulence;
  }

  public RoomEventTriggerArea GetEventTriggerAreaFromObject(IEventTriggerable triggerable)
  {
    for (int index = 0; index < this.eventTriggerAreas.Count; ++index)
    {
      RoomEventTriggerArea eventTriggerArea = this.eventTriggerAreas[index];
      if (eventTriggerArea.events.Contains(triggerable))
        return eventTriggerArea;
    }
    return (RoomEventTriggerArea) null;
  }

  public void RegisterConnectedRoom(RoomHandler other, RuntimeRoomExitData usedExit)
  {
    this.area.instanceUsedExits.Add(usedExit.referencedExit);
    this.area.exitToLocalDataMap.Add(usedExit.referencedExit, usedExit);
    this.connectedRooms.Add(other);
    this.connectedRoomsByExit.Add(usedExit.referencedExit, other);
  }

  public void DeregisterConnectedRoom(RoomHandler other, RuntimeRoomExitData usedExit)
  {
    this.area.instanceUsedExits.Remove(usedExit.referencedExit);
    this.area.exitToLocalDataMap.Remove(usedExit.referencedExit);
    this.connectedRooms.Remove(other);
    this.connectedRoomsByExit.Remove(usedExit.referencedExit);
  }

  public DungeonData.Direction GetDirectionToConnectedRoom(RoomHandler other)
  {
    return this.area.IsProceduralRoom ? (DungeonData.Direction) ((int) (other.GetExitConnectedToRoom(this).exitDirection + 4) % 8) : this.GetExitConnectedToRoom(other).exitDirection;
  }

  public void TransferInteractableOwnershipToDungeon(IPlayerInteractable ixable)
  {
    this.DeregisterInteractable(ixable);
    RoomHandler.unassignedInteractableObjects.Remove(ixable);
    RoomHandler.unassignedInteractableObjects.Add(ixable);
  }

  public void RegisterInteractable(IPlayerInteractable ixable)
  {
    if (this.interactableObjects.Contains(ixable))
      return;
    this.interactableObjects.Add(ixable);
  }

  public bool IsRegistered(IPlayerInteractable ixable) => this.interactableObjects.Contains(ixable);

  public void DeregisterInteractable(IPlayerInteractable ixable)
  {
    if (this.interactableObjects.Contains(ixable))
    {
      this.interactableObjects.Remove(ixable);
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].RemoveBrokenInteractable(ixable);
    }
    else
      UnityEngine.Debug.LogError((object) "Deregistering an unregistered interactable. Talk to Brent.");
  }

  public void RegisterAutoAimTarget(IAutoAimTarget target)
  {
    if (this.autoAimTargets.Contains(target))
      return;
    this.autoAimTargets.Add(target);
  }

  public List<IAutoAimTarget> GetAutoAimTargets() => this.autoAimTargets;

  public void DeregisterAutoAimTarget(IAutoAimTarget target)
  {
    if (!this.autoAimTargets.Contains(target))
      return;
    this.autoAimTargets.Remove(target);
  }

  public List<T> GetComponentsInRoom<T>() where T : Behaviour
  {
    T[] objectsOfType = UnityEngine.Object.FindObjectsOfType<T>();
    List<T> componentsInRoom = new List<T>();
    for (int index = 0; index < objectsOfType.Length; ++index)
    {
      if (GameManager.Instance.Dungeon.GetRoomFromPosition(objectsOfType[index].transform.position.IntXY(VectorConversions.Floor)) == this)
        componentsInRoom.Add(objectsOfType[index]);
    }
    return componentsInRoom;
  }

  public List<T> GetComponentsAbsoluteInRoom<T>() where T : Behaviour
  {
    T[] objectsOfType = UnityEngine.Object.FindObjectsOfType<T>();
    List<T> componentsAbsoluteInRoom = new List<T>();
    for (int index = 0; index < objectsOfType.Length; ++index)
    {
      if (GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(objectsOfType[index].transform.position.IntXY(VectorConversions.Floor)) == this)
        componentsAbsoluteInRoom.Add(objectsOfType[index]);
    }
    return componentsAbsoluteInRoom;
  }

  public void MakeRoomMoreDifficult(float difficultyMultiplier, List<GameObject> sourceObjects = null)
  {
    if (this.activeEnemies == null || this.activeEnemies.Count == 0)
      return;
    this.m_activeDifficultyModifier *= difficultyMultiplier;
    if ((double) difficultyMultiplier <= 1.0)
      return;
    List<AIActor> input;
    if (sourceObjects != null)
    {
      input = new List<AIActor>();
      for (int index = 0; index < sourceObjects.Count; ++index)
      {
        AIActor component = sourceObjects[index].GetComponent<AIActor>();
        if ((bool) (UnityEngine.Object) component)
          input.Add(component);
      }
    }
    else
      input = new List<AIActor>((IEnumerable<AIActor>) this.activeEnemies);
    List<AIActor> aiActorList = input.Shuffle<AIActor>();
    int num = Mathf.FloorToInt((float) aiActorList.Count * difficultyMultiplier) - aiActorList.Count;
    for (int index1 = 0; index1 < num; ++index1)
    {
      AIActor enemyToDuplicate = aiActorList[index1 % aiActorList.Count];
      IntVector2? targetCenter = new IntVector2?();
      if ((bool) (UnityEngine.Object) enemyToDuplicate.TargetRigidbody)
        targetCenter = new IntVector2?(enemyToDuplicate.TargetRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
      CellValidator cellValidator = (CellValidator) (c =>
      {
        for (int index2 = 0; index2 < enemyToDuplicate.Clearance.x; ++index2)
        {
          for (int index3 = 0; index3 < enemyToDuplicate.Clearance.y; ++index3)
          {
            if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index2, c.y + index3) || targetCenter.HasValue && (double) IntVector2.DistanceSquared(targetCenter.Value, c.x + index2, c.y + index3) < 16.0)
              return false;
          }
        }
        return true;
      });
      IntVector2? randomAvailableCell = this.GetRandomAvailableCell(new IntVector2?(enemyToDuplicate.Clearance), new CellTypes?(enemyToDuplicate.PathableTiles), cellValidator: cellValidator);
      if (randomAvailableCell.HasValue)
      {
        AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(enemyToDuplicate.EnemyGuid), randomAvailableCell.Value, this, true, autoEngage: false);
        if (GameManager.Instance.BestActivePlayer.CurrentRoom == this)
          aiActor.HandleReinforcementFallIntoRoom();
      }
    }
  }

  public virtual void WriteRoomData(DungeonData data)
  {
    if ((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null)
    {
      this.MakePredefinedRoom();
      this.StampAdditionalAppearanceData();
    }
    else if (this.area.proceduralCells != null)
    {
      this.MakeCustomProceduralRoom();
    }
    else
    {
      BraveUtility.Log("STAMPING RECTANGULAR ROOM", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
      this.area.prototypeRoom = RobotDave.RuntimeProcessIdea(!GameManager.Instance.Dungeon.UsesCustomFloorIdea ? GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultProceduralIdea : GameManager.Instance.Dungeon.FloorIdea, this.area.dimensions);
      this.MakePredefinedRoom();
      this.area.prototypeRoom = (PrototypeDungeonRoom) null;
      this.area.IsProceduralRoom = true;
    }
    this.DefineRoomBorderCells();
    this.cameraBoundingPolygon = new RoomHandlerBoundingPolygon(this.GetPolygonDecomposition(), GameManager.Instance.MainCameraController.controllerCamera.VisibleBorder);
    this.cameraBoundingRect = this.GetBoundingRect();
    if (!((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null) || !((UnityEngine.Object) this.area.prototypeRoom.associatedMinimapIcon != (UnityEngine.Object) null))
      return;
    Minimap.Instance.RegisterRoomIcon(this, this.area.prototypeRoom.associatedMinimapIcon);
  }

  private void PreprocessVisualData()
  {
    if ((UnityEngine.Object) this.area.prototypeRoom == (UnityEngine.Object) null && this.area.proceduralCells != null)
      return;
    DungeonMaterial materialDefinition = GameManager.Instance.Dungeon.roomMaterialDefinitions[this.RoomVisualSubtype];
    if (!materialDefinition.usesInternalMaterialTransitions)
      return;
    int num1 = UnityEngine.Random.Range(0, 5);
    for (int index = 0; index < num1; ++index)
    {
      int num2 = this.area.basePosition.x + UnityEngine.Random.Range(0, this.area.dimensions.x - 3);
      int num3 = this.area.basePosition.y + UnityEngine.Random.Range(0, this.area.dimensions.y - 3);
      int num4 = UnityEngine.Random.Range(3, this.area.dimensions.x - (num2 - this.area.basePosition.x));
      int num5 = UnityEngine.Random.Range(3, this.area.dimensions.y - (num3 - this.area.basePosition.y));
      for (int x = num2; x < num2 + num4; ++x)
      {
        for (int y = num3; y < num3 + num5; ++y)
        {
          CellData cellData = GameManager.Instance.Dungeon.data[x, y];
          if (cellData.type != CellType.WALL && !cellData.IsTopWall())
            cellData.cellVisualData.roomVisualTypeIndex = materialDefinition.internalMaterialTransitions[0].materialTransition;
        }
      }
    }
  }

  public void PostGenerationCleanup()
  {
    if (this.area.IsProceduralRoom)
      return;
    if ((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null)
    {
      this.area.runtimePrototypeData = new RuntimePrototypeRoomData(this.area.prototypeRoom);
      if (!this.area.runtimePrototypeData.usesCustomAmbient)
      {
        this.area.runtimePrototypeData.usesCustomAmbient = true;
        this.area.runtimePrototypeData.usesDifferentCustomAmbientLowQuality = true;
        this.area.runtimePrototypeData.customAmbient = Color.Lerp(GameManager.Instance.Dungeon.decoSettings.ambientLightColor, GameManager.Instance.Dungeon.decoSettings.ambientLightColorTwo, UnityEngine.Random.value);
        this.area.runtimePrototypeData.customAmbientLowQuality = Color.Lerp(GameManager.Instance.Dungeon.decoSettings.lowQualityAmbientLightColor, GameManager.Instance.Dungeon.decoSettings.lowQualityAmbientLightColorTwo, UnityEngine.Random.value);
      }
      this.PreLoadReinforcements();
      this.area.prototypeRoom = (PrototypeDungeonRoom) null;
    }
    else
    {
      if (this.area.runtimePrototypeData != null)
        return;
      this.area.IsProceduralRoom = true;
    }
  }

  private void DefineRoomBorderCells()
  {
    HashSet<IntVector2> startingBorder = new HashSet<IntVector2>();
    DungeonData data = GameManager.Instance.Dungeon.data;
    IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
    foreach (IntVector2 cellsWithoutExit in this.roomCellsWithoutExits)
    {
      CellData cellData1 = data[cellsWithoutExit];
      cellData1.nearestRoom = this;
      cellData1.distanceFromNearestRoom = 0.0f;
      data[cellData1.position + IntVector2.Up].nearestRoom = this;
      data[cellData1.position + IntVector2.Up].distanceFromNearestRoom = 0.0f;
      data[cellData1.position + IntVector2.Up * 2].nearestRoom = this;
      data[cellData1.position + IntVector2.Up * 2].distanceFromNearestRoom = 0.0f;
      for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
      {
        if (data.CheckInBounds(cellData1.position + cardinalsAndOrdinals[index]))
        {
          CellData cellData2 = data[cellData1.position + cardinalsAndOrdinals[index]];
          if (cellData2 != null)
          {
            if (index == 0 || index == 1 || index == 7)
              cellData2 = data[cellData2.position + IntVector2.Up * 2];
            if (cellData2.type == CellType.WALL || cellData2.isExitCell)
            {
              cellData2.distanceFromNearestRoom = 1f;
              cellData2.nearestRoom = this;
              startingBorder.Add(cellData2.position);
            }
          }
        }
      }
    }
    this.DefineEpicenter(startingBorder);
  }

  private void DebugDrawCross(Vector3 centerPoint, Color crosscolor)
  {
    UnityEngine.Debug.DrawLine(centerPoint + new Vector3(-0.5f, 0.0f, 0.0f), centerPoint + new Vector3(0.5f, 0.0f, 0.0f), crosscolor, 1000f);
    UnityEngine.Debug.DrawLine(centerPoint + new Vector3(0.0f, -0.5f, 0.0f), centerPoint + new Vector3(0.0f, 0.5f, 0.0f), crosscolor, 1000f);
  }

  private float UpdateOcclusionData(PlayerController p, float visibility, bool useFloodFill = true)
  {
    return Pixelator.Instance.ProcessOcclusionChange(!((UnityEngine.Object) p != (UnityEngine.Object) null) ? GameManager.Instance.MainCameraController.Camera.transform.position.IntXY(VectorConversions.Floor) : p.transform.position.IntXY(VectorConversions.Floor), visibility, this, useFloodFill);
  }

  private float UpdateOcclusionData(float visibility, IntVector2 startPosition, bool useFloodFill = true)
  {
    return Pixelator.Instance.ProcessOcclusionChange(startPosition, visibility, this, useFloodFill);
  }

  public AIActor GetToughestEnemy()
  {
    AIActor toughestEnemy = (AIActor) null;
    float num1 = 0.0f;
    if (this.activeEnemies != null)
    {
      for (int index = 0; index < this.activeEnemies.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) this.activeEnemies[index] && this.activeEnemies[index].IsNormalEnemy && (bool) (UnityEngine.Object) this.activeEnemies[index].healthHaver && !this.activeEnemies[index].healthHaver.IsBoss)
        {
          float num2 = this.activeEnemies[index].healthHaver.GetMaxHealth() + (!this.activeEnemies[index].IsSignatureEnemy ? 0.0f : 1000f);
          if ((double) num2 > (double) num1)
          {
            toughestEnemy = this.activeEnemies[index];
            num1 = num2;
          }
        }
      }
    }
    return toughestEnemy;
  }

  public bool AddMysteriousBulletManToRoom()
  {
    if (GameStatsManager.Instance.AnyPastBeaten())
    {
      DungeonPlaceable dungeonPlaceable = BraveResources.Load("MysteriousBullet", ".asset") as DungeonPlaceable;
      if ((UnityEngine.Object) dungeonPlaceable == (UnityEngine.Object) null)
        return false;
      IntVector2? randomAvailableCell = this.GetRandomAvailableCell(new IntVector2?(new IntVector2(2, 2)), new CellTypes?(CellTypes.FLOOR), cellValidator: (CellValidator) (a => !GameManager.Instance.Dungeon.data[a].IsTopWall()));
      if (randomAvailableCell.HasValue)
      {
        dungeonPlaceable.InstantiateObject(this, randomAvailableCell.Value - this.area.basePosition);
        return true;
      }
    }
    return false;
  }

  public void AddSpecificEnemyToRoomProcedurally(
    string enemyGuid,
    bool reinforcementSpawn = false,
    Vector2? goalPosition = null)
  {
    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(enemyGuid);
    IntVector2 clearance = orLoadByGuid.specRigidbody.UnitDimensions.ToIntVector2(VectorConversions.Ceil);
    CellValidator cellValidator = (CellValidator) (c =>
    {
      for (int index1 = 0; index1 < clearance.x; ++index1)
      {
        int x = c.x + index1;
        for (int index2 = 0; index2 < clearance.y; ++index2)
        {
          int y = c.y + index2;
          if (GameManager.Instance.Dungeon.data.isTopWall(x, y))
            return false;
        }
      }
      return true;
    });
    IntVector2? nullable = !goalPosition.HasValue ? this.GetRandomAvailableCell(new IntVector2?(clearance), new CellTypes?(CellTypes.FLOOR), cellValidator: cellValidator) : this.GetNearestAvailableCell(goalPosition.Value, new IntVector2?(clearance), new CellTypes?(CellTypes.FLOOR), cellValidator: cellValidator);
    if (nullable.HasValue)
    {
      AIActor aiActor = AIActor.Spawn(orLoadByGuid, nullable.Value, this, true, AIActor.AwakenAnimationType.Spawn, false);
      if (!(bool) (UnityEngine.Object) aiActor || !reinforcementSpawn)
        return;
      if ((bool) (UnityEngine.Object) aiActor.specRigidbody)
        aiActor.specRigidbody.CollideWithOthers = false;
      aiActor.HandleReinforcementFallIntoRoom();
    }
    else
      UnityEngine.Debug.LogError((object) "failed placement");
  }

  private void MakeCustomProceduralRoom()
  {
    for (int index = 0; index < this.area.proceduralCells.Count; ++index)
    {
      IntVector2 intVector2 = this.area.basePosition + this.area.proceduralCells[index];
      this.StampCellComplex(intVector2.x, intVector2.y, CellType.FLOOR, DiagonalWallType.NONE);
    }
    if (GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.OFFICEGEON)
      return;
    this.AssignRoomVisualType(3);
    DungeonData data = GameManager.Instance.BestGenerationDungeonPrefab.data;
    for (int index = 0; index < this.area.proceduralCells.Count; ++index)
    {
      IntVector2 intVector2 = this.area.basePosition + this.area.proceduralCells[index];
      this.HandleStampedCellVisualData(intVector2.x, intVector2.y, (PrototypeDungeonRoomCellData) null);
    }
  }

  private GameObject PreloadReinforcementObject(
    PrototypePlacedObjectData objectData,
    IntVector2 pos,
    bool suppressPlayerChecks = false)
  {
    if ((double) objectData.spawnChance < 1.0 && (double) UnityEngine.Random.value > (double) objectData.spawnChance)
      return (GameObject) null;
    if (objectData.instancePrerequisites != null && objectData.instancePrerequisites.Length > 0)
    {
      bool flag = true;
      for (int index = 0; index < objectData.instancePrerequisites.Length; ++index)
      {
        if (!objectData.instancePrerequisites[index].CheckConditionsFulfilled())
          flag = false;
      }
      if (!flag)
        return (GameObject) null;
    }
    GameObject instantiatedObject = (GameObject) null;
    IntVector2 location = pos;
    if ((UnityEngine.Object) objectData.placeableContents != (UnityEngine.Object) null)
      instantiatedObject = objectData.placeableContents.InstantiateObject(this, location, deferConfiguration: true);
    if ((UnityEngine.Object) objectData.nonenemyBehaviour != (UnityEngine.Object) null)
    {
      instantiatedObject = objectData.nonenemyBehaviour.InstantiateObject(this, location, true);
      instantiatedObject.GetComponent<DungeonPlaceableBehaviour>().PlacedPosition = location + this.area.basePosition;
    }
    if (!string.IsNullOrEmpty(objectData.enemyBehaviourGuid))
      instantiatedObject = EnemyDatabase.GetOrLoadByGuid(objectData.enemyBehaviourGuid).InstantiateObject(this, location, true);
    if ((UnityEngine.Object) instantiatedObject != (UnityEngine.Object) null)
    {
      AIActor component = instantiatedObject.GetComponent<AIActor>();
      if ((bool) (UnityEngine.Object) component)
      {
        component.IsInReinforcementLayer = true;
        if (suppressPlayerChecks)
          component.HasDonePlayerEnterCheck = true;
        if ((bool) (UnityEngine.Object) component.healthHaver && component.healthHaver.IsBoss)
          component.HasDonePlayerEnterCheck = true;
        component.PlacedPosition = location + this.area.basePosition;
      }
      this.HandleFields(objectData, instantiatedObject);
      instantiatedObject.transform.parent = this.hierarchyParent;
      instantiatedObject.SetActive(false);
    }
    return instantiatedObject;
  }

  public void HandleFields(PrototypePlacedObjectData objectData, GameObject instantiatedObject)
  {
    if (!((UnityEngine.Object) objectData.nonenemyBehaviour != (UnityEngine.Object) null) && string.IsNullOrEmpty(objectData.enemyBehaviourGuid))
      return;
    object[] components = (object[]) instantiatedObject.GetComponents<IHasDwarfConfigurables>();
    bool flag = false;
    for (int index1 = 0; index1 < components.Length; ++index1)
    {
      if (!flag)
      {
        object obj = components[index1];
        System.Type type = obj.GetType();
        for (int index2 = 0; index2 < objectData.fieldData.Count; ++index2)
        {
          FieldInfo field = type.GetField(objectData.fieldData[index2].fieldName);
          if (field != null)
          {
            flag = true;
            if (objectData.fieldData[index2].fieldType == PrototypePlacedObjectFieldData.FieldType.FLOAT)
            {
              if (field.FieldType == typeof (int))
              {
                int floatValue = (int) objectData.fieldData[index2].floatValue;
                field.SetValue(obj, (object) floatValue);
              }
              else
                field.SetValue(obj, (object) objectData.fieldData[index2].floatValue);
            }
            else
              field.SetValue(obj, (object) objectData.fieldData[index2].boolValue);
          }
        }
        if (obj is ConveyorBelt)
          (obj as ConveyorBelt).PostFieldConfiguration(this);
      }
    }
  }

  private void ForceConfigure(GameObject instantiated)
  {
    foreach (Component componentsInChild in instantiated.GetComponentsInChildren(typeof (IPlaceConfigurable)))
    {
      if (componentsInChild is IPlaceConfigurable placeConfigurable)
        placeConfigurable.ConfigureOnPlacement(this);
    }
  }

  private List<GameObject> PlaceObjectsFromLayer(
    List<PrototypePlacedObjectData> placedObjectList,
    PrototypeRoomObjectLayer sourceLayer,
    List<Vector2> placedObjectPositions,
    Dictionary<int, RoomEventTriggerArea> eventTriggerMap,
    bool spawnPoofs = false,
    bool shuffleSpawns = false,
    int randomizeSpawns = 0,
    bool suppressPlayerChecks = false,
    bool disableDrops = false,
    int specifyObjectIndex = -1,
    int specifyObjectCount = -1)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    List<Vector2> vector2List;
    if (shuffleSpawns)
    {
      vector2List = new List<Vector2>((IEnumerable<Vector2>) placedObjectPositions);
      for (int index1 = vector2List.Count - 1; index1 > 0; --index1)
      {
        int index2 = UnityEngine.Random.Range(0, index1 + 1);
        if (index1 != index2)
        {
          Vector2 vector2 = vector2List[index1];
          vector2List[index1] = vector2List[index2];
          vector2List[index2] = vector2;
        }
      }
    }
    else
      vector2List = placedObjectPositions;
    List<GameObject> gameObjectList = new List<GameObject>();
    Dictionary<PrototypePlacedObjectData, GameObject> dictionary = (Dictionary<PrototypePlacedObjectData, GameObject>) null;
    if (sourceLayer != null && this.preloadedReinforcementLayerData != null && this.preloadedReinforcementLayerData.ContainsKey(sourceLayer))
      dictionary = this.preloadedReinforcementLayerData[sourceLayer];
    int num = 0;
    for (int index3 = 0; index3 < placedObjectList.Count; ++index3)
    {
      if (specifyObjectIndex < 0 || index3 >= specifyObjectIndex)
      {
        if (specifyObjectCount >= 0)
        {
          if (num < specifyObjectCount)
            ++num;
          else
            break;
        }
        PrototypePlacedObjectData placedObject = placedObjectList[index3];
        GameObject gameObject1 = (GameObject) null;
        if (dictionary != null && dictionary.ContainsKey(placedObject))
        {
          gameObject1 = dictionary[placedObject];
          if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
            continue;
        }
        if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
        {
          if ((double) placedObject.spawnChance >= 1.0 || (double) UnityEngine.Random.value <= (double) placedObject.spawnChance)
          {
            if (placedObject.instancePrerequisites != null && placedObject.instancePrerequisites.Length > 0)
            {
              bool flag = true;
              for (int index4 = 0; index4 < placedObject.instancePrerequisites.Length; ++index4)
              {
                if (!placedObject.instancePrerequisites[index4].CheckConditionsFulfilled())
                  flag = false;
              }
              if (!flag)
                continue;
            }
          }
          else
            continue;
        }
        GameObject gameObject2 = (GameObject) null;
        IntVector2 instantiatedDimensions = IntVector2.Zero;
        if (index3 >= vector2List.Count)
          UnityEngine.Debug.LogError((object) "i > modifiedPlacedObjectPositions.Count, this is very bad!");
        IntVector2 location = vector2List[index3].ToIntVector2();
        bool flag1 = true;
        if ((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null)
        {
          AIActor component = gameObject1.GetComponent<AIActor>();
          location = !(bool) (UnityEngine.Object) component ? gameObject1.transform.position.IntXY() - this.area.basePosition : component.PlacedPosition - this.area.basePosition;
          gameObject2 = gameObject1;
          gameObject2.SetActive(true);
          if ((UnityEngine.Object) placedObject.placeableContents != (UnityEngine.Object) null)
          {
            DungeonPlaceable placeableContents = placedObject.placeableContents;
            instantiatedDimensions = new IntVector2(placeableContents.width, placeableContents.height);
            flag1 = placeableContents.isPassable;
          }
          if ((UnityEngine.Object) placedObject.nonenemyBehaviour != (UnityEngine.Object) null)
          {
            DungeonPlaceableBehaviour nonenemyBehaviour = placedObject.nonenemyBehaviour;
            instantiatedDimensions = new IntVector2(nonenemyBehaviour.placeableWidth, nonenemyBehaviour.placeableHeight);
            flag1 = nonenemyBehaviour.isPassable;
          }
          if (!string.IsNullOrEmpty(placedObject.enemyBehaviourGuid))
          {
            DungeonPlaceableBehaviour orLoadByGuid = (DungeonPlaceableBehaviour) EnemyDatabase.GetOrLoadByGuid(placedObject.enemyBehaviourGuid);
            instantiatedDimensions = new IntVector2(orLoadByGuid.placeableWidth, orLoadByGuid.placeableHeight);
            flag1 = orLoadByGuid.isPassable;
          }
          this.ForceConfigure(gameObject2);
        }
        else
        {
          if ((UnityEngine.Object) placedObject.placeableContents != (UnityEngine.Object) null)
          {
            DungeonPlaceable placeableContents = placedObject.placeableContents;
            instantiatedDimensions = new IntVector2(placeableContents.width, placeableContents.height);
            flag1 = placeableContents.isPassable;
            gameObject2 = placedObject.placeableContents.InstantiateObject(this, location);
          }
          if ((UnityEngine.Object) placedObject.nonenemyBehaviour != (UnityEngine.Object) null)
          {
            DungeonPlaceableBehaviour nonenemyBehaviour = placedObject.nonenemyBehaviour;
            instantiatedDimensions = new IntVector2(nonenemyBehaviour.placeableWidth, nonenemyBehaviour.placeableHeight);
            flag1 = nonenemyBehaviour.isPassable;
            gameObject2 = placedObject.nonenemyBehaviour.InstantiateObject(this, location);
            gameObject2.GetComponent<DungeonPlaceableBehaviour>().PlacedPosition = location + this.area.basePosition;
          }
          if (!string.IsNullOrEmpty(placedObject.enemyBehaviourGuid))
          {
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(placedObject.enemyBehaviourGuid);
            if ((UnityEngine.Object) orLoadByGuid == (UnityEngine.Object) null)
              UnityEngine.Debug.LogError((object) $"{placedObject.enemyBehaviourGuid}|{this.area.prototypeRoom.name}");
            instantiatedDimensions = new IntVector2(orLoadByGuid.placeableWidth, orLoadByGuid.placeableHeight);
            flag1 = orLoadByGuid.isPassable;
            gameObject2 = orLoadByGuid.InstantiateObject(this, location);
          }
        }
        if ((UnityEngine.Object) gameObject2 != (UnityEngine.Object) null)
        {
          AIActor component1 = gameObject2.GetComponent<AIActor>();
          if ((bool) (UnityEngine.Object) component1)
          {
            if (suppressPlayerChecks)
              component1.HasDonePlayerEnterCheck = true;
            if ((bool) (UnityEngine.Object) component1.healthHaver && component1.healthHaver.IsBoss)
              component1.HasDonePlayerEnterCheck = true;
            component1.PlacedPosition = location + this.area.basePosition;
            if ((bool) (UnityEngine.Object) component1.specRigidbody)
            {
              component1.specRigidbody.Initialize();
              instantiatedDimensions = component1.Clearance;
            }
          }
          gameObjectList.Add(gameObject2);
          AIActor component2 = gameObject2.GetComponent<AIActor>();
          if (disableDrops && (bool) (UnityEngine.Object) component2)
          {
            component2.CanDropCurrency = false;
            component2.CanDropItems = false;
          }
          if (randomizeSpawns > 0)
          {
            float sqrMinDist = 8f;
            Vector2 playerPosition = GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter;
            IntVector2 truePlaceablePosition = location + this.area.basePosition;
            CellValidator cellValidator = (CellValidator) (c =>
            {
              if (GameManager.Instance.Dungeon.data[c + IntVector2.Down] != null && GameManager.Instance.Dungeon.data[c + IntVector2.Down].isExitCell || c.x < truePlaceablePosition.x - randomizeSpawns || c.x > truePlaceablePosition.x + randomizeSpawns || c.y < truePlaceablePosition.y - randomizeSpawns || c.y > truePlaceablePosition.y + randomizeSpawns || (double) (playerPosition - Pathfinder.GetClearanceOffset(c, instantiatedDimensions)).sqrMagnitude <= (double) sqrMinDist)
                return false;
              for (int index5 = 0; index5 < instantiatedDimensions.x; ++index5)
              {
                for (int index6 = 0; index6 < instantiatedDimensions.y; ++index6)
                {
                  if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(new IntVector2(c.x + index5, c.y + index6)) || GameManager.Instance.Dungeon.data.isTopWall(c.x + index5, c.y + index6) || !GameManager.Instance.Dungeon.data[c.x + index5, c.y + index6].isGridConnected)
                    return false;
                }
              }
              return true;
            });
            CellTypes cellTypes = CellTypes.FLOOR;
            if ((bool) (UnityEngine.Object) component2)
              cellTypes = component2.PathableTiles;
            IntVector2? randomAvailableCell = this.GetRandomAvailableCell(new IntVector2?(instantiatedDimensions), new CellTypes?(cellTypes), cellValidator: cellValidator);
            if (randomAvailableCell.HasValue)
            {
              gameObject2.transform.position += (randomAvailableCell.Value - truePlaceablePosition).ToVector3();
              if ((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null)
              {
                SpeculativeRigidbody component3 = gameObject2.GetComponent<SpeculativeRigidbody>();
                if ((bool) (UnityEngine.Object) component3)
                  component3.Reinitialize();
              }
            }
          }
          if (spawnPoofs)
          {
            AIActor component4 = gameObject2.GetComponent<AIActor>();
            if ((UnityEngine.Object) component4 != (UnityEngine.Object) null)
            {
              float delay = 0.0f;
              if (sourceLayer != null && specifyObjectIndex == -1 && specifyObjectIndex == -1)
                delay = 0.25f * (float) index3;
              component4.HandleReinforcementFallIntoRoom(delay);
            }
          }
          if (placedObject.xMPxOffset != 0 || placedObject.yMPxOffset != 0)
          {
            Vector2 vector = new Vector2((float) placedObject.xMPxOffset * (1f / 16f), (float) placedObject.yMPxOffset * (1f / 16f));
            gameObject2.transform.position += vector.ToVector3ZUp();
            SpeculativeRigidbody componentInChildren = gameObject2.GetComponentInChildren<SpeculativeRigidbody>();
            if ((bool) (UnityEngine.Object) componentInChildren)
              componentInChildren.Reinitialize();
          }
          for (int index7 = 0; index7 < instantiatedDimensions.x; ++index7)
          {
            for (int index8 = 0; index8 < instantiatedDimensions.y; ++index8)
            {
              IntVector2 vec = new IntVector2(this.area.basePosition.x + location.x + index7, this.area.basePosition.y + location.y + index8);
              if (data.CheckInBoundsAndValid(vec))
                data.cellData[vec.x][vec.y].isOccupied = !flag1;
            }
          }
          foreach (IPlayerInteractable interfacesInChild in gameObject2.GetInterfacesInChildren<IPlayerInteractable>())
            this.interactableObjects.Add(interfacesInChild);
          SurfaceDecorator component5 = gameObject2.GetComponent<SurfaceDecorator>();
          if ((UnityEngine.Object) component5 != (UnityEngine.Object) null)
            component5.Decorate(this);
          if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
          {
            this.HandleFields(placedObject, gameObject2);
            gameObject2.transform.parent = this.hierarchyParent;
          }
          if (placedObject.linkedTriggerAreaIDs != null && placedObject.linkedTriggerAreaIDs.Count > 0)
          {
            for (int index9 = 0; index9 < placedObject.linkedTriggerAreaIDs.Count; ++index9)
            {
              int linkedTriggerAreaId = placedObject.linkedTriggerAreaIDs[index9];
              if (eventTriggerMap != null && eventTriggerMap.ContainsKey(linkedTriggerAreaId))
                eventTriggerMap[linkedTriggerAreaId].AddGameObject(gameObject2);
            }
          }
          if (placedObject.assignedPathIDx != -1)
          {
            PathMover component6 = gameObject2.GetComponent<PathMover>();
            if ((UnityEngine.Object) component6 != (UnityEngine.Object) null && this.area.prototypeRoom.paths.Count > placedObject.assignedPathIDx && placedObject.assignedPathIDx >= 0)
            {
              component6.Path = this.area.prototypeRoom.paths[placedObject.assignedPathIDx];
              component6.PathStartNode = placedObject.assignedPathStartNode;
              component6.RoomHandler = this;
            }
          }
        }
      }
    }
    if (sourceLayer != null && this.preloadedReinforcementLayerData != null && this.preloadedReinforcementLayerData.ContainsKey(sourceLayer))
      this.preloadedReinforcementLayerData.Remove(sourceLayer);
    return gameObjectList;
  }

  public void AddDarkSoulsRoomResetDependency(RoomHandler room)
  {
    if (this.DarkSoulsRoomResetDependencies == null)
      this.DarkSoulsRoomResetDependencies = new List<RoomHandler>();
    if (this.DarkSoulsRoomResetDependencies.Contains(room))
      return;
    this.DarkSoulsRoomResetDependencies.Add(room);
  }

  public bool CanBeEscaped() => true;

  public void ResetPredefinedRoomLikeDarkSouls()
  {
    if (GameManager.Instance.PrimaryPlayer.CurrentRoom == this || this.visibility == RoomHandler.VisibilityStatus.OBSCURED || this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && this.m_hasGivenReward)
    {
      if (this.DarkSoulsRoomResetDependencies == null)
        return;
      for (int index = 0; index < this.DarkSoulsRoomResetDependencies.Count; ++index)
      {
        this.DarkSoulsRoomResetDependencies[index].m_hasGivenReward = false;
        this.DarkSoulsRoomResetDependencies[index].ResetPredefinedRoomLikeDarkSouls();
      }
    }
    else
    {
      if (this.OnDarkSoulsReset != null)
        this.OnDarkSoulsReset();
      if (this.activeEnemies != null)
      {
        for (int index = this.activeEnemies.Count - 1; index >= 0; --index)
        {
          AIActor activeEnemy = this.activeEnemies[index];
          if ((bool) (UnityEngine.Object) activeEnemy)
          {
            if ((bool) (UnityEngine.Object) activeEnemy.behaviorSpeculator)
              activeEnemy.behaviorSpeculator.InterruptAndDisable();
            if (activeEnemy.healthHaver.IsBoss && activeEnemy.healthHaver.IsAlive)
              activeEnemy.healthHaver.EndBossState(false);
            UnityEngine.Object.Destroy((UnityEngine.Object) activeEnemy.gameObject);
          }
        }
        this.activeEnemies.Clear();
      }
      if (GameManager.Instance.InTutorial)
      {
        List<TalkDoerLite> componentsInRoom = this.GetComponentsInRoom<TalkDoerLite>();
        for (int index1 = 0; index1 < componentsInRoom.Count; ++index1)
        {
          this.DeregisterInteractable((IPlayerInteractable) componentsInRoom[index1]);
          IEventTriggerable eventTriggerable = componentsInRoom[index1].gameObject.GetInterface<IEventTriggerable>();
          for (int index2 = 0; index2 < this.eventTriggerAreas.Count; ++index2)
            this.eventTriggerAreas[index2].events.Remove(eventTriggerable);
          UnityEngine.Object.Destroy((UnityEngine.Object) componentsInRoom[index1].gameObject);
        }
        this.npcSealState = RoomHandler.NPCSealState.SealNone;
      }
      else
      {
        List<TalkDoerLite> componentsInRoom = this.GetComponentsInRoom<TalkDoerLite>();
        for (int index = 0; index < componentsInRoom.Count; ++index)
          componentsInRoom[index].SendPlaymakerEvent("resetRoomLikeDarkSouls");
      }
      if (this.bossTriggerZones != null)
      {
        for (int index = 0; index < this.bossTriggerZones.Count; ++index)
          this.bossTriggerZones[index].HasTriggered = false;
      }
      if (this.remainingReinforcementLayers != null)
        this.remainingReinforcementLayers.Clear();
      this.UnsealRoom();
      this.visibility = RoomHandler.VisibilityStatus.REOBSCURED;
      for (int index = 0; index < this.connectedDoors.Count; ++index)
        this.connectedDoors[index].Close();
      this.PreventStandardRoomReward = true;
      if (this.area.IsProceduralRoom)
      {
        if (this.area.proceduralCells != null)
          ;
      }
      else
      {
        for (int index3 = -1; index3 < this.area.runtimePrototypeData.additionalObjectLayers.Count; ++index3)
        {
          if (index3 != -1 && this.area.runtimePrototypeData.additionalObjectLayers[index3].layerIsReinforcementLayer)
          {
            PrototypeRoomObjectLayer additionalObjectLayer = this.area.runtimePrototypeData.additionalObjectLayers[index3];
            if (additionalObjectLayer.numberTimesEncounteredRequired > 0)
            {
              if ((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null)
              {
                if (GameStatsManager.Instance.QueryRoomEncountered(this.area.prototypeRoom.GUID) < additionalObjectLayer.numberTimesEncounteredRequired)
                  continue;
              }
              else if (this.area.runtimePrototypeData != null && GameStatsManager.Instance.QueryRoomEncountered(this.area.runtimePrototypeData.GUID) < additionalObjectLayer.numberTimesEncounteredRequired)
                continue;
            }
            if ((double) additionalObjectLayer.probability >= 1.0 || (double) UnityEngine.Random.value <= (double) additionalObjectLayer.probability)
            {
              if (this.remainingReinforcementLayers == null)
                this.remainingReinforcementLayers = new List<PrototypeRoomObjectLayer>();
              if (this.area.runtimePrototypeData.additionalObjectLayers[index3].placedObjects.Count > 0)
                this.remainingReinforcementLayers.Add(this.area.runtimePrototypeData.additionalObjectLayers[index3]);
            }
          }
          else
          {
            List<PrototypePlacedObjectData> placedObjectDataList = index3 != -1 ? this.area.runtimePrototypeData.additionalObjectLayers[index3].placedObjects : this.area.runtimePrototypeData.placedObjects;
            List<Vector2> vector2List = index3 != -1 ? this.area.runtimePrototypeData.additionalObjectLayers[index3].placedObjectBasePositions : this.area.runtimePrototypeData.placedObjectPositions;
            for (int index4 = 0; index4 < placedObjectDataList.Count; ++index4)
            {
              PrototypePlacedObjectData objectData = placedObjectDataList[index4];
              if ((double) objectData.spawnChance >= 1.0 || (double) UnityEngine.Random.value <= (double) objectData.spawnChance)
              {
                GameObject gameObject = (GameObject) null;
                IntVector2 intVector2 = vector2List[index4].ToIntVector2();
                if ((UnityEngine.Object) objectData.placeableContents != (UnityEngine.Object) null)
                  gameObject = objectData.placeableContents.InstantiateObject(this, intVector2, true);
                if ((UnityEngine.Object) objectData.nonenemyBehaviour != (UnityEngine.Object) null)
                {
                  DungeonPlaceableBehaviour nonenemyBehaviour = objectData.nonenemyBehaviour;
                  gameObject = GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.TUTORIAL || !((UnityEngine.Object) nonenemyBehaviour.GetComponent<TalkDoerLite>() != (UnityEngine.Object) null) ? nonenemyBehaviour.InstantiateObjectOnlyActors(this, intVector2) : nonenemyBehaviour.InstantiateObject(this, intVector2);
                }
                if (!string.IsNullOrEmpty(objectData.enemyBehaviourGuid))
                  gameObject = EnemyDatabase.GetOrLoadByGuid(objectData.enemyBehaviourGuid).InstantiateObjectOnlyActors(this, intVector2);
                if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
                {
                  AIActor component = gameObject.GetComponent<AIActor>();
                  if ((bool) (UnityEngine.Object) component)
                  {
                    if ((bool) (UnityEngine.Object) component.healthHaver && component.healthHaver.IsBoss)
                      component.HasDonePlayerEnterCheck = true;
                    if (component.EnemyGuid == GlobalEnemyGuids.GripMaster)
                    {
                      UnityEngine.Object.Destroy((UnityEngine.Object) component.gameObject);
                      continue;
                    }
                  }
                  if (objectData.xMPxOffset != 0 || objectData.yMPxOffset != 0)
                  {
                    Vector2 vector = new Vector2((float) objectData.xMPxOffset * (1f / 16f), (float) objectData.yMPxOffset * (1f / 16f));
                    gameObject.transform.position += vector.ToVector3ZUp();
                  }
                  foreach (IPlayerInteractable interfacesInChild in gameObject.GetInterfacesInChildren<IPlayerInteractable>())
                    this.interactableObjects.Add(interfacesInChild);
                  this.HandleFields(objectData, gameObject);
                  gameObject.transform.parent = this.hierarchyParent;
                }
                if (objectData.linkedTriggerAreaIDs != null && objectData.linkedTriggerAreaIDs.Count > 0 && (UnityEngine.Object) gameObject != (UnityEngine.Object) null)
                {
                  for (int index5 = 0; index5 < objectData.linkedTriggerAreaIDs.Count; ++index5)
                  {
                    int linkedTriggerAreaId = objectData.linkedTriggerAreaIDs[index5];
                    if (this.eventTriggerMap != null && this.eventTriggerMap.ContainsKey(linkedTriggerAreaId))
                      this.eventTriggerMap[linkedTriggerAreaId].AddGameObject(gameObject);
                  }
                }
                if (objectData.assignedPathIDx != -1 && (bool) (UnityEngine.Object) gameObject)
                {
                  PathMover component = gameObject.GetComponent<PathMover>();
                  if ((UnityEngine.Object) component != (UnityEngine.Object) null)
                  {
                    component.Path = this.area.runtimePrototypeData.paths[objectData.assignedPathIDx];
                    component.PathStartNode = objectData.assignedPathStartNode;
                    component.RoomHandler = this;
                  }
                }
              }
            }
          }
        }
      }
      double num = (double) Pixelator.Instance.ProcessOcclusionChange(IntVector2.Zero, 0.0f, this, false);
      if (this.DarkSoulsRoomResetDependencies == null)
        return;
      for (int index = 0; index < this.DarkSoulsRoomResetDependencies.Count; ++index)
      {
        this.DarkSoulsRoomResetDependencies[index].m_hasGivenReward = false;
        this.DarkSoulsRoomResetDependencies[index].ResetPredefinedRoomLikeDarkSouls();
      }
    }
  }

  private void HandleCellDungeonMaterialOverride(int ix, int iy, int overrideMaterialIndex)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    int num = 0;
    if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)
      num = 1;
    for (int index1 = -1 * num; index1 < num + 1; ++index1)
    {
      for (int index2 = 0; index2 < 4; ++index2)
      {
        CellData cellData = data.cellData[ix + index1][iy + index2];
        if (cellData != null && (index1 == 0 && index2 == 0 || cellData.type == CellType.WALL))
          cellData.cellVisualData.roomVisualTypeIndex = overrideMaterialIndex;
        else
          break;
      }
    }
  }

  private IntVector2 GetFirstCellOfSpecificQuality(
    int xStart,
    int yStart,
    int xDim,
    int yDim,
    Func<CellData, bool> validator)
  {
    for (int y = yStart; y < yStart + yDim; ++y)
    {
      for (int x = xStart; x < xStart + xDim; ++x)
      {
        if (validator(GameManager.Instance.Dungeon.data[x, y]))
          return new IntVector2(x, y);
      }
    }
    return IntVector2.NegOne;
  }

  public void EnsureUpstreamLocksUnlocked()
  {
    if (this.IsOnCriticalPath)
      return;
    for (int index = 0; index < this.connectedRooms.Count; ++index)
    {
      if (this.connectedRooms[index].distanceFromEntrance < this.distanceFromEntrance)
      {
        RuntimeExitDefinition forConnectedRoom = this.GetExitDefinitionForConnectedRoom(this.connectedRooms[index]);
        if (forConnectedRoom != null && (UnityEngine.Object) forConnectedRoom.linkedDoor != (UnityEngine.Object) null && forConnectedRoom.linkedDoor.isLocked)
          forConnectedRoom.linkedDoor.Unlock();
        this.connectedRooms[index].EnsureUpstreamLocksUnlocked();
      }
    }
  }

  private void HandleProceduralLocking()
  {
    if (this.IsOnCriticalPath || !this.ShouldAttemptProceduralLock || (double) UnityEngine.Random.value >= (double) this.AttemptProceduralLockChance)
      return;
    if (this.ProceduralLockingType == RoomHandler.ProceduralLockType.BASE_SHOP)
      BaseShopController.HasLockedShopProcedurally = true;
    for (int index = 0; index < this.connectedDoors.Count; ++index)
    {
      RoomHandler roomHandler = this.connectedDoors[index].upstreamRoom != this ? this.connectedDoors[index].upstreamRoom : this.connectedDoors[index].downstreamRoom;
      if (roomHandler != null && roomHandler.distanceFromEntrance < this.distanceFromEntrance)
      {
        this.connectedDoors[index].isLocked = true;
        this.connectedDoors[index].ForceBecomeLockedDoor();
      }
    }
  }

  public void PostProcessFeatures()
  {
    if (!((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null) || this.area.prototypeRoom.rectangularFeatures == null)
      return;
    for (int index = 0; index < this.area.prototypeRoom.rectangularFeatures.Count; ++index)
    {
      PrototypeRectangularFeature rectangularFeature = this.area.prototypeRoom.rectangularFeatures[index];
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.SEWERGEON)
        this.PostProcessSewersFeature(rectangularFeature);
    }
  }

  public void ProcessFeatures()
  {
    this.HandleProceduralLocking();
    if ((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null && this.area.prototypeRoom.rectangularFeatures != null)
    {
      for (int index = 0; index < this.area.prototypeRoom.rectangularFeatures.Count; ++index)
      {
        PrototypeRectangularFeature rectangularFeature = this.area.prototypeRoom.rectangularFeatures[index];
        switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
        {
          case GlobalDungeonData.ValidTilesets.SEWERGEON:
            this.ProcessSewersFeature(rectangularFeature);
            break;
          case GlobalDungeonData.ValidTilesets.WESTGEON:
            this.ProcessWestgeonFeature(rectangularFeature);
            break;
        }
      }
    }
    if (!this.area.IsProceduralRoom)
      return;
    for (int x = -1; x < this.area.dimensions.x + 2; ++x)
    {
      for (int y = -1; y < this.area.dimensions.y + 2; ++y)
      {
        IntVector2 key = this.area.basePosition + new IntVector2(x, y);
        CellData cellData1 = GameManager.Instance.Dungeon.data[key];
        if (cellData1 != null && cellData1.isExitCell)
        {
          for (int index = 0; index < 4; ++index)
          {
            IntVector2 cardinal = IntVector2.Cardinals[index];
            for (CellData cellData2 = GameManager.Instance.Dungeon.data[key + cardinal]; cellData2 != null && !cellData2.isExitCell && cellData2.parentRoom == this && cellData2.type != CellType.WALL; cellData2 = GameManager.Instance.Dungeon.data[cellData2.position + cardinal])
              cellData2.type = CellType.FLOOR;
          }
        }
      }
    }
  }

  private void PostProcessSewersFeature(PrototypeRectangularFeature feature)
  {
    for (int x = feature.basePosition.x; x < feature.basePosition.x + feature.dimensions.x; ++x)
    {
      for (int y = feature.basePosition.y; y < feature.basePosition.y + feature.dimensions.y; ++y)
        GameManager.Instance.Dungeon.data[this.area.basePosition + new IntVector2(x, y)].type = CellType.FLOOR;
    }
  }

  private void ProcessSewersFeature(PrototypeRectangularFeature feature)
  {
    for (int x = feature.basePosition.x; x < feature.basePosition.x + feature.dimensions.x; ++x)
    {
      for (int y = feature.basePosition.y; y < feature.basePosition.y + feature.dimensions.y; ++y)
      {
        IntVector2 key = this.area.basePosition + new IntVector2(x, y);
        CellData cellData = GameManager.Instance.Dungeon.data[key];
        cellData.fallingPrevented = true;
        int num = 91;
        if ((UnityEngine.Object) this.RoomMaterial.bridgeGrid != (UnityEngine.Object) null)
        {
          bool[] eightSides = new bool[8];
          for (int dir = 0; dir < eightSides.Length; ++dir)
          {
            IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection((DungeonData.Direction) dir);
            IntVector2 intVector2 = key + vector2FromDirection - this.area.basePosition;
            if (intVector2.x >= feature.basePosition.x && intVector2.x < feature.basePosition.x + feature.dimensions.x && intVector2.y >= feature.basePosition.y && intVector2.y < feature.basePosition.y + feature.dimensions.y)
              eightSides[dir] = true;
          }
          num = this.RoomMaterial.bridgeGrid.GetIndexGivenEightSides(eightSides);
        }
        cellData.cellVisualData.UsesCustomIndexOverride01 = true;
        cellData.cellVisualData.CustomIndexOverride01 = num;
        cellData.cellVisualData.CustomIndexOverride01Layer = GlobalDungeonData.patternLayerIndex;
      }
    }
  }

  private void ProcessWestgeonFeature(PrototypeRectangularFeature feature)
  {
    if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON)
      return;
    int index = UnityEngine.Random.Range(1, 3);
    DungeonMaterial materialDefinition = GameManager.Instance.Dungeon.roomMaterialDefinitions[index];
    for (int x = feature.basePosition.x; x < feature.basePosition.x + feature.dimensions.x; ++x)
    {
      for (int y = feature.basePosition.y; y < feature.basePosition.y + feature.dimensions.y; ++y)
      {
        IntVector2 key = this.area.basePosition + new IntVector2(x, y);
        if (GameManager.Instance.Dungeon.data[key].nearestRoom == this)
        {
          GameManager.Instance.Dungeon.data[key].cellVisualData.IsFeatureCell = true;
          this.featureCells.Add(key);
          GameManager.Instance.Dungeon.data[key].cellVisualData.roomVisualTypeIndex = index;
        }
      }
    }
    IntVector2 ofSpecificQuality = this.GetFirstCellOfSpecificQuality(this.area.basePosition.x + feature.basePosition.x, this.area.basePosition.y + feature.basePosition.y, feature.dimensions.x, feature.dimensions.y, (Func<CellData, bool>) (a => a.IsUpperFacewall()));
    if (!(ofSpecificQuality != IntVector2.NegOne))
      return;
    int num1 = 0;
    IntVector2 key1 = ofSpecificQuality;
    while (GameManager.Instance.Dungeon.data[key1].IsUpperFacewall() && key1.x < this.area.basePosition.x + feature.basePosition.x + feature.dimensions.x)
    {
      ++num1;
      key1 += IntVector2.Right;
    }
    if (num1 <= 3)
      return;
    int max = UnityEngine.Random.Range(0, num1 - 3);
    int num2 = num1 - max;
    IntVector2 intVector2 = ofSpecificQuality.WithX(ofSpecificQuality.x + UnityEngine.Random.Range(0, max));
    for (int x = intVector2.x; x < intVector2.x + num2; ++x)
    {
      for (int y = intVector2.y + 1; y <= intVector2.y + 2; ++y)
      {
        GameManager.Instance.Dungeon.data[x, y].cellVisualData.UsesCustomIndexOverride01 = true;
        GameManager.Instance.Dungeon.data[x, y].cellVisualData.CustomIndexOverride01Layer = GlobalDungeonData.aboveBorderLayerIndex;
        GameManager.Instance.Dungeon.data[x, y].cellVisualData.CustomIndexOverride01 = materialDefinition.facadeTopGrid.GetIndexGivenSides(y == intVector2.y + 2, y == intVector2.y + 2 && x == intVector2.x + num2 - 1, x == intVector2.x + num2 - 1, y == intVector2.y + 1 && x == intVector2.x + num2 - 1, y == intVector2.y + 1, y == intVector2.y + 1 && x == intVector2.x, x == intVector2.x, y == intVector2.y + 2 && x == intVector2.x);
      }
    }
  }

  private void StampAdditionalAppearanceData()
  {
    float num1 = UnityEngine.Random.Range(0.0f, 0.05f);
    float num2 = UnityEngine.Random.Range(0.0f, 0.05f);
    for (int x = this.area.basePosition.x; x < this.area.basePosition.x + this.area.dimensions.x; ++x)
    {
      for (int y = this.area.basePosition.y; y < this.area.basePosition.y + this.area.dimensions.y; ++y)
      {
        int ix = x - this.area.basePosition.x;
        int iy = y - this.area.basePosition.y;
        PrototypeDungeonRoomCellData cellDataAtPoint = this.area.prototypeRoom.ForceGetCellDataAtPoint(ix, iy);
        if (!cellDataAtPoint.doesDamage)
        {
          DungeonMaterial materialDefinition = GameManager.Instance.Dungeon.roomMaterialDefinitions[this.RoomVisualSubtype];
          if (cellDataAtPoint.appearance.overrideDungeonMaterialIndex != -1)
            this.HandleCellDungeonMaterialOverride(x, y, cellDataAtPoint.appearance.overrideDungeonMaterialIndex);
          else if (materialDefinition.usesInternalMaterialTransitions && materialDefinition.usesProceduralMaterialTransitions && (double) Mathf.PerlinNoise(num1 + (float) ix / 10f, num2 + (float) iy / 10f) > (double) materialDefinition.internalMaterialTransitions[0].proceduralThreshold)
            this.HandleCellDungeonMaterialOverride(x, y, materialDefinition.internalMaterialTransitions[0].materialTransition);
        }
      }
    }
  }

  private bool NonenemyPlaceableBehaviorIsEnemylike(DungeonPlaceableBehaviour dpb)
  {
    return dpb is ForgeHammerController;
  }

  private void CleanupPrototypeRoomLayers()
  {
    this.area.prototypeRoom.runtimeAdditionalObjectLayers = new List<PrototypeRoomObjectLayer>();
    for (int index1 = 0; index1 < this.area.prototypeRoom.additionalObjectLayers.Count; ++index1)
    {
      PrototypeRoomObjectLayer additionalObjectLayer = this.area.prototypeRoom.additionalObjectLayers[index1];
      if (!additionalObjectLayer.layerIsReinforcementLayer)
      {
        this.area.prototypeRoom.runtimeAdditionalObjectLayers.Add(additionalObjectLayer);
      }
      else
      {
        Action<PrototypeRoomObjectLayer, PrototypeRoomObjectLayer> action = (Action<PrototypeRoomObjectLayer, PrototypeRoomObjectLayer>) ((source, target) =>
        {
          target.shuffle = source.shuffle;
          target.randomize = source.randomize;
          target.suppressPlayerChecks = source.suppressPlayerChecks;
          target.delayTime = source.delayTime;
          target.reinforcementTriggerCondition = source.reinforcementTriggerCondition;
          target.probability = source.probability;
          target.numberTimesEncounteredRequired = source.numberTimesEncounteredRequired;
        });
        bool flag1 = false;
        bool flag2 = false;
        PrototypeRoomObjectLayer prototypeRoomObjectLayer1 = (PrototypeRoomObjectLayer) null;
        PrototypeRoomObjectLayer prototypeRoomObjectLayer2 = (PrototypeRoomObjectLayer) null;
        for (int index2 = 0; index2 < additionalObjectLayer.placedObjects.Count; ++index2)
        {
          if ((UnityEngine.Object) additionalObjectLayer.placedObjects[index2].placeableContents != (UnityEngine.Object) null)
          {
            if (additionalObjectLayer.placedObjects[index2].placeableContents.ContainsEnemy || additionalObjectLayer.placedObjects[index2].placeableContents.ContainsEnemylikeObjectForReinforcement)
            {
              flag1 = true;
              if (prototypeRoomObjectLayer1 == null)
                prototypeRoomObjectLayer1 = new PrototypeRoomObjectLayer();
              prototypeRoomObjectLayer1.placedObjects.Add(additionalObjectLayer.placedObjects[index2]);
              prototypeRoomObjectLayer1.placedObjectBasePositions.Add(additionalObjectLayer.placedObjectBasePositions[index2]);
            }
            else
            {
              flag2 = true;
              if (prototypeRoomObjectLayer2 == null)
                prototypeRoomObjectLayer2 = new PrototypeRoomObjectLayer();
              prototypeRoomObjectLayer2.placedObjects.Add(additionalObjectLayer.placedObjects[index2]);
              prototypeRoomObjectLayer2.placedObjectBasePositions.Add(additionalObjectLayer.placedObjectBasePositions[index2]);
            }
          }
          else if ((UnityEngine.Object) additionalObjectLayer.placedObjects[index2].nonenemyBehaviour != (UnityEngine.Object) null)
          {
            if (this.NonenemyPlaceableBehaviorIsEnemylike(additionalObjectLayer.placedObjects[index2].nonenemyBehaviour))
            {
              flag1 = true;
              if (prototypeRoomObjectLayer1 == null)
                prototypeRoomObjectLayer1 = new PrototypeRoomObjectLayer();
              prototypeRoomObjectLayer1.placedObjects.Add(additionalObjectLayer.placedObjects[index2]);
              prototypeRoomObjectLayer1.placedObjectBasePositions.Add(additionalObjectLayer.placedObjectBasePositions[index2]);
            }
            else
            {
              flag2 = true;
              if (prototypeRoomObjectLayer2 == null)
                prototypeRoomObjectLayer2 = new PrototypeRoomObjectLayer();
              prototypeRoomObjectLayer2.placedObjects.Add(additionalObjectLayer.placedObjects[index2]);
              prototypeRoomObjectLayer2.placedObjectBasePositions.Add(additionalObjectLayer.placedObjectBasePositions[index2]);
            }
          }
          else if (!string.IsNullOrEmpty(additionalObjectLayer.placedObjects[index2].enemyBehaviourGuid))
          {
            flag1 = true;
            if (prototypeRoomObjectLayer1 == null)
              prototypeRoomObjectLayer1 = new PrototypeRoomObjectLayer();
            prototypeRoomObjectLayer1.placedObjects.Add(additionalObjectLayer.placedObjects[index2]);
            prototypeRoomObjectLayer1.placedObjectBasePositions.Add(additionalObjectLayer.placedObjectBasePositions[index2]);
          }
        }
        if (flag1 && flag2)
        {
          action(additionalObjectLayer, prototypeRoomObjectLayer1);
          action(additionalObjectLayer, prototypeRoomObjectLayer2);
          prototypeRoomObjectLayer1.layerIsReinforcementLayer = additionalObjectLayer.layerIsReinforcementLayer;
          prototypeRoomObjectLayer2.layerIsReinforcementLayer = false;
          this.area.prototypeRoom.runtimeAdditionalObjectLayers.Add(prototypeRoomObjectLayer1);
          this.area.prototypeRoom.runtimeAdditionalObjectLayers.Add(prototypeRoomObjectLayer2);
        }
        else if (flag2)
        {
          action(additionalObjectLayer, prototypeRoomObjectLayer2);
          prototypeRoomObjectLayer2.layerIsReinforcementLayer = false;
          this.area.prototypeRoom.runtimeAdditionalObjectLayers.Add(prototypeRoomObjectLayer2);
        }
        else
          this.area.prototypeRoom.runtimeAdditionalObjectLayers.Add(additionalObjectLayer);
      }
    }
  }

  public void RegisterExternalReinforcementLayer(PrototypeDungeonRoom source, int layerIndex)
  {
    if (this.remainingReinforcementLayers == null)
      this.remainingReinforcementLayers = new List<PrototypeRoomObjectLayer>();
    if (source.runtimeAdditionalObjectLayers[layerIndex].placedObjects.Count <= 0)
      return;
    this.remainingReinforcementLayers.Add(this.area.prototypeRoom.runtimeAdditionalObjectLayers[layerIndex]);
  }

  private void MakePredefinedRoom()
  {
    this.CleanupPrototypeRoomLayers();
    DungeonData data = GameManager.Instance.Dungeon.data;
    GameObject gameObject1 = GameObject.Find("_Rooms");
    if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
      gameObject1 = new GameObject("_Rooms");
    Transform transform = new GameObject("Room_" + this.area.prototypeRoom.name).transform;
    transform.parent = gameObject1.transform;
    this.m_roomMotionHandler = transform.gameObject.AddComponent<RoomMotionHandler>();
    this.m_roomMotionHandler.Initialize(this);
    this.hierarchyParent = transform;
    List<IntVector2> intVector2List = new List<IntVector2>();
    if (this.area.prototypeRoom.ContainsEnemies)
      this.EverHadEnemies = true;
    for (int x = this.area.basePosition.x; x < this.area.basePosition.x + this.area.dimensions.x; ++x)
    {
      for (int y = this.area.basePosition.y; y < this.area.basePosition.y + this.area.dimensions.y; ++y)
      {
        int ix = x - this.area.basePosition.x;
        int iy = y - this.area.basePosition.y;
        PrototypeDungeonRoomCellData cellDataAtPoint = this.area.prototypeRoom.ForceGetCellDataAtPoint(ix, iy);
        if (this.area.prototypeRoom.HasNonWallNeighborWithDiagonals(ix, iy) || cellDataAtPoint.breakable)
        {
          bool flag = true;
          if (cellDataAtPoint.conditionalOnParentExit && !this.area.instanceUsedExits.Contains(this.area.prototypeRoom.exitData.exits[cellDataAtPoint.parentExitIndex]))
          {
            if (cellDataAtPoint.conditionalCellIsPit && this.StampCellComplex(x, y, CellType.PIT, DiagonalWallType.NONE))
              this.HandleStampedCellVisualData(x, y, cellDataAtPoint);
          }
          else
          {
            if (cellDataAtPoint.state != CellType.WALL)
            {
              flag = this.StampCellComplex(x, y, cellDataAtPoint.state, cellDataAtPoint.diagonalWallType);
              if (flag)
                this.HandleStampedCellVisualData(x, y, cellDataAtPoint);
            }
            else if (cellDataAtPoint.state == CellType.WALL)
              flag = this.StampCellComplex(x, y, cellDataAtPoint.state, cellDataAtPoint.diagonalWallType, cellDataAtPoint.breakable);
            if (flag)
            {
              CellData cellData = data.cellData[x][y];
              if (cellDataAtPoint != null)
              {
                cellData.cellVisualData.IsPhantomCarpet = cellDataAtPoint.appearance.IsPhantomCarpet;
                cellData.forceDisallowGoop = cellDataAtPoint.appearance.ForceDisallowGoop;
                if ((cellDataAtPoint.appearance.OverrideFloorType != CellVisualData.CellFloorType.Ice || this.RoomMaterial.supportsIceSquares) && cellDataAtPoint.appearance.OverrideFloorType != CellVisualData.CellFloorType.Stone)
                {
                  cellData.cellVisualData.floorType = cellDataAtPoint.appearance.OverrideFloorType;
                  if (cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Water)
                    cellData.cellVisualData.absorbsDebris = true;
                }
              }
              List<int> overridesForTilemap = cellDataAtPoint.appearance.GetOverridesForTilemap(this.area.prototypeRoom, GameManager.Instance.Dungeon.tileIndices.tilesetId);
              if (overridesForTilemap != null && overridesForTilemap.Count != 0)
              {
                int index = Mathf.FloorToInt(cellData.UniqueHash * (float) overridesForTilemap.Count);
                if (index == overridesForTilemap.Count)
                  --index;
                cellData.cellVisualData.inheritedOverrideIndex = overridesForTilemap[index];
                cellData.cellVisualData.floorTileOverridden = true;
              }
              if (cellDataAtPoint.doesDamage && GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.JUNGLEGEON)
                intVector2List.Add(cellData.position);
              else if (cellDataAtPoint.doesDamage && GameManager.Instance.Dungeon.roomMaterialDefinitions[this.m_roomVisualType].supportsLavaOrLavalikeSquares)
              {
                cellData.doesDamage = true;
                cellData.damageDefinition = cellDataAtPoint.damageDefinition;
                cellData.cellVisualData.floorType = CellVisualData.CellFloorType.Water;
              }
              if (cellDataAtPoint.ForceTileNonDecorated)
              {
                cellData.cellVisualData.containsObjectSpaceStamp = true;
                cellData.cellVisualData.containsWallSpaceStamp = true;
                data.cellData[x][y + 1].cellVisualData.containsObjectSpaceStamp = true;
                data.cellData[x][y + 1].cellVisualData.containsWallSpaceStamp = true;
                data.cellData[x][y + 2].cellVisualData.containsObjectSpaceStamp = true;
                data.cellData[x][y + 2].cellVisualData.containsWallSpaceStamp = true;
              }
            }
          }
        }
      }
    }
    for (int index = 0; index < this.area.prototypeRoom.paths.Count; ++index)
      this.area.prototypeRoom.paths[index].StampPathToTilemap(this);
    this.eventTriggerAreas = new List<RoomEventTriggerArea>();
    this.eventTriggerMap = new Dictionary<int, RoomEventTriggerArea>();
    for (int index = 0; index < this.area.prototypeRoom.eventTriggerAreas.Count; ++index)
    {
      RoomEventTriggerArea eventTriggerArea = new RoomEventTriggerArea(this.area.prototypeRoom.eventTriggerAreas[index], this.area.basePosition);
      this.eventTriggerAreas.Add(eventTriggerArea);
      this.eventTriggerMap.Add(index, eventTriggerArea);
    }
    for (int index = -1; index < this.area.prototypeRoom.runtimeAdditionalObjectLayers.Count; ++index)
    {
      if (index != -1 && this.area.prototypeRoom.runtimeAdditionalObjectLayers[index].layerIsReinforcementLayer)
      {
        PrototypeRoomObjectLayer additionalObjectLayer = this.area.prototypeRoom.runtimeAdditionalObjectLayers[index];
        if ((additionalObjectLayer.numberTimesEncounteredRequired <= 0 || GameStatsManager.Instance.QueryRoomEncountered(this.area.prototypeRoom.GUID) >= additionalObjectLayer.numberTimesEncounteredRequired) && ((double) additionalObjectLayer.probability >= 1.0 || (double) UnityEngine.Random.value <= (double) additionalObjectLayer.probability))
        {
          if (this.remainingReinforcementLayers == null)
            this.remainingReinforcementLayers = new List<PrototypeRoomObjectLayer>();
          if (this.area.prototypeRoom.runtimeAdditionalObjectLayers[index].placedObjects.Count > 0)
            this.remainingReinforcementLayers.Add(this.area.prototypeRoom.runtimeAdditionalObjectLayers[index]);
        }
      }
      else
      {
        List<PrototypePlacedObjectData> placedObjectList = index != -1 ? this.area.prototypeRoom.runtimeAdditionalObjectLayers[index].placedObjects : this.area.prototypeRoom.placedObjects;
        List<Vector2> placedObjectPositions = index != -1 ? this.area.prototypeRoom.runtimeAdditionalObjectLayers[index].placedObjectBasePositions : this.area.prototypeRoom.placedObjectPositions;
        if (index != -1)
        {
          PrototypeRoomObjectLayer additionalObjectLayer = this.area.prototypeRoom.runtimeAdditionalObjectLayers[index];
          if (additionalObjectLayer.numberTimesEncounteredRequired > 0 && GameStatsManager.Instance.QueryRoomEncountered(this.area.prototypeRoom.GUID) < additionalObjectLayer.numberTimesEncounteredRequired || (double) additionalObjectLayer.probability < 1.0 && (double) UnityEngine.Random.value > (double) additionalObjectLayer.probability)
            continue;
        }
        this.PlaceObjectsFromLayer(placedObjectList, (PrototypeRoomObjectLayer) null, placedObjectPositions, this.eventTriggerMap);
      }
    }
    GameObject gameObject2 = GameObject.Find("_Doors");
    if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
      gameObject2 = new GameObject("_Doors");
    for (int index = 0; index < this.area.instanceUsedExits.Count; ++index)
    {
      PrototypeRoomExit instanceUsedExit = this.area.instanceUsedExits[index];
      RuntimeRoomExitData exitToLocalData = this.area.exitToLocalDataMap[instanceUsedExit];
      bool isSecretConnection = false;
      if ((UnityEngine.Object) this.connectedRoomsByExit[instanceUsedExit].area.prototypeRoom != (UnityEngine.Object) null)
        isSecretConnection = this.connectedRoomsByExit[instanceUsedExit].area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET;
      RoomHandler roomHandler = this.connectedRoomsByExit[instanceUsedExit];
      if (this.exitDefinitionsByExit != null && this.exitDefinitionsByExit.ContainsKey(exitToLocalData))
      {
        RuntimeExitDefinition runtimeExitDefinition = this.exitDefinitionsByExit[exitToLocalData];
        foreach (IntVector2 intVector2 in runtimeExitDefinition.GetCellsForRoom(this))
          this.StampCellAsExit(intVector2.x, intVector2.y, instanceUsedExit.exitDirection, roomHandler, isSecretConnection);
        runtimeExitDefinition.StampCellVisualTypes(data);
        runtimeExitDefinition.ProcessExitDecorables();
      }
      else
      {
        RuntimeExitDefinition runtimeExitDefinition = new RuntimeExitDefinition(exitToLocalData, exitToLocalData.linkedExit, this, roomHandler);
        if (this.exitDefinitionsByExit == null)
          this.exitDefinitionsByExit = new Dictionary<RuntimeRoomExitData, RuntimeExitDefinition>();
        if (roomHandler.exitDefinitionsByExit == null)
          roomHandler.exitDefinitionsByExit = new Dictionary<RuntimeRoomExitData, RuntimeExitDefinition>();
        this.exitDefinitionsByExit.Add(exitToLocalData, runtimeExitDefinition);
        if (exitToLocalData.linkedExit != null)
          roomHandler.exitDefinitionsByExit.Add(exitToLocalData.linkedExit, runtimeExitDefinition);
        foreach (IntVector2 intVector2 in runtimeExitDefinition.GetCellsForRoom(this))
          this.StampCellAsExit(intVector2.x, intVector2.y, instanceUsedExit.exitDirection, roomHandler, isSecretConnection);
        if (exitToLocalData.linkedExit == null)
        {
          foreach (IntVector2 key in runtimeExitDefinition.GetCellsForRoom(roomHandler))
          {
            roomHandler.StampCellAsExit(key.x, key.y, instanceUsedExit.exitDirection, this);
            data[key].parentRoom = roomHandler;
            data[key].occlusionData.sharedRoomAndExitCell = true;
          }
          runtimeExitDefinition.StampCellVisualTypes(data);
        }
        if (runtimeExitDefinition.IntermediaryCells != null)
        {
          foreach (IntVector2 intermediaryCell in runtimeExitDefinition.IntermediaryCells)
          {
            if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intermediaryCell))
            {
              if (!this.Cells.Contains(intermediaryCell))
                this.Cells.Add(intermediaryCell);
              GameManager.Instance.Dungeon.data[intermediaryCell].parentRoom = (RoomHandler) null;
              GameManager.Instance.Dungeon.data[intermediaryCell].isDoorFrameCell = true;
            }
          }
        }
        runtimeExitDefinition.GenerateDoorsForExit(data, gameObject2.transform);
      }
    }
    if (intVector2List.Count <= 0 || GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.JUNGLEGEON)
      return;
    TallGrassPatch tallGrassPatch = new GameObject("grass patch").AddComponent<TallGrassPatch>();
    tallGrassPatch.cells = intVector2List;
    tallGrassPatch.BuildPatch();
  }

  public void AddProceduralTeleporterToRoom()
  {
    if (Minimap.Instance.HasTeleporterIcon(this))
      return;
    GameObject objectToInstantiate = ResourceCache.Acquire("Global Prefabs/Teleporter_Gungeon_01") as GameObject;
    DungeonData dungeonData = GameManager.Instance.Dungeon.data;
    bool isStrict = true;
    Func<CellData, bool> canContainTeleporter = (Func<CellData, bool>) (a => a != null && !a.isOccupied && !a.doesDamage && !a.containsTrap && !a.IsTrapZone && !a.cellVisualData.hasStampedPath && (!isStrict || !a.HasPitNeighbor(dungeonData)) && a.type == CellType.FLOOR);
    this.ProcessTeleporterTiles(canContainTeleporter);
    Func<CellData, bool> isInvalidFunction = (Func<CellData, bool>) (a => a == null || !a.cachedCanContainTeleporter || a.parentRoom != this);
    Tuple<IntVector2, IntVector2> tuple = Carpetron.RawMaxSubmatrix(dungeonData.cellData, this.area.basePosition, this.area.dimensions, isInvalidFunction);
    if (tuple.Second.x < 3 || tuple.Second.y < 3)
    {
      isStrict = false;
      this.ProcessTeleporterTiles(canContainTeleporter);
      tuple = Carpetron.RawMaxSubmatrix(dungeonData.cellData, this.area.basePosition, this.area.dimensions, isInvalidFunction);
    }
    BraveUtility.DrawDebugSquare(tuple.First.ToVector2(), tuple.Second.ToVector2(), Color.red, 1000f);
    if (tuple.Second.x < 3 || tuple.Second.y < 3)
      return;
    IntVector2 first = tuple.First;
    IntVector2 intVector2 = tuple.Second - tuple.First;
    int x = intVector2.x % 2 == 1 || intVector2.x == 4 ? 0 : -1;
    int y = intVector2.y % 2 == 1 || intVector2.y == 4 ? 0 : -1;
    for (; intVector2.x > 3; intVector2.x -= 2)
      ++first.x;
    for (; intVector2.y > 3; intVector2.y -= 2)
      ++first.y;
    IntVector2 location = first + new IntVector2(x, y);
    this.RegisterInteractable((IPlayerInteractable) DungeonPlaceableUtility.InstantiateDungeonPlaceable(objectToInstantiate, this, location, false).GetComponent<TeleporterController>());
  }

  private void ProcessTeleporterTiles(Func<CellData, bool> canContainTeleporter)
  {
    IntVector2 basePosition = this.area.basePosition;
    IntVector2 intVector2 = this.area.basePosition + this.area.dimensions - IntVector2.One;
    DungeonData data = GameManager.Instance.Dungeon.data;
    for (int x = basePosition.x; x <= intVector2.x; ++x)
    {
      for (int y = basePosition.y; y <= intVector2.y; ++y)
      {
        if (data[x, y] != null)
          data[x, y].cachedCanContainTeleporter = false;
      }
    }
    for (int x = basePosition.x; x <= intVector2.x; ++x)
    {
      for (int y = basePosition.y; y <= intVector2.y; ++y)
      {
        bool flag = true;
        for (int index1 = 0; index1 < 4 && flag; ++index1)
        {
          for (int index2 = 0; index2 < 4 && flag; ++index2)
          {
            if (!data.CheckInBounds(x + index1, y + index2) || !canContainTeleporter(data[x + index1, y + index2]))
            {
              flag = false;
              break;
            }
          }
        }
        if (flag)
        {
          for (int index3 = 0; index3 < 4 && flag; ++index3)
          {
            for (int index4 = 0; index4 < 4 && flag; ++index4)
              data[x + index3, y + index4].cachedCanContainTeleporter = true;
          }
        }
      }
    }
  }

  protected IntVector2 GetDoorPositionForExit(RuntimeRoomExitData exit)
  {
    IntVector2 doorPositionForExit = exit.ExitOrigin - IntVector2.One + this.area.basePosition;
    if (exit.jointedExit)
      doorPositionForExit = exit.TotalExitLength <= exit.linkedExit.TotalExitLength ? exit.linkedExit.HalfExitAttachPoint - IntVector2.One + this.connectedRoomsByExit[exit.referencedExit].area.basePosition : exit.HalfExitAttachPoint - IntVector2.One + this.area.basePosition;
    return doorPositionForExit;
  }

  protected void AttachDoorControllerToAllConnectedExitCells(
    DungeonDoorController controller,
    IntVector2 exitCellPosition)
  {
    Queue<CellData> cellDataQueue = new Queue<CellData>();
    cellDataQueue.Enqueue(GameManager.Instance.Dungeon.data[exitCellPosition]);
    while (cellDataQueue.Count > 0)
    {
      CellData d = cellDataQueue.Dequeue();
      d.exitDoor = controller;
      List<CellData> cellNeighbors = GameManager.Instance.Dungeon.data.GetCellNeighbors(d);
      for (int index = 0; index < cellNeighbors.Count; ++index)
      {
        CellData cellData = cellNeighbors[index];
        if (!((UnityEngine.Object) cellData.exitDoor == (UnityEngine.Object) controller) && cellData.isExitCell)
          cellDataQueue.Enqueue(cellData);
      }
    }
  }

  public bool UnsealConditionsMet()
  {
    if (this.area.IsProceduralRoom || this.area.runtimePrototypeData.roomEvents == null || this.area.runtimePrototypeData.roomEvents.Count <= 0)
      return true;
    bool flag = true;
    for (int index = 0; index < this.area.runtimePrototypeData.roomEvents.Count; ++index)
    {
      RoomEventDefinition roomEvent = this.area.runtimePrototypeData.roomEvents[index];
      if (roomEvent.action == RoomEventTriggerAction.UNSEAL_ROOM && roomEvent.condition == RoomEventTriggerCondition.ON_ENEMIES_CLEARED && this.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
        flag = false;
    }
    return flag;
  }

  public bool CanTeleportFromRoom()
  {
    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL)
    {
      if (this.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
        return false;
    }
    else
    {
      for (int index = 0; index < this.connectedDoors.Count; ++index)
      {
        if (this.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) || this.connectedDoors[index].IsSealed && this.connectedDoors[index].Mode != DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR && this.connectedDoors[index].Mode != DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS && this.connectedDoors[index].Mode != DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS)
          return false;
      }
    }
    return true;
  }

  public bool CanTeleportToRoom()
  {
    if (!this.TeleportersActive)
      return false;
    for (int index = 0; index < this.connectedDoors.Count; ++index)
    {
      if (this.connectedDoors[index].IsSealed && this.connectedDoors[index].Mode != DungeonDoorController.DungeonDoorMode.FINAL_BOSS_DOOR && this.connectedDoors[index].Mode != DungeonDoorController.DungeonDoorMode.BOSS_DOOR_ONLY_UNSEALS && this.connectedDoors[index].Mode != DungeonDoorController.DungeonDoorMode.ONE_WAY_DOOR_ONLY_UNSEALS)
        return false;
    }
    return true;
  }

  public void SealRoom()
  {
    if (this.m_isSealed)
      return;
    this.m_isSealed = true;
    for (int index = 0; index < this.connectedDoors.Count; ++index)
    {
      if (!this.connectedDoors[index].OneWayDoor && this.npcSealState == RoomHandler.NPCSealState.SealNext)
      {
        if ((this.connectedDoors[index].upstreamRoom != this ? this.connectedDoors[index].upstreamRoom : this.connectedDoors[index].downstreamRoom).distanceFromEntrance >= this.distanceFromEntrance)
          this.connectedDoors[index].DoSeal(this);
      }
      else
        this.connectedDoors[index].DoSeal(this);
    }
    for (int index = 0; index < this.standaloneBlockers.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.standaloneBlockers[index])
        this.standaloneBlockers[index].Seal();
    }
    for (int index = 0; index < this.connectedRooms.Count; ++index)
    {
      if ((UnityEngine.Object) this.connectedRooms[index].secretRoomManager != (UnityEngine.Object) null)
        this.connectedRooms[index].secretRoomManager.DoSeal();
    }
    if (GameManager.Instance.AllPlayers.Length > 1)
    {
      PlayerController playerController = (PlayerController) null;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if (GameManager.Instance.AllPlayers[index].CurrentRoom == this)
        {
          playerController = GameManager.Instance.AllPlayers[index];
          break;
        }
      }
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if (!((UnityEngine.Object) GameManager.Instance.AllPlayers[index] == (UnityEngine.Object) playerController))
          ;
      }
    }
    if (this.OnSealChanged == null)
      return;
    this.OnSealChanged(true);
  }

  public void UnsealRoom()
  {
    if (this.npcSealState == RoomHandler.NPCSealState.SealAll || this.npcSealState == RoomHandler.NPCSealState.SealNone && !this.m_isSealed)
      return;
    this.m_isSealed = false;
    for (int index = 0; index < this.connectedDoors.Count; ++index)
    {
      if (this.connectedDoors[index].IsSealed || (UnityEngine.Object) this.connectedDoors[index].subsidiaryBlocker != (UnityEngine.Object) null && this.connectedDoors[index].subsidiaryBlocker.isSealed || (UnityEngine.Object) this.connectedDoors[index].subsidiaryDoor != (UnityEngine.Object) null && this.connectedDoors[index].subsidiaryDoor.IsSealed)
      {
        if (!this.connectedDoors[index].OneWayDoor)
        {
          if (this.npcSealState == RoomHandler.NPCSealState.SealNone)
            this.connectedDoors[index].DoUnseal(this);
          else if (this.npcSealState == RoomHandler.NPCSealState.SealPrior)
          {
            if ((this.connectedDoors[index].upstreamRoom != this ? this.connectedDoors[index].upstreamRoom : this.connectedDoors[index].downstreamRoom).distanceFromEntrance >= this.distanceFromEntrance)
              this.connectedDoors[index].DoUnseal(this);
          }
          else if (this.npcSealState == RoomHandler.NPCSealState.SealNext && (this.connectedDoors[index].upstreamRoom != this ? this.connectedDoors[index].upstreamRoom : this.connectedDoors[index].downstreamRoom).distanceFromEntrance < this.distanceFromEntrance)
            this.connectedDoors[index].DoUnseal(this);
        }
        else
        {
          if ((UnityEngine.Object) this.connectedDoors[index].subsidiaryDoor != (UnityEngine.Object) null)
            this.connectedDoors[index].subsidiaryDoor.DoUnseal(this);
          if ((UnityEngine.Object) this.connectedDoors[index].subsidiaryBlocker != (UnityEngine.Object) null)
            this.connectedDoors[index].subsidiaryBlocker.Unseal();
        }
      }
    }
    for (int index = 0; index < this.standaloneBlockers.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.standaloneBlockers[index])
        this.standaloneBlockers[index].Unseal();
    }
    for (int index = 0; index < this.connectedRooms.Count; ++index)
    {
      if ((UnityEngine.Object) this.connectedRooms[index].secretRoomManager != (UnityEngine.Object) null)
        this.connectedRooms[index].secretRoomManager.DoUnseal();
    }
    if (this.OnSealChanged == null)
      return;
    this.OnSealChanged(false);
  }

  public IPlayerInteractable GetNearestInteractable(
    Vector2 position,
    float maxDistance,
    PlayerController player)
  {
    IPlayerInteractable nearestInteractable = (IPlayerInteractable) null;
    float num1 = float.MaxValue;
    bool flag = GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && GameManager.Instance.GetOtherPlayer(player).IsTalking;
    for (int index = 0; index < this.interactableObjects.Count; ++index)
    {
      IPlayerInteractable interactableObject = this.interactableObjects[index];
      if ((bool) (UnityEngine.Object) (interactableObject as MonoBehaviour) && (!flag || !(interactableObject is TalkDoer) && !(interactableObject is TalkDoerLite)) && (player.IsPrimaryPlayer || !(interactableObject is TalkDoerLite) || !(interactableObject as TalkDoerLite).PreventCoopInteraction))
      {
        float distanceToPoint = interactableObject.GetDistanceToPoint(position);
        float num2 = interactableObject.GetOverrideMaxDistance();
        if ((double) num2 <= 0.0)
          num2 = maxDistance;
        if ((double) distanceToPoint < (double) num2 && (double) distanceToPoint < (double) num1)
        {
          nearestInteractable = interactableObject;
          num1 = distanceToPoint;
        }
      }
    }
    if (RoomHandler.unassignedInteractableObjects != null)
    {
      for (int index = 0; index < RoomHandler.unassignedInteractableObjects.Count; ++index)
      {
        IPlayerInteractable interactableObject = RoomHandler.unassignedInteractableObjects[index];
        if ((bool) (UnityEngine.Object) (interactableObject as MonoBehaviour))
        {
          if (flag)
          {
            switch (interactableObject)
            {
              case TalkDoer _:
              case TalkDoerLite _:
                continue;
            }
          }
          float distanceToPoint = interactableObject.GetDistanceToPoint(position);
          float num3 = interactableObject.GetOverrideMaxDistance();
          if ((double) num3 <= 0.0)
            num3 = maxDistance;
          if ((double) distanceToPoint < (double) num3 && (double) distanceToPoint < (double) num1)
          {
            nearestInteractable = interactableObject;
            num1 = distanceToPoint;
          }
        }
      }
    }
    return nearestInteractable;
  }

  public ReadOnlyCollection<IPlayerInteractable> GetRoomInteractables()
  {
    return this.interactableObjects.AsReadOnly();
  }

  public List<IPlayerInteractable> GetNearbyInteractables(Vector2 position, float maxDistance)
  {
    List<IPlayerInteractable> nearbyInteractables = new List<IPlayerInteractable>();
    for (int index = 0; index < this.interactableObjects.Count; ++index)
    {
      IPlayerInteractable interactableObject = this.interactableObjects[index];
      if ((double) interactableObject.GetDistanceToPoint(position) < (double) maxDistance)
        nearbyInteractables.Add(interactableObject);
    }
    return nearbyInteractables;
  }

  public void RegisterEnemy(AIActor enemy)
  {
    this.EverHadEnemies = true;
    if (this.activeEnemies == null)
      this.activeEnemies = new List<AIActor>();
    if (this.activeEnemies.Contains(enemy))
    {
      BraveUtility.Log("Registering an enemy to a RoomHandler twice!", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
    }
    else
    {
      this.activeEnemies.Add(enemy);
      this.m_totalSpawnedEnemyHP += enemy.healthHaver.GetMaxHealth();
      this.m_lastTotalSpawnedEnemyHP += enemy.healthHaver.GetMaxHealth();
      this.RegisterAutoAimTarget((IAutoAimTarget) enemy);
      if (this.OnEnemyRegistered == null)
        return;
      this.OnEnemyRegistered(enemy);
    }
  }

  public AIActor GetRandomActiveEnemy(bool allowHarmless = true)
  {
    if (this.activeEnemies == null || this.activeEnemies.Count <= 0)
      return (AIActor) null;
    if (allowHarmless)
      return this.activeEnemies[UnityEngine.Random.Range(0, this.activeEnemies.Count)];
    RoomHandler.s_tempActiveEnemies.Clear();
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.activeEnemies[index] && !this.activeEnemies[index].IsHarmlessEnemy)
        RoomHandler.s_tempActiveEnemies.Add(this.activeEnemies[index]);
    }
    if (RoomHandler.s_tempActiveEnemies.Count == 0)
      return (AIActor) null;
    AIActor tempActiveEnemy = RoomHandler.s_tempActiveEnemies[UnityEngine.Random.Range(0, RoomHandler.s_tempActiveEnemies.Count)];
    RoomHandler.s_tempActiveEnemies.Clear();
    return tempActiveEnemy;
  }

  public List<AIActor> GetActiveEnemies(RoomHandler.ActiveEnemyType type)
  {
    if (type != RoomHandler.ActiveEnemyType.RoomClear)
      return this.activeEnemies;
    return this.activeEnemies == null ? (List<AIActor>) null : new List<AIActor>(this.activeEnemies.Where<AIActor>((Func<AIActor, bool>) (a => !a.IgnoreForRoomClear)));
  }

  public void GetActiveEnemies(RoomHandler.ActiveEnemyType type, ref List<AIActor> outList)
  {
    outList.Clear();
    if (this.activeEnemies == null)
      return;
    if (type == RoomHandler.ActiveEnemyType.RoomClear)
    {
      for (int index = 0; index < this.activeEnemies.Count; ++index)
      {
        if (!this.activeEnemies[index].IgnoreForRoomClear)
          outList.Add(this.activeEnemies[index]);
      }
    }
    else
      outList.AddRange((IEnumerable<AIActor>) this.activeEnemies);
  }

  public int GetActiveEnemiesCount(RoomHandler.ActiveEnemyType type)
  {
    if (this.activeEnemies == null)
      return 0;
    return type == RoomHandler.ActiveEnemyType.RoomClear ? this.activeEnemies.Count<AIActor>((Func<AIActor, bool>) (a => !a.IgnoreForRoomClear)) : this.activeEnemies.Count;
  }

  public AIActor GetNearestEnemy(
    Vector2 position,
    out float nearestDistance,
    bool includeBosses = true,
    bool excludeDying = false)
  {
    AIActor nearestEnemy = (AIActor) null;
    nearestDistance = float.MaxValue;
    if (this.activeEnemies == null)
      return (AIActor) null;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if ((includeBosses || !this.activeEnemies[index].healthHaver.IsBoss) && (!excludeDying || !this.activeEnemies[index].healthHaver.IsDead))
      {
        float num = Vector2.Distance(position, this.activeEnemies[index].CenterPosition);
        if ((double) num < (double) nearestDistance)
        {
          nearestDistance = num;
          nearestEnemy = this.activeEnemies[index];
        }
      }
    }
    return nearestEnemy;
  }

  public AIActor GetNearestEnemyInDirection(
    Vector2 position,
    Vector2 direction,
    float angleTolerance,
    out float nearestDistance,
    bool includeBosses = true)
  {
    AIActor enemyInDirection = (AIActor) null;
    nearestDistance = float.MaxValue;
    float angle1 = direction.ToAngle();
    if (this.activeEnemies == null)
      return (AIActor) null;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if (includeBosses || !this.activeEnemies[index].healthHaver.IsBoss)
      {
        Vector2 vector = this.activeEnemies[index].CenterPosition - position;
        float angle2 = vector.ToAngle();
        if ((double) Mathf.Abs(Mathf.DeltaAngle(angle1, angle2)) < (double) angleTolerance)
        {
          float magnitude = vector.magnitude;
          if ((double) magnitude < (double) nearestDistance)
          {
            nearestDistance = magnitude;
            enemyInDirection = this.activeEnemies[index];
          }
        }
      }
    }
    return enemyInDirection;
  }

  public AIActor GetNearestEnemyInDirection(
    Vector2 position,
    Vector2 direction,
    float angleTolerance,
    out float nearestDistance,
    bool includeBosses,
    AIActor excludeActor)
  {
    AIActor enemyInDirection = (AIActor) null;
    nearestDistance = float.MaxValue;
    float angle1 = direction.ToAngle();
    if (this.activeEnemies == null)
      return (AIActor) null;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if ((includeBosses || !this.activeEnemies[index].healthHaver.IsBoss) && !((UnityEngine.Object) this.activeEnemies[index] == (UnityEngine.Object) excludeActor))
      {
        Vector2 vector = this.activeEnemies[index].CenterPosition - position;
        float angle2 = vector.ToAngle();
        if ((double) Mathf.Abs(Mathf.DeltaAngle(angle1, angle2)) < (double) angleTolerance)
        {
          float magnitude = vector.magnitude;
          if ((double) magnitude < (double) nearestDistance)
          {
            nearestDistance = magnitude;
            enemyInDirection = this.activeEnemies[index];
          }
        }
      }
    }
    return enemyInDirection;
  }

  public void ApplyActionToNearbyEnemies(
    Vector2 position,
    float radius,
    Action<AIActor, float> lambda)
  {
    float num = radius * radius;
    if (this.activeEnemies == null)
      return;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) this.activeEnemies[index])
      {
        bool flag = (double) radius < 0.0;
        Vector2 vector2 = this.activeEnemies[index].CenterPosition - position;
        if (!flag)
          flag = (double) vector2.sqrMagnitude < (double) num;
        if (flag)
          lambda(this.activeEnemies[index], vector2.magnitude);
      }
    }
  }

  public bool HasOtherBoss(AIActor boss)
  {
    if (this.activeEnemies == null)
      return false;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if (!((UnityEngine.Object) this.activeEnemies[index] == (UnityEngine.Object) boss) && this.activeEnemies[index].healthHaver.IsBoss)
        return true;
    }
    return false;
  }

  public bool HasActiveEnemies(RoomHandler.ActiveEnemyType type)
  {
    if (this.activeEnemies == null)
      return false;
    if (type != RoomHandler.ActiveEnemyType.RoomClear)
      return this.activeEnemies.Count > 0;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if (!this.activeEnemies[index].IgnoreForRoomClear)
        return true;
    }
    return false;
  }

  public bool TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition condition, bool instant = false)
  {
    bool flag = false;
    if (this.remainingReinforcementLayers == null)
      return false;
    for (int index = 0; index < this.remainingReinforcementLayers.Count; ++index)
    {
      if (this.remainingReinforcementLayers[index].reinforcementTriggerCondition == condition)
      {
        flag = this.TriggerReinforcementLayer(index, instant: instant);
        break;
      }
    }
    return flag;
  }

  public void ResetEnemyHPPercentage()
  {
    this.m_totalSpawnedEnemyHP = 0.0f;
    this.m_lastTotalSpawnedEnemyHP = 0.0f;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if (!this.activeEnemies[index].IgnoreForRoomClear)
        this.m_totalSpawnedEnemyHP += this.activeEnemies[index].healthHaver.GetCurrentHealth();
    }
    this.m_lastTotalSpawnedEnemyHP = this.m_totalSpawnedEnemyHP;
  }

  private void CheckEnemyHPPercentageEvents()
  {
    float num1 = 0.0f;
    for (int index = 0; index < this.activeEnemies.Count; ++index)
    {
      if (!this.activeEnemies[index].IgnoreForRoomClear)
        num1 += this.activeEnemies[index].healthHaver.GetCurrentHealth();
    }
    float num2 = this.m_lastTotalSpawnedEnemyHP / this.m_totalSpawnedEnemyHP;
    float num3 = num1 / this.m_totalSpawnedEnemyHP;
    if ((double) num2 > 0.75 && (double) num3 <= 0.75)
    {
      this.ProcessRoomEvents(RoomEventTriggerCondition.ON_ONE_QUARTER_ENEMY_HP_DEPLETED);
      this.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.ON_ONE_QUARTER_ENEMY_HP_DEPLETED);
    }
    if ((double) num2 > 0.5 && (double) num3 <= 0.5)
    {
      this.ProcessRoomEvents(RoomEventTriggerCondition.ON_HALF_ENEMY_HP_DEPLETED);
      this.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.ON_HALF_ENEMY_HP_DEPLETED);
    }
    if ((double) num2 > 0.25 && (double) num3 <= 0.25)
    {
      this.ProcessRoomEvents(RoomEventTriggerCondition.ON_THREE_QUARTERS_ENEMY_HP_DEPLETED);
      this.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.ON_THREE_QUARTERS_ENEMY_HP_DEPLETED);
    }
    if ((double) num2 > 0.10000000149011612 && (double) num3 <= 0.10000000149011612)
    {
      this.ProcessRoomEvents(RoomEventTriggerCondition.ON_NINETY_PERCENT_ENEMY_HP_DEPLETED);
      this.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.ON_NINETY_PERCENT_ENEMY_HP_DEPLETED);
    }
    this.m_lastTotalSpawnedEnemyHP = num1;
  }

  public void DeregisterEnemy(AIActor enemy, bool suppressClearChecks = false)
  {
    if (this.activeEnemies == null || !this.activeEnemies.Contains(enemy))
    {
      this.DeregisterAutoAimTarget((IAutoAimTarget) enemy);
    }
    else
    {
      this.activeEnemies.Remove(enemy);
      if (!enemy.IgnoreForRoomClear && !suppressClearChecks)
      {
        this.CheckEnemyHPPercentageEvents();
        if (!this.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
        {
          bool flag1 = false;
          if (this.remainingReinforcementLayers != null && this.remainingReinforcementLayers.Count > 0)
            flag1 = this.TriggerReinforcementLayersOnEvent(RoomEventTriggerCondition.ON_ENEMIES_CLEARED);
          bool flag2 = flag1 || this.numberOfTimedWavesOnDeck > 0 || this.SpawnSequentialReinforcementWaves();
          if (this.PreEnemiesCleared != null)
          {
            bool flag3 = this.PreEnemiesCleared();
            flag2 = flag2 || flag3;
          }
          if (!flag2 || !this.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
          {
            this.ProcessRoomEvents(RoomEventTriggerCondition.ON_ENEMIES_CLEARED);
            GameManager.Instance.DungeonMusicController.NotifyCurrentRoomEnemiesCleared();
            this.OnEnemiesCleared();
            GameManager.BroadcastRoomTalkDoerFsmEvent("roomCleared");
          }
        }
      }
      this.DeregisterAutoAimTarget((IAutoAimTarget) enemy);
    }
  }

  private bool SpawnSequentialReinforcementWaves()
  {
    int index1 = -1;
    if (this.remainingReinforcementLayers == null)
      return false;
    for (int index2 = 0; index2 < this.remainingReinforcementLayers.Count; ++index2)
    {
      if (this.remainingReinforcementLayers[index2].reinforcementTriggerCondition == RoomEventTriggerCondition.SHRINE_WAVE_A)
        return false;
    }
    for (int index3 = 0; index3 < this.remainingReinforcementLayers.Count; ++index3)
    {
      if (this.remainingReinforcementLayers[index3].reinforcementTriggerCondition == RoomEventTriggerCondition.SEQUENTIAL_WAVE_TRIGGER)
      {
        index1 = index3;
        break;
      }
    }
    return index1 >= 0 && this.TriggerReinforcementLayer(index1);
  }

  public void BuildSecretRoomCover()
  {
    this.m_secretRoomCoverObject = SecretRoomBuilder.BuildRoomCover(this, GameManager.Instance.Dungeon.data.tilemap, GameManager.Instance.Dungeon.data);
  }

  private void StampCell(CellData cell, bool isSecretConnection = false)
  {
    cell.type = CellType.FLOOR;
    cell.parentArea = this.area;
    cell.parentRoom = this;
    if ((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null && (this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET || isSecretConnection))
      cell.isSecretRoomCell = true;
    this.roomCells.Add(cell.position);
    this.roomCellsWithoutExits.Add(cell.position);
    this.rawRoomCells.Add(cell.position);
  }

  private void StampCell(int ix, int iy, bool isSecretConnection = false)
  {
    if (ix >= GameManager.Instance.Dungeon.data.Width || iy >= GameManager.Instance.Dungeon.data.Height)
      UnityEngine.Debug.LogError((object) $"Attempting to stamp {(object) ix},{(object) iy} in cellData of lengths {(object) GameManager.Instance.Dungeon.data.Width},{(object) GameManager.Instance.Dungeon.data.Height}");
    this.StampCell(GameManager.Instance.Dungeon.data.cellData[ix][iy], isSecretConnection);
  }

  private void StampCellAsExit(
    int ix,
    int iy,
    DungeonData.Direction exitDirection,
    RoomHandler connectedRoom,
    bool isSecretConnection = false)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    if (ix < 0 || ix >= data.Width || iy < 0 || iy >= data.Height)
    {
      UnityEngine.Debug.LogWarningFormat("Invalid StampCellAsExit({0}, {1}, {2}, {3}, {4}) call!", (object) ix, (object) iy, (object) exitDirection, connectedRoom != null ? (object) connectedRoom.ToString() : (object) "null", (object) isSecretConnection);
    }
    else
    {
      CellData cellData = data.cellData[ix][iy];
      this.StampCell(ix, iy, isSecretConnection);
      this.roomCellsWithoutExits.Remove(cellData.position);
      cellData.cellVisualData.roomVisualTypeIndex = this.m_roomVisualType;
      if (exitDirection == DungeonData.Direction.NORTH || exitDirection == DungeonData.Direction.SOUTH)
      {
        IntVector2 intVector2_1 = new IntVector2(ix - 1, iy + 2);
        if (data.CheckInBoundsAndValid(intVector2_1) && data[intVector2_1].type == CellType.WALL)
          data[intVector2_1].cellVisualData.roomVisualTypeIndex = this.m_roomVisualType;
        IntVector2 intVector2_2 = new IntVector2(ix + 1, iy + 2);
        if (data.CheckInBoundsAndValid(intVector2_2) && data[intVector2_2].type == CellType.WALL)
          data[intVector2_2].cellVisualData.roomVisualTypeIndex = this.m_roomVisualType;
      }
      else
      {
        for (int index = -1; index < 4; ++index)
        {
          IntVector2 intVector2 = new IntVector2(ix, iy + index);
          if (data.CheckInBoundsAndValid(intVector2) && data[intVector2].type == CellType.WALL)
            data[intVector2].cellVisualData.roomVisualTypeIndex = this.m_roomVisualType;
          if (this.area.PrototypeLostWoodsRoom && GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON && data.CheckInBoundsAndValid(intVector2))
          {
            CellData current = data[intVector2];
            if (data.isAnyFaceWall(intVector2.x, intVector2.y))
            {
              TilesetIndexMetadata.TilesetFlagType key = TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER;
              if (data.isFaceWallLower(intVector2.x, intVector2.y))
                key = TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER;
              int indexFromTupleArray = SecretRoomUtility.GetIndexFromTupleArray(current, SecretRoomUtility.metadataLookupTableRef[key], current.cellVisualData.roomVisualTypeIndex, 0.0f);
              current.cellVisualData.faceWallOverrideIndex = indexFromTupleArray;
            }
          }
        }
      }
      cellData.isExitCell = true;
      cellData.exitDirection = exitDirection;
      cellData.connectedRoom2 = connectedRoom;
      cellData.connectedRoom1 = this;
    }
  }

  public void UpdateCellVisualData(int ix, int iy)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    for (int index1 = -1; index1 < 2; ++index1)
    {
      for (int index2 = 0; index2 < 4; ++index2)
      {
        if (data.CheckInBoundsAndValid(new IntVector2(ix + index1, iy + index2)))
          data.cellData[ix + index1][iy + index2].cellVisualData.roomVisualTypeIndex = this.m_roomVisualType;
      }
    }
  }

  private void HandleStampedCellVisualData(int ix, int iy, PrototypeDungeonRoomCellData sourceCell)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    for (int index1 = -1; index1 < 2; ++index1)
    {
      for (int index2 = 0; index2 < 4; ++index2)
      {
        if (data.CheckInBounds(ix + index1, iy + index2))
          data.cellData[ix + index1][iy + index2].cellVisualData.roomVisualTypeIndex = this.m_roomVisualType;
      }
    }
  }

  public void RuntimeStampCellComplex(
    int ix,
    int iy,
    CellType type,
    DiagonalWallType diagonalWallType)
  {
    this.StampCellComplex(ix, iy, type, diagonalWallType);
  }

  private bool StampCellComplex(
    int ix,
    int iy,
    CellType type,
    DiagonalWallType diagonalWallType,
    bool breakable = false)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    if (!data.CheckInBounds(new IntVector2(ix, iy)))
      return false;
    CellData cellData = data.cellData[ix][iy];
    if (type == CellType.WALL && cellData.type != CellType.WALL)
    {
      UnityEngine.Debug.LogError((object) $"Attempted to stamp intersecting rooms at: {(object) ix},{(object) iy}. This is a CATEGORY FOUR problem. Talk to Brent.");
      return false;
    }
    cellData.type = type;
    if (!GameManager.Instance.Dungeon.roomMaterialDefinitions[this.RoomVisualSubtype].supportsDiagonalWalls)
      diagonalWallType = DiagonalWallType.NONE;
    if (cellData.diagonalWallType == DiagonalWallType.NONE || diagonalWallType != DiagonalWallType.NONE)
      cellData.diagonalWallType = diagonalWallType;
    if (cellData.diagonalWallType != DiagonalWallType.NONE && cellData.diagonalWallType == diagonalWallType)
    {
      data.cellData[ix][iy + 1].diagonalWallType = diagonalWallType;
      data.cellData[ix][iy + 2].diagonalWallType = diagonalWallType;
    }
    cellData.breakable = breakable;
    if (!GlobalDungeonData.GUNGEON_EXPERIMENTAL && cellData.breakable)
    {
      cellData.breakable = false;
      cellData.type = CellType.FLOOR;
    }
    if (this.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
      cellData.isSecretRoomCell = true;
    cellData.parentArea = this.area;
    cellData.parentRoom = this;
    if (data.CheckInBoundsAndValid(ix, iy + 1) && data.cellData[ix][iy + 1].type == CellType.WALL)
      data.cellData[ix][iy + 1].parentRoom = this;
    if (data.CheckInBoundsAndValid(ix, iy + 2) && data.cellData[ix][iy + 2].type == CellType.WALL)
      data.cellData[ix][iy + 2].parentRoom = this;
    if (data.CheckInBoundsAndValid(ix, iy + 3) && data.cellData[ix][iy + 3].type == CellType.WALL)
      data.cellData[ix][iy + 3].parentRoom = this;
    if (type == CellType.PIT)
      cellData.cellVisualData.containsObjectSpaceStamp = true;
    if (type == CellType.FLOOR || type == CellType.PIT || type == CellType.WALL && cellData.breakable)
    {
      this.rawRoomCells.Add(cellData.position);
      this.roomCells.Add(cellData.position);
      this.roomCellsWithoutExits.Add(cellData.position);
    }
    if ((UnityEngine.Object) this.area.prototypeRoom != (UnityEngine.Object) null && this.area.prototypeRoom.usesProceduralDecoration)
    {
      if (!this.area.prototypeRoom.allowWallDecoration)
        cellData.cellVisualData.containsWallSpaceStamp = true;
      if (!this.area.prototypeRoom.allowFloorDecoration)
        cellData.cellVisualData.containsObjectSpaceStamp = true;
    }
    return true;
  }

  private bool PointInsidePolygon(List<Vector2> points, Vector2 position)
  {
    int index1 = points.Count - 1;
    bool flag = false;
    for (int index2 = 0; index2 < points.Count; ++index2)
    {
      if (((double) points[index2].y < (double) position.y && (double) points[index1].y >= (double) position.y || (double) points[index1].y < (double) position.y && (double) points[index2].y >= (double) position.y) && ((double) points[index2].x <= (double) position.x || (double) points[index1].x <= (double) position.x))
        flag ^= (double) points[index2].x + ((double) position.y - (double) points[index2].y) / ((double) points[index1].y - (double) points[index2].y) * ((double) points[index1].x - (double) points[index2].x) < (double) position.x;
      index1 = index2;
    }
    return flag;
  }

  public IntVector2 GetCellAdjacentToExit(RuntimeExitDefinition exitDef)
  {
    IntVector2 cellAdjacentToExit = IntVector2.Zero;
    for (int index = 0; index < this.CellsWithoutExits.Count; ++index)
    {
      CellData cellData = GameManager.Instance.Dungeon.data[this.CellsWithoutExits[index]];
      CellData exitNeighbor = cellData.GetExitNeighbor();
      if (exitNeighbor != null && (exitNeighbor.position - cellData.position).sqrMagnitude == 1 && exitDef.ContainsPosition(exitNeighbor.position))
      {
        cellAdjacentToExit = cellData.position;
        break;
      }
    }
    return cellAdjacentToExit;
  }

  public RuntimeExitDefinition GetExitDefinitionForConnectedRoom(RoomHandler otherRoom)
  {
    return !this.area.IsProceduralRoom ? this.exitDefinitionsByExit[this.area.exitToLocalDataMap[this.GetExitConnectedToRoom(otherRoom)]] : otherRoom.exitDefinitionsByExit[otherRoom.area.exitToLocalDataMap[otherRoom.GetExitConnectedToRoom(this)]];
  }

  public PrototypeRoomExit GetExitConnectedToRoom(RoomHandler otherRoom)
  {
    for (int index = 0; index < this.area.instanceUsedExits.Count; ++index)
    {
      if (this.connectedRoomsByExit[this.area.instanceUsedExits[index]] == otherRoom)
        return this.area.instanceUsedExits[index];
    }
    return (PrototypeRoomExit) null;
  }

  public CellData GetNearestFloorFacewall(IntVector2 startPosition)
  {
    CellData nearestFloorFacewall = (CellData) null;
    float num1 = float.MaxValue;
    for (int index = 0; index < this.CellsWithoutExits.Count; ++index)
    {
      CellData cellData = GameManager.Instance.Dungeon.data[this.CellsWithoutExits[index]];
      if (GameManager.Instance.Dungeon.data[cellData.position + IntVector2.Up].IsAnyFaceWall() && cellData.type == CellType.FLOOR)
      {
        float num2 = Vector2.Distance(cellData.position.ToCenterVector2(), startPosition.ToCenterVector2());
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          nearestFloorFacewall = cellData;
        }
      }
    }
    return nearestFloorFacewall;
  }

  public CellData GetNearestFaceOrSidewall(IntVector2 startPosition)
  {
    CellData nearestFaceOrSidewall = (CellData) null;
    float num1 = float.MaxValue;
    for (int index = 0; index < this.CellsWithoutExits.Count; ++index)
    {
      CellData cellData = GameManager.Instance.Dungeon.data[this.CellsWithoutExits[index]];
      if (GameManager.Instance.Dungeon.data[cellData.position + IntVector2.Up].IsAnyFaceWall() || cellData.IsSideWallAdjacent())
      {
        float num2 = Vector2.Distance(cellData.position.ToCenterVector2(), startPosition.ToCenterVector2());
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          nearestFaceOrSidewall = cellData;
        }
      }
    }
    return nearestFaceOrSidewall;
  }

  private List<Vector2> GetPolygonDecomposition()
  {
    List<Vector2> polygonDecomposition = new List<Vector2>();
    if (!this.area.IsProceduralRoom)
    {
      Rect boundingRect = this.GetBoundingRect();
      polygonDecomposition.Add(boundingRect.min);
      polygonDecomposition.Add(new Vector2(boundingRect.xMin, boundingRect.yMax));
      polygonDecomposition.Add(boundingRect.max);
      polygonDecomposition.Add(new Vector2(boundingRect.xMax, boundingRect.yMin));
    }
    else
    {
      Rect boundingRect = this.GetBoundingRect();
      polygonDecomposition.Add(boundingRect.min);
      polygonDecomposition.Add(new Vector2(boundingRect.xMin, boundingRect.yMax));
      polygonDecomposition.Add(boundingRect.max);
      polygonDecomposition.Add(new Vector2(boundingRect.xMax, boundingRect.yMin));
    }
    return polygonDecomposition;
  }

  private Rect GetBoundingRect()
  {
    return new Rect((float) this.area.basePosition.x, (float) this.area.basePosition.y, (float) this.area.dimensions.x, (float) this.area.dimensions.y);
  }

  public CellData GetNearestCellToPosition(Vector2 position)
  {
    CellData nearestCellToPosition = (CellData) null;
    float num1 = float.MaxValue;
    for (int index = 0; index < this.roomCells.Count; ++index)
    {
      float num2 = Vector2.Distance(position, this.roomCells[index].ToVector2());
      if ((double) num2 < (double) num1)
      {
        nearestCellToPosition = GameManager.Instance.Dungeon.data[this.roomCells[index]];
        num1 = num2;
      }
    }
    return nearestCellToPosition;
  }

  public IntVector2 GetRandomAvailableCellDumb()
  {
    for (int index = 0; index < 1000; ++index)
    {
      IntVector2 basePosition = new IntVector2(UnityEngine.Random.Range(this.area.basePosition.x, this.area.basePosition.x + this.area.dimensions.x), UnityEngine.Random.Range(this.area.basePosition.y, this.area.basePosition.y + this.area.dimensions.y));
      if (this.CheckCellArea(basePosition, IntVector2.One))
        return basePosition;
    }
    UnityEngine.Debug.LogError((object) "No available cells. Error.");
    return IntVector2.Zero;
  }

  public IntVector2? GetOffscreenCell(
    IntVector2? footprint = null,
    CellTypes? passableCellTypes = null,
    bool canPassOccupied = false,
    Vector2? idealPosition = null)
  {
    if (!footprint.HasValue)
      footprint = new IntVector2?(IntVector2.One);
    if (!passableCellTypes.HasValue)
      passableCellTypes = new CellTypes?((CellTypes) 2147483647 /*0x7FFFFFFF*/);
    Dungeon dungeon = GameManager.Instance.Dungeon;
    List<IntVector2> intVector2List = new List<IntVector2>();
    for (int x = this.area.basePosition.x; x < this.area.basePosition.x + this.area.dimensions.x; ++x)
    {
      for (int y = this.area.basePosition.y; y < this.area.basePosition.y + this.area.dimensions.y; ++y)
      {
        IntVector2 intVector2 = new IntVector2(x, y);
        if (dungeon.data.CheckInBoundsAndValid(intVector2) && !GameManager.Instance.MainCameraController.PointIsVisible(intVector2.ToCenterVector2()) && Pathfinder.Instance.IsPassable(intVector2, footprint, passableCellTypes, canPassOccupied))
          intVector2List.Add(intVector2);
      }
    }
    if (idealPosition.HasValue)
    {
      if (intVector2List.Count > 0)
      {
        intVector2List.Sort((Comparison<IntVector2>) ((a, b) => Mathf.Abs((float) a.y - idealPosition.Value.y).CompareTo(Mathf.Abs((float) b.y - idealPosition.Value.y))));
        return new IntVector2?(intVector2List[0]);
      }
    }
    else if (intVector2List.Count > 0)
      return new IntVector2?(intVector2List[UnityEngine.Random.Range(0, intVector2List.Count)]);
    return new IntVector2?();
  }

  public IntVector2? GetRandomAvailableCell(
    IntVector2? footprint = null,
    CellTypes? passableCellTypes = null,
    bool canPassOccupied = false,
    CellValidator cellValidator = null)
  {
    if (!footprint.HasValue)
      footprint = new IntVector2?(IntVector2.One);
    if (!passableCellTypes.HasValue)
      passableCellTypes = new CellTypes?((CellTypes) 2147483647 /*0x7FFFFFFF*/);
    DungeonData data = GameManager.Instance.Dungeon.data;
    List<IntVector2> intVector2List = new List<IntVector2>();
    for (int x = this.area.basePosition.x; x < this.area.basePosition.x + this.area.dimensions.x; ++x)
    {
      for (int y = this.area.basePosition.y; y < this.area.basePosition.y + this.area.dimensions.y; ++y)
      {
        CellData cellData = data[x, y];
        if (cellData != null && cellData.parentRoom == this && !cellData.isExitCell && (canPassOccupied || !cellData.containsTrap))
        {
          IntVector2 pos = new IntVector2(x, y);
          if (Pathfinder.Instance.IsPassable(pos, footprint, passableCellTypes, canPassOccupied, cellValidator))
            intVector2List.Add(pos);
        }
      }
    }
    return intVector2List.Count > 0 ? new IntVector2?(intVector2List[UnityEngine.Random.Range(0, intVector2List.Count)]) : new IntVector2?();
  }

  public IntVector2? GetNearestAvailableCell(
    Vector2 nearbyPoint,
    IntVector2? footprint = null,
    CellTypes? passableCellTypes = null,
    bool canPassOccupied = false,
    CellValidator cellValidator = null)
  {
    if (!footprint.HasValue)
      footprint = new IntVector2?(IntVector2.One);
    if (!passableCellTypes.HasValue)
      passableCellTypes = new CellTypes?((CellTypes) 2147483647 /*0x7FFFFFFF*/);
    DungeonData data = GameManager.Instance.Dungeon.data;
    Vector2 vector2_1 = footprint.Value.ToVector2() / 2f;
    float num = float.MaxValue;
    IntVector2? nearestAvailableCell = new IntVector2?();
    for (int x = this.area.basePosition.x; x < this.area.basePosition.x + this.area.dimensions.x; ++x)
    {
      for (int y = this.area.basePosition.y; y < this.area.basePosition.y + this.area.dimensions.y; ++y)
      {
        CellData cellData = data[x, y];
        if (cellData != null && cellData.parentRoom == this && !cellData.isExitCell)
        {
          IntVector2 pos = new IntVector2(x, y);
          if (Pathfinder.Instance.IsPassable(pos, footprint, passableCellTypes, canPassOccupied, cellValidator))
          {
            Vector2 vector2_2 = pos.ToVector2() + vector2_1;
            float sqrMagnitude = (nearbyPoint - vector2_2).sqrMagnitude;
            if ((double) sqrMagnitude < (double) num)
            {
              num = sqrMagnitude;
              nearestAvailableCell = new IntVector2?(pos);
            }
          }
        }
      }
    }
    return nearestAvailableCell;
  }

  public IntVector2? GetRandomWeightedAvailableCell(
    IntVector2? footprint = null,
    CellTypes? passableCellTypes = null,
    bool canPassOccupied = false,
    CellValidator cellValidator = null,
    Func<IntVector2, float> cellWeightFinder = null,
    float percentageBounds = 1f)
  {
    if (!footprint.HasValue)
      footprint = new IntVector2?(IntVector2.One);
    if (!passableCellTypes.HasValue)
      passableCellTypes = new CellTypes?((CellTypes) 2147483647 /*0x7FFFFFFF*/);
    DungeonData data = GameManager.Instance.Dungeon.data;
    List<Tuple<IntVector2, float>> tupleList = new List<Tuple<IntVector2, float>>();
    for (int x = this.area.basePosition.x; x < this.area.basePosition.x + this.area.dimensions.x; ++x)
    {
      for (int y = this.area.basePosition.y; y < this.area.basePosition.y + this.area.dimensions.y; ++y)
      {
        CellData cellData = data[x, y];
        if (cellData != null && cellData.parentRoom == this && !cellData.isExitCell)
        {
          IntVector2 pos = new IntVector2(x, y);
          if (Pathfinder.Instance.IsPassable(pos, footprint, passableCellTypes, canPassOccupied, cellValidator))
            tupleList.Add(Tuple.Create<IntVector2, float>(pos, cellWeightFinder(pos)));
        }
      }
    }
    tupleList.Sort((IComparer<Tuple<IntVector2, float>>) new RoomHandler.TupleComparer());
    return tupleList.Count > 0 ? new IntVector2?(tupleList[UnityEngine.Random.Range(0, Mathf.RoundToInt((float) tupleList.Count * percentageBounds))].First) : new IntVector2?();
  }

  public IntVector2 GetCenterCell()
  {
    return new IntVector2(this.area.basePosition.x + Mathf.FloorToInt((float) (this.area.dimensions.x / 2)), this.area.basePosition.y + Mathf.FloorToInt((float) (this.area.dimensions.y / 2)));
  }

  public void DefineEpicenter(HashSet<IntVector2> startingBorder)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    bool flag = true;
    HashSet<IntVector2> intVector2Set1 = startingBorder;
    HashSet<IntVector2> intVector2Set2 = new HashSet<IntVector2>();
    HashSet<IntVector2> intVector2Set3 = new HashSet<IntVector2>();
    while (flag)
    {
      flag = false;
      IntVector2[] cardinals = IntVector2.Cardinals;
      foreach (IntVector2 key in intVector2Set1)
      {
        if (!data[key].isExitCell)
        {
          intVector2Set3.Add(key);
          for (int index = 0; index < cardinals.Length; ++index)
          {
            IntVector2 intVector2 = key + cardinals[index];
            if (!intVector2Set3.Contains(intVector2) && !intVector2Set2.Contains(intVector2) && data.CheckInBoundsAndValid(intVector2))
            {
              CellData cellData = data[intVector2];
              if (cellData != null && !cellData.isExitCell && cellData.type != CellType.WALL && cellData.parentRoom == this)
              {
                intVector2Set2.Add(intVector2);
                this.Epicenter = intVector2;
                flag = true;
              }
            }
          }
        }
      }
      intVector2Set1 = intVector2Set2;
      intVector2Set2 = new HashSet<IntVector2>();
    }
  }

  private List<IntVector2> CollectRandomValidCells(IntVector2 objDimensions, int offset)
  {
    List<IntVector2> intVector2List = new List<IntVector2>();
    for (int x = this.area.basePosition.x + offset; x < this.area.basePosition.x + this.area.dimensions.x - offset - (objDimensions.x - 1); ++x)
    {
      for (int y = this.area.basePosition.y + offset; y < this.area.basePosition.y + this.area.dimensions.y - offset - (objDimensions.y - 1); ++y)
      {
        IntVector2 basePosition = new IntVector2(x, y);
        if (this.CheckCellArea(basePosition, objDimensions))
          intVector2List.Add(basePosition);
      }
    }
    return intVector2List;
  }

  public List<TK2DInteriorDecorator.WallExpanse> GatherExpanses(
    DungeonData.Direction dir,
    bool breakAfterFirst = true,
    bool debugMe = false,
    bool disallowPits = false)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    List<TK2DInteriorDecorator.WallExpanse> wallExpanseList = new List<TK2DInteriorDecorator.WallExpanse>(12);
    bool flag1 = false;
    TK2DInteriorDecorator.WallExpanse wallExpanse1 = new TK2DInteriorDecorator.WallExpanse();
    IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(dir);
    IntVector2 intVector2_1 = -1 * vector2FromDirection;
    int num1 = intVector2_1.x != 0 ? (intVector2_1.x >= 0 ? -1 : this.area.dimensions.x) : -1;
    int num2 = intVector2_1.y != 0 ? (intVector2_1.y >= 0 ? -1 : this.area.dimensions.y) : -1;
    int num3 = intVector2_1.x != 0 ? (intVector2_1.x >= 0 ? 1 : -1) : 1;
    int num4 = intVector2_1.y != 0 ? (intVector2_1.y >= 0 ? 1 : -1) : 1;
    bool flag2 = intVector2_1.x != 0;
    IntVector2 intVector2_2 = !flag2 ? IntVector2.Right : IntVector2.Up;
    if (flag2)
    {
      for (int y = num2; y <= this.area.dimensions.y && y >= -1; y += num4)
      {
        bool flag3 = false;
        for (int x = num1; x <= this.area.dimensions.x && x >= -1; x += num3)
        {
          IntVector2 key = this.area.basePosition + new IntVector2(x, y);
          CellData cellData1 = data[key];
          bool flag4 = cellData1 == null || cellData1.type == CellType.WALL;
          CellData cellData2 = data[key + vector2FromDirection];
          bool flag5 = cellData2 == null || (cellData2.type == CellType.WALL || data.isAnyFaceWall(cellData2.position.x, cellData2.position.y)) && !cellData2.isExitCell;
          if (flag5 && cellData2 != null && cellData2.diagonalWallType != DiagonalWallType.NONE)
            flag5 = false;
          if (!flag4 && (!disallowPits || cellData1.type != CellType.PIT) && cellData1.parentRoom == this && flag5)
          {
            flag3 = true;
            if (cellData1.isExitCell)
            {
              if (flag1)
                wallExpanseList.Add(wallExpanse1);
              flag1 = false;
              break;
            }
            if (!flag1)
            {
              flag1 = true;
              wallExpanse1 = new TK2DInteriorDecorator.WallExpanse(new IntVector2(x, y), 1);
            }
            else if (flag2 && wallExpanse1.basePosition.x == x)
              ++wallExpanse1.width;
            else if (!flag2 && wallExpanse1.basePosition.y == y)
            {
              ++wallExpanse1.width;
            }
            else
            {
              wallExpanseList.Add(wallExpanse1);
              wallExpanse1 = new TK2DInteriorDecorator.WallExpanse(new IntVector2(x, y), 1);
            }
            if (breakAfterFirst)
              break;
          }
        }
        if (!flag3)
        {
          if (flag1)
            wallExpanseList.Add(wallExpanse1);
          flag1 = false;
        }
      }
    }
    else
    {
      for (int x = num1; x <= this.area.dimensions.x && x >= -1; x += num3)
      {
        bool flag6 = false;
        for (int y = num2; y <= this.area.dimensions.y && y >= -1; y += num4)
        {
          IntVector2 key = this.area.basePosition + new IntVector2(x, y);
          CellData cellData3 = data[key];
          bool flag7 = cellData3 == null || cellData3.type == CellType.WALL;
          CellData cellData4 = data[key + vector2FromDirection];
          bool flag8 = cellData4 == null || (cellData4.type == CellType.WALL || data.isAnyFaceWall(cellData4.position.x, cellData4.position.y)) && !cellData4.isExitCell;
          if (flag8 && cellData4 != null && cellData4.diagonalWallType != DiagonalWallType.NONE)
            flag8 = false;
          if (!flag7 && cellData3.parentRoom == this && flag8)
          {
            flag6 = true;
            if (cellData3.isExitCell)
            {
              if (flag1)
                wallExpanseList.Add(wallExpanse1);
              flag1 = false;
              break;
            }
            if (!flag1)
            {
              flag1 = true;
              wallExpanse1 = new TK2DInteriorDecorator.WallExpanse(new IntVector2(x, y), 1);
            }
            else if (flag2 && wallExpanse1.basePosition.x == x)
              ++wallExpanse1.width;
            else if (!flag2 && wallExpanse1.basePosition.y == y)
            {
              ++wallExpanse1.width;
            }
            else
            {
              wallExpanseList.Add(wallExpanse1);
              wallExpanse1 = new TK2DInteriorDecorator.WallExpanse(new IntVector2(x, y), 1);
            }
            if (breakAfterFirst)
              break;
          }
        }
        if (!flag6)
        {
          if (flag1)
            wallExpanseList.Add(wallExpanse1);
          flag1 = false;
        }
      }
    }
    if (flag1 && !wallExpanseList.Contains(wallExpanse1))
      wallExpanseList.Add(wallExpanse1);
    if (debugMe)
    {
      foreach (TK2DInteriorDecorator.WallExpanse wallExpanse2 in wallExpanseList)
      {
        for (int index = 0; index < wallExpanse2.width; ++index)
          BraveUtility.DrawDebugSquare(this.area.basePosition + wallExpanse2.basePosition + intVector2_2 * index, Color.yellow);
      }
    }
    return wallExpanseList;
  }

  public List<IntVector2> GatherPitLighting(
    TilemapDecoSettings decoSettings,
    List<IntVector2> existingLights)
  {
    float num = (float) (decoSettings.lightOverlapRadius - 2);
    List<IntVector2> intVector2List = new List<IntVector2>();
    for (int index1 = 0; index1 < this.Cells.Count; ++index1)
    {
      IntVector2 cell = this.Cells[index1];
      bool flag = true;
      for (int index2 = 0; index2 < intVector2List.Count; ++index2)
      {
        if ((double) IntVector2.ManhattanDistance(cell, intVector2List[index2] + this.area.basePosition) <= (double) num)
          flag = false;
      }
      for (int index3 = 0; index3 < existingLights.Count; ++index3)
      {
        if ((double) IntVector2.ManhattanDistance(cell, existingLights[index3] + this.area.basePosition) <= (double) num)
          flag = false;
      }
      if (flag && GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(cell))
      {
        CellData cellData = GameManager.Instance.Dungeon.data[cell];
        if (cellData.type == CellType.PIT && !cellData.SurroundedByPits())
          intVector2List.Add(cell - this.area.basePosition);
      }
    }
    return intVector2List;
  }

  private List<IntVector2> GatherLightPositionForDirection(
    TilemapDecoSettings decoSettings,
    DungeonData.Direction dir)
  {
    float lightOverlapRadius = (float) decoSettings.lightOverlapRadius;
    List<IntVector2> intVector2List = new List<IntVector2>();
    DungeonData.Direction dir1 = dir;
    IntVector2 intVector2_1;
    switch (dir1)
    {
      case DungeonData.Direction.NORTH:
      case DungeonData.Direction.SOUTH:
        intVector2_1 = IntVector2.Right;
        break;
      default:
        intVector2_1 = IntVector2.Up;
        break;
    }
    IntVector2 intVector2_2 = intVector2_1;
    List<TK2DInteriorDecorator.WallExpanse> wallExpanseList = this.GatherExpanses(dir1);
    for (int index1 = 0; index1 < wallExpanseList.Count; ++index1)
    {
      TK2DInteriorDecorator.WallExpanse wallExpanse = wallExpanseList[index1];
      if (wallExpanse.width >= decoSettings.minLightExpanseWidth)
      {
        if ((double) wallExpanse.width < (double) lightOverlapRadius * 2.0)
        {
          IntVector2 intVector2_3 = wallExpanse.basePosition + intVector2_2 * Mathf.FloorToInt((float) wallExpanse.width / 2f);
          intVector2List.Add(intVector2_3);
        }
        else
        {
          int num1 = Mathf.FloorToInt((float) wallExpanse.width / lightOverlapRadius);
          int num2 = Mathf.FloorToInt((float) (((double) wallExpanse.width - (double) (num1 - 1) * (double) lightOverlapRadius) / 2.0));
          for (int index2 = 0; index2 < num1; ++index2)
          {
            int num3 = num2 + Mathf.FloorToInt(lightOverlapRadius) * index2;
            IntVector2 intVector2_4 = wallExpanse.basePosition + intVector2_2 * num3;
            intVector2List.Add(intVector2_4);
          }
        }
      }
    }
    return intVector2List;
  }

  public List<IntVector2> GatherManualLightPositions()
  {
    List<IntVector2> intVector2List = new List<IntVector2>();
    for (int x = this.area.basePosition.x; x < this.area.basePosition.x + this.area.dimensions.x; ++x)
    {
      for (int y = this.area.basePosition.y; y < this.area.basePosition.y + this.area.dimensions.y; ++y)
      {
        if (this.area.prototypeRoom.ForceGetCellDataAtPoint(x - this.area.basePosition.x, y - this.area.basePosition.y).containsManuallyPlacedLight)
          intVector2List.Add(new IntVector2(x, y) - this.area.basePosition);
      }
    }
    return intVector2List;
  }

  public List<IntVector2> GatherOptimalLightPositions(TilemapDecoSettings decoSettings)
  {
    List<IntVector2> intVector2List = this.GatherLightPositionForDirection(decoSettings, DungeonData.Direction.NORTH);
    intVector2List.AddRange((IEnumerable<IntVector2>) this.GatherLightPositionForDirection(decoSettings, DungeonData.Direction.EAST));
    intVector2List.AddRange((IEnumerable<IntVector2>) this.GatherLightPositionForDirection(decoSettings, DungeonData.Direction.SOUTH));
    intVector2List.AddRange((IEnumerable<IntVector2>) this.GatherLightPositionForDirection(decoSettings, DungeonData.Direction.WEST));
    for (int index1 = 0; index1 < intVector2List.Count; ++index1)
    {
      for (int index2 = 0; index2 < intVector2List.Count; ++index2)
      {
        if (index1 != index2 && (double) Vector2.Distance(intVector2List[index1].ToVector2(), intVector2List[index2].ToVector2()) < (double) decoSettings.nearestAllowedLight)
        {
          if (intVector2List[index1].y < intVector2List[index2].y)
          {
            intVector2List.RemoveAt(index1);
            --index1;
            break;
          }
          intVector2List.RemoveAt(index2);
          --index2;
          if (index1 > index2)
            --index1;
        }
      }
    }
    return intVector2List;
  }

  private bool CheckCellArea(IntVector2 basePosition, IntVector2 objDimensions)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    bool flag = true;
    for (int x = basePosition.x; x < basePosition.x + objDimensions.x; ++x)
    {
      for (int y = basePosition.y; y < basePosition.y + objDimensions.y; ++y)
      {
        if (!data.cellData[x][y].IsPassable)
        {
          flag = false;
          goto label_8;
        }
      }
    }
label_8:
    return flag;
  }

  private bool CellWithinRadius(Vector2 center, float radius, IntVector2 cubePos)
  {
    Vector2 b = new Vector2((float) cubePos.x + 0.5f, (float) cubePos.y + 0.5f);
    float num = Vector2.Distance(center, b);
    return (double) radius >= (double) num;
  }

  public enum ProceduralLockType
  {
    NONE,
    BASE_SHOP,
  }

  public enum VisibilityStatus
  {
    OBSCURED = 0,
    VISITED = 1,
    CURRENT = 2,
    REOBSCURED = 3,
    INVALID = 99, // 0x00000063
  }

  public enum NPCSealState
  {
    SealNone,
    SealAll,
    SealPrior,
    SealNext,
  }

  public delegate void OnEnteredEventHandler(PlayerController p);

  public enum CustomRoomState
  {
    NONE = 0,
    LICH_PHASE_THREE = 100, // 0x00000064
  }

  public delegate void OnExitedEventHandler();

  public delegate void OnBecameVisibleEventHandler(float delay);

  public delegate void OnBecameInvisibleEventHandler();

  public enum RewardLocationStyle
  {
    CameraCenter,
    PlayerCenter,
    Original,
  }

  public enum ActiveEnemyType
  {
    All,
    RoomClear,
  }

  private class TupleComparer : IComparer<Tuple<IntVector2, float>>
  {
    public int Compare(Tuple<IntVector2, float> a, Tuple<IntVector2, float> b)
    {
      if ((double) a.Second < (double) b.Second)
        return -1;
      return (double) b.Second > (double) a.Second ? 1 : 0;
    }
  }
}
