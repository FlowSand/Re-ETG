// Decompiled with JetBrains decompiler
// Type: WallMimicController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class WallMimicController : CustomEngageDoer, IPlaceConfigurable
{
  public GameObject WallDisappearVFX;
  protected bool m_playerTrueSight;
  private Vector3 m_startingPos;
  private IntVector2 pos1;
  private IntVector2 pos2;
  private DungeonData.Direction m_facingDirection;
  private GameObject m_fakeWall;
  private GameObject m_fakeCeiling;
  private GunHandController[] m_hands;
  private GoopDoer m_goopDoer;
  private bool m_isHidden = true;
  private bool m_isFinished;
  private float m_collisionKnockbackStrength;
  private bool m_configured;

  protected bool CanAwaken
  {
    get => this.m_isHidden && !PassiveItem.IsFlagSetAtAll(typeof (MimicRingItem));
  }

  public void Awake()
  {
    this.visibilityManager.OnToggleRenderers += new System.Action(this.OnToggleRenderers);
    this.aiActor.IsGone = true;
  }

  public void Start()
  {
    if (!this.m_configured)
      this.ConfigureOnPlacement(GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY(VectorConversions.Ceil)));
    this.transform.position = this.m_startingPos;
    this.specRigidbody.Reinitialize();
    this.aiAnimator.LockFacingDirection = true;
    this.aiAnimator.FacingDirection = DungeonData.GetAngleFromDirection(this.m_facingDirection);
    this.m_fakeWall = SecretRoomBuilder.GenerateWallMesh(this.m_facingDirection, this.pos1, "Mimic Wall", abridged: true);
    if (this.aiActor.ParentRoom != null)
      this.m_fakeWall.transform.parent = this.aiActor.ParentRoom.hierarchyParent;
    this.m_fakeWall.transform.position = this.pos1.ToVector3().WithZ((float) (this.pos1.y - 2)) + Vector3.down;
    if (this.m_facingDirection == DungeonData.Direction.SOUTH)
      StaticReferenceManager.AllShadowSystemDepthHavers.Add(this.m_fakeWall.transform);
    else if (this.m_facingDirection == DungeonData.Direction.WEST)
      this.m_fakeWall.transform.position += new Vector3(-3f / 16f, 0.0f);
    this.m_fakeCeiling = SecretRoomBuilder.GenerateRoomCeilingMesh(this.GetCeilingTileSet(this.pos1, this.pos2, this.m_facingDirection), "Mimic Ceiling", mimicCheck: true);
    if (this.aiActor.ParentRoom != null)
      this.m_fakeCeiling.transform.parent = this.aiActor.ParentRoom.hierarchyParent;
    this.m_fakeCeiling.transform.position = this.pos1.ToVector3().WithZ((float) (this.pos1.y - 4));
    if (this.m_facingDirection == DungeonData.Direction.NORTH)
      this.m_fakeCeiling.transform.position += new Vector3(-1f, 0.0f);
    else if (this.m_facingDirection == DungeonData.Direction.SOUTH)
      this.m_fakeCeiling.transform.position += new Vector3(-1f, 2f);
    else if (this.m_facingDirection == DungeonData.Direction.EAST)
      this.m_fakeCeiling.transform.position += new Vector3(-1f, 0.0f);
    this.m_fakeCeiling.transform.position = this.m_fakeCeiling.transform.position.WithZ(this.m_fakeCeiling.transform.position.y - 5f);
    for (int index = 0; index < this.specRigidbody.PixelColliders.Count; ++index)
      this.specRigidbody.PixelColliders[index].Enabled = false;
    if (this.m_facingDirection == DungeonData.Direction.NORTH)
    {
      this.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.LowObstacle, 38, 38, 32 /*0x20*/, 8));
      this.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.HighObstacle, 38, 54, 32 /*0x20*/, 8));
    }
    else if (this.m_facingDirection == DungeonData.Direction.SOUTH)
    {
      this.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.LowObstacle, 38, 38, 32 /*0x20*/, 16 /*0x10*/));
      this.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.HighObstacle, 38, 54, 32 /*0x20*/, 16 /*0x10*/));
    }
    else if (this.m_facingDirection == DungeonData.Direction.WEST || this.m_facingDirection == DungeonData.Direction.EAST)
    {
      this.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.LowObstacle, 46, 38, 16 /*0x10*/, 32 /*0x20*/));
      this.specRigidbody.PixelColliders.Add(PixelCollider.CreateRectangle(CollisionLayer.HighObstacle, 46, 38, 16 /*0x10*/, 32 /*0x20*/));
    }
    this.specRigidbody.ForceRegenerate();
    this.aiActor.HasDonePlayerEnterCheck = true;
    this.m_collisionKnockbackStrength = this.aiActor.CollisionKnockbackStrength;
    this.aiActor.CollisionKnockbackStrength = 0.0f;
    this.aiActor.CollisionDamage = 0.0f;
    this.m_goopDoer = this.GetComponent<GoopDoer>();
  }

  public void Update()
  {
    if (!this.CanAwaken)
      return;
    Vector2 unitBottomLeft = this.specRigidbody.PixelColliders[2].UnitBottomLeft;
    Vector2 max = unitBottomLeft;
    if (this.m_facingDirection == DungeonData.Direction.SOUTH)
    {
      unitBottomLeft += new Vector2(0.0f, -1.5f);
      max += new Vector2(2f, 0.0f);
    }
    else if (this.m_facingDirection == DungeonData.Direction.NORTH)
    {
      unitBottomLeft += new Vector2(0.0f, 1f);
      max += new Vector2(2f, 3f);
    }
    else if (this.m_facingDirection == DungeonData.Direction.WEST)
    {
      unitBottomLeft += new Vector2(-1.5f, 0.0f);
      max += new Vector2(0.0f, 2f);
    }
    else if (this.m_facingDirection == DungeonData.Direction.EAST)
    {
      unitBottomLeft += new Vector2(1f, 0.0f);
      max += new Vector2(2.5f, 2f);
    }
    bool flag = false;
    foreach (PlayerController allPlayer in GameManager.Instance.AllPlayers)
    {
      if (allPlayer.CanDetectHiddenEnemies)
      {
        flag = true;
        if (!this.m_playerTrueSight)
        {
          this.m_playerTrueSight = true;
          this.aiActor.ToggleRenderers(true);
        }
      }
      if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive && !allPlayer.IsGhost && allPlayer.specRigidbody.GetUnitCenter(ColliderType.Ground).IsWithin(unitBottomLeft, max))
      {
        if ((bool) (UnityEngine.Object) this.m_goopDoer)
        {
          Vector2 unitCenter = this.specRigidbody.PixelColliders[2].UnitCenter;
          if (this.m_facingDirection == DungeonData.Direction.NORTH)
            unitCenter += Vector2.up;
          DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.m_goopDoer.goopDefinition).TimedAddGoopArc(unitCenter, 3f, 90f, DungeonData.GetIntVector2FromDirection(this.m_facingDirection).ToVector2(), 0.2f);
        }
        this.StartCoroutine(this.BecomeMimic());
      }
    }
    if (flag || !this.m_playerTrueSight)
      return;
    this.m_playerTrueSight = false;
    this.aiActor.ToggleRenderers(false);
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.visibilityManager)
      this.visibilityManager.OnToggleRenderers -= new System.Action(this.OnToggleRenderers);
    base.OnDestroy();
  }

  public override void StartIntro() => this.StartCoroutine(this.DoIntro());

  [DebuggerHidden]
  private IEnumerator DoIntro()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new WallMimicController.\u003CDoIntro\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public override bool IsFinished => this.m_isFinished;

  private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
  {
    if (!this.CanAwaken || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.projectile)
      return;
    this.StartCoroutine(this.BecomeMimic());
  }

  private void HandleBeamCollision(BeamController beamController)
  {
    if (!this.CanAwaken)
      return;
    this.StartCoroutine(this.BecomeMimic());
  }

  private void OnToggleRenderers()
  {
    if (!this.m_isHidden || !(bool) (UnityEngine.Object) this.aiActor)
      return;
    if ((bool) (UnityEngine.Object) this.aiActor.sprite)
      this.aiActor.sprite.renderer.enabled = false;
    if (!(bool) (UnityEngine.Object) this.aiActor.ShadowObject)
      return;
    this.aiActor.ShadowObject.GetComponent<Renderer>().enabled = false;
  }

  public void ConfigureOnPlacement(RoomHandler room)
  {
    Vector2 vector = this.transform.position.XY() + new Vector2((float) this.specRigidbody.GroundPixelCollider.ManualOffsetX / 16f, (float) this.specRigidbody.GroundPixelCollider.ManualOffsetY / 16f);
    Vector2 vector2 = vector.ToIntVector2().ToVector2();
    this.transform.position += (Vector3) (vector2 - vector);
    this.pos1 = vector2.ToIntVector2(VectorConversions.Floor);
    this.pos2 = this.pos1 + IntVector2.Right;
    this.m_facingDirection = this.GetFacingDirection(this.pos1, this.pos2);
    if (this.m_facingDirection == DungeonData.Direction.WEST)
    {
      this.pos1 = this.pos2;
      this.m_startingPos = this.transform.position + new Vector3(1f, 0.0f);
    }
    else if (this.m_facingDirection == DungeonData.Direction.EAST)
    {
      this.pos2 = this.pos1;
      this.m_startingPos = this.transform.position;
    }
    else
      this.m_startingPos = this.transform.position + new Vector3(0.5f, 0.0f);
    CellData cellData1 = GameManager.Instance.Dungeon.data[this.pos1];
    CellData cellData2 = GameManager.Instance.Dungeon.data[this.pos2];
    cellData1.isSecretRoomCell = true;
    cellData2.isSecretRoomCell = true;
    cellData1.forceDisallowGoop = true;
    cellData2.forceDisallowGoop = true;
    cellData1.cellVisualData.preventFloorStamping = true;
    cellData2.cellVisualData.preventFloorStamping = true;
    cellData1.isWallMimicHideout = true;
    cellData2.isWallMimicHideout = true;
    if (this.m_facingDirection == DungeonData.Direction.WEST || this.m_facingDirection == DungeonData.Direction.EAST)
      GameManager.Instance.Dungeon.data[this.pos1 + IntVector2.Up].isSecretRoomCell = true;
    this.m_configured = true;
  }

  [DebuggerHidden]
  private IEnumerator BecomeMimic()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new WallMimicController.\u003CBecomeMimic\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  private DungeonData.Direction GetFacingDirection(IntVector2 pos1, IntVector2 pos2)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    if (data.isWall(pos1 + IntVector2.Down) && data.isWall(pos1 + IntVector2.Up))
      return DungeonData.Direction.EAST;
    if (data.isWall(pos2 + IntVector2.Down) && data.isWall(pos2 + IntVector2.Up))
      return DungeonData.Direction.WEST;
    if (data.isWall(pos1 + IntVector2.Down) && data.isWall(pos2 + IntVector2.Down))
      return DungeonData.Direction.NORTH;
    if (data.isWall(pos1 + IntVector2.Up) && data.isWall(pos2 + IntVector2.Up))
      return DungeonData.Direction.SOUTH;
    UnityEngine.Debug.LogError((object) "Not able to determine the direction of a wall mimic!");
    return DungeonData.Direction.SOUTH;
  }

  private HashSet<IntVector2> GetCeilingTileSet(
    IntVector2 pos1,
    IntVector2 pos2,
    DungeonData.Direction facingDirection)
  {
    IntVector2 intVector2_1;
    IntVector2 intVector2_2;
    switch (facingDirection)
    {
      case DungeonData.Direction.NORTH:
        intVector2_1 = pos1 + new IntVector2(-1, 0);
        intVector2_2 = pos2 + new IntVector2(1, 1);
        break;
      case DungeonData.Direction.EAST:
        intVector2_1 = pos1 + new IntVector2(-1, 0);
        intVector2_2 = pos2 + new IntVector2(0, 3);
        break;
      case DungeonData.Direction.SOUTH:
        intVector2_1 = pos1 + new IntVector2(-1, 2);
        intVector2_2 = pos2 + new IntVector2(1, 3);
        break;
      case DungeonData.Direction.WEST:
        intVector2_1 = pos1 + new IntVector2(0, 0);
        intVector2_2 = pos2 + new IntVector2(1, 3);
        break;
      default:
        return (HashSet<IntVector2>) null;
    }
    HashSet<IntVector2> ceilingTileSet = new HashSet<IntVector2>();
    for (int x = intVector2_1.x; x <= intVector2_2.x; ++x)
    {
      for (int y = intVector2_1.y; y <= intVector2_2.y; ++y)
      {
        IntVector2 intVector2_3 = new IntVector2(x, y);
        ceilingTileSet.Add(intVector2_3);
      }
    }
    return ceilingTileSet;
  }
}
