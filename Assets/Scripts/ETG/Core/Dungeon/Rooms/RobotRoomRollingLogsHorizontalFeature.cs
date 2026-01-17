// Decompiled with JetBrains decompiler
// Type: RobotRoomRollingLogsHorizontalFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

namespace ETG.Core.Dungeon.Rooms
{
    public class RobotRoomRollingLogsHorizontalFeature : RobotRoomFeature
    {
      public override bool AcceptableInIdea(
        RobotDaveIdea idea,
        IntVector2 dim,
        bool isInternal,
        int numFeatures)
      {
        return idea.UseRollingLogsHorizontal && dim.x > 6 && dim.y > 6;
      }

      public override void Develop(
        PrototypeDungeonRoom room,
        RobotDaveIdea idea,
        int targetObjectLayer)
      {
        DungeonPlaceableBehaviour rollingLogHorizontal = RobotDave.GetRollingLogHorizontal();
        SerializedPath horizontalPath = this.GenerateHorizontalPath(this.LocalBasePosition, new IntVector2(this.LocalDimensions.x - (rollingLogHorizontal.GetWidth() - 1), this.LocalDimensions.y));
        room.paths.Add(horizontalPath);
        PrototypePlacedObjectData placedObjectData = this.PlaceObject(rollingLogHorizontal, room, this.LocalBasePosition, targetObjectLayer);
        placedObjectData.assignedPathIDx = room.paths.Count - 1;
        placedObjectData.fieldData.Add(new PrototypePlacedObjectFieldData()
        {
          fieldName = "NumTiles",
          fieldType = PrototypePlacedObjectFieldData.FieldType.FLOAT,
          floatValue = (float) this.LocalDimensions.y
        });
      }
    }

}
