// Decompiled with JetBrains decompiler
// Type: MummyCurse1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class MummyCurse1 : Script
    {
      private const int AirTime = 180;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MummyCurse1__Topc__Iterator0()
        {
          _this = this
        };
      }

      public class SkullBullet : Bullet
      {
        private Script m_parentScript;

        public SkullBullet(Script parentScript)
          : base("skull")
        {
          this.m_parentScript = parentScript;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MummyCurse1.SkullBullet__Topc__Iterator0()
          {
            _this = this
          };
        }

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          if (this.m_parentScript == null)
            return;
          this.m_parentScript.ForceEnd();
        }
      }
    }

}
