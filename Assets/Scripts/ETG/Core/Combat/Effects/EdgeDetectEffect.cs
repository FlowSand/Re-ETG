// Decompiled with JetBrains decompiler
// Type: EdgeDetectEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

