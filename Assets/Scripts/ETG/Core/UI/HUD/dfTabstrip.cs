using System;

using UnityEngine;

#nullable disable

[dfTooltip("Used in conjunction with the dfTabContainer class to implement tabbed containers. This control maintains the tabs that are displayed for the user to select, and the dfTabContainer class manages the display of the tab pages themselves.")]
[dfCategory("Basic Controls")]
[AddComponentMenu("Daikon Forge/User Interface/Containers/Tab Control/Tab Strip")]
[ExecuteInEditMode]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_tabstrip.html")]
[Serializable]
public class dfTabstrip : dfControl
    {
        [SerializeField]
        protected dfAtlas atlas;
        [SerializeField]
        protected string backgroundSprite;
        [SerializeField]
        protected RectOffset layoutPadding = new RectOffset();
        [SerializeField]
        protected Vector2 scrollPosition = Vector2.zero;
        [SerializeField]
        protected int selectedIndex;
        [SerializeField]
        protected dfTabContainer pageContainer;
        [SerializeField]
        protected bool allowKeyboardNavigation = true;

        public event PropertyChangedEventHandler<int> SelectedIndexChanged;

        public dfTabContainer TabPages
        {
            get => this.pageContainer;
            set
            {
                if (!((UnityEngine.Object) this.pageContainer != (UnityEngine.Object) value))
                    return;
                this.pageContainer = value;
                if ((UnityEngine.Object) value != (UnityEngine.Object) null)
                {
                    while (value.Controls.Count < this.controls.Count)
                        value.AddTabPage();
                }
                this.pageContainer.SelectedIndex = this.SelectedIndex;
                this.Invalidate();
            }
        }

        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                if (value == this.selectedIndex)
                    return;
                this.selectTabByIndex(value);
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

        public RectOffset LayoutPadding
        {
            get
            {
                if (this.layoutPadding == null)
                    this.layoutPadding = new RectOffset();
                return this.layoutPadding;
            }
            set
            {
                value = value.ConstrainPadding();
                if (object.Equals((object) value, (object) this.layoutPadding))
                    return;
                this.layoutPadding = value;
                this.arrangeTabs();
            }
        }

        public bool AllowKeyboardNavigation
        {
            get => this.allowKeyboardNavigation;
            set => this.allowKeyboardNavigation = value;
        }

        public void EnableTab(int index)
        {
            if (this.selectedIndex < 0 || this.selectedIndex > this.controls.Count - 1)
                return;
            this.controls[index].Enable();
        }

        public void DisableTab(int index)
        {
            if (this.selectedIndex < 0 || this.selectedIndex > this.controls.Count - 1)
                return;
            this.controls[index].Disable();
        }

        public dfControl AddTab(string Text)
        {
            if (Text == null)
                Text = string.Empty;
            dfButton dfButton1 = this.controls.Where((Func<dfControl, bool>) (i => i is dfButton)).FirstOrDefault() as dfButton;
            string str = "Tab " + (object) (this.controls.Count + 1);
            if (string.IsNullOrEmpty(Text))
                Text = str;
            dfButton dfButton2 = this.AddControl<dfButton>();
            dfButton2.name = str;
            dfButton2.Atlas = this.Atlas;
            dfButton2.Text = Text;
            dfButton2.ButtonGroup = (dfControl) this;
            if ((UnityEngine.Object) dfButton1 != (UnityEngine.Object) null)
            {
                dfButton2.Atlas = dfButton1.Atlas;
                dfButton2.Font = dfButton1.Font;
                dfButton2.AutoSize = dfButton1.AutoSize;
                dfButton2.Size = dfButton1.Size;
                dfButton2.BackgroundSprite = dfButton1.BackgroundSprite;
                dfButton2.DisabledSprite = dfButton1.DisabledSprite;
                dfButton2.FocusSprite = dfButton1.FocusSprite;
                dfButton2.HoverSprite = dfButton1.HoverSprite;
                dfButton2.PressedSprite = dfButton1.PressedSprite;
                dfButton2.Shadow = dfButton1.Shadow;
                dfButton2.ShadowColor = dfButton1.ShadowColor;
                dfButton2.ShadowOffset = dfButton1.ShadowOffset;
                dfButton2.TextColor = dfButton1.TextColor;
                dfButton2.TextAlignment = dfButton1.TextAlignment;
                RectOffset padding = dfButton1.Padding;
                dfButton2.Padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom);
            }
            if ((UnityEngine.Object) this.pageContainer != (UnityEngine.Object) null)
                this.pageContainer.AddTabPage();
            this.arrangeTabs();
            this.Invalidate();
            return (dfControl) dfButton2;
        }

        protected internal override void OnGotFocus(dfFocusEventArgs args)
        {
            if (this.controls.Contains(args.GotFocus))
                this.SelectedIndex = args.GotFocus.ZOrder;
            base.OnGotFocus(args);
        }

        protected internal override void OnLostFocus(dfFocusEventArgs args)
        {
            base.OnLostFocus(args);
            if (!this.controls.Contains(args.LostFocus))
                return;
            this.showSelectedTab();
        }

        protected internal override void OnClick(dfMouseEventArgs args)
        {
            if (this.controls.Contains(args.Source))
                this.SelectedIndex = args.Source.ZOrder;
            base.OnClick(args);
        }

        private void OnClick(dfControl sender, dfMouseEventArgs args)
        {
            if (!this.controls.Contains(args.Source))
                return;
            this.SelectedIndex = args.Source.ZOrder;
        }

        protected internal override void OnKeyDown(dfKeyEventArgs args)
        {
            if (args.Used)
                return;
            if (this.allowKeyboardNavigation)
            {
                if (args.KeyCode == KeyCode.LeftArrow || args.KeyCode == KeyCode.Tab && args.Shift)
                {
                    this.SelectedIndex = Mathf.Max(0, this.SelectedIndex - 1);
                    args.Use();
                    return;
                }
                if (args.KeyCode == KeyCode.RightArrow || args.KeyCode == KeyCode.Tab)
                {
                    ++this.SelectedIndex;
                    args.Use();
                    return;
                }
            }
            base.OnKeyDown(args);
        }

        protected internal override void OnControlAdded(dfControl child)
        {
            base.OnControlAdded(child);
            this.attachEvents(child);
            this.arrangeTabs();
        }

        protected internal override void OnControlRemoved(dfControl child)
        {
            base.OnControlRemoved(child);
            this.detachEvents(child);
            this.arrangeTabs();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if ((double) this.size.sqrMagnitude < 1.4012984643248171E-45)
                this.Size = new Vector2(256f, 26f);
            if (!Application.isPlaying)
                return;
            this.selectTabByIndex(Mathf.Max(this.selectedIndex, 0));
        }

        public override void Update()
        {
            base.Update();
            if (this.isControlInvalidated)
                this.arrangeTabs();
            this.showSelectedTab();
        }

        protected internal virtual void OnSelectedIndexChanged()
        {
            this.SignalHierarchy(nameof (OnSelectedIndexChanged), (object) this, (object) this.SelectedIndex);
            if (this.SelectedIndexChanged == null)
                return;
            this.SelectedIndexChanged((dfControl) this, this.SelectedIndex);
        }

        protected override void OnRebuildRenderData()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null || string.IsNullOrEmpty(this.backgroundSprite))
                return;
            dfAtlas.ItemInfo atla = this.Atlas[this.backgroundSprite];
            if (atla == (dfAtlas.ItemInfo) null)
                return;
            this.renderData.Material = this.Atlas.Material;
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

        private void showSelectedTab()
        {
            if (this.selectedIndex < 0 || this.selectedIndex > this.controls.Count - 1)
                return;
            dfButton control = this.controls[this.selectedIndex] as dfButton;
            if (!((UnityEngine.Object) control != (UnityEngine.Object) null) || control.ContainsMouse)
                return;
            control.State = dfButton.ButtonState.Focus;
        }

        private void selectTabByIndex(int value)
        {
            value = Mathf.Max(Mathf.Min(value, this.controls.Count - 1), -1);
            if (value == this.selectedIndex)
                return;
            this.selectedIndex = value;
            for (int index = 0; index < this.controls.Count; ++index)
            {
                dfButton control = this.controls[index] as dfButton;
                if (!((UnityEngine.Object) control == (UnityEngine.Object) null))
                    control.State = index != value ? dfButton.ButtonState.Default : dfButton.ButtonState.Focus;
            }
            this.Invalidate();
            this.OnSelectedIndexChanged();
            if (!((UnityEngine.Object) this.pageContainer != (UnityEngine.Object) null))
                return;
            this.pageContainer.SelectedIndex = value;
        }

        private void arrangeTabs()
        {
            this.SuspendLayout();
            try
            {
                this.layoutPadding = this.layoutPadding.ConstrainPadding();
                float x = (float) this.layoutPadding.left - this.scrollPosition.x;
                float y = (float) this.layoutPadding.top - this.scrollPosition.y;
                float b1 = 0.0f;
                float b2 = 0.0f;
                for (int index = 0; index < this.Controls.Count; ++index)
                {
                    dfControl control = this.controls[index];
                    if (control.IsVisible && control.enabled && control.gameObject.activeSelf)
                    {
                        Vector2 vector2 = new Vector2(x, y);
                        control.RelativePosition = (Vector3) vector2;
                        float a1 = control.Width + (float) this.layoutPadding.horizontal;
                        float a2 = control.Height + (float) this.layoutPadding.vertical;
                        b1 = Mathf.Max(a1, b1);
                        b2 = Mathf.Max(a2, b2);
                        x += a1;
                    }
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        private void attachEvents(dfControl control)
        {
            control.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.control_IsVisibleChanged);
            control.PositionChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
            control.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
            control.ZOrderChanged += new PropertyChangedEventHandler<int>(this.childControlZOrderChanged);
        }

        private void detachEvents(dfControl control)
        {
            control.IsVisibleChanged -= new PropertyChangedEventHandler<bool>(this.control_IsVisibleChanged);
            control.PositionChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
            control.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        }

        private void childControlZOrderChanged(dfControl control, int value)
        {
            this.onChildControlInvalidatedLayout();
        }

        private void control_IsVisibleChanged(dfControl control, bool value)
        {
            this.onChildControlInvalidatedLayout();
        }

        private void childControlInvalidated(dfControl control, Vector2 value)
        {
            this.onChildControlInvalidatedLayout();
        }

        private void onChildControlInvalidatedLayout()
        {
            if (this.IsLayoutSuspended)
                return;
            this.arrangeTabs();
            this.Invalidate();
        }
    }

