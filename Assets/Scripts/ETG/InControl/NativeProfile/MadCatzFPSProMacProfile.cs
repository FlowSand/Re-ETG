#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzFPSProMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzFPSProMacProfile()
    {
      this.Name = "Mad Catz FPS Pro";
      this.Meta = "Mad Catz FPS Pro on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61479)
        }
      };
    }
  }
}
