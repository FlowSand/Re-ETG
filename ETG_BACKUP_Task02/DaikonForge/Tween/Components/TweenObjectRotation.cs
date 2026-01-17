// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Components.TweenObjectRotation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components;

[AddComponentMenu("Daikon Forge/Tween/Object Rotation")]
public class TweenObjectRotation : TweenComponent<Vector3>
{
  [SerializeField]
  protected bool useLocalRotation;
  [SerializeField]
  protected bool useShortestPath = true;
  private TweenEasingCallback easingFunc;

  public bool UseLocalRotation
  {
    get => this.useLocalRotation;
    set
    {
      this.useLocalRotation = value;
      if (this.State == TweenState.Stopped)
        return;
      this.Stop();
      this.Play();
    }
  }

  public bool UseShortestPath
  {
    get => this.useShortestPath;
    set
    {
      this.useShortestPath = value;
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
      this.tween = (DaikonForge.Tween.Tween<Vector3>) this.transform.TweenRotation(this.useShortestPath, this.useLocalRotation).SetEasing(new TweenEasingCallback(this.modifyEasing)).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
    }
    Vector3 vector3_1 = !this.useLocalRotation ? this.transform.eulerAngles : this.transform.localEulerAngles;
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
