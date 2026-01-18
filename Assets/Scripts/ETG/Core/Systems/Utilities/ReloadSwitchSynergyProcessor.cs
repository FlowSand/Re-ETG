using System;

using UnityEngine;

#nullable disable

public class ReloadSwitchSynergyProcessor : MonoBehaviour
    {
        [PickupIdentifier]
        public int PartnerGunID;
        [LongNumericEnum]
        public CustomSynergyType RequiredSynergy;
        public bool ReloadsTargetGun = true;
        private Gun m_gun;

        public void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            this.m_gun.OnPostFired += new Action<PlayerController, Gun>(this.HandlePostFired);
        }

        private void HandlePostFired(PlayerController sourcePlayer, Gun sourceGun)
        {
            if (!sourcePlayer.HasActiveBonusSynergy(this.RequiredSynergy) || this.m_gun.ClipShotsRemaining != 0)
                return;
            for (int index = 0; index < sourcePlayer.inventory.AllGuns.Count; ++index)
            {
                if (sourcePlayer.inventory.AllGuns[index].PickupObjectId == this.PartnerGunID && sourcePlayer.inventory.AllGuns[index].ammo > 0)
                {
                    sourcePlayer.inventory.GunChangeForgiveness = true;
                    sourcePlayer.ChangeToGunSlot(index);
                    sourcePlayer.inventory.AllGuns[index].ForceImmediateReload(true);
                    sourcePlayer.inventory.GunChangeForgiveness = false;
                    break;
                }
            }
        }
    }

