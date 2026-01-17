// Decompiled with JetBrains decompiler
// Type: QuadUVSetter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[ExecuteInEditMode]
public class QuadUVSetter : MonoBehaviour
{
  public Vector2 uv0;
  public Vector2 uv1;
  public Vector2 uv2;
  public Vector2 uv3;

  private void OnEnable()
  {
    Mesh sharedMesh = this.GetComponent<MeshFilter>().sharedMesh;
    Vector2[] uv = sharedMesh.uv;
    uv[0] = this.uv0;
    uv[1] = this.uv1;
    uv[2] = this.uv2;
    uv[3] = this.uv3;
    sharedMesh.uv = uv;
    sharedMesh.uv2 = uv;
  }
}
