#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [Tooltip("Sends Events based on the value of a Boolean Variable.")]
  public class BoolTest : FsmStateAction
  {
    [Tooltip("The Bool variable to test.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool boolVariable;
    [Tooltip("Event to send if the Bool variable is True.")]
    public FsmEvent isTrue;
    [Tooltip("Event to send if the Bool variable is False.")]
    public FsmEvent isFalse;
    [Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.boolVariable = (FsmBool) null;
      this.isTrue = (FsmEvent) null;
      this.isFalse = (FsmEvent) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.Fsm.Event(!this.boolVariable.Value ? this.isFalse : this.isTrue);
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.Fsm.Event(!this.boolVariable.Value ? this.isFalse : this.isTrue);
    }
  }
}
