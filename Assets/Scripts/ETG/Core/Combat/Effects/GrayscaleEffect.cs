// Decompiled with JetBrains decompiler
// Type: GrayscaleEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

