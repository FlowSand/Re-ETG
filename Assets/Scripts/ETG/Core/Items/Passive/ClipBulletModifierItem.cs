using System;

#nullable disable

public class ClipBulletModifierItem : PassiveItem
    {
        public float ActivationChance = 1f;
        public bool FirstShotBoost;
        public float FirstShotMultiplier = 2f;
        public bool LastShotBoost;
        public float LastShotMultiplier = 2f;
        private PlayerController m_player;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            this.m_player = player;
            base.Pickup(player);
            player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
        }

        private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
        {
            if ((double) UnityEngine.Random.value >= (double) this.ActivationChance)
                return;
            if (this.FirstShotBoost && this.m_player.CurrentGun.LastShotIndex == 0)
                obj.baseData.damage *= this.FirstShotMultiplier;
            if (!this.LastShotBoost || this.m_player.CurrentGun.LastShotIndex != this.m_player.CurrentGun.ClipCapacity - 1)
                return;
            obj.baseData.damage *= this.LastShotMultiplier;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            this.m_player = (PlayerController) null;
            debrisObject.GetComponent<ClipBulletModifierItem>().m_pickedUpThisRun = true;
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

