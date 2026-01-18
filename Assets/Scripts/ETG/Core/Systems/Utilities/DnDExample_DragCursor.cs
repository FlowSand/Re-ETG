// Decompiled with JetBrains decompiler
// Type: DnDExample_DragCursor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Drag and Drop/Drag Cursor")]
public class DnDExample_DragCursor : MonoBehaviour
  {
    private static dfSprite _sprite;
    private static dfLabel _label;

    public void Start()
    {
      DnDExample_DragCursor._sprite = this.GetComponent<dfSprite>();
      DnDExample_DragCursor._sprite.IsVisible = false;
      DnDExample_DragCursor._label = DnDExample_DragCursor._sprite.Find<dfLabel>("Count");
    }

    public void Update()
    {
      if (!DnDExample_DragCursor._sprite.IsVisible)
        return;
      DnDExample_DragCursor.SetPosition((Vector2) Input.mousePosition);
    }

    public static void Show(DndExample_InventoryItem item, Vector2 Position)
    {
      DnDExample_DragCursor.SetPosition(Position);
      DnDExample_DragCursor._sprite.SpriteName = item.Icon;
      DnDExample_DragCursor._sprite.IsVisible = true;
      DnDExample_DragCursor._sprite.BringToFront();
      DnDExample_DragCursor._label.Text = item.Count <= 1 ? string.Empty : item.Count.ToString();
    }

    public static void Hide() => DnDExample_DragCursor._sprite.IsVisible = false;

    public static void SetPosition(Vector2 position)
    {
      position = DnDExample_DragCursor._sprite.GetManager().ScreenToGui(position);
      DnDExample_DragCursor._sprite.RelativePosition = (Vector3) (position - DnDExample_DragCursor._sprite.Size * 0.5f);
    }
  }

