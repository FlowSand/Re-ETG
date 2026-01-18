using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class AdvancedSynergyEntry
  {
    public string NameKey;
    public SynergyEntry.SynergyActivation ActivationStatus;
    [PickupIdentifier]
    public List<int> MandatoryGunIDs = new List<int>();
    [PickupIdentifier]
    public List<int> MandatoryItemIDs = new List<int>();
    [PickupIdentifier]
    public List<int> OptionalGunIDs = new List<int>();
    [PickupIdentifier]
    public List<int> OptionalItemIDs = new List<int>();
    public int NumberObjectsRequired = 2;
    public bool ActiveWhenGunUnequipped;
    public bool SuppressVFX;
    public bool RequiresAtLeastOneGunAndOneItem;
    public bool IgnoreLichEyeBullets;
    public List<StatModifier> statModifiers;
    [LongNumericEnum]
    public List<CustomSynergyType> bonusSynergies;

    public bool SynergyIsActive(PlayerController p, PlayerController p2)
    {
      return this.MandatoryGunIDs.Count <= 0 && (!this.RequiresAtLeastOneGunAndOneItem || this.OptionalGunIDs.Count <= 0) || this.ActiveWhenGunUnequipped || (bool) (UnityEngine.Object) p && (bool) (UnityEngine.Object) p.CurrentGun && (this.MandatoryGunIDs.Contains(p.CurrentGun.PickupObjectId) || this.OptionalGunIDs.Contains(p.CurrentGun.PickupObjectId)) || (bool) (UnityEngine.Object) p2 && (bool) (UnityEngine.Object) p2.CurrentGun && (this.MandatoryGunIDs.Contains(p2.CurrentGun.PickupObjectId) || this.OptionalGunIDs.Contains(p2.CurrentGun.PickupObjectId)) || (bool) (UnityEngine.Object) p && (bool) (UnityEngine.Object) p.CurrentSecondaryGun && (this.MandatoryGunIDs.Contains(p.CurrentSecondaryGun.PickupObjectId) || this.OptionalGunIDs.Contains(p.CurrentSecondaryGun.PickupObjectId)) || (bool) (UnityEngine.Object) p2 && (bool) (UnityEngine.Object) p2.CurrentSecondaryGun && (this.MandatoryGunIDs.Contains(p2.CurrentSecondaryGun.PickupObjectId) || this.OptionalGunIDs.Contains(p2.CurrentSecondaryGun.PickupObjectId));
    }

    public bool ContainsPickup(int id)
    {
      return this.MandatoryGunIDs.Contains(id) || this.MandatoryItemIDs.Contains(id) || this.OptionalGunIDs.Contains(id) || this.OptionalItemIDs.Contains(id);
    }

    private bool PlayerHasSynergyCompletionItem(PlayerController p)
    {
      if ((bool) (UnityEngine.Object) p)
      {
        for (int index = 0; index < p.passiveItems.Count; ++index)
        {
          if (p.passiveItems[index] is SynergyCompletionItem)
            return true;
        }
      }
      return false;
    }

    private bool PlayerHasPickup(PlayerController p, int pickupID)
    {
      if ((bool) (UnityEngine.Object) p && p.inventory != null && p.inventory.AllGuns != null)
      {
        for (int index = 0; index < p.inventory.AllGuns.Count; ++index)
        {
          if (p.inventory.AllGuns[index].PickupObjectId == pickupID)
            return true;
        }
      }
      if ((bool) (UnityEngine.Object) p)
      {
        for (int index = 0; index < p.activeItems.Count; ++index)
        {
          if (p.activeItems[index].PickupObjectId == pickupID)
            return true;
        }
        for (int index = 0; index < p.passiveItems.Count; ++index)
        {
          if (p.passiveItems[index].PickupObjectId == pickupID)
            return true;
        }
        if (pickupID == GlobalItemIds.Map && p.EverHadMap)
          return true;
      }
      return false;
    }

    public bool SynergyIsAvailable(PlayerController p, PlayerController p2, int additionalID = -1)
    {
      if (this.ActivationStatus == SynergyEntry.SynergyActivation.INACTIVE || this.ActivationStatus == SynergyEntry.SynergyActivation.DEMO)
        return false;
      bool flag1 = this.PlayerHasSynergyCompletionItem(p) || this.PlayerHasSynergyCompletionItem(p2);
      if (this.IgnoreLichEyeBullets)
        flag1 = false;
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < this.MandatoryGunIDs.Count; ++index)
      {
        if (this.PlayerHasPickup(p, this.MandatoryGunIDs[index]) || this.PlayerHasPickup(p2, this.MandatoryGunIDs[index]) || this.MandatoryGunIDs[index] == additionalID)
          ++num1;
      }
      for (int index = 0; index < this.MandatoryItemIDs.Count; ++index)
      {
        if (this.PlayerHasPickup(p, this.MandatoryItemIDs[index]) || this.PlayerHasPickup(p2, this.MandatoryItemIDs[index]) || this.MandatoryItemIDs[index] == additionalID)
          ++num2;
      }
      int num3 = 0;
      int num4 = 0;
      for (int index = 0; index < this.OptionalGunIDs.Count; ++index)
      {
        if (this.PlayerHasPickup(p, this.OptionalGunIDs[index]) || this.PlayerHasPickup(p2, this.OptionalGunIDs[index]) || this.OptionalGunIDs[index] == additionalID)
          ++num3;
      }
      for (int index = 0; index < this.OptionalItemIDs.Count; ++index)
      {
        if (this.PlayerHasPickup(p, this.OptionalItemIDs[index]) || this.PlayerHasPickup(p2, this.OptionalItemIDs[index]) || this.OptionalItemIDs[index] == additionalID)
          ++num4;
      }
      bool flag2 = this.MandatoryItemIDs.Count > 0 && this.MandatoryGunIDs.Count == 0 && this.OptionalGunIDs.Count > 0 && this.OptionalItemIDs.Count == 0;
      if ((this.MandatoryGunIDs.Count > 0 && num1 > 0 || flag2 && num3 > 0) && flag1)
      {
        ++num1;
        ++num2;
      }
      if (num1 < this.MandatoryGunIDs.Count || num2 < this.MandatoryItemIDs.Count)
        return false;
      int num5 = this.MandatoryItemIDs.Count + this.MandatoryGunIDs.Count + num3 + num4;
      int num6 = this.MandatoryGunIDs.Count + num3;
      int num7 = this.MandatoryItemIDs.Count + num4;
      if (num6 > 0 && (this.MandatoryGunIDs.Count > 0 || flag2 || this.RequiresAtLeastOneGunAndOneItem && num6 > 0) && flag1)
      {
        ++num7;
        ++num6;
        num5 += 2;
      }
      if (this.RequiresAtLeastOneGunAndOneItem && this.OptionalGunIDs.Count + this.MandatoryGunIDs.Count > 0 && this.OptionalItemIDs.Count + this.MandatoryItemIDs.Count > 0 && (num6 < 1 || num7 < 1))
        return false;
      int num8 = Mathf.Max(2, this.NumberObjectsRequired);
      return num5 >= num8;
    }
  }

