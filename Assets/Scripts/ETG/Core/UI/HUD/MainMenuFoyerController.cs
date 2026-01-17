// Decompiled with JetBrains decompiler
// Type: MainMenuFoyerController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using InControl;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

namespace ETG.Core.UI.HUD
{
    public class MainMenuFoyerController : MonoBehaviour
    {
      public dfButton NewGameButton;
      public dfButton ControlsButton;
      public dfButton XboxLiveButton;
      public dfButton QuitGameButton;
      public dfButton ContinueGameButton;
      [FormerlySerializedAs("BetaLabel")]
      public dfLabel VersionLabel;
      public dfControl TitleCard;
      public dfSprite TEMP_ControlsPrefab;
      public dfSprite TEMP_ControlsSonyPrefab;
      private GameObject m_extantControlsPanel;
      private TempControlsController m_controlsPanelController;
      private dfGUIManager m_guiManager;
      private bool Initialized;
      private TitleDioramaController m_tdc;
      private float m_timeWithoutInput;
      private Vector3 m_cachedMousePosition;
      private bool m_faded;
      private bool m_wasFadedThisFrame;
      private float c_fadeTimer = 20f;
      private bool m_optionsOpen;
      private float m_cachedDepth = -1f;

      private void Awake()
      {
        this.m_guiManager = this.GetComponent<dfGUIManager>();
        this.NewGameButton.forceUpperCase = true;
        this.ControlsButton.forceUpperCase = true;
        this.XboxLiveButton.forceUpperCase = true;
        this.QuitGameButton.forceUpperCase = true;
        this.ContinueGameButton.forceUpperCase = true;
        this.NewGameButton.ModifyLocalizedText(this.NewGameButton.Text.ToUpperInvariant());
        this.ControlsButton.ModifyLocalizedText(this.ControlsButton.Text.ToUpperInvariant());
        this.XboxLiveButton.ModifyLocalizedText(this.XboxLiveButton.Text.ToUpperInvariant());
        this.QuitGameButton.ModifyLocalizedText(this.QuitGameButton.Text.ToUpperInvariant());
        List<dfButton> dfButtonList = new List<dfButton>();
        dfButtonList.Add(this.ContinueGameButton);
        if (GameManager.HasValidMidgameSave())
        {
          this.ContinueGameButton.IsEnabled = true;
          this.ContinueGameButton.IsVisible = true;
        }
        else
        {
          this.ContinueGameButton.IsEnabled = false;
          this.ContinueGameButton.IsVisible = false;
        }
        dfButtonList.Add(this.NewGameButton);
        dfButtonList.Add(this.ControlsButton);
        this.XboxLiveButton.IsEnabled = false;
        this.XboxLiveButton.IsVisible = false;
        dfButtonList.Add(this.QuitGameButton);
        int count = dfButtonList.Count;
        if (count > 0)
        {
          dfButton dfButton1 = dfButtonList[count - 1];
          for (int index = 0; index < dfButtonList.Count; ++index)
          {
            dfButton dfButton2 = dfButtonList[index];
            dfButton2.GetComponent<UIKeyControls>().up = (dfControl) dfButton1;
            dfButton1.GetComponent<UIKeyControls>().down = (dfControl) dfButton2;
            dfButton1 = dfButton2;
          }
        }
        this.FixButtonPositions();
        if (!Foyer.DoMainMenu)
        {
          this.NewGameButton.GUIManager.RenderCamera.enabled = false;
          int num = (int) AkSoundEngine.PostEvent("Play_MUS_State_Reset", this.gameObject);
        }
        this.VersionLabel.Text = VersionManager.DisplayVersionNumber;
      }

      private void FixButtonPositions()
      {
        this.NewGameButton.RelativePosition = this.NewGameButton.RelativePosition.WithX(this.QuitGameButton.RelativePosition.x).WithY(this.QuitGameButton.RelativePosition.y - 153f);
        this.ControlsButton.RelativePosition = this.ControlsButton.RelativePosition.WithX(this.QuitGameButton.RelativePosition.x).WithY(this.QuitGameButton.RelativePosition.y - 102f);
        this.XboxLiveButton.RelativePosition = this.XboxLiveButton.RelativePosition.WithX(this.QuitGameButton.RelativePosition.x).WithY(this.QuitGameButton.RelativePosition.y - 51f);
        this.ContinueGameButton.RelativePosition = this.ContinueGameButton.RelativePosition.WithX(this.QuitGameButton.RelativePosition.x).WithY(this.QuitGameButton.RelativePosition.y - 204f);
        if (this.XboxLiveButton.IsEnabled)
          return;
        dfButton continueGameButton = this.ContinueGameButton;
        continueGameButton.RelativePosition = continueGameButton.RelativePosition + new Vector3(0.0f, 51f, 0.0f);
        dfButton newGameButton = this.NewGameButton;
        newGameButton.RelativePosition = newGameButton.RelativePosition + new Vector3(0.0f, 51f, 0.0f);
        dfButton controlsButton = this.ControlsButton;
        controlsButton.RelativePosition = controlsButton.RelativePosition + new Vector3(0.0f, 51f, 0.0f);
      }

