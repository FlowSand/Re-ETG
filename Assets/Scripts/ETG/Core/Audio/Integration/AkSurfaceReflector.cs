using UnityEngine;

using AK.Wwise;

#nullable disable

[DisallowMultipleComponent]
[RequireComponent(typeof (MeshFilter))]
[AddComponentMenu("Wwise/AkSurfaceReflector")]
public class AkSurfaceReflector : MonoBehaviour
    {
        public AcousticTexture AcousticTexture;
        private MeshFilter MeshFilter;

        public static void AddGeometrySet(AcousticTexture acousticTexture, MeshFilter meshFilter)
        {
            if ((Object) meshFilter == (Object) null)
            {
                Debug.Log((object) (meshFilter.name + ": No mesh found!"));
            }
            else
            {
                Mesh sharedMesh = meshFilter.sharedMesh;
                Vector3[] vertices = sharedMesh.vertices;
                int[] triangles = sharedMesh.triangles;
                int num1 = sharedMesh.triangles.Length / 3;
                using (AkTriangleArray in_pTriangles = new AkTriangleArray(num1))
                {
                    for (int index = 0; index < num1; ++index)
                    {
                        using (AkTriangle triangle = in_pTriangles.GetTriangle(index))
                        {
                            Vector3 vector3_1 = meshFilter.transform.TransformPoint(vertices[triangles[3 * index]]);
                            Vector3 vector3_2 = meshFilter.transform.TransformPoint(vertices[triangles[3 * index + 1]]);
                            Vector3 vector3_3 = meshFilter.transform.TransformPoint(vertices[triangles[3 * index + 2]]);
                            triangle.point0.X = vector3_1.x;
                            triangle.point0.Y = vector3_1.y;
                            triangle.point0.Z = vector3_1.z;
                            triangle.point1.X = vector3_2.x;
                            triangle.point1.Y = vector3_2.y;
                            triangle.point1.Z = vector3_2.z;
                            triangle.point2.X = vector3_3.x;
                            triangle.point2.Y = vector3_3.y;
                            triangle.point2.Z = vector3_3.z;
                            triangle.textureID = (uint) acousticTexture.ID;
                            triangle.reflectorChannelMask = uint.MaxValue;
                            triangle.strName = $"{meshFilter.gameObject.name}_{(object) index}";
                        }
                    }
                    int num2 = (int) AkSoundEngine.SetGeometry((ulong) meshFilter.GetInstanceID(), in_pTriangles, (uint) num1);
                }
            }
        }

        public static void RemoveGeometrySet(MeshFilter meshFilter)
        {
            if (!((Object) meshFilter != (Object) null))
                return;
            int num = (int) AkSoundEngine.RemoveGeometry((ulong) meshFilter.GetInstanceID());
        }

        private void Awake() => this.MeshFilter = this.GetComponent<MeshFilter>();

        private void OnEnable()
        {
            AkSurfaceReflector.AddGeometrySet(this.AcousticTexture, this.MeshFilter);
        }

        private void OnDisable() => AkSurfaceReflector.RemoveGeometrySet(this.MeshFilter);
    }

