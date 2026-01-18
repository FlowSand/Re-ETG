using System;

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Animates the value of a Vector3 Variable using an Animation Curve.")]
    [ActionCategory(ActionCategory.AnimateVariables)]
    public class AnimateVector3 : AnimateFsmAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmVector3 vectorVariable;
        [RequiredField]
        public FsmAnimationCurve curveX;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.x.")]
        public AnimateFsmAction.Calculation calculationX;
        [RequiredField]
        public FsmAnimationCurve curveY;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.y.")]
        public AnimateFsmAction.Calculation calculationY;
        [RequiredField]
        public FsmAnimationCurve curveZ;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.z.")]
        public AnimateFsmAction.Calculation calculationZ;
        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.vectorVariable = fsmVector3;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            this.resultFloats = new float[3];
            this.fromFloats = new float[3];
            this.fromFloats[0] = !this.vectorVariable.IsNone ? this.vectorVariable.Value.x : 0.0f;
            this.fromFloats[1] = !this.vectorVariable.IsNone ? this.vectorVariable.Value.y : 0.0f;
            this.fromFloats[2] = !this.vectorVariable.IsNone ? this.vectorVariable.Value.z : 0.0f;
            this.curves = new AnimationCurve[3];
            this.curves[0] = this.curveX.curve;
            this.curves[1] = this.curveY.curve;
            this.curves[2] = this.curveZ.curve;
            this.calculations = new AnimateFsmAction.Calculation[3];
            this.calculations[0] = this.calculationX;
            this.calculations[1] = this.calculationY;
            this.calculations[2] = this.calculationZ;
            this.Init();
            if ((double) Math.Abs(this.delay.Value) >= 0.0099999997764825821)
                return;
            this.UpdateVariableValue();
        }

        private void UpdateVariableValue()
        {
            if (this.vectorVariable.IsNone)
                return;
            this.vectorVariable.Value = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
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
