// Decompiled with JetBrains decompiler
// Type: SpecialKeyItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class SpecialKeyItem : PassiveItem
  {
    public SpecialKeyItem.SpecialKeyType keyType;

    protected override void OnDestroy() => base.OnDestroy();

    public override void Pickup(PlayerController player)
    {
      base.Pickup(player);
      GameUIRoot.Instance.UpdatePlayerConsumables(player.carriedConsumables);
    }

    public enum SpecialKeyType
    {
      RESOURCEFUL_RAT_LAIR,
    }
  }

