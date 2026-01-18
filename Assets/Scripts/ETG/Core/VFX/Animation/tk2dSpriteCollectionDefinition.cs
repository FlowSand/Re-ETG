// Decompiled with JetBrains decompiler
// Type: tk2dSpriteCollectionDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class tk2dSpriteCollectionDefinition
  {
    public string name = string.Empty;
    public bool disableTrimming;
    public bool additive;
    public Vector3 scale = new Vector3(1f, 1f, 1f);
    public Texture2D texture;
    [NonSerialized]
    public Texture2D thumbnailTexture;
    public int materialId;
    public tk2dSpriteCollectionDefinition.Anchor anchor = tk2dSpriteCollectionDefinition.Anchor.MiddleCenter;
    public float anchorX;
    public float anchorY;
    public UnityEngine.Object overrideMesh;
    public bool doubleSidedSprite;
    public bool customSpriteGeometry;
    public tk2dSpriteColliderIsland[] geometryIslands = new tk2dSpriteColliderIsland[0];
    public bool dice;
    public int diceUnitX = 64 /*0x40*/;
    public int diceUnitY = 64 /*0x40*/;
    public tk2dSpriteCollectionDefinition.DiceFilter diceFilter;
    public tk2dSpriteCollectionDefinition.Pad pad;
    public int extraPadding;
    public tk2dSpriteCollectionDefinition.Source source;
    public bool fromSpriteSheet;
    public bool hasSpriteSheetId;
    public int spriteSheetId;
    public int spriteSheetX;
    public int spriteSheetY;
    public bool extractRegion;
    public int regionX;
    public int regionY;
    public int regionW;
    public int regionH;
    public int regionId;
    public tk2dSpriteCollectionDefinition.ColliderType colliderType;
    public CollisionLayer collisionLayer = CollisionLayer.HighObstacle;
    public BagelCollider[] bagelColliders;
    public TilesetIndexMetadata metadata;
    public Vector2 boxColliderMin;
    public Vector2 boxColliderMax;
    public tk2dSpriteColliderIsland[] polyColliderIslands;
    public tk2dSpriteCollectionDefinition.PolygonColliderCap polyColliderCap = tk2dSpriteCollectionDefinition.PolygonColliderCap.FrontAndBack;
    public bool colliderConvex;
    public bool colliderSmoothSphereCollisions;
    public tk2dSpriteCollectionDefinition.ColliderColor colliderColor;
    public List<tk2dSpriteDefinition.AttachPoint> attachPoints = new List<tk2dSpriteDefinition.AttachPoint>();

    public void CopyFrom(tk2dSpriteCollectionDefinition src)
    {
      this.name = src.name;
      this.disableTrimming = src.disableTrimming;
      this.additive = src.additive;
      this.scale = src.scale;
      this.texture = src.texture;
      this.materialId = src.materialId;
      this.anchor = src.anchor;
      this.anchorX = src.anchorX;
      this.anchorY = src.anchorY;
      this.overrideMesh = src.overrideMesh;
      this.doubleSidedSprite = src.doubleSidedSprite;
      this.customSpriteGeometry = src.customSpriteGeometry;
      this.geometryIslands = src.geometryIslands;
      this.dice = src.dice;
      this.diceUnitX = src.diceUnitX;
      this.diceUnitY = src.diceUnitY;
      this.diceFilter = src.diceFilter;
      this.pad = src.pad;
      this.source = src.source;
      this.fromSpriteSheet = src.fromSpriteSheet;
      this.hasSpriteSheetId = src.hasSpriteSheetId;
      this.spriteSheetX = src.spriteSheetX;
      this.spriteSheetY = src.spriteSheetY;
      this.spriteSheetId = src.spriteSheetId;
      this.extractRegion = src.extractRegion;
      this.regionX = src.regionX;
      this.regionY = src.regionY;
      this.regionW = src.regionW;
      this.regionH = src.regionH;
      this.regionId = src.regionId;
      this.colliderType = src.colliderType;
      this.collisionLayer = src.collisionLayer;
      if (src.bagelColliders != null)
      {
        this.bagelColliders = new BagelCollider[src.bagelColliders.Length];
        for (int index = 0; index < src.bagelColliders.Length; ++index)
          this.bagelColliders[index] = new BagelCollider(src.bagelColliders[index]);
      }
      if (src.metadata == null)
      {
        this.metadata = new TilesetIndexMetadata();
      }
      else
      {
        if (this.metadata == null)
          this.metadata = new TilesetIndexMetadata();
        this.metadata.CopyFrom(src.metadata);
      }
      this.boxColliderMin = src.boxColliderMin;
      this.boxColliderMax = src.boxColliderMax;
      this.polyColliderCap = src.polyColliderCap;
      this.colliderColor = src.colliderColor;
      this.colliderConvex = src.colliderConvex;
      this.colliderSmoothSphereCollisions = src.colliderSmoothSphereCollisions;
      this.extraPadding = src.extraPadding;
      if (src.polyColliderIslands != null)
      {
        this.polyColliderIslands = new tk2dSpriteColliderIsland[src.polyColliderIslands.Length];
        for (int index = 0; index < this.polyColliderIslands.Length; ++index)
        {
          this.polyColliderIslands[index] = new tk2dSpriteColliderIsland();
          this.polyColliderIslands[index].CopyFrom(src.polyColliderIslands[index]);
        }
      }
      else
        this.polyColliderIslands = new tk2dSpriteColliderIsland[0];
      if (src.geometryIslands != null)
      {
        this.geometryIslands = new tk2dSpriteColliderIsland[src.geometryIslands.Length];
        for (int index = 0; index < this.geometryIslands.Length; ++index)
        {
          this.geometryIslands[index] = new tk2dSpriteColliderIsland();
          this.geometryIslands[index].CopyFrom(src.geometryIslands[index]);
        }
      }
      else
        this.geometryIslands = new tk2dSpriteColliderIsland[0];
      this.attachPoints = new List<tk2dSpriteDefinition.AttachPoint>(src.attachPoints.Count);
      foreach (tk2dSpriteDefinition.AttachPoint attachPoint1 in src.attachPoints)
      {
        tk2dSpriteDefinition.AttachPoint attachPoint2 = new tk2dSpriteDefinition.AttachPoint();
        attachPoint2.CopyFrom(attachPoint1);
        this.attachPoints.Add(attachPoint2);
      }
    }

    public void Clear() => this.CopyFrom(new tk2dSpriteCollectionDefinition());

    public bool CompareTo(tk2dSpriteCollectionDefinition src)
    {
      if (this.name != src.name || this.additive != src.additive || this.scale != src.scale || (UnityEngine.Object) this.texture != (UnityEngine.Object) src.texture || this.materialId != src.materialId || this.anchor != src.anchor || (double) this.anchorX != (double) src.anchorX || (double) this.anchorY != (double) src.anchorY || this.overrideMesh != src.overrideMesh || this.dice != src.dice || this.diceUnitX != src.diceUnitX || this.diceUnitY != src.diceUnitY || this.diceFilter != src.diceFilter || this.pad != src.pad || this.extraPadding != src.extraPadding || this.doubleSidedSprite != src.doubleSidedSprite || this.customSpriteGeometry != src.customSpriteGeometry || this.geometryIslands != src.geometryIslands)
        return false;
      if (this.geometryIslands != null && src.geometryIslands != null)
      {
        if (this.geometryIslands.Length != src.geometryIslands.Length)
          return false;
        for (int index = 0; index < this.geometryIslands.Length; ++index)
        {
          if (!this.geometryIslands[index].CompareTo(src.geometryIslands[index]))
            return false;
        }
      }
      if (this.source != src.source || this.fromSpriteSheet != src.fromSpriteSheet || this.hasSpriteSheetId != src.hasSpriteSheetId || this.spriteSheetId != src.spriteSheetId || this.spriteSheetX != src.spriteSheetX || this.spriteSheetY != src.spriteSheetY || this.extractRegion != src.extractRegion || this.regionX != src.regionX || this.regionY != src.regionY || this.regionW != src.regionW || this.regionH != src.regionH || this.regionId != src.regionId || this.colliderType != src.colliderType || this.collisionLayer != src.collisionLayer || this.bagelColliders != src.bagelColliders || this.metadata != src.metadata || this.boxColliderMin != src.boxColliderMin || this.boxColliderMax != src.boxColliderMax || this.polyColliderIslands != src.polyColliderIslands)
        return false;
      if (this.polyColliderIslands != null && src.polyColliderIslands != null)
      {
        if (this.polyColliderIslands.Length != src.polyColliderIslands.Length)
          return false;
        for (int index = 0; index < this.polyColliderIslands.Length; ++index)
        {
          if (!this.polyColliderIslands[index].CompareTo(src.polyColliderIslands[index]))
            return false;
        }
      }
      if (this.polyColliderCap != src.polyColliderCap || this.colliderColor != src.colliderColor || this.colliderSmoothSphereCollisions != src.colliderSmoothSphereCollisions || this.colliderConvex != src.colliderConvex || this.attachPoints.Count != src.attachPoints.Count)
        return false;
      for (int index = 0; index < this.attachPoints.Count; ++index)
      {
        if (!this.attachPoints[index].CompareTo(src.attachPoints[index]))
          return false;
      }
      return true;
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
      Custom,
    }

    public enum Pad
    {
      Default,
      BlackZeroAlpha,
      Extend,
      TileXY,
    }

    public enum ColliderType
    {
      UserDefined,
      ForceNone,
      BoxTrimmed,
      BoxCustom,
      Polygon,
    }

    public enum PolygonColliderCap
    {
      None,
      FrontAndBack,
      Front,
      Back,
    }

    public enum ColliderColor
    {
      Default,
      Red,
      White,
      Black,
    }

    public enum Source
    {
      Sprite,
      SpriteSheet,
      Font,
    }

    public enum DiceFilter
    {
      Complete,
      SolidOnly,
      TransparentOnly,
    }
  }

