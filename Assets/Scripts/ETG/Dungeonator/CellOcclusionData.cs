// Decompiled with JetBrains decompiler
// Type: Dungeonator.CellOcclusionData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace Dungeonator;

public struct CellOcclusionData(CellData cell)
{
  public float cellOcclusion = 1f;
  public float minCellOccluionHistory = 1f;
  public RuntimeExitDefinition occlusionParentDefintion = (RuntimeExitDefinition) null;
  public int cellRoomVisiblityCount = 0;
  public int cellRoomVisitedCount = 0;
  public float cellVisibleTargetOcclusion = 0.0f;
  public float cellVisitedTargetOcclusion = 0.7f;
  public float remainingDelay = 0.0f;
  public bool cellOcclusionDirty = false;
  public bool sharedRoomAndExitCell = false;
  public bool overrideOcclusion = false;
}
