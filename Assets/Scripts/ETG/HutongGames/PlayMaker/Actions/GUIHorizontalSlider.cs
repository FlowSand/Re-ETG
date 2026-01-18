using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUI)]
  [HutongGames.PlayMaker.Tooltip("GUI Horizontal Slider connected to a Float Variable.")]
  public class GUIHorizontalSlider : GUIAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat floatVariable;
    [RequiredField]
    public FsmFloat leftValue;
    [RequiredField]
    public FsmFloat rightValue;
    public FsmString sliderStyle;
    public FsmString thumbStyle;

    public override void Reset()
    {
      base.Reset();
      this.floatVariable = (FsmFloat) null;
      this.leftValue = (FsmFloat) 0.0f;
      this.rightValue = (FsmFloat) 100f;
      this.sliderStyle = (FsmString) "horizontalslider";
      this.thumbStyle = (FsmString) "horizontalsliderthumb";
    }

    public override void OnGUI()
    {
      base.OnGUI();
      if (this.floatVariable == null)
        return;
      this.floatVariable.Value = GUI.HorizontalSlider(this.rect, this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, (GUIStyle) (!(this.sliderStyle.Value != string.Empty) ? "horizontalslider" : this.sliderStyle.Value), (GUIStyle) (!(this.thumbStyle.Value != string.Empty) ? "horizontalsliderthumb" : this.thumbStyle.Value));
    }
  }
}
