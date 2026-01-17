// Decompiled with JetBrains decompiler
// Type: Pixelator_Simple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class Pixelator_Simple : MonoBehaviour
    {
      public Shader renderShader;
      public Shader upsideDownShader;
      public Camera slaveCamera;
      private RenderTexture m_renderTarget;
      private Camera m_camera;
      private Material m_renderMaterial;
      private Material m_upsideDownMaterial;
      private int m_cachedCullingMask;
      private bool m_initialized;

      public Material RenderMaterial
      {
        get => this.m_renderMaterial;
        set => this.m_renderMaterial = value;
      }

      private void Start()
      {
        this.slaveCamera.GetComponent<dfGUICamera>().transform.parent.GetComponent<dfGUIManager>().OverrideCamera = true;
        this.Initialize();
      }

      public void Initialize()
      {
        if (this.m_initialized)
          return;
        this.m_initialized = true;
        if ((Object) this.renderShader != (Object) null)
          this.m_renderMaterial = new Material(this.renderShader);
        if ((Object) this.upsideDownShader != (Object) null)
          this.m_upsideDownMaterial = new Material(this.upsideDownShader);
        this.m_cachedCullingMask = this.slaveCamera.cullingMask;
      }

      private void RebuildRenderTarget(RenderTexture source)
      {
        if ((Object) Pixelator.Instance == (Object) null)
        {
          this.m_renderTarget = (RenderTexture) null;
        }
        else
        {
          int width = Pixelator.Instance.CurrentMacroResolutionX;
          int height = Pixelator.Instance.CurrentMacroResolutionY;
          if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.KOREAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
          {
            width = source.width;
            height = source.height;
          }
          if (!((Object) this.m_renderTarget == (Object) null) && this.m_renderTarget.width == width && this.m_renderTarget.height == height)
            return;
          this.m_renderTarget = new RenderTexture(width, height, 1);
          this.m_renderTarget.filterMode = FilterMode.Point;
        }
      }

      private void OnRenderImage(RenderTexture source, RenderTexture target)
      {
        if ((Object) this.m_camera == (Object) null)
          this.m_camera = this.GetComponent<Camera>();
        this.RebuildRenderTarget(source);
        RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, source.depth);
        if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.KOREAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
          temporary.filterMode = FilterMode.Point;
        Graphics.Blit((Texture) Pixelator.SmallBlackTexture, temporary);
        if ((Object) this.m_renderTarget == (Object) null)
        {
          if ((Object) this.m_renderMaterial != (Object) null)
            Graphics.Blit((Texture) source, target, this.m_renderMaterial);
          else
            Graphics.Blit((Texture) source, target);
        }
        else
        {
          this.slaveCamera.CopyFrom(this.m_camera);
          this.slaveCamera.transform.position = this.slaveCamera.transform.position + CameraController.PLATFORM_CAMERA_OFFSET;
          this.slaveCamera.cullingMask = this.m_cachedCullingMask;
          this.slaveCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
          this.slaveCamera.clearFlags = CameraClearFlags.Color;
          this.slaveCamera.backgroundColor = Color.clear;
          this.slaveCamera.targetTexture = temporary;
          this.slaveCamera.Render();
          this.slaveCamera.transform.position = this.slaveCamera.transform.position - CameraController.PLATFORM_CAMERA_OFFSET;
          Graphics.Blit((Texture) temporary, this.m_renderTarget);
          if ((Object) this.m_renderMaterial != (Object) null)
          {
            Graphics.Blit((Texture) source, temporary);
            Graphics.Blit((Texture) this.m_renderTarget, temporary, this.m_upsideDownMaterial);
            Graphics.Blit((Texture) temporary, target, this.m_renderMaterial);
          }
          else
          {
            Debug.LogError((object) "Failing...");
            Graphics.Blit((Texture) source, target);
          }
        }
        RenderTexture.ReleaseTemporary(temporary);
      }
    }

}
