// Decompiled with JetBrains decompiler
// Type: tk2dSpriteSheetSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    [Serializable]
    public class tk2dSpriteSheetSource
    {
      public Texture2D texture;
      public int tilesX;
      public int tilesY;
      public int numTiles;
      public tk2dSpriteSheetSource.Anchor anchor = tk2dSpriteSheetSource.Anchor.MiddleCenter;
      public tk2dSpriteCollectionDefinition.Pad pad;
      public Vector3 scale = new Vector3(1f, 1f, 1f);
      public bool additive;
      public bool active;
      public int tileWidth;
      public int tileHeight;
      public int tileMarginX;
      public int tileMarginY;
      public int tileSpacingX;
      public int tileSpacingY;
      public tk2dSpriteSheetSource.SplitMethod splitMethod;
      public int version;
      public const int CURRENT_VERSION = 1;
      public tk2dSpriteCollectionDefinition.ColliderType colliderType;

      public void CopyFrom(tk2dSpriteSheetSource src)
      {
        this.texture = src.texture;
        this.tilesX = src.tilesX;
        this.tilesY = src.tilesY;
        this.numTiles = src.numTiles;
        this.anchor = src.anchor;
        this.pad = src.pad;
        this.scale = src.scale;
        this.colliderType = src.colliderType;
        this.version = src.version;
        this.active = src.active;
        this.tileWidth = src.tileWidth;
        this.tileHeight = src.tileHeight;
        this.tileSpacingX = src.tileSpacingX;
        this.tileSpacingY = src.tileSpacingY;
        this.tileMarginX = src.tileMarginX;
        this.tileMarginY = src.tileMarginY;
        this.splitMethod = src.splitMethod;
      }

      public bool CompareTo(tk2dSpriteSheetSource src)
      {
        return !((UnityEngine.Object) this.texture != (UnityEngine.Object) src.texture) && this.tilesX == src.tilesX && this.tilesY == src.tilesY && this.numTiles == src.numTiles && this.anchor == src.anchor && this.pad == src.pad && !(this.scale != src.scale) && this.colliderType == src.colliderType && this.version == src.version && this.active == src.active && this.tileWidth == src.tileWidth && this.tileHeight == src.tileHeight && this.tileSpacingX == src.tileSpacingX && this.tileSpacingY == src.tileSpacingY && this.tileMarginX == src.tileMarginX && this.tileMarginY == src.tileMarginY && this.splitMethod == src.splitMethod;
      }

      public string Name
      {
        get => (UnityEngine.Object) this.texture != (UnityEngine.Object) null ? this.texture.name : "New Sprite Sheet";
      }

      public enum Anchor
      {
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight,
      }

      public enum SplitMethod
      {
        UniformDivision,
      }
    }

}
