// Decompiled with JetBrains decompiler
// Type: InfinilichMorphBoomerang1
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
    [InspectorDropdownName("Bosses/Infinilich/MorphBoomerang1")]
    public class InfinilichMorphBoomerang1 : Script
    {
      private const float EnemyBulletSpeedItem = 12f;
      private const float RotationSpeed = -5f;
      private float m_sign;

      protected override IEnumerator Top()
      {
        float num = BraveMathCollege.ClampAngle180(this.BulletBank.aiAnimator.FacingDirection);
        this.m_sign = (double) num > 90.0 || (double) num < -90.0 ? -1f : 1f;
        Vector2 vector2_1 = this.Position + new Vector2(this.m_sign * 2.5f, 1f);
        float angle = (this.BulletManager.PlayerPosition() - vector2_1).ToAngle();
        for (int index = 1; index <= 43; ++index)
        {
          string transform = "morph bullet " + (object) index;
          Vector2 vector2_2 = this.BulletManager.TransformOffset(Vector2.zero, transform);
          this.Fire(new Offset(transform), new Brave.BulletScript.Direction(angle), new Brave.BulletScript.Speed(12f), (Bullet) new InfinilichMorphBoomerang1.BoomerangBullet(vector2_1 - vector2_2, this.m_sign));
        }
        return (IEnumerator) null;
      }

      public class BoomerangBullet : Bullet
      {
        private Vector2 m_centerOfMassOffset;
        private float m_sign;

        public BoomerangBullet(Vector2 centerOfMassOffset, float sign)
          : base()
        {
          this.m_centerOfMassOffset = centerOfMassOffset;
          this.m_sign = sign;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new InfinilichMorphBoomerang1.BoomerangBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
