#nullable disable
namespace InControl.NativeProfile
{
  public class MicrosoftXbox360ControllerMacProfile : Xbox360DriverMacProfile
  {
    public MicrosoftXbox360ControllerMacProfile()
    {
      this.Name = "Microsoft Xbox 360 Controller";
      this.Meta = "Microsoft Xbox 360 Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[6]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 654)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 655)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 307)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 63233)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 672)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 672)
        }
      };
    }
  }
}
