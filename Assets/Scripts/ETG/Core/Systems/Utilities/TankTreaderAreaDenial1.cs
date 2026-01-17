// Decompiled with JetBrains decompiler
// Type: TankTreaderAreaDenial1
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
    [InspectorDropdownName("Bosses/TankTreader/AreaDenial1")]
    public class TankTreaderAreaDenial1 : Script
    {
      public static float HugeBulletStartSpeed = 6f;
      public static int HugeBulletDecelerationTime = 180;
      public static float HugeBulletHangTime = 300f;
      public static float SpinningBulletSpinSpeed = 180f;

      protected override IEnumerator Top()
      {
        this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(TankTreaderAreaDenial1.HugeBulletStartSpeed), (Bullet) new TankTreaderAreaDenial1.HugeBullet());
        return (IEnumerator) null;
      }

      public class HugeBullet : Bullet
      {
        private const int SemiCircleNumBullets = 4;
        private const int SemiCirclePhases = 3;
        private bool m_fireSemicircles;

        public HugeBullet()
          : base("hugeBullet")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new TankTreaderAreaDenial1.HugeBullet__Topc__Iterator0()
          {
            _this = this
          };
        }

        [DebuggerHidden]
        private IEnumerator FireSemicircles()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new TankTreaderAreaDenial1.HugeBullet__FireSemicirclesc__Iterator1()
          {
            _this = this
          };
        }
      }
    }

}
