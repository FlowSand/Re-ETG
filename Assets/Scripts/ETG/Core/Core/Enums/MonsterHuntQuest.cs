// Decompiled with JetBrains decompiler
// Type: MonsterHuntQuest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Enums
{
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

}
