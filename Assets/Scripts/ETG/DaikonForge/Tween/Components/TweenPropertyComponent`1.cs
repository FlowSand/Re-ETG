using DaikonForge.Editor;
using DaikonForge.Tween.Interpolation;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
  [InspectorGroupOrder(new string[] {"General", "Animation", "Looping", "Property", "Values"})]
  public class TweenPropertyComponent<T> : TweenComponent<T>, ITweenPropertyBase
  {
    [Inspector("Property", Label = "Target", Order = 0)]
    [SerializeField]
    protected GameObject target;
    [SerializeField]
    protected string componentType;
    [Inspector("Property", Label = "Field", Order = 1)]
    [SerializeField]
    protected string memberName;
    private Component component;
    private TweenEasingCallback easingFunc;

    public GameObject Target
    {
      get => this.target;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.target))
          return;
        this.target = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public string ComponentType
    {
      get => this.componentType;
      set
      {
        if (!(value != this.componentType))
          return;
        this.componentType = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public string MemberName
    {
      get => this.memberName;
      set
      {
        if (!(value != this.memberName))
          return;
        this.memberName = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public override void OnEnable()
    {
      if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
        this.target = this.gameObject;
      base.OnEnable();
    }

    protected override void validateTweenConfiguration()
    {
      base.validateTweenConfiguration();
      if ((UnityEngine.Object) this.target == (UnityEngine.Object) null || string.IsNullOrEmpty(this.componentType) || string.IsNullOrEmpty(this.memberName))
        return;
      if ((UnityEngine.Object) this.target.GetComponent(this.componentType) == (UnityEngine.Object) null)
        throw new NullReferenceException($"Object {this.target.name} does not contain a {this.componentType} component");
      if (Interpolators.Get<T>() == null)
        throw new KeyNotFoundException($"There is no default interpolator defined for type '{typeof (T).Name}'");
    }

    protected override void configureTween()
    {
      if (this.tween == null)
      {
        if ((UnityEngine.Object) this.target == (UnityEngine.Object) null || string.IsNullOrEmpty(this.componentType) || string.IsNullOrEmpty(this.memberName))
          return;
        this.component = this.target.GetComponent(this.componentType);
        if ((UnityEngine.Object) this.component == (UnityEngine.Object) null)
          return;
        this.easingFunc = TweenEasingFunctions.GetFunction(this.easingType);
        this.tween = (DaikonForge.Tween.Tween<T>) this.component.TweenProperty<T>(this.memberName).SetEasing(new TweenEasingCallback(this.modifyEasing)).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
      }
      T currentValue = TweenNamedProperty<T>.GetCurrentValue((object) this.component, this.memberName);
      Interpolator<T> interpolator = Interpolators.Get<T>();
      T rhs = this.startValue;
      if (this.startValueType == TweenStartValueType.SyncOnRun)
        rhs = currentValue;
      T lhs = this.endValue;
      if (this.endValueType == TweenEndValueType.SyncOnRun)
        lhs = currentValue;
      else if (this.endValueType == TweenEndValueType.Relative)
        lhs = interpolator.Add(lhs, rhs);
      this.tween.SetStartValue(rhs).SetEndValue(lhs).SetDelay(this.startDelay, this.assignStartValueBeforeDelay).SetDuration(this.duration).SetLoopType(this.LoopType).SetLoopCount(this.loopCount).SetPlayDirection(this.playDirection);
    }

    private float modifyEasing(float time)
    {
      if (this.animCurve != null)
        time = this.animCurve.Evaluate(time);
      return this.easingFunc(time);
    }
  }
}
