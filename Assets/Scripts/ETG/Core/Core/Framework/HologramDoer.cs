// Decompiled with JetBrains decompiler
// Type: HologramDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class HologramDoer : BraveBehaviour
    {
      public Transform Holopoint;
      public tk2dSprite Glower;
      public MeshRenderer ArcRenderer;
      private Material m_arcMaterial;
      public bool Automatic;
      public tk2dSprite TargetAutomaticSprite;
      public AdditionalBraveLight AttachedBraveLight;
      public bool parentHologram;
      public bool NotAHologram;
      private GameObject m_lastSource;
      private tk2dSprite m_hologramSprite;

      private void Start()
      {
        this.ArcRenderer.enabled = false;
        this.m_arcMaterial = this.ArcRenderer.material;
        if (!this.Automatic)
          return;
        this.StartCoroutine(this.HandleAutomaticTrigger());
      }

      [DebuggerHidden]
      private IEnumerator HandleAutomaticTrigger()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramDoer.<HandleAutomaticTrigger>c__Iterator0()
        {
          _this = this
        };
      }

      private void Update()
      {
      }

      [DebuggerHidden]
      private IEnumerator ToggleAdditionalLight(bool lightEnabled)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramDoer.<ToggleAdditionalLight>c__Iterator1()
        {
          lightEnabled = lightEnabled,
          _this = this
        };
      }

      public void HideSprite(GameObject source, bool instant = false)
      {
        if (!((Object) this.m_lastSource == (Object) source) && !instant)
          return;
        if ((bool) (Object) this.m_hologramSprite)
        {
          if (this.NotAHologram)
            SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.m_hologramSprite, false);
          this.m_hologramSprite.renderer.enabled = false;
        }
        if ((bool) (Object) this.AttachedBraveLight)
          this.StartCoroutine(this.ToggleAdditionalLight(false));
        if (instant)
          this.ArcRenderer.enabled = false;
        else
          this.StartCoroutine(this.HandleArcLerp(true));
        if ((bool) (Object) this.Glower)
          this.Glower.renderer.material.SetFloat("_EmissivePower", 0.0f);
        this.m_lastSource = (GameObject) null;
      }

      [DebuggerHidden]
      private IEnumerator HandleArcLerp(bool invert)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HologramDoer.<HandleArcLerp>c__Iterator2()
        {
          invert = invert,
          _this = this
        };
      }

      public void ChangeToSprite(GameObject source, tk2dSpriteCollectionData collection, int spriteId)
      {
        this.m_lastSource = source;
        if (this.Automatic)
          this.m_hologramSprite = this.TargetAutomaticSprite;
        else if ((Object) this.m_hologramSprite == (Object) null)
        {
          this.m_hologramSprite = tk2dSprite.AddComponent(new GameObject("hologram"), collection, spriteId);
          if (this.parentHologram)
            this.m_hologramSprite.transform.parent = this.transform;
          if (this.NotAHologram)
            SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.m_hologramSprite, Color.white);
          if ((bool) (Object) this.Glower && !this.NotAHologram)
          {
            this.Glower.usesOverrideMaterial = true;
            this.Glower.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
          }
        }
        else
        {
          this.m_hologramSprite.SetSprite(collection, spriteId);
          this.m_hologramSprite.ForceUpdateMaterial();
        }
        if (this.NotAHologram)
          SpriteOutlineManager.ToggleOutlineRenderers((tk2dBaseSprite) this.m_hologramSprite, true);
        this.m_hologramSprite.renderer.enabled = true;
        this.m_hologramSprite.usesOverrideMaterial = true;
        if (!this.NotAHologram)
          this.m_hologramSprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HologramShader");
        if (this.Automatic)
        {
          this.m_hologramSprite.renderer.material.SetFloat("_IsGreen", 1f);
          this.m_hologramSprite.spriteAnimator.PlayForDuration(this.m_hologramSprite.spriteAnimator.DefaultClip.name, this.m_hologramSprite.spriteAnimator.DefaultClip.BaseClipLength, "hbux_symbol_idle");
        }
        else
        {
          this.m_hologramSprite.PlaceAtPositionByAnchor(this.Holopoint.position, tk2dBaseSprite.Anchor.LowerCenter);
          this.m_hologramSprite.transform.localPosition = this.m_hologramSprite.transform.localPosition.Quantize(1f / 16f);
        }
        if ((bool) (Object) this.Glower && !this.NotAHologram)
        {
          this.Glower.renderer.material.SetFloat("_EmissivePower", 20f);
          this.Glower.renderer.material.SetFloat("_EmissiveColorPower", 3f);
        }
        if ((bool) (Object) this.AttachedBraveLight)
          this.StartCoroutine(this.ToggleAdditionalLight(true));
        this.StartCoroutine(this.HandleArcLerp(false));
      }
    }

}
