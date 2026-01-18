// Decompiled with JetBrains decompiler
// Type: GlobalDungeonData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

  public static class GlobalDungeonData
  {
    public static int COMMON_BASE_PRICE = 20;
    public static int D_BASE_PRICE = 35;
    public static int C_BASE_PRICE = 45;
    public static int B_BASE_PRICE = 65;
    public static int A_BASE_PRICE = 90;
    public static int S_BASE_PRICE = 120;
    public static int occlusionPartitionIndex = 0;
    public static int pitLayerIndex = 1;
    public static int floorLayerIndex = 2;
    public static int patternLayerIndex = 3;
    public static int decalLayerIndex = 4;
    public static int actorCollisionLayerIndex = 5;
    public static int collisionLayerIndex = 6;
    public static int wallStampLayerIndex = 7;
    public static int objectStampLayerIndex = 8;
    public static int shadowLayerIndex = 9;
    public static int killLayerIndex = 10;
    public static int ceilingLayerIndex = 11;
    public static int borderLayerIndex = 12;
    public static int aboveBorderLayerIndex = 13;
    public static bool GUNGEON_EXPERIMENTAL = false;
    public static readonly string[] TilesetPaths = new string[16 /*0x10*/]
    {
      "Assets\\Sprites\\Collections\\ENV_Tileset_Gungeon.prefab",
      "Assets\\Sprites\\Collections\\ENV_Tileset_Castle.prefab",
      "Assets\\Sprites\\Collections\\ENV_Tileset_Sewer.prefab",
      "Assets\\Sprites\\Collections\\ENV_Tileset_Cathedral.prefab",
      "Assets\\Sprites\\Collections\\ENV_Tileset_Mines.prefab",
      "Assets\\Sprites\\Collections\\ENV_Tileset_Catacombs.prefab",
      "Assets\\Sprites\\Collections\\ENV_Tileset_Forge.prefab",
      "Assets\\Sprites\\Collections\\ENV_Tileset_BulletHell.prefab",
      string.Empty,
      string.Empty,
      string.Empty,
      "Assets\\Sprites\\Collections\\ENV_Tileset_Nakatomi.prefab",
      string.Empty,
      string.Empty,
      string.Empty,
      "Assets\\Sprites\\Collections\\Dolphin Tilesets\\ENV_Tileset_Rat.prefab"
    };

    public static int GetBasePrice(PickupObject.ItemQuality quality)
    {
      switch (quality)
      {
        case PickupObject.ItemQuality.COMMON:
          return GlobalDungeonData.COMMON_BASE_PRICE;
        case PickupObject.ItemQuality.D:
          return GlobalDungeonData.D_BASE_PRICE;
        case PickupObject.ItemQuality.C:
          return GlobalDungeonData.C_BASE_PRICE;
        case PickupObject.ItemQuality.B:
          return GlobalDungeonData.B_BASE_PRICE;
        case PickupObject.ItemQuality.A:
          return GlobalDungeonData.A_BASE_PRICE;
        case PickupObject.ItemQuality.S:
          return GlobalDungeonData.S_BASE_PRICE;
        default:
          if (Application.isPlaying)
            Debug.LogError((object) $"Invalid quality : {(object) quality} in GetBasePrice");
          return GlobalDungeonData.S_BASE_PRICE;
      }
    }

[Flags]
public enum ValidTilesets
    {
      GUNGEON = 1,
      CASTLEGEON = 2,
      SEWERGEON = 4,
      CATHEDRALGEON = 8,
      MINEGEON = 16, // 0x00000010
      CATACOMBGEON = 32, // 0x00000020
      FORGEGEON = 64, // 0x00000040
      HELLGEON = 128, // 0x00000080
      SPACEGEON = 256, // 0x00000100
      PHOBOSGEON = 512, // 0x00000200
      WESTGEON = 1024, // 0x00000400
      OFFICEGEON = 2048, // 0x00000800
      BELLYGEON = 4096, // 0x00001000
      JUNGLEGEON = 8192, // 0x00002000
      FINALGEON = 16384, // 0x00004000
      RATGEON = 32768, // 0x00008000
    }
  }

