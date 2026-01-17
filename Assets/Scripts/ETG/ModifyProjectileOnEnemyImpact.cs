// Decompiled with JetBrains decompiler
// Type: ModifyProjectileOnEnemyImpact
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable
public class ModifyProjectileOnEnemyImpact : PassiveItem
{
  public bool ApplyRandomBounceOffEnemy = true;
  [ShowInInspectorIf("ApplyRandomBounceOffEnemy", false)]
  public float ChanceToSeekEnemyOnBounce = 0.5f;
  public bool NormalizeAcrossFireRate;
  [ShowInInspectorIf("NormalizeAcrossFireRate", false)]
  public float ActivationsPerSecond = 1f;
  [ShowInInspectorIf("NormalizeAcrossFireRate", false)]
  public float MinActivationChance = 0.05f;
  private PlayerController m_player;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    this.m_player = player;
    base.Pickup(player);
    player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
  }

  private void PostProcessProjectile(Projectile p, float effectChanceScalar)
  {
    p.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleProjectileHitEnemy);
  }

  private void HandleProjectileHitEnemy(Projectile obj, SpeculativeRigidbody enemy, bool killed)
  {
    if (!this.ApplyRandomBounceOffEnemy)
      return;
    PierceProjModifier orAddComponent = obj.gameObject.GetOrAddComponent<PierceProjModifier>();
    orAddComponent.penetratesBreakables = true;
    ++orAddComponent.penetration;
    HomingModifier component = obj.gameObject.GetComponent<HomingModifier>();
    if ((bool) (UnityEngine.Object) component)
      component.AngularVelocity *= 0.75f;
    Vector2 dirVec = UnityEngine.Random.insideUnitCircle;
    float num1 = this.ChanceToSeekEnemyOnBounce;
    Gun possibleSourceGun = obj.PossibleSourceGun;
    if (this.NormalizeAcrossFireRate && (bool) (UnityEngine.Object) possibleSourceGun)
    {
      float num2 = 1f / possibleSourceGun.DefaultModule.cooldownTime;
      if ((UnityEngine.Object) possibleSourceGun.Volley != (UnityEngine.Object) null && possibleSourceGun.Volley.UsesShotgunStyleVelocityRandomizer)
        num2 *= (float) Mathf.Max(1, possibleSourceGun.Volley.projectiles.Count);
      num1 = Mathf.Max(this.MinActivationChance, Mathf.Clamp01(this.ActivationsPerSecond / num2));
    }
    if ((double) UnityEngine.Random.value < (double) num1 && (bool) (UnityEngine.Object) enemy.aiActor)
    {
      Func<AIActor, bool> isValid = (Func<AIActor, bool>) (a => (bool) (UnityEngine.Object) a && a.HasBeenEngaged && (bool) (UnityEngine.Object) a.healthHaver && a.healthHaver.IsVulnerable);
      AIActor closestToPosition = BraveUtility.GetClosestToPosition<AIActor>(enemy.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All), enemy.UnitCenter, isValid, enemy.aiActor);
      if ((bool) (UnityEngine.Object) closestToPosition)
        dirVec = closestToPosition.CenterPosition - obj.transform.position.XY();
    }
    obj.SendInDirection(dirVec, false);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    this.m_player = (PlayerController) null;
    debrisObject.GetComponent<ModifyProjectileOnEnemyImpact>().m_pickedUpThisRun = true;
    player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (!(bool) (UnityEngine.Object) this.m_player)
      return;
    this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
  }
}
