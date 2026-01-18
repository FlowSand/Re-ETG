using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Convert)]
    [HutongGames.PlayMaker.Tooltip("Converts a Bool value to a Color.")]
    public class ConvertBoolToColor : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Bool variable to test.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmBool boolVariable;
        [HutongGames.PlayMaker.Tooltip("The Color variable to set based on the bool variable value.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmColor colorVariable;
        [HutongGames.PlayMaker.Tooltip("Color if Bool variable is false.")]
        public FsmColor falseColor;
        [HutongGames.PlayMaker.Tooltip("Color if Bool variable is true.")]
        public FsmColor trueColor;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.boolVariable = (FsmBool) null;
            this.colorVariable = (FsmColor) null;
            this.falseColor = (FsmColor) Color.black;
            this.trueColor = (FsmColor) Color.white;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoConvertBoolToColor();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoConvertBoolToColor();

        private void DoConvertBoolToColor()
        {
            this.colorVariable.Value = !this.boolVariable.Value ? this.falseColor.Value : this.trueColor.Value;
        }
    }
}
