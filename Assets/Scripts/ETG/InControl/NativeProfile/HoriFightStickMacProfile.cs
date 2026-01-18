#nullable disable
namespace InControl.NativeProfile
{
  public class HoriFightStickMacProfile : Xbox360DriverMacProfile
  {
    public HoriFightStickMacProfile()
    {
      this.Name = "Hori Fight Stick";
      this.Meta = "Hori Fight Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 13)
        }
      };
    }
  }
}
