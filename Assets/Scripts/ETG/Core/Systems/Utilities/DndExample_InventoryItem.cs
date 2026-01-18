// Decompiled with JetBrains decompiler
// Type: DndExample_InventoryItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/Drag and Drop/Inventory Item")]
public class DndExample_InventoryItem : MonoBehaviour
  {
    public string ItemName;
    public int Count;
    public string Icon;
    private static dfPanel _container;
    private static dfSprite _sprite;
    private static dfLabel _label;

    public void OnEnable() => this.Refresh();

    public void OnDoubleClick(dfControl source, dfMouseEventArgs args) => this.OnClick(source, args);

    public void OnClick(dfControl source, dfMouseEventArgs args)
    {
      if (string.IsNullOrEmpty(this.ItemName))
        return;
      if (args.Buttons == dfMouseButtons.Left)
        ++this.Count;
      else if (args.Buttons == dfMouseButtons.Right)
        this.Count = Mathf.Max(this.Count - 1, 1);
      this.Refresh();
    }

    public void OnDragStart(dfControl source, dfDragEventArgs args)
    {
      if (this.Count <= 0)
        return;
      args.Data = (object) this;
      args.State = dfDragDropState.Dragging;
      args.Use();
      DnDExample_DragCursor.Show(this, args.Position);
    }

    public void OnDragEnd(dfControl source, dfDragEventArgs args)
    {
      DnDExample_DragCursor.Hide();
      if (args.State != dfDragDropState.Dropped)
        return;
      this.Count = 0;
      this.ItemName = string.Empty;
      this.Icon = string.Empty;
      this.Refresh();
    }

    public void OnDragDrop(dfControl source, dfDragEventArgs args)
    {
      if (this.Count == 0 && args.Data is DndExample_InventoryItem)
      {
        DndExample_InventoryItem data = (DndExample_InventoryItem) args.Data;
        this.ItemName = data.ItemName;
        this.Icon = data.Icon;
        this.Count = data.Count;
        args.State = dfDragDropState.Dropped;
        args.Use();
      }
      this.Refresh();
    }

    private void Refresh()
    {
      DndExample_InventoryItem._container = this.GetComponent<dfPanel>();
      DndExample_InventoryItem._sprite = DndExample_InventoryItem._container.Find("Icon") as dfSprite;
      DndExample_InventoryItem._label = DndExample_InventoryItem._container.Find("Count") as dfLabel;
      DndExample_InventoryItem._sprite.SpriteName = this.Icon;
      DndExample_InventoryItem._label.Text = this.Count <= 1 ? string.Empty : this.Count.ToString();
    }
  }

