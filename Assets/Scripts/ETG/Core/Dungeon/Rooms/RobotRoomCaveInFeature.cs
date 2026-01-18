using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class RobotRoomCaveInFeature : RobotRoomFeature
    {
        public override bool AcceptableInIdea(
            RobotDaveIdea idea,
            IntVector2 dim,
            bool isInternal,
            int numFeatures)
        {
            return idea.UseCaveIns && dim.x >= 5 && dim.y >= 5;
        }

        public override void Develop(
            PrototypeDungeonRoom room,
            RobotDaveIdea idea,
            int targetObjectLayer)
        {
            int x = this.LocalDimensions.x / 2 - 1;
            int y = this.LocalDimensions.y / 2 - 1;
            IntVector2 position = this.LocalBasePosition + new IntVector2(x, y);
            PrototypePlacedObjectData placedObjectData = this.PlaceObject(RobotDave.GetCaveInPrefab(), room, position, targetObjectLayer);
            IntVector2 intVector2 = this.LocalBasePosition + new IntVector2[8]
            {
                new IntVector2(1, 1),
                new IntVector2(x, 1),
                new IntVector2(this.LocalDimensions.x - 2, 1),
                new IntVector2(1, y),
                new IntVector2(this.LocalDimensions.x - 2, y),
                new IntVector2(1, this.LocalDimensions.y - 2),
                new IntVector2(x, this.LocalDimensions.y - 2),
                new IntVector2(this.LocalDimensions.x - 2, this.LocalDimensions.y - 2)
            }[Random.Range(0, 8)];
            PrototypeEventTriggerArea eventTriggerArea = room.AddEventTriggerArea((IEnumerable<IntVector2>) new List<IntVector2>()
            {
                intVector2
            });
            int num = room.eventTriggerAreas.IndexOf(eventTriggerArea);
            placedObjectData.linkedTriggerAreaIDs = new List<int>()
            {
                num
            };
        }
    }

