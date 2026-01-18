using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Correction (Ramp)")]
public class ColorCorrectionEffect : ImageEffectBase
    {
        public Texture textureRamp;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            this.material.SetTexture("_RampTex", this.textureRamp);
            Graphics.Blit((Texture) source, destination, this.material);
        }
    }

