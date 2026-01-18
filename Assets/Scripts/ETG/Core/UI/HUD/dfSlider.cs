using System;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/User Interface/Slider")]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[dfTooltip("Allows the user to select any value from a specified range by moving an indicator along a horizontal or vertical track")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_slider.html")]
[Serializable]
public class dfSlider : dfControl
    {
        [SerializeField]
        protected dfAtlas atlas;
        [SerializeField]
        protected string backgroundSprite;
        [SerializeField]
        protected dfControlOrientation orientation;
        [SerializeField]
        protected float rawValue = 10f;
        [SerializeField]
        protected float minValue;
        [SerializeField]
        protected float maxValue = 100f;
        [SerializeField]
        protected float stepSize = 1f;
        [SerializeField]
        protected float scrollSize = 1f;
        [SerializeField]
        protected dfControl thumb;
        [SerializeField]
        protected dfControl fillIndicator;
        [SerializeField]
        protected dfProgressFillMode fillMode = dfProgressFillMode.Fill;
        [SerializeField]
        protected RectOffset fillPadding = new RectOffset();
        [SerializeField]
        protected Vector2 thumbOffset = Vector2.zero;
        [SerializeField]
        protected bool rightToLeft;

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
                this.updateValueIndicators(this.rawValue);
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
                this.updateValueIndicators(this.rawValue);
                this.Invalidate();
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
                this.Value = this.rawValue.Quantize(value);
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
                this.Invalidate();
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
                this.updateValueIndicators(this.rawValue);
            }
        }

        public float Value
        {
            get => this.rawValue;
            set
            {
                value = Mathf.Max(this.minValue, Mathf.Min(this.maxValue, value.RoundToNearest(this.stepSize)));
                if (Mathf.Approximately(value, this.rawValue))
                    return;
                this.rawValue = value;
                this.OnValueChanged();
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
                this.updateValueIndicators(this.rawValue);
            }
        }

        public dfControl Progress
        {
            get => this.fillIndicator;
            set
            {
                if (!((UnityEngine.Object) value != (UnityEngine.Object) this.fillIndicator))
                    return;
                this.fillIndicator = value;
                this.Invalidate();
                this.updateValueIndicators(this.rawValue);
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

        public RectOffset FillPadding
        {
            get
            {
                if (this.fillPadding == null)
                    this.fillPadding = new RectOffset();
                return this.fillPadding;
            }
            set
            {
                if (object.Equals((object) value, (object) this.fillPadding))
                    return;
                this.fillPadding = value;
                this.updateValueIndicators(this.rawValue);
                this.Invalidate();
            }
        }

        public Vector2 ThumbOffset
        {
            get => this.thumbOffset;
            set
            {
                if ((double) Vector2.Distance(value, this.thumbOffset) <= 1.4012984643248171E-45)
                    return;
                this.thumbOffset = value;
                this.updateValueIndicators(this.rawValue);
            }
        }

        public bool RightToLeft
        {
            get => this.rightToLeft;
            set
            {
                if (value == this.rightToLeft)
                    return;
                this.rightToLeft = value;
                this.updateValueIndicators(this.rawValue);
            }
        }

        protected internal override void OnKeyDown(dfKeyEventArgs args)
        {
            if (args.Used)
                return;
            if (this.Orientation == dfControlOrientation.Horizontal)
            {
                if (args.KeyCode == KeyCode.LeftArrow)
                {
                    this.Value -= !this.rightToLeft ? this.scrollSize : -this.scrollSize;
                    args.Use();
                    return;
                }
                if (args.KeyCode == KeyCode.RightArrow)
                {
                    this.Value += !this.rightToLeft ? this.scrollSize : -this.scrollSize;
                    args.Use();
                    return;
                }
            }
            else
            {
                if (args.KeyCode == KeyCode.UpArrow)
                {
                    this.Value += this.ScrollSize;
                    args.Use();
                    return;
                }
                if (args.KeyCode == KeyCode.DownArrow)
                {
                    this.Value -= this.ScrollSize;
                    args.Use();
                    return;
                }
            }
            base.OnKeyDown(args);
        }

        public override void Start()
        {
            base.Start();
            this.updateValueIndicators(this.rawValue);
        }

        public override void OnEnable()
        {
            if ((double) this.size.magnitude < 1.4012984643248171E-45)
                this.size = new Vector2(100f, 25f);
            base.OnEnable();
            this.updateValueIndicators(this.rawValue);
        }

        protected internal override void OnMouseWheel(dfMouseEventArgs args)
        {
            int num = this.orientation != dfControlOrientation.Horizontal ? 1 : -1;
            this.Value += this.scrollSize * args.WheelDelta * (float) num;
            args.Use();
            this.Signal(nameof (OnMouseWheel), (object) args);
            this.raiseMouseWheelEvent(args);
        }

        protected internal override void OnMouseMove(dfMouseEventArgs args)
        {
            if (!args.Buttons.IsSet(dfMouseButtons.Left))
            {
                base.OnMouseMove(args);
            }
            else
            {
                this.Value = this.getValueFromMouseEvent(args);
                args.Use();
                this.Signal(nameof (OnMouseMove), (object) this, (object) args);
                this.raiseMouseMoveEvent(args);
            }
        }

        protected internal override void OnMouseDown(dfMouseEventArgs args)
        {
            if (!args.Buttons.IsSet(dfMouseButtons.Left))
            {
                base.OnMouseMove(args);
            }
            else
            {
                this.Focus(true);
                this.Value = this.getValueFromMouseEvent(args);
                args.Use();
                this.Signal(nameof (OnMouseDown), (object) this, (object) args);
                this.raiseMouseDownEvent(args);
            }
        }

        protected internal override void OnSizeChanged()
        {
            base.OnSizeChanged();
            this.updateValueIndicators(this.rawValue);
        }

        protected internal virtual void OnValueChanged()
        {
            this.Invalidate();
            this.updateValueIndicators(this.rawValue);
            this.SignalHierarchy(nameof (OnValueChanged), (object) this, (object) this.Value);
            if (this.ValueChanged == null)
                return;
            this.ValueChanged((dfControl) this, this.Value);
        }

        public override bool CanFocus => this.IsEnabled && this.IsVisible || base.CanFocus;

        protected override void OnRebuildRenderData()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return;
            this.renderData.Material = this.Atlas.Material;
            this.renderBackground();
        }

        protected internal virtual void renderBackground()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return;
            dfAtlas.ItemInfo atla = this.Atlas[this.backgroundSprite];
            if (atla == (dfAtlas.ItemInfo) null)
                return;
            Color32 color32 = this.ApplyOpacity(!this.IsEnabled ? this.disabledColor : this.color);
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

        private void updateValueIndicators(float rawValue)
        {
            if (Mathf.Approximately(this.MinValue, this.MaxValue))
            {
                if (Application.isEditor)
                    Debug.LogWarning((object) "Slider Min and Max values cannot be the same", (UnityEngine.Object) this);
                if ((UnityEngine.Object) this.thumb != (UnityEngine.Object) null)
                    this.thumb.IsVisible = false;
                if (!((UnityEngine.Object) this.fillIndicator != (UnityEngine.Object) null))
                    return;
                this.fillIndicator.IsVisible = false;
            }
            else
            {
                if ((UnityEngine.Object) this.thumb != (UnityEngine.Object) null)
                    this.thumb.IsVisible = true;
                if ((UnityEngine.Object) this.fillIndicator != (UnityEngine.Object) null)
                    this.fillIndicator.IsVisible = true;
                if ((UnityEngine.Object) this.thumb != (UnityEngine.Object) null)
                {
                    Vector3[] endPoints = this.getEndPoints(true);
                    Vector3 vector3_1 = endPoints[1] - endPoints[0];
                    float num1 = this.maxValue - this.minValue;
                    float num2 = (rawValue - this.minValue) / num1 * vector3_1.magnitude;
                    Vector3 vector3_2 = (Vector3) this.thumbOffset * this.PixelsToUnits();
                    Vector3 vector3_3 = endPoints[0] + vector3_1.normalized * num2 + vector3_2;
                    if (this.orientation == dfControlOrientation.Vertical || this.rightToLeft)
                        vector3_3 = endPoints[1] + -vector3_1.normalized * num2 + vector3_2;
                    this.thumb.transform.position = vector3_3;
                    this.thumb.transform.localPosition = this.thumb.transform.localPosition.WithY(0.0f);
                }
                if ((UnityEngine.Object) this.fillIndicator == (UnityEngine.Object) null)
                    return;
                RectOffset fillPadding = this.FillPadding;
                float num = (float) (((double) rawValue - (double) this.minValue) / ((double) this.maxValue - (double) this.minValue));
                Vector3 vector3 = new Vector3((float) fillPadding.left, (float) fillPadding.top);
                Vector2 vector2 = this.size - new Vector2((float) fillPadding.horizontal, (float) fillPadding.vertical);
                dfSprite fillIndicator = this.fillIndicator as dfSprite;
                if ((UnityEngine.Object) fillIndicator != (UnityEngine.Object) null && this.fillMode == dfProgressFillMode.Fill)
                {
                    fillIndicator.FillAmount = num;
                    fillIndicator.FillDirection = this.orientation != dfControlOrientation.Horizontal ? dfFillDirection.Vertical : dfFillDirection.Horizontal;
                    fillIndicator.InvertFill = this.rightToLeft || this.orientation == dfControlOrientation.Vertical;
                }
                else if (this.orientation == dfControlOrientation.Horizontal)
                {
                    vector2.x = this.Width * num - (float) fillPadding.horizontal;
                }
                else
                {
                    vector2.y = this.Height * num - (float) fillPadding.vertical;
                    vector3.y = this.Height - vector2.y;
                }
                this.fillIndicator.Size = vector2;
                this.fillIndicator.RelativePosition = vector3;
            }
        }

        private float getValueFromMouseEvent(dfMouseEventArgs args)
        {
            Vector3[] endPoints = this.getEndPoints(true);
            Vector3 vector3 = endPoints[0];
            Vector3 end = endPoints[1];
            if (this.orientation == dfControlOrientation.Vertical || this.rightToLeft)
            {
                vector3 = endPoints[1];
                end = endPoints[0];
            }
            Plane plane = new Plane(this.transform.TransformDirection(Vector3.back), vector3);
            Ray ray = args.Ray;
            float enter = 0.0f;
            if (!plane.Raycast(ray, out enter))
                return this.rawValue;
            Vector3 point = ray.GetPoint(enter);
            return this.minValue + (this.maxValue - this.minValue) * ((dfSlider.closestPoint(vector3, end, point, true) - vector3).magnitude / (end - vector3).magnitude);
        }

        private Vector3[] getEndPoints() => this.getEndPoints(false);

        private Vector3[] getEndPoints(bool convertToWorld)
        {
            Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.Size);
            Vector3 vector3_1 = new Vector3(upperLeft.x, upperLeft.y - this.size.y * 0.5f);
            Vector3 vector3_2 = vector3_1 + new Vector3(this.size.x, 0.0f);
            if (this.orientation == dfControlOrientation.Vertical)
            {
                vector3_1 = new Vector3(upperLeft.x + this.size.x * 0.5f, upperLeft.y);
                vector3_2 = vector3_1 - new Vector3(0.0f, this.size.y);
            }
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
    }

