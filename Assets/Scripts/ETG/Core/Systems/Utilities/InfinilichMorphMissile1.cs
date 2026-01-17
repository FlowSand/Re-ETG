// Decompiled with JetBrains decompiler
// Type: InfinilichMorphMissile1
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

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/Infinilich/MorphMissile1")]
    public class InfinilichMorphMissile1 : Script
    {
      private const float EnemyBulletSpeedItem = 5f;
      private static int[] RightBoosters = new int[9]
      {
        1,
        2,
        4,
        7,
        15,
        24,
        32 /*0x20*/,
        35,
        37
      };
      private static int[] LeftBoosters = new int[9]
      {
        1,
        3,
        6,
        14,
        23,
        31 /*0x1F*/,
        34,
        36,
        37
      };
      private float m_sign;

      protected override IEnumerator Top()
      {
        float num = BraveMathCollege.ClampAngle180(this.BulletBank.aiAnimator.FacingDirection);
        this.m_sign = (double) num > 90.0 || (double) num < -90.0 ? -1f : 1f;
        Vector2 vector2_1 = this.Position + new Vector2(this.m_sign * 2.5f, 0.5f);
        for (int index = 1; index <= 37; ++index)
        {
          string transform = "morph bullet " + (object) index;
          bool isBooster = Array.IndexOf<int>((double) this.m_sign <= 0.0 ? InfinilichMorphMissile1.LeftBoosters : InfinilichMorphMissile1.RightBoosters, index) >= 0;
          Vector2 vector2_2 = this.BulletManager.TransformOffset(Vector2.zero, transform);
          this.Fire(new Offset(transform), new Brave.BulletScript.Direction((double) this.m_sign <= 0.0 ? 180f : 0.0f), new Brave.BulletScript.Speed(5f), (Bullet) new InfinilichMorphMissile1.MissileBullet(vector2_1 - vector2_2, this.m_sign, isBooster));
        }
        return (IEnumerator) null;
      }

      public class MissileBullet : Bullet
      {
        private Vector2 m_centerOfMassOffset;
        private float m_sign;
        private bool m_isBooster;

        public MissileBullet(Vector2 centerOfMassOffset, float sign, bool isBooster)
          : base()
        {
          this.m_centerOfMassOffset = centerOfMassOffset;
          this.m_sign = sign;
          this.m_isBooster = isBooster;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new InfinilichMorphMissile1.MissileBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
