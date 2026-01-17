// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.ThrustmasterFerrari458RacingWheelMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

public class ThrustmasterFerrari458RacingWheelMacProfile : Xbox360DriverMacProfile
{
  public ThrustmasterFerrari458RacingWheelMacProfile()
  {
    this.Name = "Thrustmaster Ferrari 458 Racing Wheel";
    this.Meta = "Thrustmaster Ferrari 458 Racing Wheel on Mac";
    this.Matchers = new NativeInputDeviceMatcher[2]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 23296)
      },
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 9414),
        ProductID = new ushort?((ushort) 23299)
      }
    };
  }
}
