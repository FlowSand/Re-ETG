// Decompiled with JetBrains decompiler
// Type: CogOfBattleItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class CogOfBattleItem : PassiveItem
    {
      public static float ACTIVE_RELOAD_DAMAGE_MULTIPLIER = 1.25f;
      public float DamageMultiplier = 1.25f;
      private PlayerController m_localOwner;

      private void Awake() => CogOfBattleItem.ACTIVE_RELOAD_DAMAGE_MULTIPLIER = this.DamageMultiplier;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_localOwner = player;
        if (player.IsPrimaryPlayer)
          Gun.ActiveReloadActivated = true;
        else
          Gun.ActiveReloadActivatedPlayerTwo = true;
        base.Pickup(player);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        this.m_localOwner = (PlayerController) null;
        if (player.IsPrimaryPlayer)
          Gun.ActiveReloadActivated = false;
        else
          Gun.ActiveReloadActivatedPlayerTwo = false;
        debrisObject.GetComponent<CogOfBattleItem>().m_pickedUpThisRun = true;
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        if ((Object) this.m_localOwner != (Object) null)
        {
          if (this.m_localOwner.IsPrimaryPlayer)
            Gun.ActiveReloadActivated = false;
          else
            Gun.ActiveReloadActivatedPlayerTwo = false;
        }
        base.OnDestroy();
      }
    }

}
