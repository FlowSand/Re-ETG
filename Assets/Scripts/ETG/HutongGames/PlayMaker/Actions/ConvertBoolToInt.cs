#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Converts a Bool value to an Integer value.")]
  [ActionCategory(ActionCategory.Convert)]
  public class ConvertBoolToInt : FsmStateAction
  {
    [Tooltip("The Bool variable to test.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool boolVariable;
    [Tooltip("The Integer variable to set based on the Bool variable value.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt intVariable;
    [Tooltip("Integer value if Bool variable is false.")]
    public FsmInt falseValue;
    [Tooltip("Integer value if Bool variable is false.")]
    public FsmInt trueValue;
    [Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.boolVariable = (FsmBool) null;
      this.intVariable = (FsmInt) null;
      this.falseValue = (FsmInt) 0;
      this.trueValue = (FsmInt) 1;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoConvertBoolToInt();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoConvertBoolToInt();

    private void DoConvertBoolToInt()
    {
      this.intVariable.Value = !this.boolVariable.Value ? this.falseValue.Value : this.trueValue.Value;
    }
  }
}
