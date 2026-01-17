// Decompiled with JetBrains decompiler
// Type: ColorPickerDrag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[AddComponentMenu("Daikon Forge/Examples/Color Picker/Drag and Drop")]
public class ColorPickerDrag : MonoBehaviour
{
  private Texture2D dragTexture;
  private dfSlicedSprite control;
  private bool isDragging;

  public void Start() => this.control = this.GetComponent<dfSlicedSprite>();

  private void OnGUI()
  {
    if (!Application.isPlaying || !this.isDragging)
      return;
    if ((Object) this.dragTexture == (Object) null)
    {
      this.dragTexture = new Texture2D(2, 2);
      this.dragTexture.SetPixel(0, 0, Color.white);
      this.dragTexture.SetPixel(0, 1, Color.white);
      this.dragTexture.SetPixel(1, 0, Color.white);
      this.dragTexture.SetPixel(1, 1, Color.white);
      this.dragTexture.Apply();
    }
    Vector3 mousePosition = Input.mousePosition;
    Rect position = new Rect(mousePosition.x - 15f, (float) ((double) Screen.height - (double) mousePosition.y - 5.0), 30f, 15f);
    Color color = GUI.color;
    GUI.color = (Color) this.control.Color;
    GUI.DrawTexture(position, (Texture) this.dragTexture);
    GUI.color = color;
  }

  public void OnDragStart(dfControl control, dfDragEventArgs dragEvent)
  {
    this.isDragging = true;
    dragEvent.Data = (object) control.Color;
    dragEvent.State = dfDragDropState.Dragging;
    dragEvent.Use();
  }

  public void OnDragEnd(dfControl source, dfDragEventArgs args) => this.isDragging = false;
}
