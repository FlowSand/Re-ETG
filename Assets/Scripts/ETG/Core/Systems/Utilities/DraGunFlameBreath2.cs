using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/FlameBreath2")]
public class DraGunFlameBreath2 : Script
  {
    private const int NumBullets = 120;
    private const int NumWaveBullets = 12;
    private const float Spread = 30f;
    private const int PocketResetTime = 30;
    private const float PocketWidth = 5f;
    protected static float StopYHeight;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunFlameBreath2__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class FlameBullet : Bullet
    {
      public FlameBullet()
        : base("Breath")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunFlameBreath2.FlameBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

