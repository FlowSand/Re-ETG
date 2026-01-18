// Decompiled with JetBrains decompiler
// Type: dfScrollbar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/User Interface/Scrollbar")]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[dfTooltip("Implements a common Scrollbar control")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_scrollbar.html")]
[Serializable]
public class dfScrollbar : dfControl
  {
    [SerializeField]
    public bool ControlledByRightStick;
    [NonSerialized]
    public bool IsBeingMovedByRightStick;
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected dfControlOrientation orientation;
    [SerializeField]
    protected float rawValue = 1f;
    [SerializeField]
    protected float minValue;
    [SerializeField]
    protected float maxValue = 100f;
    [SerializeField]
    protected float stepSize = 1f;
    [SerializeField]
    protected float scrollSize = 1f;
    [SerializeField]
    protected float increment = 1f;
    [SerializeField]
    protected dfControl thumb;
    [SerializeField]
    protected dfControl track;
    [SerializeField]
    protected dfControl incButton;
    [SerializeField]
    protected dfControl decButton;
    [SerializeField]
    protected RectOffset thumbPadding = new RectOffset();
    [SerializeField]
    protected bool autoHide;
    private Vector3 thumbMouseOffset = Vector3.zero;

    public event PropertyChangedEventHandler<float> ValueChanged;

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

    public float MinValue
    {
      get => this.minValue;
      set
      {
        if ((double) value == (double) this.minValue)
          return;
        this.minValue = value;
        this.Value = this.Value;
        this.Invalidate();
        this.doAutoHide();
      }
    }

    public float MaxValue
    {
      get => this.maxValue;
      set
      {
        if ((double) value == (double) this.maxValue)
          return;
        this.maxValue = value;
        this.Value = this.Value;
        this.Invalidate();
        this.doAutoHide();
      }
    }

    public float StepSize
    {
      get => this.stepSize;
      set
      {
        value = Mathf.Max(0.0f, value);
        if ((double) value == (double) this.stepSize)
          return;
        this.stepSize = value;
        this.Value = this.Value;
        this.Invalidate();
      }
    }

    public float ScrollSize
    {
      get => this.scrollSize;
      set
      {
        value = Mathf.Max(0.0f, value);
        if ((double) value == (double) this.scrollSize)
          return;
        this.scrollSize = value;
        this.Value = this.Value;
        this.Invalidate();
        this.doAutoHide();
      }
    }

    public float IncrementAmount
    {
      get => this.increment;
      set
      {
        value = Mathf.Max(0.0f, value);
        if (Mathf.Approximately(value, this.increment))
          return;
        this.increment = value;
      }
    }

    public dfControlOrientation Orientation
    {
      get => this.orientation;
      set
      {
        if (value == this.orientation)
          return;
        this.orientation = value;
        this.Invalidate();
      }
    }

    public float Value
    {
      get => this.rawValue;
      set
      {
        value = this.adjustValue(value);
        if (!Mathf.Approximately(value, this.rawValue))
        {
          this.rawValue = value;
          this.OnValueChanged();
        }
        this.updateThumb(this.rawValue);
        this.doAutoHide();
      }
    }

    public dfControl Thumb
    {
      get => this.thumb;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.thumb))
          return;
        this.thumb = value;
        this.Invalidate();
      }
    }

    public dfControl Track
    {
      get => this.track;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.track))
          return;
        this.track = value;
        this.Invalidate();
      }
    }

    public dfControl IncButton
    {
      get => this.incButton;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.incButton))
          return;
        this.incButton = value;
        this.Invalidate();
      }
    }

    public dfControl DecButton
    {
      get => this.decButton;
      set
      {
        if (!((UnityEngine.Object) value != (UnityEngine.Object) this.decButton))
          return;
        this.decButton = value;
        this.Invalidate();
      }
    }

    public RectOffset ThumbPadding
    {
      get
      {
        if (this.thumbPadding == null)
          this.thumbPadding = new RectOffset();
        return this.thumbPadding;
      }
      set
      {
        if (this.orientation == dfControlOrientation.Horizontal)
        {
          RectOffset rectOffset = value;
          int num1 = 0;
          value.bottom = num1;
          int num2 = num1;
          rectOffset.top = num2;
        }
        else
        {
          RectOffset rectOffset = value;
          int num3 = 0;
          value.right = num3;
          int num4 = num3;
          rectOffset.left = num4;
        }
        if (object.Equals((object) value, (object) this.thumbPadding))
          return;
        this.thumbPadding = value;
        this.updateThumb(this.rawValue);
      }
    }

    public bool AutoHide
    {
      get => this.autoHide;
      set
      {
        if (value == this.autoHide)
          return;
        this.autoHide = value;
        this.Invalidate();
        this.doAutoHide();
      }
    }

    public override Vector2 CalculateMinimumSize()
    {
      Vector2[] vector2Array = new Vector2[3];
      if ((UnityEngine.Object) this.decButton != (UnityEngine.Object) null)
        vector2Array[0] = this.decButton.CalculateMinimumSize();
      if ((UnityEngine.Object) this.incButton != (UnityEngine.Object) null)
        vector2Array[1] = this.incButton.CalculateMinimumSize();
      if ((UnityEngine.Object) this.thumb != (UnityEngine.Object) null)
        vector2Array[2] = this.thumb.CalculateMinimumSize();
      Vector2 zero = Vector2.zero;
      if (this.orientation == dfControlOrientation.Horizontal)
      {
        zero.x = vector2Array[0].x + vector2Array[1].x + vector2Array[2].x;
        zero.y = Mathf.Max(vector2Array[0].y, vector2Array[1].y, vector2Array[2].y);
      }
      else
      {
        zero.x = Mathf.Max(vector2Array[0].x, vector2Array[1].x, vector2Array[2].x);
        zero.y = vector2Array[0].y + vector2Array[1].y + vector2Array[2].y;
      }
      return Vector2.Max(zero, base.CalculateMinimumSize());
    }

    public override bool CanFocus => this.IsEnabled && this.IsVisible || base.CanFocus;

    protected override void OnRebuildRenderData()
    {
      this.updateThumb(this.rawValue);
      base.OnRebuildRenderData();
    }

    public override void Start()
    {
      base.Start();
      this.attachEvents();
    }

    public override void Update()
    {
      base.Update();
      if (!this.ControlledByRightStick || InputManager.ActiveDevice == null)
        return;
      float y = InputManager.ActiveDevice.RightStick.Y;
      if ((double) Mathf.Abs(y) <= 0.10000000149011612)
        return;
      this.IsBeingMovedByRightStick = true;
      this.Value += (float) ((double) this.IncrementAmount * (double) y * -6.0) * GameManager.INVARIANT_DELTA_TIME;
      this.IsBeingMovedByRightStick = false;
    }

    public override void OnDisable()
    {
      base.OnDisable();
      this.detachEvents();
    }

    public override void OnDestroy()
    {
      base.OnDestroy();
      this.detachEvents();
    }

    private void attachEvents()
    {
      if (!Application.isPlaying)
        return;
      if ((UnityEngine.Object) this.IncButton != (UnityEngine.Object) null)
      {
        this.IncButton.MouseDown += new MouseEventHandler(this.incrementPressed);
        this.IncButton.MouseHover += new MouseEventHandler(this.incrementPressed);
      }
      if (!((UnityEngine.Object) this.DecButton != (UnityEngine.Object) null))
        return;
      this.DecButton.MouseDown += new MouseEventHandler(this.decrementPressed);
      this.DecButton.MouseHover += new MouseEventHandler(this.decrementPressed);
    }

    private void detachEvents()
    {
      if (!Application.isPlaying)
        return;
      if ((UnityEngine.Object) this.IncButton != (UnityEngine.Object) null)
      {
        this.IncButton.MouseDown -= new MouseEventHandler(this.incrementPressed);
        this.IncButton.MouseHover -= new MouseEventHandler(this.incrementPressed);
      }
      if (!((UnityEngine.Object) this.DecButton != (UnityEngine.Object) null))
        return;
      this.DecButton.MouseDown -= new MouseEventHandler(this.decrementPressed);
      this.DecButton.MouseHover -= new MouseEventHandler(this.decrementPressed);
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
      if (this.Orientation == dfControlOrientation.Horizontal)
      {
        if (args.KeyCode == KeyCode.LeftArrow)
        {
          this.Value -= this.IncrementAmount;
          args.Use();
          return;
        }
        if (args.KeyCode == KeyCode.RightArrow)
        {
          this.Value += this.IncrementAmount;
          args.Use();
          return;
        }
      }
      else
      {
        if (args.KeyCode == KeyCode.UpArrow)
        {
          this.Value -= this.IncrementAmount;
          args.Use();
          return;
        }
        if (args.KeyCode == KeyCode.DownArrow)
        {
          this.Value += this.IncrementAmount;
          args.Use();
          return;
        }
      }
      base.OnKeyDown(args);
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
      if (args.Used)
        return;
      this.Value += this.IncrementAmount * -args.WheelDelta;
      args.Use();
      this.Signal(nameof (OnMouseWheel), (object) this, (object) args);
    }

    protected internal override void OnMouseHover(dfMouseEventArgs args)
    {
      if ((UnityEngine.Object) args.Source == (UnityEngine.Object) this.incButton || (UnityEngine.Object) args.Source == (UnityEngine.Object) this.decButton || (UnityEngine.Object) args.Source == (UnityEngine.Object) this.thumb)
        return;
      if ((UnityEngine.Object) args.Source != (UnityEngine.Object) this.track || !args.Buttons.IsSet(dfMouseButtons.Left))
      {
        base.OnMouseHover(args);
      }
      else
      {
        this.updateFromTrackClick(args);
        args.Use();
        this.Signal(nameof (OnMouseHover), (object) this, (object) args);
      }
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
      if ((UnityEngine.Object) args.Source == (UnityEngine.Object) this.incButton || (UnityEngine.Object) args.Source == (UnityEngine.Object) this.decButton)
        return;
      if ((UnityEngine.Object) args.Source != (UnityEngine.Object) this.track && (UnityEngine.Object) args.Source != (UnityEngine.Object) this.thumb || !args.Buttons.IsSet(dfMouseButtons.Left))
      {
        base.OnMouseMove(args);
      }
      else
      {
        float valueFromMouseEvent = this.getValueFromMouseEvent(args);
        float num = this.thumb.Height / 2f;
        float t = Mathf.InverseLerp(this.minValue, this.maxValue, valueFromMouseEvent);
        this.Value = Mathf.Lerp(this.minValue - num, this.maxValue - this.scrollSize + num, t);
        args.Use();
        this.Signal(nameof (OnMouseMove), (object) this, (object) args);
      }
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
      if (args.Buttons.IsSet(dfMouseButtons.Left))
        this.Focus(true);
      if ((UnityEngine.Object) args.Source == (UnityEngine.Object) this.incButton || (UnityEngine.Object) args.Source == (UnityEngine.Object) this.decButton)
        return;
      if ((UnityEngine.Object) args.Source != (UnityEngine.Object) this.track && (UnityEngine.Object) args.Source != (UnityEngine.Object) this.thumb || !args.Buttons.IsSet(dfMouseButtons.Left))
      {
        base.OnMouseDown(args);
      }
      else
      {
        if ((UnityEngine.Object) args.Source == (UnityEngine.Object) this.thumb)
        {
          RaycastHit hitInfo;
          this.thumb.GetComponent<Collider>().Raycast(args.Ray, out hitInfo, 1000f);
          this.thumbMouseOffset = this.thumb.transform.position + this.thumb.Pivot.TransformToCenter(this.thumb.Size * this.PixelsToUnits()) - hitInfo.point;
        }
        else
          this.updateFromTrackClick(args);
        args.Use();
        this.Signal(nameof (OnMouseDown), (object) this, (object) args);
      }
    }

    protected internal virtual void OnValueChanged()
    {
      this.doAutoHide();
      this.Invalidate();
      this.SignalHierarchy(nameof (OnValueChanged), (object) this, (object) this.Value);
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((dfControl) this, this.Value);
    }

    protected internal override void OnSizeChanged()
    {
      base.OnSizeChanged();
      this.updateThumb(this.rawValue);
    }

    private void doAutoHide()
    {
      if (!this.autoHide || !Application.isPlaying)
        return;
      if (Mathf.CeilToInt(this.ScrollSize) >= Mathf.CeilToInt(this.maxValue - this.minValue))
        this.Hide();
      else
        this.Show();
    }

    private void incrementPressed(dfControl sender, dfMouseEventArgs args)
    {
      if (!args.Buttons.IsSet(dfMouseButtons.Left))
        return;
      this.Value += this.IncrementAmount;
      args.Use();
    }

    private void decrementPressed(dfControl sender, dfMouseEventArgs args)
    {
      if (!args.Buttons.IsSet(dfMouseButtons.Left))
        return;
      this.Value -= this.IncrementAmount;
      args.Use();
    }

    private void updateFromTrackClick(dfMouseEventArgs args)
    {
      float valueFromMouseEvent = this.getValueFromMouseEvent(args);
      if ((double) valueFromMouseEvent > (double) this.rawValue + (double) this.scrollSize)
      {
        this.Value += this.scrollSize;
      }
      else
      {
        if ((double) valueFromMouseEvent >= (double) this.rawValue)
          return;
        this.Value -= this.scrollSize;
      }
    }

    private float adjustValue(float value)
    {
      return Mathf.Max(Mathf.Min(Mathf.Max(Mathf.Max(this.maxValue - this.minValue, 0.0f) - this.scrollSize, 0.0f) + this.minValue, value), this.minValue).Quantize(this.stepSize);
    }

    private void updateThumb(float rawValue)
    {
      if (this.controls.Count == 0 || (UnityEngine.Object) this.thumb == (UnityEngine.Object) null || (UnityEngine.Object) this.track == (UnityEngine.Object) null || !this.IsVisible)
        return;
      float num1 = this.maxValue - this.minValue;
      if ((double) num1 <= 0.0 || (double) num1 <= (double) this.scrollSize)
      {
        this.thumb.IsVisible = false;
      }
      else
      {
        this.thumb.IsVisible = true;
        float num2 = this.orientation != dfControlOrientation.Horizontal ? this.track.Height : this.track.Width;
        float num3 = this.orientation != dfControlOrientation.Horizontal ? Mathf.Min(this.thumb.MaximumSize.y, Mathf.Max(this.scrollSize / num1 * num2, this.thumb.MinimumSize.y)) : Mathf.Min(this.thumb.MaximumSize.x, Mathf.Max(this.scrollSize / num1 * num2, this.thumb.MinimumSize.x));
        Vector2 vector2 = this.orientation != dfControlOrientation.Horizontal ? new Vector2(this.thumb.Width, num3) : new Vector2(num3, this.thumb.Height);
        if (this.Orientation == dfControlOrientation.Horizontal)
          vector2.x -= (float) this.thumbPadding.horizontal;
        else
          vector2.y -= (float) this.thumbPadding.vertical;
        this.thumb.Size = vector2;
        float num4 = (float) (((double) rawValue - (double) this.minValue) / ((double) num1 - (double) this.scrollSize)) * (num2 - num3);
        Vector3 vector3_1 = this.orientation != dfControlOrientation.Horizontal ? Vector3.up : Vector3.right;
        Vector3 vector3_2 = this.Orientation != dfControlOrientation.Horizontal ? new Vector3((float) (((double) this.track.Width - (double) this.thumb.Width) * 0.5), 0.0f) : new Vector3(0.0f, (float) (((double) this.track.Height - (double) this.thumb.Height) * 0.5));
        if (this.Orientation == dfControlOrientation.Horizontal)
          vector3_2.x = (float) this.thumbPadding.left;
        else
          vector3_2.y = (float) this.thumbPadding.top;
        if ((UnityEngine.Object) this.thumb.Parent == (UnityEngine.Object) this)
          this.thumb.RelativePosition = this.track.RelativePosition + vector3_2 + vector3_1 * num4;
        else
          this.thumb.RelativePosition = vector3_1 * num4 + vector3_2;
      }
    }

    private float getValueFromMouseEvent(dfMouseEventArgs args)
    {
      Vector3[] corners = this.track.GetCorners();
      Vector3 vector3 = corners[0];
      Vector3 end = corners[this.orientation != dfControlOrientation.Horizontal ? 2 : 1];
      Plane plane = new Plane(this.transform.TransformDirection(Vector3.back), vector3);
      Ray ray = args.Ray;
      float enter = 0.0f;
      if (!plane.Raycast(ray, out enter))
        return this.rawValue;
      Vector3 test = ray.origin + ray.direction * enter;
      if ((UnityEngine.Object) args.Source == (UnityEngine.Object) this.thumb)
        test += this.thumbMouseOffset;
      return this.minValue + (this.maxValue - this.minValue) * ((dfScrollbar.closestPoint(vector3, end, test, true) - vector3).magnitude / (end - vector3).magnitude);
    }

    private static Vector3 closestPoint(Vector3 start, Vector3 end, Vector3 test, bool clamp)
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
  }