      public void InitializeMainMenu()
      {
        GameManager.Instance.TargetQuickRestartLevel = -1;
        GameUIRoot.Instance.Manager.RenderCamera.enabled = false;
        this.FixButtonPositions();
        if (!this.Initialized)
        {
          this.NewGameButton.GotFocus += new FocusEventHandler(this.PlayFocusNoise);
          this.ControlsButton.GotFocus += new FocusEventHandler(this.PlayFocusNoise);
          this.XboxLiveButton.GotFocus += new FocusEventHandler(this.PlayFocusNoise);
          this.QuitGameButton.GotFocus += new FocusEventHandler(this.PlayFocusNoise);
          this.ContinueGameButton.GotFocus += new FocusEventHandler(this.PlayFocusNoise);
          this.NewGameButton.Click += new MouseEventHandler(this.OnNewGameSelected);
          this.ContinueGameButton.Click += new MouseEventHandler(this.OnContinueGameSelected);
          if (GameManager.HasValidMidgameSave())
            this.ContinueGameButton.Focus(true);
          this.ControlsButton.Click += new MouseEventHandler(this.ShowOptionsPanel);
          this.XboxLiveButton.Click += new MouseEventHandler(this.SignInToPlatform);
          this.QuitGameButton.Click += new MouseEventHandler(this.Quit);
          this.Initialized = true;
        }
        if ((double) UnityEngine.Time.timeScale == 1.0)
          return;
        BraveTime.ClearAllMultipliers();
      }

      private void PlayFocusNoise(dfControl control, dfFocusEventArgs args)
      {
        if (!Foyer.DoMainMenu)
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", GameManager.Instance.gameObject);
      }

      public void UpdateMainMenuText()
      {
        if (GameManager.HasValidMidgameSave())
        {
          this.ContinueGameButton.IsEnabled = true;
          this.ContinueGameButton.IsVisible = true;
        }
        else
        {
          this.ContinueGameButton.IsEnabled = false;
          this.ContinueGameButton.IsVisible = false;
        }
      }

      public void DisableMainMenu()
      {
        BraveCameraUtility.OverrideAspect = new float?();
        GameUIRoot.Instance.Manager.RenderCamera.enabled = true;
        this.NewGameButton.GUIManager.RenderCamera.enabled = false;
        this.NewGameButton.GUIManager.enabled = false;
        this.NewGameButton.Click -= new MouseEventHandler(this.OnNewGameSelected);
        this.ControlsButton.Click -= new MouseEventHandler(this.ShowOptionsPanel);
        this.XboxLiveButton.Click -= new MouseEventHandler(this.SignInToPlatform);
        this.QuitGameButton.Click -= new MouseEventHandler(this.Quit);
        this.NewGameButton.IsInteractive = false;
        this.ControlsButton.IsInteractive = false;
        this.XboxLiveButton.IsInteractive = false;
        this.QuitGameButton.IsInteractive = false;
        if ((bool) (Object) this.NewGameButton && (bool) (Object) this.NewGameButton.GetComponent<UIKeyControls>())
          this.NewGameButton.GetComponent<UIKeyControls>().enabled = false;
        if ((bool) (Object) this.ControlsButton && (bool) (Object) this.ControlsButton.GetComponent<UIKeyControls>())
          this.ControlsButton.GetComponent<UIKeyControls>().enabled = false;
        if ((bool) (Object) this.XboxLiveButton && (bool) (Object) this.XboxLiveButton.GetComponent<UIKeyControls>())
          this.XboxLiveButton.GetComponent<UIKeyControls>().enabled = false;
        if ((bool) (Object) this.QuitGameButton && (bool) (Object) this.QuitGameButton.GetComponent<UIKeyControls>())
          this.QuitGameButton.GetComponent<UIKeyControls>().enabled = false;
        ShadowSystem.ForceAllLightsUpdate();
      }

