// Decompiled with JetBrains decompiler
// Type: dfTweenGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/Tweens/Group")]
    [Serializable]
    public class dfTweenGroup : dfTweenPlayableBase
    {
      [SerializeField]
      protected string groupName = string.Empty;
      [SerializeField]
      protected bool autoStart;
      [SerializeField]
      protected float delayBeforeStarting;
      public List<dfTweenPlayableBase> Tweens = new List<dfTweenPlayableBase>();
      public dfTweenGroup.TweenGroupMode Mode;

      public event TweenNotification TweenStarted;

      public event TweenNotification TweenStopped;

      public event TweenNotification TweenReset;

      public event TweenNotification TweenCompleted;

      public float StartDelay
      {
        get => this.delayBeforeStarting;
        set => this.delayBeforeStarting = value;
      }

      public bool AutoStart
      {
        get => this.autoStart;
        set => this.autoStart = value;
      }

      public override string TweenName
      {
        get => this.groupName;
        set => this.groupName = value;
      }

      public override bool IsPlaying
      {
        get
        {
          for (int index = 0; index < this.Tweens.Count; ++index)
          {
            if (!((UnityEngine.Object) this.Tweens[index] == (UnityEngine.Object) null) && this.Tweens[index].enabled && this.Tweens[index].IsPlaying)
              return true;
          }
          return false;
        }
      }

      public void Start()
      {
        if (!this.AutoStart || this.IsPlaying)
          return;
        this.Play();
      }

      public void EnableTween(string TweenName)
      {
        for (int index = 0; index < this.Tweens.Count; ++index)
        {
          if (!((UnityEngine.Object) this.Tweens[index] == (UnityEngine.Object) null) && this.Tweens[index].TweenName == TweenName)
          {
            this.Tweens[index].enabled = true;
            break;
          }
        }
      }

      public void DisableTween(string TweenName)
      {
        for (int index = 0; index < this.Tweens.Count; ++index)
        {
          if (!((UnityEngine.Object) this.Tweens[index] == (UnityEngine.Object) null) && this.Tweens[index].name == TweenName)
          {
            this.Tweens[index].enabled = false;
            break;
          }
        }
      }

      public override void Play()
      {
        if (this.IsPlaying)
          this.Stop();
        this.onStarted();
        if (this.Mode == dfTweenGroup.TweenGroupMode.Concurrent)
          this.StartCoroutine(this.runConcurrent());
        else
          this.StartCoroutine(this.runSequence());
      }

      public override void Stop()
      {
        if (!this.IsPlaying)
          return;
        this.StopAllCoroutines();
        for (int index = 0; index < this.Tweens.Count; ++index)
        {
          if (!((UnityEngine.Object) this.Tweens[index] == (UnityEngine.Object) null))
            this.Tweens[index].Stop();
        }
        this.onStopped();
      }

      public override void Reset()
      {
        if (!this.IsPlaying)
          return;
        this.StopAllCoroutines();
        for (int index = 0; index < this.Tweens.Count; ++index)
        {
          if (!((UnityEngine.Object) this.Tweens[index] == (UnityEngine.Object) null))
            this.Tweens[index].Reset();
        }
        this.onReset();
      }

      [HideInInspector]
      [DebuggerHidden]
      private IEnumerator runSequence()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new dfTweenGroup.<runSequence>c__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      [HideInInspector]
      private IEnumerator runConcurrent()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new dfTweenGroup.<runConcurrent>c__Iterator1()
        {
          _this = this
        };
      }

      protected internal void onStarted()
      {
        this.SendMessage("TweenStarted", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenStarted == null)
          return;
        this.TweenStarted((dfTweenPlayableBase) this);
      }

      protected internal void onStopped()
      {
        this.SendMessage("TweenStopped", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenStopped == null)
          return;
        this.TweenStopped((dfTweenPlayableBase) this);
      }

      protected internal void onReset()
      {
        this.SendMessage("TweenReset", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenReset == null)
          return;
        this.TweenReset((dfTweenPlayableBase) this);
      }

      protected internal void onCompleted()
      {
        this.SendMessage("TweenCompleted", (object) this, SendMessageOptions.DontRequireReceiver);
        if (this.TweenCompleted == null)
          return;
        this.TweenCompleted((dfTweenPlayableBase) this);
      }

      public enum TweenGroupMode
      {
        Concurrent,
        Sequence,
      }
    }

}
