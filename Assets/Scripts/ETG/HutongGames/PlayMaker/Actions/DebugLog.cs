// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DebugLog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
