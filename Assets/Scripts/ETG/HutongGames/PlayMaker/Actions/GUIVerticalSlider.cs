using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("GUI Vertical Slider connected to a Float Variable.")]
    [ActionCategory(ActionCategory.GUI)]
    public class GUIVerticalSlider : GUIAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat floatVariable;
        [RequiredField]
        public FsmFloat topValue;
        [RequiredField]
        public FsmFloat bottomValue;
        public FsmString sliderStyle;
        public FsmString thumbStyle;

        public override void Reset()
        {
            base.Reset();
            this.floatVariable = (FsmFloat) null;
            this.topValue = (FsmFloat) 100f;
            this.bottomValue = (FsmFloat) 0.0f;
            this.sliderStyle = (FsmString) "verticalslider";
            this.thumbStyle = (FsmString) "verticalsliderthumb";
            this.width = (FsmFloat) 0.1f;
        }

        public override void OnGUI()
        {
            base.OnGUI();
            if (this.floatVariable == null)
                return;
            this.floatVariable.Value = GUI.VerticalSlider(this.rect, this.floatVariable.Value, this.topValue.Value, this.bottomValue.Value, (GUIStyle) (!(this.sliderStyle.Value != string.Empty) ? "verticalslider" : this.sliderStyle.Value), (GUIStyle) (!(this.thumbStyle.Value != string.Empty) ? "verticalsliderthumb" : this.thumbStyle.Value));
        }
    }
}
