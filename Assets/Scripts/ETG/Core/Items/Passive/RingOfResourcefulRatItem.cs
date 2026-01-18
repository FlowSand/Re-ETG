using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class RingOfResourcefulRatItem : PassiveItem, ILevelLoadedListener
  {
    private bool m_initializedEver;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
        PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
      if (!this.m_initializedEver)
      {
        if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
          PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
        else
          PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
        this.m_initializedEver = true;
      }
      base.Pickup(player);
    }

    protected override void Update() => base.Update();

    public void BraveOnLevelWasLoaded()
    {
      if (!(bool) (UnityEngine.Object) this.m_owner)
        return;
      if (!PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
        PassiveItem.ActiveFlagItems[this.m_owner].Add(this.GetType(), 1);
      else
        PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] + 1;
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
      debrisObject.GetComponent<RingOfResourcefulRatItem>().m_pickedUpThisRun = true;
      debrisObject.GetComponent<RingOfResourcefulRatItem>().m_initializedEver = true;
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

