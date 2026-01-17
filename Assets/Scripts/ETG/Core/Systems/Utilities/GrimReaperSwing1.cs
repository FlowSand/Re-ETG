// Decompiled with JetBrains decompiler
// Type: GrimReaperSwing1
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
    public class GrimReaperSwing1 : Script
    {
      private const int NumBullets = 12;

      protected override IEnumerator Top()
      {
        Vector2 zero = Vector2.zero;
        Vector2 p1_1 = zero + new Vector2(-1.5f, -1.5f);
        Vector2 p3_1 = new Vector2(2f, -5f);
        Vector2 p2_1 = p3_1 + new Vector2(-1.5f, 0.4f);
        Vector2 p0 = new Vector2(-0.5f, -1.5f);
        Vector2 p1_2 = p0 + new Vector2(0.75f, 0.75f);
        Vector2 p3_2 = new Vector2(-0.5f, 1.5f);
        Vector2 p2_2 = p3_2 + new Vector2(0.75f, -0.75f);
        float num = BraveMathCollege.ClampAngle180((this.BulletManager.PlayerPosition() - this.BulletBank.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle());
        bool flag = (double) num > -45.0 && (double) num < 120.0;
        Vector2 phantomBulletPoint = this.Position + BraveMathCollege.CalculateBezierPoint(0.5f, zero, p1_1, p2_1, p3_1);
        for (int index = 0; index < 12; ++index)
        {
          float t = (float) index / 11f;
          Vector2 bezierPoint1 = BraveMathCollege.CalculateBezierPoint(t, zero, p1_1, p2_1, p3_1);
          if (flag)
            t = 1f - t;
          Vector2 bezierPoint2 = BraveMathCollege.CalculateBezierPoint(t, p0, p1_2, p2_2, p3_2);
          this.Fire(new Offset(bezierPoint1, transform: string.Empty), new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(8f), (Bullet) new GrimReaperSwing1.ReaperBullet(phantomBulletPoint, bezierPoint2));
        }
        return (IEnumerator) null;
      }

      public class ReaperBullet : Bullet
      {
        private Vector2 m_phantomBulletPoint;
        private Vector2 m_offset;

        public ReaperBullet(Vector2 phantomBulletPoint, Vector2 offset)
          : base()
        {
          this.m_phantomBulletPoint = phantomBulletPoint;
          this.m_offset = offset;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new GrimReaperSwing1.ReaperBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
