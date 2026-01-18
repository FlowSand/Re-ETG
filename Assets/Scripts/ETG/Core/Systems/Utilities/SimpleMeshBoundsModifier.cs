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

