// Decompiled with JetBrains decompiler
// Type: tk2dBatchedSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[Serializable]
public class tk2dBatchedSprite
  {
    public tk2dBatchedSprite.Type type = tk2dBatchedSprite.Type.Sprite;
    public string name = string.Empty;
    public int parentId = -1;
    public int spriteId;
    public int xRefId = -1;
    public tk2dSpriteCollectionData spriteCollection;
    public Quaternion rotation = Quaternion.identity;
    public Vector3 position = Vector3.zero;
    public Vector3 localScale = Vector3.one;
    public Color color = Color.white;
    public Vector3 baseScale = Vector3.one;
    public int renderLayer;
    [SerializeField]
    private Vector2 internalData0;
    [SerializeField]
    private Vector2 internalData1;
    [SerializeField]
    private Vector2 internalData2;
    [SerializeField]
    private Vector2 colliderData = new Vector2(0.0f, 1f);
    [SerializeField]
    private string formattedText = string.Empty;
    [SerializeField]
    private tk2dBatchedSprite.Flags flags;
    public tk2dBaseSprite.Anchor anchor;
    public Matrix4x4 relativeMatrix = Matrix4x4.identity;
    private Vector3 cachedBoundsCenter = Vector3.zero;
    private Vector3 cachedBoundsExtents = Vector3.zero;

    public tk2dBatchedSprite() => this.parentId = -1;

    public float BoxColliderOffsetZ
    {
      get => this.colliderData.x;
      set => this.colliderData.x = value;
    }

    public float BoxColliderExtentZ
    {
      get => this.colliderData.y;
      set => this.colliderData.y = value;
    }

    public string FormattedText
    {
      get => this.formattedText;
      set => this.formattedText = value;
    }

    public Vector2 ClippedSpriteRegionBottomLeft
    {
      get => this.internalData0;
      set => this.internalData0 = value;
    }

    public Vector2 ClippedSpriteRegionTopRight
    {
      get => this.internalData1;
      set => this.internalData1 = value;
    }

    public Vector2 SlicedSpriteBorderBottomLeft
    {
      get => this.internalData0;
      set => this.internalData0 = value;
    }

    public Vector2 SlicedSpriteBorderTopRight
    {
      get => this.internalData1;
      set => this.internalData1 = value;
    }

    public Vector2 Dimensions
    {
      get => this.internalData2;
      set => this.internalData2 = value;
    }

    public bool IsDrawn => this.type != tk2dBatchedSprite.Type.EmptyGameObject;

    public bool CheckFlag(tk2dBatchedSprite.Flags mask)
    {
      return (this.flags & mask) != tk2dBatchedSprite.Flags.None;
    }

    public void SetFlag(tk2dBatchedSprite.Flags mask, bool value)
    {
      if (value)
        this.flags |= mask;
      else
        this.flags &= ~mask;
    }

    public Vector3 CachedBoundsCenter
    {
      get => this.cachedBoundsCenter;
      set => this.cachedBoundsCenter = value;
    }

    public Vector3 CachedBoundsExtents
    {
      get => this.cachedBoundsExtents;
      set => this.cachedBoundsExtents = value;
    }

    public tk2dSpriteDefinition GetSpriteDefinition()
    {
      return (UnityEngine.Object) this.spriteCollection != (UnityEngine.Object) null && this.spriteId != -1 ? this.spriteCollection.inst.spriteDefinitions[this.spriteId] : (tk2dSpriteDefinition) null;
    }

    public enum Type
    {
      EmptyGameObject,
      Sprite,
      TiledSprite,
      SlicedSprite,
      ClippedSprite,
      TextMesh,
    }

    [System.Flags]
    public enum Flags
    {
      None = 0,
      Sprite_CreateBoxCollider = 1,
      SlicedSprite_BorderOnly = 2,
    }
  }

