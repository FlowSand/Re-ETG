// Decompiled with JetBrains decompiler
// Type: BloodbulonDeathBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BloodbulonDeathBurst1 : Script
    {
      private const int NumBullets = 20;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BloodbulonDeathBurst1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void QuadShot(float direction, float offset, float speed)
      {
        for (int index = 0; index < 4; ++index)
          this.Fire(new Offset(offset, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(speed - (float) index * 1.5f), (Bullet) new SpeedChangingBullet(speed, 120));
      }
    }

}
