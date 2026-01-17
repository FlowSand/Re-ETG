// Decompiled with JetBrains decompiler
// Type: FusebombFlames1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/Fusebomb/Flames1")]
    public class FusebombFlames1 : Script
    {
      public const int NumFlameRows = 4;
      public const int NumFlamesPerRow = 6;

      protected override IEnumerator Top()
      {
        List<AIActor> spawnedActors = new List<AIActor>();
        Vector2 vector2 = this.BulletBank.aiActor.ParentRoom.area.UnitBottomLeft + new Vector2(1f, 4.8f);
        float max = (float) (this.BulletBank.aiActor.ParentRoom.area.dimensions.x - 2);
        float num1 = max / 4f;
        for (int index1 = 0; index1 < 4; ++index1)
        {
          float x = (float) ((double) Random.Range(0.0f, max) % (double) num1 + (double) num1 * (double) index1);
          float num2 = Random.Range(0.0f, 0.8f);
          for (int index2 = 0; index2 < 6; ++index2)
          {
            Vector2 goalPos = vector2 + new Vector2(x, (float) index2 + num2);
            this.Fire(new Brave.BulletScript.Direction((goalPos - this.Position).ToAngle()), (Bullet) new FusebombFlames1.FlameBullet(goalPos, spawnedActors, 60 + 10 * index1 + 10 * index2));
          }
        }
        return (IEnumerator) null;
      }

      private class FlameBullet : Bullet
      {
        private Vector2 m_goalPos;
        private int m_flightTime;

        public FlameBullet(Vector2 goalPos, List<AIActor> spawnedActors, int flightTime)
          : base("flame")
        {
          this.m_goalPos = goalPos;
          this.m_flightTime = flightTime;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new FusebombFlames1.FlameBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
