// Decompiled with JetBrains decompiler
// Type: MidGamePassiveItemData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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

