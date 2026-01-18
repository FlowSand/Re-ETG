using System;

#nullable disable

public class OnPlayerItemUsedItem : PassiveItem
    {
        public float ActivationChance = 1f;
        public bool TriggersBlank;
        public bool TriggersRadialBulletBurst;
        [ShowInInspectorIf("TriggersRadialBulletBurst", false)]
        public RadialBurstInterface RadialBurstSettings;
        public float InternalCooldown = 10f;
        private float m_lastUsedTime = -1000f;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            base.Pickup(player);
            player.OnUsedPlayerItem += new Action<PlayerController, PlayerItem>(this.DoEffect);
        }

        private void DoEffect(PlayerController usingPlayer, PlayerItem usedItem)
        {
            if ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.m_lastUsedTime < (double) this.InternalCooldown)
                return;
            this.m_lastUsedTime = UnityEngine.Time.realtimeSinceStartup;
            if ((double) UnityEngine.Random.value >= (double) this.ActivationChance)
                return;
            if (this.TriggersBlank)
                usingPlayer.ForceBlank();
            if (!this.TriggersRadialBulletBurst)
                return;
            this.RadialBurstSettings.DoBurst(usingPlayer);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            OnPlayerItemUsedItem component = debrisObject.GetComponent<OnPlayerItemUsedItem>();
            player.OnUsedPlayerItem -= new Action<PlayerController, PlayerItem>(this.DoEffect);
            component.m_pickedUpThisRun = true;
            return debrisObject;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

