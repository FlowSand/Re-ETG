using FullInspector;
using System;
using UnityEngine;

#nullable disable

[Serializable]
public class EncounterDatabaseEntry : AssetBundleDatabaseEntry
  {
    [InspectorDisabled]
    public string ProxyEncounterGuid;
    [InspectorDisabled]
    public bool doNotificationOnEncounter;
    [InspectorDisabled]
    public bool IgnoreDifferentiator;
    [InspectorDisabled]
    public DungeonPrerequisite[] prerequisites;
    [InspectorDisabled]
    public JournalEntry journalData;
    [InspectorDisabled]
    public bool usesPurpleNotifications;
    [InspectorDisabled]
    public int pickupObjectId = -1;
    [InspectorDisabled]
    public int shootStyleInt = -1;
    [InspectorDisabled]
    public bool isPlayerItem;
    [InspectorDisabled]
    public bool isPassiveItem;
    [InspectorDisabled]
    public bool isInfiniteAmmoGun;
    [InspectorDisabled]
    public bool doesntDamageSecretWalls;

    public EncounterDatabaseEntry()
    {
    }

    public EncounterDatabaseEntry(EncounterTrackable encounterTrackable)
    {
      this.myGuid = encounterTrackable.EncounterGuid;
      this.SetAll(encounterTrackable);
    }

    [InspectorDisabled]
    public bool ForceEncounterState { get; set; }

    public override AssetBundle assetBundle => EncounterDatabase.AssetBundle;

    public bool PrerequisitesMet()
    {
      if (this.prerequisites == null || this.prerequisites.Length == 0 || GameStatsManager.Instance.IsForceUnlocked(this.myGuid))
        return true;
      for (int index = 0; index < this.prerequisites.Length; ++index)
      {
        if (!this.prerequisites[index].CheckConditionsFulfilled())
          return false;
      }
      return true;
    }

    public string GetSecondTapeDescriptor()
    {
      string secondTapeDescriptor = string.Empty;
      if (this.shootStyleInt >= 0)
        secondTapeDescriptor = this.GetShootStyleString((ProjectileModule.ShootStyle) this.shootStyleInt);
      else if (this.isPlayerItem)
        secondTapeDescriptor = StringTableManager.GetItemsString("#ITEMSTYLE_ACTIVE");
      else if (this.isPassiveItem)
        secondTapeDescriptor = StringTableManager.GetItemsString("#ITEMSTYLE_PASSIVE");
      return secondTapeDescriptor;
    }

    public string GetModifiedLongDescription()
    {
      return this.journalData.GetAmmonomiconFullEntry(this.isInfiniteAmmoGun, this.doesntDamageSecretWalls);
    }

    private string GetShootStyleString(ProjectileModule.ShootStyle shootStyle)
    {
      switch (shootStyle)
      {
        case ProjectileModule.ShootStyle.SemiAutomatic:
          return StringTableManager.GetItemsString("#SHOOTSTYLE_SEMIAUTOMATIC");
        case ProjectileModule.ShootStyle.Automatic:
          return StringTableManager.GetItemsString("#SHOOTSTYLE_AUTOMATIC");
        case ProjectileModule.ShootStyle.Beam:
          return StringTableManager.GetItemsString("#SHOOTSTYLE_BEAM");
        case ProjectileModule.ShootStyle.Charged:
          return StringTableManager.GetItemsString("#SHOOTSTYLE_CHARGE");
        case ProjectileModule.ShootStyle.Burst:
          return StringTableManager.GetItemsString("#SHOOTSTYLE_BURST");
        default:
          return string.Empty;
      }
    }

    public void SetAll(EncounterTrackable encounterTrackable)
    {
      this.ProxyEncounterGuid = encounterTrackable.ProxyEncounterGuid;
      this.doNotificationOnEncounter = encounterTrackable.DoNotificationOnEncounter;
      this.IgnoreDifferentiator = encounterTrackable.IgnoreDifferentiator;
      this.prerequisites = encounterTrackable.prerequisites;
      this.journalData = encounterTrackable.journalData.Clone();
      this.usesPurpleNotifications = encounterTrackable.UsesPurpleNotifications;
      PickupObject component = encounterTrackable.GetComponent<PickupObject>();
      this.pickupObjectId = !(bool) (UnityEngine.Object) component ? -1 : component.PickupObjectId;
      Gun gun = component as Gun;
      this.shootStyleInt = !(bool) (UnityEngine.Object) gun ? -1 : (int) gun.DefaultModule.shootStyle;
      this.isPlayerItem = (bool) (UnityEngine.Object) encounterTrackable.GetComponent<PlayerItem>();
      this.isPassiveItem = (bool) (UnityEngine.Object) encounterTrackable.GetComponent<PassiveItem>();
      this.isInfiniteAmmoGun = (bool) (UnityEngine.Object) gun && gun.InfiniteAmmo;
      this.doesntDamageSecretWalls = (bool) (UnityEngine.Object) gun && gun.InfiniteAmmo;
    }

    public bool Equals(EncounterTrackable other)
    {
      if ((UnityEngine.Object) other == (UnityEngine.Object) null || this.ProxyEncounterGuid != other.ProxyEncounterGuid || this.doNotificationOnEncounter != other.DoNotificationOnEncounter || this.IgnoreDifferentiator != other.IgnoreDifferentiator || !EncounterDatabaseEntry.PrereqArraysEqual(this.prerequisites, other.prerequisites) || !this.journalData.Equals((object) other.journalData) || this.usesPurpleNotifications != other.UsesPurpleNotifications)
        return false;
      PickupObject component = other.GetComponent<PickupObject>();
      if (this.pickupObjectId != (!(bool) (UnityEngine.Object) component ? -1 : component.PickupObjectId))
        return false;
      Gun gun = component as Gun;
      return this.shootStyleInt == (!(bool) (UnityEngine.Object) gun ? -1 : (int) gun.DefaultModule.shootStyle) && this.isPlayerItem == (bool) (UnityEngine.Object) other.GetComponent<PlayerItem>() && this.isPassiveItem == (bool) (UnityEngine.Object) other.GetComponent<PassiveItem>() && (this.isInfiniteAmmoGun ? 1 : 0) == (!(bool) (UnityEngine.Object) gun ? 0 : (gun.InfiniteAmmo ? 1 : 0)) && (this.doesntDamageSecretWalls ? 1 : 0) == (!(bool) (UnityEngine.Object) gun ? 0 : (gun.InfiniteAmmo ? 1 : 0));
    }

    private static bool PrereqArraysEqual(DungeonPrerequisite[] a, DungeonPrerequisite[] b)
    {
      if (a == null && b != null || b == null && a != null || a.Length != b.Length)
        return false;
      for (int index = 0; index < a.Length; ++index)
      {
        if (!a[index].Equals((object) b[index]))
          return false;
      }
      return true;
    }
  }

