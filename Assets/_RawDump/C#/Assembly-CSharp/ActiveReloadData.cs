// Decompiled with JetBrains decompiler
// Type: ActiveReloadData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[Serializable]
public class ActiveReloadData
{
  public float damageMultiply = 1f;
  public float knockbackMultiply = 1f;
  public bool usesOverrideAngleVariance;
  public float overrideAngleVariance;
  public float reloadSpeedMultiplier = 1f;
  public bool ActiveReloadStacks;
  public bool ActiveReloadIncrementsTier;
  public int MaxTier = 5;
}
