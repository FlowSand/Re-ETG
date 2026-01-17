// Decompiled with JetBrains decompiler
// Type: StatModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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

}
