using System.Collections.Generic;

#nullable disable

public class MidGamePassiveItemData
    {
        public int PickupID = -1;
        public List<object> SerializedData;

        public MidGamePassiveItemData(PassiveItem p)
        {
            this.PickupID = p.PickupObjectId;
            this.SerializedData = new List<object>();
            p.MidGameSerialize(this.SerializedData);
        }
    }

