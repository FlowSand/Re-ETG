using System.Collections.Generic;

#nullable disable

public class CoopPassiveItem : PassiveItem
    {
        public List<StatModifier> modifiers;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<CoopPassiveItem>().m_pickedUpThisRun = true;
            return debrisObject;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

