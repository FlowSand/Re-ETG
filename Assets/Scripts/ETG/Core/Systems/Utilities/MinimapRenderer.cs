using UnityEngine;

#nullable disable

public class MinimapRenderer : MonoBehaviour
    {
        public Transform QuadTransform;
        public Texture MapMaskFullscreen;
        public Texture MapMaskSmallscreen;
        private Material m_quadMaterial;
        private Camera m_camera;
        private Camera m_uiCamera;
        private int m_cmQuad;
        private int m_idMainTex;
        private int m_idMaskTex;
        private RenderTexture m_currentQuadRenderTexture;

        private void Awake()
        {
            this.m_camera = this.GetComponent<Camera>();
            this.m_quadMaterial = this.QuadTransform.GetComponent<MeshRenderer>().material;
            this.QuadTransform.parent = this.QuadTransform.parent.parent;
            this.QuadTransform.gameObject.SetLayerRecursively(LayerMask.NameToLayer("GUI"));
            this.m_idMainTex = Shader.PropertyToID("_MainTex");
            this.m_idMaskTex = Shader.PropertyToID("_MaskTex");
        }

        private void Start() => this.m_uiCamera = GameUIRoot.Instance.Manager.RenderCamera;

        private void CheckSize()
        {
            Rect rect = new Rect(1f - Minimap.Instance.currentXRectFactor, 1f - Minimap.Instance.currentYRectFactor, Minimap.Instance.currentXRectFactor, Minimap.Instance.currentYRectFactor);
            if (!Minimap.Instance.IsFullscreen && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !GameManager.Instance.IsLoadingLevel && (!GameManager.Instance.SecondaryPlayer.IsGhost || true))
                rect.y -= 0.0875f;
            this.QuadTransform.localScale = new Vector3((float) ((double) this.m_uiCamera.orthographicSize * 2.0 * 1.7777777910232544) * rect.width, this.m_uiCamera.orthographicSize * 2f * rect.height, 1f);
            Vector3 vector3 = new Vector3((float) ((double) this.m_uiCamera.orthographicSize * (double) this.m_uiCamera.aspect * -1.0), this.m_uiCamera.orthographicSize * -1f, 0.0f);
            vector3.x += (float) ((double) rect.xMin * (double) this.m_uiCamera.orthographicSize * 2.0) * this.m_uiCamera.aspect;
            vector3.y += (float) ((double) rect.yMin * (double) this.m_uiCamera.orthographicSize * 2.0);
            vector3.x += (float) ((double) this.QuadTransform.localScale.x * ((double) this.m_uiCamera.aspect / 1.7777777910232544) / 2.0);
            vector3.y += this.QuadTransform.localScale.y / 2f;
            this.QuadTransform.position = (this.m_uiCamera.transform.position + vector3).WithZ(3f);
            if (Minimap.Instance.IsFullscreen)
                this.m_quadMaterial.SetTexture(this.m_idMaskTex, this.MapMaskFullscreen);
            else
                this.m_quadMaterial.SetTexture(this.m_idMaskTex, this.MapMaskSmallscreen);
            int width = GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH ? 960 : 1920;
            int height = GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH ? 540 : 1080;
            if ((Object) this.m_currentQuadRenderTexture != (Object) null && (this.m_currentQuadRenderTexture.width != width || this.m_currentQuadRenderTexture.height != height))
            {
                RenderTexture.ReleaseTemporary(this.m_currentQuadRenderTexture);
                this.m_currentQuadRenderTexture = (RenderTexture) null;
            }
            if ((Object) this.m_currentQuadRenderTexture == (Object) null)
            {
                this.m_currentQuadRenderTexture = RenderTexture.GetTemporary(width, height);
                this.m_currentQuadRenderTexture.filterMode = FilterMode.Point;
                this.m_quadMaterial.SetTexture(this.m_idMainTex, (Texture) this.m_currentQuadRenderTexture);
            }
            if (!((Object) this.m_camera.targetTexture != (Object) this.m_currentQuadRenderTexture))
                return;
            this.m_camera.targetTexture = this.m_currentQuadRenderTexture;
        }

        private void LateUpdate() => this.CheckSize();

        private void OnDestroy()
        {
            if (!((Object) this.m_currentQuadRenderTexture != (Object) null))
                return;
            RenderTexture.ReleaseTemporary(this.m_currentQuadRenderTexture);
        }
    }

