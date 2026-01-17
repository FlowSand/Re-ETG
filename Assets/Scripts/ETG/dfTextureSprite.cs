// Decompiled with JetBrains decompiler
// Type: dfTextureSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_texture_sprite.html")]
[dfCategory("Basic Controls")]
[dfTooltip("Implements a Sprite that allows the user to use any Texture and Material they wish without having to use a Texture Atlas")]
[AddComponentMenu("Daikon Forge/User Interface/Sprite/Texture")]
[ExecuteInEditMode]
[Serializable]
public class dfTextureSprite : dfControl
{
  private static int[] TRIANGLE_INDICES = new int[6]
  {
    0,
    1,
    3,
    3,
    1,
    2
  };
  [SerializeField]
  protected Texture texture;
  [SerializeField]
  protected Material material;
  [SerializeField]
  protected dfSpriteFlip flip;
  [SerializeField]
  protected dfFillDirection fillDirection;
  [SerializeField]
  protected float fillAmount = 1f;
  [SerializeField]
  protected bool invertFill;
  [SerializeField]
  protected Rect cropRect = new Rect(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  protected bool cropImage;
  private bool createdRuntimeMaterial;
  private Material renderMaterial;
  [NonSerialized]
  public Material OverrideMaterial;

  public event PropertyChangedEventHandler<Texture> TextureChanged;

  public bool CropTexture
  {
    get => this.cropImage;
    set
    {
      if (value == this.cropImage)
        return;
      this.cropImage = value;
      this.Invalidate();
    }
  }

  public Rect CropRect
  {
    get => this.cropRect;
    set
    {
      value = this.validateCropRect(value);
      if (!(value != this.cropRect))
        return;
      this.cropRect = value;
      this.Invalidate();
    }
  }

  public Texture Texture
  {
    get => this.texture;
    set
    {
      if (!((UnityEngine.Object) value != (UnityEngine.Object) this.texture))
        return;
      this.texture = value;
      this.Invalidate();
      if ((UnityEngine.Object) value != (UnityEngine.Object) null && (double) this.size.sqrMagnitude <= 1.4012984643248171E-45)
        this.size = new Vector2((float) value.width, (float) value.height);
      this.OnTextureChanged(value);
    }
  }

  public Material Material
  {
    get => this.material;
    set
    {
      if (!((UnityEngine.Object) value != (UnityEngine.Object) this.material))
        return;
      this.disposeCreatedMaterial();
      this.renderMaterial = (Material) null;
      this.material = value;
      this.Invalidate();
    }
  }

  public dfSpriteFlip Flip
  {
    get => this.flip;
    set
    {
      if (value == this.flip)
        return;
      this.flip = value;
      this.Invalidate();
    }
  }

  public dfFillDirection FillDirection
  {
    get => this.fillDirection;
    set
    {
      if (value == this.fillDirection)
        return;
      this.fillDirection = value;
      this.Invalidate();
    }
  }

  public float FillAmount
  {
    get => this.fillAmount;
    set
    {
      if (Mathf.Approximately(value, this.fillAmount))
        return;
      this.fillAmount = Mathf.Max(0.0f, Mathf.Min(1f, value));
      this.Invalidate();
    }
  }

  public bool InvertFill
  {
    get => this.invertFill;
    set
    {
      if (value == this.invertFill)
        return;
      this.invertFill = value;
      this.Invalidate();
    }
  }

  public Material RenderMaterial => this.renderMaterial;

  public override void OnEnable()
  {
    base.OnEnable();
    this.renderMaterial = (Material) null;
  }

  public override void OnDestroy()
  {
    this.disposeCreatedMaterial();
    base.OnDestroy();
    if (!((UnityEngine.Object) this.renderMaterial != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.renderMaterial);
    this.renderMaterial = (Material) null;
  }

  public override void OnDisable()
  {
    base.OnDisable();
    if (!Application.isPlaying || !((UnityEngine.Object) this.renderMaterial != (UnityEngine.Object) null))
      return;
    this.disposeCreatedMaterial();
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.renderMaterial);
    this.renderMaterial = (Material) null;
  }

  protected override void OnRebuildRenderData()
  {
    base.OnRebuildRenderData();
    if ((UnityEngine.Object) this.texture == (UnityEngine.Object) null)
      return;
    this.ensureMaterial();
    if ((UnityEngine.Object) this.material == (UnityEngine.Object) null)
      return;
    if ((UnityEngine.Object) this.renderMaterial == (UnityEngine.Object) null)
    {
      Material material = new Material(this.material);
      material.hideFlags = HideFlags.DontSave;
      material.name = this.material.name + " (copy)";
      this.renderMaterial = material;
    }
    this.renderMaterial.mainTexture = this.texture;
    this.renderData.Material = this.OverrideMaterial ?? this.renderMaterial;
    float units = this.PixelsToUnits();
    float x1 = 0.0f;
    float y1 = 0.0f;
    float x2 = this.size.x * units;
    float y2 = -this.size.y * units;
    Vector3 vector3 = this.pivot.TransformToUpperLeft(this.size).RoundToInt() * units;
    this.renderData.Vertices.Add(new Vector3(x1, y1, 0.0f) + vector3);
    this.renderData.Vertices.Add(new Vector3(x2, y1, 0.0f) + vector3);
    this.renderData.Vertices.Add(new Vector3(x2, y2, 0.0f) + vector3);
    this.renderData.Vertices.Add(new Vector3(x1, y2, 0.0f) + vector3);
    this.renderData.Triangles.AddRange(dfTextureSprite.TRIANGLE_INDICES);
    this.rebuildUV(this.renderData);
    Color32 color32 = this.ApplyOpacity(this.color);
    this.renderData.Colors.Add(color32);
    this.renderData.Colors.Add(color32);
    this.renderData.Colors.Add(color32);
    this.renderData.Colors.Add(color32);
    if ((double) this.fillAmount >= 1.0)
      return;
    this.doFill(this.renderData);
  }

  protected virtual void disposeCreatedMaterial()
  {
    if (!this.createdRuntimeMaterial)
      return;
    UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.material);
    this.material = (Material) null;
    this.createdRuntimeMaterial = false;
  }

