using UnityEngine;

#nullable disable

[RequireComponent(typeof (MeshRenderer))]
[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/UI/Core/tk2dUIMask")]
[RequireComponent(typeof (MeshFilter))]
public class tk2dUIMask : MonoBehaviour
    {
        public tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.MiddleCenter;
        public Vector2 size = new Vector2(1f, 1f);
        public float depth = 1f;
        public bool createBoxCollider = true;
        private MeshFilter _thisMeshFilter;
        private BoxCollider _thisBoxCollider;
        private static readonly Vector2[] uv = new Vector2[4]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(1f, 0.0f),
            new Vector2(0.0f, 1f),
            new Vector2(1f, 1f)
        };
        private static readonly int[] indices = new int[6]
        {
            0,
            3,
            1,
            2,
            3,
            0
        };

        private MeshFilter ThisMeshFilter
        {
            get
            {
                if ((Object) this._thisMeshFilter == (Object) null)
                    this._thisMeshFilter = this.GetComponent<MeshFilter>();
                return this._thisMeshFilter;
            }
        }

        private BoxCollider ThisBoxCollider
        {
            get
            {
                if ((Object) this._thisBoxCollider == (Object) null)
                    this._thisBoxCollider = this.GetComponent<BoxCollider>();
                return this._thisBoxCollider;
            }
        }

        private void Awake() => this.Build();

        private void OnDestroy()
        {
            if (!((Object) this.ThisMeshFilter.sharedMesh != (Object) null))
                return;
            Object.Destroy((Object) this.ThisMeshFilter.sharedMesh);
        }

        private Mesh FillMesh(Mesh mesh)
        {
            Vector3 min = Vector3.zero;
            switch (this.anchor)
            {
                case tk2dBaseSprite.Anchor.LowerLeft:
                    min = new Vector3(0.0f, 0.0f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.LowerCenter:
                    min = new Vector3((float) (-(double) this.size.x / 2.0), 0.0f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.LowerRight:
                    min = new Vector3(-this.size.x, 0.0f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.MiddleLeft:
                    min = new Vector3(0.0f, (float) (-(double) this.size.y / 2.0), 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.MiddleCenter:
                    min = new Vector3((float) (-(double) this.size.x / 2.0), (float) (-(double) this.size.y / 2.0), 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.MiddleRight:
                    min = new Vector3(-this.size.x, (float) (-(double) this.size.y / 2.0), 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.UpperLeft:
                    min = new Vector3(0.0f, -this.size.y, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.UpperCenter:
                    min = new Vector3((float) (-(double) this.size.x / 2.0), -this.size.y, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.UpperRight:
                    min = new Vector3(-this.size.x, -this.size.y, 0.0f);
                    break;
            }
            Vector3[] vector3Array = new Vector3[4]
            {
                min + new Vector3(0.0f, 0.0f, -this.depth),
                min + new Vector3(this.size.x, 0.0f, -this.depth),
                min + new Vector3(0.0f, this.size.y, -this.depth),
                min + new Vector3(this.size.x, this.size.y, -this.depth)
            };
            mesh.vertices = vector3Array;
            mesh.uv = tk2dUIMask.uv;
            mesh.triangles = tk2dUIMask.indices;
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, min + new Vector3(this.size.x, this.size.y, 0.0f));
            mesh.bounds = bounds;
            return mesh;
        }

        private void OnDrawGizmosSelected()
        {
            Mesh sharedMesh = this.ThisMeshFilter.sharedMesh;
            if (!((Object) sharedMesh != (Object) null))
                return;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Bounds bounds = sharedMesh.bounds;
            Gizmos.color = (Color) new Color32((byte) 56, (byte) 146, (byte) 227, (byte) 96 /*0x60*/);
            float f = (float) (-(double) this.depth * 1.0010000467300415);
            Vector3 center = new Vector3(bounds.center.x, bounds.center.y, f * 0.5f);
            Vector3 size = new Vector3(bounds.extents.x * 2f, bounds.extents.y * 2f, Mathf.Abs(f));
            Gizmos.DrawCube(center, size);
            Gizmos.color = (Color) new Color32((byte) 22, (byte) 145, byte.MaxValue, byte.MaxValue);
            Gizmos.DrawWireCube(center, size);
        }

        public void Build()
        {
            if ((Object) this.ThisMeshFilter.sharedMesh == (Object) null)
            {
                Mesh mesh = new Mesh();
                mesh.hideFlags = HideFlags.DontSave;
                this.ThisMeshFilter.mesh = this.FillMesh(mesh);
            }
            else
                this.FillMesh(this.ThisMeshFilter.sharedMesh);
            if (this.createBoxCollider)
            {
                if ((Object) this.ThisBoxCollider == (Object) null)
                    this._thisBoxCollider = this.gameObject.AddComponent<BoxCollider>();
                Bounds bounds = this.ThisMeshFilter.sharedMesh.bounds;
                this.ThisBoxCollider.center = new Vector3(bounds.center.x, bounds.center.y, -this.depth);
                this.ThisBoxCollider.size = new Vector3(bounds.size.x, bounds.size.y, 0.0002f);
            }
            else
            {
                if (!((Object) this.ThisBoxCollider != (Object) null))
                    return;
                Object.Destroy((Object) this.ThisBoxCollider);
            }
        }

        public void ReshapeBounds(Vector3 dMin, Vector3 dMax)
        {
            Vector3 b = new Vector3(this.size.x, this.size.y);
            Vector3 a = Vector3.zero;
            switch (this.anchor)
            {
                case tk2dBaseSprite.Anchor.LowerLeft:
                    a.Set(0.0f, 0.0f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.LowerCenter:
                    a.Set(0.5f, 0.0f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.LowerRight:
                    a.Set(1f, 0.0f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.MiddleLeft:
                    a.Set(0.0f, 0.5f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.MiddleCenter:
                    a.Set(0.5f, 0.5f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.MiddleRight:
                    a.Set(1f, 0.5f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.UpperLeft:
                    a.Set(0.0f, 1f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.UpperCenter:
                    a.Set(0.5f, 1f, 0.0f);
                    break;
                case tk2dBaseSprite.Anchor.UpperRight:
                    a.Set(1f, 1f, 0.0f);
                    break;
            }
            a = Vector3.Scale(a, b) * -1f;
            Vector3 vector3_1 = b + dMax - dMin;
            Vector3 vector3_2 = new Vector3(!Mathf.Approximately(b.x, 0.0f) ? a.x * vector3_1.x / b.x : 0.0f, !Mathf.Approximately(b.y, 0.0f) ? a.y * vector3_1.y / b.y : 0.0f);
            this.transform.position = this.transform.TransformPoint((a + dMin - vector3_2) with
            {
                z = 0.0f
            });
            this.size = new Vector2(vector3_1.x, vector3_1.y);
            this.Build();
        }
    }

