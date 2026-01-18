#nullable disable
namespace InControl.NativeProfile
{
  public class HoriRealArcadeProHayabusaMacProfile : Xbox360DriverMacProfile
  {
    public HoriRealArcadeProHayabusaMacProfile()
    {
      this.Name = "Hori Real Arcade Pro Hayabusa";
      this.Meta = "Hori Real Arcade Pro Hayabusa on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 99)
        }
      };
    }
  }
}
