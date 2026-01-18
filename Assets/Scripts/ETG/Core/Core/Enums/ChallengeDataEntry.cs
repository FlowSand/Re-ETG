using System;
using System.Collections.Generic;

#nullable disable

[Serializable]
public class ChallengeDataEntry
    {
        public string Annotation;
        public ChallengeModifier challenge;
        [EnumFlags]
        public GlobalDungeonData.ValidTilesets excludedTilesets;
        public List<GlobalDungeonData.ValidTilesets> tilesetsWithCustomValues;
        public List<int> CustomValues;

        public int GetWeightForFloor(GlobalDungeonData.ValidTilesets tileset)
        {
            return this.tilesetsWithCustomValues.Contains(tileset) ? this.CustomValues[this.tilesetsWithCustomValues.IndexOf(tileset)] : 1;
        }
    }

