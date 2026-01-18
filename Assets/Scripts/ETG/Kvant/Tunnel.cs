using System;

using UnityEngine;
using UnityEngine.Rendering;

#nullable disable
namespace Kvant
{
    [ExecuteInEditMode]
    [AddComponentMenu("Kvant/Tunnel")]
    public class Tunnel : MonoBehaviour
    {
        [SerializeField]
        private int _slices = 40;
        [SerializeField]
        private int _stacks = 100;
        [SerializeField]
        private float _radius = 5f;
        [SerializeField]
        private float _height = 50f;
        [SerializeField]
        private float _offset;
        [SerializeField]
        private int _noiseRepeat = 2;
        [SerializeField]
        private float _noiseFrequency = 0.05f;
        [SerializeField]
        [Range(1f, 5f)]
        private int _noiseDepth = 3;
        [SerializeField]
        private float _noiseClampMin = -1f;
        [SerializeField]
        private float _noiseClampMax = 1f;
        [SerializeField]
        private float _noiseElevation = 1f;
        [SerializeField]
        [Range(0.0f, 1f)]
        private float _noiseWarp;
        [SerializeField]
        private Material _material;
        private bool _owningMaterial;
        [SerializeField]
        private ShadowCastingMode _castShadows;
        [SerializeField]
        private bool _receiveShadows;
        [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
        [SerializeField]
        private Color _lineColor = new Color(0.0f, 0.0f, 0.0f, 0.4f);
        [SerializeField]
        private Shader _kernelShader;
        [SerializeField]
        private Shader _lineShader;
        [SerializeField]
        private Shader _debugShader;
        private int _stacksPerSegment;
        private int _totalStacks;
        private RenderTexture _positionBuffer;
        private RenderTexture _normalBuffer1;
        private RenderTexture _normalBuffer2;
        private Tunnel.BulkMesh _bulkMesh;
        private Material _kernelMaterial;
        private Material _lineMaterial;
        private Material _debugMaterial;
        private bool _needsReset = true;

        public int slices => this._slices;

        public int stacks => this._totalStacks;

        public float radius
        {
            get => this._radius;
            set => this._radius = value;
        }

        public float height
        {
            get => this._height;
            set => this._height = value;
        }

        public float offset
        {
            get => this._offset;
            set => this._offset = value;
        }

        public int noiseRepeat
        {
            get => this._noiseRepeat;
            set => this._noiseRepeat = value;
        }

        public float noiseFrequency
        {
            get => this._noiseFrequency;
            set => this._noiseFrequency = value;
        }

        public int noiseDepth
        {
            get => this._noiseDepth;
            set => this._noiseDepth = value;
        }

        public float noiseClampMin
        {
            get => this._noiseClampMin;
            set => this._noiseClampMin = value;
        }

        public float noiseClampMax
        {
            get => this._noiseClampMax;
            set => this._noiseClampMax = value;
        }

        public float noiseElevation
        {
            get => this._noiseElevation;
            set => this._noiseElevation = value;
        }

        public float noiseWarp
        {
            get => this._noiseWarp;
            set => this._noiseWarp = value;
        }

        public Material sharedMaterial
        {
            get => this._material;
            set => this._material = value;
        }

        public Material material
        {
            get
            {
                if (!this._owningMaterial)
                {
                    this._material = UnityEngine.Object.Instantiate<Material>(this._material);
                    this._owningMaterial = true;
                }
                return this._material;
            }
            set
            {
                if (this._owningMaterial)
                    UnityEngine.Object.Destroy((UnityEngine.Object) this._material, 0.1f);
                this._material = value;
                this._owningMaterial = false;
            }
        }

        public ShadowCastingMode shadowCastingMode
        {
            get => this._castShadows;
            set => this._castShadows = value;
        }

        public bool receiveShadows
        {
            get => this._receiveShadows;
            set => this._receiveShadows = value;
        }

        public Color lineColor
        {
            get => this._lineColor;
            set => this._lineColor = value;
        }

        private float ZOffset
        {
            get
            {
                return Mathf.Repeat(this._offset, (float) ((double) this._height / (double) this._totalStacks * 2.0));
            }
        }

        private float VOffset => this.ZOffset - this._offset;

        private void UpdateColumnAndRowCounts()
        {
            this._slices = Mathf.Clamp(this._slices, 4, 4096 /*0x1000*/);
            this._stacks = Mathf.Clamp(this._stacks, 4, 4096 /*0x1000*/);
            int num = this._slices * (this._stacks + 1) * 6 / 65000 + 1;
            this._stacksPerSegment = num <= 1 ? this._stacks : this._stacks / num / 2 * 2;
            this._totalStacks = this._stacksPerSegment * num;
        }

        public void NotifyConfigChange() => this._needsReset = true;

        private Material CreateMaterial(Shader shader)
        {
            Material material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            return material;
        }

        private RenderTexture CreateBuffer()
        {
            RenderTexture buffer = new RenderTexture(this._slices * 2, this._totalStacks + 1, 0, RenderTextureFormat.ARGBFloat);
            buffer.hideFlags = HideFlags.DontSave;
            buffer.filterMode = FilterMode.Point;
            buffer.wrapMode = TextureWrapMode.Repeat;
            return buffer;
        }

        private void UpdateKernelShader()
        {
            Material kernelMaterial = this._kernelMaterial;
            kernelMaterial.SetVector("_Extent", (Vector4) new Vector2(this._radius, this._height));
            kernelMaterial.SetFloat("_Offset", this.VOffset);
            kernelMaterial.SetVector("_Frequency", (Vector4) new Vector2((float) this._noiseRepeat, this._noiseFrequency));
            kernelMaterial.SetVector("_Amplitude", (Vector4) (new Vector3(1f, this._noiseWarp, this._noiseWarp) * this._noiseElevation));
            kernelMaterial.SetVector("_ClampRange", (Vector4) (new Vector2(this._noiseClampMin, this._noiseClampMax) * 1.415f));
            if ((double) this._noiseWarp > 0.0)
                kernelMaterial.EnableKeyword("ENABLE_WARP");
            else
                kernelMaterial.DisableKeyword("ENABLE_WARP");
            for (int index = 1; index <= 5; ++index)
            {
                if (index == this._noiseDepth)
                    kernelMaterial.EnableKeyword("DEPTH" + (object) index);
                else
                    kernelMaterial.DisableKeyword("DEPTH" + (object) index);
            }
        }

        private void ResetResources()
        {
            this.UpdateColumnAndRowCounts();
            if (this._bulkMesh == null)
                this._bulkMesh = new Tunnel.BulkMesh(this._slices, this._stacksPerSegment, this._totalStacks);
            else
                this._bulkMesh.Rebuild(this._slices, this._stacksPerSegment, this._totalStacks);
            if ((bool) (UnityEngine.Object) this._positionBuffer)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._positionBuffer);
            if ((bool) (UnityEngine.Object) this._normalBuffer1)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._normalBuffer1);
            if ((bool) (UnityEngine.Object) this._normalBuffer2)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._normalBuffer2);
            this._positionBuffer = this.CreateBuffer();
            this._normalBuffer1 = this.CreateBuffer();
            this._normalBuffer2 = this.CreateBuffer();
            if (!(bool) (UnityEngine.Object) this._kernelMaterial)
                this._kernelMaterial = this.CreateMaterial(this._kernelShader);
            if (!(bool) (UnityEngine.Object) this._lineMaterial)
                this._lineMaterial = this.CreateMaterial(this._lineShader);
            if (!(bool) (UnityEngine.Object) this._debugMaterial)
                this._debugMaterial = this.CreateMaterial(this._debugShader);
            this._lineMaterial.SetTexture("_PositionBuffer", (Texture) this._positionBuffer);
            this._needsReset = false;
        }

        private void Reset() => this._needsReset = true;

        private void OnDestroy()
        {
            if (this._bulkMesh != null)
                this._bulkMesh.Release();
            if ((bool) (UnityEngine.Object) this._positionBuffer)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._positionBuffer);
            if ((bool) (UnityEngine.Object) this._normalBuffer1)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._normalBuffer1);
            if ((bool) (UnityEngine.Object) this._normalBuffer2)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._normalBuffer2);
            if ((bool) (UnityEngine.Object) this._kernelMaterial)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._kernelMaterial);
            if ((bool) (UnityEngine.Object) this._lineMaterial)
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._lineMaterial);
            if (!(bool) (UnityEngine.Object) this._debugMaterial)
                return;
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._debugMaterial);
        }

        private void LateUpdate()
        {
            if (this._needsReset)
                this.ResetResources();
            this.UpdateKernelShader();
            Graphics.Blit((Texture) null, this._positionBuffer, this._kernelMaterial, 0);
            Graphics.Blit((Texture) this._positionBuffer, this._normalBuffer1, this._kernelMaterial, 1);
            Graphics.Blit((Texture) this._positionBuffer, this._normalBuffer2, this._kernelMaterial, 2);
            this._lineMaterial.SetColor("_Color", this._lineColor);
            MaterialPropertyBlock properties1 = new MaterialPropertyBlock();
            MaterialPropertyBlock properties2 = new MaterialPropertyBlock();
            properties1.SetTexture("_PositionBuffer", (Texture) this._positionBuffer);
            properties2.SetTexture("_PositionBuffer", (Texture) this._positionBuffer);
            properties1.SetTexture("_NormalBuffer", (Texture) this._normalBuffer1);
            properties2.SetTexture("_NormalBuffer", (Texture) this._normalBuffer2);
            Vector3 vector3 = new Vector3(0.0f, 0.0f, this.VOffset);
            properties1.SetVector("_MapOffset", (Vector4) vector3);
            properties2.SetVector("_MapOffset", (Vector4) vector3);
            properties1.SetFloat("_UseBuffer", 1f);
            properties2.SetFloat("_UseBuffer", 1f);
            Mesh mesh = this._bulkMesh.mesh;
            Vector3 position1 = this.transform.position;
            Quaternion rotation = this.transform.rotation;
            Vector2 vector2 = new Vector2(0.5f / (float) this._positionBuffer.width, 0.0f);
            Vector3 position2 = position1 + this.transform.forward * this.ZOffset;
            for (int index = 0; index < this._totalStacks; index += this._stacksPerSegment)
            {
                vector2.y = (0.5f + (float) index) / (float) this._positionBuffer.height;
                properties1.SetVector("_BufferOffset", (Vector4) vector2);
                properties2.SetVector("_BufferOffset", (Vector4) vector2);
                if ((bool) (UnityEngine.Object) this._material)
                {
                    Graphics.DrawMesh(mesh, position2, rotation, this._material, 0, (Camera) null, 0, properties1, this._castShadows, this._receiveShadows);
                    Graphics.DrawMesh(mesh, position2, rotation, this._material, 0, (Camera) null, 1, properties2, this._castShadows, this._receiveShadows);
                }
                if ((double) this._lineColor.a > 0.0)
                    Graphics.DrawMesh(mesh, position2, rotation, this._lineMaterial, 0, (Camera) null, 2, properties1, false, false);
            }
        }

        [Serializable]
        private class BulkMesh
        {
            private Mesh _mesh;

            public BulkMesh(int columns, int rowsPerSegment, int totalRows)
            {
                this._mesh = this.BuildMesh(columns, rowsPerSegment, totalRows);
            }

            public Mesh mesh => this._mesh;

            public void Rebuild(int columns, int rowsPerSegment, int totalRows)
            {
                this.Release();
                this._mesh = this.BuildMesh(columns, rowsPerSegment, totalRows);
            }

            public void Release()
            {
                if (!((UnityEngine.Object) this._mesh != (UnityEngine.Object) null))
                    return;
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this._mesh);
                this._mesh = (Mesh) null;
            }

            private Mesh BuildMesh(int columns, int rows, int totalRows)
            {
                int num1 = columns;
                int num2 = rows + 1;
                float num3 = 0.5f / (float) num1;
                float num4 = 1f / (float) (totalRows + 1);
                float num5 = 0.0f;
                Vector2[] vector2Array1 = new Vector2[num1 * (num2 - 1) * 6];
                Vector2[] vector2Array2 = new Vector2[num1 * (num2 - 1) * 6];
                int index1 = 0;
                for (int index2 = 0; index2 < num2 - 1; ++index2)
                {
                    int num6 = 0;
                    while (num6 < num1)
                    {
                        int num7 = num6 * 2 + (index2 & 1);
                        vector2Array1[index1] = new Vector2(num3 * (float) num7, num5 + num4 * (float) index2);
                        vector2Array1[index1 + 1] = new Vector2(num3 * (float) (num7 + 1), num5 + num4 * (float) (index2 + 1));
                        vector2Array1[index1 + 2] = new Vector2(num3 * (float) (num7 + 2), num5 + num4 * (float) index2);
                        vector2Array2[index1] = vector2Array2[index1 + 1] = vector2Array2[index1 + 2] = vector2Array1[index1];
                        ++num6;
                        index1 += 3;
                    }
                }
                for (int index3 = 0; index3 < num2 - 1; ++index3)
                {
                    int num8 = 0;
                    while (num8 < num1)
                    {
                        int num9 = num8 * 2 + 2 - (index3 & 1);
                        vector2Array1[index1] = new Vector2(num3 * (float) num9, num5 + num4 * (float) index3);
                        vector2Array1[index1 + 1] = new Vector2(num3 * (float) (num9 - 1), num5 + num4 * (float) (index3 + 1));
                        vector2Array1[index1 + 2] = new Vector2(num3 * (float) (num9 + 1), num5 + num4 * (float) (index3 + 1));
                        vector2Array2[index1] = vector2Array2[index1 + 1] = vector2Array2[index1 + 2] = vector2Array1[index1];
                        ++num8;
                        index1 += 3;
                    }
                }
                int[] indices1 = new int[num1 * (num2 - 1) * 3];
                int[] indices2 = new int[num1 * (num2 - 1) * 3];
                for (int index4 = 0; index4 < indices1.Length; ++index4)
                {
                    indices1[index4] = index4;
                    indices2[index4] = index4 + indices1.Length;
                }
                int[] indices3 = new int[num1 * (num2 - 1) * 6];
                int index5 = 0;
                int num10 = 0;
                for (int index6 = 0; index6 < num2 - 1; ++index6)
                {
                    int num11 = 0;
                    while (num11 < num1)
                    {
                        indices3[index5] = num10;
                        indices3[index5 + 1] = num10 + 1;
                        indices3[index5 + 2] = num10;
                        indices3[index5 + 3] = num10 + 2;
                        indices3[index5 + 4] = num10 + 1;
                        indices3[index5 + 5] = num10 + 2;
                        ++num11;
                        index5 += 6;
                        num10 += 3;
                    }
                }
                Mesh mesh = new Mesh();
                mesh.subMeshCount = 3;
                mesh.vertices = new Vector3[vector2Array1.Length];
                mesh.uv = vector2Array1;
                mesh.uv2 = vector2Array2;
                mesh.SetIndices(indices1, MeshTopology.Triangles, 0);
                mesh.SetIndices(indices2, MeshTopology.Triangles, 1);
                mesh.SetIndices(indices3, MeshTopology.Lines, 2);
                mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f);
                mesh.hideFlags = HideFlags.DontSave;
                return mesh;
            }
        }
    }
}
