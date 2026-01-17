// Decompiled with JetBrains decompiler
// Type: HealingReceivedModificationItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class HealingReceivedModificationItem : PassiveItem
    {
      public float ChanceToImproveHealing = 0.5f;
      public float HealingImprovedBy = 0.5f;
      public GameObject OnImprovedHealingVFX;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        player.healthHaver.ModifyHealing += new Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>(this.ModifyIncomingHealing);
        base.Pickup(player);
      }

      private void ModifyIncomingHealing(HealthHaver source, HealthHaver.ModifyHealingEventArgs args)
      {
        if (args == EventArgs.Empty || (double) UnityEngine.Random.value >= (double) this.ChanceToImproveHealing)
          return;
        if ((UnityEngine.Object) this.OnImprovedHealingVFX != (UnityEngine.Object) null)
          source.GetComponent<PlayerController>().PlayEffectOnActor(this.OnImprovedHealingVFX, Vector3.zero);
        args.ModifiedHealing += this.HealingImprovedBy;
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        player.healthHaver.ModifyHealing -= new Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>(this.ModifyIncomingHealing);
        debrisObject.GetComponent<HealingReceivedModificationItem>().m_pickedUpThisRun = true;
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        if (this.m_pickedUp)
          this.m_owner.healthHaver.ModifyHealing += new Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>(this.ModifyIncomingHealing);
        base.OnDestroy();
      }
    }

}
