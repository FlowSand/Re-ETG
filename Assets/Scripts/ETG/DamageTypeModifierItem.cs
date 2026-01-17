// Decompiled with JetBrains decompiler
// Type: DamageTypeModifierItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
