// Decompiled with JetBrains decompiler
// Type: GunMergeSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
