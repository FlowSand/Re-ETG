// Decompiled with JetBrains decompiler
// Type: PlayerOrbitalSynergyData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public struct PlayerOrbitalSynergyData
  {
    [LongNumericEnum]
    public CustomSynergyType SynergyToCheck;
    public bool EngagesFiring;
    public float ShootCooldownMultiplier;
    public int AdditionalShots;
    public Projectile OverrideProjectile;
    public bool HasOverrideAnimations;
    public string OverrideIdleAnimation;
  }

