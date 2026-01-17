// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnimateColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Animates the value of a Color Variable using an Animation Curve.")]
  [ActionCategory(ActionCategory.AnimateVariables)]
  public class AnimateColor : AnimateFsmAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmColor colorVariable;
    [RequiredField]
    public FsmAnimationCurve curveR;
    [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.r.")]
    public AnimateFsmAction.Calculation calculationR;
    [RequiredField]
    public FsmAnimationCurve curveG;
    [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.g.")]
    public AnimateFsmAction.Calculation calculationG;
    [RequiredField]
    public FsmAnimationCurve curveB;
    [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.b.")]
    public AnimateFsmAction.Calculation calculationB;
    [RequiredField]
    public FsmAnimationCurve curveA;
    [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.a.")]
    public AnimateFsmAction.Calculation calculationA;
    private bool finishInNextStep;

    public override void Reset()
    {
      base.Reset();
      FsmColor fsmColor = new FsmColor();
      fsmColor.UseVariable = true;
      this.colorVariable = fsmColor;
    }

    public override void OnEnter()
    {
      base.OnEnter();
      this.finishInNextStep = false;
      this.resultFloats = new float[4];
      this.fromFloats = new float[4];
      this.fromFloats[0] = !this.colorVariable.IsNone ? this.colorVariable.Value.r : 0.0f;
      this.fromFloats[1] = !this.colorVariable.IsNone ? this.colorVariable.Value.g : 0.0f;
      this.fromFloats[2] = !this.colorVariable.IsNone ? this.colorVariable.Value.b : 0.0f;
      this.fromFloats[3] = !this.colorVariable.IsNone ? this.colorVariable.Value.a : 0.0f;
      this.curves = new AnimationCurve[4];
      this.curves[0] = this.curveR.curve;
      this.curves[1] = this.curveG.curve;
      this.curves[2] = this.curveB.curve;
      this.curves[3] = this.curveA.curve;
      this.calculations = new AnimateFsmAction.Calculation[4];
      this.calculations[0] = this.calculationR;
      this.calculations[1] = this.calculationG;
      this.calculations[2] = this.calculationB;
      this.calculations[3] = this.calculationA;
      this.Init();
      if ((double) Math.Abs(this.delay.Value) >= 0.0099999997764825821)
        return;
      this.UpdateVariableValue();
    }

    private void UpdateVariableValue()
    {
      if (this.colorVariable.IsNone)
        return;
      this.colorVariable.Value = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      if (this.isRunning)
        this.UpdateVariableValue();
      if (this.finishInNextStep && !this.looping)
      {
        this.Finish();
        this.Fsm.Event(this.finishEvent);
      }
      if (!this.finishAction || this.finishInNextStep)
        return;
      this.UpdateVariableValue();
      this.finishInNextStep = true;
    }
  }
}
