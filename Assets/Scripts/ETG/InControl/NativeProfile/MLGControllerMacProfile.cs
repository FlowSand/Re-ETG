#nullable disable
namespace InControl.NativeProfile
{
    public class MLGControllerMacProfile : Xbox360DriverMacProfile
    {
        public MLGControllerMacProfile()
        {
            this.Name = "MLG Controller";
            this.Meta = "MLG Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 61475)
                }
            };
        }
    }
}
