#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sends the next event on the state each time the state is entered.")]
    [ActionCategory(ActionCategory.StateMachine)]
    public class SequenceEvent : FsmStateAction
    {
        [HasFloatSlider(0.0f, 10f)]
        public FsmFloat delay;
        private DelayedEvent delayedEvent;
        private int eventIndex;

        public override void Reset() => this.delay = (FsmFloat) null;

        public override void OnEnter()
        {
            int length = this.State.Transitions.Length;
            if (length <= 0)
                return;
            FsmEvent fsmEvent = this.State.Transitions[this.eventIndex].FsmEvent;
            if ((double) this.delay.Value < 1.0 / 1000.0)
            {
                this.Fsm.Event(fsmEvent);
                this.Finish();
            }
            else
                this.delayedEvent = this.Fsm.DelayedEvent(fsmEvent, this.delay.Value);
            ++this.eventIndex;
            if (this.eventIndex != length)
                return;
            this.eventIndex = 0;
        }

        public override void OnUpdate()
        {
            if (!DelayedEvent.WasSent(this.delayedEvent))
                return;
            this.Finish();
        }
    }
}
