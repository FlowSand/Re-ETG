#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sends a log message to the PlayMaker Log Window.")]
  [ActionCategory(ActionCategory.Debug)]
  public class DebugLog : BaseLogAction
  {
    [Tooltip("Info, Warning, or Error.")]
    public LogLevel logLevel;
    [Tooltip("Text to send to the log.")]
    public FsmString text;

    public override void Reset()
    {
      this.logLevel = LogLevel.Info;
      this.text = (FsmString) string.Empty;
      base.Reset();
    }

    public override void OnEnter()
    {
      if (!string.IsNullOrEmpty(this.text.Value))
        ActionHelpers.DebugLog(this.Fsm, this.logLevel, this.text.Value, this.sendToUnityLog);
      this.Finish();
    }
  }
}
