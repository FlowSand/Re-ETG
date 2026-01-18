using System;
using UnityEngine;

#nullable disable

[dfCategory("Basic Controls")]
[dfTooltip("Implements a progress bar that can be used to display the progress from a start value toward an end value, such as the amount of work completed or a player's progress toward some goal, etc.")]
[AddComponentMenu("Daikon Forge/User Interface/Progress Bar")]
[ExecuteInEditMode]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_progress_bar.html")]
[Serializable]
public class dfProgressBar : dfControl
  {
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected string progressSprite;
    [SerializeField]
    protected Color32 progressColor = (Color32) UnityEngine.Color.white;
    [SerializeField]
    protected float rawValue = 0.25f;
    [SerializeField]
    protected float minValue;
    [SerializeField]
    protected float maxValue = 1f;
    [SerializeField]
    protected dfProgressFillMode fillMode;
    [SerializeField]
    protected dfFillDirection progressFillDirection;
    [SerializeField]
    protected RectOffset padding = new RectOffset();
    [SerializeField]
    protected bool actAsSlider;

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

    public string BackgroundSprite
    {
      get => this.backgroundSprite;
      set
      {
        if (!(value != this.backgroundSprite))
          return;
        this.backgroundSprite = value;
        this.setDefaultSize(value);
        this.Invalidate();
      }
    }

    public string ProgressSprite
    {
      get => this.progressSprite;
      set
      {
        if (!(value != this.progressSprite))
          return;
        this.progressSprite = value;
        this.Invalidate();
      }
    }

    public Color32 ProgressColor
    {
      get => this.progressColor;
      set
      {
        if (object.Equals((object) value, (object) this.progressColor))
          return;
        this.progressColor = value;
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
        if ((double) this.rawValue < (double) value)
          this.Value = value;
        this.Invalidate();
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
        if ((double) this.rawValue > (double) value)
          this.Value = value;
        this.Invalidate();
      }
    }

    public float Value
    {
      get => this.rawValue;
      set
      {
        value = Mathf.Max(this.minValue, Mathf.Min(this.maxValue, value));
        if (Mathf.Approximately(value, this.rawValue))
          return;
        this.rawValue = value;
        this.OnValueChanged();
      }
    }

    public dfProgressFillMode FillMode
    {
      get => this.fillMode;
      set
      {
        if (value == this.fillMode)
          return;
        this.fillMode = value;
        this.Invalidate();
      }
    }

    public dfFillDirection ProgressFillDirection
    {
      get => this.progressFillDirection;
      set
      {
        if (value == this.progressFillDirection)
          return;
        this.progressFillDirection = value;
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

    public bool ActAsSlider
    {
      get => this.actAsSlider;
      set => this.actAsSlider = value;
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
      try
      {
        if (!this.actAsSlider)
          return;
        this.Value += (float) (((double) this.maxValue - (double) this.minValue) * 0.10000000149011612) * (float) Mathf.RoundToInt(-args.WheelDelta);
        args.Use();
      }
      finally
      {
        base.OnMouseWheel(args);
      }
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
      try
      {
        if (!this.actAsSlider || !args.Buttons.IsSet(dfMouseButtons.Left))
          return;
        this.Value = this.getValueFromMouseEvent(args);
        args.Use();
      }
      finally
      {
        base.OnMouseMove(args);
      }
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
      try
      {
        if (!this.actAsSlider || !args.Buttons.IsSet(dfMouseButtons.Left))
          return;
        this.Focus(true);
        this.Value = this.getValueFromMouseEvent(args);
        args.Use();
      }
      finally
      {
        base.OnMouseDown(args);
      }
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
      try
      {
        if (!this.actAsSlider)
          return;
        float num = (float) (((double) this.maxValue - (double) this.minValue) * 0.10000000149011612);
        if (args.KeyCode == KeyCode.LeftArrow)
        {
          this.Value -= num;
          args.Use();
        }
        else
        {
          if (args.KeyCode != KeyCode.RightArrow)
            return;
          this.Value += num;
          args.Use();
        }
      }
      finally
      {
        base.OnKeyDown(args);
      }
    }

    protected internal virtual void OnValueChanged()
    {
      this.Invalidate();
      this.SignalHierarchy(nameof (OnValueChanged), (object) this, (object) this.Value);
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((dfControl) this, this.Value);
    }

    protected override void OnRebuildRenderData()
    {
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
        return;
      this.renderData.Material = this.Atlas.Material;
      this.renderBackground();
      this.renderProgressFill();
    }

    private void renderProgressFill()
    {
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
        return;
      dfAtlas.ItemInfo atla = this.Atlas[this.progressSprite];
      if (atla == (dfAtlas.ItemInfo) null)
        return;
      Vector3 vector3 = new Vector3((float) this.padding.left, (float) -this.padding.top);
      Vector2 vector2 = new Vector2(this.size.x - (float) this.padding.horizontal, this.size.y - (float) this.padding.vertical);
      float num1 = 1f;
      float num2 = (this.rawValue - this.minValue) / (this.maxValue - this.minValue);
      dfProgressFillMode fillMode = this.fillMode;
      if (fillMode != dfProgressFillMode.Stretch || (double) vector2.x * (double) num2 >= (double) atla.border.horizontal)
        ;
      if (fillMode == dfProgressFillMode.Fill)
        num1 = num2;
      else
        vector2.x = Mathf.Max((float) atla.border.horizontal, vector2.x * num2);
      Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.DisabledColor : this.ProgressColor);
      dfSprite.RenderOptions options = new dfSprite.RenderOptions()
      {
        atlas = this.atlas,
        color = color32,
        fillAmount = num1,
        flip = dfSpriteFlip.None,
        offset = this.pivot.TransformToUpperLeft(this.Size) + vector3,
        pixelsToUnits = this.PixelsToUnits(),
        size = vector2,
        spriteInfo = atla
      };
      if (this.progressFillDirection == dfFillDirection.Vertical)
        options.invertFill = true;
      options.fillDirection = this.progressFillDirection;
      if (atla.border.horizontal == 0 && atla.border.vertical == 0)
        dfSprite.renderSprite(this.renderData, options);
      else
        dfSlicedSprite.renderSprite(this.renderData, options);
    }

    private void renderBackground()
    {
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
        return;
      dfAtlas.ItemInfo atla = this.Atlas[this.backgroundSprite];
      if (atla == (dfAtlas.ItemInfo) null)
        return;
      Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.DisabledColor : this.Color);
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

    private float getValueFromMouseEvent(dfMouseEventArgs args)
    {
      Vector3[] endPoints = this.getEndPoints(true);
      Vector3 vector3 = endPoints[0];
      Vector3 end = endPoints[1];
      Plane plane = new Plane(this.transform.TransformDirection(Vector3.back), vector3);
      Ray ray = args.Ray;
      float enter = 0.0f;
      if (!plane.Raycast(ray, out enter))
        return this.rawValue;
      Vector3 test = ray.origin + ray.direction * enter;
      return this.minValue + (this.maxValue - this.minValue) * ((dfProgressBar.closestPoint(vector3, end, test, true) - vector3).magnitude / (end - vector3).magnitude);
    }

    private Vector3[] getEndPoints() => this.getEndPoints(false);

    private Vector3[] getEndPoints(bool convertToWorld)
    {
      Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.Size);
      Vector3 vector3_1 = new Vector3(upperLeft.x + (float) this.padding.left, upperLeft.y - this.size.y * 0.5f);
      Vector3 vector3_2 = vector3_1 + new Vector3(this.size.x - (float) this.padding.right, 0.0f);
      if (convertToWorld)
      {
        float units = this.PixelsToUnits();
        Matrix4x4 localToWorldMatrix = this.transform.localToWorldMatrix;
        vector3_1 = localToWorldMatrix.MultiplyPoint(vector3_1 * units);
        vector3_2 = localToWorldMatrix.MultiplyPoint(vector3_2 * units);
      }
      return new Vector3[2]{ vector3_1, vector3_2 };
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

    private void setDefaultSize(string spriteName)
    {
      if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
        return;
      dfAtlas.ItemInfo atla = this.Atlas[spriteName];
      if (!(this.size == Vector2.zero) || !(atla != (dfAtlas.ItemInfo) null))
        return;
      this.Size = atla.sizeInPixels;
    }
  }

