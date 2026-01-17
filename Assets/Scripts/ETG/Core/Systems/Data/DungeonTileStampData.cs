// Decompiled with JetBrains decompiler
// Type: DungeonTileStampData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Data
{
    [Serializable]
    public class DungeonTileStampData : ScriptableObject
    {
      public float tileStampWeight = 1f;
      public float spriteStampWeight;
      public float objectStampWeight = 1f;
      public TileStampData[] stamps;
      public SpriteStampData[] spriteStamps;
      public ObjectStampData[] objectStamps;
      public float SymmetricFrameChance = 0.5f;
      public float SymmetricCompleteChance = 0.25f;

      public bool ContainsTileIndex(int index)
      {
        if (this.stamps == null)
          return false;
        foreach (TileStampData stamp in this.stamps)
        {
          if (stamp.stampTileIndices.Contains(index))
            return true;
        }
        return false;
      }

      protected bool IsValidRoomType(StampDataBase s, int roomType)
      {
        if (s.roomTypeData.Count == 0)
          return true;
        for (int index = 0; index < s.roomTypeData.Count; ++index)
        {
          if (s.roomTypeData[index].roomSubType == roomType)
            return true;
        }
        return false;
      }

      public StampDataBase GetStampDataSimple(
        DungeonTileStampData.StampPlacementRule placement,
        Opulence oppan,
        int roomType,
        int maxWidth = 1000)
      {
        WeightedList<StampDataBase> weightedList = new WeightedList<StampDataBase>();
        for (int index = 0; index < this.stamps.Length; ++index)
        {
          TileStampData stamp = this.stamps[index];
          if (!stamp.requiresForcedMatchingStyle && placement == stamp.placementRule && this.IsValidRoomType((StampDataBase) stamp, roomType) && stamp.width <= maxWidth)
            weightedList.Add((StampDataBase) stamp, stamp.GetRelativeWeight(roomType) * this.tileStampWeight);
        }
        for (int index = 0; index < this.spriteStamps.Length; ++index)
        {
          SpriteStampData spriteStamp = this.spriteStamps[index];
          if (!spriteStamp.requiresForcedMatchingStyle && placement == spriteStamp.placementRule && this.IsValidRoomType((StampDataBase) spriteStamp, roomType) && spriteStamp.width <= maxWidth)
            weightedList.Add((StampDataBase) spriteStamp, spriteStamp.GetRelativeWeight(roomType) * this.spriteStampWeight);
        }
        for (int index = 0; index < this.objectStamps.Length; ++index)
        {
          ObjectStampData objectStamp = this.objectStamps[index];
          if (!objectStamp.requiresForcedMatchingStyle && placement == objectStamp.placementRule && this.IsValidRoomType((StampDataBase) objectStamp, roomType) && objectStamp.width <= maxWidth)
            weightedList.Add((StampDataBase) objectStamp, objectStamp.GetRelativeWeight(roomType) * this.objectStampWeight);
        }
        return weightedList.elements == null || weightedList.elements.Count == 0 ? (StampDataBase) null : weightedList.SelectByWeight();
      }

      public StampDataBase AttemptGetSimilarStampForRoomDuplication(
        StampDataBase source,
        List<StampDataBase> excluded,
        int roomType)
      {
        WeightedList<StampDataBase> weightedList = new WeightedList<StampDataBase>();
        for (int index = 0; index < this.stamps.Length; ++index)
        {
          StampDataBase stamp = (StampDataBase) this.stamps[index];
          if (this.IsValidRoomType(stamp, roomType) && source.stampCategory == stamp.stampCategory && source.placementRule == stamp.placementRule && source.width == stamp.width && source.height == stamp.height && source.occupySpace == stamp.occupySpace && source.preventRoomRepeats == stamp.preventRoomRepeats && !excluded.Contains(stamp))
            weightedList.Add(stamp, stamp.GetRelativeWeight(roomType));
        }
        return weightedList.elements == null || weightedList.elements.Count == 0 ? (StampDataBase) null : weightedList.SelectByWeight();
      }

      public StampDataBase GetStampDataSimple(
        List<DungeonTileStampData.StampPlacementRule> placements,
        Opulence oppan,
        int roomType,
        int maxWidth,
        bool excludeWallSpace,
        List<StampDataBase> excluded)
      {
        WeightedList<StampDataBase> weightedList = new WeightedList<StampDataBase>();
        for (int index = 0; index < this.stamps.Length; ++index)
        {
          TileStampData stamp = this.stamps[index];
          if ((!stamp.preventRoomRepeats || !excluded.Contains((StampDataBase) stamp)) && !stamp.requiresForcedMatchingStyle)
          {
            bool flag = stamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS || stamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS;
            if ((!excludeWallSpace || flag || stamp.height <= 1) && (!excludeWallSpace || stamp.width <= 1) && (!excludeWallSpace || stamp.occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE) && placements.Contains(stamp.placementRule) && this.IsValidRoomType((StampDataBase) stamp, roomType) && stamp.width <= maxWidth)
              weightedList.Add((StampDataBase) stamp, stamp.GetRelativeWeight(roomType) * this.tileStampWeight);
          }
        }
        for (int index = 0; index < this.spriteStamps.Length; ++index)
        {
          SpriteStampData spriteStamp = this.spriteStamps[index];
          if ((!spriteStamp.preventRoomRepeats || !excluded.Contains((StampDataBase) spriteStamp)) && (!excludeWallSpace || spriteStamp.height <= 1) && (!excludeWallSpace || spriteStamp.width <= 1) && !spriteStamp.requiresForcedMatchingStyle && (!excludeWallSpace || spriteStamp.occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE) && placements.Contains(spriteStamp.placementRule) && this.IsValidRoomType((StampDataBase) spriteStamp, roomType) && spriteStamp.width <= maxWidth)
            weightedList.Add((StampDataBase) spriteStamp, spriteStamp.GetRelativeWeight(roomType) * this.spriteStampWeight);
        }
        for (int index = 0; index < this.objectStamps.Length; ++index)
        {
          ObjectStampData objectStamp = this.objectStamps[index];
          if (!objectStamp.preventRoomRepeats || !excluded.Contains((StampDataBase) objectStamp))
          {
            bool flag = objectStamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS || objectStamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS;
            if ((!excludeWallSpace || flag || objectStamp.height <= 1) && (!excludeWallSpace || objectStamp.width <= 1) && !objectStamp.requiresForcedMatchingStyle && (!excludeWallSpace || objectStamp.occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE) && placements.Contains(objectStamp.placementRule) && this.IsValidRoomType((StampDataBase) objectStamp, roomType) && objectStamp.width <= maxWidth)
              weightedList.Add((StampDataBase) objectStamp, objectStamp.GetRelativeWeight(roomType) * this.objectStampWeight);
          }
        }
        return weightedList.elements == null || weightedList.elements.Count == 0 ? (StampDataBase) null : weightedList.SelectByWeight();
      }

      public StampDataBase GetStampDataSimpleWithForcedRule(
        List<DungeonTileStampData.StampPlacementRule> placements,
        DungeonTileStampData.IntermediaryMatchingStyle forcedStyle,
        Opulence oppan,
        int roomType,
        int maxWidth = 1000,
        bool excludeWallSpace = false)
      {
        WeightedList<StampDataBase> weightedList = new WeightedList<StampDataBase>();
        for (int index = 0; index < this.stamps.Length; ++index)
        {
          TileStampData stamp = this.stamps[index];
          bool flag = stamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS || stamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS;
          if ((!excludeWallSpace || flag || stamp.height <= 1) && (!excludeWallSpace || stamp.width <= 1) && (!excludeWallSpace || stamp.occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE) && placements.Contains(stamp.placementRule) && this.IsValidRoomType((StampDataBase) stamp, roomType) && stamp.width <= maxWidth && stamp.intermediaryMatchingStyle == forcedStyle)
            weightedList.Add((StampDataBase) stamp, stamp.GetRelativeWeight(roomType) * this.tileStampWeight);
        }
        for (int index = 0; index < this.spriteStamps.Length; ++index)
        {
          SpriteStampData spriteStamp = this.spriteStamps[index];
          if ((!excludeWallSpace || spriteStamp.height <= 1) && (!excludeWallSpace || spriteStamp.width <= 1) && (!excludeWallSpace || spriteStamp.occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE) && placements.Contains(spriteStamp.placementRule) && this.IsValidRoomType((StampDataBase) spriteStamp, roomType) && spriteStamp.width <= maxWidth && spriteStamp.intermediaryMatchingStyle == forcedStyle)
            weightedList.Add((StampDataBase) spriteStamp, spriteStamp.GetRelativeWeight(roomType) * this.spriteStampWeight);
        }
        for (int index = 0; index < this.objectStamps.Length; ++index)
        {
          ObjectStampData objectStamp = this.objectStamps[index];
          bool flag = objectStamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS || objectStamp.placementRule == DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS;
          if ((!excludeWallSpace || flag || objectStamp.height <= 1) && (!excludeWallSpace || objectStamp.width <= 1) && (!excludeWallSpace || objectStamp.occupySpace == DungeonTileStampData.StampSpace.OBJECT_SPACE) && placements.Contains(objectStamp.placementRule) && this.IsValidRoomType((StampDataBase) objectStamp, roomType) && objectStamp.width <= maxWidth && objectStamp.intermediaryMatchingStyle == forcedStyle)
            weightedList.Add((StampDataBase) objectStamp, objectStamp.GetRelativeWeight(roomType) * this.objectStampWeight);
        }
        return weightedList.elements == null || weightedList.elements.Count == 0 ? (StampDataBase) null : weightedList.SelectByWeight();
      }

      public StampDataBase GetStampDataComplex(
        DungeonTileStampData.StampPlacementRule placement,
        DungeonTileStampData.StampSpace space,
        DungeonTileStampData.StampCategory category,
        Opulence oppan,
        int roomType,
        int maxWidth = 1000)
      {
        return this.GetStampDataComplex(new List<DungeonTileStampData.StampPlacementRule>()
        {
          placement
        }, space, category, oppan, roomType, maxWidth);
      }

      public StampDataBase GetStampDataComplex(
        List<DungeonTileStampData.StampPlacementRule> placements,
        DungeonTileStampData.StampSpace space,
        DungeonTileStampData.StampCategory category,
        Opulence oppan,
        int roomType,
        int maxWidth = 1000)
      {
        bool flag = placements.Contains(DungeonTileStampData.StampPlacementRule.ALONG_LEFT_WALLS) || placements.Contains(DungeonTileStampData.StampPlacementRule.ALONG_RIGHT_WALLS);
        WeightedList<StampDataBase> weightedList = new WeightedList<StampDataBase>();
        for (int index = 0; index < this.stamps.Length; ++index)
        {
          TileStampData stamp = this.stamps[index];
          if (!stamp.requiresForcedMatchingStyle && placements.Contains(stamp.placementRule) && stamp.occupySpace == space && this.IsValidRoomType((StampDataBase) stamp, roomType) && (!flag ? stamp.width : stamp.height) <= maxWidth)
            weightedList.Add((StampDataBase) stamp, stamp.GetRelativeWeight(roomType) * this.tileStampWeight);
        }
        for (int index = 0; index < this.spriteStamps.Length; ++index)
        {
          SpriteStampData spriteStamp = this.spriteStamps[index];
          if (!spriteStamp.requiresForcedMatchingStyle && placements.Contains(spriteStamp.placementRule) && spriteStamp.occupySpace == space && this.IsValidRoomType((StampDataBase) spriteStamp, roomType) && (!flag ? spriteStamp.width : spriteStamp.height) <= maxWidth)
            weightedList.Add((StampDataBase) spriteStamp, spriteStamp.GetRelativeWeight(roomType) * this.spriteStampWeight);
        }
        for (int index = 0; index < this.objectStamps.Length; ++index)
        {
          ObjectStampData objectStamp = this.objectStamps[index];
          if (!objectStamp.requiresForcedMatchingStyle && placements.Contains(objectStamp.placementRule) && objectStamp.occupySpace == space && this.IsValidRoomType((StampDataBase) objectStamp, roomType) && (!flag ? objectStamp.width : objectStamp.height) <= maxWidth)
            weightedList.Add((StampDataBase) objectStamp, objectStamp.GetRelativeWeight(roomType) * this.objectStampWeight);
        }
        return weightedList.SelectByWeight();
      }

      public enum StampPlacementRule
      {
        ON_LOWER_FACEWALL,
        ON_UPPER_FACEWALL,
        BELOW_LOWER_FACEWALL,
        ALONG_LEFT_WALLS,
        ON_TOPWALL,
        ON_ANY_FLOOR,
        ABOVE_UPPER_FACEWALL,
        ON_ANY_CEILING,
        ALONG_RIGHT_WALLS,
        BELOW_LOWER_FACEWALL_LEFT_CORNER,
        BELOW_LOWER_FACEWALL_RIGHT_CORNER,
      }

      public enum StampSpace
      {
        OBJECT_SPACE,
        WALL_SPACE,
        BOTH_SPACES,
      }

      public enum StampCategory
      {
        STRUCTURAL,
        NATURAL,
        MUNDANE,
        DECORATIVE,
      }

      public enum IntermediaryMatchingStyle
      {
        ANY,
        COLUMN,
        WALL_HOLE,
        BANNER,
        PORTRAIT,
        WALL_HOLE_FILLER,
        SKELETON,
        ROCK,
      }
    }

}
