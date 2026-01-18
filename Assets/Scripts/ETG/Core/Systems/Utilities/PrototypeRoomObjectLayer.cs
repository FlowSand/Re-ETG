using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class PrototypeRoomObjectLayer
  {
    public List<PrototypePlacedObjectData> placedObjects = new List<PrototypePlacedObjectData>();
    public List<Vector2> placedObjectBasePositions = new List<Vector2>();
    public bool layerIsReinforcementLayer;
    public bool shuffle = true;
    public int randomize = 2;
    public bool suppressPlayerChecks;
    public float delayTime = 15f;
    public RoomEventTriggerCondition reinforcementTriggerCondition = RoomEventTriggerCondition.ON_ENEMIES_CLEARED;
    public float probability = 1f;
    public int numberTimesEncounteredRequired;

    public static PrototypeRoomObjectLayer CreateMirror(
      PrototypeRoomObjectLayer source,
      IntVector2 roomDimensions)
    {
      PrototypeRoomObjectLayer mirror = new PrototypeRoomObjectLayer();
      mirror.placedObjects = new List<PrototypePlacedObjectData>();
      for (int index = 0; index < source.placedObjects.Count; ++index)
        mirror.placedObjects.Add(source.placedObjects[index].CreateMirror(roomDimensions));
      mirror.placedObjectBasePositions = new List<Vector2>();
      for (int index = 0; index < source.placedObjectBasePositions.Count; ++index)
      {
        Vector2 objectBasePosition = source.placedObjectBasePositions[index];
        objectBasePosition.x = (float) roomDimensions.x - (objectBasePosition.x + (float) mirror.placedObjects[index].GetWidth(true));
        mirror.placedObjectBasePositions.Add(objectBasePosition);
      }
      mirror.layerIsReinforcementLayer = source.layerIsReinforcementLayer;
      mirror.shuffle = source.shuffle;
      mirror.randomize = source.randomize;
      mirror.suppressPlayerChecks = source.suppressPlayerChecks;
      mirror.delayTime = source.delayTime;
      mirror.reinforcementTriggerCondition = source.reinforcementTriggerCondition;
      mirror.probability = source.probability;
      mirror.numberTimesEncounteredRequired = source.numberTimesEncounteredRequired;
      return mirror;
    }
  }

