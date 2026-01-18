#nullable disable
namespace Dungeonator
{
  public struct CellOcclusionData
  {
    public float cellOcclusion;
    public float minCellOccluionHistory;
    public RuntimeExitDefinition occlusionParentDefintion;
    public int cellRoomVisiblityCount;
    public int cellRoomVisitedCount;
    public float cellVisibleTargetOcclusion;
    public float cellVisitedTargetOcclusion;
    public float remainingDelay;
    public bool cellOcclusionDirty;
    public bool sharedRoomAndExitCell;
    public bool overrideOcclusion;

    public CellOcclusionData(CellData cell)
    {
      cellOcclusion = 1f;
      minCellOccluionHistory = 1f;
      occlusionParentDefintion = (RuntimeExitDefinition) null;
      cellRoomVisiblityCount = 0;
      cellRoomVisitedCount = 0;
      cellVisibleTargetOcclusion = 0.0f;
      cellVisitedTargetOcclusion = 0.7f;
      remainingDelay = 0.0f;
      cellOcclusionDirty = false;
      sharedRoomAndExitCell = false;
      overrideOcclusion = false;
    }
  }
}
