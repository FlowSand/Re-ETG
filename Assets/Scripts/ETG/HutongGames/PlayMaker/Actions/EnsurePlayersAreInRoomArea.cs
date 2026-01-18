using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Force teleport players to a certain area in the room if they're not already there.")]
  [ActionCategory(".NPCs")]
  public class EnsurePlayersAreInRoomArea : FsmStateAction
  {
    public Vector2 lowerLeftRoomTile;
    public Vector2 upperRightRoomTile;

    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      Vector2 min = component.ParentRoom.area.UnitBottomLeft + this.lowerLeftRoomTile;
      Vector2 max = component.ParentRoom.area.UnitBottomLeft + this.upperRightRoomTile;
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
        if (!BraveMathCollege.AABBContains(min, max, allPlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox)))
        {
          Vector2 targetPoint = new Vector2((float) (((double) min.x + (double) max.x) / 2.0 - 0.5), min.y + 1f);
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            targetPoint.x += (float) (1.5 * (index != 0 ? 1.0 : -1.0));
          allPlayer.WarpToPoint(targetPoint, true);
        }
      }
      this.Finish();
    }
  }
}
