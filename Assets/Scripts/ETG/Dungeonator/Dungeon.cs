using HutongGames.PlayMaker.Actions;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using tk2dRuntime.TileMap;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
namespace Dungeonator
{
  public class Dungeon : MonoBehaviour
  {
    public static bool IsGenerating;
    public ContentSource contentSource;
    public int DungeonSeed;
    public string DungeonShortName = string.Empty;
    public string DungeonFloorName = "Gungeon";
    public string DungeonFloorLevelTextOverride = "no override";
    public GameManager.LevelOverrideState LevelOverrideType;
    public DebugDungeonSettings debugSettings;
    public SemioticDungeonGenSettings PatternSettings;
    public bool ForceRegenerationOfCharacters;
    public bool ActuallyGenerateTilemap = true;
    public TilemapDecoSettings decoSettings;
    public TileIndices tileIndices;
    [SerializeField]
    public DungeonMaterial[] roomMaterialDefinitions;
    [SerializeField]
    public DungeonWingDefinition[] dungeonWingDefinitions;
    [SerializeField]
    public List<TileIndexGrid> pathGridDefinitions;
    public DustUpVFX dungeonDustups;
    public DamageTypeEffectMatrix damageTypeEffectMatrix;
    public DungeonTileStampData stampData;
    [Header("Procedural Room Population")]
    public bool UsesCustomFloorIdea;
    public RobotDaveIdea FloorIdea;
    [Header("Doors")]
    public bool PlaceDoors;
    public DungeonPlaceable doorObjects;
    public DungeonPlaceable lockedDoorObjects;
    public DungeonPlaceable oneWayDoorObjects;
    public GameObject oneWayDoorPressurePlate;
    public DungeonPlaceable phantomBlockerDoorObjects;
    public DungeonPlaceable alternateDoorObjectsNakatomi;
    public bool UsesWallWarpWingDoors;
    public GameObject WarpWingDoorPrefab;
    public GenericLootTable baseChestContents;
    [Header("Secret Rooms")]
    public List<GameObject> SecretRoomSimpleTriggersFacewall;
    public List<GameObject> SecretRoomSimpleTriggersSidewall;
    public List<ComplexSecretRoomTrigger> SecretRoomComplexTriggers;
    public GameObject SecretRoomDoorSparkVFX;
    public GameObject SecretRoomHorizontalPoofVFX;
    public GameObject SecretRoomVerticalPoofVFX;
    public GameObject RatTrapdoor;
    [EnemyIdentifier]
    public string NormalRatGUID;
    public SharedDungeonSettings sharedSettingsPrefab;
    [PickupIdentifier]
    public int BossMasteryTokenItemId = -1;
    public bool UsesOverrideTertiaryBossSets;
    public List<TertiaryBossRewardSet> OverrideTertiaryRewardSets;
    public DungeonData data;
    public GameObject defaultPlayerPrefab;
    public System.Action OnAnyRoomVisited;
    public System.Action OnAllRoomsVisited;
    private TK2DDungeonAssembler assembler;
    private tk2dTileMap m_tilemap;
    [Header("Special Scene Options")]
    public bool StripPlayerOnArrival;
    public bool SuppressEmergencyCrates;
    public bool SetTutorialFlag;
    [NonSerialized]
    public bool PreventPlayerLightInDarkTerrifyingRooms;
    public bool PlayerIsLight;
    public Color PlayerLightColor = Color.white;
    public float PlayerLightIntensity = 3f;
    public float PlayerLightRadius = 5f;
    public GameObject[] PrefabsToAutoSpawn;
    [NonSerialized]
    public int FrameDungeonGenerationFinished = -1;
    [NonSerialized]
    public bool IsGlitchDungeon;
    [NonSerialized]
    public bool OverrideAmbientLight;
    [NonSerialized]
    public Color OverrideAmbientColor;
    public const int TOP_WALL_ENEMY_BULLET_BLOCKER_PIXEL_HEIGHT = 14;
    public const int TOP_WALL_ENEMY_BLOCKER_PIXEL_HEIGHT = 12;
    public const int TOP_WALL_PLAYER_BLOCKER_PIXEL_HEIGHT = 8;
    [NonSerialized]
    public bool HasGivenMasteryToken;
    [NonSerialized]
    public bool HasGivenBossrushGun;
    [NonSerialized]
    public bool IsEndTimes;
    [NonSerialized]
    public bool CurseReaperActive;
    [NonSerialized]
    public float GeneratedMagnificence;
    public static bool ShouldAttemptToLoadFromMidgameSave;
    private bool m_allRoomsVisited;
    private bool m_musicIsPlaying;
    public string musicEventName;
    private float m_newPlayerMultiplier = -1f;
    protected List<GeneratedEnemyData> m_generatedEnemyData = new List<GeneratedEnemyData>();
    private bool m_ambientVFXProcessingActive;

    public event Action<Dungeon, PlayerController> OnDungeonGenerationComplete;

    public tk2dTileMap MainTilemap => this.m_tilemap;

    public int Width => this.data.Width;

    public int Height => this.data.Height;

    public int GetDungeonSeed()
    {
      if (!BraveRandom.IsInitialized())
        BraveRandom.InitializeRandom();
      int seed = GameManager.Instance.CurrentRunSeed;
      if (seed == 0)
        seed = this.DungeonSeed;
      if (seed == 0)
        seed = BraveRandom.GenerationRandomRange(1, 1000000000);
      else
        GameManager.Instance.InitializeForRunWithSeed(seed);
      return seed;
    }

    public DungeonWingDefinition SelectWingDefinition(bool criticalPath)
    {
      List<DungeonWingDefinition> dungeonWingDefinitionList = new List<DungeonWingDefinition>();
      float num1 = 0.0f;
      for (int index = 0; index < this.dungeonWingDefinitions.Length; ++index)
      {
        if (this.dungeonWingDefinitions[index].canBeCriticalPath && criticalPath || this.dungeonWingDefinitions[index].canBeNoncriticalPath && !criticalPath)
        {
          dungeonWingDefinitionList.Add(this.dungeonWingDefinitions[index]);
          num1 += this.dungeonWingDefinitions[index].weight;
        }
      }
      float num2 = num1 * BraveRandom.GenerationRandomValue();
      float num3 = 0.0f;
      for (int index = 0; index < dungeonWingDefinitionList.Count; ++index)
      {
        num3 += dungeonWingDefinitionList[index].weight;
        if ((double) num3 >= (double) num2)
          return dungeonWingDefinitionList[index];
      }
      return dungeonWingDefinitionList[0];
    }

    public float TargetAmbientIntensity
    {
      get
      {
        return GameManager.Instance.IsFoyer || GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW ? 1f : 1.15f;
      }
    }

    public float ExplosionBulletDeletionMultiplier { get; set; }

    public bool IsExplosionBulletDeletionRecovering { get; set; }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Dungeon__Startc__Iterator0()
      {
        _this = this
      };
    }

