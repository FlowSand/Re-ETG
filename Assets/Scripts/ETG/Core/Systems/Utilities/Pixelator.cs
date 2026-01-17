// Decompiled with JetBrains decompiler
// Type: Pixelator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class Pixelator : MonoBehaviour
    {
      public Texture2D localOcclusionTexture;
      private static Pixelator m_instance;
      public float occlusionRevealSpeed = 35f;
      public float occlusionTransitionFadeMultiplier = 4f;
      [NonSerialized]
      public float pointLightMultiplier = 1f;
      public Color occludedColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
      public AnimationCurve occlusionPerimeterCurve;
      public int perimeterTileWidth = 5;
      [Header("Vignette Settings")]
      public float vignettePower;
      public float damagedVignettePower = 0.5f;
      public Color vignetteColor = Color.black;
      public Color damagedVignetteColor = Color.red;
      public Shader vignetteShader;
      public Shader fadeShader;
      public Shader utilityShader;
      public bool UseTexturedOcclusion;
      public Texture2D ouchTexture;
      public Camera minimapCameraRef;
      public Texture2D sourceOcclusionTexture;
      public float saturation = 1f;
      public float fade = 1f;
      public bool DoMinimap = true;
      public bool DoRenderGBuffer = true;
      public bool DoOcclusionLayer = true;
      [NonSerialized]
      public bool ManualDoBloom = true;
      public bool PRECLUDE_DEPTH_RENDERING;
      [NonSerialized]
      public bool DoFinalNonFadedLayer;
      [NonSerialized]
      public bool CompositePixelatedUnfadedLayer;
      private List<bool> AdditionalRenderPassesInitialized = new List<bool>();
      private List<Material> AdditionalRenderPasses = new List<Material>();
      private bool m_hasInitializedAdditionalRenderTarget;
      public Material AdditionalCoreStackRenderPass;
      public int overrideTileScale = 1;
      public List<Camera> slavedCameras;
      private Camera m_camera;
      private Camera m_backgroundCamera;
      [SerializeField]
      private Material m_vignetteMaterial;
      [SerializeField]
      private Material m_combinedVignetteFadeMaterial;
      [SerializeField]
      private Material m_fadeMaterial;
      [NonSerialized]
      private Material m_backupFadeMaterial;
      [NonSerialized]
      private Material m_compositor;
      [NonSerialized]
      private Material m_pointLightMaterial;
      [NonSerialized]
      private Material m_pointLightMaterialFast;
      [NonSerialized]
      private Material m_coronalLightMaterial;
      [NonSerialized]
      private Material m_gbufferMaskMaterial;
      [SerializeField]
      private Material m_gbufferLightMaskCombinerMaterial;
      [SerializeField]
      private Material m_partialCopyMaterial;
      private static Texture2D m_smallBlackTexture;
      private Texture2D m_smallWhiteTexture;
      private RenderTexture m_texturedOcclusionTarget;
      private RenderTexture m_reflectionTargetTexture;
      private SENaturalBloomAndDirtyLens m_bloomer;
      public Camera AdditionalPreBGCamera;
      public Camera AdditionalBGCamera;
      public int NewTileScale = 3;
      [NonSerialized]
      public float CurrentTileScale = 3f;
      [NonSerialized]
      public float ScaleTileScale;
      private bool m_occlusionDirty;
      private OcclusionLayer occluder;
      private Transform m_gameQuadTransform;
      private int m_currentMacroResolutionX = 480;
      private int m_currentMacroResolutionY = 270;
      private int cm_occlusionPartition;
      private int cm_core1;
      private int cm_core2;
      private int cm_core3;
      private int cm_core4;
      private int cm_refl;
      private int cm_gbuffer;
      private int cm_gbufferSimple;
      private int cm_fg;
      private int cm_fg_important;
      private int cm_unoccluded;
      private int cm_unpixelated;
      private int cm_unfaded;
      private int PLATFORM_DEPTH;
      private RenderTextureFormat PLATFORM_RENDER_FORMAT;
      private Shader m_simpleSpriteMaskShader;
      private Shader m_simpleSpriteMaskUnpixelatedShader;
      public static bool DebugGraphicsInfo;
      private int m_gBufferID;
      private int m_saturationID;
      private int m_fadeID;
      private int m_fadeColorID;
      private int m_occlusionMapID;
      private int m_occlusionUVID;
      private int m_reflMapID;
      private int m_reflFlipID;
      private int m_gammaID;
      private int m_vignettePowerID;
      private int m_vignetteColorID;
      private int m_damagedTexID;
      private int m_cameraWSID;
      private int m_cameraOrthoSizeID;
      private int m_cameraOrthoSizeXID;
      private int m_lightPosID;
      private int m_lightColorID;
      private int m_lightRadiusID;
      private int m_lightIntensityID;
      private int m_lightCookieID;
      private int m_lightCookieAngleID;
      private int m_lightMaskTexID;
      private int m_preBackgroundTexID;
      private GenericFullscreenEffect m_gammaEffect;
      private float m_gammaAdjustment;
      public static bool AllowPS4MotionEnhancement;
      protected Dictionary<RoomHandler, IEnumerator> RoomOcclusionCoroutineMap = new Dictionary<RoomHandler, IEnumerator>();
      protected List<RoomHandler> ActiveOcclusionCoroutines = new List<RoomHandler>();
      private bool m_occlusionGridDirty;
      private List<IntVector2> m_modifiedRangeMins = new List<IntVector2>();
      private List<IntVector2> m_modifiedRangeMaxs = new List<IntVector2>();
      public int NUM_MACRO_PIXELS_HORIZONTAL = 480;
      public int NUM_MACRO_PIXELS_VERTICAL = 270;
      private bool generatedNewTexture;
      private IntVector2 oldBaseTile;
      [NonSerialized]
      public static bool IsRenderingOcclusionTexture;
      [NonSerialized]
      public static bool IsRenderingReflectionMap;
      private int m_uvRangeID = -1;
      public FilterMode DownsamplingFilterMode = FilterMode.Bilinear;
      private RenderTexture m_cachedFrame_VeryLowSettings;
      [NonSerialized]
      private bool m_timetubedInstance;
      private int extraPixels = 2;
      [NonSerialized]
      private RenderTexture m_UnblurredProjectileMaskTex;
      [NonSerialized]
      private RenderTexture m_BlurredProjectileMaskTex;
      public float ProjectileMaskBlurSize = 0.05f;
      private Material m_blurMaterial;
      public List<AdditionalBraveLight> AdditionalBraveLights = new List<AdditionalBraveLight>();
      private bool m_gammaLocked;
      private bool m_fadeLocked;
      [NonSerialized]
      public bool KillAllFades;
      private GenericFullscreenEffect m_gammaPass;
      public Vector3 CachedPlayerViewportPoint;
      public Vector3 CachedEnemyViewportPoint;
      public const int OCCLUSION_BUFFER = 2;
      private Dictionary<Shader, Material> _shaderMap = new Dictionary<Shader, Material>();

      public static Pixelator Instance
      {
        get
        {
          if ((UnityEngine.Object) Pixelator.m_instance == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) Pixelator.m_instance)
            Pixelator.m_instance = UnityEngine.Object.FindObjectOfType<Pixelator>();
          return Pixelator.m_instance;
        }
        set => Pixelator.m_instance = value;
      }

      public static bool HasInstance => (bool) (UnityEngine.Object) Pixelator.m_instance;

      public Vector3 CameraOrigin => this.m_camera.ViewportToWorldPoint(Vector3.zero);

      public Color FadeColor
      {
        get => this.m_fadeMaterial.GetColor("_FadeColor");
        set
        {
          if (!((UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null))
            return;
          this.m_fadeMaterial.SetColor("_FadeColor", value);
        }
      }

      public bool DoBloom
      {
        get
        {
          return this.ManualDoBloom && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW;
        }
      }

      public void RegisterAdditionalRenderPass(Material pass)
      {
        if (this.AdditionalRenderPasses.Contains(pass))
          return;
        this.AdditionalRenderPasses.Add(pass);
        this.AdditionalRenderPassesInitialized.Add(false);
      }

      public void DeregisterAdditionalRenderPass(Material pass)
      {
        if (!this.AdditionalRenderPasses.Contains(pass))
          return;
        int index = this.AdditionalRenderPasses.IndexOf(pass);
        if (index < 0)
          return;
        this.AdditionalRenderPassesInitialized.RemoveAt(index);
        this.AdditionalRenderPasses.RemoveAt(index);
      }

      public Material FadeMaterial => this.m_fadeMaterial;

      public static Texture2D SmallBlackTexture
      {
        get
        {
          if ((UnityEngine.Object) Pixelator.m_smallBlackTexture == (UnityEngine.Object) null)
          {
            Pixelator.m_smallBlackTexture = new Texture2D(1, 1);
            Pixelator.m_smallBlackTexture.SetPixel(0, 0, Color.black);
            Pixelator.m_smallBlackTexture.Apply();
          }
          return Pixelator.m_smallBlackTexture;
        }
      }

      public void SetOcclusionDirty()
      {
        this.m_occlusionGridDirty = true;
        this.m_occlusionDirty = true;
      }

      private float m_deltaTime => GameManager.INVARIANT_DELTA_TIME;

      public int CurrentMacroResolutionX
      {
        get => this.m_currentMacroResolutionX;
        set => this.m_currentMacroResolutionX = value;
      }

      public int CurrentMacroResolutionY
      {
        get => this.m_currentMacroResolutionY;
        set => this.m_currentMacroResolutionY = value;
      }

      public Rect CurrentCameraRect => this.m_camera.rect;

      private void InitializePerPlatform()
      {
        this.PLATFORM_DEPTH = 24;
        bool flag = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR);
        this.PLATFORM_RENDER_FORMAT = !flag ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
        if (flag)
          return;
        this.m_camera.hdr = false;
        this.GetComponent<SENaturalBloomAndDirtyLens>().enabled = false;
      }

      private void Awake()
      {
        AkAudioListener[] components = this.GetComponents<AkAudioListener>();
        for (int index = 0; index < components.Length; ++index)
        {
          if ((UnityEngine.Object) components[index] != (UnityEngine.Object) null)
            UnityEngine.Object.Destroy((UnityEngine.Object) components[index]);
        }
        this.m_gammaEffect = this.GetComponent<GenericFullscreenEffect>();
        this.m_reflMapID = Shader.PropertyToID("_ReflMapFromPixelator");
        this.m_reflFlipID = Shader.PropertyToID("_ReflectionYFactor");
        this.m_gammaID = Shader.PropertyToID("_GammaGamma");
        this.m_saturationID = Shader.PropertyToID("_Saturation");
        this.m_fadeID = Shader.PropertyToID("_Fade");
        this.m_fadeColorID = Shader.PropertyToID("_FadeColor");
        this.m_occlusionMapID = Shader.PropertyToID("_OcclusionMap");
        this.m_gBufferID = Shader.PropertyToID("_GBuffer");
        this.m_occlusionUVID = Shader.PropertyToID("_OcclusionUV");
        this.m_vignettePowerID = Shader.PropertyToID("_VignettePower");
        this.m_vignetteColorID = Shader.PropertyToID("_VignetteColor");
        this.m_damagedTexID = Shader.PropertyToID("_DamagedTex");
        this.m_cameraWSID = Shader.PropertyToID("_CameraWS");
        this.m_cameraOrthoSizeID = Shader.PropertyToID("_CameraOrthoSize");
        this.m_cameraOrthoSizeXID = Shader.PropertyToID("_CameraOrthoSizeX");
        this.m_lightPosID = Shader.PropertyToID("_LightPos");
        this.m_lightColorID = Shader.PropertyToID("_LightColor");
        this.m_lightRadiusID = Shader.PropertyToID("_LightRadius");
        this.m_lightIntensityID = Shader.PropertyToID("_LightIntensity");
        this.m_lightCookieID = Shader.PropertyToID("_LightCookie");
        this.m_lightCookieAngleID = Shader.PropertyToID("_LightCookieAngle");
        this.m_lightMaskTexID = Shader.PropertyToID("_LightMaskTex");
        this.m_preBackgroundTexID = Shader.PropertyToID("_PreBackgroundTex");
        this.m_camera = this.GetComponent<Camera>();
        this.m_simpleSpriteMaskShader = ShaderCache.Acquire("Brave/Internal/SimpleSpriteMask");
        this.m_simpleSpriteMaskUnpixelatedShader = ShaderCache.Acquire("Brave/Internal/SimpleSpriteMaskUnpixelated");
        this.InitializePerPlatform();
        BraveCameraUtility.MaintainCameraAspect(this.m_camera);
        if ((UnityEngine.Object) Pixelator.m_smallBlackTexture == (UnityEngine.Object) null)
        {
          Pixelator.m_smallBlackTexture = new Texture2D(1, 1);
          Pixelator.m_smallBlackTexture.SetPixel(0, 0, Color.black);
          Pixelator.m_smallBlackTexture.Apply();
        }
        this.m_smallWhiteTexture = new Texture2D(1, 1);
        this.m_smallWhiteTexture.SetPixel(0, 0, Color.white);
        this.m_smallWhiteTexture.Apply();
        this.m_bloomer = this.GetComponent<SENaturalBloomAndDirtyLens>();
        this.cm_occlusionPartition = 1 << LayerMask.NameToLayer("OcclusionRenderPartition");
        this.cm_core1 = 1 << LayerMask.NameToLayer("BG_Nonsense");
        this.cm_core2 = 1 << LayerMask.NameToLayer("BG_Critical");
        this.cm_core3 = 1 << LayerMask.NameToLayer("FG_Nonsense") | 1 << LayerMask.NameToLayer("ShadowCaster") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water");
        this.cm_refl = 1 << LayerMask.NameToLayer("FG_Reflection");
        this.cm_gbuffer = 1 << LayerMask.NameToLayer("FG_Nonsense") | 1 << LayerMask.NameToLayer("ShadowCaster") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("BG_Nonsense") | 1 << LayerMask.NameToLayer("BG_Critical") | 1 << LayerMask.NameToLayer("FG_Critical") | 1 << LayerMask.NameToLayer("FG_Reflection") | 1 << LayerMask.NameToLayer("Unpixelated") | 1 << LayerMask.NameToLayer("Unfaded");
        this.cm_gbufferSimple = 1 << LayerMask.NameToLayer("FG_Nonsense") | 1 << LayerMask.NameToLayer("ShadowCaster") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("BG_Nonsense") | 1 << LayerMask.NameToLayer("BG_Critical") | 1 << LayerMask.NameToLayer("FG_Critical") | 1 << LayerMask.NameToLayer("FG_Reflection") | 1 << LayerMask.NameToLayer("Unfaded");
        this.cm_fg = 1 << LayerMask.NameToLayer("FG_Nonsense") | 1 << LayerMask.NameToLayer("ShadowCaster") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("FG_Reflection") | 1 << LayerMask.NameToLayer("FG_Critical");
        this.cm_fg_important = 1 << LayerMask.NameToLayer("ShadowCaster") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("FG_Reflection") | 1 << LayerMask.NameToLayer("FG_Critical");
        this.cm_unoccluded = 1 << LayerMask.NameToLayer("Unoccluded");
        this.cm_unfaded = 1 << LayerMask.NameToLayer("Unfaded");
        if (GameManager.Options == null)
          GameOptions.Load();
        this.OnChangedMotionEnhancementMode(GameManager.Options.MotionEnhancementMode);
        this.OnChangedLightingQuality(GameManager.Options.LightingQuality);
      }

      public void OnChangedLightingQuality(
        GameOptions.GenericHighMedLowOption lightingQuality)
      {
        switch (lightingQuality)
        {
          case GameOptions.GenericHighMedLowOption.LOW:
          case GameOptions.GenericHighMedLowOption.VERY_LOW:
            this.m_gammaAdjustment = -0.1f;
            QualitySettings.pixelLightCount = 0;
            break;
          case GameOptions.GenericHighMedLowOption.MEDIUM:
            this.m_gammaAdjustment = 0.0f;
            QualitySettings.pixelLightCount = 4;
            break;
          case GameOptions.GenericHighMedLowOption.HIGH:
            this.m_gammaAdjustment = 0.0f;
            QualitySettings.pixelLightCount = 16 /*0x10*/;
            break;
        }
      }

      public void OnChangedMotionEnhancementMode(GameOptions.PixelatorMotionEnhancementMode newMode)
      {
        if (newMode == GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE)
        {
          this.cm_core4 = 1 << LayerMask.NameToLayer("FG_Critical") | 1 << LayerMask.NameToLayer("FG_Reflection");
          this.cm_unpixelated = 1 << LayerMask.NameToLayer("Unpixelated");
        }
        else if (newMode == GameOptions.PixelatorMotionEnhancementMode.UNENHANCED_CHEAP)
        {
          this.cm_core4 = 1 << LayerMask.NameToLayer("FG_Critical") | 1 << LayerMask.NameToLayer("FG_Reflection") | 1 << LayerMask.NameToLayer("Unpixelated");
          this.cm_unpixelated = 0;
        }
        else
          UnityEngine.Debug.LogError((object) "Unsupported MotionEnhancementMode in Pixelator. This should never, ever happen.");
      }

      public static void DEBUG_LogSystemRenderingData()
      {
        UnityEngine.Debug.Log((object) ("BRV::DeviceType = " + SystemInfo.deviceType.ToString()));
        UnityEngine.Debug.Log((object) ("BRV::GraphicsDeviceType = " + SystemInfo.graphicsDeviceName.ToString()));
        UnityEngine.Debug.Log((object) ("BRV::GraphicsDeviceType = " + SystemInfo.graphicsDeviceType.ToString()));
        UnityEngine.Debug.Log((object) ("BRV::GraphicsDeviceVendor = " + SystemInfo.graphicsDeviceVendor.ToString()));
        UnityEngine.Debug.Log((object) ("BRV::GraphicsDeviceVersion = " + SystemInfo.graphicsDeviceVersion.ToString()));
        UnityEngine.Debug.Log((object) ("BRV::GraphicsShaderLevel = " + (object) SystemInfo.graphicsShaderLevel));
        UnityEngine.Debug.Log((object) ("BRV::GraphicsMemorySize = " + (object) SystemInfo.graphicsMemorySize));
        UnityEngine.Debug.Log((object) ("BRV::MaxTextureSize = " + (object) SystemInfo.maxTextureSize));
        UnityEngine.Debug.Log((object) ("BRV::NPOTSupport = " + (object) SystemInfo.npotSupport));
        UnityEngine.Debug.Log((object) ("BRV::SupportedRenderTargetCount = " + (object) SystemInfo.supportedRenderTargetCount));
        UnityEngine.Debug.Log((object) ("BRV::SupportsImageEffects = " + (object) SystemInfo.supportsImageEffects));
        UnityEngine.Debug.Log((object) ("BRV::SupportsRenderTextures = " + (object) SystemInfo.supportsRenderTextures));
        UnityEngine.Debug.Log((object) ("BRV::SupportsStencil = " + (object) SystemInfo.supportsStencil));
        UnityEngine.Debug.Log((object) ("BRV::SupportsDefaultHDR = " + (object) SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR)));
        UnityEngine.Debug.Log((object) ("BRV::SupportsDepthFormat = " + (object) SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)));
        UnityEngine.Debug.Log((object) "BRV::Iteration = 1");
      }

      private bool IsInIntro
      {
        get
        {
          return GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER && Foyer.DoIntroSequence;
        }
      }

      private bool IsInTitle
      {
        get
        {
          return GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER && Foyer.DoMainMenu;
        }
      }

      private bool IsInPunchout => PunchoutController.IsActive;

      public void SetVignettePower(float tp)
      {
        if ((UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null)
          this.m_fadeMaterial.SetFloat(this.m_vignettePowerID, tp);
        if (!((UnityEngine.Object) this.m_combinedVignetteFadeMaterial != (UnityEngine.Object) null))
          return;
        this.m_combinedVignetteFadeMaterial.SetFloat(this.m_vignettePowerID, tp);
      }

      private void Start()
      {
        if (!this.IsInIntro)
          this.minimapCameraRef = Minimap.Instance.cameraRef;
        if ((UnityEngine.Object) GameManager.Instance.Dungeon != (UnityEngine.Object) null && GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.WESTGEON)
          this.UseTexturedOcclusion = true;
        if ((UnityEngine.Object) this.vignetteShader != (UnityEngine.Object) null)
        {
          this.m_vignetteMaterial = new Material(this.vignetteShader);
          this.m_vignetteMaterial.SetColor("_OcclusionFallbackColor", this.occludedColor);
        }
        if ((UnityEngine.Object) this.m_combinedVignetteFadeMaterial == (UnityEngine.Object) null)
        {
          this.m_combinedVignetteFadeMaterial = new Material(ShaderCache.Acquire("Brave/CameraEffects/Pixelator_VignetteFade"));
          this.m_combinedVignetteFadeMaterial.SetColor("_OcclusionFallbackColor", this.occludedColor);
          this.m_combinedVignetteFadeMaterial.SetFloat(this.m_vignettePowerID, this.vignettePower);
          this.m_combinedVignetteFadeMaterial.SetColor(this.m_vignetteColorID, this.vignetteColor);
          this.m_combinedVignetteFadeMaterial.SetTexture(this.m_damagedTexID, (Texture) this.ouchTexture);
          this.m_combinedVignetteFadeMaterial.SetVector("_LowlightColor", GameManager.Instance.BestGenerationDungeonPrefab.decoSettings.lowQualityCheapLightVector);
        }
        if ((UnityEngine.Object) this.fadeShader != (UnityEngine.Object) null)
        {
          this.m_fadeMaterial = new Material(this.fadeShader);
          this.m_fadeMaterial.SetFloat(this.m_vignettePowerID, this.vignettePower);
          this.m_fadeMaterial.SetColor(this.m_vignetteColorID, this.vignetteColor);
          this.m_fadeMaterial.SetTexture(this.m_damagedTexID, (Texture) this.ouchTexture);
        }
        this.m_pointLightMaterial = new Material(ShaderCache.Acquire("Brave/Internal/GBuffer_LightRenderer"));
        this.m_pointLightMaterialFast = new Material(ShaderCache.Acquire("Brave/Internal/GBuffer_LightRenderer_Fast"));
        this.m_gbufferMaskMaterial = new Material(ShaderCache.Acquire("Brave/Internal/GBuffer_LightMask"));
        this.m_gbufferLightMaskCombinerMaterial = new Material(ShaderCache.Acquire("Brave/Internal/GBuffer_LightMaskCombiner"));
        this.m_partialCopyMaterial = new Material(ShaderCache.Acquire("Brave/Internal/PartialCopy"));
        this.occluder = new OcclusionLayer();
        this.occluder.SourceOcclusionTexture = this.sourceOcclusionTexture;
        this.occluder.occludedColor = this.occludedColor;
        this.overrideTileScale = 1;
        this.CheckSize();
        this.StartCoroutine(this.BackgroundCoroutineProcessor());
        if ((bool) (UnityEngine.Object) GameManager.Instance.BestGenerationDungeonPrefab && GameManager.Instance.BestGenerationDungeonPrefab.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON)
          this.SetLumaGain(0.1f);
        else
          this.SetLumaGain(0.0f);
      }

      private void OnDestroy()
      {
        if (!((UnityEngine.Object) this.m_reflectionTargetTexture != (UnityEngine.Object) null))
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_reflectionTargetTexture);
      }

      public void MarkOcclusionDirty() => this.m_occlusionDirty = true;

      private bool IsExitDetailCell(CellData neighbor, CellData current) => neighbor.isExitNonOccluder;

      [DebuggerHidden]
      private IEnumerator BackgroundCoroutineProcessor()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CBackgroundCoroutineProcessor\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public float ProcessOcclusionChange(
        IntVector2 startingPosition,
        float targetVisibility,
        RoomHandler source,
        bool useFloodFill = true)
      {
        return this.HandleRoomOcclusionChange(startingPosition, source, useFloodFill);
      }

      public void ProcessRoomAdditionalExits(
        IntVector2 startingPosition,
        RoomHandler source,
        bool useFloodFill = true)
      {
        this.HandleRoomExitsCheck(startingPosition, source, useFloodFill);
      }

      protected List<CellData> GetExitCellsToProcess(
        IntVector2 startingPosition,
        RoomHandler targetRoom,
        RoomHandler currentVisibleRoom,
        DungeonData data)
      {
        List<CellData> exitCellsToProcess = new List<CellData>();
        if (!targetRoom.area.IsProceduralRoom)
        {
          for (int index = 0; index < targetRoom.area.instanceUsedExits.Count; ++index)
          {
            RuntimeRoomExitData exitToLocalData = targetRoom.area.exitToLocalDataMap[targetRoom.area.instanceUsedExits[index]];
            RuntimeExitDefinition runtimeExitDefinition = targetRoom.exitDefinitionsByExit[exitToLocalData];
            if ((!runtimeExitDefinition.downstreamRoom.IsSecretRoom || targetRoom != runtimeExitDefinition.upstreamRoom) && (!runtimeExitDefinition.upstreamRoom.IsSecretRoom || targetRoom != runtimeExitDefinition.downstreamRoom))
            {
              foreach (IntVector2 key in runtimeExitDefinition.GetCellsForRoom(targetRoom))
              {
                CellData cellData1 = data[key];
                if (cellData1 != null)
                  exitCellsToProcess.Add(cellData1);
                CellData cellData2 = data[cellData1.position + IntVector2.Up];
                if (cellData2 != null)
                  exitCellsToProcess.Add(cellData2);
                CellData cellData3 = data[cellData1.position + IntVector2.Up * 2];
                if (cellData3 != null)
                  exitCellsToProcess.Add(cellData3);
              }
              if (runtimeExitDefinition.upstreamExit != null && runtimeExitDefinition.upstreamExit.isWarpWingStart && (UnityEngine.Object) runtimeExitDefinition.upstreamExit.warpWingPortal != (UnityEngine.Object) null && (UnityEngine.Object) runtimeExitDefinition.upstreamExit.warpWingPortal.failPortal != (UnityEngine.Object) null && runtimeExitDefinition.upstreamExit.warpWingPortal.parentRoom == targetRoom)
              {
                foreach (IntVector2 key in runtimeExitDefinition.upstreamExit.warpWingPortal.failPortal.parentRoom.exitDefinitionsByExit[runtimeExitDefinition.upstreamExit.warpWingPortal.failPortal.parentExit].GetCellsForRoom(runtimeExitDefinition.upstreamExit.warpWingPortal.failPortal.parentRoom))
                {
                  BraveUtility.DrawDebugSquare(key.ToVector2(), Color.yellow, 1000f);
                  CellData cellData4 = data[key];
                  if (cellData4 != null)
                    exitCellsToProcess.Add(cellData4);
                  CellData cellData5 = data[cellData4.position + IntVector2.Up];
                  if (cellData5 != null)
                    exitCellsToProcess.Add(cellData5);
                  CellData cellData6 = data[cellData4.position + IntVector2.Up * 2];
                  if (cellData6 != null)
                    exitCellsToProcess.Add(cellData6);
                }
              }
              if (runtimeExitDefinition.downstreamExit != null && runtimeExitDefinition.downstreamExit.isWarpWingStart && (UnityEngine.Object) runtimeExitDefinition.downstreamExit.warpWingPortal != (UnityEngine.Object) null && (UnityEngine.Object) runtimeExitDefinition.downstreamExit.warpWingPortal.failPortal != (UnityEngine.Object) null && runtimeExitDefinition.downstreamExit.warpWingPortal.parentRoom == targetRoom)
              {
                foreach (IntVector2 key in runtimeExitDefinition.downstreamExit.warpWingPortal.failPortal.parentRoom.exitDefinitionsByExit[runtimeExitDefinition.downstreamExit.warpWingPortal.failPortal.parentExit].GetCellsForRoom(runtimeExitDefinition.downstreamExit.warpWingPortal.failPortal.parentRoom))
                {
                  BraveUtility.DrawDebugSquare(key.ToVector2(), Color.yellow, 1000f);
                  CellData cellData7 = data[key];
                  exitCellsToProcess.Add(cellData7);
                  CellData cellData8 = data[cellData7.position + IntVector2.Up];
                  exitCellsToProcess.Add(cellData8);
                  CellData cellData9 = data[cellData7.position + IntVector2.Up * 2];
                  exitCellsToProcess.Add(cellData9);
                }
              }
              if (runtimeExitDefinition.IntermediaryCells != null)
              {
                foreach (IntVector2 intermediaryCell in runtimeExitDefinition.IntermediaryCells)
                {
                  CellData cellData10 = data[intermediaryCell];
                  exitCellsToProcess.Add(cellData10);
                  CellData cellData11 = data[cellData10.position + IntVector2.Up];
                  exitCellsToProcess.Add(cellData11);
                  CellData cellData12 = data[cellData10.position + IntVector2.Up * 2];
                  exitCellsToProcess.Add(cellData12);
                }
              }
            }
          }
        }
        else
        {
          for (int index = 0; index < targetRoom.connectedRooms.Count; ++index)
          {
            RoomHandler connectedRoom = targetRoom.connectedRooms[index];
            PrototypeRoomExit exitConnectedToRoom = connectedRoom.GetExitConnectedToRoom(targetRoom);
            if (exitConnectedToRoom != null)
            {
              RuntimeExitDefinition runtimeExitDefinition = connectedRoom.exitDefinitionsByExit[connectedRoom.area.exitToLocalDataMap[exitConnectedToRoom]];
              foreach (IntVector2 key in runtimeExitDefinition.GetCellsForRoom(targetRoom))
              {
                CellData cellData13 = data[key];
                if (cellData13 != null)
                  exitCellsToProcess.Add(cellData13);
                CellData cellData14 = data[cellData13.position + IntVector2.Up];
                if (cellData14 != null)
                  exitCellsToProcess.Add(cellData14);
                CellData cellData15 = data[cellData13.position + IntVector2.Up * 2];
                if (cellData15 != null)
                  exitCellsToProcess.Add(cellData15);
              }
              if (runtimeExitDefinition.IntermediaryCells != null)
              {
                foreach (IntVector2 intermediaryCell in runtimeExitDefinition.IntermediaryCells)
                {
                  CellData cellData16 = data[intermediaryCell];
                  if (cellData16 != null)
                    exitCellsToProcess.Add(cellData16);
                  CellData cellData17 = data[cellData16.position + IntVector2.Up];
                  if (cellData17 != null)
                    exitCellsToProcess.Add(cellData17);
                  CellData cellData18 = data[cellData16.position + IntVector2.Up * 2];
                  if (cellData18 != null)
                    exitCellsToProcess.Add(cellData18);
                }
              }
            }
          }
        }
        return exitCellsToProcess;
      }

      protected void HandleRoomExitsCheck(
        IntVector2 startingPosition,
        RoomHandler targetRoom,
        bool useFloodFill = true)
      {
        int num1 = targetRoom.visibility != RoomHandler.VisibilityStatus.CURRENT ? -1 : 1;
        int num2 = targetRoom.visibility != RoomHandler.VisibilityStatus.VISITED ? 0 : 1;
        RoomHandler currentVisibleRoom = !((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null) ? GameManager.Instance.PrimaryPlayer.CurrentRoom : GameManager.Instance.Dungeon.data.Entrance;
        DungeonData data = GameManager.Instance.Dungeon.data;
        List<CellData> exitCellsToProcess = this.GetExitCellsToProcess(startingPosition, targetRoom, currentVisibleRoom, data);
        this.m_occlusionGridDirty = true;
        IntVector2 lhs1 = IntVector2.MaxValue;
        IntVector2 lhs2 = IntVector2.MinValue;
        for (int index = 0; index < exitCellsToProcess.Count; ++index)
        {
          CellData cellData = exitCellsToProcess[index];
          if (cellData != null)
          {
            float num3 = IntVector2.Distance(cellData.position, startingPosition);
            cellData.occlusionData.remainingDelay = !useFloodFill ? 0.0f : num3 / 35f;
            if (targetRoom.visibility == RoomHandler.VisibilityStatus.REOBSCURED)
            {
              cellData.occlusionData.cellRoomVisiblityCount = 0;
              cellData.occlusionData.cellRoomVisitedCount = 0;
              cellData.occlusionData.cellVisitedTargetOcclusion = 1f;
              cellData.occlusionData.minCellOccluionHistory = 1f;
            }
            else
            {
              cellData.occlusionData.cellRoomVisiblityCount = Mathf.RoundToInt(Mathf.Clamp01((float) num1));
              cellData.occlusionData.cellRoomVisitedCount = Mathf.RoundToInt(Mathf.Clamp01((float) num2));
              cellData.occlusionData.cellVisibleTargetOcclusion = 0.0f;
              cellData.occlusionData.cellVisitedTargetOcclusion = 0.7f;
            }
            cellData.occlusionData.cellOcclusionDirty = true;
            lhs1 = IntVector2.Min(lhs1, cellData.position);
            lhs2 = IntVector2.Max(lhs2, cellData.position);
          }
        }
        this.ProcessModifiedRanges(lhs1 + new IntVector2(-3, -3), lhs2 + new IntVector2(3, 3));
      }

      public void ProcessModifiedRanges(IntVector2 newMin, IntVector2 newMax)
      {
        bool flag = false;
        for (int index = 0; index < this.m_modifiedRangeMins.Count; ++index)
        {
          if (IntVector2.AABBOverlap(newMin, newMax - newMin, this.m_modifiedRangeMins[index], this.m_modifiedRangeMaxs[index] - this.m_modifiedRangeMins[index]))
          {
            this.m_modifiedRangeMins[index] = IntVector2.Min(this.m_modifiedRangeMins[index], newMin);
            this.m_modifiedRangeMaxs[index] = IntVector2.Max(this.m_modifiedRangeMaxs[index], newMax);
            flag = true;
            break;
          }
        }
        if (flag)
          return;
        this.m_modifiedRangeMins.Add(newMin);
        this.m_modifiedRangeMaxs.Add(newMax);
      }

      protected float HandleRoomOcclusionChange(
        IntVector2 startingPosition,
        RoomHandler targetRoom,
        bool useFloodFill = true)
      {
        if (targetRoom.PreventRevealEver)
          return 0.0f;
        int num1 = targetRoom.visibility != RoomHandler.VisibilityStatus.CURRENT ? -1 : 1;
        int num2 = targetRoom.visibility != RoomHandler.VisibilityStatus.VISITED ? 0 : 1;
        RoomHandler currentVisibleRoom = !((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null) ? GameManager.Instance.PrimaryPlayer.CurrentRoom : GameManager.Instance.Dungeon.data.Entrance;
        DungeonData data = GameManager.Instance.Dungeon.data;
        HashSet<CellData> cellDataSet = new HashSet<CellData>();
        for (int index1 = 0; index1 < targetRoom.CellsWithoutExits.Count; ++index1)
        {
          CellData cellData1 = data[targetRoom.CellsWithoutExits[index1]];
          if (cellData1 != null && (!cellData1.isSecretRoomCell || targetRoom.IsSecretRoom))
          {
            cellDataSet.Add(cellData1);
            if (cellData1.position.y + 1 < data.Height)
            {
              CellData cellData2 = data[cellData1.position + IntVector2.Up];
              if (cellData2 != null)
                cellDataSet.Add(cellData2);
            }
            if (cellData1.position.y + 2 < data.Height)
            {
              CellData cellData3 = data[cellData1.position + IntVector2.Up * 2];
              if (cellData3 != null)
                cellDataSet.Add(cellData3);
            }
            if (this.UseTexturedOcclusion)
            {
              for (int index2 = 0; index2 < IntVector2.Cardinals.Length; ++index2)
              {
                CellData cellData4 = data[cellData1.position + IntVector2.Cardinals[index2]];
                if (cellData4 != null)
                  cellDataSet.Add(cellData4);
              }
            }
          }
        }
        List<CellData> exitCellsToProcess = this.GetExitCellsToProcess(startingPosition, targetRoom, currentVisibleRoom, data);
        for (int index = 0; index < exitCellsToProcess.Count; ++index)
        {
          if (exitCellsToProcess[index] != null)
            cellDataSet.Add(exitCellsToProcess[index]);
        }
        for (int index = 0; index < targetRoom.FeatureCells.Count; ++index)
        {
          CellData cellData = data[targetRoom.FeatureCells[index]];
          if (cellData != null)
            cellDataSet.Add(cellData);
        }
        this.m_occlusionGridDirty = true;
        IntVector2 lhs1 = IntVector2.MaxValue;
        IntVector2 lhs2 = IntVector2.MinValue;
        float a = 0.0f;
        if ((double) this.occlusionRevealSpeed <= 0.0)
          useFloodFill = false;
        if (useFloodFill)
        {
          foreach (CellData cellData in cellDataSet)
          {
            if (cellData != null)
            {
              float num3 = IntVector2.Distance(cellData.position, startingPosition);
              cellData.occlusionData.remainingDelay = num3 / this.occlusionRevealSpeed;
              a = Mathf.Max(a, cellData.occlusionData.remainingDelay);
              if (targetRoom.visibility == RoomHandler.VisibilityStatus.REOBSCURED)
              {
                cellData.occlusionData.cellRoomVisiblityCount = 0;
                cellData.occlusionData.cellRoomVisitedCount = 0;
                cellData.occlusionData.cellVisibleTargetOcclusion = 1f;
                cellData.occlusionData.cellVisitedTargetOcclusion = 1f;
                cellData.occlusionData.minCellOccluionHistory = 1f;
              }
              else
              {
                cellData.occlusionData.cellRoomVisiblityCount = Mathf.RoundToInt(Mathf.Clamp01((float) num1));
                cellData.occlusionData.cellRoomVisitedCount = Mathf.RoundToInt(Mathf.Clamp01((float) num2));
                cellData.occlusionData.cellVisibleTargetOcclusion = 0.0f;
                cellData.occlusionData.cellVisitedTargetOcclusion = 0.7f;
              }
              cellData.occlusionData.cellOcclusionDirty = true;
              lhs1 = IntVector2.Min(lhs1, cellData.position);
              lhs2 = IntVector2.Max(lhs2, cellData.position);
            }
          }
        }
        else
        {
          foreach (CellData cellData in cellDataSet)
          {
            if (cellData != null)
            {
              cellData.occlusionData.remainingDelay = 0.0f;
              if (targetRoom.visibility == RoomHandler.VisibilityStatus.REOBSCURED)
              {
                cellData.occlusionData.cellRoomVisiblityCount = 0;
                cellData.occlusionData.cellRoomVisitedCount = 0;
                cellData.occlusionData.cellVisibleTargetOcclusion = 1f;
                cellData.occlusionData.cellVisitedTargetOcclusion = 1f;
                cellData.occlusionData.minCellOccluionHistory = 1f;
              }
              else
              {
                cellData.occlusionData.cellRoomVisiblityCount = Mathf.RoundToInt(Mathf.Clamp01((float) num1));
                cellData.occlusionData.cellRoomVisitedCount = Mathf.RoundToInt(Mathf.Clamp01((float) num2));
                cellData.occlusionData.cellVisibleTargetOcclusion = 0.0f;
                cellData.occlusionData.cellVisitedTargetOcclusion = 0.7f;
              }
              cellData.occlusionData.cellOcclusionDirty = true;
              lhs1 = IntVector2.Min(lhs1, cellData.position);
              lhs2 = IntVector2.Max(lhs2, cellData.position);
            }
          }
        }
        this.ProcessModifiedRanges(lhs1 + new IntVector2(-3, -3), lhs2 + new IntVector2(3, 3));
        return a;
      }

      private void CheckSize()
      {
        if ((double) GameManager.Instance.Dungeon.Height > (double) this.m_camera.farClipPlane)
          this.m_camera.farClipPlane = (float) (GameManager.Instance.Dungeon.Height + 50);
        this.CurrentMacroResolutionX = this.NUM_MACRO_PIXELS_HORIZONTAL;
        this.CurrentMacroResolutionY = this.NUM_MACRO_PIXELS_VERTICAL;
        this.CurrentTileScale = 3f;
        this.ScaleTileScale = Mathf.Max(1f, Mathf.Min(20f, (float) ((double) Screen.height * (double) this.m_camera.rect.height / 270.0)));
        BraveCameraUtility.MaintainCameraAspect(this.m_camera);
        for (int index = 0; index < this.slavedCameras.Count; ++index)
          BraveCameraUtility.MaintainCameraAspect(this.slavedCameras[index]);
        this.m_camera.orthographicSize = (float) this.NUM_MACRO_PIXELS_VERTICAL / 32f;
        if (!(bool) (UnityEngine.Object) this.m_backgroundCamera)
          this.m_backgroundCamera = BraveCameraUtility.GenerateBackgroundCamera(this.m_camera);
        if (!this.IsInIntro)
          GameUIRoot.Instance.UpdateScale();
        if (GameManager.Options.CurrentPreferredFullscreenMode == GameOptions.PreferredFullscreenMode.BORDERLESS)
          return;
        if (Screen.fullScreen && GameManager.Options.CurrentPreferredFullscreenMode != GameOptions.PreferredFullscreenMode.FULLSCREEN)
        {
          GameManager.Options.CurrentPreferredFullscreenMode = GameOptions.PreferredFullscreenMode.FULLSCREEN;
        }
        else
        {
          if (Screen.fullScreen || GameManager.Options.CurrentPreferredFullscreenMode != GameOptions.PreferredFullscreenMode.FULLSCREEN)
            return;
          GameManager.Options.CurrentPreferredFullscreenMode = GameOptions.PreferredFullscreenMode.WINDOWED;
        }
      }

      private int GBUFFER_DESCALE
      {
        get
        {
          switch (GameManager.Options.ShaderQuality)
          {
            case GameOptions.GenericHighMedLowOption.LOW:
              return 8;
            case GameOptions.GenericHighMedLowOption.MEDIUM:
              return 4;
            case GameOptions.GenericHighMedLowOption.HIGH:
              return 2;
            case GameOptions.GenericHighMedLowOption.VERY_LOW:
              return 8;
            default:
              return 8;
          }
        }
      }

      private void RenderOptionalMaps()
      {
        if (this.UseTexturedOcclusion)
          this.RenderOcclusionTexture(this.m_vignetteMaterial);
        else if ((UnityEngine.Object) this.m_texturedOcclusionTarget != (UnityEngine.Object) null)
        {
          this.m_texturedOcclusionTarget.Release();
          this.m_texturedOcclusionTarget = (RenderTexture) null;
          this.m_vignetteMaterial.SetTexture("_TextureOcclusionTex", (Texture) null);
        }
        if (GameManager.Options != null && GameManager.Options.RealtimeReflections)
        {
          this.RenderReflectionMap();
        }
        else
        {
          if (!((UnityEngine.Object) this.m_reflectionTargetTexture != (UnityEngine.Object) null))
            return;
          this.m_reflectionTargetTexture.Release();
          this.m_reflectionTargetTexture = (RenderTexture) null;
        }
      }

      private void RenderOcclusionTexture(Material targetVignetteMaterial)
      {
        Pixelator.IsRenderingOcclusionTexture = true;
        if ((UnityEngine.Object) this.m_texturedOcclusionTarget == (UnityEngine.Object) null)
        {
          this.m_texturedOcclusionTarget = new RenderTexture(this.NUM_MACRO_PIXELS_HORIZONTAL, this.NUM_MACRO_PIXELS_VERTICAL, 0, RenderTextureFormat.Default);
          this.m_texturedOcclusionTarget.hideFlags = HideFlags.DontSave;
          this.m_texturedOcclusionTarget.filterMode = FilterMode.Point;
          targetVignetteMaterial.SetTexture("_TextureOcclusionTex", (Texture) this.m_texturedOcclusionTarget);
        }
        Camera slavedCamera = this.slavedCameras[0];
        slavedCamera.CopyFrom(this.m_camera);
        slavedCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        slavedCamera.hdr = false;
        slavedCamera.targetTexture = this.m_texturedOcclusionTarget;
        slavedCamera.clearFlags = CameraClearFlags.Color;
        slavedCamera.backgroundColor = Color.clear;
        slavedCamera.cullingMask = this.cm_occlusionPartition;
        slavedCamera.Render();
        Pixelator.IsRenderingOcclusionTexture = false;
      }

      public Vector2 GetCurrentSmoothCameraOffset()
      {
        Vector3 vector3_1 = this.m_camera.transform.position - (!this.IsInIntro ? CameraController.PLATFORM_CAMERA_OFFSET : Vector3.zero);
        Vector3 vector3_2 = new Vector3(Mathf.Round(vector3_1.x * 16f), Mathf.Round(vector3_1.y * 16f), Mathf.Round(vector3_1.z * 16f)) / 16f;
        return (vector3_1 - vector3_2).XY();
      }

      public IntVector2 GetCurrentMicropixelOffset()
      {
        Vector2 smoothCameraOffset = this.GetCurrentSmoothCameraOffset();
        return new IntVector2(Mathf.RoundToInt(smoothCameraOffset.x / (1f / 16f / this.ScaleTileScale)), Mathf.RoundToInt(smoothCameraOffset.y / (1f / 16f / this.ScaleTileScale)));
      }

      private void RenderReflectionMap()
      {
        Pixelator.IsRenderingReflectionMap = true;
        Vector3 vector3_1 = !this.IsInIntro ? CameraController.PLATFORM_CAMERA_OFFSET : Vector3.zero;
        Vector3 vector3_2 = this.m_camera.transform.position - vector3_1;
        this.m_camera.transform.position = new Vector3(Mathf.Round(vector3_2.x * 16f) - 1f, Mathf.Round(vector3_2.y * 16f) - 1f, Mathf.Round(vector3_2.z * 16f)) / 16f + vector3_1;
        if ((UnityEngine.Object) this.m_reflectionTargetTexture == (UnityEngine.Object) null || this.m_reflectionTargetTexture.width != this.NUM_MACRO_PIXELS_HORIZONTAL || this.m_reflectionTargetTexture.height != this.NUM_MACRO_PIXELS_VERTICAL)
        {
          if ((UnityEngine.Object) this.m_reflectionTargetTexture != (UnityEngine.Object) null)
            this.m_reflectionTargetTexture.Release();
          this.m_reflectionTargetTexture = new RenderTexture(this.NUM_MACRO_PIXELS_HORIZONTAL, this.NUM_MACRO_PIXELS_VERTICAL, 0, RenderTextureFormat.Default);
          this.m_reflectionTargetTexture.hideFlags = HideFlags.DontSave;
          this.m_reflectionTargetTexture.filterMode = FilterMode.Bilinear;
          Shader.SetGlobalTexture(this.m_reflMapID, (Texture) this.m_reflectionTargetTexture);
        }
        Shader.SetGlobalFloat(this.m_reflFlipID, 2f);
        Camera slavedCamera = this.slavedCameras[0];
        slavedCamera.CopyFrom(this.m_camera);
        slavedCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        slavedCamera.hdr = true;
        slavedCamera.targetTexture = this.m_reflectionTargetTexture;
        CameraClearFlags cameraClearFlags = CameraClearFlags.Color;
        slavedCamera.clearFlags = cameraClearFlags;
        slavedCamera.backgroundColor = Color.clear;
        slavedCamera.cullingMask = this.cm_refl;
        slavedCamera.Render();
        Shader.SetGlobalFloat(this.m_reflFlipID, 0.0f);
        this.m_camera.transform.position = vector3_2 + vector3_1;
        Pixelator.IsRenderingReflectionMap = false;
      }

      public void SetLumaGain(float gain) => this.m_gammaEffect.ActiveMaterial.SetFloat("_Gain", gain);

      private void CalculateDepixelatedOffset(
        Vector3 cachedPosition,
        Vector3 quantizedPosition,
        int corePixelatedWidth,
        int corePixelatedHeight,
        RenderTexture referenceBufferA)
      {
        Vector2 vector2 = (cachedPosition.XY() - quantizedPosition.XY()) * 16f;
        vector2.x /= (float) referenceBufferA.width;
        vector2.y /= (float) referenceBufferA.height;
        Vector4 vector4 = new Vector4(vector2.x, vector2.y, vector2.x + (float) corePixelatedWidth / (float) referenceBufferA.width, vector2.y + (float) corePixelatedHeight / (float) referenceBufferA.height);
        if (this.m_uvRangeID == -1)
          this.m_uvRangeID = Shader.PropertyToID("_UVRange");
        this.m_partialCopyMaterial.SetVector(this.m_uvRangeID, vector4);
        if (!((UnityEngine.Object) this.m_gbufferLightMaskCombinerMaterial != (UnityEngine.Object) null))
          return;
        this.m_gbufferLightMaskCombinerMaterial.SetVector(this.m_uvRangeID, vector4);
      }

      private void HandlePreDeathFramingLogic()
      {
        if (!this.CacheCurrentFrameToBuffer || GameManager.Instance.AllPlayers == null)
          return;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index])
            GameManager.Instance.AllPlayers[index].ToggleHandRenderers(false, "death");
        }
      }

      private void DoOcclusionUpdate(Material targetVignetteMaterial)
      {
        if (this.DoOcclusionLayer && !this.IsInIntro && !GameManager.Instance.IsSelectingCharacter)
        {
          Vector3 vector1 = this.m_camera.transform.position + new Vector3(BraveCameraUtility.ASPECT * -this.m_camera.orthographicSize, -this.m_camera.orthographicSize, 0.0f);
          Vector3 vector2 = vector1 + new Vector3((float) (this.CurrentMacroResolutionX / 16 /*0x10*/), (float) (this.CurrentMacroResolutionY / 16 /*0x10*/), 0.0f);
          IntVector2 intVector2 = vector1.IntXY();
          if (this.generatedNewTexture && (UnityEngine.Object) targetVignetteMaterial != (UnityEngine.Object) null && (UnityEngine.Object) this.occluder.SourceOcclusionTexture != (UnityEngine.Object) null)
            targetVignetteMaterial.SetTexture(this.m_occlusionMapID, (Texture) this.occluder.SourceOcclusionTexture);
          this.generatedNewTexture = false;
          if ((UnityEngine.Object) this.localOcclusionTexture == (UnityEngine.Object) null || this.occluder.cachedX != intVector2.x - 2 || this.occluder.cachedY != intVector2.y - 2 || this.m_occlusionDirty)
          {
            this.m_occlusionDirty = false;
            this.generatedNewTexture = true;
            this.occluder.GenerateOcclusionTexture(intVector2.x - 2, intVector2.y - 2, GameManager.Instance.Dungeon.data);
            this.localOcclusionTexture = this.occluder.SourceOcclusionTexture;
            if ((UnityEngine.Object) targetVignetteMaterial != (UnityEngine.Object) null && (UnityEngine.Object) this.occluder.SourceOcclusionTexture != (UnityEngine.Object) null)
              targetVignetteMaterial.SetTexture(this.m_occlusionMapID, (Texture) this.occluder.SourceOcclusionTexture);
          }
          if (!((UnityEngine.Object) targetVignetteMaterial != (UnityEngine.Object) null))
            return;
          Vector2 vector2_1 = vector1.XY() - intVector2.ToVector2();
          Vector2 vector2_2 = vector2.XY() - (intVector2 + new IntVector2(this.CurrentMacroResolutionX / 16 /*0x10*/, this.CurrentMacroResolutionY / 16 /*0x10*/)).ToVector2();
          int num1 = this.CurrentMacroResolutionX / 16 /*0x10*/ + 4;
          int num2 = this.CurrentMacroResolutionY / 16 /*0x10*/ + 4;
          float num3 = 2f;
          Vector4 vector4 = new Vector4((num3 + vector2_1.x) / (float) num1, (num3 + vector2_1.y) / (float) num2, (float) (1.0 - ((double) num3 - (double) vector2_2.x) / (double) num1), (float) (1.0 - ((double) num3 - (double) vector2_2.y) / (double) num2));
          if (!((UnityEngine.Object) targetVignetteMaterial != (UnityEngine.Object) null))
            return;
          targetVignetteMaterial.SetVector(this.m_occlusionUVID, vector4);
        }
        else
        {
          if ((UnityEngine.Object) this.localOcclusionTexture == (UnityEngine.Object) null || this.localOcclusionTexture.width > 1)
          {
            this.localOcclusionTexture = new Texture2D(1, 1);
            this.localOcclusionTexture.SetPixel(0, 0, new Color(0.0f, 1f, 1f, 1f));
            this.localOcclusionTexture.Apply();
          }
          if (!((UnityEngine.Object) targetVignetteMaterial != (UnityEngine.Object) null) || !((UnityEngine.Object) this.localOcclusionTexture != (UnityEngine.Object) null))
            return;
          targetVignetteMaterial.SetTexture(this.m_occlusionMapID, (Texture) this.localOcclusionTexture);
        }
      }

      private bool ShouldOverrideMultiplexing() => false;

      private void OnRenderImage(RenderTexture source, RenderTexture target)
      {
        Dungeon dungeon = GameManager.Instance.Dungeon;
        if ((UnityEngine.Object) dungeon == (UnityEngine.Object) null || dungeon.data == null || dungeon.data.cellData == null)
          UnityEngine.Debug.LogWarningFormat("No dungeon data found! {0} {1} {2}", (object) ((UnityEngine.Object) dungeon == (UnityEngine.Object) null), (object) ((UnityEngine.Object) dungeon == (UnityEngine.Object) null || dungeon.data == null), (object) ((UnityEngine.Object) dungeon == (UnityEngine.Object) null || dungeon.data == null || dungeon.data.cellData == null));
        else if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW && !this.IsInIntro && !this.IsInTitle && !this.IsInPunchout)
        {
          if ((UnityEngine.Object) this.m_backupFadeMaterial == (UnityEngine.Object) null && (UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null)
          {
            this.m_backupFadeMaterial = this.m_fadeMaterial;
            this.m_fadeMaterial = this.m_combinedVignetteFadeMaterial;
          }
          if ((bool) (UnityEngine.Object) this.m_combinedVignetteFadeMaterial)
          {
            if (GameManager.Options.LightingQuality == GameOptions.GenericHighMedLowOption.LOW && !this.m_combinedVignetteFadeMaterial.IsKeywordEnabled("LOWLIGHT_ON"))
            {
              this.m_combinedVignetteFadeMaterial.DisableKeyword("LOWLIGHT_OFF");
              this.m_combinedVignetteFadeMaterial.EnableKeyword("LOWLIGHT_ON");
            }
            else if (GameManager.Options.LightingQuality == GameOptions.GenericHighMedLowOption.HIGH && !this.m_combinedVignetteFadeMaterial.IsKeywordEnabled("LOWLIGHT_OFF"))
            {
              this.m_combinedVignetteFadeMaterial.DisableKeyword("LOWLIGHT_ON");
              this.m_combinedVignetteFadeMaterial.EnableKeyword("LOWLIGHT_OFF");
            }
          }
          if ((bool) (UnityEngine.Object) this.m_gammaEffect && (bool) (UnityEngine.Object) this.m_gammaEffect.ActiveMaterial)
          {
            if (this.m_gammaEffect.enabled)
            {
              this.m_combinedVignetteFadeMaterial.SetTexture(this.m_occlusionMapID, (Texture) this.occluder.SourceOcclusionTexture);
              this.m_gammaEffect.enabled = false;
            }
            this.m_combinedVignetteFadeMaterial.SetFloat(this.m_gammaID, this.m_gammaEffect.ActiveMaterial.GetFloat(this.m_gammaID));
          }
          this.RenderGame_Combined(source, target, Pixelator.CoreRenderMode.LOW_QUALITY);
        }
        else
        {
          if ((bool) (UnityEngine.Object) this.m_gammaEffect && !this.m_gammaEffect.enabled)
          {
            this.m_vignetteMaterial.SetTexture(this.m_occlusionMapID, (Texture) this.occluder.SourceOcclusionTexture);
            this.m_gammaEffect.enabled = true;
          }
          GameOptions.PreferredScalingMode preferredScalingMode = GameManager.Options.CurrentPreferredScalingMode;
          if (this.IsInIntro && preferredScalingMode == GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
            preferredScalingMode = GameOptions.PreferredScalingMode.UNIFORM_SCALING;
          if ((preferredScalingMode == GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST || this.ShouldOverrideMultiplexing()) && !this.IsInPunchout)
          {
            if ((UnityEngine.Object) this.m_backupFadeMaterial == (UnityEngine.Object) null && (UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null)
            {
              this.m_backupFadeMaterial = this.m_fadeMaterial;
              this.m_fadeMaterial = this.m_combinedVignetteFadeMaterial;
            }
            if (this.m_combinedVignetteFadeMaterial.IsKeywordEnabled("LOWLIGHT_ON"))
            {
              this.m_combinedVignetteFadeMaterial.DisableKeyword("LOWLIGHT_ON");
              this.m_combinedVignetteFadeMaterial.EnableKeyword("LOWLIGHT_OFF");
            }
            if ((bool) (UnityEngine.Object) this.m_gammaEffect && (bool) (UnityEngine.Object) this.m_gammaEffect.ActiveMaterial)
            {
              this.m_combinedVignetteFadeMaterial.SetTexture(this.m_occlusionMapID, (Texture) this.occluder.SourceOcclusionTexture);
              if (!this.m_gammaEffect.enabled)
                this.m_gammaEffect.enabled = true;
              this.m_combinedVignetteFadeMaterial.SetFloat(this.m_gammaID, 1f);
            }
            this.RenderGame_Combined(source, target, Pixelator.CoreRenderMode.FAST_SCALING);
          }
          else
          {
            if ((UnityEngine.Object) this.m_backupFadeMaterial != (UnityEngine.Object) null)
            {
              this.m_fadeMaterial = this.m_backupFadeMaterial;
              this.m_backupFadeMaterial = (Material) null;
            }
            this.RenderGame_Pretty(source, target);
          }
        }
      }

      public RenderTexture GetCachedFrame_VeryLowSettings() => this.m_cachedFrame_VeryLowSettings;

      public void ClearCachedFrame_VeryLowSettings()
      {
        if ((UnityEngine.Object) this.m_cachedFrame_VeryLowSettings != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(this.m_cachedFrame_VeryLowSettings);
        this.m_cachedFrame_VeryLowSettings = (RenderTexture) null;
      }

      private void RenderGame_Combined(
        RenderTexture source,
        RenderTexture target,
        Pixelator.CoreRenderMode renderMode)
      {
        this.HandlePreDeathFramingLogic();
        if (renderMode == Pixelator.CoreRenderMode.LOW_QUALITY)
        {
          if ((bool) (UnityEngine.Object) this.m_bloomer && this.m_bloomer.enabled)
            this.m_bloomer.enabled = false;
        }
        else
        {
          this.RenderOptionalMaps();
          if ((bool) (UnityEngine.Object) this.m_bloomer && this.DoBloom && !this.m_bloomer.enabled || !this.DoBloom && this.m_bloomer.enabled)
            this.m_bloomer.enabled = this.DoBloom;
        }
        this.CheckSize();
        BraveCameraUtility.MaintainCameraAspect(this.m_camera);
        if (renderMode == Pixelator.CoreRenderMode.NORMAL)
          this.DoOcclusionUpdate(this.m_vignetteMaterial);
        else
          this.DoOcclusionUpdate(this.m_combinedVignetteFadeMaterial);
        int num1 = this.CurrentMacroResolutionX / this.overrideTileScale;
        int num2 = this.CurrentMacroResolutionY / this.overrideTileScale;
        Camera slavedCamera = this.slavedCameras[0];
        slavedCamera.CopyFrom(this.m_camera);
        slavedCamera.orthographicSize = this.m_camera.orthographicSize;
        slavedCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        slavedCamera.hdr = renderMode != Pixelator.CoreRenderMode.LOW_QUALITY && (bool) (UnityEngine.Object) this.m_bloomer && this.m_bloomer.enabled;
        RenderTexture temp = (RenderTexture) null;
        if ((UnityEngine.Object) this.AdditionalPreBGCamera != (UnityEngine.Object) null)
        {
          temp = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
          temp.filterMode = FilterMode.Point;
          this.AdditionalPreBGCamera.enabled = false;
          this.AdditionalPreBGCamera.clearFlags = CameraClearFlags.Color;
          this.AdditionalPreBGCamera.backgroundColor = Color.black;
          this.AdditionalPreBGCamera.targetTexture = temp;
          this.AdditionalPreBGCamera.Render();
          Shader.SetGlobalTexture(this.m_preBackgroundTexID, (Texture) temp);
        }
        RenderTexture renderTexture = RenderTexture.GetTemporary(num1, num2, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
        renderTexture.filterMode = FilterMode.Point;
        slavedCamera.targetTexture = renderTexture;
        CameraClearFlags cameraClearFlags = CameraClearFlags.Color;
        if ((UnityEngine.Object) this.AdditionalBGCamera != (UnityEngine.Object) null)
        {
          this.AdditionalBGCamera.enabled = false;
          this.AdditionalBGCamera.clearFlags = CameraClearFlags.Color;
          cameraClearFlags = CameraClearFlags.Nothing;
          this.AdditionalBGCamera.backgroundColor = Color.black;
          this.AdditionalBGCamera.targetTexture = renderTexture;
          this.AdditionalBGCamera.Render();
        }
        slavedCamera.clearFlags = cameraClearFlags;
        slavedCamera.backgroundColor = Color.black;
        slavedCamera.cullingMask = this.cm_core1 | this.cm_core2;
        slavedCamera.Render();
        slavedCamera.clearFlags = CameraClearFlags.Depth;
        slavedCamera.backgroundColor = Color.clear;
        slavedCamera.cullingMask = renderMode != Pixelator.CoreRenderMode.FAST_SCALING ? this.cm_core3 | this.cm_core4 : this.cm_core3 | this.cm_core4 | 1 << LayerMask.NameToLayer("Unpixelated");
        slavedCamera.Render();
        if ((UnityEngine.Object) this.AdditionalCoreStackRenderPass != (UnityEngine.Object) null)
        {
          if (TimeTubeCreditsController.IsTimeTubing || this.m_timetubedInstance)
          {
            this.m_timetubedInstance = true;
            Graphics.Blit((Texture) renderTexture, renderTexture, this.AdditionalCoreStackRenderPass);
          }
          else
          {
            RenderTexture temporary = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, renderTexture.depth, renderTexture.format);
            temporary.filterMode = FilterMode.Point;
            Graphics.Blit((Texture) renderTexture, temporary, this.AdditionalCoreStackRenderPass);
            RenderTexture.ReleaseTemporary(renderTexture);
            renderTexture = temporary;
          }
        }
        if ((UnityEngine.Object) this.AdditionalPreBGCamera != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(temp);
        RenderTexture temporary1 = RenderTexture.GetTemporary(source.width, source.height, 0, this.PLATFORM_RENDER_FORMAT);
        temporary1.filterMode = FilterMode.Point;
        Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, temporary1);
        switch (renderMode)
        {
          case Pixelator.CoreRenderMode.LOW_QUALITY:
            this.RenderGBufferCheap(source, slavedCamera, renderTexture.depthBuffer, temporary1, num1, num2);
            break;
          case Pixelator.CoreRenderMode.FAST_SCALING:
            this.RenderGBufferScaling(source, slavedCamera, renderTexture.depthBuffer, temporary1);
            break;
        }
        if (this.AdditionalRenderPasses.Count > 0)
        {
          for (int index = 0; index < this.AdditionalRenderPasses.Count; ++index)
          {
            if ((UnityEngine.Object) this.AdditionalRenderPasses[index] == (UnityEngine.Object) null)
            {
              this.AdditionalRenderPasses.RemoveAt(index);
              --index;
            }
            else
            {
              RenderTexture temporary2 = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, renderTexture.depth, renderTexture.format);
              Graphics.Blit((Texture) renderTexture, temporary2, this.AdditionalRenderPasses[index], 0);
              if (this.AdditionalRenderPassesInitialized[index])
              {
                RenderTexture.ReleaseTemporary(renderTexture);
                renderTexture = temporary2;
              }
              else
              {
                this.AdditionalRenderPassesInitialized[index] = true;
                RenderTexture.ReleaseTemporary(temporary2);
              }
            }
          }
        }
        else if (!this.m_hasInitializedAdditionalRenderTarget)
        {
          RenderTexture temporary3 = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, renderTexture.depth, renderTexture.format);
          Graphics.Blit((Texture) renderTexture, temporary3);
          RenderTexture.ReleaseTemporary(renderTexture);
          renderTexture = temporary3;
          this.m_hasInitializedAdditionalRenderTarget = true;
        }
        if (!this.m_gammaLocked)
          this.m_gammaEffect.ActiveMaterial.SetFloat(this.m_gammaID, 2f - GameManager.Options.Gamma + this.m_gammaAdjustment);
        if ((UnityEngine.Object) this.m_combinedVignetteFadeMaterial != (UnityEngine.Object) null)
        {
          this.m_combinedVignetteFadeMaterial.SetTexture(this.m_gBufferID, (Texture) temporary1);
          this.m_combinedVignetteFadeMaterial.SetFloat(this.m_saturationID, this.saturation);
          this.m_combinedVignetteFadeMaterial.SetFloat(this.m_fadeID, this.fade);
        }
        if (this.DoFinalNonFadedLayer && (UnityEngine.Object) this.m_combinedVignetteFadeMaterial != (UnityEngine.Object) null)
        {
          Graphics.Blit((Texture) renderTexture, target, this.m_combinedVignetteFadeMaterial);
          BraveCameraUtility.MaintainCameraAspect(slavedCamera);
          if (this.CompositePixelatedUnfadedLayer)
          {
            RenderTexture temporary4 = RenderTexture.GetTemporary(BraveCameraUtility.H_PIXELS, BraveCameraUtility.V_PIXELS);
            temporary4.filterMode = FilterMode.Point;
            slavedCamera.targetTexture = temporary4;
            slavedCamera.clearFlags = CameraClearFlags.Color;
            slavedCamera.backgroundColor = new Color(1f, 0.0f, 0.0f);
            slavedCamera.cullingMask = this.cm_unfaded;
            slavedCamera.Render();
            if ((UnityEngine.Object) this.m_compositor == (UnityEngine.Object) null)
              this.m_compositor = new Material(ShaderCache.Acquire("Hidden/SimpleCompositor"));
            this.m_compositor.SetTexture("_BaseTex", (Texture) target);
            this.m_compositor.SetTexture("_LayerTex", (Texture) temporary4);
            Graphics.Blit((Texture) temporary4, target, this.m_compositor);
            RenderTexture.ReleaseTemporary(temporary4);
          }
          else
          {
            slavedCamera.targetTexture = target;
            slavedCamera.clearFlags = CameraClearFlags.Depth;
            slavedCamera.cullingMask = this.cm_unfaded;
            slavedCamera.Render();
          }
        }
        else if ((UnityEngine.Object) this.m_combinedVignetteFadeMaterial != (UnityEngine.Object) null)
          Graphics.Blit((Texture) renderTexture, target, this.m_combinedVignetteFadeMaterial);
        else
          Graphics.Blit((Texture) renderTexture, target);
        BraveCameraUtility.MaintainCameraAspect(slavedCamera);
        slavedCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        slavedCamera.targetTexture = target;
        slavedCamera.clearFlags = CameraClearFlags.Depth;
        slavedCamera.cullingMask = this.cm_unoccluded;
        slavedCamera.Render();
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(renderTexture);
        this.m_camera.cullingMask = 0;
        slavedCamera.cullingMask = 0;
        if (!this.CacheCurrentFrameToBuffer)
          return;
        this.ClearCachedFrame_VeryLowSettings();
        this.m_cachedFrame_VeryLowSettings = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, renderTexture.format);
        this.m_cachedFrame_VeryLowSettings.filterMode = FilterMode.Point;
        Graphics.Blit((Texture) renderTexture, this.m_cachedFrame_VeryLowSettings);
        this.CacheCurrentFrameToBuffer = false;
      }

      private void RenderGame_Pretty(RenderTexture source, RenderTexture target)
      {
        bool zoomIntermediate = GameManager.Instance.MainCameraController.IsCurrentlyZoomIntermediate;
        this.HandlePreDeathFramingLogic();
        if (Pixelator.DebugGraphicsInfo)
          Pixelator.DEBUG_LogSystemRenderingData();
        this.RenderOptionalMaps();
        if ((bool) (UnityEngine.Object) this.m_bloomer && this.DoBloom && !this.m_bloomer.enabled || !this.DoBloom && this.m_bloomer.enabled)
          this.m_bloomer.enabled = this.DoBloom;
        this.CheckSize();
        BraveCameraUtility.MaintainCameraAspect(this.m_camera);
        this.DoOcclusionUpdate(this.m_vignetteMaterial);
        int corePixelatedWidth = this.CurrentMacroResolutionX / this.overrideTileScale;
        int corePixelatedHeight = this.CurrentMacroResolutionY / this.overrideTileScale;
        int width = corePixelatedWidth + this.extraPixels;
        int height = corePixelatedHeight + this.extraPixels;
        Vector3 vector3 = !this.IsInIntro ? CameraController.PLATFORM_CAMERA_OFFSET : Vector3.zero;
        Vector3 cachedPosition = this.m_camera.transform.position - vector3;
        Vector3 quantizedPosition = new Vector3(Mathf.Round(cachedPosition.x * 16f) - 1f, Mathf.Round(cachedPosition.y * 16f) - 1f, Mathf.Round(cachedPosition.z * 16f)) / 16f + vector3;
        this.m_camera.transform.position = quantizedPosition;
        Camera slavedCamera = this.slavedCameras[0];
        slavedCamera.CopyFrom(this.m_camera);
        slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
        slavedCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
        slavedCamera.hdr = true;
        RenderTexture temp1 = (RenderTexture) null;
        if ((UnityEngine.Object) this.AdditionalPreBGCamera != (UnityEngine.Object) null)
        {
          temp1 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
          temp1.filterMode = FilterMode.Point;
          this.AdditionalPreBGCamera.enabled = false;
          this.AdditionalPreBGCamera.clearFlags = CameraClearFlags.Color;
          this.AdditionalPreBGCamera.backgroundColor = Color.black;
          this.AdditionalPreBGCamera.targetTexture = temp1;
          this.AdditionalPreBGCamera.Render();
          Shader.SetGlobalTexture(this.m_preBackgroundTexID, (Texture) temp1);
        }
        RenderTexture renderTexture1 = RenderTexture.GetTemporary(width, height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
        renderTexture1.filterMode = FilterMode.Point;
        slavedCamera.targetTexture = renderTexture1;
        CameraClearFlags cameraClearFlags = CameraClearFlags.Color;
        if ((UnityEngine.Object) this.AdditionalBGCamera != (UnityEngine.Object) null)
        {
          this.AdditionalBGCamera.enabled = false;
          this.AdditionalBGCamera.clearFlags = CameraClearFlags.Color;
          cameraClearFlags = CameraClearFlags.Nothing;
          this.AdditionalBGCamera.backgroundColor = Color.black;
          this.AdditionalBGCamera.targetTexture = renderTexture1;
          this.AdditionalBGCamera.Render();
        }
        slavedCamera.clearFlags = cameraClearFlags;
        slavedCamera.backgroundColor = Color.black;
        slavedCamera.cullingMask = this.cm_core1 | this.cm_core2;
        slavedCamera.Render();
        slavedCamera.clearFlags = CameraClearFlags.Depth;
        slavedCamera.backgroundColor = Color.clear;
        slavedCamera.cullingMask = this.cm_core3 | this.cm_core4;
        slavedCamera.Render();
        slavedCamera.orthographicSize = this.m_camera.orthographicSize;
        this.m_camera.transform.position = cachedPosition + vector3;
        RenderTexture temp2 = (RenderTexture) null;
        if ((UnityEngine.Object) this.AdditionalCoreStackRenderPass != (UnityEngine.Object) null)
        {
          if (TimeTubeCreditsController.IsTimeTubing || this.m_timetubedInstance)
          {
            this.m_timetubedInstance = true;
            Graphics.Blit((Texture) renderTexture1, renderTexture1, this.AdditionalCoreStackRenderPass);
          }
          else
          {
            RenderTexture temporary = RenderTexture.GetTemporary(renderTexture1.width, renderTexture1.height, renderTexture1.depth, renderTexture1.format);
            temporary.filterMode = FilterMode.Point;
            Graphics.Blit((Texture) renderTexture1, temporary, this.AdditionalCoreStackRenderPass);
            temp2 = renderTexture1;
            renderTexture1 = temporary;
          }
        }
        if ((UnityEngine.Object) this.AdditionalPreBGCamera != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(temp1);
        RenderTexture renderTexture2 = (RenderTexture) null;
        if (GameManager.Options.MotionEnhancementMode == GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE)
        {
          this.m_camera.transform.position = new Vector3(Mathf.Floor(cachedPosition.x * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.y * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.z * (16f * this.ScaleTileScale))) / (16f * this.ScaleTileScale);
          this.m_camera.transform.position -= new Vector3(1f / 16f, 1f / 16f, 0.0f);
          slavedCamera.orthographicSize = this.m_camera.orthographicSize;
          renderTexture2 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, RenderTextureFormat.Depth);
          Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, renderTexture2);
          slavedCamera.targetTexture = renderTexture2;
          slavedCamera.clearFlags = CameraClearFlags.Depth;
          slavedCamera.cullingMask = this.cm_fg_important;
          if (!this.PRECLUDE_DEPTH_RENDERING)
            slavedCamera.Render();
          slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
          this.m_camera.transform.position += new Vector3(1f / 16f, 1f / 16f, 0.0f);
          this.m_camera.transform.position = cachedPosition + vector3;
        }
        this.CalculateDepixelatedOffset(cachedPosition, quantizedPosition, corePixelatedWidth, corePixelatedHeight, renderTexture1);
        RenderTexture temporary1 = RenderTexture.GetTemporary(source.width, source.height, 0, this.PLATFORM_RENDER_FORMAT);
        temporary1.filterMode = FilterMode.Point;
        Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, temporary1);
        this.m_camera.transform.position = quantizedPosition;
        slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
        this.RenderGBuffer(source, slavedCamera, !(bool) (UnityEngine.Object) temp2 ? renderTexture1.depthBuffer : temp2.depthBuffer, temporary1, cachedPosition, quantizedPosition);
        slavedCamera.orthographicSize = this.m_camera.orthographicSize;
        this.m_camera.transform.position = cachedPosition + vector3;
        if ((UnityEngine.Object) temp2 != (UnityEngine.Object) null)
          RenderTexture.ReleaseTemporary(temp2);
        RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
        int num = Mathf.Max(Mathf.CeilToInt((float) source.width / (float) this.CurrentMacroResolutionX), Mathf.CeilToInt((float) source.height / (float) this.CurrentMacroResolutionY));
        if (this.CurrentMacroResolutionX * num == source.width && this.CurrentMacroResolutionY * num == source.height)
        {
          Graphics.Blit((Texture) renderTexture1, temporary2, this.m_partialCopyMaterial);
        }
        else
        {
          RenderTexture temporary3 = RenderTexture.GetTemporary(this.CurrentMacroResolutionX * num, this.CurrentMacroResolutionY * num, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
          Graphics.Blit((Texture) renderTexture1, temporary3);
          temporary3.filterMode = this.DownsamplingFilterMode;
          Graphics.Blit((Texture) temporary3, temporary2, this.m_partialCopyMaterial);
          RenderTexture.ReleaseTemporary(temporary3);
        }
        if (GameManager.Options.MotionEnhancementMode == GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE)
        {
          this.m_camera.transform.position = new Vector3(Mathf.Floor(cachedPosition.x * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.y * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.z * (16f * this.ScaleTileScale))) / (16f * this.ScaleTileScale);
          this.m_camera.transform.position -= new Vector3(1f / 16f, 1f / 16f, 0.0f);
          slavedCamera.orthographicSize = this.m_camera.orthographicSize;
          slavedCamera.targetTexture = temporary2;
          slavedCamera.SetTargetBuffers(temporary2.colorBuffer, renderTexture2.depthBuffer);
          slavedCamera.clearFlags = CameraClearFlags.Nothing;
          slavedCamera.cullingMask = this.cm_unpixelated;
          slavedCamera.Render();
          slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
          this.m_camera.transform.position += new Vector3(1f / 16f, 1f / 16f, 0.0f);
          this.m_camera.transform.position = cachedPosition + vector3;
        }
        RenderTexture renderTexture3 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
        if (!this.m_gammaLocked)
          this.m_gammaEffect.ActiveMaterial.SetFloat(this.m_gammaID, 2f - GameManager.Options.Gamma + this.m_gammaAdjustment);
        if ((UnityEngine.Object) this.m_vignetteMaterial != (UnityEngine.Object) null)
        {
          this.m_vignetteMaterial.SetTexture(this.m_gBufferID, (Texture) temporary1);
          Graphics.Blit((Texture) temporary2, renderTexture3, this.m_vignetteMaterial);
        }
        else
          Graphics.Blit((Texture) temporary2, renderTexture3);
        this.m_camera.transform.position = new Vector3(Mathf.Floor(cachedPosition.x * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.y * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.z * (16f * this.ScaleTileScale))) / (16f * this.ScaleTileScale);
        this.m_camera.transform.position -= new Vector3(1f / 16f, 1f / 16f, 0.0f);
        slavedCamera.orthographicSize = this.m_camera.orthographicSize;
        slavedCamera.targetTexture = renderTexture3;
        slavedCamera.clearFlags = CameraClearFlags.Depth;
        slavedCamera.cullingMask = this.cm_unoccluded;
        slavedCamera.Render();
        slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
        this.m_camera.transform.position += new Vector3(1f / 16f, 1f / 16f, 0.0f);
        this.m_camera.transform.position = cachedPosition + vector3;
        if (this.AdditionalRenderPasses.Count > 0)
        {
          for (int index = 0; index < this.AdditionalRenderPasses.Count; ++index)
          {
            if ((UnityEngine.Object) this.AdditionalRenderPasses[index] == (UnityEngine.Object) null)
            {
              this.AdditionalRenderPasses.RemoveAt(index);
              --index;
            }
            else
            {
              RenderTexture temporary4 = RenderTexture.GetTemporary(renderTexture3.width, renderTexture3.height, renderTexture3.depth, renderTexture3.format);
              Graphics.Blit((Texture) renderTexture3, temporary4, this.AdditionalRenderPasses[index], 0);
              if (this.AdditionalRenderPassesInitialized[index])
              {
                RenderTexture.ReleaseTemporary(renderTexture3);
                renderTexture3 = temporary4;
              }
              else
              {
                this.AdditionalRenderPassesInitialized[index] = true;
                RenderTexture.ReleaseTemporary(temporary4);
              }
            }
          }
        }
        else if (!this.m_hasInitializedAdditionalRenderTarget)
        {
          RenderTexture temporary5 = RenderTexture.GetTemporary(renderTexture3.width, renderTexture3.height, renderTexture3.depth, renderTexture3.format);
          Graphics.Blit((Texture) renderTexture3, temporary5);
          RenderTexture.ReleaseTemporary(renderTexture3);
          renderTexture3 = temporary5;
          this.m_hasInitializedAdditionalRenderTarget = true;
        }
        if ((UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null)
        {
          this.m_fadeMaterial.SetFloat(this.m_saturationID, this.saturation);
          this.m_fadeMaterial.SetFloat(this.m_fadeID, this.fade);
        }
        if (this.DoFinalNonFadedLayer && (UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null)
        {
          if (this.CompositePixelatedUnfadedLayer && SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
          {
            RenderTexture temporary6 = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit((Texture) renderTexture3, temporary6, this.m_fadeMaterial);
            Graphics.Blit((Texture) temporary6, target);
            this.m_camera.transform.position -= new Vector3(1f / 16f, 1f / 16f, 0.0f);
            slavedCamera.orthographicSize = this.m_camera.orthographicSize;
            RenderTexture temporary7 = RenderTexture.GetTemporary(BraveCameraUtility.H_PIXELS, BraveCameraUtility.V_PIXELS);
            temporary7.filterMode = FilterMode.Point;
            slavedCamera.targetTexture = temporary7;
            slavedCamera.clearFlags = CameraClearFlags.Color;
            slavedCamera.backgroundColor = new Color(1f, 0.0f, 0.0f);
            slavedCamera.cullingMask = this.cm_unfaded;
            slavedCamera.Render();
            if ((UnityEngine.Object) this.m_compositor == (UnityEngine.Object) null)
              this.m_compositor = new Material(ShaderCache.Acquire("Hidden/SimpleCompositor"));
            this.m_compositor.SetTexture("_BaseTex", (Texture) temporary6);
            this.m_compositor.SetTexture("_LayerTex", (Texture) temporary7);
            Graphics.Blit((Texture) temporary7, target, this.m_compositor);
            RenderTexture.ReleaseTemporary(temporary7);
            RenderTexture.ReleaseTemporary(temporary6);
            slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
            this.m_camera.transform.position += new Vector3(1f / 16f, 1f / 16f, 0.0f);
          }
          else
          {
            RenderTexture temporary8 = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit((Texture) renderTexture3, temporary8, this.m_fadeMaterial);
            Graphics.Blit((Texture) temporary8, target);
            if (this.CompositePixelatedUnfadedLayer)
            {
              this.m_camera.transform.position -= new Vector3(1f / 16f, 1f / 16f, 0.0f);
              slavedCamera.orthographicSize = this.m_camera.orthographicSize;
              RenderTexture temporary9 = RenderTexture.GetTemporary(BraveCameraUtility.H_PIXELS, BraveCameraUtility.V_PIXELS);
              temporary9.filterMode = FilterMode.Point;
              slavedCamera.targetTexture = temporary9;
              slavedCamera.clearFlags = CameraClearFlags.Color;
              slavedCamera.backgroundColor = new Color(1f, 0.0f, 0.0f);
              slavedCamera.cullingMask = this.cm_unfaded;
              slavedCamera.Render();
              if ((UnityEngine.Object) this.m_compositor == (UnityEngine.Object) null)
                this.m_compositor = new Material(ShaderCache.Acquire("Hidden/SimpleCompositor"));
              this.m_compositor.SetTexture("_BaseTex", (Texture) temporary8);
              this.m_compositor.SetTexture("_LayerTex", (Texture) temporary9);
              Graphics.Blit((Texture) temporary9, target, this.m_compositor);
              RenderTexture.ReleaseTemporary(temporary9);
              slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
              this.m_camera.transform.position += new Vector3(1f / 16f, 1f / 16f, 0.0f);
            }
            else
            {
              this.m_camera.transform.position = new Vector3(Mathf.Floor(cachedPosition.x * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.y * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.z * (16f * this.ScaleTileScale))) / (16f * this.ScaleTileScale);
              this.m_camera.transform.position -= new Vector3(1f / 16f, 1f / 16f, 0.0f);
              slavedCamera.orthographicSize = this.m_camera.orthographicSize;
              slavedCamera.targetTexture = target;
              slavedCamera.clearFlags = CameraClearFlags.Depth;
              slavedCamera.cullingMask = this.cm_unfaded;
              slavedCamera.Render();
              slavedCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
              this.m_camera.transform.position += new Vector3(1f / 16f, 1f / 16f, 0.0f);
              this.m_camera.transform.position = cachedPosition + vector3;
            }
            RenderTexture.ReleaseTemporary(temporary8);
          }
        }
        else if ((UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null)
          Graphics.Blit((Texture) renderTexture3, target, this.m_fadeMaterial);
        else
          Graphics.Blit((Texture) renderTexture3, target);
        RenderTexture.ReleaseTemporary(temporary1);
        if (GameManager.Options.MotionEnhancementMode == GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE)
          RenderTexture.ReleaseTemporary(renderTexture2);
        if (zoomIntermediate)
          renderTexture1.Release();
        else
          RenderTexture.ReleaseTemporary(renderTexture1);
        RenderTexture.ReleaseTemporary(temporary2);
        RenderTexture.ReleaseTemporary(renderTexture3);
        this.m_camera.cullingMask = 0;
        slavedCamera.cullingMask = 0;
      }

      private void RenderEnemyProjectileMasks(Camera stackCamera, RenderTexture source)
      {
        this.m_UnblurredProjectileMaskTex = RenderTexture.GetTemporary(GameManager.Options.MotionEnhancementMode != GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE ? this.CurrentMacroResolutionX / this.overrideTileScale : source.width, GameManager.Options.MotionEnhancementMode != GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE ? this.CurrentMacroResolutionY / this.overrideTileScale : source.height, this.PLATFORM_DEPTH, RenderTextureFormat.Default);
        this.m_UnblurredProjectileMaskTex.filterMode = FilterMode.Point;
        this.m_BlurredProjectileMaskTex = RenderTexture.GetTemporary(this.m_UnblurredProjectileMaskTex.width, this.m_UnblurredProjectileMaskTex.height, this.PLATFORM_DEPTH, RenderTextureFormat.Default);
        this.m_BlurredProjectileMaskTex.filterMode = FilterMode.Point;
        if ((UnityEngine.Object) this.m_blurMaterial == (UnityEngine.Object) null)
        {
          this.m_blurMaterial = new Material(this.GetComponent<SENaturalBloomAndDirtyLens>().shader);
          this.m_blurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        int layer = LayerMask.NameToLayer("PlayerAndProjectiles");
        ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
        for (int index = 0; index < allProjectiles.Count; ++index)
        {
          if (!allProjectiles[index].neverMaskThis)
            allProjectiles[index].CacheLayer(layer);
        }
        stackCamera.clearFlags = CameraClearFlags.Color;
        stackCamera.cullingMask = 1 << layer;
        stackCamera.SetReplacementShader(this.m_simpleSpriteMaskShader, string.Empty);
        stackCamera.targetTexture = this.m_UnblurredProjectileMaskTex;
        stackCamera.Render();
        stackCamera.ResetReplacementShader();
        stackCamera.clearFlags = CameraClearFlags.Nothing;
        stackCamera.cullingMask = this.cm_fg & ~(1 << layer);
        stackCamera.SetReplacementShader(ShaderCache.Acquire("Brave/Internal/Black"), string.Empty);
        stackCamera.Render();
        stackCamera.ResetReplacementShader();
        for (int index = 0; index < 3; ++index)
        {
          this.m_blurMaterial.SetFloat("_BlurSize", this.ProjectileMaskBlurSize * 0.5f + (float) index);
          RenderTexture temporary1 = RenderTexture.GetTemporary(this.m_UnblurredProjectileMaskTex.width, this.m_UnblurredProjectileMaskTex.height, 0, RenderTextureFormat.Default);
          temporary1.filterMode = FilterMode.Point;
          Graphics.Blit(index != 0 ? (Texture) this.m_BlurredProjectileMaskTex : (Texture) this.m_UnblurredProjectileMaskTex, temporary1, this.m_blurMaterial, 2);
          RenderTexture.ReleaseTemporary(this.m_BlurredProjectileMaskTex);
          this.m_BlurredProjectileMaskTex = temporary1;
          RenderTexture temporary2 = RenderTexture.GetTemporary(this.m_UnblurredProjectileMaskTex.width, this.m_UnblurredProjectileMaskTex.height, 0, RenderTextureFormat.Default);
          temporary2.filterMode = FilterMode.Point;
          Graphics.Blit((Texture) this.m_BlurredProjectileMaskTex, temporary2, this.m_blurMaterial, 3);
          RenderTexture.ReleaseTemporary(this.m_BlurredProjectileMaskTex);
          this.m_BlurredProjectileMaskTex = temporary2;
        }
        for (int index = 0; index < allProjectiles.Count; ++index)
        {
          if (!allProjectiles[index].neverMaskThis)
            allProjectiles[index].DecacheLayer();
        }
      }

      protected float LightCullFactor
      {
        get
        {
          if (InfiniteMinecartZone.InInfiniteMinecartZone)
            return 2f;
          return GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH ? 1.5f : 1.25f;
        }
      }

      private bool ActuallyRenderGBuffer => this.DoRenderGBuffer;

      private void RenderGBufferCheap(
        RenderTexture source,
        Camera stackCamera,
        RenderBuffer depthTarget,
        RenderTexture TempBuffer_Lighting,
        int coreBufferWidth,
        int coreBufferHeight)
      {
        RenderTexture temporary1 = RenderTexture.GetTemporary(this.CurrentMacroResolutionX / (this.GBUFFER_DESCALE * this.overrideTileScale), this.CurrentMacroResolutionY / (this.GBUFFER_DESCALE * this.overrideTileScale), 0, this.PLATFORM_RENDER_FORMAT);
        Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, temporary1);
        Graphics.Blit((Texture) temporary1, temporary1, this.m_pointLightMaterial, 1);
        RenderTexture renderTexture = (RenderTexture) null;
        if (this.IsInIntro || GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW)
        {
          renderTexture = RenderTexture.GetTemporary(coreBufferWidth, coreBufferHeight, this.PLATFORM_DEPTH, RenderTextureFormat.Default);
          renderTexture.filterMode = FilterMode.Point;
          Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, renderTexture);
          int num = GameManager.Options.MotionEnhancementMode != GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE ? this.cm_gbuffer : this.cm_gbufferSimple;
          stackCamera.clearFlags = CameraClearFlags.Nothing;
          stackCamera.cullingMask = num;
          stackCamera.targetTexture = renderTexture;
          stackCamera.SetTargetBuffers(renderTexture.colorBuffer, depthTarget);
          stackCamera.SetReplacementShader(this.m_simpleSpriteMaskShader, "UnlitTilted");
          stackCamera.Render();
          stackCamera.ResetReplacementShader();
        }
        for (int index = 0; index < this.AdditionalBraveLights.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.AdditionalBraveLights[index] && this.AdditionalBraveLights[index].gameObject.activeSelf && this.AdditionalBraveLights[index].UsesCustomMaterial)
          {
            AdditionalBraveLight additionalBraveLight = this.AdditionalBraveLights[index];
            float lightRadius = additionalBraveLight.LightRadius;
            float lightIntensity = additionalBraveLight.LightIntensity;
            if ((double) lightIntensity != 0.0)
            {
              Vector2 vector2_1 = !(bool) (UnityEngine.Object) additionalBraveLight.sprite ? (Vector2) additionalBraveLight.transform.position : additionalBraveLight.sprite.WorldCenter;
              Vector2 vector2_2 = stackCamera.transform.position.XY();
              if (!this.LightCulled(vector2_1, vector2_2, lightRadius, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
              {
                Material customLightMaterial = additionalBraveLight.CustomLightMaterial;
                customLightMaterial.SetVector(this.m_cameraWSID, vector2_2.ToVector4());
                customLightMaterial.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
                customLightMaterial.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
                customLightMaterial.SetVector(this.m_lightPosID, vector2_1.ToVector4());
                customLightMaterial.SetColor(this.m_lightColorID, additionalBraveLight.LightColor);
                customLightMaterial.SetFloat(this.m_lightRadiusID, lightRadius);
                customLightMaterial.SetFloat(this.m_lightIntensityID, lightIntensity);
                Graphics.Blit((Texture) temporary1, temporary1, customLightMaterial, 0);
              }
            }
          }
        }
        if (false && GameManager.Instance.Dungeon.PlayerIsLight && !GameManager.Instance.IsLoadingLevel && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
        {
          Vector2 vector2 = stackCamera.transform.position.XY();
          this.m_pointLightMaterialFast.SetVector(this.m_cameraWSID, vector2.ToVector4());
          this.m_pointLightMaterialFast.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
          this.m_pointLightMaterialFast.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
          float playerLightRadius = GameManager.Instance.Dungeon.PlayerLightRadius;
          float num = GameManager.Instance.Dungeon.PlayerLightIntensity / 5f;
          Vector2 centerPosition = GameManager.Instance.PrimaryPlayer.CenterPosition;
          if (!this.LightCulled(centerPosition, vector2, playerLightRadius, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
          {
            this.m_pointLightMaterialFast.SetVector(this.m_lightPosID, centerPosition.ToVector4());
            this.m_pointLightMaterialFast.SetColor(this.m_lightColorID, GameManager.Instance.Dungeon.PlayerLightColor);
            this.m_pointLightMaterialFast.SetFloat(this.m_lightRadiusID, playerLightRadius);
            this.m_pointLightMaterialFast.SetFloat(this.m_lightIntensityID, num);
            Graphics.Blit((Texture) temporary1, temporary1, this.m_pointLightMaterialFast, 0);
          }
        }
        if ((UnityEngine.Object) renderTexture == (UnityEngine.Object) null)
        {
          Graphics.Blit((Texture) temporary1, TempBuffer_Lighting);
        }
        else
        {
          RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
          Graphics.Blit((Texture) renderTexture, temporary2);
          this.m_gbufferMaskMaterial.SetTexture(this.m_lightMaskTexID, (Texture) temporary2);
          Graphics.Blit((Texture) temporary1, TempBuffer_Lighting, this.m_gbufferMaskMaterial);
          RenderTexture.ReleaseTemporary(temporary2);
          RenderTexture.ReleaseTemporary(renderTexture);
        }
        RenderTexture.ReleaseTemporary(temporary1);
      }

      private void RenderGBufferScaling(
        RenderTexture source,
        Camera stackCamera,
        RenderBuffer depthTarget,
        RenderTexture TempBuffer_Lighting)
      {
        int width = this.CurrentMacroResolutionX / this.overrideTileScale;
        int height = this.CurrentMacroResolutionY / this.overrideTileScale;
        if (this.ActuallyRenderGBuffer)
        {
          RenderTexture temporary1 = RenderTexture.GetTemporary(this.CurrentMacroResolutionX / (this.GBUFFER_DESCALE * this.overrideTileScale), this.CurrentMacroResolutionY / (this.GBUFFER_DESCALE * this.overrideTileScale), 0, this.PLATFORM_RENDER_FORMAT);
          Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, temporary1);
          Graphics.Blit((Texture) temporary1, temporary1, this.m_pointLightMaterial, 1);
          RenderTexture renderTexture = (RenderTexture) null;
          RenderTexture temp = (RenderTexture) null;
          if (this.IsInIntro || GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW)
          {
            renderTexture = RenderTexture.GetTemporary(width, height, this.PLATFORM_DEPTH, RenderTextureFormat.Default);
            renderTexture.filterMode = FilterMode.Point;
            Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, renderTexture);
            int num = GameManager.Options.MotionEnhancementMode != GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE ? this.cm_gbuffer : this.cm_gbufferSimple;
            stackCamera.clearFlags = CameraClearFlags.Nothing;
            stackCamera.cullingMask = num;
            stackCamera.targetTexture = renderTexture;
            stackCamera.SetTargetBuffers(renderTexture.colorBuffer, depthTarget);
            stackCamera.SetReplacementShader(this.m_simpleSpriteMaskShader, "UnlitTilted");
            stackCamera.Render();
            stackCamera.ResetReplacementShader();
            Vector2 vector2_1 = stackCamera.transform.position.XY();
            this.m_pointLightMaterial.SetVector(this.m_cameraWSID, vector2_1.ToVector4());
            this.m_pointLightMaterial.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
            this.m_pointLightMaterial.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
            this.m_pointLightMaterialFast.SetVector(this.m_cameraWSID, vector2_1.ToVector4());
            this.m_pointLightMaterialFast.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
            this.m_pointLightMaterialFast.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
            for (int index = 0; index < ShadowSystem.AllLights.Count; ++index)
            {
              ShadowSystem allLight = ShadowSystem.AllLights[index];
              if ((bool) (UnityEngine.Object) allLight && allLight.gameObject.activeSelf)
              {
                bool flag = (UnityEngine.Object) allLight.uLightCookie == (UnityEngine.Object) null;
                Material mat = !flag ? this.m_pointLightMaterial : this.m_pointLightMaterialFast;
                Vector2 vector2_2 = allLight.transform.position.XY();
                if (!this.LightCulled(vector2_2, vector2_1, allLight.uLightRange, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
                {
                  mat.SetVector(this.m_lightPosID, vector2_2.ToVector4());
                  mat.SetColor(this.m_lightColorID, allLight.uLightColor);
                  mat.SetFloat(this.m_lightRadiusID, allLight.uLightRange);
                  mat.SetFloat(this.m_lightIntensityID, allLight.uLightIntensity * this.pointLightMultiplier);
                  if (!flag)
                  {
                    mat.SetTexture(this.m_lightCookieID, (Texture) allLight.uLightCookie);
                    mat.SetFloat(this.m_lightCookieAngleID, allLight.uLightCookieAngle);
                  }
                  Graphics.Blit((Texture) temporary1, temporary1, mat, 0);
                }
              }
            }
            for (int index = 0; index < this.AdditionalBraveLights.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) this.AdditionalBraveLights[index] && this.AdditionalBraveLights[index].gameObject.activeSelf)
              {
                AdditionalBraveLight additionalBraveLight = this.AdditionalBraveLights[index];
                float lightRadius = additionalBraveLight.LightRadius;
                float lightIntensity = additionalBraveLight.LightIntensity;
                if ((double) lightIntensity != 0.0)
                {
                  Vector2 vector2_3 = !(bool) (UnityEngine.Object) additionalBraveLight.sprite ? (Vector2) additionalBraveLight.transform.position : additionalBraveLight.sprite.WorldCenter;
                  if (!this.LightCulled(vector2_3, vector2_1, lightRadius, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
                  {
                    Material mat = this.m_pointLightMaterialFast;
                    if (additionalBraveLight.UsesCustomMaterial)
                    {
                      mat = additionalBraveLight.CustomLightMaterial;
                      mat.SetVector(this.m_cameraWSID, vector2_1.ToVector4());
                      mat.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
                      mat.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
                    }
                    else if (additionalBraveLight.UsesCone)
                    {
                      mat = this.m_pointLightMaterial;
                      mat.SetFloat("_LightAngle", additionalBraveLight.LightAngle);
                      mat.SetFloat("_LightOrient", additionalBraveLight.LightOrient);
                    }
                    mat.SetVector(this.m_lightPosID, vector2_3.ToVector4());
                    mat.SetColor(this.m_lightColorID, additionalBraveLight.LightColor);
                    mat.SetFloat(this.m_lightRadiusID, lightRadius);
                    mat.SetFloat(this.m_lightIntensityID, lightIntensity);
                    Graphics.Blit((Texture) temporary1, temporary1, mat, 0);
                  }
                }
              }
            }
            if (GameManager.Instance.Dungeon.PlayerIsLight && !GameManager.Instance.IsLoadingLevel && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
            {
              float playerLightRadius = GameManager.Instance.Dungeon.PlayerLightRadius;
              float playerLightIntensity = GameManager.Instance.Dungeon.PlayerLightIntensity;
              Vector2 centerPosition = GameManager.Instance.PrimaryPlayer.CenterPosition;
              if (!this.LightCulled(centerPosition, vector2_1, playerLightRadius, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
              {
                this.m_pointLightMaterialFast.SetVector(this.m_lightPosID, centerPosition.ToVector4());
                this.m_pointLightMaterialFast.SetColor(this.m_lightColorID, GameManager.Instance.Dungeon.PlayerLightColor);
                this.m_pointLightMaterialFast.SetFloat(this.m_lightRadiusID, playerLightRadius);
                this.m_pointLightMaterialFast.SetFloat(this.m_lightIntensityID, playerLightIntensity);
                Graphics.Blit((Texture) temporary1, temporary1, this.m_pointLightMaterialFast, 0);
              }
            }
          }
          if ((UnityEngine.Object) renderTexture == (UnityEngine.Object) null)
          {
            Graphics.Blit((Texture) temporary1, TempBuffer_Lighting);
          }
          else
          {
            RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
            Graphics.Blit((Texture) renderTexture, temporary2);
            this.m_gbufferMaskMaterial.SetTexture(this.m_lightMaskTexID, (Texture) renderTexture);
            Graphics.Blit((Texture) temporary1, TempBuffer_Lighting, this.m_gbufferMaskMaterial);
            RenderTexture.ReleaseTemporary(temporary2);
            RenderTexture.ReleaseTemporary(renderTexture);
            if ((UnityEngine.Object) temp != (UnityEngine.Object) null)
              RenderTexture.ReleaseTemporary(temp);
          }
          RenderTexture.ReleaseTemporary(temporary1);
        }
        else
        {
          Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, TempBuffer_Lighting);
          RenderSettings.ambientMode = AmbientMode.Flat;
          RenderSettings.ambientLight = Color.white;
          Graphics.Blit((Texture) TempBuffer_Lighting, TempBuffer_Lighting, this.m_pointLightMaterial, 1);
        }
      }

      private bool LightCulled(
        Vector2 lightPosition,
        Vector2 cameraPosition,
        float lightRange,
        float orthoSize,
        float aspect)
      {
        return (double) Vector2.Distance(lightPosition, cameraPosition) > (double) lightRange + (double) orthoSize * (double) this.LightCullFactor * (double) aspect;
      }

      private void RenderGBuffer(
        RenderTexture source,
        Camera stackCamera,
        RenderBuffer depthTarget,
        RenderTexture TempBuffer_Lighting,
        Vector3 cachedPosition,
        Vector3 quantizedPosition)
      {
        int width = this.CurrentMacroResolutionX / this.overrideTileScale + this.extraPixels;
        int height = this.CurrentMacroResolutionY / this.overrideTileScale + this.extraPixels;
        Vector3 vector3 = !this.IsInIntro ? CameraController.PLATFORM_CAMERA_OFFSET : Vector3.zero;
        if (this.ActuallyRenderGBuffer)
        {
          RenderTexture temporary1 = RenderTexture.GetTemporary(this.CurrentMacroResolutionX / (this.GBUFFER_DESCALE * this.overrideTileScale), this.CurrentMacroResolutionY / (this.GBUFFER_DESCALE * this.overrideTileScale), 0, this.PLATFORM_RENDER_FORMAT);
          Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, temporary1);
          Graphics.Blit((Texture) temporary1, temporary1, this.m_pointLightMaterial, 1);
          RenderTexture renderTexture1 = (RenderTexture) null;
          RenderTexture renderTexture2 = (RenderTexture) null;
          if (this.IsInIntro || GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW)
          {
            renderTexture1 = RenderTexture.GetTemporary(width, height, this.PLATFORM_DEPTH, RenderTextureFormat.Default);
            renderTexture1.filterMode = FilterMode.Point;
            Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, renderTexture1);
            int num = GameManager.Options.MotionEnhancementMode != GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE ? this.cm_gbuffer : this.cm_gbufferSimple;
            stackCamera.clearFlags = CameraClearFlags.Nothing;
            stackCamera.cullingMask = num;
            stackCamera.targetTexture = renderTexture1;
            stackCamera.SetTargetBuffers(renderTexture1.colorBuffer, depthTarget);
            stackCamera.SetReplacementShader(this.m_simpleSpriteMaskShader, "UnlitTilted");
            stackCamera.Render();
            if (GameManager.Options.MotionEnhancementMode == GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE)
            {
              int cmUnpixelated = this.cm_unpixelated;
              renderTexture2 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, RenderTextureFormat.Default);
              Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, renderTexture2);
              this.m_camera.transform.position = new Vector3(Mathf.Floor(cachedPosition.x * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.y * (16f * this.ScaleTileScale)), Mathf.Floor(cachedPosition.z * (16f * this.ScaleTileScale))) / (16f * this.ScaleTileScale);
              this.m_camera.transform.position -= new Vector3(1f / 16f, 1f / 16f, 0.0f);
              stackCamera.orthographicSize = this.m_camera.orthographicSize;
              stackCamera.cullingMask = cmUnpixelated;
              stackCamera.targetTexture = renderTexture2;
              stackCamera.SetReplacementShader(this.m_simpleSpriteMaskUnpixelatedShader, "UnlitTilted");
              stackCamera.Render();
              stackCamera.orthographicSize = this.m_camera.orthographicSize * ((float) height / ((float) this.CurrentMacroResolutionY / (float) this.overrideTileScale));
              this.m_camera.transform.position = cachedPosition + vector3;
            }
            stackCamera.ResetReplacementShader();
            Vector2 vector2_1 = stackCamera.transform.position.XY();
            this.m_pointLightMaterial.SetVector(this.m_cameraWSID, vector2_1.ToVector4());
            this.m_pointLightMaterial.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
            this.m_pointLightMaterial.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
            this.m_pointLightMaterialFast.SetVector(this.m_cameraWSID, vector2_1.ToVector4());
            this.m_pointLightMaterialFast.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
            this.m_pointLightMaterialFast.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
            for (int index = 0; index < ShadowSystem.AllLights.Count; ++index)
            {
              ShadowSystem allLight = ShadowSystem.AllLights[index];
              if ((bool) (UnityEngine.Object) allLight && allLight.gameObject.activeSelf)
              {
                bool flag = (UnityEngine.Object) allLight.uLightCookie == (UnityEngine.Object) null;
                Material mat = !flag ? this.m_pointLightMaterial : this.m_pointLightMaterialFast;
                Vector2 vector2_2 = allLight.transform.position.XY();
                if (!this.LightCulled(vector2_2, vector2_1, allLight.uLightRange, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
                {
                  mat.SetVector(this.m_lightPosID, vector2_2.ToVector4());
                  mat.SetColor(this.m_lightColorID, allLight.uLightColor);
                  mat.SetFloat(this.m_lightRadiusID, allLight.uLightRange);
                  mat.SetFloat(this.m_lightIntensityID, allLight.uLightIntensity * this.pointLightMultiplier);
                  if (!flag)
                  {
                    mat.SetTexture(this.m_lightCookieID, (Texture) allLight.uLightCookie);
                    mat.SetFloat(this.m_lightCookieAngleID, allLight.uLightCookieAngle);
                  }
                  Graphics.Blit((Texture) temporary1, temporary1, mat, 0);
                }
              }
            }
            for (int index = 0; index < this.AdditionalBraveLights.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) this.AdditionalBraveLights[index] && this.AdditionalBraveLights[index].gameObject.activeSelf)
              {
                AdditionalBraveLight additionalBraveLight = this.AdditionalBraveLights[index];
                float lightRadius = additionalBraveLight.LightRadius;
                float lightIntensity = additionalBraveLight.LightIntensity;
                if ((double) lightIntensity != 0.0)
                {
                  Vector2 vector2_3 = !(bool) (UnityEngine.Object) additionalBraveLight.sprite ? (Vector2) additionalBraveLight.transform.position : additionalBraveLight.sprite.WorldCenter;
                  if (!this.LightCulled(vector2_3, vector2_1, lightRadius, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
                  {
                    Material mat = this.m_pointLightMaterialFast;
                    if (additionalBraveLight.UsesCustomMaterial)
                    {
                      mat = additionalBraveLight.CustomLightMaterial;
                      mat.SetVector(this.m_cameraWSID, vector2_1.ToVector4());
                      mat.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
                      mat.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
                    }
                    else if (additionalBraveLight.UsesCone)
                    {
                      mat = this.m_pointLightMaterial;
                      mat.SetFloat("_LightAngle", additionalBraveLight.LightAngle);
                      mat.SetFloat("_LightOrient", additionalBraveLight.LightOrient);
                    }
                    mat.SetVector(this.m_lightPosID, vector2_3.ToVector4());
                    mat.SetColor(this.m_lightColorID, additionalBraveLight.LightColor);
                    mat.SetFloat(this.m_lightRadiusID, lightRadius);
                    mat.SetFloat(this.m_lightIntensityID, lightIntensity);
                    Graphics.Blit((Texture) temporary1, temporary1, mat, 0);
                  }
                }
              }
            }
            if (GameManager.Instance.Dungeon.PlayerIsLight && !GameManager.Instance.IsLoadingLevel && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
            {
              float playerLightRadius = GameManager.Instance.Dungeon.PlayerLightRadius;
              float playerLightIntensity = GameManager.Instance.Dungeon.PlayerLightIntensity;
              Vector2 centerPosition = GameManager.Instance.PrimaryPlayer.CenterPosition;
              if (!this.LightCulled(centerPosition, vector2_1, playerLightRadius, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
              {
                this.m_pointLightMaterialFast.SetVector(this.m_lightPosID, centerPosition.ToVector4());
                this.m_pointLightMaterialFast.SetColor(this.m_lightColorID, GameManager.Instance.Dungeon.PlayerLightColor);
                this.m_pointLightMaterialFast.SetFloat(this.m_lightRadiusID, playerLightRadius);
                this.m_pointLightMaterialFast.SetFloat(this.m_lightIntensityID, playerLightIntensity);
                Graphics.Blit((Texture) temporary1, temporary1, this.m_pointLightMaterialFast, 0);
              }
            }
          }
          else
          {
            for (int index = 0; index < this.AdditionalBraveLights.Count; ++index)
            {
              if ((bool) (UnityEngine.Object) this.AdditionalBraveLights[index] && this.AdditionalBraveLights[index].gameObject.activeSelf && this.AdditionalBraveLights[index].UsesCustomMaterial)
              {
                AdditionalBraveLight additionalBraveLight = this.AdditionalBraveLights[index];
                Vector2 vector2_4 = stackCamera.transform.position.XY();
                float lightRadius = additionalBraveLight.LightRadius;
                float lightIntensity = additionalBraveLight.LightIntensity;
                if ((double) lightIntensity != 0.0)
                {
                  Vector2 vector2_5 = !(bool) (UnityEngine.Object) additionalBraveLight.sprite ? (Vector2) additionalBraveLight.transform.position : additionalBraveLight.sprite.WorldCenter;
                  if (!this.LightCulled(vector2_5, vector2_4, lightRadius, stackCamera.orthographicSize, BraveCameraUtility.ASPECT))
                  {
                    Material mat = this.m_pointLightMaterialFast;
                    if (additionalBraveLight.UsesCustomMaterial)
                    {
                      mat = additionalBraveLight.CustomLightMaterial;
                      mat.SetVector(this.m_cameraWSID, vector2_4.ToVector4());
                      mat.SetFloat(this.m_cameraOrthoSizeID, stackCamera.orthographicSize);
                      mat.SetFloat(this.m_cameraOrthoSizeXID, stackCamera.orthographicSize * stackCamera.aspect);
                    }
                    mat.SetVector(this.m_lightPosID, vector2_5.ToVector4());
                    mat.SetColor(this.m_lightColorID, additionalBraveLight.LightColor);
                    mat.SetFloat(this.m_lightRadiusID, lightRadius);
                    mat.SetFloat(this.m_lightIntensityID, lightIntensity);
                    Graphics.Blit((Texture) temporary1, temporary1, mat, 0);
                  }
                }
              }
            }
          }
          if ((UnityEngine.Object) renderTexture1 == (UnityEngine.Object) null)
          {
            Graphics.Blit((Texture) temporary1, TempBuffer_Lighting);
          }
          else
          {
            RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
            int num = Mathf.Max(Mathf.CeilToInt((float) source.width / (float) this.CurrentMacroResolutionX), Mathf.CeilToInt((float) source.height / (float) this.CurrentMacroResolutionY));
            if ((this.CurrentMacroResolutionX * num != source.width || this.CurrentMacroResolutionY * num != source.height) && (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH || GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW))
            {
              RenderTexture temporary3 = RenderTexture.GetTemporary(this.CurrentMacroResolutionX * num, this.CurrentMacroResolutionY * num, this.PLATFORM_DEPTH, this.PLATFORM_RENDER_FORMAT);
              Graphics.Blit((Texture) renderTexture1, temporary3);
              temporary3.filterMode = this.DownsamplingFilterMode;
              if ((UnityEngine.Object) renderTexture2 != (UnityEngine.Object) null)
              {
                this.m_gbufferLightMaskCombinerMaterial.SetTexture("_MainTex", (Texture) temporary3);
                this.m_gbufferLightMaskCombinerMaterial.SetTexture("_LightTex", (Texture) renderTexture2);
                Graphics.Blit((Texture) temporary3, temporary2, this.m_gbufferLightMaskCombinerMaterial);
              }
              else
                Graphics.Blit((Texture) temporary3, temporary2, this.m_partialCopyMaterial);
              RenderTexture.ReleaseTemporary(temporary3);
            }
            else if ((UnityEngine.Object) renderTexture2 != (UnityEngine.Object) null)
            {
              this.m_gbufferLightMaskCombinerMaterial.SetTexture("_MainTex", (Texture) renderTexture1);
              this.m_gbufferLightMaskCombinerMaterial.SetTexture("_LightTex", (Texture) renderTexture2);
              Graphics.Blit((Texture) renderTexture1, temporary2, this.m_gbufferLightMaskCombinerMaterial);
            }
            else
              Graphics.Blit((Texture) renderTexture1, temporary2, this.m_partialCopyMaterial);
            this.m_gbufferMaskMaterial.SetTexture(this.m_lightMaskTexID, (Texture) temporary2);
            Graphics.Blit((Texture) temporary1, TempBuffer_Lighting, this.m_gbufferMaskMaterial);
            RenderTexture.ReleaseTemporary(temporary2);
            RenderTexture.ReleaseTemporary(renderTexture1);
            if ((UnityEngine.Object) renderTexture2 != (UnityEngine.Object) null)
              RenderTexture.ReleaseTemporary(renderTexture2);
          }
          RenderTexture.ReleaseTemporary(temporary1);
        }
        else
        {
          Graphics.Blit((Texture) Pixelator.m_smallBlackTexture, TempBuffer_Lighting);
          RenderSettings.ambientMode = AmbientMode.Flat;
          RenderSettings.ambientLight = Color.white;
          Graphics.Blit((Texture) TempBuffer_Lighting, TempBuffer_Lighting, this.m_pointLightMaterial, 1);
        }
      }

      public void CustomFade(
        float duration,
        float holdTime,
        Color startColor,
        Color endColor,
        float startScreenBrightness,
        float endScreenBrightness)
      {
        this.StartCoroutine(this.CustomFade_CR(duration, holdTime, startColor, endColor, startScreenBrightness, endScreenBrightness));
      }

      [DebuggerHidden]
      private IEnumerator CustomFade_CR(
        float duration,
        float holdTime,
        Color startColor,
        Color endColor,
        float startScreenBrightness,
        float endScreenBrightness)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CCustomFade_CR\u003Ec__Iterator1()
        {
          holdTime = holdTime,
          startColor = startColor,
          startScreenBrightness = startScreenBrightness,
          duration = duration,
          endColor = endColor,
          endScreenBrightness = endScreenBrightness,
          \u0024this = this
        };
      }

      public void TriggerPastFadeIn() => this.StartCoroutine(this.HandlePastFadeIn());

      [DebuggerHidden]
      private IEnumerator HandlePastFadeIn()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CHandlePastFadeIn\u003Ec__Iterator2()
        {
          \u0024this = this
        };
      }

      public void SetFadeColor(Color c)
      {
        if ((UnityEngine.Object) this.m_fadeMaterial != (UnityEngine.Object) null)
          this.m_fadeMaterial.SetColor(this.m_fadeColorID, c);
        this.fade = 1f - c.a;
      }

      public void FreezeFrame() => this.StartCoroutine(this.HandleFreezeFrame());

      [DebuggerHidden]
      private IEnumerator HandleFreezeFrame()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CHandleFreezeFrame\u003Ec__Iterator3()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleTimedFreezeFrame(float duration, float holdDuration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CHandleTimedFreezeFrame\u003Ec__Iterator4()
        {
          holdDuration = holdDuration,
          duration = duration,
          \u0024this = this
        };
      }

      public void TimedFreezeFrame(float duration, float holdDuration)
      {
        this.StartCoroutine(this.HandleTimedFreezeFrame(duration, holdDuration));
      }

      public void SetSaturationColorPower(Color satColor, float t)
      {
        this.m_gammaAdjustment = Mathf.Lerp(GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW ? 0.0f : -0.1f, -0.35f, t);
        this.m_fadeMaterial.SetColor("_SaturationColor", Color.Lerp(new Color(1f, 1f, 1f), satColor, t));
        this.saturation = Mathf.Lerp(1f, 0.0f, t);
        this.m_fadeMaterial.SetFloat(this.m_saturationID, this.saturation);
      }

      public void SetFreezeFramePower(float t, bool isCameraEffect = false)
      {
        this.m_gammaAdjustment = Mathf.Lerp(GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW ? 0.0f : -0.1f, -0.35f, t);
        this.m_fadeMaterial.SetColor("_SaturationColor", Color.Lerp(new Color(1f, 1f, 1f), new Color(0.825f, 0.7f, 0.3f), t));
        this.saturation = Mathf.Lerp(1f, 0.0f, t);
        if (isCameraEffect)
          this.m_gammaAdjustment = Mathf.Lerp(GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.LOW ? 0.0f : -0.1f, -0.6f, t);
        this.m_fadeMaterial.SetFloat(this.m_saturationID, this.saturation);
      }

      public void ClearFreezeFrame()
      {
        this.OnChangedLightingQuality(GameManager.Options.LightingQuality);
        this.m_fadeMaterial.SetColor("_SaturationColor", new Color(1f, 1f, 1f));
        this.saturation = 1f;
        this.m_fadeMaterial.SetFloat(this.m_saturationID, 1f);
      }

      public void FadeToColor(float duration, Color c, bool reverse = false, float holdTime = 0.0f)
      {
        if (this.m_fadeLocked)
          return;
        this.StartCoroutine(this.FadeToColor_CR(duration, c, reverse, holdTime));
      }

      public void FadeToBlack(float duration, bool reverse = false, float holdTime = 0.0f)
      {
        if (!reverse && (double) this.fade == 0.0)
          return;
        this.m_fadeLocked = true;
        this.StartCoroutine(this.FadeToColor_CR(duration, Color.black, reverse, holdTime));
      }

      [DebuggerHidden]
      private IEnumerator FadeToColor_CR(float duration, Color targetColor, bool reverse = false, float hold = 0.0f)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CFadeToColor_CR\u003Ec__Iterator5()
        {
          targetColor = targetColor,
          hold = hold,
          reverse = reverse,
          duration = duration,
          \u0024this = this
        };
      }

      public void HandleDamagedVignette(Vector2 damageDirection)
      {
        this.StartCoroutine(this.HandleDamagedVignette_CR());
      }

      [DebuggerHidden]
      private IEnumerator HandleDamagedVignette_CR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CHandleDamagedVignette_CR\u003Ec__Iterator6()
        {
          \u0024this = this
        };
      }

      public void SetWindowbox(float targetFraction)
      {
        this.m_fadeMaterial.SetFloat("_WindowboxFrac", targetFraction);
      }

      public void LerpToLetterbox(float targetFraction, float duration)
      {
        if ((double) duration <= 0.0)
          this.m_fadeMaterial.SetFloat("_LetterboxFrac", targetFraction);
        else
          this.StartCoroutine(this.LerpToLetterbox_CR(targetFraction, duration));
      }

      [DebuggerHidden]
      private IEnumerator LerpToLetterbox_CR(float targetFraction, float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new Pixelator.\u003CLerpToLetterbox_CR\u003Ec__Iterator7()
        {
          duration = duration,
          targetFraction = targetFraction,
          \u0024this = this
        };
      }

      public bool CacheCurrentFrameToBuffer
      {
        get
        {
          if ((UnityEngine.Object) this.m_gammaPass == (UnityEngine.Object) null)
            this.m_gammaPass = this.GetComponent<GenericFullscreenEffect>();
          return this.m_gammaPass.CacheCurrentFrameToBuffer;
        }
        set
        {
          if ((UnityEngine.Object) this.m_gammaPass == (UnityEngine.Object) null)
            this.m_gammaPass = this.GetComponent<GenericFullscreenEffect>();
          this.m_gammaPass.CacheCurrentFrameToBuffer = value;
        }
      }

      public void CacheScreenSpacePositionsForDeathFrame(Vector2 playerPosition, Vector2 enemyPosition)
      {
        this.CachedPlayerViewportPoint = this.m_camera.WorldToViewportPoint(playerPosition.ToVector3ZUp());
        if (enemyPosition != Vector2.zero)
          this.CachedEnemyViewportPoint = this.m_camera.WorldToViewportPoint(enemyPosition.ToVector3ZUp());
        else
          this.CachedEnemyViewportPoint = new Vector3(-1f, -1f, 0.0f);
      }

      public RenderTexture GetCachedFrame()
      {
        return (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST) && !this.IsInIntro ? this.GetCachedFrame_VeryLowSettings() : this.GetComponent<GenericFullscreenEffect>().GetCachedFrame();
      }

      public void ClearCachedFrame()
      {
        this.ClearCachedFrame_VeryLowSettings();
        this.GetComponent<GenericFullscreenEffect>().ClearCachedFrame();
      }

      private Material GetMaterial(Shader shader)
      {
        Material material1;
        if (this._shaderMap.TryGetValue(shader, out material1))
          return material1;
        Material material2 = new Material(shader);
        this._shaderMap.Add(shader, material2);
        return material2;
      }

      public static bool IsValidReflectionObject(tk2dBaseSprite source)
      {
        return (UnityEngine.Object) source.gameActor != (UnityEngine.Object) null;
      }

      internal class OcclusionCellData
      {
        public CellData cell;
        public float distance;
        public float changePercentModifier = 1f;

        public OcclusionCellData(CellData c, float dist)
        {
          this.cell = c;
          this.distance = dist;
        }
      }

      private enum CoreRenderMode
      {
        NORMAL,
        LOW_QUALITY,
        FAST_SCALING,
      }
    }

}
