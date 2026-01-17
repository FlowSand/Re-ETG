// Decompiled with JetBrains decompiler
// Type: DungeonGenUtility.Edge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace DungeonGenUtility
{
  public class Edge
  {
    private Vector2 v0;
    private Vector2 v1;

    public Edge(Vector2 vert0, Vector2 vert1)
    {
      this.v0 = vert0;
      this.v1 = vert1;
    }

    private float Length => Vector2.Distance(this.v0, this.v1);
  }
}
