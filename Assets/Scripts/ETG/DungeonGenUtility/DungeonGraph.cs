// Decompiled with JetBrains decompiler
// Type: DungeonGenUtility.DungeonGraph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace DungeonGenUtility
{
  public class DungeonGraph
  {
    public List<Vector2> vertices;
    public List<Edge> edges;

    public DungeonGraph()
    {
      this.vertices = new List<Vector2>();
      this.edges = new List<Edge>();
    }
  }
}
