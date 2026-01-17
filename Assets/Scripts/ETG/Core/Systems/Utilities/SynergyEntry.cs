// Decompiled with JetBrains decompiler
// Type: SynergyEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class SynergyEntry
    {
      public string NameKey;
      public SynergyEntry.SynergyActivation ActivationStatus;
      [PickupIdentifier]
      public List<int> gunIDs;
      [PickupIdentifier]
      public List<int> itemIDs;
      public bool GunsOR;
      public bool ItemsOR;
      public bool ActiveWhenGunUnequipped;
      public bool SuppressVFX;
      public int ExtraItemsOrForBrents;
      public List<StatModifier> statModifiers;
      [LongNumericEnum]
      public List<CustomSynergyType> bonusSynergies;

      public bool SynergyIsActive(PlayerController p, PlayerController p2)
      {
        return this.gunIDs.Count <= 0 || this.ActiveWhenGunUnequipped || (bool) (UnityEngine.Object) p && (bool) (UnityEngine.Object) p.CurrentGun && this.gunIDs.Contains(p.CurrentGun.PickupObjectId) || (bool) (UnityEngine.Object) p2 && (bool) (UnityEngine.Object) p2.CurrentGun && this.gunIDs.Contains(p2.CurrentGun.PickupObjectId) || (bool) (UnityEngine.Object) p && (bool) (UnityEngine.Object) p.CurrentSecondaryGun && this.gunIDs.Contains(p.CurrentSecondaryGun.PickupObjectId) || (bool) (UnityEngine.Object) p2 && (bool) (UnityEngine.Object) p2.CurrentSecondaryGun && this.gunIDs.Contains(p.CurrentSecondaryGun.PickupObjectId);
      }

      public bool SynergyIsAvailable(PlayerController p, PlayerController p2)
      {
        if (this.ActivationStatus == SynergyEntry.SynergyActivation.INACTIVE || this.ActivationStatus == SynergyEntry.SynergyActivation.DEMO)
          return false;
        bool flag1 = true;
        bool flag2 = true;
        if (this.gunIDs.Count > 0)
        {
          if (this.GunsOR)
            flag1 = false;
          for (int index1 = 0; index1 < this.gunIDs.Count; ++index1)
          {
            bool flag3 = false;
            if ((bool) (UnityEngine.Object) p && p.inventory != null && p.inventory.AllGuns != null)
            {
              for (int index2 = 0; index2 < p.inventory.AllGuns.Count; ++index2)
              {
                if (p.inventory.AllGuns[index2].PickupObjectId == this.gunIDs[index1])
                {
                  flag3 = true;
                  break;
                }
              }
            }
            if ((bool) (UnityEngine.Object) p2 && p2.inventory != null && p2.inventory.AllGuns != null)
            {
              for (int index3 = 0; index3 < p2.inventory.AllGuns.Count; ++index3)
              {
                if (p2.inventory.AllGuns[index3].PickupObjectId == this.gunIDs[index1])
                {
                  flag3 = true;
                  break;
                }
              }
            }
            if (flag3 && this.GunsOR)
            {
              flag1 = true;
              break;
            }
            if (!flag3 && !this.GunsOR)
            {
              flag1 = false;
              break;
            }
          }
        }
        if (!flag1)
          return false;
        if (this.itemIDs.Count > 0)
        {
          if (this.ItemsOR)
            flag2 = false;
          int num = 0;
          for (int index4 = 0; index4 < this.itemIDs.Count; ++index4)
          {
            bool flag4 = false;
            bool flag5;
            if ((bool) (UnityEngine.Object) p)
            {
              for (int index5 = 0; index5 < p.activeItems.Count; ++index5)
              {
                if (p.activeItems[index5].PickupObjectId == this.itemIDs[index4])
                {
                  flag4 = true;
                  break;
                }
              }
              for (int index6 = 0; index6 < p.passiveItems.Count; ++index6)
              {
                if (p.passiveItems[index6].PickupObjectId == this.itemIDs[index4])
                {
                  flag4 = true;
                  break;
                }
              }
              if (this.itemIDs[index4] == GlobalItemIds.Map && p.EverHadMap)
              {
                flag5 = true;
                break;
              }
            }
            if ((bool) (UnityEngine.Object) p2)
            {
              for (int index7 = 0; index7 < p2.activeItems.Count; ++index7)
              {
                if (p2.activeItems[index7].PickupObjectId == this.itemIDs[index4])
                {
                  flag4 = true;
                  break;
                }
              }
              for (int index8 = 0; index8 < p2.passiveItems.Count; ++index8)
              {
                if (p2.passiveItems[index8].PickupObjectId == this.itemIDs[index4])
                {
                  flag4 = true;
                  break;
                }
              }
              if (this.itemIDs[index4] == GlobalItemIds.Map && p2.EverHadMap)
              {
                flag5 = true;
                break;
              }
            }
            if (flag4 && this.ItemsOR)
              ++num;
            if (!flag4 && !this.ItemsOR)
            {
              flag2 = false;
              break;
            }
          }
          if (this.ItemsOR && num > this.ExtraItemsOrForBrents)
            flag2 = true;
        }
        return flag1 && flag2;
      }

      public enum SynergyActivation
      {
        ACTIVE,
        DEMO,
        INACTIVE,
        ACTIVE_UNBOOSTED,
      }
    }

}
