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
