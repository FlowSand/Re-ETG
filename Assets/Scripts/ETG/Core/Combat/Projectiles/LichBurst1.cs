using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Bosses/Lich/Burst1")]
public class LichBurst1 : Script
  {
    private const int NumBounceBullets = 24;
    private const int NumNormalBullets = 24;

    protected override IEnumerator Top()
    {
      float startAngle = this.RandomAngle();
      for (int i = 0; i < 24; ++i)
        this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle, 24, i)), new Brave.BulletScript.Speed(9f), (Bullet) new LichBurst1.BurstBullet());
      for (int i = 0; i < 24; ++i)
        this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle, 24, i, offset: true)), new Brave.BulletScript.Speed(6f), (Bullet) null);
      return (IEnumerator) null;
    }

    public class BurstBullet : Bullet
    {
      public BurstBullet()
        : base("burst")
      {
      }

      protected override IEnumerator Top()
      {
        this.Projectile.GetComponent<BounceProjModifier>().OnBounce += new System.Action(this.OnBounce);
        this.ChangeSpeed(new Brave.BulletScript.Speed(16f), 180);
        return (IEnumerator) null;
      }

      public override void OnBulletDestruction(
        Bullet.DestroyType destroyType,
        SpeculativeRigidbody hitRigidbody,
        bool preventSpawningProjectiles)
      {
        if (!(bool) (UnityEngine.Object) this.Projectile)
          return;
        this.Projectile.GetComponent<BounceProjModifier>().OnBounce -= new System.Action(this.OnBounce);
      }

      private void OnBounce() => this.Direction = this.Projectile.Direction.ToAngle();
    }
  }

