// Decompiled with JetBrains decompiler
// Type: MetaShopTier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [Serializable]
    public class MetaShopTier
    {
      public int overrideTierCost = -1;
      [PickupIdentifier]
      public int itemId1 = -1;
      public int overrideItem1Cost = -1;
      [PickupIdentifier]
      public int itemId2 = -1;
      public int overrideItem2Cost = -1;
      [PickupIdentifier]
      public int itemId3 = -1;
      public int overrideItem3Cost = -1;
    }

}
