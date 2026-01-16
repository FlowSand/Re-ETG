// Decompiled with JetBrains decompiler
// Type: CursedItemModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class CursedItemModifier : MonoBehaviour
{
  private PickupObject m_pickup;
  private StatModifier m_addedModifier;

  private void Start()
  {
    this.m_pickup = this.GetComponent<PickupObject>();
    if (this.m_pickup is PassiveItem)
    {
      PassiveItem pickup = this.m_pickup as PassiveItem;
      StatModifier[] passiveStatModifiers = pickup.passiveStatModifiers;
      StatModifier statModifier = new StatModifier();
      statModifier.amount = 1f;
      statModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
      statModifier.statToBoost = PlayerStats.StatType.Curse;
      Array.Resize<StatModifier>(ref passiveStatModifiers, passiveStatModifiers.Length + 1);
      this.m_addedModifier = statModifier;
      passiveStatModifiers[passiveStatModifiers.Length - 1] = statModifier;
      if (!((UnityEngine.Object) pickup.Owner != (UnityEngine.Object) null))
        return;
      pickup.Owner.stats.RecalculateStats(pickup.Owner);
    }
    else if (!(this.m_pickup is PlayerItem))
      ;
  }

  private void OnDestroy()
  {
    if (!(this.m_pickup is PassiveItem))
      return;
    PassiveItem pickup = this.m_pickup as PassiveItem;
    List<StatModifier> statModifierList = new List<StatModifier>((IEnumerable<StatModifier>) pickup.passiveStatModifiers);
    statModifierList.Remove(this.m_addedModifier);
    pickup.passiveStatModifiers = statModifierList.ToArray();
    if (!((UnityEngine.Object) pickup.Owner != (UnityEngine.Object) null))
      return;
    pickup.Owner.stats.RecalculateStats(pickup.Owner);
  }
}
