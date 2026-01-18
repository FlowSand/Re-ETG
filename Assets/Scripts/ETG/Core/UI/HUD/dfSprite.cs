// Decompiled with JetBrains decompiler
// Type: dfSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[AddComponentMenu("Daikon Forge/User Interface/Sprite/Basic")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_sprite.html")]
[dfTooltip("Used to render a sprite from a Texture Atlas on the screen")]
[Serializable]
public class dfSprite : dfControl
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
    protected dfAtlas atlas;
    [SerializeField]
    protected string spriteName;
    [SerializeField]
    protected dfSpriteFlip flip;
    [SerializeField]
    protected dfFillDirection fillDirection;
    [SerializeField]
    protected float fillAmount = 1f;
    [SerializeField]
    protected bool invertFill;
    [NonSerialized]
    public Material OverrideMaterial;

    public event PropertyChangedEventHandler<string> SpriteNameChanged;

    public dfAtlas Atlas
    {
      get
      {
        if ((UnityEngine.Object) this.atlas == (UnityEngine.Object) null)
        {
          dfGUIManager manager = this.GetManager();
          if ((UnityEngine.Object) manager != (UnityEngine.Object) null)
            return this.atlas = manager.DefaultAtlas;
        }
        return this.atlas;
      }
      set
      {
        if (dfAtlas.Equals(value, this.atlas))
          return;
        this.atlas = value;
        this.Invalidate();
      }
    }

    public string SpriteName
    {
      get => this.spriteName;
      set
      {
        value = this.getLocalizedValue(value);
        if (!(value != this.spriteName))
          return;
        this.spriteName = value;
        dfAtlas.ItemInfo spriteInfo = this.SpriteInfo;
        if (this.size == Vector2.zero && spriteInfo != (dfAtlas.ItemInfo) null)
        {
          this.size = spriteInfo.sizeInPixels;
          this.updateCollider();
        }
        this.Invalidate();
        this.OnSpriteNameChanged(value);
      }
    }

    public dfAtlas.ItemInfo SpriteInfo
    {
      get
      {
        return (UnityEngine.Object) this.Atlas == (UnityEngine.Object) null ? (dfAtlas.ItemInfo) null : this.Atlas[this.spriteName];
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

    protected internal override void OnLocalize()
    {
      base.OnLocalize();
      this.SpriteName = this.getLocalizedValue(this.spriteName);
    }

    protected internal virtual void OnSpriteNameChanged(string value)
    {
      this.Signal(nameof (OnSpriteNameChanged), (object) this, (object) value);
      if (this.SpriteNameChanged == null)
        return;
      this.SpriteNameChanged((dfControl) this, value);
    }

    public override Vector2 CalculateMinimumSize()
    {
      dfAtlas.ItemInfo spriteInfo = this.SpriteInfo;
      if (spriteInfo == (dfAtlas.ItemInfo) null)
        return Vector2.zero;
      RectOffset border = spriteInfo.border;
      return border != null && border.horizontal > 0 && border.vertical > 0 ? Vector2.Max(base.CalculateMinimumSize(), new Vector2((float) border.horizontal, (float) border.vertical)) : base.CalculateMinimumSize();
    }

    protected override void OnRebuildRenderData()
    {
      if (!((UnityEngine.Object) this.Atlas != (UnityEngine.Object) null) || !((UnityEngine.Object) this.Atlas.Material != (UnityEngine.Object) null) || this.SpriteInfo == (dfAtlas.ItemInfo) null)
        return;
      this.renderData.Material = this.OverrideMaterial ?? this.Atlas.Material;
      Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.disabledColor : this.color);
      dfSprite.renderSprite(this.renderData, new dfSprite.RenderOptions()
      {
        atlas = this.Atlas,
        color = color32,
        fillAmount = this.fillAmount,
        fillDirection = this.fillDirection,
        flip = this.flip,
        invertFill = this.invertFill,
        offset = this.pivot.TransformToUpperLeft(this.Size),
        pixelsToUnits = this.PixelsToUnits(),
        size = this.Size,
        spriteInfo = this.SpriteInfo
      });
    }

    internal static void renderSprite(dfRenderData data, dfSprite.RenderOptions options)
    {
      if ((double) options.fillAmount <= 1.4012984643248171E-45)
        return;
      options.baseIndex = data.Vertices.Count;
      dfSprite.rebuildTriangles(data, options);
      dfSprite.rebuildVertices(data, options);
      dfSprite.rebuildUV(data, options);
      dfSprite.rebuildColors(data, options);
      if ((double) options.fillAmount >= 1.0)
        return;
      dfSprite.doFill(data, options);
    }

    private static void rebuildTriangles(dfRenderData renderData, dfSprite.RenderOptions options)
    {
      int baseIndex = options.baseIndex;
      dfList<int> triangles = renderData.Triangles;
      triangles.EnsureCapacity(triangles.Count + dfSprite.TRIANGLE_INDICES.Length);
      for (int index = 0; index < dfSprite.TRIANGLE_INDICES.Length; ++index)
        triangles.Add(baseIndex + dfSprite.TRIANGLE_INDICES[index]);
    }

    private static void rebuildVertices(dfRenderData renderData, dfSprite.RenderOptions options)
    {
      dfList<Vector3> vertices = renderData.Vertices;
      int baseIndex = options.baseIndex;
      float x1 = 0.0f;
      float y1 = 0.0f;
      float x2 = Mathf.Ceil(options.size.x);
      float y2 = Mathf.Ceil(-options.size.y);
      vertices.Add(new Vector3(x1, y1, 0.0f) * options.pixelsToUnits);
      vertices.Add(new Vector3(x2, y1, 0.0f) * options.pixelsToUnits);
      vertices.Add(new Vector3(x2, y2, 0.0f) * options.pixelsToUnits);
      vertices.Add(new Vector3(x1, y2, 0.0f) * options.pixelsToUnits);
      Vector3 vector3 = options.offset.RoundToInt() * options.pixelsToUnits;
      Vector3[] items = vertices.Items;
      for (int index = baseIndex; index < baseIndex + 4; ++index)
        items[index] = (items[index] + vector3).Quantize(options.pixelsToUnits);
    }

    private static void rebuildColors(dfRenderData renderData, dfSprite.RenderOptions options)
    {
      dfList<Color32> colors = renderData.Colors;
      colors.Add(options.color);
      colors.Add(options.color);
      colors.Add(options.color);
      colors.Add(options.color);
    }

    private static void rebuildUV(dfRenderData renderData, dfSprite.RenderOptions options)
    {
      Rect region = options.spriteInfo.region;
      dfList<Vector2> uv = renderData.UV;
      uv.Add(new Vector2(region.x, region.yMax));
      uv.Add(new Vector2(region.xMax, region.yMax));
      uv.Add(new Vector2(region.xMax, region.y));
      uv.Add(new Vector2(region.x, region.y));
      Vector2 zero = Vector2.zero;
      if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
      {
        Vector2 vector2_1 = uv[1];
        uv[1] = uv[0];
        uv[0] = vector2_1;
        Vector2 vector2_2 = uv[3];
        uv[3] = uv[2];
        uv[2] = vector2_2;
      }
      if (!options.flip.IsSet(dfSpriteFlip.FlipVertical))
        return;
      Vector2 vector2_3 = uv[0];
      uv[0] = uv[3];
      uv[3] = vector2_3;
      Vector2 vector2_4 = uv[1];
      uv[1] = uv[2];
      uv[2] = vector2_4;
    }

    private static void doFill(dfRenderData renderData, dfSprite.RenderOptions options)
    {
      int baseIndex = options.baseIndex;
      dfList<Vector3> vertices = renderData.Vertices;
      dfList<Vector2> uv = renderData.UV;
      int index1 = baseIndex;
      int index2 = baseIndex + 1;
      int index3 = baseIndex + 3;
      int index4 = baseIndex + 2;
      if (options.invertFill)
      {
        if (options.fillDirection == dfFillDirection.Horizontal)
        {
          index1 = baseIndex + 1;
          index2 = baseIndex;
          index3 = baseIndex + 2;
          index4 = baseIndex + 3;
        }
        else
        {
          index1 = baseIndex + 3;
          index2 = baseIndex + 2;
          index3 = baseIndex;
          index4 = baseIndex + 1;
        }
      }
      if (options.fillDirection == dfFillDirection.Horizontal)
      {
        vertices[index2] = Vector3.Lerp(vertices[index2], vertices[index1], 1f - options.fillAmount);
        vertices[index4] = Vector3.Lerp(vertices[index4], vertices[index3], 1f - options.fillAmount);
        uv[index2] = Vector2.Lerp(uv[index2], uv[index1], 1f - options.fillAmount);
        uv[index4] = Vector2.Lerp(uv[index4], uv[index3], 1f - options.fillAmount);
      }
      else
      {
        vertices[index3] = Vector3.Lerp(vertices[index3], vertices[index1], 1f - options.fillAmount);
        vertices[index4] = Vector3.Lerp(vertices[index4], vertices[index2], 1f - options.fillAmount);
        uv[index3] = Vector2.Lerp(uv[index3], uv[index1], 1f - options.fillAmount);
        uv[index4] = Vector2.Lerp(uv[index4], uv[index2], 1f - options.fillAmount);
      }
    }

    public override string ToString()
    {
      return !string.IsNullOrEmpty(this.spriteName) ? $"{this.name} ({this.spriteName})" : base.ToString();
    }

    internal struct RenderOptions
    {
      public dfAtlas atlas;
      public dfAtlas.ItemInfo spriteInfo;
      public Color32 color;
      public float pixelsToUnits;
      public Vector2 size;
      public dfSpriteFlip flip;
      public bool invertFill;
      public dfFillDirection fillDirection;
      public float fillAmount;
      public Vector3 offset;
      public int baseIndex;
    }
  }

