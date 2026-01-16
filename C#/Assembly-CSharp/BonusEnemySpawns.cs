// Decompiled with JetBrains decompiler
// Type: BonusEnemySpawns
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
[Serializable]
public class BonusEnemySpawns
{
  public DungeonPrerequisite[] Prereqs;
  [EnemyIdentifier]
  public string EnemyGuid;
  public WeightedIntCollection NumSpawnedChances;
  public float CastleChance = 0.2f;
  public float SewerChance;
  public float GungeonChance = 0.175f;
  public float CathedralChance;
  public float MinegeonChance = 0.15f;
  public float CatacombgeonChance = 0.125f;
  public float ForgegeonChance = 0.1f;
  public float BulletHellChance;

  public void Select(
    string name,
    Dictionary<GlobalDungeonData.ValidTilesets, int> numAssignedToFloors)
  {
    if (!DungeonPrerequisite.CheckConditionsFulfilled(this.Prereqs))
      return;
    int num1 = this.NumSpawnedChances.SelectByWeight();
    float num2 = this.CastleChance;
    float num3 = this.SewerChance;
    float num4 = this.GungeonChance;
    float num5 = this.CathedralChance;
    float num6 = this.MinegeonChance;
    float num7 = this.CatacombgeonChance;
    float num8 = this.ForgegeonChance;
    float num9 = this.BulletHellChance;
    for (int index = 0; index < num1; ++index)
    {
      float num10 = UnityEngine.Random.value * (num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9);
      GlobalDungeonData.ValidTilesets key;
      if ((double) num10 < (double) num2)
      {
        key = GlobalDungeonData.ValidTilesets.CASTLEGEON;
        num2 = 0.05f;
      }
      else if ((double) num10 < (double) num2 + (double) num3)
      {
        key = GlobalDungeonData.ValidTilesets.SEWERGEON;
        num3 = 0.05f;
      }
      else if ((double) num10 < (double) num2 + (double) num3 + (double) num4)
      {
        key = GlobalDungeonData.ValidTilesets.GUNGEON;
        num4 = 0.05f;
      }
      else if ((double) num10 < (double) num2 + (double) num3 + (double) num4 + (double) num5)
      {
        key = GlobalDungeonData.ValidTilesets.CATHEDRALGEON;
        num5 = 0.05f;
      }
      else if ((double) num10 < (double) num2 + (double) num3 + (double) num4 + (double) num5 + (double) num6)
      {
        key = GlobalDungeonData.ValidTilesets.MINEGEON;
        num6 = 0.05f;
      }
      else if ((double) num10 < (double) num2 + (double) num3 + (double) num4 + (double) num5 + (double) num6 + (double) num7)
      {
        key = GlobalDungeonData.ValidTilesets.CATACOMBGEON;
        num7 = 0.05f;
      }
      else if ((double) num10 < (double) num2 + (double) num3 + (double) num4 + (double) num5 + (double) num6 + (double) num7 + (double) num8)
      {
        key = GlobalDungeonData.ValidTilesets.FORGEGEON;
        num8 = 0.05f;
      }
      else
      {
        key = GlobalDungeonData.ValidTilesets.HELLGEON;
        num9 = 0.05f;
      }
      if (numAssignedToFloors.ContainsKey(key))
        ++numAssignedToFloors[key];
      else
        numAssignedToFloors.Add(key, 1);
    }
  }
}
