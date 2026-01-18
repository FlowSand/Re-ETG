// Decompiled with JetBrains decompiler
// Type: GenericFullscreenEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class GenericFullscreenEffect : MonoBehaviour
  {
    public Shader shader;
    public bool dualPass;
    public Material materialInstance;
    private bool m_cacheCurrentFrameToBuffer;
    [SerializeField]
    protected Material m_material;
    private RenderTexture m_cachedFrame;

    public bool CacheCurrentFrameToBuffer
    {
      get => this.m_cacheCurrentFrameToBuffer;
      set => this.m_cacheCurrentFrameToBuffer = value;
    }

    public Material ActiveMaterial => this.m_material;

    private void Awake()
    {
      if ((Object) this.materialInstance != (Object) null)
        this.m_material = this.materialInstance;
      else
        this.m_material = new Material(this.shader);
    }

    public void SetMaterial(Material m) => this.m_material = m;

    public RenderTexture GetCachedFrame() => this.m_cachedFrame;

    public void ClearCachedFrame()
    {
      if ((Object) this.m_cachedFrame != (Object) null)
        RenderTexture.ReleaseTemporary(this.m_cachedFrame);
      this.m_cachedFrame = (RenderTexture) null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture target)
    {
      if (!this.dualPass)
      {
        Graphics.Blit((Texture) source, target, this.m_material);
      }
      else
      {
        RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit((Texture) source, temporary, this.m_material, 0);
        Graphics.Blit((Texture) temporary, target, this.m_material, 1);
        RenderTexture.ReleaseTemporary(temporary);
      }
      if (!this.CacheCurrentFrameToBuffer)
        return;
      this.ClearCachedFrame();
      this.m_cachedFrame = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
      this.m_cachedFrame.filterMode = FilterMode.Point;
      Graphics.Blit((Texture) source, this.m_cachedFrame, this.m_material);
      this.CacheCurrentFrameToBuffer = false;
    }
  }

