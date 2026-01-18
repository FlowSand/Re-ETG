// Decompiled with JetBrains decompiler
// Type: DamageTypeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public class DamageTypeModifier
  {
    public CoreDamageTypes damageType;
    public float damageMultiplier = 1f;

    public DamageTypeModifier()
    {
    }

    public DamageTypeModifier(DamageTypeModifier other)
    {
      this.damageType = other.damageType;
      this.damageMultiplier = other.damageMultiplier;
    }
  }

