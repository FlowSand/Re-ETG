// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.TweenWait
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace DaikonForge.Tween
{
  public class TweenWait : TweenBase
  {
    private static List<TweenWait> pool = new List<TweenWait>();
    private float elapsed;

    public TweenWait(float seconds) => this.Delay = seconds;

    public static TweenWait Obtain(float seconds)
    {
      if (TweenWait.pool.Count > 0)
      {
        TweenWait tweenWait = TweenWait.pool[TweenWait.pool.Count - 1];
        TweenWait.pool.RemoveAt(TweenWait.pool.Count - 1);
        tweenWait.Delay = seconds;
        return tweenWait;
      }
      TweenWait tweenWait1 = new TweenWait(seconds);
      tweenWait1.AutoCleanup = true;
      return tweenWait1;
    }

    public void Release()
    {
      if (TweenWait.pool.Contains(this))
        return;
      this.Reset();
      TweenWait.pool.Add(this);
    }

    public override TweenBase Rewind()
    {
      this.elapsed = 0.0f;
      return base.Rewind();
    }

    public override TweenBase FastForward()
    {
      this.elapsed = this.Delay;
      return base.FastForward();
    }

    public override void Update()
    {
      if (this.State != TweenState.Playing && this.State != TweenState.Started)
        return;
      if (this.State == TweenState.Started)
      {
        this.elapsed = 0.0f;
        this.startTime = this.getCurrentTime();
        this.State = TweenState.Playing;
      }
      else
      {
        this.elapsed += this.getDeltaTime();
        if ((double) this.elapsed < (double) this.Delay)
          return;
        this.Stop();
        this.raiseCompleted();
      }
    }
  }
}
