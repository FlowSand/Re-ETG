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

