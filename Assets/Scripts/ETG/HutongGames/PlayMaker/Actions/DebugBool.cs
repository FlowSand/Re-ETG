#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Logs the value of a Bool Variable in the PlayMaker Log Window.")]
  [ActionCategory(ActionCategory.Debug)]
  public class DebugBool : BaseLogAction
  {
    [Tooltip("Info, Warning, or Error.")]
    public LogLevel logLevel;
    [Tooltip("The Bool variable to debug.")]
    [UIHint(UIHint.Variable)]
    public FsmBool boolVariable;

    public override void Reset()
    {
      this.logLevel = LogLevel.Info;
      this.boolVariable = (FsmBool) null;
      base.Reset();
    }

    public override void OnEnter()
    {
      string text = "None";
      if (!this.boolVariable.IsNone)
        text = $"{this.boolVariable.Name}: {(object) this.boolVariable.Value}";
      ActionHelpers.DebugLog(this.Fsm, this.logLevel, text, this.sendToUnityLog);
      this.Finish();
    }
  }
}
