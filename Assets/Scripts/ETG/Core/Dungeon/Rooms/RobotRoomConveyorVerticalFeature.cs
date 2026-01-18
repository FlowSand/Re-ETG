using UnityEngine;

#nullable disable

public class RobotRoomConveyorVerticalFeature : RobotRoomFeature
  {
    public override bool AcceptableInIdea(
      RobotDaveIdea idea,
      IntVector2 dim,
      bool isInternal,
      int numFeatures)
    {
      return dim.x >= 3 && dim.y >= 3 && idea.UseFloorConveyorBelts;
    }

    public override void Develop(
      PrototypeDungeonRoom room,
      RobotDaveIdea idea,
      int targetObjectLayer)
    {
      PrototypePlacedObjectData placedObjectData = this.PlaceObject(RobotDave.GetVerticalConveyorPrefab(), room, this.LocalBasePosition, targetObjectLayer);
      PrototypePlacedObjectFieldData placedObjectFieldData1 = new PrototypePlacedObjectFieldData();
      placedObjectFieldData1.fieldName = "ConveyorWidth";
      placedObjectFieldData1.fieldType = PrototypePlacedObjectFieldData.FieldType.FLOAT;
      placedObjectFieldData1.floatValue = (float) this.LocalDimensions.x;
      PrototypePlacedObjectFieldData placedObjectFieldData2 = new PrototypePlacedObjectFieldData();
      placedObjectFieldData2.fieldName = "ConveyorHeight";
      placedObjectFieldData2.fieldType = PrototypePlacedObjectFieldData.FieldType.FLOAT;
      placedObjectFieldData2.floatValue = (float) this.LocalDimensions.y;
      PrototypePlacedObjectFieldData placedObjectFieldData3 = new PrototypePlacedObjectFieldData();
      placedObjectFieldData3.fieldName = "VelocityY";
      placedObjectFieldData3.fieldType = PrototypePlacedObjectFieldData.FieldType.FLOAT;
      placedObjectFieldData3.floatValue = (double) Random.value <= 0.5 ? -4f : 4f;
      PrototypePlacedObjectFieldData placedObjectFieldData4 = new PrototypePlacedObjectFieldData();
      placedObjectFieldData4.fieldName = "VelocityX";
      placedObjectFieldData4.fieldType = PrototypePlacedObjectFieldData.FieldType.FLOAT;
      placedObjectFieldData4.floatValue = 0.0f;
      placedObjectData.fieldData.Add(placedObjectFieldData1);
      placedObjectData.fieldData.Add(placedObjectFieldData2);
      placedObjectData.fieldData.Add(placedObjectFieldData3);
      placedObjectData.fieldData.Add(placedObjectFieldData4);
    }
  }

