#nullable disable
namespace InControl.NativeProfile
{
    public class RazerOnzaControllerMacProfile : Xbox360DriverMacProfile
    {
        public RazerOnzaControllerMacProfile()
        {
            this.Name = "Razer Onza Controller";
            this.Meta = "Razer Onza Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 64769)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5769),
                    ProductID = new ushort?((ushort) 64769)
                }
            };
        }
    }
}
