using UnityEngine;

#nullable disable

public class BalloonAttachmentDoer : MonoBehaviour
    {
        public GameActor AttachTarget;
        private Vector2 m_currentOffset;
        private Mesh m_mesh;
        private Vector3[] m_vertices;
        private tk2dSprite m_sprite;
        private MeshFilter m_stringFilter;

        public void Initialize(GameActor target)
        {
            this.AttachTarget = target;
            this.m_currentOffset = new Vector2(1f, 2f);
            this.m_sprite = this.GetComponent<tk2dSprite>();
            this.m_mesh = new Mesh();
            this.m_vertices = new Vector3[20];
            this.m_mesh.vertices = this.m_vertices;
            int[] numArray = new int[54];
            Vector2[] vector2Array = new Vector2[20];
            int num = 0;
            for (int index = 0; index < 9; ++index)
            {
                numArray[index * 6] = num;
                numArray[index * 6 + 1] = num + 2;
                numArray[index * 6 + 2] = num + 1;
                numArray[index * 6 + 3] = num + 2;
                numArray[index * 6 + 4] = num + 3;
                numArray[index * 6 + 5] = num + 1;
                num += 2;
            }
            this.m_mesh.triangles = numArray;
            this.m_mesh.uv = vector2Array;
            GameObject gameObject = new GameObject("balloon string");
            this.m_stringFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>().material = BraveResources.Load("Global VFX/WhiteMaterial", ".mat") as Material;
            this.m_stringFilter.mesh = this.m_mesh;
            this.transform.position = this.AttachTarget.transform.position + this.m_currentOffset.ToVector3ZisY(-3f);
        }

        private void LateUpdate()
        {
            if ((Object) this.AttachTarget != (Object) null)
            {
                if (this.AttachTarget is AIActor && (!(bool) (Object) this.AttachTarget || this.AttachTarget.healthHaver.IsDead))
                {
                    Object.Destroy((Object) this.gameObject);
                    return;
                }
                this.m_currentOffset = new Vector2(Mathf.Lerp(0.5f, 1f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup, 3f) / 3f), Mathf.Lerp(1.33f, 2f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup / 1.75f, 3f) / 3f));
                Vector3 vector3_1 = (Vector3) this.AttachTarget.CenterPosition;
                if (this.AttachTarget is PlayerController)
                {
                    PlayerHandController primaryHand = (this.AttachTarget as PlayerController).primaryHand;
                    if (primaryHand.renderer.enabled)
                        vector3_1 = (Vector3) primaryHand.sprite.WorldCenter;
                }
                Vector3 vector3_2 = this.AttachTarget.transform.position + this.m_currentOffset.ToVector3ZisY(-3f);
                float num = Vector3.Distance(this.transform.position, vector3_2);
                this.transform.position = Vector3.MoveTowards(this.transform.position, vector3_2, BraveMathCollege.UnboundedLerp(1f, 10f, num / 3f) * BraveTime.DeltaTime);
                this.BuildMeshAlongCurve((Vector2) vector3_1, (Vector2) vector3_1, this.m_sprite.WorldCenter + new Vector2(0.0f, -2f), this.m_sprite.WorldCenter);
                this.m_mesh.vertices = this.m_vertices;
                this.m_mesh.RecalculateBounds();
                this.m_mesh.RecalculateNormals();
            }
            if ((bool) (Object) this.AttachTarget && !this.AttachTarget.healthHaver.IsDead)
                return;
            Object.Destroy((Object) this.gameObject);
        }

        private void OnDestroy()
        {
            if (!(bool) (Object) this.m_stringFilter)
                return;
            Object.Destroy((Object) this.m_stringFilter.gameObject);
        }

        private void BuildMeshAlongCurve(
            Vector2 p0,
            Vector2 p1,
            Vector2 p2,
            Vector2 p3,
            float meshWidth = 0.03125f)
        {
            Vector3[] vertices = this.m_vertices;
            Vector2? nullable1 = new Vector2?();
            for (int index = 0; index < 10; ++index)
            {
                Vector2 bezierPoint = BraveMathCollege.CalculateBezierPoint((float) index / 9f, p0, p1, p2, p3);
                Vector2? nullable2 = index != 9 ? new Vector2?(BraveMathCollege.CalculateBezierPoint((float) index / 9f, p0, p1, p2, p3)) : new Vector2?();
                Vector2 vector2 = Vector2.zero;
                if (nullable1.HasValue)
                    vector2 += (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (bezierPoint - nullable1.Value)).XY().normalized;
                if (nullable2.HasValue)
                    vector2 += (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (nullable2.Value - bezierPoint)).XY().normalized;
                vector2 = vector2.normalized;
                vertices[index * 2] = (bezierPoint + vector2 * meshWidth).ToVector3ZisY();
                vertices[index * 2 + 1] = (bezierPoint + -vector2 * meshWidth).ToVector3ZisY();
                nullable1 = new Vector2?(bezierPoint);
            }
        }
    }

