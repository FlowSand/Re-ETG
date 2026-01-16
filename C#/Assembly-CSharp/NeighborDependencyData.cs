// Decompiled with JetBrains decompiler
// Type: NeighborDependencyData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
