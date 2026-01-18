#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if the value of a Bool Variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
    public class BoolChanged : FsmStateAction
    {
        [Tooltip("The Bool variable to watch for changes.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmBool boolVariable;
        [Tooltip("Event to send if the variable changes.")]
        public FsmEvent changedEvent;
        [Tooltip("Set to True if changed.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;
        private bool previousValue;

        public override void Reset()
        {
            this.boolVariable = (FsmBool) null;
            this.changedEvent = (FsmEvent) null;
            this.storeResult = (FsmBool) null;
        }

        public override void OnEnter()
        {
            if (this.boolVariable.IsNone)
                this.Finish();
            else
                this.previousValue = this.boolVariable.Value;
        }

        public override void OnUpdate()
        {
            this.storeResult.Value = false;
            if (this.boolVariable.Value == this.previousValue)
                return;
            this.storeResult.Value = true;
            this.Fsm.Event(this.changedEvent);
        }
    }
}
