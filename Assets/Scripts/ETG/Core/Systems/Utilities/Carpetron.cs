using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class Carpetron
  {
    public static HashSet<IntVector2> PostprocessFullRoom(HashSet<IntVector2> set)
    {
      HashSet<IntVector2> intVector2Set = new HashSet<IntVector2>();
      IntVector2[] cardinalsAndOrdinals = IntVector2.CardinalsAndOrdinals;
      foreach (IntVector2 intVector2 in set)
      {
        bool flag = false;
        for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
        {
          if (!set.Contains(intVector2 + cardinalsAndOrdinals[index]))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          intVector2Set.Add(intVector2);
          for (int index = 0; index < cardinalsAndOrdinals.Length; ++index)
            intVector2Set.Add(intVector2 + cardinalsAndOrdinals[index]);
        }
      }
      return intVector2Set;
    }

    public static Tuple<IntVector2, IntVector2> PostprocessSubmatrix(
      Tuple<IntVector2, IntVector2> rect,
      out Tuple<IntVector2, IntVector2> bonusRect)
    {
      bonusRect = (Tuple<IntVector2, IntVector2>) null;
      IntVector2 intVector2 = rect.Second - rect.First;
      IntVector2 first1 = rect.First;
      IntVector2 second1 = rect.Second;
      if (intVector2.x > 12 && intVector2.y > 12)
      {
        if ((double) UnityEngine.Random.value < 1.3999999761581421)
        {
          int num1 = intVector2.x / 3;
          int num2 = intVector2.y / 3;
          first1.x += num1;
          second1.x -= num1;
          IntVector2 first2 = rect.First;
          IntVector2 second2 = rect.Second;
          first2.y += num2;
          second2.y -= num2;
          bonusRect = new Tuple<IntVector2, IntVector2>(first2, second2);
        }
        else if (intVector2.x > intVector2.y)
        {
          for (; intVector2.y > 4 && (double) UnityEngine.Random.value > 0.30000001192092896; --second1.y)
          {
            intVector2.y -= 2;
            ++first1.y;
          }
        }
        else
        {
          for (; intVector2.x > 4 && (double) UnityEngine.Random.value > 0.30000001192092896; --second1.x)
          {
            intVector2.x -= 2;
            ++first1.x;
          }
        }
      }
      else if (intVector2.x > intVector2.y && intVector2.x > 12)
      {
        for (; intVector2.x > 12 && (double) UnityEngine.Random.value > 0.30000001192092896; --second1.x)
        {
          intVector2.x -= 2;
          ++first1.x;
        }
      }
      else if (intVector2.y > intVector2.x && intVector2.y > 12)
      {
        for (; intVector2.y > 12 && (double) UnityEngine.Random.value > 0.30000001192092896; --second1.y)
        {
          intVector2.y -= 2;
          ++first1.y;
        }
      }
      return new Tuple<IntVector2, IntVector2>(first1, second1);
    }

    public static Tuple<IntVector2, IntVector2> RawMaxSubmatrix(
      CellData[][] matrix,
      IntVector2 basePosition,
      IntVector2 dimensions,
      Func<CellData, bool> isInvalidFunction)
    {
      List<IntRect> intRectList = new List<IntRect>();
      int y = dimensions.y;
      int x = dimensions.x;
      int num1 = -1;
      int[] numArray1 = new int[x];
      for (int index = 0; index < x; ++index)
        numArray1[index] = -1;
      int[] numArray2 = new int[x];
      int[] numArray3 = new int[x];
      Stack<int> intStack = new Stack<int>();
      for (int index1 = 0; index1 < y; ++index1)
      {
        for (int index2 = 0; index2 < x; ++index2)
        {
          CellData cellData = matrix[basePosition.x + index2][basePosition.y + index1];
          if (isInvalidFunction(cellData))
            numArray1[index2] = index1;
        }
        intStack.Clear();
        for (int index3 = 0; index3 < x; ++index3)
        {
          while (intStack.Count > 0 && numArray1[intStack.Peek()] <= numArray1[index3])
            intStack.Pop();
          numArray2[index3] = intStack.Count != 0 ? intStack.Peek() : -1;
          intStack.Push(index3);
        }
        intStack.Clear();
        for (int index4 = x - 1; index4 >= 0; --index4)
        {
          while (intStack.Count > 0 && numArray1[intStack.Peek()] <= numArray1[index4])
            intStack.Pop();
          numArray3[index4] = intStack.Count != 0 ? intStack.Peek() : x;
          intStack.Push(index4);
        }
        for (int index5 = 0; index5 < x; ++index5)
        {
          int num2 = (index1 - numArray1[index5]) * (numArray3[index5] - numArray2[index5] - 1);
          if (num2 > num1)
          {
            num1 = num2;
            int left = numArray2[index5] + 1;
            int bottom = numArray1[index5] + 1;
            int num3 = numArray3[index5] - 1;
            int num4 = index1;
            intRectList.Add(new IntRect(left, bottom, num3 - left, num4 - bottom));
          }
        }
      }
      IntVector2 dimensions1 = intRectList[intRectList.Count - 1].Dimensions;
      for (int index = intRectList.Count - 2; index >= 0; --index)
      {
        if (intRectList[index].Dimensions != dimensions1)
        {
          intRectList.RemoveAt(index);
          ++index;
        }
      }
      int index6 = Mathf.FloorToInt((float) intRectList.Count / 2f);
      return new Tuple<IntVector2, IntVector2>(new IntVector2(intRectList[index6].Left, intRectList[index6].Bottom), new IntVector2(intRectList[index6].Right, intRectList[index6].Top));
    }

    public static Tuple<IntVector2, IntVector2> MaxSubmatrix(
      CellData[][] matrix,
      IntVector2 basePosition,
      IntVector2 dimensions,
      bool includePits = false,
      bool includeOverrideFloors = false,
      bool includeWallNeighbors = false,
      int visualSubtype = -1)
    {
      DungeonData data = GameManager.Instance.Dungeon.data;
      List<IntRect> intRectList = new List<IntRect>();
      int y = dimensions.y;
      int x = dimensions.x;
      int num1 = -1;
      int[] numArray1 = new int[x];
      for (int index = 0; index < x; ++index)
        numArray1[index] = -1;
      int[] numArray2 = new int[x];
      int[] numArray3 = new int[x];
      Stack<int> intStack = new Stack<int>();
      for (int index1 = 0; index1 < y; ++index1)
      {
        for (int index2 = 0; index2 < x; ++index2)
        {
          CellData cellData = matrix[basePosition.x + index2][basePosition.y + index1];
          if (cellData == null)
          {
            numArray1[index2] = index1;
          }
          else
          {
            bool flag = !includeWallNeighbors && cellData.HasWallNeighbor(includeTopwalls: false) || !includePits && cellData.HasPitNeighbor(data);
            if (cellData.type == CellType.WALL || cellData.cellVisualData.floorType == CellVisualData.CellFloorType.Ice || cellData.cellVisualData.pathTilesetGridIndex > -1 || !includeOverrideFloors && cellData.doesDamage || !includePits && cellData.type == CellType.PIT || flag || !includeOverrideFloors && cellData.cellVisualData.floorTileOverridden || !includeOverrideFloors && cellData.HasPhantomCarpetNeighbor() || visualSubtype > -1 && cellData.cellVisualData.roomVisualTypeIndex != visualSubtype)
              numArray1[index2] = index1;
          }
        }
        intStack.Clear();
        for (int index3 = 0; index3 < x; ++index3)
        {
          while (intStack.Count > 0 && numArray1[intStack.Peek()] <= numArray1[index3])
            intStack.Pop();
          numArray2[index3] = intStack.Count != 0 ? intStack.Peek() : -1;
          intStack.Push(index3);
        }
        intStack.Clear();
        for (int index4 = x - 1; index4 >= 0; --index4)
        {
          while (intStack.Count > 0 && numArray1[intStack.Peek()] <= numArray1[index4])
            intStack.Pop();
          numArray3[index4] = intStack.Count != 0 ? intStack.Peek() : x;
          intStack.Push(index4);
        }
        for (int index5 = 0; index5 < x; ++index5)
        {
          int num2 = (index1 - numArray1[index5]) * (numArray3[index5] - numArray2[index5] - 1);
          if (num2 > num1)
          {
            num1 = num2;
            int left = numArray2[index5] + 1;
            int bottom = numArray1[index5] + 1;
            int num3 = numArray3[index5] - 1;
            int num4 = index1;
            intRectList.Add(new IntRect(left, bottom, num3 - left, num4 - bottom));
          }
        }
      }
      IntVector2 dimensions1 = intRectList[intRectList.Count - 1].Dimensions;
      for (int index = intRectList.Count - 2; index >= 0; --index)
      {
        if (intRectList[index].Dimensions != dimensions1)
        {
          intRectList.RemoveAt(index);
          ++index;
        }
      }
      int index6 = Mathf.FloorToInt((float) intRectList.Count / 2f);
      return new Tuple<IntVector2, IntVector2>(new IntVector2(intRectList[index6].Left, intRectList[index6].Bottom), new IntVector2(intRectList[index6].Right, intRectList[index6].Top));
    }
  }

