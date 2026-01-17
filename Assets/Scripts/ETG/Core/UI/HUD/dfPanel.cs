// Decompiled with JetBrains decompiler
// Type: dfPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.HUD
{
    [dfCategory("Basic Controls")]
    [ExecuteInEditMode]
    [dfHelp("http://www.daikonforge.com/docs/df-gui/classdf_panel.html")]
    [dfTooltip("Basic container control to facilitate user interface layout")]
    [AddComponentMenu("Daikon Forge/User Interface/Containers/Panel")]
    [Serializable]
    public class dfPanel : dfControl
    {
      [SerializeField]
      protected dfAtlas atlas;
      [SerializeField]
      protected string backgroundSprite;
      [SerializeField]
      protected Color32 backgroundColor = (Color32) UnityEngine.Color.white;
      [SerializeField]
      protected RectOffset padding = new RectOffset();

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
          value = this.getLocalizedValue(value);
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
          this.Invalidate();
        }
      }

      protected internal override void OnLocalize()
      {
        base.OnLocalize();
        this.BackgroundSprite = this.getLocalizedValue(this.backgroundSprite);
      }

      protected internal override RectOffset GetClipPadding()
      {
        return this.padding ?? dfRectOffsetExtensions.Empty;
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
        RectOffset padding = this.Padding;
        corners[0] += inNormal1 * (float) padding.left * units + inNormal4 * (float) padding.top * units;
        corners[1] += inNormal2 * (float) padding.right * units + inNormal4 * (float) padding.top * units;
        corners[2] += inNormal1 * (float) padding.left * units + inNormal3 * (float) padding.bottom * units;
        return new Plane[4]
        {
          new Plane(inNormal1, corners[0]),
          new Plane(inNormal2, corners[1]),
          new Plane(inNormal3, corners[2]),
          new Plane(inNormal4, corners[0])
        };
      }

      public override void OnEnable()
      {
        base.OnEnable();
        if (!(this.size == Vector2.zero))
          return;
        this.SuspendLayout();
        Camera camera = this.GetCamera();
        this.Size = (Vector2) new Vector3((float) (camera.pixelWidth / 2), (float) (camera.pixelHeight / 2));
        this.ResumeLayout();
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
        this.Size = lhs + new Vector2((float) this.padding.right, (float) this.padding.bottom);
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
    }

}
