using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sends an Event when a Button is released.")]
  [ActionCategory(ActionCategory.Input)]
  public class GetButtonUp : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The name of the button. Set in the Unity Input Manager.")]
    public FsmString buttonName;
    [HutongGames.PlayMaker.Tooltip("Event to send if the button is released.")]
    public FsmEvent sendEvent;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Set to True if the button is released.")]
    public FsmBool storeResult;

    public override void Reset()
    {
      this.buttonName = (FsmString) "Fire1";
      this.sendEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
    }

    public override void OnUpdate()
    {
      bool buttonUp = Input.GetButtonUp(this.buttonName.Value);
      if (buttonUp)
        this.Fsm.Event(this.sendEvent);
      this.storeResult.Value = buttonUp;
    }
  }
}
