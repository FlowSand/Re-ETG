// Decompiled with JetBrains decompiler
// Type: EndTimesNebulaController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class EndTimesNebulaController : MonoBehaviour
    {
      public Camera NebulaCamera;
      public SlicedVolume NebulaClouds;
      public float CloudParallaxFactor = 0.5f;
      public Transform BGQuad;
      private bool m_isActive;
      private Material m_nebulaMaterial;
      private RenderTexture m_partiallyActiveRenderTarget;
      private Material m_portalMaterial;
      public List<Renderer> NebulaRegisteredVisuals = new List<Renderer>();
      private int m_playerPosID = -1;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new EndTimesNebulaController__Startc__Iterator0()
        {
          _this = this
        };
      }

      public void BecomePartiallyActive()
      {
        this.m_partiallyActiveRenderTarget = RenderTexture.GetTemporary(Pixelator.Instance.CurrentMacroResolutionX, Pixelator.Instance.CurrentMacroResolutionY, 0, RenderTextureFormat.Default);
        this.NebulaCamera.enabled = true;
        this.NebulaCamera.targetTexture = this.m_partiallyActiveRenderTarget;
        this.m_portalMaterial = BraveResources.Load("Shaders/DarkPortalMaterial", ".mat") as Material;
        if ((bool) (Object) this.m_portalMaterial)
          this.m_portalMaterial.SetTexture("_PortalTex", (Texture) this.m_partiallyActiveRenderTarget);
        Shader.SetGlobalTexture("_EndTimesVortex", (Texture) this.m_partiallyActiveRenderTarget);
        if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW)
        {
          this.NebulaClouds.generateNewSlices = true;
          this.m_nebulaMaterial = this.NebulaClouds.cloudMaterial;
        }
        else
        {
          Object.Destroy((Object) this.NebulaClouds.gameObject);
          this.m_nebulaMaterial = (Material) null;
        }
      }

      private void ClearRT()
      {
        if (!((Object) this.m_partiallyActiveRenderTarget != (Object) null))
          return;
        RenderTexture.ReleaseTemporary(this.m_partiallyActiveRenderTarget);
        this.m_partiallyActiveRenderTarget = (RenderTexture) null;
      }

      public void BecomeActive()
      {
        this.m_isActive = true;
        this.NebulaCamera.enabled = true;
        this.ClearRT();
        Pixelator.Instance.AdditionalBGCamera = this.NebulaCamera;
      }

      private void Update()
      {
        if (!this.m_isActive && !GameManager.Instance.IsLoadingLevel)
        {
          if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
            this.NebulaCamera.enabled = false;
          else if (!this.NebulaCamera.enabled)
          {
            if (GameManager.Instance.AllPlayers != null)
            {
              for (int index = 0; index < this.NebulaRegisteredVisuals.Count; ++index)
              {
                if (this.NebulaRegisteredVisuals[index].isVisible)
                  this.NebulaCamera.enabled = true;
              }
            }
          }
          else if (this.NebulaCamera.enabled && GameManager.Instance.AllPlayers != null)
          {
            bool flag = false;
            for (int index = 0; index < this.NebulaRegisteredVisuals.Count; ++index)
            {
              if (this.NebulaRegisteredVisuals[index].isVisible)
                flag = true;
            }
            if (!flag)
              this.NebulaCamera.enabled = false;
          }
        }
        if (this.m_isActive && (Object) this.m_nebulaMaterial != (Object) null)
          this.m_nebulaMaterial.SetFloat("_ZOffset", GameManager.Instance.MainCameraController.transform.position.y * this.CloudParallaxFactor);
        if (this.m_isActive && (bool) (Object) this.BGQuad)
        {
          float num = BraveCameraUtility.ASPECT / 1.77777779f;
          if ((double) num > 1.0)
            this.BGQuad.transform.localScale = new Vector3(16f * num, 9f, 1f);
          else
            this.BGQuad.transform.localScale = new Vector3(16f * num, 9f, 1f);
        }
        if (!(bool) (Object) this.m_portalMaterial)
          return;
        if (this.m_playerPosID == -1)
          this.m_playerPosID = Shader.PropertyToID("_PlayerPos");
        Vector2 centerPosition = GameManager.Instance.PrimaryPlayer.CenterPosition;
        Vector2 vector2 = GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER ? Vector2.zero : GameManager.Instance.SecondaryPlayer.CenterPosition;
        this.m_portalMaterial.SetVector(this.m_playerPosID, new Vector4(centerPosition.x, centerPosition.y, vector2.x, vector2.y));
      }

      private void OnDestroy() => this.ClearRT();

      public void BecomeInactive(bool destroy = true)
      {
        this.m_isActive = false;
        if (Pixelator.HasInstance && (Object) Pixelator.Instance.AdditionalBGCamera == (Object) this.NebulaCamera)
          Pixelator.Instance.AdditionalBGCamera = (Camera) null;
        if (!destroy)
          return;
        Object.Destroy((Object) this.gameObject);
      }
    }

}
