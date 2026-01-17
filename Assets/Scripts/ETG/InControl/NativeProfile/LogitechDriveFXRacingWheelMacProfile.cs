// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.LogitechDriveFXRacingWheelMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile
{
  public class LogitechDriveFXRacingWheelMacProfile : Xbox360DriverMacProfile
  {
    public LogitechDriveFXRacingWheelMacProfile()
    {
      this.Name = "Logitech DriveFX Racing Wheel";
      this.Meta = "Logitech DriveFX Racing Wheel on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1133),
          ProductID = new ushort?((ushort) 51875)
        }
      };
    }
  }
}
