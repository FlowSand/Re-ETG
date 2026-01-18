// Decompiled with JetBrains decompiler
// Type: CharacterSelectIdlePhase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[Serializable]
public class CharacterSelectIdlePhase
  {
    public float holdMin = 4f;
    public float holdMax = 10f;
    public string inAnimation = string.Empty;
    public string holdAnimation = string.Empty;
    public float optionalHoldChance = 0.5f;
    public string optionalHoldIdleAnimation = string.Empty;
    public string outAnimation = string.Empty;
    [Header("Optional VFX")]
    public CharacterSelectIdlePhase.VFXPhaseTrigger vfxTrigger;
    public float vfxHoldPeriod = 1f;
    public tk2dSpriteAnimator vfxSpriteAnimator;
    public tk2dSpriteAnimator endVFXSpriteAnimator;

    public enum VFXPhaseTrigger
    {
      NONE,
      IN,
      HOLD,
      OUT,
    }
  }

