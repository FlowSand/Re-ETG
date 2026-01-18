#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sends an Event based on the value of an Enum Variable.")]
  [ActionCategory(ActionCategory.Logic)]
  public class EnumSwitch : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmEnum enumVariable;
    [CompoundArray("Enum Switches", "Compare Enum Values", "Send")]
    [MatchFieldType("enumVariable")]
    public FsmEnum[] compareTo;
    public FsmEvent[] sendEvent;
    public bool everyFrame;

    public override void Reset()
    {
      this.enumVariable = (FsmEnum) null;
      this.compareTo = new FsmEnum[0];
      this.sendEvent = new FsmEvent[0];
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoEnumSwitch();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoEnumSwitch();

    private void DoEnumSwitch()
    {
      if (this.enumVariable.IsNone)
        return;
      for (int index = 0; index < this.compareTo.Length; ++index)
      {
        if (object.Equals((object) this.enumVariable.Value, (object) this.compareTo[index].Value))
        {
          this.Fsm.Event(this.sendEvent[index]);
          break;
        }
      }
    }
  }
}
