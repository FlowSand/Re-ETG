// Decompiled with JetBrains decompiler
// Type: CameraRendererRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class CameraRendererRenderer : MonoBehaviour
  {
    private RendererRenderer[] m_renderers;

    private void Start() => this.m_renderers = Object.FindObjectsOfType<RendererRenderer>();

    private void OnRenderImage(RenderTexture source, RenderTexture target)
    {
      this.m_renderers[0].transform.position += new Vector3(3f, 0.0f, 0.0f);
      for (int index = 0; index < this.m_renderers.Length; ++index)
        this.m_renderers[index].GetComponent<Renderer>().sharedMaterial.SetPass(0);
      this.m_renderers[0].transform.position -= new Vector3(3f, 0.0f, 0.0f);
      Graphics.Blit((Texture) source, target);
    }
  }

