// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AncientPistol_WaitForRoomClear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Paths to near the player's current location.")]
[ActionCategory(".NPCs")]
public class AncientPistol_WaitForRoomClear : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Event sent if there are more rooms.")]
  public FsmEvent MoreRooms;
  [HutongGames.PlayMaker.Tooltip("Event sent if there aren't.")]
  public FsmEvent NoMoreRooms;
  private AncientPistolController m_pistolDoer;
  private int m_currentTargetRoomIndex;
  private Vector2 m_lastPosition;

  public override void Awake()
  {
    base.Awake();
    this.m_pistolDoer = this.Owner.GetComponent<AncientPistolController>();
    this.m_currentTargetRoomIndex = 0;
  }

  public override void OnEnter()
  {
    this.m_lastPosition = this.m_pistolDoer.specRigidbody.UnitCenter;
    Vector2 zero = Vector2.zero;
    RoomHandler otherRoom = this.m_pistolDoer.RoomSequence[this.m_currentTargetRoomIndex];
    RoomHandler absoluteParentRoom = this.m_pistolDoer.talkDoer.GetAbsoluteParentRoom();
    if (this.m_currentTargetRoomIndex > 0)
      absoluteParentRoom = this.m_pistolDoer.RoomSequence[this.m_currentTargetRoomIndex - 1];
    PrototypeRoomExit exitConnectedToRoom = absoluteParentRoom.GetExitConnectedToRoom(otherRoom);
    if (exitConnectedToRoom == null)
      return;
    Vector2 vector2 = (exitConnectedToRoom.GetExitOrigin(0) - IntVector2.One + absoluteParentRoom.area.basePosition + -5 * DungeonData.GetIntVector2FromDirection(exitConnectedToRoom.exitDirection)).ToVector2();
    this.m_pistolDoer.StartCoroutine(this.HandleDelayedPathing(absoluteParentRoom.GetBestRewardLocation(IntVector2.One, vector2, false).ToVector2()));
  }

  [DebuggerHidden]
  private IEnumerator HandleDelayedPathing(Vector2 targetPosition)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AncientPistol_WaitForRoomClear.<HandleDelayedPathing>c__Iterator0()
    {
      targetPosition = targetPosition,
      $this = this
    };
  }

  public override void OnUpdate()
  {
    base.OnUpdate();
    if (this.m_pistolDoer.talkDoer.CurrentPath == null)
      return;
    if (!this.m_pistolDoer.talkDoer.CurrentPath.WillReachFinalGoal)
    {
      RoomHandler otherRoom = this.m_pistolDoer.RoomSequence[this.m_currentTargetRoomIndex];
      RoomHandler roomHandler = this.m_currentTargetRoomIndex != 0 ? this.m_pistolDoer.RoomSequence[this.m_currentTargetRoomIndex - 1] : this.m_pistolDoer.talkDoer.GetAbsoluteParentRoom();
      PrototypeRoomExit exitConnectedToRoom = roomHandler.GetExitConnectedToRoom(otherRoom);
      if (exitConnectedToRoom == null)
        return;
      this.m_pistolDoer.transform.position = (exitConnectedToRoom.GetExitOrigin(0) - IntVector2.One + roomHandler.area.basePosition + -5 * DungeonData.GetIntVector2FromDirection(exitConnectedToRoom.exitDirection)).ToVector3();
      this.m_pistolDoer.specRigidbody.Reinitialize();
      PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_pistolDoer.specRigidbody, new int?(CollisionMask.LayerToMask(CollisionLayer.PlayerCollider)));
      this.m_pistolDoer.talkDoer.CurrentPath = (Path) null;
    }
    else
    {
      this.m_pistolDoer.talkDoer.specRigidbody.Velocity = this.m_pistolDoer.talkDoer.GetPathVelocityContribution(this.m_lastPosition, 32 /*0x20*/);
      this.m_lastPosition = this.m_pistolDoer.talkDoer.specRigidbody.UnitCenter;
    }
  }

  public override void OnLateUpdate()
  {
    if (this.m_pistolDoer.talkDoer.CurrentPath != null)
      return;
    RoomHandler roomHandler = this.m_pistolDoer.RoomSequence[this.m_currentTargetRoomIndex];
    bool flag = GameManager.Instance.BestActivePlayer.CurrentRoom != roomHandler && this.m_currentTargetRoomIndex == this.m_pistolDoer.RoomSequence.Count - 1;
    if (roomHandler.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) || flag)
      return;
    ++this.m_currentTargetRoomIndex;
    if (this.m_currentTargetRoomIndex >= this.m_pistolDoer.RoomSequence.Count)
      this.Fsm.Event(this.NoMoreRooms);
    else
      this.Fsm.Event(this.MoreRooms);
    this.Finish();
  }
}
