using System;

#nullable disable

public class GunClassPassiveItem : PassiveItem
  {
    public GunClass[] classesToModify;
    public float[] damageModifiers;
    private PlayerController m_player;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_player = player;
      base.Pickup(player);
      player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
    }

    private void PostProcessBeam(BeamController obj)
    {
      if (!(bool) (UnityEngine.Object) this.m_player || !(bool) (UnityEngine.Object) this.m_player.CurrentGun || this.damageModifiers == null || !(bool) (UnityEngine.Object) obj || !(bool) (UnityEngine.Object) obj.projectile)
        return;
      for (int index = 0; index < this.classesToModify.Length; ++index)
      {
        if (this.m_player.CurrentGun.gunClass == this.classesToModify[index])
          obj.projectile.baseData.damage *= this.damageModifiers[index];
      }
    }

    private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
    {
      for (int index = 0; index < this.classesToModify.Length; ++index)
      {
        if ((UnityEngine.Object) this.m_player.CurrentGun != (UnityEngine.Object) null && this.m_player.CurrentGun.gunClass == this.classesToModify[index])
          obj.baseData.damage *= this.damageModifiers[index];
      }
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      this.m_player = (PlayerController) null;
      debrisObject.GetComponent<GunClassPassiveItem>().m_pickedUpThisRun = true;
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

