// Decompiled with JetBrains decompiler
// Type: GameUIBossHealthController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class GameUIBossHealthController : MonoBehaviour
  {
    private const float c_minTimeBetweenHitVfx = 0.25f;
    public dfSlicedSprite tankSprite;
    public List<dfSlicedSprite> barSprites;
    public dfLabel bossNameLabel;
    public float fillTime = 1.5f;
    public GameObject damagedVFX;
    public BossCardUIController bossCardUIPrefab;
    public bool IsVertical;
    [NonSerialized]
    public float Opacity = 1f;
    [NonSerialized]
    public float OpacityChangeSpeed = 2f;
    [NonSerialized]
    public float MinOpacity = 0.6f;
    private Vector3? m_defaultBossNameRelativePosition;
    private dfAtlas EnglishAtlas;
    private dfFontBase EnglishFont;
    private dfAtlas OtherLanguageAtlas;
    private dfFontBase OtherLanguageFont;
    private StringTableManager.GungeonSupportedLanguages m_cachedLanguage;
    private List<bool> m_barsActive = new List<bool>();
    private List<float> m_cachedPercentHealths = new List<float>();
    private List<HealthHaver> m_healthHavers = new List<HealthHaver>();
    private int m_activeBarSprites;
    private bool m_isAnimating;
    private float m_targetPercent;
    private float m_vfxTimer;
    private BossCardUIController m_extantBosscard;

    private void Awake()
    {
      while (this.m_cachedPercentHealths.Count < this.barSprites.Count)
        this.m_cachedPercentHealths.Add(1f);
      while (this.m_barsActive.Count < this.barSprites.Count)
        this.m_barsActive.Add(false);
    }

    private void CheckLanguageFonts()
    {
      if ((UnityEngine.Object) this.EnglishFont == (UnityEngine.Object) null)
      {
        this.EnglishFont = this.bossNameLabel.Font;
        this.EnglishAtlas = this.bossNameLabel.Atlas;
        this.OtherLanguageFont = this.bossNameLabel.GUIManager.DefaultFont;
        this.OtherLanguageAtlas = this.bossNameLabel.GUIManager.DefaultAtlas;
      }
      switch (StringTableManager.CurrentLanguage)
      {
        case StringTableManager.GungeonSupportedLanguages.ENGLISH:
          if (this.m_cachedLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH)
          {
            this.bossNameLabel.Atlas = this.EnglishAtlas;
            this.bossNameLabel.Font = this.EnglishFont;
            goto case StringTableManager.GungeonSupportedLanguages.JAPANESE;
          }
          goto case StringTableManager.GungeonSupportedLanguages.JAPANESE;
        case StringTableManager.GungeonSupportedLanguages.JAPANESE:
        case StringTableManager.GungeonSupportedLanguages.KOREAN:
        case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
        case StringTableManager.GungeonSupportedLanguages.CHINESE:
          this.m_cachedLanguage = StringTableManager.CurrentLanguage;
          break;
        default:
          if (this.m_cachedLanguage != StringTableManager.CurrentLanguage)
          {
            this.bossNameLabel.Atlas = this.OtherLanguageAtlas;
            this.bossNameLabel.Font = this.OtherLanguageFont;
            goto case StringTableManager.GungeonSupportedLanguages.JAPANESE;
          }
          goto case StringTableManager.GungeonSupportedLanguages.JAPANESE;
      }
    }

    public void LateUpdate()
    {
      if (!this.m_defaultBossNameRelativePosition.HasValue)
        this.m_defaultBossNameRelativePosition = new Vector3?(this.bossNameLabel.RelativePosition);
      this.m_vfxTimer -= BraveTime.DeltaTime;
      if (this.m_healthHavers.Count > 0)
      {
        for (int index = 0; index < this.m_healthHavers.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_healthHavers[index])
          {
            this.UpdateBarSizes(index);
            this.UpdateBossHealth(this.barSprites[index], this.m_healthHavers[index].GetCurrentHealth() / this.m_healthHavers[index].GetMaxHealth() / (float) this.barSprites.Count);
          }
        }
      }
      else if (this.m_barsActive[0] && (double) this.m_cachedPercentHealths[0] > 0.0)
        this.UpdateBossHealth(this.barSprites[0], 0.0f);
      if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.KOREAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE)
        this.bossNameLabel.RelativePosition = this.m_defaultBossNameRelativePosition.Value + new Vector3(0.0f, -12f, 0.0f);
      else if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
        this.bossNameLabel.RelativePosition = this.m_defaultBossNameRelativePosition.Value;
      else
        this.bossNameLabel.RelativePosition = this.m_defaultBossNameRelativePosition.Value + new Vector3(0.0f, -12f, 0.0f);
      bool flag = false;
      float num1 = BraveCameraUtility.ASPECT / 1.77777779f;
      foreach (BraveBehaviour allPlayer in GameManager.Instance.AllPlayers)
      {
        Vector2 viewport = BraveUtility.WorldPointToViewport((Vector3) allPlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox), ViewportType.Camera);
        float num2 = (viewport.x - 0.5f) * num1;
        float num3 = (1f - viewport.x) * num1;
        if (GameManager.Options.SmallUIEnabled)
        {
          if (this.IsVertical && (double) num3 < 0.039999999105930328 && (double) num3 > -0.05000000074505806 && (double) viewport.y > 0.31999999284744263 && (double) viewport.y < 0.699999988079071)
          {
            flag = true;
            break;
          }
          if (!this.IsVertical && (double) num2 >= -0.11500000208616257 && (double) num2 <= 0.11500000208616257 && (double) viewport.y > -0.05000000074505806 && (double) viewport.y < 0.059999998658895493)
          {
            flag = true;
            break;
          }
        }
        else
        {
          if (this.IsVertical && (double) num3 < 0.075000002980232239 && (double) num3 > -0.05000000074505806 && (double) viewport.y > 0.14000000059604645 && (double) viewport.y < 0.89999997615814209)
          {
            flag = true;
            break;
          }
          if (!this.IsVertical && (double) num2 >= -0.23000000417232513 && (double) num2 <= 0.23000000417232513 && (double) viewport.y > -0.05000000074505806 && (double) viewport.y < 0.10999999940395355)
          {
            flag = true;
            break;
          }
        }
      }
      this.Opacity = Mathf.MoveTowards(this.Opacity, !flag ? 1f : this.MinOpacity, this.OpacityChangeSpeed * BraveTime.DeltaTime);
      for (int index = 0; index < this.barSprites.Count; ++index)
        this.barSprites[index].Opacity = this.Opacity;
      this.tankSprite.Opacity = this.Opacity;
    }

    private void UpdateBarSizes(int barIndex)
    {
      this.barSprites[barIndex].RelativePosition = this.barSprites[barIndex].RelativePosition.WithX(this.barSprites[0].RelativePosition.x + this.barSprites[0].Size.x / (float) this.m_activeBarSprites * (float) barIndex);
    }

    public void SetBossName(string bossName)
    {
      this.CheckLanguageFonts();
      if ((bool) (UnityEngine.Object) GameManager.Instance.Dungeon)
        this.bossNameLabel.Glitchy = GameManager.Instance.Dungeon.IsGlitchDungeon;
      this.bossNameLabel.Text = bossName;
    }

    public void RegisterBossHealthHaver(HealthHaver healthHaver, string bossName = null)
    {
      if (bossName != null)
        this.SetBossName(bossName);
      if (this.m_healthHavers.Contains(healthHaver))
        return;
      this.m_healthHavers.Add(healthHaver);
      if (this.m_healthHavers.Count > this.barSprites.Count)
      {
        dfSlicedSprite component = this.barSprites[0].Parent.AddPrefab(this.barSprites[0].gameObject).GetComponent<dfSlicedSprite>();
        component.RelativePosition = this.barSprites[0].RelativePosition;
        this.barSprites.Add(component);
        this.m_cachedPercentHealths.Add(1f);
        this.m_barsActive.Add(false);
      }
      else
      {
        int index = this.m_healthHavers.Count - 1;
        this.m_cachedPercentHealths[index] = 1f;
        this.m_barsActive[index] = false;
      }
    }

    public void DeregisterBossHealthHaver(HealthHaver healthHaver)
    {
      int index1 = this.m_healthHavers.IndexOf(healthHaver);
      if (index1 >= 0)
      {
        this.UpdateBossHealth(this.barSprites[index1], 0.0f);
        this.m_healthHavers[index1] = (HealthHaver) null;
      }
      for (int index2 = 0; index2 < this.m_healthHavers.Count; ++index2)
      {
        if ((bool) (UnityEngine.Object) this.m_healthHavers[index2] && this.m_healthHavers[index2].IsAlive)
          return;
      }
      this.ClearExtraBarData();
      this.m_healthHavers.Clear();
    }

    private void ClearExtraBarData()
    {
      for (int index = 1; index < this.barSprites.Count; index = index - 1 + 1)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.barSprites[index].gameObject);
        this.barSprites.RemoveAt(index);
        if (index < this.m_cachedPercentHealths.Count)
          this.m_cachedPercentHealths.RemoveAt(index);
        if (index < this.m_barsActive.Count)
          this.m_barsActive.RemoveAt(index);
      }
    }

    public void ForceUpdateBossHealth(float currentBossHealth, float maxBossHealth, string bossName = null)
    {
      if (bossName != null)
        this.SetBossName(bossName);
      this.UpdateBossHealth(this.barSprites[0], currentBossHealth / maxBossHealth);
    }

    public void DisableBossHealth()
    {
      for (int index = this.barSprites.Count - 1; index >= 0; --index)
      {
        this.m_barsActive[index] = false;
        this.m_cachedPercentHealths[index] = 0.0f;
      }
      this.ClearExtraBarData();
      this.m_activeBarSprites = 0;
      this.m_healthHavers.Clear();
      this.tankSprite.IsVisible = false;
    }

    [DebuggerHidden]
    public IEnumerator TriggerBossPortraitCR(PortraitSlideSettings pss)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameUIBossHealthController__TriggerBossPortraitCRc__Iterator0()
      {
        pss = pss,
        _this = this
      };
    }

    public void EndBossPortraitEarly()
    {
      if (!(bool) (UnityEngine.Object) this.m_extantBosscard)
        return;
      this.m_extantBosscard.BreakSequence();
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantBosscard.gameObject);
    }

    private void UpdateBossHealth(dfSlicedSprite barSprite, float percent)
    {
      int index = this.barSprites.IndexOf(barSprite);
      if ((double) percent <= 0.0)
      {
        if (!this.m_barsActive[index])
        {
          UnityEngine.Debug.LogError((object) "uh... activating a boss health bar at 0 health. this seems dumb");
          return;
        }
        this.m_targetPercent = 0.0f;
        if (!this.m_isAnimating)
          barSprite.FillAmount = percent;
      }
      else if (!this.m_barsActive[index])
      {
        this.TriggerBossHealth(barSprite, percent);
      }
      else
      {
        if ((double) percent > (double) this.m_cachedPercentHealths[this.barSprites.IndexOf(barSprite)])
          this.StartCoroutine(this.FillBossBar(barSprite));
        this.m_targetPercent = percent;
        if (!this.m_isAnimating)
          barSprite.FillAmount = percent;
      }
      if ((double) this.m_cachedPercentHealths[this.barSprites.IndexOf(barSprite)] > (double) percent && (UnityEngine.Object) this.damagedVFX != (UnityEngine.Object) null && (double) this.m_vfxTimer <= 0.0)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.damagedVFX);
        dfSprite component1 = gameObject.GetComponent<dfSprite>();
        dfSpriteAnimation component2 = gameObject.GetComponent<dfSpriteAnimation>();
        component1.BringToFront();
        component1.Size = component1.SpriteInfo.sizeInPixels * Pixelator.Instance.CurrentTileScale;
        barSprite.GetManager().AddControl((dfControl) component1);
        Bounds bounds = barSprite.GetBounds();
        if (this.IsVertical)
        {
          float y = (bounds.max.y - bounds.min.y) * barSprite.FillAmount + bounds.min.y;
          component1.transform.position = new Vector3(bounds.center.x, y, bounds.center.z);
        }
        else
        {
          float x = (bounds.max.x - bounds.min.x) * barSprite.FillAmount + bounds.min.x;
          component1.transform.position = new Vector3(x, bounds.center.y, bounds.center.z);
        }
        component1.BringToFront();
        component1.Opacity = this.Opacity;
        component2.Play();
        this.m_vfxTimer = 0.25f;
      }
      this.m_cachedPercentHealths[this.barSprites.IndexOf(barSprite)] = percent;
    }

    private void TriggerBossHealth(dfSlicedSprite barSprite, float targetPercent)
    {
      int index = this.barSprites.IndexOf(barSprite);
      if (this.m_barsActive[index])
        return;
      this.m_barsActive[index] = true;
      ++this.m_activeBarSprites;
      for (int barIndex = 0; barIndex < this.m_healthHavers.Count; ++barIndex)
        this.UpdateBarSizes(barIndex);
      barSprite.FillAmount = 0.0f;
      this.tankSprite.IsVisible = true;
      this.tankSprite.Invalidate();
      barSprite.Invalidate();
      this.m_targetPercent = targetPercent;
      this.StartCoroutine(this.FillBossBar(barSprite));
    }

    [DebuggerHidden]
    private IEnumerator FillBossBar(dfSlicedSprite barSprite)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GameUIBossHealthController__FillBossBarc__Iterator1()
      {
        barSprite = barSprite,
        _this = this
      };
    }

    public bool IsActive => this.m_barsActive[0];
  }

