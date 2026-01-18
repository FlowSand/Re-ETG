using UnityEngine;

#nullable disable

public class PierceProjModifier : MonoBehaviour
  {
    public int penetration = 1;
    public bool penetratesBreakables;
    public bool preventPenetrationOfActors;
    public PierceProjModifier.BeastModeStatus BeastModeLevel;
    public bool UsesMaxBossImpacts;
    public int MaxBossImpacts = -1;
    private int m_bossImpacts;

    public bool HandleBossImpact()
    {
      if (this.UsesMaxBossImpacts)
      {
        ++this.m_bossImpacts;
        if (this.m_bossImpacts >= this.MaxBossImpacts)
          return true;
      }
      return false;
    }

    public enum BeastModeStatus
    {
      NOT_BEAST_MODE,
      BEAST_MODE_LEVEL_ONE,
    }
  }

