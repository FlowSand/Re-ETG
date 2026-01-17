// Decompiled with JetBrains decompiler
// Type: TitleDioramaController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class TitleDioramaController : MonoBehaviour
    {
      public tk2dSpriteAnimator LichArmAnimator;
      public tk2dSpriteAnimator LichBodyAnimator;
      public tk2dSpriteAnimator LichCapeAnimator;
      public tk2dSprite BeaconBeams;
      public tk2dSprite Eyeholes;
      public MeshRenderer FadeQuad;
      public tk2dSprite PastIslandSprite;
      public MeshRenderer SkyRenderer;
      public Camera m_fadeCamera;
      public GameObject VFX_BulletImpact;
      public GameObject VFX_Splash;
      public GameObject VFX_TrailParticles;
      public GameObject CloudsPrefab;
      public GameObject BackupCloudsPrefab;
      private bool m_manualTrigger;
      private bool m_isRevealed;
      private RenderTexture m_cachedFadeBuffer;
      private bool m_rushed;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<Start>c__Iterator0()
        {
          _this = this
        };
      }

      private bool ShouldUseLQ()
      {
        return GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || GameManager.Options.GetDefaultRecommendedGraphicalQuality() != GameOptions.GenericHighMedLowOption.HIGH;
      }

      public void CacheFrameToFadeBuffer(Camera cacheCamera)
      {
        this.FadeQuad.material.SetFloat("_UseAddlTex", 1f);
        this.FadeQuad.material.SetTexture("_AddlTex", (Texture) Pixelator.Instance.GetCachedFrame());
      }

      public bool IsRevealed(bool doReveal = false)
      {
        if (doReveal)
          this.m_rushed = true;
        return this.m_isRevealed;
      }

      public void ManualTrigger()
      {
        this.m_manualTrigger = true;
        this.StartCoroutine(this.Core(false));
        this.StartCoroutine(this.HandleLichIdlePhase());
      }

      private void Update()
      {
        if ((Object) this.m_fadeCamera != (Object) null)
          BraveCameraUtility.MaintainCameraAspect(this.m_fadeCamera);
        if (this.ShouldUseLQ() && (bool) (Object) this.CloudsPrefab && this.CloudsPrefab.activeSelf)
        {
          if ((bool) (Object) this.CloudsPrefab)
            this.CloudsPrefab.SetActive(false);
          if ((bool) (Object) this.BackupCloudsPrefab)
            this.BackupCloudsPrefab.SetActive(true);
        }
        else if (!this.ShouldUseLQ() && (bool) (Object) this.BackupCloudsPrefab && this.BackupCloudsPrefab.activeSelf)
        {
          if ((bool) (Object) this.CloudsPrefab)
            this.CloudsPrefab.SetActive(true);
          if ((bool) (Object) this.BackupCloudsPrefab)
            this.BackupCloudsPrefab.SetActive(false);
        }
        if (!((Object) this.FadeQuad != (Object) null) || !this.FadeQuad.enabled || !this.IsRevealed())
          return;
        this.FadeQuad.enabled = false;
      }

      [DebuggerHidden]
      private IEnumerator Core(bool isFoyer = true)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<Core>c__Iterator1()
        {
          isFoyer = isFoyer,
          _this = this
        };
      }

      private void OnDestroy() => this.Release();

      private void Release()
      {
        if (!((Object) this.m_cachedFadeBuffer != (Object) null))
          return;
        RenderTexture.ReleaseTemporary(this.m_cachedFadeBuffer);
        this.m_cachedFadeBuffer = (RenderTexture) null;
      }

      [DebuggerHidden]
      private IEnumerator LerpFadeValue(
        float startValue,
        float endValue,
        float duration,
        bool linearStep = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<LerpFadeValue>c__Iterator2()
        {
          duration = duration,
          startValue = startValue,
          endValue = endValue,
          linearStep = linearStep,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleFinalEyeholeEmission()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<HandleFinalEyeholeEmission>c__Iterator3()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator LerpEyeholeEmissionColorPower(
        float startValue,
        float endValue,
        float duration,
        bool really = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<LerpEyeholeEmissionColorPower>c__Iterator4()
        {
          duration = duration,
          startValue = startValue,
          endValue = endValue,
          really = really,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator LerpEyeholeEmission(
        float startValue,
        float endValue,
        float duration,
        bool really = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<LerpEyeholeEmission>c__Iterator5()
        {
          duration = duration,
          startValue = startValue,
          endValue = endValue,
          really = really,
          _this = this
        };
      }

      public void ForceHideFadeQuad()
      {
        if (!(bool) (Object) this.FadeQuad)
          return;
        this.FadeQuad.material.SetFloat("_Threshold", 0.0f);
        this.FadeQuad.enabled = false;
      }

      [DebuggerHidden]
      private IEnumerator HandleReveal()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<HandleReveal>c__Iterator6()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLichIdlePhase()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<HandleLichIdlePhase>c__Iterator7()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLichFiddlePhase()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<HandleLichFiddlePhase>c__Iterator8()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleLichCapePhase()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TitleDioramaController.<HandleLichCapePhase>c__Iterator9()
        {
          _this = this
        };
      }
    }

}
