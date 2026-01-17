// Decompiled with JetBrains decompiler
// Type: CollisionData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CollisionData : CastResult
{
  public float TimeUsed;
  public bool CollidedX;
  public bool CollidedY;
  public IntVector2 NewPixelsToMove;
  public bool Overlap;
  public CollisionData.CollisionType collisionType;
  public SpeculativeRigidbody MyRigidbody;
  public SpeculativeRigidbody OtherRigidbody;
  public string TileLayerName;
  public IntVector2 TilePosition;
  public bool IsPushCollision;
  public bool IsInverse;
  public static ObjectPool<CollisionData> Pool;

  private CollisionData()
  {
  }

  public Vector2 PostCollisionUnitCenter
  {
    get => this.MyRigidbody.UnitCenter + PhysicsEngine.PixelToUnit(this.NewPixelsToMove);
  }

  public void SetAll(LinearCastResult res)
  {
    this.Contact = res.Contact;
    this.Normal = res.Normal;
    this.MyPixelCollider = res.MyPixelCollider;
    this.OtherPixelCollider = res.OtherPixelCollider;
    this.TimeUsed = res.TimeUsed;
    this.NewPixelsToMove = res.NewPixelsToMove;
    this.CollidedX = res.CollidedX;
    this.CollidedY = res.CollidedY;
    this.Overlap = res.Overlap;
  }

  public void SetAll(CollisionData data)
  {
    this.Contact = data.Contact;
    this.Normal = data.Normal;
    this.MyPixelCollider = data.MyPixelCollider;
    this.OtherPixelCollider = data.OtherPixelCollider;
    this.TimeUsed = data.TimeUsed;
    this.NewPixelsToMove = data.NewPixelsToMove;
    this.CollidedX = data.CollidedX;
    this.CollidedY = data.CollidedY;
    this.Overlap = data.Overlap;
    this.collisionType = data.collisionType;
    this.MyRigidbody = data.MyRigidbody;
    this.OtherRigidbody = data.OtherRigidbody;
    this.TileLayerName = data.TileLayerName;
    this.TilePosition = data.TilePosition;
    this.IsPushCollision = data.IsPushCollision;
    this.IsInverse = data.IsInverse;
  }

  public bool IsTriggerCollision
  {
    get
    {
      if (this.MyPixelCollider != null && this.MyPixelCollider.IsTrigger)
        return true;
      return this.OtherPixelCollider != null && this.OtherPixelCollider.IsTrigger;
    }
  }

  public CollisionData GetInverse()
  {
    CollisionData inverse = CollisionData.Pool.Allocate();
    inverse.Contact = this.Contact;
    inverse.Normal = -this.Normal;
    inverse.MyPixelCollider = this.OtherPixelCollider;
    inverse.OtherPixelCollider = this.MyPixelCollider;
    inverse.TimeUsed = this.TimeUsed;
    inverse.CollidedX = this.CollidedX;
    inverse.CollidedY = this.CollidedY;
    inverse.NewPixelsToMove = new IntVector2(-this.NewPixelsToMove.x, -this.NewPixelsToMove.y);
    inverse.Overlap = this.Overlap;
    inverse.collisionType = this.collisionType;
    inverse.MyRigidbody = this.OtherRigidbody;
    inverse.OtherRigidbody = this.MyRigidbody;
    inverse.TileLayerName = this.TileLayerName;
    inverse.TilePosition = this.TilePosition;
    inverse.IsPushCollision = this.IsPushCollision;
    inverse.IsInverse = true;
    return inverse;
  }

  public static void Cleanup(CollisionData collisionData)
  {
    collisionData.Contact.x = 0.0f;
    collisionData.Contact.y = 0.0f;
    collisionData.Normal.x = 0.0f;
    collisionData.Normal.y = 0.0f;
    collisionData.MyPixelCollider = (PixelCollider) null;
    collisionData.OtherPixelCollider = (PixelCollider) null;
    collisionData.TimeUsed = 0.0f;
    collisionData.CollidedX = false;
    collisionData.CollidedY = false;
    collisionData.NewPixelsToMove.x = 0;
    collisionData.NewPixelsToMove.y = 0;
    collisionData.Overlap = false;
    collisionData.collisionType = CollisionData.CollisionType.Rigidbody;
    collisionData.MyRigidbody = (SpeculativeRigidbody) null;
    collisionData.OtherRigidbody = (SpeculativeRigidbody) null;
    collisionData.TileLayerName = (string) null;
    collisionData.TilePosition.x = 0;
    collisionData.TilePosition.y = 0;
    collisionData.IsPushCollision = false;
    collisionData.IsInverse = false;
  }

  static CollisionData()
  {
    ObjectPool<CollisionData>.Factory factory = (ObjectPool<CollisionData>.Factory) (() => new CollisionData());
    // ISSUE: reference to a compiler-generated field
    if (CollisionData.\u003C\u003Ef__mg\u0024cache0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      CollisionData.\u003C\u003Ef__mg\u0024cache0 = new ObjectPool<CollisionData>.Cleanup(CollisionData.Cleanup);
    }
    // ISSUE: reference to a compiler-generated field
    ObjectPool<CollisionData>.Cleanup fMgCache0 = CollisionData.\u003C\u003Ef__mg\u0024cache0;
    CollisionData.Pool = new ObjectPool<CollisionData>(factory, 10, fMgCache0);
  }

  public enum CollisionType
  {
    Rigidbody,
    TileMap,
    PathEnd,
    MovementRestriction,
    Pushable,
  }
}
