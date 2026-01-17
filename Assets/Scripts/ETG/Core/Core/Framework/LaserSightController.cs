// Decompiled with JetBrains decompiler
// Type: LaserSightController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class LaserSightController : BraveBehaviour
    {
      public bool DoFlash;
      [CurveRange(0.0f, 0.0f, 1f, 1f)]
      public AnimationCurve flashCurve;
      public bool DoAnim;
      [CheckAnimation(null)]
      public string idleAnim;
      [CheckAnimation(null)]
      public string preFireAnim;
      private tk2dSpriteAnimationClip m_idleClip;
      private tk2dSpriteAnimationClip m_preFireClip;
      private float m_preFireLength;

      public void Start()
      {
        if (!(bool) (Object) this.spriteAnimator)
          return;
        this.m_idleClip = this.spriteAnimator.GetClipByName(this.idleAnim);
        this.m_preFireClip = this.spriteAnimator.GetClipByName(this.preFireAnim);
        this.m_preFireLength = this.m_preFireClip.BaseClipLength;
      }

      public void UpdateCountdown(float m_prefireTimer, float PreFireLaserTime)
      {
        this.renderer.enabled = true;
        if (this.DoFlash)
          this.renderer.enabled = (double) this.flashCurve.Evaluate((float) (1.0 - (double) m_prefireTimer / (double) PreFireLaserTime)) > 0.5;
        if (!this.DoAnim || !(bool) (Object) this.spriteAnimator)
          return;
        if ((double) m_prefireTimer < (double) this.m_preFireLength)
          this.spriteAnimator.Play(this.m_preFireClip, this.m_preFireLength - m_prefireTimer, this.m_preFireClip.fps);
        else
          this.spriteAnimator.Play(this.m_idleClip);
      }

      public void ResetCountdown()
      {
        this.renderer.enabled = false;
        if (!this.DoAnim || !(bool) (Object) this.spriteAnimator)
          return;
        this.spriteAnimator.Play(this.m_idleClip);
      }
    }

}
