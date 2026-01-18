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

