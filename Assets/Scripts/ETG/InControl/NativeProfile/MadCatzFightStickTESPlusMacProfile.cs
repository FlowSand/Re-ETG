#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzFightStickTESPlusMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzFightStickTESPlusMacProfile()
    {
      this.Name = "Mad Catz Fight Stick TES Plus";
      this.Meta = "Mad Catz Fight Stick TES Plus on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61506)
        }
      };
    }
  }
}
