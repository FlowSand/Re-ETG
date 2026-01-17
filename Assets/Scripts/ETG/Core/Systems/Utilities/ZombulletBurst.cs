// Decompiled with JetBrains decompiler
// Type: ZombulletBurst
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class ZombulletBurst : Script
    {
      private const int NumBullets = 18;

      protected override IEnumerator Top()
      {
        float num1 = this.RandomAngle();
        float num2 = 20f;
        for (int index = 0; index < 18; ++index)
          this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(7f), (Bullet) new ZombulletBurst.OscillatingBullet());
        return (IEnumerator) null;
      }

      private class OscillatingBullet : Bullet
      {
        public OscillatingBullet()
          : base()
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new ZombulletBurst.OscillatingBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
