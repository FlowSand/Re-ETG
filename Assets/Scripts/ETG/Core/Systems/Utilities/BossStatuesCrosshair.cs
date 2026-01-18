using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossStatues/Crosshair")]
public class BossStatuesCrosshair : Script
  {
    public static float QuarterPi = 0.785f;
    public static int SkipSetupBulletNum = 6;
    public static int ExtraSetupBulletNum;
    public static int SetupTime = 90;
    public static int BulletCount = 25;
    public static float Radius = 11f;
    public static int QuaterRotTime = 120;
    public static int SpinTime = 600;
    public static int PulseInitialDelay = 120;
    public static int PulseDelay = 120;
    public static int PulseCount = 4;
    public static int PulseTravelTime = 100;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossStatuesCrosshair__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void FireSpinningLine(float dir)
    {
      float num1 = (float) BossStatuesCrosshair.SkipSetupBulletNum * ((float) ((double) BossStatuesCrosshair.Radius * 2.0 * (60.0 / (double) BossStatuesCrosshair.SetupTime)) / (float) BossStatuesCrosshair.BulletCount);
      float num2 = (float) ((double) BossStatuesCrosshair.Radius * 2.0 * (60.0 / (double) BossStatuesCrosshair.SetupTime)) / (float) BossStatuesCrosshair.BulletCount;
      for (int index = 0; index < BossStatuesCrosshair.BulletCount + BossStatuesCrosshair.ExtraSetupBulletNum - BossStatuesCrosshair.SkipSetupBulletNum; ++index)
        this.Fire(new Brave.BulletScript.Direction(dir), new Brave.BulletScript.Speed(num1 + num2 * (float) index), (Bullet) new BossStatuesCrosshair.LineBullet(index + BossStatuesCrosshair.SkipSetupBulletNum));
    }

    private void FireCircleSegment(float dir)
    {
      for (int spawnTime = 0; spawnTime < BossStatuesCrosshair.BulletCount; ++spawnTime)
        this.Fire(new Brave.BulletScript.Direction(dir), new Brave.BulletScript.Speed((float) ((double) BossStatuesCrosshair.Radius * 2.0 * (60.0 / (double) BossStatuesCrosshair.SetupTime))), (Bullet) new BossStatuesCrosshair.CircleBullet(spawnTime));
    }

    private void FirePulse()
    {
      float num = 4.5f;
      for (int index = 0; index < 80 /*0x50*/; ++index)
        this.Fire(new Brave.BulletScript.Direction(((float) index + 0.5f) * num), new Brave.BulletScript.Speed(BossStatuesCrosshair.Radius / ((float) BossStatuesCrosshair.PulseTravelTime / 60f)), new Bullet("defaultPulse", forceBlackBullet: true));
    }

    public class LineBullet : Bullet
    {
      public int spawnTime;

      public LineBullet(int spawnTime)
        : base("defaultLine", forceBlackBullet: true)
      {
        this.spawnTime = spawnTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossStatuesCrosshair.LineBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class CircleBullet : Bullet
    {
      public int spawnTime;

      public CircleBullet(int spawnTime)
        : base("defaultCircle")
      {
        this.spawnTime = spawnTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossStatuesCrosshair.CircleBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class CircleExtraBullet : Bullet
    {
      public int spawnTime;

      public CircleExtraBullet(int spawnTime)
        : base("defaultCircleExtra")
      {
        this.spawnTime = spawnTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossStatuesCrosshair.CircleExtraBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

