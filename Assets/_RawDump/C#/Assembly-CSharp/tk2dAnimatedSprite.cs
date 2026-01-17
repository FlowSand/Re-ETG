// Decompiled with JetBrains decompiler
// Type: tk2dAnimatedSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AddComponentMenu("2D Toolkit/Sprite/tk2dAnimatedSprite (Obsolete)")]
public class tk2dAnimatedSprite : tk2dSprite
{
  [SerializeField]
  private tk2dSpriteAnimator _animator;
  [SerializeField]
  private tk2dSpriteAnimation anim;
  [SerializeField]
  private int clipId;
  public bool playAutomatically;
  public bool createCollider;
  public tk2dAnimatedSprite.AnimationCompleteDelegate animationCompleteDelegate;
  public tk2dAnimatedSprite.AnimationEventDelegate animationEventDelegate;

  public tk2dSpriteAnimator Animator
  {
    get
    {
      this.CheckAddAnimatorInternal();
      return this._animator;
    }
  }

  private void CheckAddAnimatorInternal()
  {
    if (!((UnityEngine.Object) this._animator == (UnityEngine.Object) null))
      return;
    this._animator = this.gameObject.GetComponent<tk2dSpriteAnimator>();
    if (!((UnityEngine.Object) this._animator == (UnityEngine.Object) null))
      return;
    this._animator = this.gameObject.AddComponent<tk2dSpriteAnimator>();
    this._animator.Library = this.anim;
    this._animator.DefaultClipId = this.clipId;
    this._animator.playAutomatically = this.playAutomatically;
  }

  protected override bool NeedBoxCollider() => this.createCollider;

  public tk2dSpriteAnimation Library
  {
    get => this.Animator.Library;
    set => this.Animator.Library = value;
  }

  public int DefaultClipId
  {
    get => this.Animator.DefaultClipId;
    set => this.Animator.DefaultClipId = value;
  }

  public static bool g_paused
  {
    get => tk2dSpriteAnimator.g_Paused;
    set => tk2dSpriteAnimator.g_Paused = value;
  }

  public bool Paused
  {
    get => this.Animator.Paused;
    set => this.Animator.Paused = value;
  }

  private void ProxyCompletedHandler(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
  {
    if (this.animationCompleteDelegate == null)
      return;
    int clipId = -1;
    tk2dSpriteAnimationClip[] clips = !((UnityEngine.Object) anim.Library != (UnityEngine.Object) null) ? (tk2dSpriteAnimationClip[]) null : anim.Library.clips;
    if (clips != null)
    {
      for (int index = 0; index < clips.Length; ++index)
      {
        if (clips[index] == clip)
        {
          clipId = index;
          break;
        }
      }
    }
    this.animationCompleteDelegate(this, clipId);
  }

  private void ProxyEventTriggeredHandler(
    tk2dSpriteAnimator anim,
    tk2dSpriteAnimationClip clip,
    int frame)
  {
    if (this.animationEventDelegate == null)
      return;
    this.animationEventDelegate(this, clip, clip.frames[frame], frame);
  }

  private void OnEnable()
  {
    this.Animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.ProxyCompletedHandler);
    this.Animator.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.ProxyEventTriggeredHandler);
  }

  private void OnDisable()
  {
    this.Animator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>) null;
    this.Animator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>) null;
  }

  private void Start() => this.CheckAddAnimatorInternal();

  public static tk2dAnimatedSprite AddComponent(
    GameObject go,
    tk2dSpriteAnimation anim,
    int clipId)
  {
    tk2dSpriteAnimationClip clip = anim.clips[clipId];
    tk2dAnimatedSprite tk2dAnimatedSprite = go.AddComponent<tk2dAnimatedSprite>();
    tk2dAnimatedSprite.SetSprite(clip.frames[0].spriteCollection, clip.frames[0].spriteId);
    tk2dAnimatedSprite.anim = anim;
    return tk2dAnimatedSprite;
  }

  public void Play()
  {
    if (this.Animator.DefaultClip == null)
      return;
    this.Animator.Play(this.Animator.DefaultClip);
  }

  public void Play(float clipStartTime)
  {
    if (this.Animator.DefaultClip == null)
      return;
    this.Animator.PlayFrom(this.Animator.DefaultClip, clipStartTime);
  }

  public void PlayFromFrame(int frame)
  {
    if (this.Animator.DefaultClip == null)
      return;
    this.Animator.PlayFromFrame(this.Animator.DefaultClip, frame);
  }

  public void Play(string name) => this.Animator.Play(name);

  public void PlayFromFrame(string name, int frame) => this.Animator.PlayFromFrame(name, frame);

  public void Play(string name, float clipStartTime) => this.Animator.PlayFrom(name, clipStartTime);

  public void Play(tk2dSpriteAnimationClip clip, float clipStartTime)
  {
    this.Animator.PlayFrom(clip, clipStartTime);
  }

  public void Play(tk2dSpriteAnimationClip clip, float clipStartTime, float overrideFps)
  {
    this.Animator.Play(clip, clipStartTime, overrideFps);
  }

  public tk2dSpriteAnimationClip CurrentClip => this.Animator.CurrentClip;

  public float ClipTimeSeconds => this.Animator.ClipTimeSeconds;

  public float ClipFps
  {
    get => this.Animator.ClipFps;
    set => this.Animator.ClipFps = value;
  }

  public void Stop() => this.Animator.Stop();

  public void StopAndResetFrame() => this.Animator.StopAndResetFrame();

  [Obsolete]
  public bool isPlaying() => this.Animator.Playing;

  public bool IsPlaying(string name) => this.Animator.Playing;

  public bool IsPlaying(tk2dSpriteAnimationClip clip) => this.Animator.IsPlaying(clip);

  public bool Playing => this.Animator.Playing;

  public int GetClipIdByName(string name) => this.Animator.GetClipIdByName(name);

  public tk2dSpriteAnimationClip GetClipByName(string name) => this.Animator.GetClipByName(name);

  public static float DefaultFps => tk2dSpriteAnimator.DefaultFps;

  public void Pause() => this.Animator.Pause();

  public void Resume() => this.Animator.Resume();

  public void SetFrame(int currFrame) => this.Animator.SetFrame(currFrame);

  public void SetFrame(int currFrame, bool triggerEvent)
  {
    this.Animator.SetFrame(currFrame, triggerEvent);
  }

  public void UpdateAnimation(float deltaTime) => this.Animator.UpdateAnimation(deltaTime);

  protected override void OnDestroy() => base.OnDestroy();

  public delegate void AnimationCompleteDelegate(tk2dAnimatedSprite sprite, int clipId);

  public delegate void AnimationEventDelegate(
    tk2dAnimatedSprite sprite,
    tk2dSpriteAnimationClip clip,
    tk2dSpriteAnimationFrame frame,
    int frameNum);
}
