using UnityEngine;

#nullable disable

public class MimicGunMimicModifier : MonoBehaviour
    {
        private Gun m_gun;
        private bool m_initialized;

        private void Start() => this.m_gun = this.GetComponent<Gun>();

        private void Update()
        {
            if (this.m_initialized || !((Object) this.m_gun.CurrentOwner != (Object) null))
                return;
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            if (currentOwner.IsGunLocked)
            {
                Object.Destroy((Object) this);
            }
            else
            {
                currentOwner.inventory.AddGunToInventory(PickupObjectDatabase.GetById(GlobalItemIds.GunMimicID) as Gun, true).GetComponent<MimicGunController>().Initialize(currentOwner, this.m_gun);
                this.m_initialized = true;
                Object.Destroy((Object) this);
            }
        }
    }

