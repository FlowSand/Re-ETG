// Decompiled with JetBrains decompiler
// Type: FlameTrapChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class FlameTrapChallengeModifier : ChallengeModifier
{
  public DungeonPlaceable FlameTrap;
  public float ChanceToTrap = 0.2f;
  public float TrapChanceDecrementPerFloor = 0.005f;
  private static List<BasicTrapController> m_activeTraps = new List<BasicTrapController>();

  private void Start()
  {
    RoomHandler currentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;
    float num = 0.0f;
    switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
    {
      case GlobalDungeonData.ValidTilesets.GUNGEON:
        num = this.TrapChanceDecrementPerFloor;
        break;
      case GlobalDungeonData.ValidTilesets.MINEGEON:
        num = this.TrapChanceDecrementPerFloor * 2f;
        break;
      case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
        num = this.TrapChanceDecrementPerFloor * 3f;
        break;
      case GlobalDungeonData.ValidTilesets.FORGEGEON:
        num = this.TrapChanceDecrementPerFloor * 4f;
        break;
      case GlobalDungeonData.ValidTilesets.HELLGEON:
        num = this.TrapChanceDecrementPerFloor * 4f;
        break;
    }
    Vector2 centerPosition = GameManager.Instance.BestActivePlayer.CenterPosition;
    for (int x = 0; x < currentRoom.area.dimensions.x; ++x)
    {
      for (int y = 0; y < currentRoom.area.dimensions.y; ++y)
      {
        IntVector2 intVector2 = currentRoom.area.basePosition + new IntVector2(x, y);
        if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2))
        {
          CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
          if ((double) Vector2.Distance(cellData.position.ToCenterVector2(), centerPosition) >= 5.0 && cellData.parentRoom == currentRoom && cellData.type == CellType.FLOOR && !cellData.containsTrap && !cellData.isOccupied && !cellData.isOccludedByTopWall && !cellData.PreventRewardSpawn && (double) Random.value < (double) this.ChanceToTrap - (double) num)
          {
            GameObject gameObject = this.FlameTrap.InstantiateObject(currentRoom, cellData.position - currentRoom.area.basePosition);
            FlameTrapChallengeModifier.m_activeTraps.Add(gameObject.GetComponent<BasicTrapController>());
            Exploder.DoRadialMinorBreakableBreak(cellData.position.ToCenterVector3((float) cellData.position.y), 1f);
            cellData.containsTrap = true;
          }
        }
      }
    }
  }

  private void OnDestroy()
  {
    for (int index = 0; index < FlameTrapChallengeModifier.m_activeTraps.Count; ++index)
    {
      if ((bool) (Object) FlameTrapChallengeModifier.m_activeTraps[index])
        FlameTrapChallengeModifier.m_activeTraps[index].triggerMethod = BasicTrapController.TriggerMethod.Script;
    }
    FlameTrapChallengeModifier.m_activeTraps.Clear();
  }
}
