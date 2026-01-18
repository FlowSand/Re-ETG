using System;
using System.Globalization;

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Color Picker/Color Hex Field")]
public class ColorHexField : MonoBehaviour
    {
        public ColorFieldSelector colorField;
        private dfTextbox control;

        public void Start() => this.control = this.GetComponent<dfTextbox>();

        public void Update()
        {
            if (this.control.HasFocus)
                return;
            Color32 selectedColor = (Color32) this.colorField.SelectedColor;
            this.control.Text = $"{selectedColor.r:X2}{selectedColor.g:X2}{selectedColor.b:X2}";
        }

        public void OnTextSubmitted(dfControl control, string value)
        {
            uint result = 0;
            if (!uint.TryParse(value, NumberStyles.HexNumber, (IFormatProvider) null, out result))
                return;
            Color color = this.UIntToColor(result);
            this.colorField.Hue = HSBColor.GetHue(color);
            this.colorField.SelectedColor = color;
        }

        private Color UIntToColor(uint color)
        {
            byte a = (byte) (color >> 24);
            return (Color) new Color32((byte) (color >> 16 /*0x10*/), (byte) (color >> 8), (byte) (color >> 0), a);
        }
    }

