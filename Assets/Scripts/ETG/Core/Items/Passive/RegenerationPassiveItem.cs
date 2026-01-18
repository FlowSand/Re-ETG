using System;

#nullable disable

public class RegenerationPassiveItem : PassiveItem
    {
        public float RequiredDamage = 1000f;
        protected PlayerController m_player;
        protected float m_damageDealtCounter;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            this.m_player = player;
            this.m_player.OnDealtDamage += new Action<PlayerController, float>(this.PlayerDealtDamage);
            base.Pickup(player);
        }

        private void PlayerDealtDamage(PlayerController p, float damage)
        {
            if ((double) p.healthHaver.GetCurrentHealthPercentage() >= 1.0)
            {
                this.m_damageDealtCounter = 0.0f;
            }
            else
            {
                this.m_damageDealtCounter += damage;
                if ((double) this.m_damageDealtCounter < (double) this.RequiredDamage)
                    return;
                p.healthHaver.ApplyHealing(0.5f);
                this.m_damageDealtCounter = 0.0f;
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<RegenerationPassiveItem>().m_pickedUpThisRun = true;
            this.m_player.OnDealtDamage -= new Action<PlayerController, float>(this.PlayerDealtDamage);
            this.m_player = (PlayerController) null;
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!(bool) (UnityEngine.Object) this.m_player)
                return;
            this.m_player.OnDealtDamage -= new Action<PlayerController, float>(this.PlayerDealtDamage);
        }
    }

