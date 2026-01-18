#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Gets the event that caused the transition to the current state, and stores it in a String Variable.")]
    [ActionCategory(ActionCategory.StateMachine)]
    public class GetLastEvent : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmString storeEvent;

        public override void Reset() => this.storeEvent = (FsmString) null;

        public override void OnEnter()
        {
            this.storeEvent.Value = this.Fsm.LastTransition != null ? this.Fsm.LastTransition.EventName : "START";
            this.Finish();
        }
    }
}
