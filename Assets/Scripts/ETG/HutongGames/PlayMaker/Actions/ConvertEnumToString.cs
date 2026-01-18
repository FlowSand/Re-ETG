#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Convert)]
  [Tooltip("Converts an Enum value to a String value.")]
  public class ConvertEnumToString : FsmStateAction
  {
    [Tooltip("The Enum variable to convert.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmEnum enumVariable;
    [Tooltip("The String variable to store the converted value.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString stringVariable;
    [Tooltip("Repeat every frame. Useful if the Enum variable is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.enumVariable = (FsmEnum) null;
      this.stringVariable = (FsmString) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoConvertEnumToString();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoConvertEnumToString();

    private void DoConvertEnumToString()
    {
      this.stringVariable.Value = this.enumVariable.Value == null ? string.Empty : this.enumVariable.Value.ToString();
    }
  }
}
