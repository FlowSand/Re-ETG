#nullable disable
namespace InControl.NativeProfile
{
    public class LogitechF310ControllerMacProfile : Xbox360DriverMacProfile
    {
        public LogitechF310ControllerMacProfile()
        {
            this.Name = "Logitech F310 Controller";
            this.Meta = "Logitech F310 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1133),
                    ProductID = new ushort?((ushort) 49693)
                }
            };
        }
    }
}
