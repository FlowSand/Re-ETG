// Decompiled with JetBrains decompiler
// Type: dfControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[Serializable]
  public abstract class dfControl : 
    MonoBehaviour,
    IDFControlHost,
    IComparable<dfControl>,
    IAmmonomiconFocusable
  {
[HideInInspector]
    public Action<dfControl> LanguageChanged;
    public Action<dfControl, Vector3, Vector3> ResolutionChangedPostLayout;
    private const float MINIMUM_OPACITY = 0.0125f;
    private static uint versionCounter;
[SerializeField]
    protected dfAnchorStyle anchorStyle;
[SerializeField]
    protected bool isEnabled = true;
[SerializeField]
    protected bool isVisible = true;
[SerializeField]
    protected bool isInteractive = true;
[SerializeField]
    protected string tooltip;
[SerializeField]
    protected dfPivotPoint pivot;
[SerializeField]
[HideInInspector]
    public int zindex = int.MaxValue;
[SerializeField]
    protected Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
[SerializeField]
    protected Color32 disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
[SerializeField]
    protected Vector2 size = Vector2.zero;
[SerializeField]
    protected Vector2 minSize = Vector2.zero;
[SerializeField]
    protected Vector2 maxSize = Vector2.zero;
[SerializeField]
    protected bool clipChildren;
[SerializeField]
    protected bool inverseClipChildren;
[SerializeField]
[HideInInspector]
    protected int tabIndex = -1;
[HideInInspector]
[SerializeField]
    protected bool canFocus;
[SerializeField]
    protected bool autoFocus;
[SerializeField]
[HideInInspector]
    protected dfControl.AnchorLayout layout;
[SerializeField]
[HideInInspector]
    protected int renderOrder = -1;
[SerializeField]
    protected bool isLocalized;
[SerializeField]
    protected Vector2 hotZoneScale = Vector2.one;
[SerializeField]
    protected bool allowSignalEvents = true;
    private static object[] signal1 = new object[1];
    private static object[] signal2 = new object[2];
    private static object[] signal3 = new object[3];
    protected bool isControlInvalidated = true;
    protected bool isControlClipped;
    protected dfControl parent;
    protected dfList<dfControl> controls = dfList<dfControl>.Obtain();
    protected dfGUIManager cachedManager;
    protected dfLanguageManager languageManager;
    protected bool languageManagerChecked;
    protected int cachedChildCount;
    protected Vector3 cachedPosition = Vector3.one * float.MinValue;
    protected Quaternion cachedRotation = Quaternion.identity;
    protected Vector3 cachedScale = Vector3.one;
    protected Bounds? cachedBounds;
    protected Transform cachedParentTransform;
    protected float cachedPixelSize;
    protected Vector3 cachedRelativePosition = Vector3.one * float.MinValue;
    protected uint relativePositionCacheVersion = uint.MaxValue;
    protected dfRenderData renderData;
    protected bool isMouseHovering;
    private object tag;
    protected bool isDisposing;
    private bool performingLayout;
    protected Vector3[] cachedCorners = new Vector3[4];
    protected Plane[] cachedClippingPlanes = new Plane[4];
    private bool shutdownInProgress;
    private uint version;
    protected bool isControlInitialized;
    private bool rendering;
    protected string localizationKey;
    public static readonly dfList<dfControl> ActiveInstances = new dfList<dfControl>();
[NonSerialized]
    public bool ForceSuspendLayout;
    public bool PrecludeUpdateCycle;
    private Transform m_transform;

[HideInInspector]
    public event ChildControlEventHandler ControlAdded;

[HideInInspector]
    public event ChildControlEventHandler ControlRemoved;

    public event FocusEventHandler GotFocus;

    public event FocusEventHandler EnterFocus;

    public event FocusEventHandler LostFocus;

    public event FocusEventHandler LeaveFocus;

    public event PropertyChangedEventHandler<bool> ControlShown;

    public event PropertyChangedEventHandler<bool> ControlHidden;

    public event PropertyChangedEventHandler<bool> ControlClippingChanged;

    public event PropertyChangedEventHandler<int> TabIndexChanged;

    public event PropertyChangedEventHandler<Vector2> PositionChanged;

    public event PropertyChangedEventHandler<Vector2> SizeChanged;

[HideInInspector]
    public event PropertyChangedEventHandler<Color32> ColorChanged;

    public event PropertyChangedEventHandler<bool> IsVisibleChanged;

    public event PropertyChangedEventHandler<bool> IsEnabledChanged;

[HideInInspector]
    public event PropertyChangedEventHandler<float> OpacityChanged;

[HideInInspector]
    public event PropertyChangedEventHandler<dfAnchorStyle> AnchorChanged;

[HideInInspector]
    public event PropertyChangedEventHandler<dfPivotPoint> PivotChanged;

[HideInInspector]
    public event PropertyChangedEventHandler<int> ZOrderChanged;

    public event DragEventHandler DragStart;

    public event DragEventHandler DragEnd;

    public event DragEventHandler DragDrop;

    public event DragEventHandler DragEnter;

    public event DragEventHandler DragLeave;

    public event DragEventHandler DragOver;

    public event KeyPressHandler KeyPress;

    public event KeyPressHandler KeyDown;

    public event KeyPressHandler KeyUp;

    public event ControlMultiTouchEventHandler MultiTouch;

    public event ControlCallbackHandler MultiTouchEnd;

    public event MouseEventHandler MouseEnter;

    public event MouseEventHandler MouseMove;

    public event MouseEventHandler MouseHover;

    public event MouseEventHandler MouseLeave;

    public event MouseEventHandler MouseDown;

    public event MouseEventHandler MouseUp;

    public event MouseEventHandler MouseWheel;

    public event MouseEventHandler Click;

    public event MouseEventHandler DoubleClick;

    public dfLanguageManager GetLanguageManager()
    {
      if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
      {
        if (this.languageManagerChecked)
          return (dfLanguageManager) null;
        this.languageManagerChecked = true;
        this.languageManager = this.GetManager().GetComponent<dfLanguageManager>();
        if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
          this.languageManager = GameUIRoot.Instance.GetComponent<dfLanguageManager>();
      }
      return this.languageManager;
    }

    public void ForceUpdateCachedParentTransform()
    {
      this.cachedParentTransform = this.transform.parent;
    }

    public string GetLocalizationKey() => this.localizationKey;

    public bool AllowSignalEvents
    {
      get => this.allowSignalEvents;
      set => this.allowSignalEvents = value;
    }

    internal bool IsInvalid => this.isControlInvalidated;

    internal bool IsControlClipped => this.isControlClipped;

    public dfGUIManager GUIManager => this.GetManager();

    public bool IsEnabled
    {
      get
      {
        if (!this.enabled || (UnityEngine.Object) this.gameObject != (UnityEngine.Object) null && !this.gameObject.activeSelf)
          return false;
        if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
          return this.isEnabled;
        return this.isEnabled && this.parent.IsEnabled;
      }
      set
      {
        if (value == this.isEnabled)
          return;
        this.isEnabled = value;
        this.OnIsEnabledChanged();
      }
    }

[SerializeField]
    public bool IsVisible
    {
      get
      {
        if ((UnityEngine.Object) this.parent == (UnityEngine.Object) null)
          return this.isVisible;
        return this.isVisible && this.parent.IsVisible;
      }
      set
      {
        if (value == this.isVisible)
          return;
        if (Application.isPlaying && !this.IsInteractive)
        {
          if ((bool) (UnityEngine.Object) this.GetComponent<Collider>())
            this.GetComponent<Collider>().enabled = false;
        }
        else if ((bool) (UnityEngine.Object) this.GetComponent<Collider>())
          this.GetComponent<Collider>().enabled = value;
        this.isVisible = value;
        this.OnIsVisibleChanged();
      }
    }

    public virtual bool IsInteractive
    {
      get => this.isInteractive;
      set
      {
        if (this.HasFocus && !value)
          dfGUIManager.SetFocus((dfControl) null);
        this.isInteractive = value;
      }
    }

[SerializeField]
    public string Tooltip
    {
      get => this.tooltip;
      set
      {
        if (!(value != this.tooltip))
          return;
        this.tooltip = value;
        this.Invalidate();
      }
    }

[SerializeField]
    public dfAnchorStyle Anchor
    {
      get
      {
        this.ensureLayoutExists();
        return this.anchorStyle;
      }
      set
      {
        if (value == this.anchorStyle)
          return;
        this.anchorStyle = value;
        this.OnAnchorChanged();
      }
    }

    public float Opacity
    {
      get => (float) this.color.a / (float) byte.MaxValue;
      set
      {
        value = Mathf.Max(0.0f, Mathf.Min(1f, value));
        float b = (float) this.color.a / (float) byte.MaxValue;
        if (Mathf.Approximately(value, b))
          return;
        this.color.a = (byte) ((double) value * (double) byte.MaxValue);
        this.OnOpacityChanged();
      }
    }

    public Color32 Color
    {
      get => this.color;
      set
      {
        value.a = (byte) ((double) this.Opacity * (double) byte.MaxValue);
        if (this.color.EqualsNonAlloc(value))
          return;
        this.color = value;
        this.OnColorChanged();
      }
    }

    public Color32 DisabledColor
    {
      get => this.disabledColor;
      set
      {
        if (value.Equals((object) this.disabledColor))
          return;
        this.disabledColor = value;
        this.Invalidate();
      }
    }

    public dfPivotPoint Pivot
    {
      get => this.pivot;
      set
      {
        if (value == this.pivot)
          return;
        Vector3 position = this.Position;
        this.pivot = value;
        Vector3 vector3 = this.Position - position;
        this.SuspendLayout();
        this.Position = position;
        for (int index = 0; index < this.controls.Count; ++index)
          this.controls[index].Position += vector3;
        this.ResumeLayout();
        this.OnPivotChanged();
      }
    }

    public Vector3 RelativePosition
    {
      get => this.getRelativePosition();
      set => this.setRelativePosition(ref value);
    }

    public Vector3 Position
    {
      get
      {
        return this.transform.localPosition / this.PixelsToUnits() + this.pivot.TransformToUpperLeft(this.Size);
      }
      set => this.setPositionInternal(value);
    }

    public Vector2 Size
    {
      get => this.size;
      set
      {
        value = Vector2.Max(this.CalculateMinimumSize(), value);
        value.x = (double) this.maxSize.x <= 0.0 ? value.x : Mathf.Min(value.x, this.maxSize.x);
        value.y = (double) this.maxSize.y <= 0.0 ? value.y : Mathf.Min(value.y, this.maxSize.y);
        if ((double) (value - this.size).sqrMagnitude <= 1.0)
          return;
        this.size = value;
        this.OnSizeChanged();
      }
    }

    public float Width
    {
      get => this.size.x;
      set => this.Size = new Vector2(value, this.size.y);
    }

    public float Height
    {
      get => this.size.y;
      set => this.Size = new Vector2(this.size.x, value);
    }

    public Vector2 MinimumSize
    {
      get => this.minSize;
      set
      {
        value = Vector2.Max(Vector2.zero, value.RoundToInt());
        if (!(value != this.minSize))
          return;
        this.minSize = value;
        this.Invalidate();
      }
    }

    public Vector2 MaximumSize
    {
      get => this.maxSize;
      set
      {
        value = Vector2.Max(Vector2.zero, value.RoundToInt());
        if (!(value != this.maxSize))
          return;
        this.maxSize = value;
        this.Invalidate();
      }
    }

[HideInInspector]
    public int ZOrder
    {
      get => this.zindex;
      set
      {
        if (value == this.zindex)
          return;
        if ((UnityEngine.Object) this.parent != (UnityEngine.Object) null)
          this.parent.SetControlIndex(this, value);
        else
          this.zindex = Mathf.Max(-1, value);
        this.OnZOrderChanged();
      }
    }

[HideInInspector]
    public int TabIndex
    {
      get => this.tabIndex;
      set
      {
        if (value == this.tabIndex)
          return;
        this.tabIndex = Mathf.Max(-1, value);
        this.OnTabIndexChanged();
      }
    }

    public dfList<dfControl> Controls => this.controls;

    public dfControl Parent => this.parent;

    public bool ClipChildren
    {
      get => this.clipChildren;
      set
      {
        if (value == this.clipChildren)
          return;
        this.clipChildren = value;
        this.Invalidate();
      }
    }

    public bool InverseClipChildren
    {
      get => this.inverseClipChildren;
      set
      {
        if (value == this.inverseClipChildren)
          return;
        this.inverseClipChildren = value;
        this.Invalidate();
      }
    }

    protected bool IsLayoutSuspended
    {
      get
      {
        if (this.ForceSuspendLayout || this.performingLayout)
          return true;
        return this.layout != null && this.layout.IsLayoutSuspended;
      }
    }

    protected bool IsPerformingLayout
    {
      get => this.performingLayout || this.layout != null && this.layout.IsPerformingLayout;
    }

    public object Tag
    {
      get => this.tag;
      set => this.tag = value;
    }

    internal uint Version => this.version;

    public bool IsLocalized
    {
      get => this.isLocalized;
      set
      {
        this.isLocalized = value;
        if (!value)
          return;
        this.Localize();
      }
    }

    public Vector2 HotZoneScale
    {
      get => this.hotZoneScale;
      set
      {
        this.hotZoneScale = Vector2.Max(value, Vector2.zero);
        this.Invalidate();
      }
    }

    public bool AutoFocus
    {
      get => this.autoFocus;
      set
      {
        if (value == this.autoFocus)
          return;
        this.autoFocus = value;
        if (!value || !this.IsEnabled || !this.CanFocus)
          return;
        this.Focus(true);
      }
    }

    public virtual bool CanFocus
    {
      get => this.canFocus && this.IsInteractive;
      set => this.canFocus = value;
    }

    public virtual bool ContainsFocus => dfGUIManager.ContainsFocus(this);

    public virtual bool HasFocus => dfGUIManager.HasFocus(this);

    public bool ContainsMouse => this.isMouseHovering;

    internal void setRenderOrder(ref int order)
    {
      this.renderOrder = ++order;
      int count = this.controls.Count;
      dfControl[] items = this.controls.Items;
      for (int index = 0; index < count; ++index)
      {
        if ((UnityEngine.Object) items[index] != (UnityEngine.Object) null)
          items[index].setRenderOrder(ref order);
      }
    }

[HideInInspector]
    public int RenderOrder => this.renderOrder;

    internal virtual void OnDragStart(dfDragEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnDragStart), (object) this, (object) args);
        if (!args.Used && this.DragStart != null)
          this.DragStart(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnDragStart(args);
    }

    internal virtual void OnDragEnd(dfDragEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnDragEnd), (object) this, (object) args);
        if (!args.Used && this.DragEnd != null)
          this.DragEnd(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnDragEnd(args);
    }

    internal virtual void OnDragDrop(dfDragEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnDragDrop), (object) this, (object) args);
        if (!args.Used && this.DragDrop != null)
          this.DragDrop(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnDragDrop(args);
    }

    internal virtual void OnDragEnter(dfDragEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnDragEnter), (object) this, (object) args);
        if (!args.Used && this.DragEnter != null)
          this.DragEnter(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnDragEnter(args);
    }

    internal virtual void OnDragLeave(dfDragEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnDragLeave), (object) this, (object) args);
        if (!args.Used && this.DragLeave != null)
          this.DragLeave(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnDragLeave(args);
    }

    internal virtual void OnDragOver(dfDragEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnDragOver), (object) this, (object) args);
        if (!args.Used && this.DragOver != null)
          this.DragOver(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnDragOver(args);
    }

    protected internal virtual void OnMultiTouchEnd()
    {
      this.Signal(nameof (OnMultiTouchEnd), (object) this);
      if (this.MultiTouchEnd != null)
        this.MultiTouchEnd(this);
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMultiTouchEnd();
    }

    protected internal virtual void OnMultiTouch(dfTouchEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnMultiTouch), (object) this, (object) args);
        if (this.MultiTouch != null)
          this.MultiTouch(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMultiTouch(args);
    }

    protected internal virtual void OnMouseEnter(dfMouseEventArgs args)
    {
      this.isMouseHovering = true;
      if (!args.Used)
      {
        this.Signal(nameof (OnMouseEnter), (object) this, (object) args);
        if (this.MouseEnter != null)
          this.MouseEnter(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMouseEnter(args);
    }

    protected internal virtual void OnMouseLeave(dfMouseEventArgs args)
    {
      this.isMouseHovering = false;
      if (!args.Used)
      {
        this.Signal(nameof (OnMouseLeave), (object) this, (object) args);
        if (this.MouseLeave != null)
          this.MouseLeave(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMouseLeave(args);
    }

    protected internal virtual void OnMouseMove(dfMouseEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnMouseMove), (object) this, (object) args);
        if (this.MouseMove != null)
          this.MouseMove(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMouseMove(args);
    }

    protected internal virtual void OnMouseHover(dfMouseEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnMouseHover), (object) this, (object) args);
        if (this.MouseHover != null)
          this.MouseHover(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMouseHover(args);
    }

    protected internal virtual void OnMouseWheel(dfMouseEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnMouseWheel), (object) this, (object) args);
        if (this.MouseWheel != null)
          this.MouseWheel(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMouseWheel(args);
    }

    protected internal virtual void OnMouseDown(dfMouseEventArgs args)
    {
      if (this.IsInteractive && this.IsEnabled && this.IsVisible && this.CanFocus && !this.ContainsFocus)
        this.Focus(true);
      if (!args.Used)
      {
        this.Signal(nameof (OnMouseDown), (object) this, (object) args);
        if (this.MouseDown != null)
          this.MouseDown(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMouseDown(args);
    }

    protected internal virtual void OnMouseUp(dfMouseEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnMouseUp), (object) this, (object) args);
        if (this.MouseUp != null)
          this.MouseUp(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnMouseUp(args);
    }

    protected internal virtual void OnClick(dfMouseEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnClick), (object) this, (object) args);
        if (this.Click != null)
          this.Click(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnClick(args);
    }

    protected internal virtual void OnDoubleClick(dfMouseEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnDoubleClick), (object) this, (object) args);
        if (this.DoubleClick != null)
          this.DoubleClick(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnDoubleClick(args);
    }

    protected internal virtual void OnKeyPress(dfKeyEventArgs args)
    {
      if (this.IsInteractive && !args.Used)
      {
        this.Signal(nameof (OnKeyPress), (object) this, (object) args);
        if (this.KeyPress != null)
          this.KeyPress(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnKeyPress(args);
    }

    protected internal virtual void OnKeyDown(dfKeyEventArgs args)
    {
      if (this.IsInteractive && !args.Used)
      {
        if (args.KeyCode == KeyCode.Tab)
          this.OnTabKeyPressed(args);
        if (!args.Used)
        {
          this.Signal(nameof (OnKeyDown), (object) this, (object) args);
          if (this.KeyDown != null)
            this.KeyDown(this, args);
        }
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnKeyDown(args);
    }

    protected virtual void OnTabKeyPressed(dfKeyEventArgs args)
    {
      List<dfControl> list = ((IEnumerable<dfControl>) this.GetManager().GetComponentsInChildren<dfControl>()).Where<dfControl>((Func<dfControl, bool>) (c => (UnityEngine.Object) c != (UnityEngine.Object) this && c.TabIndex >= 0 && c.IsInteractive && c.CanFocus && c.IsVisible)).ToList<dfControl>();
      if (list.Count == 0)
        return;
      list.Sort((Comparison<dfControl>) ((lhs, rhs) => lhs.TabIndex == rhs.TabIndex ? lhs.RenderOrder.CompareTo(rhs.RenderOrder) : lhs.TabIndex.CompareTo(rhs.TabIndex)));
      if (!args.Shift)
      {
        for (int index = 0; index < list.Count; ++index)
        {
          if (list[index].TabIndex >= this.TabIndex)
          {
            list[index].Focus(true);
            args.Use();
            return;
          }
        }
        list[0].Focus(true);
        args.Use();
      }
      else
      {
        for (int index = list.Count - 1; index >= 0; --index)
        {
          if (list[index].TabIndex <= this.TabIndex)
          {
            list[index].Focus(true);
            args.Use();
            return;
          }
        }
        list[list.Count - 1].Focus(true);
        args.Use();
      }
    }

    protected internal virtual void OnKeyUp(dfKeyEventArgs args)
    {
      if (this.IsInteractive)
      {
        this.Signal(nameof (OnKeyUp), (object) this, (object) args);
        if (this.KeyUp != null)
          this.KeyUp(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnKeyUp(args);
    }

    protected internal virtual void OnEnterFocus(dfFocusEventArgs args)
    {
      this.Signal(nameof (OnEnterFocus), (object) this, (object) args);
      if (this.EnterFocus == null)
        return;
      this.EnterFocus(this, args);
    }

    protected internal virtual void OnLeaveFocus(dfFocusEventArgs args)
    {
      this.Signal(nameof (OnLeaveFocus), (object) this, (object) args);
      if (this.LeaveFocus == null)
        return;
      this.LeaveFocus(this, args);
    }

    protected internal virtual void OnGotFocus(dfFocusEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnGotFocus), (object) this, (object) args);
        if (this.GotFocus != null)
          this.GotFocus(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnGotFocus(args);
    }

    protected internal virtual void OnLostFocus(dfFocusEventArgs args)
    {
      if (!args.Used)
      {
        this.Signal(nameof (OnLostFocus), (object) this, (object) args);
        if (this.LostFocus != null)
          this.LostFocus(this, args);
      }
      if (!((UnityEngine.Object) this.parent != (UnityEngine.Object) null))
        return;
      this.parent.OnLostFocus(args);
    }

    protected internal bool Signal(string eventName, object arg)
    {
      dfControl.signal1[0] = arg;
      return this.Signal(this.gameObject, eventName, dfControl.signal1);
    }

    protected internal bool Signal(string eventName, object arg1, object arg2)
    {
      dfControl.signal2[0] = arg1;
      dfControl.signal2[1] = arg2;
      return this.Signal(this.gameObject, eventName, dfControl.signal2);
    }

    protected internal bool Signal(string eventName, object arg1, object arg2, object arg3)
    {
      dfControl.signal3[0] = arg1;
      dfControl.signal3[1] = arg2;
      dfControl.signal3[2] = arg3;
      return this.Signal(this.gameObject, eventName, dfControl.signal3);
    }

    protected internal bool Signal(string eventName, object[] args)
    {
      return this.Signal(this.gameObject, eventName, args);
    }

    protected internal bool SignalHierarchy(string eventName, params object[] args)
    {
      if (!this.allowSignalEvents)
        return false;
      bool flag = false;
      for (Transform transform = this.transform; !flag && (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
        flag = this.Signal(transform.gameObject, eventName, args);
      return flag;
    }

[HideInInspector]
    protected internal bool Signal(GameObject target, string eventName, object arg)
    {
      dfControl.signal1[0] = arg;
      return this.Signal(target, eventName, dfControl.signal1);
    }

[HideInInspector]
    protected internal bool Signal(GameObject target, string eventName, object[] args)
    {
      if (!this.allowSignalEvents || (UnityEngine.Object) target == (UnityEngine.Object) null || this.shutdownInProgress || !Application.isPlaying)
        return false;
      MonoBehaviour[] components = target.GetComponents<MonoBehaviour>();
      if (components == null || (UnityEngine.Object) target == (UnityEngine.Object) this.gameObject && components.Length == 1)
        return false;
      if (args.Length == 0 || !object.ReferenceEquals(args[0], (object) this))
      {
        object[] destinationArray = new object[args.Length + 1];
        Array.Copy((Array) args, 0, (Array) destinationArray, 1, args.Length);
        destinationArray[0] = (object) this;
        args = destinationArray;
      }
      bool flag = false;
      for (int index = 0; index < components.Length; ++index)
      {
        MonoBehaviour target1 = components[index];
        if (!((UnityEngine.Object) target1 == (UnityEngine.Object) null) && target1.GetType() != null && !((UnityEngine.Object) target1 == (UnityEngine.Object) this) && (target1 == null || target1.enabled))
        {
          object returnValue = (object) null;
          if (dfControl.SignalCache.Invoke((Component) target1, eventName, args, out returnValue))
          {
            flag = true;
            if (returnValue is IEnumerator && target1 != null)
              target1.StartCoroutine((IEnumerator) returnValue);
          }
        }
      }
      return flag;
    }

    internal bool IsTopLevelControl(dfGUIManager manager)
    {
      return (UnityEngine.Object) this.parent == (UnityEngine.Object) null && (UnityEngine.Object) this.cachedManager == (UnityEngine.Object) manager;
    }

    internal bool GetIsVisibleRaw() => this.isVisible;

    public void Localize()
    {
      if (!this.IsLocalized)
        return;
      if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
      {
        this.languageManager = this.GetManager().GetComponent<dfLanguageManager>();
        if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
          this.languageManager = GameUIRoot.Instance.GetComponent<dfLanguageManager>();
        if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
          return;
      }
      this.OnLocalize();
    }

    public void DoClick()
    {
      Camera camera = this.GetCamera();
      Vector3 screenPoint = camera.WorldToScreenPoint(this.GetCenter());
      this.OnClick(new dfMouseEventArgs(this, dfMouseButtons.Left, 1, camera.ScreenPointToRay(screenPoint), (Vector2) screenPoint, 0.0f));
    }

[HideInInspector]
    protected internal void RemoveEventHandlers(string eventName)
    {
      ((IEnumerable<FieldInfo>) this.GetType().GetAllFields()).FirstOrDefault<FieldInfo>((Func<FieldInfo, bool>) (f => typeof (Delegate).IsAssignableFrom(f.FieldType) && f.Name == eventName))?.SetValue((object) this, (object) null);
    }

[HideInInspector]
    internal void RemoveAllEventHandlers()
    {
      foreach (FieldInfo fieldInfo in ((IEnumerable<FieldInfo>) this.GetType().GetAllFields()).Where<FieldInfo>((Func<FieldInfo, bool>) (f => typeof (Delegate).IsAssignableFrom(f.FieldType))).ToArray<FieldInfo>())
        fieldInfo.SetValue((object) this, (object) null);
    }

    public void Show() => this.IsVisible = true;

    public void Hide() => this.IsVisible = false;

    public void Enable() => this.IsEnabled = true;

    public void Disable() => this.IsEnabled = false;

    public bool HitTest(Ray ray)
    {
      Plane plane = new Plane(this.transform.TransformDirection(Vector3.back), this.transform.position);
      float enter = 0.0f;
      if (!plane.Raycast(ray, out enter))
        return false;
      Vector3 point = ray.origin + ray.direction * enter;
      Plane[] clippingPlanes = !this.ClipChildren ? (Plane[]) null : this.GetClippingPlanes();
      if (clippingPlanes != null && clippingPlanes.Length > 0)
      {
        for (int index = 0; index < clippingPlanes.Length; ++index)
        {
          if (!clippingPlanes[index].GetSide(point))
            return false;
        }
      }
      return true;
    }

    public Vector2 GetHitPosition(Ray ray)
    {
      Vector2 position;
      return !this.GetHitPosition(ray, out position, false) ? Vector2.one * float.MinValue : position;
    }

    public bool GetHitPosition(Ray ray, out Vector2 position)
    {
      return this.GetHitPosition(ray, out position, true);
    }

    public bool GetHitPosition(Ray ray, out Vector2 position, bool clamp)
    {
      position = Vector2.one * float.MinValue;
      Plane plane = new Plane(this.transform.TransformDirection(Vector3.back), this.transform.position);
      float enter = 0.0f;
      if (!plane.Raycast(ray, out enter))
        return false;
      Vector3 point = ray.GetPoint(enter);
      Plane[] clippingPlanes = !this.ClipChildren ? (Plane[]) null : this.GetClippingPlanes();
      if (clippingPlanes != null && clippingPlanes.Length > 0)
      {
        for (int index = 0; index < clippingPlanes.Length; ++index)
        {
          if (!clippingPlanes[index].GetSide(point))
            return false;
        }
      }
      Vector3[] corners = this.GetCorners();
      Vector3 start = corners[0];
      Vector3 end1 = corners[1];
      Vector3 end2 = corners[2];
      float x = this.size.x * ((dfControl.closestPointOnLine(start, end1, point, clamp) - start).magnitude / (end1 - start).magnitude);
      float y = this.size.y * ((dfControl.closestPointOnLine(start, end2, point, clamp) - start).magnitude / (end2 - start).magnitude);
      position = new Vector2(x, y);
      return true;
    }

    public T Find<T>(string controlName) where T : dfControl
    {
      if (this.name == controlName && this is T)
        return (T) this;
      this.updateControlHierarchy(true);
      for (int index = 0; index < this.controls.Count; ++index)
      {
        T control = this.controls[index] as T;
        if ((UnityEngine.Object) control != (UnityEngine.Object) null && control.name == controlName)
          return control;
      }
      for (int index = 0; index < this.controls.Count; ++index)
      {
        T obj = this.controls[index].Find<T>(controlName);
        if ((UnityEngine.Object) obj != (UnityEngine.Object) null)
          return obj;
      }
      return (T) null;
    }

    public dfControl Find(string controlName)
    {
      if (this.name == controlName)
        return this;
      this.updateControlHierarchy(true);
      for (int index = 0; index < this.controls.Count; ++index)
      {
        dfControl control = this.controls[index];
        if (control.name == controlName)
          return control;
      }
      for (int index = 0; index < this.controls.Count; ++index)
      {
        dfControl dfControl = this.controls[index].Find(controlName);
        if ((UnityEngine.Object) dfControl != (UnityEngine.Object) null)
          return dfControl;
      }
      return (dfControl) null;
    }

    public void Focus(bool allowScrolling = true)
    {
      if (!this.CanFocus || this.HasFocus || !this.IsEnabled || !this.IsVisible)
        return;
      dfGUIManager.SetFocus(this, allowScrolling);
      this.Invalidate();
    }

    public void Unfocus()
    {
      if (!this.ContainsFocus)
        return;
      dfGUIManager.SetFocus((dfControl) null);
    }

    public dfControl GetRootContainer()
    {
      dfControl rootContainer = this;
      while ((UnityEngine.Object) rootContainer.Parent != (UnityEngine.Object) null)
        rootContainer = rootContainer.Parent;
      return rootContainer;
    }

    public virtual void BringToFront()
    {
      if ((UnityEngine.Object) this.parent == (UnityEngine.Object) null)
        this.GetManager().BringToFront(this);
      else
        this.parent.SetControlIndex(this, int.MaxValue);
      this.Invalidate();
    }

    public virtual void SendToBack()
    {
      if ((UnityEngine.Object) this.parent == (UnityEngine.Object) null)
        this.GetManager().SendToBack(this);
      else
        this.parent.SetControlIndex(this, 0);
      this.Invalidate();
    }

    internal dfRenderData Render()
    {
      if (this.rendering)
        return this.renderData;
      try
      {
        this.rendering = true;
        bool isVisible = this.isVisible;
        bool flag = this.enabled && this.gameObject.activeSelf;
        if (!isVisible || !flag)
          return (dfRenderData) null;
        if (this.renderData == null)
        {
          this.renderData = dfRenderData.Obtain();
          this.isControlInvalidated = true;
        }
        if (this.isControlInvalidated)
        {
          this.renderData.Clear();
          this.OnRebuildRenderData();
          this.updateCollider();
        }
        this.renderData.Transform = this.transform.localToWorldMatrix;
        return this.renderData;
      }
      finally
      {
        this.isControlInvalidated = false;
        this.rendering = false;
      }
    }

[HideInInspector]
    public virtual void Invalidate()
    {
      if (this.shutdownInProgress)
        return;
      this.updateVersion();
      this.isControlInvalidated = true;
      this.cachedBounds = new Bounds?();
      dfGUIManager manager = this.GetManager();
      if ((UnityEngine.Object) manager != (UnityEngine.Object) null)
        manager.Invalidate();
      dfRenderGroup.InvalidateGroupForControl(this);
    }

[HideInInspector]
    public void ResetLayout() => this.ResetLayout(false, false);

[HideInInspector]
    public void ResetLayout(bool recursive, bool force)
    {
      if (this.shutdownInProgress)
        return;
      bool flag = this.IsPerformingLayout || this.IsLayoutSuspended;
      if (!force && flag)
        return;
      if (this.layout == null)
      {
        this.layout = new dfControl.AnchorLayout(this.anchorStyle, this);
      }
      else
      {
        this.layout.Attach(this);
        this.layout.Reset(true);
      }
      if (!recursive)
        return;
      int count = this.controls.Count;
      dfControl[] items = this.controls.Items;
      for (int index = 0; index < count; ++index)
        items[index].ResetLayout();
    }

[HideInInspector]
    public void PerformLayout()
    {
      if (this.shutdownInProgress || this.isDisposing)
        return;
      if (this.performingLayout)
        return;
      try
      {
        this.performingLayout = true;
        this.ensureLayoutExists();
        this.layout.PerformLayout();
        if ((UnityEngine.Object) GameUIRoot.Instance != (UnityEngine.Object) null && (UnityEngine.Object) this.GUIManager == (UnityEngine.Object) GameUIRoot.Instance.Manager)
        {
          this.updateVersion();
          this.RelativePosition = this.RelativePosition.Quantize(3f);
        }
        this.Invalidate();
      }
      finally
      {
        this.performingLayout = false;
      }
    }

[HideInInspector]
    public void SuspendLayout()
    {
      this.ensureLayoutExists();
      this.layout.SuspendLayout();
      for (int index = 0; index < this.controls.Count; ++index)
        this.controls[index].SuspendLayout();
    }

[HideInInspector]
    public void ResumeLayout()
    {
      this.ensureLayoutExists();
      this.layout.ResumeLayout();
      for (int index = 0; index < this.controls.Count; ++index)
        this.controls[index].ResumeLayout();
    }

    public virtual Vector2 CalculateMinimumSize() => this.MinimumSize;

[HideInInspector]
    public void MakePixelPerfect() => this.MakePixelPerfect(true);

[HideInInspector]
    public void MakePixelPerfect(bool recursive)
    {
    }

    public Bounds GetBounds()
    {
      if (this.isInteractive && (UnityEngine.Object) this.GetComponent<Collider>() != (UnityEngine.Object) null)
      {
        this.cachedBounds = new Bounds?(this.GetComponent<Collider>().bounds);
        return this.cachedBounds.Value;
      }
      if (this.cachedBounds.HasValue)
        return this.cachedBounds.Value;
      Vector3[] corners = this.GetCorners();
      Vector3 center = corners[0] + (corners[3] - corners[0]) * 0.5f;
      Vector3 lhs1 = center;
      Vector3 lhs2 = center;
      for (int index = 0; index < corners.Length; ++index)
      {
        lhs1 = Vector3.Min(lhs1, corners[index]);
        lhs2 = Vector3.Max(lhs2, corners[index]);
      }
      this.cachedBounds = new Bounds?(new Bounds(center, lhs2 - lhs1));
      return this.cachedBounds.Value;
    }

    public Vector3 GetCenter()
    {
      Vector3[] corners = this.GetCorners();
      return corners[0] + (corners[3] - corners[0]) * 0.5f;
    }

    public Vector3 GetAbsolutePosition()
    {
      Vector3 zero = Vector3.zero;
      for (dfControl dfControl = this; (UnityEngine.Object) dfControl != (UnityEngine.Object) null; dfControl = dfControl.Parent)
        zero += dfControl.getRelativePosition();
      return zero;
    }

    public Vector3[] GetCorners()
    {
      float units = this.PixelsToUnits();
      Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
      Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.size);
      Vector3 vector3_1 = upperLeft + new Vector3(this.size.x, 0.0f);
      Vector3 vector3_2 = upperLeft + new Vector3(0.0f, -this.size.y);
      Vector3 vector3_3 = vector3_1 + new Vector3(0.0f, -this.size.y);
      if (this.cachedCorners == null)
        this.cachedCorners = new Vector3[4];
      this.cachedCorners[0] = localToWorldMatrix.MultiplyPoint(upperLeft * units);
      this.cachedCorners[1] = localToWorldMatrix.MultiplyPoint(vector3_1 * units);
      this.cachedCorners[2] = localToWorldMatrix.MultiplyPoint(vector3_2 * units);
      this.cachedCorners[3] = localToWorldMatrix.MultiplyPoint(vector3_3 * units);
      return this.cachedCorners;
    }

    public Camera GetCamera()
    {
      dfGUIManager manager = this.GetManager();
      if (!((UnityEngine.Object) manager == (UnityEngine.Object) null))
        return manager.RenderCamera;
      UnityEngine.Debug.LogError((object) "The Manager hosting this control could not be determined");
      return (Camera) null;
    }

    protected internal virtual RectOffset GetClipPadding() => dfRectOffsetExtensions.Empty;

    public Rect GetScreenRect()
    {
      Camera camera = this.GetCamera();
      Vector3[] corners = this.GetCorners();
      Vector2 lhs1 = Vector2.one * float.MaxValue;
      Vector2 lhs2 = Vector2.one * float.MinValue;
      int length = corners.Length;
      for (int index = 0; index < length; ++index)
      {
        Vector3 screenPoint = camera.WorldToScreenPoint(corners[index]);
        lhs1 = Vector2.Min(lhs1, (Vector2) screenPoint);
        lhs2 = Vector2.Max(lhs2, (Vector2) screenPoint);
      }
      return new Rect(lhs1.x, (float) Screen.height - lhs2.y, lhs2.x - lhs1.x, lhs2.y - lhs1.y);
    }

    public dfGUIManager GetManager()
    {
      if ((UnityEngine.Object) this.cachedManager != (UnityEngine.Object) null || !this.gameObject.activeInHierarchy)
        return this.cachedManager;
      if ((UnityEngine.Object) this.parent != (UnityEngine.Object) null && (UnityEngine.Object) this.parent.cachedManager != (UnityEngine.Object) null)
        return this.cachedManager = this.parent.cachedManager;
      for (GameObject gameObject = this.gameObject; (UnityEngine.Object) gameObject != (UnityEngine.Object) null; gameObject = gameObject.transform.parent.gameObject)
      {
        dfGUIManager component = gameObject.GetComponent<dfGUIManager>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          return this.cachedManager = component;
        if ((UnityEngine.Object) gameObject.transform.parent == (UnityEngine.Object) null)
          break;
      }
      return dfGUIManager.ActiveManagers.FirstOrDefault<dfGUIManager>();
    }

    public float PixelsToUnits()
    {
      if ((double) this.cachedPixelSize > 1.4012984643248171E-45)
        return this.cachedPixelSize;
      dfGUIManager manager = this.GetManager();
      return (UnityEngine.Object) manager == (UnityEngine.Object) null ? 0.0026f : (this.cachedPixelSize = manager.PixelsToUnits());
    }

    protected internal virtual Plane[] GetClippingPlanes()
    {
      Vector3[] corners = this.GetCorners();
      Vector3 inNormal1 = this.transform.TransformDirection(Vector3.right);
      Vector3 inNormal2 = this.transform.TransformDirection(Vector3.left);
      Vector3 inNormal3 = this.transform.TransformDirection(Vector3.up);
      Vector3 inNormal4 = this.transform.TransformDirection(Vector3.down);
      this.cachedClippingPlanes[0] = new Plane(inNormal1, corners[0]);
      this.cachedClippingPlanes[1] = new Plane(inNormal2, corners[1]);
      this.cachedClippingPlanes[2] = new Plane(inNormal3, corners[2]);
      this.cachedClippingPlanes[3] = new Plane(inNormal4, corners[0]);
      return this.cachedClippingPlanes;
    }

    public bool Contains(dfControl child)
    {
      return (UnityEngine.Object) child != (UnityEngine.Object) null && child.transform.IsChildOf(this.transform);
    }

[HideInInspector]
    protected internal virtual void OnLocalize() => this.PerformLayout();

    public string ForceGetLocalizedValue(string key)
    {
      return GameUIRoot.Instance.GetComponent<dfLanguageManager>().GetValue(key);
    }

[HideInInspector]
    protected internal string getLocalizedValue(string key)
    {
      if (!this.IsLocalized || !Application.isPlaying)
        return key;
      if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
      {
        if (this.languageManagerChecked)
          return key;
        this.languageManagerChecked = true;
        this.languageManager = this.GetManager().GetComponent<dfLanguageManager>();
        if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
          this.languageManager = GameUIRoot.Instance.GetComponent<dfLanguageManager>();
        if ((UnityEngine.Object) this.languageManager == (UnityEngine.Object) null)
          return key;
      }
      return this.languageManager.GetValue(key).Replace("\\n", "\n");
    }

[HideInInspector]
    protected internal void updateCollider()
    {
      BoxCollider boxCollider = this.GetComponent<Collider>() as BoxCollider;
      if ((UnityEngine.Object) boxCollider == (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.GetComponent<Collider>() != (UnityEngine.Object) null)
          throw new Exception("Invalid collider type on control: " + this.GetComponent<Collider>().GetType().Name);
        boxCollider = this.gameObject.AddComponent<BoxCollider>();
      }
      if (Application.isPlaying && !this.isInteractive)
      {
        boxCollider.enabled = false;
      }
      else
      {
        Vector2 size = this.size * this.PixelsToUnits();
        Vector3 center = this.pivot.TransformToCenter(size);
        Vector3 vector3 = new Vector3(size.x * this.hotZoneScale.x, size.y * this.hotZoneScale.y, 1f / 1000f);
        bool flag = this.enabled && this.IsVisible;
        boxCollider.isTrigger = false;
        boxCollider.enabled = flag;
        boxCollider.size = vector3;
        boxCollider.center = center;
      }
    }

[HideInInspector]
    protected virtual void OnRebuildRenderData()
    {
    }

[HideInInspector]
    protected internal virtual void OnControlAdded(dfControl child)
    {
      this.Invalidate();
      if (this.ControlAdded != null)
        this.ControlAdded(this, child);
      this.Signal(nameof (OnControlAdded), (object) this, (object) child);
    }

[HideInInspector]
    protected internal virtual void OnControlRemoved(dfControl child)
    {
      this.Invalidate();
      if (this.ControlRemoved != null)
        this.ControlRemoved(this, child);
      this.Signal(nameof (OnControlRemoved), (object) this, (object) child);
    }

[HideInInspector]
    protected internal virtual void OnPositionChanged()
    {
      this.updateVersion();
      this.GetManager().Invalidate();
      dfRenderGroup.InvalidateGroupForControl(this);
      this.cachedPosition = this.transform.localPosition;
      if (this.isControlInitialized && Application.isPlaying && (UnityEngine.Object) this.GetComponent<Rigidbody>() == (UnityEngine.Object) null)
      {
        Rigidbody rigidbody = this.gameObject.AddComponent<Rigidbody>();
        rigidbody.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
        rigidbody.isKinematic = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        rigidbody.detectCollisions = false;
      }
      this.ResetLayout();
      if (this.PositionChanged == null)
        return;
      this.PositionChanged(this, (Vector2) this.Position);
    }

[HideInInspector]
    protected internal virtual void OnSizeChanged()
    {
      this.updateCollider();
      this.Invalidate();
      this.ResetLayout();
      if (this.Anchor.IsAnyFlagSet(dfAnchorStyle.CenterHorizontal | dfAnchorStyle.CenterVertical))
        this.PerformLayout();
      this.raiseSizeChangedEvent();
      for (int index = 0; index < this.controls.Count; ++index)
        this.controls[index].PerformLayout();
    }

[HideInInspector]
    protected internal virtual void OnPivotChanged()
    {
      this.Invalidate();
      if (this.Anchor.IsAnyFlagSet(dfAnchorStyle.CenterHorizontal | dfAnchorStyle.CenterVertical))
      {
        this.ResetLayout();
        this.PerformLayout();
      }
      if (this.PivotChanged == null)
        return;
      this.PivotChanged(this, this.pivot);
    }

[HideInInspector]
    protected internal virtual void OnAnchorChanged()
    {
      this.ResetLayout();
      if (this.anchorStyle.IsAnyFlagSet(dfAnchorStyle.CenterHorizontal | dfAnchorStyle.CenterVertical))
        this.PerformLayout();
      if (this.AnchorChanged != null)
        this.AnchorChanged(this, this.anchorStyle);
      this.Invalidate();
    }

[HideInInspector]
    protected internal virtual void OnOpacityChanged()
    {
      this.Invalidate();
      float opacity = this.Opacity;
      if (this.OpacityChanged != null)
        this.OpacityChanged(this, opacity);
      for (int index = 0; index < this.controls.Count; ++index)
        this.controls[index].OnOpacityChanged();
    }

[HideInInspector]
    protected internal virtual void OnColorChanged()
    {
      this.Invalidate();
      Color32 color = this.Color;
      if (this.ColorChanged != null)
        this.ColorChanged(this, color);
      for (int index = 0; index < this.controls.Count; ++index)
        this.controls[index].OnColorChanged();
    }

[HideInInspector]
    protected internal virtual void OnZOrderChanged()
    {
      if (this.ZOrderChanged != null)
        this.ZOrderChanged(this, this.zindex);
      this.Invalidate();
    }

[HideInInspector]
    protected internal virtual void OnTabIndexChanged()
    {
      this.Invalidate();
      if (this.TabIndexChanged == null)
        return;
      this.TabIndexChanged(this, this.tabIndex);
    }

[HideInInspector]
    protected virtual void OnControlClippingChanged()
    {
      if (this.ControlClippingChanged != null)
        this.ControlClippingChanged(this, this.isControlClipped);
      this.Signal(nameof (OnControlClippingChanged), (object) this, (object) this.isControlClipped);
    }

[HideInInspector]
    protected internal virtual void OnIsVisibleChanged()
    {
      this.updateCollider();
      bool isVisible = this.IsVisible;
      if (this.HasFocus && !isVisible)
        dfGUIManager.SetFocus((dfControl) null);
      this.Signal(nameof (OnIsVisibleChanged), (object) this, (object) isVisible);
      if (this.IsVisibleChanged != null)
        this.IsVisibleChanged(this, isVisible);
      dfControl[] items = this.controls.Items;
      int count = this.controls.Count;
      for (int index = 0; index < count; ++index)
        items[index].OnIsVisibleChanged();
      if (isVisible)
      {
        if (this.ControlShown != null)
          this.ControlShown(this, true);
        this.Signal("OnControlShown", (object) this, (object) true);
      }
      else
      {
        if (this.ControlHidden != null)
          this.ControlHidden(this, true);
        this.Signal("OnControlHidden", (object) this, (object) false);
      }
      this.Invalidate();
      if (isVisible)
      {
        this.doAutoFocus();
      }
      else
      {
        if (Application.isPlaying)
          return;
        (this.GetComponent<Collider>() as BoxCollider).size = Vector3.zero;
      }
    }

[HideInInspector]
    protected internal virtual void OnIsEnabledChanged()
    {
      if (this.shutdownInProgress)
        return;
      bool flag = this.IsEnabled && this.enabled;
      this.updateCollider();
      if (dfGUIManager.ContainsFocus(this) && !flag)
        dfGUIManager.SetFocus((dfControl) null);
      this.Invalidate();
      this.Signal(nameof (OnIsEnabledChanged), (object) this, (object) flag);
      if (this.IsEnabledChanged != null)
        this.IsEnabledChanged(this, flag);
      for (int index = 0; index < this.controls.Count; ++index)
        this.controls[index].OnIsEnabledChanged();
      this.doAutoFocus();
    }

    protected internal float CalculateOpacity()
    {
      return (UnityEngine.Object) this.parent == (UnityEngine.Object) null ? this.Opacity : this.Opacity * this.parent.CalculateOpacity();
    }

    protected internal Color32 ApplyOpacity(Color32 rawColor)
    {
      float opacity = this.CalculateOpacity();
      rawColor.a = (byte) ((double) rawColor.a * (double) opacity);
      return rawColor;
    }

    protected internal Vector2 GetHitPosition(dfMouseEventArgs args)
    {
      Vector2 position;
      this.GetHitPosition(args.Ray, out position);
      return position;
    }

    protected internal Vector3 getScaledDirection(Vector3 direction)
    {
      Vector3 localScale = this.GetManager().transform.localScale;
      direction = this.transform.TransformDirection(direction);
      return Vector3.Scale(direction, localScale);
    }

    protected internal Vector3 transformOffset(Vector3 offset)
    {
      return (offset.x * this.getScaledDirection(Vector3.right) + offset.y * this.getScaledDirection(Vector3.down)) * this.PixelsToUnits();
    }

    protected internal virtual void OnResolutionChanged(
      Vector2 previousResolution,
      Vector2 currentResolution)
    {
      this.updateControlHierarchy();
      this.cachedPixelSize = 0.0f;
      Vector3 vector3 = this.transform.localPosition / (2f / previousResolution.y) * (2f / currentResolution.y);
      this.transform.localPosition = vector3;
      this.cachedPosition = vector3;
      Vector3 relativePosition = this.RelativePosition;
      if (this.Anchor.IsAnyFlagSet(dfAnchorStyle.CenterHorizontal | dfAnchorStyle.CenterVertical | dfAnchorStyle.Proportional))
        this.PerformLayout();
      this.updateCollider();
      if (this.ResolutionChangedPostLayout != null)
        this.ResolutionChangedPostLayout(this, relativePosition, this.RelativePosition);
      this.Signal(nameof (OnResolutionChanged), (object) this, (object) previousResolution, (object) currentResolution);
      this.Invalidate();
    }

[HideInInspector]
    public virtual void Awake()
    {
      this.cachedParentTransform = this.transform.parent;
      if (this.anchorStyle == dfAnchorStyle.None && this.layout != null)
        this.anchorStyle = this.layout.AnchorStyle;
      if (!((UnityEngine.Object) this.GetComponent<Collider>() == (UnityEngine.Object) null))
        return;
      this.gameObject.AddComponent<BoxCollider>();
    }

[HideInInspector]
    public virtual void Start()
    {
    }

[HideInInspector]
    public virtual void OnEnable()
    {
      this.cachedManager = (dfGUIManager) null;
      this.cachedBounds = new Bounds?();
      this.cachedChildCount = 0;
      this.cachedParentTransform = this.transform.parent;
      this.cachedPosition = Vector3.zero;
      this.cachedRelativePosition = Vector3.zero;
      this.cachedRotation = Quaternion.identity;
      this.cachedScale = Vector3.one;
      dfControl.ActiveInstances.Add(this);
      this.initializeControl();
      if (Application.isPlaying && this.IsLocalized)
        this.Localize();
      this.OnIsEnabledChanged();
    }

[HideInInspector]
    public virtual void OnApplicationQuit()
    {
      this.shutdownInProgress = true;
      this.RemoveAllEventHandlers();
    }

[HideInInspector]
    public virtual void OnDisable()
    {
      dfControl.ActiveInstances.Remove(this);
      try
      {
        this.Invalidate();
        if (dfGUIManager.HasFocus(this))
          dfGUIManager.SetFocus((dfControl) null);
        else if ((UnityEngine.Object) dfGUIManager.GetModalControl() == (UnityEngine.Object) this)
          dfGUIManager.PopModal();
        this.OnIsEnabledChanged();
      }
      catch
      {
      }
      finally
      {
        this.isControlInitialized = false;
      }
    }

[HideInInspector]
    public virtual void OnDestroy()
    {
      this.isDisposing = true;
      this.isControlInitialized = false;
      if (Application.isPlaying)
        this.RemoveAllEventHandlers();
      if ((UnityEngine.Object) dfGUIManager.GetModalControl() == (UnityEngine.Object) this)
        dfGUIManager.PopModal();
      if (this.layout != null)
        this.layout.Dispose();
      if ((UnityEngine.Object) this.parent != (UnityEngine.Object) null && this.parent.controls != null && !this.parent.isDisposing && this.parent.controls.Remove(this))
      {
        --this.parent.cachedChildCount;
        this.parent.OnControlRemoved(this);
      }
      for (int index = 0; index < this.controls.Count; ++index)
      {
        if (this.controls[index].layout != null)
        {
          this.controls[index].layout.Dispose();
          this.controls[index].layout = (dfControl.AnchorLayout) null;
        }
        this.controls[index].parent = (dfControl) null;
      }
      this.controls.Release();
      if ((UnityEngine.Object) this.cachedManager != (UnityEngine.Object) null)
        this.cachedManager.Invalidate();
      if (this.renderData != null)
        this.renderData.Release();
      this.layout = (dfControl.AnchorLayout) null;
      this.cachedManager = (dfGUIManager) null;
      this.parent = (dfControl) null;
      this.cachedClippingPlanes = (Plane[]) null;
      this.cachedCorners = (Vector3[]) null;
      this.renderData = (dfRenderData) null;
      this.controls = (dfList<dfControl>) null;
    }

[HideInInspector]
    public virtual void LateUpdate()
    {
      if (this.layout == null || !this.layout.HasPendingLayoutRequest)
        return;
      this.layout.PerformLayout();
    }

[HideInInspector]
    public virtual void Update()
    {
      if (this.PrecludeUpdateCycle)
        return;
      if (!this.isControlInitialized)
        this.initializeControl();
      if ((UnityEngine.Object) this.m_transform == (UnityEngine.Object) null)
        this.m_transform = this.transform;
      if ((UnityEngine.Object) this.m_transform.parent != (UnityEngine.Object) this.cachedParentTransform)
      {
        this.cachedManager = (dfGUIManager) null;
        this.GetManager();
        this.cachedParentTransform = this.m_transform.parent;
        this.ResetLayout(false, true);
      }
      this.updateControlHierarchy();
      if (!this.m_transform.hasChanged)
        return;
      this.cachedBounds = new Bounds?();
      if (this.cachedScale != this.m_transform.localScale)
      {
        this.cachedScale = this.m_transform.localScale;
        this.Invalidate();
      }
      if ((double) Vector3.Distance(this.cachedPosition, this.m_transform.localPosition) > 1.4012984643248171E-45)
      {
        this.cachedPosition = this.m_transform.localPosition;
        this.OnPositionChanged();
      }
      if (this.cachedRotation != this.m_transform.localRotation)
      {
        this.cachedRotation = this.m_transform.localRotation;
        this.Invalidate();
      }
      this.m_transform.hasChanged = false;
    }

    protected internal void SetControlIndex(dfControl child, int zorder)
    {
      if (zorder < 0)
        zorder = int.MaxValue;
      this.controls.Sort();
      this.controls.Remove(child);
      if (zorder >= this.controls.Count)
        this.controls.Add(child);
      else
        this.controls.Insert(zorder, child);
      child.zindex = zorder;
      for (int index = 0; index < this.controls.Count; ++index)
      {
        if (this.controls[index].zindex != index)
        {
          dfControl control = this.controls[index];
          control.zindex = index;
          control.OnZOrderChanged();
        }
      }
    }

    public T AddControl<T>() where T : dfControl => (T) this.AddControl(typeof (T));

    public dfControl AddControl(System.Type controlType)
    {
      GameObject gameObject = typeof (dfControl).IsAssignableFrom(controlType) ? new GameObject(controlType.Name) : throw new InvalidCastException($"Type {controlType.Name} does not inherit from {typeof (dfControl).Name}");
      gameObject.transform.parent = this.transform;
      gameObject.layer = this.gameObject.layer;
      Vector2 vector2 = this.Size * this.PixelsToUnits() * 0.5f;
      gameObject.transform.localPosition = new Vector3(vector2.x, vector2.y, 0.0f);
      dfControl child = gameObject.AddComponent(controlType) as dfControl;
      child.parent = this;
      child.cachedManager = this.cachedManager;
      child.zindex = -1;
      this.AddControl(child);
      return child;
    }

    public void AddControl(dfControl child)
    {
      if ((UnityEngine.Object) child.transform == (UnityEngine.Object) null)
        throw new NullReferenceException("The child control does not have a Transform");
      if ((UnityEngine.Object) child.parent != (UnityEngine.Object) null && (UnityEngine.Object) child.parent != (UnityEngine.Object) this)
        child.parent.RemoveControl(child);
      if (!this.controls.Contains(child))
      {
        this.controls.Add(child);
        child.parent = this;
        child.transform.parent = this.transform;
        child.cachedManager = this.cachedManager;
        child.cachedParentTransform = this.transform;
      }
      if (child.zindex == -1 || child.zindex == int.MaxValue)
        this.SetControlIndex(child, int.MaxValue);
      else
        this.controls.Sort();
      this.OnControlAdded(child);
      child.Invalidate();
    }

    public dfControl AddPrefab(GameObject prefab)
    {
      GameObject gameObject = !((UnityEngine.Object) prefab.GetComponent<dfControl>() == (UnityEngine.Object) null) ? UnityEngine.Object.Instantiate<GameObject>(prefab) : throw new InvalidCastException();
      gameObject.transform.parent = this.transform;
      gameObject.layer = this.gameObject.layer;
      dfControl component = gameObject.GetComponent<dfControl>();
      component.parent = this;
      component.zindex = -1;
      this.AddControl(component);
      return component;
    }

    private int getMaxZOrder()
    {
      int b = -1;
      for (int index = 0; index < this.controls.Count; ++index)
        b = Mathf.Max(this.controls[index].zindex, b);
      return b;
    }

    public void RemoveControl(dfControl child)
    {
      if (this.isDisposing)
        return;
      if ((UnityEngine.Object) child.Parent == (UnityEngine.Object) this)
        child.parent = (dfControl) null;
      if (!this.controls.Remove(child))
        return;
      this.OnControlRemoved(child);
      child.Invalidate();
      this.Invalidate();
    }

[HideInInspector]
    public void RebuildControlOrder()
    {
      this.controls.Sort();
      for (int index = 0; index < this.controls.Count; ++index)
      {
        if (this.controls[index].zindex != index)
        {
          dfControl control = this.controls[index];
          control.zindex = index;
          control.OnZOrderChanged();
        }
      }
    }

    internal void setClippingState(bool isClipped)
    {
      if (isClipped == this.isControlClipped)
        return;
      this.isControlClipped = isClipped;
      this.OnControlClippingChanged();
    }

    private void doAutoFocus()
    {
      if (!Application.isPlaying || !this.IsEnabled || !this.enabled || !this.AutoFocus || !this.CanFocus || !this.IsVisible || !this.gameObject.activeSelf || !this.gameObject.activeInHierarchy)
        return;
      this.StartCoroutine(this.focusOnNextFrame());
    }

[DebuggerHidden]
    private IEnumerator focusOnNextFrame()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new dfControl__focusOnNextFramec__Iterator0()
      {
        _this = this
      };
    }

    protected void raiseSizeChangedEvent()
    {
      if (this.SizeChanged == null)
        return;
      this.SizeChanged(this, this.Size);
    }

    protected void raiseMouseDownEvent(dfMouseEventArgs args)
    {
      if (this.MouseDown == null)
        return;
      this.MouseDown(this, args);
    }

    protected void raiseMouseMoveEvent(dfMouseEventArgs args)
    {
      if (this.MouseMove == null)
        return;
      this.MouseMove(this, args);
    }

    protected void raiseMouseWheelEvent(dfMouseEventArgs args)
    {
      if (this.MouseWheel == null)
        return;
      this.MouseWheel(this, args);
    }

    private void initializeControl()
    {
      Transform parent = this.transform.parent;
      if ((UnityEngine.Object) parent == (UnityEngine.Object) null || (UnityEngine.Object) parent.GetComponent(typeof (IDFControlHost)) == (UnityEngine.Object) null)
        return;
      if ((UnityEngine.Object) parent != (UnityEngine.Object) null || (UnityEngine.Object) this.cachedParentTransform != (UnityEngine.Object) parent)
      {
        dfControl component = parent.GetComponent<dfControl>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          this.parent = component;
          component.AddControl(this);
        }
        if (this.controls == null)
          this.updateControlHierarchy();
      }
      if (this.renderOrder == -1)
        this.renderOrder = this.ZOrder;
      this.updateCollider();
      this.ensureLayoutExists();
      this.layout.Attach(this);
      if (!Application.isPlaying)
        this.PerformLayout();
      this.Invalidate();
      this.isControlInitialized = true;
    }

    internal void updateControlHierarchy() => this.updateControlHierarchy(false);

    internal void updateControlHierarchy(bool force)
    {
      int childCount = this.transform.childCount;
      if (!force && childCount == this.cachedChildCount)
        return;
      this.cachedChildCount = childCount;
      dfList<dfControl> childControls = this.getChildControls();
      for (int index = 0; index < childControls.Count; ++index)
      {
        dfControl child = childControls[index];
        if (!this.controls.Contains(child))
        {
          child.parent = this;
          child.cachedParentTransform = this.transform;
          if (!Application.isPlaying)
            child.ResetLayout();
          this.OnControlAdded(child);
          child.updateControlHierarchy();
        }
      }
      for (int index = 0; index < this.controls.Count; ++index)
      {
        dfControl control = this.controls[index];
        if ((UnityEngine.Object) control == (UnityEngine.Object) null || !childControls.Contains(control))
        {
          this.OnControlRemoved(control);
          if ((UnityEngine.Object) control != (UnityEngine.Object) null && (UnityEngine.Object) control.parent == (UnityEngine.Object) this)
            control.parent = (dfControl) null;
        }
      }
      this.controls.Release();
      this.controls = childControls;
      this.RebuildControlOrder();
    }

    private dfList<dfControl> getChildControls()
    {
      int childCount = this.transform.childCount;
      dfList<dfControl> childControls = dfList<dfControl>.Obtain(childCount);
      for (int index = 0; index < childCount; ++index)
      {
        Transform child = this.transform.GetChild(index);
        if (child.gameObject.activeSelf)
        {
          dfControl component = child.GetComponent<dfControl>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            childControls.Add(component);
        }
      }
      return childControls;
    }

    private void ensureLayoutExists()
    {
      if (this.layout == null)
      {
        dfAnchorStyle anchorStyle = this.anchorStyle == dfAnchorStyle.None ? dfAnchorStyle.Top | dfAnchorStyle.Left : this.anchorStyle;
        this.layout = new dfControl.AnchorLayout(anchorStyle, this);
        this.anchorStyle = anchorStyle;
      }
      else
        this.layout.Attach(this);
      for (int index = 0; this.Controls != null && index < this.Controls.Count; ++index)
      {
        if ((UnityEngine.Object) this.controls[index] != (UnityEngine.Object) null)
          this.controls[index].ensureLayoutExists();
      }
    }

    protected internal void updateVersion() => this.version = ++dfControl.versionCounter;

    private Vector3 getRelativePosition()
    {
      if ((int) this.relativePositionCacheVersion == (int) this.version)
        return this.cachedRelativePosition;
      this.relativePositionCacheVersion = this.version;
      if ((UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null)
        return Vector3.zero;
      if ((UnityEngine.Object) this.parent != (UnityEngine.Object) null)
      {
        float units = this.PixelsToUnits();
        Vector3 position1 = this.transform.parent.position;
        Vector3 position2 = this.transform.position;
        Transform parent = this.transform.parent;
        Vector3 vector3 = parent.InverseTransformPoint(position1 / units) + this.parent.pivot.TransformToUpperLeft(this.parent.size);
        return this.cachedRelativePosition = (parent.InverseTransformPoint(position2 / units) + this.pivot.TransformToUpperLeft(this.size) - vector3).Scale(1f, -1f, 1f);
      }
      dfGUIManager manager = this.GetManager();
      if ((UnityEngine.Object) manager == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogError((object) "Cannot get position: View not found");
        this.relativePositionCacheVersion = uint.MaxValue;
        return Vector3.zero;
      }
      float units1 = this.PixelsToUnits();
      Vector3 point = this.transform.position + this.pivot.TransformToUpperLeft(this.size) * units1;
      Plane[] clippingPlanes = manager.GetClippingPlanes();
      this.cachedRelativePosition = new Vector3(clippingPlanes[0].GetDistanceToPoint(point) / units1, clippingPlanes[3].GetDistanceToPoint(point) / units1).RoundToInt();
      return this.cachedRelativePosition;
    }

    private void setPositionInternal(Vector3 value)
    {
      value += this.pivot.UpperLeftToTransform(this.Size);
      value *= this.PixelsToUnits();
      if ((double) Vector3.Distance(value, this.cachedPosition) <= 1.4012984643248171E-45)
        return;
      Vector3 vector3 = value;
      this.transform.localPosition = vector3;
      this.cachedPosition = vector3;
      this.OnPositionChanged();
    }

    private void setRelativePosition(ref Vector3 value)
    {
      if ((UnityEngine.Object) this.transform.parent == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogError((object) "Cannot set relative position without a parent Transform.");
      }
      else
      {
        if ((double) (value - this.getRelativePosition()).magnitude <= 1.4012984643248171E-45)
          return;
        if ((UnityEngine.Object) this.parent != (UnityEngine.Object) null)
        {
          Vector3 vector3 = (value.Scale(1f, -1f, 1f) + this.pivot.UpperLeftToTransform(this.size) - this.parent.pivot.UpperLeftToTransform(this.parent.size)) * this.PixelsToUnits();
          if ((double) (vector3 - this.transform.localPosition).sqrMagnitude < 1.4012984643248171E-45)
            return;
          this.transform.localPosition = vector3;
          this.cachedPosition = vector3;
          this.OnPositionChanged();
        }
        else
        {
          dfGUIManager manager = this.GetManager();
          if ((UnityEngine.Object) manager == (UnityEngine.Object) null)
          {
            UnityEngine.Debug.LogError((object) "Cannot get position: View not found");
          }
          else
          {
            Vector3 corner = manager.GetCorners()[0];
            float units = this.PixelsToUnits();
            value = value.Scale(1f, -1f, 1f) * units;
            Vector3 vector3 = this.pivot.UpperLeftToTransform(this.Size) * units;
            Vector3 a = corner + manager.transform.TransformDirection(value) + vector3;
            if ((double) Vector3.Distance(a, this.cachedPosition) <= 1.4012984643248171E-45)
              return;
            this.transform.position = a;
            this.cachedPosition = this.transform.localPosition;
            this.OnPositionChanged();
          }
        }
      }
    }

    private static Vector3 closestPointOnLine(Vector3 start, Vector3 end, Vector3 test, bool clamp)
    {
      Vector3 rhs = test - start;
      Vector3 normalized = (end - start).normalized;
      float magnitude = (end - start).magnitude;
      float num = Vector3.Dot(normalized, rhs);
      if (clamp)
      {
        if ((double) num < 0.0)
          return start;
        if ((double) num > (double) magnitude)
          return end;
      }
      Vector3 vector3 = normalized * num;
      return start + vector3;
    }

    public int CompareTo(dfControl other)
    {
      if (this.ZOrder >= 0)
        return this.ZOrder.CompareTo(other.ZOrder);
      return other.ZOrder < 0 ? 0 : 1;
    }

protected class SignalCache
    {
      private static readonly List<dfControl.SignalCache.SignalCacheItem> cache = new List<dfControl.SignalCache.SignalCacheItem>();

      public static bool Invoke(
        Component target,
        string eventName,
        object[] arguments,
        out object returnValue)
      {
        returnValue = (object) null;
        if ((UnityEngine.Object) target == (UnityEngine.Object) null)
          return false;
        System.Type type = target.GetType();
        dfControl.SignalCache.SignalCacheItem signalCacheItem = dfControl.SignalCache.getItem(type, eventName);
        if (signalCacheItem == null)
        {
          System.Type[] paramTypes = new System.Type[arguments.Length];
          for (int index = 0; index < paramTypes.Length; ++index)
            paramTypes[index] = arguments[index] != null ? arguments[index].GetType() : typeof (object);
          signalCacheItem = new dfControl.SignalCache.SignalCacheItem(type, eventName, paramTypes);
          dfControl.SignalCache.cache.Add(signalCacheItem);
        }
        return signalCacheItem.Invoke((object) target, arguments, out returnValue);
      }

      private static dfControl.SignalCache.SignalCacheItem getItem(
        System.Type componentType,
        string eventName)
      {
        for (int index = 0; index < dfControl.SignalCache.cache.Count; ++index)
        {
          dfControl.SignalCache.SignalCacheItem signalCacheItem = dfControl.SignalCache.cache[index];
          if (signalCacheItem.ComponentType == componentType && signalCacheItem.EventName == eventName)
            return signalCacheItem;
        }
        return (dfControl.SignalCache.SignalCacheItem) null;
      }

      private static MethodInfo getMethod(System.Type type, string name, System.Type[] paramTypes)
      {
        return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, (Binder) null, paramTypes, (ParameterModifier[]) null);
      }

      private static bool matchesParameterTypes(MethodInfo method, System.Type[] types)
      {
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length != types.Length)
          return false;
        for (int index = 0; index < types.Length; ++index)
        {
          if (!parameters[index].ParameterType.IsAssignableFrom(types[index]))
            return false;
        }
        return true;
      }

      private class SignalCacheItem
      {
        public readonly System.Type ComponentType;
        public readonly string EventName;
        private readonly MethodInfo method;
        private readonly bool usesParameters;

        public SignalCacheItem(System.Type componentType, string eventName, System.Type[] paramTypes)
        {
          this.ComponentType = componentType;
          this.EventName = eventName;
          MethodInfo method = dfControl.SignalCache.getMethod(componentType, eventName, paramTypes);
          if (method != null)
          {
            this.method = method;
            this.usesParameters = true;
          }
          else
          {
            this.method = dfControl.SignalCache.getMethod(componentType, eventName, dfReflectionExtensions.EmptyTypes);
            this.usesParameters = false;
          }
        }

        public bool Invoke(object target, object[] arguments, out object returnValue)
        {
          if (this.method == null)
          {
            returnValue = (object) null;
            return false;
          }
          if (!this.usesParameters)
            arguments = (object[]) null;
          returnValue = this.method.Invoke(target, arguments);
          return true;
        }
      }
    }

    [Serializable]
    protected class AnchorLayout
    {
      [SerializeField]
      protected dfAnchorStyle anchorStyle;
      [SerializeField]
      protected dfAnchorMargins margins;
      [SerializeField]
      protected dfControl owner;
      private int suspendLayoutCounter;
      private bool performingLayout;
      private bool disposed;
      private bool pendingLayoutRequest;

      internal AnchorLayout(dfAnchorStyle anchorStyle) => this.anchorStyle = anchorStyle;

      internal AnchorLayout(dfAnchorStyle anchorStyle, dfControl owner)
        : this(anchorStyle)
      {
        this.Attach(owner);
        this.Reset();
      }

      internal dfAnchorStyle AnchorStyle
      {
        get => this.anchorStyle;
        set
        {
          if (value == this.anchorStyle)
            return;
          this.anchorStyle = value;
          this.Reset();
        }
      }

      internal bool IsPerformingLayout => this.performingLayout;

      internal bool IsLayoutSuspended => this.suspendLayoutCounter > 0;

      internal bool HasPendingLayoutRequest => this.pendingLayoutRequest;

      internal void Dispose()
      {
        if (this.disposed)
          return;
        this.disposed = true;
        this.owner = (dfControl) null;
      }

      internal void SuspendLayout() => ++this.suspendLayoutCounter;

      internal void ResumeLayout()
      {
        bool flag = this.suspendLayoutCounter > 0;
        this.suspendLayoutCounter = Mathf.Max(0, this.suspendLayoutCounter - 1);
        if (!flag || this.suspendLayoutCounter != 0 || !this.pendingLayoutRequest)
          return;
        this.PerformLayout();
      }

      internal void PerformLayout()
      {
        if (this.disposed)
          return;
        if (this.suspendLayoutCounter > 0)
          this.pendingLayoutRequest = true;
        else
          this.performLayoutInternal();
      }

      internal void Attach(dfControl ownerControl)
      {
        this.owner = ownerControl;
        if (!((UnityEngine.Object) ownerControl != (UnityEngine.Object) null))
          return;
        this.anchorStyle = ownerControl.anchorStyle;
      }

      internal void Reset() => this.Reset(false);

      internal void Reset(bool force)
      {
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null || (UnityEngine.Object) this.owner.transform.parent == (UnityEngine.Object) null || this.anchorStyle == dfAnchorStyle.None || !force && (this.IsPerformingLayout || this.IsLayoutSuspended) || (UnityEngine.Object) this.owner == (UnityEngine.Object) null || !this.owner.gameObject.activeSelf)
          return;
        if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Proportional))
          this.resetLayoutProportional();
        else
          this.resetLayoutAbsolute();
      }

      private void resetLayoutProportional()
      {
        Vector3 relativePosition = this.owner.RelativePosition;
        Vector2 size = this.owner.Size;
        Vector2 parentSize = this.getParentSize();
        float x = relativePosition.x;
        float y = relativePosition.y;
        float num1 = x + size.x;
        float num2 = y + size.y;
        if (this.margins == null)
          this.margins = new dfAnchorMargins();
        this.margins.left = x / parentSize.x;
        this.margins.right = num1 / parentSize.x;
        this.margins.top = y / parentSize.y;
        this.margins.bottom = num2 / parentSize.y;
      }

      private void resetLayoutAbsolute()
      {
        Vector3 relativePosition = this.owner.RelativePosition;
        Vector2 size = this.owner.Size;
        Vector2 parentSize = this.getParentSize();
        float x = relativePosition.x;
        float y = relativePosition.y;
        float num1 = parentSize.x - size.x - x;
        float num2 = parentSize.y - size.y - y;
        if (this.margins == null)
          this.margins = new dfAnchorMargins();
        this.margins.left = x;
        this.margins.right = num1;
        this.margins.top = y;
        this.margins.bottom = num2;
      }

      protected void performLayoutInternal()
      {
        if (this.anchorStyle == dfAnchorStyle.None)
          return;
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null || (UnityEngine.Object) this.owner.transform.parent == (UnityEngine.Object) null)
        {
          this.pendingLayoutRequest = true;
        }
        else
        {
          if (this.margins == null || this.IsPerformingLayout || this.IsLayoutSuspended || !this.owner.gameObject.activeSelf)
            return;
          try
          {
            this.performingLayout = true;
            this.pendingLayoutRequest = false;
            Vector2 parentSize = this.getParentSize();
            Vector2 size = this.owner.Size;
            if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Proportional))
              this.performLayoutProportional(parentSize, size);
            else
              this.performLayoutAbsolute(parentSize, size);
          }
          finally
          {
            this.performingLayout = false;
          }
        }
      }

      private void performLayoutProportional(Vector2 parentSize, Vector2 controlSize)
      {
        float num1 = this.margins.left * parentSize.x;
        float num2 = this.margins.right * parentSize.x;
        float num3 = this.margins.top * parentSize.y;
        float num4 = this.margins.bottom * parentSize.y;
        Vector3 relativePosition = this.owner.RelativePosition;
        Vector2 vector2 = controlSize;
        if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Left))
        {
          relativePosition.x = num1;
          if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Right))
            vector2.x = (this.margins.right - this.margins.left) * parentSize.x;
        }
        else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Right))
          relativePosition.x = num2 - controlSize.x;
        else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterHorizontal))
          relativePosition.x = (float) (((double) parentSize.x - (double) controlSize.x) * 0.5);
        if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Top))
        {
          relativePosition.y = num3;
          if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Bottom))
            vector2.y = (this.margins.bottom - this.margins.top) * parentSize.y;
        }
        else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Bottom))
          relativePosition.y = num4 - controlSize.y;
        else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterVertical))
          relativePosition.y = (float) (((double) parentSize.y - (double) controlSize.y) * 0.5);
        this.owner.Size = vector2;
        this.owner.RelativePosition = relativePosition;
        dfGUIManager manager = this.owner.GetManager();
        if (!((UnityEngine.Object) manager != (UnityEngine.Object) null) || !manager.PixelPerfectMode)
          return;
        this.owner.MakePixelPerfect(false);
      }

      private void performLayoutAbsolute(Vector2 parentSize, Vector2 controlSize)
      {
        float x = this.margins.left;
        float y = this.margins.top;
        float num1 = x + controlSize.x;
        float num2 = y + controlSize.y;
        if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterHorizontal))
        {
          x = (float) Mathf.RoundToInt((float) (((double) parentSize.x - (double) controlSize.x) * 0.5));
          num1 = (float) Mathf.RoundToInt(x + controlSize.x);
        }
        else
        {
          if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Left))
          {
            x = this.margins.left;
            num1 = x + controlSize.x;
          }
          if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Right))
          {
            num1 = parentSize.x - this.margins.right;
            if (!this.anchorStyle.IsFlagSet(dfAnchorStyle.Left))
              x = num1 - controlSize.x;
          }
        }
        if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterVertical))
        {
          y = (float) Mathf.RoundToInt((float) (((double) parentSize.y - (double) controlSize.y) * 0.5));
          num2 = (float) Mathf.RoundToInt(y + controlSize.y);
        }
        else
        {
          if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Top))
          {
            y = this.margins.top;
            num2 = y + controlSize.y;
          }
          if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Bottom))
          {
            num2 = parentSize.y - this.margins.bottom;
            if (!this.anchorStyle.IsFlagSet(dfAnchorStyle.Top))
              y = num2 - controlSize.y;
          }
        }
        Vector2 vector2 = new Vector2(Mathf.Max(0.0f, num1 - x), Mathf.Max(0.0f, num2 - y));
        Vector3 vector3 = new Vector3(x, y);
        this.owner.Size = vector2;
        this.owner.setRelativePosition(ref vector3);
      }

      private Vector2 getParentSize()
      {
        dfControl parent = this.owner.parent;
        return (UnityEngine.Object) parent != (UnityEngine.Object) null ? parent.Size : this.owner.GetManager().GetScreenSize();
      }

      public override string ToString()
      {
        if ((UnityEngine.Object) this.owner == (UnityEngine.Object) null)
          return "NO OWNER FOR ANCHOR";
        dfControl parent = this.owner.parent;
        return $"{(!((UnityEngine.Object) parent != (UnityEngine.Object) null) ? (object) "SCREEN" : (object) parent.name)}.{this.owner.name} - {this.margins}";
      }
    }
  }

