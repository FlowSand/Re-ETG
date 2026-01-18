// Decompiled with JetBrains decompiler
// Type: GameManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using HutongGames.PlayMaker.Actions;
using InControl;
using Pathfinding;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using tk2dRuntime.TileMap;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable

public class GameManager : BraveBehaviour
  {
    public static bool BackgroundGenerationActive;
    public static bool DivertResourcesToGeneration;
    public static bool IsShuttingDown;
    public const int EEVEE_META_COST = 5;
    public const int GUNSLINGER_META_COST = 7;
    public const bool c_RESOURCEFUL_RAT_ACTIVE = false;
    public const float CUSTOM_CULL_SQR_DIST_THRESHOLD = 420f;
    private static string DEBUG_LABEL;
    public static string SEED_LABEL = string.Empty;
    public const float SCENE_TRANSITION_TIME = 0.15f;
    public static bool AUDIO_ENABLED;
    public static float PIT_DEPTH = -2.5f;
    public static float INVARIANT_DELTA_TIME;
    public static bool SKIP_FOYER;
    public static bool PVP_ENABLED;
    public static bool IsBossIntro;
    public PlatformInterface platformInterface;
    private Coroutine CurrentResolutionShiftCoroutine;
    private static GameObject m_playerPrefabForNewGame;
    private static GameObject m_coopPlayerPrefabForNewGame;
    public static GameObject LastUsedPlayerPrefab;
    public static GameObject LastUsedCoopPlayerPrefab;
    private static GameOptions m_options;
    private static GameManager mr_manager;
    private static InControlManager m_inputManager;
    private DungeonFloorMusicController m_dungeonMusicController;
    public static bool PreventGameManagerExistence;
    public RewardManager CurrentRewardManager;
    public RewardManager OriginalRewardManager;
    public AdvancedSynergyDatabase SynergyManager;
    public MetaInjectionData GlobalInjectionData;
    [NonSerialized]
    public InputDevice LastUsedInputDeviceForConversation;
    public tk2dFontData DefaultAlienConversationFont;
    public tk2dFontData DefaultNormalConversationFont;
    public int RandomIntForCurrentRun;
    [NonSerialized]
    public bool IsLoadingFirstShortcutFloor;
    public int LastShortcutFloorLoaded;
    private bool m_forceSeedUpdate;
    [NonSerialized]
    private int m_currentRunSeed;
    private bool m_paused;
    private bool m_unpausedThisFrame;
    private bool m_pauseLockedCamera;
    private bool m_loadingLevel;
    private bool m_isFoyer;
    private GameManager.GameMode m_currentGameMode;
    public GameManager.ControlType controlType;
    private GameManager.GameType m_currentGameType;
    public bool IsSelectingCharacter;
    private GameManager.LevelOverrideState? m_generatingLevelState;
    public static bool IsCoopPast;
    public static bool IsGunslingerPast;
    private Dungeonator.Dungeon m_dungeon;
    public Dungeonator.Dungeon CurrentlyGeneratingDungeonPrefab;
    public DungeonData PregeneratedDungeonData;
    public Dungeonator.Dungeon DungeonToAutoLoad;
    private CameraController m_camera;
    private PlayerController[] m_players;
    public int LastPausingPlayerID = -1;
    private PlayerController m_player;
    private PlayerController m_secondaryPlayer;
    [NonSerialized]
    public List<string> ExtantShopTrackableGuids = new List<string>();
    public List<GameLevelDefinition> dungeonFloors;
    public List<GameLevelDefinition> customFloors;
    private GameLevelDefinition m_lastLoadedLevelDefinition;
    private int nextLevelIndex = 1;
    [NonSerialized]
    private string m_injectedFlowPath;
    [NonSerialized]
    private string m_injectedLevelName;
    private bool m_preventUnpausing;
    public RunData RunData = new RunData();
    protected static float m_deltaTime;
    protected static float m_lastFrameRealtime;
    private bool m_applicationHasFocus = true;
    private static Vector4 s_bossIntroTime;
    private static int s_bossIntroTimeId = -1;
    private const int c_framesToCount = 4;
    private CircularBuffer<float> m_frameTimes = new CircularBuffer<float>(4);
    private AkAudioListener m_audioListener;
    public int TargetQuickRestartLevel = -1;
    public static bool ForceQuickRestartAlternateCostumeP1;
    public static bool ForceQuickRestartAlternateCostumeP2;
    public static bool ForceQuickRestartAlternateGunP1;
    public static bool ForceQuickRestartAlternateGunP2;
    public static bool IsReturningToFoyerWithPlayer;
    private bool m_preparingToDestroyThisGameManagerOnCollision;
    private bool m_shouldDestroyThisGameManagerOnCollision;
    private AsyncOperation m_preDestroyAsyncHolder;
    private GameObject m_preDestroyLoadingHierarchyHolder;
    private System.Type[] BraveLevelLoadedListeners = new System.Type[10]
    {
      typeof (PlayerController),
      typeof (SpeculativeRigidbody),
      typeof (GameUIBlankController),
      typeof (AmmonomiconDeathPageController),
      typeof (GameUIHeartController),
      typeof (RingOfResourcefulRatItem),
      typeof (ReturnAmmoOnMissedShotItem),
      typeof (PlatinumBulletsItem),
      typeof (dfPoolManager),
      typeof (ChamberGunProcessor)
    };
    private const float PIXELATE_TIME = 0.15f;
    private const float PIXELATE_FADE_TARGET = 1f;
    private const float DEPIXELATE_TIME = 0.075f;
    private static float c_asyncSoundStartTime;
    private static int c_asyncSoundStartFrame;
    private static bool m_hasEnsuredHeapSize;
    private bool m_initializedDeviceCallbacks;
    public bool PREVENT_MAIN_MENU_TEXT;
    public bool DEBUG_UI_VISIBLE = true;
    public bool DAVE_INFO_VISIBLE;
    [Header("Convenient Balance Numbers")]
    public float COOP_ENEMY_HEALTH_MULTIPLIER = 1.25f;
    public float COOP_ENEMY_PROJECTILE_SPEED_MULTIPLIER = 0.9f;
    public float DUAL_WIELDING_DAMAGE_FACTOR = 0.75f;
    public float[] PierceDamageScaling;
    public BloodthirstSettings BloodthirstOptions;
    public List<AGDEnemyReplacementTier> EnemyReplacementTiers;
    [PickupIdentifier]
    public List<int> RainbowRunForceIncludedIDs;
    [PickupIdentifier]
    public List<int> RainbowRunForceExcludedIDs;
    private bool m_bgChecksActive;
    private HashSet<string> m_knownEncounterables = new HashSet<string>();
    private List<string> m_queuedUnlocks = new List<string>();
    private List<string> m_newQueuedUnlocks = new List<string>();
    private const int NUM_ENCOUNTERABLES_CHECKED_PER_FRAME = 20;

    public void DoSetResolution(int newWidth, int newHeight, bool newFullscreen)
    {
      UnityEngine.Debug.Log((object) $"Setting RESOLUTION internal to: {(object) newWidth}|{(object) newHeight}|{(object) newFullscreen}");
      if (newFullscreen != Screen.fullScreen)
      {
        bool flag = newFullscreen == Screen.fullScreen;
        Screen.SetResolution(newWidth, newHeight, newFullscreen);
        if (!flag)
          return;
        if (this.CurrentResolutionShiftCoroutine != null)
          this.StopCoroutine(this.CurrentResolutionShiftCoroutine);
        this.CurrentResolutionShiftCoroutine = this.StartCoroutine(this.SetResolutionPostFullscreenChange(newWidth, newHeight));
      }
      else
        Screen.SetResolution(newWidth, newHeight, Screen.fullScreen, Screen.currentResolution.refreshRate);
    }

    [DebuggerHidden]
    private IEnumerator SetResolutionPostFullscreenChange(int newWidth, int newHeight)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__SetResolutionPostFullscreenChangec__Iterator0()
      {
        newWidth = newWidth,
        newHeight = newHeight,
        _this = this
      };
    }

    public static void LoadResolutionFromPS4()
    {
    }

    private static void LoadResolutionFromOptions()
    {
      if (GameManager.IsReturningToBreach)
        return;
      GameOptions.PreferredFullscreenMode preferredFullscreenMode = GameManager.Options.CurrentPreferredFullscreenMode;
      if (GameManager.Options.preferredResolutionX <= 50 || GameManager.Options.preferredResolutionY <= 50 || GameManager.Options.preferredResolutionX > 50000 || GameManager.Options.preferredResolutionY > 50000)
      {
        GameManager.Options.preferredResolutionX = -1;
        GameManager.Options.preferredResolutionY = -1;
      }
      if (GameManager.Options.preferredResolutionX <= 0)
      {
        Resolution resolution = Screen.resolutions[Screen.resolutions.Length - 1];
        GameManager.Options.preferredResolutionX = resolution.width;
        GameManager.Options.preferredResolutionY = resolution.height;
      }
      Resolution resolution1 = new Resolution();
      resolution1.width = GameManager.Options.preferredResolutionX;
      resolution1.height = GameManager.Options.preferredResolutionY;
      resolution1.refreshRate = Screen.currentResolution.refreshRate;
      if (preferredFullscreenMode != GameOptions.PreferredFullscreenMode.FULLSCREEN)
      {
        BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes targetDisplayMode = BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Fullscreen;
        if (preferredFullscreenMode == GameOptions.PreferredFullscreenMode.BORDERLESS)
          targetDisplayMode = BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Borderless;
        if (preferredFullscreenMode == GameOptions.PreferredFullscreenMode.WINDOWED)
          targetDisplayMode = BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Windowed;
        bool flag = Screen.fullScreen != (preferredFullscreenMode == GameOptions.PreferredFullscreenMode.FULLSCREEN);
        UnityEngine.Debug.Log((object) ("Invoking standard WIN startup methods to set fullscreen: " + (object) flag));
        if (flag)
        {
          BraveOptionsMenuItem componentInChildren = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.GetComponentInChildren<BraveOptionsMenuItem>();
          componentInChildren.StartCoroutine(componentInChildren.FrameDelayedWindowsShift(targetDisplayMode, resolution1));
        }
        else
          BraveOptionsMenuItem.ResolutionManagerWin.TrySetDisplay(targetDisplayMode, resolution1, false, new Position?());
      }
      if (resolution1.width == Screen.width && resolution1.height == Screen.height && preferredFullscreenMode == GameOptions.PreferredFullscreenMode.FULLSCREEN == Screen.fullScreen)
        return;
      UnityEngine.Debug.Log((object) $"Invoking standard startup methods to set resolution: {(object) resolution1.width}|{(object) resolution1.height}||{preferredFullscreenMode.ToString()}");
      GameManager.Instance.DoSetResolution(resolution1.width, resolution1.height, preferredFullscreenMode == GameOptions.PreferredFullscreenMode.FULLSCREEN);
    }

    public static GameObject PlayerPrefabForNewGame
    {
      get => GameManager.m_playerPrefabForNewGame;
      set
      {
        GameManager.m_playerPrefabForNewGame = value;
        if (!((UnityEngine.Object) GameManager.m_playerPrefabForNewGame != (UnityEngine.Object) null))
          return;
        PlayableCharacters characterIdentity = GameManager.m_playerPrefabForNewGame.GetComponent<PlayerController>().characterIdentity;
        switch (characterIdentity)
        {
          case PlayableCharacters.Eevee:
          case PlayableCharacters.Gunslinger:
            GameManager.LastUsedPlayerPrefab = GameManager.m_playerPrefabForNewGame;
            if (!(bool) (UnityEngine.Object) GameManager.LastUsedPlayerPrefab || !(bool) (UnityEngine.Object) GameManager.LastUsedPlayerPrefab.GetComponent<PlayerSpaceshipController>())
              break;
            GameManager.LastUsedPlayerPrefab = (GameObject) BraveResources.Load("PlayerRogue");
            break;
          default:
            GameManager.Options.LastPlayedCharacter = characterIdentity;
            goto case PlayableCharacters.Eevee;
        }
      }
    }

    public static GameObject CoopPlayerPrefabForNewGame
    {
      get => BraveResources.Load("PlayerCoopCultist") as GameObject;
      set
      {
        GameManager.m_coopPlayerPrefabForNewGame = value;
        if (!((UnityEngine.Object) GameManager.m_coopPlayerPrefabForNewGame != (UnityEngine.Object) null))
          return;
        GameManager.LastUsedCoopPlayerPrefab = GameManager.m_coopPlayerPrefabForNewGame;
      }
    }

    public static GameOptions Options
    {
      get
      {
        if (GameManager.m_options == null)
        {
          DebugTime.RecordStartTime();
          GameOptions.Load();
          DebugTime.Log("Load game options");
        }
        return GameManager.m_options;
      }
      set => GameManager.m_options = value;
    }

    public DungeonFloorMusicController DungeonMusicController => this.m_dungeonMusicController;

    public static GameManager EnsureExistence()
    {
      return (UnityEngine.Object) GameManager.Instance == (UnityEngine.Object) null ? GameManager.Instance : GameManager.Instance;
    }

    public static GameManager Instance
    {
      get
      {
        if (GameManager.PreventGameManagerExistence)
          return (GameManager) null;
        if ((UnityEngine.Object) GameManager.mr_manager == (UnityEngine.Object) null)
          GameManager.mr_manager = (GameManager) UnityEngine.Object.FindObjectOfType(typeof (GameManager));
        if ((UnityEngine.Object) GameManager.mr_manager == (UnityEngine.Object) null)
        {
          UnityEngine.Debug.Log((object) "INSTANTRON");
          GameManager.mr_manager = new GameObject("_GameManager(temp)").AddComponent<GameManager>();
        }
        return GameManager.mr_manager;
      }
    }

    public static bool HasInstance
    {
      get
      {
        return (UnityEngine.Object) GameManager.mr_manager != (UnityEngine.Object) null && (bool) (UnityEngine.Object) GameManager.mr_manager;
      }
    }

    public static bool IsReturningToBreach { get; set; }

    public BossManager BossManager => BraveResources.Load<BossManager>("AAA_BOSS_MANAGER", ".asset");

    public RewardManager RewardManager
    {
      get
      {
        return GameManager.Options.CurrentGameLootProfile == GameOptions.GameLootProfile.ORIGINAL ? this.OriginalRewardManager : this.CurrentRewardManager;
      }
    }

    public event System.Action OnNewLevelFullyLoaded;

    public int CurrentRunSeed
    {
      get => this.m_currentRunSeed;
      set
      {
        int currentRunSeed = this.m_currentRunSeed;
        UnityEngine.Debug.LogError((object) ("SETTING GLOBAL RUN SEED TO: " + (object) value));
        this.m_currentRunSeed = value;
        UnityEngine.Random.InitState(this.m_currentRunSeed);
        BraveRandom.IgnoreGenerationDifferentiator = true;
        BraveRandom.InitializeWithSeed(value);
        if (this.m_currentRunSeed == currentRunSeed && !this.m_forceSeedUpdate)
          return;
        this.m_forceSeedUpdate = false;
        UnityEngine.Debug.LogError((object) "DOING STARTUP SEED DATA");
        MetaInjectionData.ClearBlueprint();
        GameManager.Instance.GlobalInjectionData.PreprocessRun();
        RewardManifest.ClearManifest(this.RewardManager);
        RewardManifest.Initialize(this.RewardManager);
      }
    }

    public bool IsSeeded => this.m_currentRunSeed != 0;

    public bool IsPaused => this.m_paused;

    public bool UnpausedThisFrame => this.m_unpausedThisFrame;

    public bool IsLoadingLevel
    {
      get => this.m_loadingLevel;
      private set
      {
        if (!value)
          GameManager.IsReturningToBreach = false;
        this.m_loadingLevel = value;
      }
    }

    public bool IsFoyer
    {
      get => this.m_isFoyer;
      set
      {
        this.m_isFoyer = value;
        if (this.m_isFoyer)
          return;
        Foyer.ClearInstance();
      }
    }

    public static bool IsTurboMode
    {
      get => GameStatsManager.HasInstance && GameStatsManager.Instance.isTurboMode;
    }

    public GameManager.GameMode CurrentGameMode
    {
      get => this.m_currentGameMode;
      set => this.m_currentGameMode = value;
    }

    public GameManager.GameType CurrentGameType
    {
      get => this.m_currentGameType;
      set => this.m_currentGameType = value;
    }

    public GameManager.LevelOverrideState GeneratingLevelOverrideState
    {
      get
      {
        return this.m_generatingLevelState.HasValue ? this.m_generatingLevelState.Value : this.CurrentLevelOverrideState;
      }
    }

    public GameManager.LevelOverrideState CurrentLevelOverrideState
    {
      get
      {
        if (this.IsLoadingLevel && (UnityEngine.Object) this.CurrentlyGeneratingDungeonPrefab != (UnityEngine.Object) null)
          return this.CurrentlyGeneratingDungeonPrefab.LevelOverrideType;
        return (UnityEngine.Object) this.Dungeon == (UnityEngine.Object) null ? ((UnityEngine.Object) this.BestGenerationDungeonPrefab != (UnityEngine.Object) null ? this.BestGenerationDungeonPrefab.LevelOverrideType : GameManager.LevelOverrideState.NONE) : (this.Dungeon.IsEndTimes ? GameManager.LevelOverrideState.END_TIMES : this.Dungeon.LevelOverrideType);
      }
    }

    public int CurrentFloor
    {
      get
      {
        if (this.IsFoyer)
          return 0;
        GameLevelDefinition loadedLevelDefinition = this.GetLastLoadedLevelDefinition();
        int currentFloor = -1;
        if (loadedLevelDefinition != null)
          currentFloor = this.dungeonFloors.IndexOf(loadedLevelDefinition);
        return currentFloor;
      }
    }

    public Dungeonator.Dungeon Dungeon
    {
      get
      {
        if ((UnityEngine.Object) this.m_dungeon == (UnityEngine.Object) null)
          this.m_dungeon = UnityEngine.Object.FindObjectOfType<Dungeonator.Dungeon>();
        return this.m_dungeon;
      }
    }

    public void ClearGenerativeDungeonData()
    {
      this.CurrentlyGeneratingDungeonPrefab = (Dungeon) null;
      this.PregeneratedDungeonData = (DungeonData) null;
    }

    public Dungeonator.Dungeon BestGenerationDungeonPrefab
    {
      get
      {
        return this.IsLoadingLevel && (UnityEngine.Object) this.CurrentlyGeneratingDungeonPrefab != (UnityEngine.Object) null ? this.CurrentlyGeneratingDungeonPrefab : this.m_dungeon;
      }
    }

    public CameraController MainCameraController
    {
      get
      {
        if ((UnityEngine.Object) this.m_camera == (UnityEngine.Object) null)
        {
          GameObject gameObject = GameObject.Find("Main Camera");
          if ((UnityEngine.Object) gameObject != (UnityEngine.Object) null)
            this.m_camera = gameObject.GetComponent<CameraController>();
          else if ((bool) (UnityEngine.Object) Camera.main)
            this.m_camera = Camera.main.GetComponent<CameraController>();
        }
        return this.m_camera;
      }
    }

    public bool HasPlayer(PlayerController p)
    {
      for (int index = 0; index < this.AllPlayers.Length; ++index)
      {
        if ((UnityEngine.Object) this.AllPlayers[index] == (UnityEngine.Object) p)
          return true;
      }
      return false;
    }

    public PlayerController[] AllPlayers
    {
      get
      {
        if (this.m_players != null)
        {
          for (int index = 0; index < this.m_players.Length; ++index)
          {
            if (!(bool) (UnityEngine.Object) this.m_players[index])
            {
              this.m_player = (PlayerController) null;
              this.m_secondaryPlayer = (PlayerController) null;
              this.m_players = (PlayerController[]) null;
              break;
            }
          }
        }
        if (this.m_players == null || this.m_players.Length == 0 && !this.IsSelectingCharacter)
        {
          List<PlayerController> playerControllerList = new List<PlayerController>((IEnumerable<PlayerController>) UnityEngine.Object.FindObjectsOfType<PlayerController>());
          for (int index = 0; index < playerControllerList.Count; ++index)
          {
            if (!(bool) (UnityEngine.Object) playerControllerList[index])
            {
              playerControllerList.RemoveAt(index);
              --index;
            }
          }
          this.m_players = playerControllerList.ToArray();
          if (this.m_players != null && this.m_players.Length == 2 && this.m_players[1].IsPrimaryPlayer)
          {
            PlayerController player = this.m_players[0];
            this.m_players[0] = this.m_players[1];
            this.m_players[1] = player;
          }
        }
        return this.m_players;
      }
    }

    public int NumberOfLivingPlayers
    {
      get
      {
        int numberOfLivingPlayers = 0;
        for (int index = 0; index < this.AllPlayers.Length; ++index)
        {
          if (!this.AllPlayers[index].IsGhost && !this.AllPlayers[index].healthHaver.IsDead)
            ++numberOfLivingPlayers;
        }
        return numberOfLivingPlayers;
      }
    }

    public bool PlayerIsInRoom(RoomHandler targetRoom)
    {
      for (int index = 0; index < this.AllPlayers.Length; ++index)
      {
        if (this.AllPlayers[index].CurrentRoom == targetRoom)
          return true;
      }
      return false;
    }

    public bool PlayerIsNearRoom(RoomHandler targetRoom)
    {
      for (int index = 0; index < this.AllPlayers.Length; ++index)
      {
        if (this.AllPlayers[index].CurrentRoom == targetRoom || this.AllPlayers[index].CurrentRoom != null && this.AllPlayers[index].CurrentRoom.connectedRooms != null && this.AllPlayers[index].CurrentRoom.connectedRooms.Contains(targetRoom))
          return true;
      }
      return false;
    }

    public void RefreshAllPlayers()
    {
      this.m_players = (PlayerController[]) null;
      this.m_players = this.AllPlayers;
    }

    public PlayerController PrimaryPlayer
    {
      get
      {
        if (this.IsSelectingCharacter && this.IsFoyer)
          return (PlayerController) null;
        if (GameManager.IsReturningToBreach && !GameManager.IsReturningToFoyerWithPlayer && this.IsFoyer)
          return (PlayerController) null;
        if ((UnityEngine.Object) this.m_player == (UnityEngine.Object) null)
        {
          PlayerController[] objectsOfType = UnityEngine.Object.FindObjectsOfType<PlayerController>();
          for (int index = 0; index < objectsOfType.Length; ++index)
          {
            if (objectsOfType[index].IsPrimaryPlayer)
            {
              this.m_player = objectsOfType[index];
              break;
            }
          }
        }
        return this.m_player;
      }
      set
      {
        this.m_player = value;
        if (this.m_players == null || this.m_players.Length <= 0)
          return;
        this.m_players[0] = value;
      }
    }

    public PlayerController SecondaryPlayer
    {
      get
      {
        if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
          return (PlayerController) null;
        if ((UnityEngine.Object) this.m_secondaryPlayer == (UnityEngine.Object) null)
        {
          for (int index = 0; index < this.AllPlayers.Length; ++index)
          {
            if (!this.AllPlayers[index].IsPrimaryPlayer && this.AllPlayers[index].characterIdentity == PlayableCharacters.CoopCultist)
            {
              this.m_secondaryPlayer = this.AllPlayers[index];
              break;
            }
          }
        }
        return this.m_secondaryPlayer;
      }
      set
      {
        this.m_secondaryPlayer = value;
        if (this.m_players != null && this.m_players.Length > 1)
          this.m_players[1] = value;
        if (this.m_players == null || this.m_players.Length >= 2)
          return;
        this.m_players = (PlayerController[]) null;
      }
    }

    public PlayerController GetOtherPlayer(PlayerController p)
    {
      if (this.CurrentGameType == GameManager.GameType.SINGLE_PLAYER)
        return (PlayerController) null;
      return (UnityEngine.Object) p == (UnityEngine.Object) this.PrimaryPlayer ? this.SecondaryPlayer : this.PrimaryPlayer;
    }

    public PlayerController BestActivePlayer
    {
      get
      {
        if (!(bool) (UnityEngine.Object) this.PrimaryPlayer && !(bool) (UnityEngine.Object) this.SecondaryPlayer)
          return (PlayerController) null;
        return this.PrimaryPlayer.IsGhost || this.PrimaryPlayer.healthHaver.IsDead ? this.SecondaryPlayer : this.PrimaryPlayer;
      }
    }

    public GlobalDungeonData.ValidTilesets GetNextTileset(GlobalDungeonData.ValidTilesets tilesetID)
    {
      switch (tilesetID)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          return GlobalDungeonData.ValidTilesets.MINEGEON;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          return GlobalDungeonData.ValidTilesets.GUNGEON;
        case GlobalDungeonData.ValidTilesets.SEWERGEON:
          return GlobalDungeonData.ValidTilesets.GUNGEON;
        case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
          return GlobalDungeonData.ValidTilesets.MINEGEON;
        default:
          if (tilesetID == GlobalDungeonData.ValidTilesets.MINEGEON)
            return GlobalDungeonData.ValidTilesets.CATACOMBGEON;
          if (tilesetID == GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            return GlobalDungeonData.ValidTilesets.FORGEGEON;
          if (tilesetID == GlobalDungeonData.ValidTilesets.FORGEGEON)
            return GlobalDungeonData.ValidTilesets.HELLGEON;
          if (tilesetID == GlobalDungeonData.ValidTilesets.OFFICEGEON)
            return GlobalDungeonData.ValidTilesets.FORGEGEON;
          return tilesetID == GlobalDungeonData.ValidTilesets.RATGEON ? GlobalDungeonData.ValidTilesets.CATACOMBGEON : GlobalDungeonData.ValidTilesets.CASTLEGEON;
      }
    }

    public int GetTargetLevelIndexFromSavedTileset(GlobalDungeonData.ValidTilesets tilesetID)
    {
      switch (tilesetID)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
          return 2;
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          return 1;
        case GlobalDungeonData.ValidTilesets.SEWERGEON:
          return 2;
        case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
          return 3;
        default:
          if (tilesetID == GlobalDungeonData.ValidTilesets.MINEGEON)
            return 3;
          if (tilesetID == GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            return 4;
          if (tilesetID == GlobalDungeonData.ValidTilesets.FORGEGEON)
            return 5;
          if (tilesetID == GlobalDungeonData.ValidTilesets.HELLGEON)
            return 6;
          if (tilesetID == GlobalDungeonData.ValidTilesets.OFFICEGEON)
            return 5;
          if (tilesetID == GlobalDungeonData.ValidTilesets.FINALGEON)
            return 6;
          return tilesetID == GlobalDungeonData.ValidTilesets.RATGEON ? 4 : 1;
      }
    }

    public void SetNextLevelIndex(int index) => this.nextLevelIndex = index;

    public string InjectedFlowPath
    {
      get => this.m_injectedFlowPath;
      set => this.m_injectedFlowPath = value;
    }

    public string InjectedLevelName
    {
      get => this.m_injectedLevelName;
      set => this.m_injectedLevelName = value;
    }

    public bool PreventPausing
    {
      get => this.m_preventUnpausing;
      set
      {
        if (this.m_preventUnpausing == value)
          return;
        this.m_preventUnpausing = value;
        if (this.m_preventUnpausing || this.m_applicationHasFocus || this.m_loadingLevel || this.m_paused)
          return;
        this.Pause();
        GameStatsManager.Save();
      }
    }

    public bool InTutorial { get; set; }

    public void OnApplicationQuit()
    {
      GameManager.IsShuttingDown = true;
      if (!this.ShouldDeleteSaveOnExit)
        return;
      SaveManager.DeleteCurrentSlotMidGameSave();
    }

    public bool ShouldDeleteSaveOnExit
    {
      get
      {
        return !this.IsFoyer && !SaveManager.PreventMidgameSaveDeletionOnExit && !GameManager.BackgroundGenerationActive && !Dungeon.IsGenerating;
      }
    }

    public void OnApplicationFocus(bool focusStatus)
    {
      if (Application.isEditor || MemoryTester.HasInstance)
        return;
      if (!focusStatus && (UnityEngine.Object) this.PrimaryPlayer != (UnityEngine.Object) null && !this.PreventPausing && !this.m_loadingLevel && !this.m_paused)
      {
        this.Pause();
        GameStatsManager.Save();
      }
      this.m_applicationHasFocus = focusStatus;
    }

    protected void Update()
    {
      tk2dSpriteAnimator.InDungeonScene = (UnityEngine.Object) this.m_dungeon != (UnityEngine.Object) null;
      BraveTime.UpdateScaledTimeSinceStartup();
      if (GameManager.IsBossIntro)
      {
        if (GameManager.s_bossIntroTimeId < 0)
          GameManager.s_bossIntroTimeId = Shader.PropertyToID("_BossIntroTime");
        GameManager.s_bossIntroTime += new Vector4(0.05f, 1f, 2f, 3f) * GameManager.INVARIANT_DELTA_TIME;
        Shader.SetGlobalVector(GameManager.s_bossIntroTimeId, GameManager.s_bossIntroTime);
      }
      float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
      GameManager.m_deltaTime = UnityEngine.Time.unscaledDeltaTime;
      GameManager.m_lastFrameRealtime = realtimeSinceStartup;
      GameManager.INVARIANT_DELTA_TIME = GameManager.m_deltaTime;
      this.InvariantUpdate(GameManager.m_deltaTime);
      double num = (double) this.m_frameTimes.Enqueue(GameManager.INVARIANT_DELTA_TIME);
      for (int index = 0; index < StaticReferenceManager.AllClusteredTimeInvariantBehaviours.Count; ++index)
      {
        ClusteredTimeInvariantMonoBehaviour invariantBehaviour = StaticReferenceManager.AllClusteredTimeInvariantBehaviours[index];
        if (!(bool) (UnityEngine.Object) invariantBehaviour)
        {
          StaticReferenceManager.AllClusteredTimeInvariantBehaviours.RemoveAt(index);
          --index;
        }
        else
          invariantBehaviour.DoUpdate(GameManager.INVARIANT_DELTA_TIME);
      }
      if (!GameManager.AUDIO_ENABLED)
        return;
      if (!(bool) (UnityEngine.Object) this.m_player && !(bool) (UnityEngine.Object) this.m_audioListener)
      {
        UnityEngine.Debug.LogWarning((object) "Adding a new GameManager audio listener");
        this.m_audioListener = this.gameObject.GetOrAddComponent<AkAudioListener>();
      }
      else
      {
        if (!(bool) (UnityEngine.Object) this.m_player || !(bool) (UnityEngine.Object) this.m_audioListener)
          return;
        UnityEngine.Debug.LogWarning((object) "Destroying the GameManager's audio listener");
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_audioListener);
        this.m_audioListener = (AkAudioListener) null;
      }
    }

    private void LateUpdate()
    {
      this.platformInterface.LateUpdate();
      this.m_unpausedThisFrame = false;
    }

    public PlayerController GetPlayerClosestToPoint(Vector2 point)
    {
      PlayerController playerClosestToPoint = (PlayerController) null;
      float num1 = float.MaxValue;
      for (int index = 0; index < this.AllPlayers.Length; ++index)
      {
        if (!this.AllPlayers[index].healthHaver.IsDead)
        {
          float num2 = Vector2.Distance(point, this.AllPlayers[index].CenterPosition);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            playerClosestToPoint = this.AllPlayers[index];
          }
        }
      }
      return playerClosestToPoint;
    }

    public PlayerController GetPlayerClosestToPoint(Vector2 point, out float range)
    {
      PlayerController playerClosestToPoint = (PlayerController) null;
      float num1 = float.MaxValue;
      for (int index = 0; index < this.AllPlayers.Length; ++index)
      {
        if (!this.AllPlayers[index].healthHaver.IsDead)
        {
          float num2 = Vector2.Distance(point, this.AllPlayers[index].CenterPosition);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            playerClosestToPoint = this.AllPlayers[index];
          }
        }
      }
      range = num1;
      return playerClosestToPoint;
    }

    public PlayerController GetActivePlayerClosestToPoint(Vector2 point, bool allowStealth = false)
    {
      if (this.IsSelectingCharacter)
        return (PlayerController) null;
      if (!GameManager.IsReturningToFoyerWithPlayer && GameManager.IsReturningToBreach && this.IsFoyer)
        return (PlayerController) null;
      PlayerController playerClosestToPoint = (PlayerController) null;
      float num1 = float.MaxValue;
      for (int index = 0; index < this.AllPlayers.Length; ++index)
      {
        if (!this.AllPlayers[index].IsGhost && !this.AllPlayers[index].healthHaver.IsDead && !this.AllPlayers[index].IsFalling && !this.AllPlayers[index].IsCurrentlyCoopReviving && (allowStealth || !this.AllPlayers[index].IsStealthed))
        {
          float num2 = Vector2.Distance(point, this.AllPlayers[index].CenterPosition);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            playerClosestToPoint = this.AllPlayers[index];
          }
        }
      }
      return playerClosestToPoint;
    }

    public bool IsAnyPlayerInRoom(RoomHandler room)
    {
      for (int index = 0; index < this.AllPlayers.Length; ++index)
      {
        if (!this.AllPlayers[index].healthHaver.IsDead && this.AllPlayers[index].CurrentRoom == room)
          return true;
      }
      return false;
    }

    public PlayerController GetRandomActivePlayer()
    {
      if (this.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
        return this.PrimaryPlayer;
      if (!this.PrimaryPlayer.healthHaver.IsAlive || !this.SecondaryPlayer.healthHaver.IsAlive)
        return this.BestActivePlayer;
      return (double) UnityEngine.Random.value > 0.5 ? this.PrimaryPlayer : this.SecondaryPlayer;
    }

    public PlayerController GetRandomPlayer()
    {
      return this.AllPlayers[UnityEngine.Random.Range(0, this.AllPlayers.Length)];
    }

    public void DelayedQuickRestart(float duration, QuickRestartOptions options = default (QuickRestartOptions))
    {
      this.StartCoroutine(this.DelayedQuickRestart_CR(duration, options));
    }

    [DebuggerHidden]
    private IEnumerator DelayedQuickRestart_CR(float duration, QuickRestartOptions options)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DelayedQuickRestart_CRc__Iterator1()
      {
        duration = duration,
        options = options,
        _this = this
      };
    }

    public void QuickRestart(QuickRestartOptions options = default (QuickRestartOptions))
    {
      if (this.m_paused)
        this.ForceUnpause();
      this.m_loadingLevel = true;
      ChallengeManager componentInChildren = this.GetComponentInChildren<ChallengeManager>();
      if ((bool) (UnityEngine.Object) componentInChildren)
        UnityEngine.Object.Destroy((UnityEngine.Object) componentInChildren.gameObject);
      SaveManager.DeleteCurrentSlotMidGameSave();
      if (options.BossRush)
        this.CurrentGameMode = GameManager.GameMode.BOSSRUSH;
      else if (this.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
        this.CurrentGameMode = GameManager.GameMode.NORMAL;
      bool flag = GameManager.Instance.CurrentGameMode == GameManager.GameMode.SHORTCUT;
      if ((UnityEngine.Object) this.PrimaryPlayer != (UnityEngine.Object) null)
      {
        GameManager.ForceQuickRestartAlternateCostumeP1 = this.PrimaryPlayer.IsUsingAlternateCostume;
        GameManager.ForceQuickRestartAlternateGunP1 = this.PrimaryPlayer.UsingAlternateStartingGuns;
      }
      if (this.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (UnityEngine.Object) this.SecondaryPlayer != (UnityEngine.Object) null)
      {
        GameManager.ForceQuickRestartAlternateCostumeP2 = this.SecondaryPlayer.IsUsingAlternateCostume;
        GameManager.ForceQuickRestartAlternateGunP2 = this.SecondaryPlayer.UsingAlternateStartingGuns;
      }
      this.ClearPerLevelData();
      this.FlushAudio();
      this.ClearActiveGameData(false, true);
      if (this.TargetQuickRestartLevel != -1)
      {
        this.nextLevelIndex = this.TargetQuickRestartLevel;
      }
      else
      {
        this.nextLevelIndex = 1;
        if (flag)
        {
          this.nextLevelIndex += this.LastShortcutFloorLoaded;
          this.IsLoadingFirstShortcutFloor = true;
        }
      }
      if ((UnityEngine.Object) GameManager.LastUsedPlayerPrefab != (UnityEngine.Object) null)
      {
        GameManager.PlayerPrefabForNewGame = GameManager.LastUsedPlayerPrefab;
        GameStatsManager.Instance.BeginNewSession(GameManager.PlayerPrefabForNewGame.GetComponent<PlayerController>());
      }
      if (this.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (UnityEngine.Object) GameManager.LastUsedCoopPlayerPrefab != (UnityEngine.Object) null)
        GameManager.CoopPlayerPrefabForNewGame = GameManager.LastUsedCoopPlayerPrefab;
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
      this.m_preventUnpausing = false;
      if (this.m_currentRunSeed != 0)
      {
        this.m_forceSeedUpdate = true;
        this.CurrentRunSeed = this.CurrentRunSeed;
      }
      UnityEngine.Debug.Log((object) "Quick Restarting...");
      if (this.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
      {
        this.SetNextLevelIndex(1);
        this.InstantLoadBossRushFloor(1);
        ++this.nextLevelIndex;
      }
      else
        GameManager.Instance.LoadNextLevel();
      this.StartCoroutine(this.PostQuickStartCR(options));
    }

    [DebuggerHidden]
    private IEnumerator PostQuickStartCR(QuickRestartOptions options)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__PostQuickStartCRc__Iterator2()
      {
        options = options,
        _this = this
      };
    }

    public AsyncOperation BeginAsyncLoadMainMenu()
    {
      AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainMenu");
      asyncOperation.allowSceneActivation = false;
      return asyncOperation;
    }

    public void EndAsyncLoadMainMenu(AsyncOperation loader)
    {
      if (this.m_paused)
        this.ForceUnpause();
      this.ClearPerLevelData();
      this.FlushAudio();
      this.ClearActiveGameData(true, false);
      this.m_preventUnpausing = false;
      loader.allowSceneActivation = true;
    }

    public void DelayedLoadMainMenu(float duration)
    {
      this.StartCoroutine(this.DelayedLoadMainMenu_CR(duration));
    }

    [DebuggerHidden]
    private IEnumerator DelayedLoadMainMenu_CR(float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DelayedLoadMainMenu_CRc__Iterator3()
      {
        duration = duration,
        _this = this
      };
    }

    public void LoadMainMenu()
    {
      if (this.m_paused)
        this.ForceUnpause();
      this.m_loadingLevel = true;
      this.ClearPerLevelData();
      this.FlushAudio();
      this.ClearActiveGameData(true, true);
      BraveCameraUtility.OverrideAspect = new float?(1.77777779f);
      this.m_preventUnpausing = false;
      this.IsLoadingLevel = false;
      Foyer.DoIntroSequence = false;
      Foyer.DoMainMenu = true;
      SceneManager.LoadScene("tt_foyer");
    }

    public void FrameDelayedEnteredFoyer(PlayerController p)
    {
      if ((UnityEngine.Object) Foyer.Instance != (UnityEngine.Object) null)
        Foyer.Instance.ProcessPlayerEnteredFoyer(p);
      this.StartCoroutine(this.HandleFrameDelayedEnteredFoyer(p));
    }

    [DebuggerHidden]
    private IEnumerator HandleFrameDelayedEnteredFoyer(PlayerController p)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__HandleFrameDelayedEnteredFoyerc__Iterator4()
      {
        p = p
      };
    }

    public void DelayedReturnToFoyer(float delay)
    {
      this.m_preparingToDestroyThisGameManagerOnCollision = true;
      this.StartCoroutine(this.DelayedReturnToFoyer_CR(delay));
    }

    [DebuggerHidden]
    private IEnumerator DelayedReturnToFoyer_CR(float delay)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DelayedReturnToFoyer_CRc__Iterator5()
      {
        delay = delay,
        _this = this
      };
    }

    public void ReturnToFoyer()
    {
      if (this.m_paused)
        this.ForceUnpause();
      GameManager.IsReturningToFoyerWithPlayer = true;
      this.ClearPerLevelData();
      this.FlushAudio();
      this.nextLevelIndex = 1;
      this.ClearActiveGameData(false, true);
      if ((UnityEngine.Object) GameManager.LastUsedPlayerPrefab != (UnityEngine.Object) null)
      {
        GameManager.PlayerPrefabForNewGame = GameManager.LastUsedPlayerPrefab;
        GameStatsManager.Instance.BeginNewSession(GameManager.PlayerPrefabForNewGame.GetComponent<PlayerController>());
      }
      else
        UnityEngine.Debug.LogError((object) "Attempting to clear player data on foyer return, but LastUsedPlayer is null!");
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
      this.m_preventUnpausing = false;
      this.LoadNextLevel();
    }

    public void LoadCustomFlowForDebug(string flowpath, string dungeonPrefab = "", string sceneName = "")
    {
      DungeonFlow orLoadByName = FlowDatabase.GetOrLoadByName(flowpath);
      if ((UnityEngine.Object) orLoadByName == (UnityEngine.Object) null)
        orLoadByName = FlowDatabase.GetOrLoadByName("Boss Rooms/" + flowpath);
      if ((UnityEngine.Object) orLoadByName == (UnityEngine.Object) null)
        orLoadByName = FlowDatabase.GetOrLoadByName("Boss Rush Flows/" + flowpath);
      if ((UnityEngine.Object) orLoadByName == (UnityEngine.Object) null)
        orLoadByName = FlowDatabase.GetOrLoadByName("Testing/" + flowpath);
      if ((UnityEngine.Object) orLoadByName == (UnityEngine.Object) null)
        return;
      this.m_loadingLevel = true;
      this.FlushAudio();
      this.ClearPerLevelData();
      float num1 = 1f;
      float num2 = 1f;
      if (!string.IsNullOrEmpty(sceneName))
      {
        for (int index = 0; index < this.customFloors.Count; ++index)
        {
          if (this.customFloors[index].dungeonSceneName == sceneName)
          {
            num1 = this.customFloors[index].priceMultiplier;
            num2 = this.customFloors[index].enemyHealthMultiplier;
            break;
          }
        }
        for (int index = 0; index < this.dungeonFloors.Count; ++index)
        {
          if (this.dungeonFloors[index].dungeonSceneName == sceneName)
          {
            num1 = this.dungeonFloors[index].priceMultiplier;
            num2 = this.dungeonFloors[index].enemyHealthMultiplier;
            break;
          }
        }
      }
      GameLevelDefinition gld = new GameLevelDefinition();
      gld.dungeonPrefabPath = !string.IsNullOrEmpty(dungeonPrefab) ? dungeonPrefab : "Base_Gungeon";
      gld.dungeonSceneName = !string.IsNullOrEmpty(sceneName) ? sceneName : "BB_Beholster";
      gld.priceMultiplier = num1;
      gld.enemyHealthMultiplier = num2;
      UnityEngine.Debug.Log((object) $"{gld.dungeonPrefabPath}|{gld.dungeonSceneName}");
      gld.predefinedSeeds = new List<int>();
      gld.flowEntries = new List<DungeonFlowLevelEntry>();
      gld.flowEntries.Add(new DungeonFlowLevelEntry()
      {
        flowPath = flowpath,
        forceUseIfAvailable = true,
        prerequisites = new DungeonPrerequisite[0],
        weight = 1f
      });
      this.StartCoroutine(this.LoadNextLevelAsync_CR(gld));
    }

    public void LoadCustomLevel(string custom)
    {
      if (this.dungeonFloors == null || this.dungeonFloors.Count == 0)
      {
        this.dungeonFloors = new List<GameLevelDefinition>();
        this.dungeonFloors.Add(new GameLevelDefinition()
        {
          dungeonSceneName = SceneManager.GetActiveScene().name
        });
      }
      this.m_loadingLevel = true;
      this.FlushAudio();
      this.ClearPerLevelData();
      GameLevelDefinition gld = (GameLevelDefinition) null;
      int num = -1;
      for (int index = 0; index < this.dungeonFloors.Count; ++index)
      {
        if (this.dungeonFloors[index].dungeonSceneName == custom)
        {
          gld = this.dungeonFloors[index];
          num = index + 1;
          break;
        }
      }
      if (gld == null)
      {
        for (int index = 0; index < this.customFloors.Count; ++index)
        {
          if (this.customFloors[index].dungeonSceneName == custom)
          {
            gld = this.customFloors[index];
            break;
          }
        }
      }
      if (gld != null && gld.dungeonPrefabPath == string.Empty)
      {
        if (gld.dungeonSceneName == "MainMenu")
        {
          this.LoadMainMenu();
          this.nextLevelIndex = 0;
        }
        else if (gld.dungeonSceneName == "Foyer")
        {
          SceneManager.LoadScene(gld.dungeonSceneName);
          this.IsLoadingLevel = false;
          this.nextLevelIndex = 1;
        }
        else
        {
          SceneManager.LoadScene(gld.dungeonSceneName);
          this.IsLoadingLevel = false;
        }
      }
      else
      {
        this.StartCoroutine(this.LoadNextLevelAsync_CR(gld));
        if (gld.dungeonSceneName == "tt_tutorial")
          num = 0;
        if (num == -1)
          return;
        this.nextLevelIndex = num;
      }
    }

    public static void InvalidateMidgameSave(bool saveStats)
    {
      MidGameSaveData midgameSave = (MidGameSaveData) null;
      if (!GameManager.VerifyAndLoadMidgameSave(out midgameSave, false))
        return;
      midgameSave.Invalidate();
      SaveManager.Save<MidGameSaveData>(midgameSave, SaveManager.MidGameSave, GameStatsManager.Instance.PlaytimeMin);
      GameStatsManager.Instance.midGameSaveGuid = midgameSave.midGameSaveGuid;
      if (!saveStats)
        return;
      GameStatsManager.Save();
    }

    public static void RevalidateMidgameSave()
    {
      MidGameSaveData midgameSave = (MidGameSaveData) null;
      if (!GameManager.VerifyAndLoadMidgameSave(out midgameSave, false))
        return;
      midgameSave.Revalidate();
      SaveManager.Save<MidGameSaveData>(midgameSave, SaveManager.MidGameSave, GameStatsManager.Instance.PlaytimeMin);
      GameStatsManager.Instance.midGameSaveGuid = midgameSave.midGameSaveGuid;
      GameStatsManager.Save();
    }

    public static void DoMidgameSave(GlobalDungeonData.ValidTilesets tileset)
    {
      string midGameSaveGuid = Guid.NewGuid().ToString();
      SaveManager.Save<MidGameSaveData>(new MidGameSaveData(GameManager.Instance.PrimaryPlayer, GameManager.Instance.SecondaryPlayer, tileset, midGameSaveGuid), SaveManager.MidGameSave, GameStatsManager.Instance.PlaytimeMin);
      GameStatsManager.Instance.midGameSaveGuid = midGameSaveGuid;
      GameStatsManager.Save();
    }

    public static bool HasValidMidgameSave()
    {
      return GameManager.VerifyAndLoadMidgameSave(out MidGameSaveData _);
    }

    public static bool VerifyAndLoadMidgameSave(out MidGameSaveData midgameSave, bool checkValidity = true)
    {
      if (!SaveManager.Load<MidGameSaveData>(SaveManager.MidGameSave, out midgameSave, true))
      {
        UnityEngine.Debug.LogError((object) "No mid game save found");
        return false;
      }
      if (midgameSave == null)
      {
        UnityEngine.Debug.LogError((object) "Failed to load mid game save (0)");
        return false;
      }
      if (checkValidity && !midgameSave.IsValid())
        return false;
      if (GameStatsManager.Instance.midGameSaveGuid == null || GameStatsManager.Instance.midGameSaveGuid != midgameSave.midGameSaveGuid)
      {
        UnityEngine.Debug.LogError((object) "Failed to load mid game save (1)");
        return false;
      }
      if (!new List<string>((IEnumerable<string>) Brave.PlayerPrefs.GetStringArray("recent_mgs")).Contains(midgameSave.midGameSaveGuid))
        return true;
      UnityEngine.Debug.LogError((object) "Failed to load mid game save (2)");
      UnityEngine.Debug.LogError((object) Brave.PlayerPrefs.GetString("recent_mgs"));
      UnityEngine.Debug.LogError((object) midgameSave.midGameSaveGuid);
      return false;
    }

    public void DelayedLoadMidgameSave(float delay, MidGameSaveData saveToContinue)
    {
      switch (saveToContinue.levelSaved)
      {
        case GlobalDungeonData.ValidTilesets.SEWERGEON:
          this.DelayedLoadCustomLevel(delay, "tt_sewer");
          break;
        case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
          this.DelayedLoadCustomLevel(delay, "tt_cathedral");
          break;
        case GlobalDungeonData.ValidTilesets.OFFICEGEON:
          this.DelayedLoadCustomLevel(delay, "tt_nakatomi");
          break;
        case GlobalDungeonData.ValidTilesets.FINALGEON:
          switch (saveToContinue.playerOneData.CharacterIdentity)
          {
            case PlayableCharacters.Pilot:
              this.DelayedLoadCustomLevel(delay, "fs_pilot");
              return;
            case PlayableCharacters.Convict:
              this.DelayedLoadCustomLevel(delay, "fs_convict");
              return;
            case PlayableCharacters.Robot:
              this.DelayedLoadCustomLevel(delay, "fs_robot");
              return;
            case PlayableCharacters.Ninja:
              return;
            case PlayableCharacters.Cosmonaut:
              return;
            case PlayableCharacters.Soldier:
              this.DelayedLoadCustomLevel(delay, "fs_soldier");
              return;
            case PlayableCharacters.Guide:
              this.DelayedLoadCustomLevel(delay, "fs_guide");
              return;
            case PlayableCharacters.CoopCultist:
              return;
            case PlayableCharacters.Bullet:
              this.DelayedLoadCustomLevel(delay, "fs_bullet");
              return;
            case PlayableCharacters.Eevee:
              return;
            case PlayableCharacters.Gunslinger:
              GameManager.IsGunslingerPast = true;
              this.DelayedLoadCustomLevel(delay, "tt_bullethell");
              return;
            default:
              return;
          }
        case GlobalDungeonData.ValidTilesets.RATGEON:
          this.DelayedLoadCustomLevel(delay, "ss_resourcefulrat");
          break;
        default:
          this.DelayedLoadNextLevel(0.25f);
          break;
      }
    }

    public void GeneratePlayersFromMidGameSave(MidGameSaveData loadedSave)
    {
      GameManager.PlayerPrefabForNewGame = loadedSave.GetPlayerOnePrefab();
      GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(GameManager.PlayerPrefabForNewGame, Vector3.zero, Quaternion.identity);
      GameManager.PlayerPrefabForNewGame = (GameObject) null;
      gameObject1.SetActive(true);
      PlayerController component1 = gameObject1.GetComponent<PlayerController>();
      component1.ActorName = "Player ID 0";
      component1.PlayerIDX = 0;
      if (loadedSave.playerOneData.CostumeID == 1)
        component1.SwapToAlternateCostume();
      this.CurrentGameType = loadedSave.savedGameType;
      if (loadedSave != null && loadedSave.playerOneData != null)
      {
        if (loadedSave.playerOneData.passiveItems != null)
        {
          for (int index = 0; index < loadedSave.playerOneData.passiveItems.Count; ++index)
          {
            if (loadedSave.playerOneData.passiveItems[index].PickupID == GlobalItemIds.SevenLeafClover)
              PassiveItem.IncrementFlag(component1, typeof (SevenLeafCloverItem));
          }
        }
        component1.MasteryTokensCollectedThisRun = loadedSave.playerOneData.MasteryTokensCollected;
      }
      this.RefreshAllPlayers();
      if (this.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
        return;
      GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(ResourceCache.Acquire("PlayerCoopCultist") as GameObject, Vector3.zero, Quaternion.identity);
      GameManager.CoopPlayerPrefabForNewGame = (GameObject) null;
      gameObject2.SetActive(true);
      PlayerController component2 = gameObject2.GetComponent<PlayerController>();
      component2.ActorName = "Player ID 1";
      component2.PlayerIDX = 1;
      if (loadedSave.playerTwoData.CostumeID == 1)
        component2.SwapToAlternateCostume();
      if (loadedSave != null && loadedSave.playerTwoData != null)
      {
        if (loadedSave.playerTwoData.passiveItems != null)
        {
          for (int index = 0; index < loadedSave.playerTwoData.passiveItems.Count; ++index)
          {
            if (loadedSave.playerTwoData.passiveItems[index].PickupID == GlobalItemIds.SevenLeafClover)
              PassiveItem.IncrementFlag(component2, typeof (SevenLeafCloverItem));
          }
        }
        component2.MasteryTokensCollectedThisRun = loadedSave.playerTwoData.MasteryTokensCollected;
      }
      this.RefreshAllPlayers();
    }

    public void DelayedLoadCharacterSelect(float delay, bool unloadGameData = false, bool doMainMenu = false)
    {
      this.StartCoroutine(this.DelayedLoadCharacterSelect_CR(delay, unloadGameData, doMainMenu));
    }

    [DebuggerHidden]
    private IEnumerator DelayedLoadCharacterSelect_CR(
      float delay,
      bool unloadGameData,
      bool doMainMenu)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DelayedLoadCharacterSelect_CRc__Iterator6()
      {
        delay = delay,
        unloadGameData = unloadGameData,
        doMainMenu = doMainMenu,
        _this = this
      };
    }

    public void ClearPrimaryPlayer()
    {
      if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_player.gameObject);
      this.m_player = (PlayerController) null;
    }

    public void ClearSecondaryPlayer()
    {
      if ((UnityEngine.Object) this.m_secondaryPlayer != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_secondaryPlayer.gameObject);
      this.m_secondaryPlayer = (PlayerController) null;
      this.m_players = (PlayerController[]) null;
    }

    public void ClearPlayers()
    {
      if (this.m_players != null)
      {
        for (int index = 0; index < this.m_players.Length; ++index)
        {
          PlayerController player = this.m_players[index];
          if ((bool) (UnityEngine.Object) player)
          {
            player.specRigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Unknown;
            UnityEngine.Object.Destroy((UnityEngine.Object) player.gameObject);
          }
        }
        this.m_players = (PlayerController[]) null;
        this.m_player = (PlayerController) null;
        this.m_secondaryPlayer = (PlayerController) null;
      }
      else
      {
        if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
        {
          this.m_player.specRigidbody.PhysicsRegistration = SpeculativeRigidbody.RegistrationState.Unknown;
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_player.gameObject);
        }
        this.m_player = (PlayerController) null;
        this.m_secondaryPlayer = (PlayerController) null;
      }
    }

    public void LoadCharacterSelect(bool unloadGameData = false, bool doMainMenu = false)
    {
      if (this.m_paused)
        this.ForceUnpause();
      this.m_loadingLevel = true;
      GameManager.IsReturningToBreach = true;
      this.ClearPerLevelData();
      this.FlushAudio();
      this.ClearActiveGameData(false, true);
      this.m_preventUnpausing = false;
      Foyer.DoIntroSequence = false;
      Foyer.DoMainMenu = doMainMenu;
      GameManager.IsReturningToBreach = true;
      GameManager.SKIP_FOYER = false;
      this.InjectedLevelName = string.Empty;
      this.SetNextLevelIndex(0);
      this.m_preparingToDestroyThisGameManagerOnCollision = true;
      this.LoadNextLevel();
    }

    public void DelayedLoadBossrushFloor(float delay)
    {
      int nextLevelIndex = this.nextLevelIndex;
      ++this.nextLevelIndex;
      this.StartCoroutine(this.DelayedLoadBossrushFloor_CR(delay, nextLevelIndex));
    }

    public void DebugLoadBossrushFloor(int target)
    {
      this.StartCoroutine(this.DelayedLoadBossrushFloor_CR(0.5f, target));
    }

    [DebuggerHidden]
    private IEnumerator DelayedLoadBossrushFloor_CR(float delay, int bossrushTargetFloor)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DelayedLoadBossrushFloor_CRc__Iterator7()
      {
        delay = delay,
        bossrushTargetFloor = bossrushTargetFloor,
        _this = this
      };
    }

    private void InstantLoadBossRushFloor(int bossrushTargetFloor)
    {
      this.m_loadingLevel = true;
      if (this.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
      {
        switch (bossrushTargetFloor)
        {
          case 1:
            this.LoadCustomFlowForDebug("Bossrush_01_Castle", "Base_Castle", "tt_castle");
            break;
          case 2:
            this.LoadCustomFlowForDebug("Bossrush_01a_Sewer", "Base_Sewer", "tt_sewer");
            break;
          case 3:
            this.LoadCustomFlowForDebug("Bossrush_02_Gungeon", "Base_Gungeon", "tt5");
            break;
          case 4:
            this.LoadCustomFlowForDebug("Bossrush_02a_Cathedral", "Base_Cathedral", "tt_cathedral");
            break;
          case 5:
            this.LoadCustomFlowForDebug("Bossrush_03_Mines", "Base_Mines", "tt_mines");
            break;
          case 6:
            this.LoadCustomFlowForDebug("Bossrush_04_Catacombs", "Base_Catacombs", "tt_catacombs");
            break;
          case 7:
            this.LoadCustomFlowForDebug("Bossrush_05_Forge", "Base_Forge", "tt_forge");
            break;
          case 8:
            this.LoadCustomFlowForDebug("Bossrush_06_BulletHell", "Base_BulletHell", "tt_bullethell");
            break;
          default:
            this.LoadMainMenu();
            break;
        }
      }
      else
      {
        switch (bossrushTargetFloor)
        {
          case 1:
            this.LoadCustomFlowForDebug("Bossrush_01_Castle", "Base_Castle", "tt_castle");
            break;
          case 2:
            this.LoadCustomFlowForDebug("Bossrush_02_Gungeon", "Base_Gungeon", "tt5");
            break;
          case 3:
            this.LoadCustomFlowForDebug("Bossrush_03_Mines", "Base_Mines", "tt_mines");
            break;
          case 4:
            this.LoadCustomFlowForDebug("Bossrush_04_Catacombs", "Base_Catacombs", "tt_catacombs");
            break;
          case 5:
            this.LoadCustomFlowForDebug("Bossrush_05_Forge", "Base_Forge", "tt_forge");
            break;
          default:
            this.LoadMainMenu();
            break;
        }
      }
    }

    public void DelayedLoadCustomLevel(float delay, string customLevel)
    {
      this.StartCoroutine(this.DelayedLoadCustomLevel_CR(delay, customLevel));
    }

    [DebuggerHidden]
    private IEnumerator DelayedLoadCustomLevel_CR(float delay, string customLevel)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DelayedLoadCustomLevel_CRc__Iterator8()
      {
        delay = delay,
        customLevel = customLevel,
        _this = this
      };
    }

    public void DelayedLoadNextLevel(float delay)
    {
      this.StartCoroutine(this.DelayedLoadNextLevel_CR(delay));
    }

    [DebuggerHidden]
    private IEnumerator DelayedLoadNextLevel_CR(float delay)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DelayedLoadNextLevel_CRc__Iterator9()
      {
        delay = delay,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator LoadLevelByIndex(int nextIndex)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__LoadLevelByIndexc__IteratorA()
      {
        nextIndex = nextIndex,
        _this = this
      };
    }

    public void LoadNextLevel()
    {
      if (!string.IsNullOrEmpty(this.InjectedLevelName))
      {
        this.LoadCustomLevel(this.InjectedLevelName);
        this.InjectedLevelName = string.Empty;
      }
      else
      {
        if (GameManager.SKIP_FOYER && this.nextLevelIndex == 0)
          this.nextLevelIndex = 1;
        if (this.dungeonFloors == null || this.dungeonFloors.Count == 0)
        {
          this.dungeonFloors = new List<GameLevelDefinition>();
          this.dungeonFloors.Add(new GameLevelDefinition()
          {
            dungeonSceneName = SceneManager.GetActiveScene().name
          });
        }
        if (this.nextLevelIndex >= this.dungeonFloors.Count)
          this.nextLevelIndex = 0;
        this.m_loadingLevel = true;
        this.ClearPerLevelData();
        if (this.dungeonFloors[this.nextLevelIndex].dungeonPrefabPath == string.Empty)
        {
          if (this.dungeonFloors[this.nextLevelIndex].dungeonSceneName == "MainMenu")
          {
            this.LoadMainMenu();
            this.nextLevelIndex = 0;
          }
          else if (this.dungeonFloors[this.nextLevelIndex].dungeonSceneName == "Foyer")
            this.StartCoroutine(this.LoadLevelByIndex(this.nextLevelIndex + 1));
          else
            this.StartCoroutine(this.LoadLevelByIndex(0));
        }
        else
        {
          this.StartCoroutine(this.LoadNextLevelAsync_CR(this.dungeonFloors[this.nextLevelIndex]));
          ++this.nextLevelIndex;
        }
      }
    }

    public void DoGameOver(string gameOverSource = "")
    {
      this.PauseRaw(true);
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
      AmmonomiconController.Instance.OpenAmmonomicon(true, false);
    }

    [DebuggerHidden]
    private IEnumerator LoadGameOver_CR(string gameOverSource)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__LoadGameOver_CRc__IteratorB()
      {
        gameOverSource = gameOverSource,
        _this = this
      };
    }

    public GameLevelDefinition GetLevelDefinitionFromName(string levelName)
    {
      for (int index = 0; index < this.dungeonFloors.Count; ++index)
      {
        if (this.dungeonFloors[index].dungeonSceneName == levelName)
          return this.dungeonFloors[index];
      }
      for (int index = 0; index < this.customFloors.Count; ++index)
      {
        if (this.customFloors[index].dungeonSceneName == levelName)
          return this.customFloors[index];
      }
      return (GameLevelDefinition) null;
    }

    public GameLevelDefinition GetLastLoadedLevelDefinition()
    {
      return this.m_lastLoadedLevelDefinition == null ? this.GetLevelDefinitionFromName(SceneManager.GetActiveScene().name) : this.m_lastLoadedLevelDefinition;
    }

    [DebuggerHidden]
    private IEnumerator EndLoadNextLevelAsync_CR(
      AsyncOperation async,
      GameObject loadingSceneHierarchy,
      bool isHandoff = false)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__EndLoadNextLevelAsync_CRc__IteratorC()
      {
        async = async,
        loadingSceneHierarchy = loadingSceneHierarchy,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator LoadNextLevelAsync_CR(GameLevelDefinition gld)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__LoadNextLevelAsync_CRc__IteratorD()
      {
        gld = gld,
        _this = this
      };
    }

    public void Pause()
    {
      Minimap.Instance.ToggleMinimap(false);
      GameUIRoot.Instance.ShowPauseMenu();
      BraveMemory.HandleGamePaused();
      GameStatsManager.Instance.MoveSessionStatsToSavedSessionStats();
      this.PauseRaw();
      if (GameManager.Options.CurrentFullscreenStyle == GameOptions.FullscreenStyle.BORDERLESS)
      {
        GameCursorController component = GameUIRoot.Instance.GetComponent<GameCursorController>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.ToggleClip(false);
      }
      this.StartCoroutine(this.PixelateCR());
    }

    public void PauseRaw(bool preventUnpausing = false)
    {
      GameUIRoot.Instance.levelNameUI.BanishLevelNameText();
      GameUIRoot.Instance.ForceClearReload();
      GameUIRoot.Instance.ToggleLowerPanels(false, source: "gm_pause");
      GameUIRoot.Instance.HideCoreUI("gm_pause");
      GameUIRoot.Instance.ToggleAllDefaultLabels(false, "pause");
      BraveTime.RegisterTimeScaleMultiplier(0.0f, this.gameObject);
      if (!this.IsSelectingCharacter)
      {
        if (!this.MainCameraController.ManualControl)
        {
          this.MainCameraController.OverridePosition = this.MainCameraController.transform.position;
          this.MainCameraController.SetManualControl(true, false);
          this.m_pauseLockedCamera = true;
        }
        else
          this.m_pauseLockedCamera = false;
      }
      if (preventUnpausing)
        this.m_preventUnpausing = true;
      this.m_paused = true;
    }

    public void ReturnToBasePauseState() => GameUIRoot.Instance.ReturnToBasePause();

    public void Unpause()
    {
      this.m_paused = false;
      this.m_unpausedThisFrame = true;
      if (this.m_pauseLockedCamera)
        this.MainCameraController.SetManualControl(false);
      GameUIRoot.Instance.ToggleLowerPanels(true, source: "gm_pause");
      GameUIRoot.Instance.ShowCoreUI("gm_pause");
      GameUIRoot.Instance.HidePauseMenu();
      GameUIRoot.Instance.ToggleAllDefaultLabels(true, "pause");
      BraveInput.FlushAll();
      if (this.AllPlayers != null)
      {
        for (int index = 0; index < this.AllPlayers.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) this.AllPlayers[index])
            this.AllPlayers[index].WasPausedThisFrame = true;
        }
      }
      if (GameManager.Options.CurrentFullscreenStyle == GameOptions.FullscreenStyle.BORDERLESS)
      {
        GameCursorController component = GameUIRoot.Instance.GetComponent<GameCursorController>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.ToggleClip(true);
      }
      if ((double) Pixelator.Instance.saturation == 1.0)
        return;
      this.StartCoroutine(this.DepixelateCR());
    }

    public void ForceUnpause()
    {
      this.m_paused = false;
      if ((bool) (UnityEngine.Object) this.MainCameraController && this.m_pauseLockedCamera)
        this.MainCameraController.SetManualControl(false);
      if ((UnityEngine.Object) GameUIRoot.Instance != (UnityEngine.Object) null)
      {
        GameUIRoot.Instance.ToggleLowerPanels(true, source: "gm_pause");
        GameUIRoot.Instance.ShowCoreUI("gm_pause");
        GameUIRoot.Instance.HidePauseMenu();
      }
      BraveInput.FlushAll();
      if ((UnityEngine.Object) Pixelator.Instance != (UnityEngine.Object) null)
      {
        GameManager.Options.OverrideMotionEnhancementModeForPause = false;
        Pixelator.Instance.OnChangedMotionEnhancementMode(GameManager.Options.MotionEnhancementMode);
        Pixelator.Instance.overrideTileScale = 1;
        Pixelator.Instance.saturation = 1f;
      }
      BraveTime.ClearMultiplier(this.gameObject);
    }

    [DebuggerHidden]
    private IEnumerator PixelateCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__PixelateCRc__IteratorE()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator DepixelateCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__DepixelateCRc__IteratorF()
      {
        _this = this
      };
    }

    public static void AttemptSoundEngineInitialization()
    {
      if (GameManager.AUDIO_ENABLED)
        return;
      DebugTime.RecordStartTime();
      uint out_bankID;
      int num1 = (int) AkSoundEngine.LoadBank("SFX.bnk", -1, out out_bankID);
      DebugTime.Log("GameManager.ASEI.LoadBank(SFX)");
      UnityEngine.Debug.LogError((object) ("loaded bank id: " + (object) out_bankID));
      AkChannelConfig in_channelConfig = new AkChannelConfig();
      in_channelConfig.SetStandard(uint.MaxValue);
      int num2 = (int) AkSoundEngine.SetListenerSpatialization((GameObject) null, true, in_channelConfig);
      GameManager.AUDIO_ENABLED = true;
    }

    public static void AttemptSoundEngineInitializationAsync()
    {
      GameManager.c_asyncSoundStartTime = UnityEngine.Time.realtimeSinceStartup;
      GameManager.c_asyncSoundStartFrame = UnityEngine.Time.frameCount;
      // ISSUE: reference to a compiler-generated field
      if (GameManager._f__mg_cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GameManager._f__mg_cache0 = new AkCallbackManager.BankCallback(GameManager.BankCallback);
      }
      uint out_bankID;
      // ISSUE: reference to a compiler-generated field
      int num = (int) AkSoundEngine.LoadBank("SFX.bnk", GameManager._f__mg_cache0, (object) null, -1, out out_bankID);
      UnityEngine.Debug.LogError((object) ("async loading bank id: " + (object) out_bankID));
    }

    private static void BankCallback(
      uint in_bankID,
      IntPtr in_InMemoryBankPtr,
      AKRESULT in_eLoadResult,
      uint in_memPoolId,
      object in_Cookie)
    {
      DebugTime.Log(GameManager.c_asyncSoundStartTime, GameManager.c_asyncSoundStartFrame, "GameManager.ASEI.LoadBank(SFX)");
      AkChannelConfig in_channelConfig = new AkChannelConfig();
      in_channelConfig.SetStandard(uint.MaxValue);
      int num = (int) AkSoundEngine.SetListenerSpatialization((GameObject) null, true, in_channelConfig);
      GameManager.AUDIO_ENABLED = true;
    }

    protected void LoadDungeonFloorsFromTargetPrefab(string resourcePath, bool assignNextLevelIndex)
    {
      GameManager component = (BraveResources.Load(resourcePath) as GameObject).GetComponent<GameManager>();
      this.GlobalInjectionData = component.GlobalInjectionData;
      this.CurrentRewardManager = component.CurrentRewardManager;
      this.OriginalRewardManager = component.OriginalRewardManager;
      this.SynergyManager = component.SynergyManager;
      this.DefaultAlienConversationFont = component.DefaultAlienConversationFont;
      this.DefaultNormalConversationFont = component.DefaultNormalConversationFont;
      this.dungeonFloors = component.dungeonFloors;
      this.customFloors = component.customFloors;
      if (assignNextLevelIndex)
      {
        for (int index = 0; index < this.dungeonFloors.Count; ++index)
        {
          if (SceneManager.GetActiveScene().name == this.dungeonFloors[index].dungeonSceneName)
          {
            this.nextLevelIndex = index + 1;
            break;
          }
        }
      }
      this.COOP_ENEMY_HEALTH_MULTIPLIER = component.COOP_ENEMY_HEALTH_MULTIPLIER;
      this.COOP_ENEMY_PROJECTILE_SPEED_MULTIPLIER = component.COOP_ENEMY_PROJECTILE_SPEED_MULTIPLIER;
      this.PierceDamageScaling = component.PierceDamageScaling;
      this.BloodthirstOptions = component.BloodthirstOptions;
      this.EnemyReplacementTiers = component.EnemyReplacementTiers;
    }

    public void InitializeForRunWithSeed(int seed) => this.CurrentRunSeed = seed;

    private void Awake()
    {
      DebugTime.Log("GameManager.Awake()");
      this.gameObject.AddComponent<EarlyUpdater>();
      if (!Application.isEditor)
      {
        if (!GameManager.m_hasEnsuredHeapSize)
        {
          if (SystemInfo.systemMemorySize > 1000)
          {
            if (SystemInfo.systemMemorySize > 3500)
            {
              BraveMemory.EnsureHeapSize(204800 /*0x032000*/);
              GameManager.m_hasEnsuredHeapSize = true;
            }
            else
            {
              BraveMemory.EnsureHeapSize(102400 /*0x019000*/);
              GameManager.m_hasEnsuredHeapSize = true;
            }
          }
        }
      }
      try
      {
        UnityEngine.Debug.Log((object) ("Version: " + VersionManager.UniqueVersionNumber));
        UnityEngine.Debug.LogFormat("Now: {0:MM.dd.yyyy HH:mm}", (object) DateTime.Now);
      }
      catch (Exception ex)
      {
      }
      if (this.platformInterface == null)
        this.platformInterface = !PlatformInterfaceSteam.IsSteamBuild() ? (!PlatformInterfaceGalaxy.IsGalaxyBuild() ? (PlatformInterface) new PlatformInterfaceGenericPC() : (PlatformInterface) new PlatformInterfaceGalaxy()) : (PlatformInterface) new PlatformInterfaceSteam();
      this.platformInterface.Start();
      if (GameManager.Options == null)
        GameOptions.Load();
      string path = "_GameManager";
      DebugTime.RecordStartTime();
      if (this.dungeonFloors == null)
      {
        this.dungeonFloors = new List<GameLevelDefinition>();
        GameManager component = (BraveResources.Load(path) as GameObject).GetComponent<GameManager>();
        this.GlobalInjectionData = component.GlobalInjectionData;
        this.CurrentRewardManager = component.RewardManager;
        this.SynergyManager = component.SynergyManager;
        this.DefaultAlienConversationFont = component.DefaultAlienConversationFont;
        this.DefaultNormalConversationFont = component.DefaultNormalConversationFont;
      }
      DebugTime.Log("GameManager.Awake() load dungeon floors");
      GameManager[] objectsOfType = UnityEngine.Object.FindObjectsOfType<GameManager>();
      if (objectsOfType.Length > 1)
      {
        GameManager gameManager1 = (GameManager) null;
        GameManager gameManager2 = (GameManager) null;
        for (int index = 0; index < objectsOfType.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) objectsOfType[index] && ((UnityEngine.Object) gameManager1 == (UnityEngine.Object) null || objectsOfType[index].dungeonFloors.Count > gameManager1.dungeonFloors.Count) && !objectsOfType[index].m_shouldDestroyThisGameManagerOnCollision)
            gameManager1 = objectsOfType[index];
          if (objectsOfType[index].m_shouldDestroyThisGameManagerOnCollision)
            gameManager2 = objectsOfType[index];
        }
        if ((UnityEngine.Object) gameManager1 != (UnityEngine.Object) null && (UnityEngine.Object) gameManager2 != (UnityEngine.Object) null)
        {
          UnityEngine.Debug.Log((object) "continuing from where my father left off");
          if (!GameManager.IsReturningToFoyerWithPlayer)
            this.IsSelectingCharacter = true;
          gameManager1.StartCoroutine(gameManager1.EndLoadNextLevelAsync_CR(gameManager2.m_preDestroyAsyncHolder, gameManager2.m_preDestroyLoadingHierarchyHolder, true));
        }
        for (int index = 0; index < objectsOfType.Length; ++index)
        {
          if ((UnityEngine.Object) objectsOfType[index] != (UnityEngine.Object) gameManager1)
            UnityEngine.Object.Destroy((UnityEngine.Object) objectsOfType[index].gameObject);
        }
        GameManager.mr_manager = gameManager1;
        if (!(bool) (UnityEngine.Object) this)
          return;
      }
      if ((UnityEngine.Object) GameManager.m_inputManager == (UnityEngine.Object) null)
      {
        InControlManager objectOfType = UnityEngine.Object.FindObjectOfType<InControlManager>();
        if ((bool) (UnityEngine.Object) objectOfType)
        {
          GameManager.m_inputManager = objectOfType;
          UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) GameManager.m_inputManager.gameObject);
          InputManager.Enabled = true;
        }
        else
        {
          GameObject target = new GameObject("_InputManager");
          GameManager.m_inputManager = target.AddComponent<InControlManager>();
          UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) target);
          InputManager.Enabled = true;
        }
      }
      if ((UnityEngine.Object) this.m_dungeonMusicController == (UnityEngine.Object) null)
      {
        this.m_dungeonMusicController = this.GetComponent<DungeonFloorMusicController>();
        if (!(bool) (UnityEngine.Object) this.m_dungeonMusicController)
          this.m_dungeonMusicController = this.gameObject.AddComponent<DungeonFloorMusicController>();
      }
      DebugTime.RecordStartTime();
      GameStatsManager.Load();
      DebugTime.Log("GameManager.Awake() load game stats");
      if (!GameManager.AUDIO_ENABLED)
        GameManager.AttemptSoundEngineInitialization();
      UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) this.gameObject);
      UnityEngine.Debug.Log((object) "Post GameManager.Awake.AudioInit");
      if (GameStatsManager.Instance.IsInSession)
        this.StartEncounterableUnlockedChecks();
      switch (Application.platform)
      {
        case RuntimePlatform.WindowsPlayer:
        case RuntimePlatform.WindowsEditor:
        case RuntimePlatform.LinuxPlayer:
        case RuntimePlatform.MetroPlayerX86:
        case RuntimePlatform.MetroPlayerX64:
          GameManager.LoadResolutionFromOptions();
          break;
        case RuntimePlatform.PS4:
          GameManager.LoadResolutionFromPS4();
          break;
      }
      UnityEngine.Debug.Log((object) "Post GameManager.Awake.Resolution");
      StringTableManager.LoadTablesIfNecessary();
      this.RandomIntForCurrentRun = UnityEngine.Random.Range(0, 1000);
      UnityEngine.Debug.Log((object) "Terminating GameManager Awake()");
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__Startc__Iterator10()
      {
        _this = this
      };
    }

    private void HandleDeviceShift()
    {
      this.m_initializedDeviceCallbacks = true;
      BraveInput.ReassignAllControllers();
    }

    private void OnEnable() => GameManager.m_lastFrameRealtime = UnityEngine.Time.realtimeSinceStartup;

    protected override void OnDestroy()
    {
      base.OnDestroy();
      Shader.SetGlobalFloat("_MeduziReflectionsEnabled", 0.0f);
      GameManager[] objectsOfType = UnityEngine.Object.FindObjectsOfType<GameManager>();
      if (objectsOfType.Length < 1 || objectsOfType.Length == 1 && (UnityEngine.Object) objectsOfType[0] == (UnityEngine.Object) this)
      {
        if (GameStatsManager.Instance.IsInSession)
          GameStatsManager.Instance.EndSession(true);
        if (SaveManager.ResetSaveSlot)
          GameStatsManager.DANGEROUS_ResetAllStats();
        GameStatsManager.Save();
        if (GameManager.Options != null)
          GameOptions.Save();
        UnityEngine.Debug.LogWarning((object) "SAVING DATA");
        GameManager.Options = (GameOptions) null;
        if (SaveManager.TargetSaveSlot.HasValue)
        {
          SaveManager.ChangeSlot(SaveManager.TargetSaveSlot.Value);
          SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?();
        }
      }
      UnityInputDeviceManager.OnAllDevicesReattached -= new System.Action(this.HandleDeviceShift);
    }

    protected void InvariantUpdate(float realDeltaTime)
    {
      if (!this.m_bgChecksActive && GameStatsManager.Instance.IsInSession)
        this.StartEncounterableUnlockedChecks();
      if (!((UnityEngine.Object) this.m_dungeon != (UnityEngine.Object) null) || this.m_preventUnpausing || this.IsLoadingLevel || Foyer.DoMainMenu && this.IsFoyer)
        return;
      int num = this.AllPlayers.Length;
      if (this.IsSelectingCharacter)
        num = 1;
      for (int index = 0; index < num; ++index)
      {
        int playerIdx = !this.IsSelectingCharacter ? this.AllPlayers[index].PlayerIDX : 0;
        BraveInput braveInput = !this.IsSelectingCharacter ? BraveInput.GetInstanceForPlayer(playerIdx) : BraveInput.PlayerlessInstance;
        if (!((UnityEngine.Object) braveInput == (UnityEngine.Object) null) && braveInput.ActiveActions != null)
        {
          bool flag1 = braveInput.ActiveActions.PauseAction.WasPressed;
          if (Minimap.HasInstance && Minimap.Instance.HeldOpen)
            flag1 = false;
          if (braveInput.ActiveActions.EquipmentMenuAction.WasPressed)
          {
            bool flag2 = this.IsSelectingCharacter && Foyer.IsCurrentlyPlayingCharacterSelect;
            if (!this.m_paused && !AmmonomiconController.Instance.IsOpen && !flag2)
            {
              this.LastPausingPlayerID = playerIdx;
              this.Pause();
              GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().DoShowBestiary((dfControl) null, (dfMouseEventArgs) null);
            }
            else if ((!this.m_paused || AmmonomiconController.Instance.IsOpen) && this.m_paused && AmmonomiconController.Instance.IsOpen && !AmmonomiconController.Instance.IsClosing && !AmmonomiconController.Instance.IsOpening)
            {
              AmmonomiconController.Instance.CloseAmmonomicon();
              this.ReturnToBasePauseState();
              dfGUIManager.PushModal((dfControl) GameUIRoot.Instance.PauseMenuPanel);
              this.Unpause();
            }
          }
          else if (flag1)
          {
            if (this.m_paused)
            {
              if (!GameUIRoot.Instance.AreYouSurePanel.IsVisible && !GameUIRoot.Instance.KeepMetasIsVisible)
              {
                if (GameUIRoot.Instance.HasOpenPauseSubmenu())
                {
                  PauseMenuController component = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>();
                  if (!component.OptionsMenu.ModalKeyBindingDialog.IsVisible)
                  {
                    if (component.OptionsMenu.IsVisible || component.OptionsMenu.ModalKeyBindingDialog.IsVisible)
                    {
                      component.OptionsMenu.CloseAndMaybeApplyChangesWithPrompt();
                    }
                    else
                    {
                      component.ForceMaterialVisibility();
                      this.ReturnToBasePauseState();
                    }
                  }
                }
                else if (AmmonomiconController.Instance.IsOpen)
                {
                  if (!AmmonomiconController.Instance.IsTurningPage && !AmmonomiconController.Instance.IsOpening && !AmmonomiconController.Instance.IsClosing)
                  {
                    AmmonomiconController.Instance.CloseAmmonomicon();
                    this.ReturnToBasePauseState();
                    dfGUIManager.PushModal((dfControl) GameUIRoot.Instance.PauseMenuPanel);
                  }
                }
                else
                  this.Unpause();
              }
            }
            else
            {
              this.LastPausingPlayerID = playerIdx;
              this.Pause();
            }
          }
          else if (this.m_paused && braveInput.ActiveActions.CancelAction.WasPressed && !GameUIRoot.Instance.AreYouSurePanel.IsVisible && !GameUIRoot.Instance.KeepMetasIsVisible)
          {
            if (AmmonomiconController.Instance.IsOpen && !AmmonomiconController.Instance.IsClosing && !AmmonomiconController.Instance.BookmarkHasFocus)
              AmmonomiconController.Instance.ReturnFocusToBookmark();
            else if (GameUIRoot.Instance.HasOpenPauseSubmenu())
            {
              PauseMenuController component = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>();
              if (!component.OptionsMenu.ModalKeyBindingDialog.IsVisible)
              {
                if (component.OptionsMenu.IsVisible || component.OptionsMenu.ModalKeyBindingDialog.IsVisible)
                  component.OptionsMenu.CloseAndMaybeApplyChangesWithPrompt();
                else
                  this.ReturnToBasePauseState();
              }
            }
            else if (AmmonomiconController.Instance.IsOpen && !AmmonomiconController.Instance.IsClosing)
            {
              if (!AmmonomiconController.Instance.IsTurningPage)
              {
                AmmonomiconController.Instance.CloseAmmonomicon();
                this.ReturnToBasePauseState();
                dfGUIManager.PushModal((dfControl) GameUIRoot.Instance.PauseMenuPanel);
              }
            }
            else
              this.Unpause();
          }
        }
      }
    }

    public void FlushMusicAudio()
    {
      int num = (int) AkSoundEngine.PostEvent("Stop_MUS_All", this.gameObject);
      if (!(bool) (UnityEngine.Object) this.m_dungeonMusicController)
        return;
      this.m_dungeonMusicController.ClearCoreMusicEventID();
    }

    public void FlushAudio()
    {
      int num1 = (int) AkSoundEngine.PostEvent("Stop_SND_All", this.gameObject);
      int num2 = (int) AkSoundEngine.ClearPreparedEvents();
      AkSoundEngine.StopAll();
      if (!(bool) (UnityEngine.Object) this.m_dungeonMusicController)
        return;
      this.m_dungeonMusicController.ClearCoreMusicEventID();
    }

    public void ClearPerLevelData()
    {
      BraveCameraUtility.OverrideAspect = new float?();
      SuperReaperController.PreventShooting = false;
      BossKillCam.BossDeathCamRunning = false;
      PickupObject.ItemIsBeingTakenByRat = false;
      this.LastUsedInputDeviceForConversation = (InputDevice) null;
      BossManager.PriorFloorSelectedBossRoom = (PrototypeDungeonRoom) null;
      if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null)
      {
        for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
          GameManager.Instance.Dungeon.data.rooms[index].SetRoomActive(true);
      }
      this.m_dungeon = (Dungeon) null;
      this.m_camera = (CameraController) null;
      GameUIRoot.Instance = (GameUIRoot) null;
      if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
        this.m_player.ClearPerLevelData();
      CheckEntireFloorVisited.IsQuestCallbackActive = false;
      SunglassesItem.SunglassesActive = false;
      AmmonomiconController.Instance = (AmmonomiconController) null;
      TileVFXManager.Instance = (TileVFXManager) null;
      this.InTutorial = false;
      BossKillCam.ClearPerLevelData();
      LootEngine.ClearPerLevelData();
      RoomHandler.unassignedInteractableObjects.Clear();
      ShadowSystem.ClearPerLevelData();
      SecretRoomUtility.ClearPerLevelData();
      DeadlyDeadlyGoopManager.ClearPerLevelData();
      BroController.ClearPerLevelData();
      GlobalSparksDoer.CleanupOnSceneTransition();
      SilencerInstance.s_MaxRadiusLimiter = new float?();
      TextBoxManager.ClearPerLevelData();
      SpawnManager.LastPrefabPool = (PrefabPool) null;
      TimeTubeCreditsController.ClearPerLevelData();
      GameManager.PVP_ENABLED = false;
      if (TK2DTilemapChunkAnimator.PositionToAnimatorMap != null)
        TK2DTilemapChunkAnimator.PositionToAnimatorMap.Clear();
      SecretRoomDoorBeer.AllSecretRoomDoors = (List<SecretRoomDoorBeer>) null;
      DebrisObject.ClearPerLevelData();
      ExplosionManager.ClearPerLevelData();
      StaticReferenceManager.ClearStaticPerLevelData();
      CollisionData.Pool.Clear();
      LinearCastResult.Pool.Clear();
      RaycastResult.Pool.Clear();
      SpriteChunk.ClearPerLevelData();
      AIActor.ClearPerLevelData();
      TalkDoerLite.ClearPerLevelData();
      Pathfinder.ClearPerLevelData();
      TakeCoverBehavior.ClearPerLevelData();
      if ((UnityEngine.Object) Pixelator.Instance != (UnityEngine.Object) null)
        Pixelator.Instance.ClearCachedFrame();
      this.ExtantShopTrackableGuids.Clear();
      EnemyDatabase.Instance.DropReferences();
      EncounterDatabase.Instance.DropReferences();
      GameStatsManager.Instance.MoveSessionStatsToSavedSessionStats();
      GameStatsManager.Save();
    }

    public void ClearActiveGameData(bool destroyGameManager, bool endSession)
    {
      GameStatsManager.Instance.CurrentEeveeEquipSeed = -1;
      PickupObject.RatBeatenAtPunchout = false;
      BraveCameraUtility.OverrideAspect = new float?();
      SuperReaperController.PreventShooting = false;
      BossKillCam.BossDeathCamRunning = false;
      this.ClearPlayers();
      GameManager.IsCoopPast = false;
      GameManager.IsGunslingerPast = false;
      Exploder.OnExplosionTriggered = (System.Action) null;
      MetaInjectionData.ClearBlueprint();
      RewardManifest.ClearManifest(this.RewardManager);
      RewardManager.AdditionalHeartTierMagnificence = 0.0f;
      BossManager.HasOverriddenCoreBoss = false;
      RoomHandler.HasGivenRoomChestRewardThisRun = false;
      if (PassiveItem.ActiveFlagItems != null)
        PassiveItem.ActiveFlagItems.Clear();
      GameManager.PVP_ENABLED = false;
      Gun.ActiveReloadActivated = false;
      Gun.ActiveReloadActivatedPlayerTwo = false;
      SecretHandshakeItem.NumActive = 0;
      BossKillCam.ClearPerLevelData();
      BaseShopController.HasLockedShopProcedurally = false;
      Chest.HasDroppedResourcefulRatNoteThisSession = false;
      Chest.DoneResourcefulRatMimicThisSession = false;
      Chest.HasDroppedSerJunkanThisSession = false;
      Chest.ToggleCoopChests(false);
      PlayerItem.AllowDamageCooldownOnActive = false;
      AIActor.HealthModifier = 1f;
      Projectile.BaseEnemyBulletSpeedMultiplier = 1f;
      Projectile.ResetGlobalProjectileDepth();
      BasicBeamController.ResetGlobalBeamHeight();
      if ((bool) (UnityEngine.Object) this.MainCameraController)
        this.MainCameraController.enabled = false;
      this.m_lastLoadedLevelDefinition = (GameLevelDefinition) null;
      Cursor.visible = true;
      this.nextLevelIndex = 0;
      StaticReferenceManager.ForceClearAllStaticMemory();
      if (endSession)
        GameStatsManager.Instance.EndSession(true);
      if (!destroyGameManager)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    public static void BroadcastRoomFsmEvent(string eventName)
    {
      List<PlayMakerFSM> componentsAbsoluteInRoom = GameManager.Instance.BestActivePlayer.CurrentRoom.GetComponentsAbsoluteInRoom<PlayMakerFSM>();
      for (int index = 0; index < componentsAbsoluteInRoom.Count; ++index)
        componentsAbsoluteInRoom[index].SendEvent(eventName);
    }

    public static void BroadcastRoomFsmEvent(string eventName, RoomHandler targetRoom)
    {
      List<PlayMakerFSM> componentsAbsoluteInRoom = targetRoom.GetComponentsAbsoluteInRoom<PlayMakerFSM>();
      for (int index = 0; index < componentsAbsoluteInRoom.Count; ++index)
        componentsAbsoluteInRoom[index].SendEvent(eventName);
    }

    public static void BroadcastRoomTalkDoerFsmEvent(string eventName)
    {
      for (int index = 0; index < StaticReferenceManager.AllNpcs.Count; ++index)
      {
        TalkDoerLite allNpc = StaticReferenceManager.AllNpcs[index];
        if ((bool) (UnityEngine.Object) allNpc && (bool) (UnityEngine.Object) GameManager.Instance.BestActivePlayer && GameManager.Instance.BestActivePlayer.CurrentRoom == allNpc.ParentRoom)
          allNpc.SendPlaymakerEvent(eventName);
      }
    }

    public void StartEncounterableUnlockedChecks()
    {
      this.m_bgChecksActive = true;
      this.ConstructSetOfKnownUnlocks();
      this.StartCoroutine(this.EncounterableUnlockedChecks());
    }

    private void ConstructSetOfKnownUnlocks()
    {
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < EncounterDatabase.Instance.Entries.Count; ++index)
      {
        EncounterDatabaseEntry entry = EncounterDatabase.Instance.Entries[index];
        if (entry != null && !entry.journalData.SuppressInAmmonomicon && !EncounterDatabase.IsProxy(entry.myGuid))
        {
          ++num1;
          if (entry.PrerequisitesMet())
          {
            ++num2;
            if (entry.prerequisites == null || entry.prerequisites.Length == 0 || GameStatsManager.Instance.QueryEncounterableAnnouncement(entry.myGuid))
              this.m_knownEncounterables.Add(entry.myGuid);
            else
              this.m_queuedUnlocks.Add(entry.myGuid);
          }
          else if (entry.prerequisites != null && entry.prerequisites.Length > 0)
          {
            PickupObject byId = PickupObjectDatabase.GetById(entry.pickupObjectId);
            if ((UnityEngine.Object) byId == (UnityEngine.Object) null || byId.quality == PickupObject.ItemQuality.EXCLUDED)
              ++num2;
            else if (entry.prerequisites.Length == 1 && entry.prerequisites[0].requireFlag && entry.prerequisites[0].saveFlagToCheck == GungeonFlags.COOP_PAST_REACHED)
              ++num2;
          }
        }
      }
      if (num1 > num2 + 1)
        return;
      GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE, true);
    }

    public List<EncounterDatabaseEntry> GetQueuedTrackables()
    {
      List<EncounterDatabaseEntry> queuedTrackables = new List<EncounterDatabaseEntry>(this.m_queuedUnlocks.Count + this.m_newQueuedUnlocks.Count);
      for (int index = 0; index < this.m_queuedUnlocks.Count; ++index)
        queuedTrackables.Add(EncounterDatabase.GetEntry(this.m_queuedUnlocks[index]));
      for (int index = 0; index < this.m_newQueuedUnlocks.Count; ++index)
        queuedTrackables.Add(EncounterDatabase.GetEntry(this.m_newQueuedUnlocks[index]));
      return queuedTrackables;
    }

    public void AcknowledgeKnownTrackable(EncounterDatabaseEntry trackable)
    {
      GameStatsManager.Instance.MarkEncounterableAnnounced(trackable);
      this.m_queuedUnlocks.Remove(trackable.myGuid);
      this.m_newQueuedUnlocks.Remove(trackable.myGuid);
      this.m_knownEncounterables.Add(trackable.myGuid);
    }

    [DebuggerHidden]
    private IEnumerator EncounterableUnlockedChecks()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__EncounterableUnlockedChecksc__Iterator11()
      {
        _this = this
      };
    }

    public void LaunchTimedEvent(float allowedTime, Action<bool> valueSetter)
    {
      this.StartCoroutine(this.HandleCustomTimer(allowedTime, valueSetter));
    }

    [DebuggerHidden]
    private IEnumerator HandleCustomTimer(float allowedTime, Action<bool> valueSetter)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameManager__HandleCustomTimerc__Iterator12()
      {
        valueSetter = valueSetter,
        allowedTime = allowedTime
      };
    }

    public static int GetHashByComputerID()
    {
      int savedSystemHash = GameStatsManager.Instance.savedSystemHash;
      if (savedSystemHash != -1)
        return savedSystemHash;
      int hashCode = SystemInfo.deviceUniqueIdentifier.GetHashCode();
      GameStatsManager.Instance.savedSystemHash = hashCode;
      return hashCode;
    }

    public static DungeonData.Direction[] GetResourcefulRatSolution()
    {
      System.Random random = new System.Random(GameManager.GetHashByComputerID());
      DungeonData.Direction[] resourcefulRatSolution = new DungeonData.Direction[6];
      for (int index = 0; index < 6; ++index)
      {
        int num = random.Next(0, 4);
        if (index == 0 && num == 3)
          num = random.Next(0, 3);
        switch (num)
        {
          case 0:
            resourcefulRatSolution[index] = DungeonData.Direction.NORTH;
            break;
          case 1:
            resourcefulRatSolution[index] = DungeonData.Direction.EAST;
            break;
          case 2:
            resourcefulRatSolution[index] = DungeonData.Direction.SOUTH;
            break;
          case 3:
            resourcefulRatSolution[index] = DungeonData.Direction.WEST;
            break;
          default:
            UnityEngine.Debug.LogError((object) "Error in RR Solution");
            break;
        }
      }
      return resourcefulRatSolution;
    }

    public enum ControlType
    {
      KEYBOARD,
      CONTROLLER,
    }

    public enum GameMode
    {
      NORMAL,
      SHORTCUT,
      BOSSRUSH,
      SUPERBOSSRUSH,
    }

    public enum GameType
    {
      SINGLE_PLAYER,
      COOP_2_PLAYER,
    }

    public enum LevelOverrideState
    {
      NONE,
      FOYER,
      TUTORIAL,
      RESOURCEFUL_RAT,
      END_TIMES,
      CHARACTER_PAST,
      DEBUG_TEST,
    }
  }

