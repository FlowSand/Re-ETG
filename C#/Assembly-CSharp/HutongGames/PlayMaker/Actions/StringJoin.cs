// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StringJoin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Join an array of strings into a single string.")]
[ActionCategory(ActionCategory.String)]
public class StringJoin : FsmStateAction
{
  [Tooltip("Array of string to join into a single string.")]
  [UIHint(UIHint.Variable)]
  [ArrayEditor(VariableType.String, "", 0, 0, 65536 /*0x010000*/)]
  public FsmArray stringArray;
  [Tooltip("Seperator to add between each string.")]
  public FsmString separator;
  [Tooltip("Store the joined string in string variable.")]
  [UIHint(UIHint.Variable)]
  public FsmString storeResult;

  public override void OnEnter()
  {
    if (!this.stringArray.IsNone && !this.storeResult.IsNone)
      this.storeResult.Value = string.Join(this.separator.Value, this.stringArray.stringValues);
    this.Finish();
  }
}
