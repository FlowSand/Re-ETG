using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class BriefcaseFullOfCashItem : PassiveItem
    {
        public int CurrencyAmount = 200;
        public int MetaCurrencyAmount = 3;
        private bool m_hasTriggered;

        public override void Pickup(PlayerController player)
        {
            if (this.m_pickedUp)
                return;
            if (this.m_pickedUpThisRun)
                this.m_hasTriggered = true;
            if (!this.m_pickedUpThisRun && !this.m_hasTriggered)
            {
                this.m_hasTriggered = true;
                player.carriedConsumables.Currency += this.CurrencyAmount;
                LootEngine.SpawnCurrency(player.CenterPosition, this.MetaCurrencyAmount, true, new Vector2?(Vector2.down), new float?(45f), 0.5f, 0.25f);
            }
            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            debrisObject.GetComponent<BriefcaseFullOfCashItem>().m_pickedUpThisRun = true;
            return debrisObject;
        }

        protected override void OnDestroy()
        {
            BraveTime.ClearMultiplier(this.gameObject);
            if (!this.m_pickedUp)
                ;
            base.OnDestroy();
        }

        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);
            data.Add((object) this.m_hasTriggered);
        }

        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameDeserialize(data);
            if (data.Count != 1)
                return;
            this.m_hasTriggered = (bool) data[0];
        }
    }

