// Decompiled with JetBrains decompiler
// Type: AnimationColliderTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AnimationColliderTrigger : BraveBehaviour
  {
    private void Awake()
    {
      this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
    }

    protected override void OnDestroy()
    {
      this.spriteAnimator.AnimationEventTriggered -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      base.OnDestroy();
    }

    private void AnimationEventTriggered(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      if (clip.GetFrame(frame).eventInfo == "collider_on")
      {
        if ((bool) (UnityEngine.Object) this.aiActor)
          this.aiActor.IsGone = false;
        if (!(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        this.specRigidbody.CollideWithOthers = true;
      }
      else
      {
        if (!(clip.GetFrame(frame).eventInfo == "collider_off"))
          return;
        if ((bool) (UnityEngine.Object) this.aiActor)
          this.aiActor.IsGone = true;
        if (!(bool) (UnityEngine.Object) this.specRigidbody)
          return;
        this.specRigidbody.CollideWithOthers = false;
      }
    }
  }

