// Decompiled with JetBrains decompiler
// Type: BlobulordIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [RequireComponent(typeof (GenericIntroDoer))]
    public class BlobulordIntroDoer : SpecificIntroDoer
    {
      public Transform particleTransform;
      private bool m_initialized;
      private bool m_finished;
      private tk2dBaseSprite m_shadowSprite;

      public void Update()
      {
        if (this.m_initialized || !this.aiActor.enabled || !(bool) (Object) this.aiActor.ShadowObject)
          return;
        this.m_shadowSprite = this.aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(0.0f);
        this.m_initialized = true;
      }

      protected override void OnDestroy() => base.OnDestroy();

      public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
      {
        this.aiAnimator.LockFacingDirection = true;
        this.aiAnimator.FacingDirection = -90f;
        this.aiAnimator.PlayUntilCancelled("preintro");
        if (!(bool) (Object) this.m_shadowSprite)
          return;
        this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(0.0f);
      }

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        this.aiAnimator.PlayUntilFinished("intro");
        this.StartCoroutine(this.DoIntro());
      }

      public override bool IsIntroFinished => this.m_finished;

      public override void EndIntro()
      {
        this.m_finished = true;
        this.StopAllCoroutines();
        this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
        this.aiAnimator.LockFacingDirection = false;
        this.aiAnimator.EndAnimation();
      }

      [DebuggerHidden]
      private IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlobulordIntroDoer__DoIntroc__Iterator0()
        {
          _this = this
        };
      }
    }

}
