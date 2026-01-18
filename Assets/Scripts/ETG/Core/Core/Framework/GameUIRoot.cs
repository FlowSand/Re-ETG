using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class GameUIRoot : TimeInvariantMonoBehaviour
    {
        public static GameUIRoot m_root;
        public dfLabel p_playerAmmoLabel;
        public dfLabel FoyerAmmonomiconLabel;
        public List<GameUIHeartController> heartControllers;
        public List<GameUIAmmoController> ammoControllers;
        public List<GameUIItemController> itemControllers;
        public List<GameUIBlankController> blankControllers;
        public GameUIBossHealthController bossController;
        public GameUIBossHealthController bossController2;
        public GameUIBossHealthController bossControllerSide;
        public UINotificationController notificationController;
        public dfPanel AreYouSurePanel;
        public bool KeepMetasIsVisible;
        public dfLabel p_playerCoinLabel;
        public dfLabel p_playerKeyLabel;
        public dfSprite p_specialKeySprite;
        [NonSerialized]
        private List<dfSprite> m_extantSpecialKeySprites = new List<dfSprite>();
        public dfLabel p_needsReloadLabel;
        [NonSerialized]
        private List<dfLabel> m_extantReloadLabels;
        public List<dfLabel> gunNameLabels;
        public List<dfLabel> itemNameLabels;
        public LevelNameUIManager levelNameUI;
        public GameUIReloadBarController p_playerReloadBar;
        public GameUIReloadBarController p_secondaryPlayerReloadBar;
        [NonSerialized]
        private List<GameUIReloadBarController> m_extantReloadBars;
        public GameObject undiePanel;
        [Header("Dynamism")]
        [NonSerialized]
        private List<dfControl> customNonCoreMotionGroups = new List<dfControl>();
        public List<dfControl> motionGroups;
        public List<DungeonData.Direction> motionDirections;
        [NonSerialized]
        private List<dfControl> lockedMotionGroups = new List<dfControl>();
        [NonSerialized]
        protected Dictionary<dfControl, Vector3> motionInteriorPositions;
        [NonSerialized]
        protected Dictionary<dfControl, Vector3> motionExteriorPositions;
        [NonSerialized]
        public List<DefaultLabelController> extantBasicLabels = new List<DefaultLabelController>();
        [Header("Demo Tutorial Panels")]
        public List<dfPanel> demoTutorialPanels_Keyboard;
        public List<dfPanel> demoTutorialPanels_Controller;
        private bool m_forceHideGunPanel;
        private bool m_forceHideItemPanel;
        private List<bool> m_displayingReloadNeeded;
        private bool m_bossKillCamActive;
        [NonSerialized]
        private float[] m_gunNameVisibilityTimers;
        [NonSerialized]
        private float[] m_itemNameVisibilityTimers;
        private List<dfLabel> m_inactiveDamageLabels = new List<dfLabel>();
        private OverridableBool m_defaultLabelsHidden = new OverridableBool(false);
        private float MotionGroupBufferWidth = 21f;
        private List<dfSprite> additionalGunBoxes = new List<dfSprite>();
        private List<dfSprite> additionalItemBoxes = new List<dfSprite>();
        private List<dfSprite> additionalGunBoxesSecondary = new List<dfSprite>();
        private List<dfSprite> additionalItemBoxesSecondary = new List<dfSprite>();
        protected OverridableBool CoreUIHidden = new OverridableBool(false);
        public bool GunventoryFolded = true;
        private OverridableBool ForceLowerPanelsInvisibleOverride = new OverridableBool(false);
        private bool m_metalGearGunSelectActive;
        private Dictionary<Texture, Material> MetalGearAtlasToFadeMaterialMapR = new Dictionary<Texture, Material>();
        private Dictionary<Material, Material> MetalGearFadeToOutlineMaterialMapR = new Dictionary<Material, Material>();
        private Dictionary<Material, Material> MetalGearDFAtlasMapR = new Dictionary<Material, Material>();
        private Dictionary<Texture, Material> MetalGearAtlasToFadeMaterialMapL = new Dictionary<Texture, Material>();
        private Dictionary<Material, Material> MetalGearFadeToOutlineMaterialMapL = new Dictionary<Material, Material>();
        private Dictionary<Material, Material> MetalGearDFAtlasMapL = new Dictionary<Material, Material>();
        public System.Action OnScaleUpdate;
        private dfGUIManager m_manager;
        private dfSprite p_playerCoinSprite;
        private Dictionary<AIActor, dfSlider> m_enemyToHealthbarMap = new Dictionary<AIActor, dfSlider>();
        private List<dfSlider> m_unusedHealthbars = new List<dfSlider>();
        private bool m_isDisplayingCustomReload;
        public dfPanel PauseMenuPanel;
        private PauseMenuController m_pmc;
        public ConversationBarController ConversationBar;
        protected bool m_displayingPlayerConversationOptions;
        protected bool hasSelectedOption;
        protected int selectedResponse = -1;
        private bool m_hasSelectedAreYouSureOption;
        private bool m_AreYouSureSelection;
        private dfButton m_AreYouSureYesButton;
        private dfButton m_AreYouSureNoButton;
        private dfLabel m_AreYouSurePrimaryLabel;
        private dfLabel m_AreYouSureSecondaryLabel;

        public GameUIReloadBarController GetReloadBarForPlayer(PlayerController p)
        {
            if (this.m_extantReloadBars != null && this.m_extantReloadBars.Count > 1 && (bool) (UnityEngine.Object) p)
                return this.m_extantReloadBars[!p.IsPrimaryPlayer ? 1 : 0];
            return this.m_extantReloadBars != null ? this.m_extantReloadBars[0] : (GameUIReloadBarController) null;
        }

        public bool ForceHideGunPanel
        {
            get => this.m_forceHideGunPanel;
            set
            {
                this.m_forceHideGunPanel = value;
                if (this.m_forceHideGunPanel)
                    return;
                for (int index = 0; index < this.ammoControllers.Count; ++index)
                    this.ammoControllers[index].TriggerUIDisabled();
            }
        }

        public bool ForceHideItemPanel
        {
            get => this.m_forceHideItemPanel;
            set
            {
                this.m_forceHideItemPanel = value;
                if (this.m_forceHideItemPanel)
                    return;
                for (int index = 0; index < this.itemControllers.Count; ++index)
                    this.itemControllers[index].TriggerUIDisabled();
            }
        }

        public static GameUIRoot Instance
        {
            get
            {
                if ((UnityEngine.Object) GameUIRoot.m_root == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) GameUIRoot.m_root)
                    GameUIRoot.m_root = (GameUIRoot) UnityEngine.Object.FindObjectOfType(typeof (GameUIRoot));
                return GameUIRoot.m_root;
            }
            set => GameUIRoot.m_root = value;
        }

        public static bool HasInstance => (bool) (UnityEngine.Object) GameUIRoot.m_root;

        public bool BossHealthBarVisible
        {
            get
            {
                return this.bossController.IsActive || this.bossController2.IsActive || this.bossControllerSide.IsActive;
            }
        }

        public dfPanel AddControlToMotionGroups(
            dfControl control,
            DungeonData.Direction dir,
            bool nonCore = false)
        {
            dfAnchorStyle anchor = control.Anchor;
            control.Anchor = dfAnchorStyle.None;
            if (!this.motionGroups.Contains(control))
            {
                this.motionGroups.Add(control);
                this.motionDirections.Add(dir);
            }
            if (nonCore && !this.customNonCoreMotionGroups.Contains(control))
                this.customNonCoreMotionGroups.Add(control);
            Vector3 vector1 = Vector3.zero;
            Vector3 vector2 = Vector3.zero;
            Vector3 relativePosition = control.RelativePosition;
            Vector2 size = control.Size;
            Vector3 inactivePosition = this.GetInitialInactivePosition(control, dir);
            switch (dir)
            {
                case DungeonData.Direction.NORTH:
                    vector1 = inactivePosition;
                    vector2 = relativePosition + size.ToVector3ZUp();
                    break;
                case DungeonData.Direction.EAST:
                    vector1 = inactivePosition + size.ToVector3ZUp();
                    vector2 = relativePosition;
                    break;
                case DungeonData.Direction.SOUTH:
                    vector1 = inactivePosition + size.ToVector3ZUp();
                    vector2 = relativePosition;
                    break;
                case DungeonData.Direction.WEST:
                    vector1 = inactivePosition;
                    vector2 = relativePosition + size.ToVector3ZUp();
                    break;
            }
            Vector2 vector2_1 = Vector2.Min(vector1.XY(), vector2.XY());
            Vector2 vector2_2 = Vector2.Max(vector1.XY(), vector2.XY()) - vector2_1;
            dfPanel motionGroups = !((UnityEngine.Object) control.Parent == (UnityEngine.Object) null) ? control.Parent.AddControl<dfPanel>() : control.GetManager().AddControl<dfPanel>();
            motionGroups.RelativePosition = (Vector3) vector2_1;
            motionGroups.Size = vector2_2;
            motionGroups.Pivot = control.Pivot;
            motionGroups.Anchor = anchor;
            motionGroups.IsInteractive = false;
            motionGroups.AddControl(control);
            switch (dir)
            {
                case DungeonData.Direction.NORTH:
                    control.RelativePosition = new Vector3(0.0f, vector2_2.y - control.Size.y, 0.0f);
                    break;
                case DungeonData.Direction.EAST:
                    control.RelativePosition = new Vector3(0.0f, 0.0f, 0.0f);
                    break;
                case DungeonData.Direction.SOUTH:
                    control.RelativePosition = new Vector3(0.0f, 0.0f, 0.0f);
                    break;
                case DungeonData.Direction.WEST:
                    control.RelativePosition = new Vector3(vector2_2.x - control.Size.x, 0.0f, 0.0f);
                    break;
            }
            control.Anchor = anchor;
            if (nonCore)
                this.RecalculateTargetPositions();
            return motionGroups;
        }

        public void UpdateControlMotionGroup(dfControl control)
        {
            if ((UnityEngine.Object) control == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) control || !this.motionGroups.Contains(control))
                return;
            DungeonData.Direction motionDirection = this.motionDirections[this.motionGroups.IndexOf(control)];
            this.RemoveControlFromMotionGroups(control);
            this.AddControlToMotionGroups(control, motionDirection);
            control.enabled = true;
        }

        public dfPanel GetMotionGroupParent(dfControl control)
        {
            return this.motionGroups.Contains(control) ? this.motionGroups[this.motionGroups.IndexOf(control)].Parent as dfPanel : (dfPanel) null;
        }

        public void RemoveControlFromMotionGroups(dfControl control)
        {
            int index = this.motionGroups.IndexOf(control);
            if (index != -1)
            {
                this.motionGroups.Remove(control);
                this.motionDirections.RemoveAt(index);
            }
            dfControl parent = control.Parent;
            if ((bool) (UnityEngine.Object) control.Parent && (bool) (UnityEngine.Object) control.Parent.Parent)
                control.Parent.Parent.AddControl(control);
            else if ((bool) (UnityEngine.Object) control.Parent)
                control.Parent.RemoveControl(control);
            UnityEngine.Object.Destroy((UnityEngine.Object) parent.gameObject);
        }

        private Vector3 GetActivePosition(dfControl panel, DungeonData.Direction direction)
        {
            Vector3 relativePosition = panel.RelativePosition;
            Vector3 vector3Zup = panel.Size.ToVector3ZUp();
            dfPanel parent = panel.Parent as dfPanel;
            if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
            {
                switch (direction)
                {
                    case DungeonData.Direction.NORTH:
                        return new Vector3(0.0f, parent.Size.y - vector3Zup.y, 0.0f);
                    case DungeonData.Direction.EAST:
                        return Vector3.zero;
                    case DungeonData.Direction.SOUTH:
                        return Vector3.zero;
                    case DungeonData.Direction.WEST:
                        return new Vector3(parent.Size.x - vector3Zup.x, 0.0f, 0.0f);
                }
            }
            return relativePosition;
        }

        public void DoDamageNumber(Vector3 worldPosition, float heightOffGround, int damage)
        {
            string stringForInt = IntToStringSansGarbage.GetStringForInt(damage);
            if (this.m_inactiveDamageLabels.Count == 0)
                this.m_inactiveDamageLabels.Add(((GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("DamagePopupLabel"), this.transform)).GetComponent<dfLabel>());
            dfLabel inactiveDamageLabel = this.m_inactiveDamageLabels[0];
            this.m_inactiveDamageLabels.RemoveAt(0);
            inactiveDamageLabel.gameObject.SetActive(true);
            inactiveDamageLabel.Text = stringForInt;
            inactiveDamageLabel.Color = (Color32) Color.red;
            inactiveDamageLabel.Opacity = 1f;
            inactiveDamageLabel.transform.position = dfFollowObject.ConvertWorldSpaces(worldPosition, GameManager.Instance.MainCameraController.Camera, this.m_manager.RenderCamera).WithZ(0.0f);
            inactiveDamageLabel.transform.position = inactiveDamageLabel.transform.position.QuantizeFloor(inactiveDamageLabel.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
            inactiveDamageLabel.StartCoroutine(this.HandleDamageNumberCR(worldPosition, worldPosition.y - heightOffGround, inactiveDamageLabel));
        }

        [DebuggerHidden]
        private IEnumerator HandleDamageNumberCR(
            Vector3 startWorldPosition,
            float worldFloorHeight,
            dfLabel damageLabel)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__HandleDamageNumberCRc__Iterator0()
            {
                startWorldPosition = startWorldPosition,
                worldFloorHeight = worldFloorHeight,
                damageLabel = damageLabel,
                _this = this
            };
        }

        public bool TransformHasDefaultLabel(Transform attachTransform)
        {
            for (int index = 0; index < this.extantBasicLabels.Count; ++index)
            {
                if ((UnityEngine.Object) this.extantBasicLabels[index].targetObject == (UnityEngine.Object) attachTransform)
                    return true;
            }
            return false;
        }

        public GameObject RegisterDefaultLabel(Transform attachTransform, Vector3 offset, string text)
        {
            GameObject gameObject = (GameObject) UnityEngine.Object.Instantiate(BraveResources.Load("DefaultLabelPanel"));
            DefaultLabelController component = gameObject.GetComponent<DefaultLabelController>();
            this.m_manager.AddControl((dfControl) component.panel);
            component.label.Text = text;
            component.Trigger(attachTransform, offset);
            this.extantBasicLabels.Add(component);
            return gameObject;
        }

        public void ToggleAllDefaultLabels(bool visible, string reason)
        {
            if (visible)
                this.m_defaultLabelsHidden.RemoveOverride(reason);
            else
                this.m_defaultLabelsHidden.SetOverride(reason, true);
            for (int index = 0; index < this.extantBasicLabels.Count; ++index)
            {
                if ((bool) (UnityEngine.Object) this.extantBasicLabels[index] && (bool) (UnityEngine.Object) this.extantBasicLabels[index].panel)
                    this.extantBasicLabels[index].panel.IsVisible = !this.m_defaultLabelsHidden.Value;
            }
        }

        public void ClearAllDefaultLabels()
        {
            for (int index = 0; index < this.extantBasicLabels.Count; index = index - 1 + 1)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object) this.extantBasicLabels[index].gameObject);
                this.extantBasicLabels.RemoveAt(index);
            }
        }

        public void ForceRemoveDefaultLabel(DefaultLabelController label)
        {
            int index = this.extantBasicLabels.IndexOf(label);
            if (index >= 0)
                this.extantBasicLabels.RemoveAt(index);
            UnityEngine.Object.Destroy((UnityEngine.Object) label.gameObject);
        }

        public void DeregisterDefaultLabel(Transform attachTransform)
        {
            for (int index = 0; index < this.extantBasicLabels.Count; ++index)
            {
                if ((UnityEngine.Object) this.extantBasicLabels[index].targetObject == (UnityEngine.Object) attachTransform)
                {
                    UnityEngine.Object.Destroy((UnityEngine.Object) this.extantBasicLabels[index].gameObject);
                    this.extantBasicLabels.RemoveAt(index);
                    --index;
                }
            }
        }

        public void TriggerDemoModeTutorialScreens()
        {
            if (this.demoTutorialPanels_Controller.Count == 0 || GameStatsManager.Instance.GetFlag(GungeonFlags.TUTORIAL_COMPLETED) || GameStatsManager.Instance.GetFlag(GungeonFlags.INTERNALDEBUG_HAS_SEEN_DEMO_TEXT))
                return;
            GameStatsManager.Instance.SetFlag(GungeonFlags.INTERNALDEBUG_HAS_SEEN_DEMO_TEXT, true);
            this.StartCoroutine(this.HandleDemoModeTutorialScreens());
        }

        [DebuggerHidden]
        private IEnumerator HandleDemoModeTutorialScreens()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__HandleDemoModeTutorialScreensc__Iterator1()
            {
                _this = this
            };
        }

        private Vector3 GetInactivePosition(dfControl panel, DungeonData.Direction direction)
        {
            Vector3 relativePosition = panel.RelativePosition;
            Vector3 vector3Zup = panel.Size.ToVector3ZUp();
            dfPanel parent = panel.Parent as dfPanel;
            if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
            {
                switch (direction)
                {
                    case DungeonData.Direction.NORTH:
                        return Vector3.zero;
                    case DungeonData.Direction.EAST:
                        return new Vector3(parent.Size.x - vector3Zup.x, 0.0f, 0.0f);
                    case DungeonData.Direction.SOUTH:
                        return new Vector3(0.0f, parent.Size.y - vector3Zup.y, 0.0f);
                    case DungeonData.Direction.WEST:
                        return Vector3.zero;
                }
            }
            return relativePosition;
        }

        private Vector3 GetInitialInactivePosition(dfControl panel, DungeonData.Direction direction)
        {
            Vector3 inactivePosition = panel.RelativePosition;
            Vector3 vector3Zup = panel.Size.ToVector3ZUp();
            Vector2 screenSize = panel.GetManager().GetScreenSize();
            switch (direction)
            {
                case DungeonData.Direction.NORTH:
                    inactivePosition = new Vector3(inactivePosition.x, -vector3Zup.y - this.MotionGroupBufferWidth, inactivePosition.z);
                    break;
                case DungeonData.Direction.EAST:
                    inactivePosition = new Vector3(screenSize.x + vector3Zup.x + this.MotionGroupBufferWidth, inactivePosition.y, inactivePosition.z);
                    break;
                case DungeonData.Direction.SOUTH:
                    inactivePosition = new Vector3(inactivePosition.x, screenSize.y + vector3Zup.y + this.MotionGroupBufferWidth, inactivePosition.z);
                    break;
                case DungeonData.Direction.WEST:
                    inactivePosition = new Vector3(-vector3Zup.x - this.MotionGroupBufferWidth, inactivePosition.y, inactivePosition.z);
                    break;
            }
            return inactivePosition;
        }

        public void AddPassiveItemToDock(PassiveItem item, PlayerController sourcePlayer)
        {
            MinimapUIController minimapUiController = (MinimapUIController) null;
            if ((bool) (UnityEngine.Object) Minimap.Instance)
                minimapUiController = Minimap.Instance.UIMinimap;
            if (!(bool) (UnityEngine.Object) minimapUiController)
                minimapUiController = UnityEngine.Object.FindObjectOfType<MinimapUIController>();
            minimapUiController.AddPassiveItemToDock(item, sourcePlayer);
        }

        public void RemovePassiveItemFromDock(PassiveItem item)
        {
            UnityEngine.Object.FindObjectOfType<MinimapUIController>().RemovePassiveItemFromDock(item);
        }

        [DebuggerHidden]
        private IEnumerator Start()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__Startc__Iterator2()
            {
                _this = this
            };
        }

        public void DisableCoopPlayerUI(PlayerController deadPlayer)
        {
            if (deadPlayer.IsPrimaryPlayer)
            {
                this.ammoControllers[1].ToggleRenderers(false);
                this.itemControllers[0].ToggleRenderers(false);
                this.itemControllers[0].temporarilyPreventVisible = true;
            }
            else
            {
                this.ammoControllers[0].ToggleRenderers(false);
                this.itemControllers[1].ToggleRenderers(false);
                this.itemControllers[1].temporarilyPreventVisible = true;
            }
        }

        public void ReenableCoopPlayerUI(PlayerController deadPlayer)
        {
            if (deadPlayer.IsPrimaryPlayer)
            {
                this.ammoControllers[1].GetComponent<dfPanel>().IsVisible = true;
                this.ammoControllers[1].ToggleRenderers(true);
                this.itemControllers[0].GetComponent<dfPanel>().IsVisible = true;
                this.itemControllers[0].ToggleRenderers(true);
                this.itemControllers[0].temporarilyPreventVisible = false;
            }
            else
            {
                this.ammoControllers[0].GetComponent<dfPanel>().IsVisible = true;
                this.ammoControllers[0].ToggleRenderers(true);
                this.itemControllers[1].GetComponent<dfPanel>().IsVisible = true;
                this.itemControllers[1].ToggleRenderers(true);
                this.itemControllers[1].temporarilyPreventVisible = false;
            }
        }

        public void ConvertCoreUIToCoopMode()
        {
            float units = this.gunNameLabels[0].PixelsToUnits();
            bool flag = GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER;
            this.heartControllers[1].GetComponent<dfPanel>().IsVisible = flag;
            this.blankControllers[1].GetComponent<dfPanel>().IsVisible = flag;
            this.ammoControllers[1].GetComponent<dfPanel>().IsEnabled = flag;
            this.ammoControllers[1].GetComponent<dfPanel>().IsVisible = flag;
            this.itemControllers[0].transform.position = this.ammoControllers[1].GunBoxSprite.transform.position + new Vector3((3f + this.ammoControllers[1].GunBoxSprite.Width + (float) (2 * this.ammoControllers[1].AdditionalGunBoxSprites.Count)) * units, 0.0f, 0.0f);
            this.itemControllers[1].transform.position = this.ammoControllers[0].GunBoxSprite.transform.position + new Vector3((float) ((3.0 + (double) this.ammoControllers[0].GunBoxSprite.Width + (double) (2 * this.ammoControllers[0].AdditionalGunBoxSprites.Count)) * -1.0) * units, 0.0f, 0.0f);
            dfLabel itemNameLabel1 = this.itemNameLabels[0];
            itemNameLabel1.RelativePosition = itemNameLabel1.RelativePosition + new Vector3(this.ammoControllers[0].GunBoxSprite.Width * units, 0.0f, 0.0f);
            dfLabel itemNameLabel2 = this.itemNameLabels[1];
            itemNameLabel2.RelativePosition = itemNameLabel2.RelativePosition + new Vector3(-this.ammoControllers[0].GunBoxSprite.Width * units, 0.0f, 0.0f);
            dfLabel component = this.Manager.AddPrefab(this.p_needsReloadLabel.gameObject).GetComponent<dfLabel>();
            component.IsVisible = false;
            this.m_extantReloadLabels.Add(component);
            this.m_displayingReloadNeeded.Add(false);
            this.m_extantReloadBars.Add(this.p_secondaryPlayerReloadBar);
        }

        protected void RecalculateTargetPositions()
        {
            if (this.motionInteriorPositions == null)
                this.motionInteriorPositions = new Dictionary<dfControl, Vector3>();
            else
                this.motionInteriorPositions.Clear();
            if (this.motionExteriorPositions == null)
                this.motionExteriorPositions = new Dictionary<dfControl, Vector3>();
            else
                this.motionExteriorPositions.Clear();
            for (int index = 0; index < this.motionGroups.Count; ++index)
            {
                this.motionInteriorPositions.Add(this.motionGroups[index], this.GetActivePosition(this.motionGroups[index], this.motionDirections[index]));
                this.motionExteriorPositions.Add(this.motionGroups[index], this.GetInactivePosition(this.motionGroups[index], this.motionDirections[index]));
            }
        }

        public bool IsCoreUIVisible() => !this.CoreUIHidden.Value;

        public void HideCoreUI(string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                reason = "generic";
            bool flag = this.CoreUIHidden.Value;
            this.CoreUIHidden.SetOverride(reason, true);
            if (this.CoreUIHidden.Value == flag)
                return;
            this.RecalculateTargetPositions();
            this.StartCoroutine(this.CoreUITransition());
            for (int index = 0; index < this.m_extantReloadBars.Count; ++index)
                this.m_extantReloadBars[index].SetInvisibility(true, "CoreUI");
        }

        public GameUIAmmoController GetAmmoControllerForPlayerID(int playerID)
        {
            return GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER ? (playerID == 1 ? this.ammoControllers[0] : this.ammoControllers[1]) : (this.ammoControllers.Count > 1 && playerID != 0 ? this.ammoControllers[1] : this.ammoControllers[0]);
        }

        private GameUIItemController GetItemControllerForPlayerID(int playerID)
        {
            return playerID >= this.itemControllers.Count ? (GameUIItemController) null : this.itemControllers[playerID];
        }

        public void ToggleLowerPanels(bool targetVisible, bool permanent = false, string source = "")
        {
            if (targetVisible && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                return;
            if (string.IsNullOrEmpty(source))
                source = "generic";
            this.ForceLowerPanelsInvisibleOverride.SetOverride(source, !targetVisible);
            for (int playerID = 0; playerID < this.ammoControllers.Count; ++playerID)
            {
                bool flag = targetVisible;
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                    flag = false;
                if (playerID >= GameManager.Instance.AllPlayers.Length)
                    flag = false;
                if (playerID >= GameManager.Instance.AllPlayers.Length || !GameManager.Instance.AllPlayers[playerID].IsGhost)
                {
                    if (this.ForceLowerPanelsInvisibleOverride.Value)
                        flag = false;
                    GameUIAmmoController controllerForPlayerId1 = this.GetAmmoControllerForPlayerID(playerID);
                    GameUIItemController controllerForPlayerId2 = this.GetItemControllerForPlayerID(playerID);
                    if (!controllerForPlayerId1.forceInvisiblePermanent)
                    {
                        dfPanel component1 = controllerForPlayerId1.GetComponent<dfPanel>();
                        dfPanel component2 = controllerForPlayerId2.GetComponent<dfPanel>();
                        component1.IsVisible = flag;
                        component2.IsVisible = flag;
                        controllerForPlayerId1.ToggleRenderers(flag);
                        controllerForPlayerId2.ToggleRenderers(flag);
                        if (permanent)
                            controllerForPlayerId1.forceInvisiblePermanent = !flag;
                        controllerForPlayerId1.temporarilyPreventVisible = !flag;
                        controllerForPlayerId2.temporarilyPreventVisible = !flag;
                    }
                }
            }
        }

        public void ToggleItemPanels(bool targetVisible)
        {
            for (int playerID = 0; playerID < GameManager.Instance.AllPlayers.Length; ++playerID)
            {
                bool flag = targetVisible;
                if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                    flag = false;
                if (playerID >= GameManager.Instance.AllPlayers.Length)
                    flag = false;
                if (playerID < GameManager.Instance.AllPlayers.Length && GameManager.Instance.AllPlayers[playerID].IsGhost)
                    flag = false;
                GameUIItemController controllerForPlayerId = this.GetItemControllerForPlayerID(playerID);
                if ((bool) (UnityEngine.Object) controllerForPlayerId)
                {
                    controllerForPlayerId.GetComponent<dfPanel>().IsVisible = flag;
                    controllerForPlayerId.ToggleRenderers(flag);
                    controllerForPlayerId.temporarilyPreventVisible = !flag;
                }
            }
        }

        public void MoveNonCoreGroupImmediately(dfControl control, bool offScreen = false)
        {
            if (!this.motionInteriorPositions.ContainsKey(control) || !this.motionExteriorPositions.ContainsKey(control))
                return;
            if (offScreen)
                control.RelativePosition = this.motionExteriorPositions[control];
            else
                control.RelativePosition = this.motionInteriorPositions[control];
        }

        public void MoveNonCoreGroupOnscreen(dfControl control, bool reversed = false)
        {
            if (!this.customNonCoreMotionGroups.Contains(control))
                return;
            this.StartCoroutine(this.NonCoreControlTransition(control, reversed));
        }

        [DebuggerHidden]
        private IEnumerator NonCoreControlTransition(dfControl control, bool reversed = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__NonCoreControlTransitionc__Iterator3()
            {
                reversed = reversed,
                control = control,
                _this = this
            };
        }

        public void ShowCoreUI(string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                reason = "generic";
            bool flag = this.CoreUIHidden.Value;
            this.CoreUIHidden.SetOverride(reason, false);
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || flag == this.CoreUIHidden.Value)
                return;
            this.RecalculateTargetPositions();
            this.StartCoroutine(this.CoreUITransition());
            for (int index = 0; index < this.m_extantReloadBars.Count; ++index)
                this.m_extantReloadBars[index].SetInvisibility(false, "CoreUI");
        }

        public void TransitionTargetMotionGroup(
            dfControl motionGroup,
            bool targetVisibility,
            bool targetLockState,
            bool instant)
        {
            this.RecalculateTargetPositions();
            this.StartCoroutine(this.TransitionTargetMotionGroup_CR(motionGroup, targetVisibility, targetLockState, instant));
        }

        [DebuggerHidden]
        private IEnumerator TransitionTargetMotionGroup_CR(
            dfControl motionGroup,
            bool targetVisibility,
            bool targetLockState,
            bool instant)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__TransitionTargetMotionGroup_CRc__Iterator4()
            {
                motionGroup = motionGroup,
                instant = instant,
                targetVisibility = targetVisibility,
                targetLockState = targetLockState,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator CoreUITransition()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__CoreUITransitionc__Iterator5()
            {
                _this = this
            };
        }

        public tk2dClippedSprite GetSpriteForUnfoldedItem(int playerID, int itemIndex)
        {
            Transform transform = this.GetItemControllerForPlayerID(playerID).ItemBoxSprite.transform.parent.Find("AdditionalItemBox" + IntToStringSansGarbage.GetStringForInt(itemIndex));
            return (UnityEngine.Object) transform != (UnityEngine.Object) null ? transform.GetComponent<dfSprite>().transform.Find("AdditionalItemSprite").GetComponent<tk2dClippedSprite>() : (tk2dClippedSprite) null;
        }

        public tk2dClippedSprite GetSpriteForUnfoldedGun(int playerID, int gunIndex)
        {
            Transform transform = this.GetAmmoControllerForPlayerID(playerID).GunBoxSprite.transform.parent.Find("AdditionalWeaponBox" + IntToStringSansGarbage.GetStringForInt(gunIndex));
            if (!((UnityEngine.Object) transform != (UnityEngine.Object) null))
                return (tk2dClippedSprite) null;
            dfSprite component = transform.GetComponent<dfSprite>();
            component.transform.GetChild(0).GetComponent<dfSprite>();
            return component.transform.Find("AdditionalGunSprite").GetComponent<tk2dClippedSprite>();
        }

        public void ToggleHighlightUnfoldedGun(int gunIndex, bool highlighted)
        {
            if (gunIndex == 0)
            {
                for (int index = 0; index < this.ammoControllers[0].gunSprites.Length; ++index)
                {
                    tk2dClippedSprite gunSprite = this.ammoControllers[0].gunSprites[index];
                    if (highlighted)
                        gunSprite.renderer.enabled = false;
                    else
                        gunSprite.renderer.enabled = true;
                }
            }
            else
            {
                tk2dClippedSprite component = this.additionalGunBoxes[gunIndex - 1].transform.Find("AdditionalGunSprite").GetComponent<tk2dClippedSprite>();
                if (highlighted)
                    component.renderer.enabled = false;
                else
                    component.renderer.enabled = true;
            }
        }

        public void UnfoldGunventory(bool doItems = true)
        {
            if (GameManager.Instance.PrimaryPlayer.inventory.AllGuns.Count > 8 || !this.GunventoryFolded)
                return;
            this.GunventoryFolded = false;
            this.StartCoroutine(this.HandlePauseInventoryFolding(GameManager.Instance.PrimaryPlayer, doItems: doItems));
            if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
                return;
            this.StartCoroutine(this.HandlePauseInventoryFolding(GameManager.Instance.SecondaryPlayer));
        }

        public void RefoldGunventory()
        {
            if (this.GunventoryFolded)
                return;
            this.GunventoryFolded = true;
            this.StartCoroutine(this.HandlePauseInventoryFolding(GameManager.Instance.PrimaryPlayer));
            if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
                return;
            this.StartCoroutine(this.HandlePauseInventoryFolding(GameManager.Instance.SecondaryPlayer));
        }

        private void DestroyAdditionalFrames(
            bool GunventoryFolded,
            GameUIAmmoController ammoController,
            GameUIItemController itemController,
            List<dfSprite> additionalGunFrames,
            List<dfSprite> additionalItemFrames,
            bool forceDestroy = false)
        {
            if (!GunventoryFolded)
            {
                if ((UnityEngine.Object) ammoController != (UnityEngine.Object) null)
                {
                    for (int index = 0; index < ammoController.AdditionalGunBoxSprites.Count; ++index)
                    {
                        dfControl additionalGunBoxSprite = ammoController.AdditionalGunBoxSprites[index];
                        if ((bool) (UnityEngine.Object) additionalGunBoxSprite)
                        {
                            additionalGunBoxSprite.transform.parent = (Transform) null;
                            UnityEngine.Object.Destroy((UnityEngine.Object) additionalGunBoxSprite.gameObject);
                        }
                    }
                    ammoController.AdditionalGunBoxSprites.Clear();
                }
                if ((UnityEngine.Object) itemController != (UnityEngine.Object) null)
                {
                    for (int index = 0; index < itemController.AdditionalItemBoxSprites.Count; ++index)
                    {
                        dfControl additionalItemBoxSprite = itemController.AdditionalItemBoxSprites[index];
                        if ((bool) (UnityEngine.Object) additionalItemBoxSprite)
                        {
                            additionalItemBoxSprite.transform.parent = (Transform) null;
                            UnityEngine.Object.Destroy((UnityEngine.Object) additionalItemBoxSprite.gameObject);
                        }
                    }
                    itemController.AdditionalItemBoxSprites.Clear();
                }
            }
            if (!GunventoryFolded || forceDestroy)
            {
                if (additionalGunFrames != null)
                {
                    for (int index = 0; index < additionalGunFrames.Count; ++index)
                    {
                        dfSprite additionalGunFrame = additionalGunFrames[index];
                        if ((bool) (UnityEngine.Object) additionalGunFrame)
                            UnityEngine.Object.Destroy((UnityEngine.Object) additionalGunFrame.gameObject);
                    }
                }
                if (additionalItemFrames != null)
                {
                    for (int index = 0; index < additionalItemFrames.Count; ++index)
                    {
                        dfSprite additionalItemFrame = additionalItemFrames[index];
                        if ((bool) (UnityEngine.Object) additionalItemFrame)
                            UnityEngine.Object.Destroy((UnityEngine.Object) additionalItemFrame.gameObject);
                    }
                }
            }
            additionalGunFrames?.Clear();
            additionalItemFrames?.Clear();
        }

        private void HandleStackedFrameFoldMotion(
            float t,
            dfSprite baseBoxSprite,
            List<dfSprite> additionalGunFrames,
            List<tk2dClippedSprite> gunSpritesByBox,
            Dictionary<tk2dClippedSprite, tk2dClippedSprite[]> gunToOutlineMap)
        {
            float units = this.gunNameLabels[0].PixelsToUnits();
            for (int index1 = 0; index1 < additionalGunFrames.Count; ++index1)
            {
                float num1 = 1f / (float) additionalGunFrames.Count;
                Vector3 a = baseBoxSprite.RelativePosition - baseBoxSprite.Size.WithX(0.0f).ToVector3ZUp() * (float) index1;
                Vector3 b = a - baseBoxSprite.Size.WithX(0.0f).ToVector3ZUp();
                float num2 = num1 * (float) index1;
                float t1 = Mathf.Clamp01((t - num2) / num1);
                float t2 = Mathf.SmoothStep(0.0f, 1f, t1);
                additionalGunFrames[index1].FillAmount = t2;
                additionalGunFrames[index1].IsVisible = (double) additionalGunFrames[index1].FillAmount > 0.0;
                tk2dClippedSprite key = gunSpritesByBox[index1];
                if ((UnityEngine.Object) key != (UnityEngine.Object) null)
                {
                    float num3 = key.GetUntrimmedBounds().size.y / (additionalGunFrames[index1].Size.y * units);
                    float num4 = (float) ((1.0 - (double) num3) / 2.0);
                    float num5 = Mathf.SmoothStep(0.0f, 1f, Mathf.Clamp01((t1 - num4) / num3));
                    key.clipBottomLeft = new Vector2(0.0f, 1f - num5);
                    for (int index2 = 0; index2 < gunToOutlineMap[key].Length; ++index2)
                        gunToOutlineMap[key][index2].clipBottomLeft = new Vector2(0.0f, 1f - num5);
                }
                additionalGunFrames[index1].RelativePosition = Vector3.Lerp(a, b, t2);
            }
        }

        private void UpdateFramedGunSprite(
            Gun sourceGun,
            dfSprite targetFrame,
            GameUIAmmoController ammoController)
        {
            tk2dBaseSprite sprite = sourceGun.GetSprite();
            tk2dClippedSprite componentInChildren = targetFrame.GetComponentInChildren<tk2dClippedSprite>();
            componentInChildren.SetSprite(sprite.Collection, sprite.spriteId);
            componentInChildren.scale = GameUIUtility.GetCurrentTK2D_DFScale(this.m_manager) * Vector3.one;
            Vector3 center = targetFrame.GetCenter();
            componentInChildren.transform.position = center + ammoController.GetOffsetVectorForGun(sourceGun, false);
        }

        public bool MetalGearActive => this.m_metalGearGunSelectActive;

        public void TriggerMetalGearGunSelect(PlayerController sourcePlayer)
        {
            if (sourcePlayer.IsGhost || sourcePlayer.inventory.AllGuns.Count < 2)
                return;
            int numToL = -1;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                numToL = !sourcePlayer.IsPrimaryPlayer ? -1 : 1;
            if (sourcePlayer.inventory.AllGuns.Count == 2)
                numToL = 0;
            this.StartCoroutine(this.HandleMetalGearGunSelect(sourcePlayer, numToL));
        }

        private void AssignClippedSpriteFadeFractions(
            tk2dClippedSprite gunSpr,
            float fadeScreenSpaceY,
            float fadeScreenSpaceXStart,
            float fadeScreenSpaceXEnd,
            bool leftAligned)
        {
            Dictionary<Texture, Material> dictionary1 = !leftAligned ? this.MetalGearAtlasToFadeMaterialMapR : this.MetalGearAtlasToFadeMaterialMapL;
            Dictionary<Material, Material> dictionary2 = !leftAligned ? this.MetalGearFadeToOutlineMaterialMapR : this.MetalGearFadeToOutlineMaterialMapL;
            Material sharedMaterial = gunSpr.renderer.sharedMaterial;
            Material material1;
            if (dictionary1.ContainsKey(sharedMaterial.mainTexture))
            {
                material1 = dictionary1[sharedMaterial.mainTexture];
            }
            else
            {
                material1 = gunSpr.renderer.material;
                dictionary1.Add(sharedMaterial.mainTexture, material1);
            }
            if ((UnityEngine.Object) sharedMaterial != (UnityEngine.Object) material1)
                gunSpr.renderer.sharedMaterial = material1;
            gunSpr.usesOverrideMaterial = true;
            gunSpr.renderer.sharedMaterial.shader = ShaderCache.Acquire("tk2d/BlendVertexColorFadeRange");
            gunSpr.renderer.sharedMaterial.SetFloat("_YFadeStart", Mathf.Min(0.75f, fadeScreenSpaceY));
            gunSpr.renderer.sharedMaterial.SetFloat("_YFadeEnd", 0.03f);
            gunSpr.renderer.sharedMaterial.SetFloat("_XFadeStart", fadeScreenSpaceXStart);
            gunSpr.renderer.sharedMaterial.SetFloat("_XFadeEnd", fadeScreenSpaceXEnd);
            tk2dClippedSprite[] outlineSprites = SpriteOutlineManager.GetOutlineSprites<tk2dClippedSprite>((tk2dBaseSprite) gunSpr);
            if (outlineSprites == null || outlineSprites.Length <= 0)
                return;
            Material material2;
            if (dictionary2.ContainsKey(material1))
            {
                material2 = dictionary2[material1];
            }
            else
            {
                material2 = UnityEngine.Object.Instantiate<Material>(gunSpr.renderer.sharedMaterial);
                dictionary2.Add(material1, material2);
            }
            material2.SetFloat("_YFadeStart", Mathf.Min(0.75f, fadeScreenSpaceY));
            material2.SetFloat("_YFadeEnd", 0.03f);
            material2.SetFloat("_XFadeStart", fadeScreenSpaceXStart);
            material2.SetFloat("_XFadeEnd", fadeScreenSpaceXEnd);
            material2.SetColor("_OverrideColor", new Color(1f, 1f, 1f, 1f));
            material2.SetFloat("_DivPower", 4f);
            for (int index = 0; index < outlineSprites.Length; ++index)
            {
                if ((bool) (UnityEngine.Object) outlineSprites[index])
                {
                    outlineSprites[index].usesOverrideMaterial = true;
                    outlineSprites[index].renderer.sharedMaterial = material2;
                }
            }
        }

        private Material GetDFAtlasMaterialForMetalGear(Material source, bool leftAligned)
        {
            Dictionary<Material, Material> dictionary = !leftAligned ? this.MetalGearDFAtlasMapR : this.MetalGearDFAtlasMapL;
            Material materialForMetalGear;
            if (dictionary.ContainsKey(source))
            {
                materialForMetalGear = dictionary[source];
            }
            else
            {
                materialForMetalGear = UnityEngine.Object.Instantiate<Material>(source);
                materialForMetalGear.shader = ShaderCache.Acquire("Daikon Forge/Default UI Shader FadeRange");
                dictionary.Add(source, materialForMetalGear);
            }
            return materialForMetalGear;
        }

        private void SetFadeMaterials(dfSprite targetSprite, bool leftAligned)
        {
            Material materialForMetalGear = this.GetDFAtlasMaterialForMetalGear(targetSprite.Atlas.Material, leftAligned);
            targetSprite.OverrideMaterial = materialForMetalGear;
        }

        private void SetFadeFractions(
            dfSprite targetSprite,
            float fadeScreenSpaceXStart,
            float fadeScreenSpaceXEnd,
            float fadeScreenSpaceY,
            bool isLeftAligned)
        {
            Material materialForMetalGear = this.GetDFAtlasMaterialForMetalGear(targetSprite.Atlas.Material, isLeftAligned);
            materialForMetalGear.SetFloat("_YFadeStart", Mathf.Min(0.75f, fadeScreenSpaceY));
            materialForMetalGear.SetFloat("_YFadeEnd", 0.03f);
            materialForMetalGear.SetFloat("_XFadeStart", fadeScreenSpaceXStart);
            materialForMetalGear.SetFloat("_XFadeEnd", fadeScreenSpaceXEnd);
            targetSprite.OverrideMaterial = materialForMetalGear;
            dfMaterialCache.ForceUpdate(targetSprite.OverrideMaterial);
            targetSprite.Invalidate();
        }

        [DebuggerHidden]
        private IEnumerator HandleMetalGearGunSelect(PlayerController targetPlayer, int numToL)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__HandleMetalGearGunSelectc__Iterator6()
            {
                targetPlayer = targetPlayer,
                numToL = numToL,
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator HandlePauseInventoryFolding(
            PlayerController targetPlayer,
            bool doGuns = true,
            bool doItems = true,
            float overrideTransitionTime = -1f,
            int numToL = 0,
            bool forceUseExistingList = false)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__HandlePauseInventoryFoldingc__Iterator7()
            {
                targetPlayer = targetPlayer,
                overrideTransitionTime = overrideTransitionTime,
                forceUseExistingList = forceUseExistingList,
                doGuns = doGuns,
                doItems = doItems,
                numToL = numToL,
                _this = this
            };
        }

        public void ToggleUICamera(bool enable)
        {
            this.gunNameLabels[0].GetManager().RenderCamera.enabled = enable;
        }

        public static float GameUIScalar
        {
            get
            {
                return GameManager.Instance.IsPaused || TimeTubeCreditsController.IsTimeTubing || !GameManager.Options.SmallUIEnabled ? 1f : 0.5f;
            }
        }

        public void UpdateScale()
        {
            for (int index = 0; index < this.heartControllers.Count; ++index)
                this.heartControllers[index].UpdateScale();
            for (int index = 0; index < this.blankControllers.Count; ++index)
                this.blankControllers[index].UpdateScale();
            for (int index = 0; index < this.ammoControllers.Count; ++index)
                this.ammoControllers[index].UpdateScale();
            for (int index = 0; index < this.itemControllers.Count; ++index)
                this.itemControllers[index].UpdateScale();
            for (int index = 0; index < this.gunNameLabels.Count; ++index)
                this.gunNameLabels[index].TextScale = Pixelator.Instance.CurrentTileScale;
            for (int index = 0; index < this.itemNameLabels.Count; ++index)
                this.itemNameLabels[index].TextScale = Pixelator.Instance.CurrentTileScale;
            if ((UnityEngine.Object) this.m_manager != (UnityEngine.Object) null)
                this.m_manager.UIScale = Pixelator.Instance.ScaleTileScale / 3f * GameUIRoot.GameUIScalar;
            if (this.OnScaleUpdate == null)
                return;
            this.OnScaleUpdate();
        }

        public void DisplayUndiePanel()
        {
            dfPanel component = this.undiePanel.GetComponent<dfPanel>();
            this.undiePanel.SetActive(true);
            component.ZOrder = 1500;
            dfGUIManager.PushModal((dfControl) component);
        }

        public float PixelsToUnits() => this.Manager.PixelsToUnits();

        public dfGUIManager Manager
        {
            get
            {
                if ((UnityEngine.Object) this.m_manager == (UnityEngine.Object) null)
                    this.m_manager = this.GetComponent<dfGUIManager>();
                return this.m_manager;
            }
        }

        public void DoNotification(EncounterTrackable trackable)
        {
            this.notificationController.DoNotification(trackable);
        }

        public void UpdatePlayerBlankUI(PlayerController player)
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                return;
            if (player.IsPrimaryPlayer)
                this.blankControllers[0].UpdateBlanks(player.Blanks);
            else
                this.blankControllers[1].UpdateBlanks(player.Blanks);
        }

        [DebuggerHidden]
        private IEnumerator HandleGenericPositionLerp(
            dfControl targetControl,
            Vector3 delta,
            float duration)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__HandleGenericPositionLerpc__Iterator8()
            {
                targetControl = targetControl,
                duration = duration,
                delta = delta
            };
        }

        public void TransitionToGhostUI(PlayerController player)
        {
        }

        public void UpdateGhostUI(PlayerController player)
        {
            if (!player.IsGhost)
                ;
        }

        public void UpdatePlayerHealthUI(PlayerController player, HealthHaver hh)
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                return;
            if (player.IsPrimaryPlayer)
                this.heartControllers[0].UpdateHealth(hh);
            else
                this.heartControllers[1].UpdateHealth(hh);
        }

        public void SetAmmoCountColor(Color targetcolor, PlayerController sourcePlayer)
        {
            int index = 0;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                index = !sourcePlayer.IsPrimaryPlayer ? 0 : 1;
            this.ammoControllers[index].SetAmmoCountLabelColor(targetcolor);
        }

        public void UpdateGunData(
            GunInventory inventory,
            int inventoryShift,
            PlayerController sourcePlayer)
        {
            if (sourcePlayer.healthHaver.IsDead)
                return;
            int num = 0;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                num = !sourcePlayer.IsPrimaryPlayer ? 0 : 1;
            this.UpdateGunDataInternal(sourcePlayer, inventory, inventoryShift, this.ammoControllers[num], num);
        }

        private void UpdateGunDataInternal(
            PlayerController targetPlayer,
            GunInventory inventory,
            int inventoryShift,
            GameUIAmmoController targetAmmoController,
            int labelTarget)
        {
            Gun currentGun = inventory.CurrentGun;
            float units = this.gunNameLabels[labelTarget].PixelsToUnits();
            if ((UnityEngine.Object) currentGun != (UnityEngine.Object) null)
            {
                EncounterTrackable component = currentGun.GetComponent<EncounterTrackable>();
                this.gunNameLabels[labelTarget].Text = !((UnityEngine.Object) component != (UnityEngine.Object) null) ? currentGun.gunName : component.GetModifiedDisplayName();
            }
            else
                this.gunNameLabels[labelTarget].Text = string.Empty;
            targetAmmoController.UpdateUIGun(inventory, inventoryShift);
            if (inventoryShift != 0)
                this.TemporarilyShowGunName(targetPlayer.IsPrimaryPlayer);
            if ((UnityEngine.Object) currentGun != (UnityEngine.Object) null && currentGun.ClipShotsRemaining == 0 && (currentGun.ClipCapacity > 1 || currentGun.ammo == 0) && !currentGun.IsReloading && !targetPlayer.IsInputOverridden && !currentGun.IsHeroSword)
            {
                targetPlayer.gunReloadDisplayTimer += BraveTime.DeltaTime;
                if ((double) targetPlayer.gunReloadDisplayTimer > 0.25)
                    this.InformNeedsReload(targetPlayer, new Vector3(targetPlayer.specRigidbody.UnitCenter.x - targetPlayer.transform.position.x, 1.25f, 0.0f), customKey: string.Empty);
            }
            else if (!this.m_isDisplayingCustomReload)
            {
                if (this.m_displayingReloadNeeded.Count < 2)
                    this.m_displayingReloadNeeded.Add(false);
                targetPlayer.gunReloadDisplayTimer = 0.0f;
                this.m_displayingReloadNeeded[!targetPlayer.IsPrimaryPlayer ? 1 : 0] = false;
            }
            else
                targetPlayer.gunReloadDisplayTimer = 0.0f;
            this.m_gunNameVisibilityTimers[labelTarget] -= this.m_deltaTime;
            if ((double) this.m_gunNameVisibilityTimers[labelTarget] > 1.0)
            {
                this.gunNameLabels[labelTarget].IsVisible = true;
                this.gunNameLabels[labelTarget].Opacity = 1f;
            }
            else if ((double) this.m_gunNameVisibilityTimers[labelTarget] > 0.0)
            {
                this.gunNameLabels[labelTarget].IsVisible = true;
                this.gunNameLabels[labelTarget].Opacity = this.m_gunNameVisibilityTimers[labelTarget];
            }
            else
                this.gunNameLabels[labelTarget].IsVisible = false;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                this.itemControllers[0].transform.position = this.ammoControllers[1].GunBoxSprite.transform.position + new Vector3((3f + this.ammoControllers[1].GunBoxSprite.Width + (float) (2 * this.ammoControllers[1].AdditionalGunBoxSprites.Count)) * units, 0.0f, 0.0f);
                this.itemControllers[1].transform.position = this.ammoControllers[0].GunBoxSprite.transform.position + new Vector3((float) ((3.0 + (double) this.ammoControllers[0].GunBoxSprite.Width + (double) (2 * this.ammoControllers[0].AdditionalGunBoxSprites.Count)) * -1.0) * units, 0.0f, 0.0f);
                if (this.itemControllers[labelTarget != 0 ? 0 : 1].ItemBoxSprite.IsVisible)
                    this.gunNameLabels[labelTarget].transform.position = this.itemNameLabels[labelTarget != 0 ? 0 : 1].transform.position + new Vector3(0.0f, (float) (-1.0 * ((double) this.itemNameLabels[labelTarget].Height * (double) units)), 0.0f);
                else if (targetAmmoController.IsLeftAligned)
                    this.gunNameLabels[labelTarget].transform.position = this.gunNameLabels[labelTarget].transform.position.WithX(targetAmmoController.GunBoxSprite.transform.position.x + (targetAmmoController.GunBoxSprite.Width + 4f) * units).WithY(targetAmmoController.GunBoxSprite.transform.position.y);
                else
                    this.gunNameLabels[labelTarget].transform.position = this.gunNameLabels[labelTarget].transform.position.WithX(targetAmmoController.GunBoxSprite.transform.position.x - (targetAmmoController.GunBoxSprite.Width + 4f) * units).WithY(targetAmmoController.GunBoxSprite.transform.position.y);
            }
            else if (targetAmmoController.IsLeftAligned)
                this.gunNameLabels[labelTarget].transform.position = this.gunNameLabels[labelTarget].transform.position.WithX(targetAmmoController.GunBoxSprite.transform.position.x + (targetAmmoController.GunBoxSprite.Width + 4f) * units).WithY(targetAmmoController.GunBoxSprite.transform.position.y);
            else
                this.gunNameLabels[labelTarget].transform.position = this.gunNameLabels[labelTarget].transform.position.WithX(targetAmmoController.GunBoxSprite.transform.position.x - (targetAmmoController.GunBoxSprite.Width + 4f) * units).WithY(targetAmmoController.GunBoxSprite.transform.position.y);
        }

        public void TemporarilyShowGunName(bool primaryPlayer)
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                return;
            int index = 0;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                index = !primaryPlayer ? 0 : 1;
            this.m_gunNameVisibilityTimers[index] = 3f;
            if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
                return;
            this.m_itemNameVisibilityTimers[index != 0 ? 0 : 1] = 0.0f;
        }

        public void TemporarilyShowItemName(bool primaryPlayer)
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
                return;
            int index = !primaryPlayer ? 1 : 0;
            this.m_itemNameVisibilityTimers[index] = 3f;
            if (GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
                return;
            this.m_gunNameVisibilityTimers[index != 0 ? 0 : 1] = 0.0f;
        }

        public void ClearGunName(bool primaryPlayer)
        {
            int index = 0;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                index = !primaryPlayer ? 0 : 1;
            this.m_gunNameVisibilityTimers[index] = 0.0f;
            this.gunNameLabels[index].IsVisible = false;
        }

        public void ClearItemName(bool primaryPlayer)
        {
            int index = !primaryPlayer ? 1 : 0;
            this.m_itemNameVisibilityTimers[index] = 0.0f;
            this.itemNameLabels[index].IsVisible = false;
        }

        public void UpdateItemData(
            PlayerController targetPlayer,
            PlayerItem item,
            List<PlayerItem> items)
        {
            int index = 0;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                index = !targetPlayer.IsPrimaryPlayer ? 1 : 0;
            string str = string.Empty;
            if ((UnityEngine.Object) item != (UnityEngine.Object) null)
            {
                EncounterTrackable component = item.GetComponent<EncounterTrackable>();
                str = !((UnityEngine.Object) component != (UnityEngine.Object) null) ? item.DisplayName : component.journalData.GetPrimaryDisplayName();
                if (!item.consumable || item.numberOfUses <= 1)
                    ;
            }
            this.m_itemNameVisibilityTimers[index] -= this.m_deltaTime;
            if ((double) this.m_itemNameVisibilityTimers[index] > 1.0)
            {
                this.itemNameLabels[index].IsVisible = true;
                this.itemNameLabels[index].Opacity = 1f;
            }
            else if ((double) this.m_itemNameVisibilityTimers[index] > 0.0)
            {
                this.itemNameLabels[index].IsVisible = true;
                this.itemNameLabels[index].Opacity = this.m_itemNameVisibilityTimers[index];
            }
            else
                this.itemNameLabels[index].IsVisible = false;
            this.itemNameLabels[index].Text = str;
            GameUIItemController itemController = this.itemControllers[index];
            float units = itemController.ItemBoxSprite.PixelsToUnits();
            if (itemController.IsRightAligned)
                this.itemNameLabels[index].transform.position = this.itemNameLabels[index].transform.position.WithX(itemController.ItemBoxSprite.transform.position.x + -4f * units).WithY(itemController.ItemBoxSprite.transform.position.y + this.itemNameLabels[index].Height * units);
            else
                this.itemNameLabels[index].transform.position = this.itemNameLabels[index].transform.position.WithX(itemController.ItemBoxSprite.transform.position.x + (itemController.ItemBoxSprite.Size.x + 4f) * units).WithY(itemController.ItemBoxSprite.transform.position.y + this.itemNameLabels[index].Height * units);
            itemController.UpdateItem(item, items);
        }

        public void UpdatePlayerConsumables(PlayerConsumables playerConsumables)
        {
            this.p_playerCoinLabel.Text = IntToStringSansGarbage.GetStringForInt(playerConsumables.Currency);
            this.p_playerKeyLabel.Text = IntToStringSansGarbage.GetStringForInt(playerConsumables.KeyBullets);
            this.UpdateSpecialKeys(playerConsumables);
            if ((UnityEngine.Object) GameManager.Instance.PrimaryPlayer != (UnityEngine.Object) null && GameManager.Instance.PrimaryPlayer.Blanks == 0)
            {
                this.p_playerCoinLabel.Parent.Parent.RelativePosition = this.p_playerCoinLabel.Parent.Parent.RelativePosition.WithY(this.blankControllers[0].Panel.RelativePosition.y);
                this.p_playerKeyLabel.Parent.Parent.RelativePosition = this.p_playerKeyLabel.Parent.Parent.RelativePosition.WithY(this.blankControllers[0].Panel.RelativePosition.y);
            }
            else
            {
                this.p_playerCoinLabel.Parent.Parent.RelativePosition = this.p_playerCoinLabel.Parent.Parent.RelativePosition.WithY((float) ((double) this.blankControllers[0].Panel.RelativePosition.y + (double) this.blankControllers[0].Panel.Height - 9.0));
                this.p_playerKeyLabel.Parent.Parent.RelativePosition = this.p_playerKeyLabel.Parent.Parent.RelativePosition.WithY((float) ((double) this.blankControllers[0].Panel.RelativePosition.y + (double) this.blankControllers[0].Panel.Height - 9.0));
            }
            if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER)
                return;
            int input = Mathf.RoundToInt(GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY));
            if (input > 0)
            {
                this.p_playerCoinLabel.Text = IntToStringSansGarbage.GetStringForInt(input);
                if ((UnityEngine.Object) this.p_playerCoinSprite == (UnityEngine.Object) null)
                    this.p_playerCoinSprite = this.p_playerCoinLabel.Parent.GetComponentInChildren<dfSprite>();
                this.p_playerCoinSprite.SpriteName = "hbux_text_icon";
                this.p_playerCoinSprite.Size = this.p_playerCoinSprite.SpriteInfo.sizeInPixels * 3f;
            }
            else
            {
                if ((UnityEngine.Object) this.p_playerCoinSprite == (UnityEngine.Object) null)
                    this.p_playerCoinSprite = this.p_playerCoinLabel.Parent.GetComponentInChildren<dfSprite>();
                this.p_playerCoinLabel.IsVisible = false;
                this.p_playerCoinSprite.IsVisible = false;
            }
        }

        private void UpdateSpecialKeys(PlayerConsumables playerConsumables)
        {
            bool flag1 = false;
            bool flag2 = false;
            int resourcefulRatKeys = playerConsumables.ResourcefulRatKeys;
            for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
                for (int index2 = 0; index2 < allPlayer.additionalItems.Count; ++index2)
                {
                    if (allPlayer.additionalItems[index2] is NPCCellKeyItem)
                        flag1 = true;
                }
                for (int index3 = 0; index3 < allPlayer.passiveItems.Count; ++index3)
                {
                    if (allPlayer.passiveItems[index3] is SpecialKeyItem && (allPlayer.passiveItems[index3] as SpecialKeyItem).keyType == SpecialKeyItem.SpecialKeyType.RESOURCEFUL_RAT_LAIR)
                        flag2 = true;
                }
            }
            int count = this.m_extantSpecialKeySprites.Count;
            int num = resourcefulRatKeys + (!flag2 ? 0 : 1) + (!flag1 ? 0 : 1);
            if (num == count)
                return;
            for (int index = 0; index < this.m_extantSpecialKeySprites.Count; ++index)
                UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantSpecialKeySprites[index].gameObject);
            this.m_extantSpecialKeySprites.Clear();
            for (int index = 0; index < num; ++index)
            {
                dfSprite component = UnityEngine.Object.Instantiate<GameObject>(this.p_specialKeySprite.gameObject).GetComponent<dfSprite>();
                component.IsVisible = true;
                this.p_specialKeySprite.Parent.AddControl((dfControl) component);
                component.RelativePosition = this.p_specialKeySprite.RelativePosition + new Vector3((float) (33 * index), 0.0f, 0.0f);
                this.m_extantSpecialKeySprites.Add(component);
                bool flag3 = flag1 && index == 0;
                bool flag4 = !flag1 || !flag2 ? flag2 && index == 0 : index == 1;
                bool flag5 = !flag3 && !flag4;
                if (!flag3)
                {
                    if (flag4)
                    {
                        component.SpriteName = "resourcefulrat_key_001";
                        component.RelativePosition = component.RelativePosition + new Vector3(9f, 15f, 0.0f);
                    }
                    else if (flag5)
                    {
                        component.SpriteName = "room_rat_reward_key_001";
                        component.RelativePosition = component.RelativePosition + new Vector3(6f, 12f, 0.0f);
                    }
                }
                component.Size = component.SpriteInfo.sizeInPixels * 3f;
            }
            this.p_playerCoinLabel.Parent.Parent.RelativePosition = this.p_playerCoinLabel.Parent.Parent.RelativePosition.WithX(this.p_playerCoinLabel.Parent.Parent.RelativePosition.x + (float) ((num - count) * 33));
        }

        public bool AttemptActiveReload(PlayerController targetPlayer)
        {
            bool flag = this.m_extantReloadBars[!targetPlayer.IsPrimaryPlayer ? 1 : 0].AttemptActiveReload();
            if (!flag)
                ;
            return flag;
        }

        public void DoHealthBarForEnemy(AIActor sourceEnemy)
        {
            if (this.m_enemyToHealthbarMap.ContainsKey(sourceEnemy))
            {
                this.m_enemyToHealthbarMap[sourceEnemy].Value = sourceEnemy.healthHaver.GetCurrentHealthPercentage();
            }
            else
            {
                if (this.m_unusedHealthbars.Count > 0)
                    return;
                dfFollowObject component1 = this.m_manager.AddPrefab((GameObject) BraveResources.Load("Global Prefabs/EnemyHealthBar")).GetComponent<dfFollowObject>();
                component1.mainCamera = GameManager.Instance.MainCameraController.GetComponent<Camera>();
                component1.attach = sourceEnemy.gameObject;
                component1.offset = new Vector3(0.5f, 2f, 0.0f);
                component1.enabled = true;
                dfSlider component2 = component1.GetComponent<dfSlider>();
                component2.Value = sourceEnemy.healthHaver.GetCurrentHealthPercentage();
                this.m_enemyToHealthbarMap.Add(sourceEnemy, component2);
            }
        }

        public void ForceClearReload(int targetPlayerIndex = -1)
        {
            for (int index = 0; index < this.m_extantReloadBars.Count; ++index)
            {
                if (targetPlayerIndex == -1 || targetPlayerIndex == index)
                {
                    this.m_extantReloadBars[index].CancelReload();
                    this.m_extantReloadBars[index].UpdateStatusBars((PlayerController) null);
                }
            }
            for (int index = 0; index < this.m_displayingReloadNeeded.Count; ++index)
            {
                if (targetPlayerIndex == -1 || targetPlayerIndex == index)
                    this.m_displayingReloadNeeded[index] = false;
            }
        }

        public void InformNeedsReload(
            PlayerController attachPlayer,
            Vector3 offset,
            float customDuration = -1f,
            string customKey = "")
        {
            if (!(bool) (UnityEngine.Object) attachPlayer)
                return;
            int index = !attachPlayer.IsPrimaryPlayer ? 1 : 0;
            if (this.m_displayingReloadNeeded == null || index >= this.m_displayingReloadNeeded.Count || this.m_extantReloadLabels == null || index >= this.m_extantReloadLabels.Count || this.m_displayingReloadNeeded[index])
                return;
            dfLabel extantReloadLabel = this.m_extantReloadLabels[index];
            if ((UnityEngine.Object) extantReloadLabel == (UnityEngine.Object) null || extantReloadLabel.IsVisible)
                return;
            dfFollowObject component = extantReloadLabel.GetComponent<dfFollowObject>();
            extantReloadLabel.IsVisible = true;
            if ((bool) (UnityEngine.Object) component)
                component.enabled = false;
            this.StartCoroutine(this.FlashReloadLabel((dfControl) extantReloadLabel, attachPlayer, offset, customDuration, customKey));
        }

        protected override void InvariantUpdate(float realDeltaTime)
        {
            if (GameManager.Instance.IsLoadingLevel)
            {
                this.levelNameUI.BanishLevelNameText();
            }
            else
            {
                if (this.ForceLowerPanelsInvisibleOverride.HasOverride("conversation") && !GameManager.Instance.IsSelectingCharacter && GameManager.Instance.AllPlayers != null)
                {
                    bool flag = true;
                    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                    {
                        if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index] && GameManager.Instance.AllPlayers[index].CurrentInputState != PlayerInputState.AllInput)
                            flag = false;
                    }
                    if (flag)
                        this.ToggleLowerPanels(true, source: "conversation");
                }
                if (this.m_displayingPlayerConversationOptions || !this.ForceLowerPanelsInvisibleOverride.HasOverride("conversationBar"))
                    return;
                this.ToggleLowerPanels(true, source: "conversationBar");
            }
        }

        private void UpdateReloadLabelsOnCameraFinishedFrame()
        {
            for (int index = 0; index < this.m_displayingReloadNeeded.Count; ++index)
            {
                if (this.m_displayingReloadNeeded[index])
                {
                    PlayerController playerController = GameManager.Instance.PrimaryPlayer;
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && index != 0)
                        playerController = GameManager.Instance.SecondaryPlayer;
                    dfControl extantReloadLabel = (dfControl) this.m_extantReloadLabels[index];
                    float num1 = 0.125f;
                    if (this.m_extantReloadLabels[index].GetLocalizationKey() == "#RELOAD_FULL")
                        num1 = 3f / 16f;
                    float num2 = 0.0f;
                    if ((bool) (UnityEngine.Object) playerController && (bool) (UnityEngine.Object) playerController.CurrentGun && playerController.CurrentGun.Handedness == GunHandedness.NoHanded)
                        num2 += 0.5f;
                    Vector3 vector3 = new Vector3(playerController.specRigidbody.UnitCenter.x - playerController.transform.position.x + num1, playerController.SpriteDimensions.y + num2, 0.0f);
                    Vector2 vector2 = (Vector2) dfFollowObject.ConvertWorldSpaces(playerController.transform.position + vector3, GameManager.Instance.MainCameraController.Camera, this.Manager.RenderCamera).WithZ(0.0f);
                    extantReloadLabel.transform.position = (Vector3) vector2;
                    extantReloadLabel.transform.position = extantReloadLabel.transform.position.QuantizeFloor(extantReloadLabel.PixelsToUnits() / (Pixelator.Instance.ScaleTileScale / Pixelator.Instance.CurrentTileScale));
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator FlashReloadLabel(
            dfControl target,
            PlayerController attachPlayer,
            Vector3 offset,
            float customDuration = -1f,
            string customStringKey = "")
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__FlashReloadLabelc__Iterator9()
            {
                attachPlayer = attachPlayer,
                target = target,
                customStringKey = customStringKey,
                customDuration = customDuration,
                _this = this
            };
        }

        public void StartPlayerReloadBar(PlayerController attachObject, Vector3 offset, float duration)
        {
            int index = !attachObject.IsPrimaryPlayer ? 1 : 0;
            if (index >= 0 && index < this.m_displayingReloadNeeded.Count)
                this.m_displayingReloadNeeded[index] = false;
            this.m_extantReloadBars[index].TriggerReload(attachObject, offset, duration, 0.65f, 1);
        }

        public void TriggerBossKillCam(Projectile killerProjectile, SpeculativeRigidbody bossSRB)
        {
            if (this.m_bossKillCamActive)
                return;
            if (GameManager.Instance.InTutorial)
            {
                StaticReferenceManager.DestroyAllEnemyProjectiles();
            }
            else
            {
                StaticReferenceManager.DestroyAllEnemyProjectiles();
                this.m_bossKillCamActive = true;
                this.gameObject.AddComponent<BossKillCam>().TriggerSequence(killerProjectile, bossSRB);
            }
        }

        public void EndBossKillCam() => this.m_bossKillCamActive = false;

        public void ShowPauseMenu()
        {
            int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_pause_01", this.gameObject);
            GameUIRoot.Instance.ToggleLowerPanels(false, source: "gm_pause");
            GameUIRoot.Instance.HideCoreUI("gm_pause");
            this.levelNameUI.BanishLevelNameText();
            this.notificationController.ForceHide();
            GameUIRoot.Instance.ForceClearReload();
            PauseMenuController component = this.PauseMenuPanel.GetComponent<PauseMenuController>();
            this.PauseMenuPanel.IsVisible = true;
            this.PauseMenuPanel.IsInteractive = true;
            this.PauseMenuPanel.IsEnabled = true;
            component.SetDefaultFocus();
            component.ShwoopOpen();
            component.SetDefaultFocus();
            dfGUIManager.PushModal((dfControl) this.PauseMenuPanel);
        }

        public bool HasOpenPauseSubmenu()
        {
            if ((UnityEngine.Object) this.PauseMenuPanel == (UnityEngine.Object) null)
                return false;
            if ((UnityEngine.Object) this.m_pmc == (UnityEngine.Object) null)
                this.m_pmc = this.PauseMenuPanel.GetComponent<PauseMenuController>();
            if ((UnityEngine.Object) this.m_pmc == (UnityEngine.Object) null)
                return false;
            return (UnityEngine.Object) this.m_pmc.OptionsMenu != (UnityEngine.Object) null && (this.m_pmc.OptionsMenu.IsVisible || this.m_pmc.OptionsMenu.PreOptionsMenu.IsVisible || this.m_pmc.OptionsMenu.ModalKeyBindingDialog.IsVisible) || this.m_pmc.AdditionalMenuElementsToClear.Count > 0;
        }

        public void ReturnToBasePause()
        {
            this.PauseMenuPanel.GetComponent<PauseMenuController>().RevertToBaseState();
        }

        public void HidePauseMenu()
        {
            PauseMenuController component = this.PauseMenuPanel.GetComponent<PauseMenuController>();
            if (this.PauseMenuPanel.IsVisible)
                component.ShwoopClosed();
            this.PauseMenuPanel.IsInteractive = false;
            if ((UnityEngine.Object) component.OptionsMenu != (UnityEngine.Object) null)
            {
                component.OptionsMenu.IsVisible = false;
                component.OptionsMenu.PreOptionsMenu.IsVisible = false;
            }
            if (this.PauseMenuPanel.IsVisible)
                dfGUIManager.PopModalToControl((dfControl) this.PauseMenuPanel, true);
            if ((UnityEngine.Object) AmmonomiconController.Instance != (UnityEngine.Object) null && AmmonomiconController.Instance.IsOpen)
                AmmonomiconController.Instance.CloseAmmonomicon();
            int num1 = (int) AkSoundEngine.PostEvent("Play_UI_menu_cancel_01", this.gameObject);
            int num2 = (int) AkSoundEngine.PostEvent("Play_UI_menu_unpause_01", this.gameObject);
        }

        public bool DisplayingConversationBar => this.m_displayingPlayerConversationOptions;

        public void InitializeConversationPortrait(PlayerController player)
        {
            PlayableCharacters characterIdentity = player.characterIdentity;
            dfSprite component = this.ConversationBar.transform.Find("FacecardFrame/Facecard").GetComponent<dfSprite>();
            switch (characterIdentity)
            {
                case PlayableCharacters.Pilot:
                    component.SpriteName = "talking_bar_character_window_rogue_001";
                    break;
                case PlayableCharacters.Convict:
                    component.SpriteName = "talking_bar_character_window_convict_001";
                    break;
                case PlayableCharacters.Soldier:
                    component.SpriteName = "talking_bar_character_window_marine_001";
                    break;
                case PlayableCharacters.Guide:
                    component.SpriteName = "talking_bar_character_window_guide_001";
                    break;
                case PlayableCharacters.Bullet:
                    component.SpriteName = "talking_bar_character_window_bullet_001";
                    break;
                case PlayableCharacters.Gunslinger:
                    component.SpriteName = "talking_bar_character_window_slinger_003";
                    break;
            }
        }

        public bool DisplayPlayerConversationOptions(
            PlayerController interactingPlayer,
            TalkModule sourceModule,
            string overrideResponse1 = "",
            string overrideResponse2 = "")
        {
            int b = sourceModule == null ? 0 : sourceModule.responses.Count;
            if (!string.IsNullOrEmpty(overrideResponse1))
                b = Mathf.Max(1, b);
            if (!string.IsNullOrEmpty(overrideResponse2))
                b = Mathf.Max(2, b);
            string[] responses = new string[b];
            for (int index = 0; index < b; ++index)
            {
                if (sourceModule != null && sourceModule.responses.Count > index)
                    responses[index] = StringTableManager.GetString(sourceModule.responses[index].response);
            }
            if (!string.IsNullOrEmpty(overrideResponse1))
                responses[0] = overrideResponse1;
            if (!string.IsNullOrEmpty(overrideResponse2))
                responses[1] = overrideResponse2;
            return this.DisplayPlayerConversationOptions(interactingPlayer, responses);
        }

        public bool DisplayPlayerConversationOptions(
            PlayerController interactingPlayer,
            string[] responses)
        {
            if (this.m_displayingPlayerConversationOptions)
                return false;
            this.m_displayingPlayerConversationOptions = true;
            this.hasSelectedOption = false;
            this.selectedResponse = 0;
            for (int index = 0; index < this.itemControllers.Count; ++index)
                this.itemControllers[index].DimItemSprite();
            for (int index = 0; index < this.ammoControllers.Count; ++index)
                this.ammoControllers[index].DimGunSprite();
            this.ToggleLowerPanels(false, source: "conversationBar");
            this.StartCoroutine(this.HandlePlayerConversationResponse(interactingPlayer, responses));
            return true;
        }

        public void SetConversationResponse(int selected)
        {
            if (this.selectedResponse == selected)
                return;
            this.selectedResponse = selected;
            int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_select_01", this.gameObject);
        }

        public void SelectConversationResponse()
        {
            this.hasSelectedOption = true;
            int num = (int) AkSoundEngine.PostEvent("Play_UI_menu_confirm_01", this.gameObject);
        }

        [DebuggerHidden]
        private IEnumerator HandlePlayerConversationResponse(
            PlayerController interactingPlayer,
            string[] responses)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__HandlePlayerConversationResponsec__IteratorA()
            {
                interactingPlayer = interactingPlayer,
                responses = responses,
                _this = this
            };
        }

        public bool GetPlayerConversationResponse(out int responseIndex)
        {
            responseIndex = this.selectedResponse;
            return this.hasSelectedOption;
        }

        public static void ToggleBG(dfControl rawTarget)
        {
            switch (rawTarget)
            {
                case dfButton _:
                    dfButton dfButton = rawTarget as dfButton;
                    if (StringTableManager.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                    {
                        dfButton.BackgroundSprite = string.Empty;
                        dfButton.Padding = new RectOffset(0, 0, 0, 0);
                        break;
                    }
                    dfButton.BackgroundSprite = "chamber_flash_small_001";
                    dfButton.Padding = new RectOffset(6, 6, 0, 0);
                    dfButton.NormalBackgroundColor = (Color32) Color.black;
                    dfButton.FocusBackgroundColor = (Color32) Color.black;
                    dfButton.HoverBackgroundColor = (Color32) Color.black;
                    dfButton.DisabledColor = (Color32) Color.black;
                    dfButton.PressedBackgroundColor = (Color32) Color.black;
                    break;
                case dfLabel _:
                    dfLabel dfLabel = rawTarget as dfLabel;
                    if (StringTableManager.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                    {
                        dfLabel.BackgroundSprite = string.Empty;
                        dfLabel.Padding = new RectOffset(0, 0, 0, 0);
                        break;
                    }
                    dfLabel.BackgroundSprite = "chamber_flash_small_001";
                    dfLabel.Padding = new RectOffset(6, 6, 0, 0);
                    dfLabel.BackgroundColor = (Color32) Color.black;
                    break;
            }
        }

        public void CheckKeepModifiersQuickRestart(int requiredCredits)
        {
            this.m_hasSelectedAreYouSureOption = false;
            this.KeepMetasIsVisible = true;
            dfPanel QuestionPanel = (dfPanel) this.m_manager.AddPrefab((GameObject) BraveResources.Load("QuickRestartDetailsPanel"));
            QuestionPanel.BringToFront();
            dfGUIManager.PushModal((dfControl) QuestionPanel);
            dfControl component1 = QuestionPanel.transform.Find("AreYouSurePanelBGSlicedSprite").GetComponent<dfControl>();
            QuestionPanel.PerformLayout();
            component1.PerformLayout();
            dfButton component2 = QuestionPanel.transform.Find("YesButton").GetComponent<dfButton>();
            dfButton component3 = QuestionPanel.transform.Find("NoButton").GetComponent<dfButton>();
            component2.ModifyLocalizedText($"{component2.Text} ({requiredCredits.ToString()}[sprite \"hbux_text_icon\"])");
            float metas = GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.META_CURRENCY);
            if ((double) metas >= (double) requiredCredits)
            {
                component2.Focus(true);
            }
            else
            {
                component2.Disable();
                component3.GetComponent<UIKeyControls>().up = (dfControl) null;
                component3.Focus(true);
            }
            dfLabel component4 = QuestionPanel.transform.Find("TopLabel").GetComponent<dfLabel>();
            component4.IsLocalized = true;
            component4.Text = component4.getLocalizedValue("#QUICKRESTARTDETAIL");
            Action<bool> HandleChoice = (Action<bool>) (choice =>
            {
                if (this.m_hasSelectedAreYouSureOption)
                    return;
                if (choice)
                {
                    GameStatsManager.Instance.ClearStatValueGlobal(TrackedStats.META_CURRENCY);
                    GameStatsManager.Instance.SetStat(TrackedStats.META_CURRENCY, metas - (float) requiredCredits);
                }
                this.m_hasSelectedAreYouSureOption = true;
                this.m_AreYouSureSelection = choice;
                dfGUIManager.PopModal();
                QuestionPanel.IsVisible = false;
                this.KeepMetasIsVisible = false;
            });
            component2.Click += (MouseEventHandler) ((control, mouseEvent) =>
            {
                mouseEvent.Use();
                HandleChoice(true);
            });
            component3.Click += (MouseEventHandler) ((control, mouseEvent) =>
            {
                mouseEvent.Use();
                HandleChoice(false);
            });
            this.StartCoroutine(this.DelayedCenterControl(component1));
        }

        [DebuggerHidden]
        private IEnumerator DelayedCenterControl(dfControl panel)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new GameUIRoot__DelayedCenterControlc__IteratorB()
            {
                panel = panel
            };
        }

        public void DoAreYouSure(string questionKey, bool focusYes = false, string secondaryKey = null)
        {
            this.m_hasSelectedAreYouSureOption = false;
            this.AreYouSurePanel.IsVisible = true;
            dfGUIManager.PushModal((dfControl) this.AreYouSurePanel);
            if (focusYes)
                this.m_AreYouSureYesButton.Focus(true);
            else
                this.m_AreYouSureNoButton.Focus(true);
            GameUIRoot.ToggleBG((dfControl) this.m_AreYouSureYesButton);
            GameUIRoot.ToggleBG((dfControl) this.m_AreYouSureNoButton);
            GameUIRoot.ToggleBG((dfControl) this.m_AreYouSurePrimaryLabel);
            GameUIRoot.ToggleBG((dfControl) this.m_AreYouSureSecondaryLabel);
            this.m_AreYouSurePrimaryLabel.IsLocalized = true;
            this.m_AreYouSurePrimaryLabel.Text = this.m_AreYouSurePrimaryLabel.getLocalizedValue(questionKey);
            if (!string.IsNullOrEmpty(secondaryKey))
            {
                this.m_AreYouSureSecondaryLabel.IsLocalized = true;
                this.m_AreYouSureSecondaryLabel.Text = this.m_AreYouSureSecondaryLabel.getLocalizedValue(secondaryKey);
                if (this.m_AreYouSureSecondaryLabel.Text.Contains("%CURRENTSLOT"))
                {
                    string key;
                    switch (SaveManager.CurrentSaveSlot)
                    {
                        case SaveManager.SaveSlot.A:
                            key = "#OPTIONS_SAVESLOTA";
                            break;
                        case SaveManager.SaveSlot.B:
                            key = "#OPTIONS_SAVESLOTB";
                            break;
                        case SaveManager.SaveSlot.C:
                            key = "#OPTIONS_SAVESLOTC";
                            break;
                        case SaveManager.SaveSlot.D:
                            key = "#OPTIONS_SAVESLOTD";
                            break;
                        default:
                            key = "#OPTIONS_SAVESLOTA";
                            break;
                    }
                    this.m_AreYouSureSecondaryLabel.ModifyLocalizedText(StringTableManager.PostprocessString(this.m_AreYouSureSecondaryLabel.Text.Replace("%CURRENTSLOT", this.m_AreYouSureSecondaryLabel.getLocalizedValue(key))));
                }
                else
                    this.m_AreYouSureSecondaryLabel.ModifyLocalizedText(StringTableManager.PostprocessString(this.m_AreYouSureSecondaryLabel.Text));
                this.m_AreYouSureSecondaryLabel.IsVisible = true;
            }
            else
                this.m_AreYouSureSecondaryLabel.IsVisible = false;
            this.m_AreYouSureYesButton.Click += new MouseEventHandler(this.SelectedAreYouSureYes);
            this.m_AreYouSureNoButton.Click += new MouseEventHandler(this.SelectedAreYouSureNo);
        }

        private void SelectedAreYouSureNo(dfControl control, dfMouseEventArgs mouseEvent)
        {
            mouseEvent.Use();
            this.SelectAreYouSureOption(false);
        }

        private void SelectedAreYouSureYes(dfControl control, dfMouseEventArgs mouseEvent)
        {
            mouseEvent.Use();
            this.SelectAreYouSureOption(true);
        }

        public void SelectAreYouSureOption(bool isSure)
        {
            this.m_AreYouSureNoButton.Click -= new MouseEventHandler(this.SelectedAreYouSureNo);
            this.m_AreYouSureYesButton.Click -= new MouseEventHandler(this.SelectedAreYouSureYes);
            this.m_hasSelectedAreYouSureOption = true;
            this.m_AreYouSureSelection = isSure;
            dfGUIManager.PopModal();
            this.AreYouSurePanel.IsVisible = false;
        }

        public bool HasSelectedAreYouSureOption() => this.m_hasSelectedAreYouSureOption;

        public bool GetAreYouSureOption() => this.m_AreYouSureSelection;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameUIRoot.Instance = (GameUIRoot) null;
        }
    }

