// Decompiled with JetBrains decompiler
// Type: XInputDotNetPure.Imports
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace XInputDotNetPure
{
  internal class Imports
  {
    [DllImport("XInputInterface32", EntryPoint = "XInputGamePadGetState")]
    public static extern uint XInputGamePadGetState32(uint playerIndex, IntPtr state);

    [DllImport("XInputInterface32", EntryPoint = "XInputGamePadSetState")]
    public static extern void XInputGamePadSetState32(
      uint playerIndex,
      float leftMotor,
      float rightMotor);

    [DllImport("XInputInterface64", EntryPoint = "XInputGamePadGetState")]
    public static extern uint XInputGamePadGetState64(uint playerIndex, IntPtr state);

    [DllImport("XInputInterface64", EntryPoint = "XInputGamePadSetState")]
    public static extern void XInputGamePadSetState64(
      uint playerIndex,
      float leftMotor,
      float rightMotor);

    public static uint XInputGamePadGetState(uint playerIndex, IntPtr state)
    {
      return IntPtr.Size == 4 ? Imports.XInputGamePadGetState32(playerIndex, state) : Imports.XInputGamePadGetState64(playerIndex, state);
    }

    public static void XInputGamePadSetState(uint playerIndex, float leftMotor, float rightMotor)
    {
      if (IntPtr.Size == 4)
        Imports.XInputGamePadSetState32(playerIndex, leftMotor, rightMotor);
      else
        Imports.XInputGamePadSetState64(playerIndex, leftMotor, rightMotor);
    }
  }
}
