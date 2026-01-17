// Decompiled with JetBrains decompiler
// Type: SecretRoomManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class SecretRoomManager : MonoBehaviour
{
  public SecretRoomManager.SecretRoomRevealStyle revealStyle = SecretRoomManager.SecretRoomRevealStyle.ShootToBreak;
  public Renderer ceilingRenderer;
  public Renderer borderRenderer;
  public Renderer aoRenderer;
  public RoomHandler room;
  public List<SecretRoomDoorBeer> doorObjects = new List<SecretRoomDoorBeer>();
  private List<IntVector2> ceilingCells;
  private SimpleSecretRoomTrigger m_simpleTrigger;
  private bool m_isOpen;

  public bool IsOpen
  {
    get => this.m_isOpen;
    set
    {
      if (!this.m_isOpen && value)
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.SECRET_ROOMS_FOUND, 1f);
      this.m_isOpen = value;
    }
  }

  public bool OpenedByExplosion
  {
    get => this.revealStyle != SecretRoomManager.SecretRoomRevealStyle.ComplexPuzzle;
  }

  public void InitializeCells(List<IntVector2> ceilingCellList)
  {
    this.ceilingCells = ceilingCellList;
  }

  public void InitializeForStyle()
  {
    for (int index = 0; index < this.doorObjects.Count; ++index)
      this.doorObjects[index].manager = this;
    switch (this.revealStyle)
    {
      case SecretRoomManager.SecretRoomRevealStyle.Simple:
        this.InitializeSimple();
        break;
      case SecretRoomManager.SecretRoomRevealStyle.ComplexPuzzle:
        this.InitializeSecretRoomPuzzle();
        break;
      case SecretRoomManager.SecretRoomRevealStyle.ShootToBreak:
        this.InitializeShootToBreak();
        break;
    }
    for (int index = 0; index < this.doorObjects.Count; ++index)
      this.doorObjects[index].GeneratePotentiallyNecessaryShards();
    for (int index = 0; index < this.doorObjects.Count; ++index)
      this.doorObjects[index].exitDef.GenerateSecretRoomBlocker(GameManager.Instance.Dungeon.data, this, this.doorObjects[index], (Transform) null);
  }

  public void HandleDoorBrokenOpen(SecretRoomDoorBeer doorBroken)
  {
    this.ceilingRenderer.enabled = false;
    if ((Object) this.borderRenderer != (Object) null)
      this.borderRenderer.enabled = false;
    for (int index = 0; index < this.ceilingCells.Count; ++index)
    {
      if (GameManager.Instance.Dungeon.data[this.ceilingCells[index]] != null)
        GameManager.Instance.Dungeon.data[this.ceilingCells[index]].isSecretRoomCell = false;
    }
    for (int index = 0; index < this.doorObjects.Count; ++index)
    {
      foreach (IntVector2 key in this.doorObjects[index].exitDef.GetCellsForRoom(this.room))
        GameManager.Instance.Dungeon.data[key].isSecretRoomCell = false;
      foreach (IntVector2 key in this.doorObjects[index].exitDef.GetCellsForOtherRoom(this.room))
        GameManager.Instance.Dungeon.data[key].isSecretRoomCell = false;
    }
    for (int index = 0; index < this.doorObjects.Count; ++index)
    {
      if ((Object) this.doorObjects[index].subsidiaryBlocker != (Object) null)
        this.doorObjects[index].subsidiaryBlocker.ToggleRenderers(true);
    }
    this.room.visibility = RoomHandler.VisibilityStatus.VISITED;
    Minimap.Instance.RevealMinimapRoom(this.room, true, isCurrentRoom: false);
    double num = (double) Pixelator.Instance.ProcessOcclusionChange(doorBroken.transform.position.IntXY(), 0.3f, this.room);
    for (int index = 0; index < this.doorObjects.Count; ++index)
      Pixelator.Instance.ProcessRoomAdditionalExits(this.doorObjects[index].exitDef.GetUpstreamBasePosition(), this.doorObjects[index].linkedRoom, false);
    doorBroken.gameObject.SetActive(false);
    this.OnFinishedOpeningDoors();
    if (!((Object) this.m_simpleTrigger != (Object) null))
      return;
    this.m_simpleTrigger.Disable();
  }

  protected void InitializeSimple()
  {
    for (int index = 0; index < this.doorObjects.Count; ++index)
    {
      IntVector2 key = this.doorObjects[index].collider.colliderObject.transform.position.IntXY(VectorConversions.Floor);
      IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(this.doorObjects[index].collider.exitDirection);
      CellData cellData = GameManager.Instance.Dungeon.data[key];
      RoomHandler roomHandler = cellData.parentRoom != null ? cellData.parentRoom : cellData.nearestRoom;
      CellData nearestFaceOrSidewall = roomHandler.GetNearestFaceOrSidewall(key + vector2FromDirection);
      IntVector2 zero = IntVector2.Zero;
      bool flag = false;
      GameObject original;
      IntVector2 intVector2;
      if (!nearestFaceOrSidewall.IsSideWallAdjacent())
      {
        original = GameManager.Instance.Dungeon.SecretRoomSimpleTriggersFacewall[Random.Range(0, GameManager.Instance.Dungeon.SecretRoomSimpleTriggersFacewall.Count)];
        intVector2 = IntVector2.Up;
      }
      else
      {
        original = GameManager.Instance.Dungeon.SecretRoomSimpleTriggersSidewall[Random.Range(0, GameManager.Instance.Dungeon.SecretRoomSimpleTriggersSidewall.Count)];
        intVector2 = IntVector2.Right + IntVector2.Up;
        if (GameManager.Instance.Dungeon.data[nearestFaceOrSidewall.position + IntVector2.Right].type == CellType.WALL)
          flag = true;
        else
          intVector2 += IntVector2.Left;
      }
      GameObject gameObject = Object.Instantiate<GameObject>(original);
      gameObject.transform.parent = roomHandler.hierarchyParent;
      if (flag)
        gameObject.GetComponent<tk2dSprite>().FlipX = true;
      nearestFaceOrSidewall.cellVisualData.containsObjectSpaceStamp = true;
      nearestFaceOrSidewall.cellVisualData.containsWallSpaceStamp = true;
      GameManager.Instance.Dungeon.data[nearestFaceOrSidewall.position + IntVector2.Up].cellVisualData.containsObjectSpaceStamp = true;
      GameManager.Instance.Dungeon.data[nearestFaceOrSidewall.position + IntVector2.Up].cellVisualData.containsWallSpaceStamp = true;
      gameObject.transform.position = (nearestFaceOrSidewall.position + intVector2).ToVector3();
      SimpleSecretRoomTrigger secretRoomTrigger = gameObject.AddComponent<SimpleSecretRoomTrigger>();
      secretRoomTrigger.referencedSecretRoom = this;
      secretRoomTrigger.parentRoom = roomHandler;
      gameObject.GetComponent<Renderer>().sortingLayerName = "Background";
      gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Critical"));
      gameObject.GetComponent<tk2dSprite>().UpdateZDepth();
      secretRoomTrigger.Initialize();
      this.m_simpleTrigger = secretRoomTrigger;
    }
  }

  protected void InitializeFireplacePuzzle()
  {
    for (int index = 0; index < this.doorObjects.Count; ++index)
      this.doorObjects[index].InitializeFireplace();
  }

  protected void InitializeSecretRoomPuzzle()
  {
    if (this.doorObjects.Count == 0)
      return;
    if (this.doorObjects.Count > 1)
    {
      UnityEngine.Debug.LogError((object) "Attempting to render a complex secret puzzle onto a multi-exit secret room. This is unsupported...");
    }
    else
    {
      IntVector2 key = this.doorObjects[0].collider.colliderObject.transform.position.IntXY(VectorConversions.Floor);
      IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(this.doorObjects[0].collider.exitDirection);
      CellData cellData = GameManager.Instance.Dungeon.data[key];
      RoomHandler room = cellData.parentRoom != null ? cellData.parentRoom : cellData.nearestRoom;
      CellData nearestFloorFacewall = room.GetNearestFloorFacewall(key + vector2FromDirection);
      if (nearestFloorFacewall == null)
      {
        UnityEngine.Debug.LogError((object) "failed complex puzzle generation due to lack of floor facewall.");
      }
      else
      {
        GameObject gameObject1 = GameManager.Instance.Dungeon.SecretRoomComplexTriggers[Random.Range(0, GameManager.Instance.Dungeon.SecretRoomComplexTriggers.Count)].gameObject;
        IntVector2 up = IntVector2.Up;
        GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject1);
        gameObject2.transform.parent = room.hierarchyParent;
        nearestFloorFacewall.cellVisualData.containsObjectSpaceStamp = true;
        nearestFloorFacewall.cellVisualData.containsWallSpaceStamp = true;
        GameManager.Instance.Dungeon.data[nearestFloorFacewall.position + IntVector2.Up].cellVisualData.containsObjectSpaceStamp = true;
        GameManager.Instance.Dungeon.data[nearestFloorFacewall.position + IntVector2.Up].cellVisualData.containsWallSpaceStamp = true;
        gameObject2.transform.position = (nearestFloorFacewall.position + up).ToVector3();
        ComplexSecretRoomTrigger component = gameObject2.GetComponent<ComplexSecretRoomTrigger>();
        component.referencedSecretRoom = this;
        component.Initialize(room);
      }
    }
  }

  protected void InitializeShootToBreak()
  {
    for (int index = 0; index < this.doorObjects.Count; ++index)
      this.doorObjects[index].InitializeShootToBreak();
  }

  public void DoSeal()
  {
    for (int index = 0; index < this.doorObjects.Count; ++index)
    {
      if ((Object) this.doorObjects[index].subsidiaryBlocker != (Object) null)
        this.doorObjects[index].subsidiaryBlocker.Seal();
    }
  }

  public void DoUnseal()
  {
    for (int index = 0; index < this.doorObjects.Count; ++index)
    {
      if ((Object) this.doorObjects[index].subsidiaryBlocker != (Object) null)
        this.doorObjects[index].subsidiaryBlocker.Unseal();
    }
  }

  public void OpenDoor()
  {
    int num1 = (int) AkSoundEngine.PostEvent("Play_UI_secret_reveal_01", this.gameObject);
    int num2 = (int) AkSoundEngine.PostEvent("Play_OBJ_secret_door_01", this.gameObject);
    this.IsOpen = true;
    this.ceilingRenderer.enabled = false;
    if ((Object) this.borderRenderer != (Object) null)
      this.borderRenderer.enabled = false;
    for (int index = 0; index < this.ceilingCells.Count; ++index)
    {
      if (GameManager.Instance.Dungeon.data[this.ceilingCells[index]] != null)
        GameManager.Instance.Dungeon.data[this.ceilingCells[index]].isSecretRoomCell = false;
    }
    for (int index = 0; index < this.doorObjects.Count; ++index)
    {
      if ((Object) this.doorObjects[index].subsidiaryBlocker != (Object) null)
        this.doorObjects[index].subsidiaryBlocker.ToggleRenderers(true);
    }
    Minimap.Instance.RevealMinimapRoom(this.room, true, isCurrentRoom: false);
    if (this.doorObjects.Count <= 0)
      return;
    for (int index = 0; index < this.doorObjects.Count; ++index)
      this.StartCoroutine(this.HandleDoorOpening(this.IsOpen, this.doorObjects[index]));
  }

  private string GetFrameName(string name, DungeonData.Direction dir)
  {
    if (!name.Contains("{0}"))
      return name;
    string str;
    switch (dir)
    {
      case DungeonData.Direction.NORTH:
        str = "_top_top";
        break;
      case DungeonData.Direction.EAST:
        str = "_right_top";
        break;
      default:
        str = dir == DungeonData.Direction.WEST ? "_left_top" : string.Empty;
        break;
    }
    return string.Format(name, (object) str);
  }

  private GameObject SpawnVFXAtPoint(GameObject vfx, Vector3 position)
  {
    GameObject gameObject = SpawnManager.SpawnVFX(vfx, position, Quaternion.identity);
    tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
    component.HeightOffGround = 0.25f;
    component.PlaceAtPositionByAnchor(position, tk2dBaseSprite.Anchor.MiddleCenter);
    component.IsPerpendicular = false;
    component.UpdateZDepth();
    return gameObject;
  }

  private void DoSparkAtPoint(Vector3 position, List<Transform> refTransformList)
  {
    tk2dSprite component = SpawnManager.SpawnVFX(GameManager.Instance.Dungeon.SecretRoomDoorSparkVFX).GetComponent<tk2dSprite>();
    component.HeightOffGround = 3.5f;
    component.PlaceAtPositionByAnchor(position, tk2dBaseSprite.Anchor.MiddleCenter);
    component.UpdateZDepth();
    refTransformList.Add(component.transform);
  }

  private void OnFinishedOpeningDoors()
  {
    for (int index = 0; index < this.doorObjects.Count; ++index)
      this.doorObjects[index].SetBreakable();
    ShadowSystem.ForceRoomLightsUpdate(this.room, 0.1f);
    for (int index = 0; index < this.doorObjects.Count; ++index)
      ShadowSystem.ForceRoomLightsUpdate(this.doorObjects[index].linkedRoom, 0.1f);
  }

  [DebuggerHidden]
  private IEnumerator HandleDoorOpening(bool openState, SecretRoomDoorBeer doorObject)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SecretRoomManager.\u003CHandleDoorOpening\u003Ec__Iterator0()
    {
      openState = openState,
      doorObject = doorObject,
      \u0024this = this
    };
  }

  public enum SecretRoomRevealStyle
  {
    Simple,
    ComplexPuzzle,
    ShootToBreak,
    FireplacePuzzle,
  }
}
