#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sets the value of an integer variable using a float value.")]
  [ActionCategory(ActionCategory.Math)]
  public class SetIntFromFloat : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmInt intVariable;
    public FsmFloat floatValue;
    public bool everyFrame;

    public override void Reset()
    {
      this.intVariable = (FsmInt) null;
      this.floatValue = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.intVariable.Value = (int) this.floatValue.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.intVariable.Value = (int) this.floatValue.Value;
  }
}
