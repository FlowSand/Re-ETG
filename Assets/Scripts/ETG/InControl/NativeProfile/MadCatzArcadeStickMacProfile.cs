#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzArcadeStickMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzArcadeStickMacProfile()
    {
      this.Name = "Mad Catz Arcade Stick";
      this.Meta = "Mad Catz Arcade Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 18264)
        }
      };
    }
  }
}
