#nullable disable
namespace InControl.NativeProfile
{
  public class MVCTEStickMacProfile : Xbox360DriverMacProfile
  {
    public MVCTEStickMacProfile()
    {
      this.Name = "MVC TE Stick";
      this.Meta = "MVC TE Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61497)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 46904)
        }
      };
    }
  }
}
