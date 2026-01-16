// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PlayerPrefsGetFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory("PlayerPrefs")]
[Tooltip("Returns the value corresponding to key in the preference file if it exists.")]
public class PlayerPrefsGetFloat : FsmStateAction
{
  [Tooltip("Case sensitive key.")]
  [CompoundArray("Count", "Key", "Variable")]
  public FsmString[] keys;
  [UIHint(UIHint.Variable)]
  public FsmFloat[] variables;

  public override void Reset()
  {
    this.keys = new FsmString[1];
    this.variables = new FsmFloat[1];
  }

  public override void OnEnter()
  {
    for (int index = 0; index < this.keys.Length; ++index)
    {
      if (!this.keys[index].IsNone || !this.keys[index].Value.Equals(string.Empty))
        this.variables[index].Value = PlayerPrefs.GetFloat(this.keys[index].Value, !this.variables[index].IsNone ? this.variables[index].Value : 0.0f);
    }
    this.Finish();
  }
}
