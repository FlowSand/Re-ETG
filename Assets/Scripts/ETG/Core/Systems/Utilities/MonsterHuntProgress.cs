using FullSerializer;
using System.Text;
using UnityEngine;

#nullable disable

[fsObject]
public class MonsterHuntProgress
  {
    [fsIgnore]
    private static MonsterHuntData Data;
    [fsIgnore]
    public MonsterHuntQuest ActiveQuest;
    [fsProperty]
    public int CurrentActiveMonsterHuntID = -1;
    [fsProperty]
    public int CurrentActiveMonsterHuntProgress;
    [fsIgnore]
    private StringBuilder m_sb;

    public void OnLoaded()
    {
      if ((Object) MonsterHuntProgress.Data == (Object) null)
        MonsterHuntProgress.Data = (MonsterHuntData) BraveResources.Load("Monster Hunt Data", ".asset");
      if (this.CurrentActiveMonsterHuntID != -1)
      {
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE) && GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_REWARD_GREY_MAUSER))
        {
          if (this.CurrentActiveMonsterHuntID < 0 || this.CurrentActiveMonsterHuntID >= MonsterHuntProgress.Data.ProceduralQuests.Count)
          {
            this.CurrentActiveMonsterHuntID = -1;
            this.CurrentActiveMonsterHuntProgress = 0;
          }
          else
            this.ActiveQuest = MonsterHuntProgress.Data.ProceduralQuests[this.CurrentActiveMonsterHuntID];
        }
        else if (this.CurrentActiveMonsterHuntID < 0 || this.CurrentActiveMonsterHuntID >= MonsterHuntProgress.Data.OrderedQuests.Count)
        {
          this.CurrentActiveMonsterHuntID = -1;
          this.CurrentActiveMonsterHuntProgress = 0;
        }
        else
          this.ActiveQuest = MonsterHuntProgress.Data.OrderedQuests[this.CurrentActiveMonsterHuntID];
      }
      else
        this.CurrentActiveMonsterHuntProgress = 0;
    }

    public int TriggerNextQuest()
    {
      int num = 0;
      if (this.ActiveQuest != null)
      {
        this.ActiveQuest.UnlockRewards();
        num = 5;
      }
      for (int index = 0; index < MonsterHuntProgress.Data.OrderedQuests.Count; ++index)
      {
        if (!GameStatsManager.Instance.GetFlag(MonsterHuntProgress.Data.OrderedQuests[index].QuestFlag))
        {
          this.ActiveQuest = MonsterHuntProgress.Data.OrderedQuests[index];
          this.CurrentActiveMonsterHuntID = index;
          this.CurrentActiveMonsterHuntProgress = 0;
          return num;
        }
      }
      int index1 = Random.Range(0, MonsterHuntProgress.Data.ProceduralQuests.Count);
      this.ActiveQuest = MonsterHuntProgress.Data.ProceduralQuests[index1];
      this.CurrentActiveMonsterHuntID = index1;
      this.CurrentActiveMonsterHuntProgress = 0;
      return num;
    }

    public void ProcessStatuesKill()
    {
      if (this.ActiveQuest == null || this.ActiveQuest.QuestFlag != GungeonFlags.FRIFLE_MONSTERHUNT_14_COMPLETE || this.CurrentActiveMonsterHuntProgress >= this.ActiveQuest.NumberKillsRequired)
        return;
      ++this.CurrentActiveMonsterHuntProgress;
      if (this.CurrentActiveMonsterHuntProgress < this.ActiveQuest.NumberKillsRequired)
        return;
      this.Complete();
    }

    public void ForceIncrementKillCount()
    {
      if (this.ActiveQuest == null || this.CurrentActiveMonsterHuntProgress >= this.ActiveQuest.NumberKillsRequired)
        return;
      ++this.CurrentActiveMonsterHuntProgress;
      if (this.CurrentActiveMonsterHuntProgress < this.ActiveQuest.NumberKillsRequired)
        return;
      this.Complete();
    }

    public void ProcessKill(AIActor target)
    {
      if (this.ActiveQuest == null || this.CurrentActiveMonsterHuntProgress >= this.ActiveQuest.NumberKillsRequired || !this.ActiveQuest.ContainsEnemy(target.EnemyGuid))
        return;
      ++this.CurrentActiveMonsterHuntProgress;
      if (this.CurrentActiveMonsterHuntProgress < this.ActiveQuest.NumberKillsRequired)
        return;
      this.Complete();
    }

    public string GetReplacementString()
    {
      return StringTableManager.GetEnemiesString(this.ActiveQuest.TargetStringKey);
    }

    public string GetDisplayString()
    {
      if (this.CurrentActiveMonsterHuntID < 0)
        return string.Empty;
      if (this.m_sb == null)
        this.m_sb = new StringBuilder();
      this.m_sb.Length = 0;
      this.m_sb.Append(StringTableManager.GetEnemiesString(this.ActiveQuest.TargetStringKey));
      this.m_sb.Append(" ");
      this.m_sb.Append(this.CurrentActiveMonsterHuntProgress);
      this.m_sb.Append("/");
      this.m_sb.Append(this.ActiveQuest.NumberKillsRequired);
      return this.m_sb.ToString();
    }

    public bool IsQuestComplete() => GameStatsManager.Instance.GetFlag(this.ActiveQuest.QuestFlag);

    public void Complete()
    {
      GameStatsManager.Instance.SetFlag(this.ActiveQuest.QuestFlag, true);
      if (!((Object) GameUIRoot.Instance != (Object) null) || !((Object) GameUIRoot.Instance.notificationController != (Object) null))
        return;
      tk2dSprite component = (ResourceCache.Acquire("Global VFX/Frifle_VictoryIcon") as GameObject).GetComponent<tk2dSprite>();
      GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#HUNT_COMPLETE_HEADER"), StringTableManager.GetString("#HUNT_COMPLETE_BODY"), component.Collection, component.spriteId, UINotificationController.NotificationColor.GOLD);
    }
  }

