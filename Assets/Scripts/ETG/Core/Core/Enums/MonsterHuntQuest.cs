using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class MonsterHuntQuest
  {
    [SerializeField]
    [LongEnum]
    public GungeonFlags QuestFlag;
    [SerializeField]
    public string QuestIntroString;
    [SerializeField]
    public string TargetStringKey;
    [EnemyIdentifier]
    [SerializeField]
    public List<string> ValidTargetMonsterGuids = new List<string>();
    [SerializeField]
    public int NumberKillsRequired;
    [SerializeField]
    [LongEnum]
    public List<GungeonFlags> FlagsToSetUponReward;

    public bool IsQuestComplete() => GameStatsManager.Instance.GetFlag(this.QuestFlag);

    public bool ContainsEnemy(string enemyGuid)
    {
      for (int index = 0; index < this.ValidTargetMonsterGuids.Count; ++index)
      {
        if (this.ValidTargetMonsterGuids[index] == enemyGuid)
          return true;
      }
      return false;
    }

    public void UnlockRewards()
    {
      for (int index = 0; index < this.FlagsToSetUponReward.Count; ++index)
        GameStatsManager.Instance.SetFlag(this.FlagsToSetUponReward[index], true);
    }
  }

