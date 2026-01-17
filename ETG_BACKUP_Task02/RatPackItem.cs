// Decompiled with JetBrains decompiler
// Type: RatPackItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class RatPackItem : PlayerItem
{
  public int MaxContainedBullets = 30;
  public float ScoopRadius = 1f;
  public DirectedBurstInterface Burst;
  private int m_containedBullets;
  private Action<Projectile> EatBulletAction;

  public int ContainedBullets => this.m_containedBullets;

  public override void Pickup(PlayerController player)
  {
    base.Pickup(player);
    player.OnIsRolling += new Action<PlayerController>(this.HandleRollingFrame);
  }

  private void HandleRollingFrame(PlayerController src)
  {
    if (this.EatBulletAction == null)
      this.EatBulletAction += new Action<Projectile>(this.EatBullet);
    if (src.CurrentRollState != PlayerController.DodgeRollState.InAir)
      return;
    RatPackItem.AffectNearbyProjectiles(src.CenterPosition, this.ScoopRadius, this.EatBulletAction);
  }

  private static void AffectNearbyProjectiles(
    Vector2 center,
    float radius,
    Action<Projectile> DoEffect)
  {
    for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
    {
      Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
      if ((bool) (UnityEngine.Object) allProjectile && allProjectile.Owner is AIActor && (double) (allProjectile.transform.position.XY() - center).sqrMagnitude < (double) radius)
        DoEffect(allProjectile);
    }
  }

  private void EatBullet(Projectile p)
  {
    if (!(p.Owner is AIActor))
      return;
    p.DieInAir();
    ++this.m_containedBullets;
    this.m_containedBullets = Mathf.Clamp(this.m_containedBullets, 0, this.MaxContainedBullets);
  }

  private void HandleDodgedProjectile(Projectile p) => this.EatBullet(p);

  public override bool CanBeUsed(PlayerController user)
  {
    return this.m_containedBullets > 0 && base.CanBeUsed(user);
  }

  protected override void DoEffect(PlayerController user)
  {
    base.DoEffect(user);
    this.Burst.NumberWaves = 1;
    this.Burst.MinToSpawnPerWave = this.ContainedBullets;
    this.Burst.MaxToSpawnPerWave = this.ContainedBullets;
    this.Burst.UseShotgunStyleVelocityModifier = true;
    if ((bool) (UnityEngine.Object) user && (bool) (UnityEngine.Object) user.CurrentGun)
      this.Burst.DoBurst(user, user.CurrentGun.CurrentAngle);
    this.m_containedBullets = 0;
  }

  protected override void OnPreDrop(PlayerController user)
  {
    user.OnIsRolling -= new Action<PlayerController>(this.HandleRollingFrame);
    user.OnDodgedProjectile -= new Action<Projectile>(this.HandleDodgedProjectile);
    base.OnPreDrop(user);
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.LastOwner)
    {
      this.LastOwner.OnIsRolling -= new Action<PlayerController>(this.HandleRollingFrame);
      this.LastOwner.OnDodgedProjectile -= new Action<Projectile>(this.HandleDodgedProjectile);
    }
    base.OnDestroy();
  }
}
