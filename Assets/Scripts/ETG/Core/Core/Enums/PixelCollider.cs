// Decompiled with JetBrains decompiler
// Type: PixelCollider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[Serializable]
public class PixelCollider
  {
    public bool Enabled = true;
    public CollisionLayer CollisionLayer = CollisionLayer.LowObstacle;
    public bool IsTrigger;
    public PixelCollider.PixelColliderGeneration ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Tk2dPolygon;
    public bool BagleUseFirstFrameOnly = true;
    [CheckSprite("Sprite")]
    public string SpecifyBagelFrame;
    public int BagelColliderNumber;
    public int ManualOffsetX;
    public int ManualOffsetY;
    public int ManualWidth;
    public int ManualHeight;
    [Obsolete("ManualRadius is deprecated, use ManualDiameter instead.")]
    public int ManualRadius;
    public int ManualDiameter;
    public int ManualLeftX;
    public int ManualLeftY;
    public int ManualRightX;
    public int ManualRightY;
    public tk2dBaseSprite Sprite;
    public Func<IntVector2, bool> DirectionIgnorer;
    public Func<Vector2, Vector2> NormalModifier;
    public IntVector2 m_offset;
    public IntVector2 m_transformOffset;
    private IntVector2 m_position;
    public IntVector2 m_dimensions;
    public float m_rotation;
    private Vector2 m_scale = new Vector2(1f, 1f);
    private BitArray2D m_basePixels = new BitArray2D();
    private BitArray2D m_modifiedPixels;
    private BitArray2D m_bestPixels;
    private Vector2? m_slopeStart;
    private Vector2? m_slopeEnd;
    [NonSerialized]
    public tk2dSpriteDefinition m_lastSpriteDef;
    [NonSerialized]
    private List<PixelCollider> m_frameSpecificCollisionExceptions = new List<PixelCollider>();
    [NonSerialized]
    private List<TriggerCollisionData> m_triggerCollisions = new List<TriggerCollisionData>();
    private Dictionary<int, PixelCollider.PixelCache> m_cachedBasePixels;
    public static List<PixelCollider.StepData> m_stepList = new List<PixelCollider.StepData>();

    public Vector2 UnitTopLeft => new Vector2((float) this.Min.x, (float) this.Max.y) / 16f;

    public Vector2 UnitTopRight => this.Max.ToVector2() / 16f;

    public Vector2 UnitTopCenter
    {
      get => (this.Min.ToVector2() + new Vector2((float) this.Width / 2f, (float) this.Height)) / 16f;
    }

    public Vector2 UnitCenterLeft
    {
      get => (this.Min.ToVector2() + new Vector2(0.0f, (float) this.Height / 2f)) / 16f;
    }

    public Vector2 UnitCenter
    {
      get
      {
        return (this.Min.ToVector2() + new Vector2((float) this.Width / 2f, (float) this.Height / 2f)) / 16f;
      }
    }

    public Vector2 UnitCenterRight
    {
      get => (this.Min.ToVector2() + new Vector2((float) this.Width, (float) this.Height / 2f)) / 16f;
    }

    public Vector2 UnitBottomLeft => this.Min.ToVector2() / 16f;

    public Vector2 UnitBottomCenter
    {
      get => (this.Min.ToVector2() + new Vector2((float) this.Width / 2f, 0.0f)) / 16f;
    }

    public Vector2 UnitBottomRight => new Vector2((float) this.Max.x, (float) this.Min.y) / 16f;

    public Vector2 UnitDimensions => this.Dimensions.ToVector2() / 16f;

    public float UnitLeft => (float) this.MinX / 16f;

    public float UnitRight => (float) (this.MaxX + 1) / 16f;

    public float UnitBottom => (float) this.MinY / 16f;

    public float UnitTop => (float) (this.MaxY + 1) / 16f;

    public float UnitWidth => (float) this.Dimensions.x / 16f;

    public float UnitHeight => (float) this.Dimensions.y / 16f;

    public IntVector2 Position
    {
      get => this.m_position;
      set => this.m_position = value;
    }

    public IntVector2 Dimensions
    {
      get => this.m_dimensions;
      set => this.m_dimensions = value;
    }

    public IntVector2 Offset => this.m_offset + this.m_transformOffset;

    public Vector2 UnitOffset => PhysicsEngine.PixelToUnit(this.Offset);

    public IntVector2 Min => this.m_position;

    public IntVector2 Max => this.m_position + this.m_dimensions - IntVector2.One;

    public int MinX => this.m_position.x;

    public int MaxX => this.m_position.x + this.m_dimensions.x - 1;

    public int MinY => this.m_position.y;

    public int MaxY => this.m_position.y + this.m_dimensions.y - 1;

    public IntVector2 LowerLeft => this.m_position;

    public IntVector2 LowerRight
    {
      get => new IntVector2(this.m_position.x + this.m_dimensions.x - 1, this.m_position.y);
    }

    public IntVector2 UpperLeft
    {
      get => new IntVector2(this.m_position.x, this.m_position.y + this.m_dimensions.y - 1);
    }

    public IntVector2 UpperRight => this.m_position + this.m_dimensions - IntVector2.One;

    public int X => this.m_position.x;

    public int Y => this.m_position.y;

    public int Width => this.m_dimensions.x;

    public int Height => this.m_dimensions.y;

    public bool IsSlope { get; set; }

    public float Slope { get; set; }

    public IntVector2 UpslopeDirection { get; set; }

    public Vector2 SlopeStart => this.m_slopeStart.Value;

    public Vector2 SlopeEnd => this.m_slopeEnd.Value;

    public bool IsTileCollider { get; set; }

    public float Rotation
    {
      get => this.m_rotation;
      set => this.SetRotationAndScale(value, this.m_scale);
    }

    public Vector2 Scale
    {
      get => this.m_scale;
      set => this.SetRotationAndScale(this.m_rotation, value);
    }

    public int CollisionLayerCollidableOverride { get; set; }

    public int CollisionLayerIgnoreOverride { get; set; }

    public List<TriggerCollisionData> TriggerCollisions => this.m_triggerCollisions;

    public bool this[int x, int y] => this.m_bestPixels[x, y];

    public bool this[IntVector2 pos] => this.m_bestPixels[pos.x, pos.y];

    public bool AABBOverlaps(PixelCollider otherCollider)
    {
      return IntVector2.AABBOverlap(this.m_position, this.m_dimensions, otherCollider.m_position, otherCollider.m_dimensions);
    }

    public bool AABBOverlaps(PixelCollider otherCollider, IntVector2 pixelsToMove)
    {
      int num1 = Mathf.Min(this.m_position.x, this.m_position.x + pixelsToMove.x);
      int a1 = this.m_position.x + this.m_dimensions.x - 1;
      int num2 = Mathf.Max(a1, a1 + pixelsToMove.x) - num1 + 1;
      if (num1 + num2 - 1 < otherCollider.m_position.x || num1 > otherCollider.m_position.x + otherCollider.m_dimensions.x - 1)
        return false;
      int a2 = this.m_position.y + this.m_dimensions.y - 1;
      int num3 = Mathf.Min(this.m_position.y, this.m_position.y + pixelsToMove.y);
      int num4 = Mathf.Max(a2, a2 + pixelsToMove.y) - num3 + 1;
      return num3 + num4 - 1 >= otherCollider.m_position.y && num3 <= otherCollider.m_position.y + otherCollider.m_dimensions.y - 1;
    }

    public bool AABBOverlaps(IntVector2 pos, IntVector2 dimensions)
    {
      return IntVector2.AABBOverlap(this.m_position, this.m_dimensions, pos, dimensions);
    }

    public bool Overlaps(PixelCollider otherCollider)
    {
      return this.Overlaps(otherCollider, IntVector2.Zero);
    }

    public bool Overlaps(PixelCollider otherCollider, IntVector2 otherColliderOffset)
    {
      IntVector2 intVector2 = otherCollider.m_position - this.m_position + otherColliderOffset;
      int num1 = Math.Max(0, intVector2.x);
      int num2 = Math.Max(0, intVector2.y);
      int num3 = Math.Min(this.m_bestPixels.Width - 1, otherCollider.m_bestPixels.Width - 1 + intVector2.x);
      int num4 = Math.Min(this.m_bestPixels.Height - 1, otherCollider.m_bestPixels.Height - 1 + intVector2.y);
      for (int x = num1; x <= num3; ++x)
      {
        for (int y = num2; y <= num4; ++y)
        {
          if (this.m_bestPixels[x, y] && otherCollider.m_bestPixels[x - intVector2.x, y - intVector2.y])
            return true;
        }
      }
      return false;
    }

    public bool CanCollideWith(int mask, CollisionLayer? sourceLayer = null)
    {
      if (!this.Enabled)
        return false;
      if ((mask & this.CollisionLayerCollidableOverride) > 0)
        return true;
      if (sourceLayer.HasValue)
      {
        int mask1 = CollisionMask.LayerToMask(sourceLayer.Value);
        if ((mask1 & this.CollisionLayerCollidableOverride) == mask1 || this.IsTileCollider && sourceLayer.Value == CollisionLayer.TileBlocker)
          return true;
      }
      int mask2 = CollisionMask.LayerToMask(this.CollisionLayer);
      return (mask & mask2) == mask2;
    }

    public bool CanCollideWith(PixelCollider otherCollider, bool ignoreFrameSpecificExceptions = false)
    {
      if (!this.Enabled || !otherCollider.Enabled)
        return false;
      if (this.IsTileCollider && otherCollider.CollisionLayer == CollisionLayer.TileBlocker)
        return true;
      int mask1 = CollisionMask.LayerToMask(this.CollisionLayer);
      int mask2 = CollisionMask.LayerToMask(otherCollider.CollisionLayer);
      return ((mask1 & otherCollider.CollisionLayerCollidableOverride) == mask1 || (mask2 & this.CollisionLayerCollidableOverride) == mask2 || (CollisionLayerMatrix.GetMask(otherCollider.CollisionLayer) & ~otherCollider.CollisionLayerIgnoreOverride & mask1) == mask1 && (CollisionLayerMatrix.GetMask(this.CollisionLayer) & ~this.CollisionLayerIgnoreOverride & mask2) == mask2) && (ignoreFrameSpecificExceptions || (this.m_frameSpecificCollisionExceptions.Count <= 0 || !this.m_frameSpecificCollisionExceptions.Contains(otherCollider)) && (otherCollider.m_frameSpecificCollisionExceptions.Count <= 0 || !otherCollider.m_frameSpecificCollisionExceptions.Contains(this)));
    }

    public bool CanCollideWith(CollisionLayer collisionLayer)
    {
      if (!this.Enabled)
        return false;
      if (this.IsTileCollider && collisionLayer == CollisionLayer.TileBlocker)
        return true;
      int mask = CollisionMask.LayerToMask(collisionLayer);
      return (CollisionLayerMatrix.GetMask(this.CollisionLayer) & mask) == mask;
    }

    public bool Raycast(Vector2 origin, Vector2 direction, float distance, out RaycastResult result)
    {
      result = (RaycastResult) null;
      if (!this.Enabled)
        return false;
      direction.Normalize();
      IntVector2 pixel1 = PhysicsEngine.UnitToPixel(origin);
      IntVector2 dimensionsA = PhysicsEngine.UnitToPixel(direction * distance) + new IntVector2((int) Mathf.Sign(direction.x), (int) Mathf.Sign(direction.y));
      if (dimensionsA.x < 0)
      {
        dimensionsA.x *= -1;
        pixel1.x -= dimensionsA.x;
      }
      if (dimensionsA.y < 0)
      {
        dimensionsA.y *= -1;
        pixel1.y -= dimensionsA.y;
      }
      if (!IntVector2.AABBOverlap(pixel1, dimensionsA, this.m_position, this.m_dimensions))
        return false;
      Vector2 l1 = origin;
      Vector2 l2 = l1 + distance * direction;
      Vector2 vector2 = this.m_position.ToVector2() / 16f;
      Vector2 bSize = this.m_dimensions.ToVector2() / 16f;
      bool flag1 = origin.IsWithin(vector2, vector2 + bSize);
      Vector2 intersection;
      if (!BraveUtility.LineIntersectsAABB(l1, l2, vector2, bSize, out intersection) && !flag1 || this.DirectionIgnorer != null && this.DirectionIgnorer(PhysicsEngine.UnitToPixel(direction * distance)))
        return false;
      IntVector2 pixel2 = IntVector2.NegOne;
      IntVector2 negOne = IntVector2.NegOne;
      float num1 = 0.0f;
      Vector2 b;
      IntVector2 intVector2;
      if (flag1)
      {
        b = origin * 16f;
        pixel2 = new IntVector2((int) b.x, (int) b.y);
        intVector2 = IntVector2.Zero;
      }
      else
      {
        float num2 = Mathf.Abs(intersection.x - PhysicsEngine.PixelToUnit(this.Min.x));
        float num3 = Mathf.Abs(intersection.x - PhysicsEngine.PixelToUnit(this.Max.x + 1));
        float num4 = Mathf.Abs(intersection.y - PhysicsEngine.PixelToUnit(this.Min.y));
        float num5 = Mathf.Abs(intersection.y - PhysicsEngine.PixelToUnit(this.Max.y + 1));
        if ((double) num2 <= (double) num3 && (double) num2 <= (double) num5 && (double) num2 <= (double) num4 && (double) direction.x > 0.0)
        {
          pixel2 = new IntVector2(this.Min.X - 1, PhysicsEngine.UnitToPixel(intersection.y));
          intVector2 = IntVector2.Right;
          b = new Vector2((float) this.Min.X, intersection.y * 16f);
        }
        else if ((double) num3 <= (double) num2 && (double) num3 <= (double) num5 && (double) num3 <= (double) num4 && (double) direction.x < 0.0)
        {
          pixel2 = new IntVector2(this.Max.X + 1, PhysicsEngine.UnitToPixel(intersection.y));
          intVector2 = IntVector2.Left;
          b = new Vector2((float) (this.Max.X + 1), intersection.y * 16f);
        }
        else if ((double) num4 <= (double) num3 && (double) num4 <= (double) num5 && (double) num4 <= (double) num2 && (double) direction.y > 0.0)
        {
          pixel2 = new IntVector2(PhysicsEngine.UnitToPixel(intersection.x), this.Min.Y - 1);
          intVector2 = IntVector2.Up;
          b = new Vector2(intersection.x * 16f, (float) this.Min.y);
        }
        else
        {
          if ((double) num5 > (double) num3 || (double) num5 > (double) num2 || (double) num5 > (double) num4 || (double) direction.y >= 0.0)
            return false;
          pixel2 = new IntVector2(PhysicsEngine.UnitToPixel(intersection.x), this.Max.y + 1);
          intVector2 = IntVector2.Down;
          b = new Vector2(intersection.x * 16f, (float) (this.Max.y + 1));
        }
        num1 = Vector2.Distance(origin, intersection);
      }
      bool flag2 = false;
      int x = Math.Sign(direction.x);
      int y = Math.Sign(direction.y);
      while ((!flag2 || this.AABBContainsPixel(pixel2)) && (double) num1 < (double) distance)
      {
        IntVector2 pixel3 = pixel2 + intVector2;
        if (this.AABBContainsPixel(pixel3))
        {
          flag2 = true;
          if (this[pixel3 - this.Position])
          {
            result = RaycastResult.Pool.Allocate();
            result.Contact = b / 16f;
            result.HitPixel = pixel3;
            result.LastRayPixel = pixel2;
            result.Distance = num1;
            result.Normal = ((Vector2) -intVector2).normalized;
            if (this.NormalModifier != null)
              result.Normal = this.NormalModifier(result.Normal);
            result.OtherPixelCollider = this;
            return true;
          }
        }
        pixel2 = pixel3;
        float num6 = (double) direction.x == 0.0 ? float.PositiveInfinity : (float) (pixel2.x + x) - b.x;
        float num7 = (double) direction.y == 0.0 ? float.PositiveInfinity : (float) (pixel2.y + y) - b.y;
        if (x < 0)
          ++num6;
        if (y < 0)
          ++num7;
        float num8 = (double) direction.x == 0.0 ? float.PositiveInfinity : num6 / direction.x;
        float num9 = (double) direction.y == 0.0 ? float.PositiveInfinity : num7 / direction.y;
        Vector2 a = b;
        if ((double) num8 < (double) num9)
        {
          intVector2 = new IntVector2(x, 0);
          b.x += num6;
          if ((double) direction.y != 0.0 && (double) num8 != 0.0)
            b.y += direction.y * num8;
          num1 += Vector2.Distance(a, b) / 16f;
        }
        else
        {
          intVector2 = new IntVector2(0, y);
          if ((double) direction.x != 0.0 && (double) num9 != 0.0)
            b.x += direction.x * num9;
          b.y += num7;
          num1 += Vector2.Distance(a, b) / 16f;
        }
      }
      return false;
    }

    public bool LinearCast(
      PixelCollider otherCollider,
      IntVector2 pixelsToMove,
      out LinearCastResult result)
    {
      PhysicsEngine.PixelMovementGenerator(pixelsToMove, PixelCollider.m_stepList);
      return this.LinearCast(otherCollider, pixelsToMove, PixelCollider.m_stepList, out result);
    }

    public bool LinearCast(
      PixelCollider otherCollider,
      IntVector2 pixelsToMove,
      List<PixelCollider.StepData> stepList,
      out LinearCastResult result,
      bool traverseSlopes = false,
      float currentSlope = 0.0f)
    {
      if (!this.Enabled)
      {
        result = (LinearCastResult) null;
        return false;
      }
      if (otherCollider.DirectionIgnorer != null && otherCollider.DirectionIgnorer(pixelsToMove))
      {
        result = (LinearCastResult) null;
        return false;
      }
      IntVector2 zero = IntVector2.Zero;
      IntVector2 intVector2_1 = otherCollider.m_position - this.m_position;
      result = LinearCastResult.Pool.Allocate();
      result.MyPixelCollider = this;
      result.OtherPixelCollider = (PixelCollider) null;
      result.TimeUsed = 0.0f;
      result.CollidedX = false;
      result.CollidedY = false;
      result.NewPixelsToMove.x = 0;
      result.NewPixelsToMove.y = 0;
      result.Overlap = false;
      float num = 0.0f;
      for (int index = 0; index < stepList.Count; ++index)
      {
        IntVector2 deltaPos = stepList[index].deltaPos;
        float deltaTime = stepList[index].deltaTime;
        num += deltaTime;
        IntVector2 posA = this.m_position + zero + deltaPos;
        if (IntVector2.AABBOverlap(posA, this.m_dimensions, otherCollider.Position, otherCollider.Dimensions))
        {
          IntVector2 intVector2_2 = IntVector2.Max(IntVector2.Zero, otherCollider.Position - posA);
          IntVector2 intVector2_3 = IntVector2.Min(this.m_dimensions - IntVector2.One, otherCollider.UpperRight - posA);
          for (int x = intVector2_2.x; x <= intVector2_3.x; ++x)
          {
            for (int y = intVector2_2.y; y <= intVector2_3.y; ++y)
            {
              if (this.m_bestPixels[x, y])
              {
                IntVector2 pos = new IntVector2(x, y) - intVector2_1 + zero + deltaPos;
                if (pos.x >= 0 && pos.x < otherCollider.Dimensions.x && pos.y >= 0 && pos.y < otherCollider.Dimensions.y && otherCollider[pos] && (!otherCollider.IsSlope || !traverseSlopes || (double) otherCollider.Slope != (double) currentSlope))
                {
                  result.TimeUsed = num;
                  result.CollidedX = deltaPos.x != 0;
                  result.CollidedY = deltaPos.y != 0;
                  result.NewPixelsToMove = zero;
                  if (!otherCollider.IsSlope || deltaPos.y == 1 || deltaPos.y < 0 || Math.Sign(deltaPos.x) != Math.Sign(otherCollider.SlopeEnd.y - otherCollider.SlopeStart.y))
                    ;
                  result.MyPixelCollider = this;
                  result.OtherPixelCollider = otherCollider;
                  IntVector2 intVector2_4 = this.Position + new IntVector2(x, y) + zero + deltaPos;
                  result.Contact = PixelCollider.FromCollisionVector(intVector2_4) + new Vector2(0.5f, 0.5f) / 16f;
                  result.Normal = (Vector2) -deltaPos;
                  if (otherCollider.NormalModifier != null)
                    result.Normal = otherCollider.NormalModifier(result.Normal);
                  return true;
                }
              }
            }
          }
        }
        zero += deltaPos;
      }
      result.NewPixelsToMove = zero;
      return false;
    }

    public bool AABBContainsPixel(IntVector2 pixel)
    {
      return pixel.x >= this.Min.x && pixel.x <= this.Max.x && pixel.y >= this.Min.y && pixel.y <= this.Max.y;
    }

    public bool ContainsPixel(IntVector2 pixel)
    {
      return this.AABBContainsPixel(pixel) && this.m_bestPixels[pixel.x - this.m_position.x, pixel.y - this.m_position.y];
    }

    public void SetRotationAndScale(float rotation, Vector2 scale)
    {
      BitArray2D bitArray2D = (double) rotation != 0.0 || !(scale == Vector2.one) ? this.m_modifiedPixels : this.m_basePixels;
      if ((double) this.m_rotation == (double) rotation && this.m_scale == scale && this.m_bestPixels == bitArray2D && this.m_bestPixels != null && this.m_bestPixels.IsValid)
        return;
      this.m_rotation = rotation;
      this.m_scale = scale;
      int width = this.m_basePixels.Width;
      int height = this.m_basePixels.Height;
      if ((double) rotation == 0.0 && scale == Vector2.one)
      {
        this.m_bestPixels = this.m_basePixels;
        this.m_dimensions = new IntVector2(width, height);
        this.m_transformOffset = IntVector2.Zero;
      }
      else
      {
        if (this.m_modifiedPixels == null)
          this.m_modifiedPixels = new BitArray2D();
        Vector2 pivot1 = -(Vector2) this.m_offset;
        Vector2 vector2_1 = this.TransformPixel(new Vector2(0.5f, 0.5f), pivot1, rotation, scale);
        Vector2 vector2_2 = this.TransformPixel(new Vector2((float) width - 0.5f, 0.5f), pivot1, rotation, scale);
        Vector2 vector2_3 = this.TransformPixel(new Vector2(0.5f, (float) height - 0.5f), pivot1, rotation, scale);
        Vector2 vector2_4 = this.TransformPixel(new Vector2((float) width - 0.5f, (float) height - 0.5f), pivot1, rotation, scale);
        int x1 = Mathf.FloorToInt(Mathf.Min(vector2_1.x, vector2_2.x, vector2_3.x, vector2_4.x));
        int y1 = Mathf.FloorToInt(Mathf.Min(vector2_1.y, vector2_2.y, vector2_3.y, vector2_4.y));
        int num1 = Mathf.CeilToInt(Mathf.Max(vector2_1.x, vector2_2.x, vector2_3.x, vector2_4.x));
        int num2 = Mathf.CeilToInt(Mathf.Max(vector2_1.y, vector2_2.y, vector2_3.y, vector2_4.y));
        this.m_transformOffset = new IntVector2(x1, y1);
        Vector2 pivot2 = pivot1 - (Vector2) this.m_transformOffset;
        int num3 = num1 - x1;
        int num4 = num2 - y1;
        this.m_modifiedPixels.ReinitializeWithDefault(num3, num4, false);
        if (this.m_basePixels.IsAabb)
        {
          int num5 = 4;
          Vector2[] vector2Array = new Vector2[4]
          {
            vector2_1 - this.m_transformOffset.ToVector2(),
            vector2_2 - this.m_transformOffset.ToVector2(),
            vector2_4 - this.m_transformOffset.ToVector2(),
            vector2_3 - this.m_transformOffset.ToVector2()
          };
          int[] numArray = new int[4];
          for (int y2 = 0; y2 < num4; ++y2)
          {
            int num6 = 0;
            int index1 = num5 - 1;
            for (int index2 = 0; index2 < num5; ++index2)
            {
              if ((double) vector2Array[index2].y < (double) y2 && (double) vector2Array[index1].y >= (double) y2 || (double) vector2Array[index1].y < (double) y2 && (double) vector2Array[index2].y >= (double) y2)
                numArray[num6++] = (int) ((double) vector2Array[index2].x + ((double) y2 - (double) vector2Array[index2].y) / ((double) vector2Array[index1].y - (double) vector2Array[index2].y) * ((double) vector2Array[index1].x - (double) vector2Array[index2].x));
              index1 = index2;
            }
            int index3 = 0;
            while (index3 < num6 - 1)
            {
              if (numArray[index3] > numArray[index3 + 1])
              {
                int num7 = numArray[index3];
                numArray[index3] = numArray[index3 + 1];
                numArray[index3 + 1] = num7;
                if (index3 != 0)
                  --index3;
              }
              else
                ++index3;
            }
            for (int index4 = 0; index4 < num6 && numArray[index4] < num3 - 1; index4 += 2)
            {
              if (numArray[index4 + 1] > 0)
              {
                if (numArray[index4] < 0)
                  numArray[index4] = 0;
                if (numArray[index4 + 1] > num3 - 1)
                  numArray[index4 + 1] = num3 - 1;
                for (int x2 = numArray[index4]; x2 < numArray[index4 + 1]; ++x2)
                  this.m_modifiedPixels[x2, y2] = true;
              }
            }
          }
        }
        else
        {
          float rotation1 = -rotation;
          Vector2 scale1 = new Vector2(1f / scale.x, 1f / scale.y);
          for (int x3 = 0; x3 < num3; ++x3)
          {
            for (int y3 = 0; y3 < num4; ++y3)
            {
              Vector2 vector2_5 = this.TransformPixel(new Vector2((float) x3 + 0.5f, (float) y3 + 0.5f), pivot2, rotation1, scale1) + (Vector2) this.m_transformOffset;
              this.m_modifiedPixels[x3, y3] = (double) vector2_5.x >= 0.0 && (int) vector2_5.x < width && (double) vector2_5.y >= 0.0 && (int) vector2_5.y < height && this.m_basePixels[(int) vector2_5.x, (int) vector2_5.y];
            }
          }
        }
        this.m_dimensions = new IntVector2(num3, num4);
        this.m_bestPixels = this.m_modifiedPixels;
      }
    }

    private Vector2 TransformPixel(Vector2 pixel, Vector2 pivot, float rotation, Vector2 scale)
    {
      Vector2 vector2 = pixel - pivot;
      Vector2 a;
      a.x = (float) ((double) vector2.x * (double) Mathf.Cos(rotation * ((float) Math.PI / 180f)) - (double) vector2.y * (double) Mathf.Sin(rotation * ((float) Math.PI / 180f)));
      a.y = (float) ((double) vector2.x * (double) Mathf.Sin(rotation * ((float) Math.PI / 180f)) + (double) vector2.y * (double) Mathf.Cos(rotation * ((float) Math.PI / 180f)));
      return Vector2.Scale(a, scale) + pivot;
    }

    public void RegisterFrameSpecificCollisionException(
      SpeculativeRigidbody mySpecRigidbody,
      PixelCollider pixelCollider)
    {
      if (this.m_frameSpecificCollisionExceptions.Contains(pixelCollider))
        return;
      this.m_frameSpecificCollisionExceptions.Add(pixelCollider);
      mySpecRigidbody.HasFrameSpecificCollisionExceptions = true;
    }

    public void ClearFrameSpecificCollisionExceptions()
    {
      this.m_frameSpecificCollisionExceptions.Clear();
    }

    public TriggerCollisionData RegisterTriggerCollision(
      SpeculativeRigidbody mySpecRigidbody,
      SpeculativeRigidbody otherSpecRigidbody,
      PixelCollider otherPixelCollider)
    {
      TriggerCollisionData triggerCollisionData = this.m_triggerCollisions.Find((Predicate<TriggerCollisionData>) (d => d.PixelCollider == otherPixelCollider));
      if (triggerCollisionData == null)
      {
        triggerCollisionData = new TriggerCollisionData(otherSpecRigidbody, otherPixelCollider);
        this.m_triggerCollisions.Add(triggerCollisionData);
        mySpecRigidbody.HasTriggerCollisions = true;
      }
      else
        triggerCollisionData.ContinuedCollision = true;
      return triggerCollisionData;
    }

    public void ResetTriggerCollisionData()
    {
      for (int index = 0; index < this.m_triggerCollisions.Count; ++index)
        this.m_triggerCollisions[index].Reset();
    }

    public void Regenerate(Transform transform, bool allowRotation = true, bool allowScale = true)
    {
      if (!(bool) (UnityEngine.Object) this.Sprite)
        this.Sprite = transform.GetComponentInChildren<tk2dBaseSprite>();
      float rotation = !allowRotation ? 0.0f : transform.eulerAngles.z;
      Vector2 a = !allowScale ? Vector2.one : (Vector2) transform.localScale;
      if (allowScale && (bool) (UnityEngine.Object) this.Sprite)
        a = Vector2.Scale(a, (Vector2) this.Sprite.scale);
      switch (this.ColliderGenerationMode)
      {
        case PixelCollider.PixelColliderGeneration.Manual:
          this.RegenerateFromManual(transform, new IntVector2(this.ManualOffsetX, this.ManualOffsetY), new IntVector2(this.ManualWidth, this.ManualHeight), rotation, new Vector2?(a));
          break;
        case PixelCollider.PixelColliderGeneration.Tk2dPolygon:
          this.RegenerateFrom3dCollider(this.Sprite.GetTrueCurrentSpriteDef().colliderVertices, transform, rotation, new Vector2?(a), this.Sprite.FlipX, this.Sprite.FlipY);
          break;
        case PixelCollider.PixelColliderGeneration.BagelCollider:
          this.RegenerateFromBagelCollider(this.Sprite, transform, rotation, new Vector2?(a), this.Sprite.FlipX);
          break;
        case PixelCollider.PixelColliderGeneration.Circle:
          if (this.ManualDiameter <= 0 && this.ManualRadius > 0)
            this.ManualDiameter = 2 * this.ManualRadius;
          this.RegenerateFromCircle(transform, new IntVector2(this.ManualOffsetX, this.ManualOffsetY), this.ManualDiameter);
          break;
        case PixelCollider.PixelColliderGeneration.Line:
          this.RegenerateFromLine(transform, new IntVector2(this.ManualLeftX, this.ManualLeftY), new IntVector2(this.ManualRightX, this.ManualRightY));
          break;
      }
    }

    public void RegenerateFromManual(
      Transform transform,
      IntVector2 offset,
      IntVector2 dimensions,
      float rotation = 0.0f,
      Vector2? scale = null)
    {
      this.RegenerateFromManual((Vector2) transform.position, offset, dimensions, rotation, scale);
    }

    public void RegenerateFromManual(
      Vector2 position,
      IntVector2 offset,
      IntVector2 dimensions,
      float rotation = 0.0f,
      Vector2? scale = null)
    {
      if (!scale.HasValue)
        scale = new Vector2?(new Vector2(1f, 1f));
      this.m_offset = offset;
      this.m_dimensions = dimensions;
      this.m_position = PixelCollider.ToCollisionVector(position) + this.m_offset;
      this.m_basePixels.ReinitializeWithDefault(this.m_dimensions.x, this.m_dimensions.y, true, fixedSize: true);
      this.m_bestPixels = this.m_basePixels;
      this.SetRotationAndScale(rotation, scale.Value);
    }

    public void RegenerateFrom3dCollider(
      Vector3[] allVertices,
      Transform transform,
      float rotation = 0.0f,
      Vector2? scale = null,
      bool flipX = false,
      bool flipY = false)
    {
      if (!scale.HasValue)
        scale = new Vector2?(new Vector2(1f, 1f));
      if (allVertices.Length == 2)
      {
        Vector2[] vertices = new Vector2[4];
        Vector2 allVertex1 = (Vector2) allVertices[0];
        Vector2 allVertex2 = (Vector2) allVertices[1];
        if (flipX)
          allVertex1.x *= -1f;
        if (flipY)
          allVertex1.y *= -1f;
        vertices[0] = allVertex1 + new Vector2(-allVertex2.x, allVertex2.y);
        vertices[1] = allVertex1 + new Vector2(-allVertex2.x, -allVertex2.y);
        vertices[2] = allVertex1 + new Vector2(allVertex2.x, -allVertex2.y);
        vertices[3] = allVertex1 + new Vector2(allVertex2.x, allVertex2.y);
        this.RegenerateFromVertices(vertices, transform, rotation, scale);
      }
      else
      {
        Vector2[] a = new Vector2[allVertices.Length / 2];
        int size = 0;
        for (int index1 = 0; index1 < allVertices.Length && size < a.Length; ++index1)
        {
          if ((double) allVertices[index1].z < 0.0)
          {
            Vector2 allVertex = (Vector2) allVertices[index1];
            bool flag = false;
            for (int index2 = 0; index2 < size; ++index2)
            {
              if (Mathf.Approximately(a[index2].x, allVertex.x) && Mathf.Approximately(a[index2].y, allVertex.y))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
              a[size++] = allVertex;
          }
        }
        this.RegenerateFromVertices(BraveUtility.ResizeArray(a, size), transform, rotation, scale);
      }
    }

    public void RegenerateFromBagelCollider(
      tk2dBaseSprite sprite,
      Transform transform,
      float rotation = 0.0f,
      Vector2? scale = null,
      bool flipX = false)
    {
      if (!scale.HasValue)
        scale = new Vector2?(new Vector2(1f, 1f));
      tk2dSpriteDefinition spriteDefinition = !this.BagleUseFirstFrameOnly || string.IsNullOrEmpty(this.SpecifyBagelFrame) ? sprite.GetTrueCurrentSpriteDef() : sprite.Collection.GetSpriteDefinition(this.SpecifyBagelFrame);
      this.m_lastSpriteDef = spriteDefinition;
      if (!this.BagleUseFirstFrameOnly && this.m_cachedBasePixels == null)
        this.m_cachedBasePixels = new Dictionary<int, PixelCollider.PixelCache>();
      int num1 = spriteDefinition != null ? sprite.GetSpriteIdByName(spriteDefinition.name) : -1;
      if (!this.BagleUseFirstFrameOnly && this.m_cachedBasePixels.ContainsKey(num1))
      {
        PixelCollider.PixelCache cachedBasePixel = this.m_cachedBasePixels[num1];
        this.m_dimensions = cachedBasePixel.dimensions;
        this.m_basePixels = cachedBasePixel.basePixels;
        this.m_bestPixels = this.m_basePixels;
        this.m_offset = cachedBasePixel.offset;
      }
      else
      {
        this.m_basePixels = new BitArray2D();
        BagelCollider[] bagelColliders = sprite.Collection.GetBagelColliders(num1);
        BagelCollider bagelCollider = this.BagelColliderNumber >= (bagelColliders == null ? 0 : bagelColliders.Length) ? (BagelCollider) null : bagelColliders[this.BagelColliderNumber];
        if (bagelCollider == null)
        {
          this.RegenerateEmptyCollider(transform);
          if (this.BagleUseFirstFrameOnly)
            return;
          PixelCollider.PixelCache pixelCache = new PixelCollider.PixelCache()
          {
            dimensions = this.m_dimensions,
            basePixels = this.m_basePixels,
            offset = this.m_offset
          };
          pixelCache.basePixels.ReadOnly = true;
          this.m_cachedBasePixels.Add(num1, pixelCache);
          return;
        }
        tk2dSlicedSprite sprite1 = this.Sprite as tk2dSlicedSprite;
        IntVector2 lhs1;
        IntVector2 lhs2;
        if ((bool) (UnityEngine.Object) sprite1)
        {
          lhs1 = IntVector2.Zero;
          lhs2 = new IntVector2(Mathf.RoundToInt(sprite1.dimensions.x) - 1, Mathf.RoundToInt(sprite1.dimensions.y) - 1);
        }
        else
        {
          lhs1 = IntVector2.MaxValue;
          lhs2 = IntVector2.MinValue;
          for (int x = 0; x < bagelCollider.width; ++x)
          {
            for (int y = 0; y < bagelCollider.height; ++y)
            {
              if (bagelCollider[x, bagelCollider.height - y - 1])
              {
                lhs1 = IntVector2.Min(lhs1, new IntVector2(x, y));
                lhs2 = IntVector2.Max(lhs2, new IntVector2(x, y));
              }
            }
          }
          if (lhs1 == IntVector2.MaxValue || lhs2 == IntVector2.MinValue)
          {
            this.RegenerateEmptyCollider(transform);
            return;
          }
        }
        this.m_dimensions = lhs2 - lhs1 + IntVector2.One;
        this.m_basePixels.Reinitialize(this.m_dimensions.x, this.m_dimensions.y, true);
        this.m_bestPixels = this.m_basePixels;
        if ((bool) (UnityEngine.Object) sprite1)
        {
          this.m_offset = lhs1 - sprite1.anchorOffset.ToIntVector2();
          tk2dSpriteDefinition currentSpriteDef = sprite1.GetTrueCurrentSpriteDef();
          float num2 = currentSpriteDef.position1.x - currentSpriteDef.position0.x;
          float num3 = currentSpriteDef.position2.y - currentSpriteDef.position0.y;
          float x1 = currentSpriteDef.texelSize.x;
          float y1 = currentSpriteDef.texelSize.y;
          IntVector2 intVector2_1 = new IntVector2(Mathf.RoundToInt(num2 / x1), Mathf.RoundToInt(num3 / y1));
          Vector3 boundsDataExtents = currentSpriteDef.boundsDataExtents;
          Vector3 vector3 = new Vector3(boundsDataExtents.x / currentSpriteDef.texelSize.x, boundsDataExtents.y / currentSpriteDef.texelSize.y, 1f);
          IntVector2 intVector2_2 = new IntVector2(Mathf.RoundToInt(sprite1.dimensions.x), Mathf.RoundToInt(sprite1.dimensions.y));
          int num4 = Mathf.RoundToInt(sprite1.borderTop * vector3.y);
          int num5 = Mathf.RoundToInt(sprite1.borderBottom * vector3.y);
          int num6 = Mathf.RoundToInt(sprite1.borderLeft * vector3.x);
          int num7 = Mathf.RoundToInt(sprite1.borderRight * vector3.x);
          int num8 = intVector2_1.x - num6 - num7;
          int num9 = intVector2_1.y - num4 - num5;
          for (int x2 = lhs1.x; x2 <= lhs2.x; ++x2)
          {
            int x3 = x2 >= num6 ? (x2 >= intVector2_2.x - num7 ? intVector2_1.x - (intVector2_2.x - x2) : (x2 - num6) % num8 + num6) : x2;
            for (int y2 = lhs1.y; y2 <= lhs2.y; ++y2)
            {
              int num10 = y2 >= num5 ? (y2 >= intVector2_2.y - num4 ? intVector2_1.y - (intVector2_2.y - y2) : (y2 - num5) % num9 + num4) : y2;
              this.m_basePixels[x2, y2] = bagelCollider[x3, bagelCollider.height - num10 - 1];
            }
          }
        }
        else
        {
          this.m_offset = lhs1 - this.Sprite.GetAnchorPixelOffset();
          for (int x = lhs1.x; x <= lhs2.x; ++x)
          {
            for (int y = lhs1.y; y <= lhs2.y; ++y)
              this.m_basePixels[x - lhs1.x, y - lhs1.y] = bagelCollider[x, bagelCollider.height - y - 1];
          }
        }
        if (!this.BagleUseFirstFrameOnly)
        {
          PixelCollider.PixelCache pixelCache = new PixelCollider.PixelCache()
          {
            dimensions = this.m_dimensions,
            basePixels = this.m_basePixels,
            offset = this.m_offset
          };
          pixelCache.basePixels.ReadOnly = true;
          this.m_cachedBasePixels.Add(num1, pixelCache);
        }
      }
      this.m_position = PixelCollider.ToCollisionVector((Vector2) transform.position) + this.m_offset;
      this.SetRotationAndScale(rotation, scale.Value);
    }

    public void RegenerateFromCircle(Transform transform, IntVector2 offset, int diameter)
    {
      this.RegenerateFromCircle((Vector2) transform.position, offset, diameter);
    }

    public void RegenerateFromCircle(Vector2 position, IntVector2 offset, int diameter)
    {
      this.m_offset = offset;
      this.m_dimensions = new IntVector2(diameter, diameter);
      this.m_position = PixelCollider.ToCollisionVector(position) + this.m_offset;
      this.m_basePixels.Reinitialize(this.m_dimensions.x, this.m_dimensions.y, true);
      this.m_bestPixels = this.m_basePixels;
      float num = (float) diameter / 2f;
      for (int x = 0; x < this.m_dimensions.x; ++x)
      {
        for (int y = 0; y < this.m_dimensions.y; ++y)
          this.m_basePixels[x, y] = (double) Vector2.Distance(new Vector2((float) x, (float) y), new Vector2(num, num)) < (double) num;
      }
      this.SetRotationAndScale(0.0f, new Vector2(1f, 1f));
    }

    public void RegenerateFromLine(Transform transform, IntVector2 leftPoint, IntVector2 rightPoint)
    {
      this.RegenerateFromLine((Vector2) transform.position, leftPoint, rightPoint);
    }

    public void RegenerateFromLine(Vector2 position, IntVector2 leftPoint, IntVector2 rightPoint)
    {
      this.m_offset = new IntVector2(Mathf.Min(leftPoint.x, rightPoint.x), Mathf.Min(leftPoint.y, rightPoint.y));
      this.m_dimensions = new IntVector2(Mathf.Abs(rightPoint.x - leftPoint.x) + 1, Mathf.Abs(rightPoint.y - leftPoint.y) + 1);
      this.m_position = PixelCollider.ToCollisionVector(position) + this.m_offset;
      this.m_basePixels.ReinitializeWithDefault(this.m_dimensions.x, this.m_dimensions.y, false, fixedSize: true);
      this.m_bestPixels = this.m_basePixels;
      this.PlotPixelLines(new Vector2[2]
      {
        PhysicsEngine.PixelToUnit(leftPoint),
        PhysicsEngine.PixelToUnit(rightPoint)
      }, -PhysicsEngine.PixelToUnit(this.m_offset));
      this.SetRotationAndScale(0.0f, new Vector2(1f, 1f));
    }

    public void RegenerateEmptyCollider(Transform transform)
    {
      this.m_offset = IntVector2.Zero;
      this.m_dimensions = IntVector2.Zero;
      this.m_position = PixelCollider.ToCollisionVector((Vector2) transform.position) + this.m_offset;
      this.m_basePixels.Reinitialize(0, 0, true);
      this.m_bestPixels = this.m_basePixels;
      this.SetRotationAndScale(0.0f, new Vector2(1f, 1f));
    }

    public void RegenerateFromVertices(
      Vector2[] vertices,
      Transform transform,
      float rotation = 0.0f,
      Vector2? scale = null)
    {
      this.RegenerateFromVertices(vertices, PixelCollider.ToCollisionVector((Vector2) transform.position), rotation, scale);
    }

    public void RegenerateFromVertices(
      Vector2[] vertices,
      IntVector2 position,
      float rotation = 0.0f,
      Vector2? scale = null)
    {
      if (!scale.HasValue)
        scale = new Vector2?(new Vector2(1f, 1f));
      this.m_position = position;
      this.m_bestPixels = this.m_basePixels;
      if (vertices.Length == 0)
      {
        this.m_dimensions = IntVector2.Zero;
        this.m_basePixels.Reinitialize(0, 0, true);
      }
      else
      {
        Vector2 lhs1 = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 lhs2 = new Vector2(float.MinValue, float.MinValue);
        foreach (Vector2 vertex in vertices)
        {
          Vector3 rhs = (Vector3) vertex;
          lhs1 = Vector2.Min(lhs1, (Vector2) rhs);
          lhs2 = Vector2.Max(lhs2, (Vector2) rhs);
        }
        this.m_offset = new IntVector2(Mathf.FloorToInt(lhs1.x * 16f), Mathf.FloorToInt(lhs1.y * 16f));
        this.m_position += this.m_offset;
        this.m_dimensions = new IntVector2(Mathf.CeilToInt(lhs2.x * 16f), Mathf.CeilToInt(lhs2.y * 16f)) - this.m_offset;
        this.m_basePixels.ReinitializeWithDefault(this.m_dimensions.x, this.m_dimensions.y, false, fixedSize: true);
        this.PlotPixelLines(vertices, -PhysicsEngine.PixelToUnit(this.m_offset));
        this.FillInternalPixels();
      }
      this.SetRotationAndScale(rotation, scale.Value);
    }

    private static int ToCollisionPixel(float value) => Mathf.RoundToInt(value * 16f);

    private static IntVector2 ToCollisionVector(Vector2 value)
    {
      return new IntVector2(PixelCollider.ToCollisionPixel(value.x), PixelCollider.ToCollisionPixel(value.y));
    }

    private static float FromCollisionPixel(int value) => (float) value / 16f;

    private static Vector2 FromCollisionVector(IntVector2 value)
    {
      return new Vector2(PixelCollider.FromCollisionPixel(value.x), PixelCollider.FromCollisionPixel(value.y));
    }

    private void PlotPixelLines(Vector2[] vertices) => this.PlotPixelLines(vertices, Vector2.zero);

    private void PlotPixelLines(Vector2[] vertices, Vector2 offset)
    {
      for (int index = 0; index < vertices.Length; ++index)
      {
        IntVector2 collisionVector1 = PixelCollider.ToCollisionVector(vertices[index] + offset);
        IntVector2 collisionVector2 = PixelCollider.ToCollisionVector(vertices[(index + 1) % vertices.Length] + offset);
        if (collisionVector1.x == this.m_dimensions.x)
          --collisionVector1.x;
        if (collisionVector1.y == this.m_dimensions.y)
          --collisionVector1.y;
        if (collisionVector2.x == this.m_dimensions.x)
          --collisionVector2.x;
        if (collisionVector2.y == this.m_dimensions.y)
          --collisionVector2.y;
        this.PlotPixelLine(collisionVector1.x, collisionVector1.y, collisionVector2.x, collisionVector2.y);
      }
    }

    private void PlotPixelLine(int x0, int y0, int x1, int y1)
    {
      bool flag = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
      if (flag)
      {
        this.Swap(ref x0, ref y0);
        this.Swap(ref x1, ref y1);
      }
      if (x0 > x1)
      {
        this.Swap(ref x0, ref x1);
        this.Swap(ref y0, ref y1);
      }
      int num1 = x1 - x0;
      int num2 = Mathf.Abs(y1 - y0);
      int num3 = num1 / 2;
      int num4 = y0;
      int num5 = y0 >= y1 ? -1 : 1;
      for (int index = x0; index <= x1; ++index)
      {
        if (flag)
          this.m_basePixels[num4, index] = true;
        else
          this.m_basePixels[index, num4] = true;
        num3 -= num2;
        if (num3 < 0)
        {
          num4 += num5;
          num3 += num1;
        }
      }
    }

    private void Swap(ref int a, ref int b)
    {
      int num = a;
      a = b;
      b = num;
    }

    private void FillInternalPixels()
    {
      for (int x = 0; x < this.Width; ++x)
      {
        int num1 = -1;
        int num2 = -1;
        for (int y = 0; y < this.Height; ++y)
        {
          if (this.m_basePixels[x, y])
          {
            num1 = y;
            break;
          }
        }
        if (num1 != -1)
        {
          for (int y = this.Height - 1; y >= 0; --y)
          {
            if (this.m_basePixels[x, y])
            {
              num2 = y;
              break;
            }
          }
          for (int y = num1 + 1; y < num2; ++y)
            this.m_basePixels[x, y] = true;
        }
      }
    }

    private void UpdateSlope()
    {
      if (!this.m_slopeStart.HasValue || !this.m_slopeEnd.HasValue)
        return;
      this.IsSlope = true;
      this.Slope = (float) (((double) this.m_slopeEnd.Value.y - (double) this.m_slopeStart.Value.y) / ((double) this.m_slopeEnd.Value.x - (double) this.m_slopeStart.Value.x));
    }

    public static PixelCollider CreateRectangle(
      CollisionLayer layer,
      int x,
      int y,
      int width,
      int height,
      bool enabled = true)
    {
      return new PixelCollider()
      {
        CollisionLayer = layer,
        ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
        ManualOffsetX = x,
        ManualOffsetY = y,
        ManualWidth = width,
        ManualHeight = height,
        Enabled = enabled
      };
    }

    public enum PixelColliderGeneration
    {
      Manual,
      Tk2dPolygon,
      BagelCollider,
      Circle,
      Line,
    }

    private class PixelCache
    {
      public IntVector2 dimensions;
      public BitArray2D basePixels;
      public IntVector2 offset;
    }

    public struct StepData
    {
      public IntVector2 deltaPos;
      public float deltaTime;

      public StepData(IntVector2 deltaPos, float deltaTime)
      {
        this.deltaPos = deltaPos;
        this.deltaTime = deltaTime;
      }
    }
  }

