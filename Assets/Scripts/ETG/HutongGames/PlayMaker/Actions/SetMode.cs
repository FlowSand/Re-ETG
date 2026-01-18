#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [Tooltip("Sets the variable currentMode to the given string.")]
  public class SetMode : FsmStateAction
  {
    [Tooltip("Mode to set currentMode to.")]
    public FsmString mode;
    [Tooltip("Travel immediately to the new mode.")]
    public FsmBool jumpToMode;

    public override void Reset() => this.mode = (FsmString) null;

    public override string ErrorCheck()
    {
      string str = string.Empty + BravePlayMakerUtility.CheckCurrentModeVariable(this.Fsm);
      if (!this.mode.Value.StartsWith("mode"))
        str += "Let's be civil and start all mode names with \"mode\", okay?\n";
      return str + BravePlayMakerUtility.CheckEventExists(this.Fsm, this.mode.Value) + BravePlayMakerUtility.CheckGlobalTransitionExists(this.Fsm, this.mode.Value);
    }

    public override void OnEnter()
    {
      this.Fsm.Variables.GetFsmString("currentMode").Value = this.mode.Value;
      if (this.jumpToMode.Value)
        this.JumpToState();
      this.Finish();
    }

    private void JumpToState()
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
      this.Fsm.Event(this.mode.Value);
    }
  }
}
