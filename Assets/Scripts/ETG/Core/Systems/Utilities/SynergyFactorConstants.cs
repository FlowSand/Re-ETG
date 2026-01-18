// Decompiled with JetBrains decompiler
// Type: SynergyFactorConstants
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

  public static class SynergyFactorConstants
  {
    public static float GetSynergyFactor()
    {
      int encounteredThisRun = GameStatsManager.Instance.GetNumberOfSynergiesEncounteredThisRun();
      float num1 = 0.6f;
      float num2 = (float) (0.006260341964662075 + 0.99359208345413208 * (double) Mathf.Exp(-1.626339f * (float) encounteredThisRun));
      if (encounteredThisRun == 0)
        num1 = (double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_FORGE) < 3.0 ? ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_CATACOMBS) < 3.0 ? ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_MINES) < 3.0 ? ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIMES_REACHED_GUNGEON) < 3.0 ? 5f : 3f) : 1.5f) : 1f) : 0.8f;
      return (float) (1.0 + (double) num1 * (double) num2);
    }
  }

