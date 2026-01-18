using System;

#nullable disable

public class OurPowersCombinedItem : PassiveItem
  {
    public float PercentOfOtherGunsDamage = 0.02f;
    protected PlayerController m_player;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_player = player;
      base.Pickup(player);
      player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
      player.PostProcessBeamTick += new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
    }

    private float GetDamageContribution()
    {
      float damageContribution = 0.0f;
      if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
      {
        for (int index1 = 0; index1 < this.m_player.inventory.AllGuns.Count; ++index1)
        {
          Gun allGun = this.m_player.inventory.AllGuns[index1];
          if (!((UnityEngine.Object) allGun == (UnityEngine.Object) this.m_player.CurrentGun) && allGun.DefaultModule != null)
          {
            if (allGun.DefaultModule.projectiles.Count > 0 && (UnityEngine.Object) allGun.DefaultModule.projectiles[0] != (UnityEngine.Object) null)
              damageContribution += allGun.DefaultModule.projectiles[0].baseData.damage * this.PercentOfOtherGunsDamage;
            else if (allGun.DefaultModule.chargeProjectiles != null && allGun.DefaultModule.chargeProjectiles.Count > 0)
            {
              for (int index2 = 0; index2 < allGun.DefaultModule.chargeProjectiles.Count; ++index2)
              {
                if ((UnityEngine.Object) allGun.DefaultModule.chargeProjectiles[index2].Projectile != (UnityEngine.Object) null)
                {
                  damageContribution += allGun.DefaultModule.chargeProjectiles[index2].Projectile.baseData.damage * this.PercentOfOtherGunsDamage;
                  break;
                }
              }
            }
          }
        }
      }
      return damageContribution;
    }

    private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
    {
      obj.baseData.damage += this.GetDamageContribution();
    }

    private void PostProcessBeam(BeamController beam)
    {
      beam.DamageModifier += this.GetDamageContribution();
    }

    private void PostProcessBeamTick(
      BeamController beam,
      SpeculativeRigidbody hitRigidbody,
      float tickRate)
    {
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<OurPowersCombinedItem>().m_pickedUpThisRun = true;
      this.m_player = (PlayerController) null;
      player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.m_player)
        return;
      this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      this.m_player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      this.m_player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.PostProcessBeamTick);
    }
  }

