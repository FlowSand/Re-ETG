using System.Collections.Generic;

#nullable disable

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

