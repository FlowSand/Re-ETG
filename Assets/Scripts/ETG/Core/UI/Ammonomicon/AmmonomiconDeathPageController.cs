using InControl;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class AmmonomiconDeathPageController : MonoBehaviour, ILevelLoadedListener
  {
    public static bool LastKilledPlayerPrimary = true;
    public bool isRightPage;
    public bool isVictoryPage;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel youDiedLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel gungeoneerTitle;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel areaTitle;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel timeTitle;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel moneyTitle;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel killsTitle;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel gungeoneerLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel areaLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel timeLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel moneyLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel killsLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel deathNumberLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel coopDeathNumberLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel coopIndividualDeathsNumberLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfLabel hauntingLabel;
    [HideInInspectorIf("isRightPage", false)]
    public dfButton quickRestartButton;
    [HideInInspectorIf("isRightPage", false)]
    public dfButton mainMenuButton;
    [ShowInInspectorIf("isRightPage", false)]
    public dfLabel killedByHeaderLabel;
    [ShowInInspectorIf("isRightPage", false)]
    public dfLabel killedByLabel;
    [ShowInInspectorIf("isRightPage", false)]
    public dfTextureSprite photoSprite;
    [ShowInInspectorIf("isRightPage", false)]
    public dfSprite ChallengeModeRibbon;
    [ShowInInspectorIf("isRightPage", false)]
    public dfSprite RatDeathDrawings;
    private Vector4 m_cachedUVRescale;
    private RenderTexture m_temporaryPhotoTex;
    private bool m_doingSomething;

    public void DoInitialize()
    {
      this.m_doingSomething = false;
      if (this.isRightPage)
        this.InitializeRightPage();
      else
        this.InitializeLeftPage();
    }

    private string GetDeathPortraitName()
    {
      switch (GameManager.Instance.PrimaryPlayer.characterIdentity)
      {
        case PlayableCharacters.Pilot:
          return "coop_page_death_rogue_001";
        case PlayableCharacters.Convict:
          return "coop_page_death_jailbird_001";
        case PlayableCharacters.Robot:
          return "coop_page_death_robot_001";
        case PlayableCharacters.Ninja:
          return "coop_page_death_ninja_001";
        case PlayableCharacters.Cosmonaut:
          return "coop_page_death_cultist_001";
        case PlayableCharacters.Soldier:
          return "coop_page_death_marine_001";
        case PlayableCharacters.Guide:
          return "coop_page_death_scholar_001";
        case PlayableCharacters.Bullet:
          return "coop_page_death_bullet_001";
        case PlayableCharacters.Eevee:
          return "coop_page_death_eevee_001";
        case PlayableCharacters.Gunslinger:
          return "coop_page_death_slinger_001";
        default:
          return "coop_page_death_cultist_001";
      }
    }

    private string GetStringKeyForCharacter(PlayableCharacters identity)
    {
      switch (identity)
      {
        case PlayableCharacters.Pilot:
          return "#CHAR_ROGUE_SHORT";
        case PlayableCharacters.Convict:
          return "#CHAR_CONVICT_SHORT";
        case PlayableCharacters.Robot:
          return "#CHAR_ROBOT_SHORT";
        case PlayableCharacters.Soldier:
          return "#CHAR_MARINE_SHORT";
        case PlayableCharacters.Guide:
          return "#CHAR_GUIDE_SHORT";
        case PlayableCharacters.CoopCultist:
          return "#CHAR_CULTIST_SHORT";
        case PlayableCharacters.Bullet:
          return "#CHAR_BULLET_SHORT";
        case PlayableCharacters.Eevee:
          return "#CHAR_PARADOX_SHORT";
        case PlayableCharacters.Gunslinger:
          return "#CHAR_GUNSLINGER_SHORT";
        default:
          return "#CHAR_CULTIST_SHORT";
      }
    }

    public void UpdateWidth(dfLabel target, int min = -1)
    {
      dfSlicedSprite componentInChildren = target.Parent.GetComponentInChildren<dfSlicedSprite>();
      if (!(bool) (UnityEngine.Object) componentInChildren)
        return;
      componentInChildren.Width = (float) (Mathf.CeilToInt(target.Width / 4f) + 5);
      if (min <= 0)
        return;
      componentInChildren.Width = Mathf.Max((float) min, componentInChildren.Width);
    }

    public void ToggleBG(dfLabel target)
    {
      if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
      {
        target.BackgroundSprite = string.Empty;
        target.Padding = new RectOffset(0, 0, 0, 0);
      }
      else
      {
        target.BackgroundSprite = "chamber_flash_small_001";
        target.Padding = new RectOffset(6, 6, 0, 0);
        target.BackgroundColor = (Color32) new Color(0.1764706f, 0.196078435f, 0.305882365f);
      }
    }

    private void InitializeLeftPage()
    {
      if (this.isVictoryPage)
      {
        this.youDiedLabel.Text = this.youDiedLabel.ForceGetLocalizedValue("#DEATH_YOUWON");
        foreach (dfLabel componentsInChild in this.youDiedLabel.GetComponentsInChildren<dfLabel>())
          componentsInChild.Text = this.youDiedLabel.ForceGetLocalizedValue("#DEATH_YOUWON");
      }
      else
      {
        this.youDiedLabel.Text = this.youDiedLabel.ForceGetLocalizedValue("#DEATH_YOUDIED");
        foreach (dfLabel componentsInChild in this.youDiedLabel.GetComponentsInChildren<dfLabel>())
          componentsInChild.Text = this.youDiedLabel.ForceGetLocalizedValue("#DEATH_YOUDIED");
      }
      string localizedValue1 = this.hauntingLabel.ForceGetLocalizedValue("#DEATH_PASTKILLED");
      string localizedValue2 = this.hauntingLabel.ForceGetLocalizedValue("#DEATH_PASTHAUNTS");
      string localizedValue3 = this.hauntingLabel.ForceGetLocalizedValue("#DEATH_HELLCLEARED");
      this.hauntingLabel.Text = !GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_LICH) ? (!GameStatsManager.Instance.GetCharacterSpecificFlag(GameManager.Instance.PrimaryPlayer.characterIdentity, CharacterSpecificGungeonFlags.KILLED_PAST) ? localizedValue2 : localizedValue1) : localizedValue3;
      this.UpdateWidth(this.hauntingLabel, 142);
      this.hauntingLabel.PerformLayout();
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        this.deathNumberLabel.Parent.Parent.IsVisible = false;
        this.coopDeathNumberLabel.Parent.Parent.IsVisible = true;
        this.coopDeathNumberLabel.Text = GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_DEATHS).ToString();
        this.coopIndividualDeathsNumberLabel.Text = $"[sprite \"{this.GetDeathPortraitName()}\"] x{GameManager.Instance.PrimaryPlayer.DeathsThisRun.ToString()} [sprite \"coop_page_death_cultist_001\"] x{GameManager.Instance.SecondaryPlayer.DeathsThisRun.ToString()}";
      }
      else
      {
        this.deathNumberLabel.Parent.Parent.IsVisible = true;
        this.coopDeathNumberLabel.Parent.Parent.IsVisible = false;
        this.deathNumberLabel.Text = GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.NUMBER_DEATHS).ToString();
      }
      this.UpdateWidth(this.gungeoneerTitle);
      this.UpdateWidth(this.killsTitle);
      this.UpdateWidth(this.areaTitle);
      this.UpdateWidth(this.timeTitle);
      this.UpdateWidth(this.moneyTitle);
      this.gungeoneerLabel.Text = this.gungeoneerLabel.ForceGetLocalizedValue(this.GetStringKeyForCharacter(GameManager.Instance.PrimaryPlayer.characterIdentity));
      this.areaLabel.Text = this.areaLabel.ForceGetLocalizedValue(GameManager.Instance.Dungeon.DungeonShortName);
      int seconds = Mathf.FloorToInt(GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED));
      if (GameManager.Options.SpeedrunMode)
      {
        int milliseconds = Mathf.FloorToInt((float) (1000.0 * ((double) GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TIME_PLAYED) % 1.0)));
        TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds, milliseconds);
        this.timeLabel.Text = $"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds:000}";
      }
      else
      {
        TimeSpan timeSpan = new TimeSpan(0, 0, seconds);
        this.timeLabel.Text = $"{timeSpan.Hours:0}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
      }
      this.moneyLabel.Text = GameStatsManager.Instance.GetSessionStatValue(TrackedStats.TOTAL_MONEY_COLLECTED).ToString();
      this.killsLabel.Text = string.Empty;
      this.killsLabel.Parent.Width = 30f;
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        this.killsLabel.ProcessMarkup = true;
        this.killsLabel.ColorizeSymbols = true;
        this.killsLabel.TabSize = GameManager.Instance.PrimaryPlayer.KillsThisRun <= 99 || GameManager.Instance.SecondaryPlayer.KillsThisRun <= 99 ? 4 : 2;
        this.killsLabel.Text = $"[sprite \"{this.GetDeathPortraitName()}\"]\t{GameManager.Instance.PrimaryPlayer.KillsThisRun.ToString()}\t[sprite \"coop_page_death_cultist_001\"]\t{GameManager.Instance.SecondaryPlayer.KillsThisRun.ToString()}";
      }
      else
        this.killsLabel.Text = GameStatsManager.Instance.GetSessionStatValue(TrackedStats.ENEMIES_KILLED).ToString();
      this.UpdateTapeLabel(this.gungeoneerLabel);
      this.UpdateTapeLabel(this.areaLabel);
      this.UpdateTapeLabel(this.timeLabel);
      this.UpdateTapeLabel(this.moneyLabel);
      this.killsLabel.PerformLayout();
      this.UpdateTapeLabel(this.killsLabel, this.killsLabel.GetAutosizeWidth());
      string str1 = this.quickRestartButton.ForceGetLocalizedValue("#DEATH_QUICKRESTART");
      string str2 = this.mainMenuButton.ForceGetLocalizedValue("#DEATH_MAINMENU");
      if (!str1.StartsWith(" "))
        str1 = " " + str1;
      if (!str2.StartsWith(" "))
        str2 = " " + str2;
      string str3;
      string str4;
      if (BraveInput.PrimaryPlayerInstance.IsKeyboardAndMouse())
      {
        str3 = "[sprite \"space_bar_up_001\"" + str1;
        str4 = "[sprite \"esc_up_001\"" + str2;
      }
      else
      {
        str3 = UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action1, BraveInput.PlayerOneCurrentSymbology) + str1;
        str4 = UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action2, BraveInput.PlayerOneCurrentSymbology) + str2;
      }
      this.quickRestartButton.Text = str3;
      this.mainMenuButton.Text = str4;
      this.quickRestartButton.Click += new MouseEventHandler(this.DoQuickRestart);
      this.mainMenuButton.Click += new MouseEventHandler(this.DoMainMenu);
      this.quickRestartButton.Focus(true);
      if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
      {
        this.quickRestartButton.IsVisible = false;
        this.mainMenuButton.Focus(true);
      }
      else
      {
        this.quickRestartButton.IsVisible = true;
        this.quickRestartButton.Focus(true);
      }
    }

    private void OnDestroy()
    {
      if (!(bool) (UnityEngine.Object) this.m_temporaryPhotoTex)
        return;
      RenderTexture.ReleaseTemporary(this.m_temporaryPhotoTex);
      this.m_temporaryPhotoTex = (RenderTexture) null;
    }

    private void InitializeRightPage()
    {
      if ((bool) (UnityEngine.Object) this.ChallengeModeRibbon)
        this.ChallengeModeRibbon.IsVisible = false;
      if (this.isVictoryPage)
      {
        if ((bool) (UnityEngine.Object) this.ChallengeModeRibbon && ChallengeManager.CHALLENGE_MODE_ACTIVE)
        {
          this.ChallengeModeRibbon.IsVisible = true;
          dfSprite challengeModeRibbon = this.ChallengeModeRibbon;
          challengeModeRibbon.RelativePosition = challengeModeRibbon.RelativePosition + new Vector3(-200f, -68f, 0.0f);
        }
        this.killedByLabel.Glitchy = false;
        this.killedByLabel.Text = this.killedByLabel.ForceGetLocalizedValue("#DEATH_NOBODY");
        this.UpdateTapeLabel(this.killedByLabel);
        this.UpdateWidth(this.killedByHeaderLabel);
        this.SetWinPic();
      }
      else
      {
        string str = !AmmonomiconDeathPageController.LastKilledPlayerPrimary ? GameManager.Instance.SecondaryPlayer.healthHaver.lastIncurredDamageSource : GameManager.Instance.PrimaryPlayer.healthHaver.lastIncurredDamageSource;
        if (string.IsNullOrEmpty(str))
          str = StringTableManager.GetEnemiesString("#KILLEDBYDEFAULT");
        this.killedByLabel.Text = string.Empty;
        this.killedByLabel.Parent.Width = 30f;
        this.killedByLabel.Glitchy = false;
        if (GameManager.Instance.Dungeon.IsGlitchDungeon)
          this.killedByLabel.Glitchy = true;
        if (str == "primaryplayer" || str == "secondaryplayer")
          str = StringTableManager.GetEnemiesString("#KILLEDBYDEFAULT");
        if ((bool) (UnityEngine.Object) this.RatDeathDrawings)
          this.RatDeathDrawings.IsVisible = false;
        if (str == StringTableManager.GetEnemiesString("#RESOURCEFULRAT_ENCNAME") || str == StringTableManager.GetEnemiesString("#METALGEARRAT_ENCNAME"))
        {
          if ((bool) (UnityEngine.Object) this.RatDeathDrawings)
            this.RatDeathDrawings.IsVisible = true;
          str = StringTableManager.GetEnemiesString("#KILLEDBY_RESOURCEFULRAT");
        }
        this.killedByLabel.Text = str;
        this.killedByLabel.PerformLayout();
        this.UpdateTapeLabel(this.killedByLabel, this.killedByLabel.GetAutosizeWidth());
        this.UpdateWidth(this.killedByHeaderLabel);
        if (!((UnityEngine.Object) this.photoSprite != (UnityEngine.Object) null))
          return;
        float num1 = this.photoSprite.Width / this.photoSprite.Height;
        float num2 = 1.77777779f;
        if (this.isVictoryPage)
        {
          this.photoSprite.Texture = BraveResources.Load("Win_Pic_Gun_Get_001") as Texture;
        }
        else
        {
          RenderTexture source = Pixelator.Instance.GetCachedFrame();
          if (!Mathf.Approximately(1.77777779f, BraveCameraUtility.ASPECT))
          {
            int height = source.height;
            int width = Mathf.RoundToInt((float) height * 1.77777779f);
            RenderTexture temporary = RenderTexture.GetTemporary(new RenderTextureDescriptor(width, height, source.format, source.depth));
            temporary.filterMode = FilterMode.Point;
            float num3 = (float) source.width / ((float) width * 1f);
            float x = (float) (source.width - width) / 2f / (float) source.width;
            Graphics.Blit((Texture) source, temporary, new Vector2(1f / num3, 1f), new Vector2(x, 0.0f));
            this.m_temporaryPhotoTex = temporary;
            source = temporary;
          }
          source.filterMode = FilterMode.Point;
          this.photoSprite.Texture = (Texture) source;
          this.m_cachedUVRescale = new Vector4(0.0f, 0.0f, 1f, 1f);
          Vector3 playerViewportPoint = Pixelator.Instance.CachedPlayerViewportPoint;
          if ((double) playerViewportPoint.x > 0.0 && (double) playerViewportPoint.x < 1.0 && (double) playerViewportPoint.y > 0.0 && (double) playerViewportPoint.y < 1.0)
          {
            float num4 = num1 / num2;
            float num5 = (float) Mathf.RoundToInt(this.photoSprite.Height / 4f) / 270f;
            float num6 = num5;
            float num7 = num5;
            if ((double) num1 > (double) num2)
              num7 = num6 / num4;
            else if ((double) num1 < (double) num2)
              num6 = num7 * num4;
            Vector2 vector2_1 = new Vector2(playerViewportPoint.x - num6 / 2f, playerViewportPoint.y - num7 / 2f);
            Vector2 vector2_2 = new Vector2(playerViewportPoint.x + num6 / 2f, playerViewportPoint.y + num7 / 2f);
            if ((double) vector2_1.x < 0.0)
            {
              vector2_2.x += -1f * vector2_1.x;
              vector2_1.x = 0.0f;
            }
            if ((double) vector2_2.x > 1.0)
            {
              vector2_1.x -= vector2_2.x - 1f;
              vector2_2.x = 1f;
            }
            if ((double) vector2_1.y < 0.0)
            {
              vector2_2.y += -1f * vector2_1.y;
              vector2_1.y = 0.0f;
            }
            if ((double) vector2_2.y > 1.0)
            {
              vector2_1.y -= vector2_2.y - 1f;
              vector2_2.y = 1f;
            }
            this.m_cachedUVRescale = new Vector4(vector2_1.x, vector2_1.y, vector2_2.x, vector2_2.y);
          }
          else
            this.m_cachedUVRescale = new Vector4(0.0f, 0.0f, 1f, 1f / num1);
        }
      }
    }

    private bool ShouldUseJunkPic()
    {
      for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
      {
        PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
        if ((bool) (UnityEngine.Object) allPlayer)
        {
          for (int index2 = 0; index2 < allPlayer.passiveItems.Count; ++index2)
          {
            if (allPlayer.passiveItems[index2] is CompanionItem)
            {
              CompanionItem passiveItem = allPlayer.passiveItems[index2] as CompanionItem;
              if ((bool) (UnityEngine.Object) passiveItem.ExtantCompanion && (bool) (UnityEngine.Object) passiveItem.ExtantCompanion.GetComponent<SackKnightController>() && passiveItem.ExtantCompanion.GetComponent<SackKnightController>().CurrentForm == SackKnightController.SackKnightPhase.ANGELIC_KNIGHT)
                return true;
            }
          }
        }
      }
      return false;
    }

    private void SetWinPic()
    {
      if (this.ShouldUseJunkPic() && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.FINALGEON)
      {
        switch (GameManager.Instance.PrimaryPlayer.characterIdentity)
        {
          case PlayableCharacters.Pilot:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_pilot_001", ".png") as Texture;
            break;
          case PlayableCharacters.Convict:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_convict_001", ".png") as Texture;
            break;
          case PlayableCharacters.Robot:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_robot_001", ".png") as Texture;
            break;
          case PlayableCharacters.Soldier:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_marine_001", ".png") as Texture;
            break;
          case PlayableCharacters.Guide:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_hunter_001", ".png") as Texture;
            break;
          case PlayableCharacters.Bullet:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_bullet_001", ".png") as Texture;
            break;
          case PlayableCharacters.Eevee:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_eevee_001", ".png") as Texture;
            break;
          case PlayableCharacters.Gunslinger:
            this.photoSprite.Texture = BraveResources.Load("win_pic_junkan_slinger_001", ".png") as Texture;
            break;
          default:
            this.photoSprite.Texture = BraveResources.Load("Win_Pic_Gun_Get_001", ".png") as Texture;
            break;
        }
      }
      else if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
      {
        this.photoSprite.Texture = BraveResources.Load("Win_Pic_BossRush_001", ".png") as Texture;
      }
      else
      {
        switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
        {
          case GlobalDungeonData.ValidTilesets.FORGEGEON:
            this.photoSprite.Texture = BraveResources.Load("Win_Pic_Gun_Get_001", ".png") as Texture;
            break;
          case GlobalDungeonData.ValidTilesets.HELLGEON:
            if (GameManager.IsGunslingerPast)
            {
              this.photoSprite.Texture = BraveResources.Load("Win_Pic_Slinger_001", ".png") as Texture;
              break;
            }
            this.photoSprite.Texture = BraveResources.Load("Win_Pic_Lich_Kill_001", ".png") as Texture;
            break;
          case GlobalDungeonData.ValidTilesets.FINALGEON:
            switch (GameManager.Instance.PrimaryPlayer.characterIdentity)
            {
              case PlayableCharacters.Pilot:
                this.photoSprite.Texture = BraveResources.Load("Win_Pic_Pilot_001", ".png") as Texture;
                return;
              case PlayableCharacters.Convict:
                this.photoSprite.Texture = BraveResources.Load("Win_Pic_Convict_001", ".png") as Texture;
                return;
              case PlayableCharacters.Robot:
                this.photoSprite.Texture = BraveResources.Load("Win_Pic_Robot_001", ".png") as Texture;
                return;
              case PlayableCharacters.Soldier:
                this.photoSprite.Texture = BraveResources.Load("Win_Pic_Marine_001", ".png") as Texture;
                return;
              case PlayableCharacters.Guide:
                this.photoSprite.Texture = BraveResources.Load("Win_Pic_Hunter_001", ".png") as Texture;
                return;
              case PlayableCharacters.Bullet:
                this.photoSprite.Texture = BraveResources.Load("Win_Pic_Bullet_001", ".png") as Texture;
                return;
              default:
                this.photoSprite.Texture = BraveResources.Load("Win_Pic_Gun_Get_001", ".png") as Texture;
                return;
            }
          default:
            this.photoSprite.Texture = BraveResources.Load("Win_Pic_Gun_Get_001", ".png") as Texture;
            break;
        }
      }
    }

    public void BraveOnLevelWasLoaded() => this.m_doingSomething = false;

    private void UpdateTapeLabel(dfLabel targetLabel, float overrideWidth = -1f)
    {
      dfPanel parent = targetLabel.Parent as dfPanel;
      if ((double) overrideWidth > 0.0)
        parent.Width = overrideWidth.Quantize(4f) + 32f;
      else
        parent.Width = targetLabel.Width + 32f;
    }

    private void Update()
    {
      if (this.isRightPage)
      {
        if (this.isVictoryPage || !((UnityEngine.Object) this.photoSprite.RenderMaterial != (UnityEngine.Object) null))
          return;
        this.photoSprite.RenderMaterial.SetVector("_UVRescale", this.m_cachedUVRescale);
      }
      else
      {
        if (!this.quickRestartButton.HasFocus && !this.mainMenuButton.HasFocus)
          this.quickRestartButton.Focus(true);
        if (!(bool) (UnityEngine.Object) AmmonomiconController.Instance || AmmonomiconController.Instance.HandlingQueuedUnlocks || AmmonomiconController.Instance.IsOpening || AmmonomiconController.Instance.IsClosing)
          return;
        if (Input.GetKeyDown(KeyCode.Escape) || !BraveInput.PrimaryPlayerInstance.IsKeyboardAndMouse() && BraveInput.WasCancelPressed())
        {
          this.DoMainMenu((dfControl) null, (dfMouseEventArgs) null);
        }
        else
        {
          if (!Input.GetKeyDown(KeyCode.Space) && (BraveInput.PrimaryPlayerInstance.IsKeyboardAndMouse() || !BraveInput.WasSelectPressed()))
            return;
          this.DoQuickRestart((dfControl) null, (dfMouseEventArgs) null);
        }
      }
    }

    private void DoMainMenu(dfControl control, dfMouseEventArgs mouseEvent)
    {
      if (AmmonomiconController.Instance.IsOpening || AmmonomiconController.Instance.IsClosing)
        return;
      SaveManager.DeleteCurrentSlotMidGameSave();
      GameManager.Instance.StartCoroutine(this.HandleMainMenu());
    }

    [DebuggerHidden]
    private IEnumerator HandleMainMenu()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmmonomiconDeathPageController__HandleMainMenuc__Iterator0()
      {
        _this = this
      };
    }

    private void DoQuickRestart(dfControl control, dfMouseEventArgs mouseEvent)
    {
      if (AmmonomiconController.Instance.IsOpening || AmmonomiconController.Instance.IsClosing || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
        return;
      SaveManager.DeleteCurrentSlotMidGameSave();
      GameManager.Instance.StartCoroutine(this.HandleQuickRestart());
    }

    public static QuickRestartOptions GetNumMetasToQuickRestart()
    {
      QuickRestartOptions metasToQuickRestart = new QuickRestartOptions();
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && GameManager.Instance.AllPlayers[index].CharacterUsesRandomGuns)
        {
          metasToQuickRestart.GunGame = true;
          metasToQuickRestart.NumMetas += 6;
          break;
        }
        if (GameManager.Instance.AllPlayers[index].characterIdentity == PlayableCharacters.Eevee)
          metasToQuickRestart.NumMetas += 5;
        else if (GameManager.Instance.AllPlayers[index].characterIdentity == PlayableCharacters.Gunslinger)
        {
          if (!GameStatsManager.Instance.GetFlag(GungeonFlags.GUNSLINGER_UNLOCKED))
            metasToQuickRestart.NumMetas += 5;
          else
            metasToQuickRestart.NumMetas += 7;
        }
      }
      if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
      {
        metasToQuickRestart.BossRush = true;
        metasToQuickRestart.NumMetas += 3;
      }
      if (ChallengeManager.CHALLENGE_MODE_ACTIVE)
      {
        metasToQuickRestart.ChallengeMode = ChallengeManager.ChallengeModeType;
        switch (ChallengeManager.ChallengeModeType)
        {
          case ChallengeModeType.ChallengeMode:
            if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.CHALLENGE_MODE_ATTEMPTS) >= 30.0)
            {
              ++metasToQuickRestart.NumMetas;
              break;
            }
            metasToQuickRestart.NumMetas += 6;
            break;
          case ChallengeModeType.ChallengeMegaMode:
            if ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.CHALLENGE_MODE_ATTEMPTS) >= 30.0)
            {
              metasToQuickRestart.NumMetas += 2;
              break;
            }
            metasToQuickRestart.NumMetas += 12;
            break;
        }
      }
      return metasToQuickRestart;
    }

    [DebuggerHidden]
    private IEnumerator HandleQuickRestart()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new AmmonomiconDeathPageController__HandleQuickRestartc__Iterator1()
      {
        _this = this
      };
    }
  }

