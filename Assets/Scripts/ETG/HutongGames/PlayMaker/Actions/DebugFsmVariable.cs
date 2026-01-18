#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Debug)]
  [Tooltip("Print the value of any FSM Variable in the PlayMaker Log Window.")]
  public class DebugFsmVariable : BaseLogAction
  {
    [Tooltip("Info, Warning, or Error.")]
    public LogLevel logLevel;
    [UIHint(UIHint.Variable)]
    [HideTypeFilter]
    [Tooltip("The variable to debug.")]
    public FsmVar variable;

    public override void Reset()
    {
      this.logLevel = LogLevel.Info;
      this.variable = (FsmVar) null;
      base.Reset();
    }

    public override void OnEnter()
    {
      ActionHelpers.DebugLog(this.Fsm, this.logLevel, this.variable.DebugString(), this.sendToUnityLog);
      this.Finish();
    }
  }
}
