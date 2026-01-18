// Decompiled with JetBrains decompiler
// Type: ProceduralChestContents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class ProceduralChestContents : ScriptableObject
  {
    [BetterList]
    public List<ProceduralChestItem> items;

    public PickupObject GetItem(float val)
    {
      float num1 = 0.0f;
      for (int index = 0; index < this.items.Count; ++index)
        num1 += this.items[index].chance;
      float num2 = 0.0f;
      for (int index = 0; index < this.items.Count; ++index)
      {
        num2 += this.items[index].chance;
        if ((double) num2 / (double) num1 > (double) val)
          return this.items[index].item;
      }
      return this.items[this.items.Count - 1].item;
    }
  }

