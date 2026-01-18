using InControl;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class FullOptionsMenuController : MonoBehaviour
  {
    public dfButton PrimaryCancelButton;
    public dfButton PrimaryResetDefaultsButton;
    public dfButton PrimaryConfirmButton;
    public dfScrollPanel TabAudio;
    public dfScrollPanel TabVideo;
    public dfScrollPanel TabGameplay;
    public dfScrollPanel TabControls;
    public dfScrollPanel TabKeyboardBindings;
    public dfScrollPanel TabCredits;
    public dfScrollPanel TabHowToPlay;
    public dfPanel ModalKeyBindingDialog;
    public PreOptionsMenuController PreOptionsMenu;
    protected GameOptions cloneOptions;
    protected dfPanel m_panel;
    private bool finishedInitialization;
    private List<BraveOptionsMenuItem> m_menuItems;
    private dfControl m_cachedFocusedControl;
    private dfControl m_lastSelectedBottomRowControl;
    private List<KeyboardBindingMenuOption> m_keyboardBindingLines = new List<KeyboardBindingMenuOption>();
    private Vector2 m_cachedResolution;
    public static int CurrentBindingPlayerTargetIndex;
    private bool m_firstTimeBindingsInitialization = true;
    private float m_justResetToDefaultsWithPrompt;
    private Vector2 m_cachedRelativePositionPrimaryConfirm;
    private Vector2 m_cachedRelativePositionPrimaryCancel;
    private StringTableManager.GungeonSupportedLanguages m_cachedLanguage;
    private bool m_hasCachedPositions;

    public bool IsVisible
    {
      get => this.m_panel.IsVisible;
      set
      {
        if (this.m_panel.IsVisible == value)
          return;
        if (value)
        {
          this.EnableHierarchy();
          this.m_panel.IsVisible = value;
          this.ShwoopOpen();
          this.ShowOptionsMenu();
        }
        else
        {
          this.ShwoopClosed();
          if ((UnityEngine.Object) dfGUIManager.GetModalControl() == (UnityEngine.Object) this.m_panel)
            dfGUIManager.PopModal();
          else if (dfGUIManager.ModalStackContainsControl((dfControl) this.m_panel))
            dfGUIManager.PopModalToControl((dfControl) this.m_panel, true);
          else
            UnityEngine.Debug.LogError((object) "failure.");
        }
      }
    }

    public dfPanel MainPanel => this.m_panel;

    private void Awake() => this.m_panel = this.GetComponent<dfPanel>();

    public void EnableHierarchy()
    {
      if (this.gameObject.activeSelf)
        return;
      this.gameObject.SetActive(true);
      Vector2 screenSize = this.m_panel.GUIManager.GetScreenSize();
      dfControl[] componentsInChildren = this.m_panel.GetComponentsInChildren<dfControl>();
      for (int index = 0; index < componentsInChildren.Length; ++index)
        componentsInChildren[index].OnResolutionChanged(this.m_cachedResolution, screenSize);
      for (int index = 0; index < componentsInChildren.Length; ++index)
        componentsInChildren[index].PerformLayout();
    }

    public void DisableHierarchy()
    {
      if (!this.gameObject.activeSelf)
        return;
      dfGUIManager guiManager = this.m_panel.GUIManager;
      float pixelHeight = (float) guiManager.RenderCamera.pixelHeight;
      this.m_cachedResolution = new Vector2((!guiManager.FixedAspect ? guiManager.RenderCamera.aspect : 1.77777779f) * pixelHeight, pixelHeight);
      this.gameObject.SetActive(false);
    }

    public void DoModalKeyBindingDialog(string controlName)
    {
      this.m_cachedFocusedControl = dfGUIManager.ActiveControl;
      this.ModalKeyBindingDialog.IsVisible = true;
      this.m_panel.IsVisible = false;
      this.ModalKeyBindingDialog.BringToFront();
      dfGUIManager.PushModal((dfControl) this.ModalKeyBindingDialog);
      if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
      {
        this.ModalKeyBindingDialog.transform.Find("TopLabel").GetComponent<dfLabel>().Text = $"BIND {controlName.ToUpperInvariant()}?";
        this.ModalKeyBindingDialog.transform.Find("SecondaryLabel").GetComponent<dfLabel>().IsVisible = true;
      }
      else
      {
        this.ModalKeyBindingDialog.transform.Find("TopLabel").GetComponent<dfLabel>().Text = controlName.ToUpperInvariant();
        this.ModalKeyBindingDialog.transform.Find("SecondaryLabel").GetComponent<dfLabel>().IsVisible = false;
      }
      dfButton component = this.ModalKeyBindingDialog.transform.Find("Input Thing").GetComponent<dfButton>();
      component.Text = "___";
      component.PerformLayout();
    }

    public void ToggleKeyBindingDialogState(BindingSource binding)
    {
      dfButton component = this.ModalKeyBindingDialog.transform.Find("Input Thing").GetComponent<dfButton>();
      if (binding is DeviceBindingSource)
      {
        GameOptions.ControllerSymbology currentSymbology = BraveInput.GetCurrentSymbology(FullOptionsMenuController.CurrentBindingPlayerTargetIndex);
        component.Text = UIControllerButtonHelper.GetUnifiedControllerButtonTag((binding as DeviceBindingSource).Control, currentSymbology);
      }
      else
        component.Text = binding.Name;
      component.PerformLayout();
      component.Focus(true);
      component.Click += new MouseEventHandler(this.ClearModalKeyBindingDialog);
      this.StartCoroutine(this.HandleTimedKeyBindingClear());
    }

    [DebuggerHidden]
    private IEnumerator HandleTimedKeyBindingClear()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FullOptionsMenuController__HandleTimedKeyBindingClearc__Iterator0()
      {
        _this = this
      };
    }

    public void ClearModalKeyBindingDialog(dfControl source, dfControlEventArgs args)
    {
      if (!this.ModalKeyBindingDialog.IsVisible)
        return;
      this.m_panel.IsVisible = true;
      dfGUIManager.PopModalToControl((dfControl) this.m_panel, false);
      this.ModalKeyBindingDialog.IsVisible = false;
      if (!((UnityEngine.Object) this.m_cachedFocusedControl != (UnityEngine.Object) null))
        return;
      this.m_cachedFocusedControl.Focus(true);
    }

    public void ReinitializeKeyboardBindings()
    {
      for (int index = 0; index < this.m_keyboardBindingLines.Count; ++index)
        this.m_keyboardBindingLines[index].Initialize();
    }

    public bool ActionIsMultibindable(
      GungeonActions.GungeonActionType actionType,
      GungeonActions activeActions)
    {
      return actionType == GungeonActions.GungeonActionType.DropGun || actionType == GungeonActions.GungeonActionType.DropItem || actionType == GungeonActions.GungeonActionType.SelectUp || actionType == GungeonActions.GungeonActionType.SelectDown || actionType == GungeonActions.GungeonActionType.SelectLeft || actionType == GungeonActions.GungeonActionType.SelectRight || actionType == GungeonActions.GungeonActionType.MenuInteract || actionType == GungeonActions.GungeonActionType.Cancel || actionType == GungeonActions.GungeonActionType.PunchoutDodgeLeft || actionType == GungeonActions.GungeonActionType.PunchoutDodgeRight || actionType == GungeonActions.GungeonActionType.PunchoutBlock || actionType == GungeonActions.GungeonActionType.PunchoutDuck || actionType == GungeonActions.GungeonActionType.PunchoutPunchLeft || actionType == GungeonActions.GungeonActionType.PunchoutPunchRight || actionType == GungeonActions.GungeonActionType.PunchoutSuper;
    }

    public void ClearBindingFromAllControls(int targetPlayerIndex, BindingSource bindingSource)
    {
      GungeonActions activeActions = BraveInput.GetInstanceForPlayer(targetPlayerIndex).ActiveActions;
      for (int index1 = 0; index1 < this.m_keyboardBindingLines.Count; ++index1)
      {
        bool flag = false;
        GungeonActions.GungeonActionType actionType = this.m_keyboardBindingLines[index1].ActionType;
        if (!this.ActionIsMultibindable(actionType, activeActions))
        {
          PlayerAction actionFromType = activeActions.GetActionFromType(actionType);
          for (int index2 = 0; index2 < actionFromType.Bindings.Count; ++index2)
          {
            BindingSource binding = actionFromType.Bindings[index2];
            if (binding == bindingSource)
            {
              actionFromType.RemoveBinding(binding);
              flag = true;
            }
          }
          if (flag)
          {
            actionFromType.ForceUpdateVisibleBindings();
            this.m_keyboardBindingLines[index1].Initialize();
          }
        }
      }
    }

    public void SwitchBindingsMenuMode(bool isController)
    {
      BraveOptionsMenuItem component = this.TabKeyboardBindings.Controls[0].GetComponent<BraveOptionsMenuItem>();
      component.optionType = FullOptionsMenuController.CurrentBindingPlayerTargetIndex != 0 ? BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET_P2 : BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET;
      if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
      {
        component.labelControl.IsLocalized = false;
        component.labelControl.Text = "Binding Preset";
        component.labelControl.PerformLayout();
        component.labelControl.Parent.PerformLayout();
      }
      else
      {
        component.labelControl.IsLocalized = true;
        component.labelControl.Text = !isController ? "#OPTIONS_EDITKEYBOARDBINDINGS" : (FullOptionsMenuController.CurrentBindingPlayerTargetIndex != 1 ? "#OPTIONS_EDITP1BINDINGS" : "#OPTIONS_EDITP2BINDINGS");
      }
      component.InitializeFromOptions();
      component.ForceRefreshDisplayLabel();
      for (int index = 0; index < this.m_keyboardBindingLines.Count; ++index)
      {
        this.m_keyboardBindingLines[index].IsControllerMode = isController;
        this.m_keyboardBindingLines[index].Initialize();
      }
      if (!((UnityEngine.Object) this.TabKeyboardBindings != (UnityEngine.Object) null))
        return;
      this.TabKeyboardBindings.PerformLayout();
    }

    public void FullyReinitializeKeyboardBindings()
    {
      DebugTime.Log(nameof (FullyReinitializeKeyboardBindings));
      dfPanel component1 = this.TabKeyboardBindings.GetComponentInChildren<KeyboardBindingMenuOption>().GetComponent<dfPanel>();
      for (int index = this.m_keyboardBindingLines.Count - 1; index >= 1; --index)
      {
        KeyboardBindingMenuOption keyboardBindingLine1 = this.m_keyboardBindingLines[index];
        dfPanel component2 = keyboardBindingLine1.GetComponent<dfPanel>();
        component1.RemoveControl((dfControl) component2);
        KeyboardBindingMenuOption keyboardBindingLine2 = this.m_keyboardBindingLines[index - 1];
        keyboardBindingLine2.KeyButton.GetComponent<UIKeyControls>().down = keyboardBindingLine1.KeyButton.GetComponent<UIKeyControls>().down;
        keyboardBindingLine2.AltKeyButton.GetComponent<UIKeyControls>().down = keyboardBindingLine1.AltKeyButton.GetComponent<UIKeyControls>().down;
        UnityEngine.Object.Destroy((UnityEngine.Object) component2.gameObject);
      }
      this.m_keyboardBindingLines.Clear();
      this.finishedInitialization = true;
      this.InitializeKeyboardBindingsPanel();
      KeyboardBindingMenuOption keyboardBindingLine = this.m_keyboardBindingLines[this.m_keyboardBindingLines.Count - 1];
      keyboardBindingLine.KeyButton.GetComponent<UIKeyControls>().down = (dfControl) this.PrimaryConfirmButton;
      keyboardBindingLine.AltKeyButton.GetComponent<UIKeyControls>().down = (dfControl) this.PrimaryConfirmButton;
      this.PrimaryCancelButton.GetComponent<UIKeyControls>().up = (dfControl) keyboardBindingLine.KeyButton;
      this.PrimaryConfirmButton.GetComponent<UIKeyControls>().up = (dfControl) keyboardBindingLine.KeyButton;
      this.PrimaryResetDefaultsButton.GetComponent<UIKeyControls>().up = (dfControl) keyboardBindingLine.KeyButton;
    }

    private void InitializeKeyboardBindingsPanel()
    {
      KeyboardBindingMenuOption componentInChildren = this.TabKeyboardBindings.GetComponentInChildren<KeyboardBindingMenuOption>();
      dfPanel component = componentInChildren.GetComponent<dfPanel>();
      componentInChildren.KeyButton.GetComponent<UIKeyControls>().up = this.TabKeyboardBindings.Controls[0];
      componentInChildren.AltKeyButton.GetComponent<UIKeyControls>().up = this.TabKeyboardBindings.Controls[0];
      this.TabKeyboardBindings.Controls[0].GetComponent<BraveOptionsMenuItem>().down = (dfControl) componentInChildren.KeyButton;
      if (this.m_firstTimeBindingsInitialization)
      {
        componentInChildren.KeyButton.Click += new MouseEventHandler(componentInChildren.KeyClicked);
        componentInChildren.AltKeyButton.Click += new MouseEventHandler(componentInChildren.AltKeyClicked);
        this.m_firstTimeBindingsInitialization = false;
      }
      componentInChildren.Initialize();
      this.m_keyboardBindingLines.Add(componentInChildren);
      KeyboardBindingMenuOption previousMenuOption1 = componentInChildren;
      KeyboardBindingMenuOption previousMenuOption2 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.DodgeRoll, "#OPTIONS_DODGEROLL", previousMenuOption1);
      KeyboardBindingMenuOption previousMenuOption3 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Interact, "#OPTIONS_INTERACT", previousMenuOption2);
      KeyboardBindingMenuOption previousMenuOption4 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Reload, "#OPTIONS_RELOAD", previousMenuOption3);
      KeyboardBindingMenuOption previousMenuOption5 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Up, "#OPTIONS_MOVEUP", previousMenuOption4);
      KeyboardBindingMenuOption previousMenuOption6 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Down, "#OPTIONS_MOVEDOWN", previousMenuOption5);
      KeyboardBindingMenuOption previousMenuOption7 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Left, "#OPTIONS_MOVELEFT", previousMenuOption6);
      KeyboardBindingMenuOption previousMenuOption8 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Right, "#OPTIONS_MOVERIGHT", previousMenuOption7);
      KeyboardBindingMenuOption previousMenuOption9 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.AimUp, "#OPTIONS_AIMUP", previousMenuOption8);
      KeyboardBindingMenuOption previousMenuOption10 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.AimDown, "#OPTIONS_AIMDOWN", previousMenuOption9);
      KeyboardBindingMenuOption previousMenuOption11 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.AimLeft, "#OPTIONS_AIMLEFT", previousMenuOption10);
      KeyboardBindingMenuOption previousMenuOption12 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.AimRight, "#OPTIONS_AIMRIGHT", previousMenuOption11);
      KeyboardBindingMenuOption previousMenuOption13 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.UseItem, "#OPTIONS_USEITEM", previousMenuOption12);
      KeyboardBindingMenuOption previousMenuOption14 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Blank, "#OPTIONS_USEBLANK", previousMenuOption13);
      KeyboardBindingMenuOption previousMenuOption15 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Map, "#OPTIONS_MAP", previousMenuOption14);
      if (true)
      {
        KeyboardBindingMenuOption previousMenuOption16 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.CycleGunUp, "#OPTIONS_CYCLEGUNUP", previousMenuOption15);
        KeyboardBindingMenuOption previousMenuOption17 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.CycleGunDown, "#OPTIONS_CYCLEGUNDOWN", previousMenuOption16);
        KeyboardBindingMenuOption previousMenuOption18 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.CycleItemUp, "#OPTIONS_CYCLEITEMUP", previousMenuOption17);
        previousMenuOption15 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.CycleItemDown, "#OPTIONS_CYCLEITEMDOWN", previousMenuOption18);
      }
      KeyboardBindingMenuOption previousMenuOption19 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.GunQuickEquip, "#OPTIONS_GUNMENU", previousMenuOption15);
      if (true)
      {
        KeyboardBindingMenuOption previousMenuOption20 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.DropGun, "#OPTIONS_DROPGUN", previousMenuOption19);
        previousMenuOption19 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.DropItem, "#OPTIONS_DROPITEM", previousMenuOption20);
      }
      KeyboardBindingMenuOption previousMenuOption21 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Pause, "#OPTIONS_PAUSE", previousMenuOption19);
      KeyboardBindingMenuOption previousMenuOption22 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.EquipmentMenu, "#OPTIONS_INVENTORY", previousMenuOption21);
      if (GameManager.Options.allowUnknownControllers)
      {
        KeyboardBindingMenuOption previousMenuOption23 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.SelectUp, "#OPTIONS_MENUUP", previousMenuOption22);
        KeyboardBindingMenuOption previousMenuOption24 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.SelectDown, "#OPTIONS_MENUDOWN", previousMenuOption23);
        KeyboardBindingMenuOption previousMenuOption25 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.SelectLeft, "#OPTIONS_MENULEFT", previousMenuOption24);
        KeyboardBindingMenuOption previousMenuOption26 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.SelectRight, "#OPTIONS_MENURIGHT", previousMenuOption25);
        KeyboardBindingMenuOption previousMenuOption27 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.MenuInteract, "#OPTIONS_MENUSELECT", previousMenuOption26);
        previousMenuOption22 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.Cancel, "#OPTIONS_MENUCANCEL", previousMenuOption27);
      }
      if (!PunchoutController.IsActive)
        return;
      KeyboardBindingMenuOption previousMenuOption28 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.PunchoutDodgeLeft, "#OPTIONS_PUNCHOUT_DODGELEFT", previousMenuOption22);
      KeyboardBindingMenuOption previousMenuOption29 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.PunchoutDodgeRight, "#OPTIONS_PUNCHOUT_DODGERIGHT", previousMenuOption28);
      KeyboardBindingMenuOption previousMenuOption30 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.PunchoutBlock, "#OPTIONS_PUNCHOUT_BLOCK", previousMenuOption29);
      KeyboardBindingMenuOption previousMenuOption31 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.PunchoutDuck, "#OPTIONS_PUNCHOUT_DUCK", previousMenuOption30);
      KeyboardBindingMenuOption previousMenuOption32 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.PunchoutPunchLeft, "#OPTIONS_PUNCHOUT_PUNCHLEFT", previousMenuOption31);
      KeyboardBindingMenuOption previousMenuOption33 = this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.PunchoutPunchRight, "#OPTIONS_PUNCHOUT_PUNCHRIGHT", previousMenuOption32);
      this.AddKeyboardBindingLine(component.Parent, component.gameObject, GungeonActions.GungeonActionType.PunchoutSuper, "#OPTIONS_PUNCHOUT_SUPER", previousMenuOption33);
    }

    private KeyboardBindingMenuOption AddKeyboardBindingLine(
      dfControl parentPanel,
      GameObject prefabObject,
      GungeonActions.GungeonActionType actionType,
      string CommandStringKey,
      KeyboardBindingMenuOption previousMenuOption,
      bool nonbindable = false)
    {
      dfControl dfControl = parentPanel.AddPrefab(prefabObject);
      dfControl.transform.localScale = prefabObject.transform.localScale;
      KeyboardBindingMenuOption component = dfControl.GetComponent<KeyboardBindingMenuOption>();
      component.ActionType = actionType;
      component.CommandLabel.Text = CommandStringKey;
      component.NonBindable = nonbindable;
      component.KeyButton.GetComponent<UIKeyControls>().up = (dfControl) previousMenuOption.KeyButton;
      component.AltKeyButton.GetComponent<UIKeyControls>().up = (dfControl) previousMenuOption.AltKeyButton;
      previousMenuOption.KeyButton.GetComponent<UIKeyControls>().down = (dfControl) component.KeyButton;
      previousMenuOption.AltKeyButton.GetComponent<UIKeyControls>().down = (dfControl) component.AltKeyButton;
      component.KeyButton.Click += new MouseEventHandler(component.KeyClicked);
      component.AltKeyButton.Click += new MouseEventHandler(component.AltKeyClicked);
      component.Initialize();
      this.m_keyboardBindingLines.Add(component);
      return component;
    }

    public void RegisterItem(BraveOptionsMenuItem item)
    {
      if (this.m_menuItems == null)
        this.m_menuItems = new List<BraveOptionsMenuItem>();
      this.m_menuItems.Add(item);
    }

    public void ReinitializeFromOptions()
    {
      for (int index = 0; index < this.m_menuItems.Count; ++index)
        this.m_menuItems[index].InitializeFromOptions();
    }

    private void ShowOptionsMenu()
    {
      dfGUIManager.PushModal((dfControl) this.m_panel);
      this.cloneOptions = GameOptions.CloneOptions(GameManager.Options);
      if (this.finishedInitialization)
        return;
      this.finishedInitialization = true;
      this.InitializeKeyboardBindingsPanel();
    }

    private void Update()
    {
      if (!this.m_panel.IsVisible)
        return;
      this.HandleLanguageSpecificModifications();
      if ((double) this.m_justResetToDefaultsWithPrompt <= 0.0)
        return;
      this.m_justResetToDefaultsWithPrompt -= GameManager.INVARIANT_DELTA_TIME;
    }

    private void ResetToDefaultsWithPrompt()
    {
      if ((double) this.m_justResetToDefaultsWithPrompt > 0.0)
        return;
      this.m_justResetToDefaultsWithPrompt = 0.25f;
      this.m_cachedFocusedControl = dfGUIManager.ActiveControl;
      this.m_panel.IsVisible = false;
      GameUIRoot.Instance.DoAreYouSure("#AYS_RESETDEFAULTS");
      this.StartCoroutine(this.WaitForAreYouSure((System.Action) (() =>
      {
        this.m_panel.IsVisible = true;
        if ((UnityEngine.Object) this.m_cachedFocusedControl != (UnityEngine.Object) null)
          this.m_cachedFocusedControl.Focus(true);
        this.ResetToDefaults();
      }), (System.Action) (() =>
      {
        this.m_panel.IsVisible = true;
        if (!((UnityEngine.Object) this.m_cachedFocusedControl != (UnityEngine.Object) null))
          return;
        this.m_cachedFocusedControl.Focus(true);
      })));
    }

    private void ResetToDefaults()
    {
      GameManager.Options.RevertToDefaults();
      BraveInput.ResetBindingsToDefaults();
      this.ReinitializeKeyboardBindings();
      this.ReinitializeFromOptions();
    }

    public void CloseAndApplyChangesWithPrompt()
    {
      this.m_cachedFocusedControl = dfGUIManager.ActiveControl;
      this.m_panel.IsVisible = false;
      GameUIRoot.Instance.DoAreYouSure("#AYS_SAVEOPTIONS");
      this.StartCoroutine(this.WaitForAreYouSure(new System.Action(this.CloseAndApplyChanges), (System.Action) (() =>
      {
        this.m_panel.IsVisible = true;
        if (!((UnityEngine.Object) this.m_cachedFocusedControl != (UnityEngine.Object) null))
          return;
        this.m_cachedFocusedControl.Focus(true);
      })));
    }

    public void CloseAndMaybeApplyChangesWithPrompt()
    {
      if (this.cloneOptions == null)
        return;
      SaveManager.TargetSaveSlot = new SaveManager.SaveSlot?();
      BraveInput.SaveBindingInfoToOptions();
      if (GameOptions.CompareSettings(this.cloneOptions, GameManager.Options))
      {
        this.CloseAndRevertChanges();
      }
      else
      {
        this.m_cachedFocusedControl = dfGUIManager.ActiveControl;
        this.m_panel.IsVisible = false;
        GameUIRoot.Instance.DoAreYouSure("#AYS_SAVEOPTIONS");
        this.StartCoroutine(this.WaitForAreYouSure(new System.Action(this.CloseAndApplyChanges), new System.Action(this.CloseAndRevertChanges)));
      }
    }

    [DebuggerHidden]
    private IEnumerator WaitForAreYouSure(System.Action OnYes, System.Action OnNo)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FullOptionsMenuController__WaitForAreYouSurec__Iterator1()
      {
        OnYes = OnYes,
        OnNo = OnNo
      };
    }

    private void CloseAndApplyChanges()
    {
      this.cloneOptions = (GameOptions) null;
      BraveInput.SaveBindingInfoToOptions();
      GameOptions.Save();
      this.UpAllLevels();
    }

    private void CloseAndRevertChangesWithPrompt()
    {
      BraveInput.SaveBindingInfoToOptions();
      if (GameOptions.CompareSettings(this.cloneOptions, GameManager.Options))
      {
        this.CloseAndRevertChanges();
      }
      else
      {
        this.m_cachedFocusedControl = dfGUIManager.ActiveControl;
        this.m_panel.IsVisible = false;
        GameUIRoot.Instance.DoAreYouSure("#AYS_MADECHANGES", true);
        this.StartCoroutine(this.WaitForAreYouSure(new System.Action(this.CloseAndRevertChanges), new System.Action(this.CloseAndApplyChanges)));
      }
    }

    private void CloseAndRevertChanges()
    {
      if (this.cloneOptions != null)
      {
        GameManager.Options.CurrentLanguage = this.cloneOptions.CurrentLanguage;
        GameManager.Options.ApplySettings(this.cloneOptions);
      }
      else
        UnityEngine.Debug.LogError((object) "Clone Options is NULL: this should never happen.");
      this.cloneOptions = (GameOptions) null;
      this.ReinitializeFromOptions();
      StringTableManager.SetNewLanguage(GameManager.Options.CurrentLanguage);
      GameOptions.Save();
      this.UpAllLevels();
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FullOptionsMenuController__Startc__Iterator2()
      {
        _this = this
      };
    }

    private void BottomOptionFocused(dfControl control, dfFocusEventArgs args)
    {
      this.m_lastSelectedBottomRowControl = control;
      if (this.TabAudio.IsVisible)
        this.TabAudio.Controls[this.TabAudio.Controls.Count - 1].GetComponent<BraveOptionsMenuItem>().down = this.m_lastSelectedBottomRowControl;
      else if (this.TabVideo.IsVisible)
        this.TabVideo.Controls[this.TabVideo.Controls.Count - 1].GetComponent<BraveOptionsMenuItem>().down = this.m_lastSelectedBottomRowControl;
      else if (this.TabControls.IsVisible)
        this.TabControls.Controls[this.TabControls.Controls.Count - 2].GetComponent<BraveOptionsMenuItem>().down = this.m_lastSelectedBottomRowControl;
      else if (this.TabGameplay.IsVisible)
      {
        this.TabGameplay.Controls[this.TabGameplay.Controls.Count - 1].GetComponent<BraveOptionsMenuItem>().down = this.m_lastSelectedBottomRowControl;
      }
      else
      {
        if (!this.TabKeyboardBindings.IsVisible)
          return;
        this.TabKeyboardBindings.Controls[this.TabKeyboardBindings.Controls.Count - 1].GetComponent<KeyboardBindingMenuOption>().KeyButton.GetComponent<UIKeyControls>().down = this.m_lastSelectedBottomRowControl;
        this.TabKeyboardBindings.Controls[this.TabKeyboardBindings.Controls.Count - 1].GetComponent<KeyboardBindingMenuOption>().AltKeyButton.GetComponent<UIKeyControls>().down = this.m_lastSelectedBottomRowControl;
      }
    }

    public void UpAllLevels()
    {
      InControlInputAdapter.CurrentlyUsingAllDevices = false;
      if (this.ModalKeyBindingDialog.IsVisible)
        this.ClearModalKeyBindingDialog((dfControl) null, (dfControlEventArgs) null);
      else
        this.PreOptionsMenu.ReturnToPreOptionsMenu();
    }

    public void ToggleToHowToPlay()
    {
      this.TabGameplay.IsVisible = false;
      this.TabHowToPlay.IsVisible = true;
      this.TabHowToPlay.Controls[0].Focus(true);
    }

    public void ToggleToCredits()
    {
      this.TabGameplay.IsVisible = false;
      this.TabCredits.IsVisible = true;
      this.TabCredits.Controls[0].Focus(true);
    }

    public void ToggleToKeyboardBindingsPanel(bool isController)
    {
      this.FullyReinitializeKeyboardBindings();
      this.SwitchBindingsMenuMode(isController);
      this.TabHowToPlay.IsVisible = false;
      this.TabCredits.IsVisible = false;
      this.TabAudio.IsVisible = false;
      this.TabVideo.IsVisible = false;
      this.TabGameplay.IsVisible = false;
      this.TabControls.IsVisible = false;
      this.TabKeyboardBindings.IsVisible = true;
      BraveOptionsMenuItem component1 = this.TabKeyboardBindings.Controls[0].GetComponent<BraveOptionsMenuItem>();
      component1.optionType = FullOptionsMenuController.CurrentBindingPlayerTargetIndex != 0 ? BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET_P2 : BraveOptionsMenuItem.BraveOptionsOptionType.CURRENT_BINDINGS_PRESET;
      component1.InitializeFromOptions();
      KeyboardBindingMenuOption component2 = this.TabKeyboardBindings.Controls[this.TabKeyboardBindings.Controls.Count - 1].GetComponent<KeyboardBindingMenuOption>();
      component2.KeyButton.GetComponent<UIKeyControls>().down = this.m_lastSelectedBottomRowControl ?? (dfControl) this.PrimaryConfirmButton;
      component2.AltKeyButton.GetComponent<UIKeyControls>().down = this.m_lastSelectedBottomRowControl ?? (dfControl) this.PrimaryConfirmButton;
      this.PrimaryCancelButton.GetComponent<UIKeyControls>().up = (dfControl) component2.KeyButton;
      this.PrimaryConfirmButton.GetComponent<UIKeyControls>().up = (dfControl) component2.KeyButton;
      this.PrimaryResetDefaultsButton.GetComponent<UIKeyControls>().up = (dfControl) component2.KeyButton;
      this.PrimaryCancelButton.GetComponent<UIKeyControls>().down = this.TabKeyboardBindings.Controls[0];
      this.PrimaryConfirmButton.GetComponent<UIKeyControls>().down = this.TabKeyboardBindings.Controls[0];
      this.PrimaryResetDefaultsButton.GetComponent<UIKeyControls>().down = this.TabKeyboardBindings.Controls[0];
      this.TabKeyboardBindings.Controls[0].GetComponent<BraveOptionsMenuItem>().up = (dfControl) this.PrimaryConfirmButton;
      this.TabKeyboardBindings.Controls[0].Focus(true);
      if (!PunchoutController.IsActive)
        return;
      this.TabKeyboardBindings.Controls[this.TabKeyboardBindings.Controls.Count - 7].GetComponent<KeyboardBindingMenuOption>().KeyButton.Focus(true);
      this.TabKeyboardBindings.ScrollToBottom();
    }

    public void HandleLanguageSpecificModifications()
    {
      if (!this.m_hasCachedPositions)
      {
        this.m_hasCachedPositions = true;
        this.m_cachedRelativePositionPrimaryConfirm = (Vector2) this.PrimaryConfirmButton.RelativePosition;
        this.m_cachedRelativePositionPrimaryCancel = (Vector2) this.PrimaryCancelButton.RelativePosition;
      }
      if (GameManager.Options.CurrentLanguage == this.m_cachedLanguage)
        return;
      if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.KOREAN || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.CHINESE || GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE)
      {
        this.PrimaryConfirmButton.RelativePosition = (Vector3) this.m_cachedRelativePositionPrimaryConfirm;
        this.PrimaryCancelButton.RelativePosition = (Vector3) this.m_cachedRelativePositionPrimaryCancel;
      }
      else
      {
        this.PrimaryConfirmButton.RelativePosition = (Vector3) (this.m_cachedRelativePositionPrimaryConfirm + new Vector2(15f, 0.0f));
        this.PrimaryCancelButton.RelativePosition = (Vector3) (this.m_cachedRelativePositionPrimaryCancel + new Vector2(-45f, 0.0f));
      }
      this.m_cachedLanguage = GameManager.Options.CurrentLanguage;
    }

    public void ToggleToPanel(dfScrollPanel targetPanel, bool doFocus = false)
    {
      this.IsVisible = true;
      this.TabHowToPlay.IsVisible = false;
      this.TabCredits.IsVisible = false;
      if (this.TabKeyboardBindings.IsVisible)
        BraveInput.SaveBindingInfoToOptions();
      this.TabKeyboardBindings.IsVisible = false;
      this.TabAudio.IsVisible = (UnityEngine.Object) targetPanel == (UnityEngine.Object) this.TabAudio;
      this.TabVideo.IsVisible = (UnityEngine.Object) targetPanel == (UnityEngine.Object) this.TabVideo;
      this.TabGameplay.IsVisible = (UnityEngine.Object) targetPanel == (UnityEngine.Object) this.TabGameplay;
      this.TabControls.IsVisible = (UnityEngine.Object) targetPanel == (UnityEngine.Object) this.TabControls;
      this.PrimaryCancelButton.GetComponent<UIKeyControls>().down = targetPanel.Controls[0];
      this.PrimaryConfirmButton.GetComponent<UIKeyControls>().down = targetPanel.Controls[0];
      this.PrimaryResetDefaultsButton.GetComponent<UIKeyControls>().down = targetPanel.Controls[0];
      targetPanel.Controls[0].Focus(true);
      BraveOptionsMenuItem component1 = targetPanel.Controls[0].GetComponent<BraveOptionsMenuItem>();
      if ((bool) (UnityEngine.Object) component1)
        component1.up = (dfControl) this.PrimaryConfirmButton;
      for (int index = targetPanel.Controls.Count - 1; index > 0; --index)
      {
        BraveOptionsMenuItem component2 = targetPanel.Controls[index].GetComponent<BraveOptionsMenuItem>();
        if ((UnityEngine.Object) component2 != (UnityEngine.Object) null && component2.GetComponent<dfControl>().IsEnabled)
        {
          component2.down = !((UnityEngine.Object) this.m_lastSelectedBottomRowControl != (UnityEngine.Object) null) ? (dfControl) this.PrimaryConfirmButton : this.m_lastSelectedBottomRowControl;
          this.PrimaryCancelButton.GetComponent<UIKeyControls>().up = targetPanel.Controls[index];
          this.PrimaryConfirmButton.GetComponent<UIKeyControls>().up = targetPanel.Controls[index];
          this.PrimaryResetDefaultsButton.GetComponent<UIKeyControls>().up = targetPanel.Controls[index];
          break;
        }
      }
      if (!doFocus)
        return;
      targetPanel.Controls[0].Focus(true);
    }

    public void ShwoopOpen() => this.StartCoroutine(this.HandleShwoop(false));

    [DebuggerHidden]
    private IEnumerator HandleShwoop(bool reverse)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new FullOptionsMenuController__HandleShwoopc__Iterator3()
      {
        reverse = reverse,
        _this = this
      };
    }

    public void ShwoopClosed() => this.StartCoroutine(this.HandleShwoop(true));
  }

