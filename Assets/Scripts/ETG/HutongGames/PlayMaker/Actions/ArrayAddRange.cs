#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Add values to an array.")]
    public class ArrayAddRange : FsmStateAction
    {
        [Tooltip("The Array Variable to use.")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmArray array;
        [Tooltip("The variables to add.")]
        [RequiredField]
        [MatchElementType("array")]
        public FsmVar[] variables;

        public override void Reset()
        {
            this.array = (FsmArray) null;
            this.variables = new FsmVar[2];
        }

        public override void OnEnter()
        {
            this.DoAddRange();
            this.Finish();
        }

        private void DoAddRange()
        {
            int length = this.variables.Length;
            if (length <= 0)
                return;
            this.array.Resize(this.array.Length + length);
            foreach (FsmVar variable in this.variables)
            {
                variable.UpdateValue();
                this.array.Set(this.array.Length - length, variable.GetValue());
                --length;
            }
        }
    }
}
