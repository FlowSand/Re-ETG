#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Builds a String from other Strings.")]
    [ActionCategory(ActionCategory.String)]
    public class BuildString : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Array of Strings to combine.")]
        public FsmString[] stringParts;
        [Tooltip("Separator to insert between each String. E.g. space character.")]
        public FsmString separator;
        [Tooltip("Add Separator to end of built string.")]
        public FsmBool addToEnd;
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the final String in a variable.")]
        [RequiredField]
        public FsmString storeResult;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        private string result;

        public override void Reset()
        {
            this.stringParts = new FsmString[3];
            this.separator = (FsmString) null;
            this.addToEnd = (FsmBool) true;
            this.storeResult = (FsmString) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoBuildString();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoBuildString();

        private void DoBuildString()
        {
            if (this.storeResult == null)
                return;
            this.result = string.Empty;
            for (int index = 0; index < this.stringParts.Length - 1; ++index)
            {
                this.result += (string) (object) this.stringParts[index];
                this.result += this.separator.Value;
            }
            this.result += (string) (object) this.stringParts[this.stringParts.Length - 1];
            if (this.addToEnd.Value)
                this.result += this.separator.Value;
            this.storeResult.Value = this.result;
        }
    }
}
