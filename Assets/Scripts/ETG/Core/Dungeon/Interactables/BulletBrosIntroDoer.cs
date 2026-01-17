// Decompiled with JetBrains decompiler
// Type: BulletBrosIntroDoer
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
    public class BulletBrosIntroDoer : SpecificIntroDoer
    {
      public tk2dSpriteAnimator shadowDummy;
      private bool m_initialized;
      private bool m_finished;
      private AIAnimator m_smiley;
      private tk2dBaseSprite m_smileyShadow;
      private AIAnimator m_shades;
      private tk2dBaseSprite m_shadesShadow;

      public void Update()
      {
        if (this.m_initialized || !(bool) (Object) this.aiActor.ShadowObject)
          return;
        this.m_smiley = this.aiAnimator;
        for (int index = 0; index < StaticReferenceManager.AllBros.Count; ++index)
        {
          if ((Object) StaticReferenceManager.AllBros[index].gameObject != (Object) this.gameObject)
          {
            this.m_shades = StaticReferenceManager.AllBros[index].aiAnimator;
            break;
          }
        }
        this.m_smiley.aiActor.ToggleRenderers(false);
        this.m_smiley.aiShooter.ToggleGunAndHandRenderers(false, nameof (BulletBrosIntroDoer));
        this.m_smiley.transform.position += PhysicsEngine.PixelToUnit(new IntVector2(11, 0)).ToVector3ZUp();
        this.m_smiley.specRigidbody.Reinitialize();
        this.m_shades.aiActor.ToggleRenderers(false);
        this.m_shades.aiShooter.ToggleGunAndHandRenderers(false, nameof (BulletBrosIntroDoer));
        this.m_shades.transform.position += PhysicsEngine.PixelToUnit(new IntVector2(-11, 0)).ToVector3ZUp();
        this.m_shades.specRigidbody.Reinitialize();
        this.m_smileyShadow = this.m_smiley.aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        this.m_smileyShadow.renderer.enabled = false;
        this.m_shadesShadow = this.m_shades.aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        this.m_shadesShadow.renderer.enabled = false;
        this.m_initialized = true;
      }

      protected override void OnDestroy() => base.OnDestroy();

      public override Vector2? OverrideIntroPosition
      {
        get
        {
          return new Vector2?(0.5f * (this.m_smiley.specRigidbody.GetUnitCenter(ColliderType.HitBox) + this.m_shades.specRigidbody.GetUnitCenter(ColliderType.HitBox)));
        }
      }

      public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
      {
        this.aiAnimator.LockFacingDirection = true;
        this.aiAnimator.FacingDirection = -90f;
        if ((bool) (Object) this.m_smiley && (bool) (Object) this.m_shades)
        {
          this.m_smiley.aiActor.ToggleRenderers(false);
          SpriteOutlineManager.ToggleOutlineRenderers(this.m_smiley.sprite, false);
          this.m_smiley.aiShooter.ToggleGunAndHandRenderers(false, nameof (BulletBrosIntroDoer));
          this.m_smileyShadow.renderer.enabled = false;
          this.m_shades.aiActor.ToggleRenderers(false);
          SpriteOutlineManager.ToggleOutlineRenderers(this.m_shades.sprite, false);
          this.m_shades.aiShooter.ToggleGunAndHandRenderers(false, nameof (BulletBrosIntroDoer));
          this.m_shadesShadow.renderer.enabled = false;
        }
        this.StartCoroutine(this.FuckOutlines());
      }

      [DebuggerHidden]
      private IEnumerator FuckOutlines()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletBrosIntroDoer.<FuckOutlines>c__Iterator0()
        {
          $this = this
        };
      }

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        this.StartCoroutine(this.DoIntro());
        animators.Add(this.m_shades.spriteAnimator);
        animators.Add(this.shadowDummy);
      }

      public override bool IsIntroFinished => this.m_finished;

      public override void OnBossCard()
      {
        this.m_smileyShadow.renderer.enabled = true;
        this.m_shadesShadow.renderer.enabled = true;
        this.m_smiley.aiShooter.ToggleGunAndHandRenderers(true, nameof (BulletBrosIntroDoer));
        this.m_shades.aiShooter.ToggleGunAndHandRenderers(true, nameof (BulletBrosIntroDoer));
      }

      public override void EndIntro()
      {
        this.m_finished = true;
        this.StopAllCoroutines();
        this.m_smiley.aiActor.ToggleRenderers(true);
        SpriteOutlineManager.ToggleOutlineRenderers(this.m_smiley.sprite, true);
        this.m_smiley.sprite.renderer.enabled = true;
        this.m_smiley.EndAnimation();
        this.m_smiley.aiShooter.ToggleGunAndHandRenderers(true, nameof (BulletBrosIntroDoer));
        this.m_smiley.specRigidbody.CollideWithOthers = true;
        this.m_smiley.aiActor.IsGone = false;
        this.m_smiley.aiActor.State = AIActor.ActorState.Normal;
        this.m_smiley.aiShooter.AimAtPoint(this.m_smiley.aiActor.CenterPosition + new Vector2(10f, -2f));
        this.m_smiley.FacingDirection = -90f;
        this.m_shades.aiActor.ToggleRenderers(true);
        SpriteOutlineManager.ToggleOutlineRenderers(this.m_shades.sprite, true);
        this.m_shades.sprite.renderer.enabled = true;
        this.m_shades.EndAnimation();
        this.m_shades.aiShooter.ToggleGunAndHandRenderers(true, nameof (BulletBrosIntroDoer));
        this.m_shades.specRigidbody.CollideWithOthers = true;
        this.m_shades.aiActor.IsGone = false;
        this.m_shades.aiActor.State = AIActor.ActorState.Normal;
        this.m_shades.aiShooter.AimAtPoint(this.m_shades.aiActor.CenterPosition + new Vector2(-10f, -2f));
        this.m_shades.FacingDirection = -90f;
        this.shadowDummy.renderer.enabled = false;
      }

      [DebuggerHidden]
      private IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletBrosIntroDoer.<DoIntro>c__Iterator1()
        {
          $this = this
        };
      }
    }

}
