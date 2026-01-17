// Decompiled with JetBrains decompiler
// Type: SnowballBulletsItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class SnowballBulletsItem : PassiveItem
{
  public float PercentScaleGainPerUnit = 10f;
  public float PercentDamageGainPerUnit = 2.5f;
  public float DamageMultiplierCap = 2.5f;
  private PlayerController m_player;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    this.m_player = player;
    base.Pickup(player);
    player.PostProcessProjectile += new Action<Projectile, float>(this.HandleProjectile);
    player.PostProcessBeamChanceTick += new Action<BeamController>(this.HandleBeamFrame);
  }

  private void HandleBeamFrame(BeamController sourceBeam)
  {
    if (!(sourceBeam is BasicBeamController))
      return;
    BasicBeamController basicBeamController = sourceBeam as BasicBeamController;
    basicBeamController.ProjectileScale = (basicBeamController.Owner as PlayerController).BulletScaleModifier + basicBeamController.ApproximateDistance * (this.PercentScaleGainPerUnit / 100f);
  }

  private void HandleProjectile(Projectile targetProjectile, float arg2)
  {
    ScalingProjectileModifier projectileModifier = targetProjectile.gameObject.AddComponent<ScalingProjectileModifier>();
    projectileModifier.ScaleToDamageRatio = this.PercentDamageGainPerUnit / this.PercentScaleGainPerUnit;
    projectileModifier.MaximumDamageMultiplier = this.DamageMultiplierCap;
    projectileModifier.IsSynergyContingent = false;
    if (this.Owner.HasActiveBonusSynergy(CustomSynergyType.SNOWBREAKERS))
    {
      projectileModifier.PercentGainPerUnit = this.PercentScaleGainPerUnit * 1.5f;
      projectileModifier.ScaleMultiplier = 2f;
    }
    else
      projectileModifier.PercentGainPerUnit = this.PercentScaleGainPerUnit;
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    this.m_player = (PlayerController) null;
    debrisObject.GetComponent<SnowballBulletsItem>().m_pickedUpThisRun = true;
    player.PostProcessProjectile -= new Action<Projectile, float>(this.HandleProjectile);
    player.PostProcessBeamChanceTick -= new Action<BeamController>(this.HandleBeamFrame);
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    if (!(bool) (UnityEngine.Object) this.m_player)
      return;
    this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.HandleProjectile);
    this.m_player.PostProcessBeamChanceTick -= new Action<BeamController>(this.HandleBeamFrame);
  }
}
