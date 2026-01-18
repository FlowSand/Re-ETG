using UnityEngine;

using Dungeonator;

#nullable disable

public class RobotRoomTablesFeature : RobotRoomFeature
    {
        public override bool AcceptableInIdea(
            RobotDaveIdea idea,
            IntVector2 dim,
            bool isInternal,
            int numFeatures)
        {
            return dim.x >= 7 || dim.y >= 7;
        }

        public override bool CanContainOtherFeature() => true;

        public override int RequiredInsetForOtherFeature() => 5;

        public override void Develop(
            PrototypeDungeonRoom room,
            RobotDaveIdea idea,
            int targetObjectLayer)
        {
            bool flag = this.LocalDimensions.x < this.LocalDimensions.y;
            if ((double) Mathf.Abs((float) (1.0 - (double) this.LocalDimensions.x / ((double) this.LocalDimensions.y * 1.0))) < 0.25)
                flag = (double) Random.value < 0.5;
            if (flag)
            {
                DungeonPlaceable horizontalTable = RobotDave.GetHorizontalTable();
                for (int x = this.LocalBasePosition.x + 3; x < this.LocalBasePosition.x + this.LocalDimensions.x - 3; x += 4)
                {
                    IntVector2 position1 = new IntVector2(x, this.LocalBasePosition.y + 3);
                    IntVector2 position2 = new IntVector2(x, this.LocalBasePosition.y + this.LocalDimensions.y - 4);
                    this.PlaceObject(horizontalTable, room, position1, targetObjectLayer);
                    this.PlaceObject(horizontalTable, room, position2, targetObjectLayer);
                }
            }
            else
            {
                DungeonPlaceable verticalTable = RobotDave.GetVerticalTable();
                for (int y = this.LocalBasePosition.y + 3; y < this.LocalBasePosition.y + this.LocalDimensions.y - 3; y += 4)
                {
                    IntVector2 position3 = new IntVector2(this.LocalBasePosition.x + 3, y);
                    IntVector2 position4 = new IntVector2(this.LocalBasePosition.x + this.LocalDimensions.x - 4, y);
                    this.PlaceObject(verticalTable, room, position3, targetObjectLayer);
                    this.PlaceObject(verticalTable, room, position4, targetObjectLayer);
                }
            }
        }
    }

