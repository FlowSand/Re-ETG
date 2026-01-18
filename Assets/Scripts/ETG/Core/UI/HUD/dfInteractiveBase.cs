using System;

using UnityEngine;

#nullable disable

[ExecuteInEditMode]
[Serializable]
public class dfInteractiveBase : dfControl
    {
        [SerializeField]
        protected dfAtlas atlas;
        [SerializeField]
        protected string backgroundSprite;
        [SerializeField]
        protected string hoverSprite;
        [SerializeField]
        protected string disabledSprite;
        [SerializeField]
        protected string focusSprite;

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

        public string DisabledSprite
        {
            get => this.disabledSprite;
            set
            {
                if (!(value != this.disabledSprite))
                    return;
                this.disabledSprite = value;
                this.Invalidate();
            }
        }

        public string FocusSprite
        {
            get => this.focusSprite;
            set
            {
                if (!(value != this.focusSprite))
                    return;
                this.focusSprite = value;
                this.Invalidate();
            }
        }

        public string HoverSprite
        {
            get => this.hoverSprite;
            set
            {
                if (!(value != this.hoverSprite))
                    return;
                this.hoverSprite = value;
                this.Invalidate();
            }
        }

        public override bool CanFocus => this.IsEnabled && this.IsVisible || base.CanFocus;

        protected internal override void OnGotFocus(dfFocusEventArgs args)
        {
            base.OnGotFocus(args);
            this.Invalidate();
        }

        protected internal override void OnLostFocus(dfFocusEventArgs args)
        {
            base.OnLostFocus(args);
            this.Invalidate();
        }

        protected internal override void OnMouseEnter(dfMouseEventArgs args)
        {
            base.OnMouseEnter(args);
            this.Invalidate();
        }

        protected internal override void OnMouseLeave(dfMouseEventArgs args)
        {
            base.OnMouseLeave(args);
            this.Invalidate();
        }

        public override Vector2 CalculateMinimumSize()
        {
            dfAtlas.ItemInfo backgroundSprite = this.getBackgroundSprite();
            if (backgroundSprite == (dfAtlas.ItemInfo) null)
                return base.CalculateMinimumSize();
            RectOffset border = backgroundSprite.border;
            return border.horizontal > 0 || border.vertical > 0 ? Vector2.Max(base.CalculateMinimumSize(), new Vector2((float) border.horizontal, (float) border.vertical)) : base.CalculateMinimumSize();
        }

        protected internal virtual void renderBackground()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return;
            dfAtlas.ItemInfo backgroundSprite = this.getBackgroundSprite();
            if (backgroundSprite == (dfAtlas.ItemInfo) null)
                return;
            Color32 color32 = this.ApplyOpacity(this.getActiveColor());
            dfSprite.RenderOptions options = new dfSprite.RenderOptions()
            {
                atlas = this.atlas,
                color = color32,
                fillAmount = 1f,
                flip = dfSpriteFlip.None,
                offset = this.pivot.TransformToUpperLeft(this.Size),
                pixelsToUnits = this.PixelsToUnits(),
                size = this.Size,
                spriteInfo = backgroundSprite
            };
            if (backgroundSprite.border.horizontal == 0 && backgroundSprite.border.vertical == 0)
                dfSprite.renderSprite(this.renderData, options);
            else
                dfSlicedSprite.renderSprite(this.renderData, options);
        }

        protected virtual Color32 getActiveColor()
        {
            if (this.IsEnabled)
                return this.color;
            return !string.IsNullOrEmpty(this.disabledSprite) && (UnityEngine.Object) this.Atlas != (UnityEngine.Object) null && this.Atlas[this.DisabledSprite] != (dfAtlas.ItemInfo) null ? this.color : this.disabledColor;
        }

        protected internal virtual dfAtlas.ItemInfo getBackgroundSprite()
        {
            if ((UnityEngine.Object) this.Atlas == (UnityEngine.Object) null)
                return (dfAtlas.ItemInfo) null;
            if (!this.IsEnabled)
            {
                dfAtlas.ItemInfo atla = this.atlas[this.DisabledSprite];
                return atla != (dfAtlas.ItemInfo) null ? atla : this.atlas[this.BackgroundSprite];
            }
            if (this.HasFocus)
            {
                dfAtlas.ItemInfo atla = this.atlas[this.FocusSprite];
                return atla != (dfAtlas.ItemInfo) null ? atla : this.atlas[this.BackgroundSprite];
            }
            if (this.isMouseHovering)
            {
                dfAtlas.ItemInfo atla = this.atlas[this.HoverSprite];
                if (atla != (dfAtlas.ItemInfo) null)
                    return atla;
            }
            return this.Atlas[this.BackgroundSprite];
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

