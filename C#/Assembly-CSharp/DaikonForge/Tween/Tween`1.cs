// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Tween`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Tween.Interpolation;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween;

public class Tween<T> : TweenBase, IPoolableObject
{
  public T StartValue;
  public T EndValue;
  public DaikonForge.Tween.Interpolation.Interpolator<T> Interpolator;
  public TweenAssignmentCallback<T> Execute;
  public TweenDirection PlayDirection;
  public TweenSyncCallback<T> TweenSyncStartValue;
  public TweenSyncCallback<T> TweenSyncEndValue;
  protected bool assignStartValueBeforeDelay = true;
  private static List<object> pool = new List<object>();

  public T CurrentValue { get; protected set; }

  public bool EndIsOffset { get; protected set; }

  public static DaikonForge.Tween.Tween<T> Obtain()
  {
    if (DaikonForge.Tween.Tween<T>.pool.Count <= 0)
      return new DaikonForge.Tween.Tween<T>();
    DaikonForge.Tween.Tween<T> tween = (DaikonForge.Tween.Tween<T>) DaikonForge.Tween.Tween<T>.pool[DaikonForge.Tween.Tween<T>.pool.Count - 1];
    DaikonForge.Tween.Tween<T>.pool.RemoveAt(DaikonForge.Tween.Tween<T>.pool.Count - 1);
    return tween;
  }

  public void Release()
  {
    this.Stop();
    this.Reset();
    DaikonForge.Tween.Tween<T>.pool.Add((object) this);
  }

