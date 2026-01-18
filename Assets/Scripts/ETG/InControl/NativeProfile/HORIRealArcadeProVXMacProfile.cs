#nullable disable
namespace InControl.NativeProfile
{
  public class HORIRealArcadeProVXMacProfile : Xbox360DriverMacProfile
  {
    public HORIRealArcadeProVXMacProfile()
    {
      this.Name = "HORI Real Arcade Pro VX";
      this.Meta = "HORI Real Arcade Pro VX on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 27)
        }
      };
    }
  }
}
