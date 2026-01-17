// Decompiled with JetBrains decompiler
// Type: WeightedRoomCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [Serializable]
    public class WeightedRoomCollection
    {
      [TrimElementTags]
      public List<WeightedRoom> elements;

      public WeightedRoomCollection() => this.elements = new List<WeightedRoom>();

      public void Add(WeightedRoom w) => this.elements.Add(w);

      public WeightedRoom SelectByWeight()
      {
        List<WeightedRoom> weightedRoomList = new List<WeightedRoom>();
        float num1 = 0.0f;
        for (int index1 = 0; index1 < this.elements.Count; ++index1)
        {
          WeightedRoom element = this.elements[index1];
          bool flag = true;
          for (int index2 = 0; index2 < element.additionalPrerequisites.Length; ++index2)
          {
            if (!element.additionalPrerequisites[index2].CheckConditionsFulfilled())
            {
              flag = false;
              break;
            }
          }
          if (!((UnityEngine.Object) element.room != (UnityEngine.Object) null) || element.room.CheckPrerequisites())
          {
            if (flag)
            {
              weightedRoomList.Add(element);
              num1 += element.weight;
            }
          }
        }
        if (weightedRoomList.Count == 0)
          return (WeightedRoom) null;
        float num2 = BraveRandom.GenerationRandomValue() * num1;
        float num3 = 0.0f;
        for (int index = 0; index < weightedRoomList.Count; ++index)
        {
          num3 += weightedRoomList[index].weight;
          if ((double) num3 > (double) num2)
            return weightedRoomList[index];
        }
        return weightedRoomList[weightedRoomList.Count - 1];
      }

      public WeightedRoom SelectByWeightWithoutDuplicates(List<PrototypeDungeonRoom> extant)
      {
        List<WeightedRoom> weightedRoomList = new List<WeightedRoom>();
        float num1 = 0.0f;
        for (int index1 = 0; index1 < this.elements.Count; ++index1)
        {
          WeightedRoom element = this.elements[index1];
          if (!extant.Contains(element.room))
          {
            bool flag = true;
            for (int index2 = 0; index2 < element.additionalPrerequisites.Length; ++index2)
            {
              if (!element.additionalPrerequisites[index2].CheckConditionsFulfilled())
              {
                flag = false;
                break;
              }
            }
            if (flag)
            {
              weightedRoomList.Add(element);
              num1 += element.weight;
            }
          }
        }
        float num2 = BraveRandom.GenerationRandomValue() * num1;
        float num3 = 0.0f;
        for (int index = 0; index < weightedRoomList.Count; ++index)
        {
          num3 += weightedRoomList[index].weight;
          if ((double) num3 > (double) num2)
            return weightedRoomList[index];
        }
        return weightedRoomList[weightedRoomList.Count - 1];
      }
    }

}
