// Decompiled with JetBrains decompiler
// Type: tk2dSpriteAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    [AddComponentMenu("2D Toolkit/Sprite/tk2dSpriteAnimator")]
    public class tk2dSpriteAnimator : BraveBehaviour
    {
      [SerializeField]
      private tk2dSpriteAnimation library;
      [SerializeField]
      private int defaultClipId;
      public float AdditionalCameraVisibilityRadius;
      private float m_fidgetDuration;
      private float m_fidgetElapsed;
      public bool AnimateDuringBossIntros;
      public bool AlwaysIgnoreTimeScale;
      public bool ForceSetEveryFrame;
      public bool playAutomatically;
      [NonSerialized]
      public bool alwaysUpdateOffscreen;
      [NonSerialized]
      public bool maximumDeltaOneFrame;
      [SerializeField]
      public bool IsFrameBlendedAnimation;
      private static tk2dSpriteAnimator.State globalState;
      private tk2dSpriteAnimationClip currentClip;
      public float clipTime;
      private float clipFps = -1f;
      private int previousFrame = -1;
      public Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> OnPlayAnimationCalled;
      public Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> AnimationCompleted;
      private System.Action m_onDestroyAction;
      public Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int> AnimationEventTriggered;
      private tk2dSpriteAnimator.State state;
      public bool deferNextStartClip;
      private bool m_hasAttachPoints;
      protected tk2dBaseSprite _sprite;
      protected tk2dSpriteCollectionData _startingSpriteCollection;
      protected int _startingSpriteId;
      private string m_queuedAnimationName;
      private GameObject m_overrideTargetDisableObject;
      private GameObject m_cachedAudioBaseObject;
      private bool m_isCurrentlyVisible;
      public static Vector2 CameraPositionThisFrame;
      public static bool InDungeonScene;
      [NonSerialized]
      public bool ignoreTimeScale;
      [NonSerialized]
      public float OverrideTimeScale = -1f;
      private bool m_forceNextSpriteUpdate;

      public static bool g_Paused
      {
        get
        {
          return (tk2dSpriteAnimator.globalState & tk2dSpriteAnimator.State.Paused) != tk2dSpriteAnimator.State.Init;
        }
        set
        {
          tk2dSpriteAnimator.globalState = !value ? tk2dSpriteAnimator.State.Init : tk2dSpriteAnimator.State.Paused;
        }
      }

      public bool MuteAudio { get; set; }

      public bool Paused
      {
        get => (this.state & tk2dSpriteAnimator.State.Paused) != tk2dSpriteAnimator.State.Init;
        set
        {
          if (value)
            this.state |= tk2dSpriteAnimator.State.Paused;
          else
            this.state &= ~tk2dSpriteAnimator.State.Paused;
        }
      }

      public tk2dSpriteAnimation Library
      {
        get => this.library;
        set => this.library = value;
      }

      public int DefaultClipId
      {
        get => this.defaultClipId;
        set => this.defaultClipId = value;
      }

      public tk2dSpriteAnimationClip DefaultClip => this.GetClipById(this.defaultClipId);

      public void ForceClearCurrentClip() => this.currentClip = (tk2dSpriteAnimationClip) null;

      private void OnEnable()
      {
        if (!((UnityEngine.Object) this.Sprite == (UnityEngine.Object) null))
          return;
        this.enabled = false;
      }

      private void Awake()
      {
        if ((bool) (UnityEngine.Object) this.Sprite)
        {
          this._startingSpriteCollection = this.Sprite.Collection;
          this._startingSpriteId = this.Sprite.spriteId;
        }
        if (!this.AlwaysIgnoreTimeScale)
          return;
        this.ignoreTimeScale = true;
      }

      private void Start()
      {
        if (!this.deferNextStartClip && this.playAutomatically && !this.IsPlaying(this.DefaultClip))
          this.Play(this.DefaultClip);
        this.deferNextStartClip = false;
        if (!(bool) (UnityEngine.Object) this.GetComponent<tk2dSpriteAttachPoint>())
          return;
        this.m_hasAttachPoints = true;
      }

      public void OnSpawned()
      {
        if (!this.enabled)
          return;
        this.OnEnable();
        this.Start();
      }

      public void OnDespawned()
      {
        if (this.playAutomatically)
          this.StopAndResetFrame();
        else
          this.Stop();
      }

      protected override void OnDestroy() => base.OnDestroy();

      public virtual tk2dBaseSprite Sprite
      {
        get
        {
          if ((UnityEngine.Object) this._sprite == (UnityEngine.Object) null)
          {
            this._sprite = this.GetComponent<tk2dBaseSprite>();
            if ((UnityEngine.Object) this._sprite == (UnityEngine.Object) null)
              UnityEngine.Debug.LogError((object) "Sprite not found attached to tk2dSpriteAnimator.");
          }
          return this._sprite;
        }
      }

      public static tk2dSpriteAnimator AddComponent(
        GameObject go,
        tk2dSpriteAnimation anim,
        int clipId)
      {
        tk2dSpriteAnimationClip clip = anim.clips[clipId];
        tk2dSpriteAnimator tk2dSpriteAnimator = go.AddComponent<tk2dSpriteAnimator>();
        tk2dSpriteAnimator.Library = anim;
        if (clip.frames[0].requiresOffscreenUpdate)
          tk2dSpriteAnimator.m_forceNextSpriteUpdate = true;
        tk2dSpriteAnimator.SetSprite(clip.frames[0].spriteCollection, clip.frames[0].spriteId);
        if (clip.frames[0].requiresOffscreenUpdate)
          tk2dSpriteAnimator.m_forceNextSpriteUpdate = true;
        return tk2dSpriteAnimator;
      }

      private tk2dSpriteAnimationClip GetClipByNameVerbose(string name)
      {
        if ((UnityEngine.Object) this.library == (UnityEngine.Object) null)
        {
          UnityEngine.Debug.LogError((object) "Library not set");
          return (tk2dSpriteAnimationClip) null;
        }
        tk2dSpriteAnimationClip clipByName = this.library.GetClipByName(name);
        if (clipByName != null)
          return clipByName;
        UnityEngine.Debug.LogError((object) $"Unable to find clip '{name}' in library");
        return (tk2dSpriteAnimationClip) null;
      }

      public void Play()
      {
        if (this.currentClip == null)
          this.currentClip = this.DefaultClip;
        this.Play(this.currentClip);
      }

      public void Play(string name) => this.Play(this.GetClipByNameVerbose(name));

      public void Play(tk2dSpriteAnimationClip clip)
      {
        this.Play(clip, 0.0f, tk2dSpriteAnimator.DefaultFps);
      }

      public void PlayFromFrame(int frame)
      {
        if (this.currentClip == null)
          this.currentClip = this.DefaultClip;
        this.PlayFromFrame(this.currentClip, frame);
      }

      public void PlayFromFrame(string name, int frame)
      {
        this.PlayFromFrame(this.GetClipByNameVerbose(name), frame);
      }

      public void PlayFromFrame(tk2dSpriteAnimationClip clip, int frame)
      {
        this.PlayFrom(clip, ((float) frame + 1f / 1000f) / clip.fps);
      }

      public void PlayFrom(float clipStartTime)
      {
        if (this.currentClip == null)
          this.currentClip = this.DefaultClip;
        this.PlayFrom(this.currentClip, clipStartTime);
      }

      public void PlayFrom(string name, float clipStartTime)
      {
        tk2dSpriteAnimationClip clipByName = !(bool) (UnityEngine.Object) this.library ? (tk2dSpriteAnimationClip) null : this.library.GetClipByName(name);
        if (clipByName == null)
          this.ClipNameError(name);
        else
          this.PlayFrom(clipByName, clipStartTime);
      }

      public void PlayFrom(tk2dSpriteAnimationClip clip, float clipStartTime)
      {
        this.Play(clip, clipStartTime, tk2dSpriteAnimator.DefaultFps);
      }

      public void QueueAnimation(string animationName)
      {
        this.m_queuedAnimationName = animationName;
        this.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StartQueuedAnimationSimple);
      }

      private void StartQueuedAnimationSimple(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
      {
        this.Play(this.m_queuedAnimationName);
        this.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StartQueuedAnimationSimple);
      }

      private void StopAndDisableGameObject(tk2dSpriteAnimator source, tk2dSpriteAnimationClip clip)
      {
        this.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StopAndDisableGameObject);
        this.Stop();
        if ((UnityEngine.Object) this.m_overrideTargetDisableObject != (UnityEngine.Object) null)
        {
          this.m_overrideTargetDisableObject.SetActive(false);
          this.m_overrideTargetDisableObject = (GameObject) null;
        }
        else
          this.gameObject.SetActive(false);
      }

      private void StopAndDestroyGameObject(tk2dSpriteAnimator source, tk2dSpriteAnimationClip clip)
      {
        if (this.m_onDestroyAction != null)
          this.m_onDestroyAction();
        this.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StopAndDestroyGameObject);
        this.Stop();
        SpawnManager.Despawn(this.gameObject);
      }

      public void PlayAndDestroyObject(string clipName = "", System.Action onDestroy = null)
      {
        this.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StopAndDestroyGameObject);
        if (onDestroy != null)
          this.m_onDestroyAction = onDestroy;
        if (string.IsNullOrEmpty(clipName))
          this.Play();
        else
          this.Play(clipName);
      }

      public void PlayAndDisableObject(string clipName = "", GameObject overrideTargetObject = null)
      {
        this.m_overrideTargetDisableObject = overrideTargetObject;
        this.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StopAndDisableGameObject);
        if (string.IsNullOrEmpty(clipName))
          this.Play();
        else
          this.Play(clipName);
      }

      public void PlayAndDisableRenderer(string clipName = "")
      {
        this.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StopAndDisableRenderer);
        if (string.IsNullOrEmpty(clipName))
          this.Play();
        else
          this.Play(clipName);
      }

      private void StopAndDisableRenderer(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
      {
        this.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.StopAndDisableRenderer);
        this.Stop();
        this.GetComponent<Renderer>().enabled = false;
      }

      public void PlayForDurationForceLoop(tk2dSpriteAnimationClip clip, float duration)
      {
        if (clip == null)
        {
          this.ClipNameError(this.name);
        }
        else
        {
          this.Play(clip);
          if ((double) duration < 0.0)
            duration = clip.BaseClipLength;
          this.StartCoroutine(this.RevertToClipForceLoop(clip, duration));
        }
      }

      [DebuggerHidden]
      private IEnumerator RevertToClipForceLoop(tk2dSpriteAnimationClip playingClip, float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new tk2dSpriteAnimator.<RevertToClipForceLoop>c__Iterator0()
        {
          duration = duration,
          playingClip = playingClip,
          $this = this
        };
      }

      public void PlayForDuration(
        string name,
        float duration,
        string revertAnimName,
        bool returnToLoopSection = false)
      {
        tk2dSpriteAnimationClip clipByName1 = !(bool) (UnityEngine.Object) this.library ? (tk2dSpriteAnimationClip) null : this.library.GetClipByName(revertAnimName);
        tk2dSpriteAnimationClip clipByName2 = !(bool) (UnityEngine.Object) this.library ? (tk2dSpriteAnimationClip) null : this.library.GetClipByName(name);
        if (clipByName2 == null)
          return;
        this.Play(clipByName2);
        if ((double) duration < 0.0)
          duration = clipByName2.BaseClipLength;
        this.StartCoroutine(this.RevertToClip(clipByName2, clipByName1, duration, returnToLoopSection));
      }

      public void PlayForDuration(string name, float duration)
      {
        tk2dSpriteAnimationClip currentClip = this.currentClip;
        tk2dSpriteAnimationClip clipByName = !(bool) (UnityEngine.Object) this.library ? (tk2dSpriteAnimationClip) null : this.library.GetClipByName(name);
        if (clipByName == null)
        {
          this.ClipNameError(name);
        }
        else
        {
          this.Play(clipByName);
          if ((double) duration < 0.0)
            duration = clipByName.BaseClipLength;
          this.StartCoroutine(this.RevertToClip(clipByName, currentClip, duration));
        }
      }

      [DebuggerHidden]
      private IEnumerator RevertToClip(
        tk2dSpriteAnimationClip playingClip,
        tk2dSpriteAnimationClip revertToClip,
        float duration,
        bool returnToLoopSection = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new tk2dSpriteAnimator.<RevertToClip>c__Iterator1()
        {
          duration = duration,
          playingClip = playingClip,
          revertToClip = revertToClip,
          returnToLoopSection = returnToLoopSection,
          $this = this
        };
      }

      public void PlayAndForceTime(string clipName, float forceTime)
      {
        this.PlayAndForceTime(this.GetClipByName(clipName), forceTime);
      }

      public void PlayAndForceTime(tk2dSpriteAnimationClip clip, float forceTime)
      {
        this.Play(clip, 0.0f, clip.fps * (clip.BaseClipLength / forceTime));
      }

      public void Play(
        tk2dSpriteAnimationClip clip,
        float clipStartTime,
        float overrideFps,
        bool skipEvents = false)
      {
        if (this.OnPlayAnimationCalled != null)
          this.OnPlayAnimationCalled(this, clip);
        if (clip != null)
        {
          float num = (double) overrideFps <= 0.0 ? clip.fps : overrideFps;
          if ((double) clipStartTime == 0.0 && this.IsPlaying(clip))
          {
            this.clipFps = num;
          }
          else
          {
            this.state |= tk2dSpriteAnimator.State.Playing;
            this.currentClip = clip;
            this.clipFps = num;
            if (this.currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single || this.currentClip.frames == null)
            {
              this.WarpClipToLocalTime(this.currentClip, 0.0f, skipEvents);
              this.state &= ~tk2dSpriteAnimator.State.Playing;
            }
            else if (this.currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame || this.currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
            {
              this.WarpClipToLocalTime(this.currentClip, (float) UnityEngine.Random.Range(0, this.currentClip.frames.Length), skipEvents);
              if (this.currentClip.wrapMode != tk2dSpriteAnimationClip.WrapMode.RandomFrame)
                return;
              this.previousFrame = -1;
              this.state &= ~tk2dSpriteAnimator.State.Playing;
            }
            else
            {
              float time = clipStartTime * this.clipFps;
              if (this.currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once && (double) time >= (double) this.clipFps * (double) this.currentClip.frames.Length)
              {
                this.WarpClipToLocalTime(this.currentClip, (float) (this.currentClip.frames.Length - 1), skipEvents);
                this.state &= ~tk2dSpriteAnimator.State.Playing;
              }
              else
              {
                this.WarpClipToLocalTime(this.currentClip, time, skipEvents);
                this.clipTime = time;
              }
            }
          }
        }
        else
        {
          UnityEngine.Debug.LogWarning((object) "Calling clip.Play() with a null clip");
          this.OnAnimationCompleted();
          this.state &= ~tk2dSpriteAnimator.State.Playing;
        }
      }

      public bool QueryPreviousInvulnerabilityFrame(int framesBack)
      {
        return this.CurrentClip != null && this.CurrentFrame >= framesBack && this.CurrentFrame < this.CurrentClip.frames.Length && this.CurrentClip.frames[this.CurrentFrame - framesBack].invulnerableFrame;
      }

      public bool QueryInvulnerabilityFrame()
      {
        return this.CurrentClip != null && this.CurrentFrame >= 0 && this.CurrentFrame < this.CurrentClip.frames.Length && this.CurrentClip.frames[this.CurrentFrame].invulnerableFrame;
      }

      public bool QueryGroundedFrame()
      {
        return this.CurrentClip == null || this.CurrentFrame < 0 || this.CurrentFrame >= this.CurrentClip.frames.Length || this.CurrentClip.frames[this.CurrentFrame].groundedFrame;
      }

      public void Stop() => this.state &= ~tk2dSpriteAnimator.State.Playing;

      public void StopAndResetFrame()
      {
        if (this.currentClip != null)
        {
          if (this.currentClip.frames[0].requiresOffscreenUpdate)
            this.m_forceNextSpriteUpdate = true;
          this.SetSprite(this.currentClip.frames[0].spriteCollection, this.currentClip.frames[0].spriteId);
          if (this.currentClip.frames[0].requiresOffscreenUpdate)
            this.m_forceNextSpriteUpdate = true;
        }
        this.Stop();
      }

      public void StopAndResetFrameToDefault()
      {
        if (this.currentClip != null)
        {
          if (this.currentClip.frames[0].requiresOffscreenUpdate)
            this.m_forceNextSpriteUpdate = true;
          this.SetSprite(this._startingSpriteCollection, this._startingSpriteId);
          if (this.currentClip.frames[0].requiresOffscreenUpdate)
            this.m_forceNextSpriteUpdate = true;
        }
        this.Stop();
      }

      public bool IsPlaying(string name)
      {
        return this.Playing && this.CurrentClip != null && this.CurrentClip.name == name;
      }

      public bool IsPlaying(tk2dSpriteAnimationClip clip)
      {
        return this.Playing && this.CurrentClip != null && this.CurrentClip == clip;
      }

      public bool Playing
      {
        get => (this.state & tk2dSpriteAnimator.State.Playing) != tk2dSpriteAnimator.State.Init;
      }

      public tk2dSpriteAnimationClip CurrentClip => this.currentClip;

      public float ClipTimeSeconds
      {
        get
        {
          return (double) this.clipFps > 0.0 ? this.clipTime / this.clipFps : this.clipTime / this.currentClip.fps;
        }
      }

      public float ClipFps
      {
        get => this.clipFps;
        set
        {
          if (this.currentClip == null)
            return;
          this.clipFps = (double) value <= 0.0 ? this.currentClip.fps : value;
        }
      }

      public tk2dSpriteAnimationClip GetClipById(int id)
      {
        return (UnityEngine.Object) this.library == (UnityEngine.Object) null ? (tk2dSpriteAnimationClip) null : this.library.GetClipById(id);
      }

      public static float DefaultFps => 0.0f;

      public int GetClipIdByName(string name)
      {
        return (bool) (UnityEngine.Object) this.library ? this.library.GetClipIdByName(name) : -1;
      }

      public tk2dSpriteAnimationClip GetClipByName(string name)
      {
        return (bool) (UnityEngine.Object) this.library ? this.library.GetClipByName(name) : (tk2dSpriteAnimationClip) null;
      }

      public void Pause() => this.state |= tk2dSpriteAnimator.State.Paused;

      public void Resume() => this.state &= ~tk2dSpriteAnimator.State.Paused;

      public void SetFrame(int currFrame) => this.SetFrame(currFrame, true);

      public void SetFrame(int currFrame, bool triggerEvent)
      {
        if (this.currentClip == null)
          this.currentClip = this.DefaultClip;
        if (this.currentClip == null)
          return;
        int num = currFrame % this.currentClip.frames.Length;
        this.SetFrameInternal(num);
        if (!triggerEvent || this.currentClip.frames.Length <= 0 || currFrame < 0)
          return;
        this.ProcessEvents(num - 1, num, 1);
      }

      public int CurrentFrame
      {
        get
        {
          switch (this.currentClip.wrapMode)
          {
            case tk2dSpriteAnimationClip.WrapMode.Loop:
            case tk2dSpriteAnimationClip.WrapMode.RandomLoop:
            case tk2dSpriteAnimationClip.WrapMode.LoopFidget:
              return (int) this.clipTime % this.currentClip.frames.Length;
            case tk2dSpriteAnimationClip.WrapMode.LoopSection:
              int clipTime = (int) this.clipTime;
              int num = this.currentClip.loopStart + (clipTime - this.currentClip.loopStart) % (this.currentClip.frames.Length - this.currentClip.loopStart);
              return clipTime >= this.currentClip.loopStart ? num : clipTime;
            case tk2dSpriteAnimationClip.WrapMode.Once:
              return Mathf.Min((int) this.clipTime, this.currentClip.frames.Length);
            case tk2dSpriteAnimationClip.WrapMode.PingPong:
              int currentFrame = this.currentClip.frames.Length <= 1 ? 0 : (int) this.clipTime % (this.currentClip.frames.Length + this.currentClip.frames.Length - 2);
              if (currentFrame >= this.currentClip.frames.Length)
                currentFrame = 2 * this.currentClip.frames.Length - 2 - currentFrame;
              return currentFrame;
            case tk2dSpriteAnimationClip.WrapMode.Single:
              return 0;
            default:
              UnityEngine.Debug.LogError((object) "Unhandled clip wrap mode");
              goto case tk2dSpriteAnimationClip.WrapMode.Loop;
          }
        }
      }

      public void UpdateAnimation(float deltaTime)
      {
        if ((this.state | tk2dSpriteAnimator.globalState) != tk2dSpriteAnimator.State.Playing)
          return;
        if (this.currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopFidget && (double) this.m_fidgetDuration > 0.0)
        {
          this.m_fidgetElapsed += deltaTime;
          if ((double) this.m_fidgetElapsed >= (double) this.m_fidgetDuration)
          {
            this.m_fidgetElapsed = 0.0f;
            this.m_fidgetDuration = 0.0f;
            this.clipTime += deltaTime * this.clipFps;
          }
        }
        else
          this.clipTime += deltaTime * this.clipFps;
        int previousFrame = this.previousFrame;
        switch (this.currentClip.wrapMode)
        {
          case tk2dSpriteAnimationClip.WrapMode.Loop:
          case tk2dSpriteAnimationClip.WrapMode.RandomLoop:
            int num1 = (int) this.clipTime % this.currentClip.frames.Length;
            this.SetFrameInternal(num1);
            if (num1 < previousFrame)
            {
              this.ProcessEvents(previousFrame, this.currentClip.frames.Length - 1, 1);
              this.ProcessEvents(-1, num1, 1);
              break;
            }
            this.ProcessEvents(previousFrame, num1, 1);
            break;
          case tk2dSpriteAnimationClip.WrapMode.LoopSection:
            int clipTime1 = (int) this.clipTime;
            int currFrame = this.currentClip.loopStart + (clipTime1 - this.currentClip.loopStart) % (this.currentClip.frames.Length - this.currentClip.loopStart);
            if (clipTime1 >= this.currentClip.loopStart)
            {
              this.SetFrameInternal(currFrame);
              int last = currFrame;
              if (previousFrame < this.currentClip.loopStart)
              {
                this.ProcessEvents(previousFrame, this.currentClip.loopStart - 1, 1);
                this.ProcessEvents(this.currentClip.loopStart - 1, last, 1);
                break;
              }
              if (last < previousFrame)
              {
                this.ProcessEvents(previousFrame, this.currentClip.frames.Length - 1, 1);
                this.ProcessEvents(this.currentClip.loopStart - 1, last, 1);
                break;
              }
              this.ProcessEvents(previousFrame, last, 1);
              break;
            }
            this.SetFrameInternal(clipTime1);
            this.ProcessEvents(previousFrame, clipTime1, 1);
            break;
          case tk2dSpriteAnimationClip.WrapMode.Once:
            int clipTime2 = (int) this.clipTime;
            if (clipTime2 >= this.currentClip.frames.Length)
            {
              this.SetFrameInternal(this.currentClip.frames.Length - 1);
              this.state &= ~tk2dSpriteAnimator.State.Playing;
              this.ProcessEvents(previousFrame, this.currentClip.frames.Length - 1, 1);
              this.OnAnimationCompleted();
              break;
            }
            this.SetFrameInternal(clipTime2);
            this.ProcessEvents(previousFrame, clipTime2, 1);
            break;
          case tk2dSpriteAnimationClip.WrapMode.PingPong:
            int num2 = this.currentClip.frames.Length <= 1 ? 0 : this.currentClip.frames.Length + this.currentClip.frames.Length - 2;
            int direction = 1;
            if (num2 >= this.currentClip.frames.Length)
            {
              num2 = 2 * this.currentClip.frames.Length - 2 - num2;
              direction = -1;
            }
            if (num2 < previousFrame)
              direction = -1;
            this.SetFrameInternal(num2);
            this.ProcessEvents(previousFrame, num2, direction);
            break;
          case tk2dSpriteAnimationClip.WrapMode.LoopFidget:
            int num3 = (int) this.clipTime % this.currentClip.frames.Length;
            this.SetFrameInternal(num3);
            if (num3 < previousFrame)
            {
              this.ProcessEvents(previousFrame, this.currentClip.frames.Length - 1, 1);
              this.ProcessEvents(-1, num3, 1);
              this.m_fidgetElapsed = 0.0f;
              this.m_fidgetDuration = Mathf.Lerp(this.currentClip.minFidgetDuration, this.currentClip.maxFidgetDuration, UnityEngine.Random.value);
              break;
            }
            this.ProcessEvents(previousFrame, num3, 1);
            break;
        }
      }

      private void ClipNameError(string name)
      {
        UnityEngine.Debug.LogError((object) $"Unable to find clip named '{name}' in library");
      }

      private void ClipIdError(int id)
      {
        UnityEngine.Debug.LogError((object) $"Play - Invalid clip id '{id.ToString()}' in library");
      }

      private void WarpClipToLocalTime(tk2dSpriteAnimationClip clip, float time, bool skipEvents)
      {
        this.clipTime = time;
        int index = (int) this.clipTime % clip.frames.Length;
        tk2dSpriteAnimationFrame frame = clip.frames[index];
        if (frame.requiresOffscreenUpdate)
          this.m_forceNextSpriteUpdate = true;
        this.SetSprite(frame.spriteCollection, frame.spriteId);
        if (frame.requiresOffscreenUpdate)
          this.m_forceNextSpriteUpdate = true;
        if (frame.triggerEvent && !skipEvents)
        {
          if (this.AnimationEventTriggered != null)
            this.AnimationEventTriggered(this, clip, index);
          if (!(bool) (UnityEngine.Object) this.aiActor && frame.eventOutline != tk2dSpriteAnimationFrame.OutlineModifier.Unspecified)
          {
            if (frame.eventOutline == tk2dSpriteAnimationFrame.OutlineModifier.TurnOn && !SpriteOutlineManager.HasOutline(this.sprite))
              SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black, 0.1f);
            if (frame.eventOutline == tk2dSpriteAnimationFrame.OutlineModifier.TurnOff && SpriteOutlineManager.HasOutline(this.sprite))
              SpriteOutlineManager.RemoveOutlineFromSprite(this.sprite);
          }
          if (!string.IsNullOrEmpty(frame.eventAudio) && !this.MuteAudio)
          {
            int num = (int) AkSoundEngine.PostEvent(frame.eventAudio, this.AudioBaseObject);
          }
          if (!string.IsNullOrEmpty(frame.eventVfx) && (bool) (UnityEngine.Object) this.aiAnimator)
            this.aiAnimator.PlayVfx(frame.eventVfx);
          if (!string.IsNullOrEmpty(frame.eventStopVfx) && (bool) (UnityEngine.Object) this.aiAnimator)
            this.aiAnimator.StopVfx(frame.eventStopVfx);
          if (frame.eventLerpEmissive)
            this.StartCoroutine(this.HandleEmissivePowerLerp(frame.eventLerpEmissiveTime, frame.eventLerpEmissivePower));
          if (frame.forceMaterialUpdate && (!(bool) (UnityEngine.Object) this.aiActor || !this.aiActor.IsBlackPhantom))
            this.Sprite.ForceUpdateMaterial();
        }
        this.previousFrame = index;
      }

      private void SetFrameInternal(int currFrame)
      {
        if (this.previousFrame != currFrame)
        {
          if (this.currentClip.frames[currFrame].requiresOffscreenUpdate)
            this.m_forceNextSpriteUpdate = true;
          this.SetSprite(this.currentClip.frames[currFrame].spriteCollection, this.currentClip.frames[currFrame].spriteId);
          if (this.currentClip.frames[currFrame].requiresOffscreenUpdate)
            this.m_forceNextSpriteUpdate = true;
          this.previousFrame = currFrame;
        }
        if (!this.IsFrameBlendedAnimation)
          return;
        this.sprite.renderer.material.SetFloat("_BlendFraction", this.clipTime % 1f);
      }

      private void ProcessEvents(int start, int last, int direction)
      {
        if (start == last || (double) Mathf.Sign((float) (last - start)) != (double) Mathf.Sign((float) direction))
          return;
        int num1 = last + direction;
        tk2dSpriteAnimationFrame[] frames = this.currentClip.frames;
        for (int currFrame = start + direction; currFrame != num1; currFrame += direction)
        {
          if (this.ForceSetEveryFrame)
            this.SetFrameInternal(currFrame);
          if (frames[currFrame].triggerEvent && this.AnimationEventTriggered != null)
            this.AnimationEventTriggered(this, this.currentClip, currFrame);
          if (frames[currFrame].triggerEvent && !string.IsNullOrEmpty(frames[currFrame].eventAudio) && !this.MuteAudio)
          {
            int num2 = (int) AkSoundEngine.PostEvent(frames[currFrame].eventAudio, this.AudioBaseObject);
          }
          if (!string.IsNullOrEmpty(frames[currFrame].eventVfx) && (bool) (UnityEngine.Object) this.aiAnimator)
            this.aiAnimator.PlayVfx(frames[currFrame].eventVfx);
          if (!string.IsNullOrEmpty(frames[currFrame].eventStopVfx) && (bool) (UnityEngine.Object) this.aiAnimator)
            this.aiAnimator.StopVfx(frames[currFrame].eventStopVfx);
          if (frames[currFrame].eventLerpEmissive)
            this.StartCoroutine(this.HandleEmissivePowerLerp(frames[currFrame].eventLerpEmissiveTime, frames[currFrame].eventLerpEmissivePower));
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleEmissivePowerLerp(float duration, float targetPower)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new tk2dSpriteAnimator.<HandleEmissivePowerLerp>c__Iterator2()
        {
          duration = duration,
          targetPower = targetPower,
          $this = this
        };
      }

      public GameObject AudioBaseObject
      {
        get
        {
          if ((UnityEngine.Object) this.m_cachedAudioBaseObject == (UnityEngine.Object) null)
            this.m_cachedAudioBaseObject = !(bool) (UnityEngine.Object) this.transform.parent || !(bool) (UnityEngine.Object) this.transform.parent.GetComponent<PlayerController>() ? this.gameObject : this.transform.parent.gameObject;
          return this.m_cachedAudioBaseObject;
        }
        set => this.m_cachedAudioBaseObject = value;
      }

      private void OnAnimationCompleted()
      {
        this.previousFrame = -1;
        if (this.AnimationCompleted == null)
          return;
        this.AnimationCompleted(this, this.currentClip);
      }

      private void HandleVisibilityCheck()
      {
        if (this.alwaysUpdateOffscreen)
          this.m_isCurrentlyVisible = true;
        else if (!tk2dSpriteAnimator.InDungeonScene)
        {
          this.m_isCurrentlyVisible = true;
        }
        else
        {
          Vector2 vector2 = this.transform.position.XY() - tk2dSpriteAnimator.CameraPositionThisFrame;
          vector2.y *= 1.7f;
          this.m_isCurrentlyVisible = (double) vector2.sqrMagnitude < 420.0 + (double) this.AdditionalCameraVisibilityRadius * (double) this.AdditionalCameraVisibilityRadius;
        }
      }

      private float GetDeltaTime()
      {
        float a = !this.AnimateDuringBossIntros || !GameManager.IsBossIntro ? BraveTime.DeltaTime : GameManager.INVARIANT_DELTA_TIME;
        if ((bool) (UnityEngine.Object) this.aiActor)
          a = this.aiActor.LocalDeltaTime;
        if ((double) this.OverrideTimeScale > 0.0)
          a *= this.OverrideTimeScale;
        if (this.ignoreTimeScale)
          a = GameManager.INVARIANT_DELTA_TIME;
        if (this.maximumDeltaOneFrame && this.CurrentClip != null)
          a = Mathf.Min(a, 1f / this.CurrentClip.fps);
        return a;
      }

      public virtual void LateUpdate()
      {
        if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH)
          this.HandleVisibilityCheck();
        this.deferNextStartClip = false;
        this.UpdateAnimation(this.GetDeltaTime());
      }

      public virtual void SetSprite(tk2dSpriteCollectionData spriteCollection, int spriteId)
      {
        bool flag = this.alwaysUpdateOffscreen;
        if (!this.alwaysUpdateOffscreen)
        {
          flag = this.renderer.isVisible;
          if (Application.isPlaying && GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH)
            flag |= this.m_isCurrentlyVisible;
        }
        if (this.alwaysUpdateOffscreen || !(this.Sprite is tk2dSprite) || flag || !Application.isPlaying || this.m_forceNextSpriteUpdate || this.m_hasAttachPoints)
        {
          this.Sprite.SetSprite(spriteCollection, spriteId);
          this.m_forceNextSpriteUpdate = false;
        }
        else
        {
          this.Sprite.hasOffScreenCachedUpdate = true;
          this.Sprite.offScreenCachedCollection = spriteCollection;
          this.Sprite.offScreenCachedID = spriteId;
        }
      }

      public void OnBecameVisible()
      {
        if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.Sprite || !this.Sprite.hasOffScreenCachedUpdate)
          return;
        this.Sprite.hasOffScreenCachedUpdate = false;
        this.Sprite.SetSprite(this.Sprite.offScreenCachedCollection, this.Sprite.offScreenCachedID);
        this.Sprite.UpdateZDepth();
      }

      public void ForceInvisibleSpriteUpdate()
      {
        if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.Sprite || !this.Sprite.hasOffScreenCachedUpdate)
          return;
        this.Sprite.hasOffScreenCachedUpdate = false;
        this.Sprite.SetSprite(this.Sprite.offScreenCachedCollection, this.Sprite.offScreenCachedID);
        this.Sprite.UpdateZDepth();
      }

      public Vector2[] GetNextFrameUVs()
      {
        if (this.state != tk2dSpriteAnimator.State.Playing)
          return this.Sprite.GetCurrentSpriteDef().uvs;
        int index = (this.CurrentFrame + 1) % this.currentClip.frames.Length;
        if (this.CurrentFrame + 1 >= this.currentClip.frames.Length && this.currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection)
          index = this.currentClip.loopStart;
        return this.currentClip.frames[index].spriteCollection.spriteDefinitions[this.currentClip.frames[index].spriteId].uvs;
      }

      private enum State
      {
        Init,
        Playing,
        Paused,
      }
    }

}
