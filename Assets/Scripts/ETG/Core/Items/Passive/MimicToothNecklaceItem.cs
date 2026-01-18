using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class MimicToothNecklaceItem : PassiveItem
  {
    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
        PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
      if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
        PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
      else
        PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
      for (int index = 0; index < StaticReferenceManager.AllChests.Count; ++index)
      {
        Chest allChest = StaticReferenceManager.AllChests[index];
        if ((bool) (UnityEngine.Object) allChest && !allChest.IsOpen && !allChest.IsBroken)
          allChest.MaybeBecomeMimic();
      }
      base.Pickup(player);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      if (PassiveItem.ActiveFlagItems.ContainsKey(player) && PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      {
        PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
          PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
      }
      debrisObject.GetComponent<MimicToothNecklaceItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      BraveTime.ClearMultiplier(this.gameObject);
      if (this.m_pickedUp && PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
      {
        PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
          PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
      }
      base.OnDestroy();
    }
  }

