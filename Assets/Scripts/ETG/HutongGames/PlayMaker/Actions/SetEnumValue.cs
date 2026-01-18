#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Enum)]
  [Tooltip("Sets the value of an Enum Variable.")]
  public class SetEnumValue : FsmStateAction
  {
    [Tooltip("The Enum Variable to set.")]
    [UIHint(UIHint.Variable)]
    public FsmEnum enumVariable;
    [Tooltip("The Enum value to set the variable to.")]
    [MatchFieldType("enumVariable")]
    public FsmEnum enumValue;
    [Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.enumVariable = (FsmEnum) null;
      this.enumValue = (FsmEnum) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetEnumValue();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetEnumValue();

    private void DoSetEnumValue() => this.enumVariable.Value = this.enumValue.Value;
  }
}
