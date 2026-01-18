using UnityEngine;

#nullable disable
namespace InControl
{
    public struct KeyInfo
    {
        private readonly Key key;
        private readonly string name;
        private readonly string macName;
        private readonly KeyCode[] keyCodes;
        public static readonly KeyInfo[] KeyList = new KeyInfo[111]
        {
            new KeyInfo(Key.None, "None", new KeyCode[1]),
            new KeyInfo(Key.Shift, "Shift", new KeyCode[2]
            {
                KeyCode.LeftShift,
                KeyCode.RightShift
            }),
            new KeyInfo(Key.Alt, "Alt", "Option", new KeyCode[2]
            {
                KeyCode.LeftAlt,
                KeyCode.RightAlt
            }),
            new KeyInfo(Key.Command, "Command", new KeyCode[2]
            {
                KeyCode.LeftCommand,
                KeyCode.RightCommand
            }),
            new KeyInfo(Key.Control, "Control", new KeyCode[2]
            {
                KeyCode.LeftControl,
                KeyCode.RightControl
            }),
            new KeyInfo(Key.LeftShift, "Left Shift", new KeyCode[1]
            {
                KeyCode.LeftShift
            }),
            new KeyInfo(Key.LeftAlt, "Left Alt", "Left Option", new KeyCode[1]
            {
                KeyCode.LeftAlt
            }),
            new KeyInfo(Key.LeftCommand, "Left Command", new KeyCode[1]
            {
                KeyCode.LeftCommand
            }),
            new KeyInfo(Key.LeftControl, "Left Control", new KeyCode[1]
            {
                KeyCode.LeftControl
            }),
            new KeyInfo(Key.RightShift, "Right Shift", new KeyCode[1]
            {
                KeyCode.RightShift
            }),
            new KeyInfo(Key.RightAlt, "Right Alt", "Right Option", new KeyCode[1]
            {
                KeyCode.RightAlt
            }),
            new KeyInfo(Key.RightCommand, "Right Command", new KeyCode[1]
            {
                KeyCode.RightCommand
            }),
            new KeyInfo(Key.RightControl, "Right Control", new KeyCode[1]
            {
                KeyCode.RightControl
            }),
            new KeyInfo(Key.Escape, "Escape", new KeyCode[1]
            {
                KeyCode.Escape
            }),
            new KeyInfo(Key.F1, "F1", new KeyCode[1]{ KeyCode.F1 }),
            new KeyInfo(Key.F2, "F2", new KeyCode[1]{ KeyCode.F2 }),
            new KeyInfo(Key.F3, "F3", new KeyCode[1]{ KeyCode.F3 }),
            new KeyInfo(Key.F4, "F4", new KeyCode[1]{ KeyCode.F4 }),
            new KeyInfo(Key.F5, "F5", new KeyCode[1]{ KeyCode.F5 }),
            new KeyInfo(Key.F6, "F6", new KeyCode[1]{ KeyCode.F6 }),
            new KeyInfo(Key.F7, "F7", new KeyCode[1]{ KeyCode.F7 }),
            new KeyInfo(Key.F8, "F8", new KeyCode[1]{ KeyCode.F8 }),
            new KeyInfo(Key.F9, "F9", new KeyCode[1]{ KeyCode.F9 }),
            new KeyInfo(Key.F10, "F10", new KeyCode[1]{ KeyCode.F10 }),
            new KeyInfo(Key.F11, "F11", new KeyCode[1]{ KeyCode.F11 }),
            new KeyInfo(Key.F12, "F12", new KeyCode[1]{ KeyCode.F12 }),
            new KeyInfo(Key.Key0, "Num 0", new KeyCode[1]
            {
                KeyCode.Alpha0
            }),
            new KeyInfo(Key.Key1, "Num 1", new KeyCode[1]
            {
                KeyCode.Alpha1
            }),
            new KeyInfo(Key.Key2, "Num 2", new KeyCode[1]
            {
                KeyCode.Alpha2
            }),
            new KeyInfo(Key.Key3, "Num 3", new KeyCode[1]
            {
                KeyCode.Alpha3
            }),
            new KeyInfo(Key.Key4, "Num 4", new KeyCode[1]
            {
                KeyCode.Alpha4
            }),
            new KeyInfo(Key.Key5, "Num 5", new KeyCode[1]
            {
                KeyCode.Alpha5
            }),
            new KeyInfo(Key.Key6, "Num 6", new KeyCode[1]
            {
                KeyCode.Alpha6
            }),
            new KeyInfo(Key.Key7, "Num 7", new KeyCode[1]
            {
                KeyCode.Alpha7
            }),
            new KeyInfo(Key.Key8, "Num 8", new KeyCode[1]
            {
                KeyCode.Alpha8
            }),
            new KeyInfo(Key.Key9, "Num 9", new KeyCode[1]
            {
                KeyCode.Alpha9
            }),
            new KeyInfo(Key.A, "A", new KeyCode[1]{ KeyCode.A }),
            new KeyInfo(Key.B, "B", new KeyCode[1]{ KeyCode.B }),
            new KeyInfo(Key.C, "C", new KeyCode[1]{ KeyCode.C }),
            new KeyInfo(Key.D, "D", new KeyCode[1]{ KeyCode.D }),
            new KeyInfo(Key.E, "E", new KeyCode[1]{ KeyCode.E }),
            new KeyInfo(Key.F, "F", new KeyCode[1]{ KeyCode.F }),
            new KeyInfo(Key.G, "G", new KeyCode[1]{ KeyCode.G }),
            new KeyInfo(Key.H, "H", new KeyCode[1]{ KeyCode.H }),
            new KeyInfo(Key.I, "I", new KeyCode[1]{ KeyCode.I }),
            new KeyInfo(Key.J, "J", new KeyCode[1]{ KeyCode.J }),
            new KeyInfo(Key.K, "K", new KeyCode[1]{ KeyCode.K }),
            new KeyInfo(Key.L, "L", new KeyCode[1]{ KeyCode.L }),
            new KeyInfo(Key.M, "M", new KeyCode[1]{ KeyCode.M }),
            new KeyInfo(Key.N, "N", new KeyCode[1]{ KeyCode.N }),
            new KeyInfo(Key.O, "O", new KeyCode[1]{ KeyCode.O }),
            new KeyInfo(Key.P, "P", new KeyCode[1]{ KeyCode.P }),
            new KeyInfo(Key.Q, "Q", new KeyCode[1]{ KeyCode.Q }),
            new KeyInfo(Key.R, "R", new KeyCode[1]{ KeyCode.R }),
            new KeyInfo(Key.S, "S", new KeyCode[1]{ KeyCode.S }),
            new KeyInfo(Key.T, "T", new KeyCode[1]{ KeyCode.T }),
            new KeyInfo(Key.U, "U", new KeyCode[1]{ KeyCode.U }),
            new KeyInfo(Key.V, "V", new KeyCode[1]{ KeyCode.V }),
            new KeyInfo(Key.W, "W", new KeyCode[1]{ KeyCode.W }),
            new KeyInfo(Key.X, "X", new KeyCode[1]{ KeyCode.X }),
            new KeyInfo(Key.Y, "Y", new KeyCode[1]{ KeyCode.Y }),
            new KeyInfo(Key.Z, "Z", new KeyCode[1]{ KeyCode.Z }),
            new KeyInfo(Key.Backquote, "Backquote", new KeyCode[1]
            {
                KeyCode.BackQuote
            }),
            new KeyInfo(Key.Minus, "Minus", new KeyCode[1]
            {
                KeyCode.Minus
            }),
            new KeyInfo(Key.Equals, "Equals", new KeyCode[1]
            {
                KeyCode.Equals
            }),
            new KeyInfo(Key.Backspace, "Backspace", "Delete", new KeyCode[1]
            {
                KeyCode.Backspace
            }),
            new KeyInfo(Key.Tab, "Tab", new KeyCode[1]{ KeyCode.Tab }),
            new KeyInfo(Key.LeftBracket, "Left Bracket", new KeyCode[1]
            {
                KeyCode.LeftBracket
            }),
            new KeyInfo(Key.RightBracket, "Right Bracket", new KeyCode[1]
            {
                KeyCode.RightBracket
            }),
            new KeyInfo(Key.Backslash, "Backslash", new KeyCode[1]
            {
                KeyCode.Backslash
            }),
            new KeyInfo(Key.Semicolon, "Semicolon", new KeyCode[1]
            {
                KeyCode.Semicolon
            }),
            new KeyInfo(Key.Quote, "Quote", new KeyCode[1]
            {
                KeyCode.Quote
            }),
            new KeyInfo(Key.Return, "Return", new KeyCode[1]
            {
                KeyCode.Return
            }),
            new KeyInfo(Key.Comma, "Comma", new KeyCode[1]
            {
                KeyCode.Comma
            }),
            new KeyInfo(Key.Period, "Period", new KeyCode[1]
            {
                KeyCode.Period
            }),
            new KeyInfo(Key.Slash, "Slash", new KeyCode[1]
            {
                KeyCode.Slash
            }),
            new KeyInfo(Key.Space, "Space", new KeyCode[1]
            {
                KeyCode.Space
            }),
            new KeyInfo(Key.Insert, "Insert", new KeyCode[1]
            {
                KeyCode.Insert
            }),
            new KeyInfo(Key.Delete, "Delete", "Forward Delete", new KeyCode[1]
            {
                KeyCode.Delete
            }),
            new KeyInfo(Key.Home, "Home", new KeyCode[1]
            {
                KeyCode.Home
            }),
            new KeyInfo(Key.End, "End", new KeyCode[1]{ KeyCode.End }),
            new KeyInfo(Key.PageUp, "PageUp", new KeyCode[1]
            {
                KeyCode.PageUp
            }),
            new KeyInfo(Key.PageDown, "PageDown", new KeyCode[1]
            {
                KeyCode.PageDown
            }),
            new KeyInfo(Key.LeftArrow, "Left Arrow", new KeyCode[1]
            {
                KeyCode.LeftArrow
            }),
            new KeyInfo(Key.RightArrow, "Right Arrow", new KeyCode[1]
            {
                KeyCode.RightArrow
            }),
            new KeyInfo(Key.UpArrow, "Up Arrow", new KeyCode[1]
            {
                KeyCode.UpArrow
            }),
            new KeyInfo(Key.DownArrow, "Down Arrow", new KeyCode[1]
            {
                KeyCode.DownArrow
            }),
            new KeyInfo(Key.Pad0, "Pad 0", new KeyCode[1]
            {
                KeyCode.Keypad0
            }),
            new KeyInfo(Key.Pad1, "Pad 1", new KeyCode[1]
            {
                KeyCode.Keypad1
            }),
            new KeyInfo(Key.Pad2, "Pad 2", new KeyCode[1]
            {
                KeyCode.Keypad2
            }),
            new KeyInfo(Key.Pad3, "Pad 3", new KeyCode[1]
            {
                KeyCode.Keypad3
            }),
            new KeyInfo(Key.Pad4, "Pad 4", new KeyCode[1]
            {
                KeyCode.Keypad4
            }),
            new KeyInfo(Key.Pad5, "Pad 5", new KeyCode[1]
            {
                KeyCode.Keypad5
            }),
            new KeyInfo(Key.Pad6, "Pad 6", new KeyCode[1]
            {
                KeyCode.Keypad6
            }),
            new KeyInfo(Key.Pad7, "Pad 7", new KeyCode[1]
            {
                KeyCode.Keypad7
            }),
            new KeyInfo(Key.Pad8, "Pad 8", new KeyCode[1]
            {
                KeyCode.Keypad8
            }),
            new KeyInfo(Key.Pad9, "Pad 9", new KeyCode[1]
            {
                KeyCode.Keypad9
            }),
            new KeyInfo(Key.Numlock, "Numlock", new KeyCode[1]
            {
                KeyCode.Numlock
            }),
            new KeyInfo(Key.PadDivide, "Pad Divide", new KeyCode[1]
            {
                KeyCode.KeypadDivide
            }),
            new KeyInfo(Key.PadMultiply, "Pad Multiply", new KeyCode[1]
            {
                KeyCode.KeypadMultiply
            }),
            new KeyInfo(Key.PadMinus, "Pad Minus", new KeyCode[1]
            {
                KeyCode.KeypadMinus
            }),
            new KeyInfo(Key.PadPlus, "Pad Plus", new KeyCode[1]
            {
                KeyCode.KeypadPlus
            }),
            new KeyInfo(Key.PadEnter, "Pad Enter", new KeyCode[1]
            {
                KeyCode.KeypadEnter
            }),
            new KeyInfo(Key.PadPeriod, "Pad Period", new KeyCode[1]
            {
                KeyCode.KeypadPeriod
            }),
            new KeyInfo(Key.Clear, "Clear", new KeyCode[1]
            {
                KeyCode.Clear
            }),
            new KeyInfo(Key.PadEquals, "Pad Equals", new KeyCode[1]
            {
                KeyCode.KeypadEquals
            }),
            new KeyInfo(Key.F13, "F13", new KeyCode[1]{ KeyCode.F13 }),
            new KeyInfo(Key.F14, "F14", new KeyCode[1]{ KeyCode.F14 }),
            new KeyInfo(Key.F15, "F15", new KeyCode[1]{ KeyCode.F15 }),
            new KeyInfo(Key.AltGr, "Alt Graphic", new KeyCode[1]
            {
                KeyCode.AltGr
            }),
            new KeyInfo(Key.CapsLock, "Caps Lock", new KeyCode[1]
            {
                KeyCode.CapsLock
            })
        };

        private KeyInfo(Key key, string name, params KeyCode[] keyCodes)
        {
            this.key = key;
            this.name = name;
            this.macName = name;
            this.keyCodes = keyCodes;
        }

        private KeyInfo(Key key, string name, string macName, params KeyCode[] keyCodes)
        {
            this.key = key;
            this.name = name;
            this.macName = macName;
            this.keyCodes = keyCodes;
        }

        public bool IsPressed
        {
            get
            {
                int length = this.keyCodes.Length;
                for (int index = 0; index < length; ++index)
                {
                    if (Input.GetKey(this.keyCodes[index]))
                        return true;
                }
                return false;
            }
        }

        public string Name
        {
            get
            {
                return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ? this.macName : this.name;
            }
        }

        public Key Key => this.key;
    }
}