      private void NewGameInternal()
      {
        this.DisableMainMenu();
        Pixelator.Instance.FadeToBlack(0.15f, true, 0.05f);
        GameManager.Instance.FlushAudio();
        Foyer.DoIntroSequence = false;
        Foyer.DoMainMenu = false;
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
      }

      private bool IsDioramaRevealed(bool doReveal = false)
      {
        if ((Object) this.m_tdc == (Object) null)
          this.m_tdc = Object.FindObjectOfType<TitleDioramaController>();
        return !(bool) (Object) this.m_tdc || this.m_tdc.IsRevealed(doReveal);
      }

      private void OnContinueGameSelected(dfControl control, dfMouseEventArgs mouseEvent)
      {
        MidGameSaveData.ContinuePressedDevice = InputManager.ActiveDevice;
        if (this.m_faded || this.m_wasFadedThisFrame || !this.IsDioramaRevealed(true) || !Foyer.DoMainMenu)
          return;
        MidGameSaveData midgameSave = (MidGameSaveData) null;
        GameManager.VerifyAndLoadMidgameSave(out midgameSave);
        Dungeon.ShouldAttemptToLoadFromMidgameSave = true;
        this.DisableMainMenu();
        Pixelator.Instance.FadeToBlack(0.15f, holdTime: 0.05f);
        GameManager.Instance.FlushAudio();
        int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
        GameManager.Instance.SetNextLevelIndex(GameManager.Instance.GetTargetLevelIndexFromSavedTileset(midgameSave.levelSaved));
        GameManager.Instance.GeneratePlayersFromMidGameSave(midgameSave);
        GameManager.Instance.IsFoyer = false;
        Foyer.DoIntroSequence = false;
        Foyer.DoMainMenu = false;
        GameManager.Instance.IsSelectingCharacter = false;
        GameManager.Instance.DelayedLoadMidgameSave(0.25f, midgameSave);
      }

      private void OnNewGameSelected(dfControl control, dfMouseEventArgs mouseEvent)
      {
        if (this.m_faded || this.m_wasFadedThisFrame || !this.IsDioramaRevealed(true))
          return;
        GameManager.Instance.CurrentGameType = GameManager.GameType.SINGLE_PLAYER;
        this.NewGameInternal();
        GameManager.Instance.InjectedFlowPath = (string) null;
      }

      [DebuggerHidden]
      private IEnumerator ToggleFade(bool targetFade)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MainMenuFoyerController.<ToggleFade>c__Iterator0()
        {
          targetFade = targetFade,
          _this = this
        };
      }

      private void Update()
      {
        this.m_wasFadedThisFrame = this.m_faded;
        if ((bool) (Object) this.m_guiManager && !GameManager.Instance.IsLoadingLevel)
          this.m_guiManager.UIScale = Pixelator.Instance.ScaleTileScale / 3f;
        if (!Foyer.DoMainMenu && !Foyer.DoIntroSequence && !GameManager.Instance.IsSelectingCharacter && !GameManager.IsReturningToBreach)
          return;
        if (this.IsDioramaRevealed())
          this.m_timeWithoutInput += GameManager.INVARIANT_DELTA_TIME;
        if (Input.anyKeyDown || Input.mousePosition != this.m_cachedMousePosition)
          this.m_timeWithoutInput = 0.0f;
        this.m_cachedMousePosition = Input.mousePosition;
        if ((bool) (Object) BraveInput.PlayerlessInstance && BraveInput.PlayerlessInstance.ActiveActions != null && BraveInput.PlayerlessInstance.ActiveActions.AnyActionPressed())
          this.m_timeWithoutInput = 0.0f;
        if (GameManager.Instance.PREVENT_MAIN_MENU_TEXT)
        {
          this.NewGameButton.Opacity = 0.0f;
          this.ControlsButton.Opacity = 0.0f;
          this.XboxLiveButton.Opacity = 0.0f;
          this.QuitGameButton.Opacity = 0.0f;
          this.VersionLabel.Opacity = 0.0f;
          this.TitleCard.Opacity = 0.0f;
        }
        else if ((double) this.m_timeWithoutInput > (double) this.c_fadeTimer && !this.m_faded)
          this.StartCoroutine(this.ToggleFade(true));
        else if ((double) this.m_timeWithoutInput < (double) this.c_fadeTimer && this.m_faded)
          this.StartCoroutine(this.ToggleFade(false));
        if (Foyer.DoMainMenu && !this.m_optionsOpen && (!this.IsDioramaRevealed() || !this.NewGameButton.HasFocus && !this.ControlsButton.HasFocus && !this.XboxLiveButton.HasFocus && !this.QuitGameButton.HasFocus && !this.ContinueGameButton.HasFocus))
        {
          dfGUIManager.PopModalToControl((dfControl) null, false);
          if (this.ContinueGameButton.IsEnabled && this.ContinueGameButton.IsVisible)
            this.ContinueGameButton.Focus(true);
          else
            this.NewGameButton.Focus(true);
        }
        if (this.m_optionsOpen && !GameUIRoot.Instance.AreYouSurePanel.IsVisible)
        {
          if ((bool) (Object) BraveInput.PlayerlessInstance && BraveInput.PlayerlessInstance.ActiveActions != null && BraveInput.PlayerlessInstance.ActiveActions.CancelAction.WasPressed)
          {
            if (!GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().OptionsMenu.ModalKeyBindingDialog.IsVisible)
              this.HideOptionsPanel();
          }
          else if (Input.GetKeyDown(KeyCode.Escape))
            this.HideOptionsPanel();
        }
        if (!Input.anyKeyDown || !((Object) this.m_controlsPanelController != (Object) null) || !this.m_controlsPanelController.CanClose || Input.GetMouseButtonDown(0))
          return;
        this.HideControlsPanel();
      }

