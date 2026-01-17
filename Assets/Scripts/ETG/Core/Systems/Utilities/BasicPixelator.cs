// Decompiled with JetBrains decompiler
// Type: BasicPixelator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BasicPixelator : MonoBehaviour
    {
      private Vector3 FINAL_CAMERA_POSITION_OFFSET;
      private Camera m_camera;

      private void CheckSize()
      {
        if ((Object) this.m_camera == (Object) null)
          this.m_camera = this.GetComponent<Camera>();
        BraveCameraUtility.MaintainCameraAspect(this.m_camera);
        this.m_camera.orthographicSize = 135f / 16f;
      }

      private void OnEnable()
      {
        this.FINAL_CAMERA_POSITION_OFFSET = SystemInfo.graphicsDeviceType != GraphicsDeviceType.Direct3D11 ? new Vector3(1f / 32f, 1f / 32f, 0.0f) : Vector3.zero;
        this.transform.position += this.FINAL_CAMERA_POSITION_OFFSET;
      }

      private void OnDisable() => this.transform.position -= this.FINAL_CAMERA_POSITION_OFFSET;

      private void OnRenderImage(RenderTexture source, RenderTexture target)
      {
        this.CheckSize();
        RenderTexture temporary = RenderTexture.GetTemporary(BraveCameraUtility.H_PIXELS, BraveCameraUtility.V_PIXELS, 0, RenderTextureFormat.Default);
        if (!temporary.IsCreated())
          temporary.Create();
        Graphics.Blit((Texture) Pixelator.SmallBlackTexture, temporary);
        source.filterMode = FilterMode.Point;
        temporary.filterMode = FilterMode.Point;
        Graphics.Blit((Texture) source, temporary);
        Graphics.Blit((Texture) temporary, target);
        RenderTexture.ReleaseTemporary(temporary);
      }
    }

}
