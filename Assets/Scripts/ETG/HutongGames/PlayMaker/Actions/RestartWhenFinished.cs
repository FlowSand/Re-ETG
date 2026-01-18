#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("When all other actions on this state are finished, send a RESTART event.")]
    [ActionCategory(".Brave")]
    public class RestartWhenFinished : FsmStateAction, INonFinishingState
    {
        public override string ErrorCheck()
        {
            string empty = string.Empty;
            this.Fsm.GetEvent("RESTART");
            return empty + BravePlayMakerUtility.CheckGlobalTransitionExists(this.Fsm, "RESTART");
        }

        public override void OnEnter()
        {
            if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
                return;
            this.GoToStartState();
        }

        public override void OnUpdate()
        {
            if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
                return;
            this.GoToStartState();
        }

        private void GoToStartState()
        {
            if (this.Fsm.SuppressGlobalTransitions)
            {
                foreach (FsmStateAction action in this.State.Actions)
                {
                    if (action is ResumeGlobalTransitions)
                    {
                        this.Fsm.SuppressGlobalTransitions = false;
                        break;
                    }
                }
            }
            this.Fsm.Event("RESTART");
            this.Finish();
        }
    }
}
