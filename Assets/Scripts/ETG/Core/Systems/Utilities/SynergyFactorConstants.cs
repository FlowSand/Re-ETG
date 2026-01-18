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

