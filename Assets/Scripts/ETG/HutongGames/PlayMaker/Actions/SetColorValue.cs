#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Sets the value of a Color Variable.")]
    [ActionCategory(ActionCategory.Color)]
    public class SetColorValue : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmColor colorVariable;
        [RequiredField]
        public FsmColor color;
        public bool everyFrame;

        public override void Reset()
        {
            this.colorVariable = (FsmColor) null;
            this.color = (FsmColor) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetColorValue();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetColorValue();

        private void DoSetColorValue()
        {
            if (this.colorVariable == null)
                return;
            this.colorVariable.Value = this.color.Value;
        }
    }
}
