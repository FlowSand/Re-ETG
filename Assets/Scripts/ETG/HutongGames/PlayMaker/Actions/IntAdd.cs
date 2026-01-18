#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Adds a value to an Integer Variable.")]
    public class IntAdd : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt intVariable;
        [RequiredField]
        public FsmInt add;
        public bool everyFrame;

        public override void Reset()
        {
            this.intVariable = (FsmInt) null;
            this.add = (FsmInt) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.intVariable.Value += this.add.Value;
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.intVariable.Value += this.add.Value;
    }
}
