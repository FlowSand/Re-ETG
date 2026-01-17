// Decompiled with JetBrains decompiler
// Type: AmmoThresholdTransformation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Core.Enums
{
    [Serializable]
    public struct AmmoThresholdTransformation
    {
      public float AmmoPercentage;
      [PickupIdentifier]
      public int TargetGunID;
      public bool HasSynergyChange;
      [LongNumericEnum]
      public CustomSynergyType RequiredSynergy;
      public float SynergyAmmoPercentage;

      public float GetAmmoPercentage()
      {
        int count = -1;
        return this.HasSynergyChange && PlayerController.AnyoneHasActiveBonusSynergy(this.RequiredSynergy, out count) ? this.SynergyAmmoPercentage : this.AmmoPercentage;
      }
    }

}
