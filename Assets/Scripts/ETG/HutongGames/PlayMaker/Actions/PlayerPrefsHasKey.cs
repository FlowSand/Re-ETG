// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PlayerPrefsHasKey
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Returns true if key exists in the preferences.")]
  [ActionCategory("PlayerPrefs")]
  public class PlayerPrefsHasKey : FsmStateAction
  {
    [RequiredField]
    public FsmString key;
    [Title("Store Result")]
    [UIHint(UIHint.Variable)]
    public FsmBool variable;
    [Tooltip("Event to send if key exists.")]
    public FsmEvent trueEvent;
    [Tooltip("Event to send if key does not exist.")]
    public FsmEvent falseEvent;

    public override void Reset() => this.key = (FsmString) string.Empty;

    public override void OnEnter()
    {
      this.Finish();
      if (!this.key.IsNone && !this.key.Value.Equals(string.Empty))
        this.variable.Value = PlayerPrefs.HasKey(this.key.Value);
      this.Fsm.Event(!this.variable.Value ? this.falseEvent : this.trueEvent);
    }
  }
}
