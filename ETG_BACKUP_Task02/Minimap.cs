// Decompiled with JetBrains decompiler
// Type: Minimap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using tk2dRuntime.TileMap;
using UnityEngine;

#nullable disable
public class Minimap : MonoBehaviour
{
  [NonSerialized]
  public bool PreventAllTeleports;
  public tk2dTileMap tilemap;
  public TileIndexGrid indexGrid;
  public TileIndexGrid darkIndexGrid;
  public TileIndexGrid redIndexGrid;
  public TileIndexGrid CurrentRoomBorderIndexGrid;
  public Camera cameraRef;
  public MinimapUIController UIMinimap;
  public float targetSaturation = 0.3f;
  [NonSerialized]
  public float currentXRectFactor = 1f;
  [NonSerialized]
  public float currentYRectFactor = 1f;
  private bool[,] m_simplifiedData;
  private List<Tuple<Transform, Renderer>> m_playerMarkers = new List<Tuple<Transform, Renderer>>();
  private static float SCALE_FACTOR = 15f;
  [SerializeField]
  private Material m_mapMaskMaterial;
  private Texture m_itemsMaskTex;
  private Texture2D m_whiteTex;
  private Dictionary<RoomHandler, List<GameObject>> roomToIconsMap = new Dictionary<RoomHandler, List<GameObject>>();
  private Dictionary<RoomHandler, bool> roomHasMovedTeleportIcon = new Dictionary<RoomHandler, bool>();
  private Dictionary<RoomHandler, GameObject> roomToTeleportIconMap = new Dictionary<RoomHandler, GameObject>();
  private Dictionary<RoomHandler, Vector3> roomToInitialTeleportIconPositionMap = new Dictionary<RoomHandler, Vector3>();
  public List<RoomHandler> roomsContainingTeleporters = new List<RoomHandler>();
  private Dictionary<RoomHandler, GameObject> roomToUnknownIconMap = new Dictionary<RoomHandler, GameObject>();
  private Vector3 m_cameraBasePosition;
  private Vector3 m_cameraPanOffset;
  private float m_cameraOrthoBase;
  private static float m_cameraOrthoOffset;
  private float m_currentMinimapZoom;
  private bool m_isFullscreen;
  private static Minimap m_instance;
  private bool m_isAutoPanning;
  public bool TemporarilyPreventMinimap;
  private bool[,] m_revealProcessedMap;
  protected bool m_shouldBuildTilemap;
  protected bool m_isInitialized;
  private float m_cachedFadeValue = 1f;
  private bool m_isFaded;
  private bool m_isTransitioning;

  public Minimap.MinimapDisplayMode MinimapMode
  {
    get
    {
      return this.PreventMinimap || GameManager.Instance.IsFoyer || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.RESOURCEFUL_RAT || (bool) (UnityEngine.Object) GameManager.Instance.BestActivePlayer && GameManager.Instance.BestActivePlayer.CurrentRoom != null && GameManager.Instance.BestActivePlayer.CurrentRoom.PreventMinimapUpdates ? Minimap.MinimapDisplayMode.NEVER : GameManager.Options.MinimapDisplayMode;
    }
  }

  public static bool DoMinimap
  {
    get
    {
      return !GameManager.Instance.IsLoadingLevel && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.END_TIMES && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.CHARACTER_PAST && !TextBoxManager.ExtantTextBoxVisible && !TimeTubeCreditsController.IsTimeTubing;
    }
  }

  public Dictionary<RoomHandler, GameObject> RoomToTeleportMap => this.roomToTeleportIconMap;

  public bool IsFullscreen => this.m_isFullscreen;

  public static Minimap Instance
  {
    get
    {
      if ((UnityEngine.Object) Minimap.m_instance == (UnityEngine.Object) null)
        Minimap.m_instance = UnityEngine.Object.FindObjectOfType<Minimap>();
      return Minimap.m_instance;
    }
    set => Minimap.m_instance = value;
  }

  public static bool HasInstance => (UnityEngine.Object) Minimap.m_instance != (UnityEngine.Object) null;

  public bool HeldOpen { get; set; }

  public bool this[int x, int y]
  {
    get
    {
      return this.m_simplifiedData != null && x >= 0 && y >= 0 && x < this.m_simplifiedData.GetLength(0) && y < this.m_simplifiedData.GetLength(1) && this.m_simplifiedData[x, y];
    }
  }

  private void AssignColorToTile(int ix, int iy, int layer, Color32 color, tk2dTileMap map)
  {
    if (!map.HasColorChannel())
      map.CreateColorChannel();
    ColorChannel colorChannel = map.ColorChannel;
    map.data.Layers[layer].useColor = true;
    colorChannel.SetColor(ix, iy, (Color) color);
  }

