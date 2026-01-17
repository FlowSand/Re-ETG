// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PlayerPrefsDeleteKey
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("PlayerPrefs")]
[Tooltip("Removes key and its corresponding value from the preferences.")]
public class PlayerPrefsDeleteKey : FsmStateAction
{
  public FsmString key;

  public override void Reset() => this.key = (FsmString) string.Empty;

  public override void OnEnter()
  {
    if (!this.key.IsNone && !this.key.Value.Equals(string.Empty))
      PlayerPrefs.DeleteKey(this.key.Value);
    this.Finish();
  }
}
