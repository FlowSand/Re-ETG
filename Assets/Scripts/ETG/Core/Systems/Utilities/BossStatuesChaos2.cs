using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Bosses/BossStatues/Chaos2")]
public class BossStatuesChaos2 : Script
  {
    protected override IEnumerator Top()
    {
      this.Fire(new Offset("top 0"), new Brave.BulletScript.Direction(135f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      this.Fire(new Offset("top 2"), new Brave.BulletScript.Direction(45f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      this.Fire(new Offset("right 0"), new Brave.BulletScript.Direction(45f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      this.Fire(new Offset("right 2"), new Brave.BulletScript.Direction(-45f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      this.Fire(new Offset("bottom 0"), new Brave.BulletScript.Direction(-45f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      this.Fire(new Offset("bottom 2"), new Brave.BulletScript.Direction(-135f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      this.Fire(new Offset("left 0"), new Brave.BulletScript.Direction(-135f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      this.Fire(new Offset("left 2"), new Brave.BulletScript.Direction(135f), new Brave.BulletScript.Speed(7.5f), new Bullet("egg"));
      BossStatuesChaos1.AntiCornerShot((Script) this);
      return (IEnumerator) null;
    }
  }

