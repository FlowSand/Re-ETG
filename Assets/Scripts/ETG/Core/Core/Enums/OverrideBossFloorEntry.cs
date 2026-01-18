using System;

#nullable disable

[Serializable]
public class OverrideBossFloorEntry
    {
        public string Annotation;
        [EnumFlags]
        public GlobalDungeonData.ValidTilesets AssociatedTilesets;
        public DungeonPrerequisite[] GlobalBossPrerequisites;
        public float ChanceToOverride = 0.01f;
        public GenericRoomTable TargetRoomTable;

        public bool GlobalPrereqsValid(GlobalDungeonData.ValidTilesets targetTileset)
        {
            if ((this.AssociatedTilesets | targetTileset) != this.AssociatedTilesets)
                return false;
            for (int index = 0; index < this.GlobalBossPrerequisites.Length; ++index)
            {
                if (!this.GlobalBossPrerequisites[index].CheckConditionsFulfilled())
                    return false;
            }
            return true;
        }
    }

