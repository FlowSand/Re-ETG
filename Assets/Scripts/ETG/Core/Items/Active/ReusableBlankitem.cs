// Decompiled with JetBrains decompiler
// Type: ReusableBlankitem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class ReusableBlankitem : PlayerItem
  {
    public GameObject GlassGuonStone;
    public int GlassGuonsToGive = 1;
    public int MaxGlassGuons = 4;

    protected override void DoEffect(PlayerController user)
    {
      user.ForceBlank();
      if (user.HasActiveBonusSynergy(CustomSynergyType.BULLET_KILN))
      {
        int glassGuonsToGive = this.GlassGuonsToGive;
        int num1 = 0;
        PickupObject component = this.GlassGuonStone.GetComponent<PickupObject>();
        for (int index = 0; index < user.passiveItems.Count; ++index)
        {
          if (user.passiveItems[index].PickupObjectId == component.PickupObjectId)
            ++num1;
        }
        int num2 = Mathf.Min(glassGuonsToGive, this.MaxGlassGuons - num1);
        for (int index = 0; index < num2; ++index)
        {
          EncounterTrackable.SuppressNextNotification = true;
          LootEngine.GivePrefabToPlayer(this.GlassGuonStone, user);
          EncounterTrackable.SuppressNextNotification = false;
        }
      }
      base.DoEffect(user);
    }
  }

