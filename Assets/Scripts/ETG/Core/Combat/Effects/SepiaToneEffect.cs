using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Sepia Tone")]
public class SepiaToneEffect : ImageEffectBase
  {
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      Graphics.Blit((Texture) source, destination, this.material);
    }
  }

