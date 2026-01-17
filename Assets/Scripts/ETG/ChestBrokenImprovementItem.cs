// Decompiled with JetBrains decompiler
// Type: ChestBrokenImprovementItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ChestBrokenImprovementItem : PassiveItem
{
  public static float PickupQualChance;
  public static float MinusOneQualChance = 0.5f;
  public static float EqualQualChance = 0.45f;
  public static float PlusQualChance = 0.05f;
  public float ChanceForPickupQuality;
  public float ChanceForMinusOneQuality = 0.5f;
  public float ChanceForEqualQuality = 0.45f;
  public float ChanceForPlusOneQuality = 0.05f;

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    float num = this.ChanceForPickupQuality + this.ChanceForMinusOneQuality + this.ChanceForEqualQuality + this.ChanceForPlusOneQuality;
    ChestBrokenImprovementItem.PickupQualChance = this.ChanceForPickupQuality / num;
    ChestBrokenImprovementItem.MinusOneQualChance = this.ChanceForMinusOneQuality / num;
    ChestBrokenImprovementItem.EqualQualChance = this.ChanceForEqualQuality / num;
    ChestBrokenImprovementItem.PlusQualChance = this.ChanceForPlusOneQuality / num;
    if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
      PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
    if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
    else
      PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
    base.Pickup(player);
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    if (PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
    }
    if ((bool) (UnityEngine.Object) debrisObject && (bool) (UnityEngine.Object) debrisObject.GetComponent<ChestBrokenImprovementItem>())
      debrisObject.GetComponent<ChestBrokenImprovementItem>().m_pickedUpThisRun = true;
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    BraveTime.ClearMultiplier(this.gameObject);
    if (this.m_pickedUp && PassiveItem.ActiveFlagItems != null && PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
    {
      PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
      if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
        PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
    }
    base.OnDestroy();
  }
}
