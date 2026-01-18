using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class DungeonMaterial : ScriptableObject
  {
    public WeightedGameObjectCollection wallShards;
    public WeightedGameObjectCollection bigWallShards;
    public float bigWallShardDamageThreshold = 10f;
    public VFXComplex[] fallbackVerticalTileMapEffects;
    public VFXComplex[] fallbackHorizontalTileMapEffects;
    public GameObject pitfallVFXPrefab;
    public bool UsePitAmbientVFX;
    public List<GameObject> AmbientPitVFX;
    public float PitVFXMinCooldown = 5f;
    public float PitVFXMaxCooldown = 30f;
    public float ChanceToSpawnPitVFXOnCooldown = 1f;
    public bool UseChannelAmbientVFX;
    public float ChannelVFXMinCooldown = 1f;
    public float ChannelVFXMaxCooldown = 15f;
    public List<GameObject> AmbientChannelVFX;
    [Header("Stamp Overrides")]
    public float stampFailChance = 0.2f;
    public GenericLootTable overrideTableTable;
    [Header("Weirdo Tilemap Stuff")]
    public bool supportsPits = true;
    public bool doPitAO = true;
    [ShowInInspectorIf("doPitAO", false)]
    public bool pitsAreOneDeep;
    public bool supportsDiagonalWalls = true;
    public bool supportsUpholstery;
    public bool carpetIsMainFloor;
    public TileIndexGrid[] carpetGrids;
    public bool supportsChannels;
    public int minChannelPools;
    public int maxChannelPools = 3;
    public float channelTenacity = 0.75f;
    public TileIndexGrid[] channelGrids;
    public bool supportsLavaOrLavalikeSquares;
    public TileIndexGrid[] lavaGrids;
    public bool supportsIceSquares;
    public TileIndexGrid[] iceGrids;
    public TileIndexGrid roomFloorBorderGrid;
    public TileIndexGrid roomCeilingBorderGrid;
    public TileIndexGrid pitLayoutGrid;
    public TileIndexGrid pitBorderFlatGrid;
    public TileIndexGrid pitBorderRaisedGrid;
    public TileIndexGrid additionalPitBorderFlatGrid;
    public TileIndexGrid outerCeilingBorderGrid;
    public float floorSquareDensity = 0.05f;
    public TileIndexGrid[] floorSquares;
    public bool usesFacewallGrids;
    public FacewallIndexGridDefinition[] facewallGrids;
    public bool usesInternalMaterialTransitions;
    public bool usesProceduralMaterialTransitions;
    public RoomInternalMaterialTransition[] internalMaterialTransitions;
    public List<GameObject> secretRoomWallShardCollections;
    public bool overrideStoneFloorType;
    [ShowInInspectorIf("overrideStoneFloorType", true)]
    public CellVisualData.CellFloorType overrideFloorType;
    [Header("Lighting Data")]
    public bool useLighting = true;
    public WeightedGameObjectCollection lightPrefabs;
    public List<LightStampData> facewallLightStamps;
    public List<LightStampData> sidewallLightStamps;
    [Header("Deco Overrides")]
    public bool usesDecalLayer;
    public TileIndexGrid decalIndexGrid;
    public TilemapDecoSettings.DecoStyle decalLayerStyle;
    public int decalSize = 1;
    public int decalSpacing = 1;
    public bool usesPatternLayer;
    public TileIndexGrid patternIndexGrid;
    public TilemapDecoSettings.DecoStyle patternLayerStyle;
    public int patternSize = 1;
    public int patternSpacing = 1;
    [Header("The Wild West")]
    public bool forceEdgesDiagonal;
    public TileIndexGrid exteriorFacadeBorderGrid;
    public TileIndexGrid facadeTopGrid;
    [Header("The Sewers")]
    public TileIndexGrid bridgeGrid;

    public GameObject GetSecretRoomWallShardCollection()
    {
      return this.secretRoomWallShardCollections.Count > 0 ? this.secretRoomWallShardCollections[UnityEngine.Random.Range(0, this.secretRoomWallShardCollections.Count)] : (GameObject) null;
    }

    public TileIndexGrid GetRandomGridFromArray(TileIndexGrid[] grids)
    {
      if (grids == null)
        return (TileIndexGrid) null;
      return grids.Length == 0 ? (TileIndexGrid) null : grids[UnityEngine.Random.Range(0, grids.Length)];
    }

    public void SpawnRandomVertical(
      Vector3 position,
      float rotation,
      Transform enemy,
      Vector2 sourceNormal,
      Vector2 sourceVelocity)
    {
      VFXComplex verticalTileMapEffect = this.fallbackVerticalTileMapEffects[UnityEngine.Random.Range(0, this.fallbackVerticalTileMapEffects.Length)];
      float yPositionAtGround = (float) Mathf.FloorToInt(position.y);
      if ((double) sourceNormal.y > 0.10000000149011612)
        yPositionAtGround += 0.25f;
      verticalTileMapEffect.SpawnAtPosition(position.x, yPositionAtGround, position.y - yPositionAtGround, rotation, enemy, new Vector2?(sourceNormal), new Vector2?(sourceVelocity));
    }

    public void SpawnRandomHorizontal(
      Vector3 position,
      float rotation,
      Transform enemy,
      Vector2 sourceNormal,
      Vector2 sourceVelocity)
    {
      this.fallbackHorizontalTileMapEffects[UnityEngine.Random.Range(0, this.fallbackHorizontalTileMapEffects.Length)].SpawnAtPosition(position, rotation, enemy, new Vector2?(sourceNormal), new Vector2?(sourceVelocity));
    }

    public void SpawnRandomShard(Vector3 position, Vector2 collisionNormal)
    {
      this.InternalSpawnShard(this.wallShards.SelectByWeight(), position, collisionNormal);
    }

    public void SpawnRandomShard(Vector3 position, Vector2 collisionNormal, float damage)
    {
      this.InternalSpawnShard((double) damage <= (double) this.bigWallShardDamageThreshold || this.bigWallShards.elements.Count <= 0 ? this.wallShards.SelectByWeight() : this.bigWallShards.SelectByWeight(), position, collisionNormal);
    }

    private void InternalSpawnShard(
      GameObject shardToSpawn,
      Vector3 position,
      Vector2 collisionNormal)
    {
      if (!((UnityEngine.Object) shardToSpawn != (UnityEngine.Object) null))
        return;
      DebrisObject component = SpawnManager.SpawnDebris(shardToSpawn, position, Quaternion.identity).GetComponent<DebrisObject>();
      component.angularVelocity = UnityEngine.Random.Range(0.5f, 1.5f) * component.angularVelocity;
      float num = (double) Mathf.Abs(collisionNormal.y) <= 0.10000000149011612 ? 0.0f : 0.25f;
      component.Trigger(Quaternion.Euler(0.0f, 0.0f, (float) UnityEngine.Random.Range(-30, 30)) * collisionNormal.ToVector3ZUp() * UnityEngine.Random.Range(0.0f, 4f), UnityEngine.Random.Range(0.1f, 0.5f) + num);
    }
  }

