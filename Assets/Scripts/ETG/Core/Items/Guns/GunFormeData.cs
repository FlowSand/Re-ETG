// Decompiled with JetBrains decompiler
// Type: GunFormeData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

[Serializable]
public class GunFormeData
  {
    public bool RequiresSynergy = true;
    [LongNumericEnum]
    [ShowInInspectorIf("RequiresSynergy", false)]
    public CustomSynergyType RequiredSynergy;
    [PickupIdentifier]
    public int FormeID;

    public bool IsValid(PlayerController p)
    {
      return !this.RequiresSynergy || p.HasActiveBonusSynergy(this.RequiredSynergy);
    }
  }

