using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Color Picker/Color Preset")]
public class ColorPickerPreset : MonoBehaviour
    {
        public ColorFieldSelector colorField;

        public void OnDragDrop(dfControl control, dfDragEventArgs dragEvent)
        {
            if (!(dragEvent.Data is Color32))
                return;
            control.Color = (Color32) dragEvent.Data;
            dragEvent.State = dfDragDropState.Dropped;
            dragEvent.Use();
        }

        public void OnClick(dfControl control, dfMouseEventArgs mouseEvent)
        {
            if (!((Object) this.colorField != (Object) null))
                return;
            this.colorField.Hue = HSBColor.GetHue((Color) control.Color);
            this.colorField.SelectedColor = (Color) control.Color;
        }
    }

