// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Components.TweenObjectColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components;

[AddComponentMenu("Daikon Forge/Tween/Object Color")]
public class TweenObjectColor : TweenComponent<Color>
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
        this.tween = (DaikonForge.Tween.Tween<Color>) null;
        return;
      }
    }
    if (this.tween == null)
    {
      this.easingFunc = TweenEasingFunctions.GetFunction(this.easingType);
      this.tween = (DaikonForge.Tween.Tween<Color>) this.Target.TweenColor().SetEasing(new TweenEasingCallback(this.modifyEasing)).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
    }
    Color currentValue = this.tween.CurrentValue;
    Color color1 = this.startValue;
    if (this.startValueType == TweenStartValueType.SyncOnRun)
      color1 = currentValue;
    Color color2 = this.endValue;
    if (this.endValueType == TweenEndValueType.SyncOnRun)
      color2 = currentValue;
    else if (this.endValueType == TweenEndValueType.Relative)
      color2 += color1;
    this.tween.SetStartValue(color1).SetEndValue(color2).SetDelay(this.startDelay, this.assignStartValueBeforeDelay).SetDuration(this.duration).SetLoopType(this.LoopType).SetLoopCount(this.loopCount).SetPlayDirection(this.playDirection);
  }

  private float modifyEasing(float time)
  {
    if (this.animCurve != null)
      time = this.animCurve.Evaluate(time);
    return this.easingFunc(time);
  }
}
