#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [Tooltip("Allows the FSM to fire global transitions again.")]
  public class ResumeGlobalTransitions : FsmStateAction, INonFinishingState
  {
    public override void OnEnter()
    {
      if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
        return;
      this.Fsm.SuppressGlobalTransitions = false;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
        return;
      this.Fsm.SuppressGlobalTransitions = false;
      this.Finish();
    }
  }
}
