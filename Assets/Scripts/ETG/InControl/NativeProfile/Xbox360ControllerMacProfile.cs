#nullable disable
namespace InControl.NativeProfile
{
    public class Xbox360ControllerMacProfile : Xbox360DriverMacProfile
    {
        public Xbox360ControllerMacProfile()
        {
            this.Name = "Xbox 360 Controller";
            this.Meta = "Xbox 360 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3695),
                    ProductID = new ushort?((ushort) 62721)
                }
            };
        }
    }
}
