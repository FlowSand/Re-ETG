// Decompiled with JetBrains decompiler
// Type: dfSpriteAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [AddComponentMenu("Daikon Forge/Tweens/Sprite Animator")]
    [Serializable]
    public class dfSpriteAnimation : dfTweenPlayableBase
    {
      [SerializeField]
      private string animationName = "ANIMATION";
      [SerializeField]
      private dfAnimationClip clip;
      [SerializeField]
      public dfAnimationClip alternativeLoopClip;
      [SerializeField]
      public float percentChanceToPlayAlternative = 0.05f;
      private dfAnimationClip m_cachedBaseClip;
      [SerializeField]
      private dfComponentMemberInfo memberInfo = new dfComponentMemberInfo();
      [SerializeField]
      private dfTweenLoopType loopType = dfTweenLoopType.Loop;
      [SerializeField]
      public int LoopSectionFrameTarget = 7;
      [SerializeField]
      public float LoopSectionFirstLength = -1f;
      [SerializeField]
      public float LoopDelayMin;
      [SerializeField]
      public float LoopDelayMax;
      [SerializeField]
      public bool maxOneFrameDelta;
      [SerializeField]
      private float length = 1f;
      [SerializeField]
      private bool autoStart;
      [SerializeField]
      private bool skipToEndOnStop;
      [SerializeField]
      private dfPlayDirection playDirection;
      private bool autoRunStarted;
      private bool isRunning;
      private bool isPaused;
      private dfControl m_selfControl;
      public bool UseDefaultSpriteNameProperty = true;
      private System.Random myRandom;
      private float m_elapsedSinceLoop;
      private float m_lastRealtime;

      public event TweenNotification AnimationStarted;

      public event TweenNotification AnimationStopped;

      public event TweenNotification AnimationPaused;

      public event TweenNotification AnimationResumed;

      public event TweenNotification AnimationReset;

      public event TweenNotification AnimationCompleted;

      public dfAnimationClip Clip
      {
        get => this.clip;
        set => this.clip = value;
      }

      public dfComponentMemberInfo Target
      {
        get => this.memberInfo;
        set => this.memberInfo = value;
      }

      public bool AutoRun
      {
        get => this.autoStart;
        set => this.autoStart = value;
      }

      public float Length
      {
        get => this.length;
        set => this.length = Mathf.Max(value, 0.03f);
      }

      public dfTweenLoopType LoopType
      {
        get => this.loopType;
        set => this.loopType = value;
      }

      public dfPlayDirection Direction
      {
        get => this.playDirection;
        set
        {
          this.playDirection = value;
          if (!this.IsPlaying)
            return;
          this.Play();
        }
      }

      public bool IsPaused
      {
        get => this.isRunning && this.isPaused;
        set
        {
          if (value == this.IsPaused)
            return;
          if (value)
            this.Pause();
          else
            this.Resume();
        }
      }

      public void Awake()
      {
      }

      public void Start()
      {
        this.m_lastRealtime = UnityEngine.Time.realtimeSinceStartup;
        this.m_selfControl = this.GetComponent<dfControl>();
        this.m_cachedBaseClip = this.clip;
      }

      public void LateUpdate()
      {
        if (!this.AutoRun || this.IsPlaying || this.autoRunStarted)
          return;
        this.autoRunStarted = true;
        this.Play();
      }

      public void PlayForward()
      {
        this.playDirection = dfPlayDirection.Forward;
        this.Play();
      }

      public void PlayReverse()
      {
        this.playDirection = dfPlayDirection.Reverse;
        this.Play();
      }

      public void Pause()
      {
        if (!this.isRunning)
          return;
        this.isPaused = true;
        this.onPaused();
      }

      public void Resume()
      {
        if (!this.isRunning || !this.isPaused)
          return;
        this.isPaused = false;
        this.onResumed();
      }

      public override bool IsPlaying => this.isRunning;

      public override void Play()
      {
        if (this.IsPlaying)
          this.Stop();
        if (!this.enabled || !this.gameObject.activeSelf || !this.gameObject.activeInHierarchy)
          return;
        if (this.memberInfo == null)
          throw new NullReferenceException("Animation target is NULL");
        this.StartCoroutine(this.Execute());
      }

      public override void Reset()
      {
        List<string> sprites = !((UnityEngine.Object) this.clip != (UnityEngine.Object) null) ? (List<string>) null : this.clip.Sprites;
        if (this.memberInfo.IsValid && sprites != null && sprites.Count > 0)
          dfSpriteAnimation.SetProperty((object) this.memberInfo.Component, this.memberInfo.MemberName, (object) sprites[0]);
        if (!this.isRunning)
          return;
        this.StopAllCoroutines();
        this.isRunning = false;
        this.isPaused = false;
        this.onReset();
      }

      public override void Stop()
      {
        if (!this.isRunning)
          return;
        List<string> sprites = !((UnityEngine.Object) this.clip != (UnityEngine.Object) null) ? (List<string>) null : this.clip.Sprites;
        if (this.skipToEndOnStop && sprites != null)
          this.setFrame(Mathf.Max(sprites.Count - 1, 0));
        this.StopAllCoroutines();
        this.isRunning = false;
        this.isPaused = false;
        this.onStopped();
      }

      public override string TweenName
      {
        get => this.animationName;
        set => this.animationName = value;
      }

      protected void onPaused()
      {
        this.SendMessage("AnimationPaused", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationPaused == null)
          return;
        this.AnimationPaused((dfTweenPlayableBase) this);
      }

      protected void onResumed()
      {
        this.SendMessage("AnimationResumed", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationResumed == null)
          return;
        this.AnimationResumed((dfTweenPlayableBase) this);
      }

      protected void onStarted()
      {
        this.SendMessage("AnimationStarted", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationStarted == null)
          return;
        this.AnimationStarted((dfTweenPlayableBase) this);
      }

      protected void onStopped()
      {
        this.SendMessage("AnimationStopped", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationStopped == null)
          return;
        this.AnimationStopped((dfTweenPlayableBase) this);
      }

      protected void onReset()
      {
        this.SendMessage("AnimationReset", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationReset == null)
          return;
        this.AnimationReset((dfTweenPlayableBase) this);
      }

      protected void onCompleted()
      {
        this.SendMessage("AnimationCompleted", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.AnimationCompleted == null)
          return;
        this.AnimationCompleted((dfTweenPlayableBase) this);
      }

      internal static void SetProperty(object target, string property, object value)
      {
        MemberInfo[] memberInfoArray = target != null ? target.GetType().GetMember(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : throw new NullReferenceException("Target is null");
        MemberInfo member = memberInfoArray != null && memberInfoArray.Length != 0 ? memberInfoArray[0] : throw new IndexOutOfRangeException("Property not found: " + property);
        switch (member)
        {
          case FieldInfo _:
            ((FieldInfo) member).SetValue(target, value);
            break;
          case PropertyInfo _:
            ((PropertyInfo) member).SetValue(target, value, (object[]) null);
            break;
          default:
            throw new InvalidOperationException("Member type not supported: " + (object) member.GetMemberType());
        }
      }

      [DebuggerHidden]
      private IEnumerator Execute()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new dfSpriteAnimation.\u003CExecute\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private string getPath(Transform obj)
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (; (UnityEngine.Object) obj != (UnityEngine.Object) null; obj = obj.parent)
        {
          if (stringBuilder.Length > 0)
          {
            stringBuilder.Insert(0, "\\");
            stringBuilder.Insert(0, obj.name);
          }
          else
            stringBuilder.Append(obj.name);
        }
        return stringBuilder.ToString();
      }

      public void SetFrameExternal(int index) => this.setFrame(index);

      private void setFrame(int frameIndex)
      {
        List<string> sprites = this.clip.Sprites;
        if (sprites.Count == 0)
          return;
        frameIndex = Mathf.Max(0, Mathf.Min(frameIndex, sprites.Count - 1));
        if (this.memberInfo == null)
          return;
        dfSprite component = this.memberInfo.Component as dfSprite;
        if ((bool) (UnityEngine.Object) component)
          component.SpriteName = sprites[frameIndex];
        if (!((UnityEngine.Object) this.m_selfControl != (UnityEngine.Object) null))
          return;
        this.m_selfControl.Invalidate();
      }
    }

}
