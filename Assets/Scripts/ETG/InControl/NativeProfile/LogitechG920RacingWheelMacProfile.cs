#nullable disable
namespace InControl.NativeProfile
{
  public class LogitechG920RacingWheelMacProfile : Xbox360DriverMacProfile
  {
    public LogitechG920RacingWheelMacProfile()
    {
      this.Name = "Logitech G920 Racing Wheel";
      this.Meta = "Logitech G920 Racing Wheel on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1133),
          ProductID = new ushort?((ushort) 49761)
        }
      };
    }
  }
}
