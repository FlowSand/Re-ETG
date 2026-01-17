// Decompiled with JetBrains decompiler
// Type: dfScrollPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
[dfTooltip("Implements a scrollable control container")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_scroll_panel.html")]
[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/User Interface/Containers/Scrollable Panel")]
[dfCategory("Basic Controls")]
[Serializable]
public class dfScrollPanel : dfControl
{
  [SerializeField]
  protected dfAtlas atlas;
  [SerializeField]
  protected string backgroundSprite;
  [SerializeField]
  protected Color32 backgroundColor = (Color32) UnityEngine.Color.white;
  [SerializeField]
  protected bool autoReset = true;
  [SerializeField]
  protected bool autoLayout;
  [SerializeField]
  protected RectOffset scrollPadding = new RectOffset();
  [SerializeField]
  protected RectOffset autoScrollPadding = new RectOffset();
  [SerializeField]
  protected RectOffset flowPadding = new RectOffset();
  [SerializeField]
  protected dfScrollPanel.LayoutDirection flowDirection;
  [SerializeField]
  protected bool wrapLayout;
  [SerializeField]
  protected Vector2 scrollPosition = Vector2.zero;
  [SerializeField]
  protected int scrollWheelAmount = 10;
  [SerializeField]
  protected dfScrollbar horzScroll;
  [SerializeField]
  protected dfScrollbar vertScroll;
  [SerializeField]
  protected dfControlOrientation wheelDirection;
  [SerializeField]
  protected bool scrollWithArrowKeys;
  [SerializeField]
  protected bool useScrollMomentum;
  [SerializeField]
  protected bool useVirtualScrolling;
  [SerializeField]
  protected bool autoFitVirtualTiles = true;
  [SerializeField]
  protected dfControl virtualScrollingTile;
  public bool LockScrollPanelToZero;
  private bool initialized;
  private bool resetNeeded;
  private bool scrolling;
  private bool isMouseDown;
  private Vector2 touchStartPosition = Vector2.zero;
  private Vector2 scrollMomentum = Vector2.zero;
  private object virtualScrollData;

  public event PropertyChangedEventHandler<Vector2> ScrollPositionChanged;

  public bool UseScrollMomentum
  {
    get => this.useScrollMomentum;
    set
    {
      this.useScrollMomentum = value;
      this.scrollMomentum = Vector2.zero;
    }
  }

  public bool ScrollWithArrowKeys
  {
    get => this.scrollWithArrowKeys;
    set => this.scrollWithArrowKeys = value;
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

  public bool AutoReset
  {
    get => this.autoReset;
    set
    {
      if (value == this.autoReset)
        return;
      this.autoReset = value;
      this.Reset();
    }
  }

  public RectOffset ScrollPadding
  {
    get
    {
      if (this.scrollPadding == null)
        this.scrollPadding = new RectOffset();
      return this.scrollPadding;
    }
    set
    {
      value = value.ConstrainPadding();
      if (object.Equals((object) value, (object) this.scrollPadding))
        return;
      this.scrollPadding = value;
      if (!this.AutoReset && !this.AutoLayout)
        return;
      this.Reset();
    }
  }

  public RectOffset AutoScrollPadding
  {
    get
    {
      if (this.autoScrollPadding == null)
        this.autoScrollPadding = new RectOffset();
      return this.autoScrollPadding;
    }
    set
    {
      value = value.ConstrainPadding();
      if (object.Equals((object) value, (object) this.autoScrollPadding))
        return;
      this.autoScrollPadding = value;
      if (!this.AutoReset && !this.AutoLayout)
        return;
      this.Reset();
    }
  }

  public bool AutoLayout
  {
    get => this.autoLayout;
    set
    {
      if (value == this.autoLayout)
        return;
      this.autoLayout = value;
      if (!this.AutoReset && !this.AutoLayout)
        return;
      this.Reset();
    }
  }

  public bool WrapLayout
  {
    get => this.wrapLayout;
    set
    {
      if (value == this.wrapLayout)
        return;
      this.wrapLayout = value;
      this.Reset();
    }
  }

  public dfScrollPanel.LayoutDirection FlowDirection
  {
    get => this.flowDirection;
    set
    {
      if (value == this.flowDirection)
        return;
      this.flowDirection = value;
      this.Reset();
    }
  }

  public RectOffset FlowPadding
  {
    get
    {
      if (this.flowPadding == null)
        this.flowPadding = new RectOffset();
      return this.flowPadding;
    }
    set
    {
      value = value.ConstrainPadding();
      if (object.Equals((object) value, (object) this.flowPadding))
        return;
      this.flowPadding = value;
      this.Reset();
    }
  }

  public Vector2 GetMaxScrollPositionDimensions()
  {
    return this.calculateViewSize() - new Vector2(this.size.x - (float) this.scrollPadding.horizontal, this.size.y - (float) this.scrollPadding.vertical);
  }

  public Vector2 ScrollPosition
  {
    get => this.scrollPosition;
    set
    {
      value = Vector2.Min(this.calculateViewSize() - new Vector2(this.size.x - (float) this.scrollPadding.horizontal, this.size.y - (float) this.scrollPadding.vertical), value);
      value = Vector2.Max(Vector2.zero, value);
      value = value.RoundToInt();
      if ((double) (value - this.scrollPosition).sqrMagnitude > 1.4012984643248171E-45)
      {
        Vector2 delta = value - this.scrollPosition;
        this.scrollPosition = value;
        this.scrollChildControls((Vector3) delta);
        this.updateScrollbars();
      }
      this.OnScrollPositionChanged();
    }
  }

  public int ScrollWheelAmount
  {
    get => this.scrollWheelAmount;
    set => this.scrollWheelAmount = value;
  }

  public dfScrollbar HorzScrollbar
  {
    get => this.horzScroll;
    set
    {
      this.horzScroll = value;
      this.updateScrollbars();
    }
  }

  public dfScrollbar VertScrollbar
  {
    get => this.vertScroll;
    set
    {
      this.vertScroll = value;
      this.updateScrollbars();
    }
  }

  public dfControlOrientation WheelScrollDirection
  {
    get => this.wheelDirection;
    set => this.wheelDirection = value;
  }

  public bool UseVirtualScrolling
  {
    get => this.useVirtualScrolling;
    set
    {
      this.useVirtualScrolling = value;
      if (value)
        return;
      this.VirtualScrollingTile = (dfControl) null;
    }
  }

  public bool AutoFitVirtualTiles
  {
    get => this.autoFitVirtualTiles;
    set => this.autoFitVirtualTiles = value;
  }

  public dfControl VirtualScrollingTile
  {
    get => this.useVirtualScrolling ? this.virtualScrollingTile : (dfControl) null;
    set => this.virtualScrollingTile = !this.useVirtualScrolling ? (dfControl) null : value;
  }

  protected internal override RectOffset GetClipPadding()
  {
    return this.scrollPadding ?? dfRectOffsetExtensions.Empty;
  }

  protected internal override Plane[] GetClippingPlanes()
  {
    if (!this.ClipChildren)
      return (Plane[]) null;
    Vector3[] corners = this.GetCorners();
    Vector3 inNormal1 = this.transform.TransformDirection(Vector3.right);
    Vector3 inNormal2 = this.transform.TransformDirection(Vector3.left);
    Vector3 inNormal3 = this.transform.TransformDirection(Vector3.up);
    Vector3 inNormal4 = this.transform.TransformDirection(Vector3.down);
    float units = this.PixelsToUnits();
    RectOffset scrollPadding = this.ScrollPadding;
    corners[0] += inNormal1 * (float) scrollPadding.left * units + inNormal4 * (float) scrollPadding.top * units;
    corners[1] += inNormal2 * (float) scrollPadding.right * units + inNormal4 * (float) scrollPadding.top * units;
    corners[2] += inNormal1 * (float) scrollPadding.left * units + inNormal3 * (float) scrollPadding.bottom * units;
    return new Plane[4]
    {
      new Plane(inNormal1, corners[0]),
      new Plane(inNormal2, corners[1]),
      new Plane(inNormal3, corners[2]),
      new Plane(inNormal4, corners[0])
    };
  }

  public override bool CanFocus => this.IsEnabled && this.IsVisible || base.CanFocus;

  public override void OnDestroy()
  {
    if ((UnityEngine.Object) this.horzScroll != (UnityEngine.Object) null)
      this.horzScroll.ValueChanged -= new PropertyChangedEventHandler<float>(this.horzScroll_ValueChanged);
    if ((UnityEngine.Object) this.vertScroll != (UnityEngine.Object) null)
      this.vertScroll.ValueChanged -= new PropertyChangedEventHandler<float>(this.vertScroll_ValueChanged);
    this.horzScroll = (dfScrollbar) null;
    this.vertScroll = (dfScrollbar) null;
  }

  public override void Update()
  {
    base.Update();
    if (this.useScrollMomentum && !this.isMouseDown && (double) this.scrollMomentum.magnitude > 0.25)
    {
      this.ScrollPosition += this.scrollMomentum;
      this.scrollMomentum *= 0.95f - BraveTime.DeltaTime;
    }
    if (!this.isControlInvalidated || !this.autoLayout || !this.IsVisible)
      return;
    this.AutoArrange();
    this.updateScrollbars();
  }

  public override void LateUpdate()
  {
    base.LateUpdate();
    if (this.LockScrollPanelToZero)
      this.ScrollPosition = Vector2.zero;
    this.initialize();
    if (!this.resetNeeded || !this.IsVisible)
      return;
    this.resetNeeded = false;
    if (!this.autoReset && !this.autoLayout)
      return;
    this.Reset();
  }

  public override void OnEnable()
  {
    base.OnEnable();
    if (this.size == Vector2.zero)
    {
      this.SuspendLayout();
      Camera camera = this.GetCamera();
      this.Size = (Vector2) new Vector3((float) (camera.pixelWidth / 2), (float) (camera.pixelHeight / 2));
      this.ResumeLayout();
    }
    if (this.autoLayout)
      this.AutoArrange();
    this.updateScrollbars();
  }

  protected internal override void OnIsVisibleChanged()
  {
    base.OnIsVisibleChanged();
    if (!this.IsVisible || !this.autoReset && !this.autoLayout)
      return;
    this.Reset();
    this.updateScrollbars();
  }

  protected internal override void OnSizeChanged()
  {
    base.OnSizeChanged();
    if (this.autoReset || this.autoLayout)
    {
      this.Reset();
    }
    else
    {
      Vector2 minChildPosition = this.calculateMinChildPosition();
      if ((double) minChildPosition.x > (double) this.scrollPadding.left || (double) minChildPosition.y > (double) this.scrollPadding.top)
        this.scrollChildControls((Vector3) Vector2.Max(minChildPosition - new Vector2((float) this.scrollPadding.left, (float) this.scrollPadding.top), Vector2.zero));
      this.updateScrollbars();
    }
  }

  protected internal override void OnResolutionChanged(
    Vector2 previousResolution,
    Vector2 currentResolution)
  {
    base.OnResolutionChanged(previousResolution, currentResolution);
    this.resetNeeded = this.AutoLayout || this.AutoReset;
  }

  protected internal override void OnGotFocus(dfFocusEventArgs args)
  {
    if ((UnityEngine.Object) args.Source != (UnityEngine.Object) this && args.AllowScrolling && InputManager.ActiveDevice != null)
      this.ScrollIntoView(args.Source);
    base.OnGotFocus(args);
  }

  protected internal override void OnKeyDown(dfKeyEventArgs args)
  {
    if (!this.scrollWithArrowKeys || args.Used)
    {
      base.OnKeyDown(args);
    }
    else
    {
      float x = !((UnityEngine.Object) this.horzScroll != (UnityEngine.Object) null) ? 1f : this.horzScroll.IncrementAmount;
      float y = !((UnityEngine.Object) this.vertScroll != (UnityEngine.Object) null) ? 1f : this.vertScroll.IncrementAmount;
      if (args.KeyCode == KeyCode.LeftArrow)
      {
        this.ScrollPosition += new Vector2(-x, 0.0f);
        args.Use();
      }
      else if (args.KeyCode == KeyCode.RightArrow)
      {
        this.ScrollPosition += new Vector2(x, 0.0f);
        args.Use();
      }
      else if (args.KeyCode == KeyCode.UpArrow)
      {
        this.ScrollPosition += new Vector2(0.0f, -y);
        args.Use();
      }
      else if (args.KeyCode == KeyCode.DownArrow)
      {
        this.ScrollPosition += new Vector2(0.0f, y);
        args.Use();
      }
      base.OnKeyDown(args);
    }
  }

  protected internal override void OnMouseEnter(dfMouseEventArgs args)
  {
    base.OnMouseEnter(args);
    this.touchStartPosition = args.Position;
  }

  protected internal override void OnMouseDown(dfMouseEventArgs args)
  {
    base.OnMouseDown(args);
    this.scrollMomentum = Vector2.zero;
    this.touchStartPosition = args.Position;
    this.isMouseDown = this.IsInteractive;
  }

  internal override void OnDragStart(dfDragEventArgs args)
  {
    base.OnDragStart(args);
    this.scrollMomentum = Vector2.zero;
    if (!args.Used)
      return;
    this.isMouseDown = false;
  }

  protected internal override void OnMouseUp(dfMouseEventArgs args)
  {
    base.OnMouseUp(args);
    this.isMouseDown = false;
  }

  protected internal override void OnMouseMove(dfMouseEventArgs args)
  {
    if ((args is dfTouchEventArgs || this.isMouseDown) && !args.Used && (double) (args.Position - this.touchStartPosition).magnitude > 5.0)
    {
      Vector2 vector2 = args.MoveDelta.Scale(-1f, 1f);
      dfGUIManager manager = this.GetManager();
      Vector2 screenSize = manager.GetScreenSize();
      Camera renderCamera = manager.RenderCamera;
      vector2.x = screenSize.x * (vector2.x / (float) renderCamera.pixelWidth);
      vector2.y = screenSize.y * (vector2.y / (float) renderCamera.pixelHeight);
      this.ScrollPosition += vector2;
      this.scrollMomentum = (this.scrollMomentum + vector2) * 0.5f;
      args.Use();
    }
    base.OnMouseMove(args);
  }

  protected internal override void OnMouseWheel(dfMouseEventArgs args)
  {
    try
    {
      if (args.Used)
        return;
      float num = this.wheelDirection != dfControlOrientation.Horizontal ? (!((UnityEngine.Object) this.vertScroll != (UnityEngine.Object) null) ? (float) this.scrollWheelAmount : this.vertScroll.IncrementAmount) : (!((UnityEngine.Object) this.horzScroll != (UnityEngine.Object) null) ? (float) this.scrollWheelAmount : this.horzScroll.IncrementAmount);
      if (this.wheelDirection == dfControlOrientation.Horizontal)
      {
        this.ScrollPosition = new Vector2(this.scrollPosition.x - num * args.WheelDelta, this.scrollPosition.y);
        this.scrollMomentum = new Vector2(-num * args.WheelDelta, 0.0f);
      }
      else
      {
        this.ScrollPosition = new Vector2(this.scrollPosition.x, this.scrollPosition.y - num * args.WheelDelta);
        this.scrollMomentum = new Vector2(0.0f, -num * args.WheelDelta);
      }
      args.Use();
      this.Signal(nameof (OnMouseWheel), (object) this, (object) args);
    }
    finally
    {
      base.OnMouseWheel(args);
    }
  }

  protected internal override void OnControlAdded(dfControl child)
  {
    base.OnControlAdded(child);
    this.attachEvents(child);
    if (!this.autoLayout)
      return;
    this.AutoArrange();
  }

  protected internal override void OnControlRemoved(dfControl child)
  {
    if (GameManager.IsShuttingDown)
      return;
    base.OnControlRemoved(child);
    if ((UnityEngine.Object) child != (UnityEngine.Object) null)
      this.detachEvents(child);
    if (this.autoLayout)
      this.AutoArrange();
    else
      this.updateScrollbars();
  }

  protected override void OnRebuildRenderData()
  {
    if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || string.IsNullOrEmpty(this.backgroundSprite))
      return;
    dfAtlas.ItemInfo atla = this.Atlas[this.backgroundSprite];
    if (atla == (dfAtlas.ItemInfo) null)
      return;
    this.renderData.Material = this.Atlas.Material;
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

  protected internal void OnScrollPositionChanged()
  {
    this.Invalidate();
    this.SignalHierarchy(nameof (OnScrollPositionChanged), (object) this, (object) this.ScrollPosition);
    if (this.ScrollPositionChanged == null)
      return;
    this.ScrollPositionChanged((dfControl) this, this.ScrollPosition);
  }

  public void FitToContents()
  {
    if (this.controls.Count == 0)
      return;
    Vector2 lhs = Vector2.zero;
    for (int index = 0; index < this.controls.Count; ++index)
    {
      dfControl control = this.controls[index];
      Vector2 rhs = (Vector2) control.RelativePosition + control.Size;
      lhs = Vector2.Max(lhs, rhs);
    }
    this.Size = lhs + new Vector2((float) this.scrollPadding.right, (float) this.scrollPadding.bottom);
  }

  public void CenterChildControls()
  {
    if (this.controls.Count == 0)
      return;
    Vector2 lhs1 = Vector2.one * float.MaxValue;
    Vector2 lhs2 = Vector2.one * float.MinValue;
    for (int index = 0; index < this.controls.Count; ++index)
    {
      dfControl control = this.controls[index];
      Vector2 relativePosition = (Vector2) control.RelativePosition;
      Vector2 rhs = relativePosition + control.Size;
      lhs1 = Vector2.Min(lhs1, relativePosition);
      lhs2 = Vector2.Max(lhs2, rhs);
    }
    Vector2 vector2 = (this.Size - (lhs2 - lhs1)) * 0.5f;
    for (int index = 0; index < this.controls.Count; ++index)
    {
      dfControl control = this.controls[index];
      control.RelativePosition = (Vector3) ((Vector2) control.RelativePosition - lhs1 + vector2);
    }
  }

  public void ScrollToTop()
  {
    this.scrollMomentum = Vector2.zero;
    this.ScrollPosition = new Vector2(this.scrollPosition.x, 0.0f);
  }

  public void ScrollToBottom()
  {
    this.scrollMomentum = Vector2.zero;
    this.ScrollPosition = new Vector2(this.scrollPosition.x, (float) int.MaxValue);
  }

  public void ScrollToLeft()
  {
    this.scrollMomentum = Vector2.zero;
    this.ScrollPosition = new Vector2(0.0f, this.scrollPosition.y);
  }

  public void ScrollToRight()
  {
    this.scrollMomentum = Vector2.zero;
    this.ScrollPosition = new Vector2((float) int.MaxValue, this.scrollPosition.y);
  }

  public void ScrollToPrimeViewingPosition(dfControl control)
  {
    this.scrollMomentum = Vector2.zero;
    Rect rect1 = new Rect(this.scrollPosition.x + (float) this.scrollPadding.left, this.scrollPosition.y + (float) this.scrollPadding.top, this.size.x - (float) this.scrollPadding.horizontal, this.size.y - (float) this.scrollPadding.vertical).RoundToInt();
    Vector3 relativePosition = control.RelativePosition;
    Vector2 size = control.Size;
    while (!this.controls.Contains(control))
    {
      control = control.Parent;
      relativePosition += control.RelativePosition;
    }
    Rect rect2 = new Rect(this.scrollPosition.x + relativePosition.x, this.scrollPosition.y + relativePosition.y, size.x, size.y).RoundToInt();
    Vector2 scrollPosition = this.scrollPosition;
    if ((double) rect2.center.y < (double) rect1.height / 2.0)
    {
      this.ScrollToTop();
    }
    else
    {
      scrollPosition.y = rect2.center.y - rect1.height;
      this.ScrollPosition = scrollPosition;
      this.scrollMomentum = Vector2.zero;
    }
  }

  public void ScrollIntoView(Vector2 positionRelativeToScrollPanelPx, Vector2 sizePx)
  {
    this.scrollMomentum = Vector2.zero;
    Rect rect = new Rect(this.scrollPosition.x + (float) this.scrollPadding.left, this.scrollPosition.y + (float) this.scrollPadding.top, this.size.x - (float) this.scrollPadding.horizontal, this.size.y - (float) this.scrollPadding.vertical).RoundToInt();
    Vector2 vector2_1 = positionRelativeToScrollPanelPx;
    Vector2 vector2_2 = sizePx;
    Rect other = new Rect(this.scrollPosition.x + vector2_1.x, this.scrollPosition.y + vector2_1.y, vector2_2.x, vector2_2.y).RoundToInt();
    if (rect.Contains(other))
      return;
    Vector2 scrollPosition = this.scrollPosition;
    if ((double) other.xMin < (double) rect.xMin)
      scrollPosition.x = other.xMin - (float) this.scrollPadding.left;
    else if ((double) other.xMax > (double) rect.xMax)
      scrollPosition.x = other.xMax - Mathf.Max(this.size.x, vector2_2.x) + (float) this.scrollPadding.horizontal;
    if ((double) other.y < (double) rect.y)
      scrollPosition.y = other.yMin - (float) this.scrollPadding.top;
    else if ((double) other.yMax > (double) rect.yMax)
      scrollPosition.y = other.yMax - Mathf.Max(this.size.y, vector2_2.y) + (float) this.scrollPadding.vertical;
    this.ScrollPosition = scrollPosition;
    this.scrollMomentum = Vector2.zero;
  }

  public void ScrollIntoView(dfControl control)
  {
    this.scrollMomentum = Vector2.zero;
    Rect rect = new Rect(this.scrollPosition.x + (float) this.scrollPadding.left, this.scrollPosition.y, this.size.x - (float) this.scrollPadding.horizontal, this.size.y).RoundToInt();
    Vector3 relativePosition = control.RelativePosition;
    Vector2 size = control.Size;
    while (!this.controls.Contains(control))
    {
      control = control.Parent;
      relativePosition += control.RelativePosition;
    }
    Rect other = new Rect(this.scrollPosition.x + relativePosition.x, this.scrollPosition.y + relativePosition.y, size.x, size.y).RoundToInt();
    if (rect.Contains(other))
      return;
    Vector2 scrollPosition = this.scrollPosition;
    if ((double) other.xMin < (double) rect.xMin)
      scrollPosition.x = other.xMin - (float) this.scrollPadding.left;
    else if ((double) other.xMax > (double) rect.xMax)
      scrollPosition.x = other.xMax - Mathf.Max(this.size.x, size.x) + (float) this.scrollPadding.horizontal;
    if ((double) other.y < (double) rect.y)
      scrollPosition.y = other.yMin - (float) this.autoScrollPadding.top;
    else if ((double) other.yMax > (double) rect.yMax)
      scrollPosition.y = other.yMax - Mathf.Max(this.size.y, size.y) + (float) this.autoScrollPadding.vertical;
    this.ScrollPosition = scrollPosition;
    this.scrollMomentum = Vector2.zero;
  }

  public void Reset()
  {
    try
    {
      this.SuspendLayout();
      if (this.autoLayout)
      {
        Vector2 scrollPosition = this.ScrollPosition;
        this.ScrollPosition = Vector2.zero;
        this.AutoArrange();
        this.ScrollPosition = scrollPosition;
      }
      else
      {
        this.scrollPadding = this.ScrollPadding.ConstrainPadding();
        Vector3 vector3 = (Vector3) this.calculateMinChildPosition() - new Vector3((float) this.scrollPadding.left, (float) this.scrollPadding.top);
        for (int index = 0; index < this.controls.Count; ++index)
          this.controls[index].RelativePosition -= vector3;
        this.scrollPosition = Vector2.zero;
      }
      this.Invalidate();
      this.updateScrollbars();
    }
    finally
    {
      this.ResumeLayout();
    }
  }

  private void Virtualize<T>(IList<T> backingList, int startIndex)
  {
    if (!this.useVirtualScrolling)
      Debug.LogError((object) ("Virtual scrolling not enabled for this dfScrollPanel: " + this.name));
    else if ((UnityEngine.Object) this.virtualScrollingTile == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("Virtual scrolling cannot be started without assigning VirtualScrollingTile: " + this.name));
    }
    else
    {
      if (backingList.Count != 0)
        ;
      dfVirtualScrollData<T> virtualScrollData = this.GetVirtualScrollData<T>() ?? this.initVirtualScrollData<T>(backingList);
      bool flag = this.isVerticalFlow();
      RectOffset padding = virtualScrollData.ItemPadding = new RectOffset(this.FlowPadding.left, this.FlowPadding.right, this.FlowPadding.top, this.FlowPadding.bottom);
      int num1 = !flag ? padding.horizontal : padding.vertical;
      int num2 = !flag ? padding.left : padding.top;
      float num3 = !flag ? this.Width : this.Height;
      this.AutoLayout = false;
      this.AutoReset = false;
      IDFVirtualScrollingTile virtualScrollingTile1 = virtualScrollData.DummyTop ?? (virtualScrollData.DummyTop = this.initTile(padding));
      dfPanel dfPanel1 = virtualScrollingTile1.GetDfPanel();
      float num4 = !flag ? virtualScrollingTile1.GetDfPanel().Width : virtualScrollingTile1.GetDfPanel().Height;
      dfPanel1.IsEnabled = false;
      dfPanel1.Opacity = 0.0f;
      dfPanel1.gameObject.hideFlags = HideFlags.HideInHierarchy;
      dfScrollbar dfScrollbar;
      if ((bool) (UnityEngine.Object) (dfScrollbar = this.VertScrollbar) || (bool) (UnityEngine.Object) (dfScrollbar = this.HorzScrollbar))
      {
        dfPanel dfPanel2 = (virtualScrollData.DummyBottom ?? (virtualScrollData.DummyBottom = this.initTile(padding))).GetDfPanel();
        float num5 = (!flag ? dfPanel1.RelativePosition.x : dfPanel1.RelativePosition.y) + ((float) (backingList.Count - 1) * (num4 + (float) num1) + (float) num2);
        dfPanel2.RelativePosition = !flag ? new Vector3(num5, dfPanel1.RelativePosition.y) : new Vector3(dfPanel1.RelativePosition.x, num5);
        dfPanel2.IsEnabled = dfPanel1.IsEnabled;
        dfPanel2.gameObject.hideFlags = dfPanel1.hideFlags;
        dfPanel2.Opacity = dfPanel1.Opacity;
        if (startIndex == 0 && (double) dfScrollbar.MaxValue != 0.0)
          startIndex = Mathf.RoundToInt(dfScrollbar.Value / dfScrollbar.MaxValue * (float) (backingList.Count - 1));
        dfScrollbar.Value = (float) startIndex * (num4 + (float) num1);
      }
      float f = num3 / (num4 + (float) num1);
      int num6 = Mathf.RoundToInt(f);
      int num7 = (double) num6 <= (double) f ? num6 + 2 : num6 + 1;
      float num8 = (float) num2;
      float num9 = (float) startIndex;
      for (int index1 = 0; index1 < num7 && index1 < backingList.Count; ++index1)
      {
        if (startIndex <= backingList.Count)
        {
          try
          {
            IDFVirtualScrollingTile virtualScrollingTile2 = !virtualScrollData.IsInitialized || virtualScrollData.Tiles.Count < index1 + 1 ? this.initTile(padding) : virtualScrollData.Tiles[index1];
            dfPanel dfPanel3 = virtualScrollingTile2.GetDfPanel();
            float num10 = num8;
            dfPanel3.RelativePosition = (Vector3) (!flag ? new Vector2(num10, (float) padding.top) : new Vector2((float) padding.left, num10));
            num8 = num10 + num4 + (float) num1;
            if (!virtualScrollData.Tiles.Contains(virtualScrollingTile2))
              virtualScrollData.Tiles.Add(virtualScrollingTile2);
            virtualScrollingTile2.VirtualScrollItemIndex = startIndex;
            virtualScrollingTile2.OnScrollPanelItemVirtualize((object) backingList[startIndex]);
            ++startIndex;
          }
          catch
          {
            foreach (IDFVirtualScrollingTile tile in virtualScrollData.Tiles)
            {
              int index2 = --tile.VirtualScrollItemIndex;
              tile.OnScrollPanelItemVirtualize((object) backingList[index2]);
            }
          }
        }
        else
          break;
      }
      if ((double) num9 != 0.0 && this.ScrollPositionChanged != null)
        this.ScrollPositionChanged -= new PropertyChangedEventHandler<Vector2>(this.virtualScrollPositionChanged<T>);
      virtualScrollData.IsInitialized = true;
      this.ScrollPositionChanged += new PropertyChangedEventHandler<Vector2>(this.virtualScrollPositionChanged<T>);
    }
  }

  public void Virtualize<T>(IList<T> backingList, dfPanel tile)
  {
    if (!(bool) (UnityEngine.Object) ((IEnumerable<MonoBehaviour>) tile.GetComponents<MonoBehaviour>()).FirstOrDefault<MonoBehaviour>((Func<MonoBehaviour, bool>) (t => t is IDFVirtualScrollingTile)))
    {
      Debug.LogError((object) "The tile you've chosen does not implement IDFVirtualScrollingTile!");
    }
    else
    {
      this.UseVirtualScrolling = true;
      this.VirtualScrollingTile = (dfControl) tile;
      this.Virtualize<T>(backingList, 0);
    }
  }

  public void Virtualize<T>(IList<T> backingList) => this.Virtualize<T>(backingList, 0);

  public void ResetVirtualScrollingData()
  {
    this.virtualScrollData = (object) null;
    foreach (dfControl child in this.controls.ToArray())
    {
      this.RemoveControl(child);
      UnityEngine.Object.Destroy((UnityEngine.Object) child.gameObject);
    }
    this.ScrollPosition = Vector2.zero;
  }

  public dfVirtualScrollData<T> GetVirtualScrollData<T>()
  {
    return (dfVirtualScrollData<T>) this.virtualScrollData;
  }

  [HideInInspector]
  private void AutoArrange()
  {
    this.SuspendLayout();
    try
    {
      this.scrollPadding = this.ScrollPadding.ConstrainPadding();
      this.flowPadding = this.FlowPadding.ConstrainPadding();
      float x = (float) this.scrollPadding.left + (float) this.flowPadding.left - this.scrollPosition.x;
      float y = (float) this.scrollPadding.top + (float) this.flowPadding.top - this.scrollPosition.y;
      float b1 = 0.0f;
      float b2 = 0.0f;
      for (int index = 0; index < this.controls.Count; ++index)
      {
        dfControl control = this.controls[index];
        if ((bool) (UnityEngine.Object) control && control.GetIsVisibleRaw() && control.enabled && control.gameObject.activeSelf && !((UnityEngine.Object) control == (UnityEngine.Object) this.horzScroll) && !((UnityEngine.Object) control == (UnityEngine.Object) this.vertScroll))
        {
          if (this.wrapLayout)
          {
            if (this.flowDirection == dfScrollPanel.LayoutDirection.Horizontal)
            {
              if ((double) x + (double) control.Width >= (double) this.size.x - (double) this.scrollPadding.right)
              {
                x = (float) this.scrollPadding.left + (float) this.flowPadding.left;
                y += b2;
                b2 = 0.0f;
              }
            }
            else if ((double) y + (double) control.Height + (double) this.flowPadding.vertical >= (double) this.size.y - (double) this.scrollPadding.bottom)
            {
              y = (float) this.scrollPadding.top + (float) this.flowPadding.top;
              x += b1;
              b1 = 0.0f;
            }
          }
          Vector2 vector2 = new Vector2(x, y);
          control.RelativePosition = (Vector3) vector2;
          float a1 = control.Width + (float) this.flowPadding.horizontal;
          float a2 = control.Height + (float) this.flowPadding.vertical;
          b1 = Mathf.Max(a1, b1);
          b2 = Mathf.Max(a2, b2);
          if (this.flowDirection == dfScrollPanel.LayoutDirection.Horizontal)
            x += a1;
          else
            y += a2;
        }
      }
      this.updateScrollbars();
    }
    finally
    {
      this.ResumeLayout();
    }
  }

  [HideInInspector]
  private void initialize()
  {
    if (this.initialized)
      return;
    this.initialized = true;
    if (Application.isPlaying)
    {
      if ((UnityEngine.Object) this.horzScroll != (UnityEngine.Object) null)
        this.horzScroll.ValueChanged += new PropertyChangedEventHandler<float>(this.horzScroll_ValueChanged);
      if ((UnityEngine.Object) this.vertScroll != (UnityEngine.Object) null)
        this.vertScroll.ValueChanged += new PropertyChangedEventHandler<float>(this.vertScroll_ValueChanged);
    }
    if (this.resetNeeded || this.autoLayout || this.autoReset)
      this.Reset();
    this.Invalidate();
    this.ScrollPosition = Vector2.zero;
    this.updateScrollbars();
  }

  private void vertScroll_ValueChanged(dfControl control, float value)
  {
    this.ScrollPosition = new Vector2(this.scrollPosition.x, value);
    this.ScrollPosition = new Vector2(this.scrollPosition.x, value);
  }

  private void horzScroll_ValueChanged(dfControl control, float value)
  {
    this.ScrollPosition = new Vector2(value, this.ScrollPosition.y);
  }

  private void scrollChildControls(Vector3 delta)
  {
    try
    {
      this.scrolling = true;
      delta = delta.Scale(1f, -1f, 1f);
      for (int index = 0; index < this.controls.Count; ++index)
      {
        dfControl control = this.controls[index];
        control.Position = (control.Position - delta).RoundToInt();
      }
    }
    finally
    {
      this.scrolling = false;
    }
  }

  private Vector2 calculateMinChildPosition()
  {
    float num1 = float.MaxValue;
    float num2 = float.MaxValue;
    for (int index = 0; index < this.controls.Count; ++index)
    {
      dfControl control = this.controls[index];
      if (control.enabled && control.gameObject.activeSelf)
      {
        Vector3 vector3 = control.RelativePosition.FloorToInt();
        num1 = Mathf.Min(num1, vector3.x);
        num2 = Mathf.Min(num2, vector3.y);
      }
    }
    return new Vector2(num1, num2);
  }

  private Vector2 calculateViewSize()
  {
    Vector2 vector2_1 = new Vector2((float) this.scrollPadding.horizontal, (float) this.scrollPadding.vertical).RoundToInt();
    Vector2 rhs1 = this.Size.RoundToInt() - vector2_1;
    if (!this.IsVisible || this.controls.Count == 0)
      return rhs1;
    Vector2 rhs2 = Vector2.one * float.MaxValue;
    Vector2 rhs3 = Vector2.one * float.MinValue;
    for (int index = 0; index < this.controls.Count; ++index)
    {
      dfControl control = this.controls[index];
      if (!Application.isPlaying || control.IsVisible)
      {
        Vector2 lhs1 = (Vector2) control.RelativePosition.CeilToInt();
        Vector2 lhs2 = lhs1 + control.Size.CeilToInt();
        rhs2 = Vector2.Min(lhs1, rhs2);
        rhs3 = Vector2.Max(lhs2, rhs3);
      }
    }
    Vector2 vector2_2 = Vector2.Max(Vector2.zero, rhs2 - new Vector2((float) this.scrollPadding.left, (float) this.scrollPadding.top));
    return Vector2.Max(rhs3 + vector2_2, rhs1) - rhs2 + vector2_2;
  }

  [HideInInspector]
  private void updateScrollbars()
  {
    Vector2 viewSize = this.calculateViewSize();
    Vector2 vector2 = this.Size - new Vector2((float) this.scrollPadding.horizontal, (float) this.scrollPadding.vertical);
    vector2.x = Mathf.Abs(vector2.x);
    vector2.y = Mathf.Abs(vector2.y);
    if ((UnityEngine.Object) this.horzScroll != (UnityEngine.Object) null)
    {
      this.horzScroll.MinValue = 0.0f;
      this.horzScroll.MaxValue = viewSize.x;
      this.horzScroll.ScrollSize = vector2.x;
      this.horzScroll.Value = Mathf.Max(0.0f, this.scrollPosition.x);
    }
    if (!((UnityEngine.Object) this.vertScroll != (UnityEngine.Object) null))
      return;
    this.vertScroll.MinValue = 0.0f;
    this.vertScroll.MaxValue = viewSize.y;
    this.vertScroll.ScrollSize = vector2.y;
    this.vertScroll.Value = Mathf.Max(0.0f, this.scrollPosition.y);
  }

  private void virtualScrollPositionChanged<T>(dfControl control, Vector2 value)
  {
    dfVirtualScrollData<T> virtualScrollData = this.GetVirtualScrollData<T>();
    if (virtualScrollData == null)
      return;
    IList<T> backingList = virtualScrollData.BackingList;
    RectOffset itemPadding = virtualScrollData.ItemPadding;
    List<IDFVirtualScrollingTile> tiles = virtualScrollData.Tiles;
    bool isVerticalFlow = this.isVerticalFlow();
    float f = !isVerticalFlow ? value.x - virtualScrollData.LastScrollPosition.x : value.y - virtualScrollData.LastScrollPosition.y;
    virtualScrollData.LastScrollPosition = value;
    if ((double) Mathf.Abs(f) > (double) this.Height && ((bool) (UnityEngine.Object) this.VertScrollbar || (bool) (UnityEngine.Object) this.HorzScrollbar))
    {
      int startIndex = Mathf.RoundToInt((!isVerticalFlow ? value.x / this.HorzScrollbar.MaxValue : value.y / this.VertScrollbar.MaxValue) * (float) (backingList.Count - 1));
      this.Virtualize<T>(backingList, startIndex);
    }
    else
    {
      foreach (IDFVirtualScrollingTile virtualScrollingTile in tiles)
      {
        int index = 0;
        float newY = 0.0f;
        bool flag = false;
        dfPanel dfPanel = virtualScrollingTile.GetDfPanel();
        float num1 = !isVerticalFlow ? dfPanel.RelativePosition.x : dfPanel.RelativePosition.y;
        float num2 = !isVerticalFlow ? dfPanel.Width : dfPanel.Height;
        float num3 = !isVerticalFlow ? this.Width : this.Height;
        if ((double) f > 0.0)
        {
          if ((double) num1 + (double) num2 <= 0.0)
          {
            virtualScrollData.GetNewLimits(isVerticalFlow, true, out index, out newY);
            if (index < backingList.Count)
            {
              flag = true;
              dfPanel.RelativePosition = !isVerticalFlow ? new Vector3(newY + num2 + (float) itemPadding.horizontal, dfPanel.RelativePosition.y) : new Vector3(dfPanel.RelativePosition.x, newY + num2 + (float) itemPadding.vertical);
            }
            else
              continue;
          }
          else
            continue;
        }
        else if ((double) f < 0.0)
        {
          if ((double) num1 >= (double) num3)
          {
            virtualScrollData.GetNewLimits(isVerticalFlow, false, out index, out newY);
            if (index >= 0)
            {
              flag = true;
              dfPanel.RelativePosition = !isVerticalFlow ? new Vector3(newY - (num2 + (float) itemPadding.horizontal), dfPanel.RelativePosition.y) : new Vector3(dfPanel.RelativePosition.x, newY - (num2 + (float) itemPadding.vertical));
            }
            else
              continue;
          }
          else
            continue;
        }
        if (flag)
        {
          virtualScrollingTile.VirtualScrollItemIndex = index;
          virtualScrollingTile.OnScrollPanelItemVirtualize((object) backingList[index]);
        }
      }
    }
  }

  private dfVirtualScrollData<T> initVirtualScrollData<T>(IList<T> backingList)
  {
    dfVirtualScrollData<T> virtualScrollData = new dfVirtualScrollData<T>()
    {
      BackingList = backingList
    };
    this.virtualScrollData = (object) virtualScrollData;
    return virtualScrollData;
  }

  private IDFVirtualScrollingTile initTile(RectOffset padding)
  {
    IDFVirtualScrollingTile virtualScrollingTile = (IDFVirtualScrollingTile) UnityEngine.Object.Instantiate<MonoBehaviour>(((IEnumerable<MonoBehaviour>) this.virtualScrollingTile.GetComponents<MonoBehaviour>()).FirstOrDefault<MonoBehaviour>((Func<MonoBehaviour, bool>) (p => p is IDFVirtualScrollingTile)));
    dfPanel dfPanel = virtualScrollingTile.GetDfPanel();
    bool flag = this.isVerticalFlow();
    this.AddControl((dfControl) dfPanel);
    if (this.AutoFitVirtualTiles)
    {
      if (flag)
        dfPanel.Width = this.Width - (float) padding.horizontal;
      else
        dfPanel.Height = this.Height - (float) padding.vertical;
    }
    dfPanel.RelativePosition = new Vector3((float) padding.left, (float) padding.top);
    return virtualScrollingTile;
  }

  private bool isVerticalFlow() => this.FlowDirection == dfScrollPanel.LayoutDirection.Vertical;

  private void attachEvents(dfControl control)
  {
    control.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.childIsVisibleChanged);
    control.PositionChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
    control.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
    control.ZOrderChanged += new PropertyChangedEventHandler<int>(this.childOrderChanged);
  }

  private void detachEvents(dfControl control)
  {
    control.IsVisibleChanged -= new PropertyChangedEventHandler<bool>(this.childIsVisibleChanged);
    control.PositionChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
    control.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
    control.ZOrderChanged -= new PropertyChangedEventHandler<int>(this.childOrderChanged);
  }

  private void childOrderChanged(dfControl control, int value)
  {
    this.onChildControlInvalidatedLayout();
  }

  private void childIsVisibleChanged(dfControl control, bool value)
  {
    this.onChildControlInvalidatedLayout();
  }

  private void childControlInvalidated(dfControl control, Vector2 value)
  {
    this.onChildControlInvalidatedLayout();
  }

  [HideInInspector]
  private void onChildControlInvalidatedLayout()
  {
    if (this.scrolling || this.IsLayoutSuspended)
      return;
    if (this.autoLayout)
      this.AutoArrange();
    this.updateScrollbars();
    this.Invalidate();
  }

  public enum LayoutDirection
  {
    Horizontal,
    Vertical,
  }
}
