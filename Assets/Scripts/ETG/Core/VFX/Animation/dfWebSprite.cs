using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[dfTooltip("Downloads an image from a web URL and displays it on-screen like a sprite")]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_web_sprite.html")]
[AddComponentMenu("Daikon Forge/User Interface/Sprite/Web")]
[Serializable]
public class dfWebSprite : dfTextureSprite
  {
    public PropertyChangedEventHandler<Texture> DownloadComplete;
    public PropertyChangedEventHandler<string> DownloadError;
    [SerializeField]
    protected string url = string.Empty;
    [SerializeField]
    protected Texture2D loadingImage;
    [SerializeField]
    protected Texture2D errorImage;
    [SerializeField]
    protected bool autoDownload = true;

    public string URL
    {
      get => this.url;
      set
      {
        if (!(value != this.url))
          return;
        this.url = value;
        if (!Application.isPlaying || !this.AutoDownload)
          return;
        this.LoadImage();
      }
    }

    public Texture2D LoadingImage
    {
      get => this.loadingImage;
      set => this.loadingImage = value;
    }

    public Texture2D ErrorImage
    {
      get => this.errorImage;
      set => this.errorImage = value;
    }

    public bool AutoDownload
    {
      get => this.autoDownload;
      set => this.autoDownload = value;
    }

    public override void OnEnable()
    {
      base.OnEnable();
      if ((UnityEngine.Object) this.Texture == (UnityEngine.Object) null)
        this.Texture = (Texture) this.LoadingImage;
      if (!((UnityEngine.Object) this.Texture == (UnityEngine.Object) null) || !this.AutoDownload || !Application.isPlaying)
        return;
      this.LoadImage();
    }

    public void LoadImage()
    {
      this.StopAllCoroutines();
      this.StartCoroutine(this.downloadTexture());
    }

    [DebuggerHidden]
    private IEnumerator downloadTexture()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new dfWebSprite__downloadTexturec__Iterator0()
      {
        _this = this
      };
    }
  }

