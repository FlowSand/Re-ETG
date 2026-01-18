using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  public class ToggleAllSimpleTurrets : FsmStateAction
  {
    public FsmBool toggle;

    public override void Reset() => this.toggle = (FsmBool) false;

    public override void OnEnter()
    {
      List<SimpleTurretController> componentsAbsoluteInRoom = this.Owner.GetComponent<TalkDoerLite>().ParentRoom.GetComponentsAbsoluteInRoom<SimpleTurretController>();
      for (int index = 0; index < componentsAbsoluteInRoom.Count; ++index)
      {
        if (this.toggle.Value)
          componentsAbsoluteInRoom[index].ActivateManual();
        else
          componentsAbsoluteInRoom[index].DeactivateManual();
      }
      this.Finish();
    }
  }
}
