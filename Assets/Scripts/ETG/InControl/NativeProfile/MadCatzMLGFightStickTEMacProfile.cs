#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzMLGFightStickTEMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzMLGFightStickTEMacProfile()
    {
      this.Name = "Mad Catz MLG Fight Stick TE";
      this.Meta = "Mad Catz MLG Fight Stick TE on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61502)
        }
      };
    }
  }
}
