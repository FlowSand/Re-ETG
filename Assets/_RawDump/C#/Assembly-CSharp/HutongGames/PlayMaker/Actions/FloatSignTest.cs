// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FloatSignTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Sends Events based on the sign of a Float.")]
[ActionCategory(ActionCategory.Logic)]
public class FloatSignTest : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [Tooltip("The float variable to test.")]
  [RequiredField]
  public FsmFloat floatValue;
  [Tooltip("Event to send if the float variable is positive.")]
  public FsmEvent isPositive;
  [Tooltip("Event to send if the float variable is negative.")]
  public FsmEvent isNegative;
  [Tooltip("Repeat every frame. Useful if the variable is changing and you're waiting for a particular result.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.floatValue = (FsmFloat) 0.0f;
    this.isPositive = (FsmEvent) null;
    this.isNegative = (FsmEvent) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSignTest();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSignTest();

  private void DoSignTest()
  {
    if (this.floatValue == null)
      return;
    this.Fsm.Event((double) this.floatValue.Value >= 0.0 ? this.isPositive : this.isNegative);
  }

  public override string ErrorCheck()
  {
    return FsmEvent.IsNullOrEmpty(this.isPositive) && FsmEvent.IsNullOrEmpty(this.isNegative) ? "Action sends no events!" : string.Empty;
  }
}
