// Decompiled with JetBrains decompiler
// Type: EntranceController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class EntranceController : DungeonPlaceableBehaviour, IPlaceConfigurable
{
  public int wallClearanceXStart;
  public int wallClearanceYStart;
  public int wallClearanceWidth = 4;
  public int wallClearanceHeight = 2;
  public Transform spawnTransform;

  public void ConfigureOnPlacement(RoomHandler room)
  {
    IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
    for (int wallClearanceXstart = this.wallClearanceXStart; wallClearanceXstart < this.wallClearanceWidth + this.wallClearanceXStart; ++wallClearanceXstart)
    {
      for (int wallClearanceYstart = this.wallClearanceYStart; wallClearanceYstart < this.wallClearanceHeight + this.wallClearanceYStart; ++wallClearanceYstart)
      {
        CellData cellData = GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(wallClearanceXstart, wallClearanceYstart)];
        cellData.cellVisualData.containsObjectSpaceStamp = true;
        cellData.cellVisualData.containsWallSpaceStamp = true;
        cellData.cellVisualData.shouldIgnoreWallDrawing = true;
      }
    }
  }

  protected override void OnDestroy() => base.OnDestroy();
}
