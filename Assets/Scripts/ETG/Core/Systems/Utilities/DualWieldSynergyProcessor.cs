using System;

using UnityEngine;

#nullable disable

public class DualWieldSynergyProcessor : MonoBehaviour
    {
        [LongNumericEnum]
        public CustomSynergyType SynergyToCheck;
        [PickupIdentifier]
        public int PartnerGunID;
        private Gun m_gun;
        private bool m_isCurrentlyActive;
        private PlayerController m_cachedPlayer;

        public void Awake() => this.m_gun = this.GetComponent<Gun>();

        private bool EffectValid(PlayerController p)
        {
            if (!(bool) (UnityEngine.Object) p || !p.HasActiveBonusSynergy(this.SynergyToCheck) || this.m_gun.CurrentAmmo == 0 || p.inventory.GunLocked.Value)
                return false;
            if (!this.m_isCurrentlyActive)
            {
                int indexForGun = this.GetIndexForGun(p, this.PartnerGunID);
                if (indexForGun < 0 || p.inventory.AllGuns[indexForGun].CurrentAmmo == 0)
                    return false;
            }
            else if ((UnityEngine.Object) p.CurrentSecondaryGun != (UnityEngine.Object) null && p.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID && p.CurrentSecondaryGun.CurrentAmmo == 0)
                return false;
            return true;
        }

        private bool PlayerUsingCorrectGuns()
        {
            return (bool) (UnityEngine.Object) this.m_gun && (bool) (UnityEngine.Object) this.m_gun.CurrentOwner && (bool) (UnityEngine.Object) this.m_cachedPlayer && this.m_cachedPlayer.inventory.DualWielding && this.m_cachedPlayer.HasActiveBonusSynergy(this.SynergyToCheck) && (!((UnityEngine.Object) this.m_cachedPlayer.CurrentGun != (UnityEngine.Object) this.m_gun) || this.m_cachedPlayer.CurrentGun.PickupObjectId == this.PartnerGunID) && (!((UnityEngine.Object) this.m_cachedPlayer.CurrentSecondaryGun != (UnityEngine.Object) this.m_gun) || this.m_cachedPlayer.CurrentSecondaryGun.PickupObjectId == this.PartnerGunID);
        }

        private void Update() => this.CheckStatus();

        private void CheckStatus()
        {
            if (this.m_isCurrentlyActive)
            {
                if (this.PlayerUsingCorrectGuns() && this.EffectValid(this.m_cachedPlayer))
                    return;
                this.DisableEffect();
            }
            else
            {
                if (!(bool) (UnityEngine.Object) this.m_gun || !(this.m_gun.CurrentOwner is PlayerController))
                    return;
                PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
                if (currentOwner.inventory.DualWielding && currentOwner.CurrentSecondaryGun.PickupObjectId == this.m_gun.PickupObjectId && currentOwner.CurrentGun.PickupObjectId == this.PartnerGunID)
                {
                    this.m_isCurrentlyActive = true;
                    this.m_cachedPlayer = currentOwner;
                }
                else
                    this.AttemptActivation(currentOwner);
            }
        }

        private void AttemptActivation(PlayerController ownerPlayer)
        {
            if (!this.EffectValid(ownerPlayer))
                return;
            this.m_isCurrentlyActive = true;
            this.m_cachedPlayer = ownerPlayer;
            ownerPlayer.inventory.SetDualWielding(true, "synergy");
            int indexForGun1 = this.GetIndexForGun(ownerPlayer, this.m_gun.PickupObjectId);
            int indexForGun2 = this.GetIndexForGun(ownerPlayer, this.PartnerGunID);
            ownerPlayer.inventory.SwapDualGuns();
            if (indexForGun1 >= 0 && indexForGun2 >= 0)
            {
                while (ownerPlayer.inventory.CurrentGun.PickupObjectId != this.PartnerGunID)
                    ownerPlayer.inventory.ChangeGun(1);
            }
            ownerPlayer.inventory.SwapDualGuns();
            if ((bool) (UnityEngine.Object) ownerPlayer.CurrentGun && !ownerPlayer.CurrentGun.gameObject.activeSelf)
                ownerPlayer.CurrentGun.gameObject.SetActive(true);
            if ((bool) (UnityEngine.Object) ownerPlayer.CurrentSecondaryGun && !ownerPlayer.CurrentSecondaryGun.gameObject.activeSelf)
                ownerPlayer.CurrentSecondaryGun.gameObject.SetActive(true);
            this.m_cachedPlayer.GunChanged += new Action<Gun, Gun, bool>(this.HandleGunChanged);
        }

        private int GetIndexForGun(PlayerController p, int gunID)
        {
            for (int index = 0; index < p.inventory.AllGuns.Count; ++index)
            {
                if (p.inventory.AllGuns[index].PickupObjectId == gunID)
                    return index;
            }
            return -1;
        }

        private void HandleGunChanged(Gun arg1, Gun newGun, bool arg3) => this.CheckStatus();

        private void DisableEffect()
        {
            if (!this.m_isCurrentlyActive)
                return;
            this.m_isCurrentlyActive = false;
            this.m_cachedPlayer.inventory.SetDualWielding(false, "synergy");
            this.m_cachedPlayer.GunChanged -= new Action<Gun, Gun, bool>(this.HandleGunChanged);
            this.m_cachedPlayer.stats.RecalculateStats(this.m_cachedPlayer);
            this.m_cachedPlayer = (PlayerController) null;
        }
    }

