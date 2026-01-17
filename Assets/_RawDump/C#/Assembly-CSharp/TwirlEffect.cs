// Decompiled with JetBrains decompiler
// Type: TwirlEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Twirl")]
public class TwirlEffect : ImageEffectBase
{
  public Vector2 radius = new Vector2(0.3f, 0.3f);
  public float angle = 50f;
  public Vector2 center = new Vector2(0.5f, 0.5f);

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    ImageEffects.RenderDistortion(this.material, source, destination, this.angle, this.center, this.radius);
  }
}
