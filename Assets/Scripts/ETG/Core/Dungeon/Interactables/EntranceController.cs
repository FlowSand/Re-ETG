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

