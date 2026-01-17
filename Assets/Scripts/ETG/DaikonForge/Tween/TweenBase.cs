// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.TweenBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween
{
  public abstract class TweenBase : ITweenUpdatable
  {
    public string Name;
    public float CurrentTime;
    public float Duration;
    public float Delay;
    public TweenLoopType LoopType;
    public int LoopCount;
    public TweenEasingCallback Easing;
    public bool AutoCleanup;
    public bool IsTimeScaleIndependent;
    public TweenCallback TweenStarted;
    public TweenCallback TweenStopped;
    public TweenCallback TweenPaused;
    public TweenCallback TweenResumed;
    public TweenCallback TweenCompleted;
    public TweenCallback TweenLoopCompleted;
    protected float startTime;
    protected bool registered;

    public float ElapsedTime => this.getCurrentTime() - this.startTime;

    public TweenState State { get; protected set; }

    public virtual TweenBase Play()
    {
      this.State = TweenState.Started;
      this.CurrentTime = 0.0f;
      this.startTime = this.getCurrentTime();
      this.registerWithTweenManager();
      this.raiseStarted();
      return this;
    }

    public virtual TweenBase Pause()
    {
      if (this.State != TweenState.Playing && this.State != TweenState.Started)
        return this;
      this.State = TweenState.Paused;
      this.raisePaused();
      return this;
    }

    public virtual TweenBase Resume()
    {
      if (this.State != TweenState.Paused)
        return this;
      this.State = TweenState.Playing;
      this.raiseResumed();
      return this;
    }

    public virtual TweenBase Stop()
    {
      if (this.State == TweenState.Stopped)
        return this;
      this.unregisterWithTweenManager();
      this.State = TweenState.Stopped;
      this.raiseStopped();
      return this;
    }

    public virtual TweenBase Rewind()
    {
      this.CurrentTime = 0.0f;
      this.startTime = this.getCurrentTime();
      return this;
    }

    public virtual TweenBase FastForward()
    {
      this.CurrentTime = 1f;
      return this;
    }

    [DebuggerHidden]
    public virtual IEnumerator WaitForCompletion()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TweenBase__WaitForCompletionc__Iterator0()
      {
        _this = this
      };
    }

    public virtual TweenBase Chain(TweenBase tween) => this.Chain(tween, (System.Action) null);

    public virtual TweenBase Chain(TweenBase tween, System.Action initFunction)
    {
      if (tween == null)
        throw new ArgumentNullException(nameof (tween));
      TweenCallback completedCallback = this.TweenCompleted;
      this.TweenCompleted = (TweenCallback) (sender =>
      {
        if (completedCallback != null)
          completedCallback(sender);
        if (initFunction != null)
          initFunction();
        tween.Play();
      });
      return tween;
    }

    internal virtual float CalculateTotalDuration()
    {
      float totalDuration = this.Delay + this.Duration;
      if (this.LoopCount > 0)
        totalDuration *= (float) this.LoopCount;
      else if (this.LoopType != TweenLoopType.None)
        totalDuration = float.PositiveInfinity;
      return totalDuration;
    }

    public virtual TweenBase SetIsTimeScaleIndependent(bool isTimeScaleIndependent)
    {
      this.IsTimeScaleIndependent = isTimeScaleIndependent;
      return this;
    }

    public abstract void Update();

    protected virtual void Reset()
    {
      // ISSUE: reference to a compiler-generated field
      if (TweenBase._f__mg_cache0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TweenBase._f__mg_cache0 = new TweenEasingCallback(TweenEasingFunctions.Linear);
      }
      // ISSUE: reference to a compiler-generated field
      this.Easing = TweenBase._f__mg_cache0;
      this.LoopType = TweenLoopType.None;
      this.CurrentTime = 0.0f;
      this.Delay = 0.0f;
      this.AutoCleanup = false;
      this.IsTimeScaleIndependent = false;
      this.startTime = 0.0f;
      this.TweenLoopCompleted = (TweenCallback) null;
      this.TweenCompleted = (TweenCallback) null;
      this.TweenPaused = (TweenCallback) null;
      this.TweenResumed = (TweenCallback) null;
      this.TweenStarted = (TweenCallback) null;
      this.TweenStopped = (TweenCallback) null;
    }

    protected void registerWithTweenManager()
    {
      if (this.registered)
        return;
      TweenManager.Instance.RegisterTween((ITweenUpdatable) this);
      this.registered = true;
    }

    protected void unregisterWithTweenManager()
    {
      if (!this.registered)
        return;
      TweenManager.Instance.UnregisterTween((ITweenUpdatable) this);
      this.registered = false;
    }

    protected float getTimeElapsed()
    {
      return this.State == TweenState.Playing || this.State == TweenState.Started ? Mathf.Min(this.getCurrentTime() - this.startTime, this.Duration) : 0.0f;
    }

    protected float getCurrentTime()
    {
      return this.IsTimeScaleIndependent ? TweenManager.Instance.realTimeSinceStartup : UnityEngine.Time.time;
    }

    protected float getDeltaTime()
    {
      return this.IsTimeScaleIndependent ? TweenManager.realDeltaTime : BraveTime.DeltaTime;
    }

    public TweenBase OnLoopCompleted(TweenCallback function)
    {
      this.TweenLoopCompleted = function;
      return this;
    }

    public TweenBase OnCompleted(TweenCallback function)
    {
      this.TweenCompleted = function;
      return this;
    }

    public TweenBase OnPaused(TweenCallback function)
    {
      this.TweenPaused = function;
      return this;
    }

    public TweenBase OnResumed(TweenCallback function)
    {
      this.TweenResumed = function;
      return this;
    }

    public TweenBase OnStarted(TweenCallback function)
    {
      this.TweenStarted = function;
      return this;
    }

    public TweenBase OnStopped(TweenCallback function)
    {
      this.TweenStopped = function;
      return this;
    }

    public virtual TweenBase Wait(float seconds) => this.Chain((TweenBase) new TweenWait(seconds));

    protected virtual void raisePaused()
    {
      if (this.TweenPaused == null)
        return;
      this.TweenPaused(this);
    }

    protected virtual void raiseResumed()
    {
      if (this.TweenResumed == null)
        return;
      this.TweenResumed(this);
    }

    protected virtual void raiseStarted()
    {
      if (this.TweenStarted == null)
        return;
      this.TweenStarted(this);
    }

    protected virtual void raiseStopped()
    {
      if (this.TweenStopped == null)
        return;
      this.TweenStopped(this);
    }

    protected virtual void raiseCompleted()
    {
      if (this.TweenCompleted == null)
        return;
      this.TweenCompleted(this);
    }

    public override string ToString()
    {
      return !string.IsNullOrEmpty(this.Name) ? this.Name : base.ToString();
    }
  }
}
