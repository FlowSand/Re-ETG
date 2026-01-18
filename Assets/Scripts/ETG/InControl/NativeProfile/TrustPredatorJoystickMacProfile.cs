#nullable disable
namespace InControl.NativeProfile
{
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
}
