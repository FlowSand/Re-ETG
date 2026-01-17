// Decompiled with JetBrains decompiler
// Type: GameOptions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullSerializer;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GameOptions
    {
      private static bool? m_cachedSupportsStencil;
      [fsIgnore]
      private GameOptions.GenericHighMedLowOption? m_DefaultRecommendedQuality;
      [fsProperty]
      public bool SLOW_TIME_ON_CHALLENGE_MODE_REVEAL = true;
      [fsProperty]
      private float m_gamma = 1f;
      [fsProperty]
      public float DisplaySafeArea = 1f;
      [fsProperty]
      public GameOptions.ControllerBlankControl additionalBlankControl = GameOptions.ControllerBlankControl.BOTH_STICKS_DOWN;
      [fsProperty]
      public GameOptions.ControllerBlankControl additionalBlankControlTwo = GameOptions.ControllerBlankControl.BOTH_STICKS_DOWN;
      [fsIgnore]
      public bool OverrideMotionEnhancementModeForPause;
      [fsIgnore]
      public Dictionary<int, int> PlayerIDtoDeviceIndexMap = new Dictionary<int, int>();
      [fsIgnore]
      public static bool RequiresLanguageReinitialization;
      [fsProperty]
      public GameOptions.QuickstartCharacter PreferredQuickstartCharacter;
      [fsProperty]
      public PlayableCharacters LastPlayedCharacter;
      [fsProperty]
      public GameOptions.GameLootProfile CurrentGameLootProfile;
      [fsProperty]
      public bool IncreaseSpeedOutOfCombat;
      [fsProperty]
      private GameOptions.FullscreenStyle m_fullscreenStyle;
      [fsIgnore]
      public int CurrentMonitorIndex;
      [fsProperty]
      private int m_currentCursorIndex;
      [fsProperty]
      private GameOptions.VisualPresetMode m_visualPresetMode;
      [fsProperty]
      private GameOptions.ControlPreset m_currentControlPreset;
      [fsProperty]
      private GameOptions.ControlPreset m_currentControlPresetP2;
      [fsProperty]
      private StringTableManager.GungeonSupportedLanguages m_currentLanguage;
      [fsProperty]
      private bool m_doVsync = true;
      [fsProperty]
      private GameOptions.GenericHighMedLowOption m_lightingQuality = GameOptions.GenericHighMedLowOption.HIGH;
      [fsProperty]
      public bool QuickSelectEnabled = true;
      [fsProperty]
      public bool HideEmptyGuns = true;
      [fsProperty]
      private GameOptions.GenericHighMedLowOption m_shaderQuality = GameOptions.GenericHighMedLowOption.HIGH;
      [fsProperty]
      private bool m_realtimeReflections = true;
      [fsProperty]
      private GameOptions.GenericHighMedLowOption m_debrisQuantity = GameOptions.GenericHighMedLowOption.HIGH;
      [fsProperty]
      private GameOptions.GenericHighMedLowOption m_textSpeed = GameOptions.GenericHighMedLowOption.MEDIUM;
      [fsProperty]
      private float m_screenShakeMultiplier = 1f;
      [fsProperty]
      private bool m_coopScreenShakeReduction = true;
      [fsProperty]
      private float m_stickyFrictionMultiplier = 1f;
      [fsProperty]
      public bool HasEverSeenAmmonomicon;
      [fsProperty]
      public bool SpeedrunMode;
      [fsProperty]
      public bool RumbleEnabled = true;
      [fsProperty]
      public bool SmallUIEnabled;
      [fsProperty]
      public bool m_beastmode;
      [fsProperty]
      public bool mouseAimLook = true;
      [fsProperty]
      public bool SuperSmoothCamera;
      [fsProperty]
      public bool DisplaySpeedrunCentiseconds;
      [fsProperty]
      public bool DisableQuickGunKeys;
      [fsProperty]
      public bool AllowMoveKeysToChangeGuns;
      [fsProperty]
      public bool autofaceMovementDirection = true;
      [fsProperty]
      public float controllerAimLookMultiplier = 1f;
      [fsProperty]
      public GameOptions.ControllerAutoAim controllerAutoAim;
      [fsProperty]
      public float controllerAimAssistMultiplier = 1f;
      [fsProperty]
      public bool controllerBeamAimAssist;
      [fsProperty]
      public bool allowXinputControllers = true;
      [fsProperty]
      public bool allowNonXinputControllers = true;
      [fsProperty]
      public bool allowUnknownControllers;
      [fsProperty(DeserializeOnly = true)]
      public bool wipeAllAchievements;
      [fsProperty(DeserializeOnly = true)]
      public bool scanAchievementsForUnlocks;
      [fsProperty]
      public int preferredResolutionX = -1;
      [fsProperty]
      public int preferredResolutionY = -1;
      [fsProperty]
      private GameOptions.ControllerSymbology m_playerOnePreferredSymbology = GameOptions.ControllerSymbology.AutoDetect;
      [fsProperty]
      private GameOptions.ControllerSymbology m_playerTwoPreferredSymbology = GameOptions.ControllerSymbology.AutoDetect;
      [fsProperty]
      private bool m_playerOneControllerCursor;
      [fsProperty]
      private bool m_playerTwoControllerCursor;
      [fsProperty]
      private GameOptions.PreferredScalingMode m_preferredScalingMode = GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT;
      [fsProperty]
      private GameOptions.PreferredFullscreenMode m_preferredFullscreenMode;
      [fsProperty]
      public float PreferredMapZoom;
      [fsProperty]
      public float PreferredMinimapZoom = 2f;
      [fsProperty]
      private Minimap.MinimapDisplayMode m_minimapDisplayMode = Minimap.MinimapDisplayMode.FADE_ON_ROOM_SEAL;
      [fsProperty]
      private GameOptions.AudioHardwareMode m_audioHardware;
      [fsProperty]
      private float m_musicVolume = 80f;
      [fsProperty]
      private float m_soundVolume = 80f;
      [fsProperty]
      private float m_uiVolume = 80f;
      [fsProperty]
      public string lastUsedShortcutTarget;
      [fsProperty]
      public string playerOneBindingData;
      [fsProperty]
      public string playerTwoBindingData;
      [fsProperty]
      public string playerOneBindingDataV2;
      [fsProperty]
      public string playerTwoBindingDataV2;

      public static bool SupportsStencil
      {
        get
        {
          if (GameOptions.m_cachedSupportsStencil.HasValue && GameOptions.m_cachedSupportsStencil.HasValue)
            return GameOptions.m_cachedSupportsStencil.Value;
          bool supportsStencil = SystemInfo.supportsStencil > 0;
          if (supportsStencil)
          {
            string graphicsDeviceName = SystemInfo.graphicsDeviceName;
            if (!string.IsNullOrEmpty(graphicsDeviceName) && (graphicsDeviceName.Contains("HD Graphics 4000") || graphicsDeviceName.Contains("620M") || graphicsDeviceName.Contains("630M")))
              supportsStencil = false;
          }
          Debug.Log((object) ("BRV::StencilMode: " + (object) supportsStencil));
          GameOptions.m_cachedSupportsStencil = new bool?(supportsStencil);
          return supportsStencil;
        }
      }

      public static void SetStartupQualitySettings()
      {
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;
        string graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        Debug.Log((object) ("> = > = > BRAVE QUALITY: " + (object) (false | graphicsDeviceVendor.Contains("NVIDIA") | graphicsDeviceVendor.Contains("AMD") | graphicsDeviceName.Contains("NVIDIA") | graphicsDeviceName.Contains("AMD"))));
      }

      public static GameOptions CloneOptions(GameOptions source)
      {
        GameOptions gameOptions = new GameOptions();
        foreach (FieldInfo field in typeof (GameOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
        {
          bool flag = false;
          if (field.GetCustomAttributes(typeof (fsPropertyAttribute), false).Length > 0)
            flag = true;
          if (flag)
            field.SetValue((object) gameOptions, field.GetValue((object) source));
        }
        gameOptions.UpdateCmdArgs();
        return gameOptions;
      }

      public GameOptions.GenericHighMedLowOption GetDefaultRecommendedGraphicalQuality()
      {
        if (this.m_DefaultRecommendedQuality.HasValue)
          return this.m_DefaultRecommendedQuality.Value;
        if (SystemInfo.graphicsMemorySize <= 512 /*0x0200*/ || SystemInfo.systemMemorySize <= 1536 /*0x0600*/)
          return GameOptions.GenericHighMedLowOption.LOW;
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;
        if (!string.IsNullOrEmpty(graphicsDeviceName) && graphicsDeviceName.ToLowerInvariant().Contains("intel"))
        {
          this.m_DefaultRecommendedQuality = new GameOptions.GenericHighMedLowOption?(GameOptions.GenericHighMedLowOption.MEDIUM);
          return this.m_DefaultRecommendedQuality.Value;
        }
        string graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        if (!string.IsNullOrEmpty(graphicsDeviceVendor) && graphicsDeviceVendor.ToLowerInvariant().Contains("intel"))
        {
          this.m_DefaultRecommendedQuality = new GameOptions.GenericHighMedLowOption?(GameOptions.GenericHighMedLowOption.MEDIUM);
          return this.m_DefaultRecommendedQuality.Value;
        }
        this.m_DefaultRecommendedQuality = new GameOptions.GenericHighMedLowOption?(GameOptions.GenericHighMedLowOption.HIGH);
        return this.m_DefaultRecommendedQuality.Value;
      }

      public void RevertToDefaults()
      {
        GameOptions gameOptions = new GameOptions();
        foreach (FieldInfo field in typeof (GameOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
        {
          bool flag = false;
          if (field.GetCustomAttributes(typeof (fsPropertyAttribute), false).Length > 0)
            flag = true;
          if (flag)
            field.SetValue((object) this, field.GetValue((object) gameOptions));
        }
        GameOptions.GenericHighMedLowOption graphicalQuality = this.GetDefaultRecommendedGraphicalQuality();
        this.DoVsync = true;
        this.LightingQuality = graphicalQuality != GameOptions.GenericHighMedLowOption.LOW ? GameOptions.GenericHighMedLowOption.HIGH : GameOptions.GenericHighMedLowOption.LOW;
        this.ShaderQuality = graphicalQuality;
        this.DebrisQuantity = graphicalQuality;
        this.RealtimeReflections = graphicalQuality != GameOptions.GenericHighMedLowOption.LOW;
        this.CurrentLanguage = GameManager.Instance.platformInterface.GetPreferredLanguage();
        StringTableManager.SetNewLanguage(GameManager.Options.CurrentLanguage, true);
        GameManager.Options.MusicVolume = GameManager.Options.MusicVolume;
        GameManager.Options.SoundVolume = GameManager.Options.SoundVolume;
        GameManager.Options.UIVolume = GameManager.Options.UIVolume;
        GameManager.Options.AudioHardware = GameManager.Options.AudioHardware;
        this.UpdateCmdArgs();
      }

      private void UpdateCmdArgs()
      {
        string commandLine = Environment.CommandLine;
        if (commandLine.Contains("-xinputOnly", true))
          this.allowNonXinputControllers = false;
        if (commandLine.Contains("-noXinput", true))
          this.allowXinputControllers = false;
        if (!commandLine.Contains("-allowUnknownControllers", true))
          return;
        this.allowUnknownControllers = true;
      }

      public static bool CompareSettings(GameOptions clone, GameOptions source)
      {
        if (clone == null || source == null)
        {
          Debug.LogError((object) $"{(object) clone}|{(object) source} OPTIONS ARE NULL!");
          return false;
        }
        bool flag1 = true;
        foreach (FieldInfo field in typeof (GameOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
        {
          if (field != null)
          {
            bool flag2 = false;
            if (field.GetCustomAttributes(typeof (fsPropertyAttribute), false).Length > 0)
              flag2 = true;
            if (flag2)
            {
              object obj1 = field.GetValue((object) clone);
              object obj2 = field.GetValue((object) source);
              if (obj1 != null && obj2 != null)
              {
                bool flag3 = obj1.Equals(obj2);
                flag1 &= flag3;
              }
            }
          }
        }
        return flag1;
      }

      public void ApplySettings(GameOptions clone)
      {
        foreach (FieldInfo field in typeof (GameOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
        {
          if (field.GetCustomAttributes(typeof (fsPropertyAttribute), false).Length > 0 && field.GetValue((object) this) != field.GetValue((object) clone))
            field.SetValue((object) this, field.GetValue((object) clone));
        }
        this.playerOneBindingDataV2 = clone.playerOneBindingDataV2;
        this.playerTwoBindingDataV2 = clone.playerTwoBindingDataV2;
        if (this != GameManager.Options)
          return;
        BraveInput.ForceLoadBindingInfoFromOptions();
      }

      [fsIgnore]
      public float MusicVolume
      {
        get => this.m_musicVolume;
        set
        {
          this.m_musicVolume = value;
          int num = (int) AkSoundEngine.SetRTPCValue("VOL_MUS", this.m_musicVolume);
        }
      }

      [fsIgnore]
      public float SoundVolume
      {
        get => this.m_soundVolume;
        set
        {
          this.m_soundVolume = value;
          int num = (int) AkSoundEngine.SetRTPCValue("VOL_SFX", this.m_soundVolume);
        }
      }

      [fsIgnore]
      public float UIVolume
      {
        get => this.m_uiVolume;
        set
        {
          this.m_uiVolume = value;
          int num = (int) AkSoundEngine.SetRTPCValue("VOL_UI", this.m_uiVolume);
        }
      }

      [fsIgnore]
      public float Gamma
      {
        get => this.m_gamma;
        set => this.m_gamma = value;
      }

      [fsIgnore]
      public GameOptions.PixelatorMotionEnhancementMode MotionEnhancementMode
      {
        get
        {
          return this.OverrideMotionEnhancementModeForPause || this.ShaderQuality != GameOptions.GenericHighMedLowOption.HIGH ? GameOptions.PixelatorMotionEnhancementMode.UNENHANCED_CHEAP : GameOptions.PixelatorMotionEnhancementMode.ENHANCED_EXPENSIVE;
        }
      }

      [fsIgnore]
      public GameOptions.AudioHardwareMode AudioHardware
      {
        get => this.m_audioHardware;
        set
        {
          this.m_audioHardware = value;
          switch (this.m_audioHardware)
          {
            case GameOptions.AudioHardwareMode.SPEAKERS:
              int num1 = (int) AkSoundEngine.SetPanningRule(AkPanningRule.AkPanningRule_Speakers);
              break;
            case GameOptions.AudioHardwareMode.HEADPHONES:
              int num2 = (int) AkSoundEngine.SetPanningRule(AkPanningRule.AkPanningRule_Headphones);
              break;
          }
        }
      }

      [fsIgnore]
      public Minimap.MinimapDisplayMode MinimapDisplayMode
      {
        get => this.m_minimapDisplayMode;
        set => this.m_minimapDisplayMode = value;
      }

      public static void Load()
      {
        SaveManager.Init();
        GameOptions gameOptions = (GameOptions) null;
        bool flag = SaveManager.Load<GameOptions>(SaveManager.OptionsSave, out gameOptions, true);
        if (!flag)
        {
          for (int index = 0; index < 3 && !flag; ++index)
          {
            if ((SaveManager.SaveSlot) index != SaveManager.CurrentSaveSlot)
            {
              gameOptions = (GameOptions) null;
              flag = SaveManager.Load<GameOptions>(SaveManager.OptionsSave, out gameOptions, true, overrideSaveSlot: new SaveManager.SaveSlot?((SaveManager.SaveSlot) index)) & gameOptions != null;
            }
          }
        }
        if (!flag || gameOptions == null)
        {
          GameManager.Options = new GameOptions();
          GameOptions.RequiresLanguageReinitialization = true;
        }
        else
        {
          GameManager.Options = gameOptions;
          GameManager.Options.MusicVolume = GameManager.Options.MusicVolume;
          GameManager.Options.SoundVolume = GameManager.Options.SoundVolume;
          GameManager.Options.UIVolume = GameManager.Options.UIVolume;
          GameManager.Options.AudioHardware = GameManager.Options.AudioHardware;
        }
        GameManager.Options.UpdateCmdArgs();
        GameManager.Options.controllerAimAssistMultiplier = Mathf.Clamp(GameManager.Options.controllerAimAssistMultiplier, 0.0f, 1.25f);
        GameManager.Options.DisplaySafeArea = Mathf.Clamp(GameManager.Options.DisplaySafeArea, 0.9f, 1f);
        if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.HIGH || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM)
          Shader.SetGlobalFloat("_LowQualityMode", 0.0f);
        else
          Shader.SetGlobalFloat("_LowQualityMode", 1f);
        if (!Brave.PlayerPrefs.HasKey("UnitySelectMonitor"))
          return;
        GameManager.Options.CurrentMonitorIndex = Brave.PlayerPrefs.GetInt("UnitySelectMonitor");
      }

      public static bool Save()
      {
        return SaveManager.Save<GameOptions>(GameManager.Options, SaveManager.OptionsSave, 0);
      }

      [fsIgnore]
      public GameOptions.FullscreenStyle CurrentFullscreenStyle
      {
        get => this.m_fullscreenStyle;
        set
        {
          this.m_fullscreenStyle = value;
          if (this.m_fullscreenStyle == GameOptions.FullscreenStyle.BORDERLESS)
            return;
          GameCursorController component = GameUIRoot.Instance.GetComponent<GameCursorController>();
          if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
            return;
          component.ToggleClip(false);
        }
      }

      public int CurrentCursorIndex
      {
        get => this.m_currentCursorIndex;
        set => this.m_currentCursorIndex = value;
      }

      [fsIgnore]
      public GameOptions.VisualPresetMode CurrentVisualPreset
      {
        get => this.m_visualPresetMode;
        set
        {
          if (this.m_visualPresetMode == value)
            return;
          this.m_visualPresetMode = value;
          if (this.m_visualPresetMode != GameOptions.VisualPresetMode.RECOMMENDED)
            return;
          Resolution recommendedResolution = this.GetRecommendedResolution();
          this.CurrentPreferredScalingMode = this.GetRecommendedScalingMode();
          this.CurrentPreferredFullscreenMode = GameOptions.PreferredFullscreenMode.FULLSCREEN;
          Debug.Log((object) $"Setting screen resolution RECOMMENDED: {(object) recommendedResolution.width}|{(object) recommendedResolution.height}");
          GameManager.Instance.DoSetResolution(recommendedResolution.width, recommendedResolution.height, true);
        }
      }

      [fsIgnore]
      public StringTableManager.GungeonSupportedLanguages CurrentLanguage
      {
        get => this.m_currentLanguage;
        set
        {
          if (this.m_currentLanguage == value)
            return;
          this.m_currentLanguage = value;
          StringTableManager.CurrentLanguage = value;
          BraveInput.OnLanguageChanged();
        }
      }

      private BraveInput GetBestInputInstance(int targetPlayerIndex)
      {
        return GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER || !Foyer.DoMainMenu ? (targetPlayerIndex != -1 ? BraveInput.GetInstanceForPlayer(targetPlayerIndex) : BraveInput.PrimaryPlayerInstance) : BraveInput.PlayerlessInstance;
      }

      public GameOptions.ControlPreset CurrentControlPreset
      {
        get => this.m_currentControlPreset;
        set
        {
          this.m_currentControlPreset = value;
          if (this.m_currentControlPreset == GameOptions.ControlPreset.RECOMMENDED)
          {
            this.GetBestInputInstance(0).ActiveActions.ReinitializeDefaults();
          }
          else
          {
            if (this.m_currentControlPreset != GameOptions.ControlPreset.FLIPPED_TRIGGERS)
              return;
            this.GetBestInputInstance(0).ActiveActions.InitializeSwappedTriggersPreset();
          }
        }
      }

      public GameOptions.ControlPreset CurrentControlPresetP2
      {
        get => this.m_currentControlPresetP2;
        set
        {
          this.m_currentControlPresetP2 = value;
          if (this.m_currentControlPresetP2 == GameOptions.ControlPreset.RECOMMENDED)
          {
            this.GetBestInputInstance(1).ActiveActions.ReinitializeDefaults();
          }
          else
          {
            if (this.m_currentControlPresetP2 != GameOptions.ControlPreset.FLIPPED_TRIGGERS)
              return;
            this.GetBestInputInstance(1).ActiveActions.InitializeSwappedTriggersPreset();
          }
        }
      }

      public GameOptions.PreferredScalingMode GetRecommendedScalingMode()
      {
        return Screen.width % Pixelator.Instance.CurrentMacroResolutionX == 0 && Screen.height % Pixelator.Instance.CurrentMacroResolutionY == 0 ? GameOptions.PreferredScalingMode.PIXEL_PERFECT : GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT;
      }

      public Resolution GetRecommendedResolution()
      {
        Resolution[] resolutions = Screen.resolutions;
        Resolution recommendedResolution = resolutions[0];
        float num1 = 1.77777779f;
        bool flag1 = recommendedResolution.width % Pixelator.Instance.CurrentMacroResolutionX == 0 && recommendedResolution.height % Pixelator.Instance.CurrentMacroResolutionY == 0;
        for (int index = 0; index < resolutions.Length; ++index)
        {
          Resolution resolution = resolutions[index];
          if (resolution.height >= recommendedResolution.height)
          {
            float num2 = (float) recommendedResolution.width / ((float) recommendedResolution.height * 1f);
            float num3 = (float) resolution.width / ((float) resolution.height * 1f);
            if ((double) num2 != (double) num1 || (double) num3 == (double) num1)
            {
              if ((double) num2 == (double) num1 && (double) num3 == (double) num1)
              {
                bool flag2 = resolution.width % Pixelator.Instance.CurrentMacroResolutionX == 0 && resolution.height % Pixelator.Instance.CurrentMacroResolutionY == 0;
                if (flag1)
                {
                  if (flag2 && (resolution.height > recommendedResolution.height || resolution.refreshRate > recommendedResolution.refreshRate))
                  {
                    recommendedResolution = resolution;
                    flag1 = true;
                  }
                }
                else
                {
                  recommendedResolution = resolution;
                  flag1 = flag2;
                }
              }
              else
              {
                bool flag3 = resolution.width % Pixelator.Instance.CurrentMacroResolutionX == 0 && resolution.height % Pixelator.Instance.CurrentMacroResolutionY == 0;
                recommendedResolution = resolution;
                flag1 = flag3;
              }
            }
          }
        }
        return recommendedResolution;
      }

      [fsIgnore]
      public GameOptions.PreferredScalingMode CurrentPreferredScalingMode
      {
        get => this.m_preferredScalingMode;
        set => this.m_preferredScalingMode = value;
      }

      [fsIgnore]
      public GameOptions.PreferredFullscreenMode CurrentPreferredFullscreenMode
      {
        get => this.m_preferredFullscreenMode;
        set => this.m_preferredFullscreenMode = value;
      }

      [fsIgnore]
      public bool DoVsync
      {
        get => this.m_doVsync;
        set
        {
          this.m_doVsync = value;
          QualitySettings.vSyncCount = !this.m_doVsync ? 0 : 1;
        }
      }

      [fsIgnore]
      public GameOptions.GenericHighMedLowOption LightingQuality
      {
        get => this.m_lightingQuality;
        set
        {
          if (this.m_lightingQuality == value)
            return;
          this.m_lightingQuality = value;
          if (this.m_lightingQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
            this.m_lightingQuality = GameOptions.GenericHighMedLowOption.LOW;
          if (this.m_lightingQuality == GameOptions.GenericHighMedLowOption.MEDIUM)
            this.m_lightingQuality = GameOptions.GenericHighMedLowOption.HIGH;
          ShadowSystem.ForceAllLightsUpdate();
          if (!((UnityEngine.Object) Pixelator.Instance != (UnityEngine.Object) null))
            return;
          Pixelator.Instance.OnChangedLightingQuality(this.m_lightingQuality);
        }
      }

      [fsIgnore]
      public GameOptions.GenericHighMedLowOption ShaderQuality
      {
        get => this.m_shaderQuality;
        set
        {
          this.m_shaderQuality = value;
          if (this.m_shaderQuality == GameOptions.GenericHighMedLowOption.HIGH || this.m_shaderQuality == GameOptions.GenericHighMedLowOption.MEDIUM)
            Shader.SetGlobalFloat("_LowQualityMode", 0.0f);
          else
            Shader.SetGlobalFloat("_LowQualityMode", 1f);
          if (!GameManager.HasInstance || !(bool) (UnityEngine.Object) GameManager.Instance.Dungeon)
            return;
          RenderSettings.ambientIntensity = GameManager.Instance.Dungeon.TargetAmbientIntensity;
        }
      }

      [fsIgnore]
      public bool RealtimeReflections
      {
        get => this.m_realtimeReflections;
        set
        {
          Shader.SetGlobalFloat("_GlobalReflectionsEnabled", !value ? 0.0f : 1f);
          this.m_realtimeReflections = value;
        }
      }

      [fsIgnore]
      public GameOptions.GenericHighMedLowOption DebrisQuantity
      {
        get => this.m_debrisQuantity;
        set
        {
          this.m_debrisQuantity = value;
          if (!((UnityEngine.Object) SpawnManager.Instance != (UnityEngine.Object) null))
            return;
          SpawnManager.Instance.OnDebrisQuantityChanged();
        }
      }

      [fsIgnore]
      public GameOptions.GenericHighMedLowOption TextSpeed
      {
        get => this.m_textSpeed;
        set => this.m_textSpeed = value;
      }

      [fsIgnore]
      public float ScreenShakeMultiplier
      {
        get => this.m_screenShakeMultiplier;
        set => this.m_screenShakeMultiplier = value;
      }

      [fsIgnore]
      public bool CoopScreenShakeReduction
      {
        get => this.m_coopScreenShakeReduction;
        set => this.m_coopScreenShakeReduction = value;
      }

      [fsIgnore]
      public float StickyFrictionMultiplier
      {
        get => this.m_stickyFrictionMultiplier;
        set => this.m_stickyFrictionMultiplier = value;
      }

      [fsIgnore]
      public GameOptions.ControllerSymbology PlayerOnePreferredSymbology
      {
        get => this.m_playerOnePreferredSymbology;
        set => this.m_playerOnePreferredSymbology = value;
      }

      [fsIgnore]
      public GameOptions.ControllerSymbology PlayerTwoPreferredSymbology
      {
        get => this.m_playerTwoPreferredSymbology;
        set => this.m_playerTwoPreferredSymbology = value;
      }

      [fsIgnore]
      public bool PlayerOneControllerCursor
      {
        get => this.m_playerOneControllerCursor;
        set => this.m_playerOneControllerCursor = value;
      }

      [fsIgnore]
      public bool PlayerTwoControllerCursor
      {
        get => this.m_playerTwoControllerCursor;
        set => this.m_playerTwoControllerCursor = value;
      }

      public enum ControllerBlankControl
      {
        NONE,
        BOTH_STICKS_DOWN,
        [Obsolete("Players should only see NONE and BOTH_STICKS_DOWN; this is kept for legacy conversions only.")] CIRCLE,
        [Obsolete("Players should only see NONE and BOTH_STICKS_DOWN; this is kept for legacy conversions only.")] L1,
      }

      public enum ControllerAutoAim
      {
        AUTO_DETECT,
        ALWAYS,
        NEVER,
        COOP_ONLY,
      }

      public enum QuickstartCharacter
      {
        LAST_USED,
        PILOT,
        CONVICT,
        SOLDIER,
        GUIDE,
        BULLET,
        ROBOT,
      }

      public enum AudioHardwareMode
      {
        SPEAKERS,
        HEADPHONES,
      }

      public enum PixelatorMotionEnhancementMode
      {
        ENHANCED_EXPENSIVE,
        UNENHANCED_CHEAP,
      }

      public enum GameLootProfile
      {
        CURRENT = 0,
        ORIGINAL = 5,
      }

      public enum FullscreenStyle
      {
        FULLSCREEN,
        BORDERLESS,
        WINDOWED,
      }

      public enum VisualPresetMode
      {
        RECOMMENDED,
        CUSTOM,
      }

      public enum PreferredScalingMode
      {
        PIXEL_PERFECT,
        UNIFORM_SCALING,
        FORCE_PIXEL_PERFECT,
        UNIFORM_SCALING_FAST,
      }

      public enum PreferredFullscreenMode
      {
        FULLSCREEN,
        BORDERLESS,
        WINDOWED,
      }

      public enum ControlPreset
      {
        RECOMMENDED,
        FLIPPED_TRIGGERS,
        CUSTOM,
      }

      public enum GenericHighMedLowOption
      {
        LOW,
        MEDIUM,
        HIGH,
        VERY_LOW,
      }

      public enum ControllerSymbology
      {
        PS4,
        Xbox,
        AutoDetect,
        Switch,
      }
    }

}
