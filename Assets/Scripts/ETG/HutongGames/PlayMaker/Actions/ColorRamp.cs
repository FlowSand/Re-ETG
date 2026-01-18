using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Samples a Color on a continuous Colors gradient.")]
    [ActionCategory(ActionCategory.Color)]
    public class ColorRamp : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Array of colors to defining the gradient.")]
        public FsmColor[] colors;
        [HutongGames.PlayMaker.Tooltip("Point on the gradient to sample. Should be between 0 and the number of colors in the gradient.")]
        [RequiredField]
        public FsmFloat sampleAt;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Store the sampled color in a Color variable.")]
        [UIHint(UIHint.Variable)]
        public FsmColor storeColor;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.colors = new FsmColor[3];
            this.sampleAt = (FsmFloat) 0.0f;
            this.storeColor = (FsmColor) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoColorRamp();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoColorRamp();

        private void DoColorRamp()
        {
            if (this.colors == null || this.colors.Length == 0 || this.sampleAt == null || this.storeColor == null)
                return;
            float f = Mathf.Clamp(this.sampleAt.Value, 0.0f, (float) (this.colors.Length - 1));
            this.storeColor.Value = (double) f != 0.0 ? ((double) f != (double) this.colors.Length ? Color.Lerp(this.colors[Mathf.FloorToInt(f)].Value, this.colors[Mathf.CeilToInt(f)].Value, f - Mathf.Floor(f)) : this.colors[this.colors.Length - 1].Value) : this.colors[0].Value;
        }

        public override string ErrorCheck()
        {
            return this.colors.Length < 2 ? "Define at least 2 colors to make a gradient." : (string) null;
        }
    }
}
