// Decompiled with JetBrains decompiler
// Type: TankTreaderIntroDoer
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
    public class TankTreaderIntroDoer : SpecificIntroDoer
    {
      public BodyPartController mainGun;
      public AIAnimator guy;
      public tk2dSpriteAnimator hatch;
      private bool m_finished;
      private ParticleSystem[] m_exhaustParticleSystems;

      protected override void OnDestroy() => base.OnDestroy();

      public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
      {
        this.mainGun.enabled = false;
        this.mainGun.aiAnimator.LockFacingDirection = true;
        this.mainGun.aiAnimator.FacingDirection = -90f;
        this.mainGun.aiAnimator.Update();
        this.aiAnimator.LockFacingDirection = true;
        this.aiAnimator.FacingDirection = -90f;
        this.aiAnimator.Update();
        this.m_exhaustParticleSystems = this.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem exhaustParticleSystem in this.m_exhaustParticleSystems)
        {
          BraveUtility.EnableEmission(exhaustParticleSystem, false);
          exhaustParticleSystem.Clear();
          exhaustParticleSystem.GetComponent<Renderer>().enabled = false;
        }
      }

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        animators.Add(this.guy.spriteAnimator);
        animators.Add(this.hatch);
        this.StartCoroutine(this.DoIntro());
        int num = (int) AkSoundEngine.PostEvent("Play_BOSS_tank_idle_01", this.gameObject);
      }

      public override void OnCleanup()
      {
        this.mainGun.enabled = true;
        this.mainGun.aiAnimator.LockFacingDirection = false;
        this.guy.EndAnimationIf("intro");
        this.hatch.Play("hatch_closed");
        foreach (Behaviour componentsInChild in this.GetComponentsInChildren<TankTreaderMiniTurretController>())
          componentsInChild.enabled = true;
        foreach (ParticleSystem exhaustParticleSystem in this.m_exhaustParticleSystems)
        {
          BraveUtility.EnableEmission(exhaustParticleSystem, true);
          exhaustParticleSystem.GetComponent<Renderer>().enabled = true;
        }
      }

      public override bool IsIntroFinished => this.m_finished;

      public override void OnBossCard()
      {
      }

      public override void EndIntro()
      {
      }

      [DebuggerHidden]
      private IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TankTreaderIntroDoer__DoIntroc__Iterator0()
        {
          _this = this
        };
      }
    }

}
