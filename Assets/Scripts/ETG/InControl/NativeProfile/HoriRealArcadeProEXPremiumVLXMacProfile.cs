#nullable disable
namespace InControl.NativeProfile
{
  public class HoriRealArcadeProEXPremiumVLXMacProfile : Xbox360DriverMacProfile
  {
    public HoriRealArcadeProEXPremiumVLXMacProfile()
    {
      this.Name = "Hori Real Arcade Pro EX Premium VLX";
      this.Meta = "Hori Real Arcade Pro EX Premium VLX on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 62726)
        }
      };
    }
  }
}
