using System;

using UnityEngine;

#nullable disable

[dfTooltip("Implements a drop-down list control")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_dropdown.html")]
[ExecuteInEditMode]
[dfCategory("Basic Controls")]
[AddComponentMenu("Daikon Forge/User Interface/Dropdown List")]
[Serializable]
public class dfDropdown : dfInteractiveBase, IDFMultiRender, IRendersText
    {
        [SerializeField]
        protected dfFontBase font;
        [SerializeField]
        protected int selectedIndex = -1;
        [SerializeField]
        protected dfControl triggerButton;
        [SerializeField]
        protected Color32 disabledTextColor = (Color32) UnityEngine.Color.gray;
        [SerializeField]
        protected Color32 textColor = (Color32) UnityEngine.Color.white;
        [SerializeField]
        protected float textScale = 1f;
        [SerializeField]
        protected RectOffset textFieldPadding = new RectOffset();
        [SerializeField]
        protected dfDropdown.PopupListPosition listPosition;
        [SerializeField]
        protected int listWidth;
        [SerializeField]
        protected int listHeight = 200;
        [SerializeField]
        protected RectOffset listPadding = new RectOffset();
        [SerializeField]
        protected dfScrollbar listScrollbar;
        [SerializeField]
        protected int itemHeight = 25;
        [SerializeField]
        protected string itemHighlight = string.Empty;
        [SerializeField]
        protected string itemHover = string.Empty;
        [SerializeField]
        protected string listBackground = string.Empty;
        [SerializeField]
        protected Vector2 listOffset = Vector2.zero;
        [SerializeField]
        protected string[] items = new string[0];
        [SerializeField]
        protected bool shadow;
        [SerializeField]
        protected Color32 shadowColor = (Color32) UnityEngine.Color.black;
        [SerializeField]
        protected Vector2 shadowOffset = new Vector2(1f, -1f);
        [SerializeField]
        protected bool openOnMouseDown = true;
        [SerializeField]
        protected bool clickWhenSpacePressed = true;
        private bool eventsAttached;
        private bool isFontCallbackAssigned;
        private dfListbox popup;
        private dfRenderData textRenderData;
        private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();

        public event dfDropdown.PopupEventHandler DropdownOpen;

        public event dfDropdown.PopupEventHandler DropdownClose;

        public event PropertyChangedEventHandler<int> SelectedIndexChanged;

        public bool ClickWhenSpacePressed
        {
            get => this.clickWhenSpacePressed;
            set => this.clickWhenSpacePressed = value;
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
                this.ClosePopup();
                this.unbindTextureRebuildCallback();
                this.font = value;
                this.bindTextureRebuildCallback();
                this.Invalidate();
            }
        }

        public dfScrollbar ListScrollbar
        {
            get => this.listScrollbar;
            set
            {
                if (!((UnityEngine.Object) value != (UnityEngine.Object) this.listScrollbar))
                    return;
                this.listScrollbar = value;
                this.Invalidate();
            }
        }

        public Vector2 ListOffset
        {
            get => this.listOffset;
            set
            {
                if ((double) Vector2.Distance(this.listOffset, value) <= 1.0)
                    return;
                this.listOffset = value;
                this.Invalidate();
            }
        }

        public string ListBackground
        {
            get => this.listBackground;
            set
            {
                if (!(value != this.listBackground))
                    return;
                this.ClosePopup();
                this.listBackground = value;
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

        public string ItemHighlight
        {
            get => this.itemHighlight;
            set
            {
                if (!(value != this.itemHighlight))
                    return;
                this.ClosePopup();
                this.itemHighlight = value;
                this.Invalidate();
            }
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
                this.Invalidate();
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
                if ((UnityEngine.Object) this.popup != (UnityEngine.Object) null)
                    this.popup.SelectedIndex = value;
                this.selectedIndex = value;
                this.OnSelectedIndexChanged();
                this.Invalidate();
            }
        }

        public RectOffset TextFieldPadding
        {
            get
            {
                if (this.textFieldPadding == null)
                    this.textFieldPadding = new RectOffset();
                return this.textFieldPadding;
            }
            set
            {
                value = value.ConstrainPadding();
                if (object.Equals((object) value, (object) this.textFieldPadding))
                    return;
                this.textFieldPadding = value;
                this.Invalidate();
            }
        }

        public Color32 TextColor
        {
            get => this.textColor;
            set
            {
                this.ClosePopup();
                this.textColor = value;
                this.Invalidate();
            }
        }

        public Color32 DisabledTextColor
        {
            get => this.disabledTextColor;
            set
            {
                this.ClosePopup();
                this.disabledTextColor = value;
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
                this.ClosePopup();
                dfFontManager.Invalidate(this.Font);
                this.textScale = value;
                this.Invalidate();
            }
        }

        public int ItemHeight
        {
            get => this.itemHeight;
            set
            {
                value = Mathf.Max(1, value);
                if (value == this.itemHeight)
                    return;
                this.ClosePopup();
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
                this.ClosePopup();
                if (value == null)
                    value = new string[0];
                this.items = value;
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

        public dfDropdown.PopupListPosition ListPosition
        {
            get => this.listPosition;
            set
            {
                if (value == this.ListPosition)
                    return;
                this.ClosePopup();
                this.listPosition = value;
                this.Invalidate();
            }
        }

        public int MaxListWidth
        {
            get => this.listWidth;
            set => this.listWidth = value;
        }

        public int MaxListHeight
        {
            get => this.listHeight;
            set
            {
                this.listHeight = value;
                this.Invalidate();
            }
        }

        public dfControl TriggerButton
        {
            get => this.triggerButton;
            set
            {
                if (!((UnityEngine.Object) value != (UnityEngine.Object) this.triggerButton))
                    return;
                this.detachChildEvents();
                this.triggerButton = value;
                this.attachChildEvents();
                this.Invalidate();
            }
        }

        public bool OpenOnMouseDown
        {
            get => this.openOnMouseDown;
            set => this.openOnMouseDown = value;
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

        protected internal override void OnMouseWheel(dfMouseEventArgs args)
        {
            this.SelectedIndex = Mathf.Max(0, this.SelectedIndex - Mathf.RoundToInt(args.WheelDelta));
            args.Use();
            base.OnMouseWheel(args);
        }

        protected internal override void OnMouseDown(dfMouseEventArgs args)
        {
            if (this.openOnMouseDown && !args.Used && args.Buttons == dfMouseButtons.Left && (UnityEngine.Object) args.Source == (UnityEngine.Object) this)
            {
                args.Use();
                base.OnMouseDown(args);
                if ((UnityEngine.Object) this.popup == (UnityEngine.Object) null)
                    this.OpenPopup();
                else
                    this.ClosePopup();
            }
            else
                base.OnMouseDown(args);
        }

        protected internal override void OnKeyDown(dfKeyEventArgs args)
        {
            KeyCode keyCode = args.KeyCode;
            switch (keyCode)
            {
                case KeyCode.UpArrow:
                    this.SelectedIndex = Mathf.Max(0, this.selectedIndex - 1);
                    break;
                case KeyCode.DownArrow:
                    this.SelectedIndex = Mathf.Min(this.items.Length - 1, this.selectedIndex + 1);
                    break;
                case KeyCode.Home:
                    this.SelectedIndex = 0;
                    break;
                case KeyCode.End:
                    this.SelectedIndex = this.items.Length - 1;
                    break;
                default:
                    if ((keyCode == KeyCode.Return || keyCode == KeyCode.Space) && this.ClickWhenSpacePressed && this.IsInteractive)
                    {
                        this.OpenPopup();
                        break;
                    }
                    break;
            }
            base.OnKeyDown(args);
        }

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
            this.ClosePopup(false);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.ClosePopup(false);
            this.detachChildEvents();
        }

        public override void Update()
        {
            base.Update();
            this.checkForPopupClose();
        }

        private void checkForPopupClose()
        {
            if ((UnityEngine.Object) this.popup == (UnityEngine.Object) null || !Input.GetMouseButtonDown(0))
                return;
            Camera camera = this.GetCamera();
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if ((UnityEngine.Object) this.triggerButton != (UnityEngine.Object) null && this.triggerButton.GetComponent<Collider>().Raycast(ray, out hitInfo, camera.farClipPlane) || this.popup.GetComponent<Collider>().Raycast(ray, out hitInfo, camera.farClipPlane) || (UnityEngine.Object) this.popup.Scrollbar != (UnityEngine.Object) null && this.popup.Scrollbar.GetComponent<Collider>().Raycast(ray, out hitInfo, camera.farClipPlane) || this.GetComponent<Collider>().Raycast(ray, out hitInfo, camera.farClipPlane))
                return;
            this.ClosePopup();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            if (!Application.isPlaying)
                return;
            if (!this.eventsAttached)
                this.attachChildEvents();
            if (!((UnityEngine.Object) this.popup != (UnityEngine.Object) null) || this.popup.ContainsFocus)
                return;
            this.ClosePopup();
        }

        private void attachChildEvents()
        {
            if (!((UnityEngine.Object) this.triggerButton != (UnityEngine.Object) null) || this.eventsAttached)
                return;
            this.eventsAttached = true;
            this.triggerButton.Click += new MouseEventHandler(this.trigger_Click);
        }

        private void detachChildEvents()
        {
            if (!((UnityEngine.Object) this.triggerButton != (UnityEngine.Object) null) || !this.eventsAttached)
                return;
            this.triggerButton.Click -= new MouseEventHandler(this.trigger_Click);
            this.eventsAttached = false;
        }

        private void trigger_Click(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (!((UnityEngine.Object) mouseEvent.Source == (UnityEngine.Object) this.triggerButton) || mouseEvent.Used)
                return;
            mouseEvent.Use();
            if ((UnityEngine.Object) this.popup == (UnityEngine.Object) null)
                this.OpenPopup();
            else
                this.ClosePopup();
        }

        protected internal virtual void OnSelectedIndexChanged()
        {
            this.SignalHierarchy(nameof (OnSelectedIndexChanged), (object) this, (object) this.selectedIndex);
            if (this.SelectedIndexChanged == null)
                return;
            this.SelectedIndexChanged((dfControl) this, this.selectedIndex);
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

        private void renderText(dfRenderData buffer)
        {
            if (this.selectedIndex < 0 || this.selectedIndex >= this.items.Length)
                return;
            string text = this.items[this.selectedIndex];
            float units = this.PixelsToUnits();
            Vector2 vector2 = new Vector2(this.size.x - (float) this.textFieldPadding.horizontal, this.size.y - (float) this.textFieldPadding.vertical);
            Vector3 upperLeft = this.pivot.TransformToUpperLeft(this.Size);
            Vector3 vector3 = new Vector3(upperLeft.x + (float) this.textFieldPadding.left, upperLeft.y - (float) this.textFieldPadding.top, 0.0f) * units;
            Color32 color32 = !this.IsEnabled ? this.DisabledTextColor : this.TextColor;
            using (dfFontRendererBase renderer = this.font.ObtainRenderer())
            {
                renderer.WordWrap = false;
                renderer.MaxSize = vector2;
                renderer.PixelRatio = units;
                renderer.TextScale = this.TextScale;
                renderer.VectorOffset = vector3;
                renderer.MultiLine = false;
                renderer.TextAlign = TextAlignment.Left;
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
                    dynamicFontRenderer.SpriteBuffer = buffer;
                }
                renderer.Render(text, buffer);
            }
        }

        public void AddItem(string item)
        {
            string[] destinationArray = new string[this.items.Length + 1];
            Array.Copy((Array) this.items, (Array) destinationArray, this.items.Length);
            destinationArray[this.items.Length] = item;
            this.items = destinationArray;
        }

        public void OpenPopup()
        {
            if ((UnityEngine.Object) this.popup != (UnityEngine.Object) null || this.items.Length == 0)
                return;
            Vector2 popupSize = this.calculatePopupSize();
            this.popup = this.GetManager().AddControl<dfListbox>();
            this.popup.name = this.name + " - Dropdown List";
            this.popup.gameObject.hideFlags = HideFlags.DontSave;
            this.popup.Atlas = this.Atlas;
            this.popup.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Left;
            this.popup.Color = this.Color;
            this.popup.Font = this.Font;
            this.popup.Pivot = dfPivotPoint.TopLeft;
            this.popup.Size = popupSize;
            this.popup.Font = this.Font;
            this.popup.ItemHeight = this.ItemHeight;
            this.popup.ItemHighlight = this.ItemHighlight;
            this.popup.ItemHover = this.ItemHover;
            this.popup.ItemPadding = this.TextFieldPadding;
            this.popup.ItemTextColor = this.TextColor;
            this.popup.ItemTextScale = this.TextScale;
            this.popup.Items = this.Items;
            this.popup.ListPadding = this.ListPadding;
            this.popup.BackgroundSprite = this.ListBackground;
            this.popup.Shadow = this.Shadow;
            this.popup.ShadowColor = this.ShadowColor;
            this.popup.ShadowOffset = this.ShadowOffset;
            this.popup.BringToFront();
            if ((UnityEngine.Object) dfGUIManager.GetModalControl() != (UnityEngine.Object) null)
                dfGUIManager.PushModal((dfControl) this.popup);
            if ((double) popupSize.y >= (double) this.MaxListHeight && (UnityEngine.Object) this.listScrollbar != (UnityEngine.Object) null)
            {
                dfScrollbar activeScrollbar = UnityEngine.Object.Instantiate<GameObject>(this.listScrollbar.gameObject).GetComponent<dfScrollbar>();
                float units = this.PixelsToUnits();
                Vector3 vector3 = this.popup.transform.position + this.popup.transform.TransformDirection(Vector3.right) * (popupSize.x - activeScrollbar.Width) * units;
                this.popup.AddControl((dfControl) activeScrollbar);
                this.popup.Width -= activeScrollbar.Width;
                this.popup.Scrollbar = activeScrollbar;
                this.popup.SizeChanged += (PropertyChangedEventHandler<Vector2>) ((control, size) => activeScrollbar.Height = control.Height);
                activeScrollbar.transform.parent = this.popup.transform;
                activeScrollbar.transform.position = vector3;
                activeScrollbar.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom;
                activeScrollbar.Height = this.popup.Height;
            }
            this.popup.transform.position = this.calculatePopupPosition((int) this.popup.Size.y);
            this.popup.transform.rotation = this.transform.rotation;
            this.popup.SelectedIndexChanged += new PropertyChangedEventHandler<int>(this.popup_SelectedIndexChanged);
            this.popup.LeaveFocus += new FocusEventHandler(this.popup_LostFocus);
            this.popup.ItemClicked += new PropertyChangedEventHandler<int>(this.popup_ItemClicked);
            this.popup.KeyDown += new KeyPressHandler(this.popup_KeyDown);
            this.popup.SelectedIndex = Mathf.Max(0, this.SelectedIndex);
            this.popup.EnsureVisible(this.popup.SelectedIndex);
            this.popup.Focus(true);
            if (this.DropdownOpen != null)
            {
                bool overridden = false;
                this.DropdownOpen(this, this.popup, ref overridden);
            }
            this.Signal("OnDropdownOpen", (object) this, (object) this.popup);
        }

        public void ClosePopup() => this.ClosePopup(true);

        public void ClosePopup(bool allowOverride)
        {
            if ((UnityEngine.Object) this.popup == (UnityEngine.Object) null)
                return;
            if ((UnityEngine.Object) dfGUIManager.GetModalControl() == (UnityEngine.Object) this.popup)
                dfGUIManager.PopModal();
            this.popup.LostFocus -= new FocusEventHandler(this.popup_LostFocus);
            this.popup.SelectedIndexChanged -= new PropertyChangedEventHandler<int>(this.popup_SelectedIndexChanged);
            this.popup.ItemClicked -= new PropertyChangedEventHandler<int>(this.popup_ItemClicked);
            this.popup.KeyDown -= new KeyPressHandler(this.popup_KeyDown);
            if (!allowOverride)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object) this.popup.gameObject);
                this.popup = (dfListbox) null;
            }
            else
            {
                bool overridden = false;
                if (this.DropdownClose != null)
                    this.DropdownClose(this, this.popup, ref overridden);
                if (!overridden)
                    this.Signal("OnDropdownClose", (object) this, (object) this.popup);
                if (!overridden)
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.popup.gameObject);
                this.popup = (dfListbox) null;
            }
        }

        private Vector3 calculatePopupPosition(int height)
        {
            float units = this.PixelsToUnits();
            Vector3 vector3_1 = this.transform.position + this.pivot.TransformToUpperLeft(this.size) * units;
            Vector3 scaledDirection = this.getScaledDirection(Vector3.down);
            Vector3 vector3_2 = this.transformOffset((Vector3) this.listOffset);
            Vector3 popupPosition1 = vector3_1 + (vector3_2 + scaledDirection * this.Size.y) * units;
            Vector3 popupPosition2 = vector3_1 + (vector3_2 - scaledDirection * this.popup.Size.y) * units;
            if (this.listPosition == dfDropdown.PopupListPosition.Above)
                return popupPosition2;
            if (this.listPosition == dfDropdown.PopupListPosition.Below)
                return popupPosition1;
            Vector2 screenSize = this.GetManager().GetScreenSize();
            return (double) (this.GetAbsolutePosition().y + this.Height + (float) height) >= (double) screenSize.y ? popupPosition2 : popupPosition1;
        }

        private Vector2 calculatePopupSize()
        {
            float x = this.MaxListWidth <= 0 ? this.size.x : (float) this.MaxListWidth;
            int b = this.items.Length * this.itemHeight + this.listPadding.vertical;
            if (this.items.Length == 0)
                b = this.itemHeight / 2 + this.listPadding.vertical;
            return new Vector2(x, (float) Mathf.Min(this.MaxListHeight, b));
        }

        private void popup_KeyDown(dfControl control, dfKeyEventArgs args)
        {
            if (args.KeyCode != KeyCode.Escape && args.KeyCode != KeyCode.Return)
                return;
            this.ClosePopup();
            this.Focus(true);
        }

        private void popup_ItemClicked(dfControl control, int selectedIndex) => this.Focus(true);

        private void popup_LostFocus(dfControl control, dfFocusEventArgs args)
        {
            if (!((UnityEngine.Object) this.popup != (UnityEngine.Object) null) || this.popup.ContainsFocus)
                return;
            this.ClosePopup();
        }

        private void popup_SelectedIndexChanged(dfControl control, int selectedIndex)
        {
            this.SelectedIndex = selectedIndex;
            this.Invalidate();
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
            if (!this.isControlInvalidated)
            {
                for (int index = 0; index < this.buffers.Count; ++index)
                    this.buffers[index].Transform = this.transform.localToWorldMatrix;
                return this.buffers;
            }
            this.buffers.Clear();
            this.renderData.Clear();
            this.renderData.Material = this.Atlas.Material;
            this.renderData.Transform = this.transform.localToWorldMatrix;
            this.buffers.Add(this.renderData);
            this.textRenderData.Clear();
            this.textRenderData.Material = this.Atlas.Material;
            this.textRenderData.Transform = this.transform.localToWorldMatrix;
            this.buffers.Add(this.textRenderData);
            this.renderBackground();
            this.renderText(this.textRenderData);
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
            if ((UnityEngine.Object) font == (UnityEngine.Object) null || !dfFontManager.IsDirty(this.Font))
                return;
            string selectedValue = this.SelectedValue;
            if (string.IsNullOrEmpty(selectedValue))
                return;
            int fontSize = Mathf.CeilToInt((float) this.font.FontSize * this.TextScale);
            font.AddCharacterRequest(selectedValue, fontSize, FontStyle.Normal);
        }

        private void onFontTextureRebuilt(UnityEngine.Font font)
        {
            if (!(this.Font is dfDynamicFont) || !((UnityEngine.Object) font == (UnityEngine.Object) (this.Font as dfDynamicFont).BaseFont))
                return;
            this.requestCharacterInfo();
            this.Invalidate();
        }

        public void UpdateFontInfo() => this.requestCharacterInfo();

        public enum PopupListPosition
        {
            Below,
            Above,
            Automatic,
        }

        [dfEventCategory("Popup")]
        public delegate void PopupEventHandler(dfDropdown dropdown, dfListbox popup, ref bool overridden);
    }

