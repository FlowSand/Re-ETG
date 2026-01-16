// Decompiled with JetBrains decompiler
// Type: JournalEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
[Serializable]
public class JournalEntry
{
  public bool SuppressInAmmonomicon;
  public bool SuppressKnownState;
  public bool DisplayOnLoadingScreen;
  public bool RequiresLightBackgroundInLoadingScreen;
  [StringTableString("items")]
  public string PrimaryDisplayName;
  [StringTableString("items")]
  public string NotificationPanelDescription;
  [StringTableString("items")]
  public string AmmonomiconFullEntry;
  [FormerlySerializedAs("AlternateAmmonomiconButtonSpriteName")]
  public string AmmonomiconSprite = string.Empty;
  public bool IsEnemy;
  public Texture2D enemyPortraitSprite;
  public JournalEntry.CustomJournalEntryType SpecialIdentifier;
  [NonSerialized]
  private string m_cachedPrimaryDisplayName;
  [NonSerialized]
  private string m_cachedNotificationPanelDescription;
  [NonSerialized]
  private string m_cachedAmmonomiconFullEntry;
  [NonSerialized]
  private int PrivateSemaphoreValue;

  public static int ReloadDataSemaphore { get; set; }

  private void CheckSemaphore()
  {
    if (this.PrivateSemaphoreValue >= JournalEntry.ReloadDataSemaphore)
      return;
    this.m_cachedPrimaryDisplayName = (string) null;
    this.m_cachedNotificationPanelDescription = (string) null;
    this.m_cachedAmmonomiconFullEntry = (string) null;
    this.PrivateSemaphoreValue = JournalEntry.ReloadDataSemaphore;
  }

