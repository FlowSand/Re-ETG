using Dungeonator;
using UnityEngine;

#nullable disable

public class RobotRoomSurroundingPitFeature : RobotRoomFeature
  {
    public static bool BeenUsed;

    public override void Use()
    {
      RobotRoomSurroundingPitFeature.BeenUsed = true;
      base.Use();
    }

    public override bool AcceptableInIdea(
      RobotDaveIdea idea,
      IntVector2 dim,
      bool isInternal,
      int numFeatures)
    {
      return !Application.isPlaying && !RobotRoomSurroundingPitFeature.BeenUsed && !isInternal && idea.CanIncludePits;
    }

    public override bool CanContainOtherFeature() => true;

    public override int RequiredInsetForOtherFeature() => 2;

    public override void Develop(
      PrototypeDungeonRoom room,
      RobotDaveIdea idea,
      int targetObjectLayer)
    {
      for (int x = this.LocalBasePosition.x; x < this.LocalBasePosition.x + this.LocalDimensions.x; ++x)
      {
        for (int y = this.LocalBasePosition.y; y < this.LocalBasePosition.y + this.LocalDimensions.y; ++y)
          room.ForceGetCellDataAtPoint(x, y).state = CellType.PIT;
      }
      room.RedefineAllPitEntries();
      int num = this.RequiredInsetForOtherFeature();
      for (int ix = this.LocalBasePosition.x + num; ix < this.LocalBasePosition.x + this.LocalDimensions.x - num; ++ix)
      {
        for (int iy = this.LocalBasePosition.y + num; iy < this.LocalBasePosition.y + this.LocalDimensions.y - num; ++iy)
          room.ForceGetCellDataAtPoint(ix, iy).state = CellType.FLOOR;
      }
    }
  }

