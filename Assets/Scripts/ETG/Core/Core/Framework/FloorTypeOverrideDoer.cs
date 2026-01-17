// Decompiled with JetBrains decompiler
// Type: FloorTypeOverrideDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class FloorTypeOverrideDoer : BraveBehaviour, IPlaceConfigurable
    {
      public FloorTypeOverrideDoer.OverrideMode overrideMode = FloorTypeOverrideDoer.OverrideMode.Placeable;
      [ShowInInspectorIf("overrideMode", 0, true)]
      public int xStartOffset;
      [ShowInInspectorIf("overrideMode", 0, true)]
      public int yStartOffset;
      [ShowInInspectorIf("overrideMode", 0, true)]
      public int width = 1;
      [ShowInInspectorIf("overrideMode", 0, true)]
      public int height = 1;
      public bool overrideCellFloorType;
      [ShowInInspectorIf("overrideCellFloorType", true)]
      [FormerlySerializedAs("overrideType")]
      public CellVisualData.CellFloorType cellFloorType = CellVisualData.CellFloorType.Carpet;
      public bool overrideTileIndex;
      public GlobalDungeonData.ValidTilesets[] TilesetsToOverrideFloorTile;
      public int[] OverrideFloorTiles;
      public bool preventsOtherFloorDecoration = true;
      public bool allowWallDecorationTho;

      public void Start()
      {
        if (this.overrideMode != FloorTypeOverrideDoer.OverrideMode.Rigidbody)
          return;
        this.DoFloorOverride(this.specRigidbody.UnitBottomLeft.ToIntVector2(), this.specRigidbody.UnitTopRight.ToIntVector2() - IntVector2.One);
      }

      public void ConfigureOnPlacement(RoomHandler room)
      {
        IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
        this.DoFloorOverride(intVector2 + new IntVector2(this.xStartOffset, this.yStartOffset), intVector2 + new IntVector2(this.xStartOffset + this.width - 1, this.yStartOffset + this.height - 1));
      }

      private void DoFloorOverride(IntVector2 lowerLeft, IntVector2 upperRight)
      {
        DungeonData data = GameManager.Instance.Dungeon.data;
        for (int x = lowerLeft.x; x <= upperRight.x; ++x)
        {
          for (int y = lowerLeft.y; y <= upperRight.y; ++y)
          {
            if (this.overrideCellFloorType)
              data[x, y].cellVisualData.floorType = this.cellFloorType;
            if (this.overrideTileIndex)
            {
              int index = Array.IndexOf<GlobalDungeonData.ValidTilesets>(this.TilesetsToOverrideFloorTile, GameManager.Instance.Dungeon.tileIndices.tilesetId);
              if (index >= 0)
              {
                int overrideFloorTile = this.OverrideFloorTiles[index];
                data[x, y].cellVisualData.UsesCustomIndexOverride01 = true;
                data[x, y].cellVisualData.CustomIndexOverride01 = overrideFloorTile;
                data[x, y].cellVisualData.CustomIndexOverride01Layer = GlobalDungeonData.patternLayerIndex;
              }
            }
          }
        }
        if (!this.preventsOtherFloorDecoration)
          return;
        for (int x = lowerLeft.x - 1; x <= upperRight.x + 1; ++x)
        {
          for (int y = lowerLeft.y - 1; y <= upperRight.y + 1; ++y)
          {
            data[x, y].cellVisualData.floorTileOverridden = true;
            data[x, y].cellVisualData.preventFloorStamping = true;
            data[x, y].cellVisualData.containsObjectSpaceStamp = true;
            data[x, y].cellVisualData.containsWallSpaceStamp = !this.allowWallDecorationTho;
          }
        }
      }

      public enum OverrideMode
      {
        Placeable = 10, // 0x0000000A
        Rigidbody = 20, // 0x00000014
      }
    }

}
