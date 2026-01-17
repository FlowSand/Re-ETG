// Decompiled with JetBrains decompiler
// Type: dfButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Daikon Forge/User Interface/Button")]
    [ExecuteInEditMode]
    [dfCategory("Basic Controls")]
    [dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_button.html")]
    [dfTooltip("Provides a basic Button implementation that allows the developer to specify individual sprite images to be used to represent common button states.")]
    [Serializable]
    public class dfButton : dfInteractiveBase, IDFMultiRender, IRendersText
    {
      [SerializeField]
      protected dfFontBase font;
      [SerializeField]
      protected string pressedSprite;
      [SerializeField]
      protected dfButton.ButtonState state;
      [SerializeField]
      protected dfControl group;
      [SerializeField]
      protected string text = string.Empty;
      [SerializeField]
      public int textPixelOffset;
      [SerializeField]
      public int hoverTextPixelOffset;
      [SerializeField]
      public int downTextPixelOffset;
      [SerializeField]
      protected TextAlignment textAlign = TextAlignment.Center;
      [SerializeField]
      protected dfVerticalAlignment vertAlign = dfVerticalAlignment.Middle;
      [SerializeField]
      protected Color32 normalColor = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 textColor = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 hoverText = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 pressedText = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 focusText = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 disabledText = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 hoverColor = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 pressedColor = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected Color32 focusColor = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected float textScale = 1f;
      [SerializeField]
      protected dfTextScaleMode textScaleMode;
      [SerializeField]
      protected bool wordWrap;
      [SerializeField]
      protected RectOffset padding = new RectOffset();
      [SerializeField]
      protected bool textShadow;
      [SerializeField]
      protected Color32 shadowColor = (Color32) UnityEngine.Color.black;
      [SerializeField]
      protected Vector2 shadowOffset = new Vector2(1f, -1f);
      [SerializeField]
      protected bool autoSize;
      [SerializeField]
      protected bool clickWhenSpacePressed = true;
      [SerializeField]
      public bool forceUpperCase;
      [SerializeField]
      public bool manualStateControl;
      protected dfFontBase m_defaultAssignedFont;
      protected float m_defaultAssignedFontTextScale;
      private StringTableManager.GungeonSupportedLanguages m_cachedLanguage;
      private static float m_cachedScaleTileScale = 3f;
      private Vector2 startSize = Vector2.zero;
      private bool isFontCallbackAssigned;
      private dfRenderData textRenderData;
      private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();

      public event PropertyChangedEventHandler<dfButton.ButtonState> ButtonStateChanged;

      public bool ClickWhenSpacePressed
      {
        get => this.clickWhenSpacePressed;
        set => this.clickWhenSpacePressed = value;
      }

      public dfButton.ButtonState State
      {
        get => this.state;
        set
        {
          if (this.manualStateControl || value == this.state)
            return;
          this.OnButtonStateChanged(value);
          this.Invalidate();
        }
      }

      public void ForceState(dfButton.ButtonState newState)
      {
        if (newState == this.state)
          return;
        this.OnButtonStateChanged(newState);
        this.Invalidate();
      }

      public string PressedSprite
      {
        get => this.pressedSprite;
        set
        {
          if (!(value != this.pressedSprite))
            return;
          this.pressedSprite = value;
          this.Invalidate();
        }
      }

      public dfControl ButtonGroup
      {
        get => this.group;
        set
        {
          if (!((UnityEngine.Object) value != (UnityEngine.Object) this.group))
            return;
          this.group = value;
          this.Invalidate();
        }
      }

      public bool AutoSize
      {
        get => this.autoSize;
        set
        {
          if (value == this.autoSize)
            return;
          this.autoSize = value;
          if (value)
            this.textAlign = TextAlignment.Left;
          this.Invalidate();
        }
      }

      public TextAlignment TextAlignment
      {
        get => this.autoSize ? TextAlignment.Left : this.textAlign;
        set
        {
          if (value == this.textAlign)
            return;
          this.textAlign = value;
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
          value = value.ConstrainPadding();
          if (object.Equals((object) value, (object) this.padding))
            return;
          this.padding = value;
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
          if ((UnityEngine.Object) value != (UnityEngine.Object) this.font)
          {
            this.unbindTextureRebuildCallback();
            this.font = value;
            this.bindTextureRebuildCallback();
          }
          this.Invalidate();
        }
      }

      public string Text
      {
        get => this.text;
        set
        {
          if (!(value != this.text))
            return;
          this.CheckFontsForLanguage();
          dfFontManager.Invalidate(this.Font);
          this.localizationKey = value;
          this.text = this.getLocalizedValue(value);
          this.Invalidate();
        }
      }

      public void ModifyLocalizedText(string text)
      {
        this.CheckFontsForLanguage();
        dfFontManager.Invalidate(this.Font);
        this.text = text;
        this.Invalidate();
      }

      protected void CheckFontsForLanguage()
      {
        if (!Application.isPlaying)
          return;
        StringTableManager.GungeonSupportedLanguages currentLanguage = GameManager.Options.CurrentLanguage;
        if (this.m_cachedLanguage == currentLanguage)
          return;
        if ((UnityEngine.Object) this.m_defaultAssignedFont == (UnityEngine.Object) null)
        {
          this.m_defaultAssignedFontTextScale = this.textScale;
          this.m_defaultAssignedFont = this.font;
        }
        if ((UnityEngine.Object) this.m_defaultAssignedFont == (UnityEngine.Object) null)
          return;
        float num1 = this.m_defaultAssignedFontTextScale;
        if ((bool) (UnityEngine.Object) Pixelator.Instance)
          dfButton.m_cachedScaleTileScale = Pixelator.Instance.ScaleTileScale;
        dfFontBase dfFontBase;
        switch (currentLanguage)
        {
          case StringTableManager.GungeonSupportedLanguages.JAPANESE:
            dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/JackeyFont12_DF") as GameObject).GetComponent<dfFont>();
            num1 = (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * Mathf.Max(1f, (float) this.m_defaultAssignedFont.FontSize / 14f));
            break;
          case StringTableManager.GungeonSupportedLanguages.KOREAN:
            dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/NanumGothic16_DF") as GameObject).GetComponent<dfFont>();
            float num2 = (float) this.m_defaultAssignedFont.FontSize / (float) dfFontBase.FontSize;
            float num3 = Mathf.Max(3f, dfButton.m_cachedScaleTileScale);
            if ((double) num2 < 1.0)
              num2 = (num3 - 1f) / num3;
            num1 = (double) num2 <= 1.0 ? this.m_defaultAssignedFontTextScale * num2 : (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * num2);
            break;
          case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
            dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/PixelaCYR_15_DF") as GameObject).GetComponent<dfFont>();
            float num4 = (float) this.m_defaultAssignedFont.FontSize / (float) dfFontBase.FontSize;
            if ((double) num4 < 1.0)
              num4 = 1f;
            num1 = (double) num4 <= 1.0 ? this.m_defaultAssignedFontTextScale * num4 : (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * num4);
            break;
          case StringTableManager.GungeonSupportedLanguages.CHINESE:
            dfFontBase = (dfFontBase) (ResourceCache.Acquire("Alternate Fonts/SimSun12_DF") as GameObject).GetComponent<dfFont>();
            num1 = (float) Mathf.CeilToInt(this.m_defaultAssignedFontTextScale * Mathf.Max(1f, (float) this.m_defaultAssignedFont.FontSize / 14f));
            break;
          default:
            dfFontBase = this.m_defaultAssignedFont;
            break;
        }
        if ((UnityEngine.Object) dfFontBase != (UnityEngine.Object) null && (UnityEngine.Object) this.Font != (UnityEngine.Object) dfFontBase)
        {
          this.Font = dfFontBase;
          if (dfFontBase is dfFont)
            this.Atlas = (dfFontBase as dfFont).Atlas;
          this.TextScale = num1;
        }
        this.m_cachedLanguage = currentLanguage;
      }

      public Color32 TextColor
      {
        get => this.textColor;
        set
        {
          this.textColor = value;
          this.Invalidate();
        }
      }

      public Color32 HoverTextColor
      {
        get => this.hoverText;
        set
        {
          this.hoverText = value;
          this.Invalidate();
        }
      }

      public Color32 NormalBackgroundColor
      {
        get => this.normalColor;
        set
        {
          this.normalColor = value;
          this.Invalidate();
        }
      }

      public Color32 HoverBackgroundColor
      {
        get => this.hoverColor;
        set
        {
          this.hoverColor = value;
          this.Invalidate();
        }
      }

      public Color32 PressedTextColor
      {
        get => this.pressedText;
        set
        {
          this.pressedText = value;
          this.Invalidate();
        }
      }

      public Color32 PressedBackgroundColor
      {
        get => this.pressedColor;
        set
        {
          this.pressedColor = value;
          this.Invalidate();
        }
      }

      public Color32 FocusTextColor
      {
        get => this.focusText;
        set
        {
          this.focusText = value;
          this.Invalidate();
        }
      }

      public Color32 FocusBackgroundColor
      {
        get => this.focusColor;
        set
        {
          this.focusColor = value;
          this.Invalidate();
        }
      }

      public Color32 DisabledTextColor
      {
        get => this.disabledText;
        set
        {
          this.disabledText = value;
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

      public bool Shadow
      {
        get => this.textShadow;
        set
        {
          if (value == this.textShadow)
            return;
          this.textShadow = value;
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

      protected internal override void OnLocalize()
      {
        base.OnLocalize();
        if (this.text.StartsWith("#"))
          this.localizationKey = this.text;
        if (!string.IsNullOrEmpty(this.localizationKey) && this.localizationKey.StartsWith("#"))
          this.text = this.getLocalizedValue(this.localizationKey);
        else
          this.Text = this.getLocalizedValue(this.text);
        if (this.forceUpperCase)
          this.text = this.text.ToUpperInvariant();
        this.CheckFontsForLanguage();
        this.PerformLayout();
      }

      [HideInInspector]
      public override void Invalidate()
      {
        base.Invalidate();
        if (this.m_cachedLanguage != GameManager.Options.CurrentLanguage)
          this.CheckFontsForLanguage();
        if (!this.AutoSize)
          return;
        this.autoSizeToText();
      }

      public override void Start() => base.Start();

      public override void OnEnable()
      {
        base.OnEnable();
        bool flag = (UnityEngine.Object) this.Font != (UnityEngine.Object) null && this.Font.IsValid;
        if (Application.isPlaying && !flag)
          this.Font = this.GetManager().DefaultFont;
        this.bindTextureRebuildCallback();
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
        this.startSize = this.Size;
      }

      public float GetAutosizeWidth()
      {
        using (dfFontRendererBase textRenderer = this.obtainTextRenderer())
          return textRenderer.MeasureString(this.text).RoundToInt().x + (float) this.padding.horizontal;
      }

      protected internal override void OnEnterFocus(dfFocusEventArgs args)
      {
        if (this.State != dfButton.ButtonState.Pressed)
          this.State = dfButton.ButtonState.Focus;
        base.OnEnterFocus(args);
      }

      protected internal override void OnLeaveFocus(dfFocusEventArgs args)
      {
        this.State = dfButton.ButtonState.Default;
        base.OnLeaveFocus(args);
      }

      protected internal override void OnKeyPress(dfKeyEventArgs args)
      {
        if (this.ClickWhenSpacePressed && this.IsInteractive && args.KeyCode == KeyCode.Space)
          this.OnClick(new dfMouseEventArgs((dfControl) this, dfMouseButtons.Left, 1, new Ray(), Vector2.zero, 0.0f));
        else
          base.OnKeyPress(args);
      }

      protected internal override void OnClick(dfMouseEventArgs args)
      {
        if ((UnityEngine.Object) this.group != (UnityEngine.Object) null)
        {
          foreach (dfButton componentsInChild in this.transform.parent.GetComponentsInChildren<dfButton>())
          {
            if ((UnityEngine.Object) componentsInChild != (UnityEngine.Object) this && (UnityEngine.Object) componentsInChild.ButtonGroup == (UnityEngine.Object) this.ButtonGroup && (UnityEngine.Object) componentsInChild != (UnityEngine.Object) this)
              componentsInChild.State = dfButton.ButtonState.Default;
          }
          if (!this.transform.IsChildOf(this.group.transform))
            this.Signal(this.group.gameObject, nameof (OnClick), (object) args);
        }
        base.OnClick(args);
      }

      protected internal override void OnMouseDown(dfMouseEventArgs args)
      {
        if (!(this.parent is dfTabstrip) || this.State != dfButton.ButtonState.Focus)
          this.State = dfButton.ButtonState.Pressed;
        base.OnMouseDown(args);
      }

      protected internal override void OnMouseUp(dfMouseEventArgs args)
      {
        if (!this.IsEnabled)
        {
          this.State = dfButton.ButtonState.Disabled;
          base.OnMouseUp(args);
        }
        else
        {
          this.State = !this.isMouseHovering ? (!this.HasFocus ? dfButton.ButtonState.Default : dfButton.ButtonState.Focus) : (!(this.parent is dfTabstrip) || !this.ContainsFocus ? dfButton.ButtonState.Hover : dfButton.ButtonState.Focus);
          base.OnMouseUp(args);
        }
      }

      protected internal override void OnMouseEnter(dfMouseEventArgs args)
      {
        if (!(this.parent is dfTabstrip) || this.State != dfButton.ButtonState.Focus)
          this.State = dfButton.ButtonState.Hover;
        base.OnMouseEnter(args);
      }

      protected internal override void OnMouseLeave(dfMouseEventArgs args)
      {
        this.State = !this.ContainsFocus ? dfButton.ButtonState.Default : dfButton.ButtonState.Focus;
        base.OnMouseLeave(args);
      }

      protected internal override void OnIsEnabledChanged()
      {
        this.State = this.IsEnabled ? dfButton.ButtonState.Default : dfButton.ButtonState.Disabled;
        base.OnIsEnabledChanged();
      }

      protected virtual void OnButtonStateChanged(dfButton.ButtonState value)
      {
        if (value != dfButton.ButtonState.Disabled && !this.IsEnabled)
          value = dfButton.ButtonState.Disabled;
        this.state = value;
        this.Signal(nameof (OnButtonStateChanged), (object) this, (object) value);
        if (this.ButtonStateChanged != null)
          this.ButtonStateChanged((dfControl) this, value);
        this.Invalidate();
      }

      protected override Color32 getActiveColor()
      {
        switch (this.State)
        {
          case dfButton.ButtonState.Focus:
            return this.FocusBackgroundColor;
          case dfButton.ButtonState.Hover:
            return this.HoverBackgroundColor;
          case dfButton.ButtonState.Pressed:
            return this.PressedBackgroundColor;
          case dfButton.ButtonState.Disabled:
            return this.DisabledColor;
          default:
            return this.NormalBackgroundColor;
        }
      }

      private void autoSizeToText()
      {
        if ((UnityEngine.Object) this.Font == (UnityEngine.Object) null || !this.Font.IsValid || string.IsNullOrEmpty(this.Text))
          return;
        using (dfFontRendererBase textRenderer = this.obtainTextRenderer())
        {
          Vector2 vector2_1 = textRenderer.MeasureString(this.Text);
          Vector2 vector2_2 = new Vector2(vector2_1.x + (float) this.padding.horizontal, vector2_1.y + (float) this.padding.vertical);
          if (!(this.size != vector2_2))
            return;
          this.SuspendLayout();
          this.Size = vector2_2;
          this.ResumeLayout();
        }
      }

      private dfRenderData renderText()
      {
        if ((UnityEngine.Object) this.Font == (UnityEngine.Object) null || !this.Font.IsValid || string.IsNullOrEmpty(this.Text))
          return (dfRenderData) null;
        dfRenderData destination = this.renderData;
        if (this.font is dfDynamicFont)
        {
          dfDynamicFont font = (dfDynamicFont) this.font;
          destination = this.textRenderData;
          destination.Clear();
          destination.Material = font.Material;
        }
        using (dfFontRendererBase textRenderer = this.obtainTextRenderer())
          textRenderer.Render(this.text, destination);
        return destination;
      }

      private dfFontRendererBase obtainTextRenderer()
      {
        Vector2 vector2_1 = this.Size - new Vector2((float) this.padding.horizontal, (float) this.padding.vertical);
        Vector2 vector2_2 = !this.autoSize ? vector2_1 : Vector2.one * (float) int.MaxValue;
        float units = this.PixelsToUnits();
        Vector3 vector = (this.pivot.TransformToUpperLeft(this.Size) + new Vector3((float) this.padding.left, (float) -this.padding.top)) * units;
        float num1 = this.TextScale * this.getTextScaleMultiplier();
        Color32 color32 = this.ApplyOpacity(this.getTextColorForState());
        dfFontRendererBase renderer = this.Font.ObtainRenderer();
        renderer.WordWrap = this.WordWrap;
        renderer.MultiLine = this.WordWrap;
        renderer.MaxSize = vector2_2;
        renderer.PixelRatio = units;
        renderer.TextScale = num1;
        renderer.CharacterSpacing = 0;
        int num2 = this.textPixelOffset;
        if (this.state == dfButton.ButtonState.Hover)
          num2 = this.hoverTextPixelOffset;
        if (this.state == dfButton.ButtonState.Pressed)
          num2 = this.downTextPixelOffset;
        if (this.state == dfButton.ButtonState.Disabled)
          num2 = this.downTextPixelOffset;
        renderer.VectorOffset = vector.Quantize(units) + new Vector3(0.0f, (float) num2 * units, 0.0f);
        renderer.TabSize = 0;
        renderer.TextAlign = !this.autoSize ? this.TextAlignment : TextAlignment.Left;
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
        if (this.vertAlign != dfVerticalAlignment.Top)
          renderer.VectorOffset = this.getVertAlignOffset(renderer);
        return renderer;
      }

      private float getTextScaleMultiplier()
      {
        if (this.textScaleMode == dfTextScaleMode.None || !Application.isPlaying)
          return 1f;
        if (this.textScaleMode == dfTextScaleMode.ScreenResolution)
          return (float) Screen.height / (float) this.cachedManager.FixedHeight;
        return this.autoSize ? 1f : this.Size.y / this.startSize.y;
      }

      private Color32 getTextColorForState()
      {
        if (!this.IsEnabled)
          return this.DisabledTextColor;
        switch (this.state)
        {
          case dfButton.ButtonState.Default:
            return this.TextColor;
          case dfButton.ButtonState.Focus:
            return this.FocusTextColor;
          case dfButton.ButtonState.Hover:
            return this.HoverTextColor;
          case dfButton.ButtonState.Pressed:
            return this.PressedTextColor;
          case dfButton.ButtonState.Disabled:
            return this.DisabledTextColor;
          default:
            return (Color32) UnityEngine.Color.white;
        }
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

      protected internal override dfAtlas.ItemInfo getBackgroundSprite()
      {
        if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
          return (dfAtlas.ItemInfo) null;
        dfAtlas.ItemInfo backgroundSprite = (dfAtlas.ItemInfo) null;
        switch (this.state)
        {
          case dfButton.ButtonState.Default:
            backgroundSprite = this.atlas[this.backgroundSprite];
            break;
          case dfButton.ButtonState.Focus:
            backgroundSprite = this.atlas[this.focusSprite];
            break;
          case dfButton.ButtonState.Hover:
            backgroundSprite = this.atlas[this.hoverSprite];
            break;
          case dfButton.ButtonState.Pressed:
            backgroundSprite = this.atlas[this.pressedSprite];
            break;
          case dfButton.ButtonState.Disabled:
            backgroundSprite = this.atlas[this.disabledSprite];
            break;
        }
        if (backgroundSprite == (dfAtlas.ItemInfo) null)
          backgroundSprite = this.atlas[this.backgroundSprite];
        return backgroundSprite;
      }

      public dfList<dfRenderData> RenderMultiple()
      {
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
        this.isControlInvalidated = false;
        this.buffers.Clear();
        this.renderData.Clear();
        if ((UnityEngine.Object) this.Atlas != (UnityEngine.Object) null)
        {
          this.renderData.Material = this.Atlas.Material;
          this.renderData.Transform = localToWorldMatrix;
          this.renderBackground();
          this.buffers.Add(this.renderData);
        }
        dfRenderData dfRenderData = this.renderText();
        if (dfRenderData != null && dfRenderData != this.renderData)
        {
          dfRenderData.Transform = localToWorldMatrix;
          this.buffers.Add(dfRenderData);
        }
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

      public enum ButtonState
      {
        Default,
        Focus,
        Hover,
        Pressed,
        Disabled,
      }
    }

}
