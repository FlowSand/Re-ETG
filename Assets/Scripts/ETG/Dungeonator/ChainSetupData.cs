using System;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class ChainSetupData
  {
    [SerializeField]
    public DungeonChain chain;
    [SerializeField]
    public int minSubchainsToBuild;
    [SerializeField]
    public int maxSubchainsToBuild = 3;
    [SerializeField]
    public ChainSetupData[] childChains;
    [SerializeField]
    public ChainSetupData.ExitPreferenceMetric exitMetric;

    public enum ExitPreferenceMetric
    {
      RANDOM,
      HORIZONTAL,
      VERTICAL,
      FARTHEST,
      NEAREST,
    }
  }
}
