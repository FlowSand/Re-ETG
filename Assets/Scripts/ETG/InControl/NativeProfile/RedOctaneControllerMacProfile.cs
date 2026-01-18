#nullable disable
namespace InControl.NativeProfile
{
    public class RedOctaneControllerMacProfile : Xbox360DriverMacProfile
    {
        public RedOctaneControllerMacProfile()
        {
            this.Name = "Red Octane Controller";
            this.Meta = "Red Octane Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5168),
                    ProductID = new ushort?((ushort) 63489)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5168),
                    ProductID = new ushort?((ushort) 672)
                }
            };
        }
    }
}
