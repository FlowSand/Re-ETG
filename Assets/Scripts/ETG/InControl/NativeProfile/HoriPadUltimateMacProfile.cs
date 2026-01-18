#nullable disable
namespace InControl.NativeProfile
{
  public class HoriPadUltimateMacProfile : Xbox360DriverMacProfile
  {
    public HoriPadUltimateMacProfile()
    {
      this.Name = "HoriPad Ultimate";
      this.Meta = "HoriPad Ultimate on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 144 /*0x90*/)
        }
      };
    }
  }
}
