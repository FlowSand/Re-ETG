// Decompiled with JetBrains decompiler
// Type: dfRichTextLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [AddComponentMenu("Daikon Forge/User Interface/Rich Text Label")]
    [ExecuteInEditMode]
    [Serializable]
    public class dfRichTextLabel : dfControl, IDFMultiRender, IRendersText
    {
      [SerializeField]
      protected dfAtlas atlas;
      [SerializeField]
      protected dfDynamicFont font;
      [SerializeField]
      protected string text = "Rich Text Label";
      [SerializeField]
      protected int fontSize = 16 /*0x10*/;
      [SerializeField]
      protected int lineheight = 16 /*0x10*/;
      [SerializeField]
      protected dfTextScaleMode textScaleMode;
      [SerializeField]
      protected FontStyle fontStyle;
      [SerializeField]
      protected bool preserveWhitespace;
      [SerializeField]
      protected string blankTextureSprite;
      [SerializeField]
      protected dfMarkupTextAlign align;
      [SerializeField]
      protected bool allowScrolling;
      [SerializeField]
      protected dfScrollbar horzScrollbar;
      [SerializeField]
      protected dfScrollbar vertScrollbar;
      [SerializeField]
      protected bool useScrollMomentum;
      [SerializeField]
      protected bool autoHeight;
      private static dfRenderData clipBuffer = new dfRenderData();
      private dfList<dfRenderData> buffers = new dfList<dfRenderData>();
      private dfList<dfMarkupElement> elements;
      private dfMarkupBox viewportBox;
      private dfMarkupTag mouseDownTag;
      private Vector2 mouseDownScrollPosition = Vector2.zero;
      private Vector2 scrollPosition = Vector2.zero;
      private bool initialized;
      private bool isMouseDown;
      private Vector2 touchStartPosition = Vector2.zero;
      private Vector2 scrollMomentum = Vector2.zero;
      private bool isMarkupInvalidated = true;
      private Vector2 startSize = Vector2.zero;
      private bool isFontCallbackAssigned;

      public event PropertyChangedEventHandler<string> TextChanged;

      public event PropertyChangedEventHandler<Vector2> ScrollPositionChanged;

      public event dfRichTextLabel.LinkClickEventHandler LinkClicked;

      public bool AutoHeight
      {
        get => this.autoHeight;
        set
        {
          if (this.autoHeight == value)
            return;
          this.autoHeight = value;
          this.scrollPosition = Vector2.zero;
          this.Invalidate();
        }
      }

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

      public dfDynamicFont Font
      {
        get => this.font;
        set
        {
          if (!((UnityEngine.Object) value != (UnityEngine.Object) this.font))
            return;
          this.unbindTextureRebuildCallback();
          this.font = value;
          this.bindTextureRebuildCallback();
          this.LineHeight = value.FontSize;
          dfFontManager.Invalidate((dfFontBase) this.Font);
          this.Invalidate();
        }
      }

      public string BlankTextureSprite
      {
        get => this.blankTextureSprite;
        set
        {
          if (!(value != this.blankTextureSprite))
            return;
          this.blankTextureSprite = value;
          this.Invalidate();
        }
      }

      public string Text
      {
        get => this.text;
        set
        {
          value = this.getLocalizedValue(value);
          if (string.Equals(this.text, value))
            return;
          dfFontManager.Invalidate((dfFontBase) this.Font);
          this.text = value;
          this.scrollPosition = Vector2.zero;
          this.Invalidate();
          this.OnTextChanged();
        }
      }

      public int FontSize
      {
        get => this.fontSize;
        set
        {
          value = Mathf.Max(6, value);
          if (value != this.fontSize)
          {
            dfFontManager.Invalidate((dfFontBase) this.Font);
            this.fontSize = value;
            this.Invalidate();
          }
          this.LineHeight = value;
        }
      }

      public int LineHeight
      {
        get => this.lineheight;
        set
        {
          value = Mathf.Max(this.FontSize, value);
          if (value == this.lineheight)
            return;
          this.lineheight = value;
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

      public bool PreserveWhitespace
      {
        get => this.preserveWhitespace;
        set
        {
          if (value == this.preserveWhitespace)
            return;
          this.preserveWhitespace = value;
          this.Invalidate();
        }
      }

      public FontStyle FontStyle
      {
        get => this.fontStyle;
        set
        {
          if (value == this.fontStyle)
            return;
          this.fontStyle = value;
          this.Invalidate();
        }
      }

      public dfMarkupTextAlign TextAlignment
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

      public bool AllowScrolling
      {
        get => this.allowScrolling;
        set
        {
          this.allowScrolling = value;
          if (value)
            return;
          this.ScrollPosition = Vector2.zero;
        }
      }

      public Vector2 ScrollPosition
      {
        get => this.scrollPosition;
        set
        {
          if (!this.allowScrolling || this.autoHeight)
            value = Vector2.zero;
          if (this.isMarkupInvalidated)
            this.processMarkup();
          value = Vector2.Min(this.ContentSize - this.Size, value);
          value = Vector2.Max(Vector2.zero, value);
          value = value.RoundToInt();
          if ((double) (value - this.scrollPosition).sqrMagnitude <= 1.4012984643248171E-45)
            return;
          this.scrollPosition = value;
          this.updateScrollbars();
          this.OnScrollPositionChanged();
        }
      }

      public dfScrollbar HorizontalScrollbar
      {
        get => this.horzScrollbar;
        set
        {
          this.horzScrollbar = value;
          this.updateScrollbars();
        }
      }

      public dfScrollbar VerticalScrollbar
      {
        get => this.vertScrollbar;
        set
        {
          this.vertScrollbar = value;
          this.updateScrollbars();
        }
      }

      public Vector2 ContentSize => this.viewportBox != null ? this.viewportBox.Size : this.Size;

      public bool UseScrollMomentum
      {
        get => this.useScrollMomentum;
        set
        {
          this.useScrollMomentum = value;
          this.scrollMomentum = Vector2.zero;
        }
      }

      protected internal override void OnLocalize()
      {
        base.OnLocalize();
        this.Text = this.getLocalizedValue(this.text);
      }

      [HideInInspector]
      public override void Invalidate()
      {
        base.Invalidate();
        dfFontManager.Invalidate((dfFontBase) this.Font);
        this.isMarkupInvalidated = true;
      }

      public override void Awake()
      {
        base.Awake();
        this.startSize = this.Size;
      }

      public override void OnEnable()
      {
        base.OnEnable();
        this.bindTextureRebuildCallback();
        if ((double) this.size.sqrMagnitude > 1.4012984643248171E-45)
          return;
        this.Size = new Vector2(320f, 200f);
        int num = 16 /*0x10*/;
        this.LineHeight = num;
        this.FontSize = num;
      }

      public override void OnDisable()
      {
        base.OnDisable();
        this.unbindTextureRebuildCallback();
      }

      public override void Update()
      {
        base.Update();
        if (!this.useScrollMomentum || this.isMouseDown || (double) this.scrollMomentum.magnitude <= 0.5)
          return;
        this.ScrollPosition += this.scrollMomentum;
        this.scrollMomentum *= 0.95f - BraveTime.DeltaTime;
      }

      public override void LateUpdate()
      {
        base.LateUpdate();
        this.initialize();
      }

      protected internal void OnTextChanged()
      {
        this.Invalidate();
        this.Signal(nameof (OnTextChanged), (object) this, (object) this.text);
        if (this.TextChanged == null)
          return;
        this.TextChanged((dfControl) this, this.text);
      }

      protected internal void OnScrollPositionChanged()
      {
        base.Invalidate();
        this.SignalHierarchy(nameof (OnScrollPositionChanged), (object) this, (object) this.ScrollPosition);
        if (this.ScrollPositionChanged == null)
          return;
        this.ScrollPositionChanged((dfControl) this, this.ScrollPosition);
      }

      protected internal override void OnKeyDown(dfKeyEventArgs args)
      {
        if (args.Used)
        {
          base.OnKeyDown(args);
        }
        else
        {
          int fontSize1 = this.FontSize;
          int fontSize2 = this.FontSize;
          switch (args.KeyCode)
          {
            case KeyCode.UpArrow:
              this.ScrollPosition += new Vector2(0.0f, (float) -fontSize2);
              args.Use();
              break;
            case KeyCode.DownArrow:
              this.ScrollPosition += new Vector2(0.0f, (float) fontSize2);
              args.Use();
              break;
            case KeyCode.RightArrow:
              this.ScrollPosition += new Vector2((float) fontSize1, 0.0f);
              args.Use();
              break;
            case KeyCode.LeftArrow:
              this.ScrollPosition += new Vector2((float) -fontSize1, 0.0f);
              args.Use();
              break;
            case KeyCode.Home:
              this.ScrollToTop();
              args.Use();
              break;
            case KeyCode.End:
              this.ScrollToBottom();
              args.Use();
              break;
          }
          base.OnKeyDown(args);
        }
      }

      internal override void OnDragEnd(dfDragEventArgs args)
      {
        base.OnDragEnd(args);
        this.isMouseDown = false;
      }

      protected internal override void OnMouseEnter(dfMouseEventArgs args)
      {
        base.OnMouseEnter(args);
        this.touchStartPosition = args.Position;
      }

      protected internal override void OnMouseDown(dfMouseEventArgs args)
      {
        base.OnMouseDown(args);
        this.mouseDownTag = this.hitTestTag(args);
        this.mouseDownScrollPosition = this.scrollPosition;
        this.scrollMomentum = Vector2.zero;
        this.touchStartPosition = args.Position;
        this.isMouseDown = true;
      }

      protected internal override void OnMouseUp(dfMouseEventArgs args)
      {
        base.OnMouseUp(args);
        this.isMouseDown = false;
        if ((double) Vector2.Distance(this.scrollPosition, this.mouseDownScrollPosition) <= 2.0 && this.hitTestTag(args) == this.mouseDownTag)
        {
          dfMarkupTag tag = this.mouseDownTag;
          while (true)
          {
            switch (tag)
            {
              case null:
              case dfMarkupTagAnchor _:
                goto label_4;
              default:
                tag = tag.Parent as dfMarkupTag;
                continue;
            }
          }
    label_4:
          if (tag is dfMarkupTagAnchor)
          {
            this.Signal("OnLinkClicked", (object) this, (object) tag);
            if (this.LinkClicked != null)
              this.LinkClicked(this, tag as dfMarkupTagAnchor);
          }
        }
        this.mouseDownTag = (dfMarkupTag) null;
        this.mouseDownScrollPosition = this.scrollPosition;
      }

      protected internal override void OnMouseMove(dfMouseEventArgs args)
      {
        base.OnMouseMove(args);
        if (!this.allowScrolling || this.autoHeight || !(args is dfTouchEventArgs) && !this.isMouseDown || (double) (args.Position - this.touchStartPosition).magnitude <= 5.0)
          return;
        Vector2 vector2 = args.MoveDelta.Scale(-1f, 1f);
        Vector2 screenSize = this.GetManager().GetScreenSize();
        Camera camera = Camera.main ?? this.GetCamera();
        vector2.x = screenSize.x * (vector2.x / (float) camera.pixelWidth);
        vector2.y = screenSize.y * (vector2.y / (float) camera.pixelHeight);
        this.ScrollPosition += vector2;
        this.scrollMomentum = (this.scrollMomentum + vector2) * 0.5f;
      }

      protected internal override void OnMouseWheel(dfMouseEventArgs args)
      {
        try
        {
          if (args.Used || !this.allowScrolling || this.autoHeight)
            return;
          int num1 = !this.UseScrollMomentum ? 3 : 1;
          float num2 = !((UnityEngine.Object) this.vertScrollbar != (UnityEngine.Object) null) ? (float) (this.FontSize * num1) : this.vertScrollbar.IncrementAmount;
          this.ScrollPosition = new Vector2(this.scrollPosition.x, this.scrollPosition.y - num2 * args.WheelDelta);
          this.scrollMomentum = new Vector2(0.0f, -num2 * args.WheelDelta);
          args.Use();
          this.Signal(nameof (OnMouseWheel), (object) this, (object) args);
        }
        finally
        {
          base.OnMouseWheel(args);
        }
      }

      public void ScrollToTop() => this.ScrollPosition = new Vector2(this.scrollPosition.x, 0.0f);

      public void ScrollToBottom()
      {
        this.ScrollPosition = new Vector2(this.scrollPosition.x, (float) int.MaxValue);
      }

      public void ScrollToLeft() => this.ScrollPosition = new Vector2(0.0f, this.scrollPosition.y);

      public void ScrollToRight()
      {
        this.ScrollPosition = new Vector2((float) int.MaxValue, this.scrollPosition.y);
      }

      public dfList<dfRenderData> RenderMultiple()
      {
        if (!this.isVisible || (UnityEngine.Object) this.Font == (UnityEngine.Object) null)
          return (dfList<dfRenderData>) null;
        Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
        if (!this.isControlInvalidated)
        {
          if (this.viewportBox != null)
          {
            for (int index = 0; index < this.buffers.Count; ++index)
              this.buffers[index].Transform = localToWorldMatrix;
            return this.buffers;
          }
        }
        try
        {
          this.isControlInvalidated = false;
          if (this.isMarkupInvalidated)
          {
            this.isMarkupInvalidated = false;
            this.processMarkup();
          }
          this.viewportBox.FitToContents();
          if (this.autoHeight)
            this.Height = (float) this.viewportBox.Height;
          this.updateScrollbars();
          this.buffers.Clear();
          this.gatherRenderBuffers(this.viewportBox, this.buffers);
          return this.buffers;
        }
        finally
        {
          this.updateCollider();
        }
      }

      private dfMarkupTag hitTestTag(dfMouseEventArgs args)
      {
        dfMarkupBox dfMarkupBox = this.viewportBox.HitTest(this.GetHitPosition(args) + this.scrollPosition);
        if (dfMarkupBox == null)
          return (dfMarkupTag) null;
        dfMarkupElement dfMarkupElement = dfMarkupBox.Element;
        while (true)
        {
          switch (dfMarkupElement)
          {
            case null:
            case dfMarkupTag _:
              goto label_4;
            default:
              dfMarkupElement = dfMarkupElement.Parent;
              continue;
          }
        }
    label_4:
        return dfMarkupElement as dfMarkupTag;
      }

      private void processMarkup()
      {
        this.releaseMarkupReferences();
        this.elements = dfMarkupParser.Parse(this, this.text);
        float textScaleMultiplier = this.getTextScaleMultiplier();
        int num1 = Mathf.CeilToInt((float) this.FontSize * textScaleMultiplier);
        int num2 = Mathf.CeilToInt((float) this.LineHeight * textScaleMultiplier);
        dfMarkupStyle style = new dfMarkupStyle()
        {
          Host = this,
          Atlas = this.Atlas,
          Font = this.Font,
          FontSize = num1,
          FontStyle = this.FontStyle,
          LineHeight = num2,
          Color = (UnityEngine.Color) this.ApplyOpacity(this.Color),
          Opacity = this.CalculateOpacity(),
          Align = this.TextAlignment,
          PreserveWhitespace = this.preserveWhitespace
        };
        this.viewportBox = new dfMarkupBox((dfMarkupElement) null, dfMarkupDisplayType.block, style)
        {
          Size = this.Size
        };
        for (int index = 0; index < this.elements.Count; ++index)
          this.elements[index]?.PerformLayout(this.viewportBox, style);
      }

      private float getTextScaleMultiplier()
      {
        if (this.textScaleMode == dfTextScaleMode.None || !Application.isPlaying)
          return 1f;
        return this.textScaleMode == dfTextScaleMode.ScreenResolution ? (float) Screen.height / (float) this.cachedManager.FixedHeight : this.Size.y / this.startSize.y;
      }

      private void releaseMarkupReferences()
      {
        this.mouseDownTag = (dfMarkupTag) null;
        if (this.viewportBox != null)
          this.viewportBox.Release();
        if (this.elements == null)
          return;
        for (int index = 0; index < this.elements.Count; ++index)
          this.elements[index].Release();
        this.elements.Release();
      }

      [HideInInspector]
      private void initialize()
      {
        if (this.initialized)
          return;
        this.initialized = true;
        if (Application.isPlaying)
        {
          if ((UnityEngine.Object) this.horzScrollbar != (UnityEngine.Object) null)
            this.horzScrollbar.ValueChanged += new PropertyChangedEventHandler<float>(this.horzScroll_ValueChanged);
          if ((UnityEngine.Object) this.vertScrollbar != (UnityEngine.Object) null)
            this.vertScrollbar.ValueChanged += new PropertyChangedEventHandler<float>(this.vertScroll_ValueChanged);
        }
        this.Invalidate();
        this.ScrollPosition = Vector2.zero;
        this.updateScrollbars();
      }

      private void vertScroll_ValueChanged(dfControl control, float value)
      {
        this.ScrollPosition = new Vector2(this.scrollPosition.x, value);
      }

      private void horzScroll_ValueChanged(dfControl control, float value)
      {
        this.ScrollPosition = new Vector2(value, this.ScrollPosition.y);
      }

      private void updateScrollbars()
      {
        if ((UnityEngine.Object) this.horzScrollbar != (UnityEngine.Object) null)
        {
          this.horzScrollbar.MinValue = 0.0f;
          this.horzScrollbar.MaxValue = this.ContentSize.x;
          this.horzScrollbar.ScrollSize = this.Size.x;
          this.horzScrollbar.Value = this.ScrollPosition.x;
        }
        if (!((UnityEngine.Object) this.vertScrollbar != (UnityEngine.Object) null))
          return;
        this.vertScrollbar.MinValue = 0.0f;
        this.vertScrollbar.MaxValue = this.ContentSize.y;
        this.vertScrollbar.ScrollSize = this.Size.y;
        this.vertScrollbar.Value = this.ScrollPosition.y;
      }

      private void gatherRenderBuffers(dfMarkupBox box, dfList<dfRenderData> buffers)
      {
        dfIntersectionType viewportIntersection = this.getViewportIntersection(box);
        if (viewportIntersection == dfIntersectionType.None)
          return;
        dfRenderData renderData = box.Render();
        if (renderData != null)
        {
          if ((UnityEngine.Object) renderData.Material == (UnityEngine.Object) null && (UnityEngine.Object) this.atlas != (UnityEngine.Object) null)
            renderData.Material = this.atlas.Material;
          float units = this.PixelsToUnits();
          Vector3 vector3 = (Vector3) (-this.scrollPosition.Scale(1f, -1f).RoundToInt() + box.GetOffset().Scale(1f, -1f)) + this.pivot.TransformToUpperLeft(this.Size);
          dfList<Vector3> vertices = renderData.Vertices;
          for (int index = 0; index < renderData.Vertices.Count; ++index)
            vertices[index] = (vector3 + vertices[index]) * units;
          if (viewportIntersection == dfIntersectionType.Intersecting)
            this.clipToViewport(renderData);
          renderData.Transform = this.transform.localToWorldMatrix;
          buffers.Add(renderData);
        }
        for (int index = 0; index < box.Children.Count; ++index)
          this.gatherRenderBuffers(box.Children[index], buffers);
      }

      private dfIntersectionType getViewportIntersection(dfMarkupBox box)
      {
        if (box.Display == dfMarkupDisplayType.none)
          return dfIntersectionType.None;
        Vector2 size = this.Size;
        Vector2 vector2_1 = box.GetOffset() - this.scrollPosition;
        Vector2 vector2_2 = vector2_1 + box.Size;
        if ((double) vector2_2.x <= 0.0 || (double) vector2_2.y <= 0.0 || (double) vector2_1.x >= (double) size.x || (double) vector2_1.y >= (double) size.y)
          return dfIntersectionType.None;
        return (double) vector2_1.x < 0.0 || (double) vector2_1.y < 0.0 || (double) vector2_2.x > (double) size.x || (double) vector2_2.y > (double) size.y ? dfIntersectionType.Intersecting : dfIntersectionType.Inside;
      }

      private void clipToViewport(dfRenderData renderData)
      {
        Plane[] viewportClippingPlanes = this.getViewportClippingPlanes();
        Material material = renderData.Material;
        Matrix4x4 transform = renderData.Transform;
        dfRichTextLabel.clipBuffer.Clear();
        dfClippingUtil.Clip((IList<Plane>) viewportClippingPlanes, renderData, dfRichTextLabel.clipBuffer);
        renderData.Clear();
        renderData.Merge(dfRichTextLabel.clipBuffer, false);
        renderData.Material = material;
        renderData.Transform = transform;
      }

      private Plane[] getViewportClippingPlanes()
      {
        Vector3[] corners = this.GetCorners();
        Matrix4x4 worldToLocalMatrix = this.transform.worldToLocalMatrix;
        for (int index = 0; index < corners.Length; ++index)
          corners[index] = worldToLocalMatrix.MultiplyPoint(corners[index]);
        this.cachedClippingPlanes[0] = new Plane(Vector3.right, corners[0]);
        this.cachedClippingPlanes[1] = new Plane(Vector3.left, corners[1]);
        this.cachedClippingPlanes[2] = new Plane(Vector3.up, corners[2]);
        this.cachedClippingPlanes[3] = new Plane(Vector3.down, corners[0]);
        return this.cachedClippingPlanes;
      }

      public void UpdateFontInfo()
      {
        if (!dfFontManager.IsDirty((dfFontBase) this.Font) || string.IsNullOrEmpty(this.text))
          return;
        this.updateFontInfo(this.viewportBox);
      }

      private void updateFontInfo(dfMarkupBox box)
      {
        if (box == null || (box != this.viewportBox ? (int) this.getViewportIntersection(box) : 1) == 0)
          return;
        if (box is dfMarkupBoxText dfMarkupBoxText)
          this.font.AddCharacterRequest(dfMarkupBoxText.Text, dfMarkupBoxText.Style.FontSize, dfMarkupBoxText.Style.FontStyle);
        for (int index = 0; index < box.Children.Count; ++index)
          this.updateFontInfo(box.Children[index]);
      }

      private void onFontTextureRebuilt(UnityEngine.Font font)
      {
        if (this.Font == null || !((UnityEngine.Object) font == (UnityEngine.Object) this.Font.BaseFont))
          return;
        this.Invalidate();
        this.updateFontInfo(this.viewportBox);
      }

      private void bindTextureRebuildCallback()
      {
        if (this.isFontCallbackAssigned || (UnityEngine.Object) this.Font == (UnityEngine.Object) null)
          return;
        UnityEngine.Font.textureRebuilt += new Action<UnityEngine.Font>(this.onFontTextureRebuilt);
        this.isFontCallbackAssigned = true;
      }

      private void unbindTextureRebuildCallback()
      {
        if (!this.isFontCallbackAssigned || (UnityEngine.Object) this.Font == (UnityEngine.Object) null)
          return;
        UnityEngine.Font.textureRebuilt -= new Action<UnityEngine.Font>(this.onFontTextureRebuilt);
        this.isFontCallbackAssigned = false;
      }

      [dfEventCategory("Markup")]
      public delegate void LinkClickEventHandler(dfRichTextLabel sender, dfMarkupTagAnchor tag);
    }

}
