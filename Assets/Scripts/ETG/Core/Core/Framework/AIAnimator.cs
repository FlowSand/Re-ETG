// Decompiled with JetBrains decompiler
// Type: AIAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class AIAnimator : BraveBehaviour
    {
      public AIAnimator ChildAnimator;
      public AIActor SpecifyAiActor;
      public AIAnimator.FacingType facingType;
      [ShowInInspectorIf("ShowDirectionParent", true)]
      public AIAnimator DirectionParent;
      [ShowInInspectorIf("ShowFaceSouthWhenStopped", true)]
      public bool faceSouthWhenStopped;
      [ShowInInspectorIf("ShowFaceSouthWhenStopped", true)]
      public bool faceTargetWhenStopped;
      [HideInInspector]
      public float AnimatedFacingDirection = -90f;
      public AIAnimator.DirectionalType directionalType = AIAnimator.DirectionalType.Sprite;
      [ShowInInspectorIf("ShowRotationOptions", true)]
      public float RotationQuantizeTo;
      [ShowInInspectorIf("ShowRotationOptions", true)]
      public float RotationOffset;
      public bool ForceKillVfxOnPreDeath;
      public bool SuppressAnimatorFallback;
      public bool IsBodySprite = true;
      [Header("Animations")]
      public DirectionalAnimation IdleAnimation;
      [FormerlySerializedAs("BaseAnimation")]
      public DirectionalAnimation MoveAnimation;
      public DirectionalAnimation FlightAnimation;
      public DirectionalAnimation HitAnimation;
      [ShowInInspectorIf("ShowHitAnimationOptions", true)]
      public float HitReactChance = 1f;
      [ShowInInspectorIf("ShowHitAnimationOptions", true)]
      public float MinTimeBetweenHitReacts;
      [ShowInInspectorIf("ShowHitAnimationOptions", true)]
      public AIAnimator.HitStateType HitType;
      public DirectionalAnimation TalkAnimation;
      public List<AIAnimator.NamedDirectionalAnimation> OtherAnimations;
      public List<AIAnimator.NamedVFXPool> OtherVFX;
      public List<AIAnimator.NamedScreenShake> OtherScreenShake;
      public List<DirectionalAnimation> IdleFidgetAnimations;
      private float m_facingDirection;
      private float m_cachedTurbo = -1f;
      public AIAnimator.PlayUntilFinishedDelegate OnPlayUntilFinished;
      private const float c_FIDGET_COOLDOWN = 2f;
      public AIAnimator.EndAnimationIfDelegate OnEndAnimationIf;
      public AIAnimator.PlayVfxDelegate OnPlayVfx;
      public AIAnimator.StopVfxDelegate OnStopVfx;
      private bool m_playingHitEffect;
      private bool m_hasPlayedAwaken;
      private tk2dSpriteAnimationClip m_currentBaseClip;
      private float m_currentBaseArtAngle;
      private DirectionalAnimation m_baseDirectionalAnimationOverride;
      private tk2dSpriteAnimationClip m_currentOverrideBaseClip;
      private AIAnimator.AnimatorState m_currentActionState;
      private float m_fidgetTimer;
      private float m_fidgetCooldown;
      private float m_suppressHitReactTimer;
      private float m_fpsScale = 1f;

      public bool UseAnimatedFacingDirection { get; set; }

      protected float LocalDeltaTime
      {
        get
        {
          if ((bool) (UnityEngine.Object) this.aiActor)
            return this.aiActor.LocalDeltaTime;
          return (bool) (UnityEngine.Object) this.behaviorSpeculator ? this.behaviorSpeculator.LocalDeltaTime : BraveTime.DeltaTime;
        }
      }

      private bool ShowDirectionParent() => this.facingType == AIAnimator.FacingType.SlaveDirection;

      private bool ShowFaceSouthWhenStopped() => this.facingType == AIAnimator.FacingType.Movement;

      private bool ShowRotationOptions() => this.directionalType != AIAnimator.DirectionalType.Sprite;

      private bool ShowHitAnimationOptions()
      {
        return this.HitAnimation.Type != DirectionalAnimation.DirectionType.None;
      }

      public bool SpriteFlipped
      {
        get
        {
          if (this.m_currentBaseClip == null)
            return false;
          return this.m_currentBaseClip.name == "move_back" || this.m_currentBaseClip.name == "move_left" || this.m_currentBaseClip.name == "move_forward_left" || this.m_currentBaseClip.name == "move_back_left";
        }
      }

      public bool SuppressHitStates { get; set; }

      public bool LockFacingDirection { get; set; }

      public float FacingDirection
      {
        get => this.m_facingDirection;
        set
        {
          if (float.IsNaN(value))
            return;
          this.m_facingDirection = value;
        }
      }

      public float FpsScale
      {
        get => this.m_fpsScale;
        set
        {
          if ((double) this.m_fpsScale != (double) value)
          {
            this.m_fpsScale = value;
            bool flag = this.m_currentActionState != null && (double) this.m_currentActionState.WarpClipDuration > 0.0;
            if (this.spriteAnimator.Playing && !flag)
            {
              float num = this.spriteAnimator.CurrentClip.fps * this.m_fpsScale;
              if ((double) num == 0.0)
                num = 1f / 1000f;
              this.spriteAnimator.ClipFps = num;
            }
          }
          if (!(bool) (UnityEngine.Object) this.ChildAnimator)
            return;
          this.ChildAnimator.FpsScale = value;
        }
      }

      public float CurrentClipLength
      {
        get
        {
          return (float) this.spriteAnimator.CurrentClip.frames.Length / this.spriteAnimator.CurrentClip.fps;
        }
      }

      public float CurrentClipProgress
      {
        get => Mathf.Clamp01(this.spriteAnimator.ClipTimeSeconds / this.CurrentClipLength);
      }

      public event System.Action OnSpawnCompleted;

      public string OverrideIdleAnimation { get; set; }

      public string OverrideMoveAnimation { get; set; }

      public float CurrentArtAngle
      {
        get
        {
          return this.m_currentActionState != null ? this.m_currentActionState.ArtAngle : this.FacingDirection;
        }
      }

      public void Awake()
      {
        this.spriteAnimator.playAutomatically = false;
        if (GameManager.Instance.InTutorial && this.name.Contains("turret", true))
        {
          this.FacingDirection = 180f;
          this.LockFacingDirection = true;
          this.specRigidbody.enabled = false;
        }
        if ((bool) (UnityEngine.Object) this.SpecifyAiActor)
        {
          this.aiActor = this.SpecifyAiActor;
          this.specRigidbody = this.aiActor.specRigidbody;
        }
        if (!this.ForceKillVfxOnPreDeath || !(bool) (UnityEngine.Object) this.healthHaver)
          return;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
      }

      public void Start()
      {
        this.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
        if (!(bool) (UnityEngine.Object) this.healthHaver || !(bool) (UnityEngine.Object) this.ChildAnimator || !(bool) (UnityEngine.Object) this.ChildAnimator.sprite || !this.ChildAnimator.IsBodySprite)
          return;
        this.healthHaver.RegisterBodySprite(this.ChildAnimator.sprite);
      }

      private void UpdateTurboSettings()
      {
        if ((double) this.m_cachedTurbo != (double) TurboModeController.sEnemyAnimSpeed && GameManager.IsTurboMode)
        {
          if ((double) this.m_cachedTurbo > 0.0)
          {
            this.FpsScale /= this.m_cachedTurbo;
            this.m_cachedTurbo = -1f;
          }
          this.FpsScale *= TurboModeController.sEnemyAnimSpeed;
          this.m_cachedTurbo = TurboModeController.sEnemyAnimSpeed;
        }
        else
        {
          if ((double) this.m_cachedTurbo <= 0.0 || GameManager.IsTurboMode)
            return;
          this.FpsScale /= this.m_cachedTurbo;
          this.m_cachedTurbo = -1f;
        }
      }

      public void Update()
      {
        this.m_suppressHitReactTimer = Mathf.Max(0.0f, this.m_suppressHitReactTimer - BraveTime.DeltaTime);
        this.UpdateTurboSettings();
        this.UpdateFacingDirection();
        this.UpdateCurrentBaseAnimation();
        if (this.m_currentActionState != null && this.m_currentActionState.DirectionalAnimation != null && this.m_currentActionState.AnimationClip == this.m_currentBaseClip)
          this.m_currentActionState = (AIAnimator.AnimatorState) null;
        if (this.m_currentActionState != null)
        {
          this.m_currentActionState.Update(this.spriteAnimator, this.FacingDirection);
          if (!this.m_currentActionState.HasStarted)
          {
            this.spriteAnimator.Stop();
            this.PlayClip(this.m_currentActionState.AnimationClip, this.m_currentActionState.WarpClipDuration);
            this.m_currentActionState.HasStarted = true;
            this.m_playingHitEffect = false;
          }
          else if (!this.m_playingHitEffect && this.spriteAnimator.CurrentClip != this.m_currentActionState.AnimationClip)
            this.spriteAnimator.Play(this.m_currentActionState.AnimationClip, this.spriteAnimator.ClipTimeSeconds, this.GetFps(this.m_currentActionState.AnimationClip, this.m_currentActionState.WarpClipDuration), true);
          if (this.m_currentActionState.EndType == AIAnimator.AnimatorState.StateEndType.Duration || this.m_currentActionState.EndType == AIAnimator.AnimatorState.StateEndType.DurationOrFinished)
          {
            this.m_currentActionState.Timer -= this.LocalDeltaTime;
            if ((double) this.m_currentActionState.Timer <= 0.0)
            {
              this.m_currentActionState = (AIAnimator.AnimatorState) null;
              this.m_playingHitEffect = false;
            }
          }
        }
        else if (!this.m_playingHitEffect && this.m_baseDirectionalAnimationOverride != null && !this.spriteAnimator.IsPlaying(this.m_currentOverrideBaseClip))
          this.PlayClip(this.m_currentOverrideBaseClip, -1f);
        else if (!this.m_playingHitEffect && this.m_baseDirectionalAnimationOverride == null && this.m_currentBaseClip != null && !this.spriteAnimator.IsPlaying(this.m_currentBaseClip))
          this.PlayClip(this.m_currentBaseClip, -1f);
        this.UpdateFacingRotation();
        for (int index = 0; index < this.OtherVFX.Count; ++index)
          this.OtherVFX[index].vfxPool.RemoveDespawnedVfx();
      }

      private string GetDebugString()
      {
        string debugString = $"{this.name}: {(this.m_currentActionState != null ? (object) this.m_currentActionState.Name : (object) "null")} ({(this.m_currentActionState != null ? (object) this.m_currentActionState.Timer.ToString() : (object) "null")}) - {this.spriteAnimator.CurrentClip.name} ({this.spriteAnimator.ClipTimeSeconds})";
        if ((bool) (UnityEngine.Object) this.ChildAnimator && this.ChildAnimator.IsBodySprite)
          debugString = $"{debugString} | {this.ChildAnimator.GetDebugString()}";
        return debugString;
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.spriteAnimator)
          this.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
        base.OnDestroy();
        for (int index = 0; index < this.OtherVFX.Count; ++index)
          this.OtherVFX[index].vfxPool.DestroyAll();
      }

      private void AnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
      {
        if (!this.enabled)
          return;
        if (this.m_playingHitEffect)
        {
          this.m_playingHitEffect = false;
          if (this.m_currentActionState != null && this.m_currentActionState.HasStarted)
            this.PlayClip(this.m_currentActionState.AnimationClip, this.m_currentActionState.WarpClipDuration);
          else
            this.PlayClip(this.m_currentBaseClip, -1f);
        }
        else
        {
          if (this.m_currentActionState == null || this.m_currentActionState.AnimationClip != clip || this.m_currentActionState.EndType == AIAnimator.AnimatorState.StateEndType.UntilCancelled)
            return;
          this.m_currentActionState = (AIAnimator.AnimatorState) null;
        }
      }

      public bool PlayUntilCancelled(
        string name,
        bool suppressHitStates = false,
        string overrideHitState = null,
        float warpClipDuration = -1f,
        bool skipChildAnimators = false)
      {
        bool flag = false;
        if (!skipChildAnimators && (bool) (UnityEngine.Object) this.ChildAnimator)
          flag = this.ChildAnimator.PlayUntilCancelled(name, suppressHitStates, overrideHitState);
        return this.HasDirectionalAnimation(name) ? this.Play(name, this.GetDirectionalAnimation(name), AIAnimator.AnimatorState.StateEndType.UntilCancelled, 0.0f, warpClipDuration, suppressHitStates, overrideHitState) || flag : this.Play(name, AIAnimator.AnimatorState.StateEndType.UntilCancelled, 0.0f, warpClipDuration, suppressHitStates, overrideHitState) || flag;
      }

      public bool PlayUntilFinished(
        string name,
        bool suppressHitStates = false,
        string overrideHitState = null,
        float warpClipDuration = -1f,
        bool skipChildAnimators = false)
      {
        if (this.OnPlayUntilFinished != null)
          this.OnPlayUntilFinished(name, suppressHitStates, overrideHitState, warpClipDuration, skipChildAnimators);
        bool flag = false;
        if (!skipChildAnimators && (bool) (UnityEngine.Object) this.ChildAnimator)
          flag = this.ChildAnimator.PlayUntilFinished(name, suppressHitStates, overrideHitState, warpClipDuration);
        return this.HasDirectionalAnimation(name) ? this.Play(name, this.GetDirectionalAnimation(name), AIAnimator.AnimatorState.StateEndType.UntilFinished, 0.0f, warpClipDuration, suppressHitStates, overrideHitState) || flag : this.Play(name, AIAnimator.AnimatorState.StateEndType.UntilFinished, 0.0f, warpClipDuration, suppressHitStates, overrideHitState) || flag;
      }

      public bool PlayForDuration(
        string name,
        float duration,
        bool suppressHitStates = false,
        string overrideHitState = null,
        float warpClipDuration = -1f,
        bool skipChildAnimators = false)
      {
        bool flag = false;
        if (!skipChildAnimators && (bool) (UnityEngine.Object) this.ChildAnimator)
          flag = this.ChildAnimator.PlayForDuration(name, duration, suppressHitStates, overrideHitState);
        return this.HasDirectionalAnimation(name) ? this.Play(name, this.GetDirectionalAnimation(name), AIAnimator.AnimatorState.StateEndType.Duration, duration, warpClipDuration, suppressHitStates, overrideHitState) || flag : this.Play(name, AIAnimator.AnimatorState.StateEndType.Duration, duration, warpClipDuration, suppressHitStates, overrideHitState) || flag;
      }

      public bool PlayForDurationOrUntilFinished(
        string name,
        float duration,
        bool suppressHitStates = false,
        string overrideHitState = null,
        float warpClipDuration = -1f,
        bool skipChildAnimators = false)
      {
        bool flag = false;
        if (!skipChildAnimators && (bool) (UnityEngine.Object) this.ChildAnimator)
          flag = this.ChildAnimator.PlayForDurationOrUntilFinished(name, duration, suppressHitStates, overrideHitState);
        return this.HasDirectionalAnimation(name) ? this.Play(name, this.GetDirectionalAnimation(name), AIAnimator.AnimatorState.StateEndType.DurationOrFinished, duration, warpClipDuration, suppressHitStates, overrideHitState) || flag : this.Play(name, AIAnimator.AnimatorState.StateEndType.DurationOrFinished, duration, warpClipDuration, suppressHitStates, overrideHitState) || flag;
      }

      private bool Play(
        string name,
        AIAnimator.AnimatorState.StateEndType endType,
        float duration,
        float warpClipDuration,
        bool suppressHitStates,
        string overrideHitState)
      {
        if (this.SuppressAnimatorFallback)
          return false;
        return this.Play(new AIAnimator.AnimatorState()
        {
          Name = name,
          AnimationClip = this.spriteAnimator.GetClipByName(name),
          EndType = endType,
          Timer = duration,
          WarpClipDuration = warpClipDuration,
          SuppressHitStates = suppressHitStates,
          OverrideHitStateName = overrideHitState
        });
      }

      private bool Play(
        string name,
        DirectionalAnimation directionalAnimation,
        AIAnimator.AnimatorState.StateEndType endType,
        float duration,
        float warpClipDuration,
        bool suppressHitStates,
        string overrideHitState)
      {
        if (directionalAnimation.Type == DirectionalAnimation.DirectionType.None)
          return false;
        return this.Play(new AIAnimator.AnimatorState()
        {
          Name = name,
          DirectionalAnimation = directionalAnimation,
          AnimationClip = this.spriteAnimator.GetClipByName(directionalAnimation.GetInfo(this.FacingDirection, true).name),
          EndType = endType,
          Timer = duration,
          WarpClipDuration = warpClipDuration,
          SuppressHitStates = suppressHitStates,
          OverrideHitStateName = overrideHitState
        });
      }

      private bool Play(AIAnimator.AnimatorState state)
      {
        if (state.DirectionalAnimation != null && state.DirectionalAnimation.Type == DirectionalAnimation.DirectionType.None || state.AnimationClip == null)
          return false;
        this.m_currentActionState = state;
        this.spriteAnimator.Stop();
        this.PlayClip(this.m_currentActionState.AnimationClip, state.WarpClipDuration);
        this.m_currentActionState.HasStarted = true;
        this.m_playingHitEffect = false;
        return true;
      }

      public void SetBaseAnim(string name, bool useFidgetTimer = false)
      {
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          this.ChildAnimator.SetBaseAnim(name);
        if (!this.HasDirectionalAnimation(name))
          return;
        this.m_baseDirectionalAnimationOverride = this.GetDirectionalAnimation(name);
        if (!useFidgetTimer)
          return;
        tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(this.m_baseDirectionalAnimationOverride.GetInfo(this.FacingDirection).name);
        this.m_currentOverrideBaseClip = clipByName;
        this.m_fidgetCooldown = 2f;
        this.m_fidgetTimer = clipByName.BaseClipLength;
      }

      public void ClearBaseAnim()
      {
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          this.ChildAnimator.ClearBaseAnim();
        this.m_baseDirectionalAnimationOverride = (DirectionalAnimation) null;
      }

      public bool IsIdle() => this.m_currentActionState == null;

      public bool IsPlaying(string animName)
      {
        if (this.m_currentActionState != null && this.m_currentActionState.Name == animName && this.spriteAnimator.Playing || this.spriteAnimator.IsPlaying(animName))
          return true;
        return (bool) (UnityEngine.Object) this.ChildAnimator && this.ChildAnimator.IsPlaying(animName);
      }

      public bool GetWrapType(string animName, out tk2dSpriteAnimationClip.WrapMode wrapMode)
      {
        DirectionalAnimation directionalAnimation = this.GetDirectionalAnimation(animName);
        if (directionalAnimation != null)
        {
          tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(directionalAnimation.GetInfo(0).name);
          if (clipByName != null)
          {
            wrapMode = clipByName.wrapMode;
            return true;
          }
        }
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          return this.ChildAnimator.GetWrapType(animName, out wrapMode);
        wrapMode = tk2dSpriteAnimationClip.WrapMode.Single;
        return false;
      }

      public bool EndAnimation()
      {
        bool flag = false;
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          flag = this.ChildAnimator.EndAnimation();
        if (this.m_currentActionState == null)
          return flag;
        this.m_currentActionState = (AIAnimator.AnimatorState) null;
        return true;
      }

      public bool EndAnimationIf(string name)
      {
        if (this.OnEndAnimationIf != null)
          this.OnEndAnimationIf(name);
        bool flag = false;
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          flag = this.ChildAnimator.EndAnimationIf(name);
        if (this.m_currentActionState == null || !(this.m_currentActionState.Name == name))
          return flag;
        this.m_currentActionState = (AIAnimator.AnimatorState) null;
        return true;
      }

      public string PlayDefaultSpawnState() => this.PlayDefaultSpawnState(out bool _);

      public string PlayDefaultSpawnState(out bool isPlayingAwaken)
      {
        isPlayingAwaken = false;
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          this.ChildAnimator.PlayDefaultSpawnState();
        if (!this.enabled || this.m_hasPlayedAwaken)
          return (string) null;
        tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName("spawn");
        string str;
        if (clipByName != null)
        {
          if (clipByName.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop || clipByName.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection || clipByName.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopFidget)
            this.PlayForDuration(clipByName.name, 2f, true);
          else
            this.PlayUntilFinished(clipByName.name, true);
          str = clipByName.name;
        }
        else
        {
          str = this.PlayDefaultAwakenedState();
          isPlayingAwaken = true;
        }
        this.m_hasPlayedAwaken = true;
        return str;
      }

      public string PlayDefaultAwakenedState()
      {
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          this.ChildAnimator.PlayDefaultAwakenedState();
        if (!this.enabled || this.m_hasPlayedAwaken)
          return (string) null;
        this.m_hasPlayedAwaken = true;
        if (this.HasDirectionalAnimation("awaken"))
        {
          DirectionalAnimation directionalAnimation = this.GetDirectionalAnimation("awaken");
          if (directionalAnimation.Type == DirectionalAnimation.DirectionType.None)
            return (string) null;
          tk2dSpriteAnimationClip clipByName = this.spriteAnimator.GetClipByName(directionalAnimation.GetInfo(this.FacingDirection).name);
          if (clipByName.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop || clipByName.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection || clipByName.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopFidget)
            this.PlayForDuration("awaken", 2f, true);
          else
            this.PlayUntilFinished("awaken", true);
          return "awaken";
        }
        tk2dSpriteAnimationClip spriteAnimationClip = this.spriteAnimator.GetClipByName((double) this.FacingDirection < 90.0 || (double) this.FacingDirection > 1270.0 ? "awaken_right" : "awaken_left") ?? this.spriteAnimator.GetClipByName("awaken");
        if (spriteAnimationClip == null)
          return (string) null;
        if (spriteAnimationClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop || spriteAnimationClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection || spriteAnimationClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopFidget)
          this.PlayForDuration(spriteAnimationClip.name, 2f, true);
        else
          this.PlayUntilFinished(spriteAnimationClip.name, true);
        return spriteAnimationClip.name;
      }

      public void PlayHitState(Vector2 damageVector)
      {
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          this.ChildAnimator.PlayHitState(damageVector);
        if (!this.enabled || this.SuppressHitStates || (double) this.HitReactChance < 1.0 && (double) UnityEngine.Random.value > (double) this.HitReactChance || (double) this.m_suppressHitReactTimer > 0.0 || (double) this.FpsScale == 0.0)
          return;
        DirectionalAnimation directionalAnimation = this.HitAnimation;
        if (this.m_currentActionState != null)
        {
          if (this.m_currentActionState.SuppressHitStates)
            return;
          string overrideHitStateName = this.m_currentActionState.OverrideHitStateName;
          if (!string.IsNullOrEmpty(overrideHitStateName))
          {
            if (this.HasDirectionalAnimation(overrideHitStateName))
              directionalAnimation = this.GetDirectionalAnimation(overrideHitStateName);
            else
              Debug.LogWarning((object) ("No override animation found with name " + overrideHitStateName));
          }
        }
        if (directionalAnimation.Type != DirectionalAnimation.DirectionType.None)
        {
          if (this.HitType == AIAnimator.HitStateType.Basic)
          {
            Vector2 dir = !(damageVector == Vector2.zero) ? damageVector : this.specRigidbody.Velocity;
            this.PlayClip(directionalAnimation.GetInfo(dir).name, -1f);
            this.m_playingHitEffect = true;
          }
          else
          {
            this.PlayClip(directionalAnimation.GetInfo(this.FacingDirection).name, -1f);
            this.m_playingHitEffect = true;
          }
        }
        if ((double) this.MinTimeBetweenHitReacts <= 0.0)
          return;
        this.m_suppressHitReactTimer = this.MinTimeBetweenHitReacts;
      }

      public bool HasDefaultAnimation
      {
        get
        {
          return this.MoveAnimation.Type != DirectionalAnimation.DirectionType.None || this.IdleAnimation.Type != DirectionalAnimation.DirectionType.None || this.FlightAnimation.Type != DirectionalAnimation.DirectionType.None;
        }
      }

      public bool HasDirectionalAnimation(string animName)
      {
        if (string.IsNullOrEmpty(animName))
          return false;
        if (animName.Equals("idle", StringComparison.OrdinalIgnoreCase))
          return !string.IsNullOrEmpty(this.OverrideIdleAnimation) || this.IdleAnimation.Type != DirectionalAnimation.DirectionType.None;
        if (animName.Equals("move", StringComparison.OrdinalIgnoreCase))
          return !string.IsNullOrEmpty(this.OverrideMoveAnimation) || this.MoveAnimation.Type != DirectionalAnimation.DirectionType.None;
        if (animName.Equals("talk", StringComparison.OrdinalIgnoreCase))
          return this.TalkAnimation.Type != DirectionalAnimation.DirectionType.None;
        if (animName.Equals("hit", StringComparison.OrdinalIgnoreCase))
          return this.HitAnimation.Type != DirectionalAnimation.DirectionType.None;
        if (animName.Equals("flight", StringComparison.OrdinalIgnoreCase))
          return this.FlightAnimation.Type != DirectionalAnimation.DirectionType.None;
        for (int index = 0; index < this.OtherAnimations.Count; ++index)
        {
          if (animName.Equals(this.OtherAnimations[index].name, StringComparison.OrdinalIgnoreCase))
            return true;
        }
        return false;
      }

      public float GetDirectionalAnimationLength(string animName)
      {
        DirectionalAnimation directionalAnimation = this.GetDirectionalAnimation(animName);
        return directionalAnimation == null ? 0.0f : this.spriteAnimator.GetClipByName(directionalAnimation.GetInfo(-Vector2.up).name).BaseClipLength;
      }

      public void CopyStateFrom(AIAnimator other)
      {
        this.sprite.SetSprite(other.sprite.spriteId);
        this.spriteAnimator.PlayFrom(other.spriteAnimator.CurrentClip, other.spriteAnimator.clipTime);
        this.m_currentActionState = other.m_currentActionState != null ? new AIAnimator.AnimatorState(other.m_currentActionState) : (AIAnimator.AnimatorState) null;
        this.m_playingHitEffect = other.m_playingHitEffect;
        this.m_hasPlayedAwaken = other.m_hasPlayedAwaken;
        this.m_currentBaseClip = other.m_currentBaseClip;
        this.m_currentBaseArtAngle = other.m_currentBaseArtAngle;
        this.m_baseDirectionalAnimationOverride = other.m_baseDirectionalAnimationOverride;
        this.m_currentOverrideBaseClip = other.m_currentOverrideBaseClip;
        this.m_currentActionState = other.m_currentActionState;
        this.m_fidgetTimer = other.m_fidgetTimer;
        this.m_fidgetCooldown = other.m_fidgetCooldown;
        this.m_suppressHitReactTimer = other.m_suppressHitReactTimer;
        this.m_fpsScale = other.m_fpsScale;
      }

      public void UpdateAnimation(float deltaTime)
      {
        for (AIAnimator aiAnimator = this; (bool) (UnityEngine.Object) aiAnimator; aiAnimator = aiAnimator.ChildAnimator)
        {
          if ((bool) (UnityEngine.Object) aiAnimator.spriteAnimator)
            aiAnimator.spriteAnimator.UpdateAnimation(deltaTime);
        }
      }

      public void PlayVfx(
        string name,
        Vector2? sourceNormal = null,
        Vector2? sourceVelocity = null,
        Vector2? position = null)
      {
        if (this.OnPlayVfx != null)
          this.OnPlayVfx(name, sourceNormal, sourceVelocity, position);
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          this.ChildAnimator.PlayVfx(name);
        for (int index = 0; index < this.OtherVFX.Count; ++index)
        {
          AIAnimator.NamedVFXPool namedVfxPool = this.OtherVFX[index];
          if (namedVfxPool.name == name)
          {
            if (position.HasValue)
              namedVfxPool.vfxPool.SpawnAtPosition((Vector3) position.Value, parent: this.transform, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, keepReferences: true);
            else if ((bool) (UnityEngine.Object) namedVfxPool.anchorTransform)
            {
              if (!sourceVelocity.HasValue)
                sourceVelocity = new Vector2?(new Vector2(1f, 0.0f).Rotate(namedVfxPool.anchorTransform.eulerAngles.z + 180f));
              namedVfxPool.vfxPool.SpawnAtLocalPosition(Vector3.zero, 0.0f, namedVfxPool.anchorTransform, sourceNormal, sourceVelocity, true);
            }
            else
              namedVfxPool.vfxPool.SpawnAtPosition((Vector3) this.specRigidbody.UnitCenter, parent: this.transform, sourceNormal: sourceNormal, sourceVelocity: sourceVelocity, keepReferences: true);
          }
        }
        for (int index = 0; index < this.OtherScreenShake.Count; ++index)
        {
          AIAnimator.NamedScreenShake namedScreenShake = this.OtherScreenShake[index];
          if (namedScreenShake.name == name)
          {
            Vector2 vector2 = !(bool) (UnityEngine.Object) this.specRigidbody ? this.sprite.WorldCenter : this.specRigidbody.UnitCenter;
            GameManager.Instance.MainCameraController.DoScreenShake(namedScreenShake.screenShake, new Vector2?(vector2));
          }
        }
      }

      public void StopVfx(string name)
      {
        if (this.OnStopVfx != null)
          this.OnStopVfx(name);
        if ((bool) (UnityEngine.Object) this.ChildAnimator)
          this.ChildAnimator.StopVfx(name);
        for (int index = 0; index < this.OtherVFX.Count; ++index)
        {
          AIAnimator.NamedVFXPool namedVfxPool = this.OtherVFX[index];
          if (namedVfxPool.name == name)
            namedVfxPool.vfxPool.DestroyAll();
        }
      }

      private void OnPreDeath(Vector2 deathDirection)
      {
        if (!this.ForceKillVfxOnPreDeath)
          return;
        for (int index = 0; index < this.OtherVFX.Count; ++index)
          this.OtherVFX[index].vfxPool.DestroyAll();
      }

      private void UpdateFacingDirection()
      {
        if (this.LockFacingDirection)
          return;
        if (this.UseAnimatedFacingDirection)
          this.FacingDirection = this.AnimatedFacingDirection;
        else if (this.facingType == AIAnimator.FacingType.SlaveDirection)
          this.FacingDirection = this.DirectionParent.FacingDirection;
        else if (this.facingType == AIAnimator.FacingType.Movement)
        {
          if ((bool) (UnityEngine.Object) this.aiActor)
          {
            if (this.aiActor.VoluntaryMovementVelocity != Vector2.zero)
              this.FacingDirection = this.aiActor.VoluntaryMovementVelocity.ToAngle();
            else if (this.faceSouthWhenStopped)
            {
              this.FacingDirection = -90f;
            }
            else
            {
              if (!this.faceTargetWhenStopped || !(bool) (UnityEngine.Object) this.aiActor || !(bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
                return;
              this.FacingDirection = BraveMathCollege.Atan2Degrees(this.aiActor.TargetRigidbody.UnitCenter - this.specRigidbody.UnitCenter);
            }
          }
          else
          {
            if (!(this.specRigidbody.Velocity != Vector2.zero))
              return;
            this.FacingDirection = this.specRigidbody.Velocity.ToAngle();
          }
        }
        else if (this.facingType == AIAnimator.FacingType.Target)
        {
          if (!(bool) (UnityEngine.Object) this.aiActor || !(bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
            return;
          this.FacingDirection = BraveMathCollege.Atan2Degrees(this.aiActor.TargetRigidbody.UnitCenter - this.specRigidbody.UnitCenter);
        }
        else if ((bool) (UnityEngine.Object) this.talkDoer)
        {
          if ((double) Mathf.Abs(this.specRigidbody.Velocity.x) > 9.9999997473787516E-05 || (double) Mathf.Abs(this.specRigidbody.Velocity.y) > 9.9999997473787516E-05)
          {
            this.FacingDirection = BraveMathCollege.Atan2Degrees(this.specRigidbody.Velocity);
          }
          else
          {
            PlayerController playerClosestToPoint = GameManager.Instance.GetActivePlayerClosestToPoint(this.specRigidbody.UnitCenter);
            if ((UnityEngine.Object) playerClosestToPoint != (UnityEngine.Object) null)
            {
              this.FacingDirection = BraveMathCollege.Atan2Degrees(playerClosestToPoint.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter);
            }
            else
            {
              if (!((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null))
                return;
              this.FacingDirection = BraveMathCollege.Atan2Degrees(GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter - this.specRigidbody.UnitCenter);
            }
          }
        }
        else if ((bool) (UnityEngine.Object) this.aiShooter && ((UnityEngine.Object) this.aiShooter.CurrentGun != (UnityEngine.Object) null || this.aiShooter.ManualGunAngle))
          this.FacingDirection = this.aiShooter.GunAngle;
        else if ((bool) (UnityEngine.Object) this.aiActor && (bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
        {
          this.FacingDirection = BraveMathCollege.Atan2Degrees(this.aiActor.TargetRigidbody.UnitCenter - this.specRigidbody.UnitCenter);
        }
        else
        {
          if (!(bool) (UnityEngine.Object) this.specRigidbody || !(this.specRigidbody.Velocity != Vector2.zero))
            return;
          this.FacingDirection = BraveMathCollege.Atan2Degrees(this.specRigidbody.Velocity);
        }
      }

      private void UpdateCurrentBaseAnimation()
      {
        bool flag1 = (double) this.m_fidgetTimer > 0.0;
        this.m_fidgetTimer -= this.LocalDeltaTime;
        if (flag1 && (double) this.m_fidgetTimer <= 0.0)
          this.ClearBaseAnim();
        if ((double) this.m_fidgetTimer <= 0.0 && (double) this.m_fidgetCooldown > 0.0)
          this.m_fidgetCooldown -= this.LocalDeltaTime;
        bool flag2 = this.FlightAnimation.Type != DirectionalAnimation.DirectionType.None;
        bool flag3 = !string.IsNullOrEmpty(this.OverrideMoveAnimation) || this.MoveAnimation.Type != DirectionalAnimation.DirectionType.None;
        bool flag4 = !string.IsNullOrEmpty(this.OverrideIdleAnimation) || this.IdleAnimation.Type != DirectionalAnimation.DirectionType.None;
        DirectionalAnimation directionalAnimation = (DirectionalAnimation) null;
        if (this.m_baseDirectionalAnimationOverride != null)
          this.m_currentOverrideBaseClip = this.spriteAnimator.GetClipByName(this.m_baseDirectionalAnimationOverride.GetInfo(this.FacingDirection, true).name);
        if ((bool) (UnityEngine.Object) this.aiActor && flag2 && this.aiActor.IsFlying && this.aiActor.IsOverPit)
          directionalAnimation = this.FlightAnimation;
        else if (flag3 && (bool) (UnityEngine.Object) this.specRigidbody && this.specRigidbody.Velocity != Vector2.zero)
          directionalAnimation = string.IsNullOrEmpty(this.OverrideMoveAnimation) ? this.MoveAnimation : this.GetDirectionalAnimation(this.OverrideMoveAnimation);
        else if (flag4)
        {
          directionalAnimation = string.IsNullOrEmpty(this.OverrideIdleAnimation) ? this.IdleAnimation : this.GetDirectionalAnimation(this.OverrideIdleAnimation);
          if (this.IdleFidgetAnimations.Count > 0 && (double) this.m_fidgetTimer <= 0.0 && (double) this.m_fidgetCooldown <= 0.0 && (double) UnityEngine.Random.value < (double) BraveMathCollege.SliceProbability(0.2f, this.LocalDeltaTime))
            this.SetBaseAnim(this.IdleFidgetAnimations[0].GetInfo(this.FacingDirection, true).name, true);
        }
        if (directionalAnimation == null && flag3)
          directionalAnimation = this.MoveAnimation;
        if (directionalAnimation == null || !(bool) (UnityEngine.Object) this.spriteAnimator)
          return;
        DirectionalAnimation.Info info = directionalAnimation.GetInfo(this.FacingDirection, true);
        if (info == null)
          return;
        this.m_currentBaseClip = this.spriteAnimator.GetClipByName(info.name);
        this.m_currentBaseArtAngle = info.artAngle;
      }

      private DirectionalAnimation GetDirectionalAnimation(string animName)
      {
        if (string.IsNullOrEmpty(animName))
          return (DirectionalAnimation) null;
        if (animName.Equals("idle", StringComparison.OrdinalIgnoreCase))
          return this.IdleAnimation;
        if (animName.Equals("move", StringComparison.OrdinalIgnoreCase))
          return this.MoveAnimation;
        if (animName.Equals("talk", StringComparison.OrdinalIgnoreCase))
          return this.TalkAnimation;
        if (animName.Equals("hit", StringComparison.OrdinalIgnoreCase))
          return this.HitAnimation;
        if (animName.Equals("flight", StringComparison.OrdinalIgnoreCase))
          return this.FlightAnimation;
        DirectionalAnimation directionalAnimation = (DirectionalAnimation) null;
        int max = 0;
        for (int index = 0; index < this.OtherAnimations.Count; ++index)
        {
          if (animName.Equals(this.OtherAnimations[index].name, StringComparison.OrdinalIgnoreCase))
          {
            ++max;
            directionalAnimation = this.OtherAnimations[index].anim;
          }
        }
        if (max == 0)
          return (DirectionalAnimation) null;
        if (max == 1)
          return directionalAnimation;
        int num1 = UnityEngine.Random.Range(0, max);
        int num2 = 0;
        for (int index = 0; index < this.OtherAnimations.Count; ++index)
        {
          if (animName.Equals(this.OtherAnimations[index].name, StringComparison.OrdinalIgnoreCase))
          {
            if (num2 == num1)
              return this.OtherAnimations[index].anim;
            ++num2;
          }
        }
        Debug.LogError((object) "GetDiretionalAnimation: THIS SHOULDN'T HAPPEN");
        return (DirectionalAnimation) null;
      }

      private void PlayClip(string clipName, float warpClipDuration)
      {
        this.PlayClip(this.spriteAnimator.GetClipByName(clipName), warpClipDuration);
      }

      private void PlayClip(tk2dSpriteAnimationClip clip, float warpClipDuration)
      {
        this.spriteAnimator.Play(clip, 0.0f, this.GetFps(clip, warpClipDuration));
        this.UpdateFacingRotation();
      }

      private float GetFps(tk2dSpriteAnimationClip clip, float warpClipDuration = -1f)
      {
        if ((double) warpClipDuration > 0.0)
          return (float) clip.frames.Length / warpClipDuration;
        if ((double) this.m_fpsScale == 1.0)
          return clip.fps;
        return (double) this.m_fpsScale > 0.0 ? clip.fps * this.m_fpsScale : 1E-05f;
      }

      private void UpdateFacingRotation()
      {
        if (this.directionalType == AIAnimator.DirectionalType.Sprite)
          return;
        float num = this.FacingDirection + this.RotationOffset;
        if (this.directionalType == AIAnimator.DirectionalType.SpriteAndRotation)
          num -= this.m_currentActionState == null ? this.m_currentBaseArtAngle : this.m_currentActionState.ArtAngle;
        if ((double) this.RotationQuantizeTo != 0.0)
          num = BraveMathCollege.QuantizeFloat(num, this.RotationQuantizeTo);
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num);
        this.sprite.UpdateZDepth();
        this.sprite.ForceBuild();
      }

      public enum FacingType
      {
        Default,
        Target,
        Movement,
        SlaveDirection,
      }

      public enum DirectionalType
      {
        Sprite = 10, // 0x0000000A
        Rotation = 20, // 0x00000014
        SpriteAndRotation = 30, // 0x0000001E
      }

      public delegate void PlayUntilFinishedDelegate(
        string name,
        bool suppressHitStates = false,
        string overrideHitState = null,
        float warpClipDuration = -1f,
        bool skipChildAnimators = false);

      public delegate void EndAnimationIfDelegate(string name);

      public delegate void PlayVfxDelegate(
        string name,
        Vector2? sourceNormal,
        Vector2? sourceVelocity,
        Vector2? position);

      public delegate void StopVfxDelegate(string name);

      private class AnimatorState
      {
        public string Name;
        public DirectionalAnimation DirectionalAnimation;
        public tk2dSpriteAnimationClip AnimationClip;
        public AIAnimator.AnimatorState.StateEndType EndType;
        public float Timer;
        public float WarpClipDuration;
        public float ArtAngle;
        public bool SuppressHitStates;
        public string OverrideHitStateName;
        public bool HasStarted;

        public AnimatorState()
        {
        }

        public AnimatorState(AIAnimator.AnimatorState other)
        {
          this.Name = other.Name;
          this.DirectionalAnimation = other.DirectionalAnimation;
          this.AnimationClip = other.AnimationClip;
          this.EndType = other.EndType;
          this.Timer = other.Timer;
          this.WarpClipDuration = other.WarpClipDuration;
          this.ArtAngle = other.ArtAngle;
          this.SuppressHitStates = other.SuppressHitStates;
          this.OverrideHitStateName = other.OverrideHitStateName;
          this.HasStarted = other.HasStarted;
        }

        public void Update(tk2dSpriteAnimator spriteAnimator, float facingDirection)
        {
          if (this.DirectionalAnimation == null)
            return;
          DirectionalAnimation.Info info = this.DirectionalAnimation.GetInfo(facingDirection, true);
          if (info == null)
            return;
          this.AnimationClip = spriteAnimator.GetClipByName(info.name);
          this.ArtAngle = info.artAngle;
        }

        public enum StateEndType
        {
          UntilCancelled,
          UntilFinished,
          Duration,
          DurationOrFinished,
        }
      }

      public enum HitStateType
      {
        Basic,
        FacingDirection,
      }

      [Serializable]
      public class NamedDirectionalAnimation
      {
        public string name;
        public DirectionalAnimation anim;
      }

      [Serializable]
      public class NamedVFXPool
      {
        public string name;
        public Transform anchorTransform;
        public VFXPool vfxPool;
      }

      [Serializable]
      public class NamedScreenShake
      {
        public string name;
        public ScreenShakeSettings screenShake;
      }
    }

}
