// Decompiled with JetBrains decompiler
// Type: Dungeonator.MetaInjectionData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  public class MetaInjectionData : ScriptableObject
  {
    public static bool CastleExcludedSetUsed = false;
    public static bool BlueprintGenerated = false;
    public static Dictionary<GlobalDungeonData.ValidTilesets, List<RuntimeInjectionMetadata>> CurrentRunBlueprint;
    public static List<ProceduralFlowModifierData> InjectionSetsUsedThisRun = new List<ProceduralFlowModifierData>();
    public static Dictionary<GlobalDungeonData.ValidTilesets, int> KeybulletsAssignedToFloors = new Dictionary<GlobalDungeonData.ValidTilesets, int>();
    public static Dictionary<GlobalDungeonData.ValidTilesets, int> ChanceBulletsAssignedToFloors = new Dictionary<GlobalDungeonData.ValidTilesets, int>();
    public static Dictionary<GlobalDungeonData.ValidTilesets, int> WallMimicsAssignedToFloors = new Dictionary<GlobalDungeonData.ValidTilesets, int>();
    public static bool ForceEarlyChest = false;
    public static bool CellGeneratedForCurrentBlueprint = false;
    public List<MetaInjectionDataEntry> entries;

    public static void ClearBlueprint()
    {
      MetaInjectionData.BlueprintGenerated = false;
      MetaInjectionData.CastleExcludedSetUsed = false;
      if (MetaInjectionData.CurrentRunBlueprint != null)
        MetaInjectionData.CurrentRunBlueprint.Clear();
      MetaInjectionData.CellGeneratedForCurrentBlueprint = false;
      if (MetaInjectionData.InjectionSetsUsedThisRun != null)
        MetaInjectionData.InjectionSetsUsedThisRun.Clear();
      MetaInjectionData.KeybulletsAssignedToFloors.Clear();
      MetaInjectionData.ChanceBulletsAssignedToFloors.Clear();
      MetaInjectionData.WallMimicsAssignedToFloors.Clear();
    }

    public static int GetNumKeybulletMenForFloor(GlobalDungeonData.ValidTilesets tileset)
    {
      return MetaInjectionData.KeybulletsAssignedToFloors.ContainsKey(tileset) ? MetaInjectionData.KeybulletsAssignedToFloors[tileset] : 0;
    }

    public static int GetNumChanceBulletMenForFloor(GlobalDungeonData.ValidTilesets tileset)
    {
      return MetaInjectionData.ChanceBulletsAssignedToFloors.ContainsKey(tileset) ? MetaInjectionData.ChanceBulletsAssignedToFloors[tileset] : 0;
    }

    public static int GetNumWallMimicsForFloor(GlobalDungeonData.ValidTilesets tileset)
    {
      return MetaInjectionData.WallMimicsAssignedToFloors.ContainsKey(tileset) ? MetaInjectionData.WallMimicsAssignedToFloors[tileset] : 0;
    }

    public void PreprocessRun(bool doDebug = false)
    {
      if (MetaInjectionData.CurrentRunBlueprint == null)
        MetaInjectionData.CurrentRunBlueprint = new Dictionary<GlobalDungeonData.ValidTilesets, List<RuntimeInjectionMetadata>>();
      MetaInjectionData.CurrentRunBlueprint.Clear();
      MetaInjectionData.KeybulletsAssignedToFloors.Clear();
      MetaInjectionData.ChanceBulletsAssignedToFloors.Clear();
      MetaInjectionData.WallMimicsAssignedToFloors.Clear();
      RewardManager rewardManager = GameManager.Instance.RewardManager;
      rewardManager.KeybulletsChances.Select("keybulletmen", MetaInjectionData.KeybulletsAssignedToFloors);
      rewardManager.ChanceBulletChances.Select("chancebulletmen", MetaInjectionData.ChanceBulletsAssignedToFloors);
      rewardManager.WallMimicChances.Select("wallmimics", MetaInjectionData.WallMimicsAssignedToFloors);
      GlobalDungeonData.ValidTilesets[] values = Enum.GetValues(typeof (GlobalDungeonData.ValidTilesets)) as GlobalDungeonData.ValidTilesets[];
      for (int index1 = 0; index1 < this.entries.Count; ++index1)
      {
        float modifiedChanceToTrigger = this.entries[index1].ModifiedChanceToTrigger;
        if ((double) modifiedChanceToTrigger >= 1.0 || (double) UnityEngine.Random.value <= (double) modifiedChanceToTrigger)
        {
          int index2 = BraveRandom.GenerationRandomRange(this.entries[index1].MinToAppearPerRun, this.entries[index1].MaxToAppearPerRun + 1);
          if (this.entries[index1].UsesWeightedNumberToAppearPerRun)
            index2 = this.entries[index1].WeightedNumberToAppear.SelectByWeight();
          List<GlobalDungeonData.ValidTilesets> validTilesetsList = new List<GlobalDungeonData.ValidTilesets>();
          for (int index3 = 0; index3 < values.Length; ++index3)
          {
            if ((!this.entries[index1].IsPartOfExcludedCastleSet || !MetaInjectionData.CastleExcludedSetUsed || values[index3] != GlobalDungeonData.ValidTilesets.CASTLEGEON) && (this.entries[index1].validTilesets | values[index3]) == this.entries[index1].validTilesets)
              validTilesetsList.Add(values[index3]);
          }
          List<int> intList = Enumerable.Range(0, validTilesetsList.Count).ToList<int>().GenerationShuffle<int>();
          for (int index4 = 0; index4 < index2; ++index4)
          {
            GlobalDungeonData.ValidTilesets key = validTilesetsList[intList[index4]];
            if (!MetaInjectionData.CurrentRunBlueprint.ContainsKey(key))
              MetaInjectionData.CurrentRunBlueprint.Add(key, new List<RuntimeInjectionMetadata>());
            if (key == GlobalDungeonData.ValidTilesets.CASTLEGEON && this.entries[index1].IsPartOfExcludedCastleSet)
              MetaInjectionData.CastleExcludedSetUsed = true;
            MetaInjectionData.CurrentRunBlueprint[key].Add(new RuntimeInjectionMetadata(this.entries[index1].injectionData));
          }
          if (this.entries[index1].AllowBonusSecret && index2 < intList.Count && (double) BraveRandom.GenerationRandomValue() < (double) this.entries[index1].ChanceForBonusSecret)
          {
            GlobalDungeonData.ValidTilesets key = validTilesetsList[intList[index2]];
            if (!MetaInjectionData.CurrentRunBlueprint.ContainsKey(key))
              MetaInjectionData.CurrentRunBlueprint.Add(key, new List<RuntimeInjectionMetadata>());
            MetaInjectionData.CurrentRunBlueprint[key].Add(new RuntimeInjectionMetadata(this.entries[index1].injectionData)
            {
              forceSecret = true
            });
          }
        }
      }
      if (GameStatsManager.Instance.isChump)
        MetaInjectionData.ForceEarlyChest = false;
      else if ((double) UnityEngine.Random.value < (double) rewardManager.EarlyChestChanceIfNotChump)
        MetaInjectionData.ForceEarlyChest = true;
      MetaInjectionData.BlueprintGenerated = true;
    }
  }
}
