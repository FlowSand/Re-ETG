#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sets the value of a String Variable.")]
    [ActionCategory(ActionCategory.String)]
    public class SetStringValue : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmString stringVariable;
        [UIHint(UIHint.TextArea)]
        public FsmString stringValue;
        public bool everyFrame;

        public override void Reset()
        {
            this.stringVariable = (FsmString) null;
            this.stringValue = (FsmString) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetStringValue();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetStringValue();

        private void DoSetStringValue()
        {
            if (this.stringVariable == null || this.stringValue == null)
                return;
            this.stringVariable.Value = this.stringValue.Value;
        }
    }
}