    public void RegenerationCleanup()
    {
      GameObject gameObject1 = GameObject.Find("A*");
      if ((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject1);
      GameObject gameObject2 = GameObject.Find("_Lights");
      if ((UnityEngine.Object) gameObject2 != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject2);
      GameObject gameObject3 = GameObject.Find("_Rooms");
      if ((UnityEngine.Object) gameObject3 != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject3);
      GameObject gameObject4 = GameObject.Find("_Doors");
      if ((UnityEngine.Object) gameObject4 != (UnityEngine.Object) null)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject4);
      GameObject gameObject5 = GameObject.Find("_SpawnManager");
      if (!((UnityEngine.Object) gameObject5 != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject5);
    }

    private void StartupFoyerChecks()
    {
      UnityEngine.Debug.LogError((object) "Doing startup foyer checks!");
      if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SHOP_HAS_MET_BEETLE) || GameStatsManager.Instance.GetFlag(GungeonFlags.SHOP_BEETLE_ACTIVE) || (double) GameStatsManager.Instance.AccumulatedBeetleMerchantChance <= 0.0)
        return;
      if ((double) UnityEngine.Random.value < (double) (GameStatsManager.Instance.AccumulatedBeetleMerchantChance + GameStatsManager.Instance.AccumulatedUsedBeetleMerchantChance))
      {
        GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 0.0f;
        GameStatsManager.Instance.AccumulatedUsedBeetleMerchantChance = 0.0f;
        GameStatsManager.Instance.SetFlag(GungeonFlags.SHOP_BEETLE_ACTIVE, true);
      }
      else
      {
        GameStatsManager.Instance.AccumulatedUsedBeetleMerchantChance += GameStatsManager.Instance.AccumulatedBeetleMerchantChance;
        GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 0.0f;
      }
    }

    [DebuggerHidden]
    public IEnumerator Regenerate(bool cleanup)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Dungeon__Regeneratec__Iterator1()
      {
        cleanup = cleanup,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator FrameDelayedMidgameInitialization(MidGameSaveData midgameSave)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Dungeon__FrameDelayedMidgameInitializationc__Iterator2()
      {
        midgameSave = midgameSave
      };
    }

    public void AssignCurrencyDrops()
    {
      FloorRewardData currentRewardData = GameManager.Instance.RewardManager.CurrentRewardData;
      int a1 = Mathf.CeilToInt(Mathf.Clamp(BraveMathCollege.GetRandomByNormalDistribution(currentRewardData.AverageCurrencyDropsThisFloor, currentRewardData.CurrencyDropsStandardDeviation), Mathf.Max(20f, currentRewardData.MinimumCurrencyDropsThisFloor), 250f));
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer && PrepareTakeSherpaPickup.IsOnSherpaMoneyStep && GameManager.Instance.PrimaryPlayer.carriedConsumables.KeyBullets > 1)
        a1 = Mathf.CeilToInt(Mathf.Max((float) a1, currentRewardData.AverageCurrencyDropsThisFloor + currentRewardData.CurrencyDropsStandardDeviation));
      float bossGoldCoinChance = GameManager.Instance.RewardManager.BossGoldCoinChance;
      float powerfulGoldCoinChance = GameManager.Instance.RewardManager.PowerfulGoldCoinChance;
      float normalGoldCoinChance = GameManager.Instance.RewardManager.NormalGoldCoinChance;
      List<AIActor> aiActorList1 = new List<AIActor>();
      List<AIActor> aiActorList2 = new List<AIActor>();
      List<AIActor> aiActorList3 = new List<AIActor>();
      for (int index = 0; index < StaticReferenceManager.AllEnemies.Count; ++index)
      {
        AIActor allEnemy = StaticReferenceManager.AllEnemies[index];
        if ((bool) (UnityEngine.Object) allEnemy && allEnemy.CanDropCurrency)
        {
          if ((bool) (UnityEngine.Object) allEnemy.healthHaver && allEnemy.healthHaver.IsBoss)
            aiActorList1.Add(allEnemy);
          else if (allEnemy.IsSignatureEnemy)
            aiActorList2.Add(allEnemy);
          else
            aiActorList3.Add(allEnemy);
        }
      }
      int totalEnemyCount = aiActorList3.Count + aiActorList2.Count;
      for (int index = 0; index < aiActorList3.Count; ++index)
      {
        if (!(aiActorList3[index].EnemyGuid == GlobalEnemyGuids.GripMaster) && !aiActorList3[index].IsMimicEnemy)
          this.RegisterGeneratedEnemyData(aiActorList3[index].EnemyGuid, totalEnemyCount, false);
      }
      for (int index = 0; index < aiActorList2.Count; ++index)
      {
        if (!(aiActorList2[index].EnemyGuid == GlobalEnemyGuids.GripMaster) && !aiActorList2[index].IsMimicEnemy)
          this.RegisterGeneratedEnemyData(aiActorList2[index].EnemyGuid, totalEnemyCount, true);
      }
      int b1 = aiActorList1.Count <= 0 ? 0 : BraveRandom.GenerationRandomRange(5, a1 / 4);
      int a2 = aiActorList2.Count * 10;
      int a3 = aiActorList2.Count <= 0 ? 0 : Mathf.Min(a2, Mathf.FloorToInt((float) (a1 - b1) * 0.5f));
      int num = a1 - (b1 + a3);
      int a4 = Mathf.CeilToInt((float) b1 / (float) aiActorList1.Count);
      int b2 = Mathf.CeilToInt((float) a3 / (float) aiActorList2.Count);
      for (int index = 0; index < aiActorList1.Count; ++index)
      {
        aiActorList1[index].AssignedCurrencyToDrop = Mathf.Min(a4, b1);
        b1 -= a4;
        if ((double) BraveRandom.GenerationRandomValue() < (double) bossGoldCoinChance)
          aiActorList1[index].AssignedCurrencyToDrop += 50;
      }
      for (int index = 0; index < aiActorList2.Count; ++index)
      {
        aiActorList2[index].AssignedCurrencyToDrop = Mathf.Min(a3, b2);
        a3 -= b2;
        if ((double) BraveRandom.GenerationRandomValue() < (double) powerfulGoldCoinChance)
          aiActorList2[index].AssignedCurrencyToDrop += 50;
      }
      for (; num > 0 && aiActorList3.Count > 0; --num)
        ++aiActorList3[BraveRandom.GenerationRandomRange(0, aiActorList3.Count)].AssignedCurrencyToDrop;
      for (int index = 0; index < aiActorList3.Count; ++index)
      {
        if ((double) BraveRandom.GenerationRandomValue() < (double) normalGoldCoinChance)
          aiActorList3[index].AssignedCurrencyToDrop += 50;
      }
    }

    [DebuggerHidden]
    private IEnumerator ForceRegenerateDelayed()
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Dungeon__ForceRegenerateDelayedc__Iterator3 delayedCIterator3 = new Dungeon__ForceRegenerateDelayedc__Iterator3();
      return (IEnumerator) delayedCIterator3;
    }

    [DebuggerHidden]
    private IEnumerator PostDungeonGenerationCleanup()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Dungeon__PostDungeonGenerationCleanupc__Iterator4()
      {
        _this = this
      };
    }

    public tk2dTileMap DestroyWallAtPosition(int ix, int iy, bool deferRebuild = true)
    {
      if (this.data.cellData[ix][iy] == null)
        return (tk2dTileMap) null;
      if (this.data.cellData[ix][iy].type != CellType.WALL)
        return (tk2dTileMap) null;
      if (!this.data.cellData[ix][iy].breakable)
        return (tk2dTileMap) null;
      this.data.cellData[ix][iy].type = CellType.FLOOR;
      if (this.data.isSingleCellWall(ix, iy + 1))
        this.data[ix, iy + 1].type = CellType.FLOOR;
      if (this.data.isSingleCellWall(ix, iy - 1))
        this.data[ix, iy - 1].type = CellType.FLOOR;
      RoomHandler parentRoom = this.data.cellData[ix][iy].parentRoom;
      tk2dTileMap tk2dTileMap = parentRoom == null || !((UnityEngine.Object) parentRoom.OverrideTilemap != (UnityEngine.Object) null) ? this.m_tilemap : parentRoom.OverrideTilemap;
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -2; index2 < 4; ++index2)
        {
          CellData cellData = this.data.cellData[ix + index1][iy + index2];
          if (cellData != null)
          {
            cellData.hasBeenGenerated = false;
            if (cellData.parentRoom != null)
            {
              List<GameObject> gameObjectList = new List<GameObject>();
              for (int index3 = 0; index3 < cellData.parentRoom.hierarchyParent.childCount; ++index3)
              {
                Transform child = cellData.parentRoom.hierarchyParent.GetChild(index3);
                if (child.name.StartsWith("Chunk_"))
                  gameObjectList.Add(child.gameObject);
              }
              for (int index4 = gameObjectList.Count - 1; index4 >= 0; --index4)
                UnityEngine.Object.Destroy((UnityEngine.Object) gameObjectList[index4]);
            }
            this.assembler.ClearTileIndicesForCell(this, tk2dTileMap, cellData.position.x, cellData.position.y);
            this.assembler.BuildTileIndicesForCell(this, tk2dTileMap, cellData.position.x, cellData.position.y);
            cellData.HasCachedPhysicsTile = false;
            cellData.CachedPhysicsTile = (PhysicsEngine.Tile) null;
          }
        }
      }
      if (!deferRebuild)
        this.RebuildTilemap(tk2dTileMap);
      return tk2dTileMap;
    }

    public tk2dTileMap ConstructWallAtPosition(int ix, int iy, bool deferRebuild = true)
    {
      if (this.data.cellData[ix][iy].type == CellType.WALL)
        return (tk2dTileMap) null;
      this.data.cellData[ix][iy].type = CellType.WALL;
      RoomHandler parentRoom = this.data.cellData[ix][iy].parentRoom;
      tk2dTileMap tk2dTileMap = parentRoom == null || !((UnityEngine.Object) parentRoom.OverrideTilemap != (UnityEngine.Object) null) ? this.m_tilemap : parentRoom.OverrideTilemap;
      for (int index1 = -1; index1 < 2; ++index1)
      {
        for (int index2 = -2; index2 < 4; ++index2)
        {
          CellData cellData = this.data.cellData[ix + index1][iy + index2];
          if (cellData != null)
          {
            cellData.hasBeenGenerated = false;
            if (cellData.parentRoom != null)
            {
              List<GameObject> gameObjectList = new List<GameObject>();
              for (int index3 = 0; index3 < cellData.parentRoom.hierarchyParent.childCount; ++index3)
              {
                Transform child = cellData.parentRoom.hierarchyParent.GetChild(index3);
                if (child.name.StartsWith("Chunk_"))
                  gameObjectList.Add(child.gameObject);
              }
              for (int index4 = gameObjectList.Count - 1; index4 >= 0; --index4)
                UnityEngine.Object.Destroy((UnityEngine.Object) gameObjectList[index4]);
            }
            this.assembler.ClearTileIndicesForCell(this, tk2dTileMap, cellData.position.x, cellData.position.y);
            this.assembler.BuildTileIndicesForCell(this, tk2dTileMap, cellData.position.x, cellData.position.y);
            cellData.HasCachedPhysicsTile = false;
            cellData.CachedPhysicsTile = (PhysicsEngine.Tile) null;
          }
        }
      }
      if (!deferRebuild)
        this.RebuildTilemap(tk2dTileMap);
      return tk2dTileMap;
    }

    public void RebuildTilemap(tk2dTileMap targetTilemap)
    {
      RenderMeshBuilder.CurrentCellXOffset = Mathf.RoundToInt(targetTilemap.renderData.transform.position.x);
      RenderMeshBuilder.CurrentCellYOffset = Mathf.RoundToInt(targetTilemap.renderData.transform.position.y);
      targetTilemap.Build();
      targetTilemap.renderData.transform.position = new Vector3((float) RenderMeshBuilder.CurrentCellXOffset, (float) RenderMeshBuilder.CurrentCellYOffset, (float) RenderMeshBuilder.CurrentCellYOffset);
      RenderMeshBuilder.CurrentCellXOffset = 0;
      RenderMeshBuilder.CurrentCellYOffset = 0;
    }

    public void InformRoomCleared(bool rewardDropped, bool rewardIsChest)
    {
      if (rewardDropped)
      {
        if (rewardIsChest)
        {
          GameManager.Instance.PrimaryPlayer.AdditionalChestSpawnChance = 0.0f;
          BraveUtility.Log("Spawning a chest: flooring chest spawn chance.", Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);
        }
        else
        {
          GameManager.Instance.PrimaryPlayer.AdditionalChestSpawnChance = 0.0f;
          BraveUtility.Log("Spawning a single item: flooring chest spawn chance.", Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);
        }
      }
      else
      {
        float chestSystemIncrement = GameManager.Instance.RewardManager.CurrentRewardData.ChestSystem_Increment;
        if (PassiveItem.IsFlagSetForCharacter(GameManager.Instance.PrimaryPlayer, typeof (AmazingChestAheadItem)))
        {
          chestSystemIncrement *= AmazingChestAheadItem.ChestIncrementMultiplier;
          int count = 0;
          if (PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.DOUBLE_CHEST_FRIENDS, out count))
            chestSystemIncrement *= 1.25f;
        }
        float num = GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER ? chestSystemIncrement * GameManager.Instance.RewardManager.SinglePlayerPickupIncrementModifier : chestSystemIncrement * GameManager.Instance.RewardManager.CoopPickupIncrementModifier;
        UnityEngine.Debug.Log((object) $"LootSystem::{(object) GameManager.Instance.PrimaryPlayer.AdditionalChestSpawnChance} + {(object) num}");
        GameManager.Instance.PrimaryPlayer.AdditionalChestSpawnChance += num;
      }
    }

    public void FloorReached()
    {
      GameManager.Instance.platformInterface.SetPresence(this.DungeonFloorName.Replace("#", string.Empty));
      if (this.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_RATGEON, 1f);
      if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE)
        return;
      GlobalDungeonData.ValidTilesets tilesetId = this.tileIndices.tilesetId;
      switch (tilesetId)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_GUNGEON, 1f);
          break;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.NUMBER_ATTEMPTS, 1f);
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.RUNS_PLAYED_POST_FTA, 1f);
          break;
        case GlobalDungeonData.ValidTilesets.SEWERGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_SEWERS, 1f);
          break;
        case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_CATHEDRAL, 1f);
          break;
        default:
          if (tilesetId != GlobalDungeonData.ValidTilesets.MINEGEON)
          {
            if (tilesetId != GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            {
              if (tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON)
              {
                if (tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON)
                {
                  if (tilesetId != GlobalDungeonData.ValidTilesets.SPACEGEON)
                  {
                    if (tilesetId != GlobalDungeonData.ValidTilesets.PHOBOSGEON)
                    {
                      if (tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON)
                      {
                        if (tilesetId != GlobalDungeonData.ValidTilesets.OFFICEGEON)
                        {
                          if (tilesetId != GlobalDungeonData.ValidTilesets.BELLYGEON)
                          {
                            if (tilesetId != GlobalDungeonData.ValidTilesets.JUNGLEGEON)
                            {
                              if (tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
                              {
                                GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_RATGEON, 1f);
                                break;
                              }
                              break;
                            }
                            GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_JUNGLE, 1f);
                            break;
                          }
                          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_BELLY, 1f);
                          break;
                        }
                        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_NAKATOMI, 1f);
                        break;
                      }
                      GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_WEST, 1f);
                      break;
                    }
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_PHOBOS, 1f);
                    break;
                  }
                  GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_FUTURE, 1f);
                  break;
                }
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_BULLET_HELL, 1f);
                break;
              }
              GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_FORGE, 1f);
              break;
            }
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_CATACOMBS, 1f);
            break;
          }
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_REACHED_MINES, 1f);
          break;
      }
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE)
        GameStatsManager.Instance.isChump = this.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON;
      if (!GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_UNLOCK_COMPANION_SHRINE) && GameStatsManager.Instance.GetNumberOfCompanionsUnlocked() >= 5)
        GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_UNLOCK_COMPANION_SHRINE, true);
      if (ChallengeManager.CHALLENGE_MODE_ACTIVE && this.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)
        GameStatsManager.Instance.SetFlag(GungeonFlags.DAISUKE_CHALLENGE_HALFITEM_UNLOCK, true);
      if (!GameStatsManager.Instance.GetFlag(GungeonFlags.SECRET_BULLETMAN_SEEN_05))
      {
        float num = 0.04f;
        switch (this.tileIndices.tilesetId)
        {
          case GlobalDungeonData.ValidTilesets.GUNGEON:
          case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
            num = 0.08f;
            break;
          case GlobalDungeonData.ValidTilesets.MINEGEON:
            num = 0.12f;
            break;
          case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
            num = 0.16f;
            break;
          case GlobalDungeonData.ValidTilesets.FORGEGEON:
            num = 0.2f;
            break;
          case GlobalDungeonData.ValidTilesets.HELLGEON:
            num = 0.25f;
            break;
        }
        if (GameStatsManager.Instance.AnyPastBeaten() && (double) UnityEngine.Random.value < (double) num)
        {
          List<int> intList = Enumerable.Range(0, this.data.rooms.Count).ToList<int>().Shuffle<int>();
          int index = 0;
          while (index < intList.Count && (this.data.rooms[intList[index]].area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.NORMAL || !this.data.rooms[intList[index]].EverHadEnemies || !this.data.rooms[intList[index]].AddMysteriousBulletManToRoom()))
            ++index;
        }
      }
      int keybulletMenForFloor = MetaInjectionData.GetNumKeybulletMenForFloor(this.tileIndices.tilesetId);
      if (keybulletMenForFloor > 0)
      {
        List<RoomHandler> roomHandlerList = new List<RoomHandler>();
        for (int index1 = 0; index1 < keybulletMenForFloor; ++index1)
        {
          List<int> intList = Enumerable.Range(0, this.data.rooms.Count).ToList<int>().Shuffle<int>();
          bool flag = false;
          for (int index2 = 0; index2 < 2; ++index2)
          {
            for (int index3 = 0; index3 < intList.Count; ++index3)
            {
              RoomHandler room = this.data.rooms[intList[index3]];
              if ((!roomHandlerList.Contains(room) || (double) UnityEngine.Random.value <= 0.10000000149011612) && (index2 == 1 || room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.HUB) && room.EverHadEnemies && room.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
              {
                flag = true;
                room.AddSpecificEnemyToRoomProcedurally(GameManager.Instance.RewardManager.KeybulletsChances.EnemyGuid);
                roomHandlerList.Add(room);
                break;
              }
            }
            if (flag)
              break;
          }
        }
      }
      int bulletMenForFloor = MetaInjectionData.GetNumChanceBulletMenForFloor(this.tileIndices.tilesetId);
      if (bulletMenForFloor > 0)
      {
        List<RoomHandler> roomHandlerList = new List<RoomHandler>();
        for (int index4 = 0; index4 < bulletMenForFloor; ++index4)
        {
          List<int> intList = Enumerable.Range(0, this.data.rooms.Count).ToList<int>().Shuffle<int>();
          bool flag = false;
          for (int index5 = 0; index5 < 2; ++index5)
          {
            for (int index6 = 0; index6 < intList.Count; ++index6)
            {
              RoomHandler room = this.data.rooms[intList[index6]];
              if ((!roomHandlerList.Contains(room) || (double) UnityEngine.Random.value <= 0.10000000149011612) && (index5 == 1 || room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.HUB) && room.EverHadEnemies && room.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
              {
                flag = true;
                room.AddSpecificEnemyToRoomProcedurally(GameManager.Instance.RewardManager.ChanceBulletChances.EnemyGuid);
                roomHandlerList.Add(room);
                break;
              }
            }
            if (flag)
              break;
          }
        }
      }
      if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIME_PLAYED) > 18000.0 && (double) UnityEngine.Random.value < (double) GameManager.Instance.RewardManager.FacelessChancePerFloor)
      {
        List<int> intList = Enumerable.Range(0, this.data.rooms.Count).ToList<int>().Shuffle<int>();
        for (int index = 0; index < intList.Count; ++index)
        {
          RoomHandler room = this.data.rooms[intList[index]];
          if (room.EverHadEnemies && (room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.HUB || room.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.All) > 6))
          {
            AIActor toughestEnemy = room.GetToughestEnemy();
            if ((bool) (UnityEngine.Object) toughestEnemy)
              UnityEngine.Object.Destroy((UnityEngine.Object) toughestEnemy.gameObject);
            room.AddSpecificEnemyToRoomProcedurally(GameManager.Instance.RewardManager.FacelessCultistGuid);
            break;
          }
        }
      }
      this.HandleAGDInjection();
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if ((bool) (UnityEngine.Object) allPlayer)
        {
          allPlayer.HasReceivedNewGunThisFloor = false;
          allPlayer.HasTakenDamageThisFloor = false;
        }
      }
    }

    private void HandleAGDInjection()
    {
      List<AIActor> outList = new List<AIActor>();
      RunData runData = GameManager.Instance.RunData ?? new RunData();
      if (runData.AgdInjectionRunCounts == null || runData.AgdInjectionRunCounts.Length != GameManager.Instance.EnemyReplacementTiers.Count)
        runData.AgdInjectionRunCounts = new int[GameManager.Instance.EnemyReplacementTiers.Count];
      int[] injectionRunCounts = runData.AgdInjectionRunCounts;
      List<RoomHandler> list = new List<RoomHandler>();
      if (this.data != null && this.data.rooms != null)
        list.AddRange((IEnumerable<RoomHandler>) this.data.rooms);
      for (int index1 = 0; index1 < GameManager.Instance.EnemyReplacementTiers.Count; ++index1)
      {
        AGDEnemyReplacementTier enemyReplacementTier = GameManager.Instance.EnemyReplacementTiers[index1];
        int num = 0;
        if (enemyReplacementTier != null && (this.tileIndices == null || (this.tileIndices.tilesetId & enemyReplacementTier.TargetTileset) == this.tileIndices.tilesetId) && !enemyReplacementTier.ExcludeForPrereqs() && (enemyReplacementTier.MaxPerRun <= 0 || injectionRunCounts[index1] < enemyReplacementTier.MaxPerRun))
        {
          BraveUtility.RandomizeList<RoomHandler>(list);
          foreach (RoomHandler room in list)
          {
            if (room.EverHadEnemies && room.IsStandardRoom && !enemyReplacementTier.ExcludeRoomForColumns(this.data, room) && !enemyReplacementTier.ExcludeRoom(room))
            {
              room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref outList);
              if (!enemyReplacementTier.ExcludeRoomForEnemies(room, outList))
              {
                for (int index2 = 0; index2 < outList.Count; ++index2)
                {
                  AIActor enemy1 = outList[index2];
                  if ((bool) (UnityEngine.Object) enemy1 && (enemy1.AdditionalSimpleItemDrops == null || enemy1.AdditionalSimpleItemDrops.Count <= 0) && (!(bool) (UnityEngine.Object) enemy1.healthHaver || !enemy1.healthHaver.IsBoss) && (enemyReplacementTier.TargetAllSignatureEnemies && enemy1.IsSignatureEnemy || enemyReplacementTier.TargetAllNonSignatureEnemies && !enemy1.IsSignatureEnemy || enemyReplacementTier.TargetGuids != null && enemyReplacementTier.TargetGuids.Contains(enemy1.EnemyGuid)) && (double) UnityEngine.Random.value < (double) enemyReplacementTier.ChanceToReplace)
                  {
                    Vector2? nullable1 = new Vector2?();
                    if (enemyReplacementTier.RemoveAllOtherEnemies)
                    {
                      nullable1 = new Vector2?(room.area.Center);
                      for (int index3 = outList.Count - 1; index3 >= 0; --index3)
                      {
                        AIActor enemy2 = outList[index2];
                        if ((bool) (UnityEngine.Object) enemy2)
                        {
                          room.DeregisterEnemy(enemy2, true);
                          UnityEngine.Object.Destroy((UnityEngine.Object) enemy2.gameObject);
                        }
                      }
                    }
                    else
                    {
                      if ((bool) (UnityEngine.Object) enemy1.specRigidbody)
                      {
                        enemy1.specRigidbody.Initialize();
                        nullable1 = new Vector2?(enemy1.specRigidbody.UnitBottomLeft);
                      }
                      room.DeregisterEnemy(enemy1, true);
                      UnityEngine.Object.Destroy((UnityEngine.Object) enemy1.gameObject);
                    }
                    RoomHandler roomHandler = room;
                    string str = BraveUtility.RandomElement<string>(enemyReplacementTier.ReplacementGuids);
                    Vector2? nullable2 = nullable1;
                    string enemyGuid = str;
                    Vector2? goalPosition = nullable2;
                    roomHandler.AddSpecificEnemyToRoomProcedurally(enemyGuid, goalPosition: goalPosition);
                    ++num;
                    ++injectionRunCounts[index1];
                    if (enemyReplacementTier.MaxPerFloor > 0 && num >= enemyReplacementTier.MaxPerFloor || enemyReplacementTier.MaxPerRun > 0 && injectionRunCounts[index1] >= enemyReplacementTier.MaxPerRun || enemyReplacementTier.RemoveAllOtherEnemies)
                      break;
                  }
                }
                if (enemyReplacementTier.MaxPerFloor > 0)
                {
                  if (num >= enemyReplacementTier.MaxPerFloor)
                    break;
                }
                if (enemyReplacementTier.MaxPerRun > 0)
                {
                  if (injectionRunCounts[index1] >= enemyReplacementTier.MaxPerRun)
                    break;
                }
              }
            }
          }
        }
      }
      this.CullEnemiesForNewPlayers();
    }

    private void CullEnemiesForNewPlayers()
    {
      List<AIActor> outList = new List<AIActor>();
      float playerEnemyCullFactor = GameStatsManager.Instance.NewPlayerEnemyCullFactor;
      if ((double) playerEnemyCullFactor <= 0.0)
        return;
      foreach (RoomHandler room in this.data.rooms)
      {
        if (room.EverHadEnemies && room.IsStandardRoom && !room.IsGunslingKingChallengeRoom)
        {
          if (room.area.runtimePrototypeData != null && room.area.runtimePrototypeData.roomEvents != null)
          {
            bool flag = false;
            for (int index = 0; index < room.area.runtimePrototypeData.roomEvents.Count; ++index)
            {
              if (room.area.runtimePrototypeData.roomEvents[index].action == RoomEventTriggerAction.BECOME_TERRIFYING_AND_DARK)
                flag = true;
            }
            if (flag)
              continue;
          }
          room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref outList);
          for (int index = 0; index < outList.Count; ++index)
          {
            AIActor aiActor = outList[index];
            if ((bool) (UnityEngine.Object) aiActor && (aiActor.AdditionalSimpleItemDrops == null || aiActor.AdditionalSimpleItemDrops.Count <= 0) && aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy && aiActor.IsWorthShootingAt && (!(bool) (UnityEngine.Object) aiActor.healthHaver || !aiActor.healthHaver.IsBoss) && (double) UnityEngine.Random.value < (double) playerEnemyCullFactor)
              UnityEngine.Object.Destroy((UnityEngine.Object) aiActor.gameObject);
          }
        }
      }
    }

    private void PlaceFloorObjectInternal(
      DungeonPlaceableBehaviour prefabPlaceable,
      IntVector2 dimensions,
      Vector2 offset)
    {
      List<IntVector2> intVector2List = new List<IntVector2>();
      for (int index = 0; index < this.data.rooms.Count; ++index)
      {
        RoomHandler room = this.data.rooms[index];
        if (!room.area.IsProceduralRoom && room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.NORMAL && !(bool) (UnityEngine.Object) room.OptionalDoorTopDecorable && !room.area.prototypeRoom.UseCustomMusic)
        {
          for (int x = room.area.basePosition.x; x < room.area.basePosition.x + room.area.dimensions.x; ++x)
          {
            for (int y = room.area.basePosition.y; y < room.area.basePosition.y + room.area.dimensions.y; ++y)
            {
              if (this.ClearForFloorObject(dimensions.x, dimensions.y, x, y))
                intVector2List.Add(new IntVector2(x, y));
            }
          }
        }
      }
      if (intVector2List.Count <= 0)
        return;
      IntVector2 intVector2_1 = intVector2List[BraveRandom.GenerationRandomRange(0, intVector2List.Count)];
      RoomHandler absoluteRoom = intVector2_1.ToVector2().GetAbsoluteRoom();
      GameObject gObj = prefabPlaceable.InstantiateObject(absoluteRoom, intVector2_1 - absoluteRoom.area.basePosition);
      gObj.transform.position += offset.ToVector3ZUp();
      foreach (IPlayerInteractable interfacesInChild in gObj.GetInterfacesInChildren<IPlayerInteractable>())
        absoluteRoom.RegisterInteractable(interfacesInChild);
      for (int x = 0; x < dimensions.x; ++x)
      {
        for (int y = 0; y < dimensions.y; ++y)
        {
          IntVector2 intVector2_2 = intVector2_1 + new IntVector2(x, y);
          if (this.data.CheckInBoundsAndValid(intVector2_2))
            this.data[intVector2_2].cellVisualData.floorTileOverridden = true;
        }
      }
    }

    private void PlaceParadoxPortal()
    {
      if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE)
        return;
      this.PlaceFloorObjectInternal(((GameObject) BraveResources.Load("Global Prefabs/VFX_ParadoxPortal")).GetComponent<DungeonPlaceableBehaviour>(), new IntVector2(4, 4), new Vector2(2f, 2f));
    }

    private void PlaceRatGrate()
    {
      this.PlaceFloorObjectInternal(this.RatTrapdoor.GetComponent<DungeonPlaceableBehaviour>(), new IntVector2(4, 4), Vector2.zero);
    }

    private bool ClearForFloorObject(int dmx, int dmy, int bpx, int bpy)
    {
      int num = -1;
      for (int index1 = 0; index1 < dmx; ++index1)
      {
        for (int index2 = 0; index2 < dmy; ++index2)
        {
          IntVector2 intVector2 = new IntVector2(bpx + index1, bpy + index2);
          if (!this.data.CheckInBoundsAndValid(intVector2))
            return false;
          CellData cellData = this.data[intVector2];
          if (num == -1)
          {
            num = cellData.cellVisualData.roomVisualTypeIndex;
            if (num != 0 && num != 1)
              return false;
          }
          if (cellData.parentRoom == null || cellData.parentRoom.IsMaintenanceRoom() || cellData.type != CellType.FLOOR || cellData.isOccupied || !cellData.IsPassable || cellData.containsTrap || cellData.IsTrapZone || cellData.cellVisualData.roomVisualTypeIndex != num || cellData.HasPitNeighbor(this.data) || cellData.PreventRewardSpawn || cellData.cellVisualData.isPattern || cellData.cellVisualData.IsPhantomCarpet || cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Water || cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Carpet || cellData.cellVisualData.floorTileOverridden || cellData.doesDamage || cellData.cellVisualData.preventFloorStamping || cellData.cellVisualData.hasStampedPath || cellData.forceDisallowGoop)
            return false;
        }
      }
      return true;
    }

    public void PlaceWallMimics(RoomHandler debugRoom = null)
    {
      if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.RESOURCEFUL_RAT)
        return;
      int wallMimicsForFloor = MetaInjectionData.GetNumWallMimicsForFloor(this.tileIndices.tilesetId);
      if (wallMimicsForFloor <= 0)
        return;
      List<int> intList = Enumerable.Range(0, this.data.rooms.Count).ToList<int>().Shuffle<int>();
      if (debugRoom != null)
        intList = new List<int>((IEnumerable<int>) new int[1]
        {
          this.data.rooms.IndexOf(debugRoom)
        });
      List<Tuple<IntVector2, DungeonData.Direction>> list = new List<Tuple<IntVector2, DungeonData.Direction>>();
      int num1 = 0;
      List<AIActor> outList = new List<AIActor>();
      for (int index1 = 0; index1 < intList.Count && num1 < wallMimicsForFloor; ++index1)
      {
        RoomHandler room = this.data.rooms[intList[index1]];
        if (!room.IsShop && !room.IsMaintenanceRoom() && (!room.area.IsProceduralRoom || room.area.proceduralCells == null) && (room.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS || PlayerStats.GetTotalCurse() >= 5 && !BraveUtility.RandomBool()) && !room.GetRoomName().StartsWith("DraGunRoom"))
        {
          if (room.connectedRooms != null)
          {
            for (int index2 = 0; index2 < room.connectedRooms.Count; ++index2)
            {
              if (room.connectedRooms[index2] == null || room.connectedRooms[index2].area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
                ;
            }
          }
          if (debugRoom == null)
          {
            bool flag = false;
            room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref outList);
            for (int index3 = 0; index3 < outList.Count; ++index3)
            {
              AIActor aiActor = outList[index3];
              if ((bool) (UnityEngine.Object) aiActor && aiActor.EnemyGuid == GameManager.Instance.RewardManager.WallMimicChances.EnemyGuid)
              {
                flag = true;
                break;
              }
            }
            if (flag)
              continue;
          }
          list.Clear();
          for (int index4 = -1; index4 <= room.area.dimensions.x; ++index4)
          {
            for (int index5 = -1; index5 <= room.area.dimensions.y; ++index5)
            {
              int x1 = room.area.basePosition.x + index4;
              int y1 = room.area.basePosition.y + index5;
              if (this.data.isWall(x1, y1))
              {
                int num2 = 0;
                if (this.data.isWall(x1 - 1, y1) && this.data.isWall(x1 - 1, y1 - 1) && this.data.isWall(x1 - 1, y1 - 2) && this.data.isWall(x1, y1) && this.data.isWall(x1, y1 - 1) && this.data.isWall(x1, y1 - 2) && this.data.isPlainEmptyCell(x1, y1 + 1) && this.data.isWall(x1 + 1, y1) && this.data.isWall(x1 + 1, y1 - 1) && this.data.isWall(x1 + 1, y1 - 2) && this.data.isPlainEmptyCell(x1 + 1, y1 + 1) && this.data.isWall(x1 + 2, y1) && this.data.isWall(x1 + 2, y1 - 1) && this.data.isWall(x1 + 2, y1 - 2))
                {
                  list.Add(Tuple.Create<IntVector2, DungeonData.Direction>(new IntVector2(x1, y1), DungeonData.Direction.NORTH));
                  ++num2;
                }
                else if (this.data.isWall(x1 - 1, y1) && this.data.isWall(x1 - 1, y1 + 1) && this.data.isWall(x1 - 1, y1 + 2) && this.data.isWall(x1, y1) && this.data.isWall(x1, y1 + 1) && this.data.isWall(x1, y1 + 2) && this.data.isPlainEmptyCell(x1, y1 - 1) && this.data.isWall(x1 + 1, y1) && this.data.isWall(x1 + 1, y1 + 1) && this.data.isWall(x1 + 1, y1 + 2) && this.data.isPlainEmptyCell(x1 + 1, y1 - 1) && this.data.isWall(x1 + 2, y1) && this.data.isWall(x1 + 2, y1 + 1) && this.data.isWall(x1 + 2, y1 + 2))
                {
                  list.Add(Tuple.Create<IntVector2, DungeonData.Direction>(new IntVector2(x1, y1), DungeonData.Direction.SOUTH));
                  ++num2;
                }
                else if (this.data.isWall(x1, y1 + 2) && this.data.isWall(x1, y1 + 1) && this.data.isWall(x1, y1 - 1) && this.data.isWall(x1, y1 - 2) && this.data.isWall(x1 - 1, y1) && this.data.isPlainEmptyCell(x1 + 1, y1) && this.data.isPlainEmptyCell(x1 + 1, y1 - 1))
                {
                  list.Add(Tuple.Create<IntVector2, DungeonData.Direction>(new IntVector2(x1, y1), DungeonData.Direction.EAST));
                  ++num2;
                }
                else if (this.data.isWall(x1, y1 + 2) && this.data.isWall(x1, y1 + 1) && this.data.isWall(x1, y1 - 1) && this.data.isWall(x1, y1 - 2) && this.data.isWall(x1 + 1, y1) && this.data.isPlainEmptyCell(x1 - 1, y1) && this.data.isPlainEmptyCell(x1 - 1, y1 - 1))
                {
                  list.Add(Tuple.Create<IntVector2, DungeonData.Direction>(new IntVector2(x1 - 1, y1), DungeonData.Direction.WEST));
                  ++num2;
                }
                if (num2 > 0)
                {
                  bool flag = true;
                  for (int index6 = -5; index6 <= 5 && flag; ++index6)
                  {
                    for (int index7 = -5; index7 <= 5 && flag; ++index7)
                    {
                      int x2 = x1 + index6;
                      int y2 = y1 + index7;
                      if (this.data.CheckInBoundsAndValid(x2, y2))
                      {
                        CellData cellData = this.data[x2, y2];
                        if (cellData != null && (cellData.type == CellType.PIT || cellData.diagonalWallType != DiagonalWallType.NONE))
                          flag = false;
                      }
                    }
                  }
                  if (!flag)
                  {
                    for (; num2 > 0; --num2)
                      list.RemoveAt(list.Count - 1);
                  }
                }
              }
            }
          }
          if (debugRoom == null && list.Count > 0)
          {
            Tuple<IntVector2, DungeonData.Direction> tuple = BraveUtility.RandomElement<Tuple<IntVector2, DungeonData.Direction>>(list);
            IntVector2 first = tuple.First;
            DungeonData.Direction second = tuple.Second;
            if (second != DungeonData.Direction.WEST)
              room.RuntimeStampCellComplex(first.x, first.y, CellType.FLOOR, DiagonalWallType.NONE);
            if (second != DungeonData.Direction.EAST)
              room.RuntimeStampCellComplex(first.x + 1, first.y, CellType.FLOOR, DiagonalWallType.NONE);
            AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(GameManager.Instance.RewardManager.WallMimicChances.EnemyGuid), first, room, true);
            ++num1;
          }
        }
      }
      if (num1 <= 0)
        return;
      PhysicsEngine.Instance.ClearAllCachedTiles();
    }

    public void FloorCleared()
    {
      if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.NONE)
        return;
      GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
      switch (tilesetId)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_GUNGEON, 1f);
          break;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_CASTLE, 1f);
          break;
        case GlobalDungeonData.ValidTilesets.SEWERGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_SEWERS, 1f);
          break;
        case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_CATHEDRAL, 1f);
          break;
        default:
          if (tilesetId != GlobalDungeonData.ValidTilesets.MINEGEON)
          {
            if (tilesetId != GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            {
              if (tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON)
              {
                if (tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON)
                {
                  if (tilesetId != GlobalDungeonData.ValidTilesets.SPACEGEON)
                  {
                    if (tilesetId != GlobalDungeonData.ValidTilesets.PHOBOSGEON)
                    {
                      if (tilesetId != GlobalDungeonData.ValidTilesets.WESTGEON)
                      {
                        if (tilesetId != GlobalDungeonData.ValidTilesets.OFFICEGEON)
                        {
                          if (tilesetId != GlobalDungeonData.ValidTilesets.BELLYGEON)
                          {
                            if (tilesetId == GlobalDungeonData.ValidTilesets.JUNGLEGEON)
                            {
                              GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_JUNGLE, 1f);
                              break;
                            }
                            break;
                          }
                          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_BELLY, 1f);
                          break;
                        }
                        GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_NAKATOMI, 1f);
                        break;
                      }
                      GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_WEST, 1f);
                      break;
                    }
                    GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_PHOBOS, 1f);
                    break;
                  }
                  GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_FUTURE, 1f);
                  break;
                }
                GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_BULLET_HELL, 1f);
                break;
              }
              GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_FORGE, 1f);
              break;
            }
            GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_CATACOMBS, 1f);
            break;
          }
          GameStatsManager.Instance.RegisterStatChange(TrackedStats.TIMES_CLEARED_MINES, 1f);
          break;
      }
      if (ChallengeManager.CHALLENGE_MODE_ACTIVE && this.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
      {
        GameStatsManager.Instance.SetFlag(GungeonFlags.DAISUKE_CHALLENGE_COMPLETE, true);
        GameStatsManager.Instance.SetFlag(GungeonFlags.DAISUKE_CHALLENGE_ITEM_UNLOCK, true);
        if (ChallengeManager.Instance.ChallengeMode == ChallengeModeType.ChallengeMegaMode)
          GameStatsManager.Instance.SetFlag(GungeonFlags.DAISUKE_MEGA_CHALLENGE_COMPLETE, true);
      }
      if (GameStatsManager.Instance.GetFlag(GungeonFlags.SHOP_HAS_MET_BEETLE))
      {
        switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
        {
          case GlobalDungeonData.ValidTilesets.GUNGEON:
            GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 0.4f;
            break;
          case GlobalDungeonData.ValidTilesets.CASTLEGEON:
            GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 0.2f;
            break;
          case GlobalDungeonData.ValidTilesets.MINEGEON:
            GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 0.6f;
            break;
          case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
            GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 0.8f;
            break;
          case GlobalDungeonData.ValidTilesets.FORGEGEON:
            GameStatsManager.Instance.AccumulatedBeetleMerchantChance = 1f;
            break;
        }
      }
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON)
        return;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if ((bool) (UnityEngine.Object) allPlayer)
        {
          if (!allPlayer.HasFiredNonStartingGun)
            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_STARTING_GUN, true);
          if (allPlayer.CharacterUsesRandomGuns)
            GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_GAME_WITH_ENCHANTED_GUN);
          if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
            GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_GAME_WITH_CHALLENGE_MODE);
        }
      }
    }

    private void GeneratePlayerIfNecessary(MidGameSaveData midgameSave)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (this.ForceRegenerationOfCharacters)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
          flag1 = GameManager.Instance.PrimaryPlayer.IsUsingAlternateCostume;
        if ((bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
          flag2 = GameManager.Instance.PrimaryPlayer.IsTemporaryEeveeForUnlock;
        GameManager.Instance.ClearPlayers();
      }
      PlayerController playerController = GameManager.Instance.PrimaryPlayer;
      if (!(bool) (UnityEngine.Object) playerController || this.ForceRegenerationOfCharacters)
      {
        if (midgameSave != null)
          GameManager.PlayerPrefabForNewGame = midgameSave.GetPlayerOnePrefab();
        if ((UnityEngine.Object) GameManager.PlayerPrefabForNewGame == (UnityEngine.Object) null)
        {
          if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
            return;
          BraveUtility.Log("Dungeon generation complete with no Player! Creating placeholder...", Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.defaultPlayerPrefab, Vector3.zero, Quaternion.identity);
          gameObject.SetActive(true);
          playerController = gameObject.GetComponent<PlayerController>();
          if (playerController is PlayerSpaceshipController)
          {
            playerController.IsUsingAlternateCostume = flag1;
            playerController.SetTemporaryEeveeSafeNoShader(flag2);
          }
        }
        else
        {
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GameManager.PlayerPrefabForNewGame, Vector3.zero, Quaternion.identity);
          GameManager.PlayerPrefabForNewGame = (GameObject) null;
          gameObject.SetActive(true);
          playerController = gameObject.GetComponent<PlayerController>();
        }
        if (GameManager.ForceQuickRestartAlternateCostumeP1)
        {
          playerController.SwapToAlternateCostume();
          GameManager.ForceQuickRestartAlternateCostumeP1 = false;
        }
        if (GameManager.ForceQuickRestartAlternateGunP1)
        {
          playerController.UsingAlternateStartingGuns = true;
          GameManager.ForceQuickRestartAlternateGunP1 = false;
        }
        GameManager.Instance.RefreshAllPlayers();
        if (this.StripPlayerOnArrival)
        {
          playerController.startingGunIds = new List<int>();
          playerController.startingAlternateGunIds = new List<int>();
          playerController.startingActiveItemIds.Clear();
          playerController.startingPassiveItemIds.Clear();
        }
      }
      else if (this.StripPlayerOnArrival)
      {
        playerController.inventory.DestroyAllGuns();
        playerController.RemoveAllActiveItems();
        playerController.RemoveAllPassiveItems();
      }
      playerController.PlayerIDX = 0;
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        if (GameManager.Instance.AllPlayers.Length < 2 || this.ForceRegenerationOfCharacters)
        {
          GameObject original = !((UnityEngine.Object) GameManager.CoopPlayerPrefabForNewGame == (UnityEngine.Object) null) ? GameManager.CoopPlayerPrefabForNewGame : ResourceCache.Acquire("PlayerCoopCultist") as GameObject;
          if (this.ForceRegenerationOfCharacters)
            original = ResourceCache.Acquire("PlayerCoopCultist") as GameObject;
          if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST && GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Pilot && !GameManager.IsCoopPast)
            original = BraveResources.Load("PlayerCoopShip") as GameObject;
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, Vector3.zero, Quaternion.identity);
          GameManager.CoopPlayerPrefabForNewGame = (GameObject) null;
          gameObject.SetActive(true);
          PlayerController component = gameObject.GetComponent<PlayerController>();
          component.ActorName = "Player ID 1";
          component.PlayerIDX = 1;
          if (GameManager.ForceQuickRestartAlternateCostumeP2)
          {
            component.SwapToAlternateCostume();
            GameManager.ForceQuickRestartAlternateCostumeP2 = false;
          }
          if (GameManager.ForceQuickRestartAlternateGunP2)
          {
            component.UsingAlternateStartingGuns = true;
            GameManager.ForceQuickRestartAlternateGunP2 = false;
          }
          if (this.StripPlayerOnArrival)
          {
            component.startingGunIds = new List<int>();
            component.startingAlternateGunIds = new List<int>();
            component.startingActiveItemIds.Clear();
            component.startingPassiveItemIds.Clear();
          }
          GameManager.Instance.RefreshAllPlayers();
        }
        else if (this.StripPlayerOnArrival)
        {
          GameManager.Instance.SecondaryPlayer.inventory.DestroyAllGuns();
          GameManager.Instance.SecondaryPlayer.RemoveAllActiveItems();
          GameManager.Instance.SecondaryPlayer.RemoveAllPassiveItems();
        }
      }
      if (!GameManager.Instance.InTutorial && (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.PrimaryPlayer.characterIdentity != PlayableCharacters.Convict))
        return;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if (GameManager.Instance.AllPlayers[index].healthHaver.IsAlive)
          GameManager.Instance.AllPlayers[index].sprite.gameObject.SetLayerRecursively(LayerMask.NameToLayer("ShadowCaster"));
      }
    }

    public void DarkSoulsReset(
      PlayerController targetPlayer,
      bool dropItems = true,
      int cursedHealthMaximum = -1)
    {
      this.StartCoroutine(this.HandleDarkSoulsReset_CR(targetPlayer, dropItems, cursedHealthMaximum));
    }

    [DebuggerHidden]
    private IEnumerator HandleDarkSoulsReset_CR(
      PlayerController p,
      bool dropItems,
      int cursedHealthMaximum)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Dungeon__HandleDarkSoulsReset_CRc__Iterator5()
      {
        dropItems = dropItems,
        p = p,
        cursedHealthMaximum = cursedHealthMaximum,
        _this = this
      };
    }

    private void PlacePlayerInRoom(tk2dTileMap map, RoomHandler startRoom)
    {
      PlayerController[] allPlayers = GameManager.Instance.AllPlayers;
      if (allPlayers.Length == 0)
        return;
      int num = allPlayers.Length >= 2 ? allPlayers.Length : 1;
      for (int index = 0; index < num; ++index)
      {
        PlayerController player = allPlayers.Length >= 2 ? allPlayers[index] : GameManager.Instance.PrimaryPlayer;
        EntranceController objectOfType1 = UnityEngine.Object.FindObjectOfType<EntranceController>();
        ElevatorArrivalController objectOfType2 = UnityEngine.Object.FindObjectOfType<ElevatorArrivalController>();
        Vector2 vector = Vector2.zero;
        float invisibleDelay1 = 0.25f;
        if (GameManager.IsReturningToFoyerWithPlayer)
        {
          vector = GameObject.Find("ReturnToFoyerPoint").transform.position.XY();
          vector += Vector2.right * (float) index;
          player.transform.position = vector.ToVector3ZUp(-0.1f);
          player.Reinitialize();
        }
        else
        {
          if ((UnityEngine.Object) objectOfType2 != (UnityEngine.Object) null)
          {
            vector = objectOfType2.spawnTransform.position.XY();
            float initialDelay = 1f;
            objectOfType2.DoArrival(player, initialDelay);
            invisibleDelay1 = initialDelay + 0.4f;
          }
          else
          {
            if ((UnityEngine.Object) objectOfType1 != (UnityEngine.Object) null)
            {
              vector = objectOfType1.spawnTransform.position.XY();
              vector += Vector2.right * (float) index;
              player.transform.position = new Vector3((float) ((double) map.transform.position.x + (double) vector.x - 0.5), map.transform.position.y + vector.y, -0.1f);
              player.Reinitialize();
              float invisibleDelay2 = invisibleDelay1 + 0.4f;
              player.DoSpinfallSpawn(invisibleDelay2);
              continue;
            }
            if (index == 1 && (UnityEngine.Object) GameObject.Find("SecondaryPlayerSpawnPoint") != (UnityEngine.Object) null)
            {
              vector = GameObject.Find("SecondaryPlayerSpawnPoint").transform.position.XY();
              vector += Vector2.right * (float) index;
              player.transform.position = vector.ToVector3ZUp(-0.1f);
              player.Reinitialize();
              continue;
            }
            if ((UnityEngine.Object) GameObject.Find("PlayerSpawnPoint") != (UnityEngine.Object) null)
            {
              vector = GameObject.Find("PlayerSpawnPoint").transform.position.XY();
              vector += Vector2.right * (float) index;
              player.transform.position = vector.ToVector3ZUp(-0.1f);
              player.Reinitialize();
              continue;
            }
            vector = startRoom.GetCenterCell().ToVector2();
            if (this.data[vector.ToIntVector2()].type == CellType.WALL || this.data[vector.ToIntVector2()].type == CellType.PIT)
              vector = startRoom.Epicenter.ToVector2();
            vector += Vector2.right * (float) index;
          }
          Vector3 vector3 = new Vector3((float) ((double) map.transform.position.x + (double) vector.x + 0.5), (float) ((double) map.transform.position.y + (double) vector.y + 0.5), -0.1f);
          player.transform.position = vector3;
          player.Reinitialize();
          player.DoInitialFallSpawn(invisibleDelay1);
        }
      }
      GameManager.IsReturningToFoyerWithPlayer = false;
      GameManager.Instance.MainCameraController.ForceToPlayerPosition(GameManager.Instance.PrimaryPlayer);
    }

    public bool CellExists(IntVector2 pos)
    {
      return pos.x >= 0 && pos.x < this.Width && pos.y >= 0 && pos.y < this.Height;
    }

    public bool CellExists(int x, int y) => x >= 0 && x < this.Width && y >= 0 && y < this.Height;

    public bool CellExists(Vector2 pos)
    {
      int x = (int) pos.x;
      int y = (int) pos.y;
      return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
    }

    public bool CellIsNearPit(Vector3 position)
    {
      CellData cellData = this.data[position.IntXY(VectorConversions.Floor)];
      if (cellData == null)
        return false;
      return cellData.type == CellType.PIT || cellData.HasPitNeighbor(this.data);
    }

    public bool CellIsPit(Vector3 position)
    {
      return this.data[position.IntXY(VectorConversions.Floor)].type == CellType.PIT;
    }

    public bool CellSupportsFalling(Vector3 position)
    {
      IntVector2 intVector2 = position.IntXY(VectorConversions.Floor);
      if (!this.data.CheckInBounds(intVector2))
        return false;
      CellData cellData = this.data[intVector2];
      return cellData != null && cellData.type == CellType.PIT && !cellData.fallingPrevented;
    }

    public List<SpeculativeRigidbody> GetPlatformsAt(Vector3 position)
    {
      return this.data[position.IntXY(VectorConversions.Floor)].platforms;
    }

    public bool IsPixelOnPlatform(Vector3 position, out SpeculativeRigidbody platform)
    {
      return this.IsPixelOnPlatform(PhysicsEngine.UnitToPixel(position.XY()), out platform);
    }

    public bool IsPixelOnPlatform(IntVector2 pixel, out SpeculativeRigidbody platform)
    {
      platform = (SpeculativeRigidbody) null;
      CellData cellData = this.data[PhysicsEngine.PixelToUnitMidpoint(pixel).ToIntVector2(VectorConversions.Floor)];
      if (cellData.platforms != null)
      {
        for (int index = 0; index < cellData.platforms.Count; ++index)
        {
          if (cellData.platforms[index].PrimaryPixelCollider.ContainsPixel(pixel))
          {
            platform = cellData.platforms[index];
            return true;
          }
        }
      }
      return false;
    }

    public bool PositionInCustomPitSRB(Vector3 position)
    {
      if (DebrisObject.SRB_Pits != null && DebrisObject.SRB_Pits.Count > 0)
      {
        for (int index = 0; index < DebrisObject.SRB_Pits.Count; ++index)
        {
          if (DebrisObject.SRB_Pits[index].ContainsPoint(position.XY(), collideWithTriggers: true))
            return true;
        }
      }
      return false;
    }

    public bool ShouldReallyFall(Vector3 position)
    {
      bool flag = !this.CellSupportsFalling(position);
      if (this.PositionInCustomPitSRB(position))
        flag = false;
      return !flag && !this.IsPixelOnPlatform(position, out SpeculativeRigidbody _);
    }

    public void DoSplashDustupAtPosition(Vector2 bottomCenter)
    {
      DustUpVFX dungeonDustups = this.dungeonDustups;
      Color clear = Color.clear;
      GameObject gameObject = SpawnManager.SpawnVFX(dungeonDustups.waterDustup, (Vector3) bottomCenter, Quaternion.identity);
      if ((bool) (UnityEngine.Object) gameObject)
      {
        Renderer component = gameObject.GetComponent<Renderer>();
        if ((bool) (UnityEngine.Object) component)
        {
          gameObject.GetComponent<tk2dBaseSprite>().OverrideMaterialMode = tk2dBaseSprite.SpriteMaterialOverrideMode.OVERRIDE_MATERIAL_SIMPLE;
          component.material.SetColor("_OverrideColor", clear);
        }
      }
      if (!((UnityEngine.Object) dungeonDustups.additionalWaterDustup != (UnityEngine.Object) null))
        return;
      SpawnManager.SpawnVFX(dungeonDustups.additionalWaterDustup, (Vector3) bottomCenter, Quaternion.identity, true);
    }

    public IntVector2 RandomCellInRandomRoom()
    {
      return this.data.rooms[UnityEngine.Random.Range(0, this.data.rooms.Count)].GetRandomAvailableCellDumb();
    }

    public RoomHandler GetRoomFromPosition(IntVector2 pos)
    {
      return this.data.GetAbsoluteRoomFromPosition(pos);
    }

    public CellVisualData.CellFloorType GetFloorTypeFromPosition(Vector2 pos)
    {
      for (int index = 0; index < StaticReferenceManager.AllGoops.Count; ++index)
      {
        if (StaticReferenceManager.AllGoops[index].IsPositionInGoop(pos))
          return StaticReferenceManager.AllGoops[index].IsPositionFrozen(pos) ? CellVisualData.CellFloorType.Ice : CellVisualData.CellFloorType.Water;
      }
      return this.data.GetFloorTypeFromPosition(pos.ToIntVector2(VectorConversions.Floor));
    }

    public IntVector2 RandomCellInArea(CellArea ca)
    {
      int num1 = UnityEngine.Random.Range(0, ca.dimensions.x);
      int num2 = UnityEngine.Random.Range(0, ca.dimensions.y);
      return new IntVector2(ca.basePosition.x + num1, ca.basePosition.y + num2);
    }

    public bool AllRoomsVisited => this.m_allRoomsVisited;

    public void NotifyAllRoomsVisited()
    {
      if (this.m_allRoomsVisited)
        return;
      this.m_allRoomsVisited = true;
      if (this.OnAllRoomsVisited == null)
        return;
      this.OnAllRoomsVisited();
    }

    public TertiaryBossRewardSet GetTertiaryRewardSet()
    {
      List<TertiaryBossRewardSet> tertiaryBossRewardSetList = !this.UsesOverrideTertiaryBossSets || this.OverrideTertiaryRewardSets.Count <= 0 ? GameManager.Instance.RewardManager.CurrentRewardData.TertiaryBossRewardSets : this.OverrideTertiaryRewardSets;
      float num1 = 0.0f;
      for (int index = 0; index < tertiaryBossRewardSetList.Count; ++index)
        num1 += tertiaryBossRewardSetList[index].weight;
      float num2 = UnityEngine.Random.value * num1;
      float num3 = 0.0f;
      for (int index = 0; index < tertiaryBossRewardSetList.Count; ++index)
      {
        num3 += tertiaryBossRewardSetList[index].weight;
        if ((double) num3 >= (double) num2)
          return tertiaryBossRewardSetList[index];
      }
      return tertiaryBossRewardSetList[tertiaryBossRewardSetList.Count - 1];
    }

    private void Update()
    {
      if (!this.m_ambientVFXProcessingActive)
      {
        this.StartCoroutine(this.HandleAmbientPitVFX());
        if (this.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON)
          this.StartCoroutine(this.HandleAmbientChannelVFX());
      }
      if (!this.m_musicIsPlaying && (double) UnityEngine.Time.timeScale > 0.0)
      {
        if (Foyer.DoMainMenu && SceneManager.GetSceneByName("tt_foyer").isLoaded)
          this.m_musicIsPlaying = true;
        else if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
        {
          this.m_musicIsPlaying = true;
          GameManager.Instance.DungeonMusicController.ResetForNewFloor(this);
        }
      }
      if ((double) this.ExplosionBulletDeletionMultiplier < 1.0)
      {
        if ((double) this.ExplosionBulletDeletionMultiplier <= 0.0)
          this.IsExplosionBulletDeletionRecovering = true;
        this.ExplosionBulletDeletionMultiplier = Mathf.Clamp01(this.ExplosionBulletDeletionMultiplier + BraveTime.DeltaTime / 3f);
      }
      else
        this.IsExplosionBulletDeletionRecovering = false;
    }

    public float GetNewPlayerSpeedMultiplier()
    {
      if ((bool) (UnityEngine.Object) GameManager.Instance.Dungeon && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.CASTLEGEON)
        return 1f;
      if ((double) this.m_newPlayerMultiplier > 0.0)
        return this.m_newPlayerMultiplier;
      this.m_newPlayerMultiplier = 1f - Mathf.Clamp01((float) (0.20000000298023224 - 0.02500000037252903 * ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_ATTEMPTS) - 1.0)));
      return this.m_newPlayerMultiplier;
    }

    public RoomHandler RuntimeDuplicateChunk(
      IntVector2 basePosition,
      IntVector2 dimensions,
      int tilemapExpansion,
      RoomHandler sourceRoom = null,
      bool ignoreOtherRoomCells = false)
    {
      int y1 = tilemapExpansion + 3;
      int num = tilemapExpansion;
      IntVector2 p1 = new IntVector2(this.data.Width + y1, y1);
      CellData[][] cellDataArray = BraveUtility.MultidimensionalArrayResize<CellData>(this.data.cellData, this.data.Width, this.data.Height, this.data.Width + y1 * 2 + dimensions.x, Mathf.Max(this.data.Height, dimensions.y + y1 * 2));
      CellArea a = new CellArea(p1, dimensions);
      a.IsProceduralRoom = true;
      this.data.cellData = cellDataArray;
      this.data.ClearCachedCellData();
      RoomHandler roomHandler = new RoomHandler(a);
      GameObject gameObject = GameObject.Find("_Rooms");
      Transform transform = new GameObject("Room_ChunkDuplicate").transform;
      transform.parent = gameObject.transform;
      roomHandler.hierarchyParent = transform;
      for (int x = -y1; x < dimensions.x + y1; ++x)
      {
        for (int y2 = -y1; y2 < dimensions.y + y1; ++y2)
        {
          IntVector2 intVector2 = basePosition + new IntVector2(x, y2);
          IntVector2 p2 = new IntVector2(x, y2) + p1;
          CellData sourceCell = !this.data.CheckInBoundsAndValid(intVector2) ? (CellData) null : this.data[intVector2];
          CellData targetCell = new CellData(p2);
          if (sourceCell != null && sourceRoom != null && sourceCell.nearestRoom != sourceRoom)
          {
            targetCell.cellVisualData.roomVisualTypeIndex = sourceRoom.RoomVisualSubtype;
            sourceCell = (CellData) null;
          }
          if (sourceCell != null && sourceCell.isExitCell && ignoreOtherRoomCells)
          {
            targetCell.cellVisualData.roomVisualTypeIndex = sourceRoom.RoomVisualSubtype;
            sourceCell = (CellData) null;
          }
          targetCell.positionInTilemap = targetCell.positionInTilemap - p1 + new IntVector2(num, num);
          targetCell.parentArea = a;
          targetCell.parentRoom = roomHandler;
          targetCell.nearestRoom = roomHandler;
          targetCell.occlusionData.overrideOcclusion = true;
          cellDataArray[p2.x][p2.y] = targetCell;
          BraveUtility.DrawDebugSquare(p2.ToVector2(), Color.yellow, 1000f);
          CellType type = sourceCell == null ? CellType.WALL : sourceCell.type;
          roomHandler.RuntimeStampCellComplex(p2.x, p2.y, type, DiagonalWallType.NONE);
          if (sourceCell != null)
          {
            targetCell.distanceFromNearestRoom = sourceCell.distanceFromNearestRoom;
            targetCell.cellVisualData.CopyFrom(sourceCell.cellVisualData);
            if (sourceCell.cellVisualData.containsLight)
              this.data.ReplicateLighting(sourceCell, targetCell);
          }
        }
      }
      this.data.rooms.Add(roomHandler);
      tk2dTileMap component = ((GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap"))).GetComponent<tk2dTileMap>();
      component.Editor__SpriteCollection = this.tileIndices.dungeonCollection;
      TK2DDungeonAssembler.RuntimeResizeTileMap(component, dimensions.x + num * 2, dimensions.y + num * 2, this.m_tilemap.partitionSizeX, dimensions.y + num * 2);
      for (int x = -num; x < dimensions.x + num; ++x)
      {
        for (int y3 = -num; y3 < dimensions.y + num; ++y3)
        {
          IntVector2 intVector2_1 = basePosition + new IntVector2(x, y3);
          IntVector2 intVector2_2 = new IntVector2(x, y3) + p1;
          bool flag1 = false;
          CellData cellData = !this.data.CheckInBoundsAndValid(intVector2_1) ? (CellData) null : this.data[intVector2_1];
          if (ignoreOtherRoomCells && cellData != null)
          {
            bool flag2 = cellData.isExitCell;
            if (!flag2 && sourceRoom != null && cellData.parentRoom != sourceRoom)
              flag2 = true;
            if (!flag2 && cellData.IsAnyFaceWall() && this.data.CheckInBoundsAndValid(cellData.position + new IntVector2(0, -2)) && this.data[cellData.position + new IntVector2(0, -2)].isExitCell)
              flag2 = true;
            if (!flag2 && cellData.type == CellType.WALL && this.data.CheckInBoundsAndValid(cellData.position + new IntVector2(0, -3)) && this.data[cellData.position + new IntVector2(0, -3)].isExitCell)
              flag2 = true;
            if (!flag2 && cellData.type == CellType.FLOOR && this.data.CheckInBoundsAndValid(cellData.position + new IntVector2(0, -1)) && (this.data[cellData.position + new IntVector2(0, -1)].isExitCell || this.data[cellData.position + new IntVector2(0, -1)].GetExitNeighbor() != null))
              flag2 = true;
            if (!flag2 && (cellData.IsAnyFaceWall() || cellData.type == CellType.WALL) && cellData.GetExitNeighbor() != null)
              flag2 = true;
            if (flag2)
            {
              BraveUtility.DrawDebugSquare(intVector2_2.ToVector2() + new Vector2(0.3f, 0.3f), intVector2_2.ToVector2() + new Vector2(0.7f, 0.7f), Color.cyan, 1000f);
              this.assembler.BuildTileIndicesForCell(this, component, p1.x + x, p1.y + y3);
              flag1 = true;
            }
          }
          if (!flag1 && intVector2_1.x >= 0 && intVector2_1.y >= 0)
          {
            for (int index = 0; index < component.Layers.Length; ++index)
            {
              int tile = this.MainTilemap.Layers[index].GetTile(intVector2_1.x, intVector2_1.y);
              component.Layers[index].SetTile(x + num, y3 + num, tile);
            }
          }
        }
      }
      RenderMeshBuilder.CurrentCellXOffset = p1.x - num;
      RenderMeshBuilder.CurrentCellYOffset = p1.y - num;
      component.Build();
      RenderMeshBuilder.CurrentCellXOffset = 0;
      RenderMeshBuilder.CurrentCellYOffset = 0;
      component.renderData.transform.position = new Vector3((float) (p1.x - num), (float) (p1.y - num), (float) (p1.y - num));
      roomHandler.OverrideTilemap = component;
      roomHandler.PostGenerationCleanup();
      DeadlyDeadlyGoopManager.ReinitializeData();
      return roomHandler;
    }

    private void ConnectClusteredRuntimeRooms(
      RoomHandler first,
      RoomHandler second,
      PrototypeDungeonRoom firstPrototype,
      PrototypeDungeonRoom secondPrototype,
      int firstRoomExitIndex,
      int secondRoomExitIndex)
    {
      first.area.instanceUsedExits.Add(firstPrototype.exitData.exits[firstRoomExitIndex]);
      RuntimeRoomExitData runtimeRoomExitData1 = new RuntimeRoomExitData(firstPrototype.exitData.exits[firstRoomExitIndex]);
      first.area.exitToLocalDataMap.Add(firstPrototype.exitData.exits[firstRoomExitIndex], runtimeRoomExitData1);
      second.area.instanceUsedExits.Add(secondPrototype.exitData.exits[secondRoomExitIndex]);
      RuntimeRoomExitData runtimeRoomExitData2 = new RuntimeRoomExitData(secondPrototype.exitData.exits[secondRoomExitIndex]);
      second.area.exitToLocalDataMap.Add(secondPrototype.exitData.exits[secondRoomExitIndex], runtimeRoomExitData2);
      first.connectedRooms.Add(second);
      first.connectedRoomsByExit.Add(firstPrototype.exitData.exits[firstRoomExitIndex], second);
      first.childRooms.Add(second);
      second.connectedRooms.Add(first);
      second.connectedRoomsByExit.Add(secondPrototype.exitData.exits[secondRoomExitIndex], first);
      second.parentRoom = first;
      runtimeRoomExitData1.linkedExit = runtimeRoomExitData2;
      runtimeRoomExitData2.linkedExit = runtimeRoomExitData1;
      runtimeRoomExitData1.additionalExitLength = 3;
      runtimeRoomExitData2.additionalExitLength = 3;
    }

    public List<RoomHandler> AddRuntimeRoomCluster(
      List<PrototypeDungeonRoom> prototypes,
      List<IntVector2> basePositions,
      Action<RoomHandler> postProcessCellData = null,
      DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.FORCE_COLOR)
    {
      if (prototypes.Count != basePositions.Count)
      {
        UnityEngine.Debug.LogError((object) "Attempting to add a malformed room cluster at runtime!");
        return (List<RoomHandler>) null;
      }
      List<RoomHandler> roomHandlerList = new List<RoomHandler>();
      int y1 = 6;
      int num = 3;
      IntVector2 lhs1 = new IntVector2(int.MaxValue, int.MaxValue);
      IntVector2 lhs2 = new IntVector2(int.MinValue, int.MinValue);
      for (int index = 0; index < prototypes.Count; ++index)
      {
        lhs1 = IntVector2.Min(lhs1, basePositions[index]);
        lhs2 = IntVector2.Max(lhs2, basePositions[index] + new IntVector2(prototypes[index].Width, prototypes[index].Height));
      }
      IntVector2 intVector2_1 = lhs2 - lhs1;
      IntVector2 intVector2_2 = IntVector2.Min(IntVector2.Zero, -1 * lhs1);
      IntVector2 intVector2_3 = intVector2_1 + intVector2_2;
      IntVector2 intVector2_4 = new IntVector2(this.data.Width + y1, y1);
      CellData[][] cellDataArray = BraveUtility.MultidimensionalArrayResize<CellData>(this.data.cellData, this.data.Width, this.data.Height, this.data.Width + y1 * 2 + intVector2_3.x, Mathf.Max(this.data.Height, intVector2_3.y + y1 * 2));
      this.data.cellData = cellDataArray;
      this.data.ClearCachedCellData();
      for (int index = 0; index < prototypes.Count; ++index)
      {
        IntVector2 d = new IntVector2(prototypes[index].Width, prototypes[index].Height);
        IntVector2 intVector2_5 = basePositions[index] + intVector2_2;
        IntVector2 p1 = intVector2_4 + intVector2_5;
        CellArea a = new CellArea(p1, d);
        a.prototypeRoom = prototypes[index];
        RoomHandler roomHandler = new RoomHandler(a);
        for (int x = -y1; x < d.x + y1; ++x)
        {
          for (int y2 = -y1; y2 < d.y + y1; ++y2)
          {
            IntVector2 p2 = new IntVector2(x, y2) + p1;
            if (x >= 0 && y2 >= 0 && x < d.x && y2 < d.y || cellDataArray[p2.x][p2.y] == null)
            {
              CellData cellData = new CellData(p2);
              cellData.positionInTilemap = cellData.positionInTilemap - intVector2_4 + new IntVector2(num, num);
              cellData.parentArea = a;
              cellData.parentRoom = roomHandler;
              cellData.nearestRoom = roomHandler;
              cellData.distanceFromNearestRoom = 0.0f;
              cellDataArray[p2.x][p2.y] = cellData;
            }
          }
        }
        this.data.rooms.Add(roomHandler);
        roomHandlerList.Add(roomHandler);
      }
      for (int index = 1; index < roomHandlerList.Count; ++index)
        this.ConnectClusteredRuntimeRooms(roomHandlerList[index - 1], roomHandlerList[index], prototypes[index - 1], prototypes[index], index != 1 ? 1 : 0, 0);
      for (int index = 0; index < roomHandlerList.Count; ++index)
      {
        RoomHandler rh = roomHandlerList[index];
        rh.WriteRoomData(this.data);
        GameManager.Instance.Dungeon.data.GenerateLightsForRoom(GameManager.Instance.Dungeon.decoSettings, rh, GameObject.Find("_Lights").transform, lightStyle);
        if (postProcessCellData != null)
          postProcessCellData(rh);
        if (rh.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
          rh.BuildSecretRoomCover();
      }
      GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap"));
      tk2dTileMap component = gameObject.GetComponent<tk2dTileMap>();
      string str = Guid.NewGuid().ToString();
      gameObject.name = "RuntimeTilemap_" + str;
      component.renderData.name = $"RuntimeTilemap_{str} Render Data";
      component.Editor__SpriteCollection = this.tileIndices.dungeonCollection;
      TK2DDungeonAssembler.RuntimeResizeTileMap(component, intVector2_3.x + num * 2, intVector2_3.y + num * 2, this.m_tilemap.partitionSizeX, this.m_tilemap.partitionSizeY);
      for (int index1 = 0; index1 < prototypes.Count; ++index1)
      {
        IntVector2 intVector2_6 = new IntVector2(prototypes[index1].Width, prototypes[index1].Height);
        IntVector2 intVector2_7 = basePositions[index1] + intVector2_2;
        IntVector2 intVector2_8 = intVector2_4 + intVector2_7;
        for (int index2 = -num; index2 < intVector2_6.x + num; ++index2)
        {
          for (int index3 = -num; index3 < intVector2_6.y + num + 2; ++index3)
            this.assembler.BuildTileIndicesForCell(this, component, intVector2_8.x + index2, intVector2_8.y + index3);
        }
      }
      RenderMeshBuilder.CurrentCellXOffset = intVector2_4.x - num;
      RenderMeshBuilder.CurrentCellYOffset = intVector2_4.y - num;
      component.ForceBuild();
      RenderMeshBuilder.CurrentCellXOffset = 0;
      RenderMeshBuilder.CurrentCellYOffset = 0;
      component.renderData.transform.position = new Vector3((float) (intVector2_4.x - num), (float) (intVector2_4.y - num), (float) (intVector2_4.y - num));
      for (int index = 0; index < roomHandlerList.Count; ++index)
      {
        roomHandlerList[index].OverrideTilemap = component;
        for (int x = 0; x < roomHandlerList[index].area.dimensions.x; ++x)
        {
          for (int y3 = 0; y3 < roomHandlerList[index].area.dimensions.y + 2; ++y3)
          {
            IntVector2 intVector2_9 = roomHandlerList[index].area.basePosition + new IntVector2(x, y3);
            if (this.data.CheckInBoundsAndValid(intVector2_9))
            {
              CellData currentCell = this.data[intVector2_9];
              TK2DInteriorDecorator.PlaceLightDecorationForCell(this, component, currentCell, intVector2_9);
            }
          }
        }
        Pathfinder.Instance.InitializeRegion(this.data, roomHandlerList[index].area.basePosition + new IntVector2(-3, -3), roomHandlerList[index].area.dimensions + new IntVector2(3, 3));
        roomHandlerList[index].PostGenerationCleanup();
      }
      DeadlyDeadlyGoopManager.ReinitializeData();
      return roomHandlerList;
    }

    public RoomHandler AddRuntimeRoom(
      PrototypeDungeonRoom prototype,
      Action<RoomHandler> postProcessCellData = null,
      DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.FORCE_COLOR)
    {
      int y1 = 6;
      int num = 3;
      IntVector2 d = new IntVector2(prototype.Width, prototype.Height);
      IntVector2 p1 = new IntVector2(this.data.Width + y1, y1);
      CellData[][] cellDataArray = BraveUtility.MultidimensionalArrayResize<CellData>(this.data.cellData, this.data.Width, this.data.Height, this.data.Width + y1 * 2 + d.x, Mathf.Max(this.data.Height, d.y + y1 * 2));
      CellArea a = new CellArea(p1, d);
      a.prototypeRoom = prototype;
      this.data.cellData = cellDataArray;
      this.data.ClearCachedCellData();
      RoomHandler rh = new RoomHandler(a);
      for (int x = -y1; x < d.x + y1; ++x)
      {
        for (int y2 = -y1; y2 < d.y + y1; ++y2)
        {
          IntVector2 p2 = new IntVector2(x, y2) + p1;
          CellData cellData = new CellData(p2);
          cellData.positionInTilemap = cellData.positionInTilemap - p1 + new IntVector2(num, num);
          cellData.parentArea = a;
          cellData.parentRoom = rh;
          cellData.nearestRoom = rh;
          cellData.distanceFromNearestRoom = 0.0f;
          cellDataArray[p2.x][p2.y] = cellData;
        }
      }
      rh.WriteRoomData(this.data);
      for (int x = -y1; x < d.x + y1; ++x)
      {
        for (int y3 = -y1; y3 < d.y + y1; ++y3)
        {
          IntVector2 intVector2 = new IntVector2(x, y3) + p1;
          cellDataArray[intVector2.x][intVector2.y].breakable = true;
        }
      }
      this.data.rooms.Add(rh);
      tk2dTileMap component = ((GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap"))).GetComponent<tk2dTileMap>();
      component.Editor__SpriteCollection = this.tileIndices.dungeonCollection;
      GameManager.Instance.Dungeon.data.GenerateLightsForRoom(GameManager.Instance.Dungeon.decoSettings, rh, GameObject.Find("_Lights").transform, lightStyle);
      if (postProcessCellData != null)
        postProcessCellData(rh);
      TK2DDungeonAssembler.RuntimeResizeTileMap(component, d.x + num * 2, d.y + num * 2, this.m_tilemap.partitionSizeX, this.m_tilemap.partitionSizeY);
      for (int index1 = -num; index1 < d.x + num; ++index1)
      {
        for (int index2 = -num; index2 < d.y + num; ++index2)
          this.assembler.BuildTileIndicesForCell(this, component, p1.x + index1, p1.y + index2);
      }
      RenderMeshBuilder.CurrentCellXOffset = p1.x - num;
      RenderMeshBuilder.CurrentCellYOffset = p1.y - num;
      component.Build();
      RenderMeshBuilder.CurrentCellXOffset = 0;
      RenderMeshBuilder.CurrentCellYOffset = 0;
      component.renderData.transform.position = new Vector3((float) (p1.x - num), (float) (p1.y - num), (float) (p1.y - num));
      rh.OverrideTilemap = component;
      Pathfinder.Instance.InitializeRegion(this.data, rh.area.basePosition + new IntVector2(-3, -3), rh.area.dimensions + new IntVector2(3, 3));
      rh.PostGenerationCleanup();
      DeadlyDeadlyGoopManager.ReinitializeData();
      return rh;
    }

    public RoomHandler AddRuntimeRoom(IntVector2 dimensions, GameObject roomPrefab)
    {
      IntVector2 p1 = new IntVector2(this.data.Width + 10, 10);
      CellData[][] cellDataArray = BraveUtility.MultidimensionalArrayResize<CellData>(this.data.cellData, this.data.Width, this.data.Height, this.data.Width + 10 + dimensions.x, Mathf.Max(this.data.Height, dimensions.y + 10));
      CellArea a = new CellArea(p1, dimensions);
      a.IsProceduralRoom = true;
      this.data.cellData = cellDataArray;
      this.data.ClearCachedCellData();
      RoomHandler roomHandler = new RoomHandler(a);
      for (int x = 0; x < dimensions.x; ++x)
      {
        for (int y = 0; y < dimensions.y; ++y)
        {
          IntVector2 p2 = new IntVector2(x, y) + p1;
          cellDataArray[p2.x][p2.y] = new CellData(p2, CellType.FLOOR)
          {
            parentArea = a,
            parentRoom = roomHandler,
            nearestRoom = roomHandler
          };
          roomHandler.RuntimeStampCellComplex(p2.x, p2.y, CellType.FLOOR, DiagonalWallType.NONE);
        }
      }
      this.data.rooms.Add(roomHandler);
      UnityEngine.Object.Instantiate<GameObject>(roomPrefab, new Vector3((float) p1.x, (float) p1.y, 0.0f), Quaternion.identity);
      DeadlyDeadlyGoopManager.ReinitializeData();
      return roomHandler;
    }

    public GeneratedEnemyData GetWeightedProceduralEnemy()
    {
      float num1 = 0.0f;
      float num2 = UnityEngine.Random.value;
      for (int index = 0; index < this.m_generatedEnemyData.Count; ++index)
      {
        num1 += this.m_generatedEnemyData[index].percentOfEnemies;
        if ((double) num1 > (double) num2)
          return this.m_generatedEnemyData[index];
      }
      return this.m_generatedEnemyData[this.m_generatedEnemyData.Count - 1];
    }

    protected void RegisterGeneratedEnemyData(string id, int totalEnemyCount, bool isSignature)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < this.m_generatedEnemyData.Count; ++index2)
      {
        if (this.m_generatedEnemyData[index2].enemyGuid == id)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 < 0)
      {
        this.m_generatedEnemyData.Add(new GeneratedEnemyData(id, 1f / (float) totalEnemyCount, isSignature));
      }
      else
      {
        GeneratedEnemyData generatedEnemyData = this.m_generatedEnemyData[index1];
        generatedEnemyData.percentOfEnemies += 1f / (float) totalEnemyCount;
        this.m_generatedEnemyData[index1] = generatedEnemyData;
      }
    }

    public void SpawnCurseReaper()
    {
      if (!GameManager.HasInstance || !(bool) (UnityEngine.Object) GameManager.Instance.BestActivePlayer || GameManager.Instance.BestActivePlayer.CurrentRoom == null)
        return;
      this.CurseReaperActive = true;
      GameObject superReaper = PrefabDatabase.Instance.SuperReaper;
      Vector2 vector2_1 = GameManager.Instance.BestActivePlayer.CurrentRoom.GetRandomVisibleClearSpot(2, 2).ToVector2();
      SpeculativeRigidbody component = superReaper.GetComponent<SpeculativeRigidbody>();
      if ((bool) (UnityEngine.Object) component)
      {
        PixelCollider primaryPixelCollider = component.PrimaryPixelCollider;
        Vector2 unit1 = PhysicsEngine.PixelToUnit(new IntVector2(primaryPixelCollider.ManualOffsetX, primaryPixelCollider.ManualOffsetY));
        Vector2 unit2 = PhysicsEngine.PixelToUnit(new IntVector2(primaryPixelCollider.ManualWidth, primaryPixelCollider.ManualHeight));
        Vector2 vector2_2 = new Vector2((float) (((double) new Vector2((float) Mathf.CeilToInt(unit2.x), (float) Mathf.CeilToInt(unit2.y)).x - (double) unit2.x) / 2.0), 0.0f).Quantize(1f / 16f);
        vector2_1 -= unit1 - vector2_2;
      }
      UnityEngine.Object.Instantiate<GameObject>(superReaper, vector2_1.ToVector3ZUp(), Quaternion.identity);
    }

    [DebuggerHidden]
    private IEnumerator HandleAmbientChannelVFX()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Dungeon__HandleAmbientChannelVFXc__Iterator6()
      {
        _this = this
      };
    }

    private bool IsCentralChannel(CellData cell)
    {
      IntVector2 position = cell.position;
      for (int index = 0; index < IntVector2.Cardinals.Length; ++index)
      {
        IntVector2 key = position + IntVector2.Cardinals[index];
        if (!this.data[key].cellVisualData.IsChannel && !this.data[key].doesDamage)
          return false;
      }
      return true;
    }

    [DebuggerHidden]
    private IEnumerator HandleAmbientPitVFX()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Dungeon__HandleAmbientPitVFXc__Iterator7()
      {
        _this = this
      };
    }
  }
}
