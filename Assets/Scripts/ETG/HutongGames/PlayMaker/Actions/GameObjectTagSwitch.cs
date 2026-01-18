using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sends an Event based on a Game Object's Tag.")]
  [ActionCategory(ActionCategory.Logic)]
  public class GameObjectTagSwitch : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("The GameObject to test.")]
    public FsmGameObject gameObject;
    [CompoundArray("Tag Switches", "Compare Tag", "Send Event")]
    [UIHint(UIHint.Tag)]
    public FsmString[] compareTo;
    public FsmEvent[] sendEvent;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmGameObject) null;
      this.compareTo = new FsmString[1];
      this.sendEvent = new FsmEvent[1];
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoTagSwitch();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoTagSwitch();

    private void DoTagSwitch()
    {
      GameObject gameObject = this.gameObject.Value;
      if ((Object) gameObject == (Object) null)
        return;
      for (int index = 0; index < this.compareTo.Length; ++index)
      {
        if (gameObject.tag == this.compareTo[index].Value)
        {
          this.Fsm.Event(this.sendEvent[index]);
          break;
        }
      }
    }
  }
}
