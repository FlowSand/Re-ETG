#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Logic)]
  [Tooltip("Tests if any of the given Bool Variables are True.")]
  public class BoolAnyTrue : FsmStateAction
  {
    [Tooltip("The Bool variables to check.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmBool[] boolVariables;
    [Tooltip("Event to send if any of the Bool variables are True.")]
    public FsmEvent sendEvent;
    [Tooltip("Store the result in a Bool variable.")]
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;
    [Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.boolVariables = (FsmBool[]) null;
      this.sendEvent = (FsmEvent) null;
      this.storeResult = (FsmBool) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoAnyTrue();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoAnyTrue();

    private void DoAnyTrue()
    {
      if (this.boolVariables.Length == 0)
        return;
      this.storeResult.Value = false;
      for (int index = 0; index < this.boolVariables.Length; ++index)
      {
        if (this.boolVariables[index].Value)
        {
          this.Fsm.Event(this.sendEvent);
          this.storeResult.Value = true;
          break;
        }
      }
    }
  }
}
