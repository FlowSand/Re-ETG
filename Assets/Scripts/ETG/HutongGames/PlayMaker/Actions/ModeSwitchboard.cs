#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Switchboard to jump to different NPC modes.")]
    [ActionCategory(".Brave")]
    public class ModeSwitchboard : FsmStateAction
    {
        public override string ErrorCheck()
        {
            string str = string.Empty + BravePlayMakerUtility.CheckCurrentModeVariable(this.Fsm);
            FsmString fsmString = this.Fsm.Variables.GetFsmString("currentMode");
            return str + BravePlayMakerUtility.CheckEventExists(this.Fsm, fsmString.Value) + BravePlayMakerUtility.CheckGlobalTransitionExists(this.Fsm, fsmString.Value);
        }

        public override void OnEnter()
        {
            this.Fsm.Event(this.Fsm.Variables.GetFsmString("currentMode").Value);
            this.Finish();
        }
    }
}
