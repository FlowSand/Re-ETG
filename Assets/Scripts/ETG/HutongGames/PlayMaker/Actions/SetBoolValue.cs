#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sets the value of a Bool Variable.")]
    [ActionCategory(ActionCategory.Math)]
    public class SetBoolValue : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmBool boolVariable;
        [RequiredField]
        public FsmBool boolValue;
        public bool everyFrame;

        public override void Reset()
        {
            this.boolVariable = (FsmBool) null;
            this.boolValue = (FsmBool) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.boolVariable.Value = this.boolValue.Value;
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.boolVariable.Value = this.boolValue.Value;
    }
}
