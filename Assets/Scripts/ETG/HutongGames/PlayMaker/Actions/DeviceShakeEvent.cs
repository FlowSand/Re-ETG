using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Device)]
  [HutongGames.PlayMaker.Tooltip("Sends an Event when the mobile device is shaken.")]
  public class DeviceShakeEvent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Amount of acceleration required to trigger the event. Higher numbers require a harder shake.")]
    [RequiredField]
    public FsmFloat shakeThreshold;
    [HutongGames.PlayMaker.Tooltip("Event to send when Shake Threshold is exceded.")]
    [RequiredField]
    public FsmEvent sendEvent;

    public override void Reset()
    {
      this.shakeThreshold = (FsmFloat) 3f;
      this.sendEvent = (FsmEvent) null;
    }

    public override void OnUpdate()
    {
      if ((double) Input.acceleration.sqrMagnitude <= (double) this.shakeThreshold.Value * (double) this.shakeThreshold.Value)
        return;
      this.Fsm.Event(this.sendEvent);
    }
  }
}
