#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Color)]
    [Tooltip("Get the RGBA channels of a Color Variable and store them in Float Variables.")]
    public class GetColorRGBA : FsmStateAction
    {
        [Tooltip("The Color variable.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmColor color;
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the red channel in a float variable.")]
        public FsmFloat storeRed;
        [Tooltip("Store the green channel in a float variable.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeGreen;
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the blue channel in a float variable.")]
        public FsmFloat storeBlue;
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the alpha channel in a float variable.")]
        public FsmFloat storeAlpha;
        [Tooltip("Repeat every frame. Useful if the color variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.color = (FsmColor) null;
            this.storeRed = (FsmFloat) null;
            this.storeGreen = (FsmFloat) null;
            this.storeBlue = (FsmFloat) null;
            this.storeAlpha = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetColorRGBA();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetColorRGBA();

        private void DoGetColorRGBA()
        {
            if (this.color.IsNone)
                return;
            this.storeRed.Value = this.color.Value.r;
            this.storeGreen.Value = this.color.Value.g;
            this.storeBlue.Value = this.color.Value.b;
            this.storeAlpha.Value = this.color.Value.a;
        }
    }
}
