// Decompiled with JetBrains decompiler
// Type: TimedSynergyStatBuff
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Combat.Effects
{
    [Serializable]
    public class TimedSynergyStatBuff
    {
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public PlayerStats.StatType statToBoost;
      public StatModifier.ModifyMethod modifyType;
      public float amount;
      public float duration;
    }

}
