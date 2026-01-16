// Decompiled with JetBrains decompiler
// Type: InControl.UnityInputDeviceProfileBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

public abstract class UnityInputDeviceProfileBase : InputDeviceProfile
{
  public abstract bool IsJoystick { get; }

  public abstract bool HasJoystickName(string joystickName);

  public abstract bool HasLastResortRegex(string joystickName);

  public abstract bool HasJoystickOrRegexName(string joystickName);

  public bool IsNotJoystick => !this.IsJoystick;
}
