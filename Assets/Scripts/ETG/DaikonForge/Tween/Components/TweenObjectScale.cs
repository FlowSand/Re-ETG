using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
  [AddComponentMenu("Daikon Forge/Tween/Object Scale")]
  public class TweenObjectScale : TweenComponent<Vector3>
  {
    private TweenEasingCallback easingFunc;

    protected override void configureTween()
    {
      if (this.tween == null)
      {
        this.easingFunc = TweenEasingFunctions.GetFunction(this.easingType);
        this.tween = (DaikonForge.Tween.Tween<Vector3>) this.transform.TweenScale().SetEasing(new TweenEasingCallback(this.modifyEasing)).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
      }
      Vector3 localScale = this.transform.localScale;
      Vector3 vector3_1 = this.startValue;
      if (this.startValueType == TweenStartValueType.SyncOnRun)
        vector3_1 = localScale;
      Vector3 vector3_2 = this.endValue;
      if (this.endValueType == TweenEndValueType.SyncOnRun)
        vector3_2 = localScale;
      else if (this.endValueType == TweenEndValueType.Relative)
        vector3_2 += vector3_1;
      this.tween.SetStartValue(vector3_1).SetEndValue(vector3_2).SetDelay(this.startDelay, this.assignStartValueBeforeDelay).SetDuration(this.duration).SetLoopType(this.LoopType).SetLoopCount(this.loopCount).SetPlayDirection(this.playDirection);
    }

    private float modifyEasing(float time)
    {
      if (this.animCurve != null)
        time = this.animCurve.Evaluate(time);
      return this.easingFunc(time);
    }
  }
}
