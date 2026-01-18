using Brave.BulletScript;
using FullInspector;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Chancebulon/BlobProjectileAttack1")]
public class ChancebulonBlobProjectileAttack1 : Script
  {
    private const int NumBullets = 5;
    private const int TotalTime = 200;
    public const float BulletSpeed = 10f;

    protected override IEnumerator Top()
    {
      ChancebulonBlobProjectileAttack1.BlobType blobType = (ChancebulonBlobProjectileAttack1.BlobType) UnityEngine.Random.Range(0, 3);
      for (int index = 0; index < 5; ++index)
      {
        float num = this.RandomAngle();
        this.Fire(new Offset(1f, rotation: num, transform: string.Empty), new Brave.BulletScript.Direction(num), new Brave.BulletScript.Speed(UnityEngine.Random.Range(4f, 11f)), (Bullet) new ChancebulonBlobProjectileAttack1.BlobulonBullet(blobType));
      }
      return (IEnumerator) null;
    }

    public enum BlobType
    {
      Normal,
      Poison,
      Lead,
    }

    public class BlobulonBullet : Bullet
    {
      private static string[] Projectiles = new string[3]
      {
        "blobulon",
        "blobulon",
        "blobuloid"
      };
      private ChancebulonBlobProjectileAttack1.BlobType m_blobType;

      public BlobulonBullet(ChancebulonBlobProjectileAttack1.BlobType blobType)
        : base()
      {
        this.BankName = BraveUtility.RandomElement<string>(ChancebulonBlobProjectileAttack1.BlobulonBullet.Projectiles);
        this.m_blobType = blobType;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ChancebulonBlobProjectileAttack1.BlobulonBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      private void OnTileCollision(CollisionData tilecollision)
      {
        float angle1 = (-tilecollision.MyRigidbody.Velocity).ToAngle();
        float angle2 = tilecollision.Normal.ToAngle();
        this.Direction = BraveMathCollege.ClampAngle360(angle1 + (float) (2.0 * ((double) angle2 - (double) angle1))) + UnityEngine.Random.Range(-30f, 30f);
        this.Velocity = BraveMathCollege.DegreesToVector(this.Direction, this.Speed);
        PhysicsEngine.PostSliceVelocity = new Vector2?(this.Velocity);
      }

      private void OnAnimationCompleted(
        tk2dSpriteAnimator tk2DSpriteAnimator,
        tk2dSpriteAnimationClip tk2DSpriteAnimationClip)
      {
        this.Vanish(true);
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (!(bool) (UnityEngine.Object) this.Projectile)
          return;
        this.Projectile.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
        this.Projectile.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
      }
    }
  }

