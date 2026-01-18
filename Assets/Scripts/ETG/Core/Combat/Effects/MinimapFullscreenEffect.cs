using UnityEngine;

#nullable disable

public class MinimapFullscreenEffect : MonoBehaviour
  {
    public Shader shader;
    public Material materialInstance;
    public Camera slaveCamera;
    protected Camera m_camera;
    protected Material m_material;
    protected int m_cachedCullingMask;
    private int m_bgTexID = -1;
    private int m_bgTexUVID = -1;
    private int m_cameraRectID = -1;

    private void Awake()
    {
      this.m_camera = this.GetComponent<Camera>();
      this.m_cachedCullingMask = this.m_camera.cullingMask;
      this.m_camera.cullingMask = 0;
      if ((Object) this.materialInstance != (Object) null)
        this.m_material = this.materialInstance;
      else
        this.m_material = new Material(this.shader);
    }

    public void SetMaterial(Material m) => this.m_material = m;

    private void OnRenderImage(RenderTexture source, RenderTexture target)
    {
      if (GameManager.Instance.IsFoyer)
        return;
      this.slaveCamera.CopyFrom(this.m_camera);
      this.slaveCamera.clearFlags = CameraClearFlags.Color;
      Rect rect1 = new Rect(1f - Minimap.Instance.currentXRectFactor, 1f - Minimap.Instance.currentYRectFactor, Minimap.Instance.currentXRectFactor, Minimap.Instance.currentYRectFactor);
      if (!Minimap.Instance.IsFullscreen && GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !GameManager.Instance.IsLoadingLevel && (!GameManager.Instance.SecondaryPlayer.IsGhost || true))
        rect1.y -= 0.0875f;
      RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
      Graphics.Blit((Texture) Pixelator.SmallBlackTexture, temporary);
      this.slaveCamera.cullingMask = this.m_cachedCullingMask;
      this.slaveCamera.targetTexture = temporary;
      this.slaveCamera.Render();
      Rect rect2 = BraveCameraUtility.GetRect();
      Vector4 vector4_1 = new Vector4(rect1.xMin + rect2.xMin, rect1.yMin + rect2.yMin, rect1.width * rect2.width, rect1.height * rect2.height);
      Vector4 vector4_2 = new Vector4(rect1.xMin, rect1.yMin, rect1.width, rect1.height);
      if (this.m_bgTexID == -1)
      {
        this.m_bgTexID = Shader.PropertyToID("_BGTex");
        this.m_bgTexUVID = Shader.PropertyToID("_BGTexUV");
        this.m_cameraRectID = Shader.PropertyToID("_CameraRect");
      }
      this.m_material.SetTexture(this.m_bgTexID, (Texture) temporary);
      this.m_material.SetVector(this.m_bgTexUVID, vector4_2);
      this.m_material.SetVector(this.m_cameraRectID, vector4_1);
      Graphics.Blit((Texture) source, target, this.m_material);
      RenderTexture.ReleaseTemporary(temporary);
    }
  }

