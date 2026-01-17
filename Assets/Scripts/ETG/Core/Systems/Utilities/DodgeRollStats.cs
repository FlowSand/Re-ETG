// Decompiled with JetBrains decompiler
// Type: DodgeRollStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class DodgeRollStats
    {
      [HideInInspector]
      public bool hasPreDodgeDelay;
      [TogglableProperty("hasPreDodgeDelay", "Pre-Dodge Delay")]
      public float preDodgeDelay;
      public float time;
      public float distance;
      [NonSerialized]
      public int additionalInvulnerabilityFrames;
      [NonSerialized]
      public float blinkDistanceMultiplier = 1f;
      [NonSerialized]
      public float rollTimeMultiplier = 1f;
      [NonSerialized]
      public float rollDistanceMultiplier = 1f;
      [CurveRange(0.0f, 0.0f, 1f, 1f)]
      public AnimationCurve speed;
      private const float c_moveSpeedToRollDistanceConversion = 0.5f;

      public float GetModifiedTime(PlayerController owner)
      {
        float num1 = 1f;
        if ((bool) (UnityEngine.Object) GameManager.Instance.Dungeon && GameManager.Instance.Dungeon.IsEndTimes)
          return this.time;
        if (PassiveItem.IsFlagSetForCharacter(owner, typeof (SunglassesItem)) && SunglassesItem.SunglassesActive)
          num1 *= 0.75f;
        float statModifier = owner.stats.GetStatModifier(PlayerStats.StatType.DodgeRollSpeedMultiplier);
        float num2 = (double) statModifier == 0.0 ? 1f : 1f / statModifier;
        return this.time * this.rollTimeMultiplier * num1 * num2;
      }

      public float GetModifiedDistance(PlayerController owner)
      {
        float num = 1f;
        if (PassiveItem.IsFlagSetForCharacter(owner, typeof (SunglassesItem)) && SunglassesItem.SunglassesActive)
          num *= 1.25f;
        float statModifier = owner.stats.GetStatModifier(PlayerStats.StatType.DodgeRollDistanceMultiplier);
        return (float) ((double) this.distance * (double) this.rollDistanceMultiplier * (((double) owner.stats.GetStatModifier(PlayerStats.StatType.MovementSpeed) - 1.0) * 0.5 + 1.0)) * num * this.blinkDistanceMultiplier * statModifier;
      }
    }

}