      private void SignInToPlatform(dfControl control, dfMouseEventArgs eventArg)
      {
        GameManager.Instance.platformInterface.SignIn();
      }

      private void Quit(dfControl control, dfMouseEventArgs eventArg)
      {
        if (this.m_faded || this.m_wasFadedThisFrame || !this.IsDioramaRevealed(true) || !Foyer.DoMainMenu)
          return;
        Application.Quit();
      }

      private void ShowOptionsPanel(dfControl control, dfMouseEventArgs eventArg)
      {
        if (this.m_faded || this.m_wasFadedThisFrame || !this.IsDioramaRevealed(true) || !Foyer.DoMainMenu)
          return;
        this.m_optionsOpen = true;
        this.m_cachedDepth = GameUIRoot.Instance.Manager.RenderCamera.depth;
        GameUIRoot.Instance.Manager.RenderCamera.depth += 10f;
        GameUIRoot.Instance.Manager.RenderCamera.enabled = true;
        GameUIRoot.Instance.Manager.overrideClearFlags = CameraClearFlags.Color;
        GameUIRoot.Instance.Manager.RenderCamera.backgroundColor = Color.black;
        PauseMenuController component = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>();
        if (!((Object) component != (Object) null))
          return;
        component.OptionsMenu.PreOptionsMenu.IsVisible = true;
      }

      private void HideOptionsPanel()
      {
        PauseMenuController component = GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>();
        if ((Object) component != (Object) null && !GameUIRoot.Instance.AreYouSurePanel.IsVisible)
        {
          if (component.OptionsMenu.IsVisible)
          {
            component.OptionsMenu.CloseAndMaybeApplyChangesWithPrompt();
          }
          else
          {
            this.m_optionsOpen = false;
            GameUIRoot.Instance.Manager.RenderCamera.depth = this.m_cachedDepth;
            GameUIRoot.Instance.Manager.RenderCamera.enabled = false;
            GameUIRoot.Instance.Manager.overrideClearFlags = CameraClearFlags.Depth;
            if ((Object) component != (Object) null)
              component.OptionsMenu.PreOptionsMenu.IsVisible = false;
          }
        }
        BraveInput.SavePlayerlessBindingsToOptions();
      }

      private void ShowControlsPanel(dfControl control, dfMouseEventArgs eventArg)
      {
        if (!Foyer.DoMainMenu || (Object) this.m_extantControlsPanel != (Object) null)
          return;
        GameObject gameObject1 = this.TEMP_ControlsPrefab.gameObject;
        if (!BraveInput.GetInstanceForPlayer(0).IsKeyboardAndMouse())
          gameObject1 = this.TEMP_ControlsSonyPrefab.gameObject;
        GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject1);
        this.m_extantControlsPanel = gameObject2;
        this.m_controlsPanelController = gameObject2.GetComponent<TempControlsController>();
        this.NewGameButton.GetManager().AddControl((dfControl) gameObject2.GetComponent<dfSprite>());
      }

      private void HideControlsPanel()
      {
        if (!((Object) this.m_extantControlsPanel != (Object) null))
          return;
        this.m_controlsPanelController = (TempControlsController) null;
        Object.Destroy((Object) this.m_extantControlsPanel);
      }
    }

}
