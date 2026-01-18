using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween
{
  public class TweenShake : TweenBase, IPoolableObject
  {
    public Vector3 StartValue;
    public float ShakeMagnitude;
    public float ShakeDuration;
    public float ShakeSpeed;
    public TweenAssignmentCallback<Vector3> Execute;
    public TweenCallback ShakeCompleted;
    protected Vector3 currentValue;
    private static List<TweenShake> pool = new List<TweenShake>();

    public TweenShake() => this.ShakeSpeed = 10f;

    public TweenShake(
      Vector3 StartValue,
      float ShakeMagnitude,
      float ShakeDuration,
      float ShakeSpeed,
      float StartDelay,
      bool AutoCleanup,
      TweenAssignmentCallback<Vector3> OnExecute)
    {
      this.SetStartValue(StartValue).SetShakeMagnitude(ShakeMagnitude).SetDuration(ShakeDuration).SetShakeSpeed(ShakeSpeed).SetDelay(StartDelay).SetAutoCleanup(AutoCleanup).OnExecute(OnExecute);
    }

    public static TweenShake Obtain()
    {
      if (TweenShake.pool.Count <= 0)
        return new TweenShake();
      TweenShake tweenShake = TweenShake.pool[TweenShake.pool.Count - 1];
      TweenShake.pool.RemoveAt(TweenShake.pool.Count - 1);
      return tweenShake;
    }

    public void Release()
    {
      this.Stop();
      this.StartValue = Vector3.zero;
      this.currentValue = Vector3.zero;
      this.CurrentTime = 0.0f;
      this.Delay = 0.0f;
      this.ShakeCompleted = (TweenCallback) null;
      this.Execute = (TweenAssignmentCallback<Vector3>) null;
      TweenShake.pool.Add(this);
    }

    public TweenShake SetTimeScaleIndependent(bool timeScaleIndependent)
    {
      this.IsTimeScaleIndependent = timeScaleIndependent;
      return this;
    }

    public TweenShake SetAutoCleanup(bool autoCleanup)
    {
      this.AutoCleanup = autoCleanup;
      return this;
    }

    public TweenShake SetDuration(float duration)
    {
      this.ShakeDuration = duration;
      return this;
    }

    public TweenShake SetStartValue(Vector3 value)
    {
      this.StartValue = value;
      return this;
    }

    public TweenShake SetDelay(float seconds)
    {
      this.Delay = seconds;
      return this;
    }

    public TweenShake SetShakeMagnitude(float magnitude)
    {
      this.ShakeMagnitude = magnitude;
      return this;
    }

    public TweenShake SetShakeSpeed(float speed)
    {
      this.ShakeSpeed = speed;
      return this;
    }

    public TweenShake OnExecute(TweenAssignmentCallback<Vector3> Execute)
    {
      this.Execute = Execute;
      return this;
    }

    public TweenShake OnComplete(TweenCallback Complete)
    {
      this.ShakeCompleted = Complete;
      return this;
    }

    public override void Update()
    {
      float currentTime = this.getCurrentTime();
      if (this.State == TweenState.Started)
      {
        if ((double) currentTime < (double) this.startTime + (double) this.Delay)
          return;
        this.startTime = currentTime;
        this.CurrentTime = 0.0f;
        this.State = TweenState.Playing;
      }
      else if (this.State != TweenState.Playing)
        return;
      this.CurrentTime = Mathf.MoveTowards(this.CurrentTime, 1f, this.getDeltaTime() / this.ShakeDuration);
      float num = (1f - this.CurrentTime) * this.ShakeMagnitude;
      this.currentValue = this.StartValue + new Vector3((float) ((double) Mathf.PerlinNoise(0.33f, currentTime * this.ShakeSpeed) * 2.0 - 1.0), (float) ((double) Mathf.PerlinNoise(0.66f, currentTime * this.ShakeSpeed) * 2.0 - 1.0), (float) ((double) Mathf.PerlinNoise(1f, currentTime * this.ShakeSpeed) * 2.0 - 1.0)) * num;
      if (this.Execute != null)
        this.Execute(this.currentValue);
      if ((double) this.CurrentTime < 1.0)
        return;
      this.Stop();
      this.raiseCompleted();
      if (!this.AutoCleanup)
        return;
      this.Release();
    }
  }
}
