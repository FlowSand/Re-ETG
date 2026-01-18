#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [Tooltip("Sends an Event based on the value of an Integer Variable.")]
  public class IntSwitch : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt intVariable;
    [CompoundArray("Int Switches", "Compare Int", "Send Event")]
    public FsmInt[] compareTo;
    public FsmEvent[] sendEvent;
    public bool everyFrame;

    public override void Reset()
    {
      this.intVariable = (FsmInt) null;
      this.compareTo = new FsmInt[1];
      this.sendEvent = new FsmEvent[1];
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoIntSwitch();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoIntSwitch();

    private void DoIntSwitch()
    {
      if (this.intVariable.IsNone)
        return;
      for (int index = 0; index < this.compareTo.Length; ++index)
      {
        if (this.intVariable.Value == this.compareTo[index].Value)
        {
          this.Fsm.Event(this.sendEvent[index]);
          break;
        }
      }
    }
  }
}
