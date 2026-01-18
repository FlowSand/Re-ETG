using InControl;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

#nullable disable

public class MainMenuController : MonoBehaviour
  {
    public dfButton NewGameButton;
    public dfButton CoopGameButton;
    public dfButton NewGameDebugModeButton;
    public dfButton ControlsButton;
    public dfButton PlayVideoButton;
    public dfButton QuitGameButton;
    public dfSprite TEMP_ControlsPrefab;
    public dfSprite TEMP_ControlsSonyPrefab;
    public Image FadeImage;
    public RawImage SizzleImage;
    public AudioClip movieAudio;
    private GameObject m_extantControlsPanel;
    private TempControlsController m_controlsPanelController;

    private void Start()
    {
      GameManager.Instance.TargetQuickRestartLevel = -1;
      PhysicsEngine.Instance = (PhysicsEngine) null;
      Pixelator.Instance = (Pixelator) null;
      GameUIRoot.Instance = (GameUIRoot) null;
      SpawnManager.Instance = (SpawnManager) null;
      Minimap.Instance = (Minimap) null;
      this.NewGameButton.Click += new MouseEventHandler(this.OnNewGameSelected);
      this.CoopGameButton.Click += new MouseEventHandler(this.OnNewCoopGameSelected);
      this.ControlsButton.Click += new MouseEventHandler(this.ShowControlsPanel);
      if ((Object) this.PlayVideoButton != (Object) null)
        this.PlayVideoButton.Click += (MouseEventHandler) ((control, mouseEvent) => this.PlayWindowsMediaPlayerMovie());
      this.QuitGameButton.Click += new MouseEventHandler(this.Quit);
      if ((double) UnityEngine.Time.timeScale == 1.0)
        return;
      BraveTime.ClearAllMultipliers();
    }

    private void OnNewCoopGameSelected(dfControl control, dfMouseEventArgs mouseEvent)
    {
      this.DoQuickStart();
    }

    private void OnStageModeSelected(dfControl control, dfMouseEventArgs mouseEvent)
    {
      GameManager.Instance.CurrentGameType = GameManager.GameType.COOP_2_PLAYER;
      this.NewGameInternal();
    }

    private void OnStageModeBackupSelected(dfControl control, dfMouseEventArgs mouseEvent)
    {
      GameManager.Instance.CurrentGameType = GameManager.GameType.COOP_2_PLAYER;
      this.NewGameInternal();
    }

    private void DoQuickStart()
    {
      GameManager.SKIP_FOYER = true;
      GameManager.Instance.ClearPerLevelData();
      GameManager.Instance.ClearPlayers();
      uint out_bankID = 1;
      int num1 = (int) AkSoundEngine.LoadBank("SFX.bnk", -1, out out_bankID);
      GameManager.PlayerPrefabForNewGame = (GameObject) BraveResources.Load(CharacterSelectController.GetCharacterPathFromQuickStart());
      GameStatsManager.Instance.BeginNewSession(GameManager.PlayerPrefabForNewGame.GetComponent<PlayerController>());
      this.StartCoroutine(this.LerpFadeAlpha(0.0f, 1f, 0.15f));
      GameManager.Instance.FlushAudio();
      GameManager.Instance.GlobalInjectionData.PreprocessRun();
      GameManager.Instance.DelayedLoadNextLevel(0.15f);
      int num2 = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
    }

    private void NewGameInternal()
    {
      this.StartCoroutine(this.LerpFadeAlpha(0.0f, 1f, 0.15f));
      GameManager.Instance.DelayedLoadNextLevel(0.15f);
      int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
    }

    private void OnNewGameSelected(dfControl control, dfMouseEventArgs mouseEvent)
    {
      GameManager.Instance.CurrentGameType = GameManager.GameType.SINGLE_PLAYER;
      this.NewGameInternal();
    }

    [DebuggerHidden]
    private IEnumerator LerpFadeAlpha(float startAlpha, float targetAlpha, float duration)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MainMenuController__LerpFadeAlphac__Iterator0()
      {
        startAlpha = startAlpha,
        targetAlpha = targetAlpha,
        duration = duration,
        _this = this
      };
    }

    private void Update()
    {
      if (InputManager.ActiveDevice != null && InputManager.ActiveDevice.Action4.WasPressed || Input.GetKeyDown(KeyCode.Q))
        this.DoQuickStart();
      if (InputManager.ActiveDevice != null && InputManager.ActiveDevice.LeftStickDown.IsPressed && InputManager.ActiveDevice.RightStickDown.WasPressed)
        this.OnNewCoopGameSelected((dfControl) null, (dfMouseEventArgs) null);
      if (!Input.anyKeyDown || !((Object) this.m_controlsPanelController != (Object) null) || !this.m_controlsPanelController.CanClose || Input.GetMouseButtonDown(0))
        return;
      this.HideControlsPanel();
    }

    private void Quit(dfControl control, dfMouseEventArgs eventArg) => Application.Quit();

    private void PlayWindowsMediaPlayerMovie()
    {
      Process.Start(new ProcessStartInfo("wmplayer.exe", $"\"{Application.streamingAssetsPath + "/SonyVidya.mp4"}\""));
    }

    private void ShowControlsPanel(dfControl control, dfMouseEventArgs eventArg)
    {
      if ((Object) this.m_extantControlsPanel != (Object) null)
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

