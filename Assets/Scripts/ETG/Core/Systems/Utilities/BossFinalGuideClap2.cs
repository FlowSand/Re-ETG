// Decompiled with JetBrains decompiler
// Type: BossFinalGuideClap2
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
    [InspectorDropdownName("Bosses/BossFinalGuide/Clap2")]
    public class BossFinalGuideClap2 : Script
    {
      private const int SetupTime = 40;
      private const int HoldTime = 90;
      private const float FireSpeed = 8f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalGuideClap2.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void FireLine(Vector2 spawn, Vector2 start, float direction)
      {
        this.FireLine(spawn, start, start, 1, direction);
      }

      private void FireLine(
        Vector2 start,
        Vector2 end,
        int numBullets,
        float direction,
        float timeMultiplier = 1f,
        bool lerpSpeed = false)
      {
        this.FireLine(start, start, end, numBullets, direction, timeMultiplier, lerpSpeed);
      }

      private void FireLine(
        Vector2 spawnPoint,
        Vector2 start,
        Vector2 end,
        int numBullets,
        float direction,
        float timeMultiplier = 1f,
        bool lerpSpeed = false)
      {
        Vector2 vector2 = (end - start) / (float) Mathf.Max(1, numBullets - 1);
        float num = 0.6666667f * timeMultiplier;
        for (int index = 0; index < numBullets; ++index)
        {
          Vector2 a = numBullets != 1 ? start + vector2 * (float) index : end;
          float speed = Vector2.Distance(a, spawnPoint) / num;
          this.Fire(new Offset(spawnPoint, transform: string.Empty), new Brave.BulletScript.Direction((a - spawnPoint).ToAngle()), new Brave.BulletScript.Speed(speed), (Bullet) new BossFinalGuideClap2.WingBullet(direction, !lerpSpeed ? 1f : (float) index / (float) numBullets, timeMultiplier));
        }
      }

      public class WingBullet : Bullet
      {
        private float m_direction;
        private float m_speedT;
        private float m_timeMultiplier;

        public WingBullet(float direction, float speedT, float timeMultiplier)
          : base()
        {
          this.m_direction = direction;
          this.m_speedT = speedT;
          this.m_timeMultiplier = timeMultiplier;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossFinalGuideClap2.WingBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
