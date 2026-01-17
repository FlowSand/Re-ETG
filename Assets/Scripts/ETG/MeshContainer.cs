// Decompiled with JetBrains decompiler
// Type: MeshContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MeshContainer
{
  public Mesh mesh;
  public Vector3[] vertices;
  public Vector3[] normals;

  public MeshContainer(Mesh m)
  {
    this.mesh = m;
    this.vertices = m.vertices;
    this.normals = m.normals;
  }

  public void Update()
  {
    this.mesh.vertices = this.vertices;
    this.mesh.normals = this.normals;
  }
}
