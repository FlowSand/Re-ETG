using UnityEngine;

#nullable disable

public class GunMergeSynergyProcessor : MonoBehaviour
  {
    [PickupIdentifier]
    public int OtherGunID;
    [PickupIdentifier]
    public int MergeGunID;
    private Gun m_gun;

    private void Awake() => this.m_gun = this.GetComponent<Gun>();

    public void Update()
    {
      PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
      if (!(bool) (Object) currentOwner)
        return;
      for (int index = 0; index < currentOwner.inventory.AllGuns.Count; ++index)
      {
        if (currentOwner.inventory.AllGuns[index].PickupObjectId == this.OtherGunID)
        {
          currentOwner.inventory.RemoveGunFromInventory(currentOwner.inventory.AllGuns[index]);
          currentOwner.inventory.RemoveGunFromInventory(this.m_gun);
          LootEngine.TryGiveGunToPlayer(PickupObjectDatabase.GetById(this.MergeGunID).gameObject, currentOwner, true);
        }
      }
    }
  }

