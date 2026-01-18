using UnityEngine;
using UnityEngine.Rendering;

#nullable disable

[ExecuteInEditMode]
public class SlicedVolume : MonoBehaviour
    {
        public Material cloudMaterial;
        public Material shadowCasterMat;
        public int sliceAmount = 25;
        public int segmentCount = 3;
        public Vector3 dimensions = new Vector3(1000f, 50f, 1000f);
        public Vector3 normalDirection = new Vector3(1f, 1f, 1f);
        public bool shadowCaster;
        public bool transferVariables = true;
        public bool unityFive;
        public bool curved;
        public float roundness = 2f;
        public float intensity = 1f / 1000f;
        public bool generateNewSlices;
        private bool updateCloudDirection = true;
        private int cameraCloudRelation = 1;
        private Color[] vertexColor;
        private Vector3[] vertices;
        private Vector2[] uvMap;
        private int[] triangleConstructor;
        private Vector3 posGainPerVertices;
        private float posGainPerUV;
        private GameObject meshSlices;
        private GameObject meshShadowCaster;

        private void OnDrawGizmos()
        {
            this.editorUpdate();
            if (!this.generateNewSlices)
                return;
            if ((bool) (Object) this.cloudMaterial)
            {
                this.integrityCheck();
                this.settingValuesUp(false);
                if (this.shadowCaster)
                {
                    int sliceAmount = this.sliceAmount;
                    this.sliceAmount = 1;
                    this.settingValuesUp(true);
                    this.sliceAmount = sliceAmount;
                }
            }
            this.generateNewSlices = false;
        }

        private void Update()
        {
            if (!Application.isPlaying || !this.generateNewSlices)
                return;
            if ((bool) (Object) this.cloudMaterial)
            {
                this.integrityCheck();
                this.settingValuesUp(false);
                if (this.shadowCaster)
                {
                    int sliceAmount = this.sliceAmount;
                    this.sliceAmount = 1;
                    this.settingValuesUp(true);
                    this.sliceAmount = sliceAmount;
                }
            }
            this.generateNewSlices = false;
        }

        private void syncCloudAndShadowCaster()
        {
            this.shadowCasterMat.CopyPropertiesFromMaterial(this.cloudMaterial);
        }

        private void editorUpdate()
        {
            this.sliceAmount = this.sliceAmount > 1 ? this.sliceAmount : 1;
            this.segmentCount = this.segmentCount <= 2 ? 2 : this.segmentCount;
            if (Camera.current.name != "PreRenderCamera" && (bool) (Object) this.cloudMaterial && !this.curved && (bool) (Object) this.meshSlices)
            {
                if ((double) Camera.current.transform.position.y > (double) this.transform.position.y && this.cameraCloudRelation == -1)
                {
                    this.cameraCloudRelation = 1;
                    this.updateCloudDirection = true;
                }
                else if ((double) Camera.current.transform.position.y < (double) this.transform.position.y && this.cameraCloudRelation == 1)
                {
                    this.cameraCloudRelation = -1;
                    this.updateCloudDirection = true;
                }
                if (this.updateCloudDirection)
                {
                    this.meshSlices.transform.localScale = new Vector3(Mathf.Abs(this.meshSlices.transform.localScale.x), Mathf.Abs(this.meshSlices.transform.localScale.y) * (float) this.cameraCloudRelation, Mathf.Abs(this.meshSlices.transform.localScale.z));
                    this.cloudMaterial.SetVector("_CloudNormalsDirection", new Vector4(this.normalDirection.x, this.normalDirection.y * (float) this.cameraCloudRelation, this.normalDirection.z * -1f, 0.0f));
                    this.updateCloudDirection = false;
                }
            }
            else if (this.curved && (bool) (Object) this.cloudMaterial && (bool) (Object) this.meshSlices)
            {
                this.meshSlices.transform.localScale = new Vector3(Mathf.Abs(this.meshSlices.transform.localScale.x), Mathf.Abs(this.meshSlices.transform.localScale.y) * -1f, Mathf.Abs(this.meshSlices.transform.localScale.z));
                if ((bool) (Object) this.meshShadowCaster)
                    this.meshShadowCaster.transform.localScale = new Vector3(Mathf.Abs(this.meshSlices.transform.localScale.x), Mathf.Abs(this.meshSlices.transform.localScale.y) * -1f, Mathf.Abs(this.meshSlices.transform.localScale.z));
                this.cloudMaterial.SetVector("_CloudNormalsDirection", new Vector4(this.normalDirection.x, this.normalDirection.y * -1f, this.normalDirection.z * -1f, 0.0f));
            }
            if (!this.transferVariables || !(bool) (Object) this.cloudMaterial || !(bool) (Object) this.shadowCasterMat)
                return;
            this.syncCloudAndShadowCaster();
        }

        private void integrityCheck()
        {
            if (!(bool) (Object) this.meshSlices)
            {
                foreach (Transform transform in this.transform)
                {
                    if (transform.name == "Clouds")
                        this.meshSlices = transform.gameObject;
                }
                if (!(bool) (Object) this.meshSlices)
                {
                    this.meshSlices = new GameObject("Clouds");
                    this.meshSlices.transform.parent = this.transform;
                    this.meshSlices.transform.localPosition = Vector3.zero;
                    this.meshSlices.AddComponent<MeshFilter>();
                    this.meshSlices.AddComponent<MeshRenderer>();
                    this.meshSlices.GetComponent<Renderer>().material = this.cloudMaterial;
                }
            }
            if (this.shadowCaster && !(bool) (Object) this.meshShadowCaster)
            {
                foreach (Transform transform in this.transform)
                {
                    if (transform.name == "Shadow Caster")
                        this.meshShadowCaster = transform.gameObject;
                }
                if (!(bool) (Object) this.meshShadowCaster)
                {
                    this.meshShadowCaster = new GameObject("Shadow Caster");
                    this.meshShadowCaster.transform.parent = this.transform;
                    this.meshShadowCaster.transform.localPosition = Vector3.zero;
                    this.meshShadowCaster.AddComponent<MeshFilter>();
                    this.meshShadowCaster.AddComponent<MeshRenderer>();
                    this.meshShadowCaster.GetComponent<Renderer>().material = this.shadowCasterMat;
                }
            }
            if (!this.shadowCaster)
            {
                if ((bool) (Object) this.meshShadowCaster)
                {
                    Object.DestroyImmediate((Object) this.meshShadowCaster);
                }
                else
                {
                    foreach (Transform transform in this.transform)
                    {
                        if (transform.name == "Shadow Caster")
                            Object.DestroyImmediate((Object) transform.gameObject);
                    }
                }
            }
            if ((Object) this.meshShadowCaster != (Object) null)
                this.meshShadowCaster.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            this.meshSlices.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            if ((Object) this.meshShadowCaster != (Object) null)
                this.meshShadowCaster.GetComponent<MeshRenderer>().receiveShadows = false;
            this.meshSlices.GetComponent<MeshRenderer>().receiveShadows = false;
        }

        private void settingValuesUp(bool isShadowCaster)
        {
            this.vertices = new Vector3[this.segmentCount * this.segmentCount * this.sliceAmount];
            this.uvMap = new Vector2[this.vertices.Length];
            this.triangleConstructor = new int[(this.segmentCount - 1) * (this.segmentCount - 1) * this.sliceAmount * 2 * 3];
            this.vertexColor = new Color[this.vertices.Length];
            float num = (float) (1.0 / ((double) this.segmentCount - 1.0));
            this.posGainPerVertices = new Vector3(num * this.dimensions.x, 1f / (float) Mathf.Clamp(this.sliceAmount - 1, 1, 999999) * this.dimensions.y, num * this.dimensions.z);
            this.posGainPerUV = num;
            this.trianglesCreation(isShadowCaster);
        }

        private void trianglesCreation(bool isShadowCaster)
        {
            int num1 = 0;
            int num2 = 0;
            int index1 = 0;
            float f = 0.0f;
            for (int index2 = 0; index2 < this.sliceAmount; ++index2)
            {
                float num3 = (float) ((double) index2 * (2.0 / (double) this.sliceAmount) - 1.0);
                float w = (double) index2 >= (double) this.sliceAmount * 0.5 ? (float) (2.0 - 1.0 / ((double) this.sliceAmount * 0.5) * (double) (index2 + 1)) : (float) (1.0 / ((double) this.sliceAmount * 0.5)) * (float) index2;
                if (this.sliceAmount == 1)
                    w = 1f;
                for (int index3 = 0; index3 < this.segmentCount; ++index3)
                {
                    int num4 = this.segmentCount * num1;
                    for (int index4 = 0; index4 < this.segmentCount; ++index4)
                    {
                        if (this.curved)
                            f = Vector3.Distance(new Vector3((float) ((double) this.posGainPerVertices.x * (double) index4 - (double) this.dimensions.x / 2.0), 0.0f, (float) ((double) this.posGainPerVertices.z * (double) index3 - (double) this.dimensions.z / 2.0)), Vector3.zero);
                        this.vertices[index4 + num4] = this.sliceAmount != 1 ? new Vector3((float) ((double) this.posGainPerVertices.x * (double) index4 - (double) this.dimensions.x / 2.0), (float) ((double) this.posGainPerVertices.y * (double) index2 - (double) this.dimensions.y / 2.0 + (double) Mathf.Pow(f, this.roundness) * (double) this.intensity), (float) ((double) this.posGainPerVertices.z * (double) index3 - (double) this.dimensions.z / 2.0)) : new Vector3((float) ((double) this.posGainPerVertices.x * (double) index4 - (double) this.dimensions.x / 2.0), Mathf.Pow(f, this.roundness) * this.intensity, (float) ((double) this.posGainPerVertices.z * (double) index3 - (double) this.dimensions.z / 2.0));
                        this.uvMap[index4 + num4] = new Vector2(this.posGainPerUV * (float) index4, this.posGainPerUV * (float) index3);
                        this.vertexColor[index4 + num4] = (Color) new Vector4(num3, num3, num3, w);
                    }
                    ++num1;
                    if (index3 >= 1)
                    {
                        for (int index5 = 0; index5 < this.segmentCount - 1; ++index5)
                        {
                            this.triangleConstructor[index1] = index5 + num2 + index2 * this.segmentCount;
                            this.triangleConstructor[1 + index1] = this.segmentCount + index5 + num2 + index2 * this.segmentCount;
                            this.triangleConstructor[2 + index1] = 1 + index5 + num2 + index2 * this.segmentCount;
                            this.triangleConstructor[3 + index1] = this.segmentCount + 1 + index5 + num2 + index2 * this.segmentCount;
                            this.triangleConstructor[4 + index1] = 1 + index5 + num2 + index2 * this.segmentCount;
                            this.triangleConstructor[5 + index1] = this.segmentCount + index5 + num2 + index2 * this.segmentCount;
                            index1 += 6;
                        }
                        num2 += this.segmentCount;
                    }
                }
            }
            if (!isShadowCaster)
            {
                Mesh mesh = new Mesh();
                mesh.Clear();
                mesh.name = "GeoSlices";
                mesh.vertices = this.vertices;
                mesh.triangles = this.triangleConstructor;
                mesh.uv = this.uvMap;
                mesh.colors = this.vertexColor;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                SlicedVolume.calculateMeshTangents(mesh);
                this.meshSlices.GetComponent<MeshFilter>().mesh = mesh;
            }
            else
            {
                Mesh mesh = new Mesh();
                mesh.Clear();
                mesh.name = "GeoSlices";
                mesh.vertices = this.vertices;
                mesh.triangles = this.triangleConstructor;
                mesh.uv = this.uvMap;
                mesh.colors = this.vertexColor;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                SlicedVolume.calculateMeshTangents(mesh);
                this.meshShadowCaster.GetComponent<MeshFilter>().mesh = mesh;
            }
        }

        public static void calculateMeshTangents(Mesh mesh)
        {
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;
            int length1 = triangles.Length;
            int length2 = vertices.Length;
            Vector3[] vector3Array1 = new Vector3[length2];
            Vector3[] vector3Array2 = new Vector3[length2];
            Vector4[] vector4Array = new Vector4[length2];
            for (long index1 = 0; index1 < (long) length1; index1 += 3L)
            {
                long index2 = (long) triangles[index1];
                long index3 = (long) triangles[index1 + 1L];
                long index4 = (long) triangles[index1 + 2L];
                Vector3 vector3_1 = vertices[index2];
                Vector3 vector3_2 = vertices[index3];
                Vector3 vector3_3 = vertices[index4];
                Vector2 vector2_1 = uv[index2];
                Vector2 vector2_2 = uv[index3];
                Vector2 vector2_3 = uv[index4];
                float num1 = vector3_2.x - vector3_1.x;
                float num2 = vector3_3.x - vector3_1.x;
                float num3 = vector3_2.y - vector3_1.y;
                float num4 = vector3_3.y - vector3_1.y;
                float num5 = vector3_2.z - vector3_1.z;
                float num6 = vector3_3.z - vector3_1.z;
                float num7 = vector2_2.x - vector2_1.x;
                float num8 = vector2_3.x - vector2_1.x;
                float num9 = vector2_2.y - vector2_1.y;
                float num10 = vector2_3.y - vector2_1.y;
                float num11 = (float) (1.0 / ((double) num7 * (double) num10 - (double) num8 * (double) num9));
                Vector3 vector3_4 = new Vector3((float) ((double) num10 * (double) num1 - (double) num9 * (double) num2) * num11, (float) ((double) num10 * (double) num3 - (double) num9 * (double) num4) * num11, (float) ((double) num10 * (double) num5 - (double) num9 * (double) num6) * num11);
                Vector3 vector3_5 = new Vector3((float) ((double) num7 * (double) num2 - (double) num8 * (double) num1) * num11, (float) ((double) num7 * (double) num4 - (double) num8 * (double) num3) * num11, (float) ((double) num7 * (double) num6 - (double) num8 * (double) num5) * num11);
                vector3Array1[index2] += vector3_4;
                vector3Array1[index3] += vector3_4;
                vector3Array1[index4] += vector3_4;
                vector3Array2[index2] += vector3_5;
                vector3Array2[index3] += vector3_5;
                vector3Array2[index4] += vector3_5;
            }
            for (long index = 0; index < (long) length2; ++index)
            {
                Vector3 normal = normals[index];
                Vector3 tangent = vector3Array1[index];
                Vector3.OrthoNormalize(ref normal, ref tangent);
                vector4Array[index].x = tangent.x;
                vector4Array[index].y = tangent.y;
                vector4Array[index].z = tangent.z;
                vector4Array[index].w = (double) Vector3.Dot(Vector3.Cross(normal, tangent), vector3Array2[index]) >= 0.0 ? 1f : -1f;
            }
            mesh.tangents = vector4Array;
        }
    }

