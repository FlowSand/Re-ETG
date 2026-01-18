// Decompiled with JetBrains decompiler
// Type: RGBSliders
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Color Picker/RGB Sliders Container")]
public class RGBSliders : MonoBehaviour
  {
    public ColorFieldSelector colorField;
    public dfSlider redSlider;
    public dfSlider greenSlider;
    public dfSlider blueSlider;
    private dfPanel container;
    private Color color;
    private Color hue;

    public Color SelectedColor
    {
      get => this.color;
      set
      {
        this.color = value;
        this.updateSliders();
      }
    }

    public Color Hue
    {
      get => this.hue;
      set => this.hue = value;
    }

    public void Start() => this.container = this.GetComponent<dfPanel>();

    public void Update()
    {
      if (this.container.ContainsFocus)
        return;
      this.SelectedColor = this.colorField.SelectedColor;
    }

    public void OnValueChanged(dfControl source, float value)
    {
      if (!this.container.ContainsFocus)
        return;
      this.color = new Color(this.redSlider.Value, this.greenSlider.Value, this.blueSlider.Value);
      this.colorField.Hue = this.hue = HSBColor.GetHue(this.color);
      this.colorField.SelectedColor = this.color;
    }

    private void updateSliders()
    {
      this.redSlider.Value = this.color.r;
      this.greenSlider.Value = this.color.g;
      this.blueSlider.Value = this.color.b;
    }
  }

