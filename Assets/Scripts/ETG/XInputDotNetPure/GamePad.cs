using System;
using System.Runtime.InteropServices;

#nullable disable
namespace XInputDotNetPure
{
    public class GamePad
    {
        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (GamePadState.RawState)));
            return new GamePadState(Imports.XInputGamePadGetState((uint) playerIndex, num) == 0U, (GamePadState.RawState) Marshal.PtrToStructure(num, typeof (GamePadState.RawState)));
        }

        public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        {
            Imports.XInputGamePadSetState((uint) playerIndex, leftMotor, rightMotor);
        }
    }
}
