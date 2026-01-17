// Decompiled with JetBrains decompiler
// Type: BasicStatPickup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class BasicStatPickup : PassiveItem
    {
      [BetterList]
      public List<StatModifier> modifiers;
      public int ArmorToGive;
      public bool ModifiesDodgeRoll;
      [ShowInInspectorIf("ModifiesDodgeRoll", false)]
      public float DodgeRollTimeMultiplier = 0.9f;
      [ShowInInspectorIf("ModifiesDodgeRoll", false)]
      public float DodgeRollDistanceMultiplier = 1.25f;
      [ShowInInspectorIf("ModifiesDodgeRoll", false)]
      public int AdditionalInvulnerabilityFrames;
      public bool IsJunk;
      public bool GivesCurrency;
      public int CurrencyToGive;
      public bool IsMasteryToken;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        if (this.ArmorToGive > 0 && !this.m_pickedUpThisRun)
          player.healthHaver.Armor += (float) this.ArmorToGive;
        else if (!this.m_pickedUpThisRun && this.IsMasteryToken && player.characterIdentity == PlayableCharacters.Robot)
          ++player.healthHaver.Armor;
        if (this.ModifiesDodgeRoll)
        {
          player.rollStats.rollDistanceMultiplier *= this.DodgeRollDistanceMultiplier;
          player.rollStats.rollTimeMultiplier *= this.DodgeRollTimeMultiplier;
          player.rollStats.additionalInvulnerabilityFrames += this.AdditionalInvulnerabilityFrames;
        }
        if (!this.m_pickedUpThisRun && this.IsJunk && player.characterIdentity == PlayableCharacters.Robot)
        {
          player.ownerlessStatModifiers.Add(new StatModifier()
          {
            statToBoost = PlayerStats.StatType.Damage,
            amount = 0.05f,
            modifyType = StatModifier.ModifyMethod.ADDITIVE
          });
          player.stats.RecalculateStats(player);
        }
        if (!this.m_pickedUpThisRun && this.GivesCurrency)
          player.carriedConsumables.Currency += this.CurrencyToGive;
        if (!this.m_pickedUpThisRun && player.characterIdentity == PlayableCharacters.Robot)
        {
          for (int index = 0; index < this.modifiers.Count; ++index)
          {
            if (this.modifiers[index].statToBoost == PlayerStats.StatType.Health && (double) this.modifiers[index].amount > 0.0)
            {
              int amountToDrop = Mathf.FloorToInt(this.modifiers[index].amount * (float) Random.Range(GameManager.Instance.RewardManager.RobotMinCurrencyPerHealthItem, GameManager.Instance.RewardManager.RobotMaxCurrencyPerHealthItem + 1));
              LootEngine.SpawnCurrency(player.CenterPosition, amountToDrop);
            }
          }
        }
        base.Pickup(player);
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
        debrisObject.GetComponent<BasicStatPickup>().m_pickedUpThisRun = true;
        return debrisObject;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