  private void ToggleMinimapRat(bool fullscreen, bool holdOpen = false)
  {
    this.cameraRef.cullingMask = 0;
    GameUIRoot.Instance.notificationController.ForceHide();
    GameUIRoot.Instance.ToggleAllDefaultLabels(!fullscreen, "minimap");
    this.m_isFullscreen = fullscreen;
    this.HeldOpen = holdOpen;
    if (fullscreen)
    {
      this.m_cachedFadeValue = !this.m_isFaded ? 1f : 0.0f;
      this.m_mapMaskMaterial.SetFloat("_Fade", 1f);
      this.currentXRectFactor = 1f;
      this.currentYRectFactor = 1f;
    }
    else
    {
      this.m_mapMaskMaterial.SetFloat("_Fade", this.m_cachedFadeValue);
      this.currentXRectFactor = 0.25f;
      this.currentYRectFactor = 0.25f;
      BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);
    }
    Shader.SetGlobalFloat("_FullMapActive", !fullscreen ? 0.0f : 1f);
    this.UpdateScale();
    if (fullscreen)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_UI_map_open_01", this.gameObject);
    }
    this.m_cameraPanOffset = Vector3.zero;
    if (fullscreen)
    {
      Pixelator.Instance.FadeColor = Color.black;
      Pixelator.Instance.fade = 0.3f;
      GameUIRoot.Instance.HideCoreUI(string.Empty);
      GameUIRoot.Instance.UnfoldGunventory(GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER);
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        GameUIRoot.Instance.ToggleItemPanels(false);
      this.UIMinimap.ToggleState(true);
    }
    else
    {
      Pixelator.Instance.FadeColor = Color.black;
      Pixelator.Instance.fade = 1f;
      GameUIRoot.Instance.ShowCoreUI(string.Empty);
      GameUIRoot.Instance.RefoldGunventory();
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        GameUIRoot.Instance.ToggleItemPanels(true);
      this.UIMinimap.ToggleState(false);
    }
    if (!this.m_isFullscreen)
      return;
    this.m_cameraBasePosition = this.GetCameraBasePosition();
    this.cameraRef.transform.position = this.m_cameraBasePosition + this.m_cameraPanOffset;
  }

  public void ToggleMinimap(bool fullscreen, bool holdOpen = false)
  {
    if (!fullscreen)
      this.HeldOpen = false;
    bool flag = GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.RESOURCEFUL_RAT;
    if (this.PreventMinimap && !flag)
      return;
    if (flag)
      this.cameraRef.cullingMask = 0;
    GameUIRoot.Instance.notificationController.ForceHide();
    GameUIRoot.Instance.ToggleAllDefaultLabels(!fullscreen, "minimap");
    this.m_isFullscreen = fullscreen;
    this.HeldOpen = holdOpen;
    if (fullscreen)
    {
      this.m_cachedFadeValue = !this.m_isFaded ? 1f : 0.0f;
      this.m_mapMaskMaterial.SetFloat("_Fade", 1f);
      this.currentXRectFactor = 1f;
      this.currentYRectFactor = 1f;
    }
    else
    {
      this.m_mapMaskMaterial.SetFloat("_Fade", this.m_cachedFadeValue);
      this.currentXRectFactor = 0.25f;
      this.currentYRectFactor = 0.25f;
      BraveInput.ConsumeAllAcrossInstances(GungeonActions.GungeonActionType.Shoot);
    }
    Shader.SetGlobalFloat("_FullMapActive", !fullscreen ? 0.0f : 1f);
    this.UpdateScale();
    if (fullscreen)
    {
      int num = (int) AkSoundEngine.PostEvent("Play_UI_map_open_01", this.gameObject);
    }
    this.m_cameraPanOffset = Vector3.zero;
    if (fullscreen)
    {
      Pixelator.Instance.FadeColor = Color.black;
      Pixelator.Instance.fade = 0.3f;
      GameUIRoot.Instance.HideCoreUI(string.Empty);
      GameUIRoot.Instance.UnfoldGunventory(GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER);
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        GameUIRoot.Instance.ToggleItemPanels(false);
      this.UIMinimap.ToggleState(true);
    }
    else
    {
      Pixelator.Instance.FadeColor = Color.black;
      Pixelator.Instance.fade = 1f;
      GameUIRoot.Instance.ShowCoreUI(string.Empty);
      GameUIRoot.Instance.RefoldGunventory();
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
        GameUIRoot.Instance.ToggleItemPanels(true);
      this.UIMinimap.ToggleState(false);
    }
    if (!this.m_isFullscreen)
      return;
    this.m_cameraBasePosition = this.GetCameraBasePosition();
    this.cameraRef.transform.position = this.m_cameraBasePosition + this.m_cameraPanOffset;
  }

  private Vector3 GetCameraBasePosition()
  {
    if (this.m_playerMarkers == null || this.m_playerMarkers.Count == 0)
      return Vector3.zero;
    Vector3 zero = Vector3.zero;
    int num = 0;
    for (int index = 0; index < this.m_playerMarkers.Count; ++index)
    {
      if ((index != 0 || !((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null) || !GameManager.Instance.PrimaryPlayer.healthHaver.IsDead) && (index != 1 || !((UnityEngine.Object) GameManager.Instance.SecondaryPlayer != (UnityEngine.Object) null) || !GameManager.Instance.SecondaryPlayer.healthHaver.IsDead))
      {
        ++num;
        zero += this.m_playerMarkers[index].First.position;
      }
    }
    return (zero / (float) num).WithZ(-5f);
  }

  public void AttemptPanCamera(Vector3 delta)
  {
    this.m_cameraPanOffset += delta * this.cameraRef.orthographicSize;
  }

  public bool IsPanning => this.m_isAutoPanning;

  public void PanToPosition(Vector3 position)
  {
    this.StartCoroutine(this.HandleAutoPan((position - this.m_cameraBasePosition).WithZ(0.0f)));
  }

  [DebuggerHidden]
  private IEnumerator HandleAutoPan(Vector3 targetPan)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Minimap.\u003CHandleAutoPan\u003Ec__Iterator0()
    {
      targetPan = targetPan,
      \u0024this = this
    };
  }

  public void TogglePresetZoomValue()
  {
    Minimap.m_cameraOrthoOffset = (double) Minimap.m_cameraOrthoOffset != 0.0 ? ((double) Minimap.m_cameraOrthoOffset != 4.25 ? ((double) Minimap.m_cameraOrthoOffset != 8.5 ? 0.0f : 0.0f) : 8.5f) : 4.25f;
    GameManager.Options.PreferredMapZoom = Minimap.m_cameraOrthoOffset;
  }

  public void AttemptZoomCamera(float zoom)
  {
    Minimap.m_cameraOrthoOffset = Mathf.Clamp(Minimap.m_cameraOrthoOffset + zoom * 2f, -2f, 9f);
    GameManager.Options.PreferredMapZoom = Minimap.m_cameraOrthoOffset;
  }

  public void AttemptZoomMinimap(float zoom)
  {
    this.m_currentMinimapZoom = Mathf.Clamp(this.m_currentMinimapZoom + zoom * 2f, -1f, 4f);
    GameManager.Options.PreferredMinimapZoom = this.m_currentMinimapZoom;
  }

  private bool PreventMinimap
  {
    get
    {
      return GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.RESOURCEFUL_RAT || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || (bool) (UnityEngine.Object) GameUIRoot.Instance && GameUIRoot.Instance.DisplayingConversationBar || this.TemporarilyPreventMinimap;
    }
  }

  public void InitializeMinimap(DungeonData data)
  {
    if (this.PreventMinimap)
      return;
    TK2DDungeonAssembler.RuntimeResizeTileMap(this.tilemap, data.Width, data.Height, this.tilemap.partitionSizeX, this.tilemap.partitionSizeY);
    for (int ix = 0; ix < data.Width; ++ix)
    {
      for (int iy = 0; iy < data.Height; ++iy)
      {
        Color color = new Color(1f, 1f, 1f, 0.75f);
        this.AssignColorToTile(ix, iy, 0, (Color32) color, this.tilemap);
      }
    }
    this.tilemap.ForceBuild();
    this.m_cameraBasePosition = this.tilemap.transform.position + new Vector3((float) ((double) data.Width / 2.0 * 0.125), (float) ((double) data.Height / 2.0 * 0.125), -5f);
    this.cameraRef.transform.position = this.m_cameraBasePosition;
  }

  public void UpdatePlayerPositionExact(
    Vector3 worldPosition,
    PlayerController player,
    bool isDying = false)
  {
    if (this.PreventMinimap)
      return;
    if (this.m_playerMarkers == null)
      this.m_playerMarkers = new List<Tuple<Transform, Renderer>>();
    if (this.m_playerMarkers.Count == 0)
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GameManager.Instance.AllPlayers[index].minimapIconPrefab, this.transform.position, Quaternion.identity);
        gameObject.transform.parent = this.transform;
        tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
        this.m_playerMarkers.Add(new Tuple<Transform, Renderer>(gameObject.transform, component.renderer));
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.renderer.sortingLayerName = "Foreground";
      }
    }
    Vector3 vector = this.transform.position + new Vector3(worldPosition.x * 0.125f, worldPosition.y * 0.125f, -1f);
    int index1 = !player.IsPrimaryPlayer ? 1 : 0;
    if ((bool) (UnityEngine.Object) player && player.CurrentRoom != null && player.CurrentRoom.PreventMinimapUpdates)
    {
      if (index1 < this.m_playerMarkers.Count)
        this.m_playerMarkers[index1].Second.enabled = false;
      if (!this.m_isFullscreen)
        this.m_cameraBasePosition = this.GetCameraBasePosition().Quantize(1f / 16f) + CameraController.PLATFORM_CAMERA_OFFSET;
      this.cameraRef.transform.position = this.m_cameraBasePosition + this.m_cameraPanOffset;
    }
    else
    {
      if (index1 < this.m_playerMarkers.Count)
      {
        this.m_playerMarkers[index1].First.position = vector.Quantize(1f / 16f);
        this.m_playerMarkers[index1].Second.enabled = !isDying && !player.healthHaver.IsDead;
      }
      if (!this.m_isFullscreen)
        this.m_cameraBasePosition = this.GetCameraBasePosition().Quantize(1f / 16f) + CameraController.PLATFORM_CAMERA_OFFSET;
      this.cameraRef.transform.position = this.m_cameraBasePosition + this.m_cameraPanOffset;
    }
  }

  private void PixelQuantizeCameraPosition()
  {
    Vector3 position = this.cameraRef.transform.position;
    float multiplesOf1 = (float) (1.0 / ((double) this.cameraRef.orthographicSize * 2.0 * 16.0));
    float multiplesOf2 = (float) (16.0 / ((double) this.cameraRef.orthographicSize * 2.0 * 16.0 * 9.0));
    this.cameraRef.transform.position = position.WithX(BraveMathCollege.QuantizeFloat(position.x, multiplesOf2)).WithY(BraveMathCollege.QuantizeFloat(position.y, multiplesOf1));
  }

  public void RevealAllRooms(bool revealSecretRooms)
  {
    if (this.PreventMinimap)
      return;
    for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
    {
      RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index];
      if (room.connectedRooms.Count != 0 && (revealSecretRooms || room.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.SECRET))
      {
        room.RevealedOnMap = true;
        this.RevealMinimapRoom(room, true, false, room == GameManager.Instance.PrimaryPlayer.CurrentRoom);
      }
    }
    for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
    {
      RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index];
      if (room.connectedRooms.Count != 0 && this.roomToUnknownIconMap.ContainsKey(room))
        UnityEngine.Object.Destroy((UnityEngine.Object) this.roomToUnknownIconMap[room]);
    }
    this.roomToUnknownIconMap.Clear();
    this.StartCoroutine(this.DelayedMarkDirty());
  }

  [DebuggerHidden]
  private IEnumerator DelayedMarkDirty()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Minimap.\u003CDelayedMarkDirty\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  public void DeflagPreviousRoom(RoomHandler previousRoom)
  {
    if (this.PreventMinimap)
      return;
    this.RevealMinimapRoom(previousRoom, true, isCurrentRoom: false);
  }

  private void DrawUnknownRoomSquare(
    RoomHandler current,
    RoomHandler adjacent,
    bool doBuild = true,
    int overrideCellIndex = -1,
    bool isLockedDoor = false)
  {
    if (this.PreventMinimap || adjacent.IsSecretRoom || adjacent.RevealedOnMap)
      return;
    int tile = overrideCellIndex == -1 ? 49 : overrideCellIndex;
    RuntimeExitDefinition forConnectedRoom = adjacent.GetExitDefinitionForConnectedRoom(current);
    IntVector2 cellAdjacentToExit = adjacent.GetCellAdjacentToExit(forConnectedRoom);
    IntVector2 intVector2_1 = IntVector2.Zero;
    RuntimeRoomExitData runtimeRoomExitData = forConnectedRoom.upstreamRoom != adjacent ? forConnectedRoom.downstreamExit : forConnectedRoom.upstreamExit;
    if (runtimeRoomExitData != null && runtimeRoomExitData.referencedExit != null)
      intVector2_1 = DungeonData.GetIntVector2FromDirection(runtimeRoomExitData.referencedExit.exitDirection);
    if (cellAdjacentToExit == IntVector2.Zero)
      return;
    for (int index1 = -1; index1 < 3; ++index1)
    {
      for (int index2 = -1; index2 < 3; ++index2)
        this.tilemap.SetTile(cellAdjacentToExit.x + index1, cellAdjacentToExit.y + index2, 0, tile);
    }
    IntVector2 intVector2_2 = cellAdjacentToExit + IntVector2.Left;
    IntVector2 intVector2_3 = cellAdjacentToExit + IntVector2.Left;
    GameObject gameObject1 = (GameObject) null;
    GameObject gameObject2;
    if (!adjacent.area.IsProceduralRoom && (UnityEngine.Object) adjacent.area.runtimePrototypeData.associatedMinimapIcon != (UnityEngine.Object) null)
    {
      gameObject2 = UnityEngine.Object.Instantiate<GameObject>(adjacent.area.runtimePrototypeData.associatedMinimapIcon);
      if (isLockedDoor)
      {
        gameObject1 = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Minimap_Locked_Icon"));
        intVector2_3 = intVector2_3 + IntVector2.Right + IntVector2.Down + intVector2_1 * 6;
      }
    }
    else if (isLockedDoor)
    {
      gameObject2 = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Minimap_Locked_Icon"));
      intVector2_2 = intVector2_2 + IntVector2.Right + IntVector2.Down;
    }
    else
      gameObject2 = overrideCellIndex == -1 ? (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Minimap_Unknown_Icon")) : (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("Minimap_Blocked_Icon"));
    gameObject2.transform.parent = this.transform;
    gameObject2.transform.position = this.transform.position + intVector2_2.ToVector3() * 0.125f;
    if (this.roomToUnknownIconMap.ContainsKey(adjacent))
      gameObject2.transform.parent = this.roomToUnknownIconMap[adjacent].transform;
    else
      this.roomToUnknownIconMap.Add(adjacent, gameObject2);
    if (!((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null))
      return;
    gameObject1.transform.parent = this.roomToUnknownIconMap[adjacent].transform;
    gameObject1.transform.position = this.transform.position + intVector2_3.ToVector3() * 0.125f;
  }

  private void UpdateTeleporterIconForRoom(RoomHandler targetRoom)
  {
    if (this.PreventMinimap || !this.roomToTeleportIconMap.ContainsKey(targetRoom) || !targetRoom.TeleportersActive)
      return;
    tk2dBaseSprite component = this.roomToTeleportIconMap[targetRoom].GetComponent<tk2dBaseSprite>();
    if (!(component.GetCurrentSpriteDef().name == "teleport_001"))
      return;
    component.SetSprite("teleport_active_001");
  }

  public void RevealMinimapRoom(
    RoomHandler revealedRoom,
    bool force = false,
    bool doBuild = true,
    bool isCurrentRoom = true)
  {
    if ((bool) (UnityEngine.Object) revealedRoom.OverrideTilemap)
      return;
    this.StartCoroutine(this.RevealMinimapRoomInternal(revealedRoom, force, doBuild, isCurrentRoom));
  }

  [DebuggerHidden]
  public IEnumerator RevealMinimapRoomInternal(
    RoomHandler revealedRoom,
    bool force = false,
    bool doBuild = true,
    bool isCurrentRoom = true)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Minimap.\u003CRevealMinimapRoomInternal\u003Ec__Iterator2()
    {
      revealedRoom = revealedRoom,
      isCurrentRoom = isCurrentRoom,
      force = force,
      doBuild = doBuild,
      \u0024this = this
    };
  }

  private void Start()
  {
    Minimap.m_cameraOrthoOffset = GameManager.Options.PreferredMapZoom;
    this.m_currentMinimapZoom = GameManager.Options.PreferredMinimapZoom;
    this.HandleInitialization();
  }

  private void HandleInitialization()
  {
    if (this.m_isInitialized)
      return;
    this.m_isInitialized = true;
    this.cameraRef.enabled = true;
    this.m_mapMaskMaterial = this.cameraRef.GetComponent<MinimapRenderer>().QuadTransform.GetComponent<MeshRenderer>().sharedMaterial;
    this.m_whiteTex = new Texture2D(1, 1);
    this.m_whiteTex.SetPixel(0, 0, Color.white);
    this.m_whiteTex.Apply();
    if (GameManager.Instance.IsFoyer || this.MinimapMode == Minimap.MinimapDisplayMode.NEVER)
    {
      this.m_isFaded = true;
      this.m_cachedFadeValue = 0.0f;
      this.m_mapMaskMaterial.SetFloat("_Fade", 0.0f);
    }
    this.ToggleMinimap(false);
  }

  private void UpdateScale()
  {
    if (this.m_isFullscreen)
    {
      if ((double) this.m_cameraOrthoBase != (double) GameManager.Instance.MainCameraController.GetComponent<Camera>().orthographicSize)
        this.m_cameraOrthoBase = GameManager.Instance.MainCameraController.GetComponent<Camera>().orthographicSize;
      if ((double) this.cameraRef.orthographicSize != (double) this.m_cameraOrthoBase + (double) Minimap.m_cameraOrthoOffset)
        this.cameraRef.orthographicSize = this.m_cameraOrthoBase + Minimap.m_cameraOrthoOffset;
      this.cameraRef.orthographicSize = BraveMathCollege.QuantizeFloat(this.cameraRef.orthographicSize, 0.5f);
    }
    else
      this.cameraRef.orthographicSize = 135f / 64f + this.m_currentMinimapZoom;
  }

  private void LateUpdate()
  {
    this.UpdateScale();
    if (this.MinimapMode == Minimap.MinimapDisplayMode.NEVER || !Minimap.DoMinimap || this.TemporarilyPreventMinimap || GameManager.Instance.IsPaused)
    {
      if (!this.m_isFaded)
        this.StartCoroutine(this.TransitionToNewFadeState(true));
    }
    else if (this.MinimapMode == Minimap.MinimapDisplayMode.FADE_ON_ROOM_SEAL)
      this.CheckRoomSealState();
    else if (this.MinimapMode == Minimap.MinimapDisplayMode.ALWAYS && this.m_isFaded)
      this.StartCoroutine(this.TransitionToNewFadeState(false));
    bool flag = GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.RESOURCEFUL_RAT;
    if (!this.m_shouldBuildTilemap || flag)
      return;
    this.m_shouldBuildTilemap = false;
    this.tilemap.Build(tk2dTileMap.BuildFlags.Default);
  }

  private void CheckRoomSealState()
  {
    if ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null || GameManager.Instance.IsFoyer)
      return;
    RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
    if (currentRoom == null)
      return;
    if (currentRoom.IsSealed && !this.m_isFaded)
      this.StartCoroutine(this.TransitionToNewFadeState(true));
    else if (!currentRoom.IsSealed && this.m_isFaded)
      this.StartCoroutine(this.TransitionToNewFadeState(false));
    else if (currentRoom.IsSealed && this.m_isFaded && !this.m_isTransitioning)
    {
      this.m_mapMaskMaterial.SetFloat("_Fade", !this.m_isFullscreen ? 0.0f : 1f);
    }
    else
    {
      if (currentRoom.IsSealed || this.m_isFaded || this.m_isTransitioning)
        return;
      this.m_mapMaskMaterial.SetFloat("_Fade", 1f);
    }
  }

  [DebuggerHidden]
  private IEnumerator TransitionToNewFadeState(bool newFadeState)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new Minimap.\u003CTransitionToNewFadeState\u003Ec__Iterator3()
    {
      newFadeState = newFadeState,
      \u0024this = this
    };
  }

  public RoomHandler CheckIconsNearCursor(Vector3 screenPosition, out GameObject icon)
  {
    Vector2 viewportPosition = (Vector2) BraveUtility.GetMinimapViewportPosition((Vector2) screenPosition);
    viewportPosition.x = (float) (((double) viewportPosition.x - 0.5) * ((double) BraveCameraUtility.ASPECT / 1.7777777910232544) + 0.5);
    IntVector2 intVector2 = ((this.cameraRef.ViewportPointToRay((Vector3) viewportPosition).origin.XY() - this.transform.position.XY()) * 8f).ToIntVector2(VectorConversions.Floor);
    if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
    {
      RoomHandler parentRoom = GameManager.Instance.Dungeon.data[intVector2].parentRoom;
      if (parentRoom != null && this.roomToTeleportIconMap.ContainsKey(parentRoom))
      {
        icon = this.roomToTeleportIconMap[parentRoom];
        return parentRoom;
      }
    }
    icon = (GameObject) null;
    return (RoomHandler) null;
  }

  public RoomHandler GetNearestVisibleRoom(Vector2 screenPosition, out float dist)
  {
    float num1 = screenPosition.x / (float) Screen.width;
    float num2 = screenPosition.y / (float) Screen.height;
    Vector2 vector2 = this.cameraRef.ViewportPointToRay(new Vector3((float) (((double) num1 - 0.5) / (double) BraveCameraUtility.GetRect().width + 0.5), (float) (((double) num2 - 0.5) / (double) BraveCameraUtility.GetRect().height + 0.5), 0.0f)).origin.XY();
    dist = float.MaxValue;
    RoomHandler nearestVisibleRoom = (RoomHandler) null;
    foreach (RoomHandler key in this.roomToTeleportIconMap.Keys)
    {
      if (key.TeleportersActive)
      {
        GameObject roomToTeleportIcon = this.roomToTeleportIconMap[key];
        UnityEngine.Debug.DrawLine((Vector3) vector2, (Vector3) roomToTeleportIcon.GetComponent<tk2dBaseSprite>().WorldCenter, Color.red, 5f);
        float num3 = Vector2.Distance(vector2, roomToTeleportIcon.GetComponent<tk2dBaseSprite>().WorldCenter);
        if ((double) num3 < (double) dist)
        {
          dist = num3;
          nearestVisibleRoom = key;
        }
      }
    }
    return nearestVisibleRoom;
  }

  private void OrganizeExtantIcons(RoomHandler targetRoom, bool includeTeleIcon = false)
  {
    if (!this.roomToIconsMap.ContainsKey(targetRoom) && !this.roomToTeleportIconMap.ContainsKey(targetRoom))
    {
      UnityEngine.Debug.LogError((object) $"ORGANIZING ROOM: {targetRoom.GetRoomName()} IN MINIMAP WITH NO ICONS, TELL BR.NET");
    }
    else
    {
      List<GameObject> roomToIcons = !this.roomToIconsMap.ContainsKey(targetRoom) ? (List<GameObject>) null : this.roomToIconsMap[targetRoom];
      if (this.roomHasMovedTeleportIcon.ContainsKey(targetRoom))
        includeTeleIcon = true;
      bool flag = this.roomToTeleportIconMap.ContainsKey(targetRoom) && includeTeleIcon;
      int num1 = (roomToIcons != null ? roomToIcons.Count : 0) + (!flag ? 0 : 1);
      float num2 = 6f;
      float num3 = (float) (num1 - 1) * num2 / 2f;
      IntVector2 centerCell = targetRoom.GetCenterCell();
      for (int index = 0; index < num1; ++index)
      {
        GameObject gameObject = !flag || index != num1 - 1 ? roomToIcons[index] : this.roomToTeleportIconMap[targetRoom];
        if ((bool) (UnityEngine.Object) gameObject)
        {
          tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
          if ((bool) (UnityEngine.Object) component)
          {
            Vector3 vector3 = new Vector3(num2 * (float) index - num3, 0.0f, 0.0f);
            Vector3 position = this.transform.position + (centerCell.ToVector3() + vector3) * 0.125f + new Vector3(0.0f, 0.0f, 1f);
            component.PlaceAtPositionByAnchor(position, tk2dBaseSprite.Anchor.MiddleCenter);
          }
        }
      }
      for (int index = 0; index < num1; ++index)
      {
        GameObject gameObject = !flag || index != num1 - 1 ? roomToIcons[index] : this.roomToTeleportIconMap[targetRoom];
        if ((bool) (UnityEngine.Object) gameObject)
          gameObject.transform.position = gameObject.transform.position.Quantize(1f / 64f);
      }
      if (!includeTeleIcon && this.roomToTeleportIconMap.ContainsKey(targetRoom) && num1 > 0)
      {
        tk2dBaseSprite component1 = this.roomToTeleportIconMap[targetRoom].GetComponent<tk2dBaseSprite>();
        float a = float.MaxValue;
        for (int index = 0; index < num1; ++index)
        {
          tk2dBaseSprite component2 = roomToIcons[index].GetComponent<tk2dBaseSprite>();
          a = Mathf.Min(a, Vector2.Distance(component2.WorldCenter, component1.WorldCenter));
        }
        if ((double) a > 0.375)
          return;
        this.roomHasMovedTeleportIcon.Add(targetRoom, true);
        this.OrganizeExtantIcons(targetRoom, true);
      }
      else
      {
        if (!this.roomToTeleportIconMap.ContainsKey(targetRoom) || num1 != 0)
          return;
        this.roomToTeleportIconMap[targetRoom].transform.position = this.roomToInitialTeleportIconPositionMap[targetRoom];
      }
    }
  }

  private void AddIconToRoomList(RoomHandler room, GameObject instanceIcon)
  {
    if (this.roomToIconsMap.ContainsKey(room))
      this.roomToIconsMap[room].Add(instanceIcon);
    else
      this.roomToIconsMap.Add(room, new List<GameObject>()
      {
        instanceIcon
      });
    this.OrganizeExtantIcons(room);
  }

  private void RemoveIconFromRoomList(RoomHandler room, GameObject instanceIcon)
  {
    if (!this.roomToIconsMap.ContainsKey(room) || !this.roomToIconsMap[room].Remove(instanceIcon))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) instanceIcon);
    this.OrganizeExtantIcons(room);
  }

  public bool HasTeleporterIcon(RoomHandler room) => this.roomToTeleportIconMap.ContainsKey(room);

  private void ClampIconToRoomBounds(
    RoomHandler room,
    GameObject instanceIcon,
    Vector2 placedPosition)
  {
    Vector2 vector2_1 = this.transform.position.XY() + room.area.basePosition.ToVector2() * 0.125f;
    Vector2 vector2_2 = this.transform.position.XY() + (room.area.basePosition.ToVector2() + room.area.dimensions.ToVector2()) * 0.125f;
    Vector2 min = vector2_1 + Vector2.one * 0.5f;
    Vector2 max = vector2_2 - Vector2.one * 0.5f;
    placedPosition = BraveMathCollege.ClampToBounds(placedPosition, min, max);
    instanceIcon.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((Vector3) placedPosition, tk2dBaseSprite.Anchor.MiddleCenter);
  }

  public void RegisterTeleportIcon(RoomHandler room, GameObject iconPrefab, Vector2 position)
  {
    GameObject instanceIcon = UnityEngine.Object.Instantiate<GameObject>(iconPrefab);
    Vector2 vector2 = (Vector2) (this.transform.position + position.ToVector3ZUp() * 0.125f + new Vector3(0.0f, 0.0f, 1f));
    instanceIcon.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((Vector3) vector2, tk2dBaseSprite.Anchor.MiddleCenter);
    this.ClampIconToRoomBounds(room, instanceIcon, vector2);
    instanceIcon.transform.position = instanceIcon.transform.position.WithZ(-1f);
    instanceIcon.transform.parent = this.transform;
    instanceIcon.SetActive(room.visibility != RoomHandler.VisibilityStatus.OBSCURED);
    this.roomsContainingTeleporters.Add(room);
    this.roomToTeleportIconMap.Add(room, instanceIcon);
    this.roomToInitialTeleportIconPositionMap.Add(room, instanceIcon.transform.position);
    this.roomsContainingTeleporters.Sort((Comparison<RoomHandler>) ((a, b) =>
    {
      Vector2 teleportIconPosition1 = (Vector2) this.roomToInitialTeleportIconPositionMap[a];
      Vector2 teleportIconPosition2 = (Vector2) this.roomToInitialTeleportIconPositionMap[b];
      return (double) teleportIconPosition1.y == (double) teleportIconPosition2.y ? teleportIconPosition1.x.CompareTo(teleportIconPosition2.x) : teleportIconPosition1.y.CompareTo(teleportIconPosition2.y);
    }));
    this.OrganizeExtantIcons(room);
  }

  public GameObject RegisterRoomIcon(RoomHandler room, GameObject iconPrefab, bool forceActive = false)
  {
    if ((UnityEngine.Object) iconPrefab == (UnityEngine.Object) null)
      return (GameObject) null;
    GameObject instanceIcon = UnityEngine.Object.Instantiate<GameObject>(iconPrefab);
    instanceIcon.transform.position = instanceIcon.transform.position.WithZ(-1f);
    instanceIcon.transform.parent = this.transform;
    if (forceActive)
      instanceIcon.SetActive(true);
    else
      instanceIcon.SetActive(room.visibility != RoomHandler.VisibilityStatus.OBSCURED);
    this.AddIconToRoomList(room, instanceIcon);
    return instanceIcon;
  }

  public void DeregisterRoomIcon(RoomHandler room, GameObject instanceIcon)
  {
    this.RemoveIconFromRoomList(room, instanceIcon);
  }

  public void OnDestroy() => Minimap.m_instance = (Minimap) null;

  public RoomHandler NextSelectedTeleporter(ref int selectedIndex, int dir)
  {
    selectedIndex = Mathf.Clamp(selectedIndex, 0, this.RoomToTeleportMap.Count - 1);
    int index = selectedIndex;
    do
    {
      index = (index + dir + this.RoomToTeleportMap.Count) % this.RoomToTeleportMap.Count;
      RoomHandler containingTeleporter = this.roomsContainingTeleporters[index];
      if (containingTeleporter.TeleportersActive)
      {
        selectedIndex = index;
        return containingTeleporter;
      }
    }
    while (index != selectedIndex);
    return (RoomHandler) null;
  }

  public enum MinimapDisplayMode
  {
    NEVER,
    ALWAYS,
    FADE_ON_ROOM_SEAL,
  }
}
