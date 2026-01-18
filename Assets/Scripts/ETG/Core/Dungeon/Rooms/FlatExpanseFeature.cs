using Dungeonator;

#nullable disable

public class FlatExpanseFeature : RobotRoomFeature
  {
    public override bool CanContainOtherFeature() => true;

    public override int RequiredInsetForOtherFeature() => 2;

    public override bool AcceptableInIdea(
      RobotDaveIdea idea,
      IntVector2 dim,
      bool isInternal,
      int numFeatures)
    {
      return true;
    }

    public override void Develop(
      PrototypeDungeonRoom room,
      RobotDaveIdea idea,
      int targetObjectLayer)
    {
      for (int x = this.LocalBasePosition.x; x < this.LocalBasePosition.x + this.LocalDimensions.x; ++x)
      {
        for (int y = this.LocalBasePosition.y; y < this.LocalBasePosition.y + this.LocalDimensions.y; ++y)
          room.ForceGetCellDataAtPoint(x, y).state = CellType.FLOOR;
      }
    }
  }

