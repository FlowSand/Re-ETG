// Decompiled with JetBrains decompiler
// Type: dfLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/User Interface/Label")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_label.html")]
[dfTooltip("Displays text information, optionally allowing the use of markup to specify colors and embedded sprites")]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[Serializable]
public class dfLabel : dfControl, IDFMultiRender, IRendersText
{
  public Vector3 PerCharacterOffset;
  [NonSerialized]
  protected dfFontBase m_defaultAssignedFont;
  protected float m_defaultAssignedFontTextScale;
  [SerializeField]
  public bool PreventFontChanges;
  [SerializeField]
  protected dfAtlas atlas;
  [SerializeField]
  protected dfFontBase font;
  [SerializeField]
  protected string backgroundSprite;
  [SerializeField]
  protected Color32 backgroundColor = (Color32) UnityEngine.Color.white;
  [SerializeField]
  protected bool autoSize;
  [SerializeField]
  protected bool autoHeight;
  [SerializeField]
  protected bool wordWrap;
  [SerializeField]
  protected string text = "Label";
  [SerializeField]
  protected Color32 bottomColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
  [SerializeField]
  protected TextAlignment align;
  [SerializeField]
  protected dfVerticalAlignment vertAlign;
  [SerializeField]
  protected float textScale = 1f;
  [SerializeField]
  protected dfTextScaleMode textScaleMode;
  [SerializeField]
  protected int charSpacing;
  [SerializeField]
  protected bool colorizeSymbols;
  [SerializeField]
  protected bool processMarkup;
  [SerializeField]
  protected bool outline;
  [SerializeField]
  protected int outlineWidth = 1;
  [SerializeField]
  protected bool enableGradient;
  [SerializeField]
  protected Color32 outlineColor = (Color32) UnityEngine.Color.black;
  [SerializeField]
  protected bool shadow;
  [SerializeField]
  protected Color32 shadowColor = (Color32) UnityEngine.Color.black;
  [SerializeField]
  protected Vector2 shadowOffset = new Vector2(1f, -1f);
  [SerializeField]
  protected RectOffset padding = new RectOffset();
  [SerializeField]
  protected int tabSize = 48 /*0x30*/;
  [SerializeField]
  protected List<int> tabStops = new List<int>();
  private Vector2 startSize = Vector2.zero;
  private bool isFontCallbackAssigned;
  private StringTableManager.GungeonSupportedLanguages m_cachedLanguage;
  private RectOffset m_cachedPadding;
  private static float m_cachedScaleTileScale = 3f;
  public bool MaintainJapaneseFont;
  public bool MaintainKoreanFont;
  public bool MaintainRussianFont;
  private dfRenderData textRenderData;
  private dfList<dfRenderData> renderDataBuffers = dfList<dfRenderData>.Obtain();
  [NonSerialized]
  public bool Glitchy;

  public event PropertyChangedEventHandler<string> TextChanged;

  public dfFontBase DefaultAssignedFont => this.m_defaultAssignedFont;

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

  public string BackgroundSprite
  {
    get => this.backgroundSprite;
    set
    {
      if (!(value != this.backgroundSprite))
        return;
      this.backgroundSprite = value;
      this.Invalidate();
    }
  }

  public Color32 BackgroundColor
  {
    get => this.backgroundColor;
    set
    {
      if (object.Equals((object) value, (object) this.backgroundColor))
        return;
      this.backgroundColor = value;
      this.Invalidate();
    }
  }

