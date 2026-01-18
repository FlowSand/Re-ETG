#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Compare 2 Object Variables and send events based on the result.")]
  [ActionCategory(ActionCategory.Logic)]
  public class ObjectCompare : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmObject objectVariable;
    [RequiredField]
    public FsmObject compareTo;
    [Tooltip("Event to send if the 2 object values are equal.")]
    public FsmEvent equalEvent;
    [Tooltip("Event to send if the 2 object values are not equal.")]
    public FsmEvent notEqualEvent;
    [UIHint(UIHint.Variable)]
    [Tooltip("Store the result in a variable.")]
    public FsmBool storeResult;
    [Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.objectVariable = (FsmObject) null;
      this.compareTo = (FsmObject) null;
      this.storeResult = (FsmBool) null;
      this.equalEvent = (FsmEvent) null;
      this.notEqualEvent = (FsmEvent) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoObjectCompare();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoObjectCompare();

    private void DoObjectCompare()
    {
      bool flag = this.objectVariable.Value == this.compareTo.Value;
      this.storeResult.Value = flag;
      this.Fsm.Event(!flag ? this.notEqualEvent : this.equalEvent);
    }
  }
}
