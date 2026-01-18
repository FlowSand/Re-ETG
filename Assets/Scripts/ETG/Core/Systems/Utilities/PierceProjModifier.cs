// Decompiled with JetBrains decompiler
// Type: PierceProjModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

