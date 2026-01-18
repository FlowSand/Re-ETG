#nullable disable
namespace InControl.NativeProfile
{
    public class RazerOnzaTEControllerMacProfile : Xbox360DriverMacProfile
    {
        public RazerOnzaTEControllerMacProfile()
        {
            this.Name = "Razer Onza TE Controller";
            this.Meta = "Razer Onza TE Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 64768)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5769),
                    ProductID = new ushort?((ushort) 64768)
                }
            };
        }
    }
}
