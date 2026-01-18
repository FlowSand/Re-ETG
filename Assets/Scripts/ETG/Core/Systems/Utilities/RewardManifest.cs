// Decompiled with JetBrains decompiler
// Type: RewardManifest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

  public static class RewardManifest
  {
    public static void Initialize(RewardManager manager)
    {
      manager.SeededRunManifests = new Dictionary<GlobalDungeonData.ValidTilesets, FloorRewardManifest>();
      GlobalDungeonData.ValidTilesets[] values = (GlobalDungeonData.ValidTilesets[]) Enum.GetValues(typeof (GlobalDungeonData.ValidTilesets));
      for (int index1 = 0; index1 < manager.FloorRewardData.Count; ++index1)
      {
        FloorRewardData sourceData = manager.FloorRewardData[index1];
        for (int index2 = 0; index2 < values.Length; ++index2)
        {
          GlobalDungeonData.ValidTilesets key = values[index2];
          if ((sourceData.AssociatedTilesets & key) == key)
          {
            FloorRewardManifest manifestForFloor = RewardManifest.GenerateManifestForFloor(manager, sourceData);
            if (!manager.SeededRunManifests.ContainsKey(key))
              manager.SeededRunManifests.Add(key, manifestForFloor);
          }
        }
      }
    }

    public static void Reinitialize(RewardManager manager)
    {
      GlobalDungeonData.ValidTilesets[] values = (GlobalDungeonData.ValidTilesets[]) Enum.GetValues(typeof (GlobalDungeonData.ValidTilesets));
      for (int index1 = 0; index1 < manager.FloorRewardData.Count; ++index1)
      {
        FloorRewardData sourceData = manager.FloorRewardData[index1];
        for (int index2 = 0; index2 < values.Length; ++index2)
        {
          GlobalDungeonData.ValidTilesets key = values[index2];
          if ((sourceData.AssociatedTilesets & key) == key)
          {
            RewardManifest.GenerateManifestForFloor(manager, sourceData);
            if (manager.SeededRunManifests.ContainsKey(key))
              RewardManifest.RegenerateManifest(manager, manager.SeededRunManifests[key]);
          }
        }
      }
    }

    public static void ClearManifest(RewardManager manager) => manager.SeededRunManifests.Clear();

    private static FloorRewardManifest GenerateManifestForFloor(
      RewardManager manager,
      FloorRewardData sourceData)
    {
      FloorRewardManifest manifestForFloor = new FloorRewardManifest();
      manifestForFloor.Initialize(manager);
      return manifestForFloor;
    }

    private static void RegenerateManifest(RewardManager manager, FloorRewardManifest targetData)
    {
      targetData.Reinitialize(manager);
    }
  }

