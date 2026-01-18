#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Logs the value of an Integer Variable in the PlayMaker Log Window.")]
    [ActionCategory(ActionCategory.Debug)]
    public class DebugInt : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;
        [Tooltip("The Int variable to debug.")]
        [UIHint(UIHint.Variable)]
        public FsmInt intVariable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.intVariable = (FsmInt) null;
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.intVariable.IsNone)
                text = $"{this.intVariable.Name}: {(object) this.intVariable.Value}";
            ActionHelpers.DebugLog(this.Fsm, this.logLevel, text, this.sendToUnityLog);
            this.Finish();
        }
    }
}
