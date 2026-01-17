// Decompiled with JetBrains decompiler
// Type: Dungeonator.DungeonPlaceableRoomMaterialRequirement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class DungeonPlaceableRoomMaterialRequirement
  {
    public GlobalDungeonData.ValidTilesets TargetTileset = GlobalDungeonData.ValidTilesets.CASTLEGEON;
    [PrettyDungeonMaterial("TargetTileset")]
    public int RoomMaterial;
    public bool RequireMaterial = true;
  }
}
