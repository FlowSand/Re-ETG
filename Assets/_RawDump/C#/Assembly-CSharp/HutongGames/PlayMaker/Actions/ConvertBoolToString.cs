// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ConvertBoolToString
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Converts a Bool value to a String value.")]
[ActionCategory(ActionCategory.Convert)]
public class ConvertBoolToString : FsmStateAction
{
  [Tooltip("The Bool variable to test.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmBool boolVariable;
  [Tooltip("The String variable to set based on the Bool variable value.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmString stringVariable;
  [Tooltip("String value if Bool variable is false.")]
  public FsmString falseString;
  [Tooltip("String value if Bool variable is true.")]
  public FsmString trueString;
  [Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.boolVariable = (FsmBool) null;
    this.stringVariable = (FsmString) null;
    this.falseString = (FsmString) "False";
    this.trueString = (FsmString) "True";
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoConvertBoolToString();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoConvertBoolToString();

  private void DoConvertBoolToString()
  {
    this.stringVariable.Value = !this.boolVariable.Value ? this.falseString.Value : this.trueString.Value;
  }
}
