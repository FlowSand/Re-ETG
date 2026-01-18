#nullable disable
namespace InControl.NativeProfile
{
    public class ProEXXbox360ControllerMacProfile : Xbox360DriverMacProfile
    {
        public ProEXXbox360ControllerMacProfile()
        {
            this.Name = "Pro EX Xbox 360 Controller";
            this.Meta = "Pro EX Xbox 360 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 21258)
                }
            };
        }
    }
}
