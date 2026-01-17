// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ConvertBoolToFloat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Convert)]
[Tooltip("Converts a Bool value to a Float value.")]
public class ConvertBoolToFloat : FsmStateAction
{
  [Tooltip("The Bool variable to test.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmBool boolVariable;
  [Tooltip("The Float variable to set based on the Bool variable value.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat floatVariable;
  [Tooltip("Float value if Bool variable is false.")]
  public FsmFloat falseValue;
  [Tooltip("Float value if Bool variable is true.")]
  public FsmFloat trueValue;
  [Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.boolVariable = (FsmBool) null;
    this.floatVariable = (FsmFloat) null;
    this.falseValue = (FsmFloat) 0.0f;
    this.trueValue = (FsmFloat) 1f;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoConvertBoolToFloat();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoConvertBoolToFloat();

  private void DoConvertBoolToFloat()
  {
    this.floatVariable.Value = !this.boolVariable.Value ? this.falseValue.Value : this.trueValue.Value;
  }
}
