using UnityEngine;

#nullable disable

public class GundromedaStrain : PassiveItem
    {
        public float percentageHealthReduction = 0.1f;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            base.Pickup(player);
            AIActor.HealthModifier *= Mathf.Clamp01(1f - this.percentageHealthReduction);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<GundromedaStrain>().m_pickedUpThisRun = true;
            AIActor.HealthModifier /= Mathf.Clamp01(1f - this.percentageHealthReduction);
            return debrisObject;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

