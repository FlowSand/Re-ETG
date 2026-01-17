// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Components.TweenObjectPath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Editor;
using System;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
  [InspectorGroupOrder(new string[] {"General", "Path", "Animation", "Looping"})]
  [AddComponentMenu("Daikon Forge/Tween/Move Along Path")]
  public class TweenObjectPath : TweenComponentBase
  {
    [Inspector("Animation", 0, Label = "Duration", Tooltip = "How long the Tween should take to complete the animation")]
    [SerializeField]
    protected float duration = 1f;
    [Inspector("Path", 0, Label = "Path", Tooltip = "The path for the object to follow")]
    [SerializeField]
    protected SplineObject path;
    [SerializeField]
    [Inspector("Animation", 1, Label = "Orient To Path", Tooltip = "If set to TRUE, will orient the object to face the direction of the path")]
    protected bool orientToPath = true;
    [SerializeField]
    protected TweenDirection playDirection;
    protected DaikonForge.Tween.Tween<float> tween;

    public float Duration
    {
      get => this.duration;
      set => this.duration = Mathf.Max(0.0f, value);
    }

    public override TweenBase BaseTween
    {
      get
      {
        this.configureTween();
        return (TweenBase) this.tween;
      }
    }

    public SplineObject Path
    {
      get => this.path;
      set
      {
        this.cleanup();
        this.path = value;
      }
    }

    public override TweenState State => this.tween == null ? TweenState.Stopped : this.tween.State;

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

    public bool OrientToPath
    {
      get => this.orientToPath;
      set
      {
        this.orientToPath = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.tween.Release();
        this.tween = (DaikonForge.Tween.Tween<float>) null;
        this.Play();
      }
    }

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
      this.tween = (DaikonForge.Tween.Tween<float>) null;
    }

    protected void validateTweenConfiguration()
    {
      this.loopCount = Mathf.Max(0, this.loopCount);
      if ((UnityEngine.Object) this.Path == (UnityEngine.Object) null)
        throw new InvalidOperationException("The Path property cannot be NULL");
    }

    protected void configureTween()
    {
      this.Path.CalculateSpline();
      if (this.tween == null)
        this.tween = (DaikonForge.Tween.Tween<float>) this.transform.TweenPath((IPathIterator) this.Path.Spline, this.orientToPath).OnStarted((TweenCallback) (x => this.onStarted())).OnStopped((TweenCallback) (x => this.onStopped())).OnPaused((TweenCallback) (x => this.onPaused())).OnResumed((TweenCallback) (x => this.onResumed())).OnLoopCompleted((TweenCallback) (x => this.onLoopCompleted())).OnCompleted((TweenCallback) (x => this.onCompleted()));
      this.Path.CalculateSpline();
      this.tween.SetDelay(this.startDelay).SetDuration(this.duration).SetLoopType(this.loopType).SetLoopCount(this.loopCount).SetPlayDirection(this.playDirection);
    }
  }
}
