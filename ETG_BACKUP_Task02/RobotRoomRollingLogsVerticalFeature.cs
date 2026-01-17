// Decompiled with JetBrains decompiler
// Type: RobotRoomRollingLogsVerticalFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public class RobotRoomRollingLogsVerticalFeature : RobotRoomFeature
{
  public override bool AcceptableInIdea(
    RobotDaveIdea idea,
    IntVector2 dim,
    bool isInternal,
    int numFeatures)
  {
    return idea.UseRollingLogsVertical && dim.x > 6 && dim.y > 6;
  }

  public override void Develop(
    PrototypeDungeonRoom room,
    RobotDaveIdea idea,
    int targetObjectLayer)
  {
    DungeonPlaceableBehaviour rollingLogVertical = RobotDave.GetRollingLogVertical();
    SerializedPath verticalPath = this.GenerateVerticalPath(this.LocalBasePosition, new IntVector2(this.LocalDimensions.x, this.LocalDimensions.y - (rollingLogVertical.GetHeight() - 1)));
    room.paths.Add(verticalPath);
    PrototypePlacedObjectData placedObjectData = this.PlaceObject(rollingLogVertical, room, this.LocalBasePosition, targetObjectLayer);
    placedObjectData.assignedPathIDx = room.paths.Count - 1;
    placedObjectData.fieldData.Add(new PrototypePlacedObjectFieldData()
    {
      fieldName = "NumTiles",
      fieldType = PrototypePlacedObjectFieldData.FieldType.FLOAT,
      floatValue = (float) this.LocalDimensions.x
    });
  }
}
