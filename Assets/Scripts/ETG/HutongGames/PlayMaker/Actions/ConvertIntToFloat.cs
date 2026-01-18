#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Convert)]
    [Tooltip("Converts an Integer value to a Float value.")]
    public class ConvertIntToFloat : FsmStateAction
    {
        [Tooltip("The Integer variable to convert to a float.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmInt intVariable;
        [Tooltip("Store the result in a Float variable.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat floatVariable;
        [Tooltip("Repeat every frame. Useful if the Integer variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.intVariable = (FsmInt) null;
            this.floatVariable = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoConvertIntToFloat();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoConvertIntToFloat();

        private void DoConvertIntToFloat() => this.floatVariable.Value = (float) this.intVariable.Value;
    }
}
