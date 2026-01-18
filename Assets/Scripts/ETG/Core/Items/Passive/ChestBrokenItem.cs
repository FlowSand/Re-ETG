using System;

using UnityEngine;

#nullable disable

public class ChestBrokenItem : PassiveItem
    {
        public float ActivationChance = 1f;
        public float HealAmount = 0.5f;
        public GameObject HealVFX;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            base.Pickup(player);
            player.OnChestBroken += new Action<PlayerController, Chest>(this.HandleChestBroken);
        }

        private void HandleChestBroken(PlayerController arg1, Chest arg2)
        {
            if ((double) UnityEngine.Random.value >= (double) this.ActivationChance)
                return;
            arg1.healthHaver.ApplyHealing(this.HealAmount);
            arg1.PlayEffectOnActor(this.HealVFX, Vector3.zero);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            ChestBrokenItem component = debrisObject.GetComponent<ChestBrokenItem>();
            if ((bool) (UnityEngine.Object) player)
                player.OnChestBroken -= new Action<PlayerController, Chest>(this.HandleChestBroken);
            if ((bool) (UnityEngine.Object) component)
                component.m_pickedUpThisRun = true;
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            if ((bool) (UnityEngine.Object) this.m_owner)
                this.m_owner.OnChestBroken -= new Action<PlayerController, Chest>(this.HandleChestBroken);
            base.OnDestroy();
        }
    }

