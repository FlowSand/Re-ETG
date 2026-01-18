using Brave.BulletScript;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ShotgunExecutionerChain1 : Script
  {
    private const int NumArmBullets = 20;
    private const int NumVolley = 3;
    private const int FramesBetweenVolleys = 30;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ShotgunExecutionerChain1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private ShotgunExecutionerChain1.HandBullet FireVolley(int volleyIndex, float speed)
    {
      ShotgunExecutionerChain1.HandBullet handBullet = new ShotgunExecutionerChain1.HandBullet(this);
      this.Fire(new Brave.BulletScript.Direction(this.AimDirection), new Brave.BulletScript.Speed(speed), (Bullet) handBullet);
      for (int index = 0; index < 20; ++index)
        this.Fire(new Brave.BulletScript.Direction(this.AimDirection), (Bullet) new ShotgunExecutionerChain1.ArmBullet(this, handBullet, index));
      return handBullet;
    }

    private class ArmBullet : Bullet
    {
      public const int BulletDelay = 60;
      private const float WiggleMagnitude = 0.4f;
      public const int WiggleTime = 30;
      private const int NumBulletsToPreShake = 5;
      private ShotgunExecutionerChain1 m_parentScript;
      private ShotgunExecutionerChain1 shotgunExecutionerChain1;
      private ShotgunExecutionerChain1.HandBullet m_handBullet;
      private int m_index;

      public ArmBullet(
        ShotgunExecutionerChain1 parentScript,
        ShotgunExecutionerChain1.HandBullet handBullet,
        int index)
        : base("chain")
      {
        this.m_parentScript = parentScript;
        this.m_handBullet = handBullet;
        this.m_index = index;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShotgunExecutionerChain1.ArmBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    private class HandBullet : Bullet
    {
      private ShotgunExecutionerChain1 m_parentScript;

      public HandBullet(ShotgunExecutionerChain1 parentScript)
        : base("chain")
      {
        this.m_parentScript = parentScript;
      }

      public bool HasStopped { get; set; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ShotgunExecutionerChain1.HandBullet__Topc__Iterator0()
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

