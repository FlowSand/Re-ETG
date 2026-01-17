// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.TweenGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween
{
  public class TweenGroup : TweenBase, IPoolableObject, IEnumerable<TweenBase>, IEnumerable
  {
    private static List<TweenGroup> pool = new List<TweenGroup>();
    public TweenGroupMode Mode;
    protected List<TweenBase> tweenList = new List<TweenBase>();
    protected TweenBase currentTween;
    protected int currentIndex;
    protected bool autoCleanup;

    public static TweenGroup Obtain()
    {
      if (TweenGroup.pool.Count <= 0)
        return new TweenGroup();
      TweenGroup tweenGroup = TweenGroup.pool[TweenGroup.pool.Count - 1];
      TweenGroup.pool.RemoveAt(TweenGroup.pool.Count - 1);
      return tweenGroup;
    }

    public void Release()
    {
      this.Stop();
      if (TweenGroup.pool.Contains(this))
        return;
      this.Reset();
      TweenGroup.pool.Add(this);
    }

    public TweenGroup SetAutoCleanup(bool autoCleanup)
    {
      this.AutoCleanup = true;
      return this;
    }

    public override TweenBase SetIsTimeScaleIndependent(bool isTimeScaleIndependent)
    {
      for (int index = 0; index < this.tweenList.Count; ++index)
        this.tweenList[index].SetIsTimeScaleIndependent(isTimeScaleIndependent);
      return base.SetIsTimeScaleIndependent(isTimeScaleIndependent);
    }

    public TweenGroup SetMode(TweenGroupMode mode)
    {
      this.Mode = mode;
      return this;
    }

    public TweenGroup SetDelay(float seconds)
    {
      this.Delay = seconds;
      return this;
    }

    public TweenGroup SetLoopType(TweenLoopType loopType)
    {
      this.LoopType = loopType == TweenLoopType.None || loopType == TweenLoopType.Loop ? loopType : throw new ArgumentException("LoopType may only be one of the following values: TweenLoopType.None, TweenLoopType.Loop");
      return this;
    }

    public TweenGroup SetLoopCount(int loopCount)
    {
      this.LoopCount = loopCount;
      return this;
    }

    public TweenGroup AppendTween(params TweenBase[] tweens)
    {
      if (tweens == null || tweens.Length == 0)
        throw new ArgumentException("You must provide at least one Tween");
      this.tweenList.AddRange((IEnumerable<TweenBase>) tweens);
      return this;
    }

    public TweenGroup AppendDelay(float seconds)
    {
      this.tweenList.Add((TweenBase) TweenWait.Obtain(seconds));
      return this;
    }

    public TweenGroup ClearTweens()
    {
      this.tweenList.Clear();
      return this;
    }

    public override TweenBase Play()
    {
      if (this.LoopType != TweenLoopType.None && this.LoopType != TweenLoopType.Loop)
        throw new ArgumentException("LoopType may only be one of the following values: TweenLoopType.None, TweenLoopType.Loop");
      this.currentTween = (TweenBase) null;
      this.currentIndex = -1;
      base.Play();
      return (TweenBase) this;
    }

    public override TweenBase Stop()
    {
      if (this.State != TweenState.Stopped)
      {
        for (int index = 0; index < this.tweenList.Count; ++index)
          this.tweenList[index].Stop();
        this.currentTween = (TweenBase) null;
        this.currentIndex = -1;
      }
      return base.Stop();
    }

    public override TweenBase Pause()
    {
      if (this.State == TweenState.Playing || this.State == TweenState.Started)
      {
        if (this.Mode == TweenGroupMode.Concurrent)
          this.pauseAllTweens();
        else if (this.currentTween != null)
          this.currentTween.Pause();
      }
      return base.Pause();
    }

    public override TweenBase Resume()
    {
      if (this.State == TweenState.Paused)
      {
        if (this.Mode == TweenGroupMode.Concurrent)
          this.resumeAllTweens();
        else if (this.currentTween != null)
          this.currentTween.Resume();
      }
      base.Resume();
      return (TweenBase) this;
    }

    public override TweenBase Rewind()
    {
      for (int index = 0; index < this.tweenList.Count; ++index)
        this.tweenList[index].Rewind();
      this.currentTween = (TweenBase) null;
      this.currentIndex = -1;
      return base.Rewind();
    }

    public override void Update()
    {
      if (this.tweenList.Count == 0)
        return;
      if (this.State == TweenState.Started)
      {
        float currentTime = this.getCurrentTime();
        if ((double) currentTime < (double) this.startTime + (double) this.Delay)
          return;
        if (this.Mode == TweenGroupMode.Concurrent)
          this.startAllTweens();
        else if (!this.nextTween())
          return;
        this.startTime = currentTime;
        this.CurrentTime = 0.0f;
        this.State = TweenState.Playing;
      }
      else if (this.State != TweenState.Playing)
        return;
      if (this.Mode == TweenGroupMode.Concurrent)
      {
        if (!this.allTweensComplete())
          return;
        if (this.LoopType == TweenLoopType.Loop)
        {
          if (--this.LoopCount != 0)
          {
            if (this.TweenLoopCompleted != null)
              this.TweenLoopCompleted((TweenBase) this);
            if (this.State != TweenState.Playing)
              return;
            this.Rewind();
            this.Play();
            return;
          }
        }
        this.onGroupComplete();
      }
      else
      {
        if (this.currentTween.State != TweenState.Stopped || this.nextTween())
          return;
        this.Stop();
        this.raiseCompleted();
      }
    }

    protected override void Reset()
    {
      this.Stop();
      if (this.AutoCleanup)
        this.cleanUp();
      base.Reset();
      this.Mode = TweenGroupMode.Sequential;
      this.AutoCleanup = false;
      this.tweenList.Clear();
    }

    internal override float CalculateTotalDuration()
    {
      float a = 0.0f;
      if (this.Mode == TweenGroupMode.Sequential)
      {
        for (int index = 0; index < this.tweenList.Count; ++index)
        {
          TweenBase tween = this.tweenList[index];
          if (tween != null)
            a += tween.CalculateTotalDuration();
        }
      }
      else
      {
        for (int index = 0; index < this.tweenList.Count; ++index)
        {
          TweenBase tween = this.tweenList[index];
          if (tween != null)
            a = Mathf.Max(a, tween.CalculateTotalDuration());
        }
      }
      if (this.LoopCount > 0)
        a *= (float) this.LoopCount;
      else if (this.LoopType != TweenLoopType.None)
        a = float.PositiveInfinity;
      return this.Delay + a;
    }

    private void onGroupComplete()
    {
      this.Stop();
      this.raiseCompleted();
      if (!this.autoCleanup)
        return;
      this.cleanUp();
    }

    private void startAllTweens()
    {
      for (int index = 0; index < this.tweenList.Count; ++index)
        this.tweenList[index]?.Play();
    }

    private void pauseAllTweens()
    {
      for (int index = 0; index < this.tweenList.Count; ++index)
        this.tweenList[index]?.Pause();
    }

    private void resumeAllTweens()
    {
      for (int index = 0; index < this.tweenList.Count; ++index)
        this.tweenList[index]?.Resume();
    }

    private bool nextTween()
    {
      if (this.Mode == TweenGroupMode.Concurrent)
        return true;
      if (this.State == TweenState.Started)
      {
        this.currentIndex = 0;
        this.currentTween = this.tweenList[this.currentIndex];
        this.currentTween.Play();
        return true;
      }
      if (this.currentIndex == this.tweenList.Count - 1)
      {
        if (this.LoopType == TweenLoopType.Loop)
        {
          if (--this.LoopCount != 0)
          {
            if (this.TweenLoopCompleted != null)
              this.TweenLoopCompleted((TweenBase) this);
            this.currentIndex = 0;
            if (this.State == TweenState.Stopped)
              return false;
            goto label_13;
          }
        }
        return false;
      }
      ++this.currentIndex;
  label_13:
      this.currentTween = this.tweenList[this.currentIndex];
      this.currentTween.Play();
      return true;
    }

    private bool allTweensComplete()
    {
      if (this.Mode == TweenGroupMode.Sequential && this.currentTween != null)
        return this.currentTween.State == TweenState.Stopped;
      for (int index = 0; index < this.tweenList.Count; ++index)
      {
        if (this.tweenList[index].State != TweenState.Stopped)
          return false;
      }
      return true;
    }

    private void cleanUp()
    {
      int index = 0;
      while (index < this.tweenList.Count)
      {
        TweenBase tween = this.tweenList[index];
        if (tween != null && tween.AutoCleanup)
        {
          if (tween is IPoolableObject)
            ((IPoolableObject) tween).Release();
          this.tweenList.RemoveAt(index);
        }
        else
          ++index;
      }
    }

    public IEnumerator<TweenBase> GetEnumerator()
    {
      return (IEnumerator<TweenBase>) this.tweenList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.tweenList.GetEnumerator();
  }
}
