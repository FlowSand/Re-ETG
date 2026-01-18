// Decompiled with JetBrains decompiler
// Type: AmmonomiconPageRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable disable

public class AmmonomiconPageRenderer : MonoBehaviour
  {
    public AmmonomiconPageRenderer.PageType pageType;
    public MeshRenderer targetRenderer;
    public dfGUIManager guiManager;
    public int offsetIndex = -1;
    public dfSprite HeaderBGSprite;
    private IAmmonomiconFocusable m_lastFocusTarget;
    [NonSerialized]
    public dfControl PrimaryClipPanel;
    private Camera m_camera;
    private Material renderMaterial;
    private int topBezierPropID;
    private int leftBezierPropID;
    private int rightBezierPropID;
    private int bottomBezierPropID;
    private List<AmmonomiconPokedexEntry> m_pokedexEntries = new List<AmmonomiconPokedexEntry>();
    private RenderTexture m_renderBuffer;
    private dfFontBase EnglishFont;
    private dfFontBase OtherLanguageFont;
    private dfFontBase BaseAlagardFont;
    private dfFontBase OtherAlagardFont;
    private float? OriginalHeaderRelativeY;
    private bool m_hasAdjustedForChinese;
    private StringTableManager.GungeonSupportedLanguages m_cachedLanguage;
    private List<dfButton> m_prevLineButtons;

    public IAmmonomiconFocusable LastFocusTarget
    {
      get => this.m_lastFocusTarget;
      set => this.m_lastFocusTarget = value;
    }

    public void Awake()
    {
      if (this.pageType == AmmonomiconPageRenderer.PageType.EQUIPMENT_LEFT)
      {
        this.BaseAlagardFont = this.transform.parent.Find("Scroll Panel").GetComponent<dfScrollPanel>().transform.Find("Header").GetComponentsInChildren<dfLabel>()[0].Font;
        this.OtherAlagardFont = (dfFontBase) (BraveResources.Load("Alternate Fonts/AlagardExtended22") as GameObject).GetComponent<dfFont>();
      }
      else
      {
        if (this.pageType != AmmonomiconPageRenderer.PageType.EQUIPMENT_RIGHT)
          return;
        dfLabel component = this.transform.parent.Find("Scroll Panel").GetComponent<dfScrollPanel>().transform.Find("Scroll Panel").Find("Panel").Find("Label").GetComponent<dfLabel>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        this.EnglishFont = component.Font;
        this.OtherLanguageFont = GameUIRoot.Instance.Manager.DefaultFont;
      }
    }

    public List<AmmonomiconPokedexEntry> GetPokedexEntries() => this.m_pokedexEntries;

    public AmmonomiconPokedexEntry GetPokedexEntry(EncounterTrackable targetTrackable)
    {
      for (int index = 0; index < this.m_pokedexEntries.Count; ++index)
      {
        if (this.m_pokedexEntries[index].linkedEncounterTrackable.myGuid == targetTrackable.EncounterGuid)
          return this.m_pokedexEntries[index];
      }
      return (AmmonomiconPokedexEntry) null;
    }

    protected void ToggleHeaderImage()
    {
      if (this.pageType != AmmonomiconPageRenderer.PageType.EQUIPMENT_LEFT && this.pageType != AmmonomiconPageRenderer.PageType.GUNS_LEFT && this.pageType != AmmonomiconPageRenderer.PageType.ITEMS_LEFT && this.pageType != AmmonomiconPageRenderer.PageType.ENEMIES_LEFT && this.pageType != AmmonomiconPageRenderer.PageType.BOSSES_LEFT)
        return;
      if (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH && (UnityEngine.Object) this.HeaderBGSprite != (UnityEngine.Object) null)
      {
        this.HeaderBGSprite.IsVisible = false;
      }
      else
      {
        if (!((UnityEngine.Object) this.HeaderBGSprite != (UnityEngine.Object) null))
          return;
        this.HeaderBGSprite.IsVisible = true;
      }
    }

    public void ForceUpdateLanguageFonts()
    {
      AmmonomiconPageRenderer ammonomiconPageRenderer = !((UnityEngine.Object) AmmonomiconController.Instance.ImpendingRightPageRenderer != (UnityEngine.Object) null) ? AmmonomiconController.Instance.CurrentRightPageRenderer : AmmonomiconController.Instance.ImpendingRightPageRenderer;
      if (ammonomiconPageRenderer.pageType != AmmonomiconPageRenderer.PageType.DEATH_RIGHT)
      {
        dfLabel component = ammonomiconPageRenderer.guiManager.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>().transform.Find("Scroll Panel").Find("Panel").Find("Label").GetComponent<dfLabel>();
        this.CheckLanguageFonts(component);
        component.Localize();
      }
      this.ForceUpdateHeaderFonts();
    }

    private void ForceUpdateHeaderFonts()
    {
      AmmonomiconPageRenderer ammonomiconPageRenderer = !((UnityEngine.Object) AmmonomiconController.Instance.ImpendingLeftPageRenderer != (UnityEngine.Object) null) ? AmmonomiconController.Instance.CurrentLeftPageRenderer : AmmonomiconController.Instance.ImpendingLeftPageRenderer;
      if ((UnityEngine.Object) this != (UnityEngine.Object) ammonomiconPageRenderer)
        return;
      if (this.pageType == AmmonomiconPageRenderer.PageType.EQUIPMENT_LEFT)
        this.ToggleHeaderImage();
      Transform transform = ammonomiconPageRenderer.guiManager.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>().transform.Find("Header");
      dfLabel[] componentsInChildren = transform.GetComponentsInChildren<dfLabel>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
      {
        if (!this.OriginalHeaderRelativeY.HasValue && (UnityEngine.Object) componentsInChildren[index].transform.parent == (UnityEngine.Object) transform)
          this.OriginalHeaderRelativeY = new float?(componentsInChildren[index].RelativePosition.y);
        this.CheckHeaderFonts(componentsInChildren[index], (UnityEngine.Object) componentsInChildren[index].transform.parent == (UnityEngine.Object) transform);
      }
    }

    public void UpdateOnBecameActive()
    {
      this.ForceUpdateLanguageFonts();
      if (!((UnityEngine.Object) AmmonomiconController.Instance.ImpendingLeftPageRenderer == (UnityEngine.Object) null) && AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget != null)
        return;
      switch (this.pageType)
      {
        case AmmonomiconPageRenderer.PageType.GUNS_RIGHT:
          this.SetFirstVisibleTexts();
          break;
        case AmmonomiconPageRenderer.PageType.ITEMS_RIGHT:
          this.SetFirstVisibleTexts();
          break;
        case AmmonomiconPageRenderer.PageType.ENEMIES_RIGHT:
          this.SetFirstVisibleTexts();
          break;
        case AmmonomiconPageRenderer.PageType.BOSSES_RIGHT:
          this.SetFirstVisibleTexts();
          break;
      }
    }

    private void SetFirstVisibleTexts()
    {
      if ((UnityEngine.Object) AmmonomiconController.Instance.ImpendingLeftPageRenderer != (UnityEngine.Object) null)
      {
        for (int index = 0; index < AmmonomiconController.Instance.ImpendingLeftPageRenderer.m_pokedexEntries.Count; ++index)
        {
          AmmonomiconPokedexEntry pokedexEntry = AmmonomiconController.Instance.ImpendingLeftPageRenderer.m_pokedexEntries[index];
          if (pokedexEntry.encounterState == AmmonomiconPokedexEntry.EncounterState.ENCOUNTERED)
          {
            this.SetRightDataPageTexts((tk2dBaseSprite) pokedexEntry.ChildSprite, pokedexEntry.linkedEncounterTrackable);
            if (AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget != null)
              return;
            AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget = (IAmmonomiconFocusable) pokedexEntry.GetComponent<dfControl>();
            return;
          }
          if (pokedexEntry.encounterState == AmmonomiconPokedexEntry.EncounterState.KNOWN)
          {
            this.SetPageDataUnknown(this);
            this.SetRightDataPageName((tk2dBaseSprite) pokedexEntry.ChildSprite, pokedexEntry.linkedEncounterTrackable);
            if (AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget != null)
              return;
            AmmonomiconController.Instance.ImpendingLeftPageRenderer.LastFocusTarget = (IAmmonomiconFocusable) pokedexEntry.GetComponent<dfControl>();
            return;
          }
        }
      }
      this.SetPageDataUnknown(this);
    }

    public void Initialize(MeshRenderer ts)
    {
      this.targetRenderer = ts;
      this.m_camera = this.GetComponent<Camera>();
      this.m_camera.aspect = 0.8888889f;
      this.guiManager = this.transform.parent.GetComponent<dfGUIManager>();
      this.guiManager.UIScale = 1f;
      Transform transform = this.guiManager.transform.Find("Scroll Panel");
      if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
        transform.GetComponent<dfScrollPanel>().LockScrollPanelToZero = true;
      this.RebuildRenderData();
      this.topBezierPropID = Shader.PropertyToID("_TopBezier");
      this.leftBezierPropID = Shader.PropertyToID("_LeftBezier");
      this.rightBezierPropID = Shader.PropertyToID("_RightBezier");
      this.bottomBezierPropID = Shader.PropertyToID("_BottomBezier");
      Matrix4x4 matrix = new Matrix4x4();
      matrix.SetRow(0, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
      matrix.SetRow(1, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
      matrix.SetRow(2, new Vector4(1f, 1f, 1f, 1f));
      matrix.SetRow(3, new Vector4(1f, 1f, 1f, 1f));
      this.SetMatrix(matrix);
      this.StartCoroutine(this.DelayedBuildPage());
    }

    private void RebuildRenderData()
    {
      if ((UnityEngine.Object) this.m_renderBuffer != (UnityEngine.Object) null)
      {
        RenderTexture.ReleaseTemporary(this.m_renderBuffer);
        this.m_renderBuffer = (RenderTexture) null;
      }
      UnityEngine.Debug.LogWarning((object) "Reacquiring Page Buffer 960x1080");
      this.m_renderBuffer = RenderTexture.GetTemporary(960, 1080, 0, RenderTextureFormat.Default);
      this.m_renderBuffer.name = "temporary ammonomicon render buffer";
      this.m_renderBuffer.filterMode = FilterMode.Point;
      this.m_renderBuffer.DiscardContents();
      this.m_camera.targetTexture = this.m_renderBuffer;
      this.renderMaterial = new Material(ShaderCache.Acquire("Custom/AmmonomiconPageShader"));
      this.renderMaterial.SetTexture("_MainTex", (Texture) this.m_renderBuffer);
      this.targetRenderer.material = this.renderMaterial;
    }

    [DebuggerHidden]
    private IEnumerator DelayedBuildPage()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmmonomiconPageRenderer__DelayedBuildPagec__Iterator0()
      {
        _this = this
      };
    }

    private void InitializeDeathPageLeft()
    {
      this.guiManager.GetComponent<AmmonomiconDeathPageController>().DoInitialize();
    }

    private void InitializeDeathPageRight()
    {
      AmmonomiconDeathPageController component1 = this.guiManager.GetComponent<AmmonomiconDeathPageController>();
      component1.DoInitialize();
      dfPanel component2 = component1.transform.Find("Scroll Panel").Find("Footer").Find("ScrollItemsPanel").GetComponent<dfScrollPanel>().transform.Find("AllItemsPanel").GetComponent<dfPanel>();
      for (int index = 0; index < component2.transform.childCount; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) component2.transform.GetChild(index).gameObject);
      List<tk2dBaseSprite> source = new List<tk2dBaseSprite>();
      for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
        for (int index2 = 0; index2 < allPlayer.inventory.AllGuns.Count; ++index2)
        {
          Gun allGun = allPlayer.inventory.AllGuns[index2];
          tk2dClippedSprite page = this.AddSpriteToPage<tk2dClippedSprite>(allGun.GetSprite().Collection, allGun.DefaultSpriteID);
          SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) page, Color.black, 0.1f, 0.01f);
          page.transform.parent = component2.transform;
          page.transform.position = component2.GetCenter();
          source.Add((tk2dBaseSprite) page);
        }
        for (int index3 = 0; index3 < allPlayer.activeItems.Count; ++index3)
        {
          tk2dClippedSprite page = this.AddSpriteToPage<tk2dClippedSprite>(allPlayer.activeItems[index3].sprite.Collection, allPlayer.activeItems[index3].sprite.spriteId);
          SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) page, Color.black, 0.1f, 0.01f);
          page.transform.parent = component2.transform;
          page.transform.position = component2.GetCenter();
          source.Add((tk2dBaseSprite) page);
        }
        for (int index4 = 0; index4 < allPlayer.passiveItems.Count; ++index4)
        {
          tk2dClippedSprite page = this.AddSpriteToPage<tk2dClippedSprite>(allPlayer.passiveItems[index4].sprite.Collection, allPlayer.passiveItems[index4].sprite.spriteId);
          SpriteOutlineManager.AddScaledOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) page, Color.black, 0.1f, 0.01f);
          page.transform.parent = component2.transform;
          page.transform.position = component2.GetCenter();
          source.Add((tk2dBaseSprite) page);
        }
      }
      List<tk2dBaseSprite> tk2dBaseSpriteList = source.ttOrderBy<tk2dBaseSprite, float>((Func<tk2dBaseSprite, float>) (a => a.GetBounds().size.y));
      List<tk2dBaseSprite> previousLineSprites = new List<tk2dBaseSprite>();
      this.BoxArrangeItems(component2, tk2dBaseSpriteList, new Vector2(0.0f, 6f), new Vector2(6f, 3f), ref previousLineSprites);
      this.StartCoroutine(this.HandleDeathItemsClipping(component2, tk2dBaseSpriteList));
    }

    [DebuggerHidden]
    private IEnumerator HandleDeathItemsClipping(
      dfPanel parentPanel,
      List<tk2dBaseSprite> itemSprites)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmmonomiconPageRenderer__HandleDeathItemsClippingc__Iterator1()
      {
        itemSprites = itemSprites,
        parentPanel = parentPanel
      };
    }

    public void ReturnFocusToBookmarks()
    {
      this.LastFocusTarget = (IAmmonomiconFocusable) dfGUIManager.ActiveControl;
      for (int index = 0; index < AmmonomiconController.Instance.Ammonomicon.bookmarks.Length; ++index)
      {
        if (AmmonomiconController.Instance.Ammonomicon.bookmarks[index].IsCurrentPage)
        {
          AmmonomiconController.Instance.Ammonomicon.bookmarks[index].ForceFocus();
          break;
        }
      }
    }

    public void LateUpdate()
    {
      if (this.m_camera.enabled && (!(bool) (UnityEngine.Object) this.m_renderBuffer || (UnityEngine.Object) this.m_renderBuffer == (UnityEngine.Object) null || !this.m_renderBuffer.IsCreated()))
        this.RebuildRenderData();
      this.m_camera.transform.localPosition = new Vector3(0.0f, 1f / 1000f, this.m_camera.transform.localPosition.z);
    }

    public void DoRefreshData()
    {
      if (this.pageType == AmmonomiconPageRenderer.PageType.EQUIPMENT_LEFT)
      {
        for (int index = 0; index < this.m_pokedexEntries.Count; ++index)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_pokedexEntries[index].gameObject);
        this.LastFocusTarget = (IAmmonomiconFocusable) null;
        this.m_pokedexEntries.Clear();
        this.InitializeEquipmentPageLeft();
        if (this.m_pokedexEntries.Count > 0)
          this.LastFocusTarget = (IAmmonomiconFocusable) this.m_pokedexEntries[0].GetComponent<dfButton>();
        this.guiManager.UIScaleLegacyMode = true;
        this.guiManager.UIScaleLegacyMode = false;
      }
      else if (this.pageType == AmmonomiconPageRenderer.PageType.DEATH_LEFT)
        this.InitializeDeathPageLeft();
      else if (this.pageType == AmmonomiconPageRenderer.PageType.DEATH_RIGHT)
      {
        this.InitializeDeathPageRight();
      }
      else
      {
        for (int index = 0; index < this.m_pokedexEntries.Count; ++index)
          this.m_pokedexEntries[index].UpdateEncounterState();
      }
    }

    public void BoxArrangeItems(
      dfPanel sourcePanel,
      List<tk2dBaseSprite> sourceElements,
      Vector2 panelPaddingPx,
      Vector2 elementPaddingPx,
      ref List<tk2dBaseSprite> previousLineSprites)
    {
      if (previousLineSprites == null)
        previousLineSprites = new List<tk2dBaseSprite>();
      List<tk2dBaseSprite> tk2dBaseSpriteList1 = new List<tk2dBaseSprite>((IEnumerable<tk2dBaseSprite>) sourceElements);
      float units = this.guiManager.PixelsToUnits();
      float b = (sourcePanel.Width - panelPaddingPx.x * 2f) * units;
      float a1 = 0.0f;
      for (int index = 0; index < tk2dBaseSpriteList1.Count; ++index)
        a1 = Mathf.Max(a1, tk2dBaseSpriteList1[index].GetBounds().size.y + 2f * elementPaddingPx.y * units);
      float num1 = b;
      float num2 = 0.0f;
      float num3 = -1f * panelPaddingPx.y * units;
      int num4 = 1;
      List<tk2dBaseSprite> tk2dBaseSpriteList2 = new List<tk2dBaseSprite>();
      float num5 = panelPaddingPx.y * units;
      float num6 = 0.0f;
      while (tk2dBaseSpriteList1.Count > 0)
      {
        tk2dBaseSprite tk2dBaseSprite = tk2dBaseSpriteList1[0];
        tk2dBaseSpriteList1.RemoveAt(0);
        Bounds bounds = tk2dBaseSprite.GetBounds();
        Bounds untrimmedBounds = tk2dBaseSprite.GetUntrimmedBounds();
        Vector3 size1 = bounds.size;
        Vector3 size2 = untrimmedBounds.size;
        bool flag = (double) size1.x > (double) num1;
        size1.x = Mathf.Min(size1.x, b);
        size2.x = Mathf.Min(size2.x, b);
        if (!flag)
        {
          num1 -= size1.x + 2f * elementPaddingPx.x * units;
          float y = (float) ((double) num3 - (double) a1 + ((double) a1 - (double) size1.y) / 2.0);
          Vector3 position = new Vector3((float) ((double) num2 + (double) panelPaddingPx.x * (double) units + (double) elementPaddingPx.x * (double) units), y, 0.0f);
          tk2dBaseSprite.transform.parent = sourcePanel.transform;
          tk2dBaseSprite.PlaceAtLocalPositionByAnchor(position, tk2dBaseSprite.Anchor.LowerLeft);
          num2 += size1.x + 2f * elementPaddingPx.x * units;
          tk2dBaseSpriteList2.Add(tk2dBaseSprite);
        }
        if (flag || tk2dBaseSpriteList1.Count == 0)
        {
          float num7 = num1;
          for (int index = 0; index < tk2dBaseSpriteList2.Count; ++index)
            tk2dBaseSpriteList2[index].transform.localPosition += new Vector3(num7 / 2f, 0.0f, 0.0f);
          num5 += a1;
          if (previousLineSprites.Count > 0)
          {
            float a2 = 0.0f;
            for (int index = 0; index < tk2dBaseSpriteList2.Count; ++index)
              a2 = Mathf.Max(a2, tk2dBaseSpriteList2[index].GetBounds().size.y + 2f * elementPaddingPx.y * units);
            float y = a1 - a2;
            if (tk2dBaseSpriteList1.Count == 0)
              y = (float) (0.5 * (double) num6 + (double) elementPaddingPx.y * (double) units);
            if ((double) y > 0.0)
            {
              for (int index = 0; index < tk2dBaseSpriteList2.Count; ++index)
                tk2dBaseSpriteList2[index].transform.localPosition = tk2dBaseSpriteList2[index].transform.localPosition + new Vector3(0.0f, y, 0.0f);
              num3 += y;
            }
            num5 -= y;
            num6 = y;
          }
          if (flag || tk2dBaseSpriteList1.Count != 0)
          {
            num3 -= a1;
            num2 = 0.0f;
            num1 = b;
            ++num4;
            tk2dBaseSpriteList1.Insert(0, tk2dBaseSprite);
            previousLineSprites = tk2dBaseSpriteList2;
            tk2dBaseSpriteList2 = new List<tk2dBaseSprite>();
          }
        }
      }
      previousLineSprites = tk2dBaseSpriteList2;
      sourcePanel.Height = num5 / units + panelPaddingPx.y;
    }

    private void SetPageDataUnknown(AmmonomiconPageRenderer rightPage)
    {
      if ((UnityEngine.Object) rightPage == (UnityEngine.Object) null)
        return;
      dfScrollPanel component1 = rightPage.guiManager.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>();
      Transform transform = component1.transform.Find("Header");
      if ((bool) (UnityEngine.Object) transform)
      {
        dfLabel component2 = transform.Find("Label").GetComponent<dfLabel>();
        component2.Text = component2.ForceGetLocalizedValue("#AMMONOMICON_UNKNOWN");
        component2.PerformLayout();
        dfSprite component3 = transform.Find("Sprite").GetComponent<dfSprite>();
        if ((bool) (UnityEngine.Object) component3)
        {
          component3.FillDirection = dfFillDirection.Vertical;
          component3.FillAmount = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH ? 0.8f : 1f;
          component3.InvertFill = true;
        }
      }
      dfLabel component4 = component1.transform.Find("Tape Line One").Find("Label").GetComponent<dfLabel>();
      component4.Text = component4.ForceGetLocalizedValue("#AMMONOMICON_QUESTIONS");
      component4.PerformLayout();
      component1.transform.Find("Tape Line One").GetComponentInChildren<dfSlicedSprite>().Width = (float) ((double) component4.GetAutosizeWidth() / 4.0 + 12.0);
      dfLabel component5 = component1.transform.Find("Tape Line Two").Find("Label").GetComponent<dfLabel>();
      component5.Text = component4.ForceGetLocalizedValue("#AMMONOMICON_QUESTIONS");
      component5.PerformLayout();
      component1.transform.Find("Tape Line Two").GetComponentInChildren<dfSlicedSprite>().Width = (float) ((double) component5.GetAutosizeWidth() / 4.0 + 12.0);
      dfPanel component6 = component1.transform.Find("ThePhoto").Find("Photo").Find("tk2dSpriteHolder").GetComponent<dfPanel>();
      component1.transform.Find("ThePhoto").Find("Photo").Find("ItemShadow").GetComponent<dfSprite>().IsVisible = false;
      tk2dSprite componentInChildren1 = component6.GetComponentInChildren<tk2dSprite>();
      dfTextureSprite componentInChildren2 = component1.transform.Find("ThePhoto").GetComponentInChildren<dfTextureSprite>();
      if ((UnityEngine.Object) componentInChildren2 != (UnityEngine.Object) null)
        componentInChildren2.IsVisible = false;
      if (!((UnityEngine.Object) componentInChildren1 == (UnityEngine.Object) null))
      {
        if (SpriteOutlineManager.HasOutline((tk2dBaseSprite) componentInChildren1))
          SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) componentInChildren1, true);
        componentInChildren1.renderer.enabled = false;
      }
      dfLabel component7 = component1.transform.Find("Scroll Panel").Find("Panel").Find("Label").GetComponent<dfLabel>();
      this.CheckLanguageFonts(component7);
      component7.Text = component7.ForceGetLocalizedValue("#AMMONOMICON_MYSTERIOUS");
      component7.transform.parent.GetComponent<dfPanel>().Height = component7.Height;
      component1.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>().ScrollPosition = Vector2.zero;
    }

    public void SetRightDataPageUnknown(bool impending = false)
    {
      this.SetPageDataUnknown(!impending ? AmmonomiconController.Instance.CurrentRightPageRenderer : AmmonomiconController.Instance.ImpendingRightPageRenderer);
    }

    private void CheckHeaderFonts(dfLabel headerLabel, bool isPrimaryLabel)
    {
      if ((UnityEngine.Object) this.BaseAlagardFont == (UnityEngine.Object) null)
      {
        this.BaseAlagardFont = headerLabel.Font;
        this.OtherAlagardFont = (dfFontBase) (BraveResources.Load("Alternate Fonts/AlagardExtended22") as GameObject).GetComponent<dfFont>();
      }
      if (isPrimaryLabel)
        headerLabel.BringToFront();
      switch (StringTableManager.CurrentLanguage)
      {
        case StringTableManager.GungeonSupportedLanguages.ENGLISH:
          if ((UnityEngine.Object) headerLabel.Font != (UnityEngine.Object) this.BaseAlagardFont || (double) headerLabel.TextScale != 2.0)
          {
            headerLabel.Font = this.BaseAlagardFont;
            headerLabel.TextScale = 2f;
            headerLabel.PerformLayout();
          }
          if (!isPrimaryLabel || (double) headerLabel.RelativePosition.y == (double) this.OriginalHeaderRelativeY.Value)
            break;
          headerLabel.RelativePosition = headerLabel.RelativePosition.WithY(this.OriginalHeaderRelativeY.Value);
          headerLabel.PerformLayout();
          break;
        case StringTableManager.GungeonSupportedLanguages.JAPANESE:
        case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
        case StringTableManager.GungeonSupportedLanguages.CHINESE:
          if (!isPrimaryLabel || (double) headerLabel.RelativePosition.y == (double) this.OriginalHeaderRelativeY.Value)
            break;
          headerLabel.RelativePosition = headerLabel.RelativePosition.WithY(this.OriginalHeaderRelativeY.Value);
          headerLabel.PerformLayout();
          break;
        case StringTableManager.GungeonSupportedLanguages.KOREAN:
          if (!isPrimaryLabel || (double) headerLabel.RelativePosition.y == (double) this.OriginalHeaderRelativeY.Value - 16.0)
            break;
          headerLabel.RelativePosition = headerLabel.RelativePosition.WithY(this.OriginalHeaderRelativeY.Value - 16f);
          headerLabel.PerformLayout();
          break;
        default:
          if ((UnityEngine.Object) headerLabel != (UnityEngine.Object) this.OtherAlagardFont || (double) headerLabel.TextScale != 4.0)
          {
            headerLabel.Font = this.OtherAlagardFont;
            headerLabel.TextScale = 4f;
            headerLabel.PerformLayout();
          }
          if (!isPrimaryLabel || (double) headerLabel.RelativePosition.y == (double) this.OriginalHeaderRelativeY.Value - 24.0)
            break;
          headerLabel.RelativePosition = headerLabel.RelativePosition.WithY(this.OriginalHeaderRelativeY.Value - 24f);
          headerLabel.PerformLayout();
          break;
      }
    }

    private void AdjustForChinese()
    {
      if (!((UnityEngine.Object) AmmonomiconController.Instance != (UnityEngine.Object) null) || (!this.m_hasAdjustedForChinese || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE) && (this.m_hasAdjustedForChinese || GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE))
        return;
      AmmonomiconPageRenderer ammonomiconPageRenderer = !((UnityEngine.Object) AmmonomiconController.Instance.ImpendingRightPageRenderer != (UnityEngine.Object) null) ? AmmonomiconController.Instance.CurrentRightPageRenderer : AmmonomiconController.Instance.ImpendingRightPageRenderer;
      if (!((UnityEngine.Object) ammonomiconPageRenderer != (UnityEngine.Object) null))
        return;
      dfScrollPanel component1 = ammonomiconPageRenderer.guiManager.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>();
      dfLabel component2 = component1.transform.Find("Tape Line One").Find("Label").GetComponent<dfLabel>();
      dfLabel component3 = component1.transform.Find("Tape Line Two").Find("Label").GetComponent<dfLabel>();
      if (this.m_hasAdjustedForChinese && GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE)
      {
        component2.RelativePosition = component2.RelativePosition + new Vector3(0.0f, -8f, 0.0f);
        component3.RelativePosition = component3.RelativePosition + new Vector3(0.0f, -8f, 0.0f);
        this.m_hasAdjustedForChinese = false;
      }
      else
      {
        if (this.m_hasAdjustedForChinese || GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE)
          return;
        component2.RelativePosition = component2.RelativePosition + new Vector3(0.0f, 8f, 0.0f);
        component3.RelativePosition = component3.RelativePosition + new Vector3(0.0f, 8f, 0.0f);
        this.m_hasAdjustedForChinese = true;
      }
    }

    private void CheckLanguageFonts(dfLabel mainText)
    {
      if ((UnityEngine.Object) this.EnglishFont == (UnityEngine.Object) null)
      {
        this.EnglishFont = mainText.Font;
        this.OtherLanguageFont = GameUIRoot.Instance.Manager.DefaultFont;
      }
      this.AdjustForChinese();
      if (this.m_cachedLanguage != GameManager.Options.CurrentLanguage)
      {
        this.m_cachedLanguage = GameManager.Options.CurrentLanguage;
        switch (this.pageType)
        {
          case AmmonomiconPageRenderer.PageType.GUNS_RIGHT:
            this.SetPageDataUnknown(this);
            break;
          case AmmonomiconPageRenderer.PageType.ITEMS_RIGHT:
            this.SetPageDataUnknown(this);
            break;
          case AmmonomiconPageRenderer.PageType.ENEMIES_RIGHT:
            this.SetPageDataUnknown(this);
            break;
          case AmmonomiconPageRenderer.PageType.BOSSES_RIGHT:
            this.SetPageDataUnknown(this);
            break;
        }
      }
      switch (StringTableManager.CurrentLanguage)
      {
        case StringTableManager.GungeonSupportedLanguages.ENGLISH:
          if (!((UnityEngine.Object) mainText.Font != (UnityEngine.Object) this.EnglishFont))
            break;
          mainText.Atlas = this.guiManager.DefaultAtlas;
          mainText.Font = this.EnglishFont;
          break;
        case StringTableManager.GungeonSupportedLanguages.JAPANESE:
          break;
        case StringTableManager.GungeonSupportedLanguages.KOREAN:
          break;
        case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
          break;
        case StringTableManager.GungeonSupportedLanguages.CHINESE:
          break;
        default:
          if (!((UnityEngine.Object) mainText.Font != (UnityEngine.Object) this.OtherLanguageFont))
            break;
          mainText.Atlas = GameUIRoot.Instance.Manager.DefaultAtlas;
          mainText.Font = this.OtherLanguageFont;
          break;
      }
    }

    public void SetRightDataPageName(
      tk2dBaseSprite sourceSprite,
      EncounterDatabaseEntry linkedTrackable)
    {
      JournalEntry journalData = linkedTrackable.journalData;
      Transform transform = (!((UnityEngine.Object) AmmonomiconController.Instance.ImpendingRightPageRenderer != (UnityEngine.Object) null) ? AmmonomiconController.Instance.CurrentRightPageRenderer : AmmonomiconController.Instance.ImpendingRightPageRenderer).guiManager.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>().transform.Find("Header");
      if (!(bool) (UnityEngine.Object) transform)
        return;
      dfLabel component1 = transform.Find("Label").GetComponent<dfLabel>();
      component1.Text = journalData.GetPrimaryDisplayName();
      if (linkedTrackable.ForceEncounterState)
        component1.Text = component1.ForceGetLocalizedValue("#AMMONOMICON_UNKNOWN");
      component1.PerformLayout();
      dfSprite component2 = transform.Find("Sprite").GetComponent<dfSprite>();
      if (!(bool) (UnityEngine.Object) component2)
        return;
      component2.FillDirection = dfFillDirection.Vertical;
      component2.FillAmount = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH ? 0.8f : 1f;
      component2.InvertFill = true;
    }

    public void SetRightDataPageTexts(
      tk2dBaseSprite sourceSprite,
      EncounterDatabaseEntry linkedTrackable)
    {
      JournalEntry journalData = linkedTrackable.journalData;
      dfScrollPanel component1 = (!((UnityEngine.Object) AmmonomiconController.Instance.ImpendingRightPageRenderer != (UnityEngine.Object) null) ? AmmonomiconController.Instance.CurrentRightPageRenderer : AmmonomiconController.Instance.ImpendingRightPageRenderer).guiManager.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>();
      Transform transform = component1.transform.Find("Header");
      if ((bool) (UnityEngine.Object) transform)
      {
        dfLabel component2 = transform.Find("Label").GetComponent<dfLabel>();
        component2.Text = journalData.GetPrimaryDisplayName();
        if (linkedTrackable.ForceEncounterState)
          component2.Text = component2.ForceGetLocalizedValue("#AMMONOMICON_UNKNOWN");
        component2.PerformLayout();
        dfSprite component3 = transform.Find("Sprite").GetComponent<dfSprite>();
        if ((bool) (UnityEngine.Object) component3)
        {
          component3.FillDirection = dfFillDirection.Vertical;
          component3.FillAmount = GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH ? 0.8f : 1f;
          component3.InvertFill = true;
        }
      }
      dfLabel component4 = component1.transform.Find("Tape Line One").Find("Label").GetComponent<dfLabel>();
      component4.Text = journalData.GetNotificationPanelDescription();
      component4.PerformLayout();
      component1.transform.Find("Tape Line One").GetComponentInChildren<dfSlicedSprite>().Width = (float) ((double) component4.GetAutosizeWidth() / 4.0 + 12.0);
      dfLabel component5 = component1.transform.Find("Tape Line Two").Find("Label").GetComponent<dfLabel>();
      component5.Text = linkedTrackable.GetSecondTapeDescriptor();
      component5.PerformLayout();
      component1.transform.Find("Tape Line Two").GetComponentInChildren<dfSlicedSprite>().Width = (float) ((double) component5.GetAutosizeWidth() / 4.0 + 12.0);
      dfPanel component6 = component1.transform.Find("ThePhoto").Find("Photo").Find("tk2dSpriteHolder").GetComponent<dfPanel>();
      component1.transform.Find("ThePhoto").Find("Photo").Find("ItemShadow").GetComponent<dfSprite>().IsVisible = !journalData.IsEnemy;
      tk2dSprite targetSprite = component6.GetComponentInChildren<tk2dSprite>();
      dfTextureSprite componentInChildren = component1.transform.Find("ThePhoto").GetComponentInChildren<dfTextureSprite>();
      if (journalData.IsEnemy && (UnityEngine.Object) journalData.enemyPortraitSprite != (UnityEngine.Object) null)
      {
        if ((UnityEngine.Object) targetSprite != (UnityEngine.Object) null)
        {
          if (SpriteOutlineManager.HasOutline((tk2dBaseSprite) targetSprite))
            SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) targetSprite, true);
          targetSprite.renderer.enabled = false;
        }
        componentInChildren.IsVisible = true;
        componentInChildren.Texture = (Texture) journalData.enemyPortraitSprite;
      }
      else
      {
        if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          componentInChildren.IsVisible = false;
        if ((UnityEngine.Object) targetSprite == (UnityEngine.Object) null)
        {
          targetSprite = this.AddSpriteToPage(sourceSprite);
          if (!journalData.IsEnemy)
          {
            tk2dSprite tk2dSprite = targetSprite;
            tk2dSprite.scale = tk2dSprite.scale * 2f;
          }
          targetSprite.transform.parent = component6.transform;
        }
        else
        {
          targetSprite.renderer.enabled = true;
          targetSprite.SetSprite(sourceSprite.Collection, sourceSprite.spriteId);
        }
        if (SpriteOutlineManager.HasOutline((tk2dBaseSprite) targetSprite))
          SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) targetSprite, true);
        SpriteOutlineManager.AddScaledOutlineToSprite<tk2dSprite>((tk2dBaseSprite) targetSprite, Color.black, 0.1f, 0.05f);
        if (journalData.IsEnemy)
          targetSprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.MiddleCenter);
        else
          targetSprite.PlaceAtLocalPositionByAnchor(Vector3.zero, tk2dBaseSprite.Anchor.LowerCenter);
        if (Mathf.RoundToInt(sourceSprite.GetCurrentSpriteDef().GetBounds().size.x / (1f / 16f)) % 2 == 1)
          targetSprite.transform.position = targetSprite.transform.position.WithX(targetSprite.transform.position.x - 1f / 32f * targetSprite.scale.x);
        targetSprite.usesOverrideMaterial = true;
        targetSprite.renderer.material.shader = ShaderCache.Acquire("tk2d/CutoutVertexColorTilted");
      }
      dfLabel component7 = component1.transform.Find("Scroll Panel").Find("Panel").Find("Label").GetComponent<dfLabel>();
      this.CheckLanguageFonts(component7);
      component7.Text = linkedTrackable.GetModifiedLongDescription();
      component7.transform.parent.GetComponent<dfPanel>().Height = component7.Height;
      component7.PerformLayout();
      component7.Update();
      component1.transform.Find("Scroll Panel").GetComponent<dfScrollPanel>().ScrollPosition = Vector2.zero;
      component1.PerformLayout();
      component1.Update();
    }

    [DebuggerHidden]
    private IEnumerator ConstructRectanglePageLayout(
      dfPanel sourcePanel,
      List<EncounterDatabaseEntry> journalEntries,
      Vector2 panelPaddingPx,
      Vector2 elementPaddingPx,
      bool hideButtons = false,
      List<AdvancedSynergyEntry> activeSynergies = null)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmmonomiconPageRenderer__ConstructRectanglePageLayoutc__Iterator2()
      {
        hideButtons = hideButtons,
        sourcePanel = sourcePanel,
        panelPaddingPx = panelPaddingPx,
        journalEntries = journalEntries,
        elementPaddingPx = elementPaddingPx,
        activeSynergies = activeSynergies,
        _this = this
      };
    }

    private void InternalInitializeEnemiesPage(bool isBosses)
    {
      dfPanel component1 = this.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel").Find("Guns Panel").GetComponent<dfPanel>();
      List<KeyValuePair<int, EncounterDatabaseEntry>> source = new List<KeyValuePair<int, EncounterDatabaseEntry>>();
      for (int index = 0; index < EnemyDatabase.Instance.Entries.Count; ++index)
      {
        EnemyDatabaseEntry entry = EnemyDatabase.Instance.Entries[index];
        if (entry != null && entry.isInBossTab == isBosses && !string.IsNullOrEmpty(entry.encounterGuid) && !EncounterDatabase.IsProxy(entry.encounterGuid))
        {
          int key = entry.ForcedPositionInAmmonomicon >= 0 ? entry.ForcedPositionInAmmonomicon : 1000000000;
          source.Add(new KeyValuePair<int, EncounterDatabaseEntry>(key, EncounterDatabase.GetEntry(entry.encounterGuid)));
        }
      }
      List<KeyValuePair<int, EncounterDatabaseEntry>> list = source.OrderBy<KeyValuePair<int, EncounterDatabaseEntry>, int>((Func<KeyValuePair<int, EncounterDatabaseEntry>, int>) (e => e.Key)).ToList<KeyValuePair<int, EncounterDatabaseEntry>>();
      List<EncounterDatabaseEntry> journalEntries = new List<EncounterDatabaseEntry>();
      dfPanel component2 = component1.transform.GetChild(0).GetComponent<dfPanel>();
      for (int index = 0; index < list.Count; ++index)
      {
        KeyValuePair<int, EncounterDatabaseEntry> keyValuePair = list[index];
        if (keyValuePair.Value != null && !keyValuePair.Value.journalData.SuppressInAmmonomicon)
          journalEntries.Add(keyValuePair.Value);
      }
      this.StartCoroutine(this.ConstructRectanglePageLayout(component2, journalEntries, new Vector2(12f, 20f), new Vector2(20f, 20f)));
      component2.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
      component1.Height = component2.Height;
      component2.Height = component1.Height;
    }

    public void InitializeBossesPageLeft() => this.InternalInitializeEnemiesPage(true);

    public void InitializeEnemiesPageLeft() => this.InternalInitializeEnemiesPage(false);

    public void InitializeItemsPageLeft()
    {
      dfPanel component1 = this.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel").Find("Guns Panel").GetComponent<dfPanel>();
      List<KeyValuePair<int, PickupObject>> source = new List<KeyValuePair<int, PickupObject>>();
      for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
      {
        PickupObject pickupObject = PickupObjectDatabase.Instance.Objects[index];
        if (!(pickupObject is Gun) && (UnityEngine.Object) pickupObject != (UnityEngine.Object) null)
        {
          EncounterTrackable component2 = pickupObject.GetComponent<EncounterTrackable>();
          if (!((UnityEngine.Object) component2 == (UnityEngine.Object) null) && string.IsNullOrEmpty(component2.ProxyEncounterGuid) && pickupObject.quality != PickupObject.ItemQuality.EXCLUDED)
          {
            int key = pickupObject.ForcedPositionInAmmonomicon >= 0 ? pickupObject.ForcedPositionInAmmonomicon : 1000000000;
            source.Add(new KeyValuePair<int, PickupObject>(key, pickupObject));
          }
        }
      }
      List<KeyValuePair<int, PickupObject>> list = source.OrderBy<KeyValuePair<int, PickupObject>, int>((Func<KeyValuePair<int, PickupObject>, int>) (e => e.Key)).ToList<KeyValuePair<int, PickupObject>>();
      List<EncounterDatabaseEntry> journalEntries = new List<EncounterDatabaseEntry>();
      dfPanel component3 = component1.transform.GetChild(0).GetComponent<dfPanel>();
      for (int index = 0; index < list.Count; ++index)
      {
        if (list[index].Value.quality != PickupObject.ItemQuality.EXCLUDED)
        {
          EncounterTrackable component4 = list[index].Value.GetComponent<EncounterTrackable>();
          if (!component4.journalData.SuppressInAmmonomicon)
          {
            EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(component4.EncounterGuid);
            if (list[index].Value is ContentTeaserItem || list[index].Value is ContentTeaserGun)
              entry.ForceEncounterState = true;
            if (entry != null)
              journalEntries.Add(entry);
          }
        }
      }
      this.StartCoroutine(this.ConstructRectanglePageLayout(component3, journalEntries, new Vector2(12f, 20f), new Vector2(20f, 20f)));
      component3.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
      component1.Height = component3.Height;
      component3.Height = component1.Height;
    }

    public void InitializeGunsPageLeft()
    {
      dfPanel component1 = this.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel").Find("Guns Panel").GetComponent<dfPanel>();
      List<Gun> source = new List<Gun>();
      bool flag = GameStatsManager.HasInstance && GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_AMMONOMICON_COMPLETE);
      for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
      {
        if (PickupObjectDatabase.Instance.Objects[index] is Gun)
        {
          Gun gun = PickupObjectDatabase.Instance.Objects[index] as Gun;
          EncounterTrackable component2 = gun.GetComponent<EncounterTrackable>();
          if (!((UnityEngine.Object) component2 == (UnityEngine.Object) null) && string.IsNullOrEmpty(component2.ProxyEncounterGuid) && gun.quality != PickupObject.ItemQuality.EXCLUDED && (!flag || gun.PickupObjectId != GlobalItemIds.UnfinishedGun) && (flag || gun.PickupObjectId != GlobalItemIds.FinishedGun))
            source.Add(gun);
        }
      }
      List<Gun> list = source.OrderBy<Gun, int>((Func<Gun, int>) (g => g.ForcedPositionInAmmonomicon < 0 ? 1000000000 : g.ForcedPositionInAmmonomicon)).ToList<Gun>();
      List<EncounterDatabaseEntry> journalEntries = new List<EncounterDatabaseEntry>();
      dfPanel component3 = component1.transform.GetChild(0).GetComponent<dfPanel>();
      for (int index = 0; index < list.Count; ++index)
      {
        EncounterTrackable component4 = list[index].GetComponent<EncounterTrackable>();
        if (!component4.journalData.SuppressInAmmonomicon)
        {
          EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(component4.EncounterGuid);
          if (entry != null)
          {
            if (list[index] is ContentTeaserGun)
              entry.ForceEncounterState = true;
            journalEntries.Add(entry);
          }
        }
      }
      this.StartCoroutine(this.ConstructRectanglePageLayout(component3, journalEntries, new Vector2(12f, 20f), new Vector2(20f, 20f)));
      component3.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
      component1.Height = component3.Height;
      component3.Height = component1.Height;
    }

    public void InitializeEquipmentPageRight()
    {
    }

    public void InitializeEquipmentPageLeft()
    {
      UnityEngine.Debug.Log((object) "INITIALIZING EQUIPMENT PAGE LEFT");
      Transform transform = this.guiManager.transform.Find("Scroll Panel").Find("Scroll Panel");
      this.PrimaryClipPanel = transform.GetComponent<dfControl>();
      dfPanel component1 = transform.Find("Guns Panel").GetComponent<dfPanel>();
      List<EncounterDatabaseEntry> journalEntries = new List<EncounterDatabaseEntry>();
      PlayerController playerController = (PlayerController) null;
      bool flag = false;
      if (GameManager.Instance.IsSelectingCharacter)
      {
        flag = true;
        if ((bool) (UnityEngine.Object) Foyer.Instance.CurrentSelectedCharacterFlag)
          playerController = ((GameObject) BraveResources.Load(Foyer.Instance.CurrentSelectedCharacterFlag.CharacterPrefabPath)).GetComponent<PlayerController>();
      }
      else
      {
        playerController = GameManager.Instance.PrimaryPlayer;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          if (GameManager.Instance.AllPlayers[index].PlayerIDX == GameManager.Instance.LastPausingPlayerID)
            playerController = GameManager.Instance.AllPlayers[index];
        }
      }
      if (!((UnityEngine.Object) playerController != (UnityEngine.Object) null))
        return;
      List<AdvancedSynergyEntry> activeSynergies = new List<AdvancedSynergyEntry>();
      if (flag)
      {
        for (int index = 0; index < playerController.startingGunIds.Count; ++index)
        {
          EncounterTrackable component2 = (PickupObjectDatabase.GetById(playerController.startingGunIds[index]) as Gun).GetComponent<EncounterTrackable>();
          if ((bool) (UnityEngine.Object) component2 && !string.IsNullOrEmpty(component2.EncounterGuid))
            journalEntries.Add(EncounterDatabase.GetEntry(component2.EncounterGuid));
        }
      }
      else
      {
        for (int index = 0; index < playerController.ActiveExtraSynergies.Count; ++index)
          activeSynergies.Add(GameManager.Instance.SynergyManager.synergies[playerController.ActiveExtraSynergies[index]]);
        if (playerController.inventory != null && playerController.inventory.AllGuns != null)
        {
          for (int index = 0; index < playerController.inventory.AllGuns.Count; ++index)
          {
            Gun allGun = playerController.inventory.AllGuns[index];
            if ((bool) (UnityEngine.Object) allGun && !(bool) (UnityEngine.Object) allGun.GetComponent<MimicGunController>())
            {
              EncounterTrackable component3 = allGun.GetComponent<EncounterTrackable>();
              if ((bool) (UnityEngine.Object) component3 && !component3.SuppressInInventory && !string.IsNullOrEmpty(component3.EncounterGuid))
                journalEntries.Add(EncounterDatabase.GetEntry(component3.EncounterGuid));
            }
          }
        }
      }
      dfPanel component4 = component1.transform.GetChild(0).GetComponent<dfPanel>();
      this.StartCoroutine(this.ConstructRectanglePageLayout(component4, journalEntries, new Vector2(12f, 16f), new Vector2(8f, 8f), true, activeSynergies));
      component4.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
      component1.Height = Mathf.Max(100f, component4.Height);
      component4.Height = component1.Height;
      List<PlayerItem> playerItemList = playerController.activeItems;
      if (flag)
      {
        playerItemList = new List<PlayerItem>(playerController.startingActiveItemIds.Count);
        for (int index = 0; index < playerController.startingActiveItemIds.Count; ++index)
          playerItemList.Add(PickupObjectDatabase.GetById(playerController.startingActiveItemIds[index]) as PlayerItem);
      }
      if (playerItemList != null && playerItemList.Count > 0)
      {
        dfPanel component5 = transform.Find("Active Items Panel").GetComponent<dfPanel>();
        journalEntries.Clear();
        for (int index = 0; index < playerItemList.Count; ++index)
        {
          PlayerItem playerItem = playerItemList[index];
          if ((bool) (UnityEngine.Object) playerItem)
          {
            EncounterTrackable component6 = playerItem.GetComponent<EncounterTrackable>();
            if ((bool) (UnityEngine.Object) component6 && !component6.SuppressInInventory)
              journalEntries.Add(EncounterDatabase.GetEntry(component6.EncounterGuid));
          }
        }
        dfPanel component7 = component5.transform.GetChild(0).GetComponent<dfPanel>();
        this.StartCoroutine(this.ConstructRectanglePageLayout(component7, journalEntries, new Vector2(12f, 16f), new Vector2(8f, 8f), true, activeSynergies));
        component7.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
        component5.Height = Mathf.Max(100f, component7.Height);
        component7.Height = component5.Height;
      }
      List<PassiveItem> passiveItemList = playerController.passiveItems;
      if (flag)
      {
        passiveItemList = new List<PassiveItem>(playerController.startingPassiveItemIds.Count);
        for (int index = 0; index < playerController.startingPassiveItemIds.Count; ++index)
          passiveItemList.Add(PickupObjectDatabase.GetById(playerController.startingPassiveItemIds[index]) as PassiveItem);
      }
      if (passiveItemList == null || passiveItemList.Count <= 0)
        return;
      dfPanel component8 = transform.Find("Passive Items Panel").GetComponent<dfPanel>();
      journalEntries.Clear();
      for (int index = 0; index < passiveItemList.Count; ++index)
      {
        PassiveItem passiveItem = passiveItemList[index];
        if ((bool) (UnityEngine.Object) passiveItem)
        {
          EncounterTrackable component9 = passiveItem.GetComponent<EncounterTrackable>();
          if ((bool) (UnityEngine.Object) component9 && !component9.SuppressInInventory)
            journalEntries.Add(EncounterDatabase.GetEntry(component9.EncounterGuid));
        }
      }
      dfPanel component10 = component8.transform.GetChild(0).GetComponent<dfPanel>();
      this.StartCoroutine(this.ConstructRectanglePageLayout(component10, journalEntries, new Vector2(12f, 16f), new Vector2(8f, 8f), true, activeSynergies));
      component10.Anchor = dfAnchorStyle.Top | dfAnchorStyle.Bottom | dfAnchorStyle.CenterHorizontal;
      component8.Height = Mathf.Max(100f, component10.Height);
      component10.Height = component8.Height;
    }

    public T AddSpriteToPage<T>(tk2dSpriteCollectionData collection, int spriteID) where T : tk2dBaseSprite
    {
      GameObject go = new GameObject("ammonomicon tk2d sprite");
      go.transform.parent = this.transform.parent;
      T page = tk2dBaseSprite.AddComponent<T>(go, collection, spriteID);
      Bounds untrimmedBounds = page.GetUntrimmedBounds();
      Vector2 vector2 = GameUIUtility.TK2DtoDF(untrimmedBounds.size.XY(), this.guiManager.PixelsToUnits());
      page.scale = new Vector3(vector2.x / untrimmedBounds.size.x, vector2.y / untrimmedBounds.size.y, untrimmedBounds.size.z);
      page.ignoresTiltworldDepth = true;
      go.transform.localPosition = Vector3.zero;
      go.layer = this.guiManager.gameObject.layer;
      return page;
    }

    public tk2dSprite AddSpriteToPage(tk2dBaseSprite sourceSprite)
    {
      return this.AddSpriteToPage<tk2dSprite>(sourceSprite.Collection, sourceSprite.spriteId);
    }

    public void SetMatrix(Matrix4x4 matrix)
    {
      this.renderMaterial.SetVector(this.topBezierPropID, matrix.GetRow(0));
      this.renderMaterial.SetVector(this.leftBezierPropID, matrix.GetRow(1));
      this.renderMaterial.SetVector(this.bottomBezierPropID, matrix.GetRow(2));
      this.renderMaterial.SetVector(this.rightBezierPropID, matrix.GetRow(3));
    }

    public void EnableRendering()
    {
      if ((UnityEngine.Object) this.m_renderBuffer != (UnityEngine.Object) null)
        this.m_renderBuffer.DiscardContents();
      this.guiManager.gameObject.SetActive(true);
      ++this.m_camera.depth;
      this.m_camera.enabled = true;
      this.targetRenderer.enabled = true;
      this.guiManager.GetComponent<dfInputManager>().enabled = true;
      Color backgroundColor = this.m_camera.backgroundColor;
      CameraClearFlags clearFlags = this.m_camera.clearFlags;
      this.m_camera.clearFlags = CameraClearFlags.Color;
      this.m_camera.backgroundColor = Color.black;
      this.m_camera.Render();
      this.m_camera.clearFlags = clearFlags;
      this.m_camera.backgroundColor = backgroundColor;
    }

    public void Disable(bool isPrecache = false)
    {
      --this.m_camera.depth;
      this.m_camera.enabled = false;
      this.targetRenderer.enabled = false;
      this.guiManager.GetComponent<dfInputManager>().enabled = false;
      if (isPrecache)
        this.StartCoroutine(this.HandleFrameDelayedInactivation());
      else
        this.guiManager.gameObject.SetActive(false);
    }

    [DebuggerHidden]
    private IEnumerator HandleFrameDelayedInactivation()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmmonomiconPageRenderer__HandleFrameDelayedInactivationc__Iterator3()
      {
        _this = this
      };
    }

    public void Dispose()
    {
      if ((bool) (UnityEngine.Object) this.m_camera)
        this.m_camera.RemoveAllCommandBuffers();
      if ((UnityEngine.Object) this.m_renderBuffer != (UnityEngine.Object) null)
      {
        RenderTexture.ReleaseTemporary(this.m_renderBuffer);
        this.m_renderBuffer = (RenderTexture) null;
      }
      if ((bool) (UnityEngine.Object) this.targetRenderer)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.targetRenderer.gameObject);
      if (!(bool) (UnityEngine.Object) this.guiManager)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.guiManager.gameObject);
    }

    private void OnDestroy() => this.Dispose();

    public enum PageType
    {
      NONE,
      EQUIPMENT_LEFT,
      EQUIPMENT_RIGHT,
      GUNS_LEFT,
      GUNS_RIGHT,
      ITEMS_LEFT,
      ITEMS_RIGHT,
      ENEMIES_LEFT,
      ENEMIES_RIGHT,
      BOSSES_LEFT,
      BOSSES_RIGHT,
      DEATH_LEFT,
      DEATH_RIGHT,
    }

    private struct RectangleLineInfo
    {
      public int numberOfElements;
      public float lineHeightUnits;
      public float initialXOffset;
    }
  }

