#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
    [ActionCategory(ActionCategory.StateMachine)]
    public class NextFrameEvent : FsmStateAction
    {
        [RequiredField]
        public FsmEvent sendEvent;

        public override void Reset() => this.sendEvent = (FsmEvent) null;

        public override void OnEnter()
        {
        }

        public override void OnUpdate()
        {
            this.Finish();
            this.Fsm.Event(this.sendEvent);
        }
    }
}
