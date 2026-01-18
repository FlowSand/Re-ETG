#nullable disable
namespace InControl.NativeProfile
{
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
}
