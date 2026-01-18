using Dungeonator;
using System;
using UnityEngine;

#nullable disable

[Serializable]
public class DebrisDirectionalAnimationInfo
  {
    public string fallUp;
    public string fallRight;
    public string fallDown;
    public string fallLeft;

    public string GetAnimationForVector(Vector2 dir)
    {
      switch (DungeonData.GetCardinalFromVector2(dir))
      {
        case DungeonData.Direction.NORTH:
          return this.fallUp;
        case DungeonData.Direction.EAST:
          return this.fallRight;
        case DungeonData.Direction.SOUTH:
          return this.fallDown;
        case DungeonData.Direction.WEST:
          return this.fallLeft;
        default:
          return this.fallDown;
      }
    }
  }

