using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

using InControl;
using UnityEngine;

using Dungeonator;

#nullable disable

public class PunchoutController : MonoBehaviour
    {
        public static bool IsActive;
        public static bool OverrideControlsButton;
        public static bool InTutorial;
        public static PunchoutController.TutorialControlState[] TutorialControls = new PunchoutController.TutorialControlState[7];
        public static float TutorialUiUpdateTimer;
        public PunchoutPlayerController Player;
        public PunchoutAIActor Opponent;
        public tk2dSprite CoopCultist;
        public AIAnimator TimerAnimator;
        public tk2dTextMesh TimerTextMin1;
        public tk2dTextMesh TimerTextMin2;
        public tk2dTextMesh TimerColon;
        public tk2dTextMesh TimerTextSec1;
        public tk2dTextMesh TimerTextSec2;
        public dfGUIManager UiManager;
        public dfPanel UiPanel;
        public dfSprite PlayerHealthBarBase;
        public dfSprite RatHealthBarBase;
        public dfLabel ControlsLabel;
        public dfLabel TutorialLabel;
        [Header("Rewards")]
        public float NormalHitRewardChance = 1f;
        [PickupIdentifier]
        public int[] NormalHitRewards;
        public int MaxGlassGuonStones = 3;
        [Header("Post-Punchout Stuff")]
        public DungeonPlaceableBehaviour PlayerLostNotePrefab;
        public TalkDoerLite PlayerWonRatNPC;
        [Header("Constants")]
        public float TimerStartTime = 120f;
        private bool m_isFadingControlsUi;
        private Vector2 m_cameraCenterPos;
        private bool m_isInitialized;
        private bool m_tutorialSuperReady;

        public float Timer { get; set; }

        public float HideUiAmount { get; set; }

        public float HideControlsUiAmount { get; set; }

        public float HideTutorialUiAmount { get; set; }

        public bool ShouldDoTutorial
        {
            get => !GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_BOXING_GLOVE);
        }

        [DebuggerHidden]
        public IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutController__Startc__Iterator0()
            {
                _this = this
            };
        }

        public void Update()
        {
            this.UiManager.RenderCamera.enabled = !GameManager.Instance.IsPaused;
            this.Player.ManualUpdate();
            this.Opponent.ManualUpdate();
            if ((double) this.HideControlsUiAmount <= 0.0 && !this.m_isFadingControlsUi && !(this.Opponent.state is PunchoutAIActor.IntroState))
                this.StartCoroutine(this.ControlsUiFadeOutCR());
            GameManager.Instance.MainCameraController.OverridePosition = (Vector3) (this.m_cameraCenterPos + this.Player.CameraOffset + this.Opponent.CameraOffset);
            if (this.Opponent.state is PunchoutAIActor.IntroState)
                this.Timer = this.TimerStartTime;
            else if (!this.Opponent.IsDead && !(this.Opponent.state is PunchoutAIActor.WinState))
                this.Timer = Mathf.Max(0.0f, this.Timer - BraveTime.DeltaTime);
            this.UpdateTimer();
            if (PunchoutController.InTutorial)
            {
                PunchoutController.TutorialUiUpdateTimer -= BraveTime.DeltaTime;
                if ((double) PunchoutController.TutorialUiUpdateTimer < 0.0)
                {
                    this.UpdateTutorialText();
                    PunchoutController.TutorialUiUpdateTimer = 0.5f;
                }
                if (!this.m_tutorialSuperReady)
                {
                    if (PunchoutController.TutorialControls[5] == PunchoutController.TutorialControlState.Completed)
                    {
                        this.m_tutorialSuperReady = true;
                        this.Player.AddStar();
                        PunchoutController.TutorialUiUpdateTimer = 0.0f;
                    }
                }
                else if (PunchoutController.TutorialControls[6] == PunchoutController.TutorialControlState.Completed && this.Player.state == null)
                {
                    PunchoutController.InTutorial = false;
                    this.StartCoroutine(this.TutorialUiFadeCR());
                }
            }
            if ((double) this.Timer <= 0.0)
            {
                if (this.TimerAnimator.IsIdle())
                {
                    this.TimerAnimator.PlayUntilCancelled("explode");
                    this.TimerTextMin1.gameObject.SetActive(false);
                    this.TimerTextMin2.gameObject.SetActive(false);
                    this.TimerColon.gameObject.SetActive(false);
                    this.TimerTextSec1.gameObject.SetActive(false);
                    this.TimerTextSec2.gameObject.SetActive(false);
                }
                if (this.Opponent.state == null)
                {
                    this.Opponent.state = (PunchoutGameActor.State) new PunchoutAIActor.EscapeState();
                    this.Player.Exhaust(new float?(4f));
                }
            }
            this.UpdateUI();
        }

        private void OnDestroy()
        {
            PunchoutController.IsActive = false;
            PunchoutController.OverrideControlsButton = false;
        }

        public void Init()
        {
            switch (GameManager.Instance.PrimaryPlayer.characterIdentity)
            {
                case PlayableCharacters.Pilot:
                    this.Player.SwapPlayer(new int?(3));
                    break;
                case PlayableCharacters.Convict:
                    this.Player.SwapPlayer(new int?(0));
                    break;
                case PlayableCharacters.Robot:
                    this.Player.SwapPlayer(new int?(5));
                    break;
                case PlayableCharacters.Soldier:
                    this.Player.SwapPlayer(new int?(2));
                    break;
                case PlayableCharacters.Guide:
                    this.Player.SwapPlayer(new int?(1));
                    break;
                case PlayableCharacters.Bullet:
                    this.Player.SwapPlayer(new int?(4));
                    break;
                case PlayableCharacters.Eevee:
                    this.Player.SwapPlayer(new int?(7));
                    break;
                case PlayableCharacters.Gunslinger:
                    this.Player.SwapPlayer(new int?(6));
                    break;
                default:
                    this.Player.SwapPlayer(new int?(Random.Range(0, 8)));
                    break;
            }
            this.CoopCultist.gameObject.SetActive(GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER);
            this.StartCoroutine(this.UiFadeInCR());
            this.m_isInitialized = true;
        }

        public void Reset()
        {
            this.Timer = this.TimerStartTime;
            this.TimerAnimator.EndAnimation();
            this.TimerTextMin1.gameObject.SetActive(true);
            this.TimerTextMin2.gameObject.SetActive(true);
            this.TimerColon.gameObject.SetActive(true);
            this.TimerTextSec1.gameObject.SetActive(true);
            this.TimerTextSec2.gameObject.SetActive(true);
            this.Player.SwapPlayer(new int?(Random.Range(0, 8)));
            BraveTime.ClearMultiplier(this.Player.gameObject);
            this.StartCoroutine(this.UiFadeInCR());
            this.HideControlsUiAmount = 0.0f;
            PunchoutController.OverrideControlsButton = true;
            PunchoutController.InTutorial = this.ShouldDoTutorial;
            this.HideTutorialUiAmount = !PunchoutController.InTutorial ? 1f : 0.0f;
            PunchoutController.TutorialControls = new PunchoutController.TutorialControlState[7]
            {
                PunchoutController.TutorialControlState.Shown,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden
            };
            this.m_tutorialSuperReady = false;
            PunchoutController.TutorialUiUpdateTimer = 0.0f;
            this.UiManager.Invalidate();
            this.Opponent.Reset();
        }

        [DebuggerHidden]
        private IEnumerator UiFadeInCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutController__UiFadeInCRc__Iterator1()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator ControlsUiFadeOutCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutController__ControlsUiFadeOutCRc__Iterator2()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator TutorialUiFadeCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutController__TutorialUiFadeCRc__Iterator3()
            {
                _this = this
            };
        }

        public void DoWinFade(bool skipDelay) => this.StartCoroutine(this.DoWinFadeCR(skipDelay));

        [DebuggerHidden]
        private IEnumerator DoWinFadeCR(bool skipDelay)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutController__DoWinFadeCRc__Iterator4()
            {
                skipDelay = skipDelay,
                _this = this
            };
        }

        public void DoLoseFade(bool skipDelay) => this.StartCoroutine(this.DoLoseFadeCR(skipDelay));

        [DebuggerHidden]
        private IEnumerator DoLoseFadeCR(bool skipDelay)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutController__DoLoseFadeCRc__Iterator5()
            {
                skipDelay = skipDelay,
                _this = this
            };
        }

        private void PlaceNPC()
        {
            if (!(bool) (Object) this.PlayerWonRatNPC)
                return;
            RoomHandler currentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;
            bool success = false;
            IntVector2 location = currentRoom.GetCenteredVisibleClearSpot(3, 3, out success, true) - currentRoom.area.basePosition + IntVector2.One;
            if (!success)
                return;
            GameObject gObj = DungeonPlaceableUtility.InstantiateDungeonPlaceable(this.PlayerWonRatNPC.gameObject, currentRoom, location, false);
            if (!(bool) (Object) gObj)
                return;
            foreach (IPlayerInteractable interfacesInChild in gObj.GetInterfacesInChildren<IPlayerInteractable>())
                currentRoom.RegisterInteractable(interfacesInChild);
        }

        private void PlaceNote(GameObject notePrefab)
        {
            if (!((Object) notePrefab != (Object) null))
                return;
            RoomHandler currentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;
            bool success = false;
            IntVector2 location = currentRoom.GetCenteredVisibleClearSpot(3, 3, out success, true) - currentRoom.area.basePosition + IntVector2.One;
            if (!success)
                return;
            GameObject gObj = DungeonPlaceableUtility.InstantiateDungeonPlaceable(notePrefab.gameObject, currentRoom, location, false);
            if (!(bool) (Object) gObj)
                return;
            foreach (IPlayerInteractable interfacesInChild in gObj.GetInterfacesInChildren<IPlayerInteractable>())
                currentRoom.RegisterInteractable(interfacesInChild);
        }

        public void DoBombFade() => this.StartCoroutine(this.DoBombFadeCR());

        [DebuggerHidden]
        private IEnumerator DoBombFadeCR()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PunchoutController__DoBombFadeCRc__Iterator6()
            {
                _this = this
            };
        }

        private void InitPunchout()
        {
            if (Minimap.HasInstance)
            {
                Minimap.Instance.TemporarilyPreventMinimap = true;
                GameUIRoot.Instance.HideCoreUI("punchout");
                GameUIRoot.Instance.ToggleLowerPanels(false, source: string.Empty);
                this.m_cameraCenterPos = GameManager.Instance.BestActivePlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox) + new Vector2(0.0f, -25f);
                this.transform.position = (Vector3) (this.m_cameraCenterPos - PhysicsEngine.PixelToUnit(new IntVector2(240 /*0xF0*/, 130)));
                foreach (tk2dBaseSprite componentsInChild in this.GetComponentsInChildren<tk2dBaseSprite>())
                    componentsInChild.UpdateZDepth();
                CameraController cameraController = GameManager.Instance.MainCameraController;
                cameraController.OverridePosition = (Vector3) this.m_cameraCenterPos;
                cameraController.SetManualControl(true, false);
                cameraController.LockToRoom = true;
                cameraController.SetZoomScaleImmediate(1.6f);
                foreach (PlayerController allPlayer in GameManager.Instance.AllPlayers)
                {
                    allPlayer.SetInputOverride("punchout");
                    allPlayer.healthHaver.IsVulnerable = false;
                    allPlayer.SuppressEffectUpdates = true;
                    allPlayer.IsOnFire = false;
                    allPlayer.CurrentFireMeterValue = 0.0f;
                    allPlayer.CurrentPoisonMeterValue = 0.0f;
                    if ((bool) (Object) allPlayer.specRigidbody)
                        DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(allPlayer.specRigidbody.UnitCenter, 1f);
                }
                GameManager.Instance.DungeonMusicController.SwitchToBossMusic("Play_MUS_RatPunch_Intro_01", this.gameObject);
                foreach (ParticleSystem componentsInChild in this.transform.Find("arena").GetComponentsInChildren<ParticleSystem>())
                    componentsInChild.transform.position = componentsInChild.transform.position.XY().ToVector3ZisY();
                foreach (Light componentsInChild in this.transform.Find("arena").GetComponentsInChildren<Light>())
                    componentsInChild.transform.position = componentsInChild.transform.position.XY().ToVector3ZisY(-18f);
            }
            else
            {
                int num = (int) AkSoundEngine.PostEvent("Play_MUS_RatPunch_Intro_01", this.gameObject);
            }
            PunchoutController.InTutorial = this.ShouldDoTutorial;
            this.HideTutorialUiAmount = !PunchoutController.InTutorial ? 1f : 0.0f;
            PunchoutController.TutorialControls = new PunchoutController.TutorialControlState[7]
            {
                PunchoutController.TutorialControlState.Shown,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden,
                PunchoutController.TutorialControlState.Hidden
            };
            PunchoutController.OverrideControlsButton = true;
        }

        private void TeardownPunchout()
        {
            if (this.m_isInitialized)
            {
                Minimap.Instance.TemporarilyPreventMinimap = false;
                GameUIRoot.Instance.ShowCoreUI("punchout");
                GameUIRoot.Instance.ShowCoreUI(string.Empty);
                GameUIRoot.Instance.ToggleLowerPanels(true, source: string.Empty);
                CameraController cameraController = GameManager.Instance.MainCameraController;
                cameraController.SetManualControl(false, false);
                cameraController.LockToRoom = false;
                cameraController.SetZoomScaleImmediate(1f);
                foreach (PlayerController allPlayer in GameManager.Instance.AllPlayers)
                {
                    allPlayer.ClearInputOverride("punchout");
                    allPlayer.healthHaver.IsVulnerable = true;
                    allPlayer.SuppressEffectUpdates = false;
                    allPlayer.IsOnFire = false;
                    allPlayer.CurrentFireMeterValue = 0.0f;
                    allPlayer.CurrentPoisonMeterValue = 0.0f;
                }
                GameManager.Instance.DungeonMusicController.EndBossMusic();
                MetalGearRatRoomController objectOfType = Object.FindObjectOfType<MetalGearRatRoomController>();
                if ((bool) (Object) objectOfType)
                {
                    GameObject gameObject = PickupObjectDatabase.GetById(GlobalItemIds.RatKey).gameObject;
                    Vector3 position = objectOfType.transform.position;
                    if (this.Opponent.NumKeysDropped >= 1)
                        LootEngine.SpawnItem(gameObject, position + new Vector3(14.25f, 17f), Vector2.zero, 0.0f);
                    if (this.Opponent.NumKeysDropped >= 2)
                        LootEngine.SpawnItem(gameObject, position + new Vector3(13.25f, 14.5f), Vector2.zero, 0.0f);
                    if (this.Opponent.NumKeysDropped >= 3)
                        LootEngine.SpawnItem(gameObject, position + new Vector3(14.25f, 12f), Vector2.zero, 0.0f);
                    if (this.Opponent.NumKeysDropped >= 4)
                        LootEngine.SpawnItem(gameObject, position + new Vector3(30.25f, 17f), Vector2.zero, 0.0f);
                    if (this.Opponent.NumKeysDropped >= 5)
                        LootEngine.SpawnItem(gameObject, position + new Vector3(31.25f, 14.5f), Vector2.zero, 0.0f);
                    if (this.Opponent.NumKeysDropped >= 6)
                        LootEngine.SpawnItem(gameObject, position + new Vector3(30.25f, 12f), Vector2.zero, 0.0f);
                    Vector2 vector2 = (Vector2) (position + new Vector3(22.25f, 14.5f));
                    foreach (int droppedRewardId in this.Opponent.DroppedRewardIds)
                    {
                        float degrees = (!BraveUtility.RandomBool() ? 180f : 0.0f) + Random.Range(-30f, 30f);
                        LootEngine.SpawnItem(PickupObjectDatabase.GetById(droppedRewardId).gameObject, (Vector3) (vector2 + new Vector2(11f, 0.0f).Rotate(degrees)), Vector2.zero, 0.0f);
                    }
                }
                GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_BOXING_GLOVE, true);
                BraveTime.ClearMultiplier(this.Player.gameObject);
                Object.Destroy((Object) this.gameObject);
            }
            else
                this.Reset();
        }

        private void UpdateUI()
        {
            string str = this.ControlsLabel.ForceGetLocalizedValue("#MAINMENU_CONTROLS");
            if (str == "CONTROLS")
                str = "Controls";
            this.ControlsLabel.Text = $"{str} ({StringTableManager.EvaluateReplacementToken("%CONTROL_PAUSE")})";
            float scaleTileScale = Pixelator.Instance.ScaleTileScale;
            this.PlayerHealthBarBase.transform.localScale = new Vector3(scaleTileScale, scaleTileScale, 1f);
            this.RatHealthBarBase.transform.localScale = new Vector3(scaleTileScale, scaleTileScale, 1f);
            this.ControlsLabel.transform.localScale = new Vector3(scaleTileScale, scaleTileScale, 1f);
            this.TutorialLabel.transform.localScale = new Vector3(scaleTileScale, scaleTileScale, 1f);
            float num1 = (this.PlayerHealthBarBase.Height + 8f) * scaleTileScale * this.HideUiAmount;
            this.PlayerHealthBarBase.Position = new Vector3(4f * scaleTileScale, -8f * scaleTileScale + num1);
            this.RatHealthBarBase.Position = new Vector3((float) ((double) this.UiPanel.Size.x - (double) this.RatHealthBarBase.Size.x - 4.0 * (double) scaleTileScale), -8f * scaleTileScale + num1);
            float num2 = (float) -((double) this.ControlsLabel.Height * (double) scaleTileScale + 8.0) * scaleTileScale * this.HideControlsUiAmount;
            this.ControlsLabel.Position = new Vector3((float) ((double) this.UiPanel.Size.x - (double) this.ControlsLabel.Size.x * (double) scaleTileScale - 4.0 * (double) scaleTileScale), (float) (-(double) this.UiPanel.Size.y + (double) this.ControlsLabel.Size.y + 4.0 * (double) scaleTileScale) + num2);
            float num3 = (float) -((double) this.TutorialLabel.Width + 4.0) * scaleTileScale * this.HideTutorialUiAmount;
            this.TutorialLabel.Position = new Vector3(scaleTileScale * 4f + num3, (float) (-(double) this.UiPanel.Size.y + (double) this.TutorialLabel.Size.y + 4.0 * (double) scaleTileScale));
        }

        private void UpdateTutorialText()
        {
            StringBuilder str = new StringBuilder();
            this.HandleTutorialLine(str, 0, "#OPTIONS_PUNCHOUT_DODGELEFT", GungeonActions.GungeonActionType.PunchoutDodgeLeft);
            this.HandleTutorialLine(str, 1, "#OPTIONS_PUNCHOUT_DODGERIGHT", GungeonActions.GungeonActionType.PunchoutDodgeRight);
            this.HandleTutorialLine(str, 2, "#OPTIONS_PUNCHOUT_BLOCK", GungeonActions.GungeonActionType.PunchoutBlock);
            this.HandleTutorialLine(str, 3, "#OPTIONS_PUNCHOUT_DUCK", GungeonActions.GungeonActionType.PunchoutDuck);
            this.HandleTutorialLine(str, 4, "#OPTIONS_PUNCHOUT_PUNCHLEFT", GungeonActions.GungeonActionType.PunchoutPunchLeft);
            this.HandleTutorialLine(str, 5, "#OPTIONS_PUNCHOUT_PUNCHRIGHT", GungeonActions.GungeonActionType.PunchoutPunchRight);
            this.HandleTutorialLine(str, 6, "#OPTIONS_PUNCHOUT_SUPER", GungeonActions.GungeonActionType.PunchoutSuper);
            this.TutorialLabel.Text = str.ToString();
        }

        public static void InputWasPressed(int action)
        {
            if (PunchoutController.TutorialControls[action] != PunchoutController.TutorialControlState.Shown)
                return;
            PunchoutController.TutorialControls[action] = PunchoutController.TutorialControlState.Completed;
            if (action < PunchoutController.TutorialControls.Length - 1)
                PunchoutController.TutorialControls[action + 1] = PunchoutController.TutorialControlState.Shown;
            PunchoutController.TutorialUiUpdateTimer = 0.0f;
        }

        private void HandleTutorialLine(
            StringBuilder str,
            int i,
            string commandName,
            GungeonActions.GungeonActionType action)
        {
            if (PunchoutController.TutorialControls[i] == PunchoutController.TutorialControlState.Hidden)
            {
                str.AppendLine();
            }
            else
            {
                bool flag = PunchoutController.TutorialControls[i] == PunchoutController.TutorialControlState.Completed;
                if (flag)
                    str.Append("[color green]");
                str.Append(this.TutorialLabel.ForceGetLocalizedValue(commandName));
                if (flag)
                    str.Append("[/color]");
                str.Append(" (").Append(this.GetTutorialText(action)).AppendLine(")");
            }
        }

        private string GetTutorialText(GungeonActions.GungeonActionType action)
        {
            BraveInput primaryPlayerInstance = BraveInput.PrimaryPlayerInstance;
            if (primaryPlayerInstance.IsKeyboardAndMouse())
                return StringTableManager.GetBindingText(action);
            if ((Object) primaryPlayerInstance != (Object) null)
            {
                ReadOnlyCollection<BindingSource> bindings = primaryPlayerInstance.ActiveActions.GetActionFromType(action).Bindings;
                if (bindings.Count > 0)
                {
                    for (int index = 0; index < bindings.Count; ++index)
                    {
                        DeviceBindingSource deviceBindingSource = bindings[index] as DeviceBindingSource;
                        if ((BindingSource) deviceBindingSource != (BindingSource) null && deviceBindingSource.Control != InputControlType.None)
                            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(deviceBindingSource.Control, BraveInput.PlayerOneCurrentSymbology);
                    }
                }
            }
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Start, BraveInput.PlayerOneCurrentSymbology);
        }

        private void UpdateTimer()
        {
            int num1 = Mathf.CeilToInt(this.Timer);
            int num2 = (int) ((double) num1 / 60.0);
            int num3 = num1 - num2 * 60;
            this.TimerTextMin1.text = (num2 / 10).ToString();
            this.TimerTextMin2.text = (num2 % 10).ToString();
            this.TimerTextSec1.text = (num3 / 10).ToString();
            this.TimerTextSec2.text = (num3 % 10).ToString();
        }

        public enum TutorialControlState
        {
            Hidden,
            Shown,
            Completed,
        }
    }

