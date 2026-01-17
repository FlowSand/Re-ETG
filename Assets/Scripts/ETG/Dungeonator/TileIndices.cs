// Decompiled with JetBrains decompiler
// Type: Dungeonator.TileIndices
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class TileIndices
  {
    public GlobalDungeonData.ValidTilesets tilesetId;
    public tk2dSpriteCollectionData dungeonCollection;
    public bool dungeonCollectionSupportsDiagonalWalls;
    public AOTileIndices aoTileIndices;
    public bool placeBorders = true;
    public bool placePits = true;
    public List<TileIndexVariant> chestHighWallIndices;
    public TileIndexGrid decalIndexGrid;
    public TileIndexGrid patternIndexGrid;
    public List<int> globalSecondBorderTiles;
    public TileIndexGrid edgeDecorationTiles;

    public bool PitAtPositionIsWater(Vector2 point)
    {
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
        return false;
      RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(point.ToIntVector2());
      if (roomFromPosition.RoomFallValidForMaintenance())
        return false;
      DungeonMaterial materialDefinition = GameManager.Instance.Dungeon.roomMaterialDefinitions[roomFromPosition.RoomVisualSubtype];
      return !((UnityEngine.Object) materialDefinition == (UnityEngine.Object) null) && !((UnityEngine.Object) materialDefinition.pitfallVFXPrefab == (UnityEngine.Object) null) && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER && !GameManager.PVP_ENABLED && materialDefinition.pitfallVFXPrefab.name.Contains("Splash");
    }

    public GameObject DoSplashAtPosition(Vector2 point)
    {
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
        return (GameObject) null;
      RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(point.ToIntVector2());
      if (roomFromPosition.RoomFallValidForMaintenance())
        return (GameObject) null;
      DungeonMaterial materialDefinition = GameManager.Instance.Dungeon.roomMaterialDefinitions[roomFromPosition.RoomVisualSubtype];
      if ((UnityEngine.Object) materialDefinition == (UnityEngine.Object) null || (UnityEngine.Object) materialDefinition.pitfallVFXPrefab == (UnityEngine.Object) null)
        return (GameObject) null;
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.PVP_ENABLED)
        return (GameObject) null;
      CellData cellData = GameManager.Instance.Dungeon.data[point.ToIntVector2(VectorConversions.Floor)];
      if (cellData == null)
        return (GameObject) null;
      if ((double) UnityEngine.Time.realtimeSinceStartup - (double) cellData.lastSplashTime < 0.25)
        return (GameObject) null;
      cellData.lastSplashTime = UnityEngine.Time.realtimeSinceStartup;
      GameObject gameObject = SpawnManager.SpawnVFX(materialDefinition.pitfallVFXPrefab, point.ToVector3ZUp(), Quaternion.identity);
      tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
      component.HeightOffGround = -65f / 16f;
      component.PlaceAtPositionByAnchor((Vector3) point, tk2dBaseSprite.Anchor.MiddleCenter);
      component.transform.position = component.transform.position.Quantize(1f / (float) PhysicsEngine.Instance.PixelsPerUnit);
      component.UpdateZDepth();
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON && materialDefinition.usesFacewallGrids && cellData.type != CellType.FLOOR)
        GlobalSparksDoer.DoRandomParticleBurst(30, component.transform.position + new Vector3(-0.75f, -0.75f, 0.0f), component.transform.position + new Vector3(0.75f, 0.75f, 0.0f), Vector3.up, 90f, 0.5f, systemType: GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
      return gameObject;
    }
  }
}
