using System;

#nullable disable

[Serializable]
public class IndividualBossFloorEntry
    {
        public DungeonPrerequisite[] GlobalBossPrerequisites;
        public float BossWeight = 1f;
        public GenericRoomTable TargetRoomTable;

        public float GetWeightModifier()
        {
            int num1 = 0;
            for (int index = 0; index < this.TargetRoomTable.includedRooms.elements.Count; ++index)
            {
                if (!((UnityEngine.Object) this.TargetRoomTable.includedRooms.elements[index].room == (UnityEngine.Object) null))
                {
                    int num2 = GameStatsManager.Instance.QueryRoomDifferentiator(this.TargetRoomTable.includedRooms.elements[index].room);
                    num1 += num2;
                }
            }
            if (num1 <= 0)
                return GameStatsManager.Instance.LastBossEncounteredMap.ContainsKey(GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId) && !BraveRandom.IgnoreGenerationDifferentiator && GameStatsManager.Instance.LastBossEncounteredMap[GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId] == this.TargetRoomTable.name ? 0.5f : 1f;
            if (num1 == 1)
                return 0.5f;
            return num1 >= 2 ? 0.01f : 0.01f;
        }

        public bool GlobalPrereqsValid()
        {
            for (int index = 0; index < this.GlobalBossPrerequisites.Length; ++index)
            {
                if (!this.GlobalBossPrerequisites[index].CheckConditionsFulfilled())
                    return false;
            }
            return true;
        }
    }

