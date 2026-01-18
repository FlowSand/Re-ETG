#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if the value of a string variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
    public class StringChanged : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmString stringVariable;
        public FsmEvent changedEvent;
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;
        private string previousValue;

        public override void Reset()
        {
            this.stringVariable = (FsmString) null;
            this.changedEvent = (FsmEvent) null;
            this.storeResult = (FsmBool) null;
        }

        public override void OnEnter()
        {
            if (this.stringVariable.IsNone)
                this.Finish();
            else
                this.previousValue = this.stringVariable.Value;
        }

        public override void OnUpdate()
        {
            if (!(this.stringVariable.Value != this.previousValue))
                return;
            this.storeResult.Value = true;
            this.Fsm.Event(this.changedEvent);
        }
    }
}
