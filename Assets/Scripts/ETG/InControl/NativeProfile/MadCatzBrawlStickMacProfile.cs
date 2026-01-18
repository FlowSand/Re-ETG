#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzBrawlStickMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzBrawlStickMacProfile()
    {
      this.Name = "Mad Catz Brawl Stick";
      this.Meta = "Mad Catz Brawl Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61465)
        }
      };
    }
  }
}
