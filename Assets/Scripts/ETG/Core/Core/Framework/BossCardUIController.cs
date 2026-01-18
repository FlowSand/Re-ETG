// Decompiled with JetBrains decompiler
// Type: BossCardUIController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BossCardUIController : TimeInvariantMonoBehaviour
  {
    [Header("Dave Stuff")]
    public float FLASH_DURATION = 0.1f;
    public float FLASHBAR_CROSS_DURATION = 0.2f;
    public float FLASHBAR_WAIT_DURATION = 0.1f;
    public float FLASHBAR_EXPAND_DURATION = 0.2f;
    public float TEXT_IN_DURATION = 0.5f;
    public float CHARACTER_INITIAL_MOVE_DURATION = 0.5f;
    public float CHARACTER_SLIDE_DURATION = 2f;
    public float CHARACTER_SLIDE_SPEED = 0.05f;
    public float BOSS_SLIDE_SPEED = 0.05f;
    public float PARALLAX_QUANTIZATION_STEP = 0.1f;
    [Header("Not for Daves")]
    public Camera uiCamera;
    public dfTextureSprite topTriangle;
    public dfTextureSprite bottomTriangle;
    public dfTextureSprite womboBar;
    public dfTextureSprite womboBG;
    public dfTextureSprite bossSprite;
    public Transform bossStart;
    public Transform bossTarget;
    public dfTextureSprite playerSprite;
    public Transform playerStart;
    public Transform playerTarget;
    public dfTextureSprite coopSprite;
    [Header("Parallax Bros")]
    public List<dfTextureSprite> parallaxSprites;
    public List<float> parallaxSpeeds;
    public List<Transform> parallaxStarts;
    public List<Transform> parallaxEnds;
    [Header("Light Streaks")]
    public dfSprite lightStreaksSprite;
    public List<string> lightStreakSpriteNames;
    [Header("Texts")]
    public List<Transform> floatingTexts;
    public List<Transform> floatingTextStarts;
    public List<Transform> floatingTextTargets;
    public dfLabel nameLabel;
    public dfLabel subtitleLabel;
    public dfLabel quoteLabel;
    private string m_charSpriteName;
    private Texture m_bossSprite;
    private Pixelator_Simple m_pix;
    private IntVector2 m_bossSpritePxOffset;
    private IntVector2 m_topLeftTextPxOffset;
    private IntVector2 m_bottomRightTextPxOffset;
    private bool m_isPlaying;
    private float m_cachedNameLabelTextScale = -1f;
    private bool m_doLightStreaks;

    private void Initialize()
    {
      this.m_pix = this.uiCamera.GetComponent<Pixelator_Simple>();
      this.m_pix.Initialize();
      this.ToggleCoreVisiblity(false);
      this.ResetTextsToStart();
    }

    private void InitializeTextsShared()
    {
      if (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.KOREAN && GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.CHINESE && GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.JAPANESE && GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.RUSSIAN)
        return;
      this.nameLabel.PerCharacterOffset = Vector3.zero;
      this.subtitleLabel.PerCharacterOffset = Vector3.zero;
      this.quoteLabel.PerCharacterOffset = Vector3.zero;
      this.nameLabel.transform.rotation = Quaternion.identity;
      this.subtitleLabel.transform.rotation = Quaternion.identity;
      this.quoteLabel.transform.rotation = Quaternion.identity;
      dfLabel nameLabel = this.nameLabel;
      bool flag1 = false;
      this.nameLabel.Outline = flag1;
      int num1 = flag1 ? 1 : 0;
      nameLabel.Shadow = num1 != 0;
      dfLabel subtitleLabel = this.subtitleLabel;
      bool flag2 = false;
      this.subtitleLabel.Outline = flag2;
      int num2 = flag2 ? 1 : 0;
      subtitleLabel.Shadow = num2 != 0;
      dfLabel quoteLabel = this.quoteLabel;
      bool flag3 = false;
      this.quoteLabel.Outline = flag3;
      int num3 = flag3 ? 1 : 0;
      quoteLabel.Shadow = num3 != 0;
    }

    public void InitializeTexts(PortraitSlideSettings pss)
    {
      this.m_topLeftTextPxOffset = pss.topLeftTextPxOffset;
      this.m_bottomRightTextPxOffset = pss.bottomRightTextPxOffset;
      if ((bool) (Object) GameManager.Instance.Dungeon)
      {
        this.nameLabel.Glitchy = GameManager.Instance.Dungeon.IsGlitchDungeon;
        this.subtitleLabel.Glitchy = GameManager.Instance.Dungeon.IsGlitchDungeon;
      }
      if ((double) this.m_cachedNameLabelTextScale == -1.0)
        this.m_cachedNameLabelTextScale = this.nameLabel.TextScale;
      this.nameLabel.Text = StringTableManager.GetEnemiesString(pss.bossNameString);
      float autosizeWidth = this.nameLabel.GetAutosizeWidth();
      if ((double) autosizeWidth > 800.0)
      {
        this.nameLabel.PerCharacterOffset = new Vector3(0.0f, 2f, 0.0f);
        this.nameLabel.TextScale = 1000f / autosizeWidth * this.m_cachedNameLabelTextScale;
        this.m_topLeftTextPxOffset += new IntVector2(0, -6);
        this.m_topLeftTextPxOffset += new IntVector2(0, -6);
      }
      else
      {
        this.nameLabel.PerCharacterOffset = new Vector3(0.0f, 3f, 0.0f);
        this.nameLabel.TextScale = this.m_cachedNameLabelTextScale;
      }
      this.InitializeTextsShared();
      this.subtitleLabel.Text = StringTableManager.GetEnemiesString(pss.bossSubtitleString);
      this.quoteLabel.Text = StringTableManager.GetEnemiesString(pss.bossQuoteString);
      this.m_bossSprite = pss.bossArtSprite;
      this.m_bossSpritePxOffset = pss.bossSpritePxOffset;
    }

    [DebuggerHidden]
    private IEnumerator InvariantWaitForSeconds(float seconds)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__InvariantWaitForSecondsc__Iterator0()
      {
        seconds = seconds,
        _this = this
      };
    }

    public void TriggerSequence()
    {
      for (int index = 0; index < this.parallaxSprites.Count; ++index)
      {
        if (!(bool) (Object) this.parallaxSprites[index])
        {
          this.parallaxSprites.RemoveAt(index);
          this.parallaxEnds.RemoveAt(index);
          this.parallaxStarts.RemoveAt(index);
          this.parallaxSpeeds.RemoveAt(index);
          --index;
        }
      }
      this.StartCoroutine(this.CoreSequence((PortraitSlideSettings) null));
    }

    private void ToggleCoreVisiblity(bool visible)
    {
      for (int index = 0; index < this.parallaxSprites.Count; ++index)
      {
        if (!(bool) (Object) this.parallaxSprites[index])
        {
          this.parallaxSprites.RemoveAt(index);
          this.parallaxEnds.RemoveAt(index);
          this.parallaxStarts.RemoveAt(index);
          this.parallaxSpeeds.RemoveAt(index);
          --index;
        }
      }
      if (!(bool) (Object) this.m_bossSprite)
      {
        this.bossSprite.IsVisible = false;
        this.playerSprite.IsVisible = false;
        this.coopSprite.IsVisible = false;
      }
      else
      {
        this.bossSprite.IsVisible = visible;
        this.bossSprite.Texture = this.m_bossSprite;
        this.playerSprite.IsVisible = !GameManager.Instance.PrimaryPlayer.healthHaver.IsDead;
        this.coopSprite.IsVisible = GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !GameManager.Instance.SecondaryPlayer.healthHaver.IsDead;
        if (this.coopSprite.IsVisible)
          this.coopSprite.ZOrder = this.bossSprite.ZOrder - 1;
        PlayerController primaryPlayer = GameManager.Instance.PrimaryPlayer;
        this.playerSprite.Texture = (Texture) primaryPlayer.BosscardSprites[0];
        if (primaryPlayer.characterIdentity == PlayableCharacters.Eevee)
        {
          Material material1 = new Material(Shader.Find("Brave/Internal/GlitchEevee"));
          material1.name = "Default Texture Shader";
          material1.hideFlags = HideFlags.DontSave;
          material1.mainTexture = (Texture) primaryPlayer.BosscardSprites[0];
          Material material2 = material1;
          material2.SetTexture("_EeveeTex", (Texture) primaryPlayer.GetComponent<CharacterAnimationRandomizer>().CosmicTex);
          material2.SetFloat("_WaveIntensity", 0.1f);
          material2.SetFloat("_ColorIntensity", 0.015f);
          this.playerSprite.OverrideMaterial = material2;
        }
        else if ((Object) this.playerSprite.OverrideMaterial != (Object) null)
          this.playerSprite.OverrideMaterial = (Material) null;
        BraveDFTextureAnimator component = this.playerSprite.GetComponent<BraveDFTextureAnimator>();
        component.timeless = true;
        component.textures = GameManager.Instance.PrimaryPlayer.BosscardSprites.ToArray();
        component.fps = GameManager.Instance.PrimaryPlayer.BosscardSpriteFPS;
        this.RecalculateScales();
      }
      this.topTriangle.IsVisible = visible;
      this.bottomTriangle.IsVisible = visible;
      if (visible)
        return;
      this.womboBG.IsVisible = false;
      this.womboBar.IsVisible = false;
      this.lightStreaksSprite.IsVisible = false;
      for (int index = 0; index < this.parallaxSprites.Count; ++index)
        this.parallaxSprites[index].IsVisible = false;
    }

    private void RecalculateScales()
    {
      dfGUIManager manager = this.coopSprite.GetManager();
      Vector2 screenSize = manager.GetScreenSize();
      this.playerSprite.Size = manager.GetScreenSize();
      if ((Object) this.coopSprite != (Object) null)
      {
        float num = 1.7791667f;
        this.coopSprite.Size = new Vector2(screenSize.y * num, screenSize.y);
      }
      float num1 = (float) this.bossSprite.Texture.width / (float) this.bossSprite.Texture.height;
      this.bossSprite.Size = new Vector2(screenSize.y * num1, screenSize.y);
    }

    private Vector3 GetCoopOffset()
    {
      Vector2 a = this.playerTarget.localPosition.XY();
      if (!this.playerSprite.IsVisible)
        return Vector3.zero;
      PerCharacterCoopPositionData coopBosscardOffset = GameManager.Instance.PrimaryPlayer.CoopBosscardOffset;
      if (coopBosscardOffset.flipCoopCultist && (double) this.coopSprite.transform.localScale.x != -1.0)
        this.coopSprite.transform.localScale = new Vector3(-1f, 1f, 1f);
      Vector2 b = new Vector2(coopBosscardOffset.percentOffset.x * -1f, coopBosscardOffset.percentOffset.y);
      return Vector2.Scale(a, b).ToVector3ZUp();
    }

    public void ToggleBoxing(bool enable)
    {
      if (!enable)
      {
        Pixelator.Instance.LerpToLetterbox(1f, 0.0f);
        Pixelator.Instance.SetWindowbox(1f);
      }
      else
      {
        float num = 1.77777779f;
        float aspect = BraveCameraUtility.ASPECT;
        if ((double) num < (double) aspect)
        {
          Pixelator.Instance.SetWindowbox((float) ((double) num / (double) aspect * 0.5));
        }
        else
        {
          if ((double) num <= (double) aspect)
            return;
          Pixelator.Instance.LerpToLetterbox((float) ((double) num / (double) aspect * 0.5), 0.0f);
        }
      }
    }

    protected override void Update()
    {
      base.Update();
      if (!(bool) (Object) this.playerSprite || !((Object) this.playerSprite.OverrideMaterial != (Object) null))
        return;
      this.playerSprite.OverrideMaterial.SetFloat("_AdditionalTime", this.playerSprite.OverrideMaterial.GetFloat("_AdditionalTime") + GameManager.INVARIANT_DELTA_TIME / 4f);
    }

    [DebuggerHidden]
    public IEnumerator CoreSequence(PortraitSlideSettings pss)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__CoreSequencec__Iterator1()
      {
        pss = pss,
        _this = this
      };
    }

    public void BreakSequence()
    {
      GameUIRoot.Instance.ToggleUICamera(true);
      GameUIRoot.Instance.ShowCoreUI(string.Empty);
      this.ToggleBoxing(false);
      this.m_isPlaying = false;
    }

    [DebuggerHidden]
    private IEnumerator HandleLightStreaks()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__HandleLightStreaksc__Iterator2()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator WomboCombo(PortraitSlideSettings pss)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__WomboComboc__Iterator3()
      {
        pss = pss,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleDelayedCoopCharacterSlide()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__HandleDelayedCoopCharacterSlidec__Iterator4()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator HandleCharacterSlides()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__HandleCharacterSlidesc__Iterator5()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator LerpTextsToTargets()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__LerpTextsToTargetsc__Iterator6()
      {
        _this = this
      };
    }

    private void ResetTextsToStart()
    {
      for (int index = 0; index < this.floatingTexts.Count; ++index)
        this.floatingTexts[index].position = this.floatingTextStarts[index].position;
      this.playerSprite.transform.position = this.playerStart.position;
      if (this.coopSprite.IsVisible)
        this.coopSprite.transform.position = this.playerSprite.transform.position + this.GetCoopOffset();
      this.bossSprite.transform.position = this.bossStart.position;
    }

    [DebuggerHidden]
    private IEnumerator FlashColorToColor(
      Color startColor,
      Color targetColor,
      float fadeDuration,
      Material targetMaterial)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__FlashColorToColorc__Iterator7()
      {
        fadeDuration = fadeDuration,
        startColor = startColor,
        targetColor = targetColor,
        targetMaterial = targetMaterial,
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator FlashWhiteToBlack(Material targetMaterial, bool backToClear)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossCardUIController__FlashWhiteToBlackc__Iterator8()
      {
        targetMaterial = targetMaterial,
        backToClear = backToClear,
        _this = this
      };
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

