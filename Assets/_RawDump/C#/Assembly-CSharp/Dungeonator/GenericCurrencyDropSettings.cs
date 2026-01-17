// Decompiled with JetBrains decompiler
// Type: Dungeonator.GenericCurrencyDropSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Dungeonator;

[Serializable]
public class GenericCurrencyDropSettings
{
  [PickupIdentifier]
  public int bronzeCoinId = -1;
  [PickupIdentifier]
  public int silverCoinId = -1;
  [PickupIdentifier]
  public int goldCoinId = -1;
  [PickupIdentifier]
  public int metaCoinId = -1;
  public WeightedIntCollection blackPhantomCoinDropChances;

  public GameObject bronzeCoinPrefab => PickupObjectDatabase.GetById(this.bronzeCoinId).gameObject;

  public GameObject silverCoinPrefab => PickupObjectDatabase.GetById(this.silverCoinId).gameObject;

  public GameObject goldCoinPrefab => PickupObjectDatabase.GetById(this.goldCoinId).gameObject;

  public GameObject metaCoinPrefab => PickupObjectDatabase.GetById(this.metaCoinId).gameObject;
}
