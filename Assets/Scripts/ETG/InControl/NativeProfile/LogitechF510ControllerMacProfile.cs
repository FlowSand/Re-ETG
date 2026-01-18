#nullable disable
namespace InControl.NativeProfile
{
    public class LogitechF510ControllerMacProfile : Xbox360DriverMacProfile
    {
        public LogitechF510ControllerMacProfile()
        {
            this.Name = "Logitech F510 Controller";
            this.Meta = "Logitech F510 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1133),
                    ProductID = new ushort?((ushort) 49694)
                }
            };
        }
    }
}
