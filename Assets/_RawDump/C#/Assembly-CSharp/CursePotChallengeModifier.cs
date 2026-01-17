// Decompiled with JetBrains decompiler
// Type: CursePotChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using UnityEngine;

#nullable disable
public class CursePotChallengeModifier : ChallengeModifier
{
  public DungeonPlaceable CursePot;
  public int RoughAreaPerCursePot = 50;

  private void Start()
  {
    RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
    int num = Mathf.Max(1, currentRoom.CellsWithoutExits.Count / this.RoughAreaPerCursePot);
    CellValidator cellValidator = (CellValidator) (pos =>
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if ((double) Vector2.Distance(GameManager.Instance.AllPlayers[index].CenterPosition, pos.ToCenterVector2()) < 8.0)
          return false;
      }
      return true;
    });
    for (int index = 0; index < num; ++index)
    {
      IntVector2? randomAvailableCell = currentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR), cellValidator: cellValidator);
      if (randomAvailableCell.HasValue)
      {
        CellData cellData = GameManager.Instance.Dungeon.data[randomAvailableCell.Value];
        if (cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && !cellData.isOccupied && !cellData.containsTrap && !cellData.isOccludedByTopWall)
        {
          cellData.containsTrap = true;
          this.CursePot.InstantiateObject(currentRoom, cellData.position - currentRoom.area.basePosition);
        }
      }
    }
  }
}
