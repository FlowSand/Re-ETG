// Decompiled with JetBrains decompiler
// Type: DaikonForge.Tween.Components.TweenPlayableComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using DaikonForge.Editor;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace DaikonForge.Tween.Components;

public abstract class TweenPlayableComponent : MonoBehaviour
{
  [SerializeField]
  protected bool autoRun;

  public event TweenComponentNotification TweenStarted;

  public event TweenComponentNotification TweenStopped;

  public event TweenComponentNotification TweenPaused;

  public event TweenComponentNotification TweenResumed;

  public event TweenComponentNotification TweenLoopCompleted;

  public event TweenComponentNotification TweenCompleted;

  public virtual string TweenName { get; set; }

  public abstract TweenState State { get; }

  public abstract TweenBase BaseTween { get; }

  [Inspector("General", 1, BackingField = "autoRun", Tooltip = "If set to TRUE, this Tween will automatically play when the scene starts")]
  public bool AutoRun
  {
    get => this.autoRun;
    set => this.autoRun = value;
  }

  public abstract void Play();

  public abstract void Stop();

  public abstract void Rewind();

  public abstract void FastForward();

  public abstract void Pause();

  public abstract void Resume();

  public virtual void Awake()
  {
  }

  public virtual void Start()
  {
  }

  public virtual void OnEnable()
  {
  }

  public virtual void OnDisable()
  {
  }

  public virtual void OnDestroy()
  {
  }

  public virtual void Enable() => this.enabled = true;

  public virtual void Disable() => this.enabled = false;

  [DebuggerHidden]
  public virtual IEnumerator WaitForCompletion()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new TweenPlayableComponent.<WaitForCompletion>c__Iterator0()
    {
      $this = this
    };
  }

  protected virtual void onPaused()
  {
    if (this.TweenPaused == null)
      return;
    this.TweenPaused(this);
  }

  protected virtual void onResumed()
  {
    if (this.TweenResumed == null)
      return;
    this.TweenResumed(this);
  }

  protected virtual void onStarted()
  {
    if (this.TweenStarted == null)
      return;
    this.TweenStarted(this);
  }

  protected virtual void onStopped()
  {
    if (this.TweenStopped == null)
      return;
    this.TweenStopped(this);
  }

  protected virtual void onLoopCompleted()
  {
    if (this.TweenLoopCompleted == null)
      return;
    this.TweenLoopCompleted(this);
  }

  protected virtual void onCompleted()
  {
    if (this.TweenCompleted == null)
      return;
    this.TweenCompleted(this);
  }

  public override string ToString() => $"{this.TweenName} - {base.ToString()}";
}
