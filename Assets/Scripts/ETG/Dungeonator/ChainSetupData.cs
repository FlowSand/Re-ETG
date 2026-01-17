// Decompiled with JetBrains decompiler
// Type: Dungeonator.ChainSetupData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Dungeonator;

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
