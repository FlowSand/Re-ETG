using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [HutongGames.PlayMaker.Tooltip("Animates the value of a Float Variable using an Animation Curve.")]
    public class AnimateFloat : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The animation curve to use.")]
        [RequiredField]
        public FsmAnimationCurve animCurve;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The float variable to set.")]
        public FsmFloat floatVariable;
        [HutongGames.PlayMaker.Tooltip("Optionally send an Event when the animation finishes.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
        public bool realTime;
        private float startTime;
        private float currentTime;
        private float endTime;
        private bool looping;

        public override void Reset()
        {
            this.animCurve = (FsmAnimationCurve) null;
            this.floatVariable = (FsmFloat) null;
            this.finishEvent = (FsmEvent) null;
            this.realTime = false;
        }

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0.0f;
            if (this.animCurve != null && this.animCurve.curve != null && this.animCurve.curve.keys.Length > 0)
            {
                this.endTime = this.animCurve.curve.keys[this.animCurve.curve.length - 1].time;
                this.looping = ActionHelpers.IsLoopingWrapMode(this.animCurve.curve.postWrapMode);
                this.floatVariable.Value = this.animCurve.curve.Evaluate(0.0f);
            }
            else
                this.Finish();
        }

        public override void OnUpdate()
        {
            if (this.realTime)
                this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
            else
                this.currentTime += Time.deltaTime;
            if (this.animCurve != null && this.animCurve.curve != null && this.floatVariable != null)
                this.floatVariable.Value = this.animCurve.curve.Evaluate(this.currentTime);
            if ((double) this.currentTime < (double) this.endTime)
                return;
            if (!this.looping)
                this.Finish();
            if (this.finishEvent == null)
                return;
            this.Fsm.Event(this.finishEvent);
        }
    }
}
