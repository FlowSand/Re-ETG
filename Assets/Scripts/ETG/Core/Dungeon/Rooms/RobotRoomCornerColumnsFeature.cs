using Dungeonator;
using UnityEngine;

#nullable disable

public class RobotRoomCornerColumnsFeature : RobotRoomFeature
  {
    public override bool AcceptableInIdea(
      RobotDaveIdea idea,
      IntVector2 dim,
      bool isInternal,
      int numFeatures)
    {
      return !Application.isPlaying && numFeatures < 4 && dim.x >= 8 && dim.y >= 8;
    }

    public override void Develop(
      PrototypeDungeonRoom room,
      RobotDaveIdea idea,
      int targetObjectLayer)
    {
      int num = 2;
      IntVector2 intVector2 = this.LocalBasePosition;
      for (int index1 = 0; index1 < num; ++index1)
      {
        for (int index2 = 0; index2 < num; ++index2)
          room.ForceGetCellDataAtPoint(intVector2.x + index1, intVector2.y + index2).state = CellType.WALL;
      }
      intVector2 = this.LocalBasePosition + new IntVector2(this.LocalDimensions.x - num, 0);
      for (int index3 = 0; index3 < num; ++index3)
      {
        for (int index4 = 0; index4 < num; ++index4)
          room.ForceGetCellDataAtPoint(intVector2.x + index3, intVector2.y + index4).state = CellType.WALL;
      }
      intVector2 = this.LocalBasePosition + new IntVector2(this.LocalDimensions.x - num, this.LocalDimensions.y - num);
      for (int index5 = 0; index5 < num; ++index5)
      {
        for (int index6 = 0; index6 < num; ++index6)
          room.ForceGetCellDataAtPoint(intVector2.x + index5, intVector2.y + index6).state = CellType.WALL;
      }
      intVector2 = this.LocalBasePosition + new IntVector2(0, this.LocalDimensions.y - num);
      for (int index7 = 0; index7 < num; ++index7)
      {
        for (int index8 = 0; index8 < num; ++index8)
          room.ForceGetCellDataAtPoint(intVector2.x + index7, intVector2.y + index8).state = CellType.WALL;
      }
    }
  }

