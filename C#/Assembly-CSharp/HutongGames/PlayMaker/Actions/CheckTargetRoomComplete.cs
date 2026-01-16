// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CheckTargetRoomComplete
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

public class CheckTargetRoomComplete : FsmStateAction
{
  public FsmEvent noEnemies;
  public FsmEvent hasEnemies;
  private GunslingChallengeType ChallengeType;
  private TalkDoerLite m_talkDoer;
  private bool m_challengeInitialized;

  public override void Awake()
  {
    base.Awake();
    this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
  }

  public override void OnEnter()
  {
    bool flag = this.CheckRoom(true);
    if (!this.m_challengeInitialized)
    {
      this.ChooseRandomChallenge();
      this.m_challengeInitialized = true;
    }
    if (flag)
      this.Fsm.Event(this.hasEnemies);
    else
      this.Fsm.Event(this.noEnemies);
    this.Finish();
  }

  private bool CheckRoom(bool canFallback)
  {
    RoomHandler absoluteParentRoom = this.m_talkDoer.GetAbsoluteParentRoom();
    RoomHandler injectionTarget = absoluteParentRoom.injectionTarget;
    if (injectionTarget.visibility == RoomHandler.VisibilityStatus.OBSCURED || injectionTarget.visibility == RoomHandler.VisibilityStatus.REOBSCURED || injectionTarget.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
      return true;
    if (!canFallback)
      return false;
    if (absoluteParentRoom.distanceFromEntrance > injectionTarget.distanceFromEntrance)
    {
      for (int index = 0; index < absoluteParentRoom.connectedRooms.Count; ++index)
      {
        if (absoluteParentRoom.connectedRooms[index] != null && absoluteParentRoom.connectedRooms[index].distanceFromEntrance > absoluteParentRoom.distanceFromEntrance && absoluteParentRoom.connectedRooms[index].EverHadEnemies)
        {
          absoluteParentRoom.injectionTarget = absoluteParentRoom.connectedRooms[index];
          break;
        }
      }
    }
    return this.CheckRoom(false);
  }

  private void ChooseRandomChallenge()
  {
    RoomHandler injectionTarget = this.m_talkDoer.GetAbsoluteParentRoom().injectionTarget;
    List<GunslingChallengeType> list = new List<GunslingChallengeType>((IEnumerable<GunslingChallengeType>) Enum.GetValues(typeof (GunslingChallengeType)));
    if ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null && (GameManager.Instance.PrimaryPlayer.CharacterUsesRandomGuns || GameManager.Instance.PrimaryPlayer.IsGunLocked))
      list.Remove(GunslingChallengeType.SPECIFIC_GUN);
    if (!this.IsRoomTraversibleWithoutDodgeRolls(injectionTarget))
      list.Remove(GunslingChallengeType.NO_DODGE_ROLL);
    if (!GameStatsManager.Instance.GetFlag(GungeonFlags.DAISUKE_ACTIVE_IN_FOYER))
      list.Remove(GunslingChallengeType.DAISUKE_CHALLENGES);
    if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
      list.Remove(GunslingChallengeType.DAISUKE_CHALLENGES);
    this.ChallengeType = BraveUtility.RandomElement<GunslingChallengeType>(list);
    this.Fsm.Variables.GetFsmInt("ChallengeType").Value = (int) this.ChallengeType;
  }

  private bool IsRoomTraversibleWithoutDodgeRolls(RoomHandler room)
  {
    DungeonData data = GameManager.Instance.Dungeon.data;
    for (int index = 0; index < room.Cells.Count; ++index)
    {
      if (data.CheckInBoundsAndValid(room.Cells[index]) && data[room.Cells[index]].type == CellType.PIT)
        return false;
    }
    return true;
  }
}
