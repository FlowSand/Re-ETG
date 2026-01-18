using System;

using UnityEngine;

#nullable disable

public class MoveAmmoToClipItem : PassiveItem
    {
        public int BulletsToMove = 1;
        public bool TriggerOnRoll;
        public float ActivationChance = 1f;
        public NumericSynergyMultiplier[] moveMultipliers;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            if (this.TriggerOnRoll)
                player.OnRollStarted += new Action<PlayerController, Vector2>(this.HandleRollStarted);
            base.Pickup(player);
        }

        private void HandleRollStarted(PlayerController arg1, Vector2 arg2) => this.DoEffect(arg1);

        private int GetBulletsToMove(PlayerController source)
        {
            float bulletsToMove = (float) this.BulletsToMove;
            for (int index = 0; index < this.moveMultipliers.Length; ++index)
            {
                if ((bool) (UnityEngine.Object) source && source.HasActiveBonusSynergy(this.moveMultipliers[index].RequiredSynergy))
                    bulletsToMove *= this.moveMultipliers[index].SynergyMultiplier;
            }
            return Mathf.RoundToInt(bulletsToMove);
        }

        private void DoEffect(PlayerController source)
        {
            if ((double) UnityEngine.Random.value >= (double) this.ActivationChance || !((UnityEngine.Object) source.CurrentGun != (UnityEngine.Object) null))
                return;
            source.CurrentGun.MoveBulletsIntoClip(this.GetBulletsToMove(source));
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRollStarted);
            debrisObject.GetComponent<MoveAmmoToClipItem>().m_pickedUpThisRun = true;
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null))
                return;
            this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRollStarted);
        }
    }

