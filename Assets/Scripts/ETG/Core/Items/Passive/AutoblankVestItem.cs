// Decompiled with JetBrains decompiler
// Type: AutoblankVestItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

public class AutoblankVestItem : PassiveItem
  {
    [PickupIdentifier]
    public int ElderBlankID;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      player.healthHaver.ModifyDamage += new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.HandleEffect);
    }

    private bool HasElderBlank() => this.m_owner.HasActiveItem(this.ElderBlankID);

    private void HandleEffect(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
    {
      if (args == EventArgs.Empty || (double) args.ModifiedDamage <= 0.0 || !source.IsVulnerable)
        return;
      if ((bool) (UnityEngine.Object) this.m_owner && this.HasElderBlank())
      {
        for (int index = 0; index < this.m_owner.activeItems.Count; ++index)
        {
          if (this.m_owner.activeItems[index].PickupObjectId == this.ElderBlankID && !this.m_owner.activeItems[index].IsOnCooldown)
          {
            source.TriggerInvulnerabilityPeriod();
            this.m_owner.ForceBlank();
            this.m_owner.activeItems[index].ForceApplyCooldown(this.m_owner);
            args.ModifiedDamage = 0.0f;
            return;
          }
        }
      }
      if (!(bool) (UnityEngine.Object) this.m_owner || this.m_owner.Blanks <= 0 || this.m_owner.IsFalling)
        return;
      source.TriggerInvulnerabilityPeriod();
      this.m_owner.ForceConsumableBlank();
      args.ModifiedDamage = 0.0f;
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      AutoblankVestItem component = debrisObject.GetComponent<AutoblankVestItem>();
      player.healthHaver.ModifyDamage -= new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.HandleEffect);
      component.m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.m_owner)
        this.m_owner.healthHaver.ModifyDamage -= new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.HandleEffect);
      base.OnDestroy();
    }
  }

