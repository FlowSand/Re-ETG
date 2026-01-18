using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

public class EncounterTrackable : MonoBehaviour
  {
    [HideInInspector]
    public string ProxyEncounterGuid;
    [FormerlySerializedAs("EncounterGuid")]
    [Header("Local Settings")]
    [SerializeField]
    public string m_encounterGuid;
    [FormerlySerializedAs("DoNotificationOnEncounter")]
    [SerializeField]
    public bool m_doNotificationOnEncounter = true;
    public bool SuppressInInventory;
    [Header("Database Settings")]
    [SerializeField]
    [FormerlySerializedAs("journalData")]
    public JournalEntry m_journalData;
    [FormerlySerializedAs("IgnoreDifferentiator")]
    [SerializeField]
    public bool m_ignoreDifferentiator;
    [FormerlySerializedAs("prerequisites")]
    [SerializeField]
    public DungeonPrerequisite[] m_prerequisites;
    [SerializeField]
    [FormerlySerializedAs("UsesPurpleNotifications")]
    public bool m_usesPurpleNotifications;
    [NonSerialized]
    public bool m_hasCheckedForPickup;
    [NonSerialized]
    public bool m_hasCheckedForProxy;
    [NonSerialized]
    public EncounterDatabaseEntry m_proxyEncounterTrackable;
    private PickupObject m_pickup;

    public static bool SuppressNextNotification { get; set; }

    private void GetProxy()
    {
      if (!string.IsNullOrEmpty(this.ProxyEncounterGuid))
        this.m_proxyEncounterTrackable = EncounterDatabase.GetEntry(this.ProxyEncounterGuid);
      this.m_hasCheckedForProxy = true;
    }

    public string EncounterGuid
    {
      get
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        return this.m_proxyEncounterTrackable != null ? this.ProxyEncounterGuid : this.m_encounterGuid;
      }
      set
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        if (this.m_proxyEncounterTrackable != null)
          throw new Exception("Trying to change an EncounterTrackable via a proxy!");
        this.m_encounterGuid = value;
      }
    }

    public string TrueEncounterGuid
    {
      get => this.m_encounterGuid;
      set => this.m_encounterGuid = value;
    }

    public bool DoNotificationOnEncounter
    {
      get
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        return this.m_proxyEncounterTrackable != null ? this.m_proxyEncounterTrackable.doNotificationOnEncounter : this.m_doNotificationOnEncounter;
      }
      set
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        if (this.m_proxyEncounterTrackable != null)
          throw new Exception("Trying to change an EncounterTrackable via a proxy!");
        this.m_doNotificationOnEncounter = value;
      }
    }

    public JournalEntry journalData
    {
      get
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        return this.m_proxyEncounterTrackable != null ? this.m_proxyEncounterTrackable.journalData : this.m_journalData;
      }
      set
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        if (this.m_proxyEncounterTrackable != null)
          throw new Exception("Trying to change an EncounterTrackable via a proxy!");
        this.m_journalData = value;
      }
    }

    public bool IgnoreDifferentiator
    {
      get
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        return this.m_proxyEncounterTrackable != null ? this.m_proxyEncounterTrackable.IgnoreDifferentiator : this.m_ignoreDifferentiator;
      }
      set
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        if (this.m_proxyEncounterTrackable != null)
          throw new Exception("Trying to change an EncounterTrackable via a proxy!");
        this.m_ignoreDifferentiator = value;
      }
    }

    public DungeonPrerequisite[] prerequisites
    {
      get
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        return this.m_proxyEncounterTrackable != null ? this.m_proxyEncounterTrackable.prerequisites : this.m_prerequisites;
      }
      set
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        if (this.m_proxyEncounterTrackable != null)
          throw new Exception("Trying to change an EncounterTrackable via a proxy!");
        this.m_prerequisites = value;
      }
    }

    public bool UsesPurpleNotifications
    {
      get
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        return this.m_proxyEncounterTrackable != null ? this.m_proxyEncounterTrackable.usesPurpleNotifications : this.m_usesPurpleNotifications;
      }
      set
      {
        if (!this.m_hasCheckedForProxy)
          this.GetProxy();
        if (this.m_proxyEncounterTrackable != null)
          throw new Exception("Trying to change an EncounterTrackable via a proxy!");
        this.m_usesPurpleNotifications = value;
      }
    }

    public void Awake()
    {
      this.m_pickup = this.GetComponent<PickupObject>();
      this.m_hasCheckedForPickup = true;
    }

    public bool PrerequisitesMet()
    {
      if (this.m_prerequisites == null || this.m_prerequisites.Length == 0 || GameStatsManager.Instance.IsForceUnlocked(this.EncounterGuid))
        return true;
      for (int index = 0; index < this.m_prerequisites.Length; ++index)
      {
        if (!this.m_prerequisites[index].CheckConditionsFulfilled())
          return false;
      }
      if (!this.m_hasCheckedForPickup)
      {
        this.m_pickup = this.GetComponent<PickupObject>();
        this.m_hasCheckedForPickup = true;
      }
      return !(bool) (UnityEngine.Object) this.m_pickup || this.m_pickup.quality != PickupObject.ItemQuality.EXCLUDED;
    }

    public void HandleEncounter()
    {
      GameStatsManager.Instance.HandleEncounteredObject(this);
      if (this.m_doNotificationOnEncounter && !EncounterTrackable.SuppressNextNotification && !GameUIRoot.Instance.BossHealthBarVisible)
        GameUIRoot.Instance.DoNotification(this);
      EncounterTrackable.SuppressNextNotification = false;
    }

    public void ForceDoNotification(tk2dBaseSprite overrideSprite = null)
    {
      tk2dBaseSprite tk2dBaseSprite = !((UnityEngine.Object) overrideSprite == (UnityEngine.Object) null) ? overrideSprite : this.GetComponent<tk2dBaseSprite>();
      GameUIRoot.Instance.notificationController.DoCustomNotification(this.m_journalData.GetPrimaryDisplayName(), this.m_journalData.GetNotificationPanelDescription(), tk2dBaseSprite.Collection, tk2dBaseSprite.spriteId);
    }

    public void HandleEncounter_GeneratedObjects()
    {
      GameStatsManager.Instance.HandleEncounteredObject(this);
    }

    public string GetModifiedDisplayName()
    {
      if (!this.m_hasCheckedForPickup)
      {
        this.m_pickup = this.GetComponent<PickupObject>();
        this.m_hasCheckedForPickup = true;
      }
      string modifiedDisplayName = this.m_journalData.GetPrimaryDisplayName();
      if ((UnityEngine.Object) this.m_pickup != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) this.m_pickup.GetComponent<CursedItemModifier>() != (UnityEngine.Object) null)
          modifiedDisplayName = $"{StringTableManager.GetItemsString("#CURSED_NAMEMOD")} {modifiedDisplayName}";
        if (this.m_pickup is Gun)
        {
          if ((this.m_pickup as Gun).IsMinusOneGun)
            modifiedDisplayName += " -1";
          if ((bool) (UnityEngine.Object) this.m_pickup.GetComponent<GunderfuryController>())
            modifiedDisplayName = $"{modifiedDisplayName} Lv{IntToStringSansGarbage.GetStringForInt(GunderfuryController.GetCurrentLevel())}";
        }
      }
      return modifiedDisplayName;
    }
  }

