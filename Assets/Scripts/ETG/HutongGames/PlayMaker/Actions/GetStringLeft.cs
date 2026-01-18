using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [HutongGames.PlayMaker.Tooltip("Gets the Left n characters from a String Variable.")]
    public class GetStringLeft : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString stringVariable;
        [HutongGames.PlayMaker.Tooltip("Number of characters to get.")]
        public FsmInt charCount;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.stringVariable = (FsmString) null;
            this.charCount = (FsmInt) 0;
            this.storeResult = (FsmString) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoGetStringLeft();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetStringLeft();

        private void DoGetStringLeft()
        {
            if (this.stringVariable.IsNone || this.storeResult.IsNone)
                return;
            this.storeResult.Value = this.stringVariable.Value.Substring(0, Mathf.Clamp(this.charCount.Value, 0, this.stringVariable.Value.Length));
        }
    }
}
