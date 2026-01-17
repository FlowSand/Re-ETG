// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Components.TweenComponentBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Editor;
using System;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components
{
  [Serializable]
  public abstract class TweenComponentBase : TweenPlayableComponent
  {
    [Inspector("General", Order = -1, Label = "Name", Tooltip = "For your convenience, you may specify a name for this Tween")]
    [SerializeField]
    protected string tweenName;
    [SerializeField]
    [Inspector("Animation", Order = 0, Label = "Delay", Tooltip = "The amount of time in seconds to delay before starting the animation")]
    protected float startDelay;
    [Inspector("Animation", Order = 1, Label = "Assign Start First", Tooltip = "If set, the StartValue will be assigned to the target before the delay (if any) is performed")]
    [SerializeField]
    protected bool assignStartValueBeforeDelay = true;
    [SerializeField]
    [Inspector("Looping", Order = 1, Label = "Type", Tooltip = "Specify whether the animation will loop at the end")]
    protected TweenLoopType loopType;
    [Inspector("Looping", Order = 1, Label = "Count", Tooltip = "If set to 0, the animation will loop forever")]
    [SerializeField]
    protected int loopCount;
    protected bool wasAutoStarted;

    private static bool IsLoopCountVisible(object target) => true;

    public float StartDelay
    {
      get => this.startDelay;
      set => this.startDelay = value;
    }

    public bool AssignStartValueBeforeDelay
    {
      get => this.assignStartValueBeforeDelay;
      set => this.assignStartValueBeforeDelay = value;
    }

    public TweenLoopType LoopType
    {
      get => this.loopType;
      set
      {
        this.loopType = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public int LoopCount
    {
      get => this.loopCount;
      set
      {
        this.loopCount = value;
        if (this.State == TweenState.Stopped)
          return;
        this.Stop();
        this.Play();
      }
    }

    public bool IsPlaying
    {
      get
      {
        if (!this.enabled)
          return false;
        return this.State == TweenState.Started || this.State == TweenState.Playing;
      }
    }

    public bool IsPaused => this.State == TweenState.Paused;

    public override void Start()
    {
      base.Start();
      if (!this.autoRun || this.wasAutoStarted)
        return;
      this.wasAutoStarted = true;
      this.Play();
    }

    public override void OnEnable()
    {
      base.OnEnable();
      if (!this.autoRun || this.wasAutoStarted)
        return;
      this.wasAutoStarted = true;
      this.Play();
    }

    public override void OnDisable()
    {
      base.OnDisable();
      if (this.IsPlaying)
        this.Stop();
      this.wasAutoStarted = false;
    }

    public override string ToString()
    {
      return $"{this.gameObject.name}.{this.GetType().Name} '{this.tweenName}'";
    }
  }
}
