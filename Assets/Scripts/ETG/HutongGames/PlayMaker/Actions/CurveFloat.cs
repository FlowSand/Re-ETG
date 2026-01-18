using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [HutongGames.PlayMaker.Tooltip("Animates the value of a Float Variable FROM-TO with assistance of Deformation Curve.")]
    public class CurveFloat : CurveFsmAction
    {
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat floatVariable;
        [RequiredField]
        public FsmFloat fromValue;
        [RequiredField]
        public FsmFloat toValue;
        [RequiredField]
        public FsmAnimationCurve animCurve;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue and toValue.")]
        public CurveFsmAction.Calculation calculation;
        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.floatVariable = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.toValue = fsmFloat2;
            FsmFloat fsmFloat3 = new FsmFloat();
            fsmFloat3.UseVariable = true;
            this.fromValue = fsmFloat3;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            this.resultFloats = new float[1];
            this.fromFloats = new float[1];
            this.fromFloats[0] = !this.fromValue.IsNone ? this.fromValue.Value : 0.0f;
            this.toFloats = new float[1];
            this.toFloats[0] = !this.toValue.IsNone ? this.toValue.Value : 0.0f;
            this.calculations = new CurveFsmAction.Calculation[1];
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
