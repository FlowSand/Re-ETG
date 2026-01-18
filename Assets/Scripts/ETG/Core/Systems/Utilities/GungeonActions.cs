using InControl;
using UnityEngine;

#nullable disable

public class GungeonActions : PlayerActionSet
    {
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerTwoAxisAction Move;
        public PlayerAction AimLeft;
        public PlayerAction AimRight;
        public PlayerAction AimUp;
        public PlayerAction AimDown;
        public PlayerTwoAxisAction Aim;
        public PlayerAction SelectLeft;
        public PlayerAction SelectRight;
        public PlayerAction SelectUp;
        public PlayerAction SelectDown;
        public PlayerTwoAxisAction SelectAxis;
        public PlayerAction ShootAction;
        public PlayerAction DodgeRollAction;
        public PlayerAction InteractAction;
        public PlayerAction ReloadAction;
        public PlayerAction UseItemAction;
        public PlayerAction MapAction;
        public PlayerAction GunUpAction;
        public PlayerAction GunDownAction;
        public PlayerAction ItemUpAction;
        public PlayerAction ItemDownAction;
        public PlayerAction KeybulletAction;
        public PlayerAction PauseAction;
        public PlayerAction CancelAction;
        public PlayerAction MenuSelectAction;
        public PlayerAction EquipmentMenuAction;
        public PlayerAction BlankAction;
        public PlayerAction DropGunAction;
        public PlayerAction DropItemAction;
        public PlayerAction GunQuickEquipAction;
        public PlayerAction MinimapZoomInAction;
        public PlayerAction MinimapZoomOutAction;
        public PlayerAction SwapDualGunsAction;
        public PlayerAction PunchoutDodgeLeft;
        public PlayerAction PunchoutDodgeRight;
        public PlayerAction PunchoutBlock;
        public PlayerAction PunchoutDuck;
        public PlayerAction PunchoutPunchLeft;
        public PlayerAction PunchoutPunchRight;
        public PlayerAction PunchoutSuper;
        private bool m_highAccuraceAimMode;

        public GungeonActions()
        {
            this.Left = this.CreatePlayerAction("Move Left");
            this.Right = this.CreatePlayerAction("Move Right");
            this.Up = this.CreatePlayerAction("Move Up");
            this.Down = this.CreatePlayerAction("Move Down");
            this.Move = this.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
            this.AimLeft = this.CreatePlayerAction("Aim Left");
            this.AimRight = this.CreatePlayerAction("Aim Right");
            this.AimUp = this.CreatePlayerAction("Aim Up");
            this.AimDown = this.CreatePlayerAction("Aim Down");
            this.Aim = this.CreateTwoAxisPlayerAction(this.AimLeft, this.AimRight, this.AimDown, this.AimUp);
            this.SelectLeft = this.CreatePlayerAction("Select Left");
            this.SelectRight = this.CreatePlayerAction("Select Right");
            this.SelectUp = this.CreatePlayerAction("Select Up");
            this.SelectDown = this.CreatePlayerAction("Select Down");
            this.SelectUp.StateThreshold = 0.5f;
            this.SelectDown.StateThreshold = 0.5f;
            this.SelectLeft.StateThreshold = 0.5f;
            this.SelectRight.StateThreshold = 0.5f;
            this.SelectAxis = this.CreateTwoAxisPlayerAction(this.SelectLeft, this.SelectRight, this.SelectDown, this.SelectUp);
            this.SelectAxis.StateThreshold = 0.9f;
            this.ShootAction = this.CreatePlayerAction("Shoot");
            this.DodgeRollAction = this.CreatePlayerAction("Dodge Roll");
            this.InteractAction = this.CreatePlayerAction("Interact");
            this.CancelAction = this.CreatePlayerAction("Cancel");
            this.ReloadAction = this.CreatePlayerAction("Reload");
            this.UseItemAction = this.CreatePlayerAction("Use Item");
            this.MapAction = this.CreatePlayerAction("Map");
            this.GunUpAction = this.CreatePlayerAction("Cycle Gun Up");
            this.GunDownAction = this.CreatePlayerAction("Cycle Gun Down");
            this.ItemUpAction = this.CreatePlayerAction("Cycle Item Up");
            this.ItemDownAction = this.CreatePlayerAction("Cycle Item Down");
            this.KeybulletAction = this.CreatePlayerAction("Keybullet");
            this.PauseAction = this.CreatePlayerAction("Pause");
            this.DropGunAction = this.CreatePlayerAction("Drop Gun");
            this.DropItemAction = this.CreatePlayerAction("Drop Item");
            this.EquipmentMenuAction = this.CreatePlayerAction("Equipment Menu");
            this.BlankAction = this.CreatePlayerAction("Blank");
            this.GunQuickEquipAction = this.CreatePlayerAction("Gun Quick Equip");
            this.SwapDualGunsAction = this.CreatePlayerAction("Swap Dual Guns");
            this.MenuSelectAction = this.CreatePlayerAction("Menu Select");
            this.MinimapZoomInAction = this.CreatePlayerAction("Minimap Zoom In");
            this.MinimapZoomOutAction = this.CreatePlayerAction("Minimap Zoom Out");
            this.PunchoutDodgeLeft = this.CreatePlayerAction("Dodge Left");
            this.PunchoutDodgeRight = this.CreatePlayerAction("Dodge Right");
            this.PunchoutBlock = this.CreatePlayerAction("Block");
            this.PunchoutDuck = this.CreatePlayerAction("Duck");
            this.PunchoutPunchLeft = this.CreatePlayerAction("Punch Left");
            this.PunchoutPunchRight = this.CreatePlayerAction("Punch Right");
            this.PunchoutSuper = this.CreatePlayerAction("Super");
            this.PunchoutDodgeLeft.StateThreshold = 0.3f;
            this.PunchoutDodgeRight.StateThreshold = 0.3f;
            this.PunchoutBlock.StateThreshold = 0.3f;
            this.PunchoutDuck.StateThreshold = 0.3f;
        }

        public PlayerAction GetActionFromType(GungeonActions.GungeonActionType type)
        {
            switch (type)
            {
                case GungeonActions.GungeonActionType.Left:
                    return this.Left;
                case GungeonActions.GungeonActionType.Right:
                    return this.Right;
                case GungeonActions.GungeonActionType.Up:
                    return this.Up;
                case GungeonActions.GungeonActionType.Down:
                    return this.Down;
                case GungeonActions.GungeonActionType.AimLeft:
                    return this.AimLeft;
                case GungeonActions.GungeonActionType.AimRight:
                    return this.AimRight;
                case GungeonActions.GungeonActionType.AimUp:
                    return this.AimUp;
                case GungeonActions.GungeonActionType.AimDown:
                    return this.AimDown;
                case GungeonActions.GungeonActionType.Shoot:
                    return this.ShootAction;
                case GungeonActions.GungeonActionType.DodgeRoll:
                    return this.DodgeRollAction;
                case GungeonActions.GungeonActionType.Interact:
                    return this.InteractAction;
                case GungeonActions.GungeonActionType.Reload:
                    return this.ReloadAction;
                case GungeonActions.GungeonActionType.UseItem:
                    return this.UseItemAction;
                case GungeonActions.GungeonActionType.Map:
                    return this.MapAction;
                case GungeonActions.GungeonActionType.CycleGunUp:
                    return this.GunUpAction;
                case GungeonActions.GungeonActionType.CycleGunDown:
                    return this.GunDownAction;
                case GungeonActions.GungeonActionType.CycleItemUp:
                    return this.ItemUpAction;
                case GungeonActions.GungeonActionType.CycleItemDown:
                    return this.ItemDownAction;
                case GungeonActions.GungeonActionType.Keybullet:
                    return this.KeybulletAction;
                case GungeonActions.GungeonActionType.Pause:
                    return this.PauseAction;
                case GungeonActions.GungeonActionType.SelectLeft:
                    return this.SelectLeft;
                case GungeonActions.GungeonActionType.SelectRight:
                    return this.SelectRight;
                case GungeonActions.GungeonActionType.SelectUp:
                    return this.SelectUp;
                case GungeonActions.GungeonActionType.SelectDown:
                    return this.SelectDown;
                case GungeonActions.GungeonActionType.Cancel:
                    return this.CancelAction;
                case GungeonActions.GungeonActionType.DropGun:
                    return this.DropGunAction;
                case GungeonActions.GungeonActionType.EquipmentMenu:
                    return this.EquipmentMenuAction;
                case GungeonActions.GungeonActionType.Blank:
                    return this.BlankAction;
                case GungeonActions.GungeonActionType.GunQuickEquip:
                    return this.GunQuickEquipAction;
                case GungeonActions.GungeonActionType.MenuInteract:
                    return this.MenuSelectAction;
                case GungeonActions.GungeonActionType.DropItem:
                    return this.DropItemAction;
                case GungeonActions.GungeonActionType.MinimapZoomIn:
                    return this.MinimapZoomInAction;
                case GungeonActions.GungeonActionType.MinimapZoomOut:
                    return this.MinimapZoomOutAction;
                case GungeonActions.GungeonActionType.SwapDualGuns:
                    return this.SwapDualGunsAction;
                case GungeonActions.GungeonActionType.PunchoutDodgeLeft:
                    return this.PunchoutDodgeLeft;
                case GungeonActions.GungeonActionType.PunchoutDodgeRight:
                    return this.PunchoutDodgeRight;
                case GungeonActions.GungeonActionType.PunchoutBlock:
                    return this.PunchoutBlock;
                case GungeonActions.GungeonActionType.PunchoutDuck:
                    return this.PunchoutDuck;
                case GungeonActions.GungeonActionType.PunchoutPunchLeft:
                    return this.PunchoutPunchLeft;
                case GungeonActions.GungeonActionType.PunchoutPunchRight:
                    return this.PunchoutPunchRight;
                case GungeonActions.GungeonActionType.PunchoutSuper:
                    return this.PunchoutSuper;
                default:
                    return (PlayerAction) null;
            }
        }

        public bool IntroSkipActionPressed()
        {
            return this.MenuSelectAction.WasPressed || this.PauseAction.WasPressed || this.CancelAction.WasPressed;
        }

        public bool AnyActionPressed()
        {
            for (int index = 0; index < this.Actions.Count; ++index)
            {
                if (this.Actions[index].WasPressed)
                    return true;
            }
            return false;
        }

        public void IgnoreBindingsOfType(BindingSourceType sourceType)
        {
            for (int index = 0; index < this.Actions.Count; ++index)
                this.Actions[index].IgnoreBindingsOfType(sourceType);
        }

        public void ClearBindingsOfType(BindingSourceType sourceType)
        {
            for (int index = 0; index < this.Actions.Count; ++index)
                this.Actions[index].ClearBindingsOfType(sourceType);
        }

        public bool CheckBothSticksButton()
        {
            return this.Device != null && (this.Device.LeftStickButton.WasPressed && this.Device.RightStickButton.IsPressed || this.Device.LeftStickButton.IsPressed && this.Device.RightStickButton.WasPressed || this.Device.LeftStickButton.WasPressed && this.Device.RightStickButton.WasPressed);
        }

        public bool CheckAllFaceButtonsPressed()
        {
            int num = 0;
            if (this.Device.Action1.IsPressed)
                ++num;
            if (this.Device.Action2.IsPressed)
                ++num;
            if (this.Device.Action3.IsPressed)
                ++num;
            if (this.Device.Action4.IsPressed)
                ++num;
            return num >= 3;
        }

        public void PostProcessAdditionalBlankControls(int playerNum)
        {
            switch (playerNum != 0 ? GameManager.Options.additionalBlankControlTwo : GameManager.Options.additionalBlankControl)
            {
                case GameOptions.ControllerBlankControl.CIRCLE:
                    this.DodgeRollAction.RemoveBindingOfType(InputControlType.Action2);
                    if (this.BlankAction.Bindings.Count < 2)
                        this.BlankAction.AddBinding((BindingSource) new DeviceBindingSource(InputControlType.Action2));
                    if (playerNum == 0)
                    {
                        GameManager.Options.additionalBlankControl = GameOptions.ControllerBlankControl.NONE;
                        GameManager.Options.CurrentControlPreset = GameOptions.ControlPreset.CUSTOM;
                        break;
                    }
                    if (playerNum != 1)
                        break;
                    GameManager.Options.additionalBlankControlTwo = GameOptions.ControllerBlankControl.NONE;
                    GameManager.Options.CurrentControlPresetP2 = GameOptions.ControlPreset.CUSTOM;
                    break;
                case GameOptions.ControllerBlankControl.L1:
                    this.DodgeRollAction.RemoveBindingOfType(InputControlType.LeftBumper);
                    if (this.BlankAction.Bindings.Count < 2)
                        this.BlankAction.AddBinding((BindingSource) new DeviceBindingSource(InputControlType.LeftBumper));
                    if (playerNum == 0)
                    {
                        GameManager.Options.additionalBlankControl = GameOptions.ControllerBlankControl.NONE;
                        GameManager.Options.CurrentControlPreset = GameOptions.ControlPreset.CUSTOM;
                        break;
                    }
                    if (playerNum != 1)
                        break;
                    GameManager.Options.additionalBlankControlTwo = GameOptions.ControllerBlankControl.NONE;
                    GameManager.Options.CurrentControlPresetP2 = GameOptions.ControlPreset.CUSTOM;
                    break;
            }
        }

        public void ReinitializeDefaults()
        {
            for (int index = 0; index < this.Actions.Count; ++index)
                this.Actions[index].ResetBindings();
        }

        public void InitializeSwappedTriggersPreset()
        {
            for (int index = 0; index < this.Actions.Count; ++index)
                this.Actions[index].ResetBindings();
            this.ShootAction.ClearBindings();
            this.ShootAction.AddBinding((BindingSource) new DeviceBindingSource(InputControlType.RightTrigger));
            this.DodgeRollAction.ClearBindings();
            this.DodgeRollAction.AddBinding((BindingSource) new DeviceBindingSource(InputControlType.LeftTrigger));
            this.DodgeRollAction.AddBinding((BindingSource) new DeviceBindingSource(InputControlType.Action2));
            this.MapAction.ClearBindings();
            this.MapAction.AddBinding((BindingSource) new DeviceBindingSource(InputControlType.LeftBumper));
            this.MapAction.AddBinding((BindingSource) new KeyBindingSource(new Key[1]
            {
                Key.Tab
            }));
            this.UseItemAction.ClearBindings();
            this.UseItemAction.AddBinding((BindingSource) new DeviceBindingSource(InputControlType.RightBumper));
            this.UseItemAction.AddBinding((BindingSource) new KeyBindingSource(new Key[1]
            {
                Key.Space
            }));
            this.ShootAction.AddBinding((BindingSource) new MouseBindingSource(Mouse.LeftButton));
            this.DodgeRollAction.AddBinding((BindingSource) new MouseBindingSource(Mouse.RightButton));
        }

        public static InputControlType LocalizedMenuSelectAction
        {
            get
            {
                return GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE ? InputControlType.Action2 : InputControlType.Action1;
            }
        }

        public static InputControlType LocalizedMenuCancelAction
        {
            get
            {
                return GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.JAPANESE ? InputControlType.Action1 : InputControlType.Action2;
            }
        }

        public void ReinitializeMenuDefaults()
        {
            this.CancelAction.ResetBindings();
            this.MenuSelectAction.ResetBindings();
            this.CancelAction.ClearBindings();
            this.MenuSelectAction.ClearBindings();
            this.CancelAction.AddDefaultBinding(GungeonActions.LocalizedMenuCancelAction);
            this.CancelAction.AddDefaultBinding(Key.Escape);
            this.MenuSelectAction.AddDefaultBinding(GungeonActions.LocalizedMenuSelectAction);
            this.MenuSelectAction.AddDefaultBinding(Key.Return);
        }

        public void InitializeDefaults()
        {
            this.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            this.Left.AddDefaultBinding(Key.A);
            this.Right.AddDefaultBinding(InputControlType.LeftStickRight);
            this.Right.AddDefaultBinding(Key.D);
            this.Up.AddDefaultBinding(InputControlType.LeftStickUp);
            this.Up.AddDefaultBinding(Key.W);
            this.Down.AddDefaultBinding(InputControlType.LeftStickDown);
            this.Down.AddDefaultBinding(Key.S);
            this.AimLeft.AddDefaultBinding(InputControlType.RightStickLeft);
            this.AimRight.AddDefaultBinding(InputControlType.RightStickRight);
            this.AimUp.AddDefaultBinding(InputControlType.RightStickUp);
            this.AimDown.AddDefaultBinding(InputControlType.RightStickDown);
            this.SelectLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
            this.SelectLeft.AddDefaultBinding(InputControlType.DPadLeft);
            this.SelectLeft.AddDefaultBinding(Key.A);
            this.SelectLeft.AddDefaultBinding(Key.LeftArrow);
            this.SelectRight.AddDefaultBinding(InputControlType.LeftStickRight);
            this.SelectRight.AddDefaultBinding(InputControlType.DPadRight);
            this.SelectRight.AddDefaultBinding(Key.D);
            this.SelectRight.AddDefaultBinding(Key.RightArrow);
            this.SelectUp.AddDefaultBinding(InputControlType.LeftStickUp);
            this.SelectUp.AddDefaultBinding(InputControlType.DPadUp);
            this.SelectUp.AddDefaultBinding(Key.W);
            this.SelectUp.AddDefaultBinding(Key.UpArrow);
            this.SelectDown.AddDefaultBinding(InputControlType.LeftStickDown);
            this.SelectDown.AddDefaultBinding(InputControlType.DPadDown);
            this.SelectDown.AddDefaultBinding(Key.S);
            this.SelectDown.AddDefaultBinding(Key.DownArrow);
            this.ShootAction.AddDefaultBinding(InputControlType.RightBumper);
            this.DodgeRollAction.AddDefaultBinding(InputControlType.LeftBumper);
            this.DodgeRollAction.AddDefaultBinding(InputControlType.Action2);
            this.InteractAction.AddDefaultBinding(InputControlType.Action1);
            this.InteractAction.AddDefaultBinding(Key.E);
            this.ReloadAction.AddDefaultBinding(InputControlType.Action3);
            this.ReloadAction.AddDefaultBinding(Key.R);
            this.UseItemAction.AddDefaultBinding(InputControlType.RightTrigger);
            this.UseItemAction.AddDefaultBinding(Key.Space);
            this.MapAction.AddDefaultBinding(InputControlType.LeftTrigger);
            this.MapAction.AddDefaultBinding(Key.Tab);
            this.GunUpAction.AddDefaultBinding(InputControlType.DPadLeft);
            this.GunDownAction.AddDefaultBinding(InputControlType.DPadRight);
            this.DropGunAction.AddDefaultBinding(Key.F);
            this.DropGunAction.AddDefaultBinding(InputControlType.DPadDown);
            this.DropItemAction.AddDefaultBinding(Key.G);
            this.DropItemAction.AddDefaultBinding(InputControlType.DPadUp);
            this.ItemUpAction.AddDefaultBinding(InputControlType.DPadUp);
            this.ItemUpAction.AddDefaultBinding(Key.LeftShift);
            this.GunQuickEquipAction.AddDefaultBinding(InputControlType.Action4);
            this.GunQuickEquipAction.AddDefaultBinding(Key.LeftControl);
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                this.GunQuickEquipAction.AddDefaultBinding(Key.LeftCommand);
            this.SwapDualGunsAction.AddDefaultBinding(Mouse.MiddleButton);
            this.PauseAction.AddDefaultBinding(InputControlType.Start);
            this.PauseAction.AddDefaultBinding(InputControlType.Select);
            this.PauseAction.AddDefaultBinding(InputControlType.Options);
            this.PauseAction.AddDefaultBinding(Key.Escape);
            this.EquipmentMenuAction.AddDefaultBinding(InputControlType.TouchPadButton);
            this.EquipmentMenuAction.AddDefaultBinding(InputControlType.Back);
            this.EquipmentMenuAction.AddDefaultBinding(Key.I);
            this.BlankAction.AddDefaultBinding(Key.Q);
            this.ReinitializeMenuDefaults();
            this.MinimapZoomInAction.AddDefaultBinding(Key.Equals);
            this.MinimapZoomInAction.AddDefaultBinding(Key.PadPlus);
            this.MinimapZoomOutAction.AddDefaultBinding(Key.Minus);
            this.MinimapZoomOutAction.AddDefaultBinding(Key.PadMinus);
            this.GunUpAction.AddDefaultBinding(Mouse.PositiveScrollWheel);
            this.GunDownAction.AddDefaultBinding(Mouse.NegativeScrollWheel);
            this.DodgeRollAction.AddDefaultBinding(Mouse.RightButton);
            this.ShootAction.AddDefaultBinding(Mouse.LeftButton);
            this.PunchoutDodgeLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
            this.PunchoutDodgeLeft.AddDefaultBinding(InputControlType.DPadLeft);
            this.PunchoutDodgeLeft.AddDefaultBinding(Key.A);
            this.PunchoutDodgeRight.AddDefaultBinding(InputControlType.LeftStickRight);
            this.PunchoutDodgeRight.AddDefaultBinding(InputControlType.DPadRight);
            this.PunchoutDodgeRight.AddDefaultBinding(Key.D);
            this.PunchoutBlock.AddDefaultBinding(InputControlType.LeftStickUp);
            this.PunchoutBlock.AddDefaultBinding(InputControlType.DPadUp);
            this.PunchoutBlock.AddDefaultBinding(Key.W);
            this.PunchoutDuck.AddDefaultBinding(InputControlType.LeftStickDown);
            this.PunchoutDuck.AddDefaultBinding(InputControlType.DPadDown);
            this.PunchoutDuck.AddDefaultBinding(Key.S);
            this.PunchoutPunchLeft.AddDefaultBinding(InputControlType.Action1);
            this.PunchoutPunchLeft.AddDefaultBinding(InputControlType.LeftBumper);
            this.PunchoutPunchRight.AddDefaultBinding(InputControlType.Action2);
            this.PunchoutPunchRight.AddDefaultBinding(InputControlType.RightBumper);
            this.PunchoutSuper.AddDefaultBinding(InputControlType.Action3);
            this.PunchoutSuper.AddDefaultBinding(Key.Space);
            this.PunchoutPunchLeft.AddDefaultBinding(Mouse.LeftButton);
            this.PunchoutPunchRight.AddDefaultBinding(Mouse.RightButton);
        }

        public bool HighAccuracyAimMode
        {
            get => this.m_highAccuraceAimMode;
            set
            {
                if (value == this.m_highAccuraceAimMode)
                    return;
                this.SetHighAccuracy(this.AimLeft, value);
                this.SetHighAccuracy(this.AimRight, value);
                this.SetHighAccuracy(this.AimUp, value);
                this.SetHighAccuracy(this.AimDown, value);
                this.m_highAccuraceAimMode = value;
            }
        }

        private void SetHighAccuracy(PlayerAction action, bool value)
        {
            foreach (BindingSource binding in action.Bindings)
            {
                DeviceBindingSource deviceBindingSource = binding as DeviceBindingSource;
                if ((BindingSource) deviceBindingSource != (BindingSource) null)
                    deviceBindingSource.ForceRawInput = value;
            }
        }

        public enum GungeonActionType
        {
            Left,
            Right,
            Up,
            Down,
            AimLeft,
            AimRight,
            AimUp,
            AimDown,
            Shoot,
            DodgeRoll,
            Interact,
            Reload,
            UseItem,
            Map,
            CycleGunUp,
            CycleGunDown,
            CycleItemUp,
            CycleItemDown,
            Keybullet,
            Pause,
            SelectLeft,
            SelectRight,
            SelectUp,
            SelectDown,
            Cancel,
            DropGun,
            EquipmentMenu,
            Blank,
            GunQuickEquip,
            MenuInteract,
            DropItem,
            MinimapZoomIn,
            MinimapZoomOut,
            SwapDualGuns,
            PunchoutDodgeLeft,
            PunchoutDodgeRight,
            PunchoutBlock,
            PunchoutDuck,
            PunchoutPunchLeft,
            PunchoutPunchRight,
            PunchoutSuper,
        }
    }

