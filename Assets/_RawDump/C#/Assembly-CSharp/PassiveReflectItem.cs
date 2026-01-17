// Decompiled with JetBrains decompiler
// Type: PassiveReflectItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.ObjectModel;
using UnityEngine;

#nullable disable
public class PassiveReflectItem : PassiveItem
{
  public PassiveReflectItem.Condition condition;
  public float minReflectedBulletSpeed = 10f;
  public bool retargetReflectedBullet = true;
  public int AmmoGainedOnReflection;
  private PlayerController m_player;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
    this.m_player = player;
    player.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    debrisObject.GetComponent<PassiveReflectItem>().m_pickedUpThisRun = true;
    player.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
    return debrisObject;
  }

  private void OnPreCollision(
    SpeculativeRigidbody myRigidbody,
    PixelCollider myCollider,
    SpeculativeRigidbody otherRigidbody,
    PixelCollider otherCollider)
  {
    if (this.condition == PassiveReflectItem.Condition.WhileDodgeRolling && !this.m_player.spriteAnimator.QueryInvulnerabilityFrame())
      return;
    Projectile component = otherRigidbody.GetComponent<Projectile>();
    if (!((Object) component != (Object) null))
      return;
    PassiveReflectItem.ReflectBullet(component, this.retargetReflectedBullet, (GameActor) this.m_owner, this.minReflectedBulletSpeed);
    if (this.AmmoGainedOnReflection > 0)
    {
      Gun currentGun = this.m_owner.CurrentGun;
      if ((bool) (Object) currentGun && currentGun.CanGainAmmo)
        currentGun.GainAmmo(this.AmmoGainedOnReflection);
    }
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", component.gameObject);
    otherRigidbody.transform.position += component.Direction.ToVector3ZUp() * 0.5f;
    otherRigidbody.Reinitialize();
    PhysicsEngine.SkipCollision = true;
  }

  public static int ReflectBulletsInRange(
    Vector2 centerPoint,
    float radius,
    bool retargetReflectedBulet,
    GameActor newOwner,
    float minReflectedBulletSpeed,
    float scaleModifier = 1f,
    float damageModifier = 1f,
    bool applyPostprocess = false)
  {
    int num1 = 0;
    float num2 = radius * radius;
    ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
    for (int index = 0; index < allProjectiles.Count; ++index)
    {
      Projectile p = allProjectiles[index];
      if ((!(bool) (Object) p.Owner || !(p.Owner is PlayerController)) && (bool) (Object) p.specRigidbody && (bool) (Object) p && (bool) (Object) p.sprite && (double) (p.sprite.WorldCenter - centerPoint).sqrMagnitude <= (double) num2)
      {
        PassiveReflectItem.ReflectBullet(p, retargetReflectedBulet, newOwner, minReflectedBulletSpeed, scaleModifier, damageModifier);
        ++num1;
        if (applyPostprocess && newOwner is PlayerController)
        {
          SpawnManager.PoolManager.Remove(p.transform);
          (newOwner as PlayerController).CustomPostProcessProjectile(p, 1f);
        }
      }
    }
    return num1;
  }

  public static void ReflectBullet(
    Projectile p,
    bool retargetReflectedBullet,
    GameActor newOwner,
    float minReflectedBulletSpeed,
    float scaleModifier = 1f,
    float damageModifier = 1f,
    float spread = 0.0f)
  {
    p.RemoveBulletScriptControl();
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metalskin_deflect_01", GameManager.Instance.gameObject);
    if (retargetReflectedBullet && (bool) (Object) p.Owner && (bool) (Object) p.Owner.specRigidbody)
    {
      p.Direction = (p.Owner.specRigidbody.GetUnitCenter(ColliderType.HitBox) - p.specRigidbody.UnitCenter).normalized;
    }
    else
    {
      Vector2 vector2 = p.LastVelocity;
      if (p.IsBulletScript && (bool) (Object) p.braveBulletScript && p.braveBulletScript.bullet != null)
        vector2 = p.braveBulletScript.bullet.Velocity;
      p.Direction = -vector2.normalized;
      if (p.Direction == Vector2.zero)
        p.Direction = Random.insideUnitCircle.normalized;
    }
    if ((double) spread != 0.0)
      p.Direction = p.Direction.Rotate(Random.Range(-spread, spread));
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
    {
      SpawnManager.PoolManager.Remove(p.transform);
      p.RuntimeUpdateScale(scaleModifier);
    }
    if ((double) p.Speed < (double) minReflectedBulletSpeed)
      p.Speed = minReflectedBulletSpeed;
    if ((double) p.baseData.damage < (double) ProjectileData.FixedFallbackDamageToEnemies)
      p.baseData.damage = ProjectileData.FixedFallbackDamageToEnemies;
    p.baseData.damage *= damageModifier;
    if ((double) p.baseData.damage < 10.0)
      p.baseData.damage = 15f;
    p.UpdateCollisionMask();
    p.ResetDistance();
    p.Reflected();
  }

  protected override void OnDestroy() => base.OnDestroy();

  public enum Condition
  {
    WhileDodgeRolling,
  }
}
