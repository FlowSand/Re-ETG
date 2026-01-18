#nullable disable
namespace InControl.NativeProfile
{
    public class PowerAMiniProExControllerMacProfile : Xbox360DriverMacProfile
    {
        public PowerAMiniProExControllerMacProfile()
        {
            this.Name = "PowerA Mini Pro Ex Controller";
            this.Meta = "PowerA Mini Pro Ex Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[3]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 5604),
                    ProductID = new ushort?((ushort) 16128)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 21274)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 9414),
                    ProductID = new ushort?((ushort) 21248)
                }
            };
        }
    }
}
