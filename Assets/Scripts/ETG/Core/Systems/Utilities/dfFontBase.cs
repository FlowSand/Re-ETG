using System;
using UnityEngine;

#nullable disable

[Serializable]
  public abstract class dfFontBase : MonoBehaviour
  {
    private bool m_hasCachedScaling;
    private bool m_cachedScaling;

    public abstract Material Material { get; set; }

    public abstract Texture Texture { get; }

    public abstract bool IsValid { get; }

    public abstract int FontSize { get; set; }

    public abstract int LineHeight { get; set; }

    public abstract dfFontRendererBase ObtainRenderer();

    public bool IsSpriteScaledUIFont()
    {
      if (!this.m_hasCachedScaling)
        this.m_cachedScaling = this.name == "04b03_df40";
      return this.m_cachedScaling;
    }
  }

