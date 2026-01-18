using System;
using System.Collections.Generic;

#nullable disable

[Serializable]
public class GameLevelDefinition
  {
    public string dungeonSceneName;
    public string dungeonPrefabPath;
    public float priceMultiplier = 1f;
    public float secretDoorHealthMultiplier = 1f;
    public float enemyHealthMultiplier = 1f;
    public float damageCap = -1f;
    public float bossDpsCap = -1f;
    public List<DungeonFlowLevelEntry> flowEntries;
    public List<int> predefinedSeeds;
    [NonSerialized]
    public DungeonFlowLevelEntry lastSelectedFlowEntry;

    public DungeonFlowLevelEntry LovinglySelectDungeonFlow()
    {
      List<DungeonFlowLevelEntry> dungeonFlowLevelEntryList1 = new List<DungeonFlowLevelEntry>();
      float num1 = 0.0f;
      List<DungeonFlowLevelEntry> dungeonFlowLevelEntryList2 = new List<DungeonFlowLevelEntry>();
      float num2 = 0.0f;
      for (int index1 = 0; index1 < this.flowEntries.Count; ++index1)
      {
        bool flag = true;
        for (int index2 = 0; index2 < this.flowEntries[index1].prerequisites.Length; ++index2)
        {
          if (!this.flowEntries[index1].prerequisites[index2].CheckConditionsFulfilled())
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          if (GameStatsManager.Instance.QueryFlowDifferentiator(this.flowEntries[index1].flowPath) > 0)
          {
            num2 += this.flowEntries[index1].weight;
            dungeonFlowLevelEntryList2.Add(this.flowEntries[index1]);
          }
          else
          {
            if (this.flowEntries[index1].forceUseIfAvailable)
              return this.flowEntries[index1];
            num1 += this.flowEntries[index1].weight;
            dungeonFlowLevelEntryList1.Add(this.flowEntries[index1]);
          }
        }
      }
      if (dungeonFlowLevelEntryList1.Count <= 0 && dungeonFlowLevelEntryList2.Count > 0)
      {
        dungeonFlowLevelEntryList1 = dungeonFlowLevelEntryList2;
        num1 = num2;
      }
      if (dungeonFlowLevelEntryList1.Count <= 0)
        return (DungeonFlowLevelEntry) null;
      float num3 = UnityEngine.Random.value * num1;
      float num4 = 0.0f;
      for (int index = 0; index < dungeonFlowLevelEntryList1.Count; ++index)
      {
        num4 += dungeonFlowLevelEntryList1[index].weight;
        if ((double) num4 >= (double) num3)
          return dungeonFlowLevelEntryList1[index];
      }
      return (DungeonFlowLevelEntry) null;
    }
  }

