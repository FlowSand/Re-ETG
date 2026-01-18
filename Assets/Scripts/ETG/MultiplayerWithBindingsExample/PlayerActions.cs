using InControl;

#nullable disable
namespace MultiplayerWithBindingsExample
{
    public class PlayerActions : PlayerActionSet
    {
        public PlayerAction Green;
        public PlayerAction Red;
        public PlayerAction Blue;
        public PlayerAction Yellow;
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerTwoAxisAction Rotate;

        public PlayerActions()
        {
            this.Green = this.CreatePlayerAction(nameof (Green));
            this.Red = this.CreatePlayerAction(nameof (Red));
            this.Blue = this.CreatePlayerAction(nameof (Blue));
            this.Yellow = this.CreatePlayerAction(nameof (Yellow));
            this.Left = this.CreatePlayerAction(nameof (Left));
            this.Right = this.CreatePlayerAction(nameof (Right));
            this.Up = this.CreatePlayerAction(nameof (Up));
            this.Down = this.CreatePlayerAction(nameof (Down));
            this.Rotate = this.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
        }

        public static PlayerActions CreateWithKeyboardBindings()
        {
            PlayerActions keyboardBindings = new PlayerActions();
            keyboardBindings.Green.AddDefaultBinding(Key.A);
            keyboardBindings.Red.AddDefaultBinding(Key.S);
            keyboardBindings.Blue.AddDefaultBinding(Key.D);
            keyboardBindings.Yellow.AddDefaultBinding(Key.F);
            keyboardBindings.Up.AddDefaultBinding(Key.UpArrow);
            keyboardBindings.Down.AddDefaultBinding(Key.DownArrow);
            keyboardBindings.Left.AddDefaultBinding(Key.LeftArrow);
            keyboardBindings.Right.AddDefaultBinding(Key.RightArrow);
            return keyboardBindings;
        }

        public static PlayerActions CreateWithJoystickBindings()
        {
            PlayerActions joystickBindings = new PlayerActions();
            joystickBindings.Green.AddDefaultBinding(InputControlType.Action1);
            joystickBindings.Red.AddDefaultBinding(InputControlType.Action2);
            joystickBindings.Blue.AddDefaultBinding(InputControlType.Action3);
            joystickBindings.Yellow.AddDefaultBinding(InputControlType.Action4);
            joystickBindings.Up.AddDefaultBinding(InputControlType.LeftStickUp);
            joystickBindings.Down.AddDefaultBinding(InputControlType.LeftStickDown);
            joystickBindings.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            joystickBindings.Right.AddDefaultBinding(InputControlType.LeftStickRight);
            joystickBindings.Up.AddDefaultBinding(InputControlType.DPadUp);
            joystickBindings.Down.AddDefaultBinding(InputControlType.DPadDown);
            joystickBindings.Left.AddDefaultBinding(InputControlType.DPadLeft);
            joystickBindings.Right.AddDefaultBinding(InputControlType.DPadRight);
            return joystickBindings;
        }
    }
}
