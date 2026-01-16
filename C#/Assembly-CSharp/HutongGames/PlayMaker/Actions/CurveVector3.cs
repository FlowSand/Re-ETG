// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CurveVector3
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.AnimateVariables)]
[HutongGames.PlayMaker.Tooltip("Animates the value of a Vector3 Variable FROM-TO with assistance of Deformation Curves.")]
public class CurveVector3 : CurveFsmAction
{
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmVector3 vectorVariable;
  [RequiredField]
  public FsmVector3 fromValue;
  [RequiredField]
  public FsmVector3 toValue;
  [RequiredField]
  public FsmAnimationCurve curveX;
  [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
  public CurveFsmAction.Calculation calculationX;
  [RequiredField]
  public FsmAnimationCurve curveY;
  [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
  public CurveFsmAction.Calculation calculationY;
  [RequiredField]
  public FsmAnimationCurve curveZ;
  [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.z and toValue.z.")]
  public CurveFsmAction.Calculation calculationZ;
  private Vector3 vct;
  private bool finishInNextStep;

  public override void Reset()
  {
    base.Reset();
    FsmVector3 fsmVector3_1 = new FsmVector3();
    fsmVector3_1.UseVariable = true;
    this.vectorVariable = fsmVector3_1;
    FsmVector3 fsmVector3_2 = new FsmVector3();
    fsmVector3_2.UseVariable = true;
    this.toValue = fsmVector3_2;
    FsmVector3 fsmVector3_3 = new FsmVector3();
    fsmVector3_3.UseVariable = true;
    this.fromValue = fsmVector3_3;
  }

  public override void OnEnter()
  {
    base.OnEnter();
    this.finishInNextStep = false;
    this.resultFloats = new float[3];
    this.fromFloats = new float[3];
    this.fromFloats[0] = !this.fromValue.IsNone ? this.fromValue.Value.x : 0.0f;
    this.fromFloats[1] = !this.fromValue.IsNone ? this.fromValue.Value.y : 0.0f;
    this.fromFloats[2] = !this.fromValue.IsNone ? this.fromValue.Value.z : 0.0f;
    this.toFloats = new float[3];
    this.toFloats[0] = !this.toValue.IsNone ? this.toValue.Value.x : 0.0f;
    this.toFloats[1] = !this.toValue.IsNone ? this.toValue.Value.y : 0.0f;
    this.toFloats[2] = !this.toValue.IsNone ? this.toValue.Value.z : 0.0f;
    this.curves = new AnimationCurve[3];
    this.curves[0] = this.curveX.curve;
    this.curves[1] = this.curveY.curve;
    this.curves[2] = this.curveZ.curve;
    this.calculations = new CurveFsmAction.Calculation[3];
    this.calculations[0] = this.calculationX;
    this.calculations[1] = this.calculationY;
    this.calculations[2] = this.calculationZ;
    this.Init();
  }

  public override void OnExit()
  {
  }

  public override void OnUpdate()
  {
    base.OnUpdate();
    if (!this.vectorVariable.IsNone && this.isRunning)
    {
      this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
      this.vectorVariable.Value = this.vct;
    }
    if (this.finishInNextStep && !this.looping)
    {
      this.Finish();
      if (this.finishEvent != null)
        this.Fsm.Event(this.finishEvent);
    }
    if (!this.finishAction || this.finishInNextStep)
      return;
    if (!this.vectorVariable.IsNone)
    {
      this.vct = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
      this.vectorVariable.Value = this.vct;
    }
    this.finishInNextStep = true;
  }
}