  public float TextScale
  {
    get => this.textScale;
    set
    {
      value = Mathf.Max(0.1f, value);
      if (Mathf.Approximately(this.textScale, value))
        return;
      dfFontManager.Invalidate(this.Font);
      this.textScale = value;
      this.Invalidate();
    }
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

  public int CharacterSpacing
  {
    get => this.charSpacing;
    set
    {
      value = Mathf.Max(0, value);
      if (value == this.charSpacing)
        return;
      this.charSpacing = value;
      this.Invalidate();
    }
  }

  public bool ColorizeSymbols
  {
    get => this.colorizeSymbols;
    set
    {
      if (value == this.colorizeSymbols)
        return;
      this.colorizeSymbols = value;
      this.Invalidate();
    }
  }

  public bool ProcessMarkup
  {
    get => this.processMarkup;
    set
    {
      if (value == this.processMarkup)
        return;
      this.processMarkup = value;
      this.Invalidate();
    }
  }

  public bool ShowGradient
  {
    get => this.enableGradient;
    set
    {
      if (value == this.enableGradient)
        return;
      this.enableGradient = value;
      this.Invalidate();
    }
  }

  public Color32 BottomColor
  {
    get => this.bottomColor;
    set
    {
      if (this.bottomColor.EqualsNonAlloc(value))
        return;
      this.bottomColor = value;
      this.OnColorChanged();
    }
  }

  public void ModifyLocalizedText(string text)
  {
    dfFontManager.Invalidate(this.Font);
    this.text = text;
    this.OnTextChanged();
  }

  public string Text
  {
    get => this.text;
    set
    {
      value = value != null ? value.Replace("\\t", "\t").Replace("\\n", "\n") : string.Empty;
      if (string.Equals(value, this.text))
        return;
      dfFontManager.Invalidate(this.Font);
      this.localizationKey = value;
      this.text = this.getLocalizedValue(value);
      this.OnTextChanged();
    }
  }

  public bool AutoSize
  {
    get
    {
      if (this.autoSize && this.autoHeight)
        this.autoHeight = false;
      return this.autoSize;
    }
    set
    {
      if (value == this.autoSize)
        return;
      if (value)
        this.autoHeight = false;
      this.autoSize = value;
      this.Invalidate();
    }
  }

  public bool AutoHeight
  {
    get => this.autoHeight && !this.autoSize;
    set
    {
      if (value == this.autoHeight)
        return;
      if (value)
        this.autoSize = false;
      this.autoHeight = value;
      this.Invalidate();
    }
  }

  public bool WordWrap
  {
    get => this.wordWrap;
    set
    {
      if (value == this.wordWrap)
        return;
      this.wordWrap = value;
      this.Invalidate();
    }
  }

  public TextAlignment TextAlignment
  {
    get => this.align;
    set
    {
      if (value == this.align)
        return;
      this.align = value;
      this.Invalidate();
    }
  }

  public dfVerticalAlignment VerticalAlignment
  {
    get => this.vertAlign;
    set
    {
      if (value == this.vertAlign)
        return;
      this.vertAlign = value;
      this.Invalidate();
    }
  }

  public bool Outline
  {
    get => this.outline;
    set
    {
      if (value == this.outline)
        return;
      this.outline = value;
      this.Invalidate();
    }
  }

  public int OutlineSize
  {
    get => this.outlineWidth;
    set
    {
      value = Mathf.Max(0, value);
      if (value == this.outlineWidth)
        return;
      this.outlineWidth = value;
      this.Invalidate();
    }
  }

  public Color32 OutlineColor
  {
    get => this.outlineColor;
    set
    {
      if (value.Equals((object) this.outlineColor))
        return;
      this.outlineColor = value;
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

  public RectOffset Padding
  {
    get
    {
      if (this.padding == null)
        this.padding = new RectOffset();
      return this.padding;
    }
    set
    {
      if (object.Equals((object) value, (object) this.padding))
        return;
      this.padding = value;
      this.Invalidate();
    }
  }

  public int TabSize
  {
    get => this.tabSize;
    set
    {
      value = Mathf.Max(0, value);
      if (value == this.tabSize)
        return;
      this.tabSize = value;
      this.Invalidate();
    }
  }

  public List<int> TabStops => this.tabStops;

  protected void CheckFontsForLanguage()
  {
    if (!Application.isPlaying)
      return;
    StringTableManager.GungeonSupportedLanguages supportedLanguages = GameManager.Options.CurrentLanguage;
    if (this.PreventFontChanges)
      supportedLanguages = StringTableManager.GungeonSupportedLanguages.ENGLISH;
    if (this.m_cachedLanguage == supportedLanguages)
      return;
    if ((UnityEngine.Object) this.m_defaultAssignedFont == (UnityEngine.Object) null)
    {
      this.m_defaultAssignedFontTextScale = this.textScale;
      this.m_defaultAssignedFont = this.font;
    }
    if ((UnityEngine.Object) this.m_defaultAssignedFont == (UnityEngine.Object) null)
    {
      this.font = this.GUIManager.DefaultFont;
      this.m_defaultAssignedFont = this.font;
    }
    if (this.m_cachedPadding == null)
      this.m_cachedPadding = this.padding;
    float num1 = this.m_defaultAssignedFontTextScale;
    if ((bool) (UnityEngine.Object) Pixelator.Instance)
      dfLabel.m_cachedScaleTileScale = Pixelator.Instance.ScaleTileScale;
    dfFontBase dfFontBase;
    if (supportedLanguages == StringTableManager.GungeonSupportedLanguages.JAPANESE && !this.MaintainJapaneseFont)
    {
      dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/JackeyFont12_DF") as GameObject).GetComponent<dfFont>();
      num1 = (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * Mathf.Max(1f, (float) this.m_defaultAssignedFont.FontSize / 14f));
      this.padding = this.m_defaultAssignedFont.LineHeight >= 16 /*0x10*/ ? this.m_cachedPadding : new RectOffset(this.m_cachedPadding.left, this.m_cachedPadding.right, this.m_cachedPadding.top + (this.GetManager().FixedHeight <= 1000 ? 3 : 4) * 2, this.m_cachedPadding.bottom);
    }
    else if (supportedLanguages == StringTableManager.GungeonSupportedLanguages.CHINESE && !this.MaintainJapaneseFont)
    {
      dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/SimSun12_DF") as GameObject).GetComponent<dfFont>();
      num1 = (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * Mathf.Max(1f, (float) this.m_defaultAssignedFont.FontSize / 14f));
      this.padding = this.m_defaultAssignedFont.LineHeight >= 16 /*0x10*/ ? this.m_cachedPadding : new RectOffset(this.m_cachedPadding.left, this.m_cachedPadding.right, this.m_cachedPadding.top + (this.GetManager().FixedHeight <= 1000 ? 3 : 4) * 2, this.m_cachedPadding.bottom);
    }
    else if (supportedLanguages == StringTableManager.GungeonSupportedLanguages.KOREAN && !this.MaintainKoreanFont)
    {
      dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/NanumGothic16_DF") as GameObject).GetComponent<dfFont>();
      float num2 = (float) this.m_defaultAssignedFont.FontSize / (float) dfFontBase.FontSize;
      float num3 = Mathf.Max(3f, dfLabel.m_cachedScaleTileScale);
      if ((double) num2 < 1.0)
        num2 = (num3 - 1f) / num3;
      num1 = (double) num2 <= 1.0 ? this.m_defaultAssignedFontTextScale * num2 : (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * num2);
      this.padding = this.m_defaultAssignedFont.LineHeight >= 16 /*0x10*/ ? this.m_cachedPadding : new RectOffset(this.m_cachedPadding.left, this.m_cachedPadding.right, this.m_cachedPadding.top + (this.GetManager().FixedHeight <= 1000 ? 3 : 4) * 2, this.m_cachedPadding.bottom);
    }
    else if (supportedLanguages == StringTableManager.GungeonSupportedLanguages.RUSSIAN && !this.MaintainRussianFont)
    {
      dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/PixelaCYR_15_DF") as GameObject).GetComponent<dfFont>();
      float num4 = (float) this.m_defaultAssignedFont.FontSize / (float) dfFontBase.FontSize;
      if ((double) num4 < 1.0)
        num4 = 1f;
      num1 = (double) num4 <= 1.0 ? this.m_defaultAssignedFontTextScale * num4 : (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * num4);
      this.padding = this.m_cachedPadding;
    }
    else if (supportedLanguages == StringTableManager.GungeonSupportedLanguages.POLISH && !this.MaintainRussianFont && (UnityEngine.Object) this.m_defaultAssignedFont != (UnityEngine.Object) null && (UnityEngine.Object) this.GUIManager.DefaultFont != (UnityEngine.Object) null && this.m_defaultAssignedFont.name.StartsWith("04b03"))
    {
      dfFontBase = this.GUIManager.DefaultFont;
      float num5 = (float) this.m_defaultAssignedFont.FontSize / (float) dfFontBase.FontSize;
      if ((double) num5 < 1.0)
        num5 = 1f;
      num1 = (double) num5 <= 1.0 ? this.m_defaultAssignedFontTextScale * num5 : (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * num5);
      this.padding = this.m_cachedPadding;
    }
    else
    {
      dfFontBase = this.m_defaultAssignedFont;
      this.padding = this.m_cachedPadding;
    }
    if ((UnityEngine.Object) dfFontBase != (UnityEngine.Object) null && (UnityEngine.Object) this.Font != (UnityEngine.Object) dfFontBase)
    {
      this.Font = dfFontBase;
      if (dfFontBase is dfFont)
        this.Atlas = (dfFontBase as dfFont).Atlas;
      this.TextScale = num1;
    }
    this.m_cachedLanguage = supportedLanguages;
  }

  protected internal override void OnLocalize()
  {
    base.OnLocalize();
    this.CheckFontsForLanguage();
    if (this.text.StartsWith("#"))
      this.localizationKey = this.text;
    if (!string.IsNullOrEmpty(this.localizationKey) && this.localizationKey.StartsWith("#"))
      this.text = this.getLocalizedValue(this.localizationKey);
    else
      this.Text = this.getLocalizedValue(this.text);
    if (this.text.StartsWith("#") && this.text.Contains("ENCNAME"))
      this.ModifyLocalizedText(StringTableManager.GetItemsString(this.localizationKey));
    this.PerformLayout();
  }

  protected internal void OnTextChanged()
  {
    this.CheckFontsForLanguage();
    this.Invalidate();
    this.Signal(nameof (OnTextChanged), (object) this, (object) this.text);
    if (this.TextChanged == null)
      return;
    this.TextChanged((dfControl) this, this.text);
  }

  public override void Start() => base.Start();

  public override void OnEnable()
  {
    base.OnEnable();
    bool flag = (UnityEngine.Object) this.Font != (UnityEngine.Object) null && this.Font.IsValid;
    if (Application.isPlaying && !flag)
      this.Font = this.GetManager().DefaultFont;
    this.bindTextureRebuildCallback();
    if ((double) this.size.sqrMagnitude > 1.4012984643248171E-45)
      return;
    this.Size = new Vector2(150f, 25f);
  }

  public override void OnDisable()
  {
    base.OnDisable();
    this.unbindTextureRebuildCallback();
  }

  public override void Awake()
  {
    this.localizationKey = this.Text;
    base.Awake();
    this.startSize = !Application.isPlaying ? Vector2.zero : this.Size;
  }

  public override Vector2 CalculateMinimumSize()
  {
    if (!((UnityEngine.Object) this.Font != (UnityEngine.Object) null))
      return base.CalculateMinimumSize();
    float num = (float) ((double) this.Font.FontSize * (double) this.TextScale * 0.75);
    return Vector2.Max(base.CalculateMinimumSize(), new Vector2(num, num));
  }

  public float GetAutosizeWidth()
  {
    using (dfFontRendererBase renderer = this.obtainRenderer())
      return renderer.MeasureString(this.text).RoundToInt().x + (float) this.padding.horizontal;
  }

  [HideInInspector]
  public override void Invalidate()
  {
    base.Invalidate();
    if (this.m_cachedLanguage != GameManager.Options.CurrentLanguage)
      this.CheckFontsForLanguage();
    if ((UnityEngine.Object) this.Font == (UnityEngine.Object) null || !this.Font.IsValid || (UnityEngine.Object) this.GetManager() == (UnityEngine.Object) null)
      return;
    bool flag = (double) this.size.sqrMagnitude <= 1.4012984643248171E-45;
    if (!this.AutoSize && !this.autoHeight && !flag)
      return;
    if (string.IsNullOrEmpty(this.Text))
    {
      Vector2 size = this.size;
      Vector2 vector2 = size;
      if (flag)
        vector2 = new Vector2(150f, 24f);
      if (this.AutoSize || this.AutoHeight)
        vector2.y = (float) Mathf.CeilToInt((float) this.Font.LineHeight * this.TextScale);
      if (!(size != vector2))
        return;
      this.SuspendLayout();
      this.Size = vector2;
      this.ResumeLayout();
    }
    else
    {
      Vector2 size = this.size;
      using (dfFontRendererBase renderer = this.obtainRenderer())
      {
        Vector2 vector2 = renderer.MeasureString(this.text).RoundToInt();
        if (this.AutoSize || flag)
          this.size = vector2 + new Vector2((float) this.padding.horizontal, (float) this.padding.vertical);
        else if (this.AutoHeight)
          this.size = new Vector2(this.size.x, vector2.y + (float) this.padding.vertical);
      }
      if ((double) (this.size - size).sqrMagnitude < 1.0)
        return;
      this.raiseSizeChangedEvent();
    }
  }

  private dfFontRendererBase obtainRenderer()
  {
    bool flag = (double) this.Size.sqrMagnitude <= 1.4012984643248171E-45;
    Vector2 vector2_1 = this.Size - new Vector2((float) this.padding.horizontal, (float) this.padding.vertical);
    Vector2 vector2_2 = this.AutoSize || flag ? this.getAutoSizeDefault() : vector2_1;
    if (this.autoHeight)
      vector2_2 = new Vector2(vector2_1.x, (float) int.MaxValue);
    float units = this.PixelsToUnits();
    Vector3 vector = (this.pivot.TransformToUpperLeft(this.Size) + new Vector3((float) this.padding.left, (float) -this.padding.top)) * units;
    float num = this.TextScale * this.getTextScaleMultiplier();
    dfFontRendererBase renderer = this.Font.ObtainRenderer();
    renderer.WordWrap = this.WordWrap;
    renderer.MaxSize = vector2_2;
    renderer.PixelRatio = units;
    renderer.TextScale = num;
    renderer.CharacterSpacing = this.CharacterSpacing;
    renderer.VectorOffset = vector.Quantize(units);
    renderer.PerCharacterAccumulatedOffset = this.PerCharacterOffset * units;
    renderer.MultiLine = true;
    renderer.TabSize = this.TabSize;
    renderer.TabStops = this.TabStops;
    renderer.TextAlign = !this.AutoSize ? this.TextAlignment : TextAlignment.Left;
    renderer.ColorizeSymbols = this.ColorizeSymbols;
    renderer.ProcessMarkup = this.ProcessMarkup;
    renderer.DefaultColor = !this.IsEnabled ? this.DisabledColor : this.Color;
    renderer.BottomColor = !this.enableGradient ? new Color32?() : new Color32?(this.BottomColor);
    renderer.OverrideMarkupColors = !this.IsEnabled;
    renderer.Opacity = this.CalculateOpacity();
    renderer.Outline = this.Outline;
    renderer.OutlineSize = this.OutlineSize;
    renderer.OutlineColor = this.OutlineColor;
    renderer.Shadow = this.Shadow;
    renderer.ShadowColor = this.ShadowColor;
    renderer.ShadowOffset = this.ShadowOffset;
    if (renderer is dfDynamicFont.DynamicFontRenderer dynamicFontRenderer)
    {
      dynamicFontRenderer.SpriteAtlas = this.Atlas;
      dynamicFontRenderer.SpriteBuffer = this.renderData;
    }
    if (this.vertAlign != dfVerticalAlignment.Top)
      renderer.VectorOffset = this.getVertAlignOffset(renderer);
    return renderer;
  }

  private float getTextScaleMultiplier()
  {
    if (this.textScaleMode == dfTextScaleMode.None || !Application.isPlaying)
      return 1f;
    if (this.textScaleMode == dfTextScaleMode.ScreenResolution)
      return this.GetManager().GetScreenSize().y / (float) this.GetManager().FixedHeight;
    return this.AutoSize ? 1f : this.Size.y / this.startSize.y;
  }

  private Vector2 getAutoSizeDefault()
  {
    return new Vector2((double) this.maxSize.x <= 1.4012984643248171E-45 ? (float) int.MaxValue : this.maxSize.x, (double) this.maxSize.y <= 1.4012984643248171E-45 ? (float) int.MaxValue : this.maxSize.y);
  }

  private Vector3 getVertAlignOffset(dfFontRendererBase textRenderer)
  {
    float units = this.PixelsToUnits();
    Vector2 vector2 = textRenderer.MeasureString(this.text) * units;
    Vector3 vectorOffset = textRenderer.VectorOffset;
    float num = (this.Height - (float) this.padding.vertical) * units;
    if ((double) vector2.y >= (double) num)
      return vectorOffset;
    switch (this.vertAlign)
    {
      case dfVerticalAlignment.Middle:
        vectorOffset.y -= (float) (((double) num - (double) vector2.y) * 0.5);
        break;
      case dfVerticalAlignment.Bottom:
        vectorOffset.y -= num - vector2.y;
        break;
    }
    return vectorOffset;
  }

  protected internal virtual void renderBackground()
  {
    if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
      return;
    dfAtlas.ItemInfo atla = this.Atlas[this.backgroundSprite];
    if (atla == (dfAtlas.ItemInfo) null)
      return;
    Color32 color32 = this.ApplyOpacity(this.BackgroundColor);
    dfSprite.RenderOptions options = new dfSprite.RenderOptions()
    {
      atlas = this.atlas,
      color = color32,
      fillAmount = 1f,
      flip = dfSpriteFlip.None,
      offset = this.pivot.TransformToUpperLeft(this.Size),
      pixelsToUnits = this.PixelsToUnits(),
      size = this.Size,
      spriteInfo = atla
    };
    if (atla.border.horizontal == 0 && atla.border.vertical == 0)
      dfSprite.renderSprite(this.renderData, options);
    else
      dfSlicedSprite.renderSprite(this.renderData, options);
  }

  public dfList<dfRenderData> RenderMultiple()
  {
    try
    {
      if (!this.isControlInvalidated && (this.textRenderData != null || this.renderData != null))
      {
        Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
        for (int index = 0; index < this.renderDataBuffers.Count; ++index)
          this.renderDataBuffers[index].Transform = localToWorldMatrix;
        return this.renderDataBuffers;
      }
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || (UnityEngine.Object) this.Font == (UnityEngine.Object) null || !this.isVisible)
        return (dfList<dfRenderData>) null;
      if (this.renderData == null)
      {
        this.renderData = dfRenderData.Obtain();
        this.textRenderData = dfRenderData.Obtain();
        this.isControlInvalidated = true;
      }
      this.resetRenderBuffers();
      this.renderBackground();
      if (string.IsNullOrEmpty(this.Text))
      {
        if (this.AutoSize || this.AutoHeight)
          this.Height = (float) Mathf.CeilToInt((float) this.Font.LineHeight * this.TextScale);
        return this.renderDataBuffers;
      }
      bool flag = (double) this.size.sqrMagnitude <= 1.4012984643248171E-45;
      using (dfFontRendererBase renderer = this.obtainRenderer())
      {
        this.textRenderData.Glitchy = this.Glitchy;
        renderer.Render(this.text, this.textRenderData);
        if (this.AutoSize || flag)
          this.Size = (renderer.RenderedSize + new Vector2((float) this.padding.horizontal, (float) this.padding.vertical)).CeilToInt();
        else if (this.AutoHeight)
          this.Size = new Vector2(this.size.x, renderer.RenderedSize.y + (float) this.padding.vertical).CeilToInt();
      }
      this.updateCollider();
      return this.renderDataBuffers;
    }
    finally
    {
      this.isControlInvalidated = false;
    }
  }

  private void resetRenderBuffers()
  {
    this.renderDataBuffers.Clear();
    Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
    if (this.renderData != null)
    {
      this.renderData.Clear();
      this.renderData.Material = this.Atlas.Material;
      this.renderData.Transform = localToWorldMatrix;
      this.renderDataBuffers.Add(this.renderData);
    }
    if (this.textRenderData == null)
      return;
    this.textRenderData.Clear();
    this.textRenderData.Material = this.Atlas.Material;
    this.textRenderData.Transform = localToWorldMatrix;
    this.renderDataBuffers.Add(this.textRenderData);
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
    if ((UnityEngine.Object) font == (UnityEngine.Object) null || !dfFontManager.IsDirty(this.Font) || string.IsNullOrEmpty(this.text))
      return;
    int fontSize = Mathf.CeilToInt((float) this.font.FontSize * (this.TextScale * this.getTextScaleMultiplier()));
    font.AddCharacterRequest(this.text, fontSize, FontStyle.Normal);
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
