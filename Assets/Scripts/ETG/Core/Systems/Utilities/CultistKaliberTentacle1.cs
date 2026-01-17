// Decompiled with JetBrains decompiler
// Type: CultistKaliberTentacle1
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
    public class CultistKaliberTentacle1 : Script
    {
      private const int NumTentacles = 4;
      private const int NumBullets = 8;
      private const int BulletSpeed = 10;
      private const float TentacleMagnitude = 0.75f;
      private const float TentaclePeriod = 3f;
      private Vector2?[] m_targetPositions;

      protected override IEnumerator Top()
      {
        this.m_targetPositions = new Vector2?[4];
        float aimDirection = this.AimDirection;
        for (int index1 = 0; index1 < 4; ++index1)
        {
          float direction = this.SubdivideArc(aimDirection - 65f, 130f, 4, index1) + Random.Range(-6f, 6f);
          for (int index2 = 0; index2 < 8; ++index2)
            this.Fire(new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(10f), (Bullet) new CultistKaliberTentacle1.TentacleBullet(index2 * 6, this, index1, (double) index1 >= 2.0 ? -1f : 1f));
        }
        return (IEnumerator) null;
      }

      public Vector2 GetTargetPosition(int index, Bullet bullet)
      {
        if (!this.m_targetPositions[index].HasValue)
          this.m_targetPositions[index] = new Vector2?(bullet.GetPredictedTargetPosition((double) Random.value >= 0.5 ? 1f : 0.0f, 10f));
        return this.m_targetPositions[index].Value;
      }

      public class TentacleBullet : Bullet
      {
        private int m_delay;
        private CultistKaliberTentacle1 m_parentScript;
        private int m_index;
        private float m_offsetScalar;
        private Vector2 m_target;

        public TentacleBullet(
          int delay,
          CultistKaliberTentacle1 parentScript,
          int index,
          float offsetScalar)
          : base()
        {
          this.m_delay = delay;
          this.m_parentScript = parentScript;
          this.m_index = index;
          this.m_offsetScalar = offsetScalar;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new CultistKaliberTentacle1.TentacleBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
