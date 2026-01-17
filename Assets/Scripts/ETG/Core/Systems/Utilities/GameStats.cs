// Decompiled with JetBrains decompiler
// Type: GameStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [fsObject]
    public class GameStats
    {
      [fsProperty]
      private Dictionary<TrackedStats, float> stats;
      [fsProperty]
      private Dictionary<TrackedMaximums, float> maxima;
      [fsProperty]
      public HashSet<CharacterSpecificGungeonFlags> m_flags = new HashSet<CharacterSpecificGungeonFlags>();

      public GameStats()
      {
        this.stats = new Dictionary<TrackedStats, float>((IEqualityComparer<TrackedStats>) new TrackedStatsComparer());
        this.maxima = new Dictionary<TrackedMaximums, float>((IEqualityComparer<TrackedMaximums>) new TrackedMaximumsComparer());
      }

      public float GetStatValue(TrackedStats statToCheck)
      {
        return !this.stats.ContainsKey(statToCheck) ? 0.0f : this.stats[statToCheck];
      }

      public float GetMaximumValue(TrackedMaximums maxToCheck)
      {
        return !this.maxima.ContainsKey(maxToCheck) ? 0.0f : this.maxima[maxToCheck];
      }

      public bool GetFlag(CharacterSpecificGungeonFlags flag)
      {
        if (flag != CharacterSpecificGungeonFlags.NONE)
          return this.m_flags.Contains(flag);
        Debug.LogError((object) "Something is attempting to get a NONE character-specific save flag!");
        return false;
      }

      public void SetStat(TrackedStats stat, float val)
      {
        if (this.stats.ContainsKey(stat))
          this.stats[stat] = val;
        else
          this.stats.Add(stat, val);
      }

      public void SetMax(TrackedMaximums max, float val)
      {
        if (this.maxima.ContainsKey(max))
          this.maxima[max] = Mathf.Max(this.maxima[max], val);
        else
          this.maxima.Add(max, val);
      }

      public void SetFlag(CharacterSpecificGungeonFlags flag, bool value)
      {
        if (flag == CharacterSpecificGungeonFlags.NONE)
          Debug.LogError((object) "Something is attempting to set a NONE character-specific save flag!");
        else if (value)
          this.m_flags.Add(flag);
        else
          this.m_flags.Remove(flag);
      }

      public void IncrementStat(TrackedStats stat, float val)
      {
        if (this.stats.ContainsKey(stat))
          this.stats[stat] = this.stats[stat] + val;
        else
          this.stats.Add(stat, val);
      }

      public void AddStats(GameStats otherStats)
      {
        foreach (KeyValuePair<TrackedStats, float> stat in otherStats.stats)
          this.IncrementStat(stat.Key, stat.Value);
        foreach (KeyValuePair<TrackedMaximums, float> maximum in otherStats.maxima)
          this.SetMax(maximum.Key, maximum.Value);
        foreach (CharacterSpecificGungeonFlags flag in otherStats.m_flags)
          this.m_flags.Add(flag);
      }

      public void ClearAllState()
      {
        List<TrackedStats> trackedStatsList = new List<TrackedStats>();
        foreach (KeyValuePair<TrackedStats, float> stat in this.stats)
          trackedStatsList.Add(stat.Key);
        foreach (TrackedStats key in trackedStatsList)
          this.stats[key] = 0.0f;
        List<TrackedMaximums> trackedMaximumsList = new List<TrackedMaximums>();
        foreach (KeyValuePair<TrackedMaximums, float> maximum in this.maxima)
          trackedMaximumsList.Add(maximum.Key);
        foreach (TrackedMaximums key in trackedMaximumsList)
          this.maxima[key] = 0.0f;
      }
    }

}
