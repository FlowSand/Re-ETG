// Decompiled with JetBrains decompiler
// Type: TankTreaderScatterShot1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/TankTreader/ScatterShot1")]
    public class TankTreaderScatterShot1 : Script
    {
      private const int AirTime = 30;
      private const int NumDeathBullets = 16 /*0x10*/;

      protected override IEnumerator Top()
      {
        this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(12f), (Bullet) new TankTreaderScatterShot1.ScatterBullet());
        return (IEnumerator) null;
      }

      private class ScatterBullet : Bullet
      {
        public ScatterBullet()
          : base("scatterBullet")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new TankTreaderScatterShot1.ScatterBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }

      private class LittleScatterBullet : Bullet
      {
        public LittleScatterBullet()
          : base()
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new TankTreaderScatterShot1.LittleScatterBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
