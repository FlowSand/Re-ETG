// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SampleCurve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Gets the value of a curve at a given time and stores it in a Float Variable. NOTE: This can be used for more than just animation! It's a general way to transform an input number into an output number using a curve (e.g., linear input -> bell curve).")]
[ActionCategory(ActionCategory.Math)]
public class SampleCurve : FsmStateAction
{
  [RequiredField]
  public FsmAnimationCurve curve;
  [RequiredField]
  public FsmFloat sampleAt;
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat storeValue;
  public bool everyFrame;

  public override void Reset()
  {
    this.curve = (FsmAnimationCurve) null;
    this.sampleAt = (FsmFloat) null;
    this.storeValue = (FsmFloat) null;
    this.everyFrame = false;
  }

  public override void OnEnter()
  {
    this.DoSampleCurve();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoSampleCurve();

  private void DoSampleCurve()
  {
    if (this.curve == null || this.curve.curve == null || this.storeValue == null)
      return;
    this.storeValue.Value = this.curve.curve.Evaluate(this.sampleAt.Value);
  }
}
