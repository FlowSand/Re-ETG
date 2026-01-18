using System;

using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[AddComponentMenu("Daikon Forge/User Interface/Containers/Tab Control/Tab Page Container")]
[dfTooltip("Used in conjunction with the dfTabContainer class to implement tabbed containers. This control maintains the tabs that are displayed for the user to select, and the dfTabContainer class manages the display of the tab pages themselves.")]
[dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_tab_container.html")]
[dfCategory("Basic Controls")]
[Serializable]
public class dfTabContainer : dfControl
    {
        [SerializeField]
        protected dfAtlas atlas;
        [SerializeField]
        protected string backgroundSprite;
        [SerializeField]
        protected RectOffset padding = new RectOffset();
        [SerializeField]
        protected int selectedIndex;

        public event PropertyChangedEventHandler<int> SelectedIndexChanged;

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
                this.arrangeTabPages();
            }
        }

        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                if (value == this.selectedIndex)
                    return;
                this.selectPageByIndex(value);
            }
        }

        public dfControl AddTabPage()
        {
            dfPanel dfPanel1 = this.controls.Where((Func<dfControl, bool>) (i => i is dfPanel)).FirstOrDefault() as dfPanel;
            string str = "Tab Page " + (object) (this.controls.Count + 1);
            dfPanel dfPanel2 = this.AddControl<dfPanel>();
            dfPanel2.name = str;
            dfPanel2.Atlas = this.Atlas;
            dfPanel2.Anchor = dfAnchorStyle.All;
            dfPanel2.ClipChildren = true;
            if ((UnityEngine.Object) dfPanel1 != (UnityEngine.Object) null)
            {
                dfPanel2.Atlas = dfPanel1.Atlas;
                dfPanel2.BackgroundSprite = dfPanel1.BackgroundSprite;
            }
            this.arrangeTabPages();
            this.Invalidate();
            return (dfControl) dfPanel2;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if ((double) this.size.sqrMagnitude >= 1.4012984643248171E-45)
                return;
            this.Size = new Vector2(256f, 256f);
        }

        protected internal override void OnControlAdded(dfControl child)
        {
            base.OnControlAdded(child);
            this.attachEvents(child);
            this.arrangeTabPages();
        }

        protected internal override void OnControlRemoved(dfControl child)
        {
            base.OnControlRemoved(child);
            this.detachEvents(child);
            this.arrangeTabPages();
        }

        protected internal virtual void OnSelectedIndexChanged(int Index)
        {
            this.SignalHierarchy(nameof (OnSelectedIndexChanged), (object) this, (object) Index);
            if (this.SelectedIndexChanged == null)
                return;
            this.SelectedIndexChanged((dfControl) this, Index);
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

        private void selectPageByIndex(int value)
        {
            value = Mathf.Max(Mathf.Min(value, this.controls.Count - 1), -1);
            if (value == this.selectedIndex)
                return;
            this.selectedIndex = value;
            for (int index = 0; index < this.controls.Count; ++index)
            {
                dfControl control = this.controls[index];
                if (!((UnityEngine.Object) control == (UnityEngine.Object) null))
                    control.IsVisible = index == value;
            }
            this.arrangeTabPages();
            this.Invalidate();
            this.OnSelectedIndexChanged(value);
        }

        private void arrangeTabPages()
        {
            if (this.padding == null)
                this.padding = new RectOffset(0, 0, 0, 0);
            Vector3 vector3 = new Vector3((float) this.padding.left, (float) this.padding.top);
            Vector2 vector2 = new Vector2(this.size.x - (float) this.padding.horizontal, this.size.y - (float) this.padding.vertical);
            for (int index = 0; index < this.controls.Count; ++index)
            {
                dfPanel control = this.controls[index] as dfPanel;
                if ((UnityEngine.Object) control != (UnityEngine.Object) null)
                {
                    control.Size = vector2;
                    control.RelativePosition = vector3;
                }
            }
        }

        private void attachEvents(dfControl control)
        {
            control.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.control_IsVisibleChanged);
            control.PositionChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
            control.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        }

        private void detachEvents(dfControl control)
        {
            control.IsVisibleChanged -= new PropertyChangedEventHandler<bool>(this.control_IsVisibleChanged);
            control.PositionChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
            control.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
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
            this.arrangeTabPages();
            this.Invalidate();
        }
    }

