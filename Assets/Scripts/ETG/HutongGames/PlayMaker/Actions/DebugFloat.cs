#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of a Float Variable in the PlayMaker Log Window.")]
    public class DebugFloat : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;
        [Tooltip("The Float variable to debug.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat floatVariable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.floatVariable = (FsmFloat) null;
            base.Reset();
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.floatVariable.IsNone)
                text = $"{this.floatVariable.Name}: {(object) this.floatVariable.Value}";
            ActionHelpers.DebugLog(this.Fsm, this.logLevel, text, this.sendToUnityLog);
            this.Finish();
        }
    }
}
