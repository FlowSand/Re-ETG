using UnityEngine;

using Dungeonator;

#nullable disable

public class RobotRoomColumnFeature : RobotRoomFeature
    {
        public override bool AcceptableInIdea(
            RobotDaveIdea idea,
            IntVector2 dim,
            bool isInternal,
            int numFeatures)
        {
            return dim.x >= 8 && dim.y >= 8;
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
            int x1 = Random.Range(3, this.LocalDimensions.x / 2 - 1);
            int y1 = Random.Range(3, this.LocalDimensions.y / 2 - 1);
            IntVector2 intVector2 = this.LocalBasePosition + new IntVector2(x1, y1);
            IntVector2 Dimensions = this.LocalDimensions - new IntVector2(2 * x1, 2 * y1);
            for (int x2 = intVector2.x; x2 < intVector2.x + Dimensions.x; ++x2)
            {
                for (int y2 = intVector2.y; y2 < intVector2.y + Dimensions.y; ++y2)
                    room.ForceGetCellDataAtPoint(x2, y2).state = CellType.WALL;
            }
            if (!idea.UseWallSawblades)
                return;
            SerializedPath rectanglePathInset = this.GenerateRectanglePathInset(intVector2, Dimensions);
            room.paths.Add(rectanglePathInset);
            this.PlaceObject(RobotDave.GetSawbladePrefab(), room, intVector2, targetObjectLayer).assignedPathIDx = room.paths.Count - 1;
        }
    }

