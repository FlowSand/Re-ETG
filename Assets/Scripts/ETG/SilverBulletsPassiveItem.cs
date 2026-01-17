// Decompiled with JetBrains decompiler
// Type: SilverBulletsPassiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class SilverBulletsPassiveItem : PassiveItem
{
  public float BlackPhantomDamageMultiplier = 2f;
  private PlayerController m_player;
  public bool TintBullets;
  public bool TintBeams;
  public Color TintColor = Color.grey;
  public int TintPriority = 1;
  public GameObject SynergyPowerVFX;
  private StatModifier m_synergyStat;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    this.m_player = player;
    base.Pickup(player);
    player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
    player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
    player.OnKilledEnemyContext += new Action<PlayerController, HealthHaver>(this.HandleKilledEnemy);
  }

  private void HandleKilledEnemy(PlayerController sourcePlayer, HealthHaver killedEnemy)
  {
    if (!sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.BLESSED_CURSED_BULLETS) || !(bool) (UnityEngine.Object) killedEnemy || !(bool) (UnityEngine.Object) killedEnemy.aiActor || !killedEnemy.aiActor.IsBlackPhantom)
      return;
    if (this.m_synergyStat == null)
    {
      this.m_synergyStat = StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.MULTIPLICATIVE, 1f);
      sourcePlayer.ownerlessStatModifiers.Add(this.m_synergyStat);
    }
    this.m_synergyStat.amount += 1f / 400f;
    sourcePlayer.PlayEffectOnActor(this.SynergyPowerVFX, new Vector3(0.0f, -0.5f, 0.0f));
    sourcePlayer.stats.RecalculateStats(sourcePlayer);
  }

  private void PostProcessBeam(BeamController beam)
  {
    if (!(bool) (UnityEngine.Object) beam)
      return;
    Projectile projectile = beam.projectile;
    if ((bool) (UnityEngine.Object) projectile)
      this.PostProcessProjectile(projectile, 1f);
    beam.AdjustPlayerBeamTint(this.TintColor.WithAlpha(this.TintColor.a / 2f), this.TintPriority);
  }

  private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
  {
    if (!(bool) (UnityEngine.Object) this.m_player)
      return;
    obj.BlackPhantomDamageMultiplier *= this.BlackPhantomDamageMultiplier;
    if ((bool) (UnityEngine.Object) this.m_player && this.m_player.HasActiveBonusSynergy(CustomSynergyType.DEMONHUNTER))
      obj.BlackPhantomDamageMultiplier *= 1.5f;
    if (!this.TintBullets)
      return;
    obj.AdjustPlayerProjectileTint(this.TintColor, this.TintPriority);
  }

  private void RemoveSynergyStat(PlayerController targetPlayer)
  {
    if (this.m_synergyStat == null || !(bool) (UnityEngine.Object) targetPlayer)
      return;
    targetPlayer.ownerlessStatModifiers.Remove(this.m_synergyStat);
    targetPlayer.stats.RecalculateStats(targetPlayer);
    this.m_synergyStat = (StatModifier) null;
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    this.m_player = (PlayerController) null;
    debrisObject.GetComponent<SilverBulletsPassiveItem>().m_pickedUpThisRun = true;
    player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
    player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
    player.OnKilledEnemyContext -= new Action<PlayerController, HealthHaver>(this.HandleKilledEnemy);
    this.RemoveSynergyStat(player);
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (!(bool) (UnityEngine.Object) this.m_player)
      return;
    this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
    this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
    this.m_player.OnKilledEnemyContext -= new Action<PlayerController, HealthHaver>(this.HandleKilledEnemy);
    this.RemoveSynergyStat(this.m_player);
  }
}
