using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class BossFloorEntry
  {
    public string Annotation;
    [EnumFlags]
    public GlobalDungeonData.ValidTilesets AssociatedTilesets;
    [SerializeField]
    public List<IndividualBossFloorEntry> Bosses;

    public IndividualBossFloorEntry SelectBoss()
    {
      List<IndividualBossFloorEntry> individualBossFloorEntryList = new List<IndividualBossFloorEntry>();
      float num1 = 0.0f;
      for (int index = 0; index < this.Bosses.Count; ++index)
      {
        if (this.Bosses[index].GlobalPrereqsValid())
        {
          individualBossFloorEntryList.Add(this.Bosses[index]);
          Debug.LogWarning((object) $"Adding valid boss: {this.Bosses[index].TargetRoomTable.name}|{(object) this.Bosses[index].GetWeightModifier()}");
          num1 += this.Bosses[index].GetWeightModifier() * this.Bosses[index].BossWeight;
        }
      }
      float num2 = BraveRandom.GenerationRandomValue() * num1;
      float num3 = 0.0f;
      for (int index = 0; index < individualBossFloorEntryList.Count; ++index)
      {
        num3 += this.Bosses[index].GetWeightModifier() * individualBossFloorEntryList[index].BossWeight;
        if ((double) num3 >= (double) num2)
        {
          Debug.LogWarning((object) $"Returning valid boss: {individualBossFloorEntryList[index].TargetRoomTable.name}|{(object) individualBossFloorEntryList[index].GetWeightModifier()}");
          return individualBossFloorEntryList[index];
        }
      }
      Debug.LogWarning((object) $"Returning fallback boss boss: {individualBossFloorEntryList[individualBossFloorEntryList.Count - 1].TargetRoomTable.name}|{(object) individualBossFloorEntryList[individualBossFloorEntryList.Count - 1].GetWeightModifier()}");
      return individualBossFloorEntryList[individualBossFloorEntryList.Count - 1];
    }
  }

