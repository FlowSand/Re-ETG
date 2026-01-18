#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Logs the value of a Vector2 Variable in the PlayMaker Log Window.")]
    [ActionCategory(ActionCategory.Debug)]
    public class DebugVector2 : FsmStateAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;
        [Tooltip("Prints the value of a Vector2 variable in the PlayMaker log window.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 vector2Variable;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.vector2Variable = (FsmVector2) null;
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.vector2Variable.IsNone)
                text = $"{this.vector2Variable.Name}: {(object) this.vector2Variable.Value}";
            ActionHelpers.DebugLog(this.Fsm, this.logLevel, text);
            this.Finish();
        }
    }
}