  protected virtual void rebuildUV(dfRenderData renderBuffer)
  {
    dfList<Vector2> uv = renderBuffer.UV;
    if (this.cropImage)
    {
      int width = this.texture.width;
      int height = this.texture.height;
      float num1 = Mathf.Max(0.0f, Mathf.Min(this.cropRect.x, (float) width));
      float num2 = Mathf.Max(0.0f, Mathf.Min(this.cropRect.xMax, (float) width));
      float num3 = Mathf.Max(0.0f, Mathf.Min(this.cropRect.y, (float) height));
      float num4 = Mathf.Max(0.0f, Mathf.Min(this.cropRect.yMax, (float) height));
      uv.Add(new Vector2(num1 / (float) width, num4 / (float) height));
      uv.Add(new Vector2(num2 / (float) width, num4 / (float) height));
      uv.Add(new Vector2(num2 / (float) width, num3 / (float) height));
      uv.Add(new Vector2(num1 / (float) width, num3 / (float) height));
    }
    else
    {
      uv.Add(new Vector2(0.0f, 1f));
      uv.Add(new Vector2(1f, 1f));
      uv.Add(new Vector2(1f, 0.0f));
      uv.Add(new Vector2(0.0f, 0.0f));
    }
    Vector2 zero = Vector2.zero;
    if (this.flip.IsSet(dfSpriteFlip.FlipHorizontal))
    {
      Vector2 vector2_1 = uv[1];
      uv[1] = uv[0];
      uv[0] = vector2_1;
      Vector2 vector2_2 = uv[3];
      uv[3] = uv[2];
      uv[2] = vector2_2;
    }
    if (!this.flip.IsSet(dfSpriteFlip.FlipVertical))
      return;
    Vector2 vector2_3 = uv[0];
    uv[0] = uv[3];
    uv[3] = vector2_3;
    Vector2 vector2_4 = uv[1];
    uv[1] = uv[2];
    uv[2] = vector2_4;
  }

  protected virtual void doFill(dfRenderData renderData)
  {
    dfList<Vector3> vertices = renderData.Vertices;
    dfList<Vector2> uv = renderData.UV;
    int index1 = 0;
    int index2 = 1;
    int index3 = 3;
    int index4 = 2;
    if (this.invertFill)
    {
      if (this.fillDirection == dfFillDirection.Horizontal)
      {
        index1 = 1;
        index2 = 0;
        index3 = 2;
        index4 = 3;
      }
      else
      {
        index1 = 3;
        index2 = 2;
        index3 = 0;
        index4 = 1;
      }
    }
    if (this.fillDirection == dfFillDirection.Horizontal)
    {
      vertices[index2] = Vector3.Lerp(vertices[index2], vertices[index1], 1f - this.fillAmount);
      vertices[index4] = Vector3.Lerp(vertices[index4], vertices[index3], 1f - this.fillAmount);
      uv[index2] = Vector2.Lerp(uv[index2], uv[index1], 1f - this.fillAmount);
      uv[index4] = Vector2.Lerp(uv[index4], uv[index3], 1f - this.fillAmount);
    }
    else
    {
      vertices[index3] = Vector3.Lerp(vertices[index3], vertices[index1], 1f - this.fillAmount);
      vertices[index4] = Vector3.Lerp(vertices[index4], vertices[index2], 1f - this.fillAmount);
      uv[index3] = Vector2.Lerp(uv[index3], uv[index1], 1f - this.fillAmount);
      uv[index4] = Vector2.Lerp(uv[index4], uv[index2], 1f - this.fillAmount);
    }
  }

  private Rect validateCropRect(Rect rect)
  {
    if ((UnityEngine.Object) this.texture == (UnityEngine.Object) null)
      return new Rect();
    int width = this.texture.width;
    int height = this.texture.height;
    return new Rect(Mathf.Max(0.0f, Mathf.Min(rect.x, (float) width)), Mathf.Max(0.0f, Mathf.Min(rect.y, (float) height)), Mathf.Max(0.0f, Mathf.Min(rect.width, (float) width)), Mathf.Max(0.0f, Mathf.Min(rect.height, (float) height)));
  }

  protected internal virtual void OnTextureChanged(Texture value)
  {
    this.SignalHierarchy(nameof (OnTextureChanged), (object) this, (object) value);
    if (this.TextureChanged == null)
      return;
    this.TextureChanged((dfControl) this, value);
  }

  private void ensureMaterial()
  {
    if ((UnityEngine.Object) this.material != (UnityEngine.Object) null || (UnityEngine.Object) this.texture == (UnityEngine.Object) null)
      return;
    Shader shader = Shader.Find("Daikon Forge/Default UI Shader");
    if ((UnityEngine.Object) shader == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Failed to find default shader");
    }
    else
    {
      Material material = new Material(shader);
      material.name = "Default Texture Shader";
      material.hideFlags = HideFlags.DontSave;
      material.mainTexture = this.texture;
      this.material = material;
      this.createdRuntimeMaterial = true;
    }
  }
}
