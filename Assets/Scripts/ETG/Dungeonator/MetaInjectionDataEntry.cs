using System;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class MetaInjectionDataEntry
  {
    public SharedInjectionData injectionData;
    public float OverallChanceToTrigger = 1f;
    public bool UsesUnlockedChanceToTrigger;
    public MetaInjectionUnlockedChanceEntry[] UnlockedChancesToTrigger;
    public int MinToAppearPerRun;
    public int MaxToAppearPerRun = 2;
    public bool UsesWeightedNumberToAppearPerRun;
    public WeightedIntCollection WeightedNumberToAppear;
    public bool AllowBonusSecret;
    [ShowInInspectorIf("AllowBonusSecret", false)]
    public float ChanceForBonusSecret = 0.5f;
    public bool IsPartOfExcludedCastleSet;
    [EnumFlags]
    public GlobalDungeonData.ValidTilesets validTilesets;

    public float ModifiedChanceToTrigger
    {
      get
      {
        if (this.UsesUnlockedChanceToTrigger && GameStatsManager.HasInstance)
        {
          for (int index1 = this.UnlockedChancesToTrigger.Length - 1; index1 >= 0; --index1)
          {
            MetaInjectionUnlockedChanceEntry unlockedChanceEntry = this.UnlockedChancesToTrigger[index1];
            bool flag = true;
            for (int index2 = 0; index2 < unlockedChanceEntry.prerequisites.Length; ++index2)
            {
              if (!unlockedChanceEntry.prerequisites[index2].CheckConditionsFulfilled())
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              Debug.LogError((object) $"chance to trigger: {(object) index1}|{(object) unlockedChanceEntry.ChanceToTrigger}");
              return unlockedChanceEntry.ChanceToTrigger;
            }
          }
        }
        return this.OverallChanceToTrigger;
      }
    }
  }
}
