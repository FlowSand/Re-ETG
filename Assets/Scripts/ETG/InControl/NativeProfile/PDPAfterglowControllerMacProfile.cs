#nullable disable
namespace InControl.NativeProfile
{
  public class PDPAfterglowControllerMacProfile : Xbox360DriverMacProfile
  {
    public PDPAfterglowControllerMacProfile()
    {
      this.Name = "PDP Afterglow Controller";
      this.Meta = "PDP Afterglow Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[10]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 1043)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 64252)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 63751)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 64253)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 742)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 63744)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 275)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 63744)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 531)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 4779),
          ProductID = new ushort?((ushort) 769)
        }
      };
    }
  }
}
