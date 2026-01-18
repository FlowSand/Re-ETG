using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class WeightedList<T>
  {
    public List<WeightedItem<T>> elements;

    public void Add(T item, float weight)
    {
      if (this.elements == null)
        this.elements = new List<WeightedItem<T>>();
      this.elements.Add(new WeightedItem<T>(item, weight));
    }

    public T SelectByWeight()
    {
      if (this.elements == null || this.elements.Count == 0)
        return default (T);
      float num1 = 0.0f;
      for (int index = 0; index < this.elements.Count; ++index)
        num1 += this.elements[index].weight;
      float num2 = Random.value * num1;
      float num3 = 0.0f;
      for (int index = 0; index < this.elements.Count; ++index)
      {
        num3 += this.elements[index].weight;
        if ((double) num3 > (double) num2)
          return this.elements[index].value;
      }
      return this.elements[this.elements.Count - 1].value;
    }
  }

