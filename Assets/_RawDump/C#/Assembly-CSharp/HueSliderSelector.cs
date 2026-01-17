// Decompiled with JetBrains decompiler
// Type: HueSliderSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Color Picker/Hue Slider")]
public class HueSliderSelector : MonoBehaviour
{
  private dfSlider slider;
  private Color hue;

  public Color Hue
  {
    get => this.hue;
    set
    {
      if (object.Equals((object) value, (object) this.hue))
        return;
      this.hue = value;
      if (!((Object) this.slider != (Object) null))
        return;
      this.slider.Value = HSBColor.FromColor(value).h;
    }
  }

  public void Start() => this.slider = this.GetComponent<dfSlider>();

  public void OnValueChanged(dfControl control, float value)
  {
    this.hue = new HSBColor(value, 1f, 1f, 1f).ToColor();
  }
}
