#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class SendEventByName : FsmStateAction
  {
    [Tooltip("Where to send the event.")]
    public FsmEventTarget eventTarget;
    [RequiredField]
    [Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
    public FsmString sendEvent;
    [HasFloatSlider(0.0f, 10f)]
    [Tooltip("Optional delay in seconds.")]
    public FsmFloat delay;
    [Tooltip("Repeat every frame. Rarely needed, but can be useful when sending events to other FSMs.")]
    public bool everyFrame;
    private DelayedEvent delayedEvent;

    public override void Reset()
    {
      this.eventTarget = (FsmEventTarget) null;
      this.sendEvent = (FsmString) null;
      this.delay = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      if ((double) this.delay.Value < 1.0 / 1000.0)
      {
        this.Fsm.Event(this.eventTarget, this.sendEvent.Value);
        if (this.everyFrame)
          return;
        this.Finish();
      }
      else
        this.delayedEvent = this.Fsm.DelayedEvent(this.eventTarget, FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
    }

    public override void OnUpdate()
    {
      if (!this.everyFrame)
      {
        if (!DelayedEvent.WasSent(this.delayedEvent))
          return;
        this.Finish();
      }
      else
        this.Fsm.Event(this.eventTarget, this.sendEvent.Value);
    }
  }
}
