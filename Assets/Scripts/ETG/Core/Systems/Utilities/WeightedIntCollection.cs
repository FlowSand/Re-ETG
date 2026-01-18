// Decompiled with JetBrains decompiler
// Type: WeightedIntCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

[Serializable]
public class WeightedIntCollection
  {
    public WeightedInt[] elements;

    public int SelectByWeight(System.Random generatorRandom)
    {
      List<WeightedInt> weightedIntList = new List<WeightedInt>();
      float num1 = 0.0f;
      for (int index1 = 0; index1 < this.elements.Length; ++index1)
      {
        WeightedInt element = this.elements[index1];
        if ((double) element.weight > 0.0)
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
            weightedIntList.Add(element);
            num1 += element.weight;
          }
        }
      }
      float num2 = (generatorRandom == null ? UnityEngine.Random.value : (float) generatorRandom.NextDouble()) * num1;
      float num3 = 0.0f;
      for (int index = 0; index < weightedIntList.Count; ++index)
      {
        num3 += weightedIntList[index].weight;
        if ((double) num3 > (double) num2)
          return weightedIntList[index].value;
      }
      return weightedIntList[0].value;
    }

    public int SelectByWeight() => this.SelectByWeight((System.Random) null);
  }

