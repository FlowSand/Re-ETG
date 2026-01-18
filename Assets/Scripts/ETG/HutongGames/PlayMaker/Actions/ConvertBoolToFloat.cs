#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Convert)]
  [Tooltip("Converts a Bool value to a Float value.")]
  public class ConvertBoolToFloat : FsmStateAction
  {
    [Tooltip("The Bool variable to test.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool boolVariable;
    [Tooltip("The Float variable to set based on the Bool variable value.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat floatVariable;
    [Tooltip("Float value if Bool variable is false.")]
    public FsmFloat falseValue;
    [Tooltip("Float value if Bool variable is true.")]
    public FsmFloat trueValue;
    [Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.boolVariable = (FsmBool) null;
      this.floatVariable = (FsmFloat) null;
      this.falseValue = (FsmFloat) 0.0f;
      this.trueValue = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoConvertBoolToFloat();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoConvertBoolToFloat();

    private void DoConvertBoolToFloat()
    {
      this.floatVariable.Value = !this.boolVariable.Value ? this.falseValue.Value : this.trueValue.Value;
    }
  }
}
