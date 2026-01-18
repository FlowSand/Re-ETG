#nullable disable
namespace InControl.NativeProfile
{
    public class LogitechControllerMacProfile : Xbox360DriverMacProfile
    {
        public LogitechControllerMacProfile()
        {
            this.Name = "Logitech Controller";
            this.Meta = "Logitech Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1133),
                    ProductID = new ushort?((ushort) 62209)
                }
            };
        }
    }
}
