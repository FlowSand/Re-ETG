// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.TrustPredatorJoystickMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class TrustPredatorJoystickMacProfile : Xbox360DriverMacProfile
{
  public TrustPredatorJoystickMacProfile()
  {
    this.Name = "Trust Predator Joystick";
    this.Meta = "Trust Predator Joystick on Mac";
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 2064),
        ProductID = new ushort?((ushort) 3)
      }
    };
  }
}
