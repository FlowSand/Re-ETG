#nullable disable
namespace InControl.NativeProfile
{
  public class RazerAtroxArcadeStickMacProfile : Xbox360DriverMacProfile
  {
    public RazerAtroxArcadeStickMacProfile()
    {
      this.Name = "Razer Atrox Arcade Stick";
      this.Meta = "Razer Atrox Arcade Stick on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 5426),
          ProductID = new ushort?((ushort) 2560 /*0x0A00*/)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 20480 /*0x5000*/)
        }
      };
    }
  }
}
