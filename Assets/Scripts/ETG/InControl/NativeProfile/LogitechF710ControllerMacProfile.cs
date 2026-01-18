#nullable disable
namespace InControl.NativeProfile
{
    public class LogitechF710ControllerMacProfile : Xbox360DriverMacProfile
    {
        public LogitechF710ControllerMacProfile()
        {
            this.Name = "Logitech F710 Controller";
            this.Meta = "Logitech F710 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1133),
                    ProductID = new ushort?((ushort) 49695)
                }
            };
        }
    }
}
