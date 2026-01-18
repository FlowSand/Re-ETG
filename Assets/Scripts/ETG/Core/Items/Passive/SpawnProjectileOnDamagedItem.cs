using System;
using UnityEngine;

#nullable disable

public class SpawnProjectileOnDamagedItem : PassiveItem
  {
    public float chanceToSpawn = 1f;
    public int minNumToSpawn = 1;
    public int maxNumToSpawn = 1;
    public Projectile projectileToSpawn;
    public bool CanBeModifiedBySynergy;
    public CustomSynergyType SynergyToCheck;
    public Projectile synergyProjectile;
    public bool randomAngle = true;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      player.OnReceivedDamage += new Action<PlayerController>(this.PlayerWasDamaged);
      base.Pickup(player);
    }

    private void PlayerWasDamaged(PlayerController obj)
    {
      if ((double) UnityEngine.Random.value >= (double) this.chanceToSpawn)
        return;
      int num1 = UnityEngine.Random.Range(this.minNumToSpawn, this.maxNumToSpawn + 1);
      float max = 360f / (float) num1;
      float num2 = UnityEngine.Random.Range(0.0f, max);
      Projectile projectile = this.projectileToSpawn;
      if (this.CanBeModifiedBySynergy && (bool) (UnityEngine.Object) obj && obj.HasActiveBonusSynergy(this.SynergyToCheck))
        projectile = this.synergyProjectile;
      for (int index = 0; index < num1; ++index)
      {
        float z = !this.randomAngle ? num2 + max * (float) index : (float) UnityEngine.Random.Range(0, 360);
        Projectile component = SpawnManager.SpawnProjectile(projectile.gameObject, (Vector3) obj.specRigidbody.UnitCenter, Quaternion.Euler(0.0f, 0.0f, z)).GetComponent<Projectile>();
        component.Owner = (GameActor) obj;
        component.Shooter = obj.specRigidbody;
      }
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      player.OnReceivedDamage -= new Action<PlayerController>(this.PlayerWasDamaged);
      debrisObject.GetComponent<SpawnProjectileOnDamagedItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

