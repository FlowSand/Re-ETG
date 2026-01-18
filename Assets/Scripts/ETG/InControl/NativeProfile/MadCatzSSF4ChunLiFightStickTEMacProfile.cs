#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzSSF4ChunLiFightStickTEMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzSSF4ChunLiFightStickTEMacProfile()
    {
      this.Name = "Mad Catz SSF4 Chun-Li Fight Stick TE";
      this.Meta = "Mad Catz SSF4 Chun-Li Fight Stick TE on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61501)
        }
      };
    }
  }
}
