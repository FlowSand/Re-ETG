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
