using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
  [AddComponentMenu("Daikon Forge/Tween/Object Position")]
  public class TweenObjectPosition : TweenComponent<Vector3>
  {
    [SerializeField]
    protected bool useLocalPosition;
    private TweenEasingCallback easingFunc;

    public bool UseLocalPosition
    {
      get => this.useLocalPosition;
      set
      {
        this.useLocalPosition = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    protected override void configureTween()
    {
      if (this.tween == null)
      {
        this.easingFunc = TweenEasingFunctions.GetFunction(this.easingType);
        this.tween = (DaikonForge.Tween.Tween<Vector3>) this.transform.TweenPosition(this.useLocalPosition).SetEasing(new TweenEasingCallback(this.modifyEasing)).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
      }
      Vector3 vector3_1 = !this.useLocalPosition ? this.transform.position : this.transform.localPosition;
      Vector3 vector3_2 = this.startValue;
      if (this.startValueType == TweenStartValueType.SyncOnRun)
        vector3_2 = vector3_1;
      Vector3 vector3_3 = this.endValue;
      if (this.endValueType == TweenEndValueType.SyncOnRun)
        vector3_3 = vector3_1;
      else if (this.endValueType == TweenEndValueType.Relative)
        vector3_3 += vector3_2;
      this.tween.SetStartValue(vector3_2).SetEndValue(vector3_3).SetDelay(this.startDelay, this.assignStartValueBeforeDelay).SetDuration(this.duration).SetLoopType(this.LoopType).SetLoopCount(this.loopCount).SetPlayDirection(this.playDirection);
    }

    private float modifyEasing(float time)
    {
      if (this.animCurve != null)
        time = this.animCurve.Evaluate(time);
      return this.easingFunc(time);
    }
  }
}
