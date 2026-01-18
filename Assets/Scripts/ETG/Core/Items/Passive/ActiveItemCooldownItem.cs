#nullable disable

public class ActiveItemCooldownItem : PassiveItem
    {
        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            PlayerItem.AllowDamageCooldownOnActive = true;
            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            PlayerItem.AllowDamageCooldownOnActive = false;
            debrisObject.GetComponent<ActiveItemCooldownItem>().m_pickedUpThisRun = true;
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            PlayerItem.AllowDamageCooldownOnActive = false;
            base.OnDestroy();
        }
    }

