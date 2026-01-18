using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PegasusBootsItem : PassiveItem
  {
    public bool ModifiesDodgeRoll;
    [ShowInInspectorIf("ModifiesDodgeRoll", false)]
    public float DodgeRollTimeMultiplier = 0.9f;
    [ShowInInspectorIf("ModifiesDodgeRoll", false)]
    public float DodgeRollDistanceMultiplier = 1.25f;
    [ShowInInspectorIf("ModifiesDodgeRoll", false)]
    public int AdditionalInvulnerabilityFrames;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      if (this.ModifiesDodgeRoll)
      {
        player.rollStats.rollDistanceMultiplier *= this.DodgeRollDistanceMultiplier;
        player.rollStats.rollTimeMultiplier *= this.DodgeRollTimeMultiplier;
        player.rollStats.additionalInvulnerabilityFrames += this.AdditionalInvulnerabilityFrames;
      }
      if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
        PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
      if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
        PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
      else
        PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
      player.OnRollStarted += new Action<PlayerController, Vector2>(this.OnRollStarted);
      base.Pickup(player);
    }

    private void OnRollStarted(PlayerController obj, Vector2 dirVec)
    {
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      if (this.ModifiesDodgeRoll)
      {
        player.rollStats.rollDistanceMultiplier /= this.DodgeRollDistanceMultiplier;
        player.rollStats.rollTimeMultiplier /= this.DodgeRollTimeMultiplier;
        player.rollStats.additionalInvulnerabilityFrames -= this.AdditionalInvulnerabilityFrames;
        player.rollStats.additionalInvulnerabilityFrames = Mathf.Max(player.rollStats.additionalInvulnerabilityFrames, 0);
      }
      if (PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      {
        PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
          PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
      }
      player.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
      debrisObject.GetComponent<PegasusBootsItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if (this.m_pickedUp && (bool) (UnityEngine.Object) this.m_owner && PassiveItem.ActiveFlagItems != null && PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
      {
        PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
          PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
      }
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
        this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
      base.OnDestroy();
    }
  }

