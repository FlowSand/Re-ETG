// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Components.TweenCameraShake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Editor;
using System;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
  [InspectorGroupOrder(new string[] {"General", "Animation", "Looping", "Parameters"})]
  [AddComponentMenu("Daikon Forge/Tween/Camera Shake")]
  public class TweenCameraShake : TweenComponentBase
  {
    [SerializeField]
    [Inspector("Parameters", 0, Label = "Duration")]
    protected float duration = 1f;
    [Inspector("Parameters", 0, Label = "Magnitude")]
    [SerializeField]
    protected float shakeMagnitude = 0.25f;
    [Inspector("Parameters", 0, Label = "Speed")]
    [SerializeField]
    protected float shakeSpeed = 13f;
    protected TweenShake tween;

    public float Duration
    {
      get => this.duration;
      set => this.duration = Mathf.Max(0.0f, value);
    }

    public float ShakeMagnitude
    {
      get => this.shakeMagnitude;
      set
      {
        if ((double) value == (double) this.shakeMagnitude)
          return;
        this.shakeMagnitude = value;
        this.Stop();
      }
    }

    public float ShakeSpeed
    {
      get => this.shakeSpeed;
      set
      {
        if ((double) value == (double) this.shakeSpeed)
          return;
        this.shakeSpeed = value;
        this.Stop();
      }
    }

    public override TweenBase BaseTween
    {
      get
      {
        this.configureTween();
        return (TweenBase) this.tween;
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

    protected void cleanup()
    {
      if (this.tween == null)
        return;
      this.tween.Stop();
      this.tween.Release();
      this.tween = (TweenShake) null;
    }

    protected void validateTweenConfiguration()
    {
      this.loopCount = Mathf.Max(0, this.loopCount);
      if ((UnityEngine.Object) this.gameObject.GetComponent<Camera>() == (UnityEngine.Object) null)
        throw new InvalidOperationException("Camera not found");
    }

    protected void configureTween()
    {
      Camera component = this.gameObject.GetComponent<Camera>();
      if (this.tween == null)
        this.tween = (TweenShake) component.ShakePosition(true).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
      this.tween.SetDelay(this.startDelay).SetDuration(this.Duration).SetShakeMagnitude(this.ShakeMagnitude).SetShakeSpeed(this.ShakeSpeed);
    }
  }
}
