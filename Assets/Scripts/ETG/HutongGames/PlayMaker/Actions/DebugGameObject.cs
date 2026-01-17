// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DebugGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Logs the value of a Game Object Variable in the PlayMaker Log Window.")]
  [ActionCategory(ActionCategory.Debug)]
  public class DebugGameObject : BaseLogAction
  {
    [Tooltip("Info, Warning, or Error.")]
    public LogLevel logLevel;
    [Tooltip("The GameObject variable to debug.")]
    [UIHint(UIHint.Variable)]
    public FsmGameObject gameObject;

    public override void Reset()
    {
      this.logLevel = LogLevel.Info;
      this.gameObject = (FsmGameObject) null;
      base.Reset();
    }

    public override void OnEnter()
    {
      string text = "None";
      if (!this.gameObject.IsNone)
        text = $"{this.gameObject.Name}: {(object) this.gameObject}";
      ActionHelpers.DebugLog(this.Fsm, this.logLevel, text, this.sendToUnityLog);
      this.Finish();
    }
  }
}
