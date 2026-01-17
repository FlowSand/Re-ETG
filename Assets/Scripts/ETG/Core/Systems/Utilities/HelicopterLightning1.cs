// Decompiled with JetBrains decompiler
// Type: HelicopterLightning1
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
    [InspectorDropdownName("Bosses/Helicopter/Lightning1")]
    public class HelicopterLightning1 : Script
    {
      public const float Dist = 0.8f;
      public const int MaxBulletDepth = 40;
      public const float RandomOffset = 0.3f;
      public const float TurnChance = 0.2f;
      public const float TurnAngle = 30f;

      protected override IEnumerator Top()
      {
        float direction = BraveMathCollege.QuantizeFloat(this.AimDirection, 45f);
        this.PostWwiseEvent("Play_BOSS_agunim_ribbons_01");
        this.Fire(new Offset(-0.5f, 0.5f, transform: string.Empty), (Bullet) new HelicopterLightning1.LightningBullet(direction, -1f, 40, -4));
        this.Fire(new Offset(-0.5f, 0.5f, transform: string.Empty), (Bullet) new HelicopterLightning1.LightningBullet(direction, -1f, 40, 4));
        this.Fire(new Offset(-0.5f, 0.5f, transform: string.Empty), (Bullet) new HelicopterLightning1.LightningBullet(direction, -1f, 40, 4));
        this.Fire(new Offset(0.5f, 0.5f, transform: string.Empty), (Bullet) new HelicopterLightning1.LightningBullet(direction, 1f, 40, 4));
        this.Fire(new Offset(0.5f, 0.5f, transform: string.Empty), (Bullet) new HelicopterLightning1.LightningBullet(direction, 1f, 40, 4));
        this.Fire(new Offset(0.5f, 0.5f, transform: string.Empty), (Bullet) new HelicopterLightning1.LightningBullet(direction, 1f, 40, -4));
        return (IEnumerator) null;
      }

      private class LightningBullet : Bullet
      {
        private float m_direction;
        private float m_sign;
        private int m_maxRemainingBullets;
        private int m_timeSinceLastTurn;
        private Vector2? m_truePosition;

        public LightningBullet(
          float direction,
          float sign,
          int maxRemainingBullets,
          int timeSinceLastTurn,
          Vector2? truePosition = null)
          : base()
        {
          this.m_direction = direction;
          this.m_sign = sign;
          this.m_maxRemainingBullets = maxRemainingBullets;
          this.m_timeSinceLastTurn = timeSinceLastTurn;
          this.m_truePosition = truePosition;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new HelicopterLightning1.LightningBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
