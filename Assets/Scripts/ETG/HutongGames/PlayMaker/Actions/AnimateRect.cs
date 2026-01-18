using System;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("AnimateVariables")]
    [HutongGames.PlayMaker.Tooltip("Animates the value of a Rect Variable using an Animation Curve.")]
    public class AnimateRect : AnimateFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmRect rectVariable;
        [RequiredField]
        public FsmAnimationCurve curveX;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.x.")]
        public AnimateFsmAction.Calculation calculationX;
        [RequiredField]
        public FsmAnimationCurve curveY;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.y.")]
        public AnimateFsmAction.Calculation calculationY;
        [RequiredField]
        public FsmAnimationCurve curveW;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.width.")]
        public AnimateFsmAction.Calculation calculationW;
        [RequiredField]
        public FsmAnimationCurve curveH;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.height.")]
        public AnimateFsmAction.Calculation calculationH;
        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            FsmRect fsmRect = new FsmRect();
            fsmRect.UseVariable = true;
            this.rectVariable = fsmRect;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            this.resultFloats = new float[4];
            this.fromFloats = new float[4];
            this.fromFloats[0] = !this.rectVariable.IsNone ? this.rectVariable.Value.x : 0.0f;
            this.fromFloats[1] = !this.rectVariable.IsNone ? this.rectVariable.Value.y : 0.0f;
            this.fromFloats[2] = !this.rectVariable.IsNone ? this.rectVariable.Value.width : 0.0f;
            this.fromFloats[3] = !this.rectVariable.IsNone ? this.rectVariable.Value.height : 0.0f;
            this.curves = new AnimationCurve[4];
            this.curves[0] = this.curveX.curve;
            this.curves[1] = this.curveY.curve;
            this.curves[2] = this.curveW.curve;
            this.curves[3] = this.curveH.curve;
            this.calculations = new AnimateFsmAction.Calculation[4];
            this.calculations[0] = this.calculationX;
            this.calculations[1] = this.calculationY;
            this.calculations[2] = this.calculationW;
            this.calculations[3] = this.calculationH;
            this.Init();
            if ((double) Math.Abs(this.delay.Value) >= 0.0099999997764825821)
                return;
            this.UpdateVariableValue();
        }

        private void UpdateVariableValue()
        {
            if (this.rectVariable.IsNone)
                return;
            this.rectVariable.Value = new Rect(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
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
