using System.Collections.Generic;

using UnityEngine;

using Dungeonator;

#nullable disable

public class RobotRoomChandelierFeature : RobotRoomFeature
    {
        public override bool AcceptableInIdea(
            RobotDaveIdea idea,
            IntVector2 dim,
            bool isInternal,
            int numFeatures)
        {
            return idea.UseChandeliers && dim.x >= 5 && dim.y >= 5 && !isInternal;
        }

        public override void Develop(
            PrototypeDungeonRoom room,
            RobotDaveIdea idea,
            int targetObjectLayer)
        {
            IntVector2 position = this.LocalBasePosition + new IntVector2(this.LocalDimensions.x / 2 - 1, this.LocalDimensions.y / 2 - 1);
            PrototypePlacedObjectData placedObjectData = this.PlaceObject(RobotDave.GetChandelierPrefab(), room, position, targetObjectLayer);
            IntVector2 intVector2 = this.LocalBasePosition + new IntVector2(Random.Range(0, this.LocalDimensions.x), this.LocalDimensions.y - 1);
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

