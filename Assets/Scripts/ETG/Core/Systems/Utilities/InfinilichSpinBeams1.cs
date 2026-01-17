// Decompiled with JetBrains decompiler
// Type: InfinilichSpinBeams1
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
    [InspectorDropdownName("Bosses/Infinilich/SpinBeams1")]
    public class InfinilichSpinBeams1 : Script
    {
      private const int TurnSpeed = 3;
      private const int TurnDelay = 3;
      private const int BeamSetupTime = 7;
      private const int BeamLifeTime = 12;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new InfinilichSpinBeams1.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator FireBeam(Vector2 pos, float direction)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new InfinilichSpinBeams1.<FireBeam>c__Iterator1()
        {
          direction = direction,
          pos = pos,
          _this = this
        };
      }

      public class BeamBullet : Bullet
      {
        private int m_spawnDelay;

        public BeamBullet(int spawnDelay)
          : base()
        {
          this.m_spawnDelay = spawnDelay;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new InfinilichSpinBeams1.BeamBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
