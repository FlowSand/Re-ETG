using UnityEngine;

#nullable disable

public class DamageTypeModifierItem : PassiveItem
  {
    public DamageTypeModifier[] modifiers;
    private PlayerController m_player;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_player = player;
      for (int index = 0; index < this.modifiers.Length; ++index)
        player.healthHaver.damageTypeModifiers.Add(this.modifiers[index]);
      base.Pickup(player);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      for (int index = 0; index < this.modifiers.Length; ++index)
        player.healthHaver.damageTypeModifiers.Remove(this.modifiers[index]);
      this.m_player = (PlayerController) null;
      debrisObject.GetComponent<DamageTypeModifierItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if ((Object) this.m_player != (Object) null)
      {
        for (int index = 0; index < this.modifiers.Length; ++index)
          this.m_player.healthHaver.damageTypeModifiers.Remove(this.modifiers[index]);
      }
      base.OnDestroy();
    }
  }

