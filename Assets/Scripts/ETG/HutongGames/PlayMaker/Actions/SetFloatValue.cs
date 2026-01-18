#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sets the value of a Float Variable.")]
  [ActionCategory(ActionCategory.Math)]
  public class SetFloatValue : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmFloat floatVariable;
    [RequiredField]
    public FsmFloat floatValue;
    public bool everyFrame;

    public override void Reset()
    {
      this.floatVariable = (FsmFloat) null;
      this.floatValue = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.floatVariable.Value = this.floatValue.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.floatVariable.Value = this.floatValue.Value;
  }
}
