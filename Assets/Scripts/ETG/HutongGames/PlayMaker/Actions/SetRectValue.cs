#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Rect)]
  [Tooltip("Sets the value of a Rect Variable.")]
  public class SetRectValue : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmRect rectVariable;
    [RequiredField]
    public FsmRect rectValue;
    public bool everyFrame;

    public override void Reset()
    {
      this.rectVariable = (FsmRect) null;
      this.rectValue = (FsmRect) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.rectVariable.Value = this.rectValue.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.rectVariable.Value = this.rectValue.Value;
  }
}
