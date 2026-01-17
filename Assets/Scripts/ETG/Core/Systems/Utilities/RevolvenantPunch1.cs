// Decompiled with JetBrains decompiler
// Type: RevolvenantPunch1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class RevolvenantPunch1 : Script
    {
      private const int NumArmBullets = 20;

      public bool Spew { get; set; }

      public Vector2 ArmPosition { get; set; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RevolvenantPunch1.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      private class ArmBullet : Bullet
      {
        public const int NumBullets = 3;
        public const int BulletDelay = 60;
        private const float WiggleMagnitude = 0.4f;
        private const int WiggleTime = 30;
        private const int NumBulletsToPreShake = 5;
        private RevolvenantPunch1 m_parentScript;
        private RevolvenantPunch1 revolvenantPunch1;
        private RevolvenantPunch1.HandBullet m_handBullet;
        private int m_index;

        public ArmBullet(
          RevolvenantPunch1 parentScript,
          RevolvenantPunch1.HandBullet handBullet,
          int index)
          : base()
        {
          this.m_parentScript = parentScript;
          this.m_handBullet = handBullet;
          this.m_index = index;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new RevolvenantPunch1.ArmBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }

      private class HandBullet : Bullet
      {
        private RevolvenantPunch1 m_parentScript;

        public HandBullet(RevolvenantPunch1 parentScript)
          : base("hand")
        {
          this.m_parentScript = parentScript;
        }

        public bool HasStopped { get; set; }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new RevolvenantPunch1.HandBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }

        private void OnCollision(CollisionData collision)
        {
          bool flag = collision.collisionType == CollisionData.CollisionType.TileMap;
          SpeculativeRigidbody otherRigidbody = collision.OtherRigidbody;
          if ((bool) (UnityEngine.Object) otherRigidbody)
            flag = (bool) (UnityEngine.Object) otherRigidbody.majorBreakable || otherRigidbody.PreventPiercing || !(bool) (UnityEngine.Object) otherRigidbody.gameActor && !(bool) (UnityEngine.Object) otherRigidbody.minorBreakable;
          if (flag)
          {
            this.Position = collision.MyRigidbody.UnitCenter + PhysicsEngine.PixelToUnit(collision.NewPixelsToMove);
            this.Speed = 0.0f;
            this.HasStopped = true;
            PhysicsEngine.PostSliceVelocity = new Vector2?(new Vector2(0.0f, 0.0f));
            this.Projectile.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
          }
          else
            PhysicsEngine.PostSliceVelocity = new Vector2?(collision.MyRigidbody.Velocity);
        }

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          if ((bool) (UnityEngine.Object) this.Projectile)
            this.Projectile.specRigidbody.OnCollision -= new Action<CollisionData>(this.OnCollision);
          this.HasStopped = true;
        }
      }
    }

}
