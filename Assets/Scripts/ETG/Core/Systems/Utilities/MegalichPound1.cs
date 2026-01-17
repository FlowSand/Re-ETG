// Decompiled with JetBrains decompiler
// Type: MegalichPound1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public abstract class MegalichPound1 : Script
    {
      private const int NumBurstBullets = 12;
      private const int NumOtherBullets = 30;
      private const int NumWallBullets = 60;

      protected abstract float FireDirection { get; }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichPound1__Topc__Iterator0()
        {
          _this = this
        };
      }

      public class DyingBullet : Bullet
      {
        private bool m_disappear;

        public DyingBullet(string name, bool disappear)
          : base(name)
        {
          this.m_disappear = disappear;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MegalichPound1.DyingBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
