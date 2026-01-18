using UnityEngine;

#nullable disable

public class Encircler : MonoBehaviour
    {
        private MeshFilter m_filter;
        private Renderer m_renderer;
        private AIActor m_actor;
        private int m_centerUVID;

        private void Start()
        {
            this.m_centerUVID = Shader.PropertyToID("_CenterUV");
            this.m_filter = this.GetComponent<MeshFilter>();
            this.m_renderer = this.GetComponent<Renderer>();
            this.m_actor = this.GetComponent<AIActor>();
            if (!(bool) (Object) this.m_actor || !(bool) (Object) this.m_actor.sprite)
                return;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_actor.sprite, false);
        }

        private void LateUpdate()
        {
            Vector4 zero = Vector4.zero;
            Mesh sharedMesh = this.m_filter.sharedMesh;
            Vector2 rhs1 = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 rhs2 = new Vector2(float.MinValue, float.MinValue);
            for (int index = 0; index < sharedMesh.uv.Length; ++index)
            {
                rhs1 = Vector2.Min(sharedMesh.uv[index], rhs1);
                rhs2 = Vector2.Max(sharedMesh.uv[index], rhs2);
                zero += new Vector4(sharedMesh.uv[index].x, sharedMesh.uv[index].y, 0.0f, 0.0f);
            }
            this.m_renderer.material.SetVector(this.m_centerUVID, (zero / (float) sharedMesh.uv.Length) with
            {
                z = Mathf.Min(rhs2.x - rhs1.x, rhs2.y - rhs1.y),
                w = (float) this.m_renderer.sharedMaterial.mainTexture.width / (float) this.m_renderer.sharedMaterial.mainTexture.height
            });
        }

        private void OnDestroy()
        {
        }
    }

