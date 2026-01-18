#nullable disable
namespace InControl.NativeProfile
{
  public class HoriFightingStickEX2MacProfile : Xbox360DriverMacProfile
  {
    public HoriFightingStickEX2MacProfile()
    {
      this.Name = "Hori Fighting Stick EX2";
      this.Meta = "Hori Fighting Stick EX2 on Mac";
      this.Matchers = new NativeInputDeviceMatcher[3]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 10)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 62725)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 13)
        }
      };
    }
  }
}
