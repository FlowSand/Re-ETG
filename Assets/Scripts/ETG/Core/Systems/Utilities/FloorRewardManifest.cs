// Decompiled with JetBrains decompiler
// Type: FloorRewardManifest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class FloorRewardManifest
    {
      public Dictionary<Chest, List<PickupObject>> PregeneratedChestContents = new Dictionary<Chest, List<PickupObject>>();
      public List<PickupObject> PregeneratedBossRewards = new List<PickupObject>();
      private int m_bossIndex;
      public List<PickupObject> PregeneratedBossRewardsGunsOnly = new List<PickupObject>();
      private int m_bossGunIndex;
      public List<PickupObject> OtherRegisteredRewards = new List<PickupObject>();

      public void Initialize(RewardManager manager)
      {
        for (int index = 0; index < 5; ++index)
        {
          GameObject objectForBossSeeded1 = manager.GetRewardObjectForBossSeeded((List<PickupObject>) null, false);
          if ((bool) (Object) objectForBossSeeded1)
            this.PregeneratedBossRewards.Add(objectForBossSeeded1.GetComponent<PickupObject>());
          GameObject objectForBossSeeded2 = manager.GetRewardObjectForBossSeeded((List<PickupObject>) null, true);
          if ((bool) (Object) objectForBossSeeded2)
            this.PregeneratedBossRewardsGunsOnly.Add(objectForBossSeeded2.GetComponent<PickupObject>());
        }
      }

      public void Reinitialize(RewardManager manager)
      {
        this.PregeneratedChestContents.Clear();
        this.OtherRegisteredRewards.Clear();
      }

      public bool CheckManifestDifferentiator(PickupObject testItem)
      {
        if (this.PregeneratedBossRewards.Count > 0 && testItem.PickupObjectId == this.PregeneratedBossRewards[0].PickupObjectId || this.PregeneratedBossRewardsGunsOnly.Count > 0 && testItem.PickupObjectId == this.PregeneratedBossRewardsGunsOnly[0].PickupObjectId)
          return true;
        foreach (KeyValuePair<Chest, List<PickupObject>> pregeneratedChestContent in this.PregeneratedChestContents)
        {
          for (int index = 0; index < pregeneratedChestContent.Value.Count; ++index)
          {
            if (pregeneratedChestContent.Value[index].PickupObjectId == testItem.PickupObjectId)
              return true;
          }
        }
        for (int index = 0; index < this.OtherRegisteredRewards.Count; ++index)
        {
          if (this.OtherRegisteredRewards[index].PickupObjectId == testItem.PickupObjectId)
            return true;
        }
        return false;
      }

      public PickupObject GetNextBossReward(bool forceGun)
      {
        if (forceGun)
        {
          ++this.m_bossGunIndex;
          return this.PregeneratedBossRewardsGunsOnly[this.m_bossGunIndex - 1];
        }
        ++this.m_bossIndex;
        return this.PregeneratedBossRewards[this.m_bossIndex - 1];
      }

      public void RegisterContents(Chest source, List<PickupObject> contents)
      {
        this.PregeneratedChestContents.Add(source, contents);
      }
    }

}
