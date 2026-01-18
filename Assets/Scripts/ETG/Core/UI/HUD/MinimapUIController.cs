using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using InControl;
using UnityEngine;

using Dungeonator;

#nullable disable

public class MinimapUIController : MonoBehaviour
    {
        public dfSprite DockSprite;
        private dfGUIManager m_manager;
        public dfPanel QuestPanel;
        public dfPanel GrabbyPanel;
        public dfPanel ItemPanel_PC;
        public dfPanel ItemPanel_PC_Foreign;
        public dfPanel SonyControlsPanel01;
        public dfPanel SonyControlsPanel02;
        public dfPanel SonyControlsPanel01Foreign;
        public dfPanel SonyControlsPanel02Foreign;
        public dfPanel DockPanel;
        public dfPanel CoopDockPanelLeft;
        public dfPanel CoopDockPanelRight;
        public dfSprite ControllerCrosshair;
        public dfLabel LevelNameLabel;
        public dfButton DropItemButton;
        public dfSprite DropItemSprite;
        public dfLabel DropItemLabel;
        public dfButton DropItemButtonForeign;
        public dfSprite DropItemSpriteForeign;
        public dfLabel DropItemLabelForeign;
        public List<dfControl> AdditionalControlsToToggle;
        public Camera minimapCamera;
        public dfSprite TurboModeIndicator;
        public dfSprite DispenserIcon;
        public dfLabel DispenserLabel;
        public dfSprite RatTaunty;
        private int m_targetDockIndex;
        private int m_selectedDockItemIndex = -1;
        private List<Tuple<tk2dSprite, PassiveItem>> dockItems = new List<Tuple<tk2dSprite, PassiveItem>>();
        private List<Tuple<tk2dSprite, PassiveItem>> secondaryDockItems = new List<Tuple<tk2dSprite, PassiveItem>>();
        private List<dfControl> panels;
        private Dictionary<dfControl, Vector3> activePositions;
        private Dictionary<dfControl, Vector3> inactivePositions;
        private Dictionary<dfControl, Tuple<float, float>> panelTimings;
        private bool m_active;
        private bool m_isPanning;
        private Vector3 m_lastMousePosition;
        private float m_panPixelDistTravelled;
        private tk2dBaseSprite m_currentTeleportIconSprite;
        private RoomHandler m_currentTeleportTarget;
        private int m_currentTeleportTargetIndex;
        private const float ITEM_SPACING_MPX = 20f;
        private const int NUM_ITEMS_PER_LINE = 12;
        private const int NUM_ITEMS_PER_LINE_COOP = 5;
        private Vector3? m_cachedCoopDockPanelLeftRelativePosition;
        private Vector3? m_cachedCoopDockPanelRightRelativePosition;
        private List<dfSprite> extantSynergyArrows = new List<dfSprite>();

        private List<Tuple<tk2dSprite, PassiveItem>> SelectedDockItems
        {
            get => this.m_targetDockIndex == 1 ? this.secondaryDockItems : this.dockItems;
        }

        private Vector3 GetActivePosition(dfPanel panel, DungeonData.Direction direction)
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

        private Vector3 GetInactivePosition(dfPanel panel, DungeonData.Direction direction)
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

        private void InitializeMasterPanel(dfPanel panel, float startTime, float endTime)
        {
            dfPanel dfPanel = panel;
            dfPanel.ResolutionChangedPostLayout = dfPanel.ResolutionChangedPostLayout + new Action<dfControl, Vector3, Vector3>(this.OnControlResolutionChanged);
            this.panels.Add((dfControl) panel);
            this.panelTimings.Add((dfControl) panel, new Tuple<float, float>(startTime, endTime));
        }

        private void Start()
        {
            this.m_manager = this.GrabbyPanel.GetManager();
            this.m_manager.UIScaleLegacyMode = false;
            this.DropItemButton.Click += (MouseEventHandler) ((control, mouseEvent) => this.DropSelectedItem());
            this.DropItemButtonForeign.Click += (MouseEventHandler) ((control, mouseEvent) => this.DropSelectedItem());
            this.panels = new List<dfControl>();
            this.panelTimings = new Dictionary<dfControl, Tuple<float, float>>();
            this.InitializeMasterPanel(this.GrabbyPanel, 0.0f, 0.6f);
            this.InitializeMasterPanel(this.ItemPanel_PC, 0.2f, 0.8f);
            this.InitializeMasterPanel(this.ItemPanel_PC_Foreign, 0.2f, 0.8f);
            this.InitializeMasterPanel(this.SonyControlsPanel01, 0.0f, 0.6f);
            this.InitializeMasterPanel(this.SonyControlsPanel02, 0.2f, 0.8f);
            this.InitializeMasterPanel(this.SonyControlsPanel01Foreign, 0.0f, 0.6f);
            this.InitializeMasterPanel(this.SonyControlsPanel02Foreign, 0.2f, 0.8f);
            this.InitializeMasterPanel(this.DockPanel, 0.0f, 0.8f);
            this.InitializeMasterPanel(this.CoopDockPanelLeft, 0.0f, 0.8f);
            this.InitializeMasterPanel(this.CoopDockPanelRight, 0.0f, 0.8f);
            this.RecalculatePositions();
        }

        private void PostprocessPassiveDockSprite(PassiveItem item, tk2dSprite itemSprite)
        {
            if (!(item is YellowChamberItem))
                return;
            itemSprite.gameObject.GetOrAddComponent<tk2dSpriteAnimator>().Library = item.GetComponent<tk2dSpriteAnimator>().Library;
        }

        public void AddPassiveItemToDock(PassiveItem item, PlayerController itemOwner)
        {
            if ((bool) (UnityEngine.Object) item && (bool) (UnityEngine.Object) item.encounterTrackable && item.encounterTrackable.SuppressInInventory)
                return;
            for (int index = 0; index < this.dockItems.Count; ++index)
            {
                if ((UnityEngine.Object) this.dockItems[index].Second == (UnityEngine.Object) item)
                    return;
            }
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                for (int index = 0; index < this.secondaryDockItems.Count; ++index)
                {
                    if ((UnityEngine.Object) this.secondaryDockItems[index].Second == (UnityEngine.Object) item)
                        return;
                }
                if (itemOwner.IsPrimaryPlayer)
                {
                    tk2dSprite panel = this.AddTK2DSpriteToPanel(item.GetComponent<tk2dBaseSprite>(), this.CoopDockPanelLeft.transform);
                    this.PostprocessPassiveDockSprite(item, panel);
                    this.dockItems.Add(new Tuple<tk2dSprite, PassiveItem>(panel, item));
                }
                else
                {
                    tk2dSprite panel = this.AddTK2DSpriteToPanel(item.GetComponent<tk2dBaseSprite>(), this.CoopDockPanelRight.transform);
                    this.PostprocessPassiveDockSprite(item, panel);
                    this.secondaryDockItems.Add(new Tuple<tk2dSprite, PassiveItem>(panel, item));
                }
            }
            else
            {
                tk2dSprite panel = this.AddTK2DSpriteToPanel(item.GetComponent<tk2dBaseSprite>(), this.DockPanel.transform);
                this.PostprocessPassiveDockSprite(item, panel);
                this.dockItems.Add(new Tuple<tk2dSprite, PassiveItem>(panel, item));
            }
        }

        public void InfoSelectedItem()
        {
            if (this.m_selectedDockItemIndex == -1 || this.m_selectedDockItemIndex >= this.SelectedDockItems.Count || !(bool) (UnityEngine.Object) this.SelectedDockItems[this.m_selectedDockItemIndex].Second.encounterTrackable)
                return;
            EncounterTrackable encounterTrackable = this.SelectedDockItems[this.m_selectedDockItemIndex].Second.encounterTrackable;
            if (encounterTrackable.journalData.SuppressInAmmonomicon)
                return;
            Minimap.Instance.ToggleMinimap(false);
            GameManager.Instance.Pause();
            GameUIRoot.Instance.PauseMenuPanel.GetComponent<PauseMenuController>().DoShowBestiaryToTarget(encounterTrackable);
        }

        public void DropSelectedItem()
        {
            this.ClearSynergyHighlights();
            if (this.m_selectedDockItemIndex == -1)
                return;
            PlayerController owner = GameManager.Instance.PrimaryPlayer;
            if (this.m_targetDockIndex == 1)
                owner = GameManager.Instance.SecondaryPlayer;
            if (this.m_selectedDockItemIndex >= this.SelectedDockItems.Count)
            {
                int index = this.m_selectedDockItemIndex - this.SelectedDockItems.Count;
                if (!owner.inventory.AllGuns[index].CanActuallyBeDropped(owner))
                    return;
                owner.ForceDropGun(owner.inventory.AllGuns[index]);
                this.m_selectedDockItemIndex = -1;
            }
            else
            {
                PassiveItem second = this.SelectedDockItems[this.m_selectedDockItemIndex].Second;
                if (!second.CanActuallyBeDropped(second.Owner))
                    return;
                owner.DropPassiveItem(second);
                this.m_selectedDockItemIndex = -1;
            }
        }

        public void RemovePassiveItemFromDock(PassiveItem item)
        {
            for (int index = 0; index < this.dockItems.Count; ++index)
            {
                Tuple<tk2dSprite, PassiveItem> dockItem = this.dockItems[index];
                if ((UnityEngine.Object) dockItem.Second == (UnityEngine.Object) item)
                {
                    UnityEngine.Object.Destroy((UnityEngine.Object) dockItem.First.gameObject);
                    this.dockItems.RemoveAt(index);
                    break;
                }
            }
            for (int index = 0; index < this.secondaryDockItems.Count; ++index)
            {
                Tuple<tk2dSprite, PassiveItem> secondaryDockItem = this.secondaryDockItems[index];
                if ((UnityEngine.Object) secondaryDockItem.Second == (UnityEngine.Object) item)
                {
                    UnityEngine.Object.Destroy((UnityEngine.Object) secondaryDockItem.First.gameObject);
                    this.secondaryDockItems.RemoveAt(index);
                    break;
                }
            }
        }

        protected tk2dSprite AddTK2DSpriteToPanel(tk2dBaseSprite sourceSprite, Transform parent)
        {
            GameObject go = new GameObject("tk2d item sprite");
            go.transform.parent = parent;
            go.layer = LayerMask.NameToLayer("SecondaryGUI");
            tk2dSprite panel = tk2dBaseSprite.AddComponent<tk2dSprite>(go, sourceSprite.Collection, sourceSprite.spriteId);
            Bounds untrimmedBounds = panel.GetUntrimmedBounds();
            Vector2 vector2 = GameUIUtility.TK2DtoDF(untrimmedBounds.size.XY());
            panel.scale = new Vector3(vector2.x / untrimmedBounds.size.x, vector2.y / untrimmedBounds.size.y, untrimmedBounds.size.z);
            panel.ignoresTiltworldDepth = true;
            go.transform.localPosition = Vector3.zero;
            return panel;
        }

        protected void OnControlResolutionChanged(
            dfControl source,
            Vector3 oldRelativePosition,
            Vector3 newRelativePosition)
        {
        }

        protected void RecalculatePositions()
        {
            if (this.activePositions == null)
                this.activePositions = new Dictionary<dfControl, Vector3>();
            if (this.inactivePositions == null)
                this.inactivePositions = new Dictionary<dfControl, Vector3>();
            this.activePositions.Clear();
            this.inactivePositions.Clear();
            this.activePositions.Add((dfControl) this.GrabbyPanel, this.GetActivePosition(this.GrabbyPanel, DungeonData.Direction.WEST));
            this.inactivePositions.Add((dfControl) this.GrabbyPanel, this.GetInactivePosition(this.GrabbyPanel, DungeonData.Direction.WEST));
            this.activePositions.Add((dfControl) this.ItemPanel_PC, this.GetActivePosition(this.ItemPanel_PC, DungeonData.Direction.WEST));
            this.inactivePositions.Add((dfControl) this.ItemPanel_PC, this.GetInactivePosition(this.ItemPanel_PC, DungeonData.Direction.WEST));
            this.activePositions.Add((dfControl) this.ItemPanel_PC_Foreign, this.GetActivePosition(this.ItemPanel_PC_Foreign, DungeonData.Direction.WEST));
            this.inactivePositions.Add((dfControl) this.ItemPanel_PC_Foreign, this.GetInactivePosition(this.ItemPanel_PC_Foreign, DungeonData.Direction.WEST));
            this.activePositions.Add((dfControl) this.SonyControlsPanel01, this.GetActivePosition(this.SonyControlsPanel01, DungeonData.Direction.WEST));
            this.inactivePositions.Add((dfControl) this.SonyControlsPanel01, this.GetInactivePosition(this.SonyControlsPanel01, DungeonData.Direction.WEST));
            this.activePositions.Add((dfControl) this.SonyControlsPanel02, this.GetActivePosition(this.SonyControlsPanel02, DungeonData.Direction.WEST));
            this.inactivePositions.Add((dfControl) this.SonyControlsPanel02, this.GetInactivePosition(this.SonyControlsPanel02, DungeonData.Direction.WEST));
            this.activePositions.Add((dfControl) this.SonyControlsPanel01Foreign, this.GetActivePosition(this.SonyControlsPanel01Foreign, DungeonData.Direction.WEST));
            this.inactivePositions.Add((dfControl) this.SonyControlsPanel01Foreign, this.GetInactivePosition(this.SonyControlsPanel01Foreign, DungeonData.Direction.WEST));
            this.activePositions.Add((dfControl) this.SonyControlsPanel02Foreign, this.GetActivePosition(this.SonyControlsPanel02Foreign, DungeonData.Direction.WEST));
            this.inactivePositions.Add((dfControl) this.SonyControlsPanel02Foreign, this.GetInactivePosition(this.SonyControlsPanel02Foreign, DungeonData.Direction.WEST));
            this.activePositions.Add((dfControl) this.DockPanel, this.GetActivePosition(this.DockPanel, DungeonData.Direction.SOUTH));
            this.inactivePositions.Add((dfControl) this.DockPanel, this.GetInactivePosition(this.DockPanel, DungeonData.Direction.SOUTH));
            this.activePositions.Add((dfControl) this.CoopDockPanelLeft, this.GetActivePosition(this.CoopDockPanelLeft, DungeonData.Direction.SOUTH));
            this.inactivePositions.Add((dfControl) this.CoopDockPanelLeft, this.GetInactivePosition(this.CoopDockPanelLeft, DungeonData.Direction.SOUTH));
            this.activePositions.Add((dfControl) this.CoopDockPanelRight, this.GetActivePosition(this.CoopDockPanelRight, DungeonData.Direction.SOUTH));
            this.inactivePositions.Add((dfControl) this.CoopDockPanelRight, this.GetInactivePosition(this.CoopDockPanelRight, DungeonData.Direction.SOUTH));
        }

        private void PostStateChanged(bool newState)
        {
            this.TurboModeIndicator.IsVisible = GameManager.IsTurboMode;
            this.DispenserLabel.Text = HeartDispenser.CurrentHalfHeartsStored.ToString();
            this.DispenserLabel.IsVisible = HeartDispenser.CurrentHalfHeartsStored > 0;
            this.DispenserIcon.IsVisible = HeartDispenser.CurrentHalfHeartsStored > 0;
            for (int index = 0; index < this.dockItems.Count; ++index)
            {
                if (this.dockItems[index].Second is YellowChamberItem)
                {
                    if (newState)
                    {
                        if ((double) UnityEngine.Random.value < 0.10000000149011612)
                        {
                            if ((double) UnityEngine.Random.value < 0.25)
                                this.StartCoroutine(this.HandleDelayedAnimation(this.dockItems[index].First.spriteAnimator, "yellow_chamber_eye", UnityEngine.Random.Range(2.5f, 10f)));
                            else
                                this.StartCoroutine(this.HandleDelayedAnimation(this.dockItems[index].First.spriteAnimator, "yellow_chamber_blink", UnityEngine.Random.Range(2.5f, 10f)));
                        }
                        else
                            this.dockItems[index].First.spriteAnimator.StopAndResetFrameToDefault();
                    }
                    else
                        this.dockItems[index].First.spriteAnimator.Stop();
                }
            }
            for (int index = 0; index < this.secondaryDockItems.Count; ++index)
            {
                if (this.secondaryDockItems[index].Second is YellowChamberItem)
                {
                    if (newState)
                    {
                        if ((double) UnityEngine.Random.value < 0.10000000149011612)
                        {
                            if ((double) UnityEngine.Random.value < 0.25)
                                this.StartCoroutine(this.HandleDelayedAnimation(this.secondaryDockItems[index].First.spriteAnimator, "yellow_chamber_eye", UnityEngine.Random.Range(2.5f, 10f)));
                            else
                                this.StartCoroutine(this.HandleDelayedAnimation(this.secondaryDockItems[index].First.spriteAnimator, "yellow_chamber_blink", UnityEngine.Random.Range(2.5f, 10f)));
                        }
                    }
                    else
                        this.secondaryDockItems[index].First.spriteAnimator.Stop();
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator HandleDelayedAnimation(
            tk2dSpriteAnimator targetAnimator,
            string animationName,
            float delayTime)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MinimapUIController__HandleDelayedAnimationc__Iterator0()
            {
                delayTime = delayTime,
                targetAnimator = targetAnimator,
                animationName = animationName,
                _this = this
            };
        }

        public void ToggleState(bool active)
        {
            if (active == this.m_active)
                return;
            if (active)
            {
                this.Activate();
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                {
                    PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                    if ((bool) (UnityEngine.Object) allPlayer)
                        allPlayer.CurrentInputState = PlayerInputState.OnlyMovement;
                }
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    dfSprite componentInChildren1 = this.CoopDockPanelLeft.GetComponentInChildren<dfSprite>();
                    dfSprite componentInChildren2 = this.CoopDockPanelRight.GetComponentInChildren<dfSprite>();
                    if (!this.m_cachedCoopDockPanelLeftRelativePosition.HasValue)
                        this.m_cachedCoopDockPanelLeftRelativePosition = new Vector3?(componentInChildren1.RelativePosition);
                    if (!this.m_cachedCoopDockPanelRightRelativePosition.HasValue)
                        this.m_cachedCoopDockPanelRightRelativePosition = new Vector3?(componentInChildren2.RelativePosition);
                    this.ArrangeDockItems(this.dockItems, componentInChildren1, 1);
                    this.ArrangeDockItems(this.secondaryDockItems, componentInChildren2, 2);
                }
                else
                    this.ArrangeDockItems(this.dockItems, this.DockSprite);
            }
            else
            {
                this.Deactivate();
                for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                {
                    if ((bool) (UnityEngine.Object) GameManager.Instance.AllPlayers[index])
                        GameManager.Instance.AllPlayers[index].CurrentInputState = PlayerInputState.AllInput;
                }
            }
            this.PostStateChanged(active);
        }

        protected void ArrangeDockItems(
            List<Tuple<tk2dSprite, PassiveItem>> targetDockItems,
            dfSprite targetDockSprite,
            int targetIndex = 0)
        {
            float num1 = this.DockPanel.PixelsToUnits() * Pixelator.Instance.CurrentTileScale;
            int count = targetDockItems.Count;
            float b = GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER ? 270f : 132f;
            float num2 = b / (float) count;
            float a = Pixelator.Instance.CurrentTileScale;
            for (int index = 0; index < count; ++index)
                a = a + targetDockItems[index].First.GetBounds().size.x / num1 + Pixelator.Instance.CurrentTileScale;
            float num3 = (float) (20.0 * (double) Pixelator.Instance.CurrentTileScale / 3.0);
            targetDockSprite.Width = Mathf.Min(a, b).Quantize(3f);
            targetDockSprite.Height = num3;
            if (targetIndex == 1 && this.m_cachedCoopDockPanelLeftRelativePosition.HasValue)
                targetDockSprite.RelativePosition = targetDockSprite.RelativePosition.WithX((float) ((double) targetDockSprite.Parent.Width - 132.0 - (132.0 - (double) targetDockSprite.Width) * 2.0));
            else if (targetIndex == 2 && this.m_cachedCoopDockPanelRightRelativePosition.HasValue)
                targetDockSprite.RelativePosition = targetDockSprite.RelativePosition.WithX((float) ((132.0 - (double) targetDockSprite.Width) * 3.0));
            targetDockSprite.PerformLayout();
            Vector3 corner = targetDockSprite.GetCorners()[2];
            float num4 = 0.0f;
            if (targetIndex != 2)
                num4 = (float) (20.0 * (double) Pixelator.Instance.CurrentTileScale / 6.0) * num1;
            for (int index = 0; index < count; ++index)
            {
                tk2dSprite first = targetDockItems[index].First;
                first.PlaceAtPositionByAnchor(corner, tk2dBaseSprite.Anchor.LowerCenter);
                float y = Pixelator.Instance.CurrentTileScale * 2f * num1;
                float x;
                if ((double) a < (double) b)
                {
                    if (index != 0 || targetIndex == 2)
                        num4 += targetDockItems[index].First.GetBounds().size.x / 2f + num1;
                    x = num4;
                    num4 += targetDockItems[index].First.GetBounds().size.x / 2f + num1;
                }
                else
                    x = num4 + num2 * num1 * (float) index;
                first.transform.localPosition += new Vector3(x, y, 0.0f);
            }
        }

        protected void OldArrangeDockItems(
            List<Tuple<tk2dSprite, PassiveItem>> targetDockItems,
            dfSprite targetDockSprite)
        {
            float units = this.DockPanel.PixelsToUnits();
            int a = GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER ? 12 : 5;
            int num1 = Mathf.CeilToInt((float) targetDockItems.Count / (1f * (float) a));
            float num2 = (float) ((double) num1 * 20.0 * (double) Pixelator.Instance.CurrentTileScale / 3.0);
            float num3 = a < targetDockItems.Count ? (float) ((double) (a + 1) * (20.0 * (double) Pixelator.Instance.CurrentTileScale) / 3.0) : (float) ((double) (targetDockItems.Count + 1) * (20.0 * (double) Pixelator.Instance.CurrentTileScale) / 3.0);
            targetDockSprite.Width = num3;
            targetDockSprite.Height = num2;
            targetDockSprite.PerformLayout();
            Vector3 corner = targetDockSprite.GetCorners()[2];
            int count = targetDockItems.Count;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                int num4 = Mathf.Min(a, count);
                for (int index2 = 0; index2 < num4 && count > 0; ++index2)
                {
                    int index3 = targetDockItems.Count - count;
                    tk2dSprite first = targetDockItems[index3].First;
                    first.PlaceAtPositionByAnchor(corner, tk2dBaseSprite.Anchor.LowerCenter);
                    float x = 20f * Pixelator.Instance.CurrentTileScale * units * (float) (index2 + 1);
                    float y = 20f * Pixelator.Instance.CurrentTileScale * units * (float) index1 + Pixelator.Instance.CurrentTileScale * 5f * units;
                    first.transform.localPosition += new Vector3(x, y, 0.0f);
                    --count;
                }
                if (count <= 0)
                    break;
            }
        }

        private void HandlePanelVisibility()
        {
            bool flag1 = false;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(GameManager.Instance.AllPlayers[index].PlayerIDX);
                if (instanceForPlayer.ActiveActions.MapAction.IsPressed)
                    flag1 = instanceForPlayer.IsKeyboardAndMouse();
            }
            bool flag2 = this.dockItems.Count > 0;
            bool flag3 = this.secondaryDockItems != null && this.secondaryDockItems.Count > 0;
            bool flag4 = flag2 && this.m_selectedDockItemIndex != -1;
            if (flag1)
            {
                this.GrabbyPanel.IsVisible = true;
                if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                {
                    this.ItemPanel_PC.IsVisible = flag4;
                    this.ItemPanel_PC_Foreign.IsVisible = false;
                    this.ItemPanel_PC.Parent.IsInteractive = true;
                    this.ItemPanel_PC_Foreign.Parent.IsInteractive = false;
                }
                else
                {
                    this.ItemPanel_PC_Foreign.IsVisible = flag4;
                    this.ItemPanel_PC.IsVisible = false;
                    this.ItemPanel_PC.Parent.IsInteractive = false;
                    this.ItemPanel_PC_Foreign.Parent.IsInteractive = true;
                }
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    this.CoopDockPanelLeft.IsVisible = flag2;
                    this.CoopDockPanelRight.IsVisible = flag3;
                    this.DockPanel.IsVisible = false;
                }
                else
                {
                    this.CoopDockPanelLeft.IsVisible = false;
                    this.CoopDockPanelRight.IsVisible = false;
                    this.DockPanel.IsVisible = flag2;
                }
                this.SonyControlsPanel01.IsVisible = false;
                this.SonyControlsPanel02.IsVisible = false;
                this.SonyControlsPanel01Foreign.IsVisible = false;
                this.SonyControlsPanel02Foreign.IsVisible = false;
            }
            else
            {
                this.GrabbyPanel.IsVisible = false;
                this.ItemPanel_PC.IsVisible = false;
                this.ItemPanel_PC_Foreign.IsVisible = false;
                if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
                {
                    this.CoopDockPanelLeft.IsVisible = flag2;
                    this.CoopDockPanelRight.IsVisible = flag3;
                    this.DockPanel.IsVisible = false;
                }
                else
                {
                    this.CoopDockPanelLeft.IsVisible = false;
                    this.CoopDockPanelRight.IsVisible = false;
                    this.DockPanel.IsVisible = flag2;
                }
                if (GameManager.Instance.PrimaryPlayer.inventory != null && GameManager.Instance.PrimaryPlayer.inventory.AllGuns.Count >= 5)
                {
                    this.SonyControlsPanel01.IsVisible = false;
                    this.SonyControlsPanel02.IsVisible = false;
                    this.SonyControlsPanel01Foreign.IsVisible = false;
                    this.SonyControlsPanel02Foreign.IsVisible = false;
                }
                else if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                {
                    this.SonyControlsPanel01.IsVisible = true;
                    this.SonyControlsPanel02.IsVisible = flag2;
                }
                else
                {
                    this.SonyControlsPanel01Foreign.IsVisible = true;
                    this.SonyControlsPanel02Foreign.IsVisible = flag2;
                }
            }
        }

        private Vector2 ModifyMousePosition(Vector2 inputPosition) => inputPosition;

        private void Update()
        {
            this.m_manager.UIScale = Pixelator.Instance.ScaleTileScale / 3f;
            Vector2 screenSize = this.CoopDockPanelLeft.GUIManager.GetScreenSize();
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                float x = screenSize.x / 2f;
                this.CoopDockPanelLeft.Parent.Width = x;
                this.CoopDockPanelRight.Parent.Width = x;
                this.CoopDockPanelLeft.Parent.RelativePosition = this.CoopDockPanelLeft.Parent.RelativePosition.WithX(0.0f);
                this.CoopDockPanelRight.Parent.RelativePosition = this.CoopDockPanelRight.Parent.RelativePosition.WithX(x);
                this.CoopDockPanelLeft.Parent.RelativePosition = this.CoopDockPanelLeft.Parent.RelativePosition.WithY(screenSize.y - this.CoopDockPanelLeft.Size.y);
                this.CoopDockPanelRight.Parent.RelativePosition = this.CoopDockPanelRight.Parent.RelativePosition.WithY(screenSize.y - this.CoopDockPanelRight.Size.y);
            }
            else
                this.DockPanel.Parent.RelativePosition = this.DockPanel.Parent.RelativePosition.WithY(screenSize.y - this.DockPanel.Size.y);
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                GungeonActions activeActions = BraveInput.GetInstanceForPlayer(GameManager.Instance.AllPlayers[index].PlayerIDX).ActiveActions;
                if (activeActions != null)
                {
                    if (activeActions.MinimapZoomOutAction.WasPressed)
                        Minimap.Instance.AttemptZoomMinimap(0.2f);
                    if (activeActions.MinimapZoomInAction.WasPressed)
                        Minimap.Instance.AttemptZoomMinimap(-0.2f);
                }
            }
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.RESOURCEFUL_RAT)
                this.RatTaunty.IsVisible = true;
            if (this.m_active)
            {
                if (Minimap.Instance.HeldOpen)
                {
                    GungeonActions activeActions1 = BraveInput.PrimaryPlayerInstance.ActiveActions;
                    if (activeActions1.MapAction.WasPressed || activeActions1.PauseAction.WasPressed)
                    {
                        activeActions1.MapAction.Suppress();
                        activeActions1.PauseAction.Suppress();
                        Minimap.Instance.ToggleMinimap(false);
                        return;
                    }
                    if ((UnityEngine.Object) BraveInput.SecondaryPlayerInstance != (UnityEngine.Object) null)
                    {
                        GungeonActions activeActions2 = BraveInput.SecondaryPlayerInstance.ActiveActions;
                        if (activeActions2.MapAction.WasPressed || activeActions2.PauseAction.WasPressed)
                        {
                            activeActions2.MapAction.Suppress();
                            activeActions2.PauseAction.Suppress();
                            Minimap.Instance.ToggleMinimap(false);
                            return;
                        }
                    }
                }
                else
                {
                    if (BraveInput.PrimaryPlayerInstance.ActiveActions.MapAction.WasReleased && ((UnityEngine.Object) BraveInput.SecondaryPlayerInstance == (UnityEngine.Object) null || !BraveInput.SecondaryPlayerInstance.ActiveActions.MapAction.IsPressed))
                    {
                        Minimap.Instance.ToggleMinimap(false);
                        return;
                    }
                    if ((UnityEngine.Object) BraveInput.SecondaryPlayerInstance != (UnityEngine.Object) null && BraveInput.SecondaryPlayerInstance.ActiveActions.MapAction.WasReleased && !BraveInput.PrimaryPlayerInstance.ActiveActions.MapAction.IsPressed)
                    {
                        Minimap.Instance.ToggleMinimap(false);
                        return;
                    }
                    if (!BraveInput.PrimaryPlayerInstance.ActiveActions.MapAction.IsPressed && (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER || !BraveInput.GetInstanceForPlayer(GameManager.Instance.SecondaryPlayer.PlayerIDX).ActiveActions.MapAction.IsPressed))
                    {
                        Minimap.Instance.ToggleMinimap(false);
                        return;
                    }
                }
                this.UpdateDockItemSpriteScales();
                this.HandlePanelVisibility();
                bool flag1 = false;
                for (int targetPlayerID = 0; targetPlayerID < GameManager.Instance.AllPlayers.Length; ++targetPlayerID)
                {
                    List<Tuple<tk2dSprite, PassiveItem>> tupleList = targetPlayerID != 0 ? this.secondaryDockItems : this.dockItems;
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(GameManager.Instance.AllPlayers[targetPlayerID].PlayerIDX);
                    if (instanceForPlayer.ActiveActions.MapAction.IsPressed || Minimap.Instance.HeldOpen)
                    {
                        if (instanceForPlayer.IsKeyboardAndMouse())
                        {
                            Vector2 mousePosition = (Vector2) Input.mousePosition;
                            Vector2 vector2 = this.ModifyMousePosition(mousePosition);
                            this.ControllerCrosshair.IsVisible = false;
                            this.SelectNearbyTeleportIcon(vector2);
                            if (Input.GetMouseButtonDown(0))
                            {
                                float units = this.DockPanel.PixelsToUnits();
                                Vector2 b = this.minimapCamera.ScreenToWorldPoint((Vector3) mousePosition).XY();
                                bool flag2 = false;
                                for (int index = 0; index < tupleList.Count; ++index)
                                {
                                    if ((double) Vector2.Distance(tupleList[index].First.WorldCenter, b) < 20.0 * (double) Pixelator.Instance.CurrentTileScale / 2.0 * (double) units)
                                    {
                                        flag2 = true;
                                        this.SelectDockItem(index, targetPlayerID);
                                        break;
                                    }
                                }
                                if (!flag2)
                                {
                                    this.m_isPanning = true;
                                    this.m_lastMousePosition = (Vector3) vector2;
                                    this.m_panPixelDistTravelled = 0.0f;
                                }
                            }
                            else if (Input.GetMouseButton(0) && this.m_isPanning)
                            {
                                Minimap.Instance.AttemptPanCamera((this.minimapCamera.ScreenToWorldPoint((Vector3) vector2) - this.minimapCamera.ScreenToWorldPoint(this.m_lastMousePosition)) * -1f);
                                this.m_panPixelDistTravelled += Vector2.Distance((Vector2) this.m_lastMousePosition, vector2);
                                this.m_lastMousePosition = (Vector3) vector2;
                            }
                            else if (Input.GetMouseButtonUp(0))
                            {
                                Vector2 viewportPosition = (Vector2) BraveUtility.GetMinimapViewportPosition(vector2);
                                bool flag3 = (double) viewportPosition.x > 0.17000000178813934 && (double) viewportPosition.y < 0.87999999523162842 && (double) viewportPosition.x < 0.824999988079071 && (double) viewportPosition.y > 0.11999999731779099;
                                bool flag4 = (double) this.m_panPixelDistTravelled / (double) Screen.width < 0.004999999888241291;
                                if (flag3 && flag4)
                                    this.AttemptTeleport();
                                this.m_isPanning = false;
                            }
                            if ((double) Input.GetAxis("Mouse ScrollWheel") != 0.0)
                                Minimap.Instance.AttemptZoomCamera(Input.GetAxis("Mouse ScrollWheel") * -1f);
                        }
                        else
                        {
                            Vector2 vector = Vector2.zero;
                            bool flag5 = false;
                            bool flag6 = false;
                            bool flag7 = false;
                            bool flag8 = false;
                            bool flag9 = false;
                            bool flag10 = false;
                            bool flag11 = false;
                            if (instanceForPlayer.ActiveActions != null)
                            {
                                vector = instanceForPlayer.ActiveActions.Aim.Vector;
                                flag6 = (bool) (OneAxisInputControl) instanceForPlayer.ActiveActions.InteractAction;
                                InputDevice device = instanceForPlayer.ActiveActions.Device;
                                if (device != null)
                                {
                                    flag5 = device.RightStickButton.WasPressed;
                                    flag7 = device.Action4.WasPressed;
                                    flag8 = device.DPadLeft.WasPressed;
                                    flag9 = device.DPadRight.WasPressed;
                                    flag10 = device.DPadUp.WasPressed;
                                    flag11 = device.DPadDown.WasPressed;
                                }
                            }
                            this.ControllerCrosshair.IsVisible = true;
                            if ((double) vector.magnitude > 0.0 && !flag1)
                            {
                                flag1 = true;
                                Minimap.Instance.AttemptPanCamera(vector.ToVector3ZUp() * GameManager.INVARIANT_DELTA_TIME * 0.8f);
                            }
                            this.SelectNearbyTeleportIcon(new Vector2((float) Screen.width / 2f, (float) Screen.height / 2f));
                            if (flag5)
                                Minimap.Instance.TogglePresetZoomValue();
                            if (flag6)
                                this.AttemptTeleport();
                            if (flag7)
                                this.DropSelectedItem();
                            int count = tupleList.Count;
                            if (flag9 && tupleList.Count > 0)
                            {
                                int i = Mathf.Max(0, (this.m_selectedDockItemIndex + 1) % count);
                                if (targetPlayerID != this.m_targetDockIndex)
                                    i = 0;
                                this.SelectDockItem(i, targetPlayerID);
                            }
                            if (flag8 && tupleList.Count > 0)
                            {
                                int i = Mathf.Max(0, (this.m_selectedDockItemIndex + count - 1) % count);
                                if (targetPlayerID != this.m_targetDockIndex)
                                    i = 0;
                                this.SelectDockItem(i, targetPlayerID);
                            }
                            if (flag10 || flag11)
                            {
                                Minimap instance = Minimap.Instance;
                                Vector2 screenPosition = new Vector2((float) Screen.width / 2f, (float) Screen.height / 2f);
                                if (instanceForPlayer.IsKeyboardAndMouse())
                                    screenPosition = this.ModifyMousePosition((Vector2) Input.mousePosition);
                                float dist;
                                RoomHandler nearestVisibleRoom = instance.GetNearestVisibleRoom(screenPosition, out dist);
                                if (nearestVisibleRoom != null && !instance.IsPanning)
                                    this.m_currentTeleportTargetIndex = instance.roomsContainingTeleporters.IndexOf(nearestVisibleRoom);
                                RoomHandler key;
                                if ((double) dist < 0.5 || instance.IsPanning)
                                {
                                    int dir = !flag10 ? -1 : 1;
                                    key = instance.NextSelectedTeleporter(ref this.m_currentTeleportTargetIndex, dir);
                                }
                                else
                                    key = nearestVisibleRoom;
                                if (key != null && key.TeleportersActive)
                                    instance.PanToPosition((Vector3) instance.RoomToTeleportMap[key].GetComponent<tk2dBaseSprite>().WorldCenter);
                            }
                        }
                    }
                }
            }
            else
                this.m_isPanning = false;
        }

        private void SelectNearbyTeleportIcon(Vector2 positionToCheck)
        {
            GameObject icon = (GameObject) null;
            RoomHandler roomHandler = Minimap.Instance.CheckIconsNearCursor((Vector3) positionToCheck, out icon);
            if (roomHandler != null && !roomHandler.TeleportersActive)
            {
                if (!((UnityEngine.Object) this.m_currentTeleportIconSprite != (UnityEngine.Object) null))
                    return;
                SpriteOutlineManager.RemoveOutlineFromSprite(this.m_currentTeleportIconSprite);
                this.m_currentTeleportIconSprite = (tk2dBaseSprite) null;
                this.m_currentTeleportTarget = (RoomHandler) null;
            }
            else
            {
                tk2dBaseSprite component = !((UnityEngine.Object) icon != (UnityEngine.Object) null) ? (tk2dBaseSprite) null : icon.GetComponent<tk2dBaseSprite>();
                if ((UnityEngine.Object) component == (UnityEngine.Object) null)
                {
                    if ((UnityEngine.Object) this.m_currentTeleportIconSprite != (UnityEngine.Object) null)
                    {
                        SpriteOutlineManager.RemoveOutlineFromSprite(this.m_currentTeleportIconSprite);
                        this.m_currentTeleportIconSprite = (tk2dBaseSprite) null;
                        this.m_currentTeleportTarget = (RoomHandler) null;
                    }
                }
                else
                {
                    component.ignoresTiltworldDepth = true;
                    if ((UnityEngine.Object) this.m_currentTeleportIconSprite != (UnityEngine.Object) null && (UnityEngine.Object) this.m_currentTeleportIconSprite != (UnityEngine.Object) component)
                    {
                        SpriteOutlineManager.RemoveOutlineFromSprite(this.m_currentTeleportIconSprite);
                        this.m_currentTeleportIconSprite = (tk2dBaseSprite) null;
                        this.m_currentTeleportTarget = (RoomHandler) null;
                        SpriteOutlineManager.AddOutlineToSprite(component, Color.white);
                        this.m_currentTeleportTarget = roomHandler;
                        this.m_currentTeleportIconSprite = component;
                    }
                    else if (!((UnityEngine.Object) this.m_currentTeleportIconSprite == (UnityEngine.Object) component))
                    {
                        SpriteOutlineManager.AddOutlineToSprite(component, Color.white);
                        this.m_currentTeleportTarget = roomHandler;
                        this.m_currentTeleportIconSprite = component;
                    }
                }
                if (this.m_currentTeleportTarget != null)
                {
                    this.ControllerCrosshair.SpriteName = "minimap_select_square_001";
                    this.ControllerCrosshair.Size = this.ControllerCrosshair.SpriteInfo.sizeInPixels * 3f;
                    this.ControllerCrosshair.GetComponentInChildren<dfPanel>().IsVisible = true;
                }
                else
                {
                    this.ControllerCrosshair.SpriteName = "minimap_select_crosshair_001";
                    this.ControllerCrosshair.Size = this.ControllerCrosshair.SpriteInfo.sizeInPixels * 3f;
                    this.ControllerCrosshair.GetComponentInChildren<dfPanel>().IsVisible = false;
                }
            }
        }

        private bool AttemptTeleport()
        {
            if ((bool) (UnityEngine.Object) Minimap.Instance && Minimap.Instance.PreventAllTeleports || GameUIRoot.Instance.DisplayingConversationBar)
                return false;
            PlayerController[] allPlayers = GameManager.Instance.AllPlayers;
            for (int index = 0; index < allPlayers.Length; ++index)
            {
                PlayerController playerController = allPlayers[index];
                if (playerController.CurrentRoom != null && playerController.CurrentRoom.CompletelyPreventLeaving)
                    return false;
            }
            if (this.m_currentTeleportTarget == null)
                return false;
            RoomHandler currentTeleportTarget = this.m_currentTeleportTarget;
            for (int index = 0; index < allPlayers.Length; ++index)
                allPlayers[index].AttemptTeleportToRoom(currentTeleportTarget);
            return true;
        }

        private void DeselectDockItem()
        {
            this.ClearSynergyHighlights();
            if (this.m_selectedDockItemIndex == -1)
                return;
            if (this.m_selectedDockItemIndex < this.SelectedDockItems.Count)
            {
                SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.SelectedDockItems[this.m_selectedDockItemIndex].First);
                this.m_selectedDockItemIndex = -1;
            }
            else
                this.m_selectedDockItemIndex = -1;
        }

        private void UpdateDockItemSpriteScales()
        {
            for (int index = 0; index < this.dockItems.Count; ++index)
            {
                tk2dSprite first = this.dockItems[index].First;
                first.scale = Vector3.one * GameUIUtility.GetCurrentTK2D_DFScale(this.m_manager);
                if (SpriteOutlineManager.HasOutline((tk2dBaseSprite) first))
                {
                    foreach (tk2dBaseSprite outlineSprite in SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) first))
                        outlineSprite.scale = first.scale;
                }
            }
            for (int index = 0; index < this.secondaryDockItems.Count; ++index)
            {
                tk2dSprite first = this.secondaryDockItems[index].First;
                first.scale = Vector3.one * GameUIUtility.GetCurrentTK2D_DFScale(this.m_manager);
                if (SpriteOutlineManager.HasOutline((tk2dBaseSprite) first))
                {
                    foreach (tk2dBaseSprite outlineSprite in SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) first))
                        outlineSprite.scale = first.scale;
                }
            }
        }

        private void SelectDockItem(int i, int targetPlayerID)
        {
            if (this.m_selectedDockItemIndex == i && this.m_targetDockIndex == targetPlayerID)
                return;
            this.DeselectDockItem();
            List<Tuple<tk2dSprite, PassiveItem>> tupleList = this.dockItems;
            if (targetPlayerID == 1)
                tupleList = this.secondaryDockItems;
            if (i < tupleList.Count)
            {
                SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) tupleList[i].First, Color.white);
                foreach (tk2dBaseSprite outlineSprite in SpriteOutlineManager.GetOutlineSprites((tk2dBaseSprite) tupleList[i].First))
                    outlineSprite.scale = tupleList[i].First.scale;
            }
            this.m_targetDockIndex = targetPlayerID;
            this.m_selectedDockItemIndex = i;
            PassiveItem second = tupleList[i].Second;
            this.DropItemButton.IsEnabled = second.CanActuallyBeDropped(second.Owner);
            this.DropItemSprite.Color = (Color32) (!this.DropItemButton.IsEnabled ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white);
            this.DropItemLabel.Color = (Color32) (!this.DropItemButton.IsEnabled ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white);
            this.DropItemSpriteForeign.Color = (Color32) (!this.DropItemButton.IsEnabled ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white);
            this.DropItemLabelForeign.Color = (Color32) (!this.DropItemButton.IsEnabled ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white);
            if (!(bool) (UnityEngine.Object) second)
                return;
            this.UpdateSynergyHighlights(second.PickupObjectId);
        }

        private void ClearSynergyHighlights()
        {
            for (int index = 0; index < this.extantSynergyArrows.Count; ++index)
                UnityEngine.Object.Destroy((UnityEngine.Object) this.extantSynergyArrows[index].gameObject);
            this.extantSynergyArrows.Clear();
            for (int index = 0; index < this.dockItems.Count; ++index)
                SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.dockItems[index].First);
            for (int index = 0; index < this.secondaryDockItems.Count; ++index)
                SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) this.secondaryDockItems[index].First);
        }

        private void CreateArrow(tk2dBaseSprite targetSprite, dfControl targetParent)
        {
            dfSprite dfSprite = targetParent.AddControl<dfSprite>();
            dfSprite.Atlas = Minimap.Instance.UIMinimap.DispenserIcon.Atlas;
            dfSprite.SpriteName = "synergy_ammonomicon_arrow_001";
            dfSprite.Size = dfSprite.SpriteInfo.sizeInPixels * 4f;
            Bounds bounds = targetSprite.GetBounds();
            targetSprite.GetUntrimmedBounds();
            Vector3 size = bounds.size;
            dfSprite.transform.position = (targetSprite.WorldCenter.ToVector3ZisY() + new Vector3(-8f * targetParent.PixelsToUnits(), (float) ((double) size.y / 2.0 + 32.0 * (double) targetParent.PixelsToUnits()), 0.0f)).WithZ(0.0f);
            dfSprite.BringToFront();
            dfSprite.Invalidate();
            this.extantSynergyArrows.Add(dfSprite);
        }

        private void UpdateSynergyHighlights(int selectedID)
        {
            AdvancedSynergyDatabase synergyManager = GameManager.Instance.SynergyManager;
            dfControl rootContainer = this.DockSprite.GetRootContainer();
            rootContainer.BringToFront();
            for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
                for (int index2 = 0; index2 < synergyManager.synergies.Length; ++index2)
                {
                    if (allPlayer.ActiveExtraSynergies.Contains(index2))
                    {
                        AdvancedSynergyEntry synergy = synergyManager.synergies[index2];
                        if (synergy.ContainsPickup(selectedID))
                        {
                            for (int index3 = 0; index3 < this.dockItems.Count; ++index3)
                            {
                                int pickupObjectId = this.dockItems[index3].Second.PickupObjectId;
                                if (pickupObjectId != selectedID && synergy.ContainsPickup(pickupObjectId))
                                {
                                    SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.dockItems[index3].First, SynergyDatabase.SynergyBlue);
                                    this.CreateArrow((tk2dBaseSprite) this.dockItems[index3].First, rootContainer);
                                }
                            }
                            for (int index4 = 0; index4 < this.secondaryDockItems.Count; ++index4)
                            {
                                int pickupObjectId = this.secondaryDockItems[index4].Second.PickupObjectId;
                                if (pickupObjectId != selectedID && synergy.ContainsPickup(pickupObjectId))
                                {
                                    SpriteOutlineManager.AddOutlineToSprite((tk2dBaseSprite) this.secondaryDockItems[index4].First, SynergyDatabase.SynergyBlue);
                                    this.CreateArrow((tk2dBaseSprite) this.secondaryDockItems[index4].First, rootContainer);
                                }
                            }
                            for (int index5 = 0; index5 < allPlayer.inventory.AllGuns.Count; ++index5)
                            {
                                int pickupObjectId = allPlayer.inventory.AllGuns[index5].PickupObjectId;
                                if (pickupObjectId != selectedID && synergy.ContainsPickup(pickupObjectId))
                                {
                                    int num = allPlayer.inventory.AllGuns.IndexOf(allPlayer.CurrentGun);
                                    int gunIndex = allPlayer.inventory.AllGuns.Count - (num - index5 + allPlayer.inventory.AllGuns.Count - 1) % allPlayer.inventory.AllGuns.Count - 1;
                                    tk2dClippedSprite spriteForUnfoldedGun = GameUIRoot.Instance.GetSpriteForUnfoldedGun(allPlayer.PlayerIDX, gunIndex);
                                    if ((bool) (UnityEngine.Object) spriteForUnfoldedGun)
                                    {
                                        SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) spriteForUnfoldedGun);
                                        SpriteOutlineManager.AddOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) spriteForUnfoldedGun, SynergyDatabase.SynergyBlue);
                                        this.CreateArrow((tk2dBaseSprite) spriteForUnfoldedGun, spriteForUnfoldedGun.transform.parent.parent.GetComponent<dfControl>());
                                    }
                                }
                            }
                            for (int index6 = 0; index6 < allPlayer.activeItems.Count; ++index6)
                            {
                                int pickupObjectId = allPlayer.activeItems[index6].PickupObjectId;
                                if (pickupObjectId != selectedID && synergy.ContainsPickup(pickupObjectId))
                                {
                                    int num = allPlayer.activeItems.IndexOf(allPlayer.CurrentItem);
                                    int itemIndex = allPlayer.activeItems.Count - (num - index6 + allPlayer.activeItems.Count - 1) % allPlayer.activeItems.Count - 1;
                                    tk2dClippedSprite spriteForUnfoldedItem = GameUIRoot.Instance.GetSpriteForUnfoldedItem(allPlayer.PlayerIDX, itemIndex);
                                    if ((bool) (UnityEngine.Object) spriteForUnfoldedItem)
                                    {
                                        SpriteOutlineManager.RemoveOutlineFromSprite((tk2dBaseSprite) spriteForUnfoldedItem);
                                        SpriteOutlineManager.AddOutlineToSprite<tk2dClippedSprite>((tk2dBaseSprite) spriteForUnfoldedItem, SynergyDatabase.SynergyBlue);
                                        this.CreateArrow((tk2dBaseSprite) spriteForUnfoldedItem, spriteForUnfoldedItem.transform.parent.parent.GetComponent<dfControl>());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator MoveAllThings()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MinimapUIController__MoveAllThingsc__Iterator1()
            {
                _this = this
            };
        }

        private void UpdateLevelNameLabel()
        {
            string localizedValue = this.LevelNameLabel.ForceGetLocalizedValue(GameManager.Instance.Dungeon.DungeonFloorName);
            GameLevelDefinition loadedLevelDefinition = GameManager.Instance.GetLastLoadedLevelDefinition();
            int num = -1;
            if (loadedLevelDefinition != null)
                num = GameManager.Instance.dungeonFloors.IndexOf(loadedLevelDefinition);
            this.LevelNameLabel.Text = $"{this.LevelNameLabel.ForceGetLocalizedValue("#LEVEL")}{(num >= 0 ? num.ToString() : "?")}: {localizedValue}";
            this.LevelNameLabel.Invalidate();
            this.LevelNameLabel.PerformLayout();
        }

        private void UpdateQuestText()
        {
        }

        private void Activate()
        {
            this.UpdateLevelNameLabel();
            this.UpdateQuestText();
            this.DeselectDockItem();
            this.m_active = true;
            this.minimapCamera.enabled = true;
            for (int index = 0; index < this.AdditionalControlsToToggle.Count; ++index)
            {
                this.AdditionalControlsToToggle[index].IsVisible = true;
                dfSpriteAnimation componentInChildren = this.AdditionalControlsToToggle[index].GetComponentInChildren<dfSpriteAnimation>();
                if ((bool) (UnityEngine.Object) componentInChildren)
                    componentInChildren.Play();
            }
            this.RecalculatePositions();
            this.StartCoroutine(this.MoveAllThings());
        }

        private void Deactivate()
        {
            this.DeselectDockItem();
            this.m_active = false;
            this.RecalculatePositions();
            this.ControllerCrosshair.IsVisible = false;
            this.StartCoroutine(this.MoveAllThings());
            if ((UnityEngine.Object) this.m_currentTeleportIconSprite != (UnityEngine.Object) null)
            {
                SpriteOutlineManager.RemoveOutlineFromSprite(this.m_currentTeleportIconSprite);
                this.m_currentTeleportIconSprite = (tk2dBaseSprite) null;
                this.m_currentTeleportTarget = (RoomHandler) null;
            }
            for (int index = 0; index < this.AdditionalControlsToToggle.Count; ++index)
                this.AdditionalControlsToToggle[index].IsVisible = false;
        }
    }

