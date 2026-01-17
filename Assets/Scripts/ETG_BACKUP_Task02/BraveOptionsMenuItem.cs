// Decompiled with JetBrains decompiler
// Type: BraveOptionsMenuItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Analytics;

#nullable disable
public class BraveOptionsMenuItem : MonoBehaviour
{
  public BraveOptionsMenuItem.BraveOptionsOptionType optionType;
  [Header("Control Options")]
  public BraveOptionsMenuItem.BraveOptionsMenuItemType itemType;
  public dfLabel labelControl;
  [Space(5f)]
  public dfLabel selectedLabelControl;
  public dfLabel infoControl;
  public dfProgressBar fillbarControl;
  public dfButton buttonControl;
  public dfControl checkboxChecked;
  public dfControl checkboxUnchecked;
  public string[] labelOptions;
  public string[] infoOptions;
  [Header("UI Key Controls")]
  public dfControl up;
  public dfControl down;
  public dfControl left;
  public dfControl right;
  public bool selectOnAction;
  public Action<dfControl> OnNewControlSelected;
  private int m_selectedIndex;
  private dfControl m_self;
  private bool m_isLocalized;
  private float m_actualFillbarValue;
  private const float c_arrowScale = 3f;
  private static BraveOptionsMenuItem.WindowsResolutionManager m_windowsResolutionManager;
  private bool m_changedThisFrame;
  private Vector3? m_cachedLeftArrowRelativePosition;
  private Vector3? m_cachedRightArrowRelativePosition;
  private List<GameOptions.PreferredScalingMode> m_scalingModes;
  private List<GameOptions.QuickstartCharacter> m_quickStartCharacters;
  private float m_panelStartHeight = -1f;
  private float m_additionalStartHeight = -1f;
  private bool m_infoControlHeightModified;
  private static Color m_unselectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
  private bool m_ignoreLeftRightUntilReleased;

  public static BraveOptionsMenuItem.WindowsResolutionManager ResolutionManagerWin
  {
    get
    {
      if (BraveOptionsMenuItem.m_windowsResolutionManager == null)
        BraveOptionsMenuItem.m_windowsResolutionManager = new BraveOptionsMenuItem.WindowsResolutionManager("Enter the Gungeon");
      return BraveOptionsMenuItem.m_windowsResolutionManager;
    }
  }

  private void OnDestroy()
  {
    if (this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROL_PORT && this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROL_PORT)
      return;
    InputManager.OnDeviceAttached -= new Action<InputDevice>(this.InputManager_OnDeviceAttached);
    InputManager.OnDeviceDetached -= new Action<InputDevice>(this.InputManager_OnDeviceDetached);
  }

