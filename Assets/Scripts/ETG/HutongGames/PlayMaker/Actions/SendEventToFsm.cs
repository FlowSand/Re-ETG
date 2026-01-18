using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sends an Event to another Fsm after an optional delay. Specify an Fsm Name or use the first Fsm on the object.")]
  [Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class SendEventToFsm : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.FsmName)]
    [HutongGames.PlayMaker.Tooltip("Optional name of Fsm on Game Object")]
    public FsmString fsmName;
    [RequiredField]
    [UIHint(UIHint.FsmEvent)]
    public FsmString sendEvent;
    [HasFloatSlider(0.0f, 10f)]
    public FsmFloat delay;
    private bool requireReceiver;
    private GameObject go;
    private DelayedEvent delayedEvent;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fsmName = (FsmString) null;
      this.sendEvent = (FsmString) null;
      this.delay = (FsmFloat) null;
      this.requireReceiver = false;
    }

    public override void OnEnter()
    {
      this.go = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((UnityEngine.Object) this.go == (UnityEngine.Object) null)
      {
        this.Finish();
      }
      else
      {
        PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(this.go, this.fsmName.Value);
        if ((UnityEngine.Object) gameObjectFsm == (UnityEngine.Object) null)
        {
          if (!this.requireReceiver)
            return;
          this.LogError($"GameObject doesn't have FsmComponent: {this.go.name} {this.fsmName.Value}");
        }
        else if ((double) this.delay.Value < 0.001)
        {
          gameObjectFsm.Fsm.Event(this.sendEvent.Value);
          this.Finish();
        }
        else
          this.delayedEvent = gameObjectFsm.Fsm.DelayedEvent(FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
      }
    }

    public override void OnUpdate()
    {
      if (!DelayedEvent.WasSent(this.delayedEvent))
        return;
      this.Finish();
    }
  }
}
