using System;
using UnityEngine;

#nullable disable

[Serializable]
public class StatModifier
  {
    public PlayerStats.StatType statToBoost;
    public StatModifier.ModifyMethod modifyType;
    public float amount;
    [NonSerialized]
    public bool hasBeenOwnerlessProcessed;
    [NonSerialized]
    public bool ignoredForSaveData;
    [HideInInspector]
    public bool isMeatBunBuff;

    public static StatModifier Create(
      PlayerStats.StatType targetStat,
      StatModifier.ModifyMethod method,
      float amt)
    {
      return new StatModifier()
      {
        statToBoost = targetStat,
        amount = amt,
        modifyType = method
      };
    }

    public bool PersistsOnCoopDeath
    {
      get => this.statToBoost == PlayerStats.StatType.Curse && (double) this.amount > 0.0;
    }

    public enum ModifyMethod
    {
      ADDITIVE,
      MULTIPLICATIVE,
    }
  }

