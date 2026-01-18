using System;

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
    [AddComponentMenu("Daikon Forge/Tween/Object Alpha")]
    public class TweenObjectAlpha : TweenComponent<float>
    {
        [SerializeField]
        protected Component target;
        private TweenEasingCallback easingFunc;

        public Component Target
        {
            get => this.target;
            set
            {
                this.target = value;
                this.Stop();
            }
        }

        protected override void validateTweenConfiguration()
        {
            if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
                throw new InvalidOperationException("The Target cannot be NULL");
            if ((double) this.startValue < 0.0 || (double) this.startValue > 1.0)
                throw new InvalidOperationException("The Start Value must be between 0 and 1");
            if ((double) this.endValue < 0.0 || (double) this.endValue > 1.0)
                throw new InvalidOperationException("The End Value must be between 0 and 1");
            base.validateTweenConfiguration();
        }

        protected override void configureTween()
        {
            if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
            {
                this.target = (Component) this.gameObject.GetComponent<Renderer>();
                if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
                {
                    if (this.tween == null)
                        return;
                    this.tween.Stop();
                    this.tween.Release();
                    this.tween = (DaikonForge.Tween.Tween<float>) null;
                    return;
                }
            }
            if (this.tween == null)
            {
                this.easingFunc = TweenEasingFunctions.GetFunction(this.easingType);
                this.tween = (DaikonForge.Tween.Tween<float>) this.Target.TweenAlpha().SetEasing(new TweenEasingCallback(this.modifyEasing)).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
            }
            float currentValue = this.tween.CurrentValue;
            float num1 = this.startValue;
            if (this.startValueType == TweenStartValueType.SyncOnRun)
                num1 = currentValue;
            float num2 = this.endValue;
            if (this.endValueType == TweenEndValueType.SyncOnRun)
                num2 = currentValue;
            else if (this.endValueType == TweenEndValueType.Relative)
                num2 += num1;
            this.tween.SetStartValue(num1).SetEndValue(num2).SetDelay(this.startDelay, this.assignStartValueBeforeDelay).SetDuration(this.duration).SetLoopType(this.LoopType).SetLoopCount(this.loopCount).SetPlayDirection(this.playDirection);
        }

        private float modifyEasing(float time)
        {
            if (this.animCurve != null)
                time = this.animCurve.Evaluate(time);
            return this.easingFunc(time);
        }
    }
}
