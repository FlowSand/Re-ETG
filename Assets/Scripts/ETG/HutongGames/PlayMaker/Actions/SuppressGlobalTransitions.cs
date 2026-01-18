#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [Tooltip("Prevents the FSM from firing global transitions.")]
  public class SuppressGlobalTransitions : FsmStateAction, INonFinishingState
  {
    public override void OnEnter()
    {
      this.Fsm.SuppressGlobalTransitions = true;
      this.Finish();
    }
  }
}
