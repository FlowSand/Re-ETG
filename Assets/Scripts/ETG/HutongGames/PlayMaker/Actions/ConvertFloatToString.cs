#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Converts a Float value to a String value with optional format.")]
    [ActionCategory(ActionCategory.Convert)]
    public class ConvertFloatToString : FsmStateAction
    {
        [Tooltip("The float variable to convert.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat floatVariable;
        [Tooltip("A string variable to store the converted value.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmString stringVariable;
        [Tooltip("Optional Format, allows for leading zeroes. E.g., 0000")]
        public FsmString format;
        [Tooltip("Repeat every frame. Useful if the float variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.floatVariable = (FsmFloat) null;
            this.stringVariable = (FsmString) null;
            this.everyFrame = false;
            this.format = (FsmString) null;
        }

        public override void OnEnter()
        {
            this.DoConvertFloatToString();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoConvertFloatToString();

        private void DoConvertFloatToString()
        {
            if (this.format.IsNone || string.IsNullOrEmpty(this.format.Value))
                this.stringVariable.Value = this.floatVariable.Value.ToString();
            else
                this.stringVariable.Value = this.floatVariable.Value.ToString(this.format.Value);
        }
    }
}
