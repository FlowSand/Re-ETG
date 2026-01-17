// Decompiled with JetBrains decompiler
// Type: DraGunWingFlap1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("Bosses/DraGun/WingFlap1")]
    public class DraGunWingFlap1 : Script
    {
      private const int NumBullets = 30;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunWingFlap1__Topc__Iterator0()
        {
          _this = this
        };
      }

      public class WindProjectile : Bullet
      {
        private float m_sign;

        public WindProjectile(float sign)
          : base()
        {
          this.m_sign = sign;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new DraGunWingFlap1.WindProjectile__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
