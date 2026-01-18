#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Flips the value of a Bool Variable.")]
    [ActionCategory(ActionCategory.Math)]
    public class BoolFlip : FsmStateAction
    {
        [Tooltip("Bool variable to flip.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmBool boolVariable;

        public override void Reset() => this.boolVariable = (FsmBool) null;

        public override void OnEnter()
        {
            this.boolVariable.Value = !this.boolVariable.Value;
            this.Finish();
        }
    }
}