  public string GetAmmonomiconFullEntry(bool isInfiniteAmmoGun, bool doesntDamageSecretWalls)
  {
    this.CheckSemaphore();
    if (string.IsNullOrEmpty(this.m_cachedAmmonomiconFullEntry))
    {
      if (this.SpecialIdentifier == JournalEntry.CustomJournalEntryType.RESOURCEFUL_RAT)
      {
        string key = "#RESOURCEFULRAT_AGD_LONGDESC_PREKILL";
        if (Application.isPlaying && GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_RESOURCEFULRAT))
          key = "#RESOURCEFULRAT_AGD_LONGDESC_POSTKILL";
        string str = this.HandleLongDescSuffix();
        return StringTableManager.GetEnemiesLongDescription(key) + str;
      }
      if (string.IsNullOrEmpty(this.AmmonomiconFullEntry))
        return string.Empty;
      if (this.IsEnemy)
      {
        this.m_cachedAmmonomiconFullEntry = StringTableManager.GetEnemiesLongDescription(this.AmmonomiconFullEntry);
      }
      else
      {
        string key = this.AmmonomiconFullEntry;
        if (this.AmmonomiconFullEntry == "#PIGITEM1_LONGDESC" && Application.isPlaying && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_HERO_PIG))
          key = "#PIGITEM2_LONGDESC";
        if (this.AmmonomiconFullEntry == "#BRACELETRED_LONGDESC" && Application.isPlaying && GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_RECEIVED_RUBY_BRACELET))
          key = "#BRACELETRED_LONGDESC_V2";
        string str1 = string.Empty;
        if (isInfiniteAmmoGun)
          str1 = $"{str1}{StringTableManager.GetItemsString("#INFINITEAMMO_TEXT")} ";
        if (doesntDamageSecretWalls)
          str1 = $"{str1}{StringTableManager.GetItemsString("#NOSECRETS_TEXT")} ";
        string str2 = this.HandleLongDescSuffix();
        this.m_cachedAmmonomiconFullEntry = str1 + StringTableManager.GetItemsLongDescription(key) + str2;
      }
    }
    return this.m_cachedAmmonomiconFullEntry;
  }

  private string HandleLongDescSuffix()
  {
    string str = string.Empty;
    if (this.SpecialIdentifier != JournalEntry.CustomJournalEntryType.NONE && Application.isPlaying)
    {
      DungeonData.Direction[] resourcefulRatSolution = GameManager.GetResourcefulRatSolution();
      if (this.SpecialIdentifier == JournalEntry.CustomJournalEntryType.RAT_NOTE_01 && Application.isPlaying)
        str = this.GetRatSpriteFromDirection(resourcefulRatSolution[0]);
      else if (this.SpecialIdentifier == JournalEntry.CustomJournalEntryType.RAT_NOTE_02 && Application.isPlaying)
        str = this.GetRatSpriteFromDirection(resourcefulRatSolution[1]);
      else if (this.SpecialIdentifier == JournalEntry.CustomJournalEntryType.RAT_NOTE_03 && Application.isPlaying)
        str = this.GetRatSpriteFromDirection(resourcefulRatSolution[2]);
      else if (this.SpecialIdentifier == JournalEntry.CustomJournalEntryType.RAT_NOTE_04 && Application.isPlaying)
        str = this.GetRatSpriteFromDirection(resourcefulRatSolution[3]);
      else if (this.SpecialIdentifier == JournalEntry.CustomJournalEntryType.RAT_NOTE_05 && Application.isPlaying)
        str = this.GetRatSpriteFromDirection(resourcefulRatSolution[4]);
      else if (this.SpecialIdentifier == JournalEntry.CustomJournalEntryType.RAT_NOTE_06 && Application.isPlaying)
        str = this.GetRatSpriteFromDirection(resourcefulRatSolution[5]);
    }
    return str;
  }

  private string GetRatSpriteFromDirection(DungeonData.Direction dir)
  {
    switch (dir)
    {
      case DungeonData.Direction.NORTH:
        return "[sprite \"resourcefulrat_text_note_001\"]";
      case DungeonData.Direction.EAST:
        return "[sprite \"resourcefulrat_text_note_002\"]";
      case DungeonData.Direction.SOUTH:
        return "[sprite \"resourcefulrat_text_note_003\"]";
      case DungeonData.Direction.WEST:
        return "[sprite \"resourcefulrat_text_note_004\"]";
      default:
        return string.Empty;
    }
  }

  public string GetPrimaryDisplayName(bool duringStartup = false)
  {
    this.CheckSemaphore();
    if (string.IsNullOrEmpty(this.m_cachedPrimaryDisplayName))
    {
      if (this.IsEnemy)
      {
        this.m_cachedPrimaryDisplayName = StringTableManager.GetEnemiesString(this.PrimaryDisplayName, 0);
      }
      else
      {
        string key = this.PrimaryDisplayName;
        if (Application.isPlaying && !duringStartup && this.PrimaryDisplayName == "#PIGITEM1_ENCNAME" && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_HERO_PIG))
          key = "#PIGITEM2_ENCNAME";
        this.m_cachedPrimaryDisplayName = StringTableManager.GetItemsString(key, 0);
      }
    }
    return this.m_cachedPrimaryDisplayName;
  }

  public string GetNotificationPanelDescription()
  {
    this.CheckSemaphore();
    if (string.IsNullOrEmpty(this.m_cachedNotificationPanelDescription))
    {
      if (this.IsEnemy)
      {
        this.m_cachedNotificationPanelDescription = StringTableManager.GetEnemiesString(this.NotificationPanelDescription, 0);
      }
      else
      {
        string key = this.NotificationPanelDescription;
        if (this.NotificationPanelDescription == "#PIGITEM1_SHORTDESC" && Application.isPlaying && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_HERO_PIG))
          key = "#PIGITEM2_SHORTDESC";
        if (this.NotificationPanelDescription == "#BRACELETRED_SHORTDESC" && Application.isPlaying && GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_RECEIVED_RUBY_BRACELET))
          key = "#BRACELETRED_SHORTDESC_V2";
        if (this.NotificationPanelDescription == "#JUNK_SHORTDESC" && SackKnightController.HasJunkan())
          key = "#JUNKSHRINE_SHORTDESC";
        this.m_cachedNotificationPanelDescription = StringTableManager.GetItemsString(key, 0);
      }
    }
    return this.m_cachedNotificationPanelDescription;
  }

  public string GetCustomNotificationPanelDescription(int index)
  {
    this.CheckSemaphore();
    return this.IsEnemy ? StringTableManager.GetEnemiesString(this.NotificationPanelDescription, index) : StringTableManager.GetItemsString(this.NotificationPanelDescription, index);
  }

  protected bool Equals(JournalEntry other)
  {
    return this.SuppressInAmmonomicon.Equals(other.SuppressInAmmonomicon) && this.DisplayOnLoadingScreen.Equals(other.DisplayOnLoadingScreen) && this.RequiresLightBackgroundInLoadingScreen.Equals(other.RequiresLightBackgroundInLoadingScreen) && string.Equals(this.PrimaryDisplayName, other.PrimaryDisplayName) && string.Equals(this.NotificationPanelDescription, other.NotificationPanelDescription) && string.Equals(this.AmmonomiconFullEntry, other.AmmonomiconFullEntry) && string.Equals(this.AmmonomiconSprite, other.AmmonomiconSprite) && this.IsEnemy.Equals(other.IsEnemy) && this.SuppressKnownState.Equals(other.SuppressKnownState) && object.Equals((object) this.enemyPortraitSprite, (object) other.enemyPortraitSprite);
  }

  public override bool Equals(object obj)
  {
    if (object.ReferenceEquals((object) null, obj))
      return false;
    if (object.ReferenceEquals((object) this, obj))
      return true;
    return obj.GetType() == this.GetType() && this.Equals((JournalEntry) obj);
  }

  public override int GetHashCode()
  {
    return (((((((this.SuppressInAmmonomicon.GetHashCode() * 397 ^ this.DisplayOnLoadingScreen.GetHashCode()) * 397 ^ this.RequiresLightBackgroundInLoadingScreen.GetHashCode()) * 397 ^ (this.PrimaryDisplayName == null ? 0 : this.PrimaryDisplayName.GetHashCode())) * 397 ^ (this.NotificationPanelDescription == null ? 0 : this.NotificationPanelDescription.GetHashCode())) * 397 ^ (this.AmmonomiconFullEntry == null ? 0 : this.AmmonomiconFullEntry.GetHashCode())) * 397 ^ (this.AmmonomiconSprite == null ? 0 : this.AmmonomiconSprite.GetHashCode())) * 397 ^ this.IsEnemy.GetHashCode()) * 397 ^ (!((UnityEngine.Object) this.enemyPortraitSprite != (UnityEngine.Object) null) ? 0 : this.enemyPortraitSprite.GetHashCode());
  }

  public JournalEntry Clone()
  {
    return new JournalEntry()
    {
      SuppressInAmmonomicon = this.SuppressInAmmonomicon,
      DisplayOnLoadingScreen = this.DisplayOnLoadingScreen,
      RequiresLightBackgroundInLoadingScreen = this.RequiresLightBackgroundInLoadingScreen,
      PrimaryDisplayName = this.PrimaryDisplayName,
      NotificationPanelDescription = this.NotificationPanelDescription,
      AmmonomiconFullEntry = this.AmmonomiconFullEntry,
      AmmonomiconSprite = this.AmmonomiconSprite,
      IsEnemy = this.IsEnemy,
      enemyPortraitSprite = this.enemyPortraitSprite,
      SuppressKnownState = this.SuppressKnownState,
      SpecialIdentifier = this.SpecialIdentifier
    };
  }

  public void ClearCache()
  {
    this.m_cachedPrimaryDisplayName = (string) null;
    this.m_cachedNotificationPanelDescription = (string) null;
    this.m_cachedAmmonomiconFullEntry = (string) null;
  }

  public enum CustomJournalEntryType
  {
    NONE = 0,
    RAT_NOTE_01 = 101, // 0x00000065
    RAT_NOTE_02 = 102, // 0x00000066
    RAT_NOTE_03 = 103, // 0x00000067
    RAT_NOTE_04 = 104, // 0x00000068
    RAT_NOTE_05 = 105, // 0x00000069
    RAT_NOTE_06 = 106, // 0x0000006A
    RESOURCEFUL_RAT = 107, // 0x0000006B
  }
}
