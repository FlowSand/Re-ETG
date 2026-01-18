#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Logs the value of an Object Variable in the PlayMaker Log Window.")]
    public class DebugObject : BaseLogAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;
        [Tooltip("The Object variable to debug.")]
        [UIHint(UIHint.Variable)]
        public FsmObject fsmObject;

        public override void Reset()
        {
            this.logLevel = LogLevel.Info;
            this.fsmObject = (FsmObject) null;
            base.Reset();
        }

        public override void OnEnter()
        {
            string text = "None";
            if (!this.fsmObject.IsNone)
                text = $"{this.fsmObject.Name}: {(object) this.fsmObject}";
            ActionHelpers.DebugLog(this.Fsm, this.logLevel, text, this.sendToUnityLog);
            this.Finish();
        }
    }
}
