#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Logs the value of an Enum Variable in the PlayMaker Log Window.")]
  [ActionCategory(ActionCategory.Debug)]
  public class DebugEnum : BaseLogAction
  {
    [Tooltip("Info, Warning, or Error.")]
    public LogLevel logLevel;
    [Tooltip("The Enum Variable to debug.")]
    [UIHint(UIHint.Variable)]
    public FsmEnum enumVariable;

    public override void Reset()
    {
      this.logLevel = LogLevel.Info;
      this.enumVariable = (FsmEnum) null;
      base.Reset();
    }

    public override void OnEnter()
    {
      string text = "None";
      if (!this.enumVariable.IsNone)
        text = $"{this.enumVariable.Name}: {(object) this.enumVariable.Value}";
      ActionHelpers.DebugLog(this.Fsm, this.logLevel, text, this.sendToUnityLog);
      this.Finish();
    }
  }
}
