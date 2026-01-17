// Decompiled with JetBrains decompiler
// Type: Dungeonator.CellDamageDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public struct CellDamageDefinition
  {
    public CoreDamageTypes damageTypes;
    public float damageToPlayersPerTick;
    public float damageToEnemiesPerTick;
    public float tickFrequency;
    public bool respectsFlying;
    public bool isPoison;

    public bool HasChanges()
    {
      return this.damageTypes != CoreDamageTypes.None || (double) this.damageToPlayersPerTick != 0.0 || (double) this.damageToEnemiesPerTick != 0.0 || (double) this.tickFrequency != 0.0 || this.respectsFlying || this.isPoison;
    }
  }
}
