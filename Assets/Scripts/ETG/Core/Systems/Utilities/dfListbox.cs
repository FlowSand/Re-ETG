// Decompiled with JetBrains decompiler
// Type: dfListbox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/User Interface/Listbox")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_listbox.html")]
[dfCategory("Basic Controls")]
[dfTooltip("Allows the user to select from a list of options")]
[Serializable]
public class dfListbox : dfInteractiveBase, IDFMultiRender, IRendersText
  {
    [SerializeField]
    protected dfFontBase font;
    [SerializeField]
    protected RectOffset listPadding = new RectOffset();
    [SerializeField]
    protected int selectedIndex = -1;
    [SerializeField]
    protected Color32 itemTextColor = (Color32) UnityEngine.Color.white;
    [SerializeField]
    protected float itemTextScale = 1f;
    [SerializeField]
    protected int itemHeight = 25;
    [SerializeField]
    protected RectOffset itemPadding = new RectOffset();
    [SerializeField]
    protected string[] items = new string[0];
    [SerializeField]
    protected string itemHighlight = string.Empty;
    [SerializeField]
    protected string itemHover = string.Empty;
    [SerializeField]
    protected dfScrollbar scrollbar;
    [SerializeField]
    protected bool animateHover;
    [SerializeField]
    protected bool shadow;
    [SerializeField]
    protected dfTextScaleMode textScaleMode;
    [SerializeField]
    protected Color32 shadowColor = (Color32) UnityEngine.Color.black;
    [SerializeField]
    protected Vector2 shadowOffset = new Vector2(1f, -1f);
    [SerializeField]
    protected TextAlignment itemAlignment;
    private bool isFontCallbackAssigned;
    private bool eventsAttached;
    private float scrollPosition;
    private int hoverIndex = -1;
    private float hoverTweenLocation;
    private Vector2 touchStartPosition = Vector2.zero;
    private Vector2 startSize = Vector2.zero;
    private dfRenderData textRenderData;
    private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();

    public event PropertyChangedEventHandler<int> SelectedIndexChanged;

    public event PropertyChangedEventHandler<int> ItemClicked;

    public dfFontBase Font
    {
      get
      {
        if ((UnityEngine.Object) this.font == (UnityEngine.Object) null)
        {
          dfGUIManager manager = this.GetManager();
          if ((UnityEngine.Object) manager != (UnityEngine.Object) null)
            this.font = manager.DefaultFont;
        }
        return this.font;
      }
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.font))
          return;
        this.unbindTextureRebuildCallback();
        this.font = value;
        this.bindTextureRebuildCallback();
        this.Invalidate();
      }
    }

    public float ScrollPosition
    {
      get => this.scrollPosition;
      set
      {
        if (Mathf.Approximately(value, this.scrollPosition))
          return;
        this.scrollPosition = this.constrainScrollPosition(value);
        this.Invalidate();
      }
    }

    public TextAlignment ItemAlignment
    {
      get => this.itemAlignment;
      set
      {
        if (value == this.itemAlignment)
          return;
        this.itemAlignment = value;
        this.Invalidate();
      }
    }

    public string ItemHighlight
    {
      get => this.itemHighlight;
      set
      {
        if (!(value != this.itemHighlight))
          return;
        this.itemHighlight = value;
        this.Invalidate();
      }
    }

    public string ItemHover
    {
      get => this.itemHover;
      set
      {
        if (!(value != this.itemHover))
          return;
        this.itemHover = value;
        this.Invalidate();
      }
    }

    public string SelectedItem
    {
      get => this.selectedIndex == -1 ? (string) null : this.items[this.selectedIndex];
    }

    public string SelectedValue
    {
      get => this.items[this.selectedIndex];
      set
      {
        this.selectedIndex = -1;
        for (int index = 0; index < this.items.Length; ++index)
        {
          if (this.items[index] == value)
          {
            this.selectedIndex = index;
            break;
          }
        }
      }
    }

    public int SelectedIndex
    {
      get => this.selectedIndex;
      set
      {
        value = Mathf.Max(-1, value);
        value = Mathf.Min(this.items.Length - 1, value);
        if (value == this.selectedIndex)
          return;
        this.selectedIndex = value;
        this.EnsureVisible(value);
        this.OnSelectedIndexChanged();
        this.Invalidate();
      }
    }

    public RectOffset ItemPadding
    {
      get
      {
        if (this.itemPadding == null)
          this.itemPadding = new RectOffset();
        return this.itemPadding;
      }
      set
      {
        value = value.ConstrainPadding();
        if (value.Equals((object) this.itemPadding))
          return;
        this.itemPadding = value;
        this.Invalidate();
      }
    }

    public Color32 ItemTextColor
    {
      get => this.itemTextColor;
      set
      {
        if (value.Equals((object) this.itemTextColor))
          return;
        this.itemTextColor = value;
        this.Invalidate();
      }
    }

    public float ItemTextScale
    {
      get => this.itemTextScale;
      set
      {
        value = Mathf.Max(0.1f, value);
        if (Mathf.Approximately(this.itemTextScale, value))
          return;
        this.itemTextScale = value;
        this.Invalidate();
      }
    }

    public int ItemHeight
    {
      get => this.itemHeight;
      set
      {
        this.scrollPosition = 0.0f;
        value = Mathf.Max(1, value);
        if (value == this.itemHeight)
          return;
        this.itemHeight = value;
        this.Invalidate();
      }
    }

    public string[] Items
    {
      get
      {
        if (this.items == null)
          this.items = new string[0];
        return this.items;
      }
      set
      {
        if (value == this.items)
          return;
        this.scrollPosition = 0.0f;
        if (value == null)
          value = new string[0];
        this.items = value;
        this.Invalidate();
      }
    }

    public dfScrollbar Scrollbar
    {
      get => this.scrollbar;
      set
      {
        this.scrollPosition = 0.0f;
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.scrollbar))
          return;
        this.detachScrollbarEvents();
        this.scrollbar = value;
        this.attachScrollbarEvents();
        this.Invalidate();
      }
    }

    public RectOffset ListPadding
    {
      get
      {
        if (this.listPadding == null)
          this.listPadding = new RectOffset();
        return this.listPadding;
      }
      set
      {
        value = value.ConstrainPadding();
        if (object.Equals((object) value, (object) this.listPadding))
          return;
        this.listPadding = value;
        this.Invalidate();
      }
    }

    public bool Shadow
    {
      get => this.shadow;
      set
      {
        if (value == this.shadow)
          return;
        this.shadow = value;
        this.Invalidate();
      }
    }

    public Color32 ShadowColor
    {
      get => this.shadowColor;
      set
      {
        if (value.Equals((object) this.shadowColor))
          return;
        this.shadowColor = value;
        this.Invalidate();
      }
    }

    public Vector2 ShadowOffset
    {
      get => this.shadowOffset;
      set
      {
        if (!(value != this.shadowOffset))
          return;
        this.shadowOffset = value;
        this.Invalidate();
      }
    }

    public bool AnimateHover
    {
      get => this.animateHover;
      set => this.animateHover = value;
    }

    public dfTextScaleMode TextScaleMode
    {
      get => this.textScaleMode;
      set
      {
        this.textScaleMode = value;
        this.Invalidate();
      }
    }

    public override void Awake()
    {
      base.Awake();
      this.startSize = this.Size;
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.size.magnitude == 0.0)
        this.size = new Vector2(200f, 150f);
      if (this.animateHover && this.hoverIndex != -1 && (double) Mathf.Abs(this.hoverTweenLocation - (float) (this.hoverIndex * this.itemHeight) * this.PixelsToUnits()) < 1.0)
        this.Invalidate();
      if (!this.isControlInvalidated)
        return;
      this.synchronizeScrollbar();
    }

    public override void LateUpdate()
    {
      base.LateUpdate();
      if (!Application.isPlaying)
        return;
      this.attachScrollbarEvents();
    }

    public override void OnEnable()
    {
      base.OnEnable();
      this.bindTextureRebuildCallback();
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      this.detachScrollbarEvents();
    }

    public override void OnDisable()
    {
      base.OnDisable();
      this.unbindTextureRebuildCallback();
      this.detachScrollbarEvents();
    }

    protected internal override void OnLocalize()
    {
      base.OnLocalize();
      bool flag = false;
      for (int index = 0; index < this.items.Length; ++index)
      {
        string localizedValue = this.getLocalizedValue(this.items[index]);
        if (localizedValue != this.items[index])
        {
          flag = true;
          this.items[index] = localizedValue;
        }
      }
      if (!flag)
        return;
      this.Invalidate();
    }

    protected internal virtual void OnSelectedIndexChanged()
    {
      this.SignalHierarchy(nameof (OnSelectedIndexChanged), (object) this, (object) this.selectedIndex);
      if (this.SelectedIndexChanged == null)
        return;
      this.SelectedIndexChanged((dfControl) this, this.selectedIndex);
    }

    protected internal virtual void OnItemClicked()
    {
      this.Signal(nameof (OnItemClicked), (object) this, (object) this.selectedIndex);
      if (this.ItemClicked == null)
        return;
      this.ItemClicked((dfControl) this, this.selectedIndex);
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
      base.OnMouseMove(args);
      if (args is dfTouchEventArgs)
      {
        if ((double) Mathf.Abs(args.Position.y - this.touchStartPosition.y) < (double) (this.itemHeight / 2))
          return;
        this.ScrollPosition = Mathf.Max(0.0f, this.ScrollPosition + args.MoveDelta.y);
        this.synchronizeScrollbar();
        this.hoverIndex = -1;
      }
      else
        this.updateItemHover(args);
    }

    protected internal override void OnMouseEnter(dfMouseEventArgs args)
    {
      base.OnMouseEnter(args);
      this.touchStartPosition = args.Position;
    }

    protected internal override void OnMouseLeave(dfMouseEventArgs args)
    {
      base.OnMouseLeave(args);
      this.hoverIndex = -1;
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
      base.OnMouseWheel(args);
      this.ScrollPosition = Mathf.Max(0.0f, this.ScrollPosition - (float) ((int) args.WheelDelta * this.ItemHeight));
      this.synchronizeScrollbar();
      this.updateItemHover(args);
    }

    protected internal override void OnMouseUp(dfMouseEventArgs args)
    {
      this.hoverIndex = -1;
      base.OnMouseUp(args);
      if (!(args is dfTouchEventArgs) || (double) Mathf.Abs(args.Position.y - this.touchStartPosition.y) >= (double) this.itemHeight)
        return;
      this.selectItemUnderMouse(args);
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
      base.OnMouseDown(args);
      if (args is dfTouchEventArgs)
        this.touchStartPosition = args.Position;
      else
        this.selectItemUnderMouse(args);
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
      switch (args.KeyCode)
      {
        case KeyCode.UpArrow:
          this.SelectedIndex = Mathf.Max(0, this.selectedIndex - 1);
          break;
        case KeyCode.DownArrow:
          ++this.SelectedIndex;
          break;
        case KeyCode.Home:
          this.SelectedIndex = 0;
          break;
        case KeyCode.End:
          this.SelectedIndex = this.items.Length;
          break;
        case KeyCode.PageUp:
          this.SelectedIndex = Mathf.Max(0, this.SelectedIndex - Mathf.FloorToInt((this.size.y - (float) this.listPadding.vertical) / (float) this.itemHeight));
          break;
        case KeyCode.PageDown:
          this.SelectedIndex += Mathf.FloorToInt((this.size.y - (float) this.listPadding.vertical) / (float) this.itemHeight);
          break;
      }
      base.OnKeyDown(args);
    }

    public void AddItem(string item)
    {
      string[] destinationArray = new string[this.items.Length + 1];
      Array.Copy((Array) this.items, (Array) destinationArray, this.items.Length);
      destinationArray[this.items.Length] = item;
      this.items = destinationArray;
      this.Invalidate();
    }

    public void EnsureVisible(int index)
    {
      int num1 = index * this.ItemHeight;
      if ((double) this.scrollPosition > (double) num1)
        this.ScrollPosition = (float) num1;
      float num2 = this.size.y - (float) this.listPadding.vertical;
      if ((double) this.scrollPosition + (double) num2 >= (double) (num1 + this.itemHeight))
        return;
      this.ScrollPosition = (float) num1 - num2 + (float) this.itemHeight;
    }

    private void selectItemUnderMouse(dfMouseEventArgs args)
    {
      float num1 = this.pivot.TransformToUpperLeft(this.Size).y + ((float) -this.itemHeight * ((float) this.selectedIndex - this.scrollPosition) - (float) this.listPadding.top);
      float num2 = (float) ((double) this.selectedIndex - (double) this.scrollPosition + 1.0) * (float) this.itemHeight + (float) this.listPadding.vertical - this.size.y;
      if ((double) num2 > 0.0)
      {
        float num3 = num1 + num2;
      }
      float num4 = this.GetHitPosition(args).y - (float) this.listPadding.top;
      if ((double) num4 < 0.0 || (double) num4 > (double) this.size.y - (double) this.listPadding.bottom)
        return;
      this.SelectedIndex = (int) (((double) this.scrollPosition + (double) num4) / (double) this.itemHeight);
      this.OnItemClicked();
    }

    private void renderHover()
    {
      if (!Application.isPlaying || (UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || !this.IsEnabled || this.hoverIndex < 0 || this.hoverIndex > this.items.Length - 1 || string.IsNullOrEmpty(this.ItemHover))
        return;
      dfAtlas.ItemInfo atla = this.Atlas[this.ItemHover];
      if (atla == (dfAtlas.ItemInfo) null)
        return;
      Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.Size);
      Vector3 vector3 = new Vector3(upperLeft.x + (float) this.listPadding.left, upperLeft.y - (float) this.listPadding.top + this.scrollPosition, 0.0f);
      float units = this.PixelsToUnits();
      int target = this.hoverIndex * this.itemHeight;
      if (this.animateHover)
      {
        float num1 = Mathf.Abs(this.hoverTweenLocation - (float) target);
        float num2 = (float) (((double) this.size.y - (double) this.listPadding.vertical) * 0.5);
        if ((double) num1 > (double) num2)
          this.hoverTweenLocation = (float) target + Mathf.Sign(this.hoverTweenLocation - (float) target) * num2;
        float maxDelta = (float) ((double) BraveTime.DeltaTime / (double) units * 2.0);
        this.hoverTweenLocation = Mathf.MoveTowards(this.hoverTweenLocation, (float) target, maxDelta);
      }
      else
        this.hoverTweenLocation = (float) target;
      vector3.y -= this.hoverTweenLocation.Quantize(units);
      Color32 color32 = this.ApplyOpacity(this.color);
      dfSprite.RenderOptions options = new dfSprite.RenderOptions()
      {
        atlas = this.atlas,
        color = color32,
        fillAmount = 1f,
        flip = dfSpriteFlip.None,
        pixelsToUnits = this.PixelsToUnits(),
        size = (Vector2) new Vector3(this.size.x - (float) this.listPadding.horizontal, (float) this.itemHeight),
        spriteInfo = atla,
        offset = vector3
      };
      if (atla.border.horizontal > 0 || atla.border.vertical > 0)
        dfSlicedSprite.renderSprite(this.renderData, options);
      else
        dfSprite.renderSprite(this.renderData, options);
      if ((double) target == (double) this.hoverTweenLocation)
        return;
      this.Invalidate();
    }

    private void renderSelection()
    {
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || this.selectedIndex < 0)
        return;
      dfAtlas.ItemInfo atla = this.Atlas[this.ItemHighlight];
      if (atla == (dfAtlas.ItemInfo) null)
        return;
      float units = this.PixelsToUnits();
      Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.Size);
      Vector3 vector3 = new Vector3(upperLeft.x + (float) this.listPadding.left, upperLeft.y - (float) this.listPadding.top + this.scrollPosition, 0.0f);
      vector3.y -= (float) (this.selectedIndex * this.itemHeight);
      Color32 color32 = this.ApplyOpacity(this.color);
      dfSprite.RenderOptions options = new dfSprite.RenderOptions()
      {
        atlas = this.atlas,
        color = color32,
        fillAmount = 1f,
        flip = dfSpriteFlip.None,
        pixelsToUnits = units,
        size = (Vector2) new Vector3(this.size.x - (float) this.listPadding.horizontal, (float) this.itemHeight),
        spriteInfo = atla,
        offset = vector3
      };
      if (atla.border.horizontal > 0 || atla.border.vertical > 0)
        dfSlicedSprite.renderSprite(this.renderData, options);
      else
        dfSprite.renderSprite(this.renderData, options);
    }

    private float getTextScaleMultiplier()
    {
      if (this.textScaleMode == dfTextScaleMode.None || !Application.isPlaying)
        return 1f;
      return this.textScaleMode == dfTextScaleMode.ScreenResolution ? (float) Screen.height / (float) this.cachedManager.FixedHeight : this.Size.y / this.startSize.y;
    }

    private void renderItems(dfRenderData buffer)
    {
      if ((UnityEngine.Object) this.font == (UnityEngine.Object) null || this.items == null || this.items.Length == 0)
        return;
      float units = this.PixelsToUnits();
      Vector2 vector2 = new Vector2(this.size.x - (float) this.itemPadding.horizontal - (float) this.listPadding.horizontal, (float) (this.itemHeight - this.itemPadding.vertical));
      Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.Size);
      Vector3 vector3 = new Vector3(upperLeft.x + (float) this.itemPadding.left + (float) this.listPadding.left, upperLeft.y - (float) this.itemPadding.top - (float) this.listPadding.top, 0.0f) * units;
      vector3.y += this.scrollPosition * units;
      Color32 color32 = !this.IsEnabled ? this.DisabledColor : this.ItemTextColor;
      float num1 = upperLeft.y * units;
      float num2 = num1 - this.size.y * units;
      for (int index = 0; index < this.items.Length; ++index)
      {
        using (dfFontRendererBase renderer = this.font.ObtainRenderer())
        {
          renderer.WordWrap = false;
          renderer.MaxSize = vector2;
          renderer.PixelRatio = units;
          renderer.TextScale = this.ItemTextScale * this.getTextScaleMultiplier();
          renderer.VectorOffset = vector3;
          renderer.MultiLine = false;
          renderer.TextAlign = this.ItemAlignment;
          renderer.ProcessMarkup = true;
          renderer.DefaultColor = color32;
          renderer.OverrideMarkupColors = false;
          renderer.Opacity = this.CalculateOpacity();
          renderer.Shadow = this.Shadow;
          renderer.ShadowColor = this.ShadowColor;
          renderer.ShadowOffset = this.ShadowOffset;
          if (renderer is dfDynamicFont.DynamicFontRenderer dynamicFontRenderer)
          {
            dynamicFontRenderer.SpriteAtlas = this.Atlas;
            dynamicFontRenderer.SpriteBuffer = this.renderData;
          }
          if ((double) vector3.y - (double) this.itemHeight * (double) units <= (double) num1)
            renderer.Render(this.items[index], buffer);
          vector3.y -= (float) this.itemHeight * units;
          renderer.VectorOffset = vector3;
          if ((double) vector3.y < (double) num2)
            break;
        }
      }
    }

    private void clipQuads(dfRenderData buffer, int startIndex)
    {
      dfList<Vector3> vertices = buffer.Vertices;
      dfList<Vector2> uv = buffer.UV;
      float units = this.PixelsToUnits();
      float a = (this.Pivot.TransformToUpperLeft(this.Size).y - (float) this.listPadding.top) * units;
      float b = a - (this.size.y - (float) this.listPadding.vertical) * units;
      for (int index1 = startIndex; index1 < vertices.Count; index1 += 4)
      {
        Vector3 vector3_1 = vertices[index1];
        Vector3 vector3_2 = vertices[index1 + 1];
        Vector3 vector3_3 = vertices[index1 + 2];
        Vector3 vector3_4 = vertices[index1 + 3];
        float num = vector3_1.y - vector3_4.y;
        if ((double) vector3_4.y < (double) b)
        {
          float t = (float) (1.0 - (double) Mathf.Abs(-b + vector3_1.y) / (double) num);
          dfList<Vector3> dfList1 = vertices;
          int index2 = index1;
          vector3_1 = new Vector3(vector3_1.x, Mathf.Max(vector3_1.y, b), vector3_2.z);
          Vector3 vector3_5 = vector3_1;
          dfList1[index2] = vector3_5;
          dfList<Vector3> dfList2 = vertices;
          int index3 = index1 + 1;
          vector3_2 = new Vector3(vector3_2.x, Mathf.Max(vector3_2.y, b), vector3_2.z);
          Vector3 vector3_6 = vector3_2;
          dfList2[index3] = vector3_6;
          dfList<Vector3> dfList3 = vertices;
          int index4 = index1 + 2;
          vector3_3 = new Vector3(vector3_3.x, Mathf.Max(vector3_3.y, b), vector3_3.z);
          Vector3 vector3_7 = vector3_3;
          dfList3[index4] = vector3_7;
          dfList<Vector3> dfList4 = vertices;
          int index5 = index1 + 3;
          vector3_4 = new Vector3(vector3_4.x, Mathf.Max(vector3_4.y, b), vector3_4.z);
          Vector3 vector3_8 = vector3_4;
          dfList4[index5] = vector3_8;
          float y = Mathf.Lerp(uv[index1 + 3].y, uv[index1].y, t);
          uv[index1 + 3] = new Vector2(uv[index1 + 3].x, y);
          uv[index1 + 2] = new Vector2(uv[index1 + 2].x, y);
          num = Mathf.Abs(vector3_4.y - vector3_1.y);
        }
        if ((double) vector3_1.y > (double) a)
        {
          float t = Mathf.Abs(a - vector3_1.y) / num;
          vertices[index1] = new Vector3(vector3_1.x, Mathf.Min(a, vector3_1.y), vector3_1.z);
          vertices[index1 + 1] = new Vector3(vector3_2.x, Mathf.Min(a, vector3_2.y), vector3_2.z);
          vertices[index1 + 2] = new Vector3(vector3_3.x, Mathf.Min(a, vector3_3.y), vector3_3.z);
          vertices[index1 + 3] = new Vector3(vector3_4.x, Mathf.Min(a, vector3_4.y), vector3_4.z);
          float y = Mathf.Lerp(uv[index1].y, uv[index1 + 3].y, t);
          uv[index1] = new Vector2(uv[index1].x, y);
          uv[index1 + 1] = new Vector2(uv[index1 + 1].x, y);
        }
      }
    }

    private void updateItemHover(dfMouseEventArgs args)
    {
      if (!Application.isPlaying)
        return;
      Ray ray = args.Ray;
      if (!this.GetComponent<Collider>().Raycast(ray, out RaycastHit _, 1000f))
      {
        this.hoverIndex = -1;
        this.hoverTweenLocation = 0.0f;
      }
      else
      {
        Vector2 position;
        this.GetHitPosition(ray, out position);
        float num1 = this.Pivot.TransformToUpperLeft(this.Size).y + ((float) -this.itemHeight * ((float) this.selectedIndex - this.scrollPosition) - (float) this.listPadding.top);
        float num2 = (float) ((double) this.selectedIndex - (double) this.scrollPosition + 1.0) * (float) this.itemHeight + (float) this.listPadding.vertical - this.size.y;
        if ((double) num2 > 0.0)
        {
          float num3 = num1 + num2;
        }
        int num4 = (int) ((double) this.scrollPosition + (double) (position.y - (float) this.listPadding.top)) / this.itemHeight;
        if (num4 == this.hoverIndex)
          return;
        this.hoverIndex = num4;
        this.Invalidate();
      }
    }

    private float constrainScrollPosition(float value)
    {
      value = Mathf.Max(0.0f, value);
      int num1 = this.items.Length * this.itemHeight;
      float num2 = this.size.y - (float) this.listPadding.vertical;
      return (double) num1 < (double) num2 ? 0.0f : Mathf.Min(value, (float) num1 - num2);
    }

    private void attachScrollbarEvents()
    {
      if ((UnityEngine.Object) this.scrollbar == (UnityEngine.Object) null || this.eventsAttached)
        return;
      this.eventsAttached = true;
      this.scrollbar.ValueChanged += new PropertyChangedEventHandler<float>(this.scrollbar_ValueChanged);
      this.scrollbar.GotFocus += new FocusEventHandler(this.scrollbar_GotFocus);
    }

    private void detachScrollbarEvents()
    {
      if ((UnityEngine.Object) this.scrollbar == (UnityEngine.Object) null || !this.eventsAttached)
        return;
      this.eventsAttached = false;
      this.scrollbar.ValueChanged -= new PropertyChangedEventHandler<float>(this.scrollbar_ValueChanged);
      this.scrollbar.GotFocus -= new FocusEventHandler(this.scrollbar_GotFocus);
    }

    private void scrollbar_GotFocus(dfControl control, dfFocusEventArgs args) => this.Focus(true);

    private void scrollbar_ValueChanged(dfControl control, float value)
    {
      this.ScrollPosition = value;
    }

    private void synchronizeScrollbar()
    {
      if ((UnityEngine.Object) this.scrollbar == (UnityEngine.Object) null)
        return;
      int num1 = this.items.Length * this.itemHeight;
      float num2 = this.size.y - (float) this.listPadding.vertical;
      this.scrollbar.IncrementAmount = (float) this.itemHeight;
      this.scrollbar.MinValue = 0.0f;
      this.scrollbar.MaxValue = (float) num1;
      this.scrollbar.ScrollSize = num2;
      this.scrollbar.Value = this.scrollPosition;
    }

    public dfList<dfRenderData> RenderMultiple()
    {
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || (UnityEngine.Object) this.Font == (UnityEngine.Object) null)
        return (dfList<dfRenderData>) null;
      if (!this.isVisible)
        return (dfList<dfRenderData>) null;
      if (this.renderData == null)
      {
        this.renderData = dfRenderData.Obtain();
        this.textRenderData = dfRenderData.Obtain();
        this.isControlInvalidated = true;
      }
      Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
      if (!this.isControlInvalidated)
      {
        for (int index = 0; index < this.buffers.Count; ++index)
          this.buffers[index].Transform = localToWorldMatrix;
        return this.buffers;
      }
      this.buffers.Clear();
      this.renderData.Clear();
      this.renderData.Material = this.Atlas.Material;
      this.renderData.Transform = localToWorldMatrix;
      this.buffers.Add(this.renderData);
      this.textRenderData.Clear();
      this.textRenderData.Material = this.Atlas.Material;
      this.textRenderData.Transform = localToWorldMatrix;
      this.buffers.Add(this.textRenderData);
      this.renderBackground();
      int count = this.renderData.Vertices.Count;
      this.renderHover();
      this.renderSelection();
      this.renderItems(this.textRenderData);
      this.clipQuads(this.renderData, count);
      this.clipQuads(this.textRenderData, 0);
      this.isControlInvalidated = false;
      this.updateCollider();
      return this.buffers;
    }

    private void bindTextureRebuildCallback()
    {
      if (this.isFontCallbackAssigned || (UnityEngine.Object) this.Font == (UnityEngine.Object) null || !(this.Font is dfDynamicFont))
        return;
      UnityEngine.Font.textureRebuilt += new Action<UnityEngine.Font>(this.onFontTextureRebuilt);
      this.isFontCallbackAssigned = true;
    }

    private void unbindTextureRebuildCallback()
    {
      if (!this.isFontCallbackAssigned || (UnityEngine.Object) this.Font == (UnityEngine.Object) null)
        return;
      if (this.Font is dfDynamicFont)
        UnityEngine.Font.textureRebuilt -= new Action<UnityEngine.Font>(this.onFontTextureRebuilt);
      this.isFontCallbackAssigned = false;
    }

    private void requestCharacterInfo()
    {
      dfDynamicFont font = this.Font as dfDynamicFont;
      if ((UnityEngine.Object) font == (UnityEngine.Object) null || !dfFontManager.IsDirty(this.Font) || this.items == null || this.items.Length == 0)
        return;
      int fontSize = Mathf.CeilToInt((float) this.font.FontSize * this.getTextScaleMultiplier());
      for (int index = 0; index < this.items.Length; ++index)
        font.AddCharacterRequest(this.items[index], fontSize, FontStyle.Normal);
    }

    private void onFontTextureRebuilt(UnityEngine.Font font)
    {
      if (!(this.Font is dfDynamicFont) || !((UnityEngine.Object) font == (UnityEngine.Object) (this.Font as dfDynamicFont).BaseFont))
        return;
      this.requestCharacterInfo();
      this.Invalidate();
    }

    public void UpdateFontInfo() => this.requestCharacterInfo();
  }

