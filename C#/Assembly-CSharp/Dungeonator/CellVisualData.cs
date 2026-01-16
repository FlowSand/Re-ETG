// Decompiled with JetBrains decompiler
// Type: Dungeonator.CellVisualData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Dungeonator;

public struct CellVisualData
{
  public int roomVisualTypeIndex;
  public bool isDecal;
  public bool isPattern;
  public bool IsChannel;
  public bool IsPhantomCarpet;
  public CellVisualData.CellFloorType floorType;
  public bool absorbsDebris;
  public bool facewallGridPreventsWallSpaceStamp;
  public bool containsWallSpaceStamp;
  public bool containsObjectSpaceStamp;
  public DungeonTileStampData.IntermediaryMatchingStyle forcedMatchingStyle;
  public bool precludeAllTileDrawing;
  public bool shouldIgnoreWallDrawing;
  public bool shouldIgnoreBorders;
  public bool hasAlreadyBeenTilemapped;
  public bool hasBeenLit;
  public bool floorTileOverridden;
  public bool preventFloorStamping;
  public int doorFeetOverrideMode;
  public bool containsLight;
  public GameObject lightObject;
  public LightStampData facewallLightStampData;
  public LightStampData sidewallLightStampData;
  public DungeonData.Direction lightDirection;
  public int distanceToNearestLight;
  public int faceWallOverrideIndex;
  public int pitOverrideIndex;
  public int inheritedOverrideIndex;
  public bool inheritedOverrideIndexIsFloor;
  public bool ceilingHasBeenProcessed;
  public bool occlusionHasBeenProcessed;
  public bool hasStampedPath;
  public int pathTilesetGridIndex;
  public bool IsFacewallForInteriorTransition;
  public int InteriorTransitionIndex;
  public bool IsFeatureCell;
  public bool IsFeatureAdditional;
  public bool UsesCustomIndexOverride01;
  public int CustomIndexOverride01Layer;
  public int CustomIndexOverride01;
  public bool RequiresPitBordering;
  public bool HasTriggeredPitVFX;
  public float PitVFXCooldown;
  public float PitParticleCooldown;
  public int RatChunkBorderIndex;

  public void CopyFrom(CellVisualData other)
  {
    this.roomVisualTypeIndex = other.roomVisualTypeIndex;
    this.isDecal = other.isDecal;
    this.isPattern = other.isPattern;
    this.IsPhantomCarpet = other.IsPhantomCarpet;
    this.floorType = other.floorType;
    this.precludeAllTileDrawing = other.precludeAllTileDrawing;
    this.shouldIgnoreWallDrawing = other.shouldIgnoreWallDrawing;
    this.shouldIgnoreBorders = other.shouldIgnoreBorders;
    this.floorTileOverridden = other.floorTileOverridden;
    this.preventFloorStamping = other.preventFloorStamping;
    this.faceWallOverrideIndex = other.faceWallOverrideIndex;
    this.pitOverrideIndex = other.pitOverrideIndex;
    this.inheritedOverrideIndex = other.inheritedOverrideIndex;
    this.inheritedOverrideIndexIsFloor = other.inheritedOverrideIndexIsFloor;
    this.IsFeatureCell = other.IsFeatureCell;
    this.IsFeatureAdditional = other.IsFeatureAdditional;
    this.UsesCustomIndexOverride01 = other.UsesCustomIndexOverride01;
    this.CustomIndexOverride01Layer = other.CustomIndexOverride01Layer;
    this.CustomIndexOverride01 = other.CustomIndexOverride01;
    this.RatChunkBorderIndex = other.RatChunkBorderIndex;
  }

  public enum CellFloorType
  {
    Stone,
    Water,
    Carpet,
    Ice,
    Grass,
    Bone,
    Flesh,
    ThickGoop,
  }
}
