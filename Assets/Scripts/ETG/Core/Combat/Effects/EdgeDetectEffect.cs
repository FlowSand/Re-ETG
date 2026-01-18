using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Edge Detection (Color)")]
public class EdgeDetectEffect : ImageEffectBase
    {
        public float threshold = 0.2f;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            this.material.SetFloat("_Treshold", this.threshold * this.threshold);
            Graphics.Blit((Texture) source, destination, this.material);
        }
    }

