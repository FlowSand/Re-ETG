// Decompiled with JetBrains decompiler
// Type: AmmonomiconController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.UI.Ammonomicon
{
    public class AmmonomiconController : MonoBehaviour
    {
      public static string AmmonomiconErrorSprite = "zombullet_idle_front_001";
      private static AmmonomiconController m_instance;
      public string AmmonomiconEquipmentLeftPagePath;
      public string AmmonomiconEquipmentRightPagePath;
      public List<AmmonomiconFrameDefinition> OpenAnimationFrames;
      public List<AmmonomiconFrameDefinition> TurnPageRightAnimationFrames;
      public List<AmmonomiconFrameDefinition> TurnPageLeftAnimationFrames;
      public List<AmmonomiconFrameDefinition> CloseAnimationFrames;
      public AnimationCurve DepartureYCurve;
      public float TotalDepartureTime = 0.5f;
      public float DepartureYTotalDistance = -5f;
      public tk2dSpriteCollectionData EncounterIconCollection;
      private AmmonomiconFrameDefinition m_currentFrameDefinition;
      public GameObject AmmonomiconBasePrefab;
      [SerializeField]
      private float GLOBAL_ANIMATION_SCALE = 1f;
      private GameObject m_AmmonomiconBase;
      private AmmonomiconInstanceManager m_AmmonomiconInstance;
      private MeshRenderer m_LowerRenderTargetPrefab;
      private MeshRenderer m_UpperRenderTargetPrefab;
      private dfTextureSprite m_AmmonomiconLowerImage;
      private dfTextureSprite m_AmmonomiconUpperImage;
      private dfTextureSprite m_AmmonomiconOptionalThirdImage;
      private dfTextureSprite m_CurrentLeft_RenderTarget;
      private dfTextureSprite m_CurrentRight_RenderTarget;
      private AmmonomiconPageRenderer m_CurrentLeftPageManager;
      private AmmonomiconPageRenderer m_CurrentRightPageManager;
      private AmmonomiconPageRenderer m_ImpendingLeftPageManager;
      private AmmonomiconPageRenderer m_ImpendingRightPageManager;
      private bool m_isOpening;
      private bool m_isOpen;
      private bool m_isPageTransitioning;
      private List<bool> m_offsetInUse = new List<bool>();
      private List<Vector3> m_offsets = new List<Vector3>();
      private const float m_PAGE_DEPTH = -0.5f;
      private bool m_applicationFocus = true;
      public bool HandlingQueuedUnlocks;
      private float m_cachedCorePanelY = -1f;
      private Dictionary<AmmonomiconPageRenderer.PageType, AmmonomiconPageRenderer> m_extantPageMap = new Dictionary<AmmonomiconPageRenderer.PageType, AmmonomiconPageRenderer>();
      private bool m_transitionIsQueued;
      private string m_queuedLeftPath;
      private AmmonomiconPageRenderer.PageType m_queuedLeftType;
      private string m_queuedRightPath;
      private AmmonomiconPageRenderer.PageType m_queuedRightType;
      private bool m_queuedNextPage;
      private bool m_isClosing;

      public static bool GuiManagerIsPageRenderer(dfGUIManager manager)
      {
        return (Object) AmmonomiconController.m_instance != (Object) null && AmmonomiconController.m_instance.IsOpen && (Object) AmmonomiconController.m_instance.m_AmmonomiconInstance != (Object) null && (Object) AmmonomiconController.m_instance.m_AmmonomiconInstance.GetComponent<dfGUIManager>() == (Object) manager;
      }

      public static AmmonomiconController Instance
      {
        get
        {
          if (BraveUtility.isLoadingLevel)
            return (AmmonomiconController) null;
          if ((Object) GameManager.Instance.Dungeon == (Object) null)
            return (AmmonomiconController) null;
          if ((Object) AmmonomiconController.m_instance == (Object) null)
          {
            AmmonomiconController ammonomiconController = Object.FindObjectOfType<AmmonomiconController>();
            if ((Object) ammonomiconController == (Object) null)
            {
              UnityEngine.Debug.LogError((object) "INSTANTIATING AMMONOMICON ???");
              ammonomiconController = ((GameObject) Object.Instantiate(BraveResources.Load("Ammonomicon Controller"))).GetComponent<AmmonomiconController>();
            }
            AmmonomiconController.m_instance = ammonomiconController;
          }
          return AmmonomiconController.m_instance;
        }
        set => AmmonomiconController.m_instance = value;
      }

      public static bool HasInstance => (bool) (Object) AmmonomiconController.m_instance;

      public static AmmonomiconController ForceInstance
      {
        get
        {
          if ((Object) AmmonomiconController.m_instance == (Object) null)
          {
            AmmonomiconController ammonomiconController = Object.FindObjectOfType<AmmonomiconController>();
            if ((Object) ammonomiconController == (Object) null)
            {
              UnityEngine.Debug.LogError((object) "INSTANTIATING AMMONOMICON ???");
              ammonomiconController = ((GameObject) Object.Instantiate(BraveResources.Load("Ammonomicon Controller"))).GetComponent<AmmonomiconController>();
            }
            AmmonomiconController.m_instance = ammonomiconController;
          }
          return AmmonomiconController.m_instance;
        }
      }

      public static void EnsureExistence()
      {
        if ((Object) GameManager.Instance.Dungeon == (Object) null || !((Object) AmmonomiconController.m_instance == (Object) null))
          return;
        AmmonomiconController ammonomiconController = Object.FindObjectOfType<AmmonomiconController>();
        if ((Object) ammonomiconController == (Object) null)
        {
          UnityEngine.Debug.LogError((object) "INSTANTIATING AMMONOMICON ???");
          ammonomiconController = ((GameObject) Object.Instantiate(BraveResources.Load("Ammonomicon Controller"))).GetComponent<AmmonomiconController>();
        }
        AmmonomiconController.m_instance = ammonomiconController;
      }

      public bool IsOpening => this.m_isOpening;

      public bool IsClosing => this.m_isClosing;

      public bool IsOpen => this.m_isOpen;

      public bool BookmarkHasFocus => this.m_isOpen && this.m_AmmonomiconInstance.BookmarkHasFocus;

      public void ReturnFocusToBookmark()
      {
        this.m_AmmonomiconInstance.bookmarks[this.m_AmmonomiconInstance.CurrentlySelectedTabIndex].ForceFocus();
      }

      public AmmonomiconInstanceManager Ammonomicon => this.m_AmmonomiconInstance;

      public AmmonomiconPageRenderer BestInteractingLeftPageRenderer
      {
        get
        {
          return this.IsTurningPage && (Object) this.ImpendingLeftPageRenderer != (Object) null ? this.ImpendingLeftPageRenderer : this.CurrentLeftPageRenderer;
        }
      }

      public AmmonomiconPageRenderer BestInteractingRightPageRenderer
      {
        get
        {
          return this.IsTurningPage && (Object) this.ImpendingRightPageRenderer != (Object) null ? this.ImpendingRightPageRenderer : this.CurrentRightPageRenderer;
        }
      }

      public AmmonomiconPageRenderer CurrentLeftPageRenderer => this.m_CurrentLeftPageManager;

      public AmmonomiconPageRenderer CurrentRightPageRenderer => this.m_CurrentRightPageManager;

      public AmmonomiconPageRenderer ImpendingLeftPageRenderer => this.m_ImpendingLeftPageManager;

      public AmmonomiconPageRenderer ImpendingRightPageRenderer => this.m_ImpendingRightPageManager;

      public bool IsTurningPage => this.m_isPageTransitioning;

      private void Awake()
      {
        for (int index = 0; index < 12; ++index)
        {
          this.m_offsetInUse.Add(false);
          this.m_offsets.Add(new Vector3((float) (-20 * index - 200), (float) (-20 * index - 200), 0.0f));
        }
      }

      private void Start() => this.PrecacheAllData();

      public void PrecacheAllData()
      {
        this.m_AmmonomiconBase = Object.Instantiate<GameObject>(this.AmmonomiconBasePrefab, new Vector3(-500f, -500f, 0.0f), Quaternion.identity);
        this.m_AmmonomiconInstance = this.m_AmmonomiconBase.GetComponent<AmmonomiconInstanceManager>();
        Transform transform1 = this.m_AmmonomiconBase.transform.Find("Core");
        this.m_AmmonomiconLowerImage = transform1.Find("Ammonomicon Bottom").GetComponent<dfTextureSprite>();
        this.m_AmmonomiconUpperImage = transform1.Find("Ammonomicon Top").GetComponent<dfTextureSprite>();
        this.m_AmmonomiconOptionalThirdImage = transform1.Find("Ammonomicon Toppest").GetComponent<dfTextureSprite>();
        this.m_AmmonomiconOptionalThirdImage.Material = new Material(ShaderCache.Acquire("Daikon Forge/Default UI Shader Highest Queue"));
        this.m_AmmonomiconOptionalThirdImage.IsVisible = false;
        this.m_AmmonomiconUpperImage.Material = new Material(ShaderCache.Acquire("Daikon Forge/Default UI Shader High Queue"));
        this.m_LowerRenderTargetPrefab = transform1.Find("Ammonomicon Page Renderer Lower").GetComponent<MeshRenderer>();
        this.m_LowerRenderTargetPrefab.enabled = false;
        this.m_UpperRenderTargetPrefab = transform1.Find("Ammonomicon Page Renderer Upper").GetComponent<MeshRenderer>();
        this.m_UpperRenderTargetPrefab.enabled = false;
        this.m_AmmonomiconInstance.GuiManager.RenderCamera.enabled = false;
        this.m_AmmonomiconInstance.GuiManager.enabled = false;
        AmmonomiconInstanceManager component = this.AmmonomiconBasePrefab.GetComponent<AmmonomiconInstanceManager>();
        Transform transform2 = new GameObject("_Ammonomicon").transform;
        this.m_AmmonomiconBase.transform.parent = transform2;
        this.transform.parent = transform2;
        for (int index = 0; index < component.bookmarks.Length - 1; ++index)
        {
          AmmonomiconPageRenderer ammonomiconPageRenderer1 = this.LoadPageUIAtPath(component.bookmarks[index].TargetNewPageLeft, component.bookmarks[index].LeftPageType, true);
          AmmonomiconPageRenderer ammonomiconPageRenderer2 = this.LoadPageUIAtPath(component.bookmarks[index].TargetNewPageRight, component.bookmarks[index].RightPageType, true);
          ammonomiconPageRenderer1.transform.parent.parent = transform2;
          ammonomiconPageRenderer2.transform.parent.parent = transform2;
        }
        Object.DontDestroyOnLoad((Object) transform2.gameObject);
      }

      private void OpenInternal(bool isDeath, bool isVictory, EncounterTrackable targetTrackable = null)
      {
        this.m_isOpening = true;
        while ((Object) dfGUIManager.GetModalControl() != (Object) null)
        {
          UnityEngine.Debug.LogError((object) (dfGUIManager.GetModalControl().name + " was modal, popping..."));
          dfGUIManager.PopModal();
        }
        this.m_isPageTransitioning = true;
        this.m_AmmonomiconInstance.GuiManager.enabled = true;
        this.m_AmmonomiconInstance.GuiManager.RenderCamera.enabled = true;
        int index = !isDeath ? 0 : this.m_AmmonomiconInstance.bookmarks.Length - 1;
        this.m_CurrentLeftPageManager = this.LoadPageUIAtPath(this.m_AmmonomiconInstance.bookmarks[index].TargetNewPageLeft, !isDeath ? AmmonomiconPageRenderer.PageType.EQUIPMENT_LEFT : AmmonomiconPageRenderer.PageType.DEATH_LEFT, isVictory: isVictory);
        this.m_CurrentRightPageManager = this.LoadPageUIAtPath(this.m_AmmonomiconInstance.bookmarks[index].TargetNewPageRight, !isDeath ? AmmonomiconPageRenderer.PageType.EQUIPMENT_RIGHT : AmmonomiconPageRenderer.PageType.DEATH_RIGHT, isVictory: isVictory);
        this.m_CurrentLeftPageManager.ForceUpdateLanguageFonts();
        this.m_CurrentRightPageManager.ForceUpdateLanguageFonts();
        if (this.m_CurrentRightPageManager.pageType == AmmonomiconPageRenderer.PageType.EQUIPMENT_RIGHT && this.m_CurrentLeftPageManager.LastFocusTarget != null)
        {
          AmmonomiconPokedexEntry component = (this.m_CurrentLeftPageManager.LastFocusTarget as dfButton).GetComponent<AmmonomiconPokedexEntry>();
          this.m_CurrentRightPageManager.SetRightDataPageTexts((tk2dBaseSprite) component.ChildSprite, component.linkedEncounterTrackable);
        }
        else if (this.m_CurrentRightPageManager.pageType == AmmonomiconPageRenderer.PageType.EQUIPMENT_RIGHT)
          this.m_CurrentRightPageManager.SetRightDataPageUnknown();
        this.m_CurrentRightPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconPageShader");
        this.StartCoroutine(this.HandleOpenAmmonomicon(isDeath, GameManager.Options.HasEverSeenAmmonomicon, targetTrackable));
        GameManager.Options.HasEverSeenAmmonomicon = true;
      }

      public void OpenAmmonomiconToTrackable(EncounterTrackable targetTrackable)
      {
        if (this.m_isOpen || this.m_isOpening)
          return;
        this.m_isOpen = true;
        this.OpenInternal(false, false, targetTrackable);
      }

      public void OpenAmmonomicon(bool isDeath, bool isVictory)
      {
        if (this.m_isOpen || this.m_isOpening)
          return;
        this.m_isOpen = true;
        this.OpenInternal(isDeath, isVictory);
      }

      private void LateUpdate()
      {
        if ((Object) Pixelator.Instance == (Object) null || !((Object) this.m_AmmonomiconBase != (Object) null) || GameManager.Instance.IsLoadingLevel)
          return;
        dfGUIManager component = this.m_AmmonomiconBase.GetComponent<dfGUIManager>();
        component.UIScale = Pixelator.Instance.ScaleTileScale / 3f;
        Vector2 screenSize = component.GetScreenSize();
        Vector2 vector2 = new Vector2(screenSize.x / 1920f, screenSize.y / 1080f);
        float num = Pixelator.Instance.ScaleTileScale / 3f;
        if ((Object) this.m_CurrentLeftPageManager != (Object) null)
        {
          this.m_CurrentLeftPageManager.targetRenderer.transform.localScale = new Vector3(1.77777779f * vector2.x, 2f * vector2.x, 1f) * num;
          this.m_CurrentLeftPageManager.targetRenderer.transform.localPosition = new Vector3(-0.5f * this.m_CurrentLeftPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
          if (this.m_currentFrameDefinition != null)
            this.m_CurrentLeftPageManager.targetRenderer.transform.localPosition += Vector3.Scale(this.m_currentFrameDefinition.CurrentLeftOffset, this.m_CurrentLeftPageManager.targetRenderer.transform.localScale);
        }
        if ((Object) this.m_CurrentRightPageManager != (Object) null)
        {
          this.m_CurrentRightPageManager.targetRenderer.transform.localScale = new Vector3(1.77777779f * vector2.x, 2f * vector2.x, 1f) * num;
          this.m_CurrentRightPageManager.targetRenderer.transform.localPosition = new Vector3(0.5f * this.m_CurrentRightPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
          if (this.m_currentFrameDefinition != null)
            this.m_CurrentRightPageManager.targetRenderer.transform.localPosition += Vector3.Scale(this.m_currentFrameDefinition.CurrentRightOffset, this.m_CurrentRightPageManager.targetRenderer.transform.localScale);
        }
        if ((Object) this.m_ImpendingLeftPageManager != (Object) null)
        {
          this.m_ImpendingLeftPageManager.targetRenderer.transform.localScale = new Vector3(1.77777779f * vector2.x, 2f * vector2.x, 1f) * num;
          this.m_ImpendingLeftPageManager.targetRenderer.transform.localPosition = new Vector3(-0.5f * this.m_ImpendingLeftPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
          if (this.m_currentFrameDefinition != null)
            this.m_ImpendingLeftPageManager.targetRenderer.transform.localPosition += Vector3.Scale(this.m_currentFrameDefinition.ImpendingLeftOffset, this.m_ImpendingLeftPageManager.targetRenderer.transform.localScale);
        }
        if ((Object) this.m_ImpendingRightPageManager != (Object) null)
        {
          this.m_ImpendingRightPageManager.targetRenderer.transform.localScale = new Vector3(1.77777779f * vector2.x, 2f * vector2.x, 1f) * num;
          this.m_ImpendingRightPageManager.targetRenderer.transform.localPosition = new Vector3(0.5f * this.m_ImpendingRightPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
          if (this.m_currentFrameDefinition != null)
            this.m_ImpendingRightPageManager.targetRenderer.transform.localPosition += Vector3.Scale(this.m_currentFrameDefinition.ImpendingRightOffset, this.m_ImpendingRightPageManager.targetRenderer.transform.localScale);
        }
        if (!((Object) this.m_CurrentLeftPageManager != (Object) null) || !((Object) this.m_CurrentRightPageManager != (Object) null))
          return;
        if ((double) Input.mousePosition.x > (double) Screen.width / 2.0)
        {
          if ((double) this.m_CurrentRightPageManager.guiManager.RenderCamera.depth > (double) this.m_CurrentLeftPageManager.guiManager.RenderCamera.depth)
            return;
          this.m_CurrentRightPageManager.guiManager.RenderCamera.depth = 4f;
        }
        else
        {
          if ((double) this.m_CurrentLeftPageManager.guiManager.RenderCamera.depth > (double) this.m_CurrentRightPageManager.guiManager.RenderCamera.depth)
            return;
          this.m_CurrentRightPageManager.guiManager.RenderCamera.depth = 1f;
        }
      }

      public void OnApplicationFocus(bool focusStatus) => this.m_applicationFocus = focusStatus;

      [DebuggerHidden]
      private IEnumerator HandleOpenAmmonomicon(
        bool isDeath,
        bool isShortAnimation,
        EncounterTrackable targetTrackable = null)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AmmonomiconController.\u003CHandleOpenAmmonomicon\u003Ec__Iterator0()
        {
          isShortAnimation = isShortAnimation,
          isDeath = isDeath,
          targetTrackable = targetTrackable,
          \u0024this = this
        };
      }

      private void HandleQueuedUnlocks()
      {
        List<EncounterDatabaseEntry> queuedTrackables = GameManager.Instance.GetQueuedTrackables();
        if (queuedTrackables.Count > 0)
          this.StartCoroutine(this.HandleQueuedUnlocks_CR(queuedTrackables));
        else
          this.m_isOpening = false;
      }

      [DebuggerHidden]
      private IEnumerator HandleQueuedUnlocks_CR(List<EncounterDatabaseEntry> trackableData)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AmmonomiconController.\u003CHandleQueuedUnlocks_CR\u003Ec__Iterator1()
        {
          trackableData = trackableData,
          \u0024this = this
        };
      }

      private void SetFrame(AmmonomiconFrameDefinition def)
      {
        this.m_currentFrameDefinition = def;
        this.m_AmmonomiconLowerImage.IsVisible = (Object) def.AmmonomiconBottomLayerTexture != (Object) null;
        if (this.m_AmmonomiconLowerImage.IsVisible)
          this.m_AmmonomiconLowerImage.Texture = (Texture) def.AmmonomiconBottomLayerTexture;
        this.m_AmmonomiconUpperImage.IsVisible = (Object) def.AmmonomiconTopLayerTexture != (Object) null;
        if (this.m_AmmonomiconUpperImage.IsVisible)
          this.m_AmmonomiconUpperImage.Texture = (Texture) def.AmmonomiconTopLayerTexture;
        if ((Object) def.AmmonomiconToppestLayerTexture != (Object) null)
        {
          this.m_AmmonomiconOptionalThirdImage.IsVisible = true;
          this.m_AmmonomiconOptionalThirdImage.Texture = (Texture) def.AmmonomiconToppestLayerTexture;
        }
        else
          this.m_AmmonomiconOptionalThirdImage.IsVisible = false;
        if ((Object) this.m_CurrentLeftPageManager != (Object) null)
        {
          this.m_CurrentLeftPageManager.targetRenderer.transform.localPosition = new Vector3(-0.5f * this.m_CurrentLeftPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
          this.m_CurrentLeftPageManager.targetRenderer.transform.localPosition += Vector3.Scale(def.CurrentLeftOffset, this.m_CurrentLeftPageManager.targetRenderer.transform.localScale);
          this.m_CurrentLeftPageManager.targetRenderer.enabled = def.CurrentLeftVisible;
          if (def.CurrentLeftVisible)
            this.m_CurrentLeftPageManager.SetMatrix(def.CurrentLeftMatrix);
        }
        if ((Object) this.m_CurrentRightPageManager != (Object) null)
        {
          this.m_CurrentRightPageManager.targetRenderer.transform.localPosition = new Vector3(0.5f * this.m_CurrentRightPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
          this.m_CurrentRightPageManager.targetRenderer.transform.localPosition += Vector3.Scale(def.CurrentRightOffset, this.m_CurrentRightPageManager.targetRenderer.transform.localScale);
          this.m_CurrentRightPageManager.targetRenderer.enabled = def.CurrentRightVisible;
          if (def.CurrentRightVisible)
            this.m_CurrentRightPageManager.SetMatrix(def.CurrentRightMatrix);
        }
        if ((Object) this.m_ImpendingLeftPageManager != (Object) null)
        {
          this.m_ImpendingLeftPageManager.targetRenderer.transform.localPosition = new Vector3(-0.5f * this.m_ImpendingLeftPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
          this.m_ImpendingLeftPageManager.targetRenderer.transform.localPosition += Vector3.Scale(def.ImpendingLeftOffset, this.m_ImpendingLeftPageManager.targetRenderer.transform.localScale);
          this.m_ImpendingLeftPageManager.targetRenderer.enabled = def.ImpendingLeftVisible;
          if (def.ImpendingLeftVisible)
            this.m_ImpendingLeftPageManager.SetMatrix(def.ImpendingLeftMatrix);
        }
        if (!((Object) this.m_ImpendingRightPageManager != (Object) null))
          return;
        this.m_ImpendingRightPageManager.targetRenderer.transform.localPosition = new Vector3(0.5f * this.m_ImpendingRightPageManager.targetRenderer.transform.localScale.x, 0.0f, -0.5f);
        this.m_ImpendingRightPageManager.targetRenderer.transform.localPosition += Vector3.Scale(def.ImpendingRightOffset, this.m_ImpendingRightPageManager.targetRenderer.transform.localScale);
        this.m_ImpendingRightPageManager.targetRenderer.enabled = def.ImpendingRightVisible;
        if (!def.ImpendingRightVisible)
          return;
        this.m_ImpendingRightPageManager.SetMatrix(def.ImpendingRightMatrix);
      }

      public void CloseAmmonomicon(bool doDestroy = false)
      {
        if (this.m_isClosing || this.m_isOpening)
          return;
        int num1 = (int) AkSoundEngine.PostEvent("Stop_UI_ammonomicon_open_01", this.gameObject);
        int num2 = (int) AkSoundEngine.PostEvent("Play_UI_menu_back_01", this.gameObject);
        this.m_isClosing = true;
        this.m_isPageTransitioning = true;
        this.StartCoroutine(this.HandleCloseAmmonomicon(doDestroy));
      }

      private void ForceTerminateClosing() => this.m_isClosing = false;

      [DebuggerHidden]
      private IEnumerator HandleCloseMotion()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AmmonomiconController.\u003CHandleCloseMotion\u003Ec__Iterator2()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator HandleCloseAmmonomicon(bool doDestroy = false)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AmmonomiconController.\u003CHandleCloseAmmonomicon\u003Ec__Iterator3()
        {
          \u0024this = this
        };
      }

      private float GetAnimationLength(List<AmmonomiconFrameDefinition> frames)
      {
        float animationLength = 0.0f;
        for (int index = 0; index < frames.Count; ++index)
          animationLength += frames[index].frameTime * this.GLOBAL_ANIMATION_SCALE;
        return animationLength;
      }

      private AmmonomiconPageRenderer LoadPageUIAtPath(
        string path,
        AmmonomiconPageRenderer.PageType pageType,
        bool isPreCache = false,
        bool isVictory = false)
      {
        AmmonomiconPageRenderer ammonomiconPageRenderer;
        if (this.m_extantPageMap.ContainsKey(pageType))
        {
          ammonomiconPageRenderer = this.m_extantPageMap[pageType];
          if (pageType == AmmonomiconPageRenderer.PageType.DEATH_LEFT || pageType == AmmonomiconPageRenderer.PageType.DEATH_RIGHT)
            ammonomiconPageRenderer.transform.parent.GetComponent<AmmonomiconDeathPageController>().isVictoryPage = isVictory;
          ammonomiconPageRenderer.EnableRendering();
          ammonomiconPageRenderer.DoRefreshData();
        }
        else
        {
          GameObject gameObject1 = (GameObject) Object.Instantiate(BraveResources.Load(path));
          ammonomiconPageRenderer = gameObject1.GetComponentInChildren<AmmonomiconPageRenderer>();
          dfGUIManager component1 = this.m_AmmonomiconBase.GetComponent<dfGUIManager>();
          GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_LowerRenderTargetPrefab.gameObject);
          gameObject2.transform.parent = component1.transform.Find("Core");
          gameObject2.transform.localPosition = Vector3.zero;
          gameObject2.layer = LayerMask.NameToLayer("SecondaryGUI");
          MeshRenderer component2 = gameObject2.GetComponent<MeshRenderer>();
          if (isVictory)
            ammonomiconPageRenderer.transform.parent.GetComponent<AmmonomiconDeathPageController>().isVictoryPage = true;
          ammonomiconPageRenderer.Initialize(component2);
          ammonomiconPageRenderer.EnableRendering();
          for (int index = 0; index < this.m_offsets.Count; ++index)
          {
            if (!this.m_offsetInUse[index])
            {
              this.m_offsetInUse[index] = true;
              gameObject1.transform.position = this.m_offsets[index];
              ammonomiconPageRenderer.offsetIndex = index;
              break;
            }
          }
          this.m_extantPageMap.Add(pageType, ammonomiconPageRenderer);
          if (isPreCache)
            ammonomiconPageRenderer.Disable(isPreCache);
          else
            ammonomiconPageRenderer.transform.parent.parent = this.m_AmmonomiconBase.transform.parent;
        }
        return ammonomiconPageRenderer;
      }

      private void MakeImpendingCurrent()
      {
        if (!this.m_isOpen)
          return;
        this.m_CurrentLeftPageManager.Disable();
        this.m_CurrentRightPageManager.Disable();
        this.m_CurrentLeftPageManager = this.m_ImpendingLeftPageManager;
        this.m_CurrentRightPageManager = this.m_ImpendingRightPageManager;
        this.m_CurrentLeftPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconPageShader");
        this.m_CurrentRightPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconPageShader");
        this.m_ImpendingLeftPageManager = (AmmonomiconPageRenderer) null;
        this.m_ImpendingRightPageManager = (AmmonomiconPageRenderer) null;
      }

      public void TurnToPreviousPage(
        string pathToNextLeftPage,
        AmmonomiconPageRenderer.PageType leftPageType,
        string pathToNextRightPage,
        AmmonomiconPageRenderer.PageType rightPageType)
      {
        if (this.m_isPageTransitioning)
        {
          this.SetQueuedTransition(false, pathToNextLeftPage, leftPageType, pathToNextRightPage, rightPageType);
        }
        else
        {
          this.m_isPageTransitioning = true;
          this.m_ImpendingLeftPageManager = this.LoadPageUIAtPath(pathToNextLeftPage, leftPageType);
          this.m_ImpendingRightPageManager = this.LoadPageUIAtPath(pathToNextRightPage, rightPageType);
          this.m_ImpendingLeftPageManager.UpdateOnBecameActive();
          this.m_ImpendingRightPageManager.UpdateOnBecameActive();
          this.m_ImpendingRightPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconTransitionPageShader");
          this.m_CurrentLeftPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconTransitionPageShader");
          this.m_CurrentRightPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconPageShader");
          this.m_ImpendingLeftPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconPageShader");
          this.StartCoroutine(this.HandleTurnToPreviousPage());
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleTurnToPreviousPage()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AmmonomiconController.\u003CHandleTurnToPreviousPage\u003Ec__Iterator4()
        {
          \u0024this = this
        };
      }

      private void SetQueuedTransition(
        bool nextPage,
        string pathToNextLeftPage,
        AmmonomiconPageRenderer.PageType leftPageType,
        string pathToNextRightPage,
        AmmonomiconPageRenderer.PageType rightPageType)
      {
        if (this.m_isClosing)
          return;
        if (this.m_isPageTransitioning && this.ImpendingLeftPageRenderer.pageType == leftPageType)
        {
          this.m_transitionIsQueued = false;
        }
        else
        {
          this.m_transitionIsQueued = true;
          this.m_queuedLeftPath = pathToNextLeftPage;
          this.m_queuedLeftType = leftPageType;
          this.m_queuedRightPath = pathToNextRightPage;
          this.m_queuedRightType = rightPageType;
          this.m_queuedNextPage = nextPage;
        }
      }

      public void TurnToNextPage(
        string pathToNextLeftPage,
        AmmonomiconPageRenderer.PageType leftPageType,
        string pathToNextRightPage,
        AmmonomiconPageRenderer.PageType rightPageType)
      {
        if (this.m_isPageTransitioning)
        {
          this.SetQueuedTransition(true, pathToNextLeftPage, leftPageType, pathToNextRightPage, rightPageType);
        }
        else
        {
          this.m_isPageTransitioning = true;
          this.m_ImpendingLeftPageManager = this.LoadPageUIAtPath(pathToNextLeftPage, leftPageType);
          this.m_ImpendingRightPageManager = this.LoadPageUIAtPath(pathToNextRightPage, rightPageType);
          this.m_ImpendingLeftPageManager.UpdateOnBecameActive();
          this.m_ImpendingRightPageManager.UpdateOnBecameActive();
          this.m_ImpendingLeftPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconTransitionPageShader");
          this.m_CurrentRightPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconTransitionPageShader");
          this.m_CurrentLeftPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconPageShader");
          this.m_ImpendingRightPageManager.targetRenderer.sharedMaterial.shader = ShaderCache.Acquire("Custom/AmmonomiconPageShader");
          this.StartCoroutine(this.HandleTurnToNextPage());
        }
      }

      [DebuggerHidden]
      private IEnumerator HandleTurnToNextPage()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AmmonomiconController.\u003CHandleTurnToNextPage\u003Ec__Iterator5()
        {
          \u0024this = this
        };
      }
    }

}
