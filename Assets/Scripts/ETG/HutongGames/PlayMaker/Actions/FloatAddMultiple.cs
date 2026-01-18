#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Adds multipe float variables to float variable.")]
    [ActionCategory(ActionCategory.Math)]
    public class FloatAddMultiple : FsmStateAction
    {
        [Tooltip("The float variables to add.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat[] floatVariables;
        [Tooltip("Add to this variable.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat addTo;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.floatVariables = (FsmFloat[]) null;
            this.addTo = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoFloatAdd();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoFloatAdd();

        private void DoFloatAdd()
        {
            for (int index = 0; index < this.floatVariables.Length; ++index)
                this.addTo.Value += this.floatVariables[index].Value;
        }
    }
}
