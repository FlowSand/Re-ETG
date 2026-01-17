// Decompiled with JetBrains decompiler
// Type: dfVirtualScrollData`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfVirtualScrollData<T>
    {
      public IList<T> BackingList;
      public List<IDFVirtualScrollingTile> Tiles = new List<IDFVirtualScrollingTile>();
      public RectOffset ItemPadding;
      public Vector2 LastScrollPosition = Vector2.zero;
      public int MaxExtraOffscreenTiles = 10;
      public IDFVirtualScrollingTile DummyTop;
      public IDFVirtualScrollingTile DummyBottom;
      public bool IsPaging;
      public bool IsInitialized;

      public void GetNewLimits(bool isVerticalFlow, bool getMaxes, out int index, out float newY)
      {
        IDFVirtualScrollingTile tile1 = this.Tiles[0];
        index = tile1.VirtualScrollItemIndex;
        newY = !isVerticalFlow ? tile1.GetDfPanel().RelativePosition.x : tile1.GetDfPanel().RelativePosition.y;
        foreach (IDFVirtualScrollingTile tile2 in this.Tiles)
        {
          dfPanel dfPanel = tile2.GetDfPanel();
          float num = !isVerticalFlow ? dfPanel.RelativePosition.x : dfPanel.RelativePosition.y;
          if (getMaxes)
          {
            if ((double) num > (double) newY)
              newY = num;
            if (tile2.VirtualScrollItemIndex > index)
              index = tile2.VirtualScrollItemIndex;
          }
          else
          {
            if ((double) num < (double) newY)
              newY = num;
            if (tile2.VirtualScrollItemIndex < index)
              index = tile2.VirtualScrollItemIndex;
          }
        }
        if (getMaxes)
          ++index;
        else
          --index;
      }
    }

}
