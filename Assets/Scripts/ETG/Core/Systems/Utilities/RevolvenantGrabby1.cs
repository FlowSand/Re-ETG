// Decompiled with JetBrains decompiler
// Type: RevolvenantGrabby1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class RevolvenantGrabby1 : Script
    {
      private const int NumArmBullets = 8;
      private const int ArmSpawnDelay = 3;
      private const int ArmDestroyTime = 4;
      private const int NumCircleBullets = 6;
      private const float CircleRadius = 3f;
      private const float CircleSpeed = 120f;
      private const int InitialHandAttackDelay = 30;
      private const int HandAttackTime = 120;
      private const int NumHandAttacks = 2;
      private RevolvenantGrabby1.ArmBullet firstLeftBullet;
      private RevolvenantGrabby1.ArmBullet firstRightBullet;
      private RevolvenantGrabby1.HandBullet leftHandBullet;
      private RevolvenantGrabby1.HandBullet rightHandBullet;

      public bool Aborting { get; set; }

      public bool NearDone { get; set; }

      public bool DoShrink { get; set; }

      public Vector2 PlayerPos { get; set; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new RevolvenantGrabby1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private bool ShouldAbort(bool checkHands = true)
      {
        this.Aborting = this.firstLeftBullet.Destroyed || this.firstRightBullet.Destroyed;
        if (checkHands)
        {
          RevolvenantGrabby1 revolvenantGrabby1 = this;
          revolvenantGrabby1.Aborting = ((revolvenantGrabby1.Aborting ? 1 : 0) | (!this.leftHandBullet.Destroyed ? 0 : (this.rightHandBullet.Destroyed ? 1 : 0))) != 0;
        }
        return this.Aborting;
      }

      private class ArmBullet : Bullet
      {
        private RevolvenantGrabby1 m_parentScript;
        private string m_armTransform;
        private int m_index;
        private float m_offsetAngle;

        public ArmBullet(
          RevolvenantGrabby1 parentScript,
          string armTransform,
          int i,
          float offsetAngle)
          : base()
        {
          this.m_parentScript = parentScript;
          this.m_armTransform = armTransform;
          this.m_index = i;
          this.m_offsetAngle = offsetAngle;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new RevolvenantGrabby1.ArmBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }

        public Vector2 GetArmPosition(float circleRadius = 3f)
        {
          Vector2 a = this.BulletManager.TransformOffset(this.m_parentScript.Position, this.m_armTransform);
          Vector2 playerPos = this.m_parentScript.PlayerPos;
          Vector2 b = playerPos + (playerPos - this.m_parentScript.Position).Rotate(this.m_offsetAngle).normalized * circleRadius;
          float t = (float) this.m_index / 7f;
          Vector2 vector2 = (b - a).Rotate(this.m_offsetAngle).normalized * (Mathf.Sin(t * 3.14159274f) * 0.5f * Mathf.PingPong((float) (this.Tick + this.m_index * 3) / 75f, 1f));
          if ((bool) (Object) this.Projectile)
          {
            float num = BraveMathCollege.ClampAngle360((b - a).ToAngle());
            if ((double) this.m_offsetAngle < 0.0 && (double) num > 90.0 && (double) num < 210.0 || (double) this.m_offsetAngle > 0.0 && (double) num < 90.0 && (double) num > -30.0)
              this.Projectile.sprite.HeightOffGround = 0.0f;
            else
              this.Projectile.sprite.HeightOffGround = (float) ((1.0 - (double) t) * 10.0);
          }
          return Vector2.Lerp(a, b, t) + vector2;
        }
      }

      private class CircleBullet : Bullet
      {
        private RevolvenantGrabby1 m_parentScript;
        private float m_angle;
        private float m_desiredAngle;

        public CircleBullet(RevolvenantGrabby1 parentScript, float angle, float desiredAngle)
          : base()
        {
          this.m_parentScript = parentScript;
          this.m_angle = angle;
          this.m_desiredAngle = desiredAngle;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new RevolvenantGrabby1.CircleBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }

      private class HandBullet : Bullet
      {
        private const int AttackTime = 75;
        private const int ResetTime = 30;
        public float Angle;
        private RevolvenantGrabby1.HandBullet.State m_state;
        private bool m_hasDoneTell;
        private RevolvenantGrabby1 m_parentScript;
        private int m_stateChangeTimer;

        public HandBullet(RevolvenantGrabby1 parentScript, int initialAttackDelay)
          : base("hand")
        {
          this.m_parentScript = parentScript;
          this.m_stateChangeTimer = initialAttackDelay;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new RevolvenantGrabby1.HandBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }

        private enum State
        {
          Spin,
          Attack,
          Return,
        }
      }
    }

}
