#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzNeoFightStickMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzNeoFightStickMacProfile()
    {
      this.Name = "Mad Catz Neo Fight Stick";
      this.Meta = "Mad Catz Neo Fight Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61498)
        }
      };
    }
  }
}
