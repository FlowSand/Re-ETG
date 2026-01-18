using DaikonForge.Editor;
using System;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
  [InspectorGroupOrder(new string[] {"General", "Animation", "Looping", "Values"})]
  public abstract class TweenComponent<T> : TweenComponentBase
  {
    [Inspector("Animation", Order = 4, Tooltip = "How long the Tween should take to complete the animation")]
    [SerializeField]
    protected float duration = 1f;
    [Inspector("Animation", Order = 2, Tooltip = "The type of easing, if any, to apply to the animation")]
    [SerializeField]
    protected EasingType easingType;
    [Inspector("Animation", Order = 3, Label = "Curve", Tooltip = "An animation curve can be used to modify the animation timeline")]
    [SerializeField]
    protected AnimationCurve animCurve = new AnimationCurve(new Keyframe[2]
    {
      new Keyframe(0.0f, 0.0f, 0.0f, 1f),
      new Keyframe(1f, 1f, 1f, 0.0f)
    });
    [SerializeField]
    [Inspector("Animation", Order = 5, Label = "Direction")]
    protected TweenDirection playDirection;
    [Inspector("Values", Order = 0)]
    [SerializeField]
    protected TweenStartValueType startValueType;
    [Inspector("Values", Order = 1)]
    [SerializeField]
    protected T startValue;
    [SerializeField]
    [Inspector("Values", Order = 2)]
    protected TweenEndValueType endValueType;
    [SerializeField]
    [Inspector("Values", Order = 3)]
    protected T endValue;
    protected DaikonForge.Tween.Tween<T> tween;

    public override TweenBase BaseTween
    {
      get
      {
        this.configureTween();
        return (TweenBase) this.tween;
      }
    }

    public AnimationCurve AnimationCurve
    {
      get => this.animCurve;
      set => this.animCurve = value;
    }

    public float Duration
    {
      get => this.duration;
      set => this.duration = Mathf.Max(0.0f, value);
    }

    public T StartValue
    {
      get => this.startValue;
      set
      {
        this.startValue = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public TweenStartValueType StartValueType
    {
      get => this.startValueType;
      set
      {
        this.startValueType = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public T EndValue
    {
      get => this.endValue;
      set
      {
        this.endValue = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public TweenEndValueType EndValueType
    {
      get => this.endValueType;
      set
      {
        this.endValueType = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public TweenDirection PlayDirection
    {
      get => this.playDirection;
      set
      {
        this.playDirection = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public override TweenState State => this.tween == null ? TweenState.Stopped : this.tween.State;

    public virtual void OnApplicationQuit() => this.cleanup();

    public override void OnDisable()
    {
      base.OnDisable();
      this.cleanup();
    }

    public override void Play()
    {
      if (this.State != TweenState.Stopped)
        this.Stop();
      this.configureTween();
      this.validateTweenConfiguration();
      this.tween.Play();
    }

    public override void Stop()
    {
      if (!this.IsPlaying)
        return;
      this.validateTweenConfiguration();
      this.tween.Stop();
    }

    public override void Pause()
    {
      if (!this.IsPlaying)
        return;
      this.validateTweenConfiguration();
      this.tween.Pause();
    }

    public override void Resume()
    {
      if (!this.IsPaused)
        return;
      this.validateTweenConfiguration();
      this.tween.Resume();
    }

    public override void Rewind()
    {
      this.validateTweenConfiguration();
      this.tween.Rewind();
    }

    public override void FastForward()
    {
      this.validateTweenConfiguration();
      this.tween.FastForward();
    }

    protected virtual void cleanup()
    {
      if (this.tween == null)
        return;
      this.tween.Stop();
      this.tween.Release();
      this.tween = (DaikonForge.Tween.Tween<T>) null;
    }

    protected virtual void validateTweenConfiguration()
    {
      this.loopCount = Mathf.Max(0, this.loopCount);
      if (this.tween == null)
        throw new InvalidOperationException("The tween has not been properly configured");
    }

    protected abstract void configureTween();
  }
}
