// Decompiled with JetBrains decompiler
// Type: BlobulordSplitAttack1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("Bosses/Blobulord/SplitAttack1")]
    public class BlobulordSplitAttack1 : Script
    {
      private const int NumBullets = 32 /*0x20*/;
      private const int TotalTime = 352;
      private const float BulletSpeed = 10f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BlobulordSplitAttack1.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      public class BlobulonBullet : Bullet
      {
        private static string[] Projectiles = new string[3]
        {
          "blobulon",
          "blobulon",
          "blobuloid"
        };
        private Vector2 m_spawnPoint;
        private int m_spawnDelay;
        private bool m_doSpawn;

        public BlobulonBullet(Vector2 spawnPoint, int spawnDelay = 0, bool doSpawn = false)
          : base()
        {
          this.BankName = BraveUtility.RandomElement<string>(BlobulordSplitAttack1.BlobulonBullet.Projectiles);
          this.m_spawnPoint = spawnPoint;
          this.m_spawnDelay = spawnDelay;
          this.m_doSpawn = doSpawn;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BlobulordSplitAttack1.BlobulonBullet.<Top>c__Iterator0()
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

}
