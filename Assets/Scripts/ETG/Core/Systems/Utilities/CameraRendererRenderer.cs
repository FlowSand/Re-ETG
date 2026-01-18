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

