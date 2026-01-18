using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/BigNoseShot")]
public class DraGunBigNoseShot : Script
  {
    protected override IEnumerator Top()
    {
      this.Fire(new Brave.BulletScript.Direction(-90f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
      this.Fire(new Brave.BulletScript.Direction(-110f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
      this.Fire(new Brave.BulletScript.Direction(-130f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
      this.Fire(new Brave.BulletScript.Direction(-70f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
      this.Fire(new Brave.BulletScript.Direction(-50f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
      if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
      {
        this.Fire(new Brave.BulletScript.Direction(-60f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
        this.Fire(new Brave.BulletScript.Direction(-80f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
        this.Fire(new Brave.BulletScript.Direction(-100f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
        this.Fire(new Brave.BulletScript.Direction(-120f), new Brave.BulletScript.Speed(6f), new Bullet("homing"));
      }
      return (IEnumerator) null;
    }
  }

