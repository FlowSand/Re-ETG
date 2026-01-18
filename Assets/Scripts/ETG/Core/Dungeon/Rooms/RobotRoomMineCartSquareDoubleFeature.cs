using UnityEngine;

using Dungeonator;

#nullable disable

public class RobotRoomMineCartSquareDoubleFeature : RobotRoomFeature
    {
        public override bool AcceptableInIdea(
            RobotDaveIdea idea,
            IntVector2 dim,
            bool isInternal,
            int numFeatures)
        {
            return dim.x >= 8 && dim.y >= 8 && idea.UseMineCarts;
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
            int x1 = Random.Range(1, this.LocalDimensions.x / 2 - 2);
            int y1 = Random.Range(1, this.LocalDimensions.y / 2 - 2);
            IntVector2 intVector2 = this.LocalBasePosition + new IntVector2(x1, y1);
            IntVector2 Dimensions = this.LocalDimensions - new IntVector2(2 * x1, 2 * y1);
            SerializedPath rectanglePath = this.GenerateRectanglePath(intVector2, Dimensions);
            rectanglePath.tilesetPathGrid = 0;
            room.paths.Add(rectanglePath);
            DungeonPlaceableBehaviour mineCartPrefab = RobotDave.GetMineCartPrefab();
            this.PlaceObject(mineCartPrefab, room, intVector2, targetObjectLayer).assignedPathIDx = room.paths.Count - 1;
            PrototypePlacedObjectData placedObjectData = this.PlaceObject(mineCartPrefab, room, intVector2 + Dimensions, targetObjectLayer);
            placedObjectData.assignedPathIDx = room.paths.Count - 1;
            placedObjectData.assignedPathStartNode = 2;
        }
    }

