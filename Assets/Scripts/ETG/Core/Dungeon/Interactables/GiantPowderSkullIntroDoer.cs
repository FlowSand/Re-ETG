// Decompiled with JetBrains decompiler
// Type: GiantPowderSkullIntroDoer
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
    public class GiantPowderSkullIntroDoer : SpecificIntroDoer
    {
      [CurveRange(0.0f, 0.0f, 1f, 1f)]
      public AnimationCurve emissionRate;
      private bool m_initialized;
      private bool m_finished;
      private ParticleSystem m_mainParticleSystem;
      private ParticleSystem m_trailParticleSystem;
      private float m_startParticleRate;
      private tk2dBaseSprite m_shadowSprite;

      public void Update()
      {
        if (this.m_initialized || !(bool) (Object) this.aiActor.ShadowObject || !((Object) this.m_mainParticleSystem != (Object) null))
          return;
        this.m_shadowSprite = this.aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, false);
        this.aiActor.ToggleRenderers(false);
        this.m_shadowSprite.renderer.enabled = false;
        this.m_mainParticleSystem.GetComponent<Renderer>().enabled = true;
        this.m_initialized = true;
        UnityEngine.Debug.Log((object) "INITIALIZED!");
      }

      protected override void OnDestroy() => base.OnDestroy();

      public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
      {
        this.StartCoroutine(this.RunParticleSystems());
        int num = (int) AkSoundEngine.PostEvent("Play_ENM_cannonball_intro_01", this.gameObject);
        this.aiAnimator.LockFacingDirection = true;
        this.aiAnimator.FacingDirection = -90f;
      }

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        this.StartCoroutine(this.DoIntro());
      }

      public override bool IsIntroFinished => this.m_finished;

      public override void OnBossCard() => this.m_shadowSprite.renderer.enabled = true;

      public override void EndIntro()
      {
        this.m_finished = true;
        this.StopAllCoroutines();
        SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, true);
        this.aiActor.ToggleRenderers(true);
        this.m_shadowSprite.renderer.enabled = true;
        this.aiAnimator.LockFacingDirection = false;
        this.aiAnimator.EndAnimation();
        BraveUtility.SetEmissionRate(this.m_mainParticleSystem, this.m_startParticleRate);
        BraveUtility.EnableEmission(this.m_mainParticleSystem, true);
        this.m_mainParticleSystem.Play();
        BraveUtility.EnableEmission(this.m_trailParticleSystem, true);
      }

      [DebuggerHidden]
      private IEnumerator RunParticleSystems()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GiantPowderSkullIntroDoer.\u003CRunParticleSystems\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GiantPowderSkullIntroDoer.\u003CDoIntro\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }
    }

}
