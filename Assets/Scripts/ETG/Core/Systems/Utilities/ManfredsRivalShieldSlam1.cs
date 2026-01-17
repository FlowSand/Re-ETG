// Decompiled with JetBrains decompiler
// Type: ManfredsRivalShieldSlam1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("ManfredsRival/ShieldSlam1")]
    public class ManfredsRivalShieldSlam1 : Script
    {
      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ManfredsRivalShieldSlam1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      protected void FireExpandingLine(Vector2 start, Vector2 end, int numBullets)
      {
        start *= 0.5f;
        end *= 0.5f;
        for (int index = 0; index < numBullets; ++index)
        {
          Vector2 vector2 = Vector2.Lerp(start, end, (float) index / ((float) numBullets - 1f));
          this.Fire(new Offset(vector2, transform: string.Empty), new Brave.BulletScript.Direction(vector2.ToAngle()), (Bullet) new ManfredsRivalShieldSlam1.ExpandingBullet(this.Position));
        }
      }

      protected void FireSpinningLine(Vector2 start, Vector2 end, int numBullets)
      {
        start *= 0.5f;
        end *= 0.5f;
        float rotationSign = (double) BraveMathCollege.AbsAngleBetween(0.0f, (this.BulletManager.PlayerPosition() - this.Position).ToAngle()) >= 90.0 ? 1f : -1f;
        float aimDirection = this.GetAimDirection(1f, 9f);
        for (int index = 0; index < numBullets; ++index)
          this.Fire(new Offset(Vector2.Lerp(start, end, (float) index / ((float) numBullets - 1f)), transform: string.Empty), new Brave.BulletScript.Direction(aimDirection), (Bullet) new ManfredsRivalShieldSlam1.SpinningBullet(this.Position, rotationSign));
      }

      public class ExpandingBullet : Bullet
      {
        private Vector2 m_origin;

        public ExpandingBullet(Vector2 origin)
          : base("shield")
        {
          this.m_origin = origin;
          this.SuppressVfx = true;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new ManfredsRivalShieldSlam1.ExpandingBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }

      public class SpinningBullet : Bullet
      {
        private const float RotationSpeed = 8f;
        private Vector2 m_origin;
        private float m_rotationSign;

        public SpinningBullet(Vector2 origin, float rotationSign)
          : base("sword")
        {
          this.m_origin = origin;
          this.m_rotationSign = rotationSign;
          this.SuppressVfx = true;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new ManfredsRivalShieldSlam1.SpinningBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
