// Decompiled with JetBrains decompiler
// Type: dfFontBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

