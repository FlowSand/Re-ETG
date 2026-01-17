// Decompiled with JetBrains decompiler
// Type: BilliardsStickItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class BilliardsStickItem : PassiveItem
{
  public float KnockbackForce = 800f;
  public float AngleTolerance = 30f;
  public Color TintColor = Color.white;
  public int TintPriority = 2;

  public override void Pickup(PlayerController player)
  {
    base.Pickup(player);
    player.PostProcessProjectile += new Action<Projectile, float>(this.HandlePostProcessProjectile);
    player.PostProcessBeam += new Action<BeamController>(this.HandlePostProcessBeam);
    player.PostProcessBeamTick += new Action<BeamController, SpeculativeRigidbody, float>(this.HandlePostProcessBeamTick);
  }

  private void HandlePostProcessProjectile(Projectile targetProjectile, float effectChanceScalar)
  {
    targetProjectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
    targetProjectile.AdjustPlayerProjectileTint(this.TintColor, this.TintPriority);
  }

  private void HandleHitEnemy(
    Projectile sourceProjectile,
    SpeculativeRigidbody hitRigidbody,
    bool fatal)
  {
    if (!fatal || !(bool) (UnityEngine.Object) hitRigidbody)
      return;
    if ((bool) (UnityEngine.Object) sourceProjectile)
      sourceProjectile.baseData.force = 0.0f;
    AIActor aiActor = hitRigidbody.aiActor;
    KnockbackDoer knockbackDoer = hitRigidbody.knockbackDoer;
    if ((bool) (UnityEngine.Object) aiActor)
    {
      if ((bool) (UnityEngine.Object) aiActor.GetComponent<ExplodeOnDeath>())
        UnityEngine.Object.Destroy((UnityEngine.Object) aiActor.GetComponent<ExplodeOnDeath>());
      hitRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
      hitRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
    }
    if (!(bool) (UnityEngine.Object) knockbackDoer || !(bool) (UnityEngine.Object) sourceProjectile)
      return;
    float nearestDistance = -1f;
    AIActor enemyInDirection = aiActor.ParentRoom.GetNearestEnemyInDirection(aiActor.CenterPosition, sourceProjectile.Direction, this.AngleTolerance, out nearestDistance);
    Vector2 direction = sourceProjectile.Direction;
    if ((bool) (UnityEngine.Object) enemyInDirection)
      direction = enemyInDirection.CenterPosition - aiActor.CenterPosition;
    knockbackDoer.ApplyKnockback(direction, this.KnockbackForce, true);
  }

  private void HandleHitEnemyHitEnemy(
    SpeculativeRigidbody myRigidbody,
    PixelCollider myPixelCollider,
    SpeculativeRigidbody otherRigidbody,
    PixelCollider otherPixelCollider)
  {
    if (!(bool) (UnityEngine.Object) otherRigidbody || !(bool) (UnityEngine.Object) otherRigidbody.aiActor || !(bool) (UnityEngine.Object) myRigidbody || !(bool) (UnityEngine.Object) myRigidbody.healthHaver)
      return;
    AIActor aiActor = otherRigidbody.aiActor;
    myRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
    if (!aiActor.IsNormalEnemy || !(bool) (UnityEngine.Object) aiActor.healthHaver)
      return;
    aiActor.healthHaver.ApplyDamage(myRigidbody.healthHaver.GetMaxHealth() * 2f, myRigidbody.Velocity, "Pinball");
  }

  private void HandlePostProcessBeam(BeamController targetBeam)
  {
  }

  private void HandlePostProcessBeamTick(
    BeamController arg1,
    SpeculativeRigidbody arg2,
    float arg3)
  {
  }

  public override DebrisObject Drop(PlayerController player)
  {
    if ((bool) (UnityEngine.Object) player)
    {
      player.PostProcessProjectile -= new Action<Projectile, float>(this.HandlePostProcessProjectile);
      player.PostProcessBeam -= new Action<BeamController>(this.HandlePostProcessBeam);
      player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.HandlePostProcessBeamTick);
    }
    return base.Drop(player);
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.Owner)
    {
      this.Owner.PostProcessProjectile -= new Action<Projectile, float>(this.HandlePostProcessProjectile);
      this.Owner.PostProcessBeam -= new Action<BeamController>(this.HandlePostProcessBeam);
      this.Owner.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.HandlePostProcessBeamTick);
    }
    base.OnDestroy();
  }
}
