// Decompiled with JetBrains decompiler
// Type: ColorFieldSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Color Picker/Color Field Selector")]
public class ColorFieldSelector : MonoBehaviour
  {
    public dfControl indicator;
    public dfControl sliders;
    public dfControl selectedColorDisplay;
    private dfTextureSprite control;
    private Color hue;
    private Color color;

    public Color SelectedColor
    {
      get => this.color;
      set
      {
        this.color = value;
        this.updateHotspot();
      }
    }

    public Color Hue
    {
      get => this.hue;
      set
      {
        this.hue = value;
        this.updateSelectedColor();
      }
    }

    public void Start()
    {
      this.control = this.GetComponent<dfTextureSprite>();
      this.hue = HSBColor.GetHue((Color) this.control.Color);
      this.color = (Color) this.control.Color;
      this.updateHotspot();
    }

    public void Update()
    {
      if ((Object) this.control.RenderMaterial != (Object) null)
        this.control.RenderMaterial.color = this.hue;
      if (!((Object) this.selectedColorDisplay != (Object) null))
        return;
      this.selectedColorDisplay.Color = (Color32) this.color;
    }

    public void OnMouseDown(dfControl control, dfMouseEventArgs mouseEvent)
    {
      this.updateHotspot(mouseEvent);
    }

    public void OnMouseMove(dfControl control, dfMouseEventArgs mouseEvent)
    {
      if (mouseEvent.Buttons != dfMouseButtons.Left)
        return;
      this.updateHotspot(mouseEvent);
    }

    private void updateHotspot()
    {
      if ((Object) this.control == (Object) null)
        return;
      HSBColor hsbColor = HSBColor.FromColor(this.SelectedColor);
      this.indicator.RelativePosition = (Vector3) (new Vector2(hsbColor.s * this.control.Width, (1f - hsbColor.b) * this.control.Height) - this.indicator.Size * 0.5f);
    }

    private void updateHotspot(dfMouseEventArgs mouseEvent)
    {
      if ((Object) this.indicator == (Object) null)
        return;
      this.indicator.RelativePosition = (Vector3) (this.control.GetHitPosition(mouseEvent) - this.indicator.Size * 0.5f);
      this.updateSelectedColor();
    }

    private void updateSelectedColor()
    {
      if ((Object) this.control == (Object) null)
        this.control = this.GetComponent<dfTextureSprite>();
      Vector3 vector3 = this.indicator.RelativePosition + (Vector3) this.indicator.Size * 0.5f;
      this.color = this.getColor(vector3.x, vector3.y, this.control.Width, this.control.Height, this.Hue);
    }

    private Color getColor(float x, float y, float width, float height, Color hue)
    {
      float num1 = x / width;
      float num2 = y / height;
      float t = Mathf.Clamp01(num1);
      float num3 = Mathf.Clamp01(num2);
      return (Color) (Vector4.Lerp((Vector4) Color.white, (Vector4) hue, t) * (1f - num3));
    }
  }

