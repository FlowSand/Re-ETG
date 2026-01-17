// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.TweenTimeline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween;

public class TweenTimeline : TweenBase, IEnumerable<TweenBase>, IEnumerable
{
  private List<TweenTimeline.Entry> tweenList = new List<TweenTimeline.Entry>();
  private List<TweenTimeline.Entry> pending = new List<TweenTimeline.Entry>();
  private List<TweenTimeline.Entry> triggered = new List<TweenTimeline.Entry>();
  private static List<object> pool = new List<object>();

  public static TweenTimeline Obtain()
  {
    if (TweenTimeline.pool.Count <= 0)
      return new TweenTimeline();
    TweenTimeline tweenTimeline = (TweenTimeline) TweenTimeline.pool[TweenTimeline.pool.Count - 1];
    TweenTimeline.pool.RemoveAt(TweenTimeline.pool.Count - 1);
    return tweenTimeline;
  }

  public void Release()
  {
    this.Stop();
    this.Reset();
    TweenTimeline.pool.Add((object) this);
  }

  public TweenTimeline Add(float time, params TweenBase[] tweens)
  {
    for (int index = 0; index < tweens.Length; ++index)
    {
      TweenBase tween = tweens[index];
      this.Duration = Mathf.Max(this.Delay + this.Duration, time + tween.Delay + tween.Duration + this.Delay);
      this.tweenList.Add(new TweenTimeline.Entry()
      {
        Time = time,
        Tween = tween
      });
    }
    return this;
  }

  public override TweenBase Play()
  {
    this.pending.AddRange((IEnumerable<TweenTimeline.Entry>) this.tweenList);
    this.pending.Sort();
    this.triggered.Clear();
    if ((double) this.Delay > 0.0)
    {
      for (int index = 0; index < this.pending.Count; ++index)
        this.pending[index] = new TweenTimeline.Entry()
        {
          Time = this.pending[index].Time + this.Delay,
          Tween = this.pending[index].Tween
        };
    }
    this.State = TweenState.Playing;
    this.CurrentTime = 0.0f;
    this.startTime = this.getCurrentTime();
    this.registerWithTweenManager();
    this.raiseStarted();
    return (TweenBase) this;
  }

  public override TweenBase Stop()
  {
    if (this.State == TweenState.Stopped)
      return (TweenBase) this;
    for (int index = 0; index < this.tweenList.Count; ++index)
      this.tweenList[index].Tween.Stop();
    this.pending.Clear();
    this.triggered.Clear();
    return base.Stop();
  }

  public override TweenBase Pause()
  {
    if (this.State != TweenState.Playing && this.State != TweenState.Started)
      return (TweenBase) this;
    for (int index = 0; index < this.triggered.Count; ++index)
      this.triggered[index].Tween.Pause();
    return base.Pause();
  }

  public override TweenBase Resume()
  {
    if (this.State != TweenState.Paused)
      return (TweenBase) this;
    for (int index = 0; index < this.triggered.Count; ++index)
      this.triggered[index].Tween.Resume();
    return base.Resume();
  }

  public override TweenBase SetIsTimeScaleIndependent(bool isTimeScaleIndependent)
  {
    for (int index = 0; index < this.tweenList.Count; ++index)
      this.tweenList[index].Tween.SetIsTimeScaleIndependent(isTimeScaleIndependent);
    return base.SetIsTimeScaleIndependent(isTimeScaleIndependent);
  }

  public TweenTimeline SetLoopType(TweenLoopType loopType)
  {
    this.LoopType = loopType == TweenLoopType.None || loopType == TweenLoopType.Loop ? loopType : throw new ArgumentException("LoopType may only be one of the following values: TweenLoopType.None, TweenLoopType.Loop");
    return this;
  }

  public TweenTimeline SetLoopCount(int loopCount)
  {
    this.LoopCount = loopCount;
    return this;
  }

  internal override float CalculateTotalDuration()
  {
    float a = 0.0f;
    for (int index = 0; index < this.tweenList.Count; ++index)
    {
      TweenTimeline.Entry tween = this.tweenList[index];
      if (tween.Tween != null)
        a = Mathf.Max(a, tween.Time + tween.Tween.CalculateTotalDuration());
    }
    if (this.LoopCount > 0)
      a *= (float) this.LoopCount;
    else if (this.LoopType != TweenLoopType.None)
      a = float.PositiveInfinity;
    return this.Delay + a;
  }

  protected override void Reset()
  {
    this.tweenList.Clear();
    this.pending.Clear();
    this.triggered.Clear();
    base.Reset();
  }

  public override void Update()
  {
    if (this.State != TweenState.Started && this.State != TweenState.Playing)
      return;
    float num = this.getCurrentTime() - this.startTime;
    while (this.pending.Count > 0)
    {
      TweenTimeline.Entry entry = this.pending[0];
      if ((double) entry.Time <= (double) num)
      {
        this.pending.RemoveAt(0);
        this.triggered.Add(entry);
        entry.Tween.Play();
      }
      else
        break;
    }
    if (!this.allTweensComplete())
      return;
    if (this.LoopType == TweenLoopType.Loop)
    {
      if (--this.LoopCount != 0)
      {
        if (this.TweenLoopCompleted != null)
          this.TweenLoopCompleted((TweenBase) this);
        this.Rewind();
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

  private bool allTweensComplete()
  {
    if (this.pending.Count > 0)
      return false;
    for (int index = 0; index < this.triggered.Count; ++index)
    {
      if (this.triggered[index].Tween.State != TweenState.Stopped)
        return false;
    }
    return true;
  }

  public IEnumerator<TweenBase> GetEnumerator() => this.enumerateTweens();

  [DebuggerHidden]
  private IEnumerator<TweenBase> enumerateTweens()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator<TweenBase>) new TweenTimeline.\u003CenumerateTweens\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.enumerateTweens();

  private struct Entry : IComparable<TweenTimeline.Entry>
  {
    public float Time;
    public TweenBase Tween;

    public int CompareTo(TweenTimeline.Entry other) => this.Time.CompareTo(other.Time);
  }
}
