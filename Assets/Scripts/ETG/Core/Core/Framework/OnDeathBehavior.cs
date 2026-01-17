// Decompiled with JetBrains decompiler
// Type: OnDeathBehavior
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
    public abstract class OnDeathBehavior : BraveBehaviour
    {
      public OnDeathBehavior.DeathType deathType = OnDeathBehavior.DeathType.Death;
      [ShowInInspectorIf("deathType", 0, false)]
      public float preDeathDelay;
      [ShowInInspectorIf("deathType", 2, false)]
      public string triggerName;
      private Vector2 m_deathDir;

      public virtual void Start()
      {
        if (!(bool) (UnityEngine.Object) this.healthHaver)
          return;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
        if (this.deathType == OnDeathBehavior.DeathType.Death)
        {
          this.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);
        }
        else
        {
          if (this.deathType != OnDeathBehavior.DeathType.DeathAnimTrigger)
            return;
          this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
        }
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.healthHaver)
        {
          if (this.deathType == OnDeathBehavior.DeathType.Death)
            this.healthHaver.OnDeath -= new Action<Vector2>(this.OnDeath);
          else if (this.deathType == OnDeathBehavior.DeathType.DeathAnimTrigger)
            this.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
        }
        base.OnDestroy();
      }

      protected abstract void OnTrigger(Vector2 dirVec);

      private void OnPreDeath(Vector2 dirVec)
      {
        this.m_deathDir = dirVec;
        if (this.deathType != OnDeathBehavior.DeathType.PreDeath)
          return;
        if ((double) this.preDeathDelay > 0.0)
          this.StartCoroutine(this.DelayedOnTriggerCR(this.preDeathDelay));
        else
          this.OnTrigger(this.m_deathDir);
      }

      private void OnDeath(Vector2 dirVec) => this.OnTrigger(this.m_deathDir);

      private void AnimationEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frame)
      {
        if (!this.healthHaver.IsDead || !(clip.GetFrame(frame).eventInfo == this.triggerName))
          return;
        this.OnTrigger(this.m_deathDir);
      }

      [DebuggerHidden]
      private IEnumerator DelayedOnTriggerCR(float delay)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new OnDeathBehavior.\u003CDelayedOnTriggerCR\u003Ec__Iterator0()
        {
          delay = delay,
          \u0024this = this
        };
      }

      public enum DeathType
      {
        PreDeath,
        Death,
        DeathAnimTrigger,
      }
    }

}