  private void ToggleAbledness(bool value)
  {
    if (value)
      return;
    if ((bool) (UnityEngine.Object) this.m_self)
    {
      this.m_self.Disable();
      this.m_self.CanFocus = false;
      this.m_self.IsInteractive = false;
      this.m_self.DisabledColor = (Color32) new Color(0.2f, 0.2f, 0.2f, 1f);
    }
    if ((bool) (UnityEngine.Object) this.labelControl)
      this.labelControl.DisabledColor = (Color32) new Color(0.2f, 0.2f, 0.2f, 1f);
    if ((bool) (UnityEngine.Object) this.left)
      this.left.DisabledColor = (Color32) new Color(0.2f, 0.2f, 0.2f, 1f);
    if ((bool) (UnityEngine.Object) this.right)
      this.right.DisabledColor = (Color32) new Color(0.2f, 0.2f, 0.2f, 1f);
    if ((bool) (UnityEngine.Object) this.selectedLabelControl)
      this.selectedLabelControl.DisabledColor = (Color32) new Color(0.2f, 0.2f, 0.2f, 1f);
    if ((bool) (UnityEngine.Object) this.buttonControl)
    {
      this.buttonControl.Disable();
      this.buttonControl.CanFocus = false;
      this.buttonControl.IsInteractive = false;
      this.buttonControl.DisabledColor = (Color32) new Color(0.2f, 0.2f, 0.2f, 1f);
      this.buttonControl.DisabledTextColor = (Color32) new Color(0.2f, 0.2f, 0.2f, 1f);
    }
    if ((bool) (UnityEngine.Object) this.up && (bool) (UnityEngine.Object) this.down)
    {
      this.up.GetComponent<BraveOptionsMenuItem>().down = this.down;
      this.down.GetComponent<BraveOptionsMenuItem>().up = this.up;
    }
    else if ((bool) (UnityEngine.Object) this.up)
    {
      this.up.GetComponent<BraveOptionsMenuItem>().down = (dfControl) null;
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this.down)
        return;
      this.down.GetComponent<BraveOptionsMenuItem>().up = (dfControl) null;
    }
  }

  private bool DisablePlatformSpecificOptions()
  {
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.GAMEPLAY_SPEED)
    {
      this.DelControl();
      return true;
    }
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SWITCH_PERFORMANCE_MODE || this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SWITCH_REASSIGN_CONTROLLERS)
    {
      this.DelControl();
      return true;
    }
    if (this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL_PS4 && this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.STICKY_FRICTION_AMOUNT && this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.BOTH_CONTROLLER_CURSOR)
      return false;
    this.DelControl();
    return true;
  }

  public void Awake()
  {
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.FULLSCREEN)
    {
      if (BraveOptionsMenuItem.m_windowsResolutionManager == null)
        BraveOptionsMenuItem.m_windowsResolutionManager = new BraveOptionsMenuItem.WindowsResolutionManager("Enter the Gungeon");
      this.labelOptions = new string[3]
      {
        "Fullscreen",
        "Borderless",
        "Windowed"
      };
    }
    dfControl component = this.GetComponent<dfControl>();
    this.m_self = component;
    component.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.self_IsVisibleChanged);
    this.m_isLocalized = component.IsLocalized;
    component.CanFocus = true;
    component.GotFocus += new FocusEventHandler(this.DoFocus);
    component.LostFocus += new FocusEventHandler(this.LostFocus);
    if ((this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESET_SAVE_SLOT || this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SAVE_SLOT) && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
      this.ToggleAbledness(false);
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.LOOT_PROFILE && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
      this.ToggleAbledness(false);
    if (this.labelOptions != null && this.labelOptions.Length > 0 && this.labelOptions[0].StartsWith("#"))
    {
      this.selectedLabelControl.IsLocalized = true;
      this.selectedLabelControl.Localize();
    }
    this.RelocalizeOptions();
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROL_PORT || this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROL_PORT)
    {
      InputManager.OnDeviceAttached += new Action<InputDevice>(this.InputManager_OnDeviceAttached);
      InputManager.OnDeviceDetached += new Action<InputDevice>(this.InputManager_OnDeviceDetached);
    }
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION || this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE || this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.VISUAL_PRESET)
      component.ResolutionChangedPostLayout += new Action<dfControl, Vector3, Vector3>(this.HandleResolutionChanged);
    if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow || this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
    {
      if ((UnityEngine.Object) this.left != (UnityEngine.Object) null)
      {
        this.left.HotZoneScale = Vector2.one * 3f;
        this.left.Click += new MouseEventHandler(this.DecrementArrow);
      }
      if ((UnityEngine.Object) this.right != (UnityEngine.Object) null)
      {
        this.right.HotZoneScale = Vector2.one * 3f;
        this.right.Click += new MouseEventHandler(this.IncrementArrow);
      }
    }
    else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Fillbar)
    {
      this.fillbarControl.Click += new MouseEventHandler(this.HandleFillbarClick);
      this.fillbarControl.MouseDown += new MouseEventHandler(this.HandleFillbarDown);
      this.fillbarControl.MouseMove += new MouseEventHandler(this.HandleFillbarMove);
      this.fillbarControl.MouseHover += new MouseEventHandler(this.HandleFillbarHover);
    }
    else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox)
    {
      component.Click += new MouseEventHandler(this.ToggleCheckbox);
      this.checkboxUnchecked.Click += new MouseEventHandler(this.ToggleCheckbox);
      this.checkboxUnchecked.IsInteractive = false;
      this.checkboxChecked.IsInteractive = false;
    }
    else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Button)
      this.buttonControl.Click += new MouseEventHandler(this.OnButtonClicked);
    if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow || this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
    {
      this.left.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
      this.right.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
      this.selectedLabelControl.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
    }
    else
    {
      if (this.itemType != BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox)
        return;
      this.checkboxChecked.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
      this.checkboxUnchecked.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
    }
  }

  private void HandleLocalTextChangeReposition(dfControl control, string value)
  {
    if (!(bool) (UnityEngine.Object) this.labelControl)
      return;
    this.labelControl.Pivot = dfPivotPoint.BottomLeft;
  }

  private void InputManager_OnDeviceAttached(InputDevice obj) => this.InitializeFromOptions();

  private void InputManager_OnDeviceDetached(InputDevice obj) => this.InitializeFromOptions();

  private void OnButtonClicked(dfControl control, dfMouseEventArgs mouseEvent)
  {
    this.DoSelectedAction();
  }

  private void ToggleCheckbox(dfControl control, dfMouseEventArgs args)
  {
    if (this.m_changedThisFrame || args != null && args.Used)
      return;
    this.m_changedThisFrame = true;
    args?.Use();
    this.m_selectedIndex = (this.m_selectedIndex + 1) % 2;
    this.checkboxChecked.IsVisible = this.m_selectedIndex == 1;
    this.HandleCheckboxValueChanged();
  }

  private void self_IsVisibleChanged(dfControl control, bool value)
  {
    if (!value)
      return;
    this.ConvertPivots();
    dfControl component = this.GetComponent<dfControl>();
    if ((bool) (UnityEngine.Object) component)
      component.PerformLayout();
    this.UpdateSelectedLabelText();
    this.UpdateInfoControl();
  }

  private void RelocalizeOptions()
  {
    if (!this.m_isLocalized || this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE)
      return;
    for (int index = 0; index < this.labelOptions.Length; ++index)
    {
      string labelOption = this.labelOptions[index];
      string str = this.m_self.GetLanguageManager().GetValue(labelOption);
      this.labelOptions[index] = str;
    }
    for (int index = 0; index < this.infoOptions.Length; ++index)
    {
      string infoOption = this.infoOptions[index];
      this.infoOptions[index] = this.m_self.GetLanguageManager().GetValue(infoOption);
    }
  }

  public void LateUpdate() => this.m_changedThisFrame = false;

  public void Update()
  {
    if ((UnityEngine.Object) this.labelControl != (UnityEngine.Object) null && this.labelControl.IsVisible)
    {
      if (GameManager.Options.CurrentVisualPreset == GameOptions.VisualPresetMode.RECOMMENDED)
      {
        if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION)
        {
          Resolution recommendedResolution = GameManager.Options.GetRecommendedResolution();
          if (Screen.width != recommendedResolution.width || Screen.height != recommendedResolution.height)
            BraveOptionsMenuItem.HandleScreenDataChanged(recommendedResolution.width, recommendedResolution.height);
        }
        else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE)
        {
          if (GameManager.Options.CurrentPreferredScalingMode != GameManager.Options.GetRecommendedScalingMode())
          {
            GameManager.Options.CurrentPreferredScalingMode = GameManager.Options.GetRecommendedScalingMode();
            BraveOptionsMenuItem.HandleScreenDataChanged(Screen.width, Screen.height);
          }
        }
        else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.FULLSCREEN && GameManager.Options.CurrentPreferredFullscreenMode != GameOptions.PreferredFullscreenMode.FULLSCREEN)
        {
          GameManager.Options.CurrentPreferredFullscreenMode = GameOptions.PreferredFullscreenMode.FULLSCREEN;
          BraveOptionsMenuItem.HandleScreenDataChanged(Screen.width, Screen.height);
        }
      }
      if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.FULLSCREEN)
      {
        int fromFullscreenMode = this.GetIndexFromFullscreenMode(GameManager.Options.CurrentPreferredFullscreenMode);
        if (this.m_selectedIndex != fromFullscreenMode)
        {
          this.m_selectedIndex = fromFullscreenMode;
          this.DetermineAvailableOptions();
        }
      }
    }
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE && this.m_scalingModes != null && this.m_scalingModes.Count > 0)
    {
      if (this.m_selectedIndex < 0 || this.m_selectedIndex >= this.m_scalingModes.Count)
        this.m_selectedIndex = this.GetScalingIndex(GameManager.Options.CurrentPreferredScalingMode);
      if (this.m_selectedIndex < 0 || this.m_selectedIndex >= this.m_scalingModes.Count)
        this.m_selectedIndex = this.GetScalingIndex(GameOptions.PreferredScalingMode.PIXEL_PERFECT);
      if (this.m_selectedIndex < 0 || this.m_selectedIndex >= this.m_scalingModes.Count)
        this.m_selectedIndex = this.GetScalingIndex(GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT);
      if (this.m_selectedIndex < 0 || this.m_selectedIndex >= this.m_scalingModes.Count)
        this.m_selectedIndex = !this.m_scalingModes.Contains(GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT) ? 0 : this.GetScalingIndex(GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT);
      if (this.m_selectedIndex < this.m_scalingModes.Count && this.m_scalingModes[this.m_selectedIndex] != GameManager.Options.CurrentPreferredScalingMode)
      {
        this.m_selectedIndex = this.GetScalingIndex(GameManager.Options.CurrentPreferredScalingMode);
        if (this.m_selectedIndex < this.labelOptions.Length && this.m_selectedIndex >= 0)
          this.UpdateSelectedLabelText();
      }
    }
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.VISUAL_PRESET && (GameOptions.VisualPresetMode) this.m_selectedIndex != GameManager.Options.CurrentVisualPreset)
    {
      this.m_selectedIndex = (int) GameManager.Options.CurrentVisualPreset;
      if (this.m_selectedIndex >= 0 && this.m_selectedIndex < this.labelOptions.Length)
        this.UpdateSelectedLabelText();
    }
    if (this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION || GameManager.Options.CurrentPreferredFullscreenMode != GameOptions.PreferredFullscreenMode.BORDERLESS)
      return;
    BraveOptionsMenuItem.HandleScreenDataChanged(Screen.currentResolution.width, Screen.currentResolution.height);
  }

  private void HandleResolutionChanged(dfControl arg1, Vector3 arg2, Vector3 arg3)
  {
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION)
      this.DetermineAvailableOptions();
    else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE)
    {
      this.DetermineAvailableOptions();
      if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.PIXEL_PERFECT)
        GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT;
      else if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.UNIFORM_SCALING)
        GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.UNIFORM_SCALING;
      else if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
        GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST;
      else if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT)
        GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT;
      BraveOptionsMenuItem.HandleScreenDataChanged(Screen.width, Screen.height);
    }
    else
    {
      if (this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.FULLSCREEN)
        return;
      this.m_selectedIndex = this.GetIndexFromFullscreenMode(GameManager.Options.CurrentPreferredFullscreenMode);
      this.DetermineAvailableOptions();
    }
  }

  [DebuggerHidden]
  private IEnumerator DeferFunctionNumberOfFrames(int numFrames, System.Action action)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BraveOptionsMenuItem.\u003CDeferFunctionNumberOfFrames\u003Ec__Iterator0()
    {
      numFrames = numFrames,
      action = action
    };
  }

  private void UpdateSelectedLabelText()
  {
    if (!(bool) (UnityEngine.Object) this.selectedLabelControl || this.labelOptions == null || this.labelOptions.Length == 0 || this.m_selectedIndex < 0 || this.m_selectedIndex >= this.labelOptions.Length)
      return;
    string labelOption = this.labelOptions[this.m_selectedIndex];
    if (labelOption.StartsWith("%"))
    {
      string[] strArray = labelOption.Split(' ');
      string empty = string.Empty;
      for (int index = 0; index < strArray.Length; ++index)
        empty += StringTableManager.EvaluateReplacementToken(strArray[index]);
      this.selectedLabelControl.ModifyLocalizedText(empty);
    }
    else
      this.selectedLabelControl.Text = this.labelOptions[this.m_selectedIndex];
    if (!this.left.IsVisible || !this.right.IsVisible)
      return;
    if (!this.m_cachedLeftArrowRelativePosition.HasValue && (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow || this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo) && (bool) (UnityEngine.Object) this.left)
      this.m_cachedLeftArrowRelativePosition = new Vector3?(this.left.RelativePosition);
    if (!this.m_cachedRightArrowRelativePosition.HasValue && (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow || this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo) && (bool) (UnityEngine.Object) this.right)
      this.m_cachedRightArrowRelativePosition = new Vector3?(this.right.RelativePosition);
    this.left.PerformLayout();
    this.right.PerformLayout();
    if (!this.m_cachedLeftArrowRelativePosition.HasValue || !this.m_cachedRightArrowRelativePosition.HasValue)
      return;
    float x = (float) (((double) Mathf.Max(this.m_cachedRightArrowRelativePosition.Value.x - this.m_cachedLeftArrowRelativePosition.Value.x, this.selectedLabelControl.Width + 45f).Quantize(3f, VectorConversions.Ceil) - (double) (this.right.RelativePosition.x - this.left.RelativePosition.x)) / 2.0);
    this.left.RelativePosition += new Vector3(-x, 0.0f, 0.0f);
    this.right.RelativePosition += new Vector3(x, 0.0f, 0.0f);
  }

  private void InitializeVisualPreset()
  {
    Resolution recommendedResolution = GameManager.Options.GetRecommendedResolution();
    GameOptions.PreferredScalingMode recommendedScalingMode = GameManager.Options.GetRecommendedScalingMode();
    this.m_selectedIndex = Screen.width != recommendedResolution.width || Screen.height != recommendedResolution.height || recommendedScalingMode != GameManager.Options.CurrentPreferredScalingMode || GameManager.Options.CurrentPreferredFullscreenMode != GameOptions.PreferredFullscreenMode.FULLSCREEN ? 1 : 0;
    GameManager.Options.CurrentVisualPreset = (GameOptions.VisualPresetMode) this.m_selectedIndex;
    this.UpdateSelectedLabelText();
  }

  private StringTableManager.GungeonSupportedLanguages IntToLanguage(int index)
  {
    switch (index)
    {
      case 0:
        return StringTableManager.GungeonSupportedLanguages.ENGLISH;
      case 1:
        return StringTableManager.GungeonSupportedLanguages.SPANISH;
      case 2:
        return StringTableManager.GungeonSupportedLanguages.FRENCH;
      case 3:
        return StringTableManager.GungeonSupportedLanguages.ITALIAN;
      case 4:
        return StringTableManager.GungeonSupportedLanguages.GERMAN;
      case 5:
        return StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE;
      case 6:
        return StringTableManager.GungeonSupportedLanguages.POLISH;
      case 7:
        return StringTableManager.GungeonSupportedLanguages.RUSSIAN;
      case 8:
        return StringTableManager.GungeonSupportedLanguages.JAPANESE;
      case 9:
        return StringTableManager.GungeonSupportedLanguages.KOREAN;
      case 10:
        return StringTableManager.GungeonSupportedLanguages.CHINESE;
      default:
        return StringTableManager.GungeonSupportedLanguages.ENGLISH;
    }
  }

  private int LanguageToInt(
    StringTableManager.GungeonSupportedLanguages language)
  {
    switch (language)
    {
      case StringTableManager.GungeonSupportedLanguages.ENGLISH:
        return 0;
      case StringTableManager.GungeonSupportedLanguages.FRENCH:
        return 2;
      case StringTableManager.GungeonSupportedLanguages.SPANISH:
        return 1;
      case StringTableManager.GungeonSupportedLanguages.ITALIAN:
        return 3;
      case StringTableManager.GungeonSupportedLanguages.GERMAN:
        return 4;
      case StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE:
        return 5;
      case StringTableManager.GungeonSupportedLanguages.JAPANESE:
        return 8;
      case StringTableManager.GungeonSupportedLanguages.KOREAN:
        return 9;
      case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
        return 7;
      case StringTableManager.GungeonSupportedLanguages.POLISH:
        return 6;
      case StringTableManager.GungeonSupportedLanguages.CHINESE:
        return 10;
      default:
        return 0;
    }
  }

  private void DetermineAvailableOptions()
  {
    BraveOptionsMenuItem.BraveOptionsOptionType optionType = this.optionType;
    switch (optionType)
    {
      case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION:
        this.HandleResolutionDetermination();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE:
        int width = Screen.width;
        int height = Screen.height;
        int num1 = BraveMathCollege.GreatestCommonDivisor(width, height);
        int num2 = width / num1;
        int num3 = height / num1;
        List<string> stringList1 = new List<string>();
        if (this.m_scalingModes == null)
          this.m_scalingModes = new List<GameOptions.PreferredScalingMode>();
        this.m_scalingModes.Clear();
        if (num2 == 16 /*0x10*/ && num3 == 9)
        {
          if (width % 480 == 0 && height % 270 == 0)
          {
            stringList1.Add("#OPTIONS_PIXELPERFECT");
            this.m_scalingModes.Add(GameOptions.PreferredScalingMode.PIXEL_PERFECT);
          }
          else
          {
            stringList1.Add("#OPTIONS_UNIFORMSCALING");
            this.m_scalingModes.Add(GameOptions.PreferredScalingMode.UNIFORM_SCALING);
            stringList1.Add("#OPTIONS_FORCEPIXELPERFECT");
            this.m_scalingModes.Add(GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT);
            stringList1.Add("#OPTIONS_UNIFORMSCALINGFAST");
            this.m_scalingModes.Add(GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST);
          }
        }
        else
        {
          stringList1.Add("#OPTIONS_UNIFORMSCALING");
          this.m_scalingModes.Add(GameOptions.PreferredScalingMode.UNIFORM_SCALING);
          stringList1.Add("#OPTIONS_FORCEPIXELPERFECT");
          this.m_scalingModes.Add(GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT);
          stringList1.Add("#OPTIONS_UNIFORMSCALINGFAST");
          this.m_scalingModes.Add(GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST);
        }
        this.labelOptions = stringList1.ToArray();
        if (this.m_scalingModes.Contains(GameManager.Options.CurrentPreferredScalingMode))
        {
          this.m_selectedIndex = this.GetScalingIndex(GameManager.Options.CurrentPreferredScalingMode);
        }
        else
        {
          this.m_selectedIndex = 0;
          if (stringList1.Count >= 2 && GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.PIXEL_PERFECT)
            this.m_selectedIndex = 1;
        }
        this.RelocalizeOptions();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.MONITOR_SELECT:
        List<string> stringList2 = new List<string>();
        for (int index = 0; index < Display.displays.Length; ++index)
          stringList2.Add((index + 1).ToString());
        this.labelOptions = stringList2.ToArray();
        break;
      default:
        switch (optionType - 20)
        {
          case BraveOptionsMenuItem.BraveOptionsOptionType.NONE:
            List<string> stringList3 = new List<string>();
            for (int index = 0; index < InputManager.Devices.Count; ++index)
            {
              string name = InputManager.Devices[index].Name;
              int num4 = 1;
              string str;
              for (str = name; stringList3.Contains(str); str = $"{name} {num4.ToString()}")
                ++num4;
              stringList3.Add(str);
            }
            stringList3.Add(this.m_self.ForceGetLocalizedValue("#OPTIONS_KEYBOARDMOUSE"));
            this.labelOptions = stringList3.ToArray();
            break;
          case BraveOptionsMenuItem.BraveOptionsOptionType.UI_VOLUME:
            List<string> stringList4 = new List<string>();
            for (int index = 0; index < InputManager.Devices.Count; ++index)
            {
              string name = InputManager.Devices[index].Name;
              int num5 = 1;
              string str;
              for (str = name; stringList4.Contains(str); str = $"{name} {num5.ToString()}")
                ++num5;
              stringList4.Add(str);
            }
            stringList4.Add(this.m_self.ForceGetLocalizedValue("#OPTIONS_KEYBOARDMOUSE"));
            this.labelOptions = stringList4.ToArray();
            break;
          default:
            switch (optionType - 35)
            {
              case BraveOptionsMenuItem.BraveOptionsOptionType.NONE:
                this.labelOptions = new List<string>()
                {
                  "#LANGUAGE_ENGLISH",
                  "#LANGUAGE_SPANISH",
                  "#LANGUAGE_FRENCH",
                  "#LANGUAGE_ITALIAN",
                  "#LANGUAGE_GERMAN",
                  "#LANGUAGE_PORTUGUESE",
                  "#LANGUAGE_POLISH",
                  "#LANGUAGE_RUSSIAN",
                  "#LANGUAGE_JAPANESE",
                  "#LANGUAGE_KOREAN",
                  "#LANGUAGE_CHINESE"
                }.ToArray();
                this.RelocalizeOptions();
                break;
              case BraveOptionsMenuItem.BraveOptionsOptionType.SOUND_VOLUME:
                if (this.m_quickStartCharacters == null)
                  this.m_quickStartCharacters = new List<GameOptions.QuickstartCharacter>();
                else
                  this.m_quickStartCharacters.Clear();
                List<string> stringList5 = new List<string>(7);
                this.m_quickStartCharacters.Add(GameOptions.QuickstartCharacter.LAST_USED);
                stringList5.Add("#CHAR_LASTUSED");
                this.m_quickStartCharacters.Add(GameOptions.QuickstartCharacter.PILOT);
                stringList5.Add("#CHAR_ROGUE");
                this.m_quickStartCharacters.Add(GameOptions.QuickstartCharacter.CONVICT);
                stringList5.Add("#CHAR_CONVICT");
                this.m_quickStartCharacters.Add(GameOptions.QuickstartCharacter.SOLDIER);
                stringList5.Add("#CHAR_MARINE");
                this.m_quickStartCharacters.Add(GameOptions.QuickstartCharacter.GUIDE);
                stringList5.Add("#CHAR_GUIDE");
                if (GameStatsManager.HasInstance && GameStatsManager.Instance.GetFlag(GungeonFlags.SECRET_BULLETMAN_SEEN_05))
                {
                  this.m_quickStartCharacters.Add(GameOptions.QuickstartCharacter.BULLET);
                  stringList5.Add("#CHAR_BULLET");
                }
                if (GameStatsManager.HasInstance && GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_RECEIVED_BUSTED_TELEVISION))
                {
                  this.m_quickStartCharacters.Add(GameOptions.QuickstartCharacter.ROBOT);
                  stringList5.Add("#CHAR_ROBOT");
                }
                this.labelOptions = stringList5.ToArray();
                this.m_selectedIndex = this.GetQuickStartCharIndex(GameManager.Options.PreferredQuickstartCharacter);
                if (this.m_selectedIndex < 0 || this.m_selectedIndex >= this.labelOptions.Length)
                  this.m_selectedIndex = 0;
                this.UpdateSelectedLabelText();
                break;
            }
            break;
        }
        break;
    }
    if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow || this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
    {
      if (this.m_selectedIndex >= this.labelOptions.Length)
        this.m_selectedIndex = 0;
      if (this.labelOptions != null && this.m_selectedIndex > -1 && this.m_selectedIndex < this.labelOptions.Length)
      {
        this.UpdateSelectedLabelText();
        if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
          this.UpdateInfoControl();
      }
      else
        this.selectedLabelControl.Text = "?";
    }
    if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox)
      this.RepositionCheckboxControl();
    if ((bool) (UnityEngine.Object) this.labelControl)
      this.labelControl.PerformLayout();
    this.UpdateSelectedLabelText();
    this.UpdateInfoControl();
  }

  private void UpdateInfoControl()
  {
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION)
    {
      List<Resolution> availableResolutions = this.GetAvailableResolutions();
      this.m_selectedIndex = Mathf.Clamp(this.m_selectedIndex, 0, availableResolutions.Count - 1);
      int width = availableResolutions[this.m_selectedIndex].width;
      int height = availableResolutions[this.m_selectedIndex].height;
      int num1 = BraveMathCollege.GreatestCommonDivisor(width, height);
      int num2 = width / num1;
      int num3 = height / num1;
      bool flag1 = num2 == 16 /*0x10*/ && num3 == 9;
      bool flag2 = (double) Mathf.Min((float) width / 480f, (float) height / 270f) % 1.0 == 0.0;
      if (flag1 && flag2)
      {
        this.infoControl.Color = (Color32) Color.green;
        this.infoControl.Text = this.infoControl.ForceGetLocalizedValue("#OPTIONS_RESOLUTION_BEST");
      }
      else if (flag2)
      {
        this.infoControl.Color = (Color32) Color.green;
        this.infoControl.Text = this.infoControl.ForceGetLocalizedValue("#OPTIONS_RESOLUTION_GOOD");
      }
      else
      {
        this.infoControl.Color = (Color32) Color.red;
        this.infoControl.Text = this.infoControl.ForceGetLocalizedValue("#OPTIONS_RESOLUTION_BAD");
      }
    }
    else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE)
    {
      if (GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.PIXEL_PERFECT)
      {
        this.infoControl.Color = (Color32) Color.green;
        this.infoControl.Text = this.infoControl.ForceGetLocalizedValue("#OPTIONS_PIXELPERFECT_INFO");
      }
      else if (GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.UNIFORM_SCALING || GameManager.Options.CurrentPreferredScalingMode == GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
      {
        if ((double) Mathf.Min((float) Screen.width / 480f, (float) Screen.height / 270f) % 1.0 == 0.0)
        {
          this.infoControl.Color = (Color32) Color.green;
          this.infoControl.Text = this.infoControl.ForceGetLocalizedValue("#OPTIONS_UNIFORMSCALING_INFOGOOD");
        }
        else
        {
          this.infoControl.Color = (Color32) Color.red;
          this.infoControl.Text = this.infoControl.ForceGetLocalizedValue("#OPTIONS_UNIFORMSCALING_INFOBAD");
        }
      }
      else
      {
        this.infoControl.Color = (Color32) Color.green;
        this.infoControl.Text = this.infoControl.ForceGetLocalizedValue("#OPTIONS_FORCEPIXELPERFECT_INFO");
      }
    }
    this.UpdateInfoControlHeight();
  }

  private int GetScalingIndex(GameOptions.PreferredScalingMode scalingMode)
  {
    for (int index = 0; index < this.m_scalingModes.Count; ++index)
    {
      if (this.m_scalingModes[index] == scalingMode)
        return index;
    }
    return -1;
  }

  private int GetQuickStartCharIndex(GameOptions.QuickstartCharacter quickstartChar)
  {
    for (int index = 0; index < this.m_quickStartCharacters.Count; ++index)
    {
      if (this.m_quickStartCharacters[index] == quickstartChar)
        return index;
    }
    return -1;
  }

  private void HandleResolutionDetermination()
  {
    List<Resolution> availableResolutions = this.GetAvailableResolutions();
    this.labelOptions = new string[availableResolutions.Count];
    this.m_selectedIndex = 0;
    for (int index = 0; index < availableResolutions.Count; ++index)
    {
      int width = availableResolutions[index].width;
      int height = availableResolutions[index].height;
      int num1 = BraveMathCollege.GreatestCommonDivisor(width, height);
      int num2 = width / num1;
      int num3 = height / num1;
      this.labelOptions[index] = $"{width.ToString()} x {height.ToString()} ({(object) num2}:{(object) num3})";
      if (width == Screen.width && height == Screen.height)
        this.m_selectedIndex = index;
    }
  }

  private void HandleFillbarClick(dfControl control, dfMouseEventArgs mouseEvent)
  {
    if (!mouseEvent.Buttons.IsSet(dfMouseButtons.Left))
      return;
    Collider component = control.GetComponent<Collider>();
    RaycastHit hitInfo;
    bool flag = component.Raycast(mouseEvent.Ray, out hitInfo, 1000f);
    Vector2 vector2 = Vector2.zero;
    if (flag)
    {
      vector2 = (Vector2) hitInfo.point;
    }
    else
    {
      float enter;
      if (new Plane(Vector3.back, component.bounds.center.WithZ(0.0f)).Raycast(mouseEvent.Ray, out enter))
        vector2 = BraveMathCollege.ClosestPointOnRectangle((Vector2) mouseEvent.Ray.GetPoint(enter), (Vector2) component.bounds.min, (Vector2) (component.bounds.extents * 2f));
    }
    float num = control.Width * control.transform.localScale.x * control.PixelsToUnits();
    this.m_actualFillbarValue = Mathf.Clamp((vector2.x - (control.transform.position.x - num / 2f)) / num + this.FillbarDelta / 2f, 0.0f, 1f).Quantize(this.FillbarDelta);
    this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
    this.HandleFillbarValueChanged();
    mouseEvent.Use();
  }

  private void HandleFillbarDown(dfControl control, dfMouseEventArgs mouseEvent)
  {
    this.HandleFillbarClick(control, mouseEvent);
  }

  private void HandleFillbarMove(dfControl control, dfMouseEventArgs mouseEvent)
  {
    this.HandleFillbarClick(control, mouseEvent);
  }

  private void HandleFillbarHover(dfControl control, dfMouseEventArgs mouseEvent)
  {
    this.HandleFillbarClick(control, mouseEvent);
  }

  private void DelControl()
  {
    BraveOptionsMenuItem component1 = !((UnityEngine.Object) this.up != (UnityEngine.Object) null) ? (BraveOptionsMenuItem) null : this.up.GetComponent<BraveOptionsMenuItem>();
    BraveOptionsMenuItem component2 = !((UnityEngine.Object) this.down != (UnityEngine.Object) null) ? (BraveOptionsMenuItem) null : this.down.GetComponent<BraveOptionsMenuItem>();
    if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
    {
      component1.down = this.down;
    }
    else
    {
      UIKeyControls component3 = !((UnityEngine.Object) this.up != (UnityEngine.Object) null) ? (UIKeyControls) null : this.up.GetComponent<UIKeyControls>();
      if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
        component3.down = this.down;
    }
    if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
    {
      component2.up = this.up;
    }
    else
    {
      UIKeyControls component4 = !((UnityEngine.Object) this.down != (UnityEngine.Object) null) ? (UIKeyControls) null : this.down.GetComponent<UIKeyControls>();
      if ((UnityEngine.Object) component4 != (UnityEngine.Object) null)
        component4.up = this.up;
    }
    this.m_self.Parent.RemoveControl(this.m_self);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
  }

  [DebuggerHidden]
  public IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BraveOptionsMenuItem.\u003CStart\u003Ec__Iterator1()
    {
      \u0024this = this
    };
  }

  private void UpdateInfoControlHeight()
  {
    if (!(bool) (UnityEngine.Object) this.infoControl)
      return;
    if ((double) this.m_panelStartHeight < 0.0)
      this.m_panelStartHeight = this.GetComponent<dfControl>().Height;
    if ((double) this.m_additionalStartHeight < 0.0)
      this.m_additionalStartHeight = this.infoControl.Height;
    if (Application.platform == RuntimePlatform.PS4 && Application.platform == RuntimePlatform.XboxOne || this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION && this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE)
      return;
    if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.KOREAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
    {
      if (!this.m_infoControlHeightModified)
      {
        this.GetComponent<dfControl>().Height = this.m_panelStartHeight + 30f;
        this.infoControl.Height = this.m_additionalStartHeight + 30f;
        this.infoControl.RelativePosition = this.infoControl.RelativePosition + new Vector3(0.0f, 30f, 0.0f);
        this.m_infoControlHeightModified = true;
      }
    }
    else if (this.m_infoControlHeightModified)
    {
      this.GetComponent<dfControl>().Height = this.m_panelStartHeight;
      this.infoControl.Height = this.m_additionalStartHeight;
      this.infoControl.RelativePosition = this.infoControl.RelativePosition - new Vector3(0.0f, 30f, 0.0f);
      this.m_infoControlHeightModified = false;
    }
    this.infoControl.PerformLayout();
  }

  private void ConvertPivots()
  {
    if ((bool) (UnityEngine.Object) this.labelControl)
      this.labelControl.Pivot = dfPivotPoint.BottomLeft;
    if ((bool) (UnityEngine.Object) this.selectedLabelControl)
      this.selectedLabelControl.Pivot = dfPivotPoint.BottomLeft;
    if ((bool) (UnityEngine.Object) this.infoControl)
      this.infoControl.Pivot = dfPivotPoint.BottomLeft;
    if (!(bool) (UnityEngine.Object) this.buttonControl)
      return;
    this.buttonControl.Pivot = dfPivotPoint.BottomLeft;
  }

  private int GetIndexFromFullscreenMode(GameOptions.PreferredFullscreenMode fMode)
  {
    if (fMode == GameOptions.PreferredFullscreenMode.FULLSCREEN)
      return 0;
    return fMode == GameOptions.PreferredFullscreenMode.BORDERLESS ? 1 : 2;
  }

  public void ForceRefreshDisplayLabel()
  {
    if (!(bool) (UnityEngine.Object) this.selectedLabelControl)
      return;
    this.UpdateSelectedLabelText();
    this.selectedLabelControl.PerformLayout();
  }

  public void InitializeFromOptions()
  {
    switch (this.optionType)
    {
      case BraveOptionsMenuItem.BraveOptionsOptionType.MUSIC_VOLUME:
        this.m_actualFillbarValue = GameManager.Options.MusicVolume / 100f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SOUND_VOLUME:
        this.m_actualFillbarValue = GameManager.Options.SoundVolume / 100f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.UI_VOLUME:
        this.m_actualFillbarValue = GameManager.Options.UIVolume / 100f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SPEAKER_TYPE:
        this.m_selectedIndex = (int) GameManager.Options.AudioHardware;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.VISUAL_PRESET:
        this.InitializeVisualPreset();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.FULLSCREEN:
        this.m_selectedIndex = this.GetIndexFromFullscreenMode(GameManager.Options.CurrentPreferredFullscreenMode);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.MONITOR_SELECT:
        this.m_selectedIndex = GameManager.Options.CurrentMonitorIndex;
        this.DetermineAvailableOptions();
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.VSYNC:
        if (QualitySettings.vSyncCount > 0 && !GameManager.Options.DoVsync)
          GameManager.Options.DoVsync = false;
        this.m_selectedIndex = !GameManager.Options.DoVsync ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.LIGHTING_QUALITY:
        this.m_selectedIndex = GameManager.Options.LightingQuality != GameOptions.GenericHighMedLowOption.HIGH ? 1 : 0;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SHADER_QUALITY:
        switch (GameManager.Options.ShaderQuality)
        {
          case GameOptions.GenericHighMedLowOption.LOW:
            this.m_selectedIndex = 2;
            break;
          case GameOptions.GenericHighMedLowOption.MEDIUM:
            this.m_selectedIndex = 3;
            break;
          case GameOptions.GenericHighMedLowOption.HIGH:
            this.m_selectedIndex = 0;
            break;
          case GameOptions.GenericHighMedLowOption.VERY_LOW:
            this.m_selectedIndex = 1;
            break;
          default:
            this.m_selectedIndex = 0;
            break;
        }
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.DEBRIS_QUANTITY:
        switch (GameManager.Options.DebrisQuantity)
        {
          case GameOptions.GenericHighMedLowOption.LOW:
            this.m_selectedIndex = 2;
            break;
          case GameOptions.GenericHighMedLowOption.MEDIUM:
            this.m_selectedIndex = 3;
            break;
          case GameOptions.GenericHighMedLowOption.HIGH:
            this.m_selectedIndex = 0;
            break;
          case GameOptions.GenericHighMedLowOption.VERY_LOW:
            this.m_selectedIndex = 1;
            break;
          default:
            this.m_selectedIndex = 0;
            break;
        }
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SCREEN_SHAKE_AMOUNT:
        this.m_actualFillbarValue = GameManager.Options.ScreenShakeMultiplier * 0.5f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.STICKY_FRICTION_AMOUNT:
        this.m_actualFillbarValue = GameManager.Options.StickyFrictionMultiplier * 0.8f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.TEXT_SPEED:
        this.m_selectedIndex = GameManager.Options.TextSpeed != GameOptions.GenericHighMedLowOption.MEDIUM ? (GameManager.Options.TextSpeed != GameOptions.GenericHighMedLowOption.LOW ? 1 : 2) : 0;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CONTROLLER_AIM_ASSIST_AMOUNT:
        this.m_actualFillbarValue = GameManager.Options.controllerAimAssistMultiplier * 0.8f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.BEASTMODE:
        this.m_selectedIndex = !GameManager.Options.m_beastmode ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROL_PORT:
        this.m_selectedIndex = !((UnityEngine.Object) GameManager.Instance.PrimaryPlayer == (UnityEngine.Object) null) ? (!GameManager.Options.PlayerIDtoDeviceIndexMap.ContainsKey(GameManager.Instance.PrimaryPlayer.PlayerIDX) ? 0 : GameManager.Options.PlayerIDtoDeviceIndexMap[GameManager.Instance.PrimaryPlayer.PlayerIDX]) : 0;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROLLER_SYMBOLOGY:
        this.m_selectedIndex = (int) GameManager.Options.PlayerOnePreferredSymbology;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROL_PORT:
        if (GameManager.Instance.AllPlayers.Length > 1)
        {
          this.m_selectedIndex = GameManager.Options.PlayerIDtoDeviceIndexMap[GameManager.Instance.SecondaryPlayer.PlayerIDX];
          break;
        }
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROLLER_SYMBOLOGY:
        this.m_selectedIndex = (int) GameManager.Options.PlayerTwoPreferredSymbology;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.MINIMAP_STYLE:
        this.m_selectedIndex = GameManager.Options.MinimapDisplayMode != Minimap.MinimapDisplayMode.FADE_ON_ROOM_SEAL ? (GameManager.Options.MinimapDisplayMode != Minimap.MinimapDisplayMode.ALWAYS ? 1 : 2) : 0;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.COOP_SCREEN_SHAKE_AMOUNT:
        this.m_selectedIndex = !GameManager.Options.CoopScreenShakeReduction ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CONTROLLER_AIM_LOOK:
        this.m_actualFillbarValue = GameManager.Options.controllerAimLookMultiplier * 0.8f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.GAMMA:
        this.m_actualFillbarValue = GameManager.Options.Gamma - 0.5f;
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.REALTIME_REFLECTIONS:
        this.m_selectedIndex = !GameManager.Options.RealtimeReflections ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.QUICKSELECT:
        this.m_selectedIndex = !GameManager.Options.QuickSelectEnabled ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.HIDE_EMPTY_GUNS:
        this.m_selectedIndex = !GameManager.Options.HideEmptyGuns ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.LANGUAGE:
        this.m_selectedIndex = this.LanguageToInt(GameManager.Options.CurrentLanguage);
        if (this.m_selectedIndex >= this.labelOptions.Length)
          this.DetermineAvailableOptions();
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SPEEDRUN:
        this.m_selectedIndex = !GameManager.Options.SpeedrunMode ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.QUICKSTART_CHARACTER:
        if (this.m_quickStartCharacters != null)
        {
          this.m_selectedIndex = this.GetQuickStartCharIndex(GameManager.Options.PreferredQuickstartCharacter);
          if (this.m_selectedIndex < 0 || this.m_selectedIndex >= this.m_quickStartCharacters.Count)
            this.m_selectedIndex = 0;
          this.UpdateSelectedLabelText();
          break;
        }
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL:
        this.m_selectedIndex = (int) GameManager.Options.additionalBlankControl;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL_TWO:
        this.m_selectedIndex = (int) GameManager.Options.additionalBlankControlTwo;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET:
        this.UpdateLabelOptions(0);
        this.m_selectedIndex = Mathf.Clamp((int) GameManager.Options.CurrentControlPreset, 0, this.labelOptions.Length - 1);
        this.selectedLabelControl.PerformLayout();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET_P2:
        this.UpdateLabelOptions(1);
        this.m_selectedIndex = Mathf.Clamp((int) GameManager.Options.CurrentControlPresetP2, 0, this.labelOptions.Length - 1);
        this.selectedLabelControl.PerformLayout();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SAVE_SLOT:
        this.m_selectedIndex = (int) SaveManager.CurrentSaveSlot;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.RUMBLE:
        this.m_selectedIndex = !GameManager.Options.RumbleEnabled ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURSOR_VARIATION:
        this.m_selectedIndex = GameManager.Options.CurrentCursorIndex;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL_PS4:
        this.UpdateLabelOptions(FullOptionsMenuController.CurrentBindingPlayerTargetIndex);
        this.m_selectedIndex = FullOptionsMenuController.CurrentBindingPlayerTargetIndex != 0 ? (int) GameManager.Options.additionalBlankControlTwo : (int) GameManager.Options.additionalBlankControl;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROLLER_CURSOR:
        this.m_selectedIndex = !GameManager.Options.PlayerOneControllerCursor ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROLLER_CURSOR:
        this.m_selectedIndex = !GameManager.Options.PlayerTwoControllerCursor ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ALLOWED_CONTROLLER_TYPES:
        this.m_selectedIndex = !GameManager.Options.allowXinputControllers || !GameManager.Options.allowNonXinputControllers ? (GameManager.Options.allowNonXinputControllers ? 2 : 1) : 0;
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ALLOW_UNKNOWN_CONTROLLERS:
        this.m_selectedIndex = !GameManager.Options.allowUnknownControllers ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SMALL_UI:
        this.m_selectedIndex = !GameManager.Options.SmallUIEnabled ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.BOTH_CONTROLLER_CURSOR:
        this.m_selectedIndex = !GameManager.Options.PlayerOneControllerCursor ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.DISPLAY_SAFE_AREA:
        this.m_actualFillbarValue = (float) (((double) GameManager.Options.DisplaySafeArea - 0.89999997615814209) * 10.0);
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.OUT_OF_COMBAT_SPEED_INCREASE:
        this.m_selectedIndex = !GameManager.Options.IncreaseSpeedOutOfCombat ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CONTROLLER_BEAM_AIM_ASSIST:
        this.m_selectedIndex = !GameManager.Options.controllerBeamAimAssist ? 0 : 1;
        this.HandleCheckboxValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.LOOT_PROFILE:
        switch (GameManager.Options.CurrentGameLootProfile)
        {
          case GameOptions.GameLootProfile.CURRENT:
            this.m_selectedIndex = 0;
            break;
          case GameOptions.GameLootProfile.ORIGINAL:
            this.m_selectedIndex = 1;
            break;
          default:
            this.m_selectedIndex = 0;
            break;
        }
        this.UpdateSelectedLabelText();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.AUTOAIM:
        switch (GameManager.Options.controllerAutoAim)
        {
          case GameOptions.ControllerAutoAim.AUTO_DETECT:
            this.m_selectedIndex = 0;
            break;
          case GameOptions.ControllerAutoAim.ALWAYS:
            this.m_selectedIndex = 1;
            break;
          case GameOptions.ControllerAutoAim.NEVER:
            this.m_selectedIndex = 2;
            break;
          case GameOptions.ControllerAutoAim.COOP_ONLY:
            this.m_selectedIndex = 3;
            break;
        }
        this.UpdateSelectedLabelText();
        break;
    }
    this.DetermineAvailableOptions();
    if (this.itemType != BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox)
      return;
    this.labelControl.TextChanged += (PropertyChangedEventHandler<string>) ((a, b) => this.RepositionCheckboxControl());
    dfLabel labelControl = this.labelControl;
    labelControl.LanguageChanged = labelControl.LanguageChanged + (Action<dfControl>) (a => this.RepositionCheckboxControl());
    this.RepositionCheckboxControl();
  }

  private void RepositionCheckboxControl()
  {
    this.labelControl.AutoSize = true;
    this.labelControl.Parent.GetComponent<dfPanel>().Width = this.checkboxChecked.Width + 21f + this.labelControl.Width;
    this.checkboxChecked.Parent.RelativePosition = this.checkboxChecked.Parent.RelativePosition.WithX(0.0f).WithY(6f);
    this.checkboxChecked.RelativePosition = new Vector3(0.0f, 0.0f, 0.0f);
    this.checkboxUnchecked.RelativePosition = new Vector3(0.0f, 0.0f, 0.0f);
    this.labelControl.RelativePosition = this.labelControl.RelativePosition.WithX(this.checkboxChecked.Width + 21f);
  }

  private void DoFocus(dfControl control, dfFocusEventArgs args)
  {
    if ((UnityEngine.Object) this.labelControl != (UnityEngine.Object) null)
      this.labelControl.Color = (Color32) new Color(1f, 1f, 1f, 1f);
    if ((UnityEngine.Object) this.buttonControl != (UnityEngine.Object) null)
      this.buttonControl.TextColor = (Color32) new Color(1f, 1f, 1f, 1f);
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROL_PORT || this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROL_PORT || this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.ALLOWED_CONTROLLER_TYPES)
    {
      InControlInputAdapter.CurrentlyUsingAllDevices = true;
      InControlInputAdapter.SkipInputForRestOfFrame = true;
    }
    if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow || this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
    {
      this.left.Color = (Color32) new Color(1f, 1f, 1f, 1f);
      this.right.Color = (Color32) new Color(1f, 1f, 1f, 1f);
      this.selectedLabelControl.Color = (Color32) new Color(1f, 1f, 1f, 1f);
    }
    else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox)
    {
      this.checkboxUnchecked.Color = (Color32) new Color(1f, 1f, 1f, 1f);
      this.checkboxChecked.Color = (Color32) new Color(1f, 1f, 1f, 1f);
    }
    if (!(control.Parent is dfScrollPanel))
      return;
    dfScrollPanel parent = control.Parent as dfScrollPanel;
    BraveInput bestInputInstance = this.GetBestInputInstance(GameManager.Instance.LastPausingPlayerID);
    if (!((UnityEngine.Object) bestInputInstance == (UnityEngine.Object) null) && bestInputInstance.ActiveActions != null && !Input.anyKeyDown && !bestInputInstance.ActiveActions.AnyActionPressed())
      return;
    parent.ScrollIntoView(control);
  }

  private void DoArrowBounce(dfControl targetControl)
  {
    this.StartCoroutine(this.HandleArrowBounce(targetControl));
  }

  [DebuggerHidden]
  private IEnumerator HandleArrowBounce(dfControl targetControl)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BraveOptionsMenuItem.\u003CHandleArrowBounce\u003Ec__Iterator2()
    {
      targetControl = targetControl
    };
  }

  private void ArrowReturnScale(dfControl control, dfMouseEventArgs mouseEvent)
  {
    control.transform.localScale = Vector3.one;
  }

  private void ArrowHoverGrow(dfControl control, dfMouseEventArgs mouseEvent)
  {
    control.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
  }

  private void SetUnselectedColors()
  {
    if ((UnityEngine.Object) this.labelControl != (UnityEngine.Object) null)
      this.labelControl.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
    if ((UnityEngine.Object) this.buttonControl != (UnityEngine.Object) null)
      this.buttonControl.TextColor = (Color32) BraveOptionsMenuItem.m_unselectedColor;
    if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow || this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
    {
      this.left.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
      this.right.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
      this.selectedLabelControl.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
    }
    else
    {
      if (this.itemType != BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox)
        return;
      this.checkboxUnchecked.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
      this.checkboxChecked.Color = (Color32) BraveOptionsMenuItem.m_unselectedColor;
    }
  }

  public void LostFocus(dfControl control, dfFocusEventArgs args)
  {
    this.SetUnselectedColors();
    InControlInputAdapter.CurrentlyUsingAllDevices = false;
    if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION)
      this.DoChangeResolution();
    if (this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.SAVE_SLOT || !SaveManager.TargetSaveSlot.HasValue)
      return;
    if (SaveManager.TargetSaveSlot.Value != SaveManager.CurrentSaveSlot)
      this.AskToChangeSaveSlot();
    else
      SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?();
  }

  private void AskToChangeSaveSlot()
  {
    FullOptionsMenuController OptionsMenu = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu;
    OptionsMenu.MainPanel.IsVisible = false;
    GameUIRoot.Instance.DoAreYouSure("#AYS_CHANGESAVESLOT");
    this.StartCoroutine(this.WaitForAreYouSure(new System.Action(this.ChangeSaveSlot), (System.Action) (() =>
    {
      this.m_selectedIndex = (int) SaveManager.CurrentSaveSlot;
      this.HandleValueChanged();
      SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?();
      OptionsMenu.MainPanel.IsVisible = true;
      if ((bool) (UnityEngine.Object) this.up)
        this.up.GetComponent<BraveOptionsMenuItem>().LostFocus((dfControl) null, (dfFocusEventArgs) null);
      if ((bool) (UnityEngine.Object) this.down)
      {
        BraveOptionsMenuItem component = this.down.GetComponent<BraveOptionsMenuItem>();
        if ((bool) (UnityEngine.Object) component)
        {
          component.LostFocus((dfControl) null, (dfFocusEventArgs) null);
        }
        else
        {
          this.down.Focus(true);
          this.down.Unfocus();
        }
      }
      this.m_self.Focus(true);
    })));
  }

  private void ChangeSaveSlot() => GameManager.Instance.LoadMainMenu();

  [DebuggerHidden]
  private IEnumerator WaitForAreYouSure(System.Action OnYes, System.Action OnNo)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BraveOptionsMenuItem.\u003CWaitForAreYouSure\u003Ec__Iterator3()
    {
      OnYes = OnYes,
      OnNo = OnNo
    };
  }

  private void DoSelectedAction()
  {
    if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox)
      this.ToggleCheckbox((dfControl) null, (dfMouseEventArgs) null);
    else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Button)
    {
      FullOptionsMenuController OptionsMenu = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu;
      if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.EDIT_KEYBOARD_BINDINGS)
      {
        FullOptionsMenuController.CurrentBindingPlayerTargetIndex = 0;
        OptionsMenu.ToggleToKeyboardBindingsPanel(false);
      }
      else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROLLER_BINDINGS)
      {
        FullOptionsMenuController.CurrentBindingPlayerTargetIndex = 0;
        OptionsMenu.ToggleToKeyboardBindingsPanel(true);
      }
      else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROLLER_BINDINGS)
      {
        FullOptionsMenuController.CurrentBindingPlayerTargetIndex = GameManager.Instance.AllPlayers.Length <= 1 ? 0 : GameManager.Instance.SecondaryPlayer.PlayerIDX;
        OptionsMenu.ToggleToKeyboardBindingsPanel(true);
      }
      else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.VIEW_CREDITS)
        OptionsMenu.ToggleToCredits();
      else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.VIEW_PRIVACY)
        DataPrivacy.FetchPrivacyUrl((Action<string>) (url => Application.OpenURL(url)));
      else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.HOW_TO_PLAY)
      {
        OptionsMenu.ToggleToHowToPlay();
      }
      else
      {
        if (this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.RESET_SAVE_SLOT)
          return;
        OptionsMenu.MainPanel.IsVisible = false;
        GameUIRoot.Instance.DoAreYouSure("#AYS_RESETSAVESLOT", secondaryKey: "#AYS_RESETSAVESLOT2");
        this.StartCoroutine(this.WaitForAreYouSure((System.Action) (() =>
        {
          GameUIRoot.Instance.DoAreYouSure("#AREYOUSURE");
          this.StartCoroutine(this.WaitForAreYouSure((System.Action) (() =>
          {
            SaveManager.ResetSaveSlot = true;
            GameManager.Instance.LoadMainMenu();
          }), (System.Action) (() =>
          {
            OptionsMenu.MainPanel.IsVisible = true;
            this.m_self.Focus(true);
          })));
        }), (System.Action) (() =>
        {
          OptionsMenu.MainPanel.IsVisible = true;
          this.m_self.Focus(true);
        })));
      }
    }
    else if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION)
    {
      this.DoChangeResolution();
    }
    else
    {
      if (this.optionType != BraveOptionsMenuItem.BraveOptionsOptionType.SAVE_SLOT || !SaveManager.TargetSaveSlot.HasValue || SaveManager.TargetSaveSlot.Value == SaveManager.CurrentSaveSlot)
        return;
      this.AskToChangeSaveSlot();
    }
  }

  private void IncrementArrow(dfControl control, dfMouseEventArgs mouseEvent)
  {
    int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
    this.m_selectedIndex = (this.m_selectedIndex + 1) % this.labelOptions.Length;
    this.HandleValueChanged();
  }

  private void DecrementArrow(dfControl control, dfMouseEventArgs mouseEvent)
  {
    int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
    this.m_selectedIndex = (this.m_selectedIndex - 1 + this.labelOptions.Length) % this.labelOptions.Length;
    this.HandleValueChanged();
  }

  private void HandleValueChanged()
  {
    switch (this.itemType)
    {
      case BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow:
        this.HandleLeftRightArrowValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo:
        this.HandleLeftRightArrowValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsMenuItemType.Fillbar:
        this.HandleFillbarValueChanged();
        break;
      case BraveOptionsMenuItem.BraveOptionsMenuItemType.Checkbox:
        this.HandleCheckboxValueChanged();
        break;
    }
    if ((bool) (UnityEngine.Object) this.selectedLabelControl)
      this.selectedLabelControl.PerformLayout();
    if (!(bool) (UnityEngine.Object) this.infoControl)
      return;
    this.infoControl.PerformLayout();
  }

  private void HandleCheckboxValueChanged()
  {
    this.checkboxChecked.IsVisible = this.m_selectedIndex == 1;
    this.checkboxUnchecked.IsVisible = this.m_selectedIndex != 1;
    BraveOptionsMenuItem.BraveOptionsOptionType optionType = this.optionType;
    switch (optionType)
    {
      case BraveOptionsMenuItem.BraveOptionsOptionType.RUMBLE:
        GameManager.Options.RumbleEnabled = this.m_selectedIndex == 1;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROLLER_CURSOR:
        GameManager.Options.PlayerOneControllerCursor = this.m_selectedIndex == 1;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROLLER_CURSOR:
        GameManager.Options.PlayerTwoControllerCursor = this.m_selectedIndex == 1;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ALLOW_UNKNOWN_CONTROLLERS:
        GameManager.Options.allowUnknownControllers = this.m_selectedIndex == 1;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SMALL_UI:
        GameManager.Options.SmallUIEnabled = this.m_selectedIndex == 1;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.BOTH_CONTROLLER_CURSOR:
        GameManager.Options.PlayerOneControllerCursor = this.m_selectedIndex == 1;
        GameManager.Options.PlayerTwoControllerCursor = this.m_selectedIndex == 1;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.OUT_OF_COMBAT_SPEED_INCREASE:
        GameManager.Options.IncreaseSpeedOutOfCombat = this.m_selectedIndex == 1;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CONTROLLER_BEAM_AIM_ASSIST:
        GameManager.Options.controllerBeamAimAssist = this.m_selectedIndex == 1;
        break;
      default:
        switch (optionType - 28)
        {
          case BraveOptionsMenuItem.BraveOptionsOptionType.NONE:
            GameManager.Options.CoopScreenShakeReduction = this.m_selectedIndex == 1;
            return;
          case BraveOptionsMenuItem.BraveOptionsOptionType.UI_VOLUME:
            GameManager.Options.RealtimeReflections = this.m_selectedIndex == 1;
            return;
          case BraveOptionsMenuItem.BraveOptionsOptionType.SPEAKER_TYPE:
            GameManager.Options.QuickSelectEnabled = this.m_selectedIndex == 1;
            return;
          case BraveOptionsMenuItem.BraveOptionsOptionType.VISUAL_PRESET:
            GameManager.Options.HideEmptyGuns = this.m_selectedIndex == 1;
            return;
          case BraveOptionsMenuItem.BraveOptionsOptionType.FULLSCREEN:
            GameManager.Options.SpeedrunMode = this.m_selectedIndex == 1;
            return;
          default:
            if (optionType != BraveOptionsMenuItem.BraveOptionsOptionType.VSYNC)
            {
              if (optionType != BraveOptionsMenuItem.BraveOptionsOptionType.BEASTMODE)
                return;
              GameManager.Options.m_beastmode = this.m_selectedIndex == 1;
              return;
            }
            GameManager.Options.DoVsync = this.m_selectedIndex == 1;
            return;
        }
    }
  }

  private List<Resolution> GetAvailableResolutions()
  {
    if (GameManager.Options.CurrentPreferredFullscreenMode == GameOptions.PreferredFullscreenMode.BORDERLESS)
      return new List<Resolution>((IEnumerable<Resolution>) new Resolution[1]
      {
        Screen.currentResolution
      });
    List<Resolution> resolutions1 = new List<Resolution>();
    Resolution[] resolutions2 = Screen.resolutions;
    int refreshRate = Screen.currentResolution.refreshRate;
    for (int index = 0; index < resolutions2.Length; ++index)
    {
      Resolution resolution = resolutions2[index];
      this.AddResolutionInOrder(resolutions1, new Resolution()
      {
        width = resolution.width,
        height = resolution.height,
        refreshRate = refreshRate
      });
    }
    if (GameManager.Options.CurrentPreferredFullscreenMode == GameOptions.PreferredFullscreenMode.WINDOWED || Application.platform == RuntimePlatform.OSXPlayer)
      this.AddResolutionInOrder(resolutions1, new Resolution()
      {
        width = Screen.width,
        height = Screen.height,
        refreshRate = Screen.currentResolution.refreshRate
      });
    if (GameManager.Options.CurrentPreferredFullscreenMode == GameOptions.PreferredFullscreenMode.WINDOWED)
    {
      int num1 = 0;
      int num2 = 0;
      if (resolutions1.Count > 0)
      {
        num1 = resolutions1[resolutions1.Count - 1].width;
        num2 = resolutions1[resolutions1.Count - 1].height;
      }
      int num3 = 480;
      for (int index = 270; num3 <= num1 && index <= num2; index += 270)
      {
        this.AddResolutionInOrder(resolutions1, new Resolution()
        {
          width = num3,
          height = index,
          refreshRate = refreshRate
        });
        num3 += 480;
      }
    }
    return resolutions1;
  }

  private void AddResolutionInOrder(List<Resolution> resolutions, Resolution newResolution)
  {
    for (int index = 0; index < resolutions.Count; ++index)
    {
      if (resolutions[index].width == newResolution.width && resolutions[index].height == newResolution.height)
        return;
      if (resolutions[index].width > newResolution.width || resolutions[index].width == newResolution.width && resolutions[index].height > newResolution.height)
      {
        resolutions.Insert(index, newResolution);
        return;
      }
    }
    resolutions.Add(newResolution);
  }

  private void DoChangeResolution()
  {
    List<Resolution> availableResolutions = this.GetAvailableResolutions();
    this.m_selectedIndex = Mathf.Clamp(this.m_selectedIndex, 0, availableResolutions.Count - 1);
    if (availableResolutions[this.m_selectedIndex].width == Screen.width && availableResolutions[this.m_selectedIndex].height == Screen.height)
      return;
    GameManager.Options.CurrentVisualPreset = GameOptions.VisualPresetMode.CUSTOM;
    BraveOptionsMenuItem.HandleScreenDataChanged(availableResolutions[this.m_selectedIndex].width, availableResolutions[this.m_selectedIndex].height);
  }

  private BraveInput GetBestInputInstance(int targetPlayerIndex)
  {
    return GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER || !Foyer.DoMainMenu ? (targetPlayerIndex != -1 ? BraveInput.GetInstanceForPlayer(targetPlayerIndex) : BraveInput.PrimaryPlayerInstance) : BraveInput.PlayerlessInstance;
  }

  private void HandleLeftRightArrowValueChanged()
  {
    this.UpdateSelectedLabelText();
    BraveOptionsMenuItem.BraveOptionsOptionType optionType = this.optionType;
    switch (optionType)
    {
      case BraveOptionsMenuItem.BraveOptionsOptionType.SPEAKER_TYPE:
        GameManager.Options.AudioHardware = (GameOptions.AudioHardwareMode) this.m_selectedIndex;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.VISUAL_PRESET:
        GameManager.Options.CurrentVisualPreset = (GameOptions.VisualPresetMode) this.m_selectedIndex;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION:
        if (this.itemType != BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
          break;
        this.UpdateInfoControl();
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SCALING_MODE:
        GameManager.Options.CurrentVisualPreset = GameOptions.VisualPresetMode.CUSTOM;
        if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.PIXEL_PERFECT)
          GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT;
        else if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.UNIFORM_SCALING)
          GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.UNIFORM_SCALING;
        else if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST)
          GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.UNIFORM_SCALING_FAST;
        else if (this.m_scalingModes[this.m_selectedIndex] == GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT)
          GameManager.Options.CurrentPreferredScalingMode = GameOptions.PreferredScalingMode.FORCE_PIXEL_PERFECT;
        BraveOptionsMenuItem.HandleScreenDataChanged(Screen.width, Screen.height);
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.FULLSCREEN:
        GameManager.Options.CurrentVisualPreset = GameOptions.VisualPresetMode.CUSTOM;
        GameManager.Options.CurrentPreferredFullscreenMode = this.m_selectedIndex != 0 ? (this.m_selectedIndex != 1 ? GameOptions.PreferredFullscreenMode.WINDOWED : GameOptions.PreferredFullscreenMode.BORDERLESS) : GameOptions.PreferredFullscreenMode.FULLSCREEN;
        BraveOptionsMenuItem.HandleScreenDataChanged(Screen.width, Screen.height);
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.MONITOR_SELECT:
        GameManager.Options.CurrentMonitorIndex = this.m_selectedIndex;
        PlayerPrefs.SetInt("UnitySelectMonitor", this.m_selectedIndex);
        this.DoChangeResolution();
        Resolution recommendedResolution = GameManager.Options.GetRecommendedResolution();
        if (Screen.width != recommendedResolution.width || Screen.height != recommendedResolution.height)
        {
          BraveOptionsMenuItem.HandleScreenDataChanged(recommendedResolution.width, recommendedResolution.height);
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.LIGHTING_QUALITY:
        GameManager.Options.LightingQuality = this.m_selectedIndex != 0 ? GameOptions.GenericHighMedLowOption.LOW : GameOptions.GenericHighMedLowOption.HIGH;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SHADER_QUALITY:
        if (this.m_selectedIndex == 0)
          GameManager.Options.ShaderQuality = GameOptions.GenericHighMedLowOption.HIGH;
        if (this.m_selectedIndex == 1)
          GameManager.Options.ShaderQuality = GameOptions.GenericHighMedLowOption.VERY_LOW;
        if (this.m_selectedIndex == 2)
          GameManager.Options.ShaderQuality = GameOptions.GenericHighMedLowOption.LOW;
        if (this.m_selectedIndex == 3)
          GameManager.Options.ShaderQuality = GameOptions.GenericHighMedLowOption.MEDIUM;
        if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW)
        {
          GameManager.Options.RealtimeReflections = false;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.DEBRIS_QUANTITY:
        if (this.m_selectedIndex == 0)
          GameManager.Options.DebrisQuantity = GameOptions.GenericHighMedLowOption.HIGH;
        if (this.m_selectedIndex == 1)
          GameManager.Options.DebrisQuantity = GameOptions.GenericHighMedLowOption.VERY_LOW;
        if (this.m_selectedIndex == 2)
          GameManager.Options.DebrisQuantity = GameOptions.GenericHighMedLowOption.LOW;
        if (this.m_selectedIndex == 3)
        {
          GameManager.Options.DebrisQuantity = GameOptions.GenericHighMedLowOption.MEDIUM;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.TEXT_SPEED:
        if (this.m_selectedIndex == 0)
          GameManager.Options.TextSpeed = GameOptions.GenericHighMedLowOption.MEDIUM;
        if (this.m_selectedIndex == 1)
          GameManager.Options.TextSpeed = GameOptions.GenericHighMedLowOption.HIGH;
        if (this.m_selectedIndex == 2)
        {
          GameManager.Options.TextSpeed = GameOptions.GenericHighMedLowOption.LOW;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROL_PORT:
        BraveInput.ReassignPlayerPort(0, this.m_selectedIndex);
        this.m_ignoreLeftRightUntilReleased = true;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_ONE_CONTROLLER_SYMBOLOGY:
        if (this.m_selectedIndex == 0)
          GameManager.Options.PlayerOnePreferredSymbology = GameOptions.ControllerSymbology.PS4;
        if (this.m_selectedIndex == 1)
          GameManager.Options.PlayerOnePreferredSymbology = GameOptions.ControllerSymbology.Xbox;
        if (this.m_selectedIndex == 2)
          GameManager.Options.PlayerOnePreferredSymbology = GameOptions.ControllerSymbology.AutoDetect;
        if (this.m_selectedIndex == 3)
        {
          GameManager.Options.PlayerOnePreferredSymbology = GameOptions.ControllerSymbology.Switch;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROL_PORT:
        if (GameManager.Instance.AllPlayers.Length > 1)
        {
          BraveInput.ReassignPlayerPort(GameManager.Instance.SecondaryPlayer.PlayerIDX, this.m_selectedIndex);
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.PLAYER_TWO_CONTROLLER_SYMBOLOGY:
        if (this.m_selectedIndex == 0)
          GameManager.Options.PlayerTwoPreferredSymbology = GameOptions.ControllerSymbology.PS4;
        if (this.m_selectedIndex == 1)
          GameManager.Options.PlayerTwoPreferredSymbology = GameOptions.ControllerSymbology.Xbox;
        if (this.m_selectedIndex == 2)
          GameManager.Options.PlayerTwoPreferredSymbology = GameOptions.ControllerSymbology.AutoDetect;
        if (this.m_selectedIndex == 3)
        {
          GameManager.Options.PlayerOnePreferredSymbology = GameOptions.ControllerSymbology.Switch;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.MINIMAP_STYLE:
        if (this.m_selectedIndex == 0)
          GameManager.Options.MinimapDisplayMode = Minimap.MinimapDisplayMode.FADE_ON_ROOM_SEAL;
        if (this.m_selectedIndex == 1)
          GameManager.Options.MinimapDisplayMode = Minimap.MinimapDisplayMode.NEVER;
        if (this.m_selectedIndex == 2)
        {
          GameManager.Options.MinimapDisplayMode = Minimap.MinimapDisplayMode.ALWAYS;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.LANGUAGE:
        GameManager.Options.CurrentLanguage = this.IntToLanguage(this.m_selectedIndex);
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.QUICKSTART_CHARACTER:
        GameManager.Options.PreferredQuickstartCharacter = this.m_selectedIndex < 0 || this.m_selectedIndex >= this.m_quickStartCharacters.Count ? GameOptions.QuickstartCharacter.LAST_USED : this.m_quickStartCharacters[this.m_selectedIndex];
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL:
        GameManager.Options.additionalBlankControl = (GameOptions.ControllerBlankControl) this.m_selectedIndex;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL_TWO:
        GameManager.Options.additionalBlankControlTwo = (GameOptions.ControllerBlankControl) this.m_selectedIndex;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET:
        FullOptionsMenuController optionsMenu1 = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu;
        GameManager.Options.CurrentControlPreset = (GameOptions.ControlPreset) this.m_selectedIndex;
        optionsMenu1.ReinitializeKeyboardBindings();
        this.selectedLabelControl.PerformLayout();
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET_P2:
        FullOptionsMenuController optionsMenu2 = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu;
        GameManager.Options.CurrentControlPresetP2 = (GameOptions.ControlPreset) this.m_selectedIndex;
        optionsMenu2.ReinitializeKeyboardBindings();
        this.selectedLabelControl.PerformLayout();
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SAVE_SLOT:
        if (this.m_selectedIndex == 0)
          SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?(SaveManager.SaveSlot.A);
        if (this.m_selectedIndex == 1)
          SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?(SaveManager.SaveSlot.B);
        if (this.m_selectedIndex == 2)
          SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?(SaveManager.SaveSlot.C);
        if (this.m_selectedIndex == 3)
        {
          SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?(SaveManager.SaveSlot.D);
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURSOR_VARIATION:
        GameManager.Options.CurrentCursorIndex = this.m_selectedIndex;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL_PS4:
        if (FullOptionsMenuController.CurrentBindingPlayerTargetIndex == 0)
        {
          GameManager.Options.additionalBlankControl = (GameOptions.ControllerBlankControl) this.m_selectedIndex;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        GameManager.Options.additionalBlankControlTwo = (GameOptions.ControllerBlankControl) this.m_selectedIndex;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ALLOWED_CONTROLLER_TYPES:
        if (this.m_selectedIndex == 0)
        {
          GameManager.Options.allowXinputControllers = true;
          GameManager.Options.allowNonXinputControllers = true;
        }
        if (this.m_selectedIndex == 1)
        {
          GameManager.Options.allowXinputControllers = true;
          GameManager.Options.allowNonXinputControllers = false;
        }
        if (this.m_selectedIndex == 2)
        {
          GameManager.Options.allowXinputControllers = false;
          GameManager.Options.allowNonXinputControllers = true;
        }
        InControlInputAdapter.SkipInputForRestOfFrame = true;
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
      default:
        if (optionType != BraveOptionsMenuItem.BraveOptionsOptionType.LOOT_PROFILE)
        {
          if (optionType == BraveOptionsMenuItem.BraveOptionsOptionType.AUTOAIM)
          {
            if (this.m_selectedIndex == 0)
              GameManager.Options.controllerAutoAim = GameOptions.ControllerAutoAim.AUTO_DETECT;
            if (this.m_selectedIndex == 1)
              GameManager.Options.controllerAutoAim = GameOptions.ControllerAutoAim.ALWAYS;
            if (this.m_selectedIndex == 2)
              GameManager.Options.controllerAutoAim = GameOptions.ControllerAutoAim.NEVER;
            if (this.m_selectedIndex == 3)
            {
              GameManager.Options.controllerAutoAim = GameOptions.ControllerAutoAim.COOP_ONLY;
              goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
            }
            goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
          }
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        if (this.m_selectedIndex == 0)
          GameManager.Options.CurrentGameLootProfile = GameOptions.GameLootProfile.CURRENT;
        if (this.m_selectedIndex == 1)
        {
          GameManager.Options.CurrentGameLootProfile = GameOptions.GameLootProfile.ORIGINAL;
          goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
        }
        goto case BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION;
    }
  }

  private static void HandleScreenDataChanged(int screenWidth, int screenHeight)
  {
    if (GameManager.Instance.IsLoadingLevel || GameManager.IsReturningToBreach)
      return;
    GameOptions.PreferredFullscreenMode preferredFullscreenMode = GameManager.Options.CurrentPreferredFullscreenMode;
    Resolution resolution = new Resolution();
    if (preferredFullscreenMode == GameOptions.PreferredFullscreenMode.BORDERLESS)
    {
      screenWidth = Screen.currentResolution.width;
      screenHeight = Screen.currentResolution.height;
    }
    GameManager.Options.preferredResolutionX = screenWidth;
    GameManager.Options.preferredResolutionY = screenHeight;
    resolution.width = screenWidth;
    resolution.height = screenHeight;
    resolution.refreshRate = Screen.currentResolution.refreshRate;
    BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes targetDisplayMode = BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Fullscreen;
    if (preferredFullscreenMode == GameOptions.PreferredFullscreenMode.BORDERLESS)
      targetDisplayMode = BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Borderless;
    if (preferredFullscreenMode == GameOptions.PreferredFullscreenMode.WINDOWED)
      targetDisplayMode = BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Windowed;
    if (Screen.fullScreen != (preferredFullscreenMode == GameOptions.PreferredFullscreenMode.FULLSCREEN))
    {
      BraveOptionsMenuItem componentInChildren = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.GetComponentInChildren<BraveOptionsMenuItem>();
      componentInChildren.StartCoroutine(componentInChildren.FrameDelayedWindowsShift(targetDisplayMode, resolution));
    }
    else
      BraveOptionsMenuItem.ResolutionManagerWin.TrySetDisplay(targetDisplayMode, resolution, false, new Position?());
    if (screenWidth == Screen.width && screenHeight == Screen.height && preferredFullscreenMode == GameOptions.PreferredFullscreenMode.FULLSCREEN == Screen.fullScreen)
      return;
    UnityEngine.Debug.Log((object) $"BOMI setting resolution to: {(object) screenWidth}|{(object) screenHeight}||{preferredFullscreenMode.ToString()}");
    GameManager.Instance.DoSetResolution(screenWidth, screenHeight, preferredFullscreenMode == GameOptions.PreferredFullscreenMode.FULLSCREEN);
  }

  [DebuggerHidden]
  public IEnumerator FrameDelayedWindowsShift(
    BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes targetDisplayMode,
    Resolution targetRes)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BraveOptionsMenuItem.\u003CFrameDelayedWindowsShift\u003Ec__Iterator4()
    {
      targetDisplayMode = targetDisplayMode,
      targetRes = targetRes
    };
  }

  private void HandleFillbarValueChanged()
  {
    BraveOptionsMenuItem.BraveOptionsOptionType optionType = this.optionType;
    switch (optionType)
    {
      case BraveOptionsMenuItem.BraveOptionsOptionType.MUSIC_VOLUME:
        GameManager.Options.MusicVolume = Mathf.Clamp(this.m_actualFillbarValue * 100f, 0.0f, 100f);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SOUND_VOLUME:
        GameManager.Options.SoundVolume = Mathf.Clamp(this.m_actualFillbarValue * 100f, 0.0f, 100f);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.UI_VOLUME:
        GameManager.Options.UIVolume = Mathf.Clamp(this.m_actualFillbarValue * 100f, 0.0f, 100f);
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.SCREEN_SHAKE_AMOUNT:
        GameManager.Options.ScreenShakeMultiplier = this.m_actualFillbarValue / 0.5f;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.STICKY_FRICTION_AMOUNT:
        GameManager.Options.StickyFrictionMultiplier = this.m_actualFillbarValue / 0.8f;
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.CONTROLLER_AIM_ASSIST_AMOUNT:
        GameManager.Options.controllerAimAssistMultiplier = Mathf.Clamp(this.m_actualFillbarValue / 0.8f, 0.0f, 1.25f);
        break;
      default:
        if (optionType != BraveOptionsMenuItem.BraveOptionsOptionType.CONTROLLER_AIM_LOOK)
        {
          if (optionType != BraveOptionsMenuItem.BraveOptionsOptionType.GAMMA)
          {
            if (optionType != BraveOptionsMenuItem.BraveOptionsOptionType.DISPLAY_SAFE_AREA)
              break;
            GameManager.Options.DisplaySafeArea = Mathf.Clamp((float) ((double) BraveMathCollege.QuantizeFloat(this.m_actualFillbarValue, 0.2f) * 0.10000000149011612 + 0.89999997615814209), 0.9f, 1f);
            break;
          }
          GameManager.Options.Gamma = Mathf.Clamp(this.m_actualFillbarValue + 0.5f, 0.5f, 1.5f);
          break;
        }
        GameManager.Options.controllerAimLookMultiplier = Mathf.Clamp(this.m_actualFillbarValue / 0.8f, 0.0f, 1.25f);
        break;
    }
  }

  public void OnKeyUp(dfControl sender, dfKeyEventArgs args)
  {
    if (args.Used)
      return;
    if (args.KeyCode == KeyCode.LeftArrow)
    {
      this.m_ignoreLeftRightUntilReleased = false;
    }
    else
    {
      if (args.KeyCode != KeyCode.RightArrow)
        return;
      this.m_ignoreLeftRightUntilReleased = false;
    }
  }

  private float FillbarDelta
  {
    get
    {
      return this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.DISPLAY_SAFE_AREA ? 0.2f : 0.05f;
    }
  }

  public void OnKeyDown(dfControl sender, dfKeyEventArgs args)
  {
    if (args.Used)
      return;
    if (args.KeyCode == KeyCode.UpArrow && (bool) (UnityEngine.Object) this.up)
    {
      if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION)
        this.DoChangeResolution();
      if (this.OnNewControlSelected != null)
        this.OnNewControlSelected(this.up);
      this.up.Focus(true);
    }
    else if (args.KeyCode == KeyCode.DownArrow && (bool) (UnityEngine.Object) this.down)
    {
      if (this.optionType == BraveOptionsMenuItem.BraveOptionsOptionType.RESOLUTION)
        this.DoChangeResolution();
      if (this.OnNewControlSelected != null)
        this.OnNewControlSelected(this.down);
      this.down.Focus(true);
    }
    else if (args.KeyCode == KeyCode.LeftArrow)
    {
      if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Fillbar)
      {
        this.m_actualFillbarValue = Mathf.Clamp01(this.m_actualFillbarValue - this.FillbarDelta);
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        this.HandleValueChanged();
      }
      else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow)
      {
        if (!this.m_ignoreLeftRightUntilReleased)
        {
          this.DecrementArrow((dfControl) null, (dfMouseEventArgs) null);
          this.DoArrowBounce(this.left);
        }
      }
      else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
      {
        if (!this.m_ignoreLeftRightUntilReleased)
        {
          this.DecrementArrow((dfControl) null, (dfMouseEventArgs) null);
          this.DoArrowBounce(this.left);
        }
      }
      else if ((bool) (UnityEngine.Object) this.left)
      {
        if (this.OnNewControlSelected != null)
          this.OnNewControlSelected(this.left);
        this.left.Focus(true);
      }
    }
    else if (args.KeyCode == KeyCode.RightArrow)
    {
      if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.Fillbar)
      {
        this.m_actualFillbarValue = Mathf.Clamp01(this.m_actualFillbarValue + this.FillbarDelta);
        this.fillbarControl.Value = (float) ((double) this.m_actualFillbarValue * 0.98000001907348633 + 0.0099999997764825821);
        this.HandleValueChanged();
      }
      else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrow)
      {
        if (!this.m_ignoreLeftRightUntilReleased)
        {
          this.IncrementArrow((dfControl) null, (dfMouseEventArgs) null);
          this.DoArrowBounce(this.right);
        }
      }
      else if (this.itemType == BraveOptionsMenuItem.BraveOptionsMenuItemType.LeftRightArrowInfo)
      {
        if (!this.m_ignoreLeftRightUntilReleased)
        {
          this.IncrementArrow((dfControl) null, (dfMouseEventArgs) null);
          this.DoArrowBounce(this.right);
        }
      }
      else if ((bool) (UnityEngine.Object) this.right)
      {
        if (this.OnNewControlSelected != null)
          this.OnNewControlSelected(this.right);
        this.right.Focus(true);
      }
    }
    if (!this.selectOnAction || args.KeyCode != KeyCode.Return)
      return;
    this.DoSelectedAction();
    args.Use();
  }

  public void UpdateLabelOptions(int playerIndex)
  {
    switch (this.optionType)
    {
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET:
      case BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET_P2:
        this.labelOptions = new string[3]
        {
          this.selectedLabelControl.ForceGetLocalizedValue("#OPTIONS_RECOMMENDED") + " 1",
          this.selectedLabelControl.ForceGetLocalizedValue("#OPTIONS_RECOMMENDED") + " 2",
          this.selectedLabelControl.ForceGetLocalizedValue("#OPTIONS_CUSTOM")
        };
        if (GameManager.Options.CurrentLanguage != StringTableManager.GungeonSupportedLanguages.ENGLISH)
          break;
        this.labelOptions[0] = "Recommended";
        this.labelOptions[1] = "Flipped Triggers";
        break;
      case BraveOptionsMenuItem.BraveOptionsOptionType.ADDITIONAL_BLANK_CONTROL_PS4:
        this.labelOptions = new string[2]
        {
          this.selectedLabelControl.ForceGetLocalizedValue("#OPTIONS_NONE"),
          "%CONTROL_L_STICK_DOWN %CONTROL_R_STICK_DOWN"
        };
        break;
    }
  }

  public enum BraveOptionsMenuItemType
  {
    LeftRightArrow,
    LeftRightArrowInfo,
    Fillbar,
    Checkbox,
    Button,
  }

  public enum BraveOptionsOptionType
  {
    NONE,
    MUSIC_VOLUME,
    SOUND_VOLUME,
    UI_VOLUME,
    SPEAKER_TYPE,
    VISUAL_PRESET,
    RESOLUTION,
    SCALING_MODE,
    FULLSCREEN,
    MONITOR_SELECT,
    VSYNC,
    LIGHTING_QUALITY,
    SHADER_QUALITY,
    DEBRIS_QUANTITY,
    SCREEN_SHAKE_AMOUNT,
    STICKY_FRICTION_AMOUNT,
    TEXT_SPEED,
    CONTROLLER_AIM_ASSIST_AMOUNT,
    BEASTMODE,
    EDIT_KEYBOARD_BINDINGS,
    PLAYER_ONE_CONTROL_PORT,
    PLAYER_ONE_CONTROLLER_SYMBOLOGY,
    PLAYER_ONE_CONTROLLER_BINDINGS,
    PLAYER_TWO_CONTROL_PORT,
    PLAYER_TWO_CONTROLLER_SYMBOLOGY,
    PLAYER_TWO_CONTROLLER_BINDINGS,
    VIEW_CREDITS,
    MINIMAP_STYLE,
    COOP_SCREEN_SHAKE_AMOUNT,
    CONTROLLER_AIM_LOOK,
    GAMMA,
    REALTIME_REFLECTIONS,
    QUICKSELECT,
    HIDE_EMPTY_GUNS,
    HOW_TO_PLAY,
    LANGUAGE,
    SPEEDRUN,
    QUICKSTART_CHARACTER,
    ADDITIONAL_BLANK_CONTROL,
    ADDITIONAL_BLANK_CONTROL_TWO,
    CURRENT_BINDINGS_PRESET,
    CURRENT_BINDINGS_PRESET_P2,
    SAVE_SLOT,
    RESET_SAVE_SLOT,
    RUMBLE,
    CURSOR_VARIATION,
    ADDITIONAL_BLANK_CONTROL_PS4,
    PLAYER_ONE_CONTROLLER_CURSOR,
    PLAYER_TWO_CONTROLLER_CURSOR,
    ALLOWED_CONTROLLER_TYPES,
    ALLOW_UNKNOWN_CONTROLLERS,
    SMALL_UI,
    BOTH_CONTROLLER_CURSOR,
    DISPLAY_SAFE_AREA,
    GAMEPLAY_SPEED,
    OUT_OF_COMBAT_SPEED_INCREASE,
    CONTROLLER_BEAM_AIM_ASSIST,
    SWITCH_PERFORMANCE_MODE,
    SWITCH_REASSIGN_CONTROLLERS,
    LOOT_PROFILE,
    AUTOAIM,
    VIEW_PRIVACY,
  }

  public class WindowsResolutionManager
  {
    private string _title;
    private int _borderWidth;
    private int _captionHeight;
    private const int WS_BORDER = 8388608 /*0x800000*/;
    private const int WS_CAPTION = 12582912 /*0xC00000*/;
    private const int WS_CHILD = 1073741824 /*0x40000000*/;
    private const int WS_CHILDWINDOW = 1073741824 /*0x40000000*/;
    private const int WS_CLIPCHILDREN = 33554432 /*0x02000000*/;
    private const int WS_CLIPSIBLINGS = 67108864 /*0x04000000*/;
    private const int WS_DISABLED = 134217728 /*0x08000000*/;
    private const int WS_DLGFRAME = 4194304 /*0x400000*/;
    private const int WS_GROUP = 131072 /*0x020000*/;
    private const int WS_HSCROLL = 1048576 /*0x100000*/;
    private const int WS_ICONIC = 536870912 /*0x20000000*/;
    private const int WS_MAXIMIZE = 16777216 /*0x01000000*/;
    private const int WS_MAXIMIZEBOX = 65536 /*0x010000*/;
    private const int WS_MINIMIZE = 536870912 /*0x20000000*/;
    private const int WS_MINIMIZEBOX = 131072 /*0x020000*/;
    private const int WS_OVERLAPPED = 0;
    private const int WS_OVERLAPPEDWINDOW = 13565952 /*0xCF0000*/;
    private const int WS_POPUP = -2147483648 /*0x80000000*/;
    private const int WS_POPUPWINDOW = -2138570752 /*0x80880000*/;
    private const int WS_SIZEBOX = 262144 /*0x040000*/;
    private const int WS_SYSMENU = 524288 /*0x080000*/;
    private const int WS_TABSTOP = 65536 /*0x010000*/;
    private const int WS_THICKFRAME = 262144 /*0x040000*/;
    private const int WS_TILED = 0;
    private const int WS_TILEDWINDOW = 13565952 /*0xCF0000*/;
    private const int WS_VISIBLE = 268435456 /*0x10000000*/;
    private const int WS_VSCROLL = 2097152 /*0x200000*/;
    private const int WS_EX_DLGMODALFRAME = 1;
    private const int WS_EX_CLIENTEDGE = 512 /*0x0200*/;
    private const int WS_EX_STATICEDGE = 131072 /*0x020000*/;
    private const int SWP_FRAMECHANGED = 32 /*0x20*/;
    private const int SWP_NOMOVE = 2;
    private const int SWP_NOSIZE = 1;
    private const int SWP_NOZORDER = 4;
    private const int SWP_NOOWNERZORDER = 512 /*0x0200*/;
    private const int SWP_SHOWWINDOW = 64 /*0x40*/;
    private const int SWP_NOSENDCHANGING = 1024 /*0x0400*/;
    private const int GWL_STYLE = -16;
    private const int GWL_EXSTYLE = -20;

    public WindowsResolutionManager(string title) => this._title = title;

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowPos(
      IntPtr hWnd,
      int hWndInsertAfter,
      int x,
      int Y,
      int cx,
      int cy,
      int uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(
      IntPtr hWnd,
      out BraveOptionsMenuItem.WindowsResolutionManager.RECT rect);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetClientRect(
      IntPtr hWnd,
      out BraveOptionsMenuItem.WindowsResolutionManager.RECT rect);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetMonitorInfo(
      IntPtr hmonitor,
      [In, Out] BraveOptionsMenuItem.WindowsResolutionManager.MONITORINFOEX info);

    public Position? GetWindowPosition()
    {
      BraveOptionsMenuItem.WindowsResolutionManager.RECT rect;
      return !BraveOptionsMenuItem.WindowsResolutionManager.GetWindowRect(this.Window, out rect) ? new Position?() : new Position?(new Position(rect.Left, rect.Top));
    }

    public Position? GetCenteredPosition(
      Resolution resolution,
      BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes displayMode)
    {
      BraveOptionsMenuItem.WindowsResolutionManager.RECT rect;
      if (!BraveOptionsMenuItem.WindowsResolutionManager.GetWindowRect(this.Desktop, out rect))
        return new Position?();
      int num1 = rect.Right - rect.Left;
      int num2 = rect.Bottom - rect.Top;
      int num3 = 0;
      int num4 = 0;
      IntPtr hmonitor = BraveOptionsMenuItem.WindowsResolutionManager.MonitorFromWindow(this.Window, 2U);
      BraveOptionsMenuItem.WindowsResolutionManager.MONITORINFOEX info = new BraveOptionsMenuItem.WindowsResolutionManager.MONITORINFOEX();
      if (BraveOptionsMenuItem.WindowsResolutionManager.GetMonitorInfo(hmonitor, info))
      {
        num1 = info.rcMonitor.Right - info.rcMonitor.Left;
        num2 = info.rcMonitor.Bottom - info.rcMonitor.Top;
        num3 = info.rcMonitor.Left;
        num4 = info.rcMonitor.Top;
      }
      int num5;
      int num6;
      if (displayMode == BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Windowed)
      {
        num5 = (num1 - (resolution.width + this._borderWidth * 2)) / 2;
        num6 = (num2 - (resolution.height + this._borderWidth * 2 + this._captionHeight)) / 2;
      }
      else
      {
        num5 = (num1 - resolution.width) / 2;
        num6 = (num2 - resolution.height) / 2;
      }
      return new Position?(new Position(num5 + num3, num6 + num4));
    }

    private void UpdateWindowRect(IntPtr window, int x, int y, int width, int height)
    {
      BraveOptionsMenuItem.WindowsResolutionManager.SetWindowPos(window, -2, x, y, width, height, 32 /*0x20*/);
    }

    private bool UpdateDecorationSize(IntPtr window)
    {
      BraveOptionsMenuItem.WindowsResolutionManager.RECT rect1;
      BraveOptionsMenuItem.WindowsResolutionManager.RECT rect2;
      if (!BraveOptionsMenuItem.WindowsResolutionManager.GetWindowRect(this.Window, out rect1) || !BraveOptionsMenuItem.WindowsResolutionManager.GetClientRect(this.Window, out rect2))
        return false;
      int num1 = rect1.Right - rect1.Left - (rect2.Right - rect2.Left);
      int num2 = rect1.Bottom - rect1.Top - (rect2.Bottom - rect2.Top);
      this._borderWidth = num1 / 2;
      this._captionHeight = num2 - this._borderWidth * 2;
      return true;
    }

    public bool TrySetDisplay(
      BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes targetDisplayMode,
      Resolution targetResolution,
      bool setPosition,
      Position? position)
    {
      int windowLongPtr = (int) BraveOptionsMenuItem.WindowsResolutionManager.GetWindowLongPtr(this.Window, -16);
      if (targetDisplayMode == BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Windowed && BraveOptionsMenuItem.WindowsResolutionManager.Flags.Contains(windowLongPtr, 8388608 /*0x800000*/))
        position = this.GetWindowPosition();
      switch (targetDisplayMode)
      {
        case BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Fullscreen:
          return true;
        case BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Borderless:
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Unset<int>(ref windowLongPtr, 8388608 /*0x800000*/);
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Unset<int>(ref windowLongPtr, 262144 /*0x040000*/);
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Unset<int>(ref windowLongPtr, 12582912 /*0xC00000*/);
          BraveOptionsMenuItem.WindowsResolutionManager.SetWindowLongPtr(this.Window, -16, windowLongPtr);
          if (!setPosition || !position.HasValue)
            position = this.GetCenteredPosition(targetResolution, targetDisplayMode);
          this.UpdateWindowRect(this.Window, position.Value.X, position.Value.Y, targetResolution.width, targetResolution.height);
          BraveOptionsMenuItem.WindowsResolutionManager.SetWindowLongPtr(this.Window, -16, windowLongPtr);
          BraveOptionsMenuItem.WindowsResolutionManager.SetWindowLongPtr(this.Window, -16, windowLongPtr);
          return true;
        case BraveOptionsMenuItem.WindowsResolutionManager.DisplayModes.Windowed:
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Set<int>(ref windowLongPtr, 8388608 /*0x800000*/);
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Set<int>(ref windowLongPtr, 262144 /*0x040000*/);
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Set<int>(ref windowLongPtr, 12582912 /*0xC00000*/);
          BraveOptionsMenuItem.WindowsResolutionManager.SetWindowLongPtr(this.Window, -16, windowLongPtr);
          this.UpdateDecorationSize(this.Window);
          if (!position.HasValue)
            position = this.GetCenteredPosition(targetResolution, targetDisplayMode);
          int width = targetResolution.width + this._borderWidth * 2;
          int height = targetResolution.height + this._captionHeight + this._borderWidth * 2;
          this.UpdateWindowRect(this.Window, position.Value.X, position.Value.Y, width, height);
          return true;
        default:
          return false;
      }
    }

    private IntPtr Window
    {
      get
      {
        return BraveOptionsMenuItem.WindowsResolutionManager.FindWindowByCaption(IntPtr.Zero, this._title);
      }
    }

    private IntPtr Desktop => BraveOptionsMenuItem.WindowsResolutionManager.GetDesktopWindow();

    public enum DisplayModes
    {
      Fullscreen,
      Borderless,
      Windowed,
    }

    public struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    public class MONITORINFOEX
    {
      public int cbSize = Marshal.SizeOf(typeof (BraveOptionsMenuItem.WindowsResolutionManager.MONITORINFOEX));
      public BraveOptionsMenuItem.WindowsResolutionManager.RECT rcMonitor = new BraveOptionsMenuItem.WindowsResolutionManager.RECT();
      public BraveOptionsMenuItem.WindowsResolutionManager.RECT rcWork = new BraveOptionsMenuItem.WindowsResolutionManager.RECT();
      public int dwFlags;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 /*0x20*/)]
      public char[] szDevice = new char[32 /*0x20*/];
    }

    internal static class Flags
    {
      public static void Set<T>(ref T mask, T flag) where T : struct
      {
        int num1 = (int) (ValueType) mask;
        int num2 = (int) (ValueType) flag;
        mask = (T) (ValueType) (num1 | num2);
      }

      public static void Unset<T>(ref T mask, T flag) where T : struct
      {
        int num1 = (int) (ValueType) mask;
        int num2 = (int) (ValueType) flag;
        mask = (T) (ValueType) (num1 & ~num2);
      }

      public static void Toggle<T>(ref T mask, T flag) where T : struct
      {
        if (BraveOptionsMenuItem.WindowsResolutionManager.Flags.Contains<T>(mask, flag))
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Unset<T>(ref mask, flag);
        else
          BraveOptionsMenuItem.WindowsResolutionManager.Flags.Set<T>(ref mask, flag);
      }

      public static bool Contains<T>(T mask, T flag) where T : struct
      {
        return BraveOptionsMenuItem.WindowsResolutionManager.Flags.Contains((int) (ValueType) mask, (int) (ValueType) flag);
      }

      public static bool Contains(int mask, int flag) => (mask & flag) != 0;
    }
  }
}
