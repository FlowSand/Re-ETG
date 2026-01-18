using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Grayscale")]
public class GrayscaleEffect : ImageEffectBase
  {
    public Texture textureRamp;
    public float rampOffset;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.material.SetTexture("_RampTex", this.textureRamp);
      this.material.SetFloat("_RampOffset", this.rampOffset);
      Graphics.Blit((Texture) source, destination, this.material);
    }
  }

