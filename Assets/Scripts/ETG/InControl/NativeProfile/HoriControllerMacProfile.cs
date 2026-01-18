#nullable disable
namespace InControl.NativeProfile
{
    public class HoriControllerMacProfile : Xbox360DriverMacProfile
    {
        public HoriControllerMacProfile()
        {
            this.Name = "Hori Controller";
            this.Meta = "Hori Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 21760)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 654)
                }
            };
        }
    }
}
