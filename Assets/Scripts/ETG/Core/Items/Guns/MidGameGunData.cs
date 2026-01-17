// Decompiled with JetBrains decompiler
// Type: MidGameGunData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Items.Guns
{
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

}
