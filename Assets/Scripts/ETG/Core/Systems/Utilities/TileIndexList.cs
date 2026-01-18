using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable

[Serializable]
public class TileIndexList
  {
    [SerializeField]
    public List<int> indices;
    [SerializeField]
    public List<float> indexWeights;

    public TileIndexList()
    {
      this.indices = new List<int>();
      this.indexWeights = new List<float>();
    }

    public int GetIndexOfIndex(int index)
    {
      for (int index1 = 0; index1 < this.indices.Count; ++index1)
      {
        if (this.indices[index1] == index)
          return index1;
      }
      return -1;
    }

    public int GetIndexByWeight()
    {
      float num1 = this.indexWeights.Sum() * UnityEngine.Random.value;
      float num2 = 0.0f;
      for (int index = 0; index < this.indices.Count; ++index)
      {
        num2 += this.indexWeights[index];
        if ((double) num2 >= (double) num1)
          return this.indices[index];
      }
      return this.indices.Count == 0 ? -1 : this.indices[this.indices.Count - 1];
    }

    public bool ContainsValid()
    {
      for (int index = 0; index < this.indices.Count; ++index)
      {
        if (this.indices[index] != -1)
          return true;
      }
      return false;
    }
  }

