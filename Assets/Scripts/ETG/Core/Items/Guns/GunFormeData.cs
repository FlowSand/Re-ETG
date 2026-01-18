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

