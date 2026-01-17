// Decompiled with JetBrains decompiler
// Type: XInputDotNetPure.GamePad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace XInputDotNetPure;

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
