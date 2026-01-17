// Decompiled with JetBrains decompiler
// Type: MidGameActiveItemData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Items.Guns
{
    public class MidGameActiveItemData
    {
      public int PickupID = -1;
      public bool IsOnCooldown;
      public float DamageCooldown;
      public int RoomCooldown;
      public float TimeCooldown;
      public int NumberOfUses;
      public List<object> SerializedData;

      public MidGameActiveItemData(PlayerItem p)
      {
        this.PickupID = p.PickupObjectId;
        this.IsOnCooldown = p.IsOnCooldown;
        this.DamageCooldown = p.CurrentDamageCooldown;
        this.RoomCooldown = p.CurrentRoomCooldown;
        this.TimeCooldown = p.CurrentTimeCooldown;
        this.NumberOfUses = p.numberOfUses;
        this.SerializedData = new List<object>();
        p.MidGameSerialize(this.SerializedData);
      }
    }

}
