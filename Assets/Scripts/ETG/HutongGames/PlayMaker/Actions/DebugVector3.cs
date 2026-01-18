#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Logs the value of a Vector3 Variable in the PlayMaker Log Window.")]
  [ActionCategory(ActionCategory.Debug)]
  public class DebugVector3 : BaseLogAction
  {
    [Tooltip("Info, Warning, or Error.")]
    public LogLevel logLevel;
    [Tooltip("The Vector3 variable to debug.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 vector3Variable;

    public override void Reset()
    {
      this.logLevel = LogLevel.Info;
      this.vector3Variable = (FsmVector3) null;
      base.Reset();
    }

    public override void OnEnter()
    {
      string text = "None";
      if (!this.vector3Variable.IsNone)
        text = $"{this.vector3Variable.Name}: {(object) this.vector3Variable.Value}";
      ActionHelpers.DebugLog(this.Fsm, this.logLevel, text, this.sendToUnityLog);
      this.Finish();
    }
  }
}
