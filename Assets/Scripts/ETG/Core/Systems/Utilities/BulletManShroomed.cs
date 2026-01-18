using Brave.BulletScript;
using System.Collections;

#nullable disable

public class BulletManShroomed : Script
  {
    protected override IEnumerator Top()
    {
      this.Fire(new Brave.BulletScript.Direction(-20f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
      this.Fire(new Brave.BulletScript.Direction(20f, Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), (Bullet) null);
      return (IEnumerator) null;
    }
  }

