// Decompiled with JetBrains decompiler
// Type: KeyboardBindingMenuOption
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class KeyboardBindingMenuOption : MonoBehaviour
  {
    public dfLabel CenterColumnLabel;
    public dfLabel CommandLabel;
    public dfButton KeyButton;
    public dfButton AltKeyButton;
    public dfLabel AltAlignLabel;
    public GungeonActions.GungeonActionType ActionType;
    public bool IsControllerMode;
    private FullOptionsMenuController m_parentOptionsMenu;
    private Vector2 m_cachedKeyButtonPosition;

    public bool NonBindable { get; set; }

    public string OverrideKeyString { get; set; }

    public string OverrideAltKeyString { get; set; }

    public void Initialize()
    {
      if ((UnityEngine.Object) this.m_parentOptionsMenu == (UnityEngine.Object) null)
        this.m_parentOptionsMenu = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu;
      if (this.IsControllerMode)
        this.InitializeController();
      else
        this.InitializeKeyboard();
      if (this.NonBindable)
      {
        this.CommandLabel.IsInteractive = false;
        this.CommandLabel.Color = (Color32) new Color(0.25f, 0.25f, 0.25f);
      }
      else
      {
        this.CommandLabel.IsInteractive = true;
        this.CommandLabel.Color = (Color32) new Color(0.596f, 0.596f, 0.596f, 1f);
      }
    }

    private void InitializeController()
    {
      GungeonActions activeActions = this.GetBestInputInstance().ActiveActions;
      PlayerAction actionFromType = activeActions.GetActionFromType(this.ActionType);
      bool flag1 = false;
      string str1 = "-";
      bool flag2 = false;
      string str2 = "-";
      for (int index = 0; index < actionFromType.Bindings.Count; ++index)
      {
        BindingSource binding = actionFromType.Bindings[index];
        if (binding.BindingSourceType == BindingSourceType.DeviceBindingSource)
        {
          DeviceBindingSource deviceBindingSource = binding as DeviceBindingSource;
          GameOptions.ControllerSymbology currentSymbology = BraveInput.GetCurrentSymbology(FullOptionsMenuController.CurrentBindingPlayerTargetIndex);
          if (!flag1)
          {
            flag1 = true;
            str1 = UIControllerButtonHelper.GetUnifiedControllerButtonTag(deviceBindingSource.Control, currentSymbology, activeActions);
          }
          else if (!flag2)
          {
            str2 = UIControllerButtonHelper.GetUnifiedControllerButtonTag(deviceBindingSource.Control, currentSymbology, activeActions);
            break;
          }
        }
        if (binding.BindingSourceType == BindingSourceType.UnknownDeviceBindingSource)
        {
          UnknownDeviceBindingSource deviceBindingSource = binding as UnknownDeviceBindingSource;
          if (!flag1)
          {
            flag1 = true;
            str1 = deviceBindingSource.Control.Control.ToString();
          }
          else if (!flag2)
          {
            flag2 = true;
            str2 = deviceBindingSource.Control.Control.ToString();
          }
        }
      }
      this.KeyButton.Text = string.IsNullOrEmpty(this.OverrideKeyString) ? str1.Trim() : this.OverrideKeyString;
      this.AltKeyButton.Text = string.IsNullOrEmpty(this.OverrideAltKeyString) ? str2.Trim() : this.OverrideAltKeyString;
      this.AltKeyButton.transform.position = this.AltKeyButton.transform.position.WithX(this.AltAlignLabel.GetCenter().x);
      this.KeyButton.Padding = GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ITALIAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.GERMAN ? new RectOffset(60, 0, 0, 0) : (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH ? new RectOffset(0, 0, 0, 0) : new RectOffset(180, 0, 0, 0));
      if ((bool) (UnityEngine.Object) this.CenterColumnLabel)
        this.CenterColumnLabel.Padding = this.KeyButton.Padding;
      this.GetComponent<dfPanel>().PerformLayout();
      this.CommandLabel.RelativePosition = this.CommandLabel.RelativePosition.WithX(0.0f);
    }

    public BraveInput GetBestInputInstance()
    {
      return GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER || !Foyer.DoMainMenu ? BraveInput.GetInstanceForPlayer(FullOptionsMenuController.CurrentBindingPlayerTargetIndex) : BraveInput.PlayerlessInstance;
    }

    private void InitializeKeyboard()
    {
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
      {
        BraveInput instanceForPlayer1 = BraveInput.GetInstanceForPlayer(0);
        BraveInput instanceForPlayer2 = BraveInput.GetInstanceForPlayer(1);
        if ((bool) (UnityEngine.Object) instanceForPlayer1 && (bool) (UnityEngine.Object) instanceForPlayer2)
        {
          GungeonActions activeActions1 = instanceForPlayer1.ActiveActions;
          GungeonActions activeActions2 = instanceForPlayer2.ActiveActions;
          if (activeActions1 != null && activeActions2 != null)
          {
            PlayerAction actionFromType1 = activeActions1.GetActionFromType(this.ActionType);
            PlayerAction actionFromType2 = activeActions2.GetActionFromType(this.ActionType);
            actionFromType2.ClearBindingsOfType(BindingSourceType.KeyBindingSource);
            for (int index = 0; index < actionFromType1.Bindings.Count; ++index)
            {
              BindingSource binding1 = actionFromType1.Bindings[index];
              if (binding1.BindingSourceType == BindingSourceType.KeyBindingSource && binding1 is KeyBindingSource)
              {
                BindingSource binding2 = (BindingSource) new KeyBindingSource((binding1 as KeyBindingSource).Control);
                actionFromType2.AddBinding(binding2);
              }
            }
          }
        }
      }
      PlayerAction actionFromType = this.GetBestInputInstance().ActiveActions.GetActionFromType(this.ActionType);
      bool flag1 = false;
      string str1 = "-";
      bool flag2 = false;
      string str2 = "-";
      for (int index = 0; index < actionFromType.Bindings.Count; ++index)
      {
        BindingSource binding = actionFromType.Bindings[index];
        if (binding.BindingSourceType == BindingSourceType.KeyBindingSource || binding.BindingSourceType == BindingSourceType.MouseBindingSource)
        {
          if (!flag1)
          {
            flag1 = true;
            str1 = binding.Name;
          }
          else if (!flag2)
          {
            str2 = binding.Name;
            break;
          }
        }
      }
      this.KeyButton.Text = str1.Trim();
      this.AltKeyButton.Text = str2.Trim();
      this.AltKeyButton.transform.position = this.AltKeyButton.transform.position.WithX(this.AltAlignLabel.GetCenter().x);
      this.KeyButton.Padding = GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ITALIAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.GERMAN ? new RectOffset(60, 0, 0, 0) : (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH ? new RectOffset(0, 0, 0, 0) : new RectOffset(180, 0, 0, 0));
      if ((bool) (UnityEngine.Object) this.CenterColumnLabel)
        this.CenterColumnLabel.Padding = this.KeyButton.Padding;
      this.GetComponent<dfPanel>().PerformLayout();
      this.CommandLabel.RelativePosition = this.CommandLabel.RelativePosition.WithX(0.0f);
    }

    public void KeyClicked(dfControl source, dfControlEventArgs args)
    {
      if (this.NonBindable)
        return;
      this.EnterAssignmentMode(false);
      GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.DoModalKeyBindingDialog(this.CommandLabel.Text);
      this.StartCoroutine(this.WaitForAssignmentModeToEnd());
    }

    public void AltKeyClicked(dfControl source, dfControlEventArgs args)
    {
      if (this.NonBindable)
        return;
      this.EnterAssignmentMode(true);
      GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.DoModalKeyBindingDialog(this.CommandLabel.Text);
      this.StartCoroutine(this.WaitForAssignmentModeToEnd());
    }

    private void Update()
    {
      if (!Input.GetKeyDown(KeyCode.Delete))
        return;
      if (this.KeyButton.HasFocus)
      {
        PlayerAction actionFromType = this.GetBestInputInstance().ActiveActions.GetActionFromType(this.ActionType);
        if (this.IsControllerMode)
        {
          actionFromType.ClearSpecificBindingByType(0, BindingSourceType.DeviceBindingSource);
          this.InitializeController();
        }
        else
        {
          actionFromType.ClearSpecificBindingByType(0, BindingSourceType.KeyBindingSource, BindingSourceType.MouseBindingSource);
          this.InitializeKeyboard();
        }
      }
      else
      {
        if (!this.AltKeyButton.HasFocus)
          return;
        PlayerAction actionFromType = this.GetBestInputInstance().ActiveActions.GetActionFromType(this.ActionType);
        if (this.IsControllerMode)
        {
          actionFromType.ClearSpecificBindingByType(1, BindingSourceType.DeviceBindingSource);
          this.InitializeController();
        }
        else
        {
          actionFromType.ClearSpecificBindingByType(1, BindingSourceType.KeyBindingSource, BindingSourceType.MouseBindingSource);
          this.InitializeKeyboard();
        }
      }
    }

    [DebuggerHidden]
    private IEnumerator WaitForAssignmentModeToEnd()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KeyboardBindingMenuOption__WaitForAssignmentModeToEndc__Iterator0()
      {
        _this = this
      };
    }

    public void EnterAssignmentMode(bool isAlternateKey)
    {
      GungeonActions activeActions = this.GetBestInputInstance().ActiveActions;
      PlayerAction actionFromType = activeActions.GetActionFromType(this.ActionType);
      BindingListenOptions bindingOptions = new BindingListenOptions();
      if (this.IsControllerMode)
      {
        bindingOptions.IncludeControllers = true;
        bindingOptions.IncludeNonStandardControls = true;
        bindingOptions.IncludeKeys = true;
        bindingOptions.IncludeMouseButtons = false;
        bindingOptions.IncludeMouseScrollWheel = false;
        bindingOptions.IncludeModifiersAsFirstClassKeys = false;
        bindingOptions.IncludeUnknownControllers = GameManager.Options.allowUnknownControllers;
      }
      else
      {
        bindingOptions.IncludeControllers = false;
        bindingOptions.IncludeNonStandardControls = false;
        bindingOptions.IncludeKeys = true;
        bindingOptions.IncludeMouseButtons = true;
        bindingOptions.IncludeMouseScrollWheel = true;
        bindingOptions.IncludeModifiersAsFirstClassKeys = true;
      }
      bindingOptions.MaxAllowedBindingsPerType = 2U;
      bindingOptions.OnBindingFound = (Func<PlayerAction, BindingSource, bool>) ((action, binding) =>
      {
        if (binding == (BindingSource) new KeyBindingSource(new Key[1]
        {
          Key.Escape
        }))
        {
          action.StopListeningForBinding();
          GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.ClearModalKeyBindingDialog((dfControl) null, (dfControlEventArgs) null);
          return false;
        }
        if (binding == (BindingSource) new KeyBindingSource(new Key[1]
        {
          Key.Delete
        }))
        {
          action.StopListeningForBinding();
          GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.ClearModalKeyBindingDialog((dfControl) null, (dfControlEventArgs) null);
          return false;
        }
        if (this.IsControllerMode && binding is KeyBindingSource)
          return false;
        action.StopListeningForBinding();
        if (!this.m_parentOptionsMenu.ActionIsMultibindable(this.ActionType, activeActions))
          this.m_parentOptionsMenu.ClearBindingFromAllControls(FullOptionsMenuController.CurrentBindingPlayerTargetIndex, binding);
        action.SetBindingOfTypeByNumber(binding, binding.BindingSourceType, !isAlternateKey ? 0 : 1, bindingOptions.OnBindingAdded);
        GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.ToggleKeyBindingDialogState(binding);
        this.Initialize();
        return false;
      });
      bindingOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>) ((action, binding) =>
      {
        if (FullOptionsMenuController.CurrentBindingPlayerTargetIndex == 1)
          GameManager.Options.CurrentControlPresetP2 = GameOptions.ControlPreset.CUSTOM;
        else
          GameManager.Options.CurrentControlPreset = GameOptions.ControlPreset.CUSTOM;
        BraveOptionsMenuItem[] componentsInChildren = this.CenterColumnLabel.Parent.Parent.Parent.GetComponentsInChildren<BraveOptionsMenuItem>();
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
          componentsInChildren[index].InitializeFromOptions();
          componentsInChildren[index].ForceRefreshDisplayLabel();
        }
        this.Initialize();
      });
      actionFromType.ListenOptions = bindingOptions;
      if (actionFromType.IsListeningForBinding)
        return;
      actionFromType.ListenForBinding();
    }
  }

