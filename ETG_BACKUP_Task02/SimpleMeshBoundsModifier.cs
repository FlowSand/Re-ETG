// Decompiled with JetBrains decompiler
// Type: SimpleMeshBoundsModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SimpleMeshBoundsModifier : MonoBehaviour
{
  public Vector3 expansionVector = new Vector3(0.0f, 20f, 0.0f);

  private void Start()
  {
    MeshFilter component = this.GetComponent<MeshFilter>();
    Bounds bounds = component.sharedMesh.bounds;
    bounds.Expand(this.expansionVector);
    component.mesh.bounds = bounds;
  }
}
