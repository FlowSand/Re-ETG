// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AnimateFloatV2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Animates the value of a Float Variable using an Animation Curve.")]
  [ActionCategory(ActionCategory.AnimateVariables)]
  public class AnimateFloatV2 : AnimateFsmAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmFloat floatVariable;
    [RequiredField]
    public FsmAnimationCurve animCurve;
    [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to floatVariable")]
    public AnimateFsmAction.Calculation calculation;
    private bool finishInNextStep;

    public override void Reset()
    {
      base.Reset();
      FsmFloat fsmFloat = new FsmFloat();
      fsmFloat.UseVariable = true;
      this.floatVariable = fsmFloat;
    }

    public override void OnEnter()
    {
      base.OnEnter();
      this.finishInNextStep = false;
      this.resultFloats = new float[1];
      this.fromFloats = new float[1];
      this.fromFloats[0] = !this.floatVariable.IsNone ? this.floatVariable.Value : 0.0f;
      this.calculations = new AnimateFsmAction.Calculation[1];
      this.calculations[0] = this.calculation;
      this.curves = new AnimationCurve[1];
      this.curves[0] = this.animCurve.curve;
      this.Init();
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
      base.OnUpdate();
      if (!this.floatVariable.IsNone && this.isRunning)
        this.floatVariable.Value = this.resultFloats[0];
      if (this.finishInNextStep && !this.looping)
      {
        this.Finish();
        if (this.finishEvent != null)
          this.Fsm.Event(this.finishEvent);
      }
      if (!this.finishAction || this.finishInNextStep)
        return;
      if (!this.floatVariable.IsNone)
        this.floatVariable.Value = this.resultFloats[0];
      this.finishInNextStep = true;
    }
  }
}
