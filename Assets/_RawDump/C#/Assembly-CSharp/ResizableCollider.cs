// Decompiled with JetBrains decompiler
// Type: ResizableCollider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;

#nullable disable
public class ResizableCollider : DungeonPlaceableBehaviour, IPlaceConfigurable, IDwarfDrawable
{
  public bool IsHorizontal = true;
  [DwarfConfigurable]
  public float NumTiles = 3f;
  public tk2dSlicedSprite[] spriteSources;
  private OccupiedCells m_cells;

  public IntVector2 GetOverrideDwarfDimensions(PrototypePlacedObjectData objectData)
  {
    int fieldValueByName = (int) objectData.GetFieldValueByName("NumTiles");
    return this.IsHorizontal ? new IntVector2(fieldValueByName, 1) : new IntVector2(1, fieldValueByName);
  }

  [DebuggerHidden]
  private IEnumerator FrameDelayedConfiguration()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ResizableCollider.\u003CFrameDelayedConfiguration\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  protected override void OnDestroy()
  {
    if (this.m_cells != null)
      this.m_cells.Clear();
    base.OnDestroy();
  }

  public void ConfigureOnPlacement(RoomHandler room)
  {
    IntVector2 intVector2_1 = this.transform.position.IntXY();
    for (int index = 0; (double) index < (double) this.NumTiles; ++index)
    {
      IntVector2 intVector2_2 = intVector2_1 + new IntVector2(0, index);
      if (this.IsHorizontal)
        intVector2_2 = intVector2_1 + new IntVector2(index, 0);
      if (GameManager.Instance.Dungeon.data.CheckInBounds(intVector2_2))
      {
        CellData cellData = GameManager.Instance.Dungeon.data[intVector2_2];
        if (cellData != null)
          cellData.isOccupied = true;
      }
    }
    this.StartCoroutine(this.FrameDelayedConfiguration());
  }
}
