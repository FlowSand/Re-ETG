using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionTarget(typeof (GameObject), "eventTarget", false)]
    [HutongGames.PlayMaker.Tooltip("Sends an Event after an optional delay. NOTE: To send events between FSMs they must be marked as Global in the Events Browser.")]
    [ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof (PlayMakerFSM), "eventTarget", false)]
    public class SendEvent : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
        public FsmEvent sendEvent;
        [HasFloatSlider(0.0f, 10f)]
        [HutongGames.PlayMaker.Tooltip("Optional delay in seconds.")]
        public FsmFloat delay;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Rarely needed, but can be useful when sending events to other FSMs.")]
        public bool everyFrame;
        private DelayedEvent delayedEvent;

        public override void Reset()
        {
            this.eventTarget = (FsmEventTarget) null;
            this.sendEvent = (FsmEvent) null;
            this.delay = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            if ((double) this.delay.Value < 1.0 / 1000.0)
            {
                this.Fsm.Event(this.eventTarget, this.sendEvent);
                if (this.everyFrame)
                    return;
                this.Finish();
            }
            else
                this.delayedEvent = this.Fsm.DelayedEvent(this.eventTarget, this.sendEvent, this.delay.Value);
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
                this.Fsm.Event(this.eventTarget, this.sendEvent);
        }
    }
}
