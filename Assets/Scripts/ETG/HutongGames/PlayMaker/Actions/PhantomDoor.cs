using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Triggers phantom door events.")]
  [ActionCategory(".NPCs")]
  public class PhantomDoor : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Seals the room the Owner is in.")]
    public FsmBool seal;

    public override void Reset() => this.seal = (FsmBool) false;

    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      component.specRigidbody.Initialize();
      RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(component.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
      if (this.seal.Value)
        BraveUtility.GetClosestToPosition<DungeonDoorSubsidiaryBlocker>(new List<DungeonDoorSubsidiaryBlocker>((IEnumerable<DungeonDoorSubsidiaryBlocker>) Object.FindObjectsOfType<DungeonDoorSubsidiaryBlocker>()), roomFromPosition.area.Center).Seal();
      this.Finish();
    }
  }
}
