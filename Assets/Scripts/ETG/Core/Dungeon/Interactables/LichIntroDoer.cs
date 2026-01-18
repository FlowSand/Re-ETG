using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[RequireComponent(typeof (GenericIntroDoer))]
public class LichIntroDoer : SpecificIntroDoer
  {
    public static bool DoubleLich;
    public tk2dSprite HandSprite;
    public Texture2D CosmicTex;
    private AIActor m_otherLich;

    protected override void OnDestroy() => base.OnDestroy();

    public override string OverrideBossMusicEvent
    {
      get => GameManager.IsGunslingerPast ? "Play_MUS_Lich_Double_01" : (string) null;
    }

    public override void PlayerWalkedIn(PlayerController player, List<tk2dSpriteAnimator> animators)
    {
      LichIntroDoer.DoubleLich = GameManager.IsGunslingerPast;
      if (!LichIntroDoer.DoubleLich)
        return;
      this.aiActor.PreventBlackPhantom = true;
      this.m_otherLich = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.aiActor.EnemyGuid), this.specRigidbody.UnitBottomLeft, this.aiActor.ParentRoom, autoEngage: false);
      this.m_otherLich.transform.position = this.transform.position + new Vector3(0.25f, 0.25f, 0.0f);
      this.m_otherLich.specRigidbody.Reinitialize();
      this.m_otherLich.OverrideBlackPhantomShader = ShaderCache.Acquire("Brave/PlayerShaderEevee");
      this.m_otherLich.ForceBlackPhantom = true;
      this.m_otherLich.sprite.renderer.material.SetTexture("_EeveeTex", (Texture) this.CosmicTex);
      this.m_otherLich.sprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
      this.m_otherLich.sprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
      animators.Add(this.m_otherLich.spriteAnimator);
      this.m_otherLich.aiAnimator.PlayUntilCancelled("preintro");
      this.StartCoroutine(this.HandleDelayedTextureCR());
    }

    [DebuggerHidden]
    private IEnumerator HandleDelayedTextureCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new LichIntroDoer__HandleDelayedTextureCRc__Iterator0()
      {
        _this = this
      };
    }

    public override void StartIntro(List<tk2dSpriteAnimator> animators)
    {
      if (!LichIntroDoer.DoubleLich)
        return;
      this.m_otherLich.aiAnimator.PlayUntilCancelled("intro");
    }

    public override void OnCameraOutro()
    {
      if (!LichIntroDoer.DoubleLich)
        return;
      this.aiAnimator.FacingDirection = -90f;
      this.aiAnimator.PlayUntilCancelled("idle");
      this.m_otherLich.aiAnimator.FacingDirection = -90f;
      this.m_otherLich.aiAnimator.PlayUntilCancelled("idle");
    }
  }

