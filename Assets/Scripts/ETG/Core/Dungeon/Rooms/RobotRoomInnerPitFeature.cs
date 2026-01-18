using UnityEngine;

using Dungeonator;

#nullable disable

public class RobotRoomInnerPitFeature : RobotRoomFeature
    {
        public override bool AcceptableInIdea(
            RobotDaveIdea idea,
            IntVector2 dim,
            bool isInternal,
            int numFeatures)
        {
            return dim.x > 7 && dim.y > 7 && idea.CanIncludePits;
        }

        public override void Develop(
            PrototypeDungeonRoom room,
            RobotDaveIdea idea,
            int targetObjectLayer)
        {
            int num1 = Random.Range(4, this.LocalDimensions.x / 2);
            int num2 = Random.Range(4, this.LocalDimensions.y / 2);
            int num3 = (this.LocalDimensions.x - num1) / 2 + this.LocalBasePosition.x;
            int num4 = (this.LocalDimensions.y - num2) / 2 + this.LocalBasePosition.y;
            for (int ix = num3; ix < num3 + num1; ++ix)
            {
                for (int iy = num4; iy < num4 + num2; ++iy)
                    room.ForceGetCellDataAtPoint(ix, iy).state = CellType.PIT;
            }
            room.RedefineAllPitEntries();
        }
    }

