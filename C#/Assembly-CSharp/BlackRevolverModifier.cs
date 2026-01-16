// Decompiled with JetBrains decompiler
// Type: BlackRevolverModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BlackRevolverModifier : MonoBehaviour
{
  public float WakeRadius = 3f;
  private Projectile m_projectile;
  private Gun m_gun;

  public void Start()
  {
    this.m_projectile = this.GetComponent<Projectile>();
    this.m_gun = this.m_projectile.PossibleSourceGun;
  }

  public void Update()
  {
    if (!(bool) (Object) this.m_gun || !(bool) (Object) this.m_gun.CurrentOwner || !(bool) (Object) this.m_projectile)
      return;
    Vector2 unitCenter = this.m_projectile.specRigidbody.UnitCenter;
    Vector2 direction = this.m_projectile.Direction;
    float num = this.WakeRadius * this.WakeRadius;
    for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
    {
      Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
      if ((bool) (Object) allProjectile && (Object) allProjectile.Owner != (Object) this.m_gun.CurrentOwner && (double) ((!(bool) (Object) allProjectile.specRigidbody ? allProjectile.transform.position.XY() : allProjectile.specRigidbody.UnitCenter) - unitCenter).sqrMagnitude < (double) num)
      {
        Vector2 newDirection = direction;
        this.RedirectBullet(allProjectile, this.m_gun.CurrentOwner, newDirection, 10f);
      }
    }
  }

  public void RedirectBullet(
    Projectile p,
    GameActor newOwner,
    Vector2 newDirection,
    float minReflectedBulletSpeed,
    float angleVariance = 0.0f,
    float scaleModifier = 1f,
    float damageModifier = 1f)
  {
    p.RemoveBulletScriptControl();
    p.Direction = newDirection.normalized;
    if (p.Direction == Vector2.zero)
      p.Direction = Random.insideUnitCircle.normalized;
    if ((double) angleVariance != 0.0)
      p.Direction = p.Direction.Rotate(Random.Range(-angleVariance, angleVariance));
    if ((bool) (Object) p.Owner && (bool) (Object) p.Owner.specRigidbody)
      p.specRigidbody.DeregisterSpecificCollisionException(p.Owner.specRigidbody);
    p.Owner = newOwner;
    p.SetNewShooter(newOwner.specRigidbody);
    p.allowSelfShooting = false;
    if (newOwner is AIActor)
    {
      p.collidesWithPlayer = true;
      p.collidesWithEnemies = false;
    }
    else
    {
      p.collidesWithPlayer = false;
      p.collidesWithEnemies = true;
    }
    if ((double) scaleModifier != 1.0)
      p.RuntimeUpdateScale(scaleModifier);
    if ((double) p.Speed < (double) minReflectedBulletSpeed)
      p.Speed = minReflectedBulletSpeed;
    if ((double) p.baseData.damage < (double) ProjectileData.FixedFallbackDamageToEnemies)
      p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
    p.baseData.damage *= damageModifier;
    p.UpdateCollisionMask();
    p.ResetDistance();
    p.Reflected();
  }
}
