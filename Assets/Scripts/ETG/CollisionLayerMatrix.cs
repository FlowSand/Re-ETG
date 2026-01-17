// Decompiled with JetBrains decompiler
// Type: CollisionLayerMatrix
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
public static class CollisionLayerMatrix
{
  private static int[] m_collisionMatrix = new int[17];

  static CollisionLayerMatrix()
  {
    CollisionLayerMatrix.m_collisionMatrix[0] = CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.Projectile, CollisionLayer.Pickup, CollisionLayer.Trap);
    CollisionLayerMatrix.m_collisionMatrix[1] = CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.LowObstacle, CollisionLayer.HighObstacle, CollisionLayer.PlayerBlocker, CollisionLayer.MovingPlatform);
    CollisionLayerMatrix.m_collisionMatrix[2] = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider, CollisionLayer.Projectile, CollisionLayer.Trap);
    CollisionLayerMatrix.m_collisionMatrix[3] = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider, CollisionLayer.LowObstacle, CollisionLayer.HighObstacle, CollisionLayer.EnemyBlocker, CollisionLayer.MovingPlatform);
    CollisionLayerMatrix.m_collisionMatrix[4] = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.EnemyHitBox, CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.EnemyBulletBlocker);
    CollisionLayerMatrix.m_collisionMatrix[5] = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.EnemyCollider, CollisionLayer.LowObstacle, CollisionLayer.HighObstacle);
    CollisionLayerMatrix.m_collisionMatrix[6] = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.EnemyCollider, CollisionLayer.Projectile, CollisionLayer.LowObstacle, CollisionLayer.HighObstacle);
    CollisionLayerMatrix.m_collisionMatrix[7] = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.MovingPlatform);
    CollisionLayerMatrix.m_collisionMatrix[8] = CollisionMask.LayerToMask(CollisionLayer.Projectile);
    CollisionLayerMatrix.m_collisionMatrix[9] = CollisionMask.LayerToMask(CollisionLayer.EnemyCollider);
    CollisionLayerMatrix.m_collisionMatrix[10] = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider);
    CollisionLayerMatrix.m_collisionMatrix[11] = CollisionMask.LayerToMask(CollisionLayer.PlayerCollider, CollisionLayer.EnemyCollider, CollisionLayer.Pickup);
    CollisionLayerMatrix.m_collisionMatrix[12] = CollisionMask.LayerToMask(CollisionLayer.Projectile);
    CollisionLayerMatrix.m_collisionMatrix[13] = 0;
    CollisionLayerMatrix.m_collisionMatrix[14] = 0;
    CollisionLayerMatrix.m_collisionMatrix[15] = CollisionMask.LayerToMask(CollisionLayer.Projectile);
    CollisionLayerMatrix.m_collisionMatrix[16 /*0x10*/] = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.EnemyHitBox);
  }

  public static int GetMask(CollisionLayer layer)
  {
    return CollisionLayerMatrix.m_collisionMatrix[(int) layer];
  }

  public static bool CanCollide(CollisionLayer a, CollisionLayer b)
  {
    int num = 1 << (int) (b & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    return (CollisionLayerMatrix.m_collisionMatrix[(int) a] & num) == num;
  }
}
