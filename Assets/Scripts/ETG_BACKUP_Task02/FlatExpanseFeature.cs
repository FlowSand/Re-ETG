// Decompiled with JetBrains decompiler
// Type: FlatExpanseFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
