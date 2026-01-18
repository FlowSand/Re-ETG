// Decompiled with JetBrains decompiler
// Type: CollisionMask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

  public static class CollisionMask
  {
    public const int None = 0;
    public const int All = 2147483647 /*0x7FFFFFFF*/;
    public static readonly int StandardPlayerVisibilityMask = CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker);
    public static readonly int StandardEnemyVisibilityMask = CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider, CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyBulletBlocker);
    public static readonly int BothEnemyVisibilityMask = CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider, CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker, CollisionLayer.EnemyBulletBlocker);
    public static readonly int WallOnlyEnemyVisibilityMask = CollisionMask.LayerToMask(CollisionLayer.HighObstacle, CollisionLayer.BulletBlocker);

    public static int LayerToMask(CollisionLayer layer)
    {
      return 1 << (int) (layer & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    }

    public static int LayerToMask(CollisionLayer layer1, CollisionLayer layer2)
    {
      return 1 << (int) (layer1 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer2 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    }

    public static int LayerToMask(
      CollisionLayer layer1,
      CollisionLayer layer2,
      CollisionLayer layer3)
    {
      return 1 << (int) (layer1 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer2 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer3 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    }

    public static int LayerToMask(
      CollisionLayer layer1,
      CollisionLayer layer2,
      CollisionLayer layer3,
      CollisionLayer layer4)
    {
      return 1 << (int) (layer1 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer2 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer3 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer4 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    }

    public static int LayerToMask(
      CollisionLayer layer1,
      CollisionLayer layer2,
      CollisionLayer layer3,
      CollisionLayer layer4,
      CollisionLayer layer5)
    {
      return 1 << (int) (layer1 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer2 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer3 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer4 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer5 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    }

    public static int LayerToMask(
      CollisionLayer layer1,
      CollisionLayer layer2,
      CollisionLayer layer3,
      CollisionLayer layer4,
      CollisionLayer layer5,
      CollisionLayer layer6)
    {
      return 1 << (int) (layer1 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer2 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer3 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer4 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer5 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer6 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    }

    public static int LayerToMask(
      CollisionLayer layer1,
      CollisionLayer layer2,
      CollisionLayer layer3,
      CollisionLayer layer4,
      CollisionLayer layer5,
      CollisionLayer layer6,
      CollisionLayer layer7)
    {
      return 1 << (int) (layer1 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer2 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer3 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer4 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer5 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer6 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap)) | 1 << (int) (layer7 & (CollisionLayer.EnemyBulletBlocker | CollisionLayer.Trap));
    }

    public static int GetComplexEnemyVisibilityMask(bool canTargetPlayers, bool canTargetEnemies)
    {
      if (canTargetPlayers && canTargetEnemies)
        return CollisionMask.BothEnemyVisibilityMask;
      if (!canTargetEnemies)
        return CollisionMask.StandardEnemyVisibilityMask;
      return !canTargetPlayers ? CollisionMask.StandardPlayerVisibilityMask : CollisionMask.WallOnlyEnemyVisibilityMask;
    }
  }

