// Decompiled with JetBrains decompiler
// Type: SENaturalBloomAndDirtyLens
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[RequireComponent(typeof (Camera))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Sonic Ether/SE Natural Bloom and Dirty Lens")]
public class SENaturalBloomAndDirtyLens : MonoBehaviour
  {
    [Range(0.0f, 0.4f)]
    public float bloomIntensity = 0.05f;
    public Shader shader;
    private Material material;
    public Texture2D lensDirtTexture;
    [Range(0.0f, 0.95f)]
    public float lensDirtIntensity = 0.05f;
    private bool isSupported;
    private float blurSize = 4f;
    public bool inputIsHDR;
    [HideInInspector]
    public bool overrideDisable;

    private void Start()
    {
      this.isSupported = true;
      if (!(bool) (Object) this.material)
        this.material = new Material(this.shader);
      if (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        return;
      this.isSupported = false;
    }

    private void OnDisable()
    {
      if (!(bool) (Object) this.material)
        return;
      Object.DestroyImmediate((Object) this.material);
    }

    protected int IterationCount
    {
      get
      {
        return !Application.isPlaying || GameManager.Options == null || GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH ? 1 : 2;
      }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (this.overrideDisable)
        return;
      if (!this.isSupported)
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        if (!(bool) (Object) this.material)
          this.material = new Material(this.shader);
        this.material.hideFlags = HideFlags.HideAndDontSave;
        this.material.SetFloat("_BloomIntensity", Mathf.Exp(this.bloomIntensity) - 1f);
        this.material.SetFloat("_LensDirtIntensity", Mathf.Exp(this.lensDirtIntensity) - 1f);
        source.filterMode = FilterMode.Bilinear;
        int width = source.width / 2;
        int height = source.height / 2;
        RenderTexture source1 = source;
        int iterationCount = this.IterationCount;
        for (int index1 = 0; index1 < 6; ++index1)
        {
          RenderTexture renderTexture1 = RenderTexture.GetTemporary(width, height, 0, source.format);
          renderTexture1.filterMode = FilterMode.Bilinear;
          Graphics.Blit((Texture) source1, renderTexture1, this.material, 1);
          source1 = renderTexture1;
          float num = index1 <= 1 ? 0.5f : 1f;
          if (index1 == 2)
            num = 0.75f;
          for (int index2 = 0; index2 < iterationCount; ++index2)
          {
            this.material.SetFloat("_BlurSize", (this.blurSize * 0.5f + (float) index2) * num);
            RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0, source.format);
            temporary1.filterMode = FilterMode.Bilinear;
            Graphics.Blit((Texture) renderTexture1, temporary1, this.material, 2);
            RenderTexture.ReleaseTemporary(renderTexture1);
            RenderTexture renderTexture2 = temporary1;
            RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, source.format);
            temporary2.filterMode = FilterMode.Bilinear;
            Graphics.Blit((Texture) renderTexture2, temporary2, this.material, 3);
            RenderTexture.ReleaseTemporary(renderTexture2);
            renderTexture1 = temporary2;
          }
          switch (index1)
          {
            case 0:
              this.material.SetTexture("_Bloom0", (Texture) renderTexture1);
              break;
            case 1:
              this.material.SetTexture("_Bloom1", (Texture) renderTexture1);
              break;
            case 2:
              this.material.SetTexture("_Bloom2", (Texture) renderTexture1);
              break;
            case 3:
              this.material.SetTexture("_Bloom3", (Texture) renderTexture1);
              break;
            case 4:
              this.material.SetTexture("_Bloom4", (Texture) renderTexture1);
              break;
            case 5:
              this.material.SetTexture("_Bloom5", (Texture) renderTexture1);
              break;
          }
          RenderTexture.ReleaseTemporary(renderTexture1);
          width /= 2;
          height /= 2;
        }
        this.material.SetTexture("_LensDirt", (Texture) this.lensDirtTexture);
        Graphics.Blit((Texture) source, destination, this.material, 0);
      }
    }
  }

