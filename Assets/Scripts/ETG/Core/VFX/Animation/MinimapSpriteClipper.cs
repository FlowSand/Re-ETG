using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class MinimapSpriteClipper : MonoBehaviour
    {
        private tk2dBaseSprite m_baseSprite;

        public void ForceUpdate() => this.ClipToTileBounds();

        private void ClipToTileBounds()
        {
            Transform transform = this.transform;
            if ((Object) this.m_baseSprite == (Object) null)
                this.m_baseSprite = this.GetComponent<tk2dBaseSprite>();
            Bounds bounds = this.m_baseSprite.GetBounds();
            Vector2 vector2_1 = transform.position.XY() + bounds.min.XY();
            Vector2 vector2_2 = transform.position.XY() + bounds.max.XY();
            IntVector2 intVector2_1 = ((vector2_1 - Minimap.Instance.transform.position.XY()) * 8f).ToIntVector2(VectorConversions.Floor);
            IntVector2 intVector2_2 = ((vector2_2 - Minimap.Instance.transform.position.XY()) * 8f).ToIntVector2(VectorConversions.Floor);
            tk2dSpriteDefinition spriteDefinition = this.m_baseSprite.Collection.spriteDefinitions[this.m_baseSprite.spriteId];
            Vector2 lhs1 = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 lhs2 = new Vector2(float.MinValue, float.MinValue);
            for (int index = 0; index < spriteDefinition.uvs.Length; ++index)
            {
                lhs1 = Vector2.Min(lhs1, spriteDefinition.uvs[index]);
                lhs2 = Vector2.Max(lhs2, spriteDefinition.uvs[index]);
            }
            List<Vector3> vector3List = new List<Vector3>();
            List<int> intList = new List<int>();
            List<Vector2> vector2List = new List<Vector2>();
            for (int x1 = intVector2_1.x; x1 <= intVector2_2.x; ++x1)
            {
                for (int y1 = intVector2_1.y; y1 <= intVector2_2.y; ++y1)
                {
                    if (Minimap.Instance[x1, y1])
                    {
                        int count = vector3List.Count;
                        float a1 = (float) x1 / 8f + Minimap.Instance.transform.position.x;
                        float a2 = (float) y1 / 8f + Minimap.Instance.transform.position.y;
                        float x2 = Mathf.Max(a1, vector2_1.x) - transform.position.x;
                        float x3 = Mathf.Min(a1 + 0.125f, vector2_2.x) - transform.position.x;
                        float y2 = Mathf.Max(a2, vector2_1.y) - transform.position.y;
                        float y3 = Mathf.Min(a2 + 0.125f, vector2_2.y) - transform.position.y;
                        vector3List.Add(new Vector3(x2, y2, 0.0f));
                        vector3List.Add(new Vector3(x3, y2, 0.0f));
                        vector3List.Add(new Vector3(x2, y3, 0.0f));
                        vector3List.Add(new Vector3(x3, y3, 0.0f));
                        intList.Add(count);
                        intList.Add(count + 2);
                        intList.Add(count + 1);
                        intList.Add(count + 2);
                        intList.Add(count + 3);
                        intList.Add(count + 1);
                        float t1 = (float) (((double) x2 + (double) transform.position.x - (double) vector2_1.x) / ((double) vector2_2.x - (double) vector2_1.x));
                        float t2 = (float) (((double) x3 + (double) transform.position.x - (double) vector2_1.x) / ((double) vector2_2.x - (double) vector2_1.x));
                        float t3 = (float) (((double) y2 + (double) transform.position.y - (double) vector2_1.y) / ((double) vector2_2.y - (double) vector2_1.y));
                        float t4 = (float) (((double) y3 + (double) transform.position.y - (double) vector2_1.y) / ((double) vector2_2.y - (double) vector2_1.y));
                        float x4 = Mathf.Lerp(lhs1.x, lhs2.x, t1);
                        float x5 = Mathf.Lerp(lhs1.x, lhs2.x, t2);
                        float y4 = Mathf.Lerp(lhs1.y, lhs2.y, t3);
                        float y5 = Mathf.Lerp(lhs1.y, lhs2.y, t4);
                        vector2List.Add(new Vector2(x4, y4));
                        vector2List.Add(new Vector2(x5, y4));
                        vector2List.Add(new Vector2(x4, y5));
                        vector2List.Add(new Vector2(x5, y5));
                    }
                }
            }
            MeshFilter component = this.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.vertices = vector3List.ToArray();
            mesh.triangles = intList.ToArray();
            mesh.uv = vector2List.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            component.mesh = mesh;
        }
    }

