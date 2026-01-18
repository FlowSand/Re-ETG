#nullable disable
namespace InControl.NativeProfile
{
    public class JoytekXbox360ControllerMacProfile : Xbox360DriverMacProfile
    {
        public JoytekXbox360ControllerMacProfile()
        {
            this.Name = "Joytek Xbox 360 Controller";
            this.Meta = "Joytek Xbox 360 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5678),
                    ProductID = new ushort?((ushort) 48879)
                }
            };
        }
    }
}
