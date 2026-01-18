using System;
using System.Collections.Generic;

#nullable disable

[Serializable]
public class NeighborDependencyData
  {
    public List<IndexNeighborDependency> neighborDependencies;

    public NeighborDependencyData(List<IndexNeighborDependency> bcs)
    {
      this.neighborDependencies = bcs;
    }
  }

