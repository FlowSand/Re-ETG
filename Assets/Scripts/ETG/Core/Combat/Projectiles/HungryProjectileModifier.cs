// Decompiled with JetBrains decompiler
// Type: HungryProjectileModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class HungryProjectileModifier : MonoBehaviour
    {
      public float DamagePercentGainPerSnack = 0.25f;
      public float MaxMultiplier = 3f;
      public float HungryRadius = 3f;
      public int MaximumBulletsEaten = 10;
      private Projectile m_projectile;
      private int m_numberOfBulletsEaten;
      private bool m_sated;

      private void Awake()
      {
        this.m_projectile = this.GetComponent<Projectile>();
        this.m_projectile.AdjustPlayerProjectileTint(new Color(0.45f, 0.3f, 0.87f), 2);
        this.m_projectile.collidesWithProjectiles = true;
        this.m_projectile.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision);
      }

      private void Update()
      {
        if (this.m_sated)
          return;
        Vector2 vector2 = this.m_projectile.transform.position.XY();
        for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
        {
          Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
          if ((bool) (Object) allProjectile && allProjectile.Owner is AIActor && (double) (allProjectile.transform.position.XY() - vector2).sqrMagnitude < (double) this.HungryRadius)
            this.EatBullet(allProjectile);
        }
      }

      private void HandlePreCollision(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if (this.m_sated || !(bool) (Object) otherRigidbody || !(bool) (Object) otherRigidbody.projectile)
          return;
        if (otherRigidbody.projectile.Owner is AIActor)
          this.EatBullet(otherRigidbody.projectile);
        PhysicsEngine.SkipCollision = true;
      }

      private void EatBullet(Projectile other)
      {
        if (this.m_sated)
          return;
        other.DieInAir();
        float num = Mathf.Min(this.MaxMultiplier, (float) (1.0 + (double) this.m_numberOfBulletsEaten * (double) this.DamagePercentGainPerSnack));
        ++this.m_numberOfBulletsEaten;
        float multiplier = Mathf.Max(1f, Mathf.Min(this.MaxMultiplier, (float) (1.0 + (double) this.m_numberOfBulletsEaten * (double) this.DamagePercentGainPerSnack)) / num);
        if ((double) multiplier > 1.0)
        {
          this.m_projectile.RuntimeUpdateScale(multiplier);
          this.m_projectile.baseData.damage *= multiplier;
        }
        if (this.m_numberOfBulletsEaten < this.MaximumBulletsEaten)
          return;
        this.m_sated = true;
        this.m_projectile.AdjustPlayerProjectileTint(this.m_projectile.DefaultTintColor, 3);
      }
    }

}
