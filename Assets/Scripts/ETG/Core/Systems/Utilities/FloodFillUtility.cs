// Decompiled with JetBrains decompiler
// Type: FloodFillUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class FloodFillUtility
    {
      private static List<int> s_openList = new List<int>();
      private static bool[] s_reachable = new bool[0];
      private static IntVector2 m_areaMin;
      private static IntVector2 m_areaDim;

      public static void PreprocessContiguousCells(RoomHandler room, IntVector2 myPos, int bufferCells = 0)
      {
        DungeonData data = GameManager.Instance.Dungeon.data;
        FloodFillUtility.m_areaMin = room.area.basePosition - new IntVector2(bufferCells, bufferCells);
        FloodFillUtility.m_areaDim = room.area.dimensions + new IntVector2(2 * bufferCells, 2 * bufferCells);
        int length = FloodFillUtility.m_areaDim.x * FloodFillUtility.m_areaDim.y;
        if (FloodFillUtility.s_reachable.Length < length)
          FloodFillUtility.s_reachable = new bool[length];
        for (int index = 0; index < length; ++index)
          FloodFillUtility.s_reachable[index] = false;
        FloodFillUtility.s_openList.Clear();
        if (data.GetCellTypeSafe(myPos) == CellType.FLOOR)
        {
          int index = myPos.x - FloodFillUtility.m_areaMin.x + (myPos.y - FloodFillUtility.m_areaMin.y) * FloodFillUtility.m_areaDim.x;
          FloodFillUtility.s_openList.Add(index);
          FloodFillUtility.s_reachable[index] = true;
        }
        for (int index = 0; FloodFillUtility.s_openList.Count > 0 && index < 1000; ++index)
        {
          int open = FloodFillUtility.s_openList[0];
          int num1 = FloodFillUtility.s_openList[0] % FloodFillUtility.m_areaDim.x;
          int num2 = FloodFillUtility.s_openList[0] / FloodFillUtility.m_areaDim.x;
          int x1 = FloodFillUtility.m_areaMin.x + num1;
          int y = FloodFillUtility.m_areaMin.y + num2;
          FloodFillUtility.s_openList.RemoveAt(0);
          int num3 = -1;
          if (num1 > 0 && data.GetCellTypeSafe(x1 - 1, y) == CellType.FLOOR && !FloodFillUtility.s_reachable[open + num3])
          {
            FloodFillUtility.s_reachable[open + num3] = true;
            FloodFillUtility.s_openList.Add(open + num3);
          }
          int num4 = 1;
          if (num1 < FloodFillUtility.m_areaDim.x - 1 && data.GetCellTypeSafe(x1 + 1, y) == CellType.FLOOR && !FloodFillUtility.s_reachable[open + num4])
          {
            FloodFillUtility.s_reachable[open + num4] = true;
            FloodFillUtility.s_openList.Add(open + num4);
          }
          int num5 = -FloodFillUtility.m_areaDim.x;
          if (num2 > 0 && data.GetCellTypeSafe(x1, y - 1) == CellType.FLOOR && !FloodFillUtility.s_reachable[open + num5])
          {
            FloodFillUtility.s_reachable[open + num5] = true;
            FloodFillUtility.s_openList.Add(open + num5);
          }
          int x2 = FloodFillUtility.m_areaDim.x;
          if (num2 < FloodFillUtility.m_areaDim.y - 1 && data.GetCellTypeSafe(x1, y + 1) == CellType.FLOOR && !FloodFillUtility.s_reachable[open + x2])
          {
            FloodFillUtility.s_reachable[open + x2] = true;
            FloodFillUtility.s_openList.Add(open + x2);
          }
        }
      }

      public static bool WasFilled(IntVector2 c)
      {
        return FloodFillUtility.s_reachable[c.x - FloodFillUtility.m_areaMin.x + (c.y - FloodFillUtility.m_areaMin.y) * FloodFillUtility.m_areaDim.x];
      }
    }

}