  public DaikonForge.Tween.Tween<T> SetEndRelative(bool relative)
  {
    this.EndIsOffset = relative;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetAutoCleanup(bool autoCleanup)
  {
    this.AutoCleanup = autoCleanup;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetPlayDirection(TweenDirection direction)
  {
    this.PlayDirection = direction;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetEasing(TweenEasingCallback easingFunction)
  {
    this.Easing = easingFunction;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetDuration(float duration)
  {
    this.Duration = duration;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetEndValue(T value)
  {
    this.EndValue = value;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetStartValue(T value)
  {
    T obj = value;
    this.CurrentValue = obj;
    this.StartValue = obj;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetDelay(float seconds)
  {
    return this.SetDelay(seconds, this.assignStartValueBeforeDelay);
  }

  public DaikonForge.Tween.Tween<T> SetDelay(float seconds, bool assignStartValueBeforeDelay)
  {
    this.Delay = seconds;
    this.assignStartValueBeforeDelay = assignStartValueBeforeDelay;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetLoopType(TweenLoopType loopType)
  {
    this.LoopType = loopType;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetLoopCount(int loopCount)
  {
    this.LoopCount = loopCount;
    return this;
  }

  public DaikonForge.Tween.Tween<T> SetTimeScaleIndependent(bool timeScaleIndependent)
  {
    this.IsTimeScaleIndependent = timeScaleIndependent;
    return this;
  }

  public override TweenBase Play()
  {
    if (this.TweenSyncStartValue != null)
      this.StartValue = this.TweenSyncStartValue();
    if (this.TweenSyncEndValue != null)
      this.EndValue = this.TweenSyncEndValue();
    base.Play();
    this.ensureInterpolator();
    if (this.assignStartValueBeforeDelay)
      this.evaluateAtTime(this.CurrentTime);
    return (TweenBase) this;
  }

  public override TweenBase Rewind()
  {
    base.Rewind();
    this.ensureInterpolator();
    this.evaluateAtTime(this.CurrentTime);
    return (TweenBase) this;
  }

  public override TweenBase FastForward()
  {
    base.FastForward();
    this.ensureInterpolator();
    this.evaluateAtTime(this.CurrentTime);
    return (TweenBase) this;
  }

  public virtual TweenBase Seek(float time)
  {
    this.CurrentTime = Mathf.Clamp01(time);
    this.evaluateAtTime(this.CurrentTime);
    return (TweenBase) this;
  }

  public virtual TweenBase ReversePlayDirection()
  {
    this.PlayDirection = this.PlayDirection != TweenDirection.Forward ? TweenDirection.Forward : TweenDirection.Reverse;
    return (TweenBase) this;
  }

  public DaikonForge.Tween.Tween<T> SetInterpolator(DaikonForge.Tween.Interpolation.Interpolator<T> interpolator)
  {
    this.Interpolator = interpolator;
    return this;
  }

  public DaikonForge.Tween.Tween<T> OnExecute(TweenAssignmentCallback<T> function)
  {
    this.Execute = function;
    return this;
  }

  public DaikonForge.Tween.Tween<T> OnSyncStartValue(TweenSyncCallback<T> function)
  {
    this.TweenSyncStartValue = function;
    return this;
  }

  public DaikonForge.Tween.Tween<T> OnSyncEndValue(TweenSyncCallback<T> function)
  {
    this.TweenSyncEndValue = function;
    return this;
  }

  public override void Update()
  {
    if (this.State == TweenState.Started)
    {
      float currentTime = this.getCurrentTime();
      if ((double) currentTime < (double) this.startTime + (double) this.Delay)
        return;
      this.startTime = currentTime;
      this.CurrentTime = 0.0f;
      this.State = TweenState.Playing;
    }
    else if (this.State != TweenState.Playing)
      return;
    this.CurrentTime = Mathf.MoveTowards(this.CurrentTime, 1f, this.getDeltaTime() / this.Duration);
    float time = this.CurrentTime;
    if (this.Easing != null)
      time = this.Easing(this.CurrentTime);
    this.evaluateAtTime(time);
    if ((double) this.CurrentTime < 1.0)
      return;
    if (this.LoopType == TweenLoopType.Loop)
    {
      if (--this.LoopCount != 0)
      {
        if (this.TweenLoopCompleted != null)
          this.TweenLoopCompleted((TweenBase) this);
        if (this.EndIsOffset)
          this.StartValue = this.CurrentValue;
        this.Rewind();
        this.Play();
        return;
      }
    }
    if (this.LoopType == TweenLoopType.Pingpong)
    {
      if (--this.LoopCount != 0)
      {
        if (this.TweenLoopCompleted != null)
          this.TweenLoopCompleted((TweenBase) this);
        this.ReversePlayDirection();
        this.Play();
        return;
      }
    }
    this.Stop();
    this.raiseCompleted();
    if (!this.AutoCleanup)
      return;
    this.Release();
  }

  private void ensureInterpolator()
  {
    if (this.Interpolator != null)
      return;
    this.Interpolator = Interpolators.Get<T>();
  }

  protected override void Reset()
  {
    base.Reset();
    this.StartValue = default (T);
    this.EndValue = default (T);
    this.CurrentValue = default (T);
    this.Duration = 1f;
    this.EndIsOffset = false;
    this.PlayDirection = TweenDirection.Forward;
    this.LoopCount = -1;
    this.assignStartValueBeforeDelay = true;
    this.Interpolator = (DaikonForge.Tween.Interpolation.Interpolator<T>) null;
    this.Execute = (TweenAssignmentCallback<T>) null;
  }

  private void evaluateAtTime(float time)
  {
    if (this.Interpolator == null)
      throw new InvalidOperationException($"No interpolator for type '{typeof (T).Name}' has been assigned");
    T obj = this.EndValue;
    if (this.EndIsOffset)
      obj = this.Interpolator.Add(this.StartValue, this.EndValue);
    this.CurrentValue = this.PlayDirection != TweenDirection.Reverse ? this.Interpolator.Interpolate(this.StartValue, obj, time) : this.Interpolator.Interpolate(obj, this.StartValue, time);
    if (this.Execute == null)
      return;
    this.Execute(this.CurrentValue);
  }
}
