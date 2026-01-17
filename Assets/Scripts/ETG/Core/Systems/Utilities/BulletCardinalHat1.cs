// Decompiled with JetBrains decompiler
// Type: BulletCardinalHat1
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
    public class BulletCardinalHat1 : Script
    {
      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletCardinalHat1__Topc__Iterator0()
        {
          _this = this
        };
      }

      private void FireSpinningLine(Vector2 start, Vector2 end, int numBullets)
      {
        start *= 0.5f;
        end *= 0.5f;
        float angle = (this.BulletManager.PlayerPosition() - this.Position).ToAngle();
        for (int index = 0; index < numBullets; ++index)
        {
          Vector2 vector2 = Vector2.Lerp(start, end, (float) index / ((float) numBullets - 1f));
          this.Fire(new Brave.BulletScript.Direction(angle), (Bullet) new BulletCardinalHat1.SpinningBullet(this.Position, this.Position + vector2));
        }
      }

      public class SpinningBullet : Bullet
      {
        private const float RotationSpeed = 6f;
        private Vector2 m_origin;
        private Vector2 m_startPos;

        public SpinningBullet(Vector2 origin, Vector2 startPos)
          : base("hat")
        {
          this.m_origin = origin;
          this.m_startPos = startPos;
          this.SuppressVfx = true;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BulletCardinalHat1.SpinningBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
