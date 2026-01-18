using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class BloodbulonDeathBurst1 : Script
  {
    private const int NumBullets = 20;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BloodbulonDeathBurst1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void QuadShot(float direction, float offset, float speed)
    {
      for (int index = 0; index < 4; ++index)
        this.Fire(new Offset(offset, rotation: direction, transform: string.Empty), new Brave.BulletScript.Direction(direction), new Brave.BulletScript.Speed(speed - (float) index * 1.5f), (Bullet) new SpeedChangingBullet(speed, 120));
    }
  }

