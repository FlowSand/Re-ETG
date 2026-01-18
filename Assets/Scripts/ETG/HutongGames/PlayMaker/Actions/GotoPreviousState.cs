#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.StateMachine)]
  [Tooltip("Immediately return to the previously active state.")]
  public class GotoPreviousState : FsmStateAction
  {
    public override void Reset()
    {
    }

    public override void OnEnter()
    {
      if (this.Fsm.PreviousActiveState != null)
      {
        this.Log("Goto Previous State: " + this.Fsm.PreviousActiveState.Name);
        this.Fsm.GotoPreviousState();
      }
      this.Finish();
    }
  }
}
