using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/Rocket1")]
public class DraGunRocket1 : Script
  {
    private const int NumBullets = 42;

    protected override IEnumerator Top()
    {
      if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
      {
        if ((double) Random.value < 0.5)
        {
          this.Fire(new Brave.BulletScript.Direction(-60f), new Brave.BulletScript.Speed(40f), (Bullet) new DraGunRocket1.Rocket());
          this.Fire(new Brave.BulletScript.Direction(-120f), new Brave.BulletScript.Speed(20f), (Bullet) new DraGunRocket1.Rocket());
        }
        else
        {
          this.Fire(new Brave.BulletScript.Direction(-60f), new Brave.BulletScript.Speed(20f), (Bullet) new DraGunRocket1.Rocket());
          this.Fire(new Brave.BulletScript.Direction(-120f), new Brave.BulletScript.Speed(40f), (Bullet) new DraGunRocket1.Rocket());
        }
      }
      else
        this.Fire(new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(40f), (Bullet) new DraGunRocket1.Rocket());
      return (IEnumerator) null;
    }

    public class Rocket : Bullet
    {
      public Rocket()
        : base("rocket")
      {
      }

      protected override IEnumerator Top() => (IEnumerator) null;

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        for (int i = 0; i < 42; ++i)
        {
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(-10f, 200f, 42, i)), new Brave.BulletScript.Speed(12f), new Bullet("default_novfx"));
          if (i < 41)
            this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(-10f, 200f, 42, i, true)), new Brave.BulletScript.Speed(8f), (Bullet) new SpeedChangingBullet("default_novfx", 12f, 60));
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(-10f, 200f, 42, i)), new Brave.BulletScript.Speed(4f), (Bullet) new SpeedChangingBullet("default_novfx", 12f, 60));
        }
        for (int index = 0; index < 12; ++index)
          this.Fire(new Brave.BulletScript.Direction(Random.Range(20f, 160f)), new Brave.BulletScript.Speed(Random.Range(10f, 16f)), (Bullet) new DraGunRocket1.ShrapnelBullet());
      }
    }

    public class ShrapnelBullet : Bullet
    {
      private const float WiggleMagnitude = 0.75f;
      private const float WigglePeriod = 3f;

      public ShrapnelBullet()
        : base("shrapnel")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunRocket1.ShrapnelBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

