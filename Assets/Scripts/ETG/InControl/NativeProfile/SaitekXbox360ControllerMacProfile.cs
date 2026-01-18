#nullable disable
namespace InControl.NativeProfile
{
    public class SaitekXbox360ControllerMacProfile : Xbox360DriverMacProfile
    {
        public SaitekXbox360ControllerMacProfile()
        {
            this.Name = "Saitek Xbox 360 Controller";
            this.Meta = "Saitek Xbox 360 Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1848),
                    ProductID = new ushort?((ushort) 51970)
                }
            };
        }
    }
}
