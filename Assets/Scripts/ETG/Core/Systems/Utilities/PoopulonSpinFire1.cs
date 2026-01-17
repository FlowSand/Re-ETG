// Decompiled with JetBrains decompiler
// Type: PoopulonSpinFire1
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
    public class PoopulonSpinFire1 : Script
    {
      private const int NumBullets = 100;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new PoopulonSpinFire1.<Top>c__Iterator0()
        {
          $this = this
        };
      }

      public class RotatingBullet : Bullet
      {
        private Vector2 m_origin;

        public RotatingBullet(Vector2 origin)
          : base()
        {
          this.m_origin = origin;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new PoopulonSpinFire1.RotatingBullet.<Top>c__Iterator0()
          {
            $this = this
          };
        }
      }
    }

}
