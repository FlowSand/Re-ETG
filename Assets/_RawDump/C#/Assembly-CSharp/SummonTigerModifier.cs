// Decompiled with JetBrains decompiler
// Type: SummonTigerModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable
public class SummonTigerModifier : BraveBehaviour
{
  public Projectile TigerProjectilePrefab;
  private bool m_hasSummonedTiger;

  private void Start()
  {
    this.projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
    this.projectile.OnDestruction += new Action<Projectile>(this.HandleDestruction);
  }

  private void HandleDestruction(Projectile source)
  {
    if (this.m_hasSummonedTiger)
      return;
    this.SummonTiger((SpeculativeRigidbody) null);
  }

  private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
  {
    if (this.m_hasSummonedTiger)
      return;
    this.SummonTiger(arg2);
  }

  private void SummonTiger(SpeculativeRigidbody optionalTarget)
  {
    this.m_hasSummonedTiger = true;
    RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
    Vector2? idealPosition = new Vector2?();
    if ((UnityEngine.Object) optionalTarget != (UnityEngine.Object) null)
      idealPosition = new Vector2?(optionalTarget.UnitCenter);
    IntVector2 intVector2 = new IntVector2(4, 2);
    if ((bool) (UnityEngine.Object) this.sprite)
      intVector2 = Vector2.Scale(new Vector2(4f, 2f), this.sprite.scale.XY()).ToIntVector2(VectorConversions.Ceil);
    IntVector2? nullable = roomFromPosition.GetOffscreenCell(new IntVector2?(intVector2), new CellTypes?(CellTypes.FLOOR), idealPosition: idealPosition);
    if (!nullable.HasValue)
      nullable = roomFromPosition.GetRandomAvailableCell(new IntVector2?(intVector2), new CellTypes?(CellTypes.FLOOR));
    if (!nullable.HasValue)
      return;
    if ((UnityEngine.Object) optionalTarget != (UnityEngine.Object) null)
      this.ShootSingleProjectile(nullable.Value.ToVector2(), BraveMathCollege.Atan2Degrees(optionalTarget.UnitCenter - nullable.Value.ToVector2()));
    else
      this.ShootSingleProjectile(nullable.Value.ToVector2(), BraveMathCollege.Atan2Degrees(roomFromPosition.GetCenterCell().ToVector2() - nullable.Value.ToVector2()));
  }

  private void ShootSingleProjectile(Vector2 spawnPosition, float angle)
  {
    Projectile component = SpawnManager.SpawnProjectile(this.TigerProjectilePrefab.gameObject, spawnPosition.ToVector3ZUp(spawnPosition.y), Quaternion.Euler(0.0f, 0.0f, angle)).GetComponent<Projectile>();
    component.Owner = this.projectile.Owner;
    component.Shooter = component.Owner.specRigidbody;
    if (!(component.Owner is PlayerController))
      return;
    PlayerStats stats = (component.Owner as PlayerController).stats;
    component.baseData.damage *= stats.GetStatValue(PlayerStats.StatType.Damage);
    component.baseData.speed *= stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
    component.baseData.force *= stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
    (component.Owner as PlayerController).DoPostProcessProjectile(component);
  }
}
