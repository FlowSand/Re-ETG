// Decompiled with JetBrains decompiler
// Type: RingOfPitFriendship
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class RingOfPitFriendship : PassiveItem
    {
      private string boolKey = string.Empty;
      private PlayerController m_currentOwner;

      private void Awake() => this.boolKey = "ringPitFriend" + Guid.NewGuid().ToString();

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        base.Pickup(player);
        this.m_currentOwner = player;
        player.ImmuneToPits.SetOverride(this.boolKey, true);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        debrisObject.GetComponent<RingOfPitFriendship>().m_pickedUpThisRun = true;
        player.ImmuneToPits.SetOverride(this.boolKey, false);
        this.m_currentOwner = (PlayerController) null;
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        if ((UnityEngine.Object) this.m_currentOwner != (UnityEngine.Object) null)
          this.m_currentOwner.ImmuneToPits.SetOverride(this.boolKey, false);
        base.OnDestroy();
      }
    }

}
