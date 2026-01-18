using System.Collections.Generic;

#nullable disable

public class MidGameGunData
    {
        public int PickupID = -1;
        public int CurrentAmmo = -1;
        public List<object> SerializedData;
        public List<int> DuctTapedGunIDs;

        public MidGameGunData(Gun g)
        {
            this.PickupID = g.PickupObjectId;
            this.CurrentAmmo = g.CurrentAmmo;
            this.SerializedData = new List<object>();
            g.MidGameSerialize(this.SerializedData);
            this.DuctTapedGunIDs = new List<int>();
            if (g.DuctTapeMergedGunIDs == null)
                return;
            this.DuctTapedGunIDs.AddRange((IEnumerable<int>) g.DuctTapeMergedGunIDs);
        }
    }

